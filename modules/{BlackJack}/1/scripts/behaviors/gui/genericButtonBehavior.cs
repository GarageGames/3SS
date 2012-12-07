//-----------------------------------------------------------------------------
// Torque Card Template
// Copyright © GarageGames, LLC.
//-----------------------------------------------------------------------------

if (!isObject(GenericButtonBehavior))
{
    %template = new BehaviorTemplate(GenericButtonBehavior);

    %template.friendlyName = "Generic Button";
    %template.behaviorType = "GUI";
    %template.description  = "Executes a method when clicked. The method is called on the behavior's owner";

    %template.addBehaviorField(clickImage, "The image to display when clicking on the object", object, "", ImageAsset);
    %template.addBehaviorField(clickFrame, "The frame of the clickImage to display", int, 0);
    %template.addBehaviorField(method, "The method to call on touch up", string, "onClick");
    %template.addBehaviorField(delay, "Time (in ms) the button is inactive after use.", int, 64);

    %template.addBehaviorField(playSound, "Play sound when pressed.", bool, true);
}

function GenericButtonBehavior::onBehaviorAdd(%this)
{
    %this.owner.UseInputEvents = true;
    %this.touchID = "";

    %this.startImage = %this.owner.getImageMap();
    %this.startFrame = %this.owner.getFrame();
}

function GenericButtonBehavior::reactivate(%this)
{
    %this.owner.UseInputEvents = true;
}

function GenericButtonBehavior::onTouchDown(%this, %touchID, %worldPos)
{
    if ((%this.touchID !$= "") || !isObject(%this.clickImage))
        return;

    %this.touchID = %touchID;

    %this.owner.setImageMap(%this.clickImage, %this.clickFrame);

    if (%this.playSound)
        alxPlay($Save::GeneralSettings::Sound::ButtonClick);
}

function GenericButtonBehavior::onTouchUp(%this, %touchID, %worldPos)
{
    if (%this.touchID $= %touchID)
    {
        %this.touchID = "";

        if (%this.delay > 0)
        {
            %this.owner.UseInputEvents = true;
            %this.schedule(%this.delay, reactivate);
        }

        %this.owner.setImageMap(%this.startImage, %this.startFrame);

        if (%this.owner.isMethod(%this.method))
            %this.owner.call(%this.method, %worldPos);
    }
}