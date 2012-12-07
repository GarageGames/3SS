//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
GuiFormManager::AddFormContent( "LevelBuilderQuickEditClasses", "Scene", "LBQEScene::CreateContent", "LBQEScene::SaveContent", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBQEScene::CreateContent( %contentCtrl, %quickEditObj )
{
   %base = %contentCtrl.createBaseStack("LBQESceneObjectClass", %quickEditObj);
   %rollout = %base.createRolloutStack("Camera", true);
   %rollout.createTextEdit2("CameraPositionX", "CameraPositionY", 3, "Camera", "X", "Y", "Set the camera Position");
   %rollout.createTextEdit2("CameraSizeX", "CameraSizeY", 3, "", "Width", "Height", "Set the camera Size");
   %rollout.createTextEdit2("DesignResolutionX", "DesignResolutionY", 0, "Design Resolution", "X", "Y", "Set the design resolution");
   
   %scriptingRollout = %base.createRolloutStack("Scene Graph Scripting");
   %scriptingRollout.createTextEdit("Name", "TEXT", "Name", "Name the Object for Referencing in Script");
   %scriptingRollout.createTextEdit("Class", "TEXT", "Class", "Link this Object to a Class");
   %scriptingRollout.createTextEdit("SuperClass", "TEXT", "Super Class", "Link this Object to a Parent Class");
   
   %sortRollout = %base.createRolloutStack("Layer Management");
   //%sortRollout.createCheckBox("UseLayerSorting", "Use Layer Sorting", "Enables sorting of objects before the layer is rendered.");
   //%sortRollout.createLabel("Layer      L   H       Sort Mode");
   for ( %i = 0; %i < 32; %i++ )
      %sortRollout.createLayerManager( %i );

   %debugRollout = %base.createRolloutStack("Debug Rendering");

   %debugRollout.createCheckBox("ShowMetrics", "Metrics", "Show System Metrics");
   %debugRollout.createCheckBox("ShowJoints", "Joints", "Show Scene Joints");
   %debugRollout.createCheckBox("ShowAABB", "AABB", "Show Scene-Object Axis-Aligned Bounding Boxes");
   %debugRollout.createCheckBox("ShowOOBB", "OOBB", "Show Scene-Object Object-Orientated Bounding Boxes");
   %debugRollout.createCheckBox("ShowAsleep", "Asleep", "Show Scene-Object Asleep/Awake Status");
   %debugRollout.createCheckBox("ShowCollisionShapes", "Collision Shapes", "Show Scene-Object Collision Shape(s)");
   %debugRollout.createCheckBox("ShowPositionAndCOM", "Position/Center-of-Mass", "Show Scene-Object Position and Center of Mass");
   %debugRollout.createCheckBox("ShowSortPoints", "Sort Points", "Show Scene-Object Sort Points");
   
   %dynamicFieldRollout = %base.createRolloutStack("Dynamic Fields");
   %dynamicFieldRollout.createDynamicFieldStack();
   
   // Return Ref to Base.
   return %base;

}


//-Mat for setting default values from the buttons
function LBQEScene::setPortraitValues( %this ) {
   $currentScene.setCameraSizeX( 75 );
   $currentScene.setCameraSizeY( 100 );
   
   $currentScene.setDesignResolutionX( 320 );
   $currentScene.setDesignResolutionY( 480 );
   $pref::iPhone::ScreenOrientation = "Portrait";
}

function LBQEScene::updateCameraValues( %this ) 
{
   switch($pref::iOS::DeviceType)
   {
      case $iOS::constant::iPhone:
         if($pref::iOS::ScreenOrientation == $iOS::constant::Landscape)
         {
            $currentScene.setCameraSizeX( $iOS::constant::iPhoneWidth );
            $currentScene.setCameraSizeY( $iOS::constant::iPhoneHeight );
            
            $currentScene.setDesignResolutionX( $iOS::constant::iPhoneWidth );
            $currentScene.setDesignResolutionY( $iOS::constant::iPhoneHeight );
         }
         else
         {
            $currentScene.setCameraSizeX( $iOS::constant::iPhoneHeight );
            $currentScene.setCameraSizeY( $iOS::constant::iPhoneWidth );
            
            $currentScene.setDesignResolutionX( $iOS::constant::iPhoneHeight );
            $currentScene.setDesignResolutionY( $iOS::constant::iPhoneWidth );
         }
         
      case $iOS::constant::iPhone4:
         echo("iPhone 4 device, do nothing to editor window");
         
      case $iOS::constant::iPad:
         if($pref::iOS::ScreenOrientation == $iOS::constant::Landscape)
         {
            $currentScene.setCameraSizeX( $iOS::constant::iPadWidth );
            $currentScene.setCameraSizeY( $iOS::constant::iPadHeight );
            
            $currentScene.setDesignResolutionX( $iOS::constant::iPadWidth );
            $currentScene.setDesignResolutionY( $iOS::constant::iPadHeight );
         }
         else
         {
            $currentScene.setCameraSizeX( $iOS::constant::iPadHeight );
            $currentScene.setCameraSizeY( $iOS::constant::iPadWidth );
            
            $currentScene.setDesignResolutionX( $iOS::constant::iPadHeight );
            $currentScene.setDesignResolutionY( $iOS::constant::iPadWidth );            
         }
   }
}


//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBQEScene::SaveContent( %contentCtrl )
{
   // Nothing.
}

function Scene::setCameraPositionX(%this, %value)
{
   %oldPos = %this.cameraPosition;
   %oldSize = %this.cameraSize;
   
   %this.cameraPosition = %value SPC %this.getCameraPositionY();
   if (ToolManager.getActiveTool().getID() == LevelEditorCameraTool.getID())
      LevelEditorCameraTool.setCameraPosition(%this.cameraPosition);
   
   %newPos = %this.cameraPosition;
   %newSize = %this.cameraSize;
   ToolManager.onCameraChanged( %oldPos, %oldSize, %newPos, %newSize );
}

function Scene::setCameraPositionY(%this, %value)
{
   %oldPos = %this.cameraPosition;
   %oldSize = %this.cameraSize;
   
   %this.cameraPosition = %this.getCameraPositionX() SPC %value;
   if (ToolManager.getActiveTool().getID() == LevelEditorCameraTool.getID())
      LevelEditorCameraTool.setCameraPosition(%this.cameraPosition);
   
   %newPos = %this.cameraPosition;
   %newSize = %this.cameraSize;
   ToolManager.onCameraChanged( %oldPos, %oldSize, %newPos, %newSize );
}

function Scene::getCameraPositionX(%this)
{
   return getWord(%this.cameraPosition, 0);
}

function Scene::getCameraPositionY(%this)
{
   return getWord(%this.cameraPosition, 1);
}

function Scene::setCameraSizeX(%this, %value)
{
   %oldPos = %this.cameraPosition;
   %oldSize = %this.cameraSize;
   
   %this.cameraSize = %value SPC %this.getCameraSizeY();
   if(isObject(ToolManager.getActiveTool()))
   {
      if (ToolManager.getActiveTool().getID() == LevelEditorCameraTool.getID())
         LevelEditorCameraTool.setCameraSize(%this.cameraSize);
   }
   
   %newPos = %this.cameraPosition;
   %newSize = %this.cameraSize;
   ToolManager.onCameraChanged( %oldPos, %oldSize, %newPos, %newSize );
}

function Scene::setCameraSizeY(%this, %value)
{
   %oldPos = %this.cameraPosition;
   %oldSize = %this.cameraSize;
   
   %this.cameraSize = %this.getCameraSizeX() SPC %value;
   
   if(isObject(ToolManager.getActiveTool()))
   {
      if (ToolManager.getActiveTool().getID() == LevelEditorCameraTool.getID())
         LevelEditorCameraTool.setCameraSize(%this.cameraSize);
   }
   
   %newPos = %this.cameraPosition;
   %newSize = %this.cameraSize;
   ToolManager.onCameraChanged( %oldPos, %oldSize, %newPos, %newSize );
}

function Scene::getCameraSizeX(%this)
{
   return getWord(%this.cameraSize, 0);
}

function Scene::getCameraSizeY(%this)
{
   return getWord(%this.cameraSize, 1);
}

function Scene::setClass(%this, %class)
{
   %this.class = %class;
}

function Scene::getClass(%this, %class)
{
   return %this.class;
}

function Scene::setSuperClass(%this, %class)
{
   %this.superClass = %class;
}

function Scene::getSuperClass(%this, %class)
{
   return %this.superClass;
}

function Scene::setUseLayerSorting(%this, %enable)
{
   %this.useLayerSorting = %enable;
}

function Scene::getUseLayerSorting(%this)
{
   return %this.useLayerSorting;
}


// ----------------------------------------------------------------------------------
// Debug Rendering.
// ----------------------------------------------------------------------------------

function Scene::setShowMetrics(%this, %value)
{
   if (%value)
      %this.setDebugOn(0);
   else
      %this.setDebugOff(0);
}


function Scene::setShowJoints(%this, %value)
{
   if (%value)
      %this.setDebugOn(1);
   else
      %this.setDebugOff(1);
}

function Scene::setShowAABB(%this, %value)
{
   if (%value)
      %this.setDebugOn(2);
   else
      %this.setDebugOff(2);
}

function Scene::setShowOOBB(%this, %value)
{
   if (%value)
      %this.setDebugOn(3);
   else
      %this.setDebugOff(3);
}

function Scene::setShowAsleep(%this, %value)
{
   if (%value)
      %this.setDebugOn(4);
   else
      %this.setDebugOff(4);
}

function Scene::setShowCollisionShapes(%this, %value)
{
   if (%value)
      %this.setDebugOn(5);
   else
      %this.setDebugOff(5);
}

function Scene::setShowPositionAndCOM(%this, %value)
{
   if (%value)
      %this.setDebugOn(6);
   else
      %this.setDebugOff(6);
}

function Scene::setShowSortPoints(%this, %value)
{
   if (%value)
      %this.setDebugOn(7);
   else
      %this.setDebugOff(7);
}

function Scene::getShowMetrics(%this)
{
   return %this.getDebugOn(0);
}

function Scene::getShowJoints(%this)
{
   return %this.getDebugOn(1);
}

function Scene::getShowAABB(%this)
{
   return %this.getDebugOn(2);
}

function Scene::getShowOOBB(%this)
{
   return %this.getDebugOn(3);
}

function Scene::getShowAsleep(%this)
{
   return %this.getDebugOn(4);
}

function Scene::getShowCollisionShapes(%this)
{
   return %this.getDebugOn(5);
}

function Scene::getShowPositionAndCOM(%this)
{
   return %this.getDebugOn(6);
}

function Scene::getShowSortPoints(%this)
{
   return %this.getDebugOn(7);
}
