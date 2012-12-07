//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------



//--------------------------------
// Terrain Tool Help
//--------------------------------
/// <summary>
/// This function opens a browser and navigates to the Tower Defense Template 
/// Terrain Tool help page.
/// </summary>
function TerrainToolHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/towerdefense/terrain/");
}


/// <summary>
/// This function handles initializing the tool on wake.
/// </summary>
function TerrainTool::onWake(%this)
{
   if (!%this.toolInit)
   {
      // Selected Tab
      %this.GRAPHICSTAB = 0;
      %this.PATHSTAB = 1;
      %this.TOWERSTAB = 2;
      
      // Actions
      %this.GRAPHICSEDIT = 1;
      %this.TOWEREDIT = 2;
      %this.PATHEDIT = 3;
      
      // Operations
      %this.PAINTOPERATION = 1;
      %this.ERASEOPERATION = 2;
      %this.FILLOPERATION = 3;
      
      // Primary cell types
      %this.CELLINVALID = 0;
      %this.CELLEMPTY = 1;
      %this.CELLEMPTYNOTOWER = 2;
      %this.CELLEMPTYPATHENDPOINTONLY = 3;
      %this.CELLTOWER = 4;
      %this.CELLPATH = 5;
      %this.CELLPATHENDPOINTONLY = 6;
      
      // Secondary cell types
      %this.CELLPATHSTART = 1;
      %this.CELLPATHPATH = 2;
      %this.CELLPATHEND = 3;
      
      // Images
      %this.NOIMAGE = 0;
      %this.TOWERCELLIMAGE = 1;
      %value = 2;
      for (%i=0; %i<4; %i++)
      {
         %this.PATHSTARTIMAGE[%i] = %value;
         %value++;
         %this.PATHIMAGE[%i] = %value;
         %value++;
         %this.PATHENDIMAGE[%i] = %value;
         %value++;
      }
      
      // Quad Images
      %this.TOWERCELLIMAGEQUAD = 101;
      %this.PATH01STARTIMAGEQUAD = 111;
      %this.PATH01IMAGEQUAD = 121;
      %this.PATH01ENDIMAGEQUAD = 131;
      %this.PATH02STARTIMAGEQUAD = 141;
      %this.PATH02IMAGEQUAD = 151;
      %this.PATH02ENDIMAGEQUAD = 161;
      %this.PATH03STARTIMAGEQUAD = 171;
      %this.PATH03IMAGEQUAD = 181;
      %this.PATH03ENDIMAGEQUAD = 191;
      %this.PATH04STARTIMAGEQUAD = 201;
      %this.PATH04IMAGEQUAD = 211;
      %this.PATH04ENDIMAGEQUAD = 221;
      
      // Used to offset path image indices when they need to fade
      %this.PATHFADEOFFSET = 1000;
      
      // Path paint brushes
      %this.PATHBRUSHNONE = 0;
      %this.PATHBRUSHSTART = 1;
      %this.PATHBRUSHPATH = 2;
      %this.PATHBRUSHEND = 3;
      
      // Operation buttons
      %this-->PaintButton.operation = %this.PAINTOPERATION;
      %this-->FillButton.operation = %this.FILLOPERATION;
      %this-->EraseButton.operation = %this.ERASEOPERATION;
      
      // Set up a a new scene
      %this.scene = new Scene();
      
      // Graphics tab
      %this.buildGraphicsBrushes();
      %this-->GraphicsTabScroll.scrollToTop();
      %this.graphicsShowPath = false;
      %this.graphicsShowTower = false;
      
      // Paths tab
      %this.pathsShowPath = true;
      %this.pathsShowTower = true;
      
      // Towers tab
      %this.buildTowersBrushes();
      %this.towersShowPath = true;
      %this.towersShowTower = true;
      
      // Some default text
      %this-->PreviewText.setText("");
      
      %this.toolInit = true;
   }

   // Paths tab always needs to be regenerated to catch the grid names
   %this.buildPathsBrushes();

   // Tracks the grid that is being painted
   %this.currentGrid = -1;
   %this.currentGridIndex = -1;
   
   // Tracks what is being painted for a grid
   %this.currentGridBrush = %this.PATHBRUSHNONE;
   
   // Tracks what is being painted for graphics tab
   %this.clearCurrentGraphicsBrush();
   
   // Make the Graphics tab the default one
   %this-->BrushesTabBook.selectPage(0);
   %this.tabSelected = %this.GRAPHICSTAB;
   
   // Default operation
   %this-->PaintButton.performClick();
   
   // Prepare the undo buffers
   %this.setupUndo();
   
   // When the tool opens add the display grid
   if (isObject(TerrainDisplayGrid))
      TerrainDisplayGrid.delete();
   
   // The grid we use as a basis for the display grid
   %copyFromGrid = EnemyPathGrid01;
   
   // The display grid has twice as many cells in each direction to support
   // displaying the icons for multiple paths in each path cell.
   %copyFromGridCellCount = %copyFromGrid.getCellCount();
   %numCells = (getWord(%copyFromGridCellCount, 0)*2) @ " " @ (getWord(%copyFromGridCellCount, 1)*2);
   
   %grid = new Grid(TerrainDisplayGrid)
      {
         Position = %copyFromGrid.getPosition();
         size = %copyFromGrid.getSize();
         cellCount = %numCells;
         defaultCellInteger = "0";
         renderCellImages = "1";
         noCellSerialize = "1";
         renderGrid = "0";
         renderGridStep = "2";
         visible = "1";
      };
      
   %this.buildDisplayGridImages(%grid);
   %this.buildDisplayGrid(%grid);
   
   %scene = ToolManager.getLastWindow().getScene();
   %scene.addToScene(%grid);
   
   %this.setPathGridDirtyState(false);
   %this.setGlobalDirtyState(false);
   
   // Make our tool active
   if (!isObject(LevelEditorTDTerrainTool))
   {
      %tool = new LevelBuilderMouseScriptTool(LevelEditorTDTerrainTool);
      ToolManager.addTool(LevelEditorTDTerrainTool);
   }
   LevelBuilderToolManager::setTool(LevelEditorTDTerrainTool);
}

/// <summary>
/// This function handles cleanup when the tool is closed.
/// </summary>
function TerrainTool::onSleep(%this)
{
   // Remove our tool
   if (isObject(LevelEditorTDTerrainTool))
   {
      LevelEditorTDTerrainTool.endTool();
      ToolManager.removeTool(LevelEditorTDTerrainTool);
      LevelEditorTDTerrainTool.delete();
   }
   
   // Remove the GraphicsGrid grid, if any
   GraphicsGrid.renderGrid = 0;
   
   // Remove the TowerPlacementGrid grid, if any
   TowerPlacementGrid.renderGrid = 0;
   
   // Remove the display grid
   if (isObject(TerrainDisplayGrid))
      TerrainDisplayGrid.delete();
   
   // Discard any undo buffers
   %this.discardUndo();
   
   // Save the graphics brushes
   LBProjectObj.persistToDisk(false, false, false, true, false);
}

/// <summary>
/// This function registers the tool with the Asset Library so that it will be notified
/// if any assets that are in use in the terrain are deleted.
/// </summary>
function TerrainTool::onAdd(%this)
{ 
   // Register with the Asset Library that we're interested
   // in knowning when an asset is deleted.  It could be one
   // of our brush's image datablocks.      
   AssetLibrary.registerTemplateToolForCallbacks(%this);
}

/// <summary>
/// This function handles final cleanup when the tool is destroyed.
/// </summary>
function TerrainTool::onRemove(%this)
{
   if (isObject(%this.scene))
      %this.scene.delete();
}

/// <summary>
/// This function handles deletion of active assets in the Asset Library.
/// </summary>
/// <param name="asset">The name of the asset that was deleted.</param>
/// <param name="type">The type of asset that was deleted.</param>
function TerrainTool::onAssetDeleted(%this, %asset, %type)
{
   if ($LoadedTemplate.Name !$= "TowerDefense")
      return;  
      
   //echo("@@@ TerrainTool::onAssetDeleted: " @ %asset SPC %type);
   
   // Start by removing all graphic brushes that use this datablock
   %this.removeGraphicsBrush(%asset);
   
   // Now tell the graphics grid to remove the image datablock
   %result = GraphicsGrid.removeImageDatablock(%asset);
   
   // If the GraphicsGrid had to make any internal changes due
   // to removing the datablock, then we need to save it out.
   // Only rebuild the undo buffers if the tool is being used.
   %this.saveTerrainData(%this.isAwake());
}

/// <summary>
/// This function handles closing the terrain tool.
/// </summary>
function TerrainTool::onCloseButton(%this)
{
   if (TerrainToolDoneButton.isActive())
      TerrainToolDoneButton.onClick();
}

/// <summary>
/// This function handles the tool's dirty state to keep track of the need to save data.
/// </summary>
/// <param name="dirty">True if data has been changed, false if not.</param>
function TerrainTool::setGlobalDirtyState(%this, %dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      TerrainToolWindow.setText("Terrain Tool *");
   else
      TerrainToolWindow.setText("Terrain Tool");
}

/// <summary>
/// This function handles the grid's dirty state to keep track of changes to the terrain grids.
/// </summary>
/// <param name="dirty">Sets the tool dirty if true, clean if false.</param>
function TerrainTool::setPathGridDirtyState(%this, %dirty)
{
   %this.dirtyPathGrid = %dirty;
}

/// <summary>
/// This function accesses the path grid dirty state.
/// </summary>
function TerrainTool::isPathGridDirty(%this)
{
   return %this.dirtyPathGrid;
}

/// <summary>
/// This function prepares the terrain undo system.
/// </summary>
function TerrainTool::setupUndo(%this)
{
   // Discard any previous undo buffers
   if (%this.undoBuffersActive)
   {
      %this.discardUndo();
   }
   
   // Graphics grid
   %this.undoGraphicsGrid = new Grid();
   GraphicsGrid.copyCellWeightsToGrid(%this.undoGraphicsGrid);
   GraphicsGrid.copyCellIntegersToGrid(%this.undoGraphicsGrid);
   
   // Path grids
   for (%i=1; %i<=4; %i++)
   {
      %this.undoPathGrid[%i] = new Grid();
      %grid = "EnemyPathGrid0" @ %i;
      (%grid).copyCellWeightsToGrid(%this.undoPathGrid[%i]);
      (%grid).copyCellIntegersToGrid(%this.undoPathGrid[%i]);
   }
   
   // Tower grid
   %this.undoTowersGrid = new Grid();
   TowerPlacementGrid.copyCellWeightsToGrid(%this.undoTowersGrid);
   TowerPlacementGrid.copyCellIntegersToGrid(%this.undoTowersGrid);
   
   %this.undoBuffersActive = true;
}

/// <summary>
/// This function cleans out the undo buffer.
/// </summary>
function TerrainTool::discardUndo(%this)
{
   if (%this.undoBuffersActive)
   {
      %this.undoBuffersActive = false;
      
      // Graphics grid
      %this.undoGraphicsGrid.delete();
      %this.undoGraphicsGrid = 0;
      
      // Path grids
      for (%i=1; %i<=4; %i++)
      {
         %this.undoPathGrid[%i].delete();
         %this.undoPathGrid[%i] = 0;
      }

      // Tower grid
      %this.undoTowersGrid.delete();
      %this.undoTowersGrid = 0;
   }
}

/// <summary>
/// This function reverts the tool to it's last known pre-change state.
/// </summary>
function TerrainTool::applyUndo(%this)
{
   if (%this.undoBuffersActive)
   {
      // Graphics grid
      %this.undoGraphicsGrid.copyCellWeightsToGrid(GraphicsGrid);
      %this.undoGraphicsGrid.copyCellIntegersToGrid(GraphicsGrid);
   
      // Path grids
      for (%i=1; %i<=4; %i++)
      {
         %grid = "EnemyPathGrid0" @ %i;
         %this.undoPathGrid[%i].copyCellWeightsToGrid(%grid);
         %this.undoPathGrid[%i].copyCellIntegersToGrid(%grid);
      }
   
      // Tower grid
      %this.undoTowersGrid.copyCellWeightsToGrid(TowerPlacementGrid);
      %this.undoTowersGrid.copyCellIntegersToGrid(TowerPlacementGrid);
   }
}

/// <summary>
/// This function saves the terrain grid data.
/// </summary>
/// <param name="rebuildUndo">If true, sets up a clean undo set after clearing the existing one.  Does not set up a fresh undo set if false.</param>
function TerrainTool::saveTerrainData(%this, %rebuildUndo)
{
   // Remove the display grid from the scene graph so it isn't saved.
   %scene = ToolManager.getLastWindow().getScene();
   if (isObject(TerrainDisplayGrid))
   {
      %scene.removeFromScene(TerrainDisplayGrid);
   }

   // Remove the GraphicsGrid grid, if any
   %graphicsGridGrid = GraphicsGrid.renderGrid;
   GraphicsGrid.renderGrid = 0;

   // Remove the TowerPlacementGrid grid, if any
   %towersGridGrid = TowerPlacementGrid.renderGrid;
   TowerPlacementGrid.renderGrid = 0;
   
   // Copy all enemy path names to the grids
   %count = TerrainTool-->PathsBrushesArray.getCount();
   for (%i=0; %i<%count; %i++)
   {
      %obj = TerrainTool-->PathsBrushesArray.getObject(%i);
      %grid = "EnemyPathGrid0" @ (%i+1);
      (%grid).setInternalName(%obj-->NameEditField.getText());
   }

   // Save the level
   LBProjectObj.saveLevel();

   // Put back the GraphicsGrid grid, if any
   GraphicsGrid.renderGrid = %graphicsGridGrid;

   // Put back the TowerPlacementGrid grid, if any
   TowerPlacementGrid.renderGrid = %towersGridGrid;

   // Put back the display grid
   if (isObject(TerrainDisplayGrid))
   {
      %scene.addToScene(TerrainDisplayGrid);
      
      // Do a little dance here to force the scene graph to update properly.
      // Otherwise, our display grid never comes back!
      TerrainDisplayGrid.setPosition(TerrainDisplayGrid.getPosition());
   }
   
   // The saveLevel() call above makes the standard TGB selection tool the active
   // one, which deactivates our tool.  Make our tool the active one again.
   LevelBuilderToolManager::setTool(LevelEditorTDTerrainTool);

   %this.setGlobalDirtyState(false);
   %this.discardUndo();
   if (%rebuildUndo)
   {
      %this.setupUndo();
   }
}

/// <summary>
/// This function handles discarding terrain changes on tool close.
/// </summary>
function TerrainTool::doNotSaveTerrainData(%this)
{
   %this.applyUndo();
   %this.discardUndo();
   %this.setGlobalDirtyState(false);
}

/// <summary>
/// This function disables the Done button so that it can't be clicked multiple times during
/// save of the terrain data.
/// </summary>
/// <param name="state">True to enable the Done button, false to disable it.</param>
function TerrainTool::setSaveCloseState(%this, %state)
{
   TerrainToolDoneButton.setActive(%state);
}

/// <summary>
/// This function builds the grid display image sets for displaying tower and path
/// information on the path grids.
/// </summary>
/// <param name="grid">The grid to prepare the images for.</param>
function TerrainTool::buildDisplayGridImages(%this, %grid)
{
   // Full sized images
   %grid.addImage(%this.TOWERCELLIMAGE, TDTerrainPathTImageMap, 0);
   %grid.addImage(%this.TOWERCELLIMAGE+%this.PATHFADEOFFSET, TDTerrainPathTFadeImageMap, 0);
   
   %grid.addImage(%this.PATHSTARTIMAGE[0], TDTerrainPathS1ImageMap, 0);
   %grid.addImage(%this.PATHSTARTIMAGE[0]+%this.PATHFADEOFFSET, TDTerrainPathS1FadeImageMap, 0);
   %grid.addImage(%this.PATHIMAGE[0], TDTerrainPathP1ImageMap, 0);
   %grid.addImage(%this.PATHIMAGE[0]+%this.PATHFADEOFFSET, TDTerrainPathP1FadeImageMap, 0);
   %grid.addImage(%this.PATHENDIMAGE[0], TDTerrainPathE1ImageMap, 0);
   %grid.addImage(%this.PATHENDIMAGE[0]+%this.PATHFADEOFFSET, TDTerrainPathE1FadeImageMap, 0);
   
   %grid.addImage(%this.PATHSTARTIMAGE[1], TDTerrainPathS2ImageMap, 0);
   %grid.addImage(%this.PATHSTARTIMAGE[1]+%this.PATHFADEOFFSET, TDTerrainPathS2FadeImageMap, 0);
   %grid.addImage(%this.PATHIMAGE[1], TDTerrainPathP2ImageMap, 0);
   %grid.addImage(%this.PATHIMAGE[1]+%this.PATHFADEOFFSET, TDTerrainPathP2FadeImageMap, 0);
   %grid.addImage(%this.PATHENDIMAGE[1], TDTerrainPathE2ImageMap, 0);
   %grid.addImage(%this.PATHENDIMAGE[1]+%this.PATHFADEOFFSET, TDTerrainPathE2FadeImageMap, 0);
   
   %grid.addImage(%this.PATHSTARTIMAGE[2], TDTerrainPathS3ImageMap, 0);
   %grid.addImage(%this.PATHSTARTIMAGE[2]+%this.PATHFADEOFFSET, TDTerrainPathS3FadeImageMap, 0);
   %grid.addImage(%this.PATHIMAGE[2], TDTerrainPathP3ImageMap, 0);
   %grid.addImage(%this.PATHIMAGE[2]+%this.PATHFADEOFFSET, TDTerrainPathP3FadeImageMap, 0);
   %grid.addImage(%this.PATHENDIMAGE[2], TDTerrainPathE3ImageMap, 0);
   %grid.addImage(%this.PATHENDIMAGE[2]+%this.PATHFADEOFFSET, TDTerrainPathE3FadeImageMap, 0);
   
   %grid.addImage(%this.PATHSTARTIMAGE[3], TDTerrainPathS4ImageMap, 0);
   %grid.addImage(%this.PATHSTARTIMAGE[3]+%this.PATHFADEOFFSET, TDTerrainPathS4FadeImageMap, 0);
   %grid.addImage(%this.PATHIMAGE[3], TDTerrainPathP4ImageMap, 0);
   %grid.addImage(%this.PATHIMAGE[3]+%this.PATHFADEOFFSET, TDTerrainPathP4FadeImageMap, 0);
   %grid.addImage(%this.PATHENDIMAGE[3], TDTerrainPathE4ImageMap, 0);
   %grid.addImage(%this.PATHENDIMAGE[3]+%this.PATHFADEOFFSET, TDTerrainPathE4FadeImageMap, 0);
   
   // Images broken up into quads
   for (%i=0; %i<4; %i++)
   {
      %grid.addImage(%this.TOWERCELLIMAGEQUAD+%i, TDTerrainPathTQuadImageMap, %i);
      %grid.addImage(%this.TOWERCELLIMAGEQUAD+%this.PATHFADEOFFSET+%i, TDTerrainPathTFadeQuadImageMap, %i);
      
      %grid.addImage(%this.PATH01STARTIMAGEQUAD+%i, TDTerrainPathS1QuadImageMap, %i);
      %grid.addImage(%this.PATH01STARTIMAGEQUAD+%this.PATHFADEOFFSET+%i, TDTerrainPathS1FadeQuadImageMap, %i);
      %grid.addImage(%this.PATH01IMAGEQUAD+%i, TDTerrainPathP1QuadImageMap, %i);
      %grid.addImage(%this.PATH01IMAGEQUAD+%this.PATHFADEOFFSET+%i, TDTerrainPathP1FadeQuadImageMap, %i);
      %grid.addImage(%this.PATH01ENDIMAGEQUAD+%i, TDTerrainPathE1QuadImageMap, %i);
      %grid.addImage(%this.PATH01ENDIMAGEQUAD+%this.PATHFADEOFFSET+%i, TDTerrainPathE1FadeQuadImageMap, %i);
      
      %grid.addImage(%this.PATH02STARTIMAGEQUAD+%i, TDTerrainPathS2QuadImageMap, %i);
      %grid.addImage(%this.PATH02STARTIMAGEQUAD+%this.PATHFADEOFFSET+%i, TDTerrainPathS2FadeQuadImageMap, %i);
      %grid.addImage(%this.PATH02IMAGEQUAD+%i, TDTerrainPathP2QuadImageMap, %i);
      %grid.addImage(%this.PATH02IMAGEQUAD+%this.PATHFADEOFFSET+%i, TDTerrainPathP2FadeQuadImageMap, %i);
      %grid.addImage(%this.PATH02ENDIMAGEQUAD+%i, TDTerrainPathE2QuadImageMap, %i);
      %grid.addImage(%this.PATH02ENDIMAGEQUAD+%this.PATHFADEOFFSET+%i, TDTerrainPathE2FadeQuadImageMap, %i);
      
      %grid.addImage(%this.PATH03STARTIMAGEQUAD+%i, TDTerrainPathS3QuadImageMap, %i);
      %grid.addImage(%this.PATH03STARTIMAGEQUAD+%this.PATHFADEOFFSET+%i, TDTerrainPathS3FadeQuadImageMap, %i);
      %grid.addImage(%this.PATH03IMAGEQUAD+%i, TDTerrainPathP3QuadImageMap, %i);
      %grid.addImage(%this.PATH03IMAGEQUAD+%this.PATHFADEOFFSET+%i, TDTerrainPathP3FadeQuadImageMap, %i);
      %grid.addImage(%this.PATH03ENDIMAGEQUAD+%i, TDTerrainPathE3QuadImageMap, %i);
      %grid.addImage(%this.PATH03ENDIMAGEQUAD+%this.PATHFADEOFFSET+%i, TDTerrainPathE3FadeQuadImageMap, %i);
      
      %grid.addImage(%this.PATH04STARTIMAGEQUAD+%i, TDTerrainPathS4QuadImageMap, %i);
      %grid.addImage(%this.PATH04STARTIMAGEQUAD+%this.PATHFADEOFFSET+%i, TDTerrainPathS4FadeQuadImageMap, %i);
      %grid.addImage(%this.PATH04IMAGEQUAD+%i, TDTerrainPathP4QuadImageMap, %i);
      %grid.addImage(%this.PATH04IMAGEQUAD+%this.PATHFADEOFFSET+%i, TDTerrainPathP4FadeQuadImageMap, %i);
      %grid.addImage(%this.PATH04ENDIMAGEQUAD+%i, TDTerrainPathE4QuadImageMap, %i);
      %grid.addImage(%this.PATH04ENDIMAGEQUAD+%this.PATHFADEOFFSET+%i, TDTerrainPathE4FadeQuadImageMap, %i);
   }
}

/// <summary>
/// This function builds the grid contents values.
/// </summary>
/// <param name="grid">The name of the grid to populate.</param>
function TerrainTool::buildDisplayGrid(%this, %grid)
{
   if (!isObject(%grid))
      return;
   
   %grid.clearCellIntegers();
   
   %xCells = %grid.getCellCountX();
   %yCells = %grid.getCellCountY();
   
   // Used to fade paths
   if (%this.tabSelected == %this.GRAPHICSTAB)
   {
      %pathFade[0] = %this.PATHFADEOFFSET;
      %pathFade[1] = %this.PATHFADEOFFSET;
      %pathFade[2] = %this.PATHFADEOFFSET;
      %pathFade[3] = %this.PATHFADEOFFSET;
      %towerFade = %this.PATHFADEOFFSET;
   }
   else if (%this.currentActivity == TerrainTool.TOWEREDIT)
   {
      %pathFade[0] = %this.PATHFADEOFFSET;
      %pathFade[1] = %this.PATHFADEOFFSET;
      %pathFade[2] = %this.PATHFADEOFFSET;
      %pathFade[3] = %this.PATHFADEOFFSET;
      %towerFade = 0;
   }
   else if (%this.currentGrid == -1)
   {
      %pathFade[0] = 0;
      %pathFade[1] = 0;
      %pathFade[2] = 0;
      %pathFade[3] = 0;
      %towerFade = %this.PATHFADEOFFSET;
   }
   else
   {
      %pathFade[0] = %this.PATHFADEOFFSET;
      %pathFade[1] = %this.PATHFADEOFFSET;
      %pathFade[2] = %this.PATHFADEOFFSET;
      %pathFade[3] = %this.PATHFADEOFFSET;
      %towerFade = %this.PATHFADEOFFSET;
      
      %pathFade[%this.currentGridIndex] = 0;
   }
   
   // Which type of cells to display
   %displayTowers = false;
   %displayerPaths = false;
   if (%this.tabSelected == %this.GRAPHICSTAB && %this.graphicsShowPath || 
       %this.tabSelected == %this.PATHSTAB && %this.pathsShowPath || 
       %this.tabSelected == %this.TOWERSTAB && %this.towersShowPath)
   {
      %displayerPaths = true;
   }
   if (%this.tabSelected == %this.GRAPHICSTAB && %this.graphicsShowTower || 
       %this.tabSelected == %this.PATHSTAB && %this.pathsShowTower || 
       %this.tabSelected == %this.TOWERSTAB && %this.towersShowTower)
   {
      %displayTowers = true;
   }
   
   // Set up the cell images
   for (%x=0; %x<%xCells; %x++)
   {
      for (%y=0; %y<%yCells; %y++)
      {
         // By our rules the outer cells may not contain towers.  They are
         // reserved for path start and end points.  And the next rows and columns
         // of cells are not allowed to give some space for the towers' interface.
         if (%displayTowers)
         {
            if (%x > 1 && %x < (%xCells-2) && %y > 1 && %y < (%yCells-2))
            {
               %tx = %x-2;
               %ty = %y-2;
               
               %tower = TowerPlacementGrid.getCellWeight(%tx, %ty);
               
               // If there is a tower in this cell then set for it and ignore
               // the path settings
               if (%tower > 0.001)
               {
                  %this.setCellQuadImage(%grid, %x, %y, %this.TOWERCELLIMAGEQUAD+%towerFade);
                  continue;
               }
            }
         }
         
         if (!%displayerPaths)
            continue;
         
         %pathInt[0] = EnemyPathGrid01.getCellInteger(%x, %y);
         %path01w = EnemyPathGrid01.getCellWeight(%x, %y);
         if (%pathInt[0] == 0 && %path01w > 0.001)
         {
            %pathInt[0] = 1;
         }
         
         %pathInt[1] = EnemyPathGrid02.getCellInteger(%x, %y);
         %path02w = EnemyPathGrid02.getCellWeight(%x, %y);
         if (%pathInt[1] == 0 && %path02w > 0.001)
         {
            %pathInt[1] = 1;
         }

         %pathInt[2] = EnemyPathGrid03.getCellInteger(%x, %y);
         %path03w = EnemyPathGrid03.getCellWeight(%x, %y);
         if (%pathInt[2] == 0 && %path03w > 0.001)
         {
            %pathInt[2] = 1;
         }

         %pathInt[3] = EnemyPathGrid04.getCellInteger(%x, %y);
         %path04w = EnemyPathGrid04.getCellWeight(%x, %y);
         if (%pathInt[3] == 0 && %path04w > 0.001)
         {
            %pathInt[3] = 1;
         }

         // Test if none of the path cells are set.  Then we can skip this cell.
         if (%pathInt[0] == 0 && %pathInt[1] == 0 && %pathInt[2] == 0 && %pathInt[3] == 0)
            continue;
         
         %bitfield = %pathInt[0] + %pathInt[1] * 8 + %pathInt[2] * 64 + %pathInt[3] * 512;
         switch( %bitfield )
         {
            case 1:
               // Path01 path
               %this.setCellQuadImage(%grid, %x, %y, %this.PATH01IMAGEQUAD+%pathFade[0]);
               
            case 2:
               // Path01 start
               %this.setCellQuadImage(%grid, %x, %y, %this.PATH01STARTIMAGEQUAD+%pathFade[0]);
               
            case 4:
               // Path01 end
               %this.setCellQuadImage(%grid, %x, %y, %this.PATH01ENDIMAGEQUAD+%pathFade[0]);


            case 8:
               // Path02 path
               %this.setCellQuadImage(%grid, %x, %y, %this.PATH02IMAGEQUAD+%pathFade[1]);
               
            case 16:
               // Path02 start
               %this.setCellQuadImage(%grid, %x, %y, %this.PATH02STARTIMAGEQUAD+%pathFade[1]);
               
            case 32:
               // Path02 end
               %this.setCellQuadImage(%grid, %x, %y, %this.PATH02ENDIMAGEQUAD+%pathFade[1]);


            case 64:
               // Path03 path
               %this.setCellQuadImage(%grid, %x, %y, %this.PATH03IMAGEQUAD+%pathFade[2]);
               
            case 128:
               // Path03 start
               %this.setCellQuadImage(%grid, %x, %y, %this.PATH03STARTIMAGEQUAD+%pathFade[2]);
               
            case 256:
               // Path03 end
               %this.setCellQuadImage(%grid, %x, %y, %this.PATH03ENDIMAGEQUAD+%pathFade[2]);


            case 512:
               // Path04 path
               %this.setCellQuadImage(%grid, %x, %y, %this.PATH04IMAGEQUAD+%pathFade[3]);
               
            case 1024:
               // Path04 start
               %this.setCellQuadImage(%grid, %x, %y, %this.PATH04STARTIMAGEQUAD+%pathFade[3]);
               
            case 2048:
               // Path04 end
               %this.setCellQuadImage(%grid, %x, %y, %this.PATH04ENDIMAGEQUAD+%pathFade[3]);


            default:
               // If we're here then there must be multiple paths going through here.
               // Set up each corner for rendering.
               for (%i=0; %i<4; %i++)
               {
                  %cornerImage[%i] = %this.NOIMAGE;
                  
                  if ((%pathInt[%i] & 1) == 1)
                  {
                     %cornerImage[%i] = %this.PATHIMAGE[%i]+%pathFade[%i];
                  }
                  else if ((%pathInt[%i] & 2) == 2)
                  {
                     %cornerImage[%i] = %this.PATHSTARTIMAGE[%i]+%pathFade[%i];
                  }
                  else if ((%pathInt[%i] & 4) == 4)
                  {
                     %cornerImage[%i] = %this.PATHENDIMAGE[%i]+%pathFade[%i];
                  }
               }
               %this.setCellCornerImages(%grid, %x, %y, %cornerImage[0], %cornerImage[1], %cornerImage[2], %cornerImage[3]);
         }
      }
   }
}

/// <summary>
/// This function sets the correct path images for the selected cell.
/// </summary>
/// <param name="grid">The grid to build the display for.</param>
/// <param name="x">The X coordinate of the cell to set.</param>
/// <param name="y">The Y coordinate of the cell to set.</param>
/// <param name="quadImage">The quadrant sprite to use for the cell.</param>
function TerrainTool::setCellQuadImage(%this, %grid, %x, %y, %quadImage)
{
   %dx = %x * 2;
   %dy = %y * 2;
   
   %grid.setCellInteger(%dx,   %dy,   %quadImage);
   %grid.setCellInteger(%dx+1, %dy,   %quadImage+1);
   %grid.setCellInteger(%dx,   %dy+1, %quadImage+2);
   %grid.setCellInteger(%dx+1, %dy+1, %quadImage+3);
}

/// <summary>
/// This function displays the correct portions of the quad image for the selected cell.
/// </summary>
/// <param name="grid">The grid to pick the cell from.</param>
/// <param name="x">The X coordinate of the cell to set.</param>
/// <param name="y">The Y coordinate of the cell to set.</param>
/// <param name="imageTL">The top left path image.</param>
/// <param name="imageTR">The top right path image.</param>
/// <param name="imageBL">The bottom left path image.</param>
/// <param name="imageBR">The bottom right path image.</param>
function TerrainTool::setCellCornerImages(%this, %grid, %x, %y, %imageTL, %imageTR, %imageBL, %imageBR)
{
   %dx = %x * 2;
   %dy = %y * 2;
   
   %grid.setCellInteger(%dx,   %dy,   %imageTL);
   %grid.setCellInteger(%dx+1, %dy,   %imageTR);
   %grid.setCellInteger(%dx,   %dy+1, %imageBL);
   %grid.setCellInteger(%dx+1, %dy+1, %imageBR);
}

/// <summary>
/// This function builds the graphics brushes for the terrain editor palette.
/// </summary>
function TerrainTool::buildGraphicsBrushes(%this)
{
   %this-->GraphicsBrushesArray.clear();
   
   // Build out the brushes on the Graphics tab
   %count = $brushSet.getCount();
   for (%i=0; %i<%count; %i++)
   {
      // Get the ScriptObject that contains our brush info
      %brush = $brushSet.getObject(%i);

      // Create Sprite Object
      %sprite = new t2dStaticSprite();
      %sprite.scene = %this.scene;
      
      // Set the image map
      %sprite.setImageMap( %brush.asset.getName(), %brush.frame );
      
      // Set the appropriate size
      %sprite.setSize( %brush.asset.getFrameSize(0) );
      
      // Create the container to hold the brush
      %container = new GuiControl() {
         isContainer = "1";
         Profile = "GuiTransparentProfile";
         Extent = "68 68";
         Visible = "1";
      };
               
      // -- render control
      %t2dContainer = new GuiSceneObjectCtrl()
      {
         class = TerrainToolGraphicsBrushButton;
         Profile = ObjectBrowserThumbProfile @ $levelEditor::ObjectLibraryBackgroundColor;
         RenderMargin = 3;
         extent = "64 64";
         position = "2 2";
         brushIndex = %i;
         imageMap = %brush.asset;
         frame = %brush.frame;
         ToolTip = %brush.asset.getName() @ " frame " @ %brush.frame;
      };
   
   
      %t2dContainer.setSceneObject( %sprite );
   
      %container.add( %t2dContainer );
      
      %this-->GraphicsBrushesArray.addGuiControl(%container);
   }
   
   %this-->GraphicsBrushesArray.refresh();
   
   // Need to give the scroll control a bump as GuiDynamicCtrlArrayControl doesn't
   // inform its parent when it resizes due to new children.
   %se = %this-->GraphicsTabScroll.getExtent();
   %this-->GraphicsTabScroll.setExtent(getWord(%se, 0), getWord(%se, 1));
}

/// <summary>
/// This function builds the path brushes for the terrain editor palette.
/// </summary>
function TerrainTool::buildPathsBrushes(%this)
{
   %this-->PathsBrushesArray.clear();
   
   // Build out some example brushes on the Paths tab
   for (%i=0; %i<4; %i++)
   {
      %ctrl = %this.buildPathsBrush(%i, "EnemyPathGrid0" @ (%i+1));
      
      // Get the size of the path control and use that for the dynamic control's
      // row size
      %rowSize = getWord(%ctrl.getExtent(),1);
      
      %this-->PathsBrushesArray.addGuiControl(%ctrl);
   }
   
   // Resize the array to fit the path controls
   %this-->PathsBrushesArray.rowSize = %rowSize;
   %this-->PathsBrushesArray.refresh();
   
   // There is a bug with GuiDynamicCtrlArrayControl now properly informing
   // the parent scroll control of its new size.  Bump things here.
   %newSize = Vector2Add(%this-->PathsBrushesArray.getExtent(), "0 1");
   %this-->PathsBrushesArray.setExtent(getWord(%newSize,0), getWord(%newSize,1));
}

/// <summary>
/// This function builds the path graphics brush selection palette.
/// </summary>
function TerrainTool::buildPathsBrush(%this, %index, %grid)
{
   %width = 205;
   %height = 100;
   
   %imageNum = %index + 1;
   
   %name = %grid.getInternalName();
   
   // Build out the control stack
   %ctrl = new GuiControl() {
      isContainer = "1";
      Profile = "EditorPanelMedium";
      HorizSizing = "width";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = %width SPC %height;
      index = %index;
      grid = %grid;
         
      new GuiControl() {
         internalName = "PathPropertyStack";
         isContainer = "1";
         Profile = "GuiTransparentProfile";
         HorizSizing = "width";
         VertSizing = "bottom";
         Position = "0 0";
         Extent = %width SPC %height;

         new GuiTextCtrl() {
            internalName = "NameTextCtrl";
            canSaveDynamicFields = "0";
                isContainer = "0";
            Profile = "TerrainPathTitleProfile";
            HorizSizing = "width";
            VertSizing = "bottom";
            Position = "0 0";
            Extent = %width @ " 20";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
            maxLength = "1024";
            text = %name;
         };
         
         new GuiControl() {
            internalName = "PathEditStack";
            isContainer = "1";
            Profile = "GuiTransparentProfile";
            HorizSizing = "width";
            VertSizing = "bottom";
            Position = "0 28";
            Extent = %width SPC (%height - 28);
            Visible = "1";

            new GuiControl() {
               isContainer = "1";
               Profile = "GuiTransparentProfile";
               HorizSizing = "width";
               VertSizing = "bottom";
               Position = "0 0";
               Extent = %width @ " 24";
            
               new GuiTextCtrl() {
                  canSaveDynamicFields = "0";
                            isContainer = "0";
                  Profile = "GuiTextProfile";
                  HorizSizing = "right";
                  VertSizing = "bottom";
                  Position = "10 2";
                  Extent = "60 20";
                  MinExtent = "8 2";
                  canSave = "1";
                  Visible = "1";
                  hovertime = "1000";
                  maxLength = "1024";
                  text = "Name";
               };
               new GuiTextEditCtrl() {
                  internalName = "NameEditField";
                  class = "TerrainToolPathNameEdit";
                  canSaveDynamicFields = "0";
                            isContainer = "0";
                  Profile = "GuiTextEditProfile";
                  HorizSizing = "width";
                  VertSizing = "bottom";
                  Position = "60 0";
                  Extent = "130 24";
                  MinExtent = "8 2";
                  canSave = "1";
                  Visible = "1";
                  validate = "$ThisControl.validateText();";
                  hovertime = "1000";
                  maxLength = "1024";
                  historySize = "0";
                  password = "0";
                  tabComplete = "0";
                  sinkAllKeyEvents = "0";
                  password = "0";
                  passwordMask = "*";
                  text = %name;
                  grid = %grid;
               };
            };
            
            new GuiControl() {
               isContainer = "1";
               Profile = "GuiTransparentProfile";
               HorizSizing = "width";
               VertSizing = "bottom";
               Position = "0 32";
               Extent = %width @ " 32";
            
               new GuiTextCtrl() {
                  canSaveDynamicFields = "0";
                            isContainer = "0";
                  Profile = "GuiTextProfile";
                  HorizSizing = "right";
                  VertSizing = "bottom";
                  Position = "10 2";
                  Extent = "60 20";
                  MinExtent = "8 2";
                  canSave = "1";
                  Visible = "1";
                  hovertime = "1000";
                  maxLength = "1024";
                  text = "Layout";
               };
               new GuiButtonCtrl() {
                  internalName = "LayoutEditButton";
                  class = "TerrainToolLayoutEditButton";
                  canSaveDynamicFields = "0";
                            isContainer = "0";
                  Profile = "GuiButtonProfile";
                  HorizSizing = "right";
                  VertSizing = "bottom";
                  Position = "60 0";
                  Extent = "64 24";
                  MinExtent = "8 2";
                  canSave = "1";
                  Visible = "1";
                  hovertime = "1000";
                  groupNum = "-1";
                  buttonType = "PushButton";
                  useMouseEvents = "0";
                  text = "Edit";
               };
            };
         };
         
         new GuiControl() {
            internalName = "PathLayoutStack";
            isContainer = "1";
            Profile = "GuiTransparentProfile";
            HorizSizing = "width";
            VertSizing = "bottom";
            Position = "0 24";
            Extent = %width SPC (%height - 24);
            Visible = "0";

            new GuiControl() {
               isContainer = "1";
               Profile = "GuiTransparentProfile";
               HorizSizing = "width";
               VertSizing = "bottom";
               Position = "0 0";
               Extent = %width SPC (%height - 28 - 20); // 20 for buttons below
               Visible = "1";
               
               new GuiControl() {
                  isContainer = "1";
                  Profile = "GuiTransparentProfile";
                  HorizSizing = "center";
                  VertSizing = "center";
                  Position = ((%width - 112) / 2) @ " 0";
                  Extent = "112 48";
                  Visible = "1";

                  new GuiBitmapButtonCtrl() {
                     internalName = "PathBrushStart";
                     class = TerrainToolLayoutBrushButton;
                     canSaveDynamicFields = "0";
                                  isContainer = "0";
                     Profile = "GuiButtonProfile";
                     HorizSizing = "right";
                     VertSizing = "bottom";
                     Position = "0 0";
                     Extent = "32 32";
                     MinExtent = "8 2";
                     canSave = "1";
                     Visible = "1";
                     hovertime = "1000";
                     groupNum = "-1";
                     buttonType = "PushButton";
                     useMouseEvents = "0";
                     ToolTip = "Spawn Point";
                     isLegacyVersion = "0";
                     bitmapNormal = "../gui/images/PathS" @ %imageNum;
                     bitmapHilight = "../gui/images/PathS" @ %imageNum;
                     bitmapDepressed = "../gui/images/PathS" @ %imageNum;
                     grid = %grid;
                     gridIndex = %index;
                     brush = %this.PATHBRUSHSTART;
                  };
                  new GuiMLTextCtrl() {
                     canSaveDynamicFields = "0";
                                  isContainer = "0";
                     Profile = "GuiMLWhiteTextProfile";
                     HorizSizing = "width";
                     VertSizing = "bottom";
                     Position = "0 32";
                     Extent = "32 16";
                     MinExtent = "8 2";
                     canSave = "1";
                     Visible = "1";
                     hovertime = "1000";
                     lineSpacing = "2";
                     allowColorChars = "0";
                     maxChars = "-1";
                     text = "<just:center>Start";
                  };

                  new GuiBitmapButtonCtrl() {
                     internalName = "PathBrushPath";
                     class = TerrainToolLayoutBrushButton;
                     canSaveDynamicFields = "0";
                                  isContainer = "0";
                     Profile = "GuiButtonProfile";
                     HorizSizing = "right";
                     VertSizing = "bottom";
                     Position = "40 0";
                     Extent = "32 32";
                     MinExtent = "8 2";
                     canSave = "1";
                     Visible = "1";
                     hovertime = "1000";
                     groupNum = "-1";
                     buttonType = "PushButton";
                     useMouseEvents = "0";
                     ToolTip = "Enemy Path";
                     isLegacyVersion = "0";
                     bitmapNormal = "../gui/images/PathP" @ %imageNum;
                     bitmapHilite = "../gui/images/PathP" @ %imageNum;
                     bitmapDepressed = "../gui/images/PathP" @ %imageNum;
                     grid = %grid;
                     gridIndex = %index;
                     brush = %this.PATHBRUSHPATH;
                  };
                  new GuiMLTextCtrl() {
                     canSaveDynamicFields = "0";
                                  isContainer = "0";
                     Profile = "GuiMLWhiteTextProfile";
                     HorizSizing = "width";
                     VertSizing = "bottom";
                     Position = "40 32";
                     Extent = "32 16";
                     MinExtent = "8 2";
                     canSave = "1";
                     Visible = "1";
                     hovertime = "1000";
                     lineSpacing = "2";
                     allowColorChars = "0";
                     maxChars = "-1";
                     text = "<just:center>Path";
                  };

                  new GuiBitmapButtonCtrl() {
                     internalName = "PathBrushEnd";
                     class = TerrainToolLayoutBrushButton;
                     canSaveDynamicFields = "0";
                                  isContainer = "0";
                     Profile = "GuiButtonProfile";
                     HorizSizing = "right";
                     VertSizing = "bottom";
                     Position = "80 0";
                     Extent = "32 32";
                     MinExtent = "8 2";
                     canSave = "1";
                     Visible = "1";
                     hovertime = "1000";
                     groupNum = "-1";
                     buttonType = "PushButton";
                     useMouseEvents = "0";
                     ToolTip = "End Point";
                     isLegacyVersion = "0";
                     bitmapNormal = "../gui/images/PathE" @ %imageNum;
                     bitmapHilite = "../gui/images/PathE" @ %imageNum;
                     bitmapDepressed = "../gui/images/PathE" @ %imageNum;
                     grid = %grid;
                     gridIndex = %index;
                     brush = %this.PATHBRUSHEND;
                  };
                  new GuiMLTextCtrl() {
                     canSaveDynamicFields = "0";
                                  isContainer = "0";
                     Profile = "GuiMLWhiteTextProfile";
                     HorizSizing = "width";
                     VertSizing = "bottom";
                     Position = "80 32";
                     Extent = "32 16";
                     MinExtent = "8 2";
                     canSave = "1";
                     Visible = "1";
                     hovertime = "1000";
                     lineSpacing = "2";
                     allowColorChars = "0";
                     maxChars = "-1";
                     text = "<just:center>End";
                  };
               };
            };
            
            new GuiButtonCtrl() {
               internalName = "LayoutCancelButton";
               class = "TerrainToolLayoutCancelButton";
               canSaveDynamicFields = "0";
                      isContainer = "0";
               Profile = "GuiButtonProfile";
               HorizSizing = "right";
               VertSizing = "bottom";
               Position = (%width - 64 - 4 - 64 - 4) SPC (%height - 28 - 20);
               Extent = "64 20";
               MinExtent = "8 2";
               canSave = "1";
               Visible = "1";
               hovertime = "1000";
               groupNum = "-1";
               buttonType = "PushButton";
               useMouseEvents = "0";
               text = "Cancel";
            };

            new GuiButtonCtrl() {
               internalName = "LayoutSaveButton";
               class = "TerrainToolLayoutSaveButton";
               canSaveDynamicFields = "0";
                      isContainer = "0";
               Profile = "GuiButtonProfile";
               HorizSizing = "right";
               VertSizing = "bottom";
               Position = (%width - 64 - 4) SPC (%height - 28 - 20);
               Extent = "64 20";
               MinExtent = "8 2";
               canSave = "1";
               Visible = "1";
               hovertime = "1000";
               groupNum = "-1";
               buttonType = "PushButton";
               useMouseEvents = "0";
               text = "Save";
            };
         };
      };
      
      new GuiControl() {
         internalName = "DisabledOverlay";
         isContainer = "1";
         Profile = "TerrainPathDisabledProfile";
         HorizSizing = "width";
         VertSizing = "bottom";
         Position = "0 0";
         Extent = %width SPC %height;
         Visible = "0";
      };
   };
   
   // Set up some parent linkages for script quick access
   %ctrl-->NameEditField.stackRoot = %ctrl;
   %ctrl-->LayoutEditButton.stackRoot = %ctrl;
   %ctrl-->LayoutSaveButton.stackRoot = %ctrl;
   %ctrl-->LayoutCancelButton.stackRoot = %ctrl;
   
   return %ctrl;
}

/// <summary>
/// This function builds the tower brush selection palette.
/// </summary>
function TerrainTool::buildTowersBrushes(%this)
{
   %this-->TowersBrushesArray.clear();
   
   // Build out some example brushes for the Towers tab
   %ctrl = new GuiControl() {
         Profile = "GuiTransparentProfile";
         Extent = "64 96";
         
         new GuiBitmapButtonCtrl() {
            canSaveDynamicFields = "0";
            class = TerrainToolTowerBrushButton;
                isContainer = "0";
            Profile = "GuiButtonProfile";
            HorizSizing = "right";
            VertSizing = "bottom";
            Position = "0 0";
            Extent = "64 64";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
            groupNum = "-1";
            buttonType = "PushButton";
            useMouseEvents = "0";
            ToolTip = "";
            isLegacyVersion = "0";
            bitmapNormal = "../gui/images/PathT";
            bitmapHilite = "../gui/images/PathT";
            bitmapDepressed = "../gui/images/PathT";
         };
         
         new GuiMLTextCtrl() {
            Profile = "GuiMLWhiteTextProfile";
            Position = "0 64";
            Extent = "64 32";
            maxChars = "-1";
            text = "<just:center>Tower Placement";
         };
      };
   %this-->TowersBrushesArray.addGuiControl(%ctrl);
   %this-->TowersBrushesArray.refresh();
}

/// <summary>
/// This function opens the Asset Library to select graphics to add to the graphics palette.
/// </summary>
function TerrainTool::onAddGraphicButton(%this)
{
   // Open the asset selector
   AssetLibrary.open(%this, $SpriteSheetPage, "");
}

/// <summary>
/// This function removes a selected graphic from the brush selection palette.
/// </summary>
function TerrainTool::onRemoveGraphicButton(%this)
{
   if (%this.currentGraphicsImageMap $= "")
      return;
   
   %this.removeGraphicsBrushFrame(%this.currentGraphicsImageMap, %this.currentGraphicsFrame);
}

// Called by the Asset Library when an asset is selected
/// <summary>
/// This function adds the asset from the Asset Library to the palette
/// </summary>
/// <param name="asset">The graphic to add the palette.</param>
function TerrainTool::setSelectedAsset(%this, %asset)
{
   // Build the ScriptObjects that will hold the asset's information.
   %frames = %asset.getFrameCount();
   for (%i=0; %i<%frames; %i++)
   {
      // Only add the brush if it isn't already
      if (!%this.isGraphicsBrush(%asset, %i))
      {
         %brush = new ScriptObject()
            {
               asset = %asset;
               name = %asset.getName();
               frame = %i;
            };
         $brushSet.add(%brush);
      }
   }
   
   %this.sortGraphicsBrushes();
   %this.buildGraphicsBrushes();
}

// Returns true if the requested asset and frame are already a graphics brush
/// <summary>
/// This function returns true if the requested asset and image frame are already a graphics brush, or false if not.
/// </summary>
function TerrainTool::isGraphicsBrush(%this, %asset, %frame)
{
   %count = $brushSet.getCount();
   for (%i=0; %i<%count; %i++)
   {
      %brush = $brushSet.getObject(%i);
      if (%brush.asset $= %asset && %brush.frame == %frame)
      {
         return true;
      }
   }
   
   return false;
}

// Removes all frames of the given image map
/// <summary>
/// This function removes all frames of the passed image map from the graphics palette.
/// </summary>
/// <param name="imageMap">The name of the image map to remove from the graphics palette.</param>
function TerrainTool::removeGraphicsBrush(%this, %imageMap)
{
   //echo("@@@ TerrainTool::removeGraphicsBrush(): " @ %imageMap);
   
   %found = false;
   
   %index = 0;
   while (%index < $brushSet.getCount())
   {
      %brush = $brushSet.getObject(%index);
      if (%brush.asset $= %imageMap)
      {
         %found = true;
         
         // First remove this brush from the GraphicsGrid.  We need to check
         // each possible rotation
         for (%i=0; %i<4; %i++)
         {
            %cellInteger = GraphicsGrid.findImage(%brush.asset, %brush.frame, %i);
            if (%cellInteger !$= "")
            {
               GraphicsGrid.setCellsFromIntegerToValue(%cellInteger, 0);
            }
         }
         
         // Now remove the brush from the set
         $brushSet.remove(%brush);
         %brush.delete();
      }
      else
      {
         %index++;
      }
   }
   
   if (%found)
   {
      %this.clearCurrentGraphicsBrush();
      %this.buildGraphicsBrushes();
   }
}

// Removes a specific frame of an image map
/// <summary>
/// This function removes the specified frame of an image map from the graphics palette.
/// </summary>
/// <param name="imageMap">The image map that the image frame belongs to.</param>
/// <param name="frame">The frame number of the graphic to remove from the palette.</param>
function TerrainTool::removeGraphicsBrushFrame(%this, %imageMap, %frame)
{
   %found = false;
   
   %count = $brushSet.getCount();
   for (%i=0; %i<%count; %i++)
   {
      %brush = $brushSet.getObject(%i);
      if (%brush.asset $= %imageMap && %brush.frame == %frame)
      {
         %found = true;
         
         $brushSet.remove(%brush);
         %brush.delete();
         
         break;
      }
   }

   if (%found)
   {
      %this.clearCurrentGraphicsBrush();
      %this.buildGraphicsBrushes();
   }
}

/// <summary>
/// This function sorts the available graphics brushes by image map and frame number.
/// </summary>
function TerrainTool::sortGraphicsBrushes(%this)
{
   %count = $brushSet.getCount();
   for (%i=0; %i<%count; %i++)
   {
      %brush1 = $brushSet.getObject(%i);
      
      for (%j=%i+1; %j<%count; %j++)
      {
         %brush2 = $brushSet.getObject(%j);
         if (%brush1.asset $= %brush2.asset && %brush1.frame > %brush2.frame)
         {
            $brushSet.reorderChild(%brush2, %brush1);
         }
         else if (stricmp(%brush1.asset, %brush2.asset) > 0)
         {
            $brushSet.reorderChild(%brush2, %brush1);
         }
      }
   }
}

/// <summary>
/// This function sets up to edit a selected path.
/// </summary>
/// <param name="pathGuiStack">The grid undo set associated with the grid to save.</param>
function TerrainTool::startPathLayoutEdit(%this, %pathGuiStack)
{
   // Make sure the Terrain Window's Save and Close buttons are
   // not active.
   %this.setSaveCloseState(false);

   // Make sure the user cannot click on one of the tabs
   %this-->TabBookBlocker.setVisible(1);
   
   // Bring the layout stack forwards
   %pathGuiStack-->PathEditStack.setVisible(0);
   %pathGuiStack-->PathLayoutStack.setVisible(1);
   %pathGuiStack-->PathLayoutStack-->PathBrushPath.performClick();
   
   // Disable all other path edit controls
   %count = %this-->PathsBrushesArray.getCount();
   for (%i=0; %i<%count; %i++)
   {
      %stack = %this-->PathsBrushesArray.getObject(%i);
      if (%stack != %pathGuiStack)
      {
         %stack-->DisabledOverlay.setVisible(1);
      }
   }
   
   // Store the current grid states in case the user cancels
   new Grid(TerrainToolTempPathGrid01);
   %pathGuiStack.grid.copyCellWeightsToGrid(TerrainToolTempPathGrid01);
   %pathGuiStack.grid.copyCellIntegersToGrid(TerrainToolTempPathGrid01);
   new Grid(TerrainToolTempPathGrid02);
   TowerPlacementGrid.copyCellWeightsToGrid(TerrainToolTempPathGrid02);
   TowerPlacementGrid.copyCellIntegersToGrid(TerrainToolTempPathGrid02);

   // Rebuild display
   TerrainTool.buildDisplayGrid(TerrainDisplayGrid);
   
   %this.setPathGridDirtyState(false);
}

/// <summary>
/// This function starts the save process for the path grids.
/// </summary>
/// <param name="pathGuiStack">The grid undo set associated with the grid to save.</param>
function TerrainTool::savePathLayoutEdit(%this, %pathGuiStack)
{
   // If there is no start point, no end point, and no path markers that's OK.  The
   // user likely deleted the entire path and wants to now save these changes.
   if (!%this.currentGrid.gridHasCellInteger(2) && !%this.currentGrid.gridHasCellInteger(4))
   {
      // No start or end point.  Check if all grid cells are considered blocked.
      %xcount = %this.currentGrid.getCellCountX();
      %ycount = %this.currentGrid.getCellCountY();
      %blocked = true;
      for (%x=0; %x<%xcount; %x++)
      {
         for (%y=0; %y<%ycount; %y++)
         {
            if (!%this.currentGrid.isCellBlocked(%x, %y))
            {
               %blocked = false;
               break;
            }
         }
         
         if (!%blocked)
            break;
      }
      
      if (%blocked)
      {
         // The entire grid is empty so just save it
         %this.savePathLayoutEdit2(%pathGuiStack);
         return;
      }
   }
   
   // Make sure there is a valid start and end point to the path.
   if (!%this.currentGrid.gridHasCellInteger(2))
   {
      ConfirmDialog.setupAndShow(TerrainToolWindow.getGlobalCenter(), "Your path is missing a Start point.",
         "", "", "", "", "OK", "");
      return;
   }
   else if (!%this.currentGrid.gridHasCellInteger(4))
   {
      ConfirmDialog.setupAndShow(TerrainToolWindow.getGlobalCenter(), "Your path is missing an End point.",
         "", "", "", "", "OK", "");
      return;
   }
   
   // Test that the path is valid from start to end
   %startPoint = getGridStartPoint(%this.currentGrid);
   %endPoint = getGridEndPoint(%this.currentGrid);
   if (%startPoint $= "" || %endPoint $= "")
   {
      // This should not be the case given the checks above...
      ConfirmDialog.setupAndShow(TerrainToolWindow.getGlobalCenter(), "Your path is missing a Start or End point.",
         "", "", "", "", "OK", "");
      return;
   }
   
   %this.currentGrid.buildNodeGraph(true);
   %path = %this.currentGrid.createPath(%startPoint, %endPoint, true);

   if (%path == 0)
   {
      %center = TerrainToolWindow.getGlobalCenter();
      TerrainToolPathDialogWindow.setPositionGlobal(%center.x - (TerrainToolPathDialogWindow.extent.x / 2), 
         %center.y - (TerrainToolPathDialogWindow.extent.y / 2));
      Canvas.pushDialog(TerrainToolPathDialog);
      return;
   }
   
   %path.delete();
   %this.savePathLayoutEdit2(%pathGuiStack);
}
/// <summary>
/// This function is stage 2 of the grid save process.  It does some housekeeping and reenables the Save and Done buttons when it finishes.
/// </summary>
/// <param name="pathGuiStack">The grid undo set associated with the grid to save.</param>
function TerrainTool::savePathLayoutEdit2(%this, %pathGuiStack)
{
   %this.currentGrid = -1;
   %this.currentGridIndex = -1;
   %this.currentGridBrush = %this.PATHBRUSHNONE;
   %this-->PreviewArea.bitmap = "";

   // Allow the user to click on the tabs again
   %this-->TabBookBlocker.setVisible(0);

   // Bring the edit stack forwards
   %pathGuiStack-->PathEditStack.setVisible(1);
   %pathGuiStack-->PathLayoutStack.setVisible(0);
   
   // Enable all path edit controls
   %count = %this-->PathsBrushesArray.getCount();
   for (%i=0; %i<%count; %i++)
   {
      %stack = %this-->PathsBrushesArray.getObject(%i);
      if (%stack != %pathGuiStack)
      {
         %stack-->DisabledOverlay.setVisible(0);
      }
   }
   
   // Discard the temporary storage grids
   TerrainToolTempPathGrid01.delete();
   TerrainToolTempPathGrid02.delete();

   // Rebuild display
   TerrainTool.buildDisplayGrid(TerrainDisplayGrid);
   
   if (%this.isPathGridDirty())
   {
      %this.setGlobalDirtyState(true);
      %this.setPathGridDirtyState(false);
   }

   // Reactivate the Terrain Window's Save and Close buttons.   
   %this.setSaveCloseState(true);
}

/// <summary>
/// This function handles cancellation of the path edit.  If the user confirms, executes the cancellation.
/// </summary>
/// <param name="pathGuiStack">The grid undo set associated with the grid to save.</param>
function TerrainTool::cancelPathLayoutEdit(%this, %pathGuiStack)
{
   if (%this.isPathGridDirty())
   {
      // Changes have been made
      ConfirmDialog.setupAndShow(TerrainToolWindow.getGlobalCenter(), "Cancel your changes to the path?",
         "Yes", "TerrainTool.cancelPathLayoutEdit2(" @ %pathGuiStack @ ");", "", "", "No", "");
   }
   else
   {
      // No changes so go straight to the clean up
      %this.cancelPathLayoutEdit2(%pathGuiStack);
   }
}

/// <summary>
/// This function handles the actual cancellation of the path edit.
/// </summary>
/// <param name="pathGuiStack">The grid undo set associated with the grid to save.</param>
function TerrainTool::cancelPathLayoutEdit2(%this, %pathGuiStack)
{
   %this.currentGrid = -1;
   %this.currentGridIndex = -1;
   %this.currentGridBrush = %this.PATHBRUSHNONE;
   %this-->PreviewArea.bitmap = "";

   // Allow the user to click on the tabs again
   %this-->TabBookBlocker.setVisible(0);

   // Bring the edit stack forwards
   %pathGuiStack-->PathEditStack.setVisible(1);
   %pathGuiStack-->PathLayoutStack.setVisible(0);
   
   // Enable all path edit controls
   %count = %this-->PathsBrushesArray.getCount();
   for (%i=0; %i<%count; %i++)
   {
      %stack = %this-->PathsBrushesArray.getObject(%i);
      if (%stack != %pathGuiStack)
      {
         %stack-->DisabledOverlay.setVisible(0);
      }
   }
   
   // Copy back the stored values from the temporary grids
   TerrainToolTempPathGrid01.copyCellWeightsToGrid(%pathGuiStack.grid);
   TerrainToolTempPathGrid01.copyCellIntegersToGrid(%pathGuiStack.grid);
   TerrainToolTempPathGrid02.copyCellWeightsToGrid(TowerPlacementGrid);
   TerrainToolTempPathGrid02.copyCellIntegersToGrid(TowerPlacementGrid);
   
   // Discard the temporary storage grids
   TerrainToolTempPathGrid01.delete();
   TerrainToolTempPathGrid02.delete();

   // Rebuild display
   TerrainTool.buildDisplayGrid(TerrainDisplayGrid);
   
   %this.setPathGridDirtyState(false);

   // Reactivate the Terrain Window's Save and Close buttons.   
   %this.setSaveCloseState(true);
}

// Returns what is in the requested cell from a tower point of view
/// <summary>
/// This function accesses the tower placement value of the grid cell under the mouse.
/// </summary>
/// <param name="mouseX">The world X coordinate of the mouse.</param>
/// <param name="mosueY">The world Y coordinate of the mouse.</param>
function TerrainTool::getCellTowerType(%this, %mouseX, %mouseY)
{
   %result = %this.CELLINVALID;
   
   // Test tower grid first
   %withinTowerGrid = false;
   if (TowerPlacementGrid.getIsPointInOOBB(%mouseX, %mouseY))
   {
      %withinTowerGrid = true;
      %worldPos = %mouseX @ " " @ %mouseY;
      
      // Test if this cell has a tower already within it
      if (!TowerPlacementGrid.isWorldPositionBlocked(%worldPos))
      {
         // Yes, we have a tower here so we're done with the classification
         return %this.CELLTOWER;
      }
      
      // Check each path for an empty cell
      if (!EnemyPathGrid01.isWorldPositionBlocked(%worldPos) || 
          !EnemyPathGrid02.isWorldPositionBlocked(%worldPos) || 
          !EnemyPathGrid03.isWorldPositionBlocked(%worldPos) || 
          !EnemyPathGrid04.isWorldPositionBlocked(%worldPos))
      {
         // There is at least one path cell here
         return %this.CELLPATH;
      }
      
      // If we're here then this is an empty cell for towers
      %result = %this.CELLEMPTY;
   }
   
   return %result;
}

// Returns what is in the requested cell from a path point of view
/// <summary>
/// This function accesses the path value of the grid cell under the mouse.
/// </summary>
/// <param name="mouseX">The world X coordinate of the mouse.</param>
/// <param name="mosueY">The world Y coordinate of the mouse.</param>
/// <param name="pathGrid">The pathgrid we want the cell from.</param
function TerrainTool::getCellPathType(%this, %mouseX, %mouseY, %pathGrid)
{
   %result = %this.CELLINVALID;
   %worldPos = %mouseX @ " " @ %mouseY;
   
   // Test the tower grid first
   if (TowerPlacementGrid.getIsPointInOOBB(%mouseX, %mouseY))
   {
      // Test if there is a tower in this cell
      if (!TowerPlacementGrid.isWorldPositionBlocked(%worldPos))
      {
         // Yes, we have a tower here so we're done with the classification
         return %this.CELLTOWER;
      }
   }
   
   // Now test the path grid
   if (%pathGrid.getIsPointInOOBB(%mouseX, %mouseY))
   {
      // Check if this cell is within the outer first cells of the grid.
      // If so then our rule is that only start and end points may go here.
      %endPointOnly = false;
      %cells = %pathGrid.getCellFromWorldPosition(%worldPos);
      %cx = getWord(%cells, 0);
      %cy = getWord(%cells, 1);
      if (%cx == 0 || %cy == 0 || 
          %cx == (%pathGrid.getCellCountX()-1) || %cy == (%pathGrid.getCellCountY()-1))
      {
         %endPointOnly = true;
      }
      
      if (%pathGrid.isWorldPositionBlocked(%worldPos))
      {
         // Empty cell, but of what type?
         if (%endPointOnly)
         {
            %result = %this.CELLEMPTYPATHENDPOINTONLY;
         }
         else
         {
            %result = %this.CELLEMPTY;
         }
      }
      else
      {
         // Path cell, but of what type?
         if (%endPointOnly)
         {
            %result = %this.CELLPATHENDPOINTONLY;
         }
         else
         {
            %result = %this.CELLPATH;
         }
      }
   }
   
   return %result;
}

// Edit a graphic tile
/// <summary>
/// This function paints graphics on the graphics grid.
/// </summary>
/// <param name="mouseX">The world X coordinate of the mouse.</param>
/// <param name="mouseY">The world Y coordinate of the mouse.</param>
/// <param name="isDrag">Indicates if the mouse is being dragged.</param>
function TerrainTool::editGraphics(%this, %mouseX, %mouseY, %isDrag)
{
   %dirty = false;
   
   if (%this.currentOperation == %this.PAINTOPERATION && %this.currentGraphicsImageMap !$= "")
   {
      %cellInteger = GraphicsGrid.findImage(%this.currentGraphicsImageMap, %this.currentGraphicsFrame, %this.currentGraphicsRotation);
      if (%cellInteger $= "")
      {
         // First time painting with this image so add it to the grid
         %cellInteger = GraphicsGrid.findNextFreeImageKey(1);
         GraphicsGrid.addImage(%cellInteger, %this.currentGraphicsImageMap, %this.currentGraphicsFrame, %this.currentGraphicsRotation);
      }

      %worldPos = %mouseX @ " " @ %mouseY;
      %cell = GraphicsGrid.getCellFromWorldPosition(%worldPos);
      %cx = getWord(%cell, 0);
      %cy = getWord(%cell, 1);
      GraphicsGrid.setCellInteger(%cx, %cy, %cellInteger);
      
      %dirty = true;
   }
   else if (%this.currentOperation == %this.FILLOPERATION && !%isDrag && %this.currentGraphicsImageMap !$= "")
   {
      %cellInteger = GraphicsGrid.findImage(%this.currentGraphicsImageMap, %this.currentGraphicsFrame, %this.currentGraphicsRotation);
      if (%cellInteger $= "")
      {
         // First time painting with this image so add it to the grid
         %cellInteger = GraphicsGrid.findNextFreeImageKey(1);
         GraphicsGrid.addImage(%cellInteger, %this.currentGraphicsImageMap, %this.currentGraphicsFrame, %this.currentGraphicsRotation);
      }
      
      // Fill the entire grid with the image
      %cx = GraphicsGrid.getCellCountX();
      %cy = GraphicsGrid.getCellCountY();
      for (%x=0; %x<%cx; %x++)
      {
         for (%y=0; %y<%cy; %y++)
         {
            GraphicsGrid.setCellInteger(%x, %y, %cellInteger);
         }
      }
      
      %dirty = true;
   }
   else if (%this.currentOperation == %this.ERASEOPERATION)
   {
      %worldPos = %mouseX @ " " @ %mouseY;
      
      // Erase the graphics brush
      %cell = GraphicsGrid.getCellFromWorldPosition(%worldPos);
      %cx = getWord(%cell, 0);
      %cy = getWord(%cell, 1);
      GraphicsGrid.setCellWeight(%cx, %cy, 0);
      GraphicsGrid.setCellInteger(%cx, %cy, 0);
      
      %dirty = true;
   }
   
   if (%dirty)
   {
      %this.setGlobalDirtyState(true);
      %dirty = false;
   }
}

/// <summary>
/// This function sets the current graphics brush to blank.
/// </summary>
function TerrainTool::clearCurrentGraphicsBrush(%this)
{
   %this.currentGraphicsImageMap = "";
   %this.currentGraphicsFrame = -1;
   %this.currentGraphicsRotation = 0;
   
   %this-->PreviewAreaWindow.clear();
}

/// <summary>
/// This function rotates the current graphics brush.
/// </summary>
/// <param name="direction">1 to rotate right, -1 to rotate left.</param>
function TerrainTool::rotateGraphicsBrush(%this, %direction)
{
   %this.currentGraphicsRotation += %direction;
   if (%this.currentGraphicsRotation < 0)
   {
      %this.currentGraphicsRotation = 3;
   }
   else if (%this.currentGraphicsRotation > 3)
   {
      %this.currentGraphicsRotation = 0;
   }
   %this-->PreviewAreaWindow.sprite.setAngle(%this.currentGraphicsRotation * 90);
}

// Edit a path tile
/// <summary>
/// This function paints path values on the currently edited path grid.
/// </summary>
/// <param name="mouseX">The world X coordinate of the mouse.</param>
/// <param name="mouseY">The world Y coordinate of the mouse.</param>
/// <param name="isDrag">Indicates if the mouse is being dragged.</param>
function TerrainTool::editPath(%this, %mouseX, %mouseY, %isDrag)
{
   if (%this.currentGrid == -1 || %this.currentGridBrush == %this.PATHBRUSHNONE)
      return;
   
   %dirty = false;
   
   if (%this.currentOperation == %this.PAINTOPERATION)
   {
      %cellType = %this.getCellPathType(%mouseX, %mouseY, %this.currentGrid);
      %worldPos = %mouseX @ " " @ %mouseY;
      
      // If there is a tower here then erase it
      if (%cellType == %this.CELLTOWER)
      {
         // Unless the user is dragging the mouse.  Then just do nothing according
         // to our rules.
         if (%isDrag)
            return;
         
         %cell = TowerPlacementGrid.getCellFromWorldPosition(%worldPos);
         TowerPlacementGrid.setCellWeight(getWord(%cell, 0), getWord(%cell, 1), 0);
         %dirty = true;
         
         // With the tower cell gone, we need to reclassify our cell
         %cellType = %this.getCellPathType(%mouseX, %mouseY, %this.currentGrid);
      }
      
      // According to our rules, do not allow drag placement of start or end points
      if (%isDrag && %this.currentGridBrush != %this.PATHBRUSHPATH)
         return;
      
      // Paint the grid brush
      if (%cellType == %this.CELLEMPTY || %cellType == %this.CELLPATH)
      {
         %cell = %this.currentGrid.getCellFromWorldPosition(%worldPos);
         %cx = getWord(%cell, 0);
         %cy = getWord(%cell, 1);
         %this.currentGrid.setCellWeight(%cx, %cy, 1);
         
         if (%this.currentGridBrush == %this.PATHBRUSHPATH)
         {
            %this.currentGrid.setCellInteger(%cx, %cy, 0);
         }
         else if (%this.currentGridBrush == %this.PATHBRUSHSTART)
         {
            // First, clear out any other Start cells
            %this.clearSpecialPathMarkers(%this.currentGrid, 2);
            
            // Now paint this cell
            %this.currentGrid.setCellInteger(%cx, %cy, 2);
         }
         else if (%this.currentGridBrush == %this.PATHBRUSHEND)
         {
            // First, clear out any other End cells
            %this.clearSpecialPathMarkers(%this.currentGrid, 4);
            
            // Now paint this cell
            %this.currentGrid.setCellInteger(%cx, %cy, 4);
         }
         
         %dirty = true;
      }
      else if (%this.currentGridBrush != %this.PATHBRUSHPATH && (%cellType == %this.CELLEMPTYPATHENDPOINTONLY || %cellType == %this.CELLPATHENDPOINTONLY))
      {
         %cell = %this.currentGrid.getCellFromWorldPosition(%worldPos);
         %cx = getWord(%cell, 0);
         %cy = getWord(%cell, 1);
         %this.currentGrid.setCellWeight(%cx, %cy, 1);
         
         if (%this.currentGridBrush == %this.PATHBRUSHSTART)
         {
            // First, clear out any other Start cells
            %this.clearSpecialPathMarkers(%this.currentGrid, 2);
            
            // Now paint this cell
            %this.currentGrid.setCellInteger(%cx, %cy, 2);
         }
         else if (%this.currentGridBrush == %this.PATHBRUSHEND)
         {
            // First, clear out any other End cells
            %this.clearSpecialPathMarkers(%this.currentGrid, 4);
            
            // Now paint this cell
            %this.currentGrid.setCellInteger(%cx, %cy, 4);
         }
         else
         {
            %this.currentGrid.setCellInteger(%cx, %cy, 0);
         }
         
         %dirty = true;
      }
   }
   else if (%this.currentOperation == %this.ERASEOPERATION)
   {
      %cellType = %this.getCellPathType(%mouseX, %mouseY, %this.currentGrid);
      %worldPos = %mouseX @ " " @ %mouseY;
      
      // Erase the grid brush
      if (%cellType == %this.CELLPATH || %cellType == %this.CELLPATHENDPOINTONLY)
      {
         %cell = %this.currentGrid.getCellFromWorldPosition(%worldPos);
         %cx = getWord(%cell, 0);
         %cy = getWord(%cell, 1);
         %this.currentGrid.setCellWeight(%cx, %cy, 0);
         %this.currentGrid.setCellInteger(%cx, %cy, 0);
         
         %dirty = true;
      }
   }
   
   if (%dirty)
   {
      %this.setPathGridDirtyState(true);
      %this.buildDisplayGrid(TerrainDisplayGrid);
      %dirty = false;
   }
}

/// <summary>
/// This function clears a specific value from the selected grid.
/// </summary>
/// <param name="grid">The grid to perform the clear upon.</param>
/// <param name="value">The value to search for and clear.</param>
function TerrainTool::clearSpecialPathMarkers(%this, %grid, %value)
{
   %cellInt = %grid.findFirstCellWithInteger(%value);
   while (%cellInt !$= "")
   {
      %cix = getWord(%cellInt, 0);
      %ciy = getWord(%cellInt, 1);
      %grid.setCellInteger(%cix, %ciy, 0);
      %grid.setCellWeight(%cix, %ciy, 0);
      
      %cellInt = %grid.findNextCellWithInteger();
   }
}

// Edit a tower tile
/// <summary>
/// This function paints tower placement values on the tower placement grid.
/// </summary>
/// <param name="mouseX">The world X coordinate of the mouse.</param>
/// <param name="mouseY">The world Y coordinate of the mouse.</param>
/// <param name="isDrag">Indicates that the mouse is being dragged.</param>
function TerrainTool::editTowers(%this, %mouseX, %mouseY, %isDrag)
{
   %dirty = false;
   
   if (%this.currentOperation == %this.PAINTOPERATION)
   {
      // Add a tower if the cell is empty
      if (%this.getCellTowerType(%mouseX, %mouseY) == %this.CELLEMPTY)
      {
         %cell = TowerPlacementGrid.getCellFromWorldPosition(%mouseX @ " " @ %mouseY);
         TowerPlacementGrid.setCellWeight(getWord(%cell, 0), getWord(%cell, 1), 1);
         %dirty = true;
      }
   }
   else if (%this.currentOperation == %this.FILLOPERATION && !%isDrag)
   {
      // Fill all empty cells with towers
      %cx = GraphicsGrid.getCellCountX();
      %cy = GraphicsGrid.getCellCountY();
      for (%x=0; %x<%cx; %x++)
      {
         for (%y=0; %y<%cy; %y++)
         {
            %worldPos = TowerPlacementGrid.getCellWorldPosition(%x, %y);
            if (%this.getCellTowerType(getWord(%worldPos, 0), getWord(%worldPos, 1)) == %this.CELLEMPTY)
            {
               TowerPlacementGrid.setCellWeight(%x, %y, 1);
               %dirty = true;
            }
         }
      }
   }
   else if (%this.currentOperation == %this.ERASEOPERATION)
   {
      // Remove a tower if the cell contains one
      if (%this.getCellTowerType(%mouseX, %mouseY) == %this.CELLTOWER)
      {
         %cell = TowerPlacementGrid.getCellFromWorldPosition(%mouseX @ " " @ %mouseY);
         TowerPlacementGrid.setCellWeight(getWord(%cell, 0), getWord(%cell, 1), 0);
         %dirty = true;
      }
   }
   
   if (%dirty)
   {
      %this.setGlobalDirtyState(true);
      %this.buildDisplayGrid(TerrainDisplayGrid);
      %dirty = false;
   }
}

/// <summary>
/// This function handles setting the current terrain operation based on the terrain tool operation 
/// selection buttons.
/// </summary>
function TerrainToolOperationButton::onClick(%this)
{
   if (TerrainTool.currentOperation == TerrainTool.ERASEOPERATION)
   {
      TerrainTool-->PreviewOperationErase.setVisible(0);
      TerrainTool-->PreviewArea.setVisible(1);
      if (TerrainTool.tabSelected == TerrainTool.GRAPHICSTAB )
      {
         TerrainTool-->PreviewAreaWindow.setVisible(1);
      }
   }
   
   TerrainTool.currentOperation = %this.operation;
   TerrainTool-->PreviewOperation.bitmap = %this.previewIcon;
   if (%this.previewIcon $= "")
   {
      TerrainTool-->PreviewOperation.setVisible(0);
   }
   else
   {
      TerrainTool-->PreviewOperation.setVisible(1);
   }
   
   // Handle erase
   if (TerrainTool.currentOperation == TerrainTool.ERASEOPERATION)
   {
      TerrainTool-->PreviewOperationErase.setVisible(1);
      TerrainTool-->PreviewArea.setVisible(0);
      TerrainTool-->PreviewAreaWindow.setVisible(0);
   }
}

/// <summary>
/// This function handles displaying the correct grid based on the currently selected tool mode.
/// </summary>
function TerrainToolShowGridButton::onClick(%this)
{
   if (%this.gridType $= "path")
   {
      // Show Path Button
      switch(TerrainTool.tabSelected)
      {
         case TerrainTool.GRAPHICSTAB:
            TerrainTool.graphicsShowPath = %this.getValue();
         
         case TerrainTool.PATHSTAB:
            TerrainTool.pathsShowPath = %this.getValue();
         
         case TerrainTool.TOWERSTAB:
            TerrainTool.towersShowPath = %this.getValue();
      }
   }
   else
   {
      // Show Towers Button
      switch(TerrainTool.tabSelected)
      {
         case TerrainTool.GRAPHICSTAB:
            TerrainTool.graphicsShowTower = %this.getValue();
         
         case TerrainTool.PATHSTAB:
            TerrainTool.pathsShowTower = %this.getValue();
         
         case TerrainTool.TOWERSTAB:
            TerrainTool.towersShowTower = %this.getValue();
      }
   }
   
   // Rebuild the grid display
   TerrainTool.buildDisplayGrid(TerrainDisplayGrid);
}

/// <summary>
/// This function updates the tool pane based on the currently selected too mode.
/// </summary>
/// <param name="tabName">The name of the currently selected tab.</param>
function TerrainToolBrushesTabBook::onTabSelected(%this, %tabName)
{
   if (!TerrainTool.toolInit)
      return;
   
   switch$(%tabName)
   {
      case "Graphics":
         TerrainTool.tabSelected = TerrainTool.GRAPHICSTAB;
         TerrainTool-->RotateLeftButton.setActive(1);
         TerrainTool-->RotateRightButton.setActive(1);
         TerrainTool-->AddGraphicButton.setActive(1);
         TerrainTool-->AddGraphicButton.setVisible(1);
         TerrainTool-->RemoveGraphicButton.setActive(1);
         TerrainTool-->RemoveGraphicButton.setVisible(1);
         TerrainTool-->PreviewText.setText("Optional graphic tiles to help build your level.");
         GraphicsGrid.renderGrid = 1;
         if (isObject(TerrainDisplayGrid))
         {
            TerrainDisplayGrid.renderCellImages = 1;
            TerrainDisplayGrid.setVisible(1);
            TerrainDisplayGrid.renderGrid = 0;
         }
         if (isObject(TowerPlacementGrid))
         {
            TowerPlacementGrid.renderGrid = 0;
         }
         TerrainTool.currentActivity = TerrainTool.GRAPHICSEDIT;
         
         // Make sure the Fill button is enabled
         TerrainTool-->FillButton.setActive(true);
         
         // Populate the preview control
         TerrainTool-->PreviewArea.bitmap = "";
         TerrainTool-->PreviewAreaWindow.setVisible(1);

         // Grid show buttons
         TerrainTool-->ShowPathButton.setActive(1);
         TerrainTool-->ShowPathButton.setStateOn(TerrainTool.graphicsShowPath);
         TerrainTool-->ShowTowerButton.setActive(1);
         TerrainTool-->ShowTowerButton.setStateOn(TerrainTool.graphicsShowTower);

         // Rebuild display
         TerrainTool.buildDisplayGrid(TerrainDisplayGrid);
         
      case "Enemy Paths":
         TerrainTool.tabSelected = TerrainTool.PATHSTAB;
         TerrainTool-->RotateLeftButton.setActive(0);
         TerrainTool-->RotateRightButton.setActive(0);
         TerrainTool-->AddGraphicButton.setActive(0);
         TerrainTool-->AddGraphicButton.setVisible(0);
         TerrainTool-->RemoveGraphicButton.setActive(0);
         TerrainTool-->RemoveGraphicButton.setVisible(0);
         %text = "The Emeny path will need to have a Start Point and End Point as well as a fully connected path between the two points before you can save your level. ";
         %text = %text @ "You can create up to 4 separate paths in 1 level.";
         TerrainTool-->PreviewText.setText(%text);
         GraphicsGrid.renderGrid = 0;
         TerrainDisplayGrid.renderCellImages = 1;
         TerrainDisplayGrid.setVisible(1);
         TerrainDisplayGrid.renderGrid = 1;
         TowerPlacementGrid.renderGrid = 0;
         TerrainTool.currentActivity = TerrainTool.PATHEDIT;
         
         // Disable the fill button
         if (TerrainTool.currentOperation == TerrainTool.FILLOPERATION)
         {
            TerrainTool-->PaintButton.performClick();
         }
         TerrainTool-->FillButton.setActive(false);
         
         // Populate the preview control
         TerrainTool-->PreviewArea.bitmap = "";
         TerrainTool-->PreviewAreaWindow.setVisible(0);

         // Grid show buttons
         TerrainTool-->ShowPathButton.setActive(0);
         TerrainTool-->ShowPathButton.setStateOn(TerrainTool.pathsShowPath);
         TerrainTool-->ShowTowerButton.setActive(1);
         TerrainTool-->ShowTowerButton.setStateOn(TerrainTool.pathsShowTower);

         // Rebuild display
         TerrainTool.buildDisplayGrid(TerrainDisplayGrid);

      case "Towers":
         TerrainTool.tabSelected = TerrainTool.TOWERSTAB;
         TerrainTool-->RotateLeftButton.setActive(0);
         TerrainTool-->RotateRightButton.setActive(0);
         TerrainTool-->AddGraphicButton.setActive(0);
         TerrainTool-->AddGraphicButton.setVisible(0);
         TerrainTool-->RemoveGraphicButton.setActive(0);
         TerrainTool-->RemoveGraphicButton.setVisible(0);
         TerrainTool-->PreviewText.setText("The Tower Placement Icon allows the player to drag their tower on to the field.");
         GraphicsGrid.renderGrid = 0;
         TerrainDisplayGrid.renderCellImages = 1;
         TerrainDisplayGrid.setVisible(1);
         TerrainDisplayGrid.renderGrid = 0;
         TowerPlacementGrid.renderGrid = 1;
         TerrainTool.currentActivity = TerrainTool.TOWEREDIT;
         
         // Make sure the Fill button is enabled
         TerrainTool-->FillButton.setActive(true);
         
         // Populate the preview control
         TerrainTool-->PreviewArea.bitmap = TDTerrainPathTImageMap.getImageName();
         TerrainTool-->PreviewAreaWindow.setVisible(0);

         // Grid show buttons
         TerrainTool-->ShowPathButton.setActive(1);
         TerrainTool-->ShowPathButton.setStateOn(TerrainTool.towersShowPath);
         TerrainTool-->ShowTowerButton.setActive(0);
         TerrainTool-->ShowTowerButton.setStateOn(TerrainTool.towersShowTower);

         // Rebuild display
         TerrainTool.buildDisplayGrid(TerrainDisplayGrid);
   }
}

/// <summary>
/// This function sets the preview area image based on the currently selected graphics brush.
/// </summary>
function TerrainToolGraphicsBrushButton::onClick(%this)
{

   TerrainTool.currentGraphicsImageMap = %this.imageMap;
   TerrainTool.currentGraphicsFrame = %this.frame;
   TerrainTool.currentGraphicsRotation = 0;

   TerrainTool-->PreviewAreaWindow.display(TerrainTool.currentGraphicsImageMap.getName(), "t2dStaticSprite");
   TerrainTool-->PreviewAreaWindow.sprite.setFrame(TerrainTool.currentGraphicsFrame);
   TerrainTool-->PreviewAreaWindow.sprite.setAngle(TerrainTool.currentGraphicsRotation * -90);
   TerrainTool-->PreviewAreaWindow.setVisible(1);
   
   TerrainTool-->PreviewArea.bitmap = "";

   if (TerrainTool.currentOperation == TerrainTool.ERASEOPERATION)
   {
      // Switch to a paint operation
      TerrainTool-->PaintButton.performClick();
   }
}

/// <summary>
/// This function validates the contents of the TerrainToolPathNameEdit control.
/// </summary>
function TerrainToolPathNameEdit::validateText(%this)
{
   %text = %this.getText();
   if (%text $= "")
   {
      %pathNum = %this.stackRoot.index + 1;
      WarningDialog.setupAndShow(TerrainToolWindow.getGlobalCenter(), "Invalid Path Name", "You cannot have a blank name for enemy path " @ %pathNum @ ".  Reverting to original name.", 
      "", "", "", "", "OK", %this.getId() @ ".resetText();");
      return false;
   }
   else if (%text !$= %this.grid.getInternalName())
   {
      TerrainTool.setGlobalDirtyState(true);
      
      // Copy the name to the text control
      %this.stackRoot-->NameTextCtrl.setText(%this.getText());
   }
   
   return true;
}

/// <summary>
/// This function requests validation if a return is entered in the TerrainToolPathNameEdit control.
/// </summary>
function TerrainToolPathNameEdit::onReturn(%this)
{
   %this.validateText();
}

/// <summary>
/// This function clears the contents of the TerrainToolPathNameEdit control.
/// </summary>
function TerrainToolPathNameEdit::resetText(%this)
{
   %this.setText(%this.grid.getInternalName());
}

// Start to edit a path
/// <summary>
/// This function requests path editing from the tool.
/// </summary>
function TerrainToolLayoutEditButton::onClick(%this)
{
   TerrainTool.startPathLayoutEdit(%this.stackRoot);
}

// Save a path after an edit
/// <summary>
/// This function requests save of the terrain data.
/// </summary>
function TerrainToolLayoutSaveButton::onClick(%this)
{
   TerrainTool.savePathLayoutEdit(%this.stackRoot);
}

// Cancel a path edit
/// <summary>
/// This function requests cancellation of path editing.
/// </summary>
function TerrainToolLayoutCancelButton::onClick(%this)
{
   TerrainTool.cancelPathLayoutEdit(%this.stackRoot);
}

/// <summary>
/// This function handles brush selection.
/// </summary>
function TerrainToolLayoutBrushButton::onClick(%this)
{
   TerrainTool.currentGrid = %this.grid;
   TerrainTool.currentGridIndex = %this.gridIndex;
   TerrainTool.currentGridBrush = %this.brush;
   TerrainTool-->PreviewArea.bitmap = %this.bitmapNormal;
   
   if (TerrainTool.currentOperation == TerrainTool.ERASEOPERATION)
   {
      // Switch to a paint operation
      TerrainTool-->PaintButton.performClick();
   }
}

/// <summary>
/// This function handles tower brush selection.
/// </summary>
function TerrainToolTowerBrushButton::onClick(%this)
{
   TerrainTool-->PreviewArea.bitmap = TDTerrainPathTImageMap.getImageName();
         
   if (TerrainTool.currentOperation == TerrainTool.ERASEOPERATION)
   {
      // Switch to a paint operation
      TerrainTool-->PaintButton.performClick();
   }
}

/// <summary>
/// This function asks the user to save if needed and closes the tool.
/// </summary>
function TerrainToolDoneButton::onClick(%this)
{
   // Validate all path text
   %invalidText = false;
   %count = TerrainTool-->PathsBrushesArray.getCount();
   for (%i=0; %i<%count; %i++)
   {
      %obj = TerrainTool-->PathsBrushesArray.getObject(%i);
      %check = %obj-->NameEditField.validateText();
      if (!%check)
      {
         %invalidText = true;
      }
   }
   
   // If there was invalid text then the user will be told about it.  We don't
   // want to close the dialog in this case.
   if (%invalidText)
      return;
   
   if ($Tools::TemplateToolDirty)
   {
      ConfirmDialog.setupAndShow(TerrainToolWindow.getGlobalCenter(), "Save Terrain Changes?", 
            "Save", "TerrainTool.saveTerrainData(false); Canvas.popDialog(TerrainTool);", 
            "Don't Save", "TerrainTool.doNotSaveTerrainData(); Canvas.popDialog(TerrainTool);", 
            "Cancel", "");
   }
   else
      Canvas.popDialog(TerrainTool);
}

/// <summary>
/// This function passes mouseDown events through the editor window.
/// </summary>
/// <param name="mx">The world X coordinate of the mouse.</param>
/// <param name="my">The world Y coordinate of the mouse.</param>
function LevelEditorTDTerrainTool::onMouseDown(%this, %mx, %my)
{
   %this.handleMouseMove(%mx, %my, false);
}

/// <summary>
/// This function passes mouseDragged events through the editor window.
/// </summary>
/// <param name="mx">The world X coordinate of the mouse.</param>
/// <param name="my">The world Y coordinate of the mouse.</param>
function LevelEditorTDTerrainTool::onMouseDragged(%this, %mx, %my)
{
   %this.handleMouseMove(%mx, %my, true);
}

/// <summary>
/// This function passes mouseMove events through the editor window.
/// </summary>
/// <param name="mx">The world X coordinate of the mouse.</param>
/// <param name="my">The world Y coordinate of the mouse.</param>
/// <param name="isDrag">Indicates that the mouse is being dragged.</param>
function LevelEditorTDTerrainTool::handleMouseMove(%this, %mx, %my, %isDrag)
{
   //echo("LevelEditorTDTerrainTool::onMouseDown() " @ %mx @ "," @ %my);
   
   switch(TerrainTool.currentActivity)
   {
      case TerrainTool.GRAPHICSEDIT:
         TerrainTool.editGraphics(%mx, %my, %isDrag);
      
      case TerrainTool.PATHEDIT:
         TerrainTool.editPath(%mx, %my, %isDrag);
      
      case TerrainTool.TOWEREDIT:
         TerrainTool.editTowers(%mx, %my, %isDrag);
   }
}
