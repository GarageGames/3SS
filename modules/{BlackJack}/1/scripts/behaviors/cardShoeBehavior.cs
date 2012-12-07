//-----------------------------------------------------------------------------
// Torque Card Template
// Copyright © GarageGames, LLC.
//-----------------------------------------------------------------------------
// Behavior for the 'Shoe' object.
// A shoe is the collection of cards used in the game.
//-----------------------------------------------------------------------------

//-------------------------------------
// Card Suits
//-------------------------------------

/// Number of different card suits.
$CardSuitCount = 4;

/// Heart
$CardSuit0 = "H";

/// Club
$CardSuit1 = "C";

/// Diamond
$CardSuit2 = "D";

/// Spade
$CardSuit3 = "S";

//-------------------------------------
// Card Values
//-------------------------------------

/// Number of different card values.
$CardValueCount = 13;

/// Ace
$CardValue0 = "A";

/// 2-10
$CardValue1 = "2";
$CardValue2 = "3";
$CardValue3 = "4";
$CardValue4 = "5";
$CardValue5 = "6";
$CardValue6 = "7";
$CardValue7 = "8";
$CardValue8 = "9";
$CardValue9 = "T";

/// Jack
$CardValue10 = "J";

/// Queen
$CardValue11 = "Q";

/// King
$CardValue12 = "K";


/// <summary>
/// Create this behavior only if it does not already exist
/// <summary>
if (!isObject(CardShoeBehavior))
{
    // Create this behavior from the blank BehaviorTemplate
    %template = new BehaviorTemplate(CardShoeBehavior);

    // friendlyName will be what is displayed in the editor
    %template.friendlyName = "Card Shoe";

    // behaviorType organize this behavior in the editor
    %template.behaviorType = "Card Template";

    // description briefly explains what this behavior does
    %template.description  = "Manages the 'shoe' (the ordered collection of cards).";

    //----------------------------------
    // Behavior Fields
    //----------------------------------

    %template.addBehaviorField(deckCount, "Number of decks in the shoe (1 - 10).", int, 5);
    %template.addBehaviorField(shoePenetrationPercent, "Percentage of total shoe we play before re-shuffling (after finishing the current hand).", int, 70);
    %template.addBehaviorField(playingCards, "PlayingCardBehavior used by this shoe.", object, "", SceneObject);
    %template.addBehaviorField(penetrationCard, "The Penetration Card used by this shoe.", object, "", SceneObject);
    %template.addBehaviorField(shuffleLabel, "Label to display while shuffling.", object, "", SceneObject);
    %template.addBehaviorField(shuffleTime, "Time (in seconds) needed to shuffle.", float, 3.0);

    //----------------------------------
    // Vars
    //----------------------------------

    /// Total number of cards in the shoe.
    %template.shoeSize = 0;
}

//------------------------------------------------------------------------------
// Editor Accessors
//------------------------------------------------------------------------------

///<return>Get value.</return>
function CardShoeBehavior::getDeckCount(%this)
{
    return %this.deckCount;
}

///<param="%value">Value to set target to.</param>
function CardShoeBehavior::setDeckCount(%this, %value)
{
    %this.deckCount = %value;
}

///<return>Get value.</return>
function CardShoeBehavior::getShoePenetrationPercent(%this)
{
    return %this.shoePenetrationPercent;
}

///<param="%value">Value to set target to.</param>
function CardShoeBehavior::setShoePenetrationPercent(%this, %value)
{
    %this.shoePenetrationPercent = %value;
}

///<return>Get value.</return>
function CardShoeBehavior::getPlayingCards(%this)
{
    return %this.playingCards;
}

///<param="%value">Value to set target to.</param>
function CardShoeBehavior::setPlayingCards(%this, %value)
{
    %this.playingCards = %value;
}

///<return>Get value.</return>
function CardShoeBehavior::getPenetrationCard(%this)
{
    return %this.penetrationCard;
}

///<param="%value">Value to set target to.</param>
function CardShoeBehavior::setPenetrationCard(%this, %value)
{
    %this.penetrationCard = %value;
}

///<return>Get value.</return>
function CardShoeBehavior::getShuffleLabel(%this)
{
    return %this.shuffleLabel;
}

///<param="%value">Value to set target to.</param>
function CardShoeBehavior::setShuffleLabel(%this, %value)
{
    %this.shuffleLabel = %value;
}

///<return>Get value.</return>
function CardShoeBehavior::getShuffleTime(%this)
{
    return %this.shuffleTime;
}

///<param="%value">Value to set target to.</param>
function CardShoeBehavior::setShuffleTime(%this, %value)
{
    %this.shuffleTime = %value;
}

//------------------------------------------------------------------------------

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function CardShoeBehavior::onBehaviorAdd(%this)
{
    if (isObject($CurrentShoe))
        warn("$CurrentShoe is already set before we added this one!");

    $CurrentShoe = %this;

    %this.init();
    %this.shuffle();

    // Uncomment the next line to run unit tests.
    //%this.runTests();
}

/// <summary>
/// Initialize the shoe with all the cards in order.
/// </summary>
function CardShoeBehavior::init(%this)
{
    // Current index into the shoe
    %this.shoeIndex = 0;

    // size of the shoe
    %this.shoeSize = %this.deckCount * $cardSuitCount * $cardValueCount;

    // calculate the penetration card position
    %this.penetrationIndex = mFloor((%this.shoePenetrationPercent / 100) * %this.shoeSize);
    %this.hasReachedPenetration = false;

    %this.isShuffling = false;

    // fill the array with sorted cards
    %base = 0;
    for (%deck = 0; %deck < %this.deckCount; %deck++)
    {
        for (%suit = 0; %suit < $cardSuitCount; %suit++)
        {
            for (%value = 0; %value < $cardValueCount; %value++)
            {
                %this.shoeArray[%base] = $cardValue[%value] SPC $cardSuit[%suit];
                %base++;
            }
        }
    }
}

/// <summary>
/// Swap the places of two cards in the shoe.
/// </summary>
/// <param name="firstCard">Index of card 1.</param>
/// <param name="secondCard">Index of card 2.</param>
function CardShoeBehavior::swapCards(%this, %firstCard, %secondCard)
{
    %t = %this.shoeArray[%firstCard];
    %this.shoeArray[%firstCard] = %this.shoeArray[%secondCard];
    %this.shoeArray[%secondCard] = %t;
}

/// <summary>
/// Shuffle the entire shoe (using a Knuth shuffle algorithm).
/// </summary>
function CardShoeBehavior::shuffle(%this)
{
    %this.showShuffleLabel();
    if (!$LevelEditorActive)
        alxPlay($Save::GeneralSettings::Sound::Shuffling);

    for (%this.shoeIndex = 0; %this.shoeIndex < %this.shoeSize; %this.shoeIndex++)
    {
        %this.swapCards(%this.shoeIndex, getRandom(%this.shoeIndex, %this.shoeSize - 1));
    }

    %this.shoeIndex = 0;
}

/// <summary>
/// Draw the next card from the shoe.
/// </summary>
/// <param "hand">Hand drawing the card</param>
/// <return>The card object if %hand is valid, otherwise (value SPC suit)</return>
function CardShoeBehavior::drawCard(%this, %hand)
{
    if (%this.nextCard $= "")
    {
        // check to see if we're out of cards
        if (%this.shoeIndex >= %this.shoeSize)
        {
            // new shoe.
            %this.init();
            %this.shuffle();
        }

        // check for penetration
        if (%this.shoeIndex >= %this.penetrationIndex)
        {
            if (!%this.hasReachedPenetration && isObject(%this.penetrationCard))
            {
                alxPlay($Save::GeneralSettings::Sound::CardDealt);

                // toss out penetration card.
                %yellowCard = %this.penetrationCard.clone();
                %yellowCard.setPosition(%this.owner.getPosition());
                %yellowCard.setAngle(%this.owner.getAngle());
                %yellowCard.disposeCard();
            }

            %this.hasReachedPenetration = true;
        }

        %this.nextCard = %this.shoeArray[%this.shoeIndex];
        %this.shoeIndex++;
    }

    if (isObject(%this.playingCards) && isObject(%hand))
    {
        alxPlay($Save::GeneralSettings::Sound::CardDealt);

        %clone = %this.playingCards.clone();
        %clone.setPosition(%this.owner.getPosition());
        %clone.setAngle(%this.owner.getAngle());
        %clone.cardValue = %this.nextCard;

        %this.nextCard = "";
        return %clone;
    }

    %temp = %this.nextCard;
    %this.nextCard = "";
    return %temp;
}

/// <summary>
/// Number of cards left in the shoe.
/// </summary>
/// <return>The number of cards left in the shoe.</return>
function CardShoeBehavior::getRemainingCount(%this)
{
    return %this.shoeSize - %this.shoeIndex;
}

/// <summary>
/// Remove the current label.
/// </summary>
function CardShoeBehavior::removeShuffleLabel(%this)
{
    %this.removeLabelHandle = 0;
    %this.isShuffling = false;

    if (isObject(%this.label))
    {
        %this.label.safeDelete();
    }
}

/// <summary>
/// Show a shuffling label over the hand.
/// </summary>
function CardShoeBehavior::showShuffleLabel(%this)
{
    // don't allow labels to stack.
    if (isEventPending(%this.removeLabelHandle))
    {
        // handle it manually
        cancel(%this.removeLabelHandle);
        %this.removeLabel(false);
    }

    if (isObject(%this.shuffleLabel))
    {
        %this.isShuffling = true;
        %this.label = %this.shuffleLabel.clone();

        %this.label.setPosition(%this.owner.getPosition());
        %this.label.setVisible(true);
        %this.removeLabelHandle = %this.schedule(%this.shuffleTime * 1000, "removeShuffleLabel", true);
    }
}


//------------------------------------------------------------------------------
// DEBUG BLOCK STARTS
//------------------------------------------------------------------------------

function setNextCard(%val)
{
    if (isObject($CurrentShoe))
        $CurrentShoe.nextCard = %val;
}

function setNextCardA()
{
    setNextCard("A S");
}

function setNextCard2()
{
    setNextCard("2 S");
}

function setNextCard3()
{
    setNextCard("3 S");
}

function setNextCard4()
{
    setNextCard("4 S");
}

function setNextCard5()
{
    setNextCard("5 S");
}

function setNextCard6()
{
    setNextCard("6 S");
}

function setNextCard7()
{
    setNextCard("7 S");
}

function setNextCard8()
{
    setNextCard2("8 S");
}

function setNextCard9()
{
    setNextCard("9 S");
}

function setNextCardT()
{
    setNextCard("T S");
}

GlobalActionMap.bind(keyboard, "1", setNextCardA);
GlobalActionMap.bind(keyboard, "2", setNextCard2);
GlobalActionMap.bind(keyboard, "3", setNextCard3);
GlobalActionMap.bind(keyboard, "4", setNextCard4);
GlobalActionMap.bind(keyboard, "5", setNextCard5);
GlobalActionMap.bind(keyboard, "6", setNextCard6);
GlobalActionMap.bind(keyboard, "7", setNextCard7);
GlobalActionMap.bind(keyboard, "8", setNextCard8);
GlobalActionMap.bind(keyboard, "9", setNextCard9);
GlobalActionMap.bind(keyboard, "0", setNextCardT);

