//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod( AssetTagsManifest, createTag, void, 3, 3,    "(tagName) Creates an asset tag.\n"
                                                            "@param tagName The tag name to create.\n"
                                                            "@return No return value." )
{
    object->createTag( argv[2] );
}

//-----------------------------------------------------------------------------

ConsoleMethod( AssetTagsManifest, renameTag, bool, 4, 4,    "(oldTagName, newTagName) Renames an existing asset tag.\n"
                                                            "@param tagName The tag name to rename.\n"
                                                            "@param newTagName The new tag name to assign.\n"
                                                            "@return Whether the asset tag was renamed or not." )
{
    return object->renameTag( argv[2], argv[3] );
}

//-----------------------------------------------------------------------------

ConsoleMethod( AssetTagsManifest, deleteTag, bool, 3, 3,    "(tagName) Deletes an asset tag.\n"
                                                            "@param tagName The tag name to delete.\n"
                                                            "@return Whether the asset tag was deleted or not." )
{
    return object->deleteTag( argv[2] );
}

//-----------------------------------------------------------------------------

ConsoleMethod( AssetTagsManifest, isTag, bool, 3, 3,        "(tagName) Checks whether the specified asset tag exists or not.\n"
                                                            "@param tagName The tag name to check.\n"
                                                            "@return Whether the specified asset tag exists or not." )
{
    return object->isTag( argv[2] );
}

//-----------------------------------------------------------------------------

ConsoleMethod( AssetTagsManifest, getTagCount, S32, 2, 2,   "() Gets the total asset tag count.\n"
                                                            "@return The total asset tag count.")
{
    return object->getTagCount();
}

//-----------------------------------------------------------------------------

ConsoleMethod( AssetTagsManifest, getTag, const char*, 3, 3,    "(int tagIndex) Gets the asset tag at the specified index.\n"
                                                                "@param tagIndex The asset tag index.  This must be 0 to the asset tag count less one.\n"
                                                                "@return The asset tag at the specified index or NULL if invalid.")
{
    // Fetch tag index.
    const U32 tagIndex = dAtoi(argv[2]);

    // Is the tag index out-of-bounds?
    if ( tagIndex >= object->getTagCount() )
    {
        // Yes, so warn.
        Con::warnf( "AssetTagsManifest: Asset tag index '%d' is out of bounds.  Asset tag count is '%d'", tagIndex, object->getTagCount() );
        return StringTable->EmptyString;
    }

    return object->getTag( tagIndex );
}

//-----------------------------------------------------------------------------

ConsoleMethod( AssetTagsManifest, getAssetTagCount, S32, 3, 3,  "(assetId) Gets the asset tag count on the specified asset Id.\n"
                                                                "@param assetId The asset Id to count tags on.\n"
                                                                "@return The asset tag count on the specified asset Id.")
{
    return object->getAssetTagCount( argv[2] );
}

//-----------------------------------------------------------------------------

ConsoleMethod( AssetTagsManifest, getAssetTag, const char*, 4, 4,   "(assetId, int tagIndex) Gets the asset tag on the specified asset Id at the specified index.\n"
                                                                    "@param assetId The asset Id to count tags on.\n"
                                                                    "@param tagIndex The asset tag index.  This must be 0 to the asset tag count less one.\n"
                                                                    "@return The asset tag on the specified asset Id at the specified index or NULL if invalid.")
{
    // Fetch asset Id.
    const char* pAssetId = argv[2];

    // Fetch tag index.
    const U32 tagIndex = dAtoi(argv[3]);

    // Is the tag index out-of-bounds?
    if ( tagIndex >= object->getAssetTagCount( pAssetId ) )
    {
        // Yes, so warn.
        Con::warnf( "AssetTagsManifest: Asset tag index '%d' is out of bounds.  Asset tag count is '%d'", tagIndex, object->getAssetTagCount( pAssetId ) );
        return StringTable->EmptyString;
    }

    return object->getAssetTag( pAssetId, tagIndex );
}

//-----------------------------------------------------------------------------

ConsoleMethod( AssetTagsManifest, tag, bool, 4, 4,          "(assetId, tagName) Tags the asset Id with the specified asset tag.\n"
                                                            "@param assetId The asset Id to tag.\n"
                                                            "@param tagName The tag name to assign.\n"
                                                            "@return Whether the tag operation was successful or not." )
{
    return object->tag( argv[2], argv[3] );
}

//-----------------------------------------------------------------------------

ConsoleMethod( AssetTagsManifest, untag, bool, 4, 4,        "(assetId, tagName) Un-tags the asset Id from the specified asset tag.\n"
                                                            "@param assetId The asset Id to un-tag.\n"
                                                            "@param tagName The tag name to un-assign.\n"
                                                            "@return Whether the un-tag operation was successful or not." )
{
    return object->untag( argv[2], argv[3] );
}

//-----------------------------------------------------------------------------

ConsoleMethod( AssetTagsManifest, hasTag, bool, 4, 4,       "(assetId, tagName) Checks whether the asset Id is tagged with the specified asset tag.\n"
                                                            "@param assetId The asset Id to check.\n"
                                                            "@param tagName The tag name to check.\n"
                                                            "@return Whether the asset Id is tagged with the specified asset tag or not." )
{
    return object->hasTag( argv[2], argv[3] );
}