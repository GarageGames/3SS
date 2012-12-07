//-----------------------------------------------------------------------------
// Torque Card Template
// Copyright © GarageGames, LLC.
//-----------------------------------------------------------------------------
// Behavior for a Blackjack player AI.
//-----------------------------------------------------------------------------
// Requires: BlackjackHandBehavior
//-----------------------------------------------------------------------------

/// <summary>
/// Create this behavior only if it does not already exist
/// </summary>
if (!isObject(BlackjackAiPlayerBehavior))
{
    // Create this behavior from the blank BehaviorTemplate
    %template = new BehaviorTemplate(BlackjackAiPlayerBehavior);

    // friendlyName will be what is displayed in the editor
    %template.friendlyName = "Blackjack AI Player";

    // behaviorType organize this behavior in the editor
    %template.behaviorType = "Blackjack AI";

    // description briefly explains what this behavior does
    %template.description  = "Create an AI that plays Blackjack.";

    %template.addBehaviorField(startingCash, "Starting cash.", float, 1000.0);
    %template.addBehaviorField(aiStrategy, "AI Strategy.", int, 1);
    %template.addBehaviorField(reactionTime, "Time between actions (in seconds).", int, 1);

    %template.addBehaviorField(isAvailable, "Is the AI available in game?", bool, true);
    %template.addBehaviorField(isAvailableOn1, "Is available on table #1?", bool, true);
    %template.addBehaviorField(isAvailableOn2, "Is available on table #2?", bool, true);
    %template.addBehaviorField(isAvailableOn3, "Is available on table #3?", bool, true);
    %template.addBehaviorField(isAvailableOn4, "Is available on table #4?", bool, true);
    %template.addBehaviorField(isAvailableOn5, "Is available on table #5?", bool, true);
}

//------------------------------------------------------------------------------
// Editor Accessors
//------------------------------------------------------------------------------

///<return>Get value.</return>
function BlackjackAiPlayerBehavior::getStrategy(%this)
{
    return %this.aiStrategy;
}

///<param="%value">Value to set target to.</param>
function BlackjackAiPlayerBehavior::setStrategy(%this, %value)
{
    %this.aiStrategy = %value;
}

///<return>Get value.</return>
function BlackjackAiPlayerBehavior::getReactionTime(%this)
{
    return %this.reactionTime;
}

///<param="%value">Value to set target to.</param>
function BlackjackAiPlayerBehavior::setReactionTime(%this, %value)
{
    %this.reactionTime = %value;
}

///<return>Get value.</return>
function BlackjackAiPlayerBehavior::getCash(%this)
{
    return %this.startingCash;
}

///<param="%value">Value to set target to.</param>
function BlackjackAiPlayerBehavior::setCash(%this, %value)
{
    %this.startingCash = %value;
}

///<param="%value">Index of target value.</param>
///<return>Get value.</return>
function BlackjackAiPlayerBehavior::getIsAvailableOn(%this, %index)
{
    switch (%index)
    {
    case 1:
        return %this.isAvailableOn1;
    case 2:
        return %this.isAvailableOn2;
    case 3:
        return %this.isAvailableOn3;
    case 4:
        return %this.isAvailableOn4;
    case 5:
        return %this.isAvailableOn5;
    }

    return false;
}

///<param="%value">Index of target value.</param>
///<param="%value">Value to set target to.</param>
function BlackjackAiPlayerBehavior::setIsAvailableOn(%this, %index, %value)
{
    switch (%index)
    {
    case 1:
        %this.isAvailableOn1 = %value;
    case 2:
        %this.isAvailableOn2 = %value;
    case 3:
        %this.isAvailableOn3 = %value;
    case 4:
        %this.isAvailableOn4 = %value;
    case 5:
        %this.isAvailableOn5 = %value;
    }
}

///<return>Get value.</return>
function BlackjackAiPlayerBehavior::getIsAvailable(%this)
{
    return %this.isAvailable;
}

///<param="%value">Value to set target to.</param>
function BlackjackAiPlayerBehavior::setIsAvailable(%this, %value)
{
    %this.isAvailable = %value;
}

//------------------------------------------------------------------------------

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function BlackjackAiPlayerBehavior::onBehaviorAdd(%this)
{
    // seat this player is at (null if not seated).
    %this.seat = null;

    // not active in ANY part of the game.
    if (!%this.isAvailable)
        return;

    %this.bank = PlayerBankBehavior.createInstance();
    %this.bank.startingCash = %this.startingCash;
    %this.bank.init();

    %this.isUserControlled = false;

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
}

/// <summary>
/// Called when behavior is removed from object.
/// </summary>
function BlackjackAiPlayerBehavior::onBehaviorRemove(%this)
{
    if (isObject(%this.bank))
        %this.bank.delete();
}

/// <summary>
/// Reset their value to start add them to a seat number.
/// </summary>
//return: True if all the elements are in place for a game, otherwise false.
function BlackjackAiPlayerBehavior::addToSeat(%this, %seatNumber)
{
    // reset stats
    %this.wins = 0;
    %this.lost = 0;
    %this.wonLastHand = false;
    %this.lastBet = 0;
    %this.cardWaitTime = 0;
}

/// <return>true if all the elements are in place for a game, otherwise false.</return>
function BlackjackAiPlayerBehavior::isGameValid(%this)
{
    return BlackjackPlayerCommon::isGameValid(%this);
}

/// <summary>
/// Reset for the start of the next round.
/// </summary>
function BlackjackAiPlayerBehavior::reset(%this)
{
    BlackjackPlayerCommon::reset(%this);
}

/// <summary>
/// Reset for the start of the next round.
/// </summary>
function BlackjackAiPlayerBehavior::start(%this)
{
    %this.isActive = true;
    %this.lastTime = getRealTime();
}

/// <summary>
/// Start waiting for a card?
/// </summary>
function BlackjackAiPlayerBehavior::startCardWaiting(%this)
{
    BlackjackPlayerCommon::startCardWaiting(%this);
}

/// <summary>
/// Should we wait for a card?
/// </summary>
function BlackjackAiPlayerBehavior::isCardWaiting(%this)
{
    return BlackjackPlayerCommon::isCardWaiting(%this);
}

/// <param="%index">Strategy index we want the name for.</param>
/// <return>Strategy name (empty if bad index).</return>
function BlackjackAiPlayerBehavior::getStrategyNameForIndex(%this, %index)
{
    %count = getFieldCount($StrategyGrid::PlayStyles);
    if ((%index >= 0) && (%index < %count))
    {
        return getField($StrategyGrid::PlayStyles, %index);
    }

    return "";
}

/// <summary>
/// Calculate a bet and return it.
/// </summary>
/// <return>Amount of money to bet, negitive to leave the table.</return>
function BlackjackAiPlayerBehavior::getBet(%this)
{
    %cnt = $CurrentRound % $StrategyGrid::BettingCount[%this.aiStrategy];
    %this.lastBet = $CurrentBetRules.minBet * $StrategyGrid::Betting[%this.aiStrategy, %cnt];

    return %this.lastBet;
}

/// <summary>
/// Cancel all bets for this player
/// </summary>
function BlackjackAiPlayerBehavior::cancelAllBets(%this)
{
    %this.seat.mainBetArea.cancelBet(%this);

    if (isObject(%this.seat.sideBetArea))
        %this.seat.sideBetArea.cancelBet(%this);
}

/// <summary>
/// Perform a single update depending on the cards current held.
/// </summary>
/// <return>action we wish to preform.</return>
function BlackjackAiPlayerBehavior::update(%this)
{
    // reaction time
    if ((getRealTime() - %this.lastTime) < (%this.reactionTime * 1000))
    {
        return "Wait";
    }
    %this.lastTime = getRealTime();

    if (!%this.isGameValid())
        return "Stand";

    %handValue = %this.hand.getValue();
    if (%handValue >= 21)
        return "Stand";

    %dealerUpCardValue = $CurrentDealer.hand.getCardFaceValue(0);

    if (%this.isSplittable())
        return $StrategyGrid::Pairs[%this.aiStrategy, %this.hand.getCardFaceValue(0), %dealerUpCardValue];

    if (%this.hand.hasSoftValue())
        return $StrategyGrid::Soft[%this.aiStrategy, %handValue, %dealerUpCardValue];

    return $StrategyGrid::Hard[%this.aiStrategy, %handValue, %dealerUpCardValue];
}

/// <return>true if the player has at least one hand still in play.</return>
function BlackjackAiPlayerBehavior::hasActiveHand(%this)
{
    return BlackjackPlayerCommon::hasActiveHand(%this);
}

/// <return>true if the player can split their current hand.</return>
function BlackjackAiPlayerBehavior::isSplittable(%this)
{
    return BlackjackPlayerCommon::isSplittable(%this);
}

/// <return>true if the player can double down on their current hand.</return>
function BlackjackAiPlayerBehavior::canDoubleDown(%this)
{
    return BlackjackPlayerCommon::canDoubleDown(%this);
}

/// <summary>
/// Split hand.
/// </summary>
/// <return>return true if split aces (which finishes the player's turn).</return>
function BlackjackAiPlayerBehavior::splitHand(%this)
{
    return BlackjackPlayerCommon::splitHand(%this);
}

/// <summary>
/// Handle touch-down event.
/// </summary>
function BlackjackAiPlayerBehavior::onTouchDown(%this, %touchID, %worldPos)
{
    if (%this.touchID !$= "")
        return;

    %this.touchID = %touchID;
}

/// <summary>
/// Handle touch-up event by assigning us to a seat (if we're available).
/// </summary>
function BlackjackAiPlayerBehavior::onTouchUp(%this, %touchID, %worldPos)
{
    // check if we are the selected icon
    if (%this.touchID !$= %touchID)
        return;

    %this.touchID = "";

    BlackjackPlayerCommon::selectPlayer(%this, $SeatSelectingPlayer);
}
