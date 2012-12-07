//-----------------------------------------------------------------------------
// Torque Card Template
// Copyright © GarageGames, LLC.
//-----------------------------------------------------------------------------
// Behavior for the Discard Pile.
//-----------------------------------------------------------------------------

/// <summary>
/// Create this behavior only if it does not already exist
/// <summary>
if (!isObject(DiscardPileBehavior))
{
    // Create this behavior from the blank BehaviorTemplate
    %template = new BehaviorTemplate(DiscardPileBehavior);

    // friendlyName will be what is displayed in the editor
    %template.friendlyName = "Discard Pile";

    // behaviorType organize this behavior in the editor
    %template.behaviorType = "Card Template";

    // description briefly explains what this behavior does
    %template.description  = "Behavior for the Discard Pile.";

    //----------------------------------
    // Behavior Fields
    //----------------------------------

    %template.addBehaviorField(maxSize, "Maximum number of cards to hold onto.", int, 10);
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function DiscardPileBehavior::onBehaviorAdd(%this)
{
    %this.index = 0;
    %this.card[0] = 0;
    //echo("DiscardPileBehavior" SPC %this.index);
}

/// <summary>
/// Add a card to the discard pile.
/// </summary>
function DiscardPileBehavior::addCard(%this, %card)
{
    %behavior = %card.getBehavior("PlayingCardsBehavior");

    if (!isObject(%behavior))
    {
        %card.safeDelete();
        return;
    }

    if (isObject(%this.card[%this.index]))
    {
        %this.card[%this.index].safeDelete();
    }

    %this.card[%this.index] = %card;
    
    %target = %this.owner.getWorldPoint(%this.owner.getLocalCenter());
    %angle = %this.owner.getAngle();
    %card.callOnBehaviors(dealCardTo, %target, %angle);
    %this.index++;
    while (%this.index >= %this.maxSize)
        %this.index -= %this.maxSize;
}

