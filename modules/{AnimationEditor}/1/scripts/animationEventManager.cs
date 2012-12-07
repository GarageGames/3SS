//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// Builds the event manager and script listener that will be responsible for
// handling important system events, such as a module group finishing a load,
// a patch being available, a download completing, etc
function initializeAnimBuilderEventManager()
{
    if (!isObject(AnimBuilderEventManager))
    {
        $AnimBuilderEventManager = new EventManager(AnimBuilderEventManager)
        { 
            queue = "AnimBuilderEventManager"; 
        };
        
        // Module related signals
        AnimBuilderEventManager.registerEvent("_AnimUpdateRequest");
        AnimBuilderEventManager.registerEvent("_AnimPreviewUpdateRequest");
        AnimBuilderEventManager.registerEvent("_AnimContentPaneUpdateComplete");
        AnimBuilderEventManager.registerEvent("_StoryboardContentPaneUpdateComplete");
        AnimBuilderEventManager.registerEvent("_TimelineDeleteRequest");
        AnimBuilderEventManager.registerEvent("_ClearTimelineRequest");
        AnimBuilderEventManager.registerEvent("_SetTimelineRequest");
    }
    
    if (!isObject(AnimBuilderListener))
    {
        $AnimBuilderListener = new ScriptMsgListener(AnimBuilderListener) 
        { 
            class = "AnimBuilder"; 
        };
        
        // Module related subscriptions
        AnimBuilderEventManager.subscribe(AnimBuilderListener, "_AnimUpdateRequest", "onAnimUpdateRequest");
        AnimBuilderEventManager.subscribe(AnimBuilderListener, "_AnimPreviewUpdateRequest", "onAnimPreviewUpdateRequest");
        AnimBuilderEventManager.subscribe(AnimBuilderListener, "_AnimContentPaneUpdateComplete", "onAnimContentPaneUpdateComplete");
        AnimBuilderEventManager.subscribe(AnimBuilderListener, "_StoryboardContentPaneUpdateComplete", "onStoryboardContentPaneUpdateComplete");
        AnimBuilderEventManager.subscribe(AnimBuilderListener, "_TimelineDeleteRequest", "onTimelineDeleteRequest");
        AnimBuilderEventManager.subscribe(AnimBuilderListener, "_ClearTimelineRequest", "onClearTimelineRequest");
        AnimBuilderEventManager.subscribe(AnimBuilderListener, "_SetTimelineRequest", "onSetTimelineRequest");
    }
}

// Cleanup the AnimBuilderEventManager
function destroyAnimBuilderEventManager()
{
    if (isObject(AnimBuilderEventManager) && isObject(AnimBuilderListener))
    {
        // Remove all the subscriptions5        AnimBuilderEventManager.remove(AnimBuilderListener, "_AnimUpdateRequest");
        AnimBuilderEventManager.remove(AnimBuilderListener, "_AnimPreviewUpdateRequest");
        AnimBuilderEventManager.remove(AnimBuilderListener, "_AnimContentPaneUpdateComplete");
        AnimBuilderEventManager.remove(AnimBuilderListener, "_StoryboardContentPaneUpdateComplete");
        AnimBuilderEventManager.remove(AnimBuilderListener, "_TimelineDeleteRequest");
        AnimBuilderEventManager.remove(AnimBuilderListener, "_ClearTimelineRequest");
        AnimBuilderEventManager.remove(AnimBuilderListener, "_SetTimelineRequest");

        // Delete the actual objects
        AnimBuilderEventManager.delete();
        AnimBuilderListener.delete();
        
        // Clear the global variables, just in case
        $AnimBuilderEventManager = "";
        $AnimBuilderListener = "";
    }
}

function AnimBuilder::onAnimContentPaneUpdateComplete(%this, %msgData)
{
    %obj = getWord(%msgData, 0);
    %count = getWord(%msgData, 1);
    %time = (%count / 10);
    %wait = (%time < 32 ? 32 : %time);
    %obj.schedule(%wait, "update", %count);
}

function AnimBuilder::onStoryboardContentPaneUpdateComplete(%this, %msgData)
{
    // %msgData contains: ABStoryboardWindow @ sourceImage @ frameList
    %obj = getWord(%msgData, 0);
    %image = getWord(%msgData, 1);
    %count = getWordCount(%msgData);
    for (%i = 2; %i < %count; %i++)
    {
        %frames = %frames @ " " @ getWord(%msgData, %i);
    }
    %frames = trim(%frames);
    %time = (%count / 10);
    %wait = (%time < 32 ? 32 : %time);
    %obj.schedule(%wait, "update", %image, %frames);
}

function AnimBuilder::onAnimUpdateRequest(%this, %msgData)
{
    AnimationBuilder.schedule(64, updateGui);
}

function AnimBuilder::onTimelineDeleteRequest(%this, %msgData)
{
    AnimationBuilder.schedule(64, completeFrameRemoval, %msgData);
}

function AnimBuilder::onClearTimelineRequest(%this, %msgData)
{
    AnimationBuilder.schedule(64, completeClearTimeline);
}

function AnimBuilder::onSetTimelineRequest(%this, %msgData)
{
    AnimationBuilder.schedule(64, completeSetFrames);
}

function AnimBuilder::onAnimPreviewUpdateRequest(%this, %msgData)
{
    AnimationBuilder.schedule(64, updatePreviewAnimation);
}