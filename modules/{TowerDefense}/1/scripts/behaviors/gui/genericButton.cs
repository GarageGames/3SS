//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// The GenericButtonBehavior allows any scene object to behave as if it were a 
/// button.
/// </summary>
if (!isObject(GenericButtonBehavior))
{
   %template = new BehaviorTemplate(GenericButtonBehavior);
   
   %template.friendlyName = "Generic Button";
   %template.behaviorType = "GUI";
   %template.description  = "Executes a method when clicked. The method is called on the behavior's owner";

   %template.addBehaviorField(clickImage, "The image to display when clicking on the object", object, "", ImageAsset);
   %template.addBehaviorField(clickFrame, "The frame of the clickImage to display", int, 0);
   %template.addBehaviorField(buttonDown, "The method to call on touch down", string, "onButtonDown");
   %template.addBehaviorField(buttonUp, "The method to call on touch up", string, "onButtonUp");
}

/// <summary>
/// This handles initialization.  Specifically it ensures that the object is set up to 
/// handle input events and sets the normal button image.
/// </summary>
function GenericButtonBehavior::onBehaviorAdd(%this)
{
   %this.owner.UseInputEvents = true;
   %this.touchID = "";
   
   %this.startImage = %this.owner.getImageMap();
   %this.startFrame = %this.owner.getFrame();
   
   $CapturedTouchIDsOwners.add(%this); 
}

/// <summary>
/// The onTouchDown handler covers the button onClick role for the scene object, including changing
/// the current image to present a clicked state image.
/// </summary>
/// <param name="touchID">The index of the touch - indicates which of up to 11 simultaneous touch events this is.</param>
/// <param name="worldPos">The world position at which the touch event occured.</param>
function GenericButtonBehavior::onTouchDown(%this, %touchID, %worldPos)
{
   if (%this.touchID !$= "")
      return;
      
   %this.touchID = %touchID;  
      
   if (isObject(%this.clickImage))
      %this.owner.setImageMap(%this.clickImage, %this.clickFrame);

   if (%this.owner.isMethod(%this.buttonDown))
      %this.owner.call(%this.buttonDown, %worldPos, %touchID);
}

/// <summary>
/// The onTouchUp handler covers the button onButtonUp role for the scene object, including returning
/// the current image to the unclicked state image.
/// </summary>
/// <param name="touchID">The index of the touch - indicates which of up to 11 simultaneous touch events this is.</param>
/// <param name="worldPos">The world position at which the touch event occured.</param>
function GenericButtonBehavior::onTouchUp(%this, %touchID, %worldPos)
{
   if (%this.touchID $= %touchID)
   {
      %this.touchID = "";
      
      if (isObject(%this.clickImage))
         %this.owner.setImageMap(%this.startImage, %this.startFrame);
      
      if (%this.owner.isMethod(%this.buttonUp))
         %this.owner.call(%this.buttonUp, %worldPos);
   }
}