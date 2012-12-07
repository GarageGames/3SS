//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod(Path, attachObject, void, 4, 10, "(obj, speed, [dir], [start], [end], [pathMode], [loops], [sendToStart]) Attach an object to the path\n"
              "@param obj The object to attach.\n"
              "@param speed The movement speed along path.\n"
              "@param [dir] The direction of movement, either positive or negative (default positive).\n"
              "@param [start] The start point (default 0)\n"
              "@param [end] The end point (default 0)\n"
              "@param [pathMode] The path mode (default PATH_WRAP)\n"
              "@param [loops] Number of loops to make around path (default 0)\n"
              "@param [sendToStart] Start position at startnode (default false)\n"
              "@return No return value."
              )
{
   SceneObject* obj = dynamic_cast<SceneObject*>(Sim::findObject(argv[2]));
   if (!obj)
   {
      Con::warnf("Invalid SceneObject passed to Path::attachObject.");
      return;
   }
   F32 speed = dAtof(argv[3]);

   S32 dir = 1;
   if (argc > 4)
   {
      dir = (dAtoi(argv[4]) > 0) ? 1 : -1;
   }

   S32 start = 0;
   if (argc > 5)
      start = dAtoi(argv[5]);
   
   S32 end = 0;
   if (argc > 6)
      end = dAtoi(argv[6]);

   ePathMode path = PATH_WRAP;
   if (argc > 7)
      path = getPathMode(argv[7]);

   S32 loops = 0;
   if (argc > 8)
      loops = dAtoi(argv[8]);

   bool send = false;
   if (argc > 9)
      send = dAtob(argv[9]);

   object->attachObject(obj, speed, dir, true, start, end, path, loops, send);
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, detachObject, void, 3, 3, "(object) Detach object from path\n"
              "@param object The object to detach.\n"
              "@return No return value.")
{
   SceneObject* obj = dynamic_cast<SceneObject*>(Sim::findObject(argv[2]));
   if (obj)
      object->detachObject(obj);
   else
      Con::warnf("Invalid object passed to Path::detachObject.");
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, setPathType, void, 3, 3, "(type) Sets the interpolation type for the path.\n"
              "@param type Interpolation type (either LINEAR, BEZIER, CATMULL, or CUSTOM)\n"
              "@return No return value.")
{
   object->setPathType(getFollowMethod(argv[2]));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, getPathType, const char*, 2, 2, "() Gets the interpolation type.\n"
              "@return Returns the interpolation type as a string.")
{
   return getFollowMethodDescription(object->getPathType());
}

//---------------------------------------------------------------------------------------------

ConsoleMethod( Path, getAttachedObjectCount, S32, 2, 2, "() \n @return Returns the number of objects attached to the path.")
{
   return object->getPathedObjectCount();
}

//---------------------------------------------------------------------------------------------

ConsoleMethod( Path, getAttachedObject, S32, 3, 3, "(S32 index) @return Returns the object at the specified index." )
{
   SceneObject* obj = object->getPathedObject( dAtoi( argv[2] ) );
   if( !obj )
      return 0;

   return obj->getId();
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, addNode, S32, 3, 6, "(position, [location], [rotation], [weight]) Add node to path\n"
              "@param position Spatial position of node.\n"
              "@param location Where in path array it should be placed (default -1 ie beginning)\n"
              "@param rotation The rotation of the node.\n"
              "@param weight The weight of the node\n"
              "@return Returns the node count\n")
{
   Vector2 position = Utility::mGetStringElementVector(argv[2]);

   S32 location = -1;
   if (argc > 3)
      location = dAtoi(argv[3]);

   F32 weight = 10.0f;
   if (argc > 4)
      weight = dAtof(argv[4]);

   F32 rotation = 0.0f;
   if (argc > 5)
      rotation = mDegToRad(dAtof(argv[5]));

   return object->addNode(position, rotation, weight, location);
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, removeNode, S32, 3, 3, "(index) Removes the node at given index\n"
              "@return Returns node count.")
{
   return object->removeNode(dAtoi(argv[2]));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, clear, void, 2, 2, "() Clears all nodes in Path\n"
              "@return No return value.")
{
   object->clear();
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, setStartNode, void, 4, 4, "(object, node) Sets the start node on path for given object.\n"
              "@param object Object to modify path settings\n"
              "@param node Node to set as start\n"
              "@return No return value.")
{
   object->setStartNode(dynamic_cast<SceneObject*>(Sim::findObject(argv[2])), dAtoi(argv[3]));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, setEndNode, void, 4, 4, "(object, node) Sets the end node on path for given object.\n"
              "@param object Object to modify path settings\n"
              "@param node Node to set as end\n"
              "@return No return value.")
{
   object->setEndNode(dynamic_cast<SceneObject*>(Sim::findObject(argv[2])), dAtoi(argv[3]));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, setSpeed, void, 4, 5, "(object, speed, resetObject)  Sets the speed on path for given object.n"
              "@param object Object to modify path settingsn"
              "@param speed Desired speedn"
              "@param resetObject Reset object to path start (default false)n"
              "@return No return value.")
{
   object->setSpeed(dynamic_cast<SceneObject*>(Sim::findObject(argv[2])), dAtof(argv[3]),dAtob(argv[4]));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, setMoveForward, void, 4, 4, "(object, forward)  Sets the direction to forward (or not) on path for given object.\n"
              "@param object Object to modify path settings\n"
              "@param forward Bool. Whether to set direction as forward\n"
              "@param resetObject Reset object to path start (default false)n"
              "@return No return value.")
{
   object->setMoveForward(dynamic_cast<SceneObject*>(Sim::findObject(argv[2])), dAtob(argv[3]),dAtob(argv[4]));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, setOrient, void, 4, 4, "(object, orient)  Sets the orientation for given object.\n"
              "@param object Object to modify path settings\n"
              "@param orient Object orientation\n"
              "@param resetObject Reset object to path start (default false)n"
              "@return No return value.")
{
   object->setOrient(dynamic_cast<SceneObject*>(Sim::findObject(argv[2])), dAtob(argv[3]),dAtob(argv[4]));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, setAngleOffset, void, 4, 4, "(object, offset) Sets the offset on path for given object.\n"
              "@param object Object to modify path settings\n"
              "@param offset Desired offset\n"
              "@param resetObject Reset object to path start (default false)n"
              "@return No return value.")
{
   object->setAngleOffset(dynamic_cast<SceneObject*>(Sim::findObject(argv[2])), mDegToRad(dAtof(argv[3])), dAtob(argv[4]));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, setLoops, void, 4, 4, "(object, loops) Sets the loop number on path for given object.\n"
              "@param object Object to modify path settings\n"
              "@param loops Desired number of loops\n"
              "@param resetObject Reset object to path start (default false)n"
              "@return No return value.")
{
   object->setLoops(dynamic_cast<SceneObject*>(Sim::findObject(argv[2])), dAtoi(argv[3]),dAtob(argv[4]));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, setFollowMode, void, 4, 4, "(object, pathMode) Sets the follow mode on path for given object.\n"
              "@param object Object to modify path settings\n"
              "@param pathMode Desired follow style for object\n"
              "@param resetObject Reset object to path start (default false)n"
              "@return No return value.")
{
   object->setFollowMode(dynamic_cast<SceneObject*>(Sim::findObject(argv[2])), getPathMode(argv[3]),dAtob(argv[4]));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, getStartNode, S32, 3, 3, "(object) Gets the start node on path for given object.\n"
              "@param object Object to get path setting\n"
              "@return Returns the node index of the start node.")
{
   return object->getStartNode(dynamic_cast<SceneObject*>(Sim::findObject(argv[2])));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, getEndNode, S32, 3, 3, "(object) Gets the end node on path for given object.\n"
              "@param object Object to get path setting\n"
              "@return Returns the node index of the end node.")
{
   return object->getEndNode(dynamic_cast<SceneObject*>(Sim::findObject(argv[2])));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, getSpeed, F32, 3, 3, "(object) Gets the speed on path for given object.\n"
              "@param object Object to get path setting\n"
              "@return Returns the speed of the object.")
{
   return object->getSpeed(dynamic_cast<SceneObject*>(Sim::findObject(argv[2])));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, getMoveForward, bool, 3, 3, "(object) Gets the direction on path for given object.\n"
              "@param object Object to get path setting\n"
              "@return Returns the direction of the object (1 is forward, -1 backward).")
{
   return object->getMoveForward(dynamic_cast<SceneObject*>(Sim::findObject(argv[2])));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, getOrient, bool, 3, 3, "(object) Gets the orientation on path for given object.\n"
              "@param object Object to get path setting\n"
              "@return Returns the orientation of the object.")
{
   return object->getOrient(dynamic_cast<SceneObject*>(Sim::findObject(argv[2])));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, getAngleOffset, F32, 3, 3, "(object) Gets the rotation offset of object on path\n"
              "@param object Object to get path setting\n"
              "@return Returns the offset of the object.")
{
   return mRadToDeg(object->getAngleOffset(dynamic_cast<SceneObject*>(Sim::findObject(argv[2]))));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, getLoops, S32, 3, 3, "(object) Gets the number of loops of the object on path\n"
              "@param object Object to get path setting\n"
              "@return Returns the number of loops of the object.")
{
   return object->getLoops(dynamic_cast<SceneObject*>(Sim::findObject(argv[2])));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, getFollowMode, const char*, 3, 3, "(object) Gets the follow mode of the object on path\n"
              "@param object Object to get path setting\n"
              "@return Returns the follow mode of the object.")
{
   return getPathModeDescription(object->getFollowMode(dynamic_cast<SceneObject*>(Sim::findObject(argv[2]))));
}

//---------------------------------------------------------------------------------------------

ConsoleMethod(Path, getNodeCount, S32, 2, 2, "() \n @return Returns the number of nodes in path")
{
   return object->getNodeCount();
}

//---------------------------------------------------------------------------------------------

ConsoleMethod( Path, getNode, const char*, 3, 3, "(index) Gets the nodes position, rotation, and weight.\n"
              "@param index The index of the node to get information for.\n"
              "@return Returns the position, rotation, and weight formatted as \"pos rot weight\"")
{
   Path::PathNode& node = object->getNode( dAtoi( argv[2] ) );
   char* buffer = Con::getReturnBuffer( 64 );
   dSprintf( buffer, 64, "%g %g %g %g", node.position.x, node.position.y, mRadToDeg(node.rotation), node.weight );
   return buffer;
}
