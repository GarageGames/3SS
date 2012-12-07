//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, setPositionX, void, 3, 3, "(xpos) Set the x component of the position of the object\n"
              "@return No return value.")
{
   object->setPosition(Vector2(dAtof(argv[2]), object->getPosition().y));
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, setPositionY, void, 3, 3, "(ypos) Set the y component of the position of the object\n"
              "@return No return value.")
{
   object->setPosition(Vector2(object->getPosition().x, dAtof(argv[2])));
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, setWidth, void, 3, 3, "(width) Set the width of the object\n"
              "@return No return value.")
{
   object->setSize(Vector2(dAtof(argv[2]), object->getSize().y));
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, setHeight, void, 3, 3, "(height) Set the height of the object\n"
              "@return No return value.")
{
   object->setSize(Vector2(object->getSize().x, dAtof(argv[2])));
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, setAngle, void, 3, 3, "(angle) Set the angle of the object\n"
              "@param The angle in degrees"
              "@return No return value.")
{
   object->setAngle(mDegToRad(dAtof(argv[2])));
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, setFlipX, void, 3, 3, "(flipX) Set the flipX flag\n"
              "@return No return value.")
{
   object->flipX();
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, setFlipY, void, 3, 3, "(flipY) Set the flipY flag\n"
              "@return No return value.")
{
   object->flipY();
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, setSceneGroup, void, 3, 3, "(scenegroup) Sets the scenegroup of the set.\n"
              "@return No return value.")
{
   object->setSceneGroup(dAtoi(argv[2]));
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, setSceneLayer, void, 3, 3, "(layer) Sets the layer of the set.\n"
              "@return No return value.")
{
   object->setSceneLayer(dAtoi(argv[2]));
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, getPosition, const char*, 2, 2, "()\n @return Returns position.")
{
   char* buffer = Con::getReturnBuffer( 32 );
   Vector2 pos = object->getPosition();
   dSprintf( buffer, 32, "%g %g", pos.x, pos.y );
   return buffer;
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, getPositionX, F32, 2, 2, "()\n Returns x position.")
{
   return object->getPosition().x;
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, getPositionY, F32, 2, 2, "()\n @return Returns y position.")
{
   return object->getPosition().y;
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, getSize, const char*, 2, 2, "()\n @return Returns size.")
{
   char* buffer = Con::getReturnBuffer( 32 );
   Vector2 size = object->getSize();
   dSprintf( buffer, 32, "%g %g", size.x, size.y );
   return buffer;
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, getWidth, F32, 2, 2, "()\n @return Returns width.")
{
   return object->getSize().x;
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, getHeight, F32, 2, 2, "()\n @return Returns height.")
{
   return object->getSize().y;
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, getAngle, F32, 2, 2, "()\n @return Returns angle.")
{
   return mRadToDeg( object->getAngle() );
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, getFlipX, bool, 2, 2, "() \n @return Returns flip x.")
{
   return object->getFlipX();
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, getFlipY, bool, 2, 2, "() \n @return Returns flip y.")
{
   return object->getFlipY();
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, getSceneGroup, S32, 2, 2, "() \n @return Returns the scene group.")
{
   return object->getSceneGroup();
}

//------------------------------------------------------------------------------

ConsoleMethod(SceneObjectSet, getSceneLayer, S32, 2, 2, "() \n @return Returns the layer.")
{
   return object->getSceneLayer();
}
