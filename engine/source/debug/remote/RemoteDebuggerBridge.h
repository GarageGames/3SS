//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _REMOTE_DEBUGGER_BRIDGE_H_
#define _REMOTE_DEBUGGER_BRIDGE_H_

#ifndef _PLATFORM_H_
#include "platform/platform.h"
#endif

//-----------------------------------------------------------------------------

#define REMOTE_DEBUGGER_COMMAND_LINE_ARG    "-RemoteDebugger"

//-----------------------------------------------------------------------------

class RemoteDebuggerBase;

//-----------------------------------------------------------------------------

class RemoteDebuggerBridge
{
public:
    enum ConnectionState
    {
        // Bridge is closed.
        Closed,

        // Bridge is open.
        Open,

        // Bridge is connection.
        Connected,

        // Bridge-session is logged off.
        LoggedOff,

        // Bridge-session is logged on.
        LoggedOn
    };

public:
    RemoteDebuggerBridge() {}
    virtual ~RemoteDebuggerBridge() {}

    /// Direct bridge control.
    static void processCommandLine( S32 argc, const char **argv );
    static bool open( const S32 debuggerVersion, const S32 port, const char* pPassword );
    static bool close( void );
    static ConnectionState getConnectionState( void );
    static StringTableEntry getConnectionPassword( void );
    
private:
    static void WaitForClientConnection( void );
};

#endif // _REMOTE_DEBUGGER_BRIDGE_H_
