//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
GuiFormManager::AddFormContent( "LevelBuilderQuickEditClasses", "SceneObject", "LBQESceneObject::CreateContent", "LBQESceneObject::SaveContent", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBQESceneObject::CreateContent( %contentCtrl, %quickEditObj )
{
   %base = %contentCtrl.createBaseStack("LBQESceneObjectClass", %quickEditObj);
   
   // Scene Object Rollout.
   %sceneObjectRollout = %base.createRolloutStack("Scene Object", true);
   %sceneObjectRollout.createTextEdit2("PositionX", "PositionY", 3, "Position", "X", "Y", "Position", true);
   %sceneObjectRollout.createTextEdit2("Width", "Height", 3, "Size", "Width", "Height", "Size", true);
   
   %hiddenCheck = %base @ ".object.getClassName() $= \"Path\"";
   %hiddenCheck = %hiddenCheck @ " || " @ %base @ ".object.getClassName() $= \"t2dShape3D\"";
   %hiddenCheck = %hiddenCheck @ " || " @ %base @ ".object.getClassName() $= \"TileLayer\"";
   %hiddenCheck = %hiddenCheck @ ";";
   %hidden = %sceneObjectRollout.createHideableStack( %hiddenCheck );
   %hidden.createTextEdit ("Angle", 3, "Angle", "Angle", true);
   
   %sceneObjectRollout.createTextEdit2("SortPointX", "SortPointY", 3, "Sort Point", "X", "Y", "The layer draw order sorting point.", true);
   %sceneObjectRollout.createTextEdit ("Lifetime", 3, "Lifetime", "Lifetime");
   %sceneObjectRollout.createLeftRightEdit("SceneLayer", "0;", "31;", 1, "Scene Layer", "Scene Rendering Layer");
   %sceneObjectRollout.createLeftRightEdit("SceneGroup", "0;", "31;", 1, "Scene Group", "Scene Group");
   %sceneObjectRollout.createLeftRight("moveBackwardInLayer", "moveForwardInLayer", "Back/Forward", "Move the object within its scene layer.");      
   %sceneObjectRollout.createCheckBox ("Visible", "Visible", "Visibility");   

   %hiddenCheck = %base @ ".object.getClassName() $= \"TileLayer\";";
   %hidden = %sceneObjectRollout.createHideableStack( %hiddenCheck );
   %hidden.createCheckBox ("FixedAngle", "Fixed Angle", "Whether forces can change the angle or not.");

   %hiddenCheck = %base @ ".object.getClassName() $= \"TileLayer\"";
   %hiddenCheck = %hiddenCheck @ " || " @ %base @ ".object.getClassName() $= \"ParticleEffect\"";
   %hiddenCheck = %hiddenCheck @ " || " @ %base @ ".object.getClassName() $= \"Trigger\"";
   %hiddenCheck = %hiddenCheck @ " || " @ %base @ ".object.getClassName() $= \"Path\"";
   %hiddenCheck = %hiddenCheck @ " || " @ %base @ ".object.getClassName() $= \"t2dShape3D\"";
   %hiddenCheck = %hiddenCheck @ ";";
   %hidden = %sceneObjectRollout.createHideableStack( %hiddenCheck );
   %hidden.createCheckBox ("FlipX", "Flip Horizontal", "Flip Horizontal", true);
   %hidden.createCheckBox ("FlipY", "Flip Vertical", "Flip Vertical", true);  
      
   // Physics Rollout.
   %physicsRollout = %base.createRolloutStack("Physics", true);   
   %physicsRollout.createEnumList("BodyType", true, "Body Type", "Body Type", "SceneObject", "BodyType");
   %physicsRollout.createTextEdit2("LinearVelocity", "linearVelocity", 3, "Linear Velocity", "X", "Y", "Linear Velocity");
   %physicsRollout.createTextEdit ("AngularVelocity", 3, "Angular Velocity", "Angular Velocity");
   %physicsRollout.createTextEdit ("LinearDamping", 3, "Linear Damping", "Reduces the linear velocity over time.");
   %physicsRollout.createTextEdit ("AngularDamping", 3, "Angular Damping", "Reduces the angular velocity over time.");
   %physicsRollout.createTextEdit ("GravityScale", 3, "Gravity Scale", "Scales how much the scene-wide gravity affects this object.");
   %physicsRollout.createTextEdit ("DefaultDensity", 3, "Default Density", "The default density of all collision shapes.");
   %physicsRollout.createTextEdit ("DefaultFriction", 3, "Default Friction", "The default contact friction of all collision shapes.");
   %physicsRollout.createTextEdit ("DefaultRestitution", 3, "Default Restitution", "The default contact restitution of all collision shapes.");
   %physicsRollout.createMask("CollisionLayers", "Collision Layers", 0, 31, "Change the Layers of Objects With Which This Can Collide.");
   %physicsRollout.createMask("CollisionGroups", "Collision Groups", 0, 31, "Change the Groups of Objects With Which This Can Collide.");
   %physicsRollout.createCheckBox("Awake", "Awake", "Whether the object is initially awake or not.");
   %physicsRollout.createCheckBox("Bullet", "Bullet", "Whether to use Continuous Collision Detection or Not.");
   %physicsRollout.createCheckBox("SleepingAllowed", "Sleeping Allowed", "Whether the Sleeping is Allowed if the object is not disturbed.");
   %physicsRollout.createCheckBox("CollisionSuppress", "Collision Suppress", "Whether to Suppress All Collisions or Not.");
        
   // Pathing Rollout.
   %pathNodeCountCommand = %base @ ".object.getAttachedToPath().getNodeCount() - 1;";
   %pathedStack = %base.createHideableStack("!" @ %base @ ".object.getAttachedToPath();");  
   %pathedRollout = %pathedStack.createRolloutStack("Pathing");
   %pathedRollout.createLeftRightEdit("PathStartNode", "0;", %pathNodeCountCommand, 1, "Start Node", "The Node the Object Starts At.");
   %pathedRollout.createLeftRightEdit("PathEndNode", "0;", %pathNodeCountCommand, 1, "End Node", "The Node the Object Ends At.");
   %pathedRollout.createTextEdit("PathSpeed", 3, "Speed", "The Speed at Which to Follow the Path.");
   %pathedRollout.createCheckBox("PathMoveForward", "Move Forward", "Uncheck to Change the Direction of the object.");
   %pathedRollout.createCheckBox("PathOrient", "Orient To Path", "Rotate the Object With the Direction it is Facing.");
   %pathedRollout.createTextEdit("PathRotationOffset", 3, "Rotation Offset", "The offset of the rotation when using orient to path.");
   %pathedRollout.createTextEdit("PathLoops", 0, "Loops", "The number of Loops Along the Path to Take.");
   %pathedRollout.createEnumList("PathFollowMode", false, "Follow Mode", "The Response of the Object Upon Reaching the End of the Path.", "Path", "pathModeEnum");  
   
   // Rendering Effects Rollout.
   %blendingRollout = %base.createRolloutStack("Rendering Effects");
   %blendingEnabledCheck = %blendingRollout.createCheckBox("BlendingStatus", "Enabled", "Enable Blending");
   
   %blendingContainer = %blendingRollout.createHideableStack("!" @ %base @ ".object.getBlendMode();");
   %blendingContainer.addControlDependency(%blendingEnabledCheck);
   %blendingContainer.createEnumList("SrcBlendFactor", false, "Source Factor", "Source Blend Factor", "SceneObject", "srcBlendFactor");
   %blendingContainer.createEnumList("DstBlendFactor", false, "Destination Factor", "Destination Blend Factor", "SceneObject", "dstBlendFactor");
   %blendingContainer.createColorPicker("BlendColor", "Blend Color", "Blend Color");
   %blendingRollout.createTextEdit("alphaTest", 0, "Alpha Test", "Enables alpha testing for this object iF value is between 0 - 255. -1 to disable");
   
   // Align Rollout.
   %alignRollout = %base.createRolloutStack( "Align", false );
   %alignRollout.createAlignTools( true );
   
   // Scripting Rollout.
   %scriptingRollout = %base.createRolloutStack("Scripting");   
   %hiddenCheck = %base @ ".object.getClassName() $= \"TileLayer\"";
   %hiddenCheck = %hiddenCheck @ ";";
   %hidden = %scriptingRollout.createHideableStack( %hiddenCheck );
   %hidden.createCheckBox("Persistent", "Persist", "Set This Object to Persist Across All Levels.");
   %scriptingRollout.createT2DDatablockList("ConfigDatablock", "Config Datablock", "t2dSceneObjectdatablock", "The Configuration Datablock to associate this sceneobject with");   
   %scriptingRollout.createTextEdit("Name", "TEXT", "Name", "Name the Object for Referencing in Script");
   %scriptingRollout.createTextEdit("ClassNamespace", "TEXT", "Class", "Link this Object to a Class");
   %scriptingRollout.createTextEdit("SuperClassNamespace", "TEXT", "Super Class", "Link this Object to a Parent Class");

   // Callbacks Rollout.
   %callbacksRollout = %base.createRolloutStack("Callbacks");   
   %callbacksRollout.createCheckBox("UseInputEvents", "Use Input Events", "Enable Input event callbacks on the object.");   
   %callbacksRollout.createCheckBox("CollisionCallback", "Collision Callback", "Whether to Receive Script Notifications of Collision Events or Not.");

   // Dynamic Fields Rollout.   
   %dynamicFieldRollout = %base.createRolloutStack("Dynamic Fields");
   %dynamicFieldRollout.createDynamicFieldStack();
   
   // Return Ref to Base.
   return %base;
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBQESceneObject::SaveContent( %contentCtrl )
{
   // Nothing.
}

function SceneObject::setPathStartNode(%this, %value)
{
   if (%this.getAttachedToPath())
      %this.getAttachedToPath().setStartNode(%this, %value);
}

function SceneObject::getPathStartNode(%this)
{
   if (%this.getAttachedToPath())
      return %this.getAttachedToPath().getStartNode(%this);
   else
      return 0;
}

function SceneObject::setPathEndNode(%this, %value)
{
   if (%this.getAttachedToPath())
      %this.getAttachedToPath().setEndNode(%this, %value);
}

function SceneObject::getPathEndNode(%this)
{
   if (%this.getAttachedToPath())
      return %this.getAttachedToPath().getEndNode(%this);
   else
      return 0;
}

function SceneObject::setPathSpeed(%this, %value)
{
   if (%this.getAttachedToPath())
      %this.getAttachedToPath().setSpeed(%this, %value);
}

function SceneObject::getPathSpeed(%this)
{
   if (%this.getAttachedToPath())
      return %this.getAttachedToPath().getSpeed(%this);
   else
      return 0;
}

function SceneObject::setPathMoveForward(%this, %value)
{
   if (%this.getAttachedToPath())
      %this.getAttachedToPath().setMoveForward(%this, %value);
}

function SceneObject::getPathMoveForward(%this)
{
   if (%this.getAttachedToPath())
      return %this.getAttachedToPath().getMoveForward(%this);
   else
      return 0;
}

function SceneObject::setPathOrient(%this, %value)
{
   if (%this.getAttachedToPath())
      %this.getAttachedToPath().setOrient(%this, %value);
}

function SceneObject::getPathOrient(%this)
{
   if (%this.getAttachedToPath())
      return %this.getAttachedToPath().getOrient(%this);
   else
      return 0;
}

function SceneObject::setPathRotationOffset(%this, %value)
{
   if (%this.getAttachedToPath())
      %this.getAttachedToPath().setAngleOffset(%this, %value);
}

function SceneObject::getPathRotationOffset(%this)
{
   if (%this.getAttachedToPath())
      return %this.getAttachedToPath().getAngleOffset(%this);
   else
      return 0;
}

function SceneObject::setPathLoops(%this, %value)
{
   if (%this.getAttachedToPath())
      %this.getAttachedToPath().setLoops(%this, %value);
}

function SceneObject::getPathLoops(%this)
{
   if (%this.getAttachedToPath())
      return %this.getAttachedToPath().getLoops(%this);
   else
      return 0;
}

function SceneObject::setPathFollowMode(%this, %value)
{
   if (%this.getAttachedToPath())
      %this.getAttachedToPath().setFollowMode(%this, %value);
}

function SceneObject::getPathFollowMode(%this)
{
   if (%this.getAttachedToPath())
      return %this.getAttachedToPath().getFollowMode(%this);
   else
      return 0;
}

function SceneObject::moveForwardInLayer(%this)
{
    %this.setSceneLayerDepthForward();
}

function SceneObject::moveBackwardInLayer(%this)
{
    %this.setSceneLayerDepthBackward();
}
