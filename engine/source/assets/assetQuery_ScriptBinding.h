//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod(AssetQuery, clear, void, 2, 2,    "() - Clears all asset Id results.\n"
                                                "@return () No return value.")
{
    object->clear();
}

//-----------------------------------------------------------------------------

ConsoleMethod(AssetQuery, set, bool, 3, 3,  "(assetQuery) Sets the asset query to a copy of the specified asset query.\n"
                                            "@param assetQuery The asset query to copy.\n"
                                            "@return Whether the operation succeeded or not." )
{
    // Find asset query.
    AssetQuery* pAssetQuery = Sim::findObject<AssetQuery>( argv[2] );

    // Did we find the asset query?
    if ( pAssetQuery == NULL )
    {
        // No, so warn.
        Con::warnf( "AssetQuery::set() - Could not find asset query '%s'.", argv[2] );
        return false;
    }

    // Set asset query.
    object->set( *pAssetQuery );

    return true;
}

//-----------------------------------------------------------------------------

ConsoleMethod(AssetQuery, getCount, S32, 2, 2,  "() - Gets the count of asset Id results.\n"
                                                "@return (int) The count of asset Id results.")
{
    return object->size();
}

//-----------------------------------------------------------------------------

ConsoleMethod(AssetQuery, getAsset, const char*, 3, 3,  "(int resultIndex) - Gets the asset Id at the specified query result index.\n"
                                                        "@param resultIndex The query result index to use.\n"
                                                        "@return (assetId) The asset Id at the specified index or NULL if not valid.")
{
    // Fetch result index.
    const S32 resultIndex = dAtoi(argv[2]);

    // Is index within bounds?
    if ( resultIndex >= object->size() )
    {
        // No, so warn.
        Con::warnf( "AssetQuery::getAsset() - Result index '%s' is out of bounds.", argv[2] );
        return StringTable->EmptyString;
    }

    return object->at(resultIndex);
}
