//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// The toggleShowRangeOnTouchDownBehavior controls the display of the tower 
/// range circles and provides placement assistance for the tower upgrade and 
/// sell buttons.
/// </summary>
if (!isObject(toggleShowRangeOnTouchDownBehavior))
{
   %template = new BehaviorTemplate(toggleShowRangeOnTouchDownBehavior);
   
   %template.friendlyName = "Show/Hide Range on Touch Down/Up";
   %template.behaviorType = "GUI";
   %template.description  = "Show the owner on touch down, hide it on next touch down";

   %template.addBehaviorField(RangeObj, "Object used to show the range.", object, "", SceneObject);
}

/// <summary>
/// This function hides the range circle and any buttons currently displayed
/// </summary>
function toggleShowRangeOnTouchDownBehavior::hide(%this)
{
   if (!%this.Visible)
      return;
      
   %this.Visible = false;
   %this.RangeObj.Visible = %this.Visible;
   %this.owner.setSceneLayer(%this.showLayer);
   //echo(" -- hide range, tower layer : " @ %this.owner.getSceneLayer());

   HideTowerOptions();
}

/// <summary>
/// This handles initial setup of the range circle display for each tower.
/// The behavior instance is added to a SimSet so that we can toggle all of them
/// off from the ClearRangesBehavior.
/// </summary>
function toggleShowRangeOnTouchDownBehavior::onAddToScene(%this)
{
   if (!$LevelEditorActive)
   {
      %this.touchID = "";
      %this.owner.UseInputEvents = true;

      if (!isObject(RangeBehaviorSet))
         $RangeBehaviorCollection = new SimSet(RangeBehaviorSet);
      
      $RangeBehaviorCollection.add(%this);
   
      %this.Visible = false;
      %this.trycount = 0;
   
      %this.setupRangeObj();
   }
}

/// <summary>
/// The onTouchDown handler simply clears touch states so that we can show the
/// range circle object in the onTouchUp handler.
/// </summary>
/// <param name="touchID">The index of the touch - indicates which of up to 11 simultaneous touch events this is.</param>
/// <param name="worldPos">The world position at which the touch event occured.</param>
function toggleShowRangeOnTouchDownBehavior::onTouchDown(%this, %touchID, %worldPos)
{
   if (isObject($TowerBeingPlaced) || $TouchHandled)
      return;
      
   if (%this.touchID !$= "")
      return;
      
   %this.touchID = %touchID;
   
   $TouchHandled = true;
   
   if (isObject($TowerBeingPlaced))
      %this.Visible = false;
}

/// <summary>
/// The onTouchUp handler takes care of actually displaying the range circle and
/// any relevant tower management buttons.
/// </summary>
/// <param name="touchID">The index of the touch - indicates which of up to 11 simultaneous touch events this is.</param>
/// <param name="worldPos">The world position at which the touch event occured.</param>
function toggleShowRangeOnTouchDownBehavior::onTouchUp(%this, %touchID, %worldPos)
{
   if (%this.touchID $= %touchID)
   {
      %this.touchID = "";
   
      if (%this.Visible)
         return;   
         
      // Cruise through the scene and find range circle behaviors, then
      // toggle them all off
      if (isObject($RangeBehaviorCollection))
      {
         %count = $RangeBehaviorCollection.getCount();
         
         for (%i = 0; %i < %count; %i++)
         {
            %behavior = $RangeBehaviorCollection.getObject(%i);
            %behavior.hide();
         }
      }
   
      %this.Visible = true;

      if (!%this.owner.centerPoint)
         %towerPos = %this.owner.position;
      else
         %towerPos = %this.owner.centerPoint;

      %this.setupRangeObj();
         
      %x = getWord(%towerPos, 0);
      %y = getWord(%towerPos, 1);
      %this.RangeObj.setPosition(%x, %y);
      %this.showLayer = %this.owner.getSceneLayer();
      %this.RangeObj.setSceneLayer(3);
      %this.owner.setSceneLayer(2);
      %this.RangeObj.setVisible(%this.Visible); 
      //echo(" -- show range, tower layer : " @ %this.owner.getSceneLayer());
      
      if (%this.Visible)
         ShowTowerOptions(%this.owner);
   }
}

/// <summary>
/// Setup the Range Object's size and starting postion (need to call this *after* the TowerShootsBehavior is added).
/// </summary>
function toggleShowRangeOnTouchDownBehavior::setupRangeObj(%this)
{
   if (isObject(%this.RangeObj))
   {
      // Place this range circle below the bottom/top HUD
      %this.RangeObj.layer = 3;
      
      %this.RangeObj.setVisible(%this.Visible);

      %sizer = %this.owner.callOnBehaviors(getRadius) * 2.0;
      if (%sizer > 0.0)
      {
         %this.RangeObj.setSize(%sizer, %sizer);
         %this.RangeObj.setBlendColor($Towers::RangeDropColor);

         if (%this.owner.centerPoint)
         {
            %position = %this.owner.centerPoint;
            %x = getWord(%position, 0);
            %y = getWord(%position, 1);
            //echo("toggleShowRangeOnTouchDownBehavior::setupRangeObj posX = " @ %x );
            //echo("toggleShowRangeOnTouchDownBehavior::setupRangeObj posY = " @ %y );
            %this.RangeObj.setPosition(%x, %y);
            %this.RangeObj.Visible = %this.Visible;
         }
      }
   }
}
