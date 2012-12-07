//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// Builds the event manager and script listener that will be responsible for
// handling important game system events.
function initializeGameEventManager()
{
    if (!isObject(GameEventManager))
    {
        $GameEventManager = new EventManager(GameEventManager)
        { 
            queue = "GameEventManager"; 
        };
        
        // Module related signals
        GameEventManager.registerEvent("_StartUpComplete");
        GameEventManager.registerEvent("_StartGame");
        GameEventManager.registerEvent("_TemplateLoaded");
        GameEventManager.registerEvent("_Cleanup");
        GameEventManager.registerEvent("_AllTemplatesLoaded");
        GameEventManager.registerEvent("_LevelLoaded");
    }
    
    if (!isObject(GameListener))
    {
        $GameListener = new ScriptMsgListener(GameListener) 
        { 
            class = "Game"; 
        };
        
        // Module related subscriptions
        GameEventManager.subscribe(GameListener, "_StartUpComplete", "onStartUpComplete");
        GameEventManager.subscribe(GameListener, "_StartGame", "onStartGame");
        GameEventManager.subscribe(GameListener, "_LevelLoaded", "onLevelLoaded");
        GameEventManager.subscribe(GameListener, "_Cleanup", "onCleanup");
        GameEventManager.subscribe(GameListener, "_TemplateLoaded", "onTemplateLoaded");
        GameEventManager.subscribe(GameListener, "_AllTemplatesLoaded", "onAllTemplatesLoaded");
    }
}

// Cleanup the event manager
function destroyGameEventManager()
{
    if (isObject(GameEventManager) && isObject(GameListener))
    {
        // Remove all the subscriptions
        GameEventManager.remove(GameListener, "_StartUpComplete");
        GameEventManager.remove(GameListener, "_StartGame");
        GameEventManager.remove(GameListener, "_Cleanup");
        GameEventManager.remove(GameListener, "_TemplateLoaded");
        GameEventManager.remove(GameListener, "_AllTemplatesLoaded");
        GameEventManager.remove(GameListener, "_LevelLoaded");
        
        // Delete the actual objects
        GameEventManager.delete();
        GameListener.delete();
        
        // Clear the global variables, just in case
        $GameEventManager = "";
        $GameListener = "";
    }
}

function Game::onStartUpComplete(%this, %messageData)
{
    %templateIDs = ModuleDatabase.findModuleTypes("Template", false);
    
    for (%i = 0; %i < getWordCount(%templateIDs); %i++)
    {
        %templateModule = getWord(%templateIDs, %i);
        
        %templateGroup = %templateModule.group;
        
        %loadSuccess = ModuleDatabase.loadGroup(%templateGroup);
        
        if (%loadSuccess)
            GameEventManager.postEvent("_TemplateLoaded", %templateModule.ModuleId);
    }
    
    GameEventManager.postEvent("_AllTemplatesLoaded");
}

function Game::onTemplateLoaded(%this, %messageData)
{
    
}

function Game::onCleanup(%this, %messageData)
{
    // A game entity is ready for deletion
    if (isObject(%messageData))
    {
        //echo(" @@@ object " @ %messageData @ " cleaning up.");
        %messageData.safeDelete();
    }
}