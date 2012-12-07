//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function initializeAssetLibrary(%scopeSet)
{
    //-----------------------------------------------------------------------------
    // Load profiles
    //-----------------------------------------------------------------------------
    %profilePattern = expandPath("./gui/profiles/*.taml");
    %file = findFirstFile(%profilePattern);
    
    while(%file !$= "")
    {
        TamlRead(%file);
        %file = findNextFile(%profilePattern);
    }
    
    //-----------------------------------------------------------------------------
    // Execute scripts
    //-----------------------------------------------------------------------------
    exec("./scripts/gui.cs");
    exec("./scripts/assetLibraryDialog.cs");
    
    //-----------------------------------------------------------------------------
    // Load GUIs
    //----------------------------------------------------------------------------- 
    %scopeSet.add( TamlRead("./gui/AssetLibrary.gui.taml") );
    %scopeSet.add( TamlRead("./gui/AssetLibraryDialog.gui.taml") );

    //-----------------------------------------------------------------------------
    // Initialization
    //----------------------------------------------------------------------------- 
    $AssetLibrary::SpriteSheetPage = 0;
    $AssetLibrary::AnimatedSpritePage = 1;
    $AssetLibrary::SoundsPage = 2;
    $AssetLibrary::GuisPage = 3;
    $AssetLibrary::AssetAutoTag = "";
    $RestoreAssetLibraryBehindToolOnClose = false;
    $RestoreAssetLibraryBehind2ndToolOnClose = false;
    
    if (!isObject(AssetLibrary))
    {
        %scopeSet.add( new ScriptObject(AssetLibrary) );
        AssetLibrary.callbackTools = new SimSet();
    }
}

function destroyAssetLibrary()
{
}

function AssetLibrary::onAdd(%this)
{
    %this.currentPage = $AssetLibrary::SpriteSheetPage;
    %this.lockPage = false;
}

function AssetLibrary::onRemove(%this)
{
    %this.destroy();
}

package AssetLibraryPackage
{

function AnimationAsset::getFrameCount(%this)
{
    return getWordCount(%this.animationFrames);
}
};