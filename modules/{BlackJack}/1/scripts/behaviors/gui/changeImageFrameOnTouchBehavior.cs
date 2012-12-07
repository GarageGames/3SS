//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

if (!isObject(ChangeImageFrameOnTouchBehavior))
{
    %template = new BehaviorTemplate(ChangeImageFrameOnTouchBehavior);

    %template.friendlyName = "Change Image Frame On Touch";
    %template.behaviorType = "GUI";
    %template.description  = "Increases/Decreases the frame of the image for an object upon touch";

    %template.addBehaviorField(IncreaseFrame, "Check to increase upon touch, uncheck to decrease", bool, true);
}

function ChangeImageFrameOnTouchBehavior::onBehaviorAdd(%this)
{
    %this.touchID = "";
    %this.owner.UseInputEvents = true;
}

function ChangeImageFrameOnTouchBehavior::onTouchDown(%this, %touchID, %worldPos)
{
    if (%this.touchID $= "")
        %this.touchID = %touchID;
}

function ChangeImageFrameOnTouchBehavior::onTouchUp(%this, %touchID, %worldPos)
{
    if (%this.touchID !$= %touchID)
        return;

    %this.touchID = "";

    if (%this.IncreaseFrame)
    {
        if (%this.owner.getFrame() == %this.owner.getImageMap().getFrameCount() - 1)
            %this.owner.setFrame(0);
        else
            %this.owner.setFrame(%this.owner.getFrame() + 1);
    }
    else
    {
        if (%this.owner.getFrame() == 0)
            %this.owner.setFrame(%this.owner.getImageMap().getFrameCount() - 1);
        else
            %this.owner.setFrame(%this.owner.getFrame() - 1);
    }
}