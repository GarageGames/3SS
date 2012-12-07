//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LevelBuilderSceneEdit::setHistoryUndoManager(%this, %undoManager)
{
   GuiFormManager::SendContentMessage($LBHistoryViewContent, %this, "setUndoManager" SPC %undoManager);
}

function LevelBuilderSceneEdit::onAcquireObject(%this, %object)
{   
   if (%object.getClassName() $= "TileMap")
   {
      %this.clearAcquisition(%object);
      return;
   }

   GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspect" SPC %object );
   
   // select the object in tree view
   GuiFormManager::SendContentMessage( $LBTreeViewContent, %this, "setSelection" SPC %object);
   
   //if (%object.getClassName() $= "ParticleEffect")
   //{
      //LevelBuilderParticleEditor.onEffectAcquired(%object);
   //}
}

function LevelBuilderSceneEdit::onAcquireObjectSet(%this, %object)
{
   GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspect" SPC %object );
 
    // select the objects in tree view
   GuiFormManager::SendContentMessage( $LBTreeViewContent, %this, "setSelections" SPC %object);  
}

function LevelBuilderSceneEdit::onObjectDoubleClicked(%this, %object)
{
   if( %object.getClassName() $= "TextObject" )
      ToolManager.editTextObject( %object );
      
   levelBuilderSetEditPanel(1);
}

function LevelBuilder::reloadTileLayers(%layerFile)
{
   if (isObject(ToolManager) && isObject(ToolManager.getLastWindow()))
   {
      %scene = ToolManager.getLastWindow().getScene();
      if (isObject(%scene))
      {
         %count = %scene.getSceneObjectCount();
         for (%i = 0; %i < %count; %i++)
         {
            %object = %scene.getSceneObject(%i);
            if (%object.getClassName() $= "TileLayer")
            {
               if (%object.layerFile $= %layerFile)
               {
                  %object.loadTileLayer(%layerFile);
               }
            }
         }
      }
   }
}

function LevelBuilderSceneEdit::onQuickEdit(%this, %object)
{
   %object.onChanged();
   ToolManager.updateAcquiredObjectSet();
}

function LevelBuilderSceneEdit::onObjectChanged(%this, %object)
{
   %object.onChanged();
   GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspectUpdate" );
}

function LevelBuilderSceneEdit::onObjectSpatialChanged(%this, %object)
{
   %object.onChanged();
   GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspectSpatial" SPC %object );
   ToolManager.updateAcquiredObjectSet();
}

function LevelBuilderCameraTool::onCameraChanged(%this, %cameraPos, %cameraSize)
{
   if (ToolManager.getAcquiredObjectCount() < 1)
   {
      %scene = ToolManager.getLastWindow().getScene();
      %scene.cameraPosition = %cameraPos;
      %scene.cameraSize = %cameraSize;
      GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspect" );
   }
}

function LevelBuilderCreateTool::onObjectCreated(%this, %object, %class)
{
   if (%class $= "Path")
      ToolManager.editPath(%object);
   else if( %class $= "TextObject" )
   {
      %this.onTextObjectCreated( %object );
      ToolManager.editTextObject(%object);
   }
   else if( %class $= "TileLayer" )
      ToolManager.editTileLayer( %object );
   else
      LevelBuilderToolManager::setTool(LevelEditorSelectionTool);
}

function LevelBuilderCreateTool::onTextObjectCreated( %this, %object )
{
   %object.hideOverlap = false;
   if( %object.getWidth() > 2.0 )
   {
      %object.autoSize = false;
      %object.wordWrap = true;
   }
   else
   {
      %object.wordWrap = false;
      %object.autoSize = true;
   }
   
   %object.font = "Arial";
   %object.lineHeight = 10;
   %camHeight = 75;
   if( isObject( ToolManager.getLastWindow() ) && isObject( ToolManager.getLastWindow().getScene() ) )
      %camHeight = getWord( ToolManager.getlastWindow().getScene().cameraSize, 1 );
      
   %object.addAutoFontSize( $levelEditor::DesignResolutionY, %camHeight, %object.lineHeight );
   %object.textAlign = "Left";
   %object.aspectRatio = 1;
   %object.lineSpacing = 0;
   %object.characterSpacing = 0;
}

function LevelBuilderSceneEdit::onRelinquishObject(%this, %object)
{
   %count = %this.getAcquiredObjectCount();
   %acquiredObjects = %this.getAcquiredObjects();
   %acquiredGroup = %this.getAcquiredGroup();
   if (%count == 0)
      GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspect" );
   else if (%count == 1)
      GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspect" SPC %acquiredObjects.getObject(0));
   else if (isObject(%acquiredGroup))
      GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspect" SPC %acquiredGroup);
   else
      GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspect" SPC %acquiredObjects);
   
   // clear the selected object from tree view
   GuiFormManager::SendContentMessage( $LBTreeViewContent, %this, "clearSelection" SPC %object);
}

// Pass in the initiating delete action   [KNM | 08/10/11 | ITGB-152]
function LevelBuilderSceneEdit::onPreObjectDeleted(%this, %object, %deleteAction)
{
   // rdbnote: added this so objects will properly dismount when being deleted
   //  this also takes care of Path objects, even if they are the object
   //  being deleted
   //%this.clearAllMounts(%object, %deleteAction);
}

$LevelEditor::DeletedPersistentObjects = new SimSet();
function LevelBuilderSceneEdit::onPostObjectDeleted(%this, %object)
{
   if (%object.getPersistent())
   {
      %object.setPersistent(false);
      $LevelEditor::DeletedPersistentObjects.add(%object);
   }
   
   GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspect" );

   // clear the selected object from tree view
   GuiFormManager::SendContentMessage( $LBTreeViewContent, %this, "clearSelection" SPC %object);
   
   // Deleted objects no longer need to be saved.
   if( %object.getClassName() $= "ParticleEffect" )
      %object.onEffectSaved();
   else if( %object.getClassName() $= "TileLayer" )
      %object.onLayerSaved();
}

function LevelBuilderSceneEdit::onCameraChanged( %this, %oldPos, %oldSize, %newPos, %newSize )
{
   %oldX = getWord( %oldPos, 0 );
   %oldY = getWord( %oldPos, 1 );
   %newX = getWord( %newPos, 0 );
   %newY = getWord( %newPos, 1 );
   
   %oldHalfWidth = getWord( %oldSize, 0 ) * 0.5;
   %oldHalfHeight = getWord( %oldSize, 1 ) * 0.5;
   %newHalfWidth = getWord( %newSize, 0 ) * 0.5;
   %newHalfHeight = getWord( %newSize, 1 ) * 0.5;
      
   // Update camera guides for proper snapping.
   ToolManager.removeXGuide( %oldX - %oldHalfWidth );
   ToolManager.removeXGuide( %oldX + %oldHalfWidth );
   ToolManager.removeYGuide( %oldY - %oldHalfHeight );
   ToolManager.removeYGuide( %oldY + %oldHalfHeight );
   
   ToolManager.addXGuide( %newX - %newHalfWidth );
   ToolManager.addXGuide( %newX + %newHalfWidth );
   ToolManager.addYGuide( %newY - %newHalfHeight );
   ToolManager.addYGuide( %newY + %newHalfHeight );
}

function LevelBuilderSceneEdit::onObjectResurrected(%this, %object)
{
   if ($LevelEditor::DeletedPersistentObjects.isMember(%object))
      %object.setPersistent(true);

   // Force saving
   %object.onChanged();
}

function LevelBuilderSceneEdit::editCollisionPoly(%this, %object)
{
   //LevelBuilderToolManager::setTool(LevelEditorPolyTool);
   //LevelEditorPolyTool.editObject(%object);
   CollisionPolyEditor.open(%object);
}

function LevelBuilderSceneEdit::editShapeVector(%this, %object)
{
   ShapeVectorEditor.open(%object);
}

function LevelBuilderSceneEdit::editSortPoint(%this, %object)
{
   LevelBuilderToolManager::setTool(LevelEditorSortPointTool);
   LevelEditorSortPointTool.editObject(%object);
}

function LevelBuilderSceneEdit::editTileLayer(%this, %object)
{
   LevelBuilderToolManager::setTool(LevelEditorTileMapEditTool);
   LevelEditorTileMapEditTool.editObject(%object);
   $editingTileLayer = %object;
   levelBuilderSetEditPanel(1);
}

function LevelBuilderSceneEdit::editTextObject( %this, %object )
{
   LevelBuilderToolManager::setTool(LevelEditorTextEditTool);
   LevelEditorTextEditTool.editObject(%object);
   levelBuilderSetEditPanel(1);
}

function LevelBuilderSceneEdit::createTextObject( %this, %pos )
{
   LevelBuilderToolManager::setCreateTool( "TextObject" );
   ToolManager.getActiveTool().startCreate(ToolManager.getLastWindow(), %pos);
}

function LevelBuilderSceneEdit::editPath(%this, %object)
{
   LevelBuilderToolManager::setTool(LevelEditorPathEditTool);
   LevelEditorPathEditTool.editObject(%object);
   levelBuilderSetEditPanel(1);
}

function LevelBuilderSceneEdit::onFinishEdit(%this, %object)
{
   LevelBuilderToolManager::setTool(LevelEditorSelectionTool);
   LevelEditorSelectionTool.refreshWidgets();
}

function LevelBuilderSceneEdit::editImageMap(%this, %object)
{
   launchEditImageMap(%object.getImageMap());
}

function LevelBuilderSceneEdit::editAnimation(%this, %object)
{
   AnimationBuilder.editAnimation(%object.getAnimation());
}

function LevelBuilder::iOSResolutionFromSetting(%deviceType, %deviceScreenOrientation)
{
   // A helper function to get a string based resolution from the settings given.
   %x = 0;
   %y = 0;
   
   switch(%deviceType)
   {
      case $iOS::constant::iPhone:
         if(%deviceScreenOrientation == $iOS::constant::Landscape)
         {
            %x =  $iOS::constant::iPhoneWidth;
            %y =  $iOS::constant::iPhoneHeight;
         }
         else
         {
            %x =  $iOS::constant::iPhoneHeight;
            %y =  $iOS::constant::iPhoneWidth;
         }
      
      case $iOS::constant::iPhone4:
         if(%deviceScreenOrientation == $iOS::constant::Landscape)
         {
            %x =  $iOS::constant::iPhoneWidth;
            %y =  $iOS::constant::iPhoneHeight;
         }
         else
         {
            %x =  $iOS::constant::iPhoneHeight;
            %y =  $iOS::constant::iPhoneWidth;
         }
         
      case $iOS::constant::iPad:
         if(%deviceScreenOrientation == $iOS::constant::Landscape)
         {
            %x =  $iOS::constant::iPadWidth;
            %y =  $iOS::constant::iPadHeight;
         }
         else
         {
            %x =  $iOS::constant::iPadHeight;
            %y =  $iOS::constant::iPadWidth;
         }
   }
   return %x @ " " @ %y;
}