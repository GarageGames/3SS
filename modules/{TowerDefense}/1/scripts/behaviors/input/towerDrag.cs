//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
$TowerPlaceGrid = TowerPlacementGrid;

%towerDragAudioProfiles = "None" TAB "TowerPlacementSound" TAB "TowerMisplacementSound";

$Towers::PlacedColor = "1.0 1.0 1.0 1.0";
$Towers::CreatedColor = "0.8 0.8 0.8 0.65";
$Towers::DraggingColor = "0.8 0.8 0.8 0.65";
$Towers::WarnColor = "1.0 1.0 0.25 0.5";

$Towers::RangeDropColor = "0.0 1.0 0.0";
$Towers::RangeNoDropColor = "1.0 0.0 0.0";

$Towers::PlacementHighlight[0,0] = 0;
// while dragging, set the sprite to be translucent (alpha 128?)
// and set a warning color blend if you can't drop the tower at the current
// location.

/// <summary>
/// Create the TowerDragBehavior behavior only if it does not already exist.
/// This behavior allows the user to drag an associated tower object and place it 
/// on the play field.
/// </summary>
if (!isObject(TowerDragBehavior))
{
   // Create this behavior from the blank BehaviorTemplate
   // Name it TowerDrag
   %template = new BehaviorTemplate(TowerDragBehavior);
   
   // friendlyName will be what is displayed in the editor
   // behaviorType organize this behavior in the editor
   // description briefly explains what this behavior does
   %template.friendlyName = "Tower Drag";
   %template.behaviorType = "Movement Styles";
   %template.description  = "Allows the user to drag the tower using the touch screen";
   
   %template.addBehaviorField(DragInX, "Allow dragging along the X-axis", bool, true);
   %template.addBehaviorField(DragInY, "Allow dragging along the Y-axis", bool, true);
   
   %template.AddBehaviorField(NotifyPathChange, "Test all A* Actor paths before allowing placement", bool, true);
   
   %template.addBehaviorField(RangeObj, "Object used to show the range.", object, "", SceneObject);
   
   %template.addBehaviorField(PlacementSound, "Sound that plays when the tower is placed in its final position", enum, "", %towerDragAudioProfiles);
}

/// <summary>
/// Handle setup tasks onLevelLoaded
/// </summary>
/// <param name="sg">The scene that this behavior instance is assigned to.</param>
function TowerDragBehavior::onLevelLoaded(%this, %sg)
{
    for (%x = 0; %x < %this.towerGrid.getCellCountX(); %x++)
    {
        for (%y = 0; %y < %this.towerGrid.getCellCountY(); %y++)
        {
            $Towers::PlacementHighlight[%x, %y] = 0;
        }
    }
}

/// <summary>
/// Handle setup tasks onBehaviorAdd
/// This is primarily responsible for getting local access to the tower placement grid
/// object and setting up the correct display layer for use during the drag operation.
/// It also sets the blend color on the tower and disables firing during the drag operation.
/// </summary>
function TowerDragBehavior::onBehaviorAdd(%this)
{
   %this.touchID = "";
   %this.owner.UseInputEvents = true;
   
   %this.towerGrid = $TowerPlaceGrid;
   %this.owner.canFire = false;
   %this.owner.BlendColor = $Towers::CreatedColor;
   %this.placeLayer = %this.owner.getSceneLayer();
}

/// <summary>
/// This function is responsible for setting up the initial range display for the new
/// tower.
/// </summary>
function TowerDragBehavior::setupRangeObject(%this)
{
   %this.RangeObj.Visible = true;

    %range = %this.owner.callOnBehaviors("getRadius");
    if (%range > 0.0)
    {
        %sizer = %range * 2.0;
        %this.RangeObj.setSize(%sizer, %sizer);
        %this.RangeObj.setBlendColor($Towers::RangeNoDropColor);

        %position = %this.owner.getPosition();
        %x = getWord(%position, 0);
        %y = getWord(%position, 1);
        %this.RangeObj.setPosition(%x, %y);
        %this.RangeObj.Visible = true;
    }
    else
    {
        %this.setupRangeObject();
    }
}

/// <summary>
/// This function sets up to be able to pass the onButtonUp callback to the originating 
/// scene button behavior.
/// </summary>
/// <param name="theButton">The scene object carrying the Generic Button Behavior that is creating the new tower.</param>
/// <param name="touchID">The index of the touch event associated with this call.</param>
/// <param name="worldPos">The world position of the originating touch event.</param>
function TowerDragBehavior::createNewTower(%this, %theButton, %touchID, %worldPos)
{
   %this.createButton = %theButton;
   %this.owner.setSceneLayer(0);
   %this.onTouchDown(%touchID, %worldPos);
}

/// <summary>
/// Handle the onTouchUp event.  Takes care of enabling the tower placement highlighting, setting the tower in up for dragging, showing the range display,
/// and some setup for tower tile size comparisons.
/// </summary>
/// <param name="touchID">The index of the touch event associated with this call.</param>
/// <param name="worldPos">The world position at which to place everything associated with the new tower.</param>
function TowerDragBehavior::onTouchDown(%this, %touchID, %worldPos)
{
   if (%this.touchID !$= "")
      return;
      
   %this.owner.inValidPos = false;

   tileHighlight(true);
   
   // If we are already dragging, do nothing
   // Otherwise, enable the drag variable
   %this.touchID = %touchID; 

   %this.RangeObj.Visible = true;

   %this.setupRangeObject();
   //$Tower::StartPos = %this.owner.Position;
   %this.owner.BlendColor = $Towers::DraggingColor;
   
   // Define constants
   %numTiles = %this.towerGrid.getCellCount();
   %this.owner.numTilesX = getWord(%numTiles, 0);
   %this.owner.numTilesY = getWord(%numTiles, 1);
   
   %this.owner.tileSizeX = %this.towerGrid.getWidth() / %this.owner.numTilesX;
   %this.owner.tileSizeY = %this.towerGrid.getHeight() / %this.owner.numTilesY;
   %this.owner.yOffset = calculateYOffset(%this.owner, %this.owner.tileSizeY);

   %tileLayerPos = %this.towerGrid.getPosition();
   %this.owner.tileLayerPosX = getWord(%tileLayerPos, 0);
   %this.owner.tileLayerPosY = getWord(%tileLayerPos, 1);
   %this.owner.tileLayerSizeX = %this.towerGrid.getSizeX();
   %this.owner.tileLayerSizeY = %this.towerGrid.getSizeY();

   %tileLayerCounts = %this.towerGrid.getCellCount();
   %this.owner.tileLayerCountX = getWord(%tileLayerCounts, 0);
   %this.owner.tileLayerCountY = getWord(%tileLayerCounts, 1);

   %this.owner.tileSize = %this.calculateTileSize();
}

/// <summary>
/// This handles verifying that the current drop location is valid, places the tower if it is or deletes it if not, 
/// purchasing the tower and playing the placement sound if it is placed, and hiding the range display object.
/// </summary>
/// <param name="touchID">The index of the touch event associated with this call.</param>
/// <param name="worldPos">The world position of the touch event.</param>
function TowerDragBehavior::onTouchUp(%this, %touchID, %worldPos)
{
   if (%touchID !$= %this.touchID)
      return;
      
   %this.touchID = "";
   
   %this.RangeObj.setVisible(false);

   %this.createButton.callOnBehaviors("onTouchUp", %touchID);
   
   tileHighlight(false);

   if (%this.owner.inValidPos)
      return;
      
   if ($OnCancelTower)
      cancelTower.onButtonUp();

   %tileList = testTowerPlacement(%this.owner, %worldPos, %this.owner.tileSize);
   if(%tileList !$= "")
   {
      // Drop the tower at the current world position.
      // TODO: Snap to grid position
      
      // Snap tower to position
      
      %tileListCount = getWordCount(%tileList) / 2;
      %posX = 0;
      %posY = 0;
      for (%i = 0; %i < %tileListCount; %i++)
      {
         %tx = getWord(%tileList, %i*2);
         %ty = getWord(%tileList, %i*2+1);
         %this.towerGrid.setCellWeight(%tx, %ty, 0);
         
         %wp = %this.towerGrid.getCellWorldPosition(%tx, %ty);
         
         %posX += getWord(%wp, 0);
         %posY += getWord(%wp, 1);
      }
      
      %posX = %posX / %tileListCount;
      %posY = %posY / %tileListCount;
      %pos = %posX SPC %posY;
      
      %this.owner.centerPoint = %pos;
      %this.owner.Position = Vector2Add(%pos, %this.owner.yOffset);
      
      %this.owner.BlendColor = $Towers::PlacedColor;
      
      %this.owner.canFire = true;
      %this.owner.inValidPos = true;
      
      //Play placement sound
      if($TowerPlacementSound !$= "None" && $TowerPlacementSound !$= "")
      {
         //echo(" -- playing : " @ %this.PlacementSound);
         alxPlay($TowerPlacementSound);
      }
      
      %this.owner.callOnBehaviors(buy);
      
      $TowerBeingPlaced = "";
         
      cancelTower.Visible = false;
      
      %this.owner.setSceneLayer(%this.placeLayer);
         
      // Activate the tower
      %this.owner.callOnBehaviors(toggle, true);
      
      // Remove the TowerDragBehavior behavior so that towers are permanent placements.
      %this.owner.schedule(0, removeBehavior, %this, false);

      //echo(" -- TowerDragBehavior::onTouchUp() : " @ %this.owner @ " : " @ %this SPC %touchID SPC %worldPos);
   }
   else
   {
      cancelTower.onButtonUp();
      
      if($TowerMisplacementSound !$= "None" && $TowerMisplacementSound !$= "")
      {
         //echo(" -- playing : " @ %this.PlacementSound);
         alxPlay($TowerMisplacementSound);
      }
   }
}

/// <summary>
/// This handles verifying that the current location under the tower is a valid drop location, updating the color of 
/// the range display to indicate validity of the current location and updating the position of the tower and range 
/// display as they are dragged
/// </summary>
/// <param name="touchID">The index of the touch event associated with this call.</param>
/// <param name="worldPos">The world position of the touch event.</param>
function TowerDragBehavior::onTouchDragged(%this, %touchID, %worldPos)
{
   //echo(" -- TowerDragBehavior::onTouchDragged(): " @ %this SPC %touchID SPC %worldPos);
   if (%touchID !$= %this.touchID)
       return;
      
   %graphicsTile = GraphicsGrid.getCellFromWorldPosition(%worldPos);
   %graphicsTileWorldPos = GraphicsGrid.getCellWorldPosition(%graphicsTile.x, %graphicsTile.y);
   %newOwnerPos = Vector2Add(%graphicsTileWorldPos, %this.owner.yOffset);

   if (%this.DragInX)      
      %this.owner.setPositionX(getWord(%newOwnerPos, 0));
      
   if (%this.DragInY)      
      %this.owner.setPositionY(getWord(%newOwnerPos, 1));

   %this.owner.BlendColor = $Towers::DraggingColor;

   if (isObject(%this.RangeObj))
   {
      %position = %this.owner.getPosition();
      %x = getWord(%position, 0);
      %y = getWord(%position, 1);
      %this.RangeObj.setPosition(%x, %y);
      
      %tilePos = $TowerPlaceGrid.getCellFromWorldPosition(%worldPos);
      if ($TowerPlaceGrid.isCellBlocked(getWord(%tilePos, 0), getWord(%tilePos, 1)))
         %this.RangeObj.setBlendColor($Towers::RangeNoDropColor);      
      else
         %this.RangeObj.setBlendColor($Towers::RangeDropColor);
   }
   
   //determineLayer(testTowerPlacement(%this.owner, %worldPos, %this.owner.tileSize));
}

/// <summary>
/// This helps to ensure that the tower is not inadvertently dropped when the touch event leaves the
/// physical bounds of the object.
/// </summary>
/// <param name="touchID">The index of the touch event associated with this call.</param>
/// <param name="worldPos">The world position of the touch event.</param>
function TowerDragBehavior::onTouchLeave(%this, %touchID, %worldPos)
{
   if (%touchID !$= %this.touchID || %touchID $= "")
       return;
   
   sceneWindow2D.addLockedObject(%this);
}

/// <summary>
/// This reaquires the object for dragging if the touch event has previously left the 
/// physical bounds of the object before the onTouchUp event is called.
/// </summary>
/// <param name="touchID">The index of the touch event associated with this call.</param>
/// <param name="worldPos">The world position of the touch event.</param>
function TowerDragBehavior::onTouchEnter(%this, %touchID, %worldPos)
{
   if (%touchID !$= %this.touchID || %touchID $= "")
       return;
       
   sceneWindow2D.removeLockedObject(%this);
}

/// <summary>
/// This calculates the size of the tower's 'footprint' on the placement grid.  At the moment this is 
/// not really used since all towers only take up one grid square, but we kept it in case this changes again.
/// </summary>
/// <return>Returns the X and Y tile count of the current tower's footprint</return>
function TowerDragBehavior::calculateTileSize(%this)
{
   // Determine the number of tiles the owner takes up
   // will have to adjust this based on the idea that we only occupy one tile
   // even if we're more than one tile high.  Don't know about width yet....
   %ownerSizeX = 1; //%this.owner.getSizeX();
   %ownerSizeY = 1; //%this.owner.getSizeY();
   
   %tilesX = mFloor(%ownerSizeX / %this.owner.tileSizeX);
   if (%ownerSizeX % %this.owner.tileSizeX > 0)
      %tilesX++;
   %tilesY = mFloor(%ownerSizeY / %this.owner.tileSizeY);
   if (%ownerSizeY % %this.owner.tileSizeY > 0)
      %tilesY++;
   
   return %tilesX SPC %tilesY;
}

/// <summary>
/// This calculates the vertical offset for tower placement to align the bottom edge of the 
/// tower with the bottom edge of its placement square.
/// </summary>
/// <param name="obj">The tower being placed.</param>
/// <param name="tileHeight">The Y axis dimension of the tile that the tower is being placed in.</param>
/// <return>Returns the offset to use when placing the tower in question</return>
function calculateYOffset(%obj, %tileHeight)
{
   %height = %obj.getSizeY();
   return 0 SPC ((%height - %tileHeight) / 2);
}

/// <summary>
/// This function checks the current tile to see if it is a valid tower placement location.
/// This originally was capable of testing to ensure that blocking the target location did not
/// block off all valid paths from to the enemy destination location.
/// </summary>
/// <param name="obj">The tower being placed.</param>
/// <param name="worldPos">The world position of the touch event.</param>
/// <param name="tileSize">The world unit size of a square tile.</param>
/// <param name="notTower">This should be true if the object being placed is not a tower - it won't be tested against the tower placement grid.</param>
/// <return>Returns a list representing valid placement locations</return>
function testTowerPlacement(%obj, %worldPos, %tileSize, %notTower)
{
   //echo("!!! testTowerPlacement( " @ %obj @ " : " @ %worldPos @ " : " @ %tileSize @ ")");

   %tileList = calculateTilesInvolved(%obj, %worldPos, %tileSize);
   %tileListCount = getWordCount(%tileList) / 2;

   // If any tile space is already set as a blocker then we cannot drop here
   for (%i = 0; %i < %tileListCount; %i++)
   {
      %prevCustomData[%i] = $TowerPlaceGrid.getCellWeight(getWord(%tileList, %i*2), getWord(%tileList, %i*2+1));
      if (%prevCustomData[%i] != 1 && !%notTower)
      {
         //echo("!!! testTowerPlacement(): Tile is already a blocker! : " @ %prevCustomData[i] @ " @ " @ %worldPos);
         return "";
      }
   }

   // If we got this far then we're good to go
   if (%tileList !$= "")
   {
      //echo(" -- testTowerPlacement returned true : " @ %tileList);
      return %tileList;
   }
   else
   {
      //echo(" -- testTowerPlacement returned false : " @ %tileList);
      return "";
   }
}

/// <summary>
/// ! Deprecated !
/// This function uses a list of tiles that use their tile row to determine which scene layer to render the tower on.
/// </summary>
/// <param name="tileList">The tiles we wish to test for render sorting.</param>
/// <return>Returns an integer representing the target scene render layer.</return>
function determineLayer(%tileList)
{
   // get the row we're on
   %currentRow = getWord(%tileList, 1);
   //echo(" -- current row : " @ %currentRow);
   %numTiles = $TowerPlaceGrid.getCellCount();
   %totalRows = getWord(%numTiles, 1);
   
   %targetLayer = 2 + ((%totalRows - %currentRow) * 2);
   //echo(" -- Target Layer : " @ %targetLayer);
   return %targetLayer;
}

/// <summary>
/// This function toggles highlighting on the tiles where towers can be placed.  Note that this
/// highlights valid tiles with the same color as the range circle - $Towers::RangeDropColor.
/// </summary>
/// <param name="highlight">True to show the highlight, false to hide it.</param>
function tileHighlight(%highlight)
{
    if (!isObject(HighlightObject))
        echo(" -- Highlight Object Missing!");
    // cycle through the $towerGrid and highlight valid tower placement squares
    %numTiles = $TowerPlaceGrid.getCellCount();

    if (!TowerPlacementGrid.tilesX)
        TowerPlacementGrid.tilesX = getWord(%numTiles, 0);
    if (!TowerPlacementGrid.tilesY)
        TowerPlacementGrid.tilesY = getWord(%numTiles, 1);
   
   if (%highlight)
   {
      // show placement grid
      for (%x = 0; %x < TowerPlacementGrid.tilesX; %x++)
      {
         for (%y = 0; %y < TowerPlacementGrid.tilesY; %y++)
         {
            if ($TowerPlaceGrid.getCellWeight(%x, %y) == 1)
            {
               if (!$Towers::PlacementHighlight[%x, %y])
               {
                  $Towers::PlacementHighlight[%x, %y] = HighlightObject.clone();
                  %sizeX = 0.9375;
                  %sizeY = 0.9375;
                  %newSize = %sizeX SPC %sizeY;
                  //echo(" -- Tower Highlight: "@$Towers::PlacementHighlight[%x, %y]@", "@%x@", "@%y);
                  $Towers::PlacementHighlight[%x, %y].size = %newSize;
                  $Towers::PlacementHighlight[%x, %y].BlendColor = $Towers::RangeDropColor;

                  %wp = $TowerPlaceGrid.getCellWorldPosition(%x, %y);
                      
                  %posX = getWord(%wp, 0);
                  %posY = getWord(%wp, 1);
                  %pos = %posX SPC %posY;
                  $Towers::PlacementHighlight[%x, %y].Position = %wp;
                  
                  MainScene.addToScene($Towers::PlacementHighlight[%x, %y]);
               }               
               $Towers::PlacementHighlight[%x, %y].Visible = true;
            }
         }
      }
   }
   else
   {
      // hide placement grid
      for (%x = 0; %x < TowerPlacementGrid.tilesX; %x++)
      {
         for (%y = 0; %y < TowerPlacementGrid.tilesY; %y++)
         {
               $Towers::PlacementHighlight[%x, %y].Visible = false;
         }
      }
   }
}

/// <summary>
/// This function returns a list of tiles that will be affected by tower placement at the given
/// world coordinates in the given size.
/// </summary>
/// <param name="obj">The tower being placed.</param>
/// <param name="worldPos">The world position of the tower placement.</param>
/// <param name="tileCounts">The number of tiles in the X and Y directions of the tower's 'footprint'.</param>
/// <return>Returns a list of tiles involved in placing the tower at the current world position.</return>
function calculateTilesInvolved(%obj, %worldPos, %tileCounts)
{
   %tileCountX = getWord(%tileCounts, 0);
   %tileCountY = getWord(%tileCounts, 1);
   
   // Find out the number of tiles we take up in the X and Y directions
   %tileCheckCountX = 0;
   %tileFudgeAmountX = 0;
   if (%tileCountX > 1)
   {
      %tileCheckCountX = %tileCountX - 1;
      // 0.01 is a fudge factor to make sure we don't end up right on a tile boundary
      // Tweaked the fudge amount from 0.1 to "loosen" snapping.
      %tileFudgeAmountX = 0.002; 
   }
   
   %tileCheckCountY = 0;
   %tileFudgeAmountY = 0;
   if (%tileCountY > 1)
   {
      %tileCheckCountY = %tileCountY - 1;
      %tileFudgeAmountY = 0.002;
   }
   
   // Calculate the world position of the top left tile
   %posX = getWord(%worldPos, 0);
   %posY = getWord(%worldPos, 1);
   
   %checkPosX = %posX + (%tileCheckCountX * %obj.tileSizeX * 0.5) + %tileFudgeAmountX; 
   %checkPosY = %posY + (%tileCheckCountY * %obj.tileSizeY * 0.5) + %tileFudgeAmountY;
   
   // Make sure we're within the bounds of the tile set
   %tileLayerBoundsLeft = %obj.tileLayerPosX - %obj.tileLayerSizeX;
   %tileLayerBoundsTop = %obj.tileLayerPosY - %obj.tileLayerSizeY;
   
   if (%checkPosX < %tileLayerBoundsLeft)
   {
      // Shift our check position so that we're within bounds
      %checkPosX = %tileLayerBoundsLeft + %obj.tileSizeX * 0.5 - %tileFudgeAmountX;
   }
   if (%checkPosY < %tileLayerBoundsTop)
   {
      // Shift our check position so that we're within bounds
      %checkPosY = %tileLayerBoundsTop + %obj.tileSizeY * 0.5 - %tileFudgeAmountY;
   }
   
   //echo("!!! World pos: " @ %worldPos @ "  Check pos: " @ %checkPosX SPC %checkPosY);
   
   // Get our tile
   %point = %checkPosX SPC %checkPosY;
   %tile = $TowerPlaceGrid.getCellFromWorldPosition(%point);
   
   // We now have our top left tile.  If we cover more than one tile then make
   // sure the other side is also in bounds
   %tileLeft = getWord(%tile, 0);
   %tileTop = getWord(%tile, 1);

   if ( (%tileLeft + %tileCountX - 1) > (%obj.tileLayerCountX - 1) )
   {
      // We've gone beyond the right side.  Need to pull back.
      %tileLeft = %obj.tileLayerCountX - %tileCountX;
   }
   
   if ( (%tileTop + %tileCountY - 1) > (%obj.tileLayerCountY - 1) )
   {
      // We've gone beyond the bottom side.  Need to pull back.
      %tileTop = %obj.tileLayerCountY - %tileCountY;
   }
   
   // Now build our tile list
   %tileList = "";
   %first = true;
   for (%i = 0; %i < %tileCountX; %i++)
   {
      for (%j = 0; %j < %tileCountY; %j++)
      {
         if (!%first)
         {
            %tileList = %tileList @ " ";
         }
         else
         {
            %first = false;
         }
         
         %tileList = %tileList @ %tileLeft + %i SPC %tileTop + %j;
      }
   }
   
   return %tileList;
}
