//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------


ConsoleMethod( RemoteDebugger1, getCodeFiles, const char*, 2, 2,    "() - Get the count of active code files.\n"
                                                                    "@return A count of the active count files." )
{
    // Fetch a return buffer.  This may be excessive but it avoids reallocation code.
    S32 bufferSize = 1024 * 65;
    char* pBuffer = Con::getReturnBuffer( bufferSize );

    // Get the code files.
    if ( !object->getCodeFiles( pBuffer, bufferSize ) )
    {
        // Warn.
        Con::warnf( "Fetching code files resulted in a buffer overflow." );
        return NULL;
    }

    return pBuffer;
}


//-----------------------------------------------------------------------------

ConsoleMethod( RemoteDebugger1, setNextStatementBreak, void, 3, 3,  "(bool enabled) - Set whether to break on next statement or not.\n"
                                                                    "@return No return value." )
{
    // Fetch enabled flag.
    const bool enabled = dAtob(argv[2]);

    object->setNextStatementBreak( enabled );
}

