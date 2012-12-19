//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// This module is reponsible setting variables and calling functions that
// will be used during the life of the entire editor. Examples include DSO
// preferences, reacting to specific OS requirements, etc.
function initializePhysicsLauncherTools()
{
    // Load Asset Tools Group
    ModuleDatabase.loadGroup(assetTools);
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
    exec("./scripts/PhysicsEventManager.ed.cs");
    exec("./scripts/templateToolbar.ed.cs");
    exec("./scripts/physicsLauncherTools.ed.cs");
    exec("./scripts/builders/tutorialDataBuilder.ed.cs");
    exec("./scripts/LevelBuilderTool/PassiveViewInterfaces/levelBuilderToolLeftPassiveViewInterface.ed.cs");
    exec("./scripts/LevelBuilderTool/PassiveViews/levelBuilderToolLeftPassiveView.ed.cs");
    exec("./scripts/LevelBuilderTool/PassiveViewInterfaces/levelBuilderToolRightPassiveViewInterface.ed.cs");
    exec("./scripts/LevelBuilderTool/PassiveViews/levelBuilderToolRightPassiveView.ed.cs");
    exec("./scripts/LevelBuilderTool/Presenter/levelBuilderToolPresenter.ed.cs");
    exec("./scripts/builders/worldObjectBuilder.ed.cs"); 
    exec("./scripts/builders/projectileBuilder.ed.cs");  
    exec("./scripts/builders/slingshotLauncherBuilder.ed.cs");  
    exec("./scripts/LauncherTool/launcherTool.ed.cs");
    exec("./scripts/LauncherTool/launcherPreview.ed.cs");
    exec("./scripts/ProjectileTool/projectileTool.ed.cs");
    exec("./scripts/ProjectileTool/projectileToolForm.ed.cs");
    exec("./scripts/ProjectileTool/ptConfirmDeleteGui.ed.cs");
    exec("./scripts/WorldObjectTool/worldObjectTool.ed.cs");
    exec("./scripts/WorldTool/WorldTool.ed.cs");
    exec("./scripts/WorldTool/worldToolAssignLevel.ed.cs");
    exec("./scripts/WorldTool/worldToolConfirmWorldDelete.ed.cs");
    exec("./scripts/WorldTool/worldToolConfirmLevelDelete.ed.cs");
    exec("./scripts/InterfaceTool/interfaceTool.ed.cs");
    exec("./scripts/InterfaceTool/worldButtonPreview.ed.cs");
    exec("./scripts/InterfaceTool/levelListPane.cs");
    exec("./scripts/CollisionSidebar/collisionSidebar.ed.cs");
    exec("./scripts/ToolConfirmDeleteGui.ed.cs");

    
    //-----------------------------------------------------------------------------
    // Load GUIs
    //-----------------------------------------------------------------------------
    TamlRead("./gui/templateToolbar.ed.gui.taml");
    TamlRead("./gui/LevelBuilderTool/LevelBuilderToolLeftPassiveView.ed.gui.taml");
    TamlRead("./gui/LevelBuilderTool/LevelBuilderToolRightPassiveView.ed.gui.taml");
    TamlRead("./gui/LauncherTool/launcherTool.gui.taml");
    TamlRead("./gui/ProjectileTool/projectileToolForm.ed.gui.taml");    
    TamlRead("./gui/ProjectileTool/ptConfirmDeleteGui.ed.gui.taml");
    TamlRead("./gui/WorldObjectTool/worldObjectTool.gui.taml");    
    TamlRead("./gui/WorldTool/WorldTool.ed.gui.taml");
    TamlRead("./gui/WorldTool/worldToolAssignLevel.ed.gui.taml");
    TamlRead("./gui/WorldTool/worldToolConfirmDelete.ed.gui.taml");
    TamlRead("./gui/WorldTool/worldToolConfirmWorldDelete.ed.gui.taml");
    TamlRead("./gui/WorldTool/worldToolLevelLimit.ed.gui.taml");
    TamlRead("./gui/InterfaceTool/interfaceTool.ed.gui.taml");
    TamlRead("./gui/InterfaceTool/WorldButtonPreview.ed.gui.taml");
    TamlRead("./gui/ToolConfirmDeleteGui.ed.gui.taml");    
    
    //-----------------------------------------------------------------------------
    // Initialization
    //----------------------------------------------------------------------------- 
    // Register Events
    initializePhysicsLauncherToolsEventManager();
    EditorShellGui.setToolBar(TemplateToolbar);
    LevelBuilderToolPresenter.initialize(LevelBuilderToolLeftPassiveView, LevelBuilderToolRightPassiveView);
    
    $CurrentTemplatePackage = "PhysicsLauncherPackage";

    activatePackage($CurrentTemplatePackage);
    $PhysicsLauncher::WorldListFile = $PhysicsLauncher::UserHomeDirectory @ "/My Games/" @ $Game::CompanyName @ "/{PhysicsLauncher}/" @ $Game::ProductName @ "/worldList.taml";
    if ( isObject($PhysicsLauncherTools::currentWorldData) )
        $PhysicsLauncherTools::currentWorldData = "";
        
    // Open World Tool on start-up
    Tt_WorldToolButton.setStateOn(true);
    Tt_WorldToolButton.onClick();
    //makeAssetFiles("^{PhysicsLauncherAssets}/audio/", "sound", "", false);   
}

function destroyPhysicsLauncherTools()
{
    // If the launcher tool is open, update the current launcher prefab in all levels that use it
    if ( isObject(LauncherToolGui) )
    {
        if (LauncherToolGui.isAwake() && isObject(LauncherTool.currentObject))
            SlingshotLauncherBuilder::updateLauncherPrefabInAllLevels(LauncherTool.currentObject);    
    }

    SaveTemplateData();
    destroyPhysicsLauncherToolsEventManager();
    
    ModuleDatabase.unloadGroup(assetTools);
}


package PhysicsLauncherPackage
{
    function CreateNewSpriteSheet(%tag)
    {
        launchNewImageMap(true);
    }

    function CreateNewAnimation(%tag)
    {
        AnimationBuilder.createAnimation();
    }
   
    function CreateNewGuiImage(%tag)
    {
        // Get the game's gui/images directory
        %gameGuiDir = LBProjectObj.gamePath @ "/";

        %dlg = new OpenFileDialog()
        {
            Filters = $T2D::ImageMapSpec;
            ChangePath = false;
            MustExist = true;
            MultipleFiles = false;
        };

        if (%dlg.Execute())
        {
            %fileName = %dlg.fileName;
            
            // Check file validity
            if (!isValidImageFile(%fileName))
            {
                //MessageBoxOK("Warning", "'" @ %fileOnlyName @ "' is not a valid image file.", "");
				NoticeGui.display("The image you have chosen is not a valid image file.");
                return;
            }
            
			// Check valid image size
			if (!isValidImageSize(%fileName))
			{
				NoticeGui.display("The image you have chosen is too large and cannot be loaded.");
				return;
			}

            generateGuiImageMapFromFile(%fileName);
        }

        %dlg.delete();
    }
       
    function CreateNewAudioProfile(%tag)
    {
        launchSoundImporter("");
    }

    function CreateNewLevel(%tag)
    {
        error("### Physics Launcher CreateNewLevel not yet implemented");
    }
   
    function CreateNewBitmapFont(%tag)
    {
        launchFontTool("");
    }

    function EditSpriteSheet(%assetId)
    {
        launchEditImageMap(%assetId);
    }

    function EditAnimation(%assetId)
    {
        launchEditAnimation(%assetId);
    }

    function EditSoundProfile(%assetId)
    {
        launchSoundImporter(%assetId);
    }
    
    function EditBitmapFont(%assetId)
    {
        launchFontTool(%assetId);
    }

    function EditLevel(%assetId)
    {
        error("### Physics Launcher EditLevel not yet implemented");
    }

    function SaveTemplateData()
    {
        %writePrefabs = false;
        
        if (LevelBuilderToolLeftPassiveView.isAwake())
            LevelBuilderToolPresenter.saveCurrentLevel(true);
            
        if ( isObject(WorldObjectToolGui) )
        {
            if ( WorldObjectToolGui.isAwake() )
                %writePrefabs = true;
        }
        if ( isObject (LauncherToolGui) )
        {
            if ( LauncherToolGui.isAwake() )
                %writePrefabs = true;
        }
        if ( isObject(ProjectileToolForm) )
        {
            if ( ProjectileToolForm.isAwake() )
                %writePrefabs = true;
        }
        if ( %writePrefabs )
            PhysicsLauncherTools::writePrefabs();
        if ( isObject(WorldTool) )
        {
            if ( WorldTool.isAwake() )
               WorldTool.saveData();
        }
        if ( isObject(InterfaceTool) )
        {
            if ( InterfaceTool.isAwake() )
                InterfaceTool.saveData();
        }
    }
    
    function GetTemplateToolsGroup()
    {
        return "PhysicsLauncherToolsGroup";
    }
};