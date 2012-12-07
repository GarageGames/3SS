//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

//file: moveChipEffectBehavior.cs

// Create this behavior only if it does not already exist
if (!isObject(MoveChipEffectBehavior))
{
    %template = new BehaviorTemplate(MoveChipEffectBehavior);

    %template.friendlyName = "Move Chip Effect";
    %template.behaviorType = "Animation Effect";
    %template.description  = "Set the object to move toward another object";

    %template.addBehaviorField(speed, "The speed to move toward the object at (world units per second)", float, 2.0);
}

/// <summary>
/// The chip object has completed its movement.  If the chip is not in the correct
/// position, simply warp it there.
/// </summary>
/// <param name="obj">The behavior instance.</param>
/// <param name="targetPosition">The intended location at the end of the movement.</param>
function MoveChipEffectBehavior::onMoveToComplete(%obj, %targetPosition)
{
    %obj.owner.setPosition(%targetPosition);

    %obj.onPositionTarget();
}

/// <summary>
/// onPositionTarget callback
/// </summary>
function MoveChipEffectBehavior::onPositionTarget(%this)
{
    %betAreaBehavior = %this.owner.betAreaBehavior;

    // Update the GUI for the bet area object
    %betAreaBehavior.update();

    // Delete the object this behavior is attached to
    %this.owner.safeDelete();
}