//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
GuiFormManager::AddFormContent( "LevelBuilderQuickEditClasses", "TileLayer", "LBQETileLayer::CreateContent", "LBQETileLayer::SaveContent", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBQETileLayer::CreateContent( %contentCtrl, %quickEditObj )
{
   %base = %contentCtrl.createBaseStack("LBQETileLayerClass", %quickEditObj);
   %rollout = %base.createRolloutStack("Tile Map", true);
   %rollout.createCheckBox("GridActive", "Render Grid");   
   %rollout.createTextEdit2("tileCountXAndWarn", "tileCountYAndWarn", 0, "Tile Count", "X", "Y", "Tile Count");
   %rollout.createTextEdit2("tileSizeX", "tileSizeY", 0, "Tile Size", "X", "Y", "Tile Size");
   %rollout.createCommandButton("TileBuilder::sizeObjectToLayer();", "Size Object to Layer");
   %rollout.createSpacer( 16 );
   
   
   %hiddenTest = "ToolManager.getActiveTool().getId() == LevelEditorTileMapEditTool.getId();";
   %hidden = %rollout.createHideableStack(%hiddenTest);
   %hidden.createCommandButton("TileBuilder::editSelectedTileLayer();", "Edit Tile Layer");
   
   %hiddenBrushesTest = "ToolManager.getActiveTool().getId() != LevelEditorTileMapEditTool.getId();";
   %hiddenBrushes = %rollout.createHideableStack(%hiddenBrushesTest);
   %brushRollout = %hiddenBrushes.createBrushStack("Tile Editing", true);
   
   // Create Layer Persistence Buttons
   %brushRollout.createLayerPersistence();
   
   %toolbar = %brushRollout.createToolbar();
   %toolbar.addSpacer();
   %toolbar.addTool("Select", "LevelEditorTileMapEditTool.setSelectTool();", "^{TileLayerEditor}/selectTool");
   LevelEditorTileMapEditTool.paintTool = %toolbar.addTool("Paint", "LevelEditorTileMapEditTool.setPaintTool();", "^{TileLayerEditor}/paintTool");
   %toolbar.addTool("Flood Fill", "LevelEditorTileMapEditTool.setFloodTool();", "^{TileLayerEditor}/floodTool");
   %toolbar.addTool("Eye Dropper", "LevelEditorTileMapEditTool.setEyeTool();", "^{TileLayerEditor}/eyeTool");
   %toolbar.addTool("Eraser", "LevelEditorTileMapEditTool.setEraserTool();", "^{TileLayerEditor}/eraserTool");
   %toolbar.addSpacer();
   
   LevelEditorTileMapEditTool.paintTool.performClick();
   
   %brushRollout.createDropDownList("brush", "Brush", $brushSet, "BLANK", "", false);
   %brushRollout.createT2DDatablockList("image", "Image", "ImageAsset AnimationAsset", "", "No Change\tNone", false);
   
   %frameStack = %brushRollout.createHideableStack("hideFrameQuickEdit();");
   %frameStack.createT2DFramePicker( "frame", "Frame", "", false );
   
   %brushRollout.createDropDownEditList("tileScript", "Tile Script", $tileScriptSet, "", "", false);
   %brushRollout.createDropDownEditList("customData", "Custom Data", $customDataSet, "", "", false);
   %brushRollout.createCheckBox("flipX", "Flip Horizontal", "", "", true, false);
   %brushRollout.createCheckBox("flipY", "Flip Vertical", "", "", true, false);
   %brushRollout.createSpacer(6);
   %brushRollout.createBrushPreview();
   %brushRollout.createSpacer(16);
   %brushRollout.createCommandButton("ActiveBrush.apply();", "Apply To Selection");
   %brushRollout.createSpacer(16);
   %brushRollout.createTextCommandButton("ActiveBrush.save", "Save Brush", "ActiveBrush.getBrush();");
   %brushRollout.createCommandButton("ActiveBrush.deleteBrush();", "Delete Brush", "", 148, 78);
   
   // Return Ref to Base.
   %base.hiddenStack1 = %hidden;
   %base.hiddenStack2 = %hiddenBrushes;
   %base.brushesRollout = %brushRollout;
   $TileEditor::QuickEditPane = %base;
   return %base;
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBQETileLayer::SaveContent( %contentCtrl )
{
   // Nothing.
}

$TileEditor::RequestedTileCountX = -1;
function TileLayer::setTileCountXAndWarn(%this, %count)
{
   %currentCount = %this.getTileCountX();
   if (%count < %currentCount)
   {
      $TileEditor::RequestedTileCountX = %count;
      MessageBoxOKCancel("Warning", "Reducing the tile count will erase the tiles outside the bounds of the layer and cannot be undone. Proceed?",
                         "doSetTileCountX(" @ %this @ ", " @ %count @ ");", "refreshTileCount();");
   }
   
   else
      doSetTileCountX(%this, %count);
}

function doSetTileCountX(%layer, %count)
{
   $TileEditor::RequestedTileCountX = -1;
   %layer.setTileCountX(%count);
}

function TileLayer::getTileCountXAndWarn(%this)
{
   %count = $TileEditor::RequestedTileCountX;
   if (%count < 0)
      %count = %this.getTileCountX();
   return %count;
}

$TileEditor::RequestedTileCountY = -1;
function TileLayer::setTileCountYAndWarn(%this, %count)
{
   %currentCount = %this.getTileCountY();
   if (%count < %currentCount)
   {
      $TileEditor::RequestedTileCountY = %count;
      MessageBoxOKCancel("Warning", "Reducing the tile count will erase the tiles outside the bounds of the layer and cannot be undone. Proceed?",
                         "doSetTileCountY(" @ %this @ ", " @ %count @ ");", "refreshTileCount();");
   }
   
   else
      doSetTileCountY(%this, %count);
}

function doSetTileCountY(%layer, %count)
{
   $TileEditor::RequestedTileCountY = -1;
   %layer.setTileCountY(%count);
}

function TileLayer::getTileCountYAndWarn(%this)
{
   %count = $TileEditor::RequestedTileCountY;
   if (%count < 0)
      %count = %this.getTileCountY();
      
   return %count;
}

function refreshTileCount()
{
   $TileEditor::RequestedTileCountY = -1;
   $TileEditor::RequestedTileCountX = -1;
   GuiFormManager::SendContentMessage( $LBQuickEdit, "", "inspectUpdate" );
}
