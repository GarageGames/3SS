//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _REMOTE_DEBUGGER_BASE_H_
#include "debug/remote/RemoteDebuggerBase.h"
#endif

#ifndef _REMOTE_DEBUGGER_BRIDGE_H_
#include "debug/remote/RemoteDebuggerBridge.h"
#endif

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

#ifndef _CODEBLOCK_H_
#include "console/codeBlock.h"
#endif

// Script bindings.
#include "debug/remote/RemoteDebuggerBase_ScriptBinding.h"

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(RemoteDebuggerBase);

//-----------------------------------------------------------------------------

RemoteDebuggerBase::RemoteDebuggerBase() :
    mClientSocket( InvalidSocket ),
    mClientAuthenticated( false ),
    mReceiveCommandCursor( 0 ),
    mpSendResponseBuffer( NULL ),
    mSendResponseBufferSize( 0 )
{
    // Turn-on tick processing.
    setProcessTicks( true );
}

//-----------------------------------------------------------------------------

RemoteDebuggerBase::~RemoteDebuggerBase()
{
    // Delete the send response buffer.
    if ( mpSendResponseBuffer != NULL )
        delete [] mpSendResponseBuffer;
}

//-----------------------------------------------------------------------------

bool RemoteDebuggerBase::login( const char* pPassword )
{
    // Set client authentication.
    mClientAuthenticated = ( dStrcmp( RemoteDebuggerBridge::getConnectionPassword(), pPassword ) == 0 );

    // Perform callback if the client is authenticated.
    if ( mClientAuthenticated )
        onClientLogin();

    return mClientAuthenticated;
}

//-----------------------------------------------------------------------------

bool RemoteDebuggerBase::logout( const char* pPassword )
{
    // Finish if client is authenticated.
    if ( !mClientAuthenticated )
        return true;

    // Finish if not authenticated.
    if ( dStrcmp( RemoteDebuggerBridge::getConnectionPassword(), pPassword ) != 0 )
        return false;

    // Logged out.
    mClientAuthenticated = false;

    // Perform callback.
    onClientLogout();

    return true;
}

//-----------------------------------------------------------------------------

bool RemoteDebuggerBase::addCodeBlock( CodeBlock* pCodeBlock )
{
    // Finish if client it not authenticated.
    if ( !isClientAuthenticated() )
        return false;

    return true;
}

//-----------------------------------------------------------------------------

bool RemoteDebuggerBase::removeCodeBlock( CodeBlock* pCodeBlock )
{
    // Finish if client it not authenticated.
    if ( !isClientAuthenticated() )
        return false;

    return true;
}

//-----------------------------------------------------------------------------

bool RemoteDebuggerBase::pushStackFrame( void )
{
    // Finish if client it not authenticated.
    if ( !isClientAuthenticated() )
        return false;

    return true;
}

//-----------------------------------------------------------------------------

bool RemoteDebuggerBase::popStackFrame( void )
{
    // Finish if client it not authenticated.
    if ( !isClientAuthenticated() )
        return false;

    return true;
}

//-----------------------------------------------------------------------------

bool RemoteDebuggerBase::executionStopped( CodeBlock *code, U32 lineNumber )
{
    // Finish if client it not authenticated.
    if ( !isClientAuthenticated() )
        return false;

    return true;
}

//-----------------------------------------------------------------------------

RemoteDebuggerBase* RemoteDebuggerBase::getRemoteDebugger( void )
{
    return Sim::findObject<RemoteDebuggerBase>( REMOTE_DEBUGGER_NAME );
}

//-----------------------------------------------------------------------------

void RemoteDebuggerBase::processTick( void )
{
    // Finish if the client socket is invalid.
    if ( mClientSocket == InvalidSocket )
        return;
    
    // Calculate read point.
    char* pReadPoint = (mReceiveCommandBuffer + mReceiveCommandCursor);

    // Read from the socket.
    S32 readCount;
    Net::Error readStatus = Net::recv(mClientSocket, (U8*)pReadPoint, sizeof(mReceiveCommandBuffer)-mReceiveCommandCursor, &readCount);

    // Is the connection invalid?
    if ( readCount == 0 || (readStatus != Net::NoError && readStatus != Net::WouldBlock) )
    {
        // Yes, so terminate it.
        setProcessTicks(false);
        RemoteDebuggerBridge::close();
        return;
    }

    // Finish if the read would've blocked.
    if ( readStatus == Net::WouldBlock )
        return;

    // Process last read segment.
    for( S32 index = 0; index < readCount; ++index )
    {
        // Fetch character.
        const char character = pReadPoint[index];

        // Skip if this is not a command termination character.
        if ( character != '\r' && character != '\n' )
            continue;

        // Yes, so terminate command.
        pReadPoint[index] = 0;

        // Receive the command.
        receiveCommand( mReceiveCommandBuffer );

        // Search for any trailing characters after the command.
        for ( S32 trailIndex = index+1; trailIndex < readCount; ++trailIndex )
        {
            // Fetch trail character.
            const char trailCharacter = pReadPoint[trailIndex];

            // Skip if this is a command termination character.
            if ( trailCharacter == '\r' || trailCharacter == '\n' )
                continue;

            // Calculate remaining command characters.
            mReceiveCommandCursor = readCount-trailIndex;

            // Move the trailing characters to the start of the command buffer.
            dMemmove( mReceiveCommandBuffer, pReadPoint+trailIndex, mReceiveCommandCursor );

            // Finish!
            return;
        }

        // Reset receive command cursor.
        mReceiveCommandCursor = 0;

        // Finish!
        return;
    }

    // Move receive cursor.
    mReceiveCommandCursor += readCount;

    // Is the receive buffer full?
    if ( mReceiveCommandCursor >= sizeof(mReceiveCommandBuffer) )
    {
        // Yes, so warn.
        Con::warnf( "%s - Command receive buffer is full!  Resetting buffer.", getClassName() );
        mReceiveCommandCursor = 0;
    }
}

//-----------------------------------------------------------------------------

void RemoteDebuggerBase::receiveCommand( const char* pCommand )
{
    // Sanity!
    AssertFatal( pCommand != NULL, "Remote debugger command cannot be NULL." );

    // Finish if no command available.
    if ( dStrlen(pCommand) == 0 )
        return;

    // Is the client authenticated?
    if ( mClientAuthenticated )
    {
        // Yes, so evaluate the command.
        const char* pReturnValue = Con::evaluatef( pCommand );

        // Send the return value if it exists.
        if ( dStrlen(pReturnValue) > 0 )
            sendCommand( pReturnValue );
        return;
    }

    // Attempt authentication with the received command.
    sendCommand( login( pCommand ) ? "1" : "0" );
}

//-----------------------------------------------------------------------------

bool RemoteDebuggerBase::sendCommand( const char* pCommand )
{
    // Is the client socket valid?
    if ( mClientSocket == InvalidSocket )
    {
        // No, so warn.
        Con::warnf( "Cannot send command with invalid client socket." );
        return false;
    }

    // Fetch command length.
    const S32 commandLength = dStrlen(pCommand);

    // Is the command empty?
    if ( commandLength == 0 )
    {
        // Yes, so warn.
        Con::warnf( "Cannot send an empty command." );
        return false;
    }

    // Calculate required send response size.
    // This size is the original command response plus termination null plus an extra for the newline command termination.
    const S32 requiredSendResponseSize = commandLength+2;

    // Do we need to create a new response buffer?
    if ( requiredSendResponseSize > mSendResponseBufferSize )
    {
        // Yes, so delete any existing buffer.
        if ( mpSendResponseBuffer != NULL )
            delete [] mpSendResponseBuffer;

        // Generate a new one.
        mSendResponseBufferSize = requiredSendResponseSize;
        mpSendResponseBuffer = new char[mSendResponseBufferSize];
    }

    // Append carriage-return to send command.
    dSprintf( mpSendResponseBuffer, mSendResponseBufferSize, "%s\n", pCommand );

    // Send the command.
    Net::send( mClientSocket, (const U8*)mpSendResponseBuffer, dStrlen(mpSendResponseBuffer) );

    return true;
}

