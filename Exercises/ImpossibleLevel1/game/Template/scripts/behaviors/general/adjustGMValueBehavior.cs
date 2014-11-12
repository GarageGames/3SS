//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

if(!isObject(AdjustGMValueBehavior))
{
    %template = new BehaviorTemplate(AdjustGMValueBehavior);
    %template.friendlyName = "Adjust Game Master Value";
    %template.behaviorType = "general";
    %template.description = "Adjusts a Value on the Game Master";

    %template.addBehaviorField(adjustableValueField, "Value field on the Game Master which may be adjusted", string, "");
    %template.addBehaviorField(adjustment, "Adjustment this behavior imparts", float, 0.0);

    // Inputs
    %template.addBehaviorInput(adjustGMValue, "Adjust Game Master Value", "Triggers the behavior to trigger a value adjustment");
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function AdjustGMValueBehavior::onBehaviorAdd(%this)
{
    
}

/// <summary>
/// Called when the behavior is added to the scene
/// </summary>
/// <param name="scene">The scene object the behavior was added to.</param>
function AdjustGMValueBehavior::onAddToScene(%this, %scene)
{
    %this.scene = %scene;
}


/// <summary>
/// Input method that triggers a value adjustment
/// </summary>
/// <param name="fromBehavior">The output behavior that triggered this input.</param>
/// <param name="fromOutput">The output method that triggered this input.</param>
function AdjustGMValueBehavior::adjustGMValue(%this, %fromBehavior, %fromOutput)
{
    if (!isObject(GameMaster))
    {
        warn("AdjustGMValueBehavior::adjustGMValue - no Game Master object present"); 
        return;  
    }
    
    // Send the adjustment to the Game Master
    %result = GameMaster.callonBehaviors(adjustValue, %this.adjustableValueField, %this.adjustment);
    
    if (%result $= "ERR_CALL_NOT_HANDLED")
        warn("AdjustGMValueBehavior::adjustGMValue - Game Master does not have a behavior with the \"adjustValue\" method");
}


function AdjustGMValueBehavior::getAdjustableValueField(%this)
{
    return %this.adjustableValueField;
}

function AdjustGMValueBehavior::setAdjustableValueField(%this, %adjustableValueField)
{
    %this.adjustableValueField = %adjustableValueField;
}