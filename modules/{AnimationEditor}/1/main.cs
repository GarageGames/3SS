//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function initializeAnimationEditor(%scopeSet)
{
    //-----------------------------------------------------------------------------
    // Load scripts
    //-----------------------------------------------------------------------------
    exec("./scripts/gui.cs");    
    exec("./scripts/utility.cs");
    exec("./scripts/tagHelpers.cs");
    exec("./scripts/animationBuilder.cs");
    exec("./scripts/previewWindows/ABImageMapPreviewWindow.cs");    
    exec("./scripts/previewWindows/storyboardWindow.cs");
    exec("./scripts/previewWindows/imageMapPreviewWindow.cs");
    exec("./scripts/previewWindows/animationPreviewWindow.cs");
    exec("./scripts/previewWindows/genericPreview.cs");
    exec("./scripts/animationContentPane.cs");
    exec("./scripts/storyboardContentPane.cs");
    exec("./scripts/ab_FrameControl.cs");
    exec("./scripts/animationEventManager.cs");

    //-----------------------------------------------------------------------------
    // Load GUIs
    //-----------------------------------------------------------------------------
    %scopeSet.add( TamlRead("./gui/animBuilder.gui.taml") );
    %scopeSet.add( TamlRead("./gui/imageMapSelect.gui.taml") );
    
    //-----------------------------------------------------------------------------
    // Initialization
    //----------------------------------------------------------------------------- 
    $IMAGE_MAP_FILTER_FULL = BIT(0);
    $IMAGE_MAP_FILTER_CELL = BIT(1);
    $IMAGE_MAP_FILTER_LINK = BIT(2);
    $IMAGE_MAP_FILTER_KEY  = BIT(3);
    
    $MaxFPS = 30;
    $MinFPS = 1;

    $ABAssetNameTextEditMessageString = "Enter asset name...";
    $ABAssetLocationTextEditMessageString = "Select an asset...";
    
    if (!isObject(AnimationBuilder))
        new ScriptObject(AnimationBuilder);

    activatePackage(AnimationBuilderPackage);

    initializeAnimBuilderEventManager();
}

function destroyAnimationEditor()
{
    destroyAnimBuilderEventManager();

    deactivatePackage(AnimationBuilderPackage);
}

function launchEditAnimation(%animationId)
{
    AnimationBuilder.editAnimation(%animationId);
}