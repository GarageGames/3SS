//-----------------------------------------------------------------------------
// Torque Card Template
// Copyright © GarageGames, LLC.
//-----------------------------------------------------------------------------
// Behavior for the a controlled Blackjack player.
//-----------------------------------------------------------------------------
// Requires: BlackjackHandBehavior
//-----------------------------------------------------------------------------

/// <summary>
/// Create this behavior only if it does not already exist
/// </summary>
if (!isObject(BlackjackUserPlayerBehavior))
{
    // Create this behavior from the blank BehaviorTemplate
    %template = new BehaviorTemplate(BlackjackUserPlayerBehavior);

    // friendlyName will be what is displayed in the editor
    %template.friendlyName = "User Controlled Blackjack Player";

    // behaviorType organize this behavior in the editor
    %template.behaviorType = "Blackjack AI";

    // description briefly explains what this behavior does
    %template.description  = "User controlled Blackjack Player.";

    %template.addBehaviorField(bankDisplay, "NumericalDisplayBehavior obect to use to display the bank cash amount.", object, "", BitmapFontObject);
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function BlackjackUserPlayerBehavior::onBehaviorAdd(%this)
{
    // seat this player is at (null if not seated).
    %this.seat = null;

    %this.bank = PlayerBankBehavior.createInstance();
    %this.bank.numericalDisplay = %this.bankDisplay;
    %this.bank.init();

    %this.isUserControlled = true;
    %this.action = "Wait";

    // Add to PlayerArray
    $PlayerArray[$NumberOfPlayers] = %this;
    %this.playerIndex = $NumberOfPlayers;
    $NumberOfPlayers++;

    // primary hand
    %this.firstHand = new BehaviorTemplate(BlackjackHandBehavior);
    %this.firstHand.init(%this.owner, $HandOffsetX SPC $HandOffsetY);

    // secondary hand (used for splits)
    %this.secondHand = new BehaviorTemplate(BlackjackHandBehavior);
    %this.secondHand.init(%this.owner, $SplitHandOffsetX SPC $SplitHandOffsetY);

    // number of active hands
    %this.activeHandCount = 1;
    BlackjackPlayerCommon::setHand(%this, %this.firstHand);

    //echo ("****** " @ $UserControlledPlayer @ " --- " @ %this);
    if (isObject($UserControlledPlayer))
        warn("There is more than one player being assigned to $UserControlledPlayer (only the last one will be used)!");

    // This assumes there is only one user controlled player
    $UserControlledPlayer = %this;
}

/// <summary>
/// Called when behavior is removed from object.
/// </summary>
function BlackjackUserPlayerBehavior::onBehaviorRemove(%this)
{
    %this.bank.delete();

    if ($UserControlledPlayer == %this)
        $UserControlledPlayer = 0;
}

/// <summary>
/// Called when a level has loaded
/// </summary>
function BlackjackUserPlayerBehavior::onLevelLoaded(%this)
{
    %this.bank.numericalDisplay = PlayerBankDisplay;

    // sync bank with player profile
    %this.bank.loadPlayerCash($DefaultPlayerProfile);
}

/// <summary>
/// Called when a level has ended
/// </summary>
function BlackjackUserPlayerBehavior::onLevelEnded(%this)
{
    // sync bank with player profile
    %this.bank.savePlayerCash($DefaultPlayerProfile);
}

/// <summary>
/// Reset their value to start add them to a seat number.
/// </summary>
function BlackjackUserPlayerBehavior::addToSeat(%this, %seatNumber)
{
}

/// <return>true if all the elements are in place for a game, otherwise false.</return>
function BlackjackUserPlayerBehavior::isGameValid(%this)
{
    return BlackjackPlayerCommon::isGameValid(%this);
}

/// <summary>
/// Reset for the start of the next round.
/// </summary>
function BlackjackUserPlayerBehavior::reset(%this)
{
    BlackjackPlayerCommon::reset(%this);
}

/// <summary>
/// Start waiting for a card?
/// </summary>
function BlackjackUserPlayerBehavior::startCardWaiting(%this)
{
    BlackjackPlayerCommon::startCardWaiting(%this);
}

/// <summary>
/// Should we wait for a card?
/// </summary>
function BlackjackUserPlayerBehavior::isCardWaiting(%this)
{
    return BlackjackPlayerCommon::isCardWaiting(%this);
}

/// <summary>
/// Cancel all bets for this player
/// </summary>
function BlackjackUserPlayerBehavior::cancelAllBets(%this)
{
    if (isObject(%this.seat.mainBetArea))
        %this.seat.mainBetArea.cancelBet(%this);
    if (isObject(%this.seat.sideBetArea))
        %this.seat.sideBetArea.cancelBet(%this);
}

/// <summary>
/// Activate player for the start of their turn.
/// </summary>
function BlackjackUserPlayerBehavior::start(%this)
{
    %this.isActive = true;
    %this.action = "Wait";
}

/// <summary>
/// Perform a single update action.
/// </summary>
/// <return>action we wish to preform.</return>
function BlackjackUserPlayerBehavior::update(%this)
{
    %act = %this.action;
    %this.action = "Wait";
    return %act;
}

/// <return>true if the player has at least one hand still in play.</return>
function BlackjackUserPlayerBehavior::hasActiveHand(%this)
{
    return BlackjackPlayerCommon::hasActiveHand(%this);
}

/// <return>true if the player can split their current hand.</return>
function BlackjackUserPlayerBehavior::isSplittable(%this)
{
    return BlackjackPlayerCommon::isSplittable(%this);
}

/// <return>true if the player can double down on their current hand.</return>
function BlackjackUserPlayerBehavior::canDoubleDown(%this)
{
    return BlackjackPlayerCommon::canDoubleDown(%this);
}

/// <summary>
/// Split hand.
/// </summary>
/// <return>return true if split aces (which finishes the player's turn).</return>
function BlackjackUserPlayerBehavior::splitHand(%this)
{
    return BlackjackPlayerCommon::splitHand(%this);
}

/// <summary>
/// Handle touch-down event.
/// </summary>
function BlackjackUserPlayerBehavior::onTouchDown(%this, %touchID, %worldPos)
{
    if (%this.touchID !$= "")
        return;

    %this.touchID = %touchID;
}

/// <summary>
/// Handle touch-up event by assigning us to a seat (if we're available).
/// </summary>
function BlackjackUserPlayerBehavior::onTouchUp(%this, %touchID, %worldPos)
{
    // check if we are the selected icon
    if (%this.touchID !$= %touchID)
        return;

    %this.touchID = "";

    BlackjackPlayerCommon::selectPlayer(%this, $SeatSelectingPlayer);
}