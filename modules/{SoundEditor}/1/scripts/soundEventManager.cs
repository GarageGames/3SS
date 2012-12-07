//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// Builds the event manager and script listener that will be responsible for
// handling important system events, such as a module group finishing a load,
// a patch being available, a download completing, etc
function initializeSoundEditorEventManager()
{
    if (!isObject(SoundEditorEventManager))
    {
        $SoundEditorEventManager = new EventManager(SoundEditorEventManager)
        { 
            queue = "SoundEditorEventManager"; 
        };
        
        // Module related signals
        SoundEditorEventManager.registerEvent("_PauseRequest");
        SoundEditorEventManager.registerEvent("_PlayRequest");
    }
    
    if (!isObject(SoundEditorListenerObject))
    {
        $SoundEditorListener = new ScriptMsgListener(SoundEditorListenerObject) 
        { 
            class = "SoundEditorListener"; 
        };
        
        // Module related subscriptions
        SoundEditorEventManager.subscribe(SoundEditorListenerObject, "_PauseRequest", "onPauseRequest");
        SoundEditorEventManager.subscribe(SoundEditorListenerObject, "_PlayRequest", "onPlayRequest");
    }
}

// Cleanup the SoundEditorEventManager
function destroySoundEditorEventManager()
{
    if (isObject(SoundEditorEventManager) && isObject(SoundEditorListenerObject))
    {
        // Remove all the subscriptions5        SoundEditorEventManager.remove(SoundEditorListener, "_AnimUpdateRequest");
        SoundEditorEventManager.remove(SoundEditorListenerObject, "_PauseRequest");
        SoundEditorEventManager.remove(SoundEditorListenerObject, "_PlayRequest");

        // Delete the actual objects
        SoundEditorEventManager.delete();
        SoundEditorListenerObject.delete();
        
        // Clear the global variables, just in case
        $SoundEditorEventManager = "";
        $SoundEditorListener = "";
    }
}

function SoundEditorListener::onPauseRequest(%this, %msgData)
{
    EditorSoundPlayer.schedule(64, pause);
}

function SoundEditorListener::onPlayRequest(%this, %msgData)
{
    EditorSoundPlayer.schedule(64, play);
}