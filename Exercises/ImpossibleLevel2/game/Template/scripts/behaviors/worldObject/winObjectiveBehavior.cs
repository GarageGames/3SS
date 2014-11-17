//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

if(!isObject(WinObjectiveBehavior))
{
    %template = new BehaviorTemplate(WinObjectiveBehavior);
    %template.friendlyName = "Win Objective";
    %template.behaviorType = "World Object";
    %template.description = "Defines the owner as a win objective when destroyed (can't win while the object exists).";
    
    %template.addBehaviorField(instanceName, "The behavior instance name.", string, ""); 
    %template.addBehaviorField(isActive, "Flag that controls whether the behavior is active in level", bool, false);   
    
    // Inputs
    %template.addBehaviorInput(onDestroyInput, "On Destroyed", "Receive when the owner of the behavior should be destroyed");

    // Outputs
    %template.addBehaviorOutput(onCreatedOutput, "On Created", "Trigger when the owner of the behavior is created");
    %template.addBehaviorOutput(onDestroyedOutput, "On Destroyed", "Trigger when the owner of the behavior is destroyed");
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function WinObjectiveBehavior::onBehaviorAdd(%this)
{
    
}

/// <summary>
/// Called when the level has been loaded.  This is to allow all persistent objects to 
/// be created before trying to register creation with the GameMaster object.
/// </summary>
/// <param name="scene">The scene object the behavior was added to.</param>
function WinObjectiveBehavior::onLevelLoaded(%this, %scene)
{
    %this.scene = %scene;  
    
    if (%this.isActive == false)
        return;

    // clones don't have names, template objects aren't guaranteed to be disabled
    // apparently.
    if (%this.owner.getName() $= "")
        %this.owner.Raise(%this, onCreatedOutput);
}

function WinObjectiveBehavior::onDestroyInput(%this)
{    
    if (%this.isActive == false)
        return;
        
    // Raise an OnDestroyed signal on the owner
    %this.owner.Raise(%this, onDestroyedOutput);
}

function WinObjectiveBehavior::setWinObjectiveIsActive(%this, %flag)
{
    %this.isActive = %flag;
}

function WinObjectiveBehavior::getWinObjectiveIsActive(%this)
{
    return %this.isActive;   
}