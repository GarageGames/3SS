//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
GuiFormManager::AddFormContent( "LevelBuilderQuickEditClasses", "t2dShape3D", "LBQEShape3D::CreateContent", "LBQEShape3D::SaveContent", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBQEShape3D::CreateContent( %contentCtrl, %quickEditObj )
{
   %base = %contentCtrl.createBaseStack("LBQEShape3DClass", %quickEditObj);
   %rollout = %base.createRolloutStack("3D Shape", true);  
   %rollout.createTextEdit3("shapeAngularVelocityX", "shapeAngularVelocityY", "shapeAngularVelocityZ", 3, "Angular Velocity", "X", "Y", "Z", "Angular Velocity of the Shape");
   %rollout.createTextEdit3("shapeRotationX", "shapeRotationY", "shapeRotationZ", 3, "Rotation", "X", "Y", "Z", "Rotation of the Shape");
   %rollout.createTextEdit3("shapeOffsetX", "shapeOffsetY", "shapeOffsetZ", 3, "Offset", "X", "Y", "Z", "Offset of the Shape");
   %rollout.createTextEdit3("shapeScaleX", "shapeScaleY", "shapeScaleZ", 3, "Scale", "X", "Y", "Z", "Scale of the Shape");
   
   // Return Ref to Base.
   return %base;
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBQEShape3D::SaveContent( %contentCtrl )
{
   // Nothing.
}

function t2dShape3D::setShapeAngularVelocityX(%this, %val)
{
   %vel = %this.getShapeAngularVelocity();
   %x = %val;
   %y = mRadToDeg(getWord(%vel, 2));
   %z = mRadToDeg(getWord(%vel, 1));
   
   %this.setShapeAngularVelocity(%x, %y, %z);
}

function t2dShape3D::setShapeAngularVelocityY(%this, %val)
{
   %vel = %this.getShapeAngularVelocity();
   %x = mRadToDeg(getWord(%vel, 0));
   %y = %val;
   %z = mRadToDeg(getWord(%vel, 1));
   
   %this.setShapeAngularVelocity(%x, %y, %z);
}

function t2dShape3D::setShapeAngularVelocityZ(%this, %val)
{
   %vel = %this.getShapeAngularVelocity();
   %x = mRadToDeg(getWord(%vel, 0));
   %y = mRadToDeg(getWord(%vel, 2));
   %z = %val;
   
   %this.setShapeAngularVelocity(%x, %y, %z);
}

function t2dShape3D::getShapeAngularVelocityX(%this)
{
   return mRadToDeg(getWord(%this.getShapeAngularVelocity(), 0));
}

function t2dShape3D::getShapeAngularVelocityY(%this)
{
   return mRadToDeg(getWord(%this.getShapeAngularVelocity(), 2));
}

function t2dShape3D::getShapeAngularVelocityZ(%this)
{
   return mRadToDeg(getWord(%this.getShapeAngularVelocity(), 1));
}

function t2dShape3D::setShapeRotationX(%this, %val)
{
   %vel = %this.getShapeRotation();
   %x = %val;
   %y = mRadToDeg(getWord(%vel, 2));
   %z = mRadToDeg(getWord(%vel, 1));
   
   %this.setShapeRotation(%x, %y, %z);
}

function t2dShape3D::setShapeRotationY(%this, %val)
{
   %vel = %this.getShapeRotation();
   %x = mRadToDeg(getWord(%vel, 0));
   %y = %val;
   %z = mRadToDeg(getWord(%vel, 1));
   
   %this.setShapeRotation(%x, %y, %z);
}

function t2dShape3D::setShapeRotationZ(%this, %val)
{
   %vel = %this.getShapeRotation();
   %x = mRadToDeg(getWord(%vel, 0));
   %y = mRadToDeg(getWord(%vel, 2));
   %z = %val;
   
   %this.setShapeRotation(%x, %y, %z);
}

function t2dShape3D::getShapeRotationX(%this)
{
   return mRadToDeg(getWord(%this.getShapeRotation(), 0));
}

function t2dShape3D::getShapeRotationY(%this)
{
   return mRadToDeg(getWord(%this.getShapeRotation(), 2));
}

function t2dShape3D::getShapeRotationZ(%this)
{
   return mRadToDeg(getWord(%this.getShapeRotation(), 1));
}

function t2dShape3D::setShapeOffsetX(%this, %val)
{
   %vel = %this.getShapeOffset();
   %x = %val;
   %y = getWord(%vel, 2);
   %z = getWord(%vel, 1);
   
   %this.setShapeOffset(%x, %y, %z);
}

function t2dShape3D::setShapeOffsetY(%this, %val)
{
   %vel = %this.getShapeOffset();
   %x = getWord(%vel, 0);
   %y = %val;
   %z = getWord(%vel, 1);
   
   %this.setShapeOffset(%x, %y, %z);
}

function t2dShape3D::setShapeOffsetZ(%this, %val)
{
   %vel = %this.getShapeOffset();
   %x = getWord(%vel, 0);
   %y = getWord(%vel, 2);
   %z = %val;
   
   %this.setShapeOffset(%x, %y, %z);
}

function t2dShape3D::getShapeOffsetX(%this)
{
   return getWord(%this.getShapeOffset(), 0);
}

function t2dShape3D::getShapeOffsetY(%this)
{
   return getWord(%this.getShapeOffset(), 2);
}

function t2dShape3D::getShapeOffsetZ(%this)
{
   return getWord(%this.getShapeOffset(), 1);
}

function t2dShape3D::setShapeScaleX(%this, %val)
{
   %vel = %this.getShapeScale();
   %x = %val;
   %y = getWord(%vel, 1);
   %z = getWord(%vel, 2);
   
   %this.setShapeScale(%x, %y, %z);
}

function t2dShape3D::setShapeScaleY(%this, %val)
{
   %vel = %this.getShapeScale();
   %x = getWord(%vel, 0);
   %y = %val;
   %z = getWord(%vel, 2);
   
   %this.setShapeScale(%x, %y, %z);
}

function t2dShape3D::setShapeScaleZ(%this, %val)
{
   %vel = %this.getShapeScale();
   %x = getWord(%vel, 0);
   %y = getWord(%vel, 1);
   %z = %val;
   
   %this.setShapeScale(%x, %y, %z);
}

function t2dShape3D::getShapeScaleX(%this)
{
   return getWord(%this.getShapeScale(), 0);
}

function t2dShape3D::getShapeScaleY(%this)
{
   return getWord(%this.getShapeScale(), 1);
}

function t2dShape3D::getShapeScaleZ(%this)
{
   return getWord(%this.getShapeScale(), 2);
}

function t2dShape3D::get()
{
   %taheu = 5;
}
