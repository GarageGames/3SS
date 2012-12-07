//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
GuiFormManager::AddFormContent( "LevelBuilder", "ContentManagementToolBar", "LBContentManagementToolBar::CreateForm", "", 2 );


//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBContentManagementToolBar::CreateForm( %formCtrl )
{    

   //---------------------------------------------------------------------------------------------
   // Toolbar Buttons - Utility
   //---------------------------------------------------------------------------------------------
   // Trash Can
   LevelBuilderToolManager::addButtonToBar( %formCtrl, $LevelBuilder::GuiPath @ "/images/iconTrashCan", "MessageBoxOk(\"Removing Items From Your Library\", \"Drag an object from the library here to remove it.\");",  "Drag an object from the library here to remove it.", true, false, "SBCreateTrash" );
   // New ImageMap
   LevelBuilderToolManager::addButtonToBar( %formCtrl, $LevelBuilder::GuiPath @ "/images/iconImageAdd", "launchNewImageMap(false);",  "Create a new ImageMap" );
   // New Linked Image Map
   //LevelBuilderToolManager::addButtonToBar( %formCtrl, $LevelBuilder::GuiPath @ "/images/iconImageLinked", "launchNewLinkImageMap();",  "Create a new Linked ImageMap" );
   // New Animation
   LevelBuilderToolManager::addButtonToBar( %formCtrl, $LevelBuilder::GuiPath @ "/images/iconAnimationAdd", "AnimationBuilder.createAnimation();",  "Create a new Animation" );

   // Resize as appropriate.
   if( %formCtrl.isMethod("sizeContentsToFit") )
      %formctrl.sizeContentsToFit(%base, %formCtrl.contentID.margin);

   //*** Return back the base control to indicate we were successful
   return %base;

}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBContentManagementToolBar::SaveForm( %formCtrl, %contentObj )
{
}


function SBCreateTrash::onControlDropped(%this, %control, %position)
{
   if (isObject(%control.sceneObject))
   {
      
      %datablockName = %control.datablockName;
      %toolType      = %control.toolType;
      
      %scene = ToolManager.getLastWindow().getScene();
   
      switch$( %toolType )
      {
         case "t2dStaticSprite":
            SBCreateTrash::deleteImageMap(%datablockName);
         case "t2dAnimatedSprite":
            SBCreateTrash::deleteAnimation(%datablockName);
         case "Scroller":
            SBCreateTrash::deleteImageMap(%datablockName);
         case "TileLayer":
            SBCreateTrash::deleteTileLayer( %datablockName, %scene );
         case "ParticleEffect":
            SBCreateTrash::deleteParticleEffect( %datablockName, %scene );
         case "t2dShape3D":
            SBCreateTrash::deleteShape3D( %datablockName, %scene );
      }
   }
}

function SBCreateTrash::deleteImageMap( %imageMap )
{
   %scene = ToolManager.getLastWindow().getScene();
   %referenceList = new SimSet();
   %referenceCount = %scene.getImageMapReferenceList(%imageMap, %referenceList);
   
   %message = "Do you really want to delete this material?";
   if(%referenceCount > 0)
   {
      %message = getCountString("There {are} {count} object{s}", %referenceCount) @ " referencing this material. If the" NL
                                "material is deleted, the objects will also be deleted. Continue?";
   }
   
   %result = messageBox("Delete Material", %message, "OkCancel", "Question");
   
   if(%result == $MROk)
   {
      %referenceList.deleteContents();
      %imageMap.delete();
      GuiFormManager::SendContentMessage($LBCreateSiderBar, %this, "refreshall 1");
      LBProjectObj.persistToDisk(true);
   }
   
   %referenceList.delete();
}

function SBCreateTrash::deleteAnimation(%animation)
{
   %scene = ToolManager.getLastWindow().getScene();
   %referenceList = new SimSet();
   %referenceCount = %scene.getAnimationReferenceList(%animation, %referenceList);
   
   %message = "Do you really want to delete this animation?";
   if(%referenceCount > 0)
   {
      %message = getCountString("There {are} {count} object{s}", %referenceCount) @ " referencing this animation. If the" NL
                                "animation is deleted, the objects will also be deleted. Continue?";
   }
   
   %result = messageBox("Delete Animation", %message, "YesNo", "Question");
   
   // [neo, 15/6/2007 - #3233]
   //if(%result == $MRYes)
   if( %result == $MROK )
   {
      %referenceList.deleteContents();
      %animation.delete();
      GuiFormManager::SendContentMessage($LBCAnimatedSprite, %this, "refresh 1");
      LBProjectObj.persistToDisk(true);
   }
   
   %referenceList.delete();
}

function SBCreateTrash::deleteItem( %datablock )
{
   if( isObject( %datablock ) )
      %datablock.delete();
   
   else if( isFile( %datablock ) )
      fileDelete( %datablock );
}

function SBCreateTrash::deleteTileLayer( %filename, %scene )
{
   if( fileName( %filename ) $= "newLayer.lyr" )
      return;
      
   %count = %scene.getSceneObjectCount();
   %layers = "";
   for( %i = 0; %i < %count; %i++ )
   {
      %object = %scene.getSceneObject( %i );
      if( %object.getClassName() $= "TileLayer" )
      {
         if( %object.layerFile $= %filename )
            %layers = %layers SPC %object;
      }
   }
   
   %count = ToolManager.getRecycledObjectCount();
   for( %i = 0; %i < %count; %i++)
   {
      %object = ToolManager.getRecycledObject( %i );
      if( %object.getClassName() $= "TileLayer" )
      {
         if( %object.layerFile $= %filename )
            %layers = %layers SPC %object;
      }
   }
   
   %layers = trim( %layers );
   
   if( getWordCount( %layers ) > 0 )
   {
      %callback = "SBCreateTrash::doTileLayerDelete(\"" @ %filename @ "\", \"" @ %layers @ "\");";
      MessageBoxYesNo( "Delete Objects", "There are objects using this layer in your scene. If you continue, " @
                       "they will be deleted. Continue?", %callback, "" );
   }
   else
      SBCreateTrash::doTileLayerDelete( %filename, %layers );
}

function SBCreateTrash::doTileLayerDelete( %filename, %layers )
{
   %count = getWordCount( %layers );
   for( %i = 0; %i < %count; %i++ )
   {
      %layer = getWord( %layers, %i );
      %layer.delete();
   }
   
   fileDelete( %filename );
   GuiFormManager::SendContentMessage($LBCreateSiderBar, 0, "refreshAll 0");
}

function SBCreateTrash::deleteParticleEffect( %filename, %scene )
{
   if( fileName( %filename ) $= "newEffect.eff" )
      return;
      
   %count = %scene.getSceneObjectCount();
   %effects = "";
   for( %i = 0; %i < %count; %i++ )
   {
      %object = %scene.getSceneObject( %i );
      if( %object.getClassName() $= "ParticleEffect" )
      {
         if( %object.effectFile $= %filename )
            %effects = %effects SPC %object;
      }
   }
   
   %count = ToolManager.getRecycledObjectCount();
   for( %i = 0; %i < %count; %i++)
   {
      %object = ToolManager.getRecycledObject( %i );
      if( %object.getClassName() $= "ParticleEffect" )
      {
         if( %object.effectFile $= %filename )
            %effects = %effects SPC %object;
      }
   }
   
   %effects = trim( %effects );
   if( %effects $= "" )
      return SBCreateTrash::doParticleEffectDelete( %filename, %effects );
   
   if( getWordCount( %effects ) > 0 )
   {
      %callback = "SBCreateTrash::doParticleEffectDelete(\"" @ %filename @ "\", \"" @ %effects @ "\");";
      MessageBoxYesNo( "Delete Objects", "There are objects using this effect in your scene. If you continue, " @
                       "they will be deleted. Continue?", %callback, "" );
   }
}

function SBCreateTrash::doParticleEffectDelete( %filename, %effects )
{
   %count = getWordCount( %effects );
   for( %i = 0; %i < %count; %i++ )
   {
      %effect = getWord( %effects, %i );
      if( isObject( %effect ) )
         %effect.delete();
   }
   
   fileDelete( %filename );
   GuiFormManager::SendContentMessage($LBCreateSiderBar, 0, "refreshAll 1");
}

function SBCreateTrash::deleteShape3D( %filename, %scene )
{
   %count = %scene.getSceneObjectCount();
   %shapes = "";
   for( %i = 0; %i < %count; %i++ )
   {
      %object = %scene.getSceneObject( %i );
      if( %object.getClassName() $= "t2dShape3D" )
      {
         if( %object.shape $= %filename )
            %shapes = %shapes SPC %object;
      }
   }
   
   %count = ToolManager.getRecycledObjectCount();
   for( %i = 0; %i < %count; %i++)
   {
      %object = ToolManager.getRecycledObject( %i );
      if( %object.getClassName() $= "t2dShape3D" )
      {
         if( %object.shape $= %filename )
            %shapes = %shapes SPC %object;
      }
   }
   
   %shapes = trim( %shapes );
   
   if( getWordCount( %shapes ) > 0 )
   {
      %callback = "SBCreateTrash::doShape3DDelete(\"" @ %filename @ "\", \"" @ %shapes @ "\");";
      MessageBoxYesNo( "Delete Objects", "There are objects using this shape in your scene. If you continue, " @
                       "they will be deleted. Continue?", %callback, "" );
   }
   else
      SBCreateTrash::doShape3DDelete( %filename, %shapes );
}

function SBCreateTrash::doShape3DDelete( %filename, %shapes )
{
   %msg = "Do you want to save changes to document " @ %documentName @ "?";
   
   if( %cancelOption == true ) 
      %buttons = "SaveDontSaveCancel";
   else
      %buttons = "SaveDontSave";
      
   %mbResult = MessageBox( "Delete Textures", "Would you like to delete the texture files associated with this 3D shape?",
                     "SaveDontSaveCancel", "Question" );   
   
   // Cancel aborts deleting the shape
   if( %mbResult $= $MRCancel ) 
      return false;
   else if( %mbResult $= $MROk )
      // Yes deletes the shape and it's textures
      SBCreateTrash::deleteShape3DTextures(%filename ,%shapes ,true );
   else if( %mbResult $= $MRDontSave )
      // No delete just the DTS file
      SBCreateTrash::deleteShape3DTextures(%filename ,%shapes ,false );
}                       

function SBCreateTrash::deleteShape3DTextures( %filename, %shapes, %deleteTextures )
{
   %count = getWordCount( %shapes );
   for( %i = 0; %i < %count; %i++ )
   {
      %shape = getWord( %shapes, %i );
      if( isObject( %shape ) )
         %shape.delete();
   }
   
   if( %deleteTextures && isFile( %fileName ) )
   {
      %shape = new t2dShape3D() { shape = %filename; };
      %textureList = %shape.getTextureFileList();
      %shape.delete();
      %textureCount = getFieldCount( %textureList );
      for( %i = 0; %i < %textureCount; %i++ )
      {
         %texture = getField( %textureList, %i );
         
         if( !isFile( %texture ) )
            continue;
         
         fileDelete( %texture );
      }
   }
   
   fileDelete( %filename );
   GuiFormManager::SendContentMessage($LBC3DShape, 0, "destroy");
   purgeResources();
   GuiFormManager::SendContentMessage($LBC3DShape, 0, "refresh");
}
