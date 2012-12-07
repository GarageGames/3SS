//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _REMOTE_DEBUGGER_BASE_H_
#define _REMOTE_DEBUGGER_BASE_H_

#ifndef _PLATFORM_H_
#include "platform/platform.h"
#endif

#ifndef _VECTOR_H_
#include "collection/vector.h"
#endif

#ifndef _SIM_OBJECT_H_
#include "sim/simObject.h"
#endif

#ifndef _TICKABLE_H_
#include "platform/Tickable.h"
#endif

//-----------------------------------------------------------------------------

#define REMOTE_DEBUGGER_MAX_COMMAND_SIZE    4096
#define REMOTE_DEBUGGER_NAME                "NucleusDebugger"

//-----------------------------------------------------------------------------

class CodeBlock;

//-----------------------------------------------------------------------------

class RemoteDebuggerBase : public SimObject, public virtual Tickable
{
    friend class RemoteDebuggerBridge;

private:
    typedef SimObject Parent;
    NetSocket mClientSocket;
    bool mClientAuthenticated;
    char mReceiveCommandBuffer[REMOTE_DEBUGGER_MAX_COMMAND_SIZE];
    char* mpSendResponseBuffer;
    S32 mSendResponseBufferSize;
    U32 mReceiveCommandCursor;

public:
    RemoteDebuggerBase();
    virtual ~RemoteDebuggerBase();

    bool login( const char* pPassword );
    bool logout( const char* pPassword );
    inline bool isClientAuthenticated( void ) { return mClientAuthenticated; }

    /// Virtual functionality.
    virtual bool addCodeBlock( CodeBlock* pCodeBlock );
    virtual bool removeCodeBlock( CodeBlock* pCodeBlock );
    virtual bool pushStackFrame( void );
    virtual bool popStackFrame( void );
    virtual bool executionStopped( CodeBlock *code, U32 lineNumber );

    /// Fetch remote debugger.
    static RemoteDebuggerBase* getRemoteDebugger( void );

    DECLARE_CONOBJECT(RemoteDebuggerBase);

protected:
    virtual void processTick( void );
    virtual void interpolateTick( F32 delta ) {}
    virtual void advanceTime( F32 timeDelta ) {}

    virtual void onClientLogin( void ) {}
    virtual void onClientLogout( void ) {}

private:
    void receiveCommand( const char* pCommand );
    bool sendCommand( const char* pCommand );
};

#endif // _REMOTE_DEBUGGER_BASE_H_
