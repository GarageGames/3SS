//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
// LevelLoadBehavior - Behavior for buttons that load new levels.
//-----------------------------------------------------------------------------

// Create this behavior only if it does not already exist
if (!isObject(LevelLoadBehavior))
{
    // Create this behavior from the blank BehaviorTemplate
    // Name it TouchDrag
    %template = new BehaviorTemplate(LevelLoadBehavior);

    // friendlyName will be what is displayed in the editor
    // behaviorType organize this behavior in the editor
    // description briefly explains what this behavior does
    %template.friendlyName = "Level Loading";
    %template.behaviorType = "Level Component";
    %template.description  = "Loads a specified level when clicked.";

    %template.addBehaviorField(levelFile, "Level file to load", FileList, "", "*.scene.taml");
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function LevelLoadBehavior::onBehaviorAdd(%this)
{
    %this.owner.UseInputEvents = true;
}

function LevelLoadBehavior::onTouchUp(%this, %touchID, %worldPos)
{
    %this.loadLevel(%this.levelFile);
}

/// <summary>
/// Sets the level file for the behavior
/// </summary>
function LevelLoadBehavior::setLevelFile(%this, %level)
{
    %this.levelFile = %level;
}

/// <summary>
/// Loads a new scene.
/// <param name="level">The path string of the level file.</param>
/// </summary>
function LevelLoadBehavior::loadLevel(%this, %level)
{
    // Reset values used by the levels
    $NumberOfPlayers = 0;
    $SeatSelectingPlayer = 0;
    $PlayInputPlayer = null;

    // Extract the file name from the full path
    %levelFileName = fileName(%level);

    // Schedule the level to load
    sceneWindow2D.schedule(200, "loadLevel", "^BlackjackTemplate/data/levels/" @ %levelFileName);
}
