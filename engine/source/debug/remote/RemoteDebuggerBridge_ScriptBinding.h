//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleFunction( OpenRemoteDebugger, bool, 4, 4,    "( int debuggerVersion, int port, string password ) - Open the remote debugger.\n"
                                                    "@param debuggerVersion The debugger version required.\n"
                                                    "@param port The port the remote debugger should be listening for a debugging session on.\n"
                                                    "@param password The optional password the remote debugger should use for a debugging session authentication.\n"
                                                    "@return Whether the remote debugger was opened or not." )
{
    // Fetch debugger version.
    const S32 debuggerVersion = dAtoi(argv[1]);

    // Fetch port.
    const S32 port = dAtoi(argv[2]);

    // Fetch password.
    const char* pPassword = argv[3];

    // Open remote debugger with port and password.
    return RemoteDebuggerBridge::open( debuggerVersion, port, pPassword );
}
