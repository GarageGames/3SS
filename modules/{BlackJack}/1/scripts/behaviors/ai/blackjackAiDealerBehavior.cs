//-----------------------------------------------------------------------------
// Torque Card Template
// Copyright © GarageGames, LLC.
//-----------------------------------------------------------------------------
// Dealer handles the game logic for Standard Blackjack game.
//-----------------------------------------------------------------------------
// Requires: BlackjackHandBehavior
//-----------------------------------------------------------------------------

/// <summary>
/// Create the BlackjackAiDealerBehavior only if it does not already exist
/// </summary>
if (!isObject(BlackjackAiDealerBehavior))
{
    // Create this behavior from the blank BehaviorTemplate
    %template = new BehaviorTemplate(BlackjackAiDealerBehavior);

    // friendlyName will be what is displayed in the editor
    %template.friendlyName = "Blackjack Dealer";

    // behaviorType organize this behavior in the editor
    %template.behaviorType = "Blackjack AI";

    // description briefly explains what this behavior does
    %template.description  = "Create an AI that deals a game of Blackjack.";

    //----------------------------------
    // Behavior Fields
    //----------------------------------

    %template.addBehaviorField(reactionTime, "Time between actions (in seconds).", float, 0.5);
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function BlackjackAiDealerBehavior::onBehaviorAdd(%this)
{
    if (isObject($CurrentDealer))
        warn("$CurrentDealer is already set before we added this one!");

    $CurrentDealer = %this;

    %this.hand = new BehaviorTemplate(BlackjackHandBehavior);
    %this.hand.init(%this.owner, "0 0");
}

/// <return>true if all the elements are in place for a game, otherwise false.</return>
function BlackjackAiDealerBehavior::isGameValid(%this)
{
    if (!isObject($CurrentTable))
    {
        error("$CurrentTable doesn't exist!");
        return false;
    }

    return $CurrentTable.isValid();
}

/// <summary>
/// Set up a new shoe, then schedule the start of a new round.
/// </summary>
function BlackjackAiDealerBehavior::newShoe(%this)
{
    if (!%this.isGameValid())
    {
        %this.abortGame();
        return;
    }

    // set up the shoe
    $CurrentShoe.init();
    $CurrentShoe.shuffle();
}

//-----------------------------------------------------------------------------
// Handle updates
//-----------------------------------------------------------------------------

/// <summary>
/// Start a new round.
/// </summary>
function BlackjackAiDealerBehavior::start(%this)
{
    if (!%this.isGameValid())
    {
        %this.abortGame();
        return;
    }

    $CurrentRound++;
    //echo("START OF ROUND" SPC $CurrentRound);

    // reset card hands
    %this.hand.reset();
    $CurrentTable.currentSeatIndex = -1;
    for (%player = $CurrentTable.getNextPlayer(); %player != 0; %player = $CurrentTable.getNextPlayer())
    {
        %player.reset();
    }

    // Check the shoe.
    if ($CurrentShoe.hasReachedPenetration)
    {
        //echo("Start a new shoe");
        %this.newShoe();
    }
}

/// <summary>
/// Handle bets.
/// </summary>
/// <return>return true if player is finished</return>
function BlackjackAiDealerBehavior::updateBets(%this, %player)
{
    return true;
}

/// <summary>
/// The opening deal, deal the first two cards.
/// </summary>
/// <return>return true if player is finished</return>
function BlackjackAiDealerBehavior::updateOpening(%this, %player, %cardNum)
{
    // let the shoe shuffle
    if ($CurrentShoe.isShuffling)
        return false;

    // reaction time
    %realtime = getRealTime();
    if ((%realtime - %this.lastTime) < (%this.reactionTime * 1000))
    {
        return false;
    }
    %this.lastTime = getRealTime();

    if (%player > 0)
    {
        %player.hand.drawCard(false);

        // check blackjack
        if ((%player.hand.cardArrayCount == 2) && (%player.hand.getValue() == 21))
            %player.hand.hasBlackjack = true;
        else
            %player.hand.hasBlackjack = false;

        return true;
    }

    // dealer's turn (first card down).
    if (%cardNum == 0)
        %this.hand.drawCard(true);
    else
        %this.hand.drawCard(false);

    return true;
}

/// <summary>
/// Update side bet placement.
/// </summary>
function BlackjackAiDealerBehavior::updateSideBets(%this, %player)
{
    // Get the player's side bet area behavior
    %sideBetAreaObject = %player.seat.sideBetArea;

    if (!isObject(%sideBetAreaObject))
    {
        //warn("No SideBetArea found for the player. Unable to update side bets.");
        return;
    }

    // Update the player's side bet
    %sideBetAreaObject.callOnBehaviors(sideBetPayout, %player, %player.hand.echoCards());
}

/// <summary>
/// Update offer insurence to player.
/// </summary>
function BlackjackAiDealerBehavior::updateOfferInsurence(%this, %player)
{
    // ++ Insurence?
}

/// <summary>
/// Update check for Dealer Blackjack.
/// </summary>
/// <return>return true if dealer has blackjack.</return>
function BlackjackAiDealerBehavior::updateDealerBlackjack(%this)
{
    // dealer has 'Natural Blackjack'?
    if ((%this.hand.cardArrayCount == 2) && (%this.hand.getValue() == 21))
    {
        %this.hand.hasBlackjack = true;
        
        //%this.dealerScore = 21;
        %this.hand.showAllCards();
        $CurrentTable.labelBlackjack(%this.hand.getLabelPosition(), %this.hand);

        $CurrentTable.currentSeatIndex = -1;
        for (%player = $CurrentTable.getNextPlayer(); %player != 0; %player = $CurrentTable.getNextPlayer())
        {
            //-------------------------
            // Pay off insured players
            //-------------------------

        }

        return true;
    }
    
    %this.hand.hasBlackjack = false;
    return false;
}

/// <summary>
/// Check to see if the player has busted (gone over 21).
/// </summary>
/// <return>true if the player has busted.</return>
function BlackjackAiDealerBehavior::checkPlayerBust(%this, %player, %currentBetAreaBehavior)
{
    %score = %player.hand.getValue();

    if (%score > 21)
    {
        $CurrentTable.labelBust(%player.hand.getLabelPosition(), %player.hand);
        %currentBetAreaBehavior.loseBet(%player);
        return true;
    }

    return false;
}

/// <summary>
/// Check for player "natural blackjack" or 21. Handle payout on blackjack.
/// </summary>
/// <param name="player">Player being updated.</param>
/// <return>return true if player has a "natural blackjack" or "21"</return>
function BlackjackAiDealerBehavior::checkNaturalBlackJack(%this, %player, %currentBetAreaBehavior)
{
    // check  21
    %score = %player.hand.getValue();
    if (%score == 21)
    {
        if (%player.hand.hasBlackjack)
        {
            // player has 'natural blackjack'
            $CurrentTable.labelBlackjack(%player.hand.getLabelPosition(), %player.hand);

            // Get blackjack payout
            %currentBetAreaBehavior.collectPayout($CurrentBetRules.payoutBlackjack, %player);
        }

        return true;
    }
    return false;
}

/// <summary>
/// Update the players playing.
/// </summary>
/// <param name="player">Player being updated.</param>
/// <return>return true if player is finished</return>
function BlackjackAiDealerBehavior::updatePlay(%this, %player)
{
    // check if we're still waiting for the next card to arrive...
    if (%player.isCardWaiting())
        return false;

    %score = %player.hand.getValue();
    %this.lastTime = getRealTime();

    // Get the main bet area behavior
    %mainBetAreaObject = %player.seat.mainBetArea;
    %splitBetAreaObject = %mainBetAreaObject.callOnBehaviors(getSplitBetAreaBehavior).owner;

    // Determine which bet area to use based on which hand we are playing
    if (%player.hand == %player.firstHand)
        %currentBetAreaObject = %mainBetAreaObject;
    else
        %currentBetAreaObject = %splitBetAreaObject;

    if (%this.checkNaturalBlackJack(%player, %currentBetAreaObject))
        return true;

    if (%this.checkPlayerBust(%player, %currentBetAreaObject))
        return true;

    %action = %player.update();

    switch$ (%action)
    {
    case "Wait":
        // player thinking...
        return false;

    case "Stand":
        //echo("Player Stands");
        return true;

    case "Hit":
        %player.hand.drawCard(false);
        //echo("Player Hits to:" SPC %player.hand.echoCards());
        %player.startCardWaiting();
        return false;

    case "Double":
        // Update the bet
        %mainBetAreaObject = %player.seat.mainBetArea;
        if (!isObject(%mainBetAreaObject))
        {
            warn("Could not find main bet area for seat");
            return;
        }
        %splitBetAreaBehavior = %mainBetAreaObject.callOnBehaviors(getSplitBetAreaBehavior);

        // Determine which bet area to use based on which hand we are playing
        if (%player.hand == %player.firstHand)
            %currentBetAreaObject = %mainBetAreaObject;
        else
            %currentBetAreaObject = %splitBetAreaBehavior.owner;

        %currentBetAreaObject.callOnBehaviors(doubleDown, %player);

        %player.hand.drawCard(false);
        %this.checkPlayerBust(%player, %currentBetAreaObject);
        return true;

    case "Split":
        // Update the bet for a split
        %player.seat.mainBetArea.callOnBehaviors(split, %player);
        return %player.splitHand();

    default:
        //echo("Player does something crazy->" SPC %action SPC "<-!!!!!!!");
    }

    return true;
}

/// <summary>
/// Dealer updates.
/// </summary>
function BlackjackAiDealerBehavior::updateDealer(%this)
{
    // let the shoe shuffle
    if ($CurrentShoe.isShuffling)
        return false;

    // reaction time
    %realtime = getRealTime();
    if ((%realtime - %this.lastTime) < (%this.reactionTime * 1000))
    {
        return false;
    }
    %this.lastTime = getRealTime();

    // dealer plays
    //%this.dealerScore = %this.hand.getValue();
    if ((%this.hand.getValue() < $CurrentTable.standOn)
            || ($CurrentTable.hitOnSoft && %this.hand.hasSoftValue() && (%this.hand.getValue() == $CurrentTable.standOn)))
    {
        %this.hand.drawCard(false);
        return false;
    }

    if (%this.hand.getValue() > 21)
    {
        $CurrentTable.labelBust(%this.hand.getLabelPosition(), %this.hand);
    }

    //echo("Dealer final score:" SPC %this.dealerScore);
    return true;
}

/// <summary>
/// Find the winners and losers.
/// </summary>
/// <param name="player">Player being updated.</param>
/// <return>return true if player is finished</return>
function BlackjackAiDealerBehavior::updateEndOfGame(%this, %player)
{
    // Get the main bet area behavior
    %mainBetAreaObject = %player.seat.mainBetArea;
    %splitBetAreaObject = %mainBetAreaObject.callOnBehaviors(getSplitBetAreaBehavior).owner;

    // Determine which bet area to use based on which hand we are playing
    if (%player.hand == %player.firstHand)
        %currentBetAreaObject = %mainBetAreaObject;
    else
        %currentBetAreaObject = %splitBetAreaObject;
        
    %playerScore = %player.hand.getValue();

    // If player has blackjack
    if (%player.hand.hasBlackjack == true)
    {
        if (%this.hand.hasBlackjack == true)
        {
            //echo("Player " SPC %i SPC "PUSHES on dealer BlackJack");
            $CurrentTable.labelPush(%player.hand.getLabelPosition(), %player.hand);
            %currentBetAreaObject.callOnBehaviors(collectPayout, $CurrentBetRules.payoutPush, %player);
        }
        // else - we already handled BlackJack payout above.

        return true;
    }

    // check for zero because a busted hand may already have been cleared.
    if ((%playerScore > 21) || (%playerScore <= 0))
    {
        //echo("Player " SPC %i SPC "BUSTS!");
        //%currentBetAreaBehavior.loseBet(%player);
    }
    else
    {
        if (%this.hand.getValue() > 21)
        {
            //echo("Player " SPC %i SPC "WINS because dealer busted!!!");
            $CurrentTable.labelWinner(%player.hand.getLabelPosition(), %player.hand);
            %currentBetAreaObject.callOnBehaviors(collectPayout, $CurrentBetRules.payoutWin, %player);
        }
        else
        {
            if (%this.hand.getValue() < %playerScore)
            {
                //echo("Player " SPC %i SPC "WINS!!!");
                $CurrentTable.labelWinner(%player.hand.getLabelPosition(), %player.hand);
                %currentBetAreaObject.callOnBehaviors(collectPayout, $CurrentBetRules.payoutWin, %player);
            }
            else
            {
                if (%this.hand.getValue() == %playerScore)
                {
                    //echo("Player " SPC %i SPC "PUSHES");
                    $CurrentTable.labelPush(%player.hand.getLabelPosition(), %player.hand);
                    %currentBetAreaObject.callOnBehaviors(collectPayout, $CurrentBetRules.payoutPush, %player);
                }
                else
                {
                    //echo("Player " SPC %i SPC "Loses");
                    $CurrentTable.labelLoser(%player.hand.getLabelPosition(), %player.hand);
                    %currentBetAreaObject.callOnBehaviors(loseBet, %player);
                }
            }
        }
    }
    return true;
}

/// <summary>
/// For some reason, the game is no longer playable.
/// We need to abort the game and return to the main menu.
/// </summary>
function BlackjackAiDealerBehavior::abortGame(%this)
{
    error("****NEED TO ABORT***");
}
