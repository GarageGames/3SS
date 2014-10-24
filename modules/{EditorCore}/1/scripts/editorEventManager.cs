//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// Builds the event manager and script listener that will be responsible for
// handling important system events, such as a module group finishing a load,
// a patch being available, a download completing, etc
function initializeEditorEventManager()
{
    if (!isObject(EditorEventManager))
    {
        $EditorEventManager = new EventManager(EditorEventManager)
        { 
            queue = "EditorEventManager"; 
        };
        
        // Module and editor related signals. Implemented in {3SSEditor}/scripts/events.cs"
        EditorEventManager.registerEvent("_StartUpComplete");
        EditorEventManager.registerEvent("_ProjectToolsLoaded");
        EditorEventManager.registerEvent("_ProjectLoaded");
        EditorEventManager.registerEvent("_CoreShutdown");
    }
    
    if (!isObject(EditorListener))
    {
        $EditorListener = new ScriptMsgListener(EditorListener) 
        { 
            class = "Editor"; 
        };
        
        // Module and editor related subscriptions
        EditorEventManager.subscribe(EditorListener, "_StartUpComplete", "onStartUpComplete");
        EditorEventManager.subscribe(EditorListener, "_ProjectToolsLoaded", "onProjectToolsLoaded");
        EditorEventManager.subscribe(EditorListener, "_ProjectLoaded", "onProjectLoaded");
        EditorEventManager.subscribe(EditorListener, "_CoreShutdown", "onCoreShutdown");
    }
}

// Cleanup the 
function destroyEditorEventManager()
{
    if (isObject(EditorEventManager) && isObject(EditorListener))
    {
        // Remove all the subscriptions
        EditorEventManager.remove(EditorListener, "_StartUpComplete");
        EditorEventManager.remove(EditorListener, "_ProjectToolsLoaded");
        EditorEventManager.remove(EditorListener, "_ProjectLoaded");
        EditorEventManager.remove(EditorListener, "_CoreShutdown");
        
        // Delete the actual objects
        EditorEventManager.delete();
        EditorListener.delete();
        
        // Clear the global variables, just in case
        $EditorEventManager = "";
        $EditorListener = "";
    }
}
