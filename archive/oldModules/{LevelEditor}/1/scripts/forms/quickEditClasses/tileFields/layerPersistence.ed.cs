//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$T2D::TileLayerSpec = "TGB Tile Layers (*.lyr)|*.lyr|All Files (*.*)|*.*|";

function LBQuickEditContent::createLayerPersistence(%this )
{
   %container = new GuiControl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorPanelLight";
      HorizSizing = "right";
      VertSizing = "bottom";
      position = "324 186";
      Extent = "230 37";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
   };
   
   %loadEffectButton = new GuiIconButtonCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorButton";
      HorizSizing = "right";
      VertSizing = "bottom";
      class = LayerLoadButton;
      position = "6 7";
      Extent = "94 23";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      text = "Load Layer";
      groupNum = "-1";
      buttonType = "PushButton";
      buttonMargin = "4 4";
      iconBitmap = expandPath("^{EditorAssets}/gui/iconOpen.png");
      iconLocation = "Left";
      sizeIconToButton = "0";
      textLocation = "Right";
      textMargin = "4";
   };
   
   %saveEffectButton = new GuiIconButtonCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorButton";
      class = LayerSaveButton;
      HorizSizing = "left";
      VertSizing = "bottom";
      position = "130 7";
      Extent = "94 23";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      text = "Save Layer";
      groupNum = "-1";
      buttonType = "PushButton";
      buttonMargin = "4 4";
      iconBitmap = expandPath("^{EditorAssets}/gui/iconSave.png");
      iconLocation = "Left";
      sizeIconToButton = "0";
      textLocation = "Right";
      textMargin = "4";   
   };
   
   %container.add(%loadEffectButton);
   %container.add(%saveEffectButton);
   
   %this.add(%container);
   
   return %container;
}


function LayerSaveButton::onClick( %this )
{
   if( !isObject( $editingTileLayer ) )
      return;
   %callback = "GuiFormManager::SendContentMessage(" @ $LBCTileMap @ "," @ %this @ ", refresh);";
   TileEditor::saveLayer( $editingTileLayer, %callback, true );
}

function LayerLoadButton::onClick( %this )
{
   %layerObject = $editingTileLayer;
   if( !isObject( %layerObject ) )
      return;
      
      
   %currentLayer = %layerObject.LayerFile;
   if( fileName(%currentLayer) $= $TileEditor::NewLayerFileName )
      %currentLayer = "";
      
   // getLoadFileName Here result to doLoadEffect
   %fileName = TileLayer::getLevelOpenName( %currentLayer );

   if( %fileName $= "" )
      return;      
      
   %position = %layerObject.getPosition();
   %size = %layerObject.getSize();
      
   %layerObject.loadTileLayer( %fileName );
   
   %layerObject.setSize( GetWord( %size, 0 ), GetWord( %size, 1 ) );
   %layerObject.setPosition( GetWord( %position, 0 ), GetWord( %position, 1 ) );
      
   // Update Quick Edit
   GuiFormManager::BroadcastContentMessage( "LevelBuilderQuickEditClasses", %this, "syncQuickEdit" SPC %layerObject );
   // refresh the particle object library
   GuiFormManager::BroadcastContentMessage( "LevelBuilderSidebarCreate", 0, "refresh" );

}