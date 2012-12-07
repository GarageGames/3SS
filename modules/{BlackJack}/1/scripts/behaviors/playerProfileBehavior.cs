//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
// PlayerProfileBehavior - Behavior for persistent player profiles
//-----------------------------------------------------------------------------

$PlayerProfileFilePath = "^BlackjackTemplate/data/files/";
$PlayerProfileFileSuffix = "PlayerProfile.xml";

// Create this behavior only if it does not already exist
if (!isObject(PlayerProfileBehavior))
{
    // Create this behavior from the blank BehaviorTemplate
    // Name it TouchDrag
    %template = new BehaviorTemplate(PlayerProfileBehavior);

    // friendlyName will be what is displayed in the editor
    // behaviorType organize this behavior in the editor
    // description briefly explains what this behavior does
    %template.friendlyName = "Player Profile";
    %template.behaviorType = "Player";
    %template.description  = "Stores a persistent profile for a player";

    %template.addBehaviorField(playerName, "The player's name", string, "DefaultPlayerName");
    %template.addBehaviorField(startingCash, "The cash a player starts with", int, 5000);
}

/// <summary>
/// Init function
/// </summary>
function PlayerProfileBehavior::init(%this)
{
    %this.reset();

    %this.loadProfileXMLFile();
}

/// <summary>
/// resets the profile currentCash to the startingCash.
/// </summary>
function PlayerProfileBehavior::reset(%this)
{
    %this.currentCash = %this.startingCash;
}

/// <summary>
/// Updates the PlayerProfileBehavior with the data in %player
/// </summary>
function PlayerProfileBehavior::updateFromPlayer(%this, %player)
{
    if (isObject(%player.bank))
        %player.bank.savePlayerCash(%this);
}

/// <summary>
/// Load PlayerProfileBehavior from XML file, if it exists
/// </summary>
function PlayerProfileBehavior::loadProfileXMLFile(%this)
{
    %filePath = expandPath($PlayerProfileFilePath @ %this.playerName @ $PlayerProfileFileSuffix);

    %xml = new SimXMLDocument();
    %success = %xml.loadFile(%filePath);

    if (%success)
    {
        %xml.pushChildElement("playerProfile");
        %xml.pushFirstChildElement("currentCash");
        %result = %xml.getData();

        %this.currentCash = %result;
    }
}

/// <summary>
/// Saves the PlayerProfileBehavior to an XML file
/// </summary>
function PlayerProfileBehavior::createProfileXMLFile(%this)
{
    %filePath = expandPath($PlayerProfileFilePath @ %this.playerName @ $PlayerProfileFileSuffix);

    %xml = new SimXMLDocument();
    %xml.addHeader();
    %xml.addComment("Player profile for " @ %this.playerName);
    %xml.addNewElement("playerProfile");
    %xml.pushNewElement("currentCash");
    %xml.addData(%this.currentCash);
    %success = %xml.saveFile(%filePath);

    if (%success)
    {
        //echo("Save player profile to " @ %filePath);
    }
    else
        warn("***Failed to save player profile to "@ %filePath);
}

