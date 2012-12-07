//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// Create this behavior only if it does not already exist
if (!isObject(PlayingCardsBehavior))
{
    %template = new BehaviorTemplate(PlayingCardsBehavior);

    %template.FriendlyName = "Playing Cards";
    %template.BehaviorType = "Blackjack Decks";
    %template.Description = "Defines the images to use for the deck and the manipulation of cards on screen";

    //%template.addBehaviorField(beginningRotation, "The angle the card should be at before it is dealt", float, 45.0);
    //%template.addBehaviorField(endingRotation, "The angle the card should be at when it has reached it's destination", float, 0.0);
    %template.addBehaviorField(msecsPerMeter, "The time it takes the card to travel a unit of distance", float, 60.0);
    %template.addBehaviorField(isPenetrationCard, "Is this a 'penetration card' (no value, suit, or back)?", bool, false);
    %template.addBehaviorField(discardPile, "Discard pile.", object, "", SceneObject);
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function PlayingCardsBehavior::onBehaviorAdd(%this)
{
    // set the default card value and whether this is the dealer's second card
    %this.isFaceDown = false;
    if (%this.isPenetrationCard)
        %this.cardValue = "P P";
    else
        %this.cardValue = "A S";

    //set the beginning rotation
    //%this.owner.setAngle(%this.beginningRotation);

    //interval counter
    %this.intervalCounter = 0;
    
    %this.owner.setSceneLayer(0);

    %this.owner.UseInputEvents = true;
}

/// <summary>
/// Gets isFaceDown
/// </summary>
function PlayingCardsBehavior::isFaceDown(%this)
{
    return %this.isFaceDown;
}

/// <summary>
/// Sets isFaceDown
/// </summary>
function PlayingCardsBehavior::setIsFaceDown(%this, %value)
{
    %this.isFaceDown = %value;
}

/// <summary>
/// Passes the card from the card shoe to the player it's intended for
/// </summary>
/// <param name="destination">The location to move this card to.</param>
function PlayingCardsBehavior::dealCardTo(%this, %destination, %rotation)
{
    %this.endingRotation = %rotation;
    %this.beginningRotation = %this.owner.getAngle();
    %this.updateCalculated = false;
    %this.calculateIntervals(%destination);
    %this.updateOrientation();
    %this.owner.setFrame(52); // set to card back
    %this.owner.moveTo(%destination, %this.getTravelTimeMsecs(%destination), true);
}

/// <summary>
/// The Card object has completed its movement.  If the card is not in the correct
/// position, simply warp it there.
/// </summary>
/// <param name="obj">The behavior instance.</param>
/// <param name="targetPosition">The intended location at the end of the movement.</param>
function PlayingCardsBehavior::onMoveToComplete(%obj, %targetPosition)
{
    %obj.owner.setPosition(%targetPosition);

    %obj.onPositionTarget();
}

/// <summary>
/// Called when the object reaches it's target
/// </summary>
function PlayingCardsBehavior::onPositionTarget(%this)
{
    if (!%this.isFaceDown && !%this.isPenetrationCard)
    {
        //show the card when it's reach the player
        %this.flipCard();
    }
}

/// <summary>
/// If there is a discardPile, move the card there and delete any card already there.
/// Otherwise, just delete the card.
/// </summary>
/// <param name="destination">The location to move this card to.</param>
function PlayingCardsBehavior::disposeCard(%this)
{
    %this.updateCalculated = false;

    if (isObject(%this.discardPile))
    {
        %this.owner.setFrame(51); // set to card back
        %this.isFaceDown = true;
        %this.discardPile.addCard(%this.owner);
    }
    else
        %this.owner.safeDelete();
}

/// <summary>
/// Flips the card over to be viewed by the player
/// </summary>
function PlayingCardsBehavior::flipCard(%this)
{
    if (!%this.isPenetrationCard)
    {
        %this.owner.setFrame(%this.FindSpriteSheetFrame() - 1);
        %this.isFaceDown = false;
    }
}

/// <summary>
/// Return the time, in secs, before the card reaches it's target
/// </summary>
/// <param="target">The target position the card is moving to.</param>
/// <return>float Travel time, in seconds.</return>
function PlayingCardsBehavior::getTravelTimeMsecs(%this, %target)
{
    //calculate the distance we need to move
    %distanceToMove = VectorDist(%this.owner.getPosition(), %target);

    //calculate how long it will take us to reach our taget at our speed (pixels per second)
    %timeToTravel = (%this.msecsPerMeter * %distanceToMove);

    return %timeToTravel;
}

/// <summary>
/// Calculate the number of intervals needed to rotate before reaching the target.
/// </summary>
/// <param="target">The target position the card is moving to.</param>
function PlayingCardsBehavior::calculateIntervals(%this, %target)
{
    //calculate the distance we need to move
    %distanceToMove = VectorDist(%this.owner.getPosition(), %target);

    //calculate how long it will take us to reach our taget at our speed (pixels per second)
    %timeToTravel = %this.getTravelTimeMsecs(%target);

    //determine number of intervals need to go from beginningRotation to endRotation
    %this.angleDifference = %this.beginningRotation - %this.endingRotation;
    %numberOfIntervals = mAbs(%this.angleDifference);

    //calculate length of intervals (timetotravel / numberofintervals), adjust for ms if needed
    %this.intervalLength = (%timeToTravel / %numberOfIntervals) / 1000;

    %this.intervalCounter = %numberOfIntervals;
    %this.updateCalculated = true;
}

/// <summary>
/// Updates the orientation of the card based on how far it has to travel and what rotation is should be at when it gets there
/// NOTE: Call "PlayingCardsBehavior::calculateIntervals(%this, %target)" first to calculate values!
/// </summary>
function PlayingCardsBehavior::updateOrientation(%this)
{
    if (%this.intervalCounter <= 0)
    {
        %this.beginningRotation = %this.owner.getAngle();
        return;
    }

    if (%this.angleDifference < 0)
        %this.owner.setAngle(%this.owner.getAngle() + 1);
    else
        %this.owner.setAngle(%this.owner.getAngle() - 1);

    %this.intervalCounter--;

    //schedule next orientation change if there are still changes to make
    %this.schedule(%this.intervalLength, updateOrientation);
}

/// Returns the sprite sheet frame to display based on the card value
/// </summary>
function PlayingCardsBehavior::findSpriteSheetFrame(%this)
{
    %cardValue = getWord(%this.owner.cardValue, 0);
    %cardSuit = getWord(%this.owner.cardValue, 1);

    switch$ (%cardSuit)
    {
    case "S":
        switch$ (%cardValue)
        {
        case "A":
            return 1;
        case "2":
            return 2;
        case "3":
            return 3;
        case "4":
            return 4;
        case "5":
            return 5;
        case "6":
            return 6;
        case "7":
            return 7;
        case "8":
            return 8;
        case "9":
            return 9;
        case "T":
            return 10;
        case "J":
            return 11;
        case "Q":
            return 12;
        case "K":
            return 13;
        }
    case "C":
        switch$ (%cardValue)
        {
        case "A":
            return 14;
        case "2":
            return 15;
        case "3":
            return 16;
        case "4":
            return 17;
        case "5":
            return 18;
        case "6":
            return 19;
        case "7":
            return 20;
        case "8":
            return 21;
        case "9":
            return 22;
        case "T":
            return 23;
        case "J":
            return 24;
        case "Q":
            return 25;
        case "K":
            return 26;
        }
    case "H":
        switch$ (%cardValue)
        {
        case "A":
            return 27;
        case "2":
            return 28;
        case "3":
            return 29;
        case "4":
            return 30;
        case "5":
            return 31;
        case "6":
            return 32;
        case "7":
            return 33;
        case "8":
            return 34;
        case "9":
            return 35;
        case "T":
            return 36;
        case "J":
            return 37;
        case "Q":
            return 38;
        case "K":
            return 39;
        }
    case "D":
        switch$ (%cardValue)
        {
        case "A":
            return 40;
        case "2":
            return 41;
        case "3":
            return 42;
        case "4":
            return 43;
        case "5":
            return 44;
        case "6":
            return 45;
        case "7":
            return 46;
        case "8":
            return 47;
        case "9":
            return 48;
        case "T":
            return 49;
        case "J":
            return 50;
        case "Q":
            return 51;
        case "K":
            return 52;
        }
    }

}
