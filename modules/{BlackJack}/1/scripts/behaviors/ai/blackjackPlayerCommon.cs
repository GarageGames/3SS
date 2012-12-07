//-----------------------------------------------------------------------------
// Torque Card Template
// Copyright © GarageGames, LLC.
//-----------------------------------------------------------------------------
// Common function for BlackjackAiPlayerBehavior and BlackjackUserPlayerBehavior.
//-----------------------------------------------------------------------------

// Time to wait for card to reach the player
$CardWaitingTimeInTicks = 800;

// Offsets for main hands
$HandOffsetX = -0.234275;
$HandOffsetY = -0.234275;

// Offsets for split hands
$SplitHandOffsetX = -0.234275;
$SplitHandOffsetY = 0.234275;

/// <return>true if all the elements are in place for a game, otherwise false.</return>
function BlackjackPlayerCommon::isGameValid(%player)
{
    if (!isObject($CurrentShoe))
    {
        error("$CurrentShoe doesn't exist!");
        return false;
    }

    return true;
}

/// <summary>
/// Reset for the start of the next round.
/// </summary>
function BlackjackPlayerCommon::reset(%player)
{
    %player.firstHand.reset();
    %player.secondHand.reset();
    %player.activeHandCount = 1;
    BlackjackPlayerCommon::setHand(%player, %player.firstHand);
    %this.cardWaitingActive = false;

    %player.start();
}


/// <summary>
/// Start waiting for a card?
/// </summary>
function BlackjackPlayerCommon::startCardWaiting(%this)
{
    %this.cardWaitingActive = true;
    %this.cardWaitingLastTime = getRealTime();
}

/// <summary>
/// Should we wait for a card?
/// </summary>
function BlackjackPlayerCommon::isCardWaiting(%this)
{
    if (%this.cardWaitingActive)
    {
        if ((getRealTime() - %this.cardWaitingLastTime) < $CardWaitingTimeInTicks)
            return true;
        else
            %this.cardWaitingActive = false;
    }

    return false;
}

/// <return>true if the player has at least one hand still in play.</return>
function BlackjackPlayerCommon::hasActiveHand(%player)
{
    %cardValue = %player.firstHand.getValue();
    if ((%cardValue > 0) && (%cardValue <= 21))
    {
        // Don't return true if a natural blackjack.
        if (!((%cardValue == 21) && (%player.firstHand.cardArrayCount == 2)))
            return true;
    }

    if (%player.activeHandCount > 1)
    {
        %cardValue = %player.secondHand.getValue();
        if ((%cardValue > 0) && (%cardValue <= 21))
        {
            // Don't return true if a natural blackjack.
            if (!((%cardValue == 21) && (%player.secondHand.cardArrayCount == 2)))
                return true;
        }
    }

    return false;
}

/// <return>true if the player can split their current hand.</return>
function BlackjackPlayerCommon::isSplittable(%player)
{
    if (%player.activeHandCount > 1)
        return false;

    // Check if the player has enough money to split
    %currentBet = %player.seat.mainBetArea.callOnBehaviors(getFieldValue, currentBet);
    %currentCash = %player.bank.currentCash;
    if (%currentBet > %currentCash)
        return false;

    return %player.hand.isSplittable();
}

/// <return>true if the player can double down on their current hand.</return>
function BlackjackPlayerCommon::canDoubleDown(%player)
{
    // only doubledown if we have two cards
    if (%player.hand.cardArrayCount != 2)
        return false;

    // check if table rules allow double at all
    if (!$CurrentTable.canDoubleDown)
        return false;

    // check if table rules allow a double after a split
    if ($CurrentTable && !($CurrentTable.canDoubleAfterSplit) && (%player.activeHandCount > 1))
        return false;

    // Get the bet area associated with the current hand
    %currentBetArea = %player.seat.mainBetArea;
    if (%player.hand == %player.secondHand)
        %currentBetArea = %currentBetArea.callOnBehaviors(getSplitBetAreaBehavior).owner;

    // Check if the player has enough money to double
    %currentBet = %currentBetArea.callOnBehaviors(getFieldValue, currentBet);
    %currentCash = %player.bank.currentCash;

    if (%currentBet > %currentCash)
        return false;

    return true;
}

/// <summary>
/// Split hand.
/// </summary>
/// <return>return true if split aces (which finishes the player's turn).</return>
function BlackjackPlayerCommon::splitHand(%player)
{
    if (%player.activeHandCount > 1)
        return;

    %player.secondHand.reset();
    %player.activeHandCount = 2;

    %player.secondHand.drawCardFromHand(%player.firstHand);
    %player.secondHand.drawCard();

    if (%player.secondHand.getCardFaceRawValue(0) $= "A")
    {
        // After splitting aces, the common rule is that only one card will be dealt to each ace;
        //the player cannot split, double, or take another hit on either hand.
        %player.firstHand.drawCard();

        BlackjackPlayerCommon::setHand(%player, %player.firstHand);

        // player is finished
        return true;
    }

    BlackjackPlayerCommon::setHand(%player, %player.secondHand);

    // player is not finished
    return false;
}

// Sets the current hand for the player and updates
// the hand's layers
function BlackjackPlayerCommon::setHand(%player, %hand)
{
    %player.hand = %hand;

    if (!isObject($CurrentShoe.playingCards))
        return;

    // Move the current hand to the front
    %player.hand.setHandLayer($CurrentShoe.playingCards.getSceneLayer());

    if (%player.activeHandCount > 1)
    {
        // Move the other hand back
        if (%player.hand == %player.secondHand)
            %player.firstHand.setHandLayer($CurrentShoe.playingCards.getSceneLayer() + 1);
        else
            %player.secondHand.setHandLayer($CurrentShoe.playingCards.getSceneLayer() + 1);
    }
}

/// <summary>
/// Place the selected player into a seat.
/// </summary>
/// <return>true if player was assigned to a seat, otherwise false</return>
function BlackjackPlayerCommon::selectPlayer(%player, %seat)
{
    // bad objects.
    if (!isObject(%seat) || !isObject(%player))
        return false;

    // Already assigned.
    if (isObject(%player.seat))
        return false;

    %seat.setPlayer(%player);
    return true;
}

/// <summary>
/// Check if the player is bankrupt
/// </summary>
/// <return>true if player is bankrupt, otherwise false</return>
function BlackjackPlayerCommon::isBankrupt(%player)
{
    %cash = %player.bank.currentCash;

    if (%cash <= 0)
        return true;
    else
        return false;
}