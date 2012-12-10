//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod( AssetBase, refreshAsset, void, 2, 2,        "() Refresh the asset.\n"
                                                                "@return No return value.")
{
    object->refreshAsset();
}

//-----------------------------------------------------------------------------

ConsoleMethod( AssetBase, reloadAsset, void, 2, 2,        "() Reload the asset data from the file.\n"
                                                                "@return No return value.")
{
    object->reloadAsset();
}


//-----------------------------------------------------------------------------

ConsoleMethod( AssetBase, getAssetId, const char*, 2, 2,   "() - Gets the assets' Asset Id.  This is only available if the asset was acquired from the asset manager.\n"
                                                                "@return The assets' Asset Id.")
{
    return object->getAssetId();
}