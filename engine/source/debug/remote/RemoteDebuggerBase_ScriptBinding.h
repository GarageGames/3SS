//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod( RemoteDebuggerBase, logout, bool, 3, 3,  "(password) - Log out of the the remote debugger.\n"
                                                        "@return Whether the authentication was successful or not." )
{
    return object->logout( argv[2] );
}
