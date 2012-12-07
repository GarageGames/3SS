//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

//file: betAreaBehavior.cs

//globalvariable: TestBankCash
$TestBankCash = 1000;

//globalvariable: SideBetHorizontalOffset
//Spacing used to place side bet in absence of a splitBetTarget object
$SideBetHorizontalOffset = 1.171875;

/// Stores list of chips objects being animated for the player payout.
$UserPayoutChipList = "";

// Create this behavior only if it does not already exist
if (!isObject(BetAreaBehavior))
{
    // Create this behavior from the blank BehaviorTemplate
    // Name it TouchDrag
    %template = new BehaviorTemplate(BetAreaBehavior);

    // friendlyName will be what is displayed in the editor
    // behaviorType organize this behavior in the editor
    // description briefly explains what this behavior does
    %template.friendlyName = "Bet Area";
    %template.behaviorType = "Betting Component";
    %template.description  = "Defines a bet area on the table with a set of rules for payouts (should only be set for trigger objects)";

    %template.addBehaviorField(userEditable, "Determines whether bet editing is enabled by default for this bet area", bool, true);

    %template.addBehaviorField(chipThickness, "Vertical spacing of the chip stack", float, 0.04);
    %template.addBehaviorField(chipMoveSpeedOn, "The speed at which chips move onto the bet area", int, 250);
    %template.addBehaviorField(chipMoveSpeedOff, "The speed at which chips move off the bet area", int, 250);
    %template.addBehaviorField(splitBetArea, "Bet area for the split bet chip stack", object, "", SceneObject);
    %template.addBehaviorField(numericalDisplay, "NumericalDisplayBehavior obect to use to display the bet amount.", object, "", BitmapFontObject);

    %template.addBehaviorField(playerTargetObject, "The object that player chips are targeted toward", object, "", SceneObject);
    %template.addBehaviorField(dealerTargetObject, "The object that dealer chips are targeted toward", object, "", SceneObject);

    %template.addBehaviorField(animatePayout, "Determines whether chips animate for payouts", bool, true);
    %template.addBehaviorField(startDelayPayout, "Time in seconds before the payout stack animates when you win or push", float, 1.0);
    %template.addBehaviorField(chipDelayPayout, "Time in seconds between each chip payout animation when you win or push", float, 0.1);

    %template.addBehaviorField(animateLose, "Determines whether chips animate for losses", bool, true);
    %template.addBehaviorField(startDelayLose, "Time in seconds before the payout stack animates when you lose", float, 0.0);
    %template.addBehaviorField(chipDelayLose, "Time in seconds between each chip payout animation when you lose", float, 0.0);

    %template.addBehaviorField(animateCancel, "Determines whether chips animate when the player cancels the bet", bool, true);
    %template.addBehaviorField(startDelayCancel, "Time in seconds before the payout stack animates when you cancel the bet", float, 0.0);
    %template.addBehaviorField(chipDelayCancel, "Time in seconds between each chip payout animation when you cancel the bet", float, 0.0);

    %template.addBehaviorField(chipStackJitter, "Horizontal spacing noise in the chip stack", float, 0.0);
}


/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function BetAreaBehavior::onBehaviorAdd(%this)
{
    %this.touchID = "";
    %this.currentBet = 0;
    %this.betChipStackList = "";
    %this.isEditingBet = %this.userEditable;
//   %this.splitBetAreaObject = "";

    %this.owner.UseInputEvents = true;
}

/// <summary>
/// Called when the behavior is added to the scene.
/// </summary>
/// <param name="scene">The scene object the behavior was added to.</param>
function BetAreaBehavior::onAddToScene(%this, %scene)
{
    %this.scene = %scene;

    // Call update on the behavior to refresh the display
    %this.update();
}

/// <summary>
/// Returns the BetAreaBehavior associated with the splitBetArea object of this behavior.
/// </summary>
function BetAreaBehavior::getSplitBetAreaBehavior(%this)
{
    if (!isObject(%this.splitBetArea))
    {
        warn("Could not find splitBetArea object in BetAreaBehavior::getSplitBetAreaBehavior");
        return "";
    }

    %behavior = %this.splitBetArea.getBehavior("BetAreaBehavior");

    if (!isObject(%behavior))
    {
        warn("Could not find splitBetArea behavior in BetAreaBehavior::getSplitBetAreaBehavior");
        return "";
    }

    return %behavior;
}

/// <summary>
/// Enables/disables editing the bet for the bet area
/// </summary>
/// <param name="isEditingFlag">bool used to set the isEditingBet state.</param>
function BetAreaBehavior::setEditingBet(%this, %isEditingFlag)
{
    %this.isEditingBet = %isEditingFlag;
}

/// <summary>
/// Returns a boolean indicating if the the bet amount is legal
/// </summary>
/// <param name="betAmount">The value to increment the bet by.</param>
/// <return>true if bet is legal, false otherwise.</return>
function BetAreaBehavior::isBetLegal(%this, %betAmount)
{
    // Check if the currentBet + betAmount is legal according to the bet rules
    if (isObject($CurrentBetRules))
    {
        %totalBet = %this.currentBet + %betAmount;
        if (%totalBet > $CurrentBetRules.maxBet)
            return false;
    }

    return true;
}

/// <summary>
/// Attempts to increment the current bet amount if
/// the bet is legal according to the table rules.
/// </summary>
/// <param name="betAmount">The value to increment the bet by.</param>
/// <return>true if bet was added successfully, false otherwise.</return>
function BetAreaBehavior::addBet(%this, %betAmount, %playerBankBehavior)
{
    // Check if bets can be edited
    if (!%this.isEditingBet)
        return false;

    // Make sure the player has enough money to make the bet
    if (%betAmount > %playerBankBehavior.currentCash)
        return false;
        
    // clear out "payout" as needed.
	if ($UserPayoutChipList !$= "")
		clearUserControlledPlayerPayoutChipList();

    //  Subtract the bet amount from the bank total
    %playerBankBehavior.currentCash -= %betAmount;

    // Add the bet amount to the current bet
    %this.currentBet += %betAmount;

    alxPlay($Save::GeneralSettings::Sound::ChipSelect);


    // Return true to indicate that the addBet was successful
    return true;
}

/// <summary>
/// Attempts to increment the current bet amount if
/// the bet is legal according to the table rules and includes
/// a chip movement animation.
/// </summary>
/// <param name="%bankStackBehavior">The bank behavior to add the bet from.</param>
/// <return>true if bet was added successfully, false otherwise.</return>
function BetAreaBehavior::addBetWithAnimation(%this, %bankStackBehavior, %playerBankBehavior)
{
    %betAmount = %bankStackBehavior.denomination;

    // Check if bets can be edited
    if (!%this.isEditingBet)
        return false;

    // Make sure the player has enough money to make the bet
    if (%betAmount > %playerBankBehavior.currentCash)
        return false;

    // Add the bet
    %this.addBet(%betAmount, %playerBankBehavior);

    // Clone the chip without behaviors
    %clone = %bankStackBehavior.owner.clone();

    // Create MoveChipEffectBehavior
    %moveChipEffectBehavior = MoveChipEffectBehavior.createInstance();

    // Add the behavior to the cloned object
    // and set some variables for the cloned object
    %clone.addBehavior(%moveChipEffectBehavior);
    %clone.betAreaBehavior = %this;
    %clone.setBodyType("dynamic");
    %clone.setCollisionSuppress(true);
    %x = getWord(%bankStackBehavior.owner.position, 0);
    %y = getWord(%bankStackBehavior.owner.position, 1);
    %clone.setPosition(%x, %y);
    %clone.moveTo(%this.owner.getPosition(), %this.chipMoveSpeedOn, true);

    return true;
}

/// <summary>
/// Doubles the current bet if the player has enough money
/// </summary>
/// <param name="player">The player that is receiving the payout</param>
/// <return>true if bet was doubled successfully, false otherwise.</return>
function BetAreaBehavior::doubleDown(%this, %player)
{
    // Get the player bank behavior
    %playerBankBehavior = %player.bank;

    // Check if behavior exists
    if (!isObject(%playerBankBehavior))
    {
        warn("Could not find bank behavior for player. Unable to doubleDown.");
        return false;
    }

    // Set the new bet amount equal to the current bet
    %betAmount = %this.currentBet;

    // Make sure the player has enough money to make the bet
    if (%betAmount > %playerBankBehavior.currentCash)
        return false;

    //  Subtract the bet amount from the bank total
    %playerBankBehavior.currentCash -= %betAmount;

    // Add the bet amount to the current bet
    %this.currentBet += %betAmount;

    // Call update on the behavior to refresh the display
    %this.update();

    return true;
}

/// <summary>
/// Split on the hand if the player has enough money
/// </summary>
/// <param name="player">The player that is splitting</param>
/// <return>true if bet was split successfully, false otherwise.</return>
function BetAreaBehavior::split(%this, %player)
{
    // Get the player bank behavior
    %playerBankBehavior = %player.bank;

    // Check if behavior exists
    if (!isObject(%playerBankBehavior))
    {
        warn("Could not find bank behavior for player. Unable to split.");
        return;
    }

    // Set the new bet amount equal to the current bet
    %betAmount = %this.currentBet;

    // Make sure the player has enough money to make the bet
    if (%betAmount > %playerBankBehavior.currentCash)
        return false;

    //  Subtract the bet amount from the bank total
    %playerBankBehavior.currentCash -= %betAmount;

    // Add the new bet amount
    %this.getSplitBetAreaBehavior().currentBet = %betAmount;


    // Update the displays of both bet areas
    %this.getSplitBetAreaBehavior().update();
    %this.update();

    return true;
}

/// <summary>
/// Collect a payout with the given payout ratio
/// <param name="payoutRatio">float giving the ratio for the payout (X:1).</param>
/// </summary>
/// <param name="player">The player that is receiving the payout</param>
function BetAreaBehavior::collectPayout(%this, %payoutRatio, %player)
{
    if (%payoutRatio > 0)
    {
        if (%player == $UserControlledPlayer)
            alxPlay($Save::GeneralSettings::Sound::ChipStacking);

        %player.wins++;
        %player.wonLastHand = true;
    }

    %isAnimated = %this.animatePayout;

    // Do not animate if the player is ai-controlled
    if (!%player.isUserControlled)
        %isAnimated = false;

    // Get the player bank object
    %playerBankObject = %player.bank;

    // Check if object exists
    if (!isObject(%playerBankObject))
    {
        warn("Could not find bank object for player. Unable to collectPayout.");
        return;
    }

    // Get the player bank behavior
    %playerBankBehavior = %playerBankObject;

    // Check if behavior exists
    if (!isObject(%playerBankBehavior))
    {
        warn("Could not find bank behavior for player. Unable to collectPayout.");
        return;
    }

    %this.payout(%payoutRatio, %playerBankBehavior, %isAnimated, %this.playerTargetObject.position, %this.startDelayPayout, %this.chipDelayPayout);
}

/// <summary>
/// Dealer takes the bet chips
/// </summary>
/// <param name="playerBankBehavior">The player bank behavior the payout is connected to.</param>
function BetAreaBehavior::loseBet(%this, %player)
{
    // Calculate the payout ratio given the bet rulesa
    if (isObject($CurrentBetRules))
    {
        %payoutRatio = $CurrentBetRules.payoutLose;
    }
    else
    {
        warn("No bet rules behavior found in loseBet.");
        %payoutRatio = -1;
    }

    %player.lost++;
    %player.wonLastHand = false;

    %isAnimated = %this.animateLose;

    // Do not animate if the player is ai-controlled
    if (!%player.isUserControlled)
        %isAnimated = false;

    // Get the player bank object
    %playerBankObject = %player.bank;

    // Check if object exists
    if (!isObject(%playerBankObject))
    {
        warn("Could not find bank object for player. Unable to finish loseBet.");
        return;
    }

    // Get the player bank behavior
    %playerBankBehavior = %playerBankObject;

    // Check if behavior exists
    if (!isObject(%playerBankBehavior))
    {
        warn("Could not find bank behavior for player. Unable to finish loseBet.");
        return;
    }

    // Play Lose sound if it is the user-controlled player
    if (%player == $UserControlledPlayer)
        alxPlay($Save::GeneralSettings::Sound::LostHand);

    %this.payout(%payoutRatio, %playerBankBehavior, %isAnimated, %this.dealerTargetObject.position, %this.startDelayLose, %this.chipDelayLose);
}

/// <summary>
/// Cancels the current bet and returns it to the player bank
/// </summary>
/// <param name="player">The player the payout is connected to.</param>
function BetAreaBehavior::cancelBet(%this, %player)
{
    if (%this.currentBet == 0)
        return;

    %payoutRatio = 0;

    %isAnimated = %this.animateCancel;

    // Do not animate if the player is ai-controlled
    if (!%player.isUserControlled)
        %isAnimated = false;

    // Get the player bank object
    %playerBankObject = %player.bank;

    // Check if object exists
    if (!isObject(%playerBankObject))
    {
        warn("Could not find bank object for player. Unable to cancelBet.");
        return;
    }

    // Get the player bank behavior
    %playerBankBehavior = %playerBankObject;

    // Check if behavior exists
    if (!isObject(%playerBankBehavior))
    {
        warn("Could not find bank behavior for player. Unable to cancelBet.");
        return;
    }

    alxPlay($Save::GeneralSettings::Sound::ChipSelect);
    %this.payout(%payoutRatio, %playerBankBehavior, %isAnimated, %this.playerTargetObject.position, %this.startDelayCancel, %this.chipDelayCancel);
}

/// <summary>
/// Checks the cards with a sideBetPayoutBehavior and
/// either removes chips to the dealer or pays out to the player
/// </summary>
/// <param name="sideBetPayoutBehavior">Handle the a SideBetPayoutBehavior that specifies the payout conditions.</param>
/// <param name="player">The player the payout is connected to.</param>
/// <param name="cards">List of cards to check with the payout conditions</param>
function BetAreaBehavior::sideBetPayout(%this, %player, %cards)
{
    if (!isObject($CurrentSideBetPayouts))
    {
        warn("No SideBetPayoutBehavior found in the scene. Unable do sideBetPayout");
        return;
    }

    // Get the payout ratio for the given cards
    %payoutRatio = $CurrentSideBetPayouts.getPayout(%cards);

    // If the payout ratio is -1, we lost the bet
    // otherwise we won the bet
    if (%payoutRatio == -1)
        %this.loseBet(%player);
    else if (%payoutRatio >= 0)
        %this.collectPayout(%payoutRatio, %player);
    else
        warn("Bad payout ratio in BetAreaBehavior::sideBetPayout!");
}

/// <summary>
/// Checks the insurance condition and
/// either removes chips to the dealer or pays out to the player
/// </summary>
/// <param name="player">The player the payout is connected to.</param>
/// <param name="dealerCards">List of cards to check with the payout conditions</param>
function BetAreaBehavior::insurancePayout(%this, %player, %dealerCards)
{
    if (!isObject($CurrentInsurancePayouts))
    {
        warn("No InsurancePayoutBehavior found in the scene. Unable do insurancePayout");
        return;
    }

    // Get the payout ratio for the given cards
    %payoutRatio = $CurrentInsurancePayouts.getPayout(%dealerCards);

    // If the payout ratio is -1, we lost the bet
    // otherwise we won the bet
    if (%payoutRatio == -1)
        %this.loseBet(%player);
    else if (%payoutRatio >= 0)
        %this.collectPayout(%payoutRatio, %player);
    else
        warn("Bad payout ratio in BetAreaBehavior::insurancePayout!");
}

/// <summary>
/// Resolves the bet with the given payout ratio.
/// May include chip animation effect.
/// Payout ratios of -1 can be used if the player lost their bet.
/// </summary>
/// <param name="payoutRatio">float giving the ratio for the payout (X:1).</param>
/// <param name="playerBankBehavior">The player bank behavior the payout is connected to.</param>
/// <param name="isAnimated">bool that determines whether to animate the payout.</param>
/// <param name="animationTargetPosition">World position that the chips move to.</param>
/// <param name="startDelay">Time in msec before starting the payout animation.</param>
/// <param name="chipDelay">Time in msec between each chip animation.</param>
function BetAreaBehavior::payout(%this, %payoutRatio, %playerBankBehavior, %isAnimated, %animationTargetPosition, %startDelay, %chipDelay)
{
    // Convert seconds to milliseconds
    %startDelay = %startDelay * 1000;
    %chipDelay = %chipDelay * 1000;

    // Calculate the payout
    %payout = %this.currentBet * %payoutRatio;

    // Add the payout to the original bet
    %totalValueReturned = %this.currentBet + %payout;

    if (%isAnimated)
    {
        // If the totalReturn value is zero, we've lost
        // so create a stack the size of the current bet
        // otherwise, create a stack the size of our current bet
        // plus winnings
        if (%totalValueReturned == 0)
            %tempChipStackSize = %this.currentBet;
        else
            %tempChipStackSize = %totalValueReturned;

        // Create a new betChipStackList with the winnings/losses
        %this.createChipStack(%tempChipStackSize, %this.owner.position);

        // Create a payoutChipStack copied from the betChipStackList
        for (%i = getWordCount(%this.betChipStackList) - 1; %i >= 0; %i--)
        {
            %objectToClone = getWord(%this.betChipStackList, %i);
            %clone = %objectToClone.clone();

            // Create MoveChipEffectBehavior
            %moveChipEffectBehavior = MoveChipEffectBehavior.createInstance();

            // Add the behavior to the cloned object
            // and set some variables for the cloned object
            %clone.addBehavior(%moveChipEffectBehavior);
            %clone.betAreaBehavior = %this;
            %clone.setBodyType("dynamic");
            %clone.usesPhysics = true;
            %clone.position = %objectToClone.position;

            %clone.updateHandle = %clone.schedule(%startDelay + (%i * %chipDelay), "moveTo", %animationTargetPosition, %this.chipMoveSpeedOff, true, true);
            
            $UserPayoutChipList = %clone SPC $UserPayoutChipList;
        }
    }

    // Add the total the to bank
    %playerBankBehavior.currentCash += %totalValueReturned;

    // Reset the bet
    %this.currentBet = 0;

    // Call update on the behavior to refresh the display
    %this.update();
}

/// <summary>
/// Updates the bet display gui to match the current bet amount.
/// </summary>
function BetAreaBehavior::update(%this)
{
    // Create a stack of chips representing the current bet
    // using only the chips in the backStackList
    %this.createChipStack(%this.currentBet, %this.owner.position);

    // Update the numerical display if it exists
    if (isObject(%this.numericalDisplay))
    {
        if (%this.currentBet == 0)
            %this.numericalDisplay.setText("");
        else
            %this.numericalDisplay.setText("$" @ %this.currentBet);
    }
}

/// <summary>
/// Sets the visibility of the bet area and its associated numerical displays
/// </summary>
/// <param name="visibility">Boolean that determines visibility setting.</param>
function BetAreaBehavior::setVisibility(%this, %visibility)
{
    %this.owner.setVisible(%visibility);

    if (isObject(%this.splitBetArea))
        %this.splitBetArea.setVisibility(%visibility);

    if (isObject(%this.numericalDisplay))
        %this.numericalDisplay.setVisible(%visibility);
}

/// <summary>
/// Delete existing chips in the chipStackList
/// </summary>
function BetAreaBehavior::deleteChipStack(%this)
{
    for (%i = 0; %i < getWordCount(%this.betChipStackList); %i++)
    {
        %chipToBeDeleted = getWord(%this.betChipStackList, %i);
        %chipToBeDeleted.safeDelete();
    }
    %this.betChipStackList = "";
}

/// <summary>
/// Creates a stack of chips based on
/// the denominations in bankStackList.
/// </summary>
/// <param name="amount">Amount of money to make the chip stack for.</param>
/// <param name="bankStackList">Ordered list of all objects with BankStackBehavior</param>
/// <param name="basePosition">The position of the base of the chip stack.</param>
function BetAreaBehavior::createChipStack(%this, %amount, %basePosition)
{
    // Get a list of all objects with the BankStackBehavior
    %bankStackList = %this.getAllBankStackObjects();

    // Delete existing chips in the chipStackList
    %this.deleteChipStack();

    // Iterate through each denomination, and create chips for each
    for (%i = getWordCount(%bankStackList) - 1; %i >= 0; %i--)
    {
        %currentObject = getWord(%bankStackList, %i);
        %currentDenomination = %currentObject.callOnBehaviors(getFieldValue, denomination);
        %remainder = %amount % %currentDenomination;
        %numChips = (%amount - %remainder) / %currentDenomination;
        %numChips = mFloor(%numChips);
        %amount = %remainder;

        %this.createChips(%currentObject, %numChips, %basePosition);
    }
}

/// <summary>
/// Creates a stack of chips of the same denomination by cloning a specified object.
/// </summary>
/// <param name="objectToClone">The BankStackBehavior object to clone.</param>
/// <param name="count">The number of copies to make.</param>
/// <param name="basePosition">The position of the base of the chip stack.</param>
function BetAreaBehavior::createChips(%this, %objectToClone, %count, %basePosition)
{
    //echo("numChips of denomination " @ %currentDenomination @ " = " @ %numChips);

    for (%i = 0; %i < %count; %i++)
    {
        // Get the current chip stack size
        %chipCount = getWordCount(%this.betChipStackList);

        // Clone a new chip object
        %clone = %objectToClone.clone(); //REDO: Should be clone() once the chip object is set up for it
        %clone.setVisible(true);
        %clone.setBodyType("dynamic");

        // Position the object in the chip stack
        %jitterX = %this.chipStackJitter * (getRandom() * 2 - 1);
        %positionX = %basePosition.x + %jitterX;
        %positionY = %basePosition.y + (%chipCount * %this.chipThickness);
        %clone.setPosition(%positionX, %positionY);

        // Add the object to the stack list
        %this.betChipStackList = %clone SPC %this.betChipStackList;
    }
}

/// <summary>
/// Gets a list of all objects that have the BankStackBehavior
/// sorted in increasing order of their denomination field.
/// </summary>
/// <return>A tab-delimited string of object ids</return>
function BetAreaBehavior::getAllBankStackObjects(%this)
{
    // Initialize the return list
    %bankStackObjectList = "";

    // Get a list of all objects in the scene
    %sceneObjectList = %this.scene.GetSceneObjectList();

    // Get a count of objects in the scene
    %numOfSceneObjects = %this.scene.GetSceneObjectCount();

    // Iterate through the objects in the scene to see if they have a
    // behavior called, "BankStackBehavior"
    for (%i = 0; %i < %numOfSceneObjects; %i++)
    {
        // Get a handle to the object's BankStackBehavior
        %currentObject = getWord(%sceneObjectList, %i);
        %bankStackBehavior = %currentObject.getBehavior("BankStackBehavior");

        // If the behavior exists for that object, add the object to
        // the list of BankStackBehavior objects
        if (isObject(%bankStackBehavior) && %currentObject.isEnabled())
        {
            %bankStackObjectList = %currentObject SPC %bankStackObjectList;
        }
    }

    // Sort the %bankStackObjectList in order of denomination
    %sortedList = "";
    while (getWordCount(%bankStackObjectList) > 0)
    {
        %largestIndex = 0;
        %largestValue = getWord(%bankStackObjectList, 0).getBehavior("BankStackBehavior").denomination;

        for (%i = 1; %i < getWordCount(%bankStackObjectList); %i++)
        {
            %newValue = getWord(%bankStackObjectList, %i).getBehavior("BankStackBehavior").denomination;
            if (%newValue > %largestValue)
            {
                %largestIndex = %i;
                %largestValue = %newValue;
            }
        }

        %sortedList = getWord(%bankStackObjectList, %largestIndex) SPC %sortedList;
        %bankStackObjectList = removeWord(%bankStackObjectList, %largestIndex);
    }


    return %sortedList;
}

//-----------------------------------------------------------------------------
// Callbacks
//-----------------------------------------------------------------------------

/// <summary>
/// onTouchUp callback
/// </summary>
/// <param name="touchID">ID of the touch event.</param>
/// <param name="worldPos">Position of the touch event.</param>
function BetAreaBehavior::onTouchUp(%this, %touchID, %worldPos)
{
    if (%this.touchID $= %touchID)
    {
        %this.touchID = "";

        // Return the existing bet to the bank
        if (%this.touchedDownOnBetArea && %this.isEditingBet)
            %this.cancelBet($UserControlledPlayer);

        %this.touchedDownOnBetArea = false;
    }
}

/// <summary>
/// onTouchDown callback
/// </summary>
/// <param name="touchID">ID of the touch event.</param>
/// <param name="worldPos">Position of the touch event.</param>
function BetAreaBehavior::onTouchDown(%this, %touchID, %worldPos)
{
    if (%this.touchID $= "")
    {
        // If we are not in the betting stage, do not register mouse clicks
        if ($CurrentTable.state !$= "Ready")
            return;

        %this.touchID = %touchID;

        %this.touchedDownOnBetArea = true;
    }
}