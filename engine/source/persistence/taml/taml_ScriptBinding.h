//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod(Taml, setFormat, void, 3, 3,  "(format) - Sets the format that Taml should use to read/write.\n"
                                            "@param format The format to use: 'xml' or 'binary'.\n"
                                            "@return No return value.")
{
    // Fetch format mode.
    const Taml::TamlFormatMode formatMode = getFormatModeEnum(argv[2]);

    // Was the format valid?
    if ( formatMode == Taml::InvalidFormat )
    {
        // No, so warn.
        Con::warnf( "Taml::setFormat() - Invalid format mode used: '%s'.", argv[2] );
        return;
    }

    // Set format mode.
    object->setFormatMode( formatMode );
}

//-----------------------------------------------------------------------------

ConsoleMethod(Taml, getFormat, const char*, 2, 2,   "() - Gets the format that Taml should use to read/write.\n"
                                                    "@return The format that Taml should use to read/write.")
{
    // Fetch format mode.
    return getFormatModeDescription( object->getFormatMode() );
}

//-----------------------------------------------------------------------------

ConsoleMethod(Taml, setCompressed, void, 3, 3,    "(compressed) - Sets whether ZIP compression is used on binary formatting or not.\n"
                                                "@param compressed Whether compression is on or off.\n"
                                                "@return No return value.")
{
    // Set compression.
    object->setCompressed( dAtob(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(Taml, getCompressed, bool, 2, 2,  "() - Gets whether ZIP compression is used on binary formatting or not.\n"
                                                "@return Whether ZIP compression is used on binary formatting or not.")
{
    // Fetch compression.
    return object->getCompressed();
}

//-----------------------------------------------------------------------------

ConsoleMethod(Taml, write, bool, 4, 4,  "(object, filename) - Writes an object to a file using Taml.\n"
                                        "@param object The object to write.\n"
                                        "@param filename The filename to write to.\n"
                                        "@return Whether the write was successful or not.")
{
    // Fetch filename.
    const char* pFilename = argv[3];

    // Find object.
    SimObject* pSimObject = Sim::findObject( argv[2] );

    // Did we find the object?
    if ( pSimObject == NULL )
    {
        // No, so warn.
        Con::warnf( "Taml::write() - Could not find object '%s' to write to file '%s'.", argv[2], pFilename );
        return false;
    }

    return object->write( pSimObject, pFilename );
}

//-----------------------------------------------------------------------------

ConsoleMethod(Taml, read, const char*, 3, 3,    "(filename) - Read an object from a file using Taml.\n"
                                                "@param filename The filename to read from.\n"
                                                "@return (Object) The object read from the file or an empty string if read failed.")
{
    // Fetch filename.
    const char* pFilename = argv[2];

    // Read object.
    SimObject* pSimObject = object->read( pFilename );

    // Did we find the object?
    if ( pSimObject == NULL )
    {
        // No, so warn.
        Con::warnf( "Taml::read() - Could not read object from file '%s'.", pFilename );
        return StringTable->EmptyString;
    }

    return pSimObject->getIdString();
}

//-----------------------------------------------------------------------------

ConsoleFunction(TamlWrite, bool, 3, 5,  "(object, filename, [format], [compressed]) - Writes an object to a file using Taml.\n"
                                        "@param object The object to write.\n"
                                        "@param filename The filename to write to.\n"
                                        "@param format The file format to use.  Optional: Defaults to 'xml'.  Can be set to 'binary'.\n"
                                        "@param compressed Whether ZIP compression is used on binary formatting or not.  Optional: Defaults to 'true'.\n"
                                        "@return Whether the write was successful or not.")
{
    // Fetch filename.
    const char* pFilename = argv[2];

    // Find object.
    SimObject* pSimObject = Sim::findObject( argv[1] );

    // Did we find the object?
    if ( pSimObject == NULL )
    {
        // No, so warn.
        Con::warnf( "Taml::write() - Could not find object '%s' to write to file '%s'.", argv[2], pFilename );
        return false;
    }

    // Set the format mode.
    Taml taml;
    taml.setFormatMode( argc > 3 ? getFormatModeEnum( argv[3] ) : Taml::XmlFormat );  

    // Set compression.
    taml.setCompressed( argc > 4 ? dAtob(argv[4]) : true );

    // Write.
    return taml.write( pSimObject, pFilename );
}

//-----------------------------------------------------------------------------

ConsoleFunction(TamlRead, const char*, 2, 4,    "(filename, [format]) - Read an object from a file using Taml.\n"
                                                "@param filename The filename to read from.\n"
                                                "@param format The file format to use.  Optional: Defaults to 'xml'.  Can be set to 'binary'.\n"
                                                "@return (Object) The object read from the file or an empty string if read failed.")
{
    // Fetch filename.
    const char* pFilename = argv[1];

    // Set the format mode.
    Taml taml;
    taml.setFormatMode( argc > 2 ? getFormatModeEnum( argv[2] ) : Taml::XmlFormat );  

    // Read object.
    SimObject* pSimObject = taml.read( pFilename );

    // Did we find the object?
    if ( pSimObject == NULL )
    {
        // No, so warn.
        Con::warnf( "Taml::read() - Could not read object from file '%s'.", pFilename );
        return StringTable->EmptyString;
    }

    return pSimObject->getIdString();
}