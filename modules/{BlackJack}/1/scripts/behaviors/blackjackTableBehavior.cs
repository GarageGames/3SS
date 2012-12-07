//-----------------------------------------------------------------------------
// Torque Card Template
// Copyright © GarageGames, LLC.
//-----------------------------------------------------------------------------
// Blackjack Table behavior.
// Sets up the game and manages updates.
//-----------------------------------------------------------------------------

/// The player currently associated with the PlayInput buttons.
$PlayInputPlayer = null;

$BottomBarBitmapFonts[0] = BottomBarAIBankDisplay0;
$BottomBarBitmapFonts[1] = BottomBarAIBankDisplay1;
$BottomBarAvatarImages[0] = BottomBarAIAvatarImage0;
$BottomBarAvatarImages[1] = BottomBarAIAvatarImage1;
$BottomBarBackground[0] = BottomBarAIBackground0;
$BottomBarBackground[1] = BottomBarAIBackground1;

// Y Offset for the current hand arrow.
$CurrentHandArrowOffsetY = 0.1953125;

// Create this behavior only if it does not already exist
if (!isObject(BlackjackTableBehavior))
{
    // Create this behavior from the blank BehaviorTemplate
    %template = new BehaviorTemplate(BlackjackTableBehavior);

    // friendlyName will be what is displayed in the editor
    %template.friendlyName = "Blackjack Table";

    // behaviorType organize this behavior in the editor
    %template.behaviorType = "Card Template";

    // description briefly explains what this behavior does
    %template.description  = "Manages the game of Blackjack.";

    //----------------------------------
    // Behavior Fields
    //----------------------------------

    %template.addBehaviorField(tableIndex, "Unique Table Index.", int, 1);

    // Rules
    %template.addBehaviorField(standOn, "Dealer stands on this value or greater.", int, 17);
    %template.addBehaviorField(hitOnSoft, "Does dealer hit on a soft value?", bool, false);
    %template.addBehaviorField(canSplit, "Allows the player to split.", bool, true);
    %template.addBehaviorField(canSplitTens, "Allows the player to split any 'ten' (T, J, Q, K).", bool, true);
    %template.addBehaviorField(canDoubleDown, "Allow the player to double-down.", bool, true);
    %template.addBehaviorField(canDoubleAfterSplit, "Allow the player to double-down after a split.", bool, true);
    %template.addBehaviorField(showDealerCardAfterBust, "Whether the face-down dealer card must be revealed after all players bust", bool, true);

    // Seat selection
    %template.addBehaviorField(canPlayerSelectAi, "Can the player select the AI (true), or is it auto-filled (false)?", bool, false);

    // UI elements
    %template.addBehaviorField(currentHandArrow, "Icon used to display the current hand.", object, "", SceneObject);
    %template.addBehaviorField(showHandValues, "Will show the numeric values of hands if true", bool, true);
    %template.addBehaviorField(handNumericalDisplay, "NumericalDisplayBehavior template to use to display the hand value.", object, "", BitmapFontObject);
    %template.addBehaviorField(winLabel, "Label for a winning hand.", object, "", SceneObject);
    %template.addBehaviorField(loseLabel, "Label for a losing hand.", object, "", SceneObject);
    %template.addBehaviorField(pushLabel, "Label for a push hand.", object, "", SceneObject);
    %template.addBehaviorField(bustLabel, "Label for a busted hand.", object, "", SceneObject);
    %template.addBehaviorField(blackjackLabel, "Label for a blackjack hand.", object, "", SceneObject);
    %template.addBehaviorField(showLabelTime, "Ammount of time (in seconds) to display win/lose/push labels on hand.", float, 3.0);
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function BlackjackTableBehavior::onBehaviorAdd(%this)
{
    if (isObject($CurrentTable))
        warn("$CurrentTable is already set before we added this one!");

    $CurrentTable = %this;
    $TableSeatCount = 0;
    %this.enterState("Ready");
    $LastUserBet = 0;
}

/// <summary>
/// Called when the level is loaded.
/// </summary>
function BlackjackTableBehavior::onLevelLoaded(%this)
{
    Canvas.pushDialog(TablePlayGui);

    /// Which round are we playing?
    $CurrentRound = 0;

    %this.enterState("Ready");
    %this.schedule(10, "setPlayInputButtonsVisiblity", false);
    %this.schedule(32, "updateLoop");

    %this.clearBottomBar();
}

/// <summary>
/// Called when the level has ended.
/// </summary>
function BlackjackTableBehavior::onLevelEnded(%this)
{
    Canvas.popDialog(TablePlayGui);

    // If we are still betting, return all bets
    if (%this.state $= "Ready")
    {
        %this.goToFirstValidSeat();
        while (isObject(%this.player))
        {
            //echo("Returning bets for player" SPC %this.player);
            %this.player.cancelAllBets();

            %this.player = %this.getNextPlayer();
        }
    }

    %this.enterState("Quit");

    %this.clearBottomBar();
}

/// <return>true if all the elements are in place for a game, otherwise false.</return>
function BlackjackTableBehavior::isValid(%this)
{
    if (!isObject($CurrentShoe))
    {
        error("$CurrentShoe doesn't exist!");
        return false;
    }

    if ($NumberOfPlayers <= 0)
    {
        error("No players!");
        return false;
    }

    if (!isObject($CurrentDealer))
    {
        error("No dealer!");
        return false;
    }

    return true;
}

/// <summary>
/// Update the position and visibility of the Current Hand Arrow.
/// </summary>
function BlackjackTableBehavior::updateCurrentHandArrow(%this, %show, %player)
{
    if (!isObject(%this.currentHandArrow))
        return;

    %this.currentHandArrow.setVisible(%show);

    if ((%show == true) && isObject(%player))
    {
        %pos = %player.hand.getCardPosition(0);
        %rotation = %player.hand.getCardRotation(0);

        %pos.x += -(%this.currentHandArrow.getWidth() * mCos(mDegToRad(%rotation))) + ($CurrentHandArrowOffsetY * mSin(mDegToRad(%rotation)));
        %pos.y += -(%this.currentHandArrow.getWidth() * mSin(mDegToRad(%rotation))) + ($CurrentHandArrowOffsetY * mCos(mDegToRad(%rotation)));
        %this.currentHandArrow.setPosition(%pos);
        %this.currentHandArrow.setAngle(%rotation);
    }
}

/// <summary>
/// Label hand as a winner.
/// </summary>
function BlackjackTableBehavior::labelWinner(%this, %pos, %hand)
{
    if (isObject(%this.winLabel))
    {
        %hand.showLabel(%this.winLabel.clone(), %pos, %this.showLabelTime);
    }
}

/// <summary>
/// Label hand as a loser.
/// </summary>
function BlackjackTableBehavior::labelLoser(%this, %pos, %hand)
{
    if (isObject(%this.loseLabel))
    {
        %hand.showLabel(%this.loseLabel.clone(), %pos, %this.showLabelTime);
    }
}

/// <summary>
/// Label hand as a push.
/// </summary>
function BlackjackTableBehavior::labelPush(%this, %pos, %hand)
{
    if (isObject(%this.pushLabel))
    {
        %hand.showLabel(%this.pushLabel.clone(), %pos, %this.showLabelTime);
    }
}

/// <summary>
/// Label hand as a bust.
/// </summary>
function BlackjackTableBehavior::labelBust(%this, %pos, %hand)
{
    if (isObject(%this.bustLabel))
    {
        %hand.showLabel(%this.bustLabel.clone(), %pos, %this.showLabelTime);
    }
}

/// <summary>
/// Label hand as a Blackjack.
/// </summary>
function BlackjackTableBehavior::labelBlackjack(%this, %pos, %hand)
{
    if (isObject(%this.blackjackLabel))
    {
        %hand.showLabel(%this.blackjackLabel.clone(), %pos, %this.showLabelTime);
    }
}

/// <return>The current number of 'valid' seats.</return>
function BlackjackTableBehavior::getValidSeatCount(%this)
{
    %count = 0;
    for (%i = 0; %i < $TableSeatCount; %i++)
    {
        if (%this.isSeatValid(%i))
            %count++;
    }

    return %count;
}

/// <summary>
/// Go to the first valid seat.
/// </summary>
function BlackjackTableBehavior::goToFirstValidSeat(%this)
{
    %this.currentSeatIndex = -1;
    %this.player = %this.getNextPlayer();
}

/// <summary>
/// Start play by dealing out a new round.
/// </summary>
function BlackjackTableBehavior::newDeal(%this)
{
    if (!%this.isValid() || (%this.state !$= "Ready") || (%this.getValidSeatCount() <= 0))
        return;

    %this.enterState("FinishBetting");
}

/// <summary>
/// Handle changing to a new state.
/// </summary>
/// <param "%nextState">State we're going to.</param>
function BlackjackTableBehavior::enterState(%this, %nextState)
{
    // clean up state we're exiting.
    switch$ (%this.state)
    {
    case "Ready":
        TablePlayGUI.setButtonVisibility(TablePlayDealButton, false);
        TablePlayGUI.setButtonVisibility(TablePlayRebetButton, false);

    case "UpdateEndOfGame":
        // Handle bankruptcy for user-controlled player
        if (isObject($UserControlledPlayer) && BlackjackPlayerCommon::isBankrupt($UserControlledPlayer))
        {
            $UserControlledPlayer.bank.showBankruptLabel();
            alxPlay($Save::GeneralSettings::Sound::Bankrupt);
        }
    }

    //echo ("***State Change:" SPC %this.state SPC "-->" SPC %nextState);
    %this.state = %nextState;

    // set up state we're entering
    switch$ (%this.state)
    {
    case "Ready":
        %this.checkForBrokePlayers();
        %this.updateCurrentHandArrow(false, 0);

    case "FinishBetting":
        TablePlayGUI.setButtonVisibility(TablePlayDealButton, false);

        TablePlayGUI.setButtonVisibility(TablePlayRebetButton, false);

    case "OpeningCards":
        %this.updateCurrentHandArrow(false, 0);
        %this.setPlayInputButtonsVisiblity(false);

        // Save the users bet
        $LastUserBet = $UserControlledPlayer.seat.mainBetArea.callOnBehaviors("getFieldValue", currentBet);
        $CurrentDealer.start();

        %this.goToFirstValidSeat();
        %this.stateValue = 0;

        $CurrentDealer.lastTime = getRealTime();

    case "UpdateOfferInsurence":
        %this.goToFirstValidSeat();

    case "UpdatePlay":
        %this.goToFirstValidSeat();
        %this.player.start();

    case "UpdateEndOfGame":
        %this.setPlayInputButtonsVisiblity(false);
        %this.goToFirstValidSeat();
        %this.player.start();
        if (%this.player.activeHandCount > 1)
        {
            //%this.player.hand = %this.player.secondHand;
            BlackjackPlayerCommon::setHand(%this.player, %this.player.secondHand);
            %this.updateCurrentHandArrow(true, %this.player);
        }

    case "CleanUpHands":
        %this.resetHandsHandle = %this.schedule(2000, "resetHands");
    }
}

/// <summary>
/// Update game loop.
/// </summary>
function BlackjackTableBehavior::updateLoop(%this)
{
    switch$ (%this.state)
    {
    case "Ready":
        // User can add/remove players to seats and handle bets.
        TablePlayGUI.setButtonVisibility(TablePlayDealButton, false);
        TablePlayGUI.setButtonVisibility(TablePlayRebetButton, false);

        if (isObject($UserControlledPlayer.seat))
        {
            %currentUserBet = $UserControlledPlayer.seat.mainBetArea.callOnBehaviors("getFieldValue", currentBet);
            if (%currentUserBet >= $CurrentBetRules.minBet)
                TablePlayGUI.setButtonVisibility(TablePlayDealButton, true);
            else if ($LastUserBet > 0 && %currentUserBet == 0 && $UserControlledPlayer.bank.currentCash >= $LastUserBet)
                TablePlayGUI.setButtonVisibility(TablePlayRebetButton, true);
        }

    case "FinishBetting":
        // AI finishes placing bets.
        %haveAllPlayerBet = %this.updatePlaceBets();
        if (%haveAllPlayerBet)
        {
            %this.enterState("OpeningCards");
        }

    case "OpeningCards":
        // Lay out the openning cards.
        if ($CurrentDealer.updateOpening(%this.player, %this.stateValue))
        {
            %this.player = %this.getNextPlayer();
            if ($CurrentDealer.hand.cardArrayCount >= (%this.stateValue + 1))
            {
                %this.goToFirstValidSeat();
                %this.stateValue++;
                if (%this.stateValue > 1)
                {
                    %this.enterState("UpdateSideBets");
                }
            }
        }

    case "UpdateSideBets":
        $CurrentDealer.updateSideBets(%this.player);
        %this.player = %this.getNextPlayer();
        if (%this.player == 0)
        {
            %this.enterState("UpdateOfferInsurence");
        }

    case "UpdateOfferInsurence":
        // Future: Handle insurence
        %this.enterState("UpdateDealerBlackjack");

    case "UpdateDealerBlackjack":
        if ($CurrentDealer.updateDealerBlackjack())
        {
            // jump right to end-of-game.
            %this.enterState("UpdateEndOfGame");
        }
        else
        {
            %this.enterState("UpdatePlay");
        }

    case "UpdatePlay":
        // Players Hit/Double/Split/or Stand.
        %this.setPlayInputButtonsVisiblity(true, %this.player);
        %isPlayerFinished = $CurrentDealer.updatePlay(%this.player);
        if (%isPlayerFinished)
        {
            // is the player playing the first of multiple hands?
            if (isObject(%this.player) && (%this.player.hand != %this.player.firstHand))
            {
                //echo("Player" SPC %this.player SPC "finishes first hand.");
                %this.player.start();
                //%this.player.hand = %this.player.firstHand;
                BlackjackPlayerCommon::setHand(%this.player, %this.player.firstHand);
                while (%this.player.hand.cardArrayCount < 2)
                {
                    %this.player.hand.drawCard(false);
                }

                %this.updateCurrentHandArrow(true, %this.player);
            }
            else
            {
                %this.player = %this.getNextPlayer();
                if (%this.player == 0)
                {
                    // check if any player hand is left to check against the dealer.
                    for (%i = 0; %i < $TableSeatCount; %i++)
                    {
                        if (%this.isSeatValid(%i))
                        {
                            //echo("**********" @ $TableSeatArray[%i].player);
                            %player = $TableSeatArray[%i].player;
                            if (%player.hasActiveHand())
                            {
                                $CurrentDealer.hand.showAllCards();
                                %this.enterState("UpdateDealer");
                                %this.setPlayInputButtonsVisiblity(false);

                                %this.schedule(32, "updateLoop");
                                return;
                            }

                            // Show the dealers card even if no active hands are left
                            // if showDealerCardAfterBust is true
                            if (%this.showDealerCardAfterBust)
                            {
                                $CurrentDealer.hand.showAllCards();
                            }
                        }
                    }

                    // no active hand, skip to the end
                    %this.enterState("UpdateEndOfGame");
                }
                else
                {
                    %this.player.start();
                }
            }
        }

        if (isObject(%this.player))
            %this.updateCurrentHandArrow(true, %this.player);

    case "UpdateDealer":
        %isPlayerFinished = $CurrentDealer.updateDealer();
        if (%isPlayerFinished)
        {
            %this.enterState("UpdateEndOfGame");
        }

    case "UpdateEndOfGame":
        //echo("UpdateEndOfGame for player:" SPC %this.player);
        %isPlayerFinished = $CurrentDealer.updateEndOfGame(%this.player);
        if (%isPlayerFinished)
        {
            // is the player playing the first of multiple hands?
            if (isObject(%this.player) && (%this.player.hand != %this.player.firstHand))
            {
                //echo("UpdateEndOfGame for player:" SPC %this.player SPC "switching from second to first hand.");
                %this.player.start();
                BlackjackPlayerCommon::setHand(%this.player, %this.player.firstHand);
                %this.updateCurrentHandArrow(true, %this.player);
            }
            else
            {
                %this.player = %this.getNextPlayer();
                if (%this.player == 0)
                {
                    %this.enterState("CleanUpHands");
                }
                else
                {
                    if (%this.player.activeHandCount > 1)
                    {
                        //%this.player.hand = %this.player.secondHand;
                        BlackjackPlayerCommon::setHand(%this.player, %this.player.secondHand);
                        %this.updateCurrentHandArrow(true, %this.player);
                    }
                }
            }
        }

    case "CleanUpHands":
        // waiting for cleanup...

    case "Quit":
        //echo("BlackjackTableBehavior::updateLoop() - Exiting...");
        return;

    default:
        error("The state isn't valid:" SPC %this.state);
    }

    %this.schedule(32, "updateLoop");
}

// Resets the dealer's hand and resets all players
function BlackjackTableBehavior::resetHands(%this)
{
    // Reset the dealer hand
    $CurrentDealer.hand.reset();

    // Reset all players
    %this.goToFirstValidSeat();
    while (%this.player != 0)
    {
        %this.player.reset();
        %this.player = %this.getNextPlayer();
    }

    %this.enterState("Ready");
}

/// <summary>
/// AI controlled players place bets or leave the table.
/// </summary>
function BlackjackTableBehavior::updatePlaceBets(%this)
{
    %this.goToFirstValidSeat();
    while (isObject(%this.player))
    {
        if (%this.player.isUserControlled == false)
        {
            %bet = %this.player.getBet();
            if (%bet <= 0)
            {
                %this.player.seat.setPlayer(null);
                %this.autoFillAi();
            }
            else
            {
                // Place bet
                %betArea = %this.player.seat.mainBetArea;
                %playerBankBehavior = %this.player.bank;

                if (isObject(%betArea) && isObject(%playerBankBehavior) && isObject($CurrentBetRules))
                {
                    %bet = mClamp(%bet, $CurrentBetRules.minBet, $CurrentBetRules.maxBet);
                    %bet = mClamp(%bet, $CurrentBetRules.minBet, %playerBankBehavior.currentCash);

                    %betArea.callOnBehaviors(addBet, %bet, %playerBankBehavior);
                    %betArea.callOnBehaviors(update);
                }
            }
        }

        %this.player = %this.getNextPlayer();
    }

    return true;
}

/// <summary>
/// Players without money are removed from the table.
/// </summary>
function BlackjackTableBehavior::checkForBrokePlayers(%this)
{
    %this.goToFirstValidSeat();
    while (isObject(%this.player))
    {
        if (%this.player.isUserControlled == false)
        {
            if (%this.player.bank.currentCash < $CurrentBetRules.minBet)
            {
                // Leave table
                %this.player.seat.setPlayer(null);
                //%this.autoFillAi();
            }
        }

        %this.player = %this.getNextPlayer();
    }
}

/// <return>
/// true if the seat at the index given has been set, otherwise false.
/// </return>
function BlackjackTableBehavior::isSeatValid(%this, %index)
{
    if (%index >= $TableSeatCount)
        return false;

    if (!isObject($TableSeatArray[%index]))
        return false;

    return ($TableSeatArray[%index].player == 0) ? false : true;
}

/// <summary>
/// Set the table seat at %index to the %object.
/// </summary>
function BlackjackTableBehavior::setSeat(%this, %object, %index)
{
    $TableSeatArray[%index] = %object;
    if (%index >= $TableSeatCount)
        $TableSeatCount = (%index + 1);
}

/// <summary>
/// Fill the empty seats with available AI players
/// </summary>
function BlackjackTableBehavior::autoFillAi(%this)
{
    //%aiPlacedCount = 0;
    for (%i = 0; %i < $TableSeatCount; %i++)
    {
        if (isObject($TableSeatArray[%i]) && !isObject($TableSeatArray[%i].player) && $TableSeatArray[%i].owner.getVisible())
        {
            %rndStart = getRandom(0, ($NumberOfPlayers - 1));
            %j = %rndStart;
            do
            {
                if (isObject($PlayerArray[%j]) && (!isObject($PlayerArray[%j].seat))
                        && $PlayerArray[%j].getIsAvailable()
                        && $PlayerArray[%j].getIsAvailableOn(%this.tableIndex)
                        && ($PlayerArray[%j].bank.currentCash >= $CurrentBetRules.minBet))
                {
                    if (BlackjackPlayerCommon::selectPlayer($PlayerArray[%j], $TableSeatArray[%i]))
                    {
                        // Set the bottom bar
                        //$BottomBarAvatarImages[%aiPlacedCount].setImageMap($PlayerArray[%j].owner.getImageMap());
                        //$BottomBarBitmapFonts[%aiPlacedCount];

                        //%aiPlacedCount++;

                        break;
                    }
                }

                %j++;
                if (%j >= $NumberOfPlayers)
                    %j = 0;
            } while (%j != %rndStart)
        }
    }

    %this.updateBottomBar();
}

/// <summary>
/// Updates the bottom bar with the currently seated AIs
/// </summary>
function BlackjackTableBehavior::updateBottomBar(%this)
{
    %aiPlacedCount = 0;
    for (%i = 0; %i < $TableSeatCount; %i++)
    {
        if (isObject($TableSeatArray[%i].player))
        {
            if ($TableSeatArray[%i].player != $UserControlledPlayer)
            {
                $BottomBarBackground[%aiPlacedCount].setImageMap(BottomBarAIBackgroundTemplate.getImageMap());
                $BottomBarAvatarImages[%aiPlacedCount].setImageMap($TableSeatArray[%i].player.owner.getImageMap(), $TableSeatArray[%i].player.owner.getFrame());
                $TableSeatArray[%i].player.bank.numericalDisplay = $BottomBarBitmapFonts[%aiPlacedCount];

                $BottomBarBackground[%aiPlacedCount].setVisible(true);
                $BottomBarAvatarImages[%aiPlacedCount].setVisible(true);
                $BottomBarBitmapFonts[%aiPlacedCount].setVisible(true);

                %aiPlacedCount++;
            }
        }
    }
}

/// <summary>
/// Clears AI elements on bottom bar
/// </summary>
function BlackjackTableBehavior::clearBottomBar(%this)
{
    $BottomBarAvatarImages[0].setVisible(false);
    $BottomBarAvatarImages[1].setVisible(false);
    $BottomBarBitmapFonts[0].setVisible(false);
    $BottomBarBitmapFonts[1].setVisible(false);
    $BottomBarBackground[0].setVisible(false);
    $BottomBarBackground[1].setVisible(false);
}


/// <return>
/// The next valid player at the table, or 0 if no more valid players.
/// </return>
function BlackjackTableBehavior::getNextPlayer(%this)
{
    %this.currentSeatIndex++;
    if (%this.currentSeatIndex < 0)
        %this.currentSeatIndex = 0;

    if (%this.currentSeatIndex >= $TableSeatCount)
    {
        %this.updateCurrentHandArrow(false, 0);

        return 0;
    }

    if (%this.isSeatValid(%this.currentSeatIndex))
    {
        %player = $TableSeatArray[%this.currentSeatIndex].player;
        %player.start();

        return %player;
    }

    return %this.getNextPlayer();
}

/// <return>
/// Set the visiblity of player input buttons.
/// </return>
function BlackjackTableBehavior::setPlayInputButtonsVisiblity(%this, %show, %player)
{
    if ((%show = false) || !isObject(%player) || (%player.isUserControlled == false))
    {
        $PlayInputPlayer = null;

        TablePlayGUI.setButtonVisibility(TablePlayHitButton, false);

        TablePlayGUI.setButtonVisibility(TablePlayStandButton, false);

        TablePlayGUI.setButtonVisibility(TablePlaySplitButton, false);

        TablePlayGUI.setButtonVisibility(TablePlayDoubleButton, false);
    }
    else
    {
        $PlayInputPlayer = %player;

        TablePlayGUI.setButtonVisibility(TablePlayHitButton, true);

        TablePlayGUI.setButtonVisibility(TablePlayStandButton, true);

        if (%player.isSplittable())
        {
            TablePlayGUI.setButtonVisibility(TablePlaySplitButton, true);
        }
        else
        {
            TablePlayGUI.setButtonVisibility(TablePlaySplitButton, false);
        }

        if (%player.canDoubleDown())
        {
            TablePlayGUI.setButtonVisibility(TablePlayDoubleButton, true);
        }
        else
        {
            TablePlayGUI.setButtonVisibility(TablePlayDoubleButton, false);
        }
    }
}

//-----------------------------------------------------------------------------
// Gui methods
//-----------------------------------------------------------------------------

/// <summary>
/// Exit table button
/// </summary>
function BlackjackTableBehavior::exitButtonClicked(%this)
{
    alxPlay($Save::GeneralSettings::Sound::ButtonClick);
    sceneWindow2D.schedule(200, "loadLevel", "^BlackjackTemplate/data/levels/startLevel.scene.taml");
}

/// <summary>
/// Deal button
/// </summary>
function howtoButtonClicked(%this)
{
    alxPlay($Save::GeneralSettings::Sound::ButtonClick);
    HelpScreenBackground.bitmap = $Save::Interface::HelpScreens[$CurrentHelpScreen];

    if ($Save::Interface::NumberOfHelpScreens <= 1)
        helpScreenNextButton.setVisible(false);

    Canvas.pushDialog(HelpScreenGui);
}

/// <summary>
/// Credits button
/// </summary>
function creditsButtonClicked(%this)
{
    alxPlay($Save::GeneralSettings::Sound::ButtonClick);
    Canvas.pushDialog(creditsGui);
}

/// <summary>
/// Clear $UserControlledPlayer payout chips.
/// </summary>
function clearUserControlledPlayerPayoutChipList()
{
    for (%i = 0; %i < getWordCount($UserPayoutChipList); %i++)
    {
        %chipToBeDeleted = getWord($UserPayoutChipList, %i);
        if (isObject(%chipToBeDeleted))
        {
            if (isEventPending(%this.updateHandle))
                cancel(%this.updateHandle);
                
            %chipToBeDeleted.safeDelete();
        }
    }
    $UserPayoutChipList = "";
}

/// <summary>
/// Deal button
/// </summary>
function BlackjackTableBehavior::dealButtonClicked(%this)
{
    if (!isObject($CurrentTable))
    {
        error("No Table!");
        return;
    }

    alxPlay($Save::GeneralSettings::Sound::ButtonClick);
    clearUserControlledPlayerPayoutChipList();
    
    $CurrentTable.newDeal();
}

/// <summary>
/// Rebet button
/// </summary>
function BlackjackTableBehavior::rebetButtonClicked(%this)
{
    alxPlay($Save::GeneralSettings::Sound::ButtonClick);
    clearUserControlledPlayerPayoutChipList();

    %mainBetAreaObject = $UserControlledPlayer.seat.mainBetArea;

    %playerBankBehavior = $UserControlledPlayer.bank;

    %mainBetAreaObject.callOnBehaviors(addBet, $lastUserBet, %playerBankBehavior);
    %mainBetAreaObject.callOnBehaviors(update);
}

/// <summary>
/// Hit button
/// </summary>
function BlackjackTableBehavior::hitButtonClicked(%this)
{
    if (isObject($PlayInputPlayer))
    {
        alxPlay($Save::GeneralSettings::Sound::ButtonClick);

        $PlayInputPlayer.action = "Hit";
    }
}

/// <summary>
/// Split button
/// </summary>
function BlackjackTableBehavior::splitButtonClicked(%this)
{
    if (isObject($PlayInputPlayer))
    {
        alxPlay($Save::GeneralSettings::Sound::ButtonClick);

        $PlayInputPlayer.action = "Split";
    }
}

/// <summary>
/// Stand button
/// </summary>
function BlackjackTableBehavior::standButtonClicked(%this)
{
    if (isObject($PlayInputPlayer))
    {
        alxPlay($Save::GeneralSettings::Sound::ButtonClick);

        $PlayInputPlayer.action = "Stand";
    }
}

/// <summary>
/// Double button
/// </summary>
function BlackjackTableBehavior::doubleDownButtonClicked(%this)
{
    if (isObject($PlayInputPlayer))
    {
        alxPlay($Save::GeneralSettings::Sound::ButtonClick);

        $PlayInputPlayer.action = "Double";
    }
}

