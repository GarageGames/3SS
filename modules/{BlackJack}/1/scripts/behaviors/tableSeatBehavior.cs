//-----------------------------------------------------------------------------
// Torque Card Template
// Copyright © GarageGames, LLC.
//-----------------------------------------------------------------------------
// A Seat at the table behavior.
//-----------------------------------------------------------------------------

/// Seat object selecting a player (0 if none).
$SeatSelectingPlayer = 0;

// Create this behavior only if it does not already exist
if (!isObject(TableSeatBehavior))
{
    // Create this behavior from the blank BehaviorTemplate
    %template = new BehaviorTemplate(TableSeatBehavior);

    // friendlyName will be what is displayed in the editor
    %template.friendlyName = "Table Seat";

    // behaviorType organize this behavior in the editor
    %template.behaviorType = "Card Template";

    // description briefly explains what this behavior does
    %template.description  = "A Seat at the table behavior.";

    //--------------------
    // Behavior Fields
    //--------------------

    %template.addBehaviorField(seatIndex, "Where in play order this seat is.", int, -1);
    %template.addBehaviorField(emptyIcon, "Icon displayed to empty seat.", object, "", SceneObject);
    %template.addBehaviorField(iconWidth, "Width of icons used to display available players.", int, 100);
    %template.addBehaviorField(mainBetArea, "The default bet area object associated with this seat", object, "", SceneObject);
    %template.addBehaviorField(sideBetArea, "The side bet area object associated with this seat", object, "", SceneObject);
    %template.addBehaviorField(insuranceBetArea, "The insurance bet area object associated with this seat", object, "", SceneObject);
    %template.addBehaviorField(aiBankDisplay, "The numerical display object to use if the seated player is an AI player", object, "", BitmapFontObject);
}

/// <summary>
/// Called when TableSeatBehavior is added to an object.
/// </summary>
function TableSeatBehavior::onBehaviorAdd(%this)
{
    %this.player = 0;

    // Give it time
    %this.schedule(100, "addToTable");
}

/// <summary>
/// Add this seat the the $CurrentTable.
/// </summary>
function TableSeatBehavior::addToTable(%this)
{
    if (!isObject($CurrentTable))
        warn("Tried to add TableSeatBehavior without a valid $CurrentTable.");

    %originalSeatIndex = %this.seatIndex;

    if (%this.seatIndex < 0)
        %this.seatIndex = 0;

    // get the first non-valid seat index
    while ($CurrentTable.isSeatValid(%this.seatIndex))
    {
        %this.seatIndex++;
    }

    if (%this.seatIndex != %originalSeatIndex)
        warn("***Tried to add TableSeatBehavior at index" SPC %originalSeatIndex SPC "but had to add at" SPC %this.seatIndex);

    %result = $CurrentTable.setSeat(%this, %this.seatIndex);
    if (%result == false)
        error("Could not add TableSeatBehavior at index" SPC %this.seatIndex);
}

/// <return>The number of players not currently assigned to a seat.</return>
function TableSeatBehavior::getAvailablePlayerCount(%this)
{
    %count = 0;
    for (%i = 0; %i < $NumberOfPlayers; %i++)
    {
        if (!isObject($PlayerArray[%i].seat))
            %count++;
    }

    return %count;
}

/// <summary>
/// Set the player at this seat (replacing any current player).
/// </summary>
/// <param "player">Player being added to this seat (0 if we're clearing the seat).</param>
function TableSeatBehavior::setPlayer(%this, %player)
{
    if (%this.player != 0)
    {
        // clear the seat.
        if (isObject(%this.player.seat))
            %this.player.seat.clear();

        %this.player.seat = null;
    }

    // Clear the aiBankDisplay
    %this.aiBankDisplay.setText("");

    %this.player = %player;
    if (isObject(%player))
    {
        %player.owner.setPosition(%this.owner.getPosition());
        %player.owner.setAngle(%this.owner.getAngle());
        %player.seat = %this;
        %player.owner.UseInputEvents = false;

        // Set the numerical display for the player if it is ai controlled
        if (%player.isUserControlled == false)
        {
            //%player.bank.numericalDisplay = %this.aiBankDisplay;
        }
    }

    // clear unwanted players off the table
    for (%i = 0; %i < $NumberOfPlayers; %i++)
    {
        if (!isObject($PlayerArray[%i].seat))
        {
            $PlayerArray[%i].owner.setPositionX(sceneWindow2D.Extent.x + 5000);
            $PlayerArray[%i].owner.UseInputEvents = false;

            // Reset the numerical display for the player if it is ai controlled
            if ($PlayerArray[%i].isUserControlled == false)
            {
                $PlayerArray[%i].bank.numericalDisplay = null;
            }
        }
    }
    %this.emptyIcon.setPositionX(sceneWindow2D.Extent.x + 5000);

    $SeatSelectingPlayer = 0;
    %this.owner.UseInputEvents = true;
}

/// <summary>
/// Clear the seat of the current player.
/// </summary>
function TableSeatBehavior::clear(%this)
{
    if (isObject(%this.player))
    {
        // clear any cards on the table.
        %this.player.firstHand.reset();
        %this.player.secondHand.reset();

        // return any chips
        if (isObject(%this.player.seat.mainBetArea))
            %this.player.seat.mainBetArea.cancelBet(%this.player);
        if (isObject(%this.player.seat.sideBetArea))
            %this.player.seat.sideBetArea.cancelBet(%this.player);

        // clear the seat.
        %this.player.seat = null;
        %this.player = 0;
    }
}

/// <summary>
/// Sets the visibility of the seat and associated bet areas
/// </summary>
function TableSeatBehavior::setSeatVisibility(%this, %visibility)
{
    %this.owner.setVisible(%visibility);

    if (isObject(%this.mainBetArea))
        %this.mainBetArea.setVisibility(%visibility);

    if (isObject(%this.sideBetArea))
        %this.sideBetArea.setVisibility(%visibility);

    if (isObject(%this.aiBankDisplay))
        %this.aiBankDisplay.setVisible(%visibility);
}

/// <summary>
/// Handle touch-down event by allowing the user to select/clear the player
// seated in this seat.
/// </summary>
function TableSeatBehavior::onTouchDown(%this, %touchID, %worldPos)
{
    // Is somebody else showing a selection?
    if (isObject($SeatSelectingPlayer))
        return;

    if (isObject($CurrentTable))
    {
        // only add / change player at start of round
        if ($CurrentTable.state !$= "Ready")
        {
            $SeatSelectingPlayer = 0;
            return;
        }
    }

    // clear the seat
    %this.clear();
    $SeatSelectingPlayer = %this;

    // Check if player can only selects their seat.
    if (isObject($CurrentTable) && ($CurrentTable.canPlayerSelectAi == false))
    {
        if (%this.touchID !$= "")
        {
            $SeatSelectingPlayer = 0;
            return;
        }

        %this.touchID = %touchID;
    }
    else
    {
        %this.owner.UseInputEvents = false;

        // Display available players + user + empty.
        %pos = %this.owner.getPosition();
        %playerCount = %this.getAvailablePlayerCount();
        %pos.y -= ((%playerCount + 1) / 2) * %this.iconWidth;

        %this.emptyIcon.setPosition(%pos);
        %this.emptyIcon.UseInputEvents = true;
        for (%i = 0; %i < $NumberOfPlayers; %i++)
        {
            if (!isObject($PlayerArray[%i].seat))
            {
                %pos.y += %this.iconWidth;
                $PlayerArray[%i].owner.position = %pos;
                $PlayerArray[%i].owner.UseInputEvents = true;
            }
        }
    }
}

/// <summary>
/// Select player seat (if valid).
/// </summary>
function TableSeatBehavior::onTouchUp(%this, %touchID, %worldPos)
{
    if (%this.touchID $= %touchID)
    {
        %this.touchID = "";

        if (isObject($CurrentTable) && ($CurrentTable.canPlayerSelectAi == false) && isObject($UserControlledPlayer))
        {
            if (isObject($UserControlledPlayer.seat))
            {
                // remove from this seat.
                $UserControlledPlayer.seat.clear();
            }

            BlackjackPlayerCommon::selectPlayer($UserControlledPlayer, $SeatSelectingPlayer);
            clearUserControlledPlayerPayoutChipList();

            $CurrentTable.autoFillAi();
        }
    }
}

//-----------------------------------------------------------------------------
// Gui methods
//-----------------------------------------------------------------------------

/// <summary>
/// Handle selection of 'empty seat' icon.
/// </summary>
function emptySeatIcon::onClick(%this)
{
    // Nobody looking for use.
    if (!isObject($SeatSelectingPlayer))
        return;

    $SeatSelectingPlayer.setPlayer(null);
}

