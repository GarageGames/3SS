//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// Builds the event manager and script listener that will be responsible for
// handling important game system events.
function initializePhysicsLauncherToolsEventManager()
{
    if (!isObject(PhysicsLauncherToolsEventManager))
    {
        $PhysicsLauncherToolsEventManager = new EventManager(PhysicsLauncherToolsEventManager)
        { 
            queue = "PhysicsLauncherToolsEventManager"; 
        };
        
        // Level Builder Tool Events

        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolLeftPassiveView::Wake");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolLeftPassiveView::Sleep");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolLeftPassiveView::ControlDropped");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolLeftPassiveView::DuplicationStampMove");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolLeftPassiveView::DuplicationStampDown");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolLeftPassiveView::WorldSelect");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolLeftPassiveView::LevelSelect");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolLeftPassiveView::CollisionToggle");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolLeftPassiveView::ZoomSelect");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolLeftPassiveView::PhysicsPreviewToggle");
        
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::Wake");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::Sleep");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::SelectionToolSelected");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::RotateSelected");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::DuplicateSelected");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::BringToFrontSelected");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::SendToBackSelected");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::DeleteSelected");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::SelectedObjectEditSelected");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::GravityUpSelected");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::GravityNoneSelected");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::GravityDownSelected");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::GravityStrengthSelect");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::LevelSizeSelect");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::BackgroundFormatTileSelect");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::BackgroundFormatStretchSelect");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::ForegroundParallaxSpeedSelect");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::Background1ParallaxSpeedSelect");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::Background2ParallaxSpeedSelect");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::SkyParallaxSpeedSelect");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::ForegroundAssetSelect");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::Background1AssetSelect");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::Background2AssetSelect");
        PhysicsLauncherToolsEventManager.registerEvent("LevelBuilderToolRightPassiveView::SkyAssetSelect");
        
        PhysicsLauncherToolsEventManager.registerEvent("_LevelButtonUpdateComplete");
        PhysicsLauncherToolsEventManager.registerEvent("_WorldButtonUpdateComplete");
        PhysicsLauncherToolsEventManager.registerEvent("_ProjectileButtonUpdateComplete");
        PhysicsLauncherToolsEventManager.registerEvent("_AddWorldButtonRequest");
        PhysicsLauncherToolsEventManager.registerEvent("_LauncherRemoveRequest");

        // Module related signals
        PhysicsLauncherToolsEventManager.registerEvent("_TemplateLoaded");
        PhysicsLauncherToolsEventManager.registerEvent("_Cleanup");
        PhysicsLauncherToolsEventManager.registerEvent("_LevelLoaded");
    }
    
    if (!isObject(PhysicsLauncherToolsListener))
    {
        $PhysicsLauncherToolsListener = new ScriptMsgListener(PhysicsLauncherToolsListener) 
        { 
            class = "PhysicsLauncherToolsEvents"; 
        };
        
        // Module related subscriptions
        PhysicsLauncherToolsEventManager.subscribe(PhysicsLauncherToolsListener, "_LevelLoaded", "onLevelLoaded");
        PhysicsLauncherToolsEventManager.subscribe(PhysicsLauncherToolsListener, "_Cleanup", "onCleanup");
        PhysicsLauncherToolsEventManager.subscribe(PhysicsLauncherToolsListener, "_TemplateLoaded", "onTemplateLoaded");

        PhysicsLauncherToolsEventManager.subscribe(PhysicsLauncherToolsListener, "_LevelButtonUpdateComplete", "onLevelButtonUpdateComplete");
        PhysicsLauncherToolsEventManager.subscribe(PhysicsLauncherToolsListener, "_WorldButtonUpdateComplete", "onWorldButtonUpdateComplete");
        PhysicsLauncherToolsEventManager.subscribe(PhysicsLauncherToolsListener, "_ProjectileButtonUpdateComplete", "onProjectileButtonUpdateComplete");
        PhysicsLauncherToolsEventManager.subscribe(PhysicsLauncherToolsListener, "_AddWorldButtonRequest", "onAddWorldButtonRequest");
        PhysicsLauncherToolsEventManager.subscribe(PhysicsLauncherToolsListener, "_LauncherRemoveRequest", "onLauncherRemoveRequest");
    }
}

// Cleanup the event manager
function destroyPhysicsLauncherToolsEventManager()
{
    if (isObject(PhysicsLauncherToolsEventManager) && isObject(PhysicsLauncherToolsListener))
    {
        // Remove all the subscriptions
        PhysicsLauncherToolsEventManager.remove(PhysicsLauncherToolsListener, "_Cleanup");
        PhysicsLauncherToolsEventManager.remove(PhysicsLauncherToolsListener, "_TemplateLoaded");
        PhysicsLauncherToolsEventManager.remove(PhysicsLauncherToolsListener, "_LevelLoaded");

        PhysicsLauncherToolsEventManager.remove(PhysicsLauncherToolsListener, "_LevelButtonUpdateComplete");
        PhysicsLauncherToolsEventManager.remove(PhysicsLauncherToolsListener, "_WorldButtonUpdateComplete");
        PhysicsLauncherToolsEventManager.remove(PhysicsLauncherToolsListener, "_ProjectileButtonUpdateComplete");
        PhysicsLauncherToolsEventManager.remove(PhysicsLauncherToolsListener, "_AddWorldButtonRequest");
        PhysicsLauncherToolsEventManager.remove(PhysicsLauncherToolsListener, "_LauncherRemoveRequest");

        // Delete the actual objects
        PhysicsLauncherToolsEventManager.delete();
        PhysicsLauncherToolsListener.delete();
        
        // Clear the global variables, just in case
        $PhysicsLauncherToolsEventManager = "";
        $PhysicsLauncherToolsListener = "";
    }
}

function PhysicsLauncherToolsEvents::onLevelButtonUpdateComplete(%this, %messageData)
{
    WorldTool.schedule(64, SetSelectedLevelButton, %messageData);
    WorldTool.LevelListContainer.schedule(64, setSelected, %messageData);
}

function PhysicsLauncherToolsEvents::onWorldButtonUpdateComplete(%this, %messageData)
{
    WorldTool.schedule(32, refreshWorldList);
    WorldTool.WorldListContainer.schedule(64, setSelected, %messageData);
}

function PhysicsLauncherToolsEvents::onProjectileButtonUpdateComplete(%this, %messageData)
{
    ProjectileTool.schedule(64, SetSelectedProjectileButton, %messageData);
    ProjectileTool.ProjectileContainer.schedule(64, setSelected, %messageData);
}

function PhysicsLauncherToolsEvents::onTemplateLoaded(%this, %messageData)
{
    echo("@@@ Finished launching module:" SPC %messageData);
}

function PhysicsLauncherToolsEvents::onCleanup(%this, %messageData)
{
    // A game entity is ready for deletion
    if (isObject(%messageData))
    {
        //echo(" @@@ object " @ %messageData @ " cleaning up.");
        %messageData.safeDelete();
    }
}

function PhysicsLauncherToolsEvents::onAddWorldButtonRequest(%this, %messageData)
{
    WorldTool.schedule(64, "finalizeWorldAdd", %messageData);
}

function PhysicsLauncherToolsEvents::onLauncherRemoveRequest(%this, %messageData)
{
    LauncherTool.schedule(64, "onLauncherRemove", %messageData);
}