//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// Call this function to scan a folder for images or sound files and create 
/// asset files for them.
/// </summary>
/// <param name="path">The path to scan - "^{EditorAssets}/data/images/".</param>
/// <param name="type">The asset type - "image" or "sound".</param>
/// <param name="category">The asset category.  If none, leave this blank.</param>
/// <param name="internal">Flag the asset as an internal asset.</param>
function makeAssetFiles(%path, %type, %category, %internal)
{
    switch$(%type)
    {
        case "image":
            %imageFile = findFirstFile(expandPath(%path @ "*.png"));
            while (%imageFile !$= "")
            {
                createAsset(%imageFile, %path, %type, %category, %internal);
                %imagefile = findNextFile(expandPath(%path @ "*.png"));
            }

        case "sound":
            %audioFile = findFirstFile(expandPath(%path @ "*.wav"));
            while (%audioFile !$= "")
            {
                createAsset(%audioFile, %path, %type, %category, %internal);
                %audioFile = findNextFile(expandPath(%path @ "*.wav"));
            }
    }
}


/// <summary>
/// This function creates an individual asset file for a particular image or sound.
/// </summary>
/// <param name="file">The file to create the asset for.</param>
/// <param name="path">The path to the file - "^{EditorAssets}/data/images/"</param>
/// <param name="type">The asset type - "image" or "sound"</param>
/// <param name="category">The asset category.  If none, leave this blank</param>
function createAsset(%file, %path, %type, %category, %internal)
{
    switch$(%type)
    {
        case "image":
            %datablock = new ImageAsset();
            %datablock.AssetName = fileBase(%file);
            %datablock.AssetInternal = %internal;
            %datablock.AutoUnload = "0";
            %datablock.ImageFile = %file;
            %datablock.AssetCategory = %category;
            
        case "sound":
            %datablock = new AudioAsset();
            %datablock.AssetName = fileBase(%file);
            %datablock.AssetInternal = %internal;
            %datablock.AutoUnload = "0";
            %datablock.AudioFile = %file;
            %datablock.AssetCategory = %category;
            
    }
    %assetFileName = expandPath(%path @ fileBase(%file) @ ".asset.taml");

    TamlWrite(%datablock, %assetFileName);
}