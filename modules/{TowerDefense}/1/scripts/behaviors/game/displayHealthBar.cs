//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// 

/// <summary>
/// Display a health bar above the assigned object.  Interacts with  
/// TakesDamageBehavior.
/// </summary>
if (!isObject(DisplayHealthBar))
{
   // Create this behavior from the blank BehaviorTemplate
   // Name it DisplayHealthBar
   %template = new BehaviorTemplate(DisplayHealthBar);
   
   // friendlyName will be what is displayed in the editor
   // behaviorType organize this behavior in the editor
   // description briefly explains what this behavior does
   %template.friendlyName = "Display Health Bar";
   %template.behaviorType = "Game";
   %template.description  = "Displays a health bar over a game entity.";
   
   %template.addBehaviorField(HealthBarObj, "Object used to show the health bar.", object, "", SceneObject);
   %template.addBehaviorField(BarFrameObj, "Object used to show the health bar frame.", object, "", SceneObject);
   %template.addBehaviorField(FrameOnTop, "Health bar frame is in front of bar.", bool, 1);
   %template.addBehaviorField(BarToLeft, "Health bar justifies left.", bool, 1);
   %template.addBehaviorField(ChangeColor, "Health bar changes color based on damage level.", bool, 1);
   %template.addBehaviorField(ColorGood, "Color of health bar when in Good condition.", color, "0.0 1.0 0.0 1.0");
   %template.addBehaviorField(DamageState1, "The health level that sets the Hurt state color.", float, 0.8);
   %template.addBehaviorField(ColorHurt, "Color of health bar when in Hurt condition.", color, "1.0 1.0 0.0 1.0");
   %template.addBehaviorField(DamageState2, "The health level that sets the Dying state color.", float, 0.3);
   %template.addBehaviorField(ColorDying, "Color of health bar when in Dying condition.", color, "1.0 0.0 0.0 1.0");
}

/// <summary>
/// This function performs basic initialization.
/// </summary>
function DisplayHealthBar::onBehaviorAdd(%this)
{
   //%this.touchID = "";
   //%this.owner.UseInputEvents = true;
   
   %this.updateFrequency = 250; // ms
}

/// <summary>
/// This function performs basic initialization.
/// </summary>
function DisplayHealthBar::onAddToScene(%this)
{
   // Enable the onUpdate callback on the owner to allow us to use our
   // onUpdate callback for this behavior.
   if($LevelEditorActive)
      %this.owner.updateCallback = false;

   %this.offset = 0;
   %this.barOffsetX = 0;

   //%this.createHealthBar();
}

/// <summary>
/// This function creates a clone of the health bar and frame objects in preparation for
/// calling setupHealthBar().
/// </summary>
function DisplayHealthBar::createHealthBar(%this)
{
   %this.offset = 0;
   %this.barOffsetX = 0;

   if (isObject(%this.HealthBarObj))
   {
      // size & position have to be set later
      %this.HealthBarObj.Visible = false;
      %this.displayObj = %this.HealthBarObj.clone();
      
      if (isObject(%this.BarFrameObj))
      {
         %this.BarFrameObj.Visible = false;
         %this.frameObj = %this.BarFrameObj.clone();
      }
         
      %this.schedule(50, "setupHealthBar");
   }
}

/// <summary>
/// Setup the Health Bar's size and starting postion (need to call this *after* the TakesDamageBehavior is added).
/// </summary>
function DisplayHealthBar::setupHealthBar(%this)
{
   if (isObject(%this.displayObj))
   {
      %this.displayObj.Visible = true;
      
      if (isObject(%this.frameObj))
         %this.frameObj.Visible = true;

      %this.totalHealth = %this.owner.callOnBehaviors(getHealth);
      %this.currentHealth = %this.totalHealth;

      if (%this.totalHealth !$= "ERR_CALL_NOT_HANDLED")
      {
         if(isObject(%this.displayObj))
            %this.displayObj.setSize((%this.owner.getWidth() / 2), %this.displayObj.getHeight());
         if(isObject(%this.frameObj))
            %this.frameObj.setSize((%this.owner.getWidth() / 2), %this.frameObj.getHeight());
            
         %this.offset = (%this.owner.getHeight() - %this.displayObj.getHeight()) / 2;
      }

      if (!%this.FrameOnTop)
      {
         if (isObject(%this.frameObj))
         {
            %x = getWord(%this.owner.position, 0);
            %y = getWord(%this.owner.position, 1) + %this.offset;
            %this.frameObj.setPosition(%x, %y);
            %this.frameObj.setSceneLayer(3);
         }
      }
      
      %x = getWord(%this.owner.position, 0);
      %y = getWord(%this.owner.position, 1) + %this.offset;
      %position = %x SPC %y;
      %this.displayObj.position = %position;

      %this.displayObj.setSceneLayer(2);
      %this.displayObj.BlendColor = %this.ColorGood;
   }
   
   if (%this.FrameOnTop)
   {
      if (isObject(%this.frameObj))
      {
         %this.frameObj.setSceneLayer(1);
      }
   }
}

/// <summary>
/// This function updates the health bar's and value display.
/// </summary>
function DisplayHealthBar::updateHealthBar(%this)
{
   if (%this.deleting)
      return;
   
   if(MainScene.getScenePause())
   {
      // Paused so skip the logic and reschedule
      %this.rescheduleUpdate();
      return;
   }

   // Update health bar to display current health
   %health = %this.owner.callOnBehaviors(getHealth);
   
   // Test to see that health has changed since last update
   if (%health < %this.currentHealth)
   {
      // Since health has changed we need to update the health bar.
      // Keep the current health to see if we need to update the health bar
      // on the next update.
      %this.currentHealth = %health;
      
      // Set the new size of the health bar.
      %sizer = (%this.currentHealth / %this.totalHealth) * (%this.owner.getWidth() / 2);
      if (%sizer < 0)
        %sizer = 0;

      if (isObject(%this.displayObj))
      {
         %this.displayObj.setWidth(%sizer);
         
         // If we want the health bar to shrink to the left we have to calculate 
         // our mounting offset and update the health bar position.
         if (%this.BarToLeft)
         {
            %mobWidth = %this.owner.getWidth() / 2;
            %barWidth = %this.displayObj.getWidth();
            %this.barOffsetX = (%mobWidth / 2) - (%barWidth / 2);
         }
      }
      
      if (isObject(%this.frameObj))
      {
         %x = getWord(%this.owner.position, 0);
         %y = getWord(%this.owner.position, 1) + %this.offset;
         %this.frameObj.setPosition(%x, %y);
      }

      // This section adjusts the color of the health bar to reflect the damage 
      // state.  These colors are GREEN, YELLOW and RED by default, but they can be
      // adjusted in the editor using the appropriate fields.
      
      // The DamageStateX variables are also set in the editor to determine when
      // 
      if (%this.ChangeColor)
      {
         %damagePercent = %this.currentHealth / %this.totalHealth;
         if (%damagePercent <= %this.DamageState1 && %damagePercent > %this.DamageState2)
         {
            %this.displayObj.BlendColor = %this.ColorHurt;
         }
         else if (%damagePercent <= %this.DamageState2)
         {
            %this.displayObj.BlendColor = %this.ColorDying;
         }
      }
   }
   
   %this.rescheduleUpdate();
}

/// <summary>
/// This function reschedules the next update.
/// </summary>
function DisplayHealthBar::rescheduleUpdate(%this)
{
   %this.updateSchedule = %this.schedule(%this.updateFrequency, updateHealthBar);
}

/// <summary>
/// This function handles the onUpdate callback for the health bar and keeps the 
/// health bar's position correct.
/// </summary>
function DisplayHealthBar::onUpdate(%this)
{
   if (%this.deleting)
      return;
      
   // This remounts the health bar with its offset.  Each parameter of
   // the mount menthod is commented for clarity.
   %x = getWord(%this.owner.position, 0) - %this.barOffsetX;
   %y = getWord(%this.owner.position, 1) + %this.offset;
   //echo(" -- DisplayHealthBar::onUpdate() - " @ %position);
   %this.displayObj.setPosition(%x, %y);

   %x = getWord(%this.owner.position, 0);
   %y = getWord(%this.owner.position, 1) + %this.offset;
   %this.frameObj.setPosition(%x, %y);
}

/// <summary>
/// This function cleans up the health bar display on enemy death.
/// </summary>
function DisplayHealthBar::cleanupHealthBar(%this)
{
    %this.deleting = true;
    cancel(%this.updateSchedule);

    //echo(" -- DisplayHealthBar::cleanup()");
    if(isObject(%this.displayObj))
    {
        %this.displayObj.setVisible(false);
        %this.displayObj.safeDelete();
    }

    if(isObject(%this.frameObj))
    {
        %this.frameObj.setVisible(false);
        %this.frameObj.safeDelete();
    }
}

/// <summary>
/// This function handles resetting values for use when recycling enemies.
/// </summary>
function DisplayHealthBar::resetHealthBar(%this)
{
   %this.deleting = false;
   %this.createHealthBar();
   %this.rescheduleUpdate();
}
