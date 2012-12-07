//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _REMOTE_DEBUGGER_BRIDGE_H_
#include "debug/remote/RemoteDebuggerBridge.h"
#endif

#ifndef _REMOTE_DEBUGGER_BASE_H_
#include "debug/remote/RemoteDebuggerBase.h"
#endif

#ifndef _SIM_OBJECT_PTR_H_
#include "sim/simObjectPtr.h"
#endif

#ifndef _EVENT_H_
#include "platform/event.h"
#endif

// Script bindings.
#include "debug/remote/RemoteDebuggerBridge_ScriptBinding.h"

//-----------------------------------------------------------------------------

static S32 DebuggerVersion = 0;
static S32 DebuggerPort = 0;
static StringTableEntry DebuggerPassword = NULL;
static NetSocket ServerSocket = InvalidSocket;
static NetSocket ClientSocket = InvalidSocket;
static RemoteDebuggerBridge::ConnectionState BridgeState = RemoteDebuggerBridge::Closed;

//-----------------------------------------------------------------------------

void RemoteDebuggerBridge::processCommandLine( S32 argc, const char **argv )
{
    // Fetch the remote debugger argument length.
    const S32 remoteDebuggerArgLength = dStrlen( REMOTE_DEBUGGER_COMMAND_LINE_ARG );

    // Find if the remote debugger is specified on the command-line.
    for( S32 argIndex = 0; argIndex < argc; ++argIndex )
    {
        // Skip if this is this the remote debugger argument.
        if ( dStrnicmp( argv[argIndex], REMOTE_DEBUGGER_COMMAND_LINE_ARG, remoteDebuggerArgLength ) != 0 )
            continue;

        // Are there enough arguments for opening the remote bridge?
        if ( argIndex+3 >= argc )
        {
            // No, so warn.
            Con::warnf( "Found the debugger command-line however not enough arguments were specified." );
            return;
        }

        // Fetch debugger version.
        const S32 debuggerVersion = dAtoi(argv[argIndex+1]);

        // Fetch port.
        const S32 port = dAtoi(argv[argIndex+2]);

        // Fetch password.
        const char* pPassword = argv[argIndex+3];

        // Open remote debugger with port and password.
        RemoteDebuggerBridge::open( debuggerVersion, port, pPassword );

        return;
    }
}

//-----------------------------------------------------------------------------

bool RemoteDebuggerBridge::open( const S32 debuggerVersion, const S32 port, const char* pPassword )
{
    // Sanity!
    AssertFatal( pPassword != NULL, "Debugger password cannot be NULL." );

    // Is the bridge closed?
    if ( BridgeState != RemoteDebuggerBridge::Closed )
    {
        // Yes, so warn.
        Con::warnf( "Cannot start remote debugger sessions as it is already started." );
        return false;
    }

    // Is the debugger version valid?
    if ( debuggerVersion < 1 )
    {
        // No, so warn.
        Con::warnf( "Invalid debugger version '%d'.", debuggerVersion );
        return false;
    }

    // Format debugger version.
    char debuggerClassBuffer[64];
    dSprintf( debuggerClassBuffer, sizeof(debuggerClassBuffer), "RemoteDebugger%d", debuggerVersion );

    // Find debugger.
    AbstractClassRep* pDebuggerRep = AbstractClassRep::findClassRep( debuggerClassBuffer );
    
    // Did we find the debugger?
    if ( pDebuggerRep == NULL )
    {
        // No, so warn.
        Con::warnf( "Failed to find debugger version '%d' (%s).", debuggerVersion, debuggerClassBuffer );
        return false;
    }

    // Is the password valid?
    if ( dStrlen(pPassword) == 0 )
    {
        // No, so warn.
        Con::warnf( "Debugger password cannot be empty." );
        return false;
    }

    // Create debugger.
    RemoteDebuggerBase* pRemoteDebuggerBase = dynamic_cast<RemoteDebuggerBase*>( pDebuggerRep->create() );

    // Did we create the debugger?
    if ( pRemoteDebuggerBase == NULL )
    {
        // No, so warn.
        Con::warnf( "Failed to create debugger version '%d' (%s).", debuggerVersion, debuggerClassBuffer );
        return false;
    }

    // Register the debugger.
    pRemoteDebuggerBase->registerObject( REMOTE_DEBUGGER_NAME );

    // Set debugger, its version, port and password.
    DebuggerVersion = debuggerVersion;
    DebuggerPort = port;
    DebuggerPassword = StringTable->insert( pPassword );

    // Set bridge state.
    BridgeState = Open;

    // Open the server socket.
    ServerSocket = Net::openSocket();

    // Did we get a valid server socket?
    if ( ServerSocket == InvalidSocket )
    {
        // No, so warn.
        Con::warnf( "Could not open a remote debugger server socket. " );
        return false;
    }

    // Start the server listening.
    Net::bind( ServerSocket, DebuggerPort );
    Net::listen( ServerSocket, 4 );
    Net::setBlocking( ServerSocket, false );

    // Wait for the client connection.
    WaitForClientConnection();

    // Set debugger client socket.
    pRemoteDebuggerBase->mClientSocket = ClientSocket;

    return true;
}

//-----------------------------------------------------------------------------

bool RemoteDebuggerBridge::close( void )
{
    return false;
}

//-----------------------------------------------------------------------------

RemoteDebuggerBridge::ConnectionState RemoteDebuggerBridge::getConnectionState( void )
{
    return BridgeState;
}

//-----------------------------------------------------------------------------

StringTableEntry RemoteDebuggerBridge::getConnectionPassword( void )
{
    return DebuggerPassword;
}

//-----------------------------------------------------------------------------

void RemoteDebuggerBridge::WaitForClientConnection( void )
{
    // Sanity!
    AssertFatal( BridgeState == Open, "Invalid bridge state waiting for connection." );

    // Wait for connection.
    while( BridgeState == Open )
    {
        // Wait a while.
        Platform::sleep( 100 );

        NetAddress address;
        NetSocket socket = Net::accept( ServerSocket, &address );

        // Skip if we don't have a valid socket.
        if ( socket == InvalidSocket )
            continue;

        // Info.
        Con::printf( "Client connected to remote debugger (port '%d') at %d.%d.%d.%d (port %d).",
            DebuggerPort,
            address.netNum[0], address.netNum[1], address.netNum[2], address.netNum[3], 
            address.port );

        // Set client socket.
        ClientSocket = socket;

        // Set non-blocking socket.
        Net::setBlocking( ClientSocket, false );

        // Set bridge state.
        BridgeState = Connected;
    }
}
