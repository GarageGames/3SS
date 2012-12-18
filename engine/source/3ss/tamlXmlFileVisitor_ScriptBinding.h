//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod(TamlXmlFileVisitor, containsValue, bool, 5, 5,   "(filename, field, value) - Finds the specified field if it contains the specified value.\n"
                                        "@param filename The file to search in.\n"
                                        "@param field The field to look for - such as internalName.\n"
                                        "@param value The value to find - such as New Projectile.\n"
                                        "@return Whether the write was successful or not.")
{
    // Fetch format mode.
    const char* pFilename = argv[2];
	const char* pFieldName = argv[3];
	const char* pFieldValue = argv[4];

	object->clearFound();
    return object->findElementByAttribValue( pFilename, pFieldName, pFieldValue );
}
