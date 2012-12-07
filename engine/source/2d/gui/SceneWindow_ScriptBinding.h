//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

extern SceneWindow::CameraInterpolationMode getInterpolationMode(const char* label);

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getWindowExtents, const char*, 2, 2, "() Fetch Window Extents (Position/Size)."
              "@return Returns the window dimensions as a string formatted as follows: <position.x> <position.y> <width> <height>")
{
    // Get Size Argument Buffer.
    char* pExtentsBuffer = Con::getReturnBuffer(64);
    // Format Buffer.
    dSprintf( pExtentsBuffer, 64, "%d %d %d %d", object->getPosition().x, object->getPosition().y, object->getExtent().x, object->getExtent().y );
    // Return Buffer.
    return pExtentsBuffer;
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getScene, const char*, 2, 2, "() - Returns the Scene associated with this window."
              "@return Returns the scene ID as a string")
{
   Scene* pScene = object->getScene();

   char* id = Con::getReturnBuffer(8);
   if (pScene)
   {
        dItoa(pScene->getId(), id );
   }
   else
   {
      id[0] = '\0';
   }

   return id;
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setScene, void, 2, 3, "(Scene) Associates Scene Object."
              "@param Scene The scene ID or name.\n"
              "@return No return value.")
{
    // No scene specified?
    if ( argc < 3 )
    {
        // No, so reset the scene.
        object->resetScene();
        // Finish here.
        return;
    }

    // Find Scene Object.
    Scene* pScene = (Scene*)(Sim::findObject(argv[2]));

    // Validate Object.
    if ( !pScene )
    {
        Con::warnf("SceneWindow::setScene() - Couldn't find object '%s'.", argv[2]);
        return;
    }

    // Set Scene.
    object->setScene( pScene );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, resetScene, void, 2, 2, "() Detaches the window from any Scene Object.\n"
              "@return No return value")
{
    // Reset Scene.
    object->resetScene();
}   

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setCurrentCameraArea, void, 3, 6, "(x1 / y1 / x2 / y2) - Set current camera area."
              "@param x1,y1,x2,y2 The coordinates of the minimum and maximum points (top left, bottom right)\n"
              "The input can be formatted as either \"x1 y1 x2 y2\", \"x1 y1, x2 y2\", \"x1, y1, x2, y2\"\n"
              "@return No return value.")
{
   // Upper left bound.
   Vector2 v1;
   // Lower right bound.
   Vector2 v2;

   // Grab the number of elements in the first two parameters.
   U32 elementCount1 =Utility::mGetStringElementCount(argv[2]);
   U32 elementCount2 = 1;
   if (argc > 3)
      elementCount2 =Utility::mGetStringElementCount(argv[3]);

   // ("x1 y1 x2 y2")
   if ((elementCount1 == 4) && (argc == 3))
   {
       v1 = Utility::mGetStringElementVector(argv[2]);
       v2 = Utility::mGetStringElementVector(argv[2], 2);
   }
   
   // ("x1 y1", "x2 y2")
   else if ((elementCount1 == 2) && (elementCount2 == 2) && (argc == 4))
   {
      v1 = Utility::mGetStringElementVector(argv[2]);
      v2 = Utility::mGetStringElementVector(argv[3]);
   }
   
   // (x1, y1, x2, y2)
   else if (argc == 6)
   {
       v1 = Vector2(dAtof(argv[2]), dAtof(argv[3]));
       v2 = Vector2(dAtof(argv[4]), dAtof(argv[5]));
   }
   
   // Invalid
   else
   {
      Con::warnf("SceneWindow::setCurrentCameraArea() - Invalid number of parameters!");
      return;
   }

    // Calculate Normalised Rectangle.
    Vector2 topLeft( (v1.x <= v2.x) ? v1.x : v2.x, (v1.y <= v2.y) ? v1.y : v2.y );
    Vector2 bottomRight( (v1.x > v2.x) ? v1.x : v2.x, (v1.y > v2.y) ? v1.y : v2.y );

    // Set Current Camera Area.
    object->setCurrentCameraArea( RectF(topLeft.x, topLeft.y, bottomRight.x-topLeft.x, bottomRight.y-topLeft.y) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getCurrentCameraArea, const char*, 2, 2, "() Get current camera Area.\n"
              "@return The camera area formatted as \"x1 y1 x2 y2\"")
{
    // Fetch Camera Window.
    const RectF cameraWindow = object->getCurrentCameraArea();

    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer(64);
    // Format Buffer.
    dSprintf(pBuffer, 64, "%g %g %g %g", cameraWindow.point.x, cameraWindow.point.y, cameraWindow.point.x+cameraWindow.extent.x, cameraWindow.point.y+cameraWindow.extent.y);
    // Return Buffer.
    return pBuffer;
}   

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getCurrentCameraSize, const char*, 2, 2, "() Get current camera Size.\n"
              "@return Returns the cameras width and height as a string formatted as \"width height\"")
{
    // Fetch Camera Window.
    const RectF cameraWindow = object->getCurrentCameraArea();

    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer(64);
    // Format Buffer.
    dSprintf(pBuffer, 64, "%g %g", cameraWindow.extent.x, cameraWindow.extent.y);
    // Return Buffer.
    return pBuffer;
}   

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setCurrentCameraPosition, void, 3, 6, "(x / y / [width / height]) - Set current camera position.\n"
              "@param There are 5 possible formats for input: (x1, y1), (\"x1 y1\"), (x, y, width, height) (\"x1 y1\", \"width height\") (x1, y1, x2, y2)"
              "@return No return value.")
{
   // Position.
   Vector2 position;
   // Dimensions.
   F32 width = object->getCurrentCameraWidth();
   F32 height = object->getCurrentCameraHeight();

   // Grab the number of elements in the first two parameters.
   U32 elementCount1 =Utility::mGetStringElementCount(argv[2]);
   U32 elementCount2 = 1;
   if (argc > 3)
      elementCount2 =Utility::mGetStringElementCount(argv[3]);

   // ("x1 y1 width height")
   if ((elementCount1 == 4) && (argc == 3))
   {
       position = Utility::mGetStringElementVector(argv[2]);
       width = dAtof(Utility::mGetStringElement(argv[2], 2));
       height = dAtof(Utility::mGetStringElement(argv[2], 3));
   }
   
   // ("x1 y1", "width height")
   else if ((elementCount1 == 2) && (elementCount2 == 2) && (argc == 4))
   {
       position = Utility::mGetStringElementVector(argv[2]);
       width = dAtof(Utility::mGetStringElement(argv[3], 0));
       height = dAtof(Utility::mGetStringElement(argv[3], 1));
   }

   // ("x1 y1")
   else if ((elementCount1 == 2) && (argc == 3))
   {
       position = Utility::mGetStringElementVector(argv[2]);
   }
   
   // (x1, y1, x2, y2)
   else if (argc == 6)
   {
       position = Vector2(dAtof(argv[2]), dAtof(argv[3]));
       width = dAtof(argv[4]);
       height = dAtof(argv[5]);
   }

   // (x1, y1)
   else if (argc == 4)
   {
       position = Vector2(dAtof(argv[2]), dAtof(argv[3]));
   }
   
   // Invalid
   else
   {
      Con::warnf("SceneObject::setArea() - Invalid number of parameters!");
      return;
   }

    // Set Current Camera Position.
    object->setCurrentCameraPosition( position, width, height );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getCurrentCameraPosition, const char*, 2, 2, "() Get current camera position.\n"
                                                                        "@return The current camera position.")
{
    return object->getCurrentCameraPosition().scriptThis();
}   

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getCurrentCameraRenderPosition, const char*, 2, 2,   "() Get current camera position post-view-limit clamping.\n"
                                                                                "@return The current camera render position.")
{
    return object->getCurrentCameraRenderPosition().scriptThis();
}   

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setCurrentCameraZoom, void, 3, 3, "(zoomFactor) - Set current camera Zoom Factor.\n"
              "@param zoomFactor A float value representing the zoom factor\n"
              "@return No return value.")
{
    // Set Current Camera Zoom.
    object->setCurrentCameraZoom( dAtof(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getCurrentCameraZoom, F32, 2, 2, "() Get current camera Zoom.\n"
              "@return Returns the camera zoom factor as a 32-bit floating point value.")
{
    // Get Current Camera Zoom.
    return object->getCurrentCameraZoom();
} 

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getCurrentCameraWorldScale, const char*, 2, 2, "() Get current camera scale to world.\n"
              "@return Returns the cameras window width/height scale to world as a string formatted as \"widthScale heightScale\"")
{
    // Fetch camera window
    const Vector2 cameraWindowScale = object->getCurrentCameraWindowScale();

    return cameraWindowScale.scriptThis();
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getCurrentCameraRenderScale, const char*, 2, 2, "() Get current camera scale to render.\n"
              "@return Returns the cameras window width/height scale to render as a string formatted as \"widthScale heightScale\"")
{
    // Fetch camera window scale.
    Vector2 cameraWindowScale = object->getCurrentCameraWindowScale();

    // Inverse scale.
    cameraWindowScale.receiprocate();

    return cameraWindowScale.scriptThis();
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setTargetCameraArea, void, 3, 6, "(x / y / width / height) - Set target camera area."
              "@return No return value.")
{
   // Upper left bound.
   Vector2 v1;
   // Lower right bound.
   Vector2 v2;

   // Grab the number of elements in the first two parameters.
   U32 elementCount1 =Utility::mGetStringElementCount(argv[2]);
   U32 elementCount2 = 1;
   if (argc > 3)
      elementCount2 =Utility::mGetStringElementCount(argv[3]);

   // ("x1 y1 x2 y2")
   if ((elementCount1 == 4) && (argc == 3))
   {
       v1 = Utility::mGetStringElementVector(argv[2]);
       v2 = Utility::mGetStringElementVector(argv[2], 2);
   }
   
   // ("x1 y1", "x2 y2")
   else if ((elementCount1 == 2) && (elementCount2 == 2) && (argc == 4))
   {
      v1 = Utility::mGetStringElementVector(argv[2]);
      v2 = Utility::mGetStringElementVector(argv[3]);
   }
   
   // (x1, y1, x2, y2)
   else if (argc == 6)
   {
       v1 = Vector2(dAtof(argv[2]), dAtof(argv[3]));
       v2 = Vector2(dAtof(argv[4]), dAtof(argv[5]));
   }
   
   // Invalid
   else
   {
      Con::warnf("SceneWindow::setTargetCameraArea() - Invalid number of parameters!");
      return;
   }

    // Calculate Normalised Rectangle.
    Vector2 topLeft( (v1.x <= v2.x) ? v1.x : v2.x, (v1.y <= v2.y) ? v1.y : v2.y );
    Vector2 bottomRight( (v1.x > v2.x) ? v1.x : v2.x, (v1.y > v2.y) ? v1.y : v2.y );

    // Set Target Camera Area.
    object->setTargetCameraArea( RectF(topLeft.x, topLeft.y, bottomRight.x-topLeft.x+1, bottomRight.y-topLeft.y+1) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setTargetCameraPosition, void, 3, 6, "(x / y / [width / height]) - Set target camera position."
              "@return No return value.")
{
   // Position.
   Vector2 position;
   // Dimensions.
   F32 width = object->getCurrentCameraWidth();
   F32 height = object->getCurrentCameraHeight();

   // Grab the number of elements in the first two parameters.
   U32 elementCount1 =Utility::mGetStringElementCount(argv[2]);
   U32 elementCount2 = 1;
   if (argc > 3)
      elementCount2 =Utility::mGetStringElementCount(argv[3]);

   // ("x1 y1 width height")
   if ((elementCount1 == 4) && (argc == 3))
   {
       position = Utility::mGetStringElementVector(argv[2]);
       width = dAtof(Utility::mGetStringElement(argv[2], 2));
       height = dAtof(Utility::mGetStringElement(argv[2], 3));
   }
   
   // ("x1 y1", "width height")
   else if ((elementCount1 == 2) && (elementCount2 == 2) && (argc == 4))
   {
       position = Utility::mGetStringElementVector(argv[2]);
       width = dAtof(Utility::mGetStringElement(argv[3], 0));
       height = dAtof(Utility::mGetStringElement(argv[3], 1));
   }

   // ("x1 y1")
   else if ((elementCount1 == 2) && (argc == 3))
   {
       position = Utility::mGetStringElementVector(argv[2]);
   }
   
   // (x1, y1, width, height)
   else if (argc == 6)
   {
       position = Vector2(dAtof(argv[2]), dAtof(argv[3]));
       width = dAtof(argv[4]);
       height = dAtof(argv[5]);
   }
   
   // (x1, y1)
   else if (argc == 4)
   {
       position = Vector2(dAtof(argv[2]), dAtof(argv[3]));
   }
   
   // Invalid
   else
   {
      Con::warnf("SceneObject::setArea() - Invalid number of parameters!");
      return;
   }

    // Set Target Camera Position.
    object->setTargetCameraPosition( position, width, height );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setTargetCameraZoom, void, 3, 3, "(zoomFactor) - Set target camera Zoom Factor."
              "@return No return value.")
{
    // Set Target Camera Zoom.
    object->setTargetCameraZoom( dAtof(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setCameraInterpolationTime, void, 3, 3, "(interpolationTime) - Set camera interpolation time."
              "@return No return value")
{
    // Set Camera Interpolation Time.
    object->setCameraInterpolationTime( dAtof(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setCameraInterpolationMode, void, 3, 3, "(interpolationMode) - Set camera interpolation mode."
              "@return No return value.")
{
    // Set Camera Interpolation Mode.
    object->setCameraInterpolationMode( getInterpolationMode(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, startCameraMove, void, 2, 3, "([interpolationTime]) - Start Camera Move."
              "@return No return value.")
{
    F32 interpolationTime;

    // Interpolation Time?
    if ( argc >= 3 )
        interpolationTime = dAtof(argv[2]);
    else
        interpolationTime = object->getCameraInterpolationTime();

    // Start Camera Move.
    object->startCameraMove( interpolationTime );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, stopCameraMove, void, 2, 2, "() Stops current camera movement"
              "@return No return value.")
{
    // Stop Camera Move.
    object->stopCameraMove();
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, completeCameraMove, void, 2, 2, "() Moves camera directly to target.\n"
              "@return No return value.")
{
    // Complete Camera Move.
    object->completeCameraMove();
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, undoCameraMove, void, 2, 3, "([interpolationTime]) - Reverses previous camera movement."
              "@return No return value.")
{
    F32 interpolationTime;

    // Interpolation Time?
    if ( argc >= 3 )
        interpolationTime = dAtof(argv[2]);
    else
        interpolationTime = object->getCameraInterpolationTime();

    // Undo Camera Move.
    object->undoCameraMove( interpolationTime );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getIsCameraMoving, bool, 2, 2, "() Check the camera moving status.\n"
              "@return Returns a boolean value as to whether or not the camera is moving.")
{
    // Is Camera Moving?
    return object->isCameraMoving();
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getIsCameraMounted, bool, 2, 2, "() Check the camera mounted status.\n"
              "@return Returns a boolean value as to whether or not the camera is mounted.")
{
    // Is Camera Mounted.
    return object->isCameraMounted();
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, startCameraShake, void, 4, 4, "(shakeMagnitude, time) - Starts the camera shaking."
              "@param shakeMagnitude The intensity of the shaking\n"
              "@param time The length of the shake"
              "@return No return value")
{
    // Start Camera Shake.
    object->startCameraShake( dAtof(argv[2]), dAtof(argv[3]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, stopCameraShake, void, 2, 2, "() Stops the camera shaking."
              "@return No return value")
{
    // Stop Camera Shake.
    object->stopCameraShake();
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, mount, void, 3, 7, "(SceneObject, [offsetX / offsetY], [mountForce], [sendToMount?]) - Mounts Camera onto a specified object."
              "@return No return value")
{
    // Grab the object. Always specified.
    SceneObject* pSceneObject = dynamic_cast<SceneObject*>(Sim::findObject(argv[2]));

    // Validate Object.
    if (!pSceneObject)
    {
        Con::warnf("SceneWindow::mount() - Couldn't find/Invalid object '%s'.", argv[2]);
        return;
    }

    // Reset Element Count.
    U32 elementCount = 2;
    // Calculate Mount-Offset.
    Vector2 mountOffset(0.0f, 0.0f);

    if (argc > 3)
    {
        // Fetch Element Count.
        elementCount =Utility::mGetStringElementCount(argv[3]);

        // (object, "offsetX offsetY", ...)
        if ((elementCount == 2) && (argc < 7))
            mountOffset = Utility::mGetStringElementVector(argv[3]);

        // (object, offsetX, offsetY, ...)
        else if ((elementCount == 1) && (argc > 4))
            mountOffset = Vector2(dAtof(argv[3]), dAtof(argv[4]));

        // Invalid.
        else
        {
            Con::warnf("SceneWindow::mount() - Invalid number of parameters!");
            return;
        }

    }

    // Set the next arg index.
    // The argv index of the first parameter after the offset.
    U32 firstArg = 6 - elementCount;

    // Grab the mount force - if it's specified.
    F32 mountForce = 0.0f;
    if ( (U32)argc > firstArg )
        mountForce = dAtof(argv[firstArg]);

    // Grab the send to mount flag.
    bool sendToMount = true;
    if ( (U32)argc > (firstArg + 1) )
        sendToMount = dAtob(argv[firstArg + 1]);

    // Mount Object.
    object->mount( pSceneObject, mountOffset, mountForce, sendToMount );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, dismount, void, 2, 2, "() Dismounts Camera from object."
              "@return No return value")
{
    // Dismount Object.
    object->dismount();
}

//-----------------------------------------------------------------------------

void SceneWindow::dismountMe( SceneObject* pSceneObject )
{
    // Are we mounted to the specified object?
    if ( isCameraMounted() && pSceneObject != mpMountedTo )
    {
        // No, so warn.
        Con::warnf("SceneWindow::dismountMe() - Object is not mounted by the camera!");
        return;
    }

    // Dismount Object.
    dismount();
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setViewLimitOn, void, 3, 6, "([minX / minY / maxX / maxY]) - Set View Limit On."
              "@return No return value")
{
   // Upper left bound.
   Vector2 v1;
   // Lower right bound.
   Vector2 v2;

   // Grab the number of elements in the first two parameters.
   U32 elementCount1 =Utility::mGetStringElementCount(argv[2]);
   U32 elementCount2 = 1;
   if (argc > 3)
      elementCount2 =Utility::mGetStringElementCount(argv[3]);

   // ("x1 y1 x2 y2")
   if ((elementCount1 == 4) && (argc == 3))
   {
       v1 = Utility::mGetStringElementVector(argv[2]);
       v2 = Utility::mGetStringElementVector(argv[2], 2);
   }
   
   // ("x1 y1", "x2 y2")
   else if ((elementCount1 == 2) && (elementCount2 == 2) && (argc == 4))
   {
      v1 = Utility::mGetStringElementVector(argv[2]);
      v2 = Utility::mGetStringElementVector(argv[3]);
   }
   
   // (x1, y1, x2, y2)
   else if (argc == 6)
   {
       v1 = Vector2(dAtof(argv[2]), dAtof(argv[3]));
       v2 = Vector2(dAtof(argv[4]), dAtof(argv[5]));
   }
   
   // Invalid
   else
   {
      Con::warnf("SceneWindow::setViewLimitOn() - Invalid number of parameters!");
      return;
   }

   // Calculate Normalised Rectangle.
   Vector2 topLeft((v1.x <= v2.x) ? v1.x : v2.x, (v1.y <= v2.y) ? v1.y : v2.y);
   Vector2 bottomRight((v1.x > v2.x) ? v1.x : v2.x, (v1.y > v2.y) ? v1.y : v2.y);

    // Set the View Limit On.
    object->setViewLimitOn(topLeft, bottomRight);
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setViewLimitOff, void, 2, 2, "() Set View Limit Off."
              "@return No return value")
{
    // Set View Limit Off.
    object->setViewLimitOff();
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, clampCurrentCameraViewLimit, void, 2, 2, "() Clamps the current camera to the current view limit.\n"
                                                                    "Nothing will happen if the view-limit is not active or the camera is moving.\n"
                                                                    "@return No return value")
{
    object->clampCurrentCameraViewLimit();
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setRenderGroups, void, 3, 2 + MASK_BITCOUNT, "(groups$) - Sets the render group(s).\n"
              "@param groups The list of groups you wish to set.\n"
              "@return No return value.")
{
   // The mask.
   U32 mask = 0;

   // Grab the element count of the first parameter.
   U32 elementCount =Utility::mGetStringElementCount(argv[2]);

   // Make sure we get at least one number.
   if (elementCount < 1)
   {
      Con::warnf("SceneWindow::setRenderGroups() - Invalid number of parameters!");
      return;
   }

   // Space separated list.
   if (argc == 3)
   {
      // Convert the string to a mask.
      for (U32 i = 0; i < elementCount; i++)
      {
         S32 bit = dAtoi(Utility::mGetStringElement(argv[2], i));
         
         // Make sure the group is valid.
         if ((bit < 0) || (bit >= MASK_BITCOUNT))
         {
            Con::warnf("SceneWindow::setRenderGroups() - Invalid group specified (%d); skipped!", bit);
            continue;
         }
         
         mask |= (1 << bit);
      }
   }

   // Comma separated list.
   else
   {
      // Convert the list to a mask.
      for ( U32 i = 2; i < (U32)argc; i++ )
      {
         S32 bit = dAtoi(argv[i]);
         
         // Make sure the group is valid.
         if ((bit < 0) || (bit >= MASK_BITCOUNT))
         {
            Con::warnf("SceneWindow::setRenderGroups() - Invalid group specified (%d); skipped!", bit);
            continue;
         }

         mask |= (1 << bit);
      }
   }
   // Set Collision Groups.
   object->setRenderGroups(mask);
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setRenderLayers, void, 3, 2 + MASK_BITCOUNT, "(layers$) - Sets the render layers(s)."
              "@param The layer numbers you wish to set.\n"
              "@return No return value.")
{
   // The mask.
   U32 mask = 0;

   // Grab the element count of the first parameter.
   U32 elementCount =Utility::mGetStringElementCount(argv[2]);

   // Make sure we get at least one number.
   if (elementCount < 1)
   {
      Con::warnf("SceneWindow::setRenderLayers() - Invalid number of parameters!");
      return;
   }

   // Space separated list.
   if (argc == 3)
   {
      // Convert the string to a mask.
      for (U32 i = 0; i < elementCount; i++)
      {
         S32 bit = dAtoi(Utility::mGetStringElement(argv[2], i));
         
         // Make sure the group is valid.
         if ((bit < 0) || (bit >= MASK_BITCOUNT))
         {
            Con::warnf("SceneWindow::setRenderLayers() - Invalid layer specified (%d); skipped!", bit);
            continue;
         }
         
         mask |= (1 << bit);
      }
   }

   // Comma separated list.
   else
   {
      // Convert the list to a mask.
      for ( U32 i = 2; i < (U32)argc; i++ )
      {
         S32 bit = dAtoi(argv[i]);
         
         // Make sure the group is valid.
         if ((bit < 0) || (bit >= MASK_BITCOUNT))
         {
            Con::warnf("SceneWindow::setRenderLayers() - Invalid layer specified (%d); skipped!", bit);
            continue;
         }

         mask |= (1 << bit);
      }
   }
   // Set Collision Groups.
   object->setRenderLayers(mask);
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setRenderMasks, void, 3, 4, "(layerMask, groupMask) - Sets the layer/group mask which control what is rendered."
              "@param layermask The bitmask for setting the layers to render\n"
              "@param groupmask The bitmask for setting the groups to render\n"
              "@return No return value.")
{
    // Set Render Masks.
   if( argc < 4 )
      object->setRenderLayers( dAtoi( argv[2] ) );
   else
      object->setRenderMasks( dAtoi(argv[2]), dAtoi(argv[3]) );
}   

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getRenderLayerMask, S32, 2, 2, "() - Gets the layer mask which controls what is rendered."
              "@returns The bit mask corresponding to the layers which are to be rendered")
{
   return object->getRenderLayerMask();
}   

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getRenderGroupMask, S32, 2, 2, "() - Gets the group mask which controls what is rendered."
              "@returns The bit mask corresponding to the groups which are to be rendered")
{
   return object->getRenderGroupMask();
} 

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setUseWindowInputEvents, void, 3, 3, "(inputStatus) Sets whether input events are monitored by the window or not.\n"
              "@param inputStatus Whether input events are processed by the window or not.\n"
              "@return No return value.")
{
    object->setUseWindowInputEvents( dAtob(argv[2]) );
}   

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getUseWindowInputEvents, bool, 2, 2, "() Gets whether input events are monitored by the window or not.\n"
              "@return Whether input events are monitored by the window or not.")
{
    return object->getUseWindowInputEvents();
}   

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setUseObjectInputEvents, void, 3, 3, "(inputStatus) Sets whether input events are passed onto scene objects or not.\n"
              "@param mouseStatus Whether input events are passed onto scene objects or not.\n")
{
    object->setUseObjectInputEvents( dAtob(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getUseObjectInputEvents, bool, 2, 2, "() Gets whether input events are passed onto scene objects or not.\n"
              "@return Whether input events are passed onto scene objects or not..")
{
    return object->getUseObjectInputEvents();
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setObjectInputEventGroupFilter, void, 3, 2 + MASK_BITCOUNT, "(groups$) Sets the input events group filter.\n"
              "@param List of groups to filter input events with.\n"
              "@return No return value.")
{
   // The mask.
   U32 mask = 0;

   // Grab the element count of the first parameter.
   U32 elementCount =Utility::mGetStringElementCount(argv[2]);

   // Make sure we get at least one number.
   if (elementCount < 1)
   {
      Con::warnf("SceneWindow::setObjectInputEventGroupFilter() - Invalid number of parameters!");
      return;
   }

   // Space separated list.
   if (argc == 3)
   {
      // Convert the string to a mask.
      for (U32 i = 0; i < elementCount; i++)
      {
         S32 bit = dAtoi(Utility::mGetStringElement(argv[2], i));
         
         // Make sure the group is valid.
         if ((bit < 0) || (bit >= MASK_BITCOUNT))
         {
            Con::warnf("SceneWindow::setObjectInputEventGroupFilter() - Invalid group specified (%d); skipped!", bit);
            continue;
         }
         
         mask |= (1 << bit);
      }
   }

   // Comma separated list.
   else
   {
      // Convert the list to a mask.
      for ( U32 i = 2; i < (U32)argc; i++ )
      {
         S32 bit = dAtoi(argv[i]);
         
         // Make sure the group is valid.
         if ((bit < 0) || (bit >= MASK_BITCOUNT))
         {
            Con::warnf("SceneWindow::setObjectInputEventGroupFilter() - Invalid group specified (%d); skipped!", bit);
            continue;
         }

         mask |= (1 << bit);
      }
   }

   // Set group filter.
   object->setObjectInputEventGroupFilter(mask);
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setObjectInputEventLayerFilter, void, 3, 2 + MASK_BITCOUNT, "(layers$) Sets the input events layer filter."
              "@param The list of layers to filter input events with.\n"
              "@return No return value.")
{
   // The mask.
   U32 mask = 0;

   // Grab the element count of the first parameter.
   U32 elementCount =Utility::mGetStringElementCount(argv[2]);

   // Make sure we get at least one number.
   if (elementCount < 1)
   {
      Con::warnf("SceneWindow::setObjectInputEventLayerFilter() - Invalid number of parameters!");
      return;
   }

   // Space separated list.
   if (argc == 3)
   {
      // Convert the string to a mask.
      for (U32 i = 0; i < elementCount; i++)
      {
         S32 bit = dAtoi(Utility::mGetStringElement(argv[2], i));
         
         // Make sure the group is valid.
         if ((bit < 0) || (bit >= MASK_BITCOUNT))
         {
            Con::warnf("SceneWindow::setObjectInputEventLayerFilter() - Invalid layer specified (%d); skipped!", bit);
            continue;
         }
         
         mask |= (1 << bit);
      }
   }

   // Comma separated list.
   else
   {
      // Convert the list to a mask.
      for ( U32 i = 2; i < (U32)argc; i++ )
      {
         S32 bit = dAtoi(argv[i]);
         
         // Make sure the group is valid.
         if ((bit < 0) || (bit >= MASK_BITCOUNT))
         {
            Con::warnf("SceneWindow::setObjectInputEventLayerFilter() - Invalid layer specified (%d); skipped!", bit);
            continue;
         }

         mask |= (1 << bit);
      }
   }

   // Set layer filter.
   object->setObjectInputEventLayerFilter(mask);
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setObjectInputEventFilter, void, 4, 5, "(groupMask, layerMask, [useInvisibleFilter?]) Sets input filter for input events.")
{
    // Calculate Use Invisible Flag.
    bool useInvisible = argc >= 5 ? dAtob(argv[4]) : true;

    // Set input event filter.
    object->setObjectInputEventFilter( dAtoi(argv[2]), dAtoi(argv[3]), useInvisible );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setObjectInputEventInvisibleFilter, void, 3, 3, "(bool useInvisibleFilter) Sets whether invisible objects should be filtered for input events or not.")
{
   object->setObjectInputEventInvisibleFilter(dAtob(argv[2]));
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setLockMouse, void, 3, 3, "(bool lockSet) Sets the window mouse-lock status."
              "@return No return value.")
{
    // Set Lock Mouse.
    object->setLockMouse( dAtob(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getLockMouse, bool, 2, 2, "() Gets the window mouse-lock status.")
{
    // Get Lock Mouse.
    return object->getLockMouse();
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setDebugBanner, void, 4, 12, "(fontName, fontSize, textR/G/B/[A]$, backgroundR/G/B/[A]$) - Set Debug Font/Size/textColour/backgroundColour")
{
   // Note: If only one of the two alphas are passed in a comma separated list, it is assumed to
   // be for the background. i.e (fontName, fontSize, r, g, b, r, g, b, a).

   ColorF debugTextColor, debugBannerColor;
   if (argc > 4)
   {
      U32 elementCount1 =Utility::mGetStringElementCount(argv[4]);
      U32 elementCount2 = 0;

      // (fontName, fontSize, "R G B [A]", "R G B [A]")
      if ((elementCount1 > 2) && (argc < 7))
      {
         debugTextColor.red = dAtof(Utility::mGetStringElement(argv[4], 0));
         debugTextColor.green = dAtof(Utility::mGetStringElement(argv[4], 1));
         debugTextColor.blue = dAtof(Utility::mGetStringElement(argv[4], 2));

         // Alpha specified?
         if (elementCount1 > 3)
            debugTextColor.alpha = dAtof(Utility::mGetStringElement(argv[4], 3));
         else
            debugTextColor.alpha = 1.0f;

         // Banner color specified?
         if (argc > 5)
            elementCount2 =Utility::mGetStringElementCount(argv[5]);
         else
            debugBannerColor.set(0.5f, 0.5f, 0.5f, 0.2f);

         if (elementCount2 > 2)
         {
            debugBannerColor.red = dAtof(Utility::mGetStringElement(argv[5], 0));
            debugBannerColor.green = dAtof(Utility::mGetStringElement(argv[5], 1));
            debugBannerColor.blue = dAtof(Utility::mGetStringElement(argv[5], 2));
   
            // Alpha specified?
            if (elementCount2 > 3)
               debugBannerColor.alpha = dAtof(Utility::mGetStringElement(argv[5], 3));
            else
               debugBannerColor.alpha = 1.0f;
         }
         else
         {
            // Warn.
            Con::warnf("SceneWindow::setDebugBanner() - Invalid debug banner colour specified; Must be at least three elements! (%s)", argv[4]);
            return;
         }
      }

      // (fontName, fontSize, R, G, B, [A], R, G, B, [A])
      else if ((elementCount1 == 1) && (argc > 6))
      {
         debugTextColor.red = dAtof(argv[4]);
         debugTextColor.green = dAtof(argv[5]);
         debugTextColor.blue = dAtof(argv[6]);

         U32 firstBannerArg = 0;

         // Alpha specified?
         if ((argc == 8) || (argc == 12))
         {
            firstBannerArg = 8;
            debugTextColor.alpha = dAtof(argv[7]);
         }
         else
         {
            firstBannerArg = 7;
            debugTextColor.alpha = 1.0f;
         }

         // Banner color specified?
         if (argc > 9)
         {
            debugBannerColor.red = dAtof(argv[firstBannerArg]);
            debugBannerColor.green = dAtof(argv[firstBannerArg + 1]);
            debugBannerColor.blue = dAtof(argv[firstBannerArg + 2]);

            // Alpha specified?
            if (argc == 11)
               debugBannerColor.alpha = dAtof(argv[10]);
            else if (argc == 12)
               debugBannerColor.alpha = dAtof(argv[11]);
            else
               debugBannerColor.alpha = 1.0f;
         }
         else
            debugBannerColor.set(0.5f, 0.5f, 0.5f, 0.2f);
      }

      // Invalid
      else
      {
         // Warn.
         Con::warnf("SceneWindow::setDebugBanner() - Invalid debug text colour specified; Must be at least three elements! (%s)", argv[4]);
         return;
      }
   }
   else
   {
      debugTextColor.set(0.0f, 0.0f, 0.0f, 1.0f);
      debugBannerColor.set(0.5f, 0.5f, 0.5f, 0.2f);
   }

   // Set Debug Banner.
   object->setDebugBanner( argv[2], dAtoi(argv[3]), debugTextColor, debugBannerColor );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setDebugBannerBackgroundColor, void, 3, 3, "(backgroundColor R/G/B/[A]) - Sets the debug banner background color.")
{
    ColorF backgroundColor;

    if ( argc == 3 )
    {
        U32 colorElements =Utility::mGetStringElementCount(argv[2]);
        if ( colorElements < 3 )
        {
            Con::warnf("SceneWindow::setDebugBannerBackgroundColor() - Invalid color specified.");
            return;
        }

        backgroundColor.red = dAtof(Utility::mGetStringElement(argv[2],0));
        backgroundColor.green = dAtof(Utility::mGetStringElement(argv[2],1));
        backgroundColor.blue = dAtof(Utility::mGetStringElement(argv[2],2));
        backgroundColor.alpha = ( colorElements == 3 ) ? 1.0f : dAtof(Utility::mGetStringElement(argv[2],3));
    }
    else
    {
        if ( argc < 5 )
        {
            Con::warnf("SceneWindow::setDebugBannerBackgroundColor() - Invalid color specified.");
            return;
        }

        backgroundColor.red = dAtof(argv[2]);
        backgroundColor.green = dAtof(argv[3]);
        backgroundColor.blue = dAtof(argv[4]);
        backgroundColor.alpha = argc == 5 ? 1.0f : dAtof(argv[5]);
    }

    // Set background color.
    object->setDebugBannerBackgroundColor( backgroundColor );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setDebugBannerForegroundColor, void, 3, 3, "(foregroundColor R/G/B/[A]) - Sets the debug banner foreground color.")
{
    ColorF foregroundColor;

    if ( argc == 3 )
    {
        U32 colorElements =Utility::mGetStringElementCount(argv[2]);
        if ( colorElements < 3 )
        {
            Con::warnf("SceneWindow::setDebugBannerForegroundColor() - Invalid color specified.");
            return;
        }

        foregroundColor.red = dAtof(Utility::mGetStringElement(argv[2],0));
        foregroundColor.green = dAtof(Utility::mGetStringElement(argv[2],1));
        foregroundColor.blue = dAtof(Utility::mGetStringElement(argv[2],2));
        foregroundColor.alpha = ( colorElements == 3 ) ? 1.0f : dAtof(Utility::mGetStringElement(argv[2],3));
    }
    else
    {
        if ( argc < 5 )
        {
            Con::warnf("SceneWindow::setDebugBannerForegroundColor() - Invalid color specified.");
            return;
        }

        foregroundColor.red = dAtof(argv[2]);
        foregroundColor.green = dAtof(argv[3]);
        foregroundColor.blue = dAtof(argv[4]);
        foregroundColor.alpha = argc == 5 ? 1.0f : dAtof(argv[5]);
    }

    // Set foreground color.
    object->setDebugBannerForegroundColor( foregroundColor );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, setMousePosition, void, 3, 4, "(x/y) Sets Current Mouse Position."
              "@param x,y The coordinates to set the mouse cursor. Accepts either (x,y) or (\"x y\")")
{
   // The new position.
   Vector2 position;

   // Elements in the first argument.
   U32 elementCount =Utility::mGetStringElementCount(argv[2]);

   // ("x y")
   if ((elementCount == 2) && (argc == 3))
      position = Utility::mGetStringElementVector(argv[2]);

   // (x, y)
   else if ((elementCount == 1) && (argc == 4))
      position = Vector2(dAtof(argv[2]), dAtof(argv[3]));

   // Invalid
   else
   {
      Con::warnf("SceneWindow::setMousePosition() - Invalid number of parameters!");
      return;
   }

    // Set Mouse Position.
    object->setMousePosition( position );
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getMousePosition, const char*, 2, 2, "() Gets Current Mouse Position."
              "@return Returns a string with the current mouse cursor coordinates formatted as \"x y\"")
{
    // Fetch Mouse Position.
    Vector2 worldMousePoint = object->getMousePosition();

    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer(32);

    // Generate Script Parameters.
    dSprintf(pBuffer, 32, "%g %g", worldMousePoint.x, worldMousePoint.y);

    // Return Buffer.
    return pBuffer;
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getWorldPoint, const char*, 3, 4, "(X / Y) - Returns World coordinate of Window coordinate."
              "@param x,y The coordinates in window coordinates you wish to convert to world coordinates. Accepts either (x,y) or (\"x y\")"
              "@return Returns the desired world coordinates as a string formatted as \"x y\"")
{
   // The new position.
   Vector2 srcPoint;

   // Elements in the first argument.
   U32 elementCount =Utility::mGetStringElementCount(argv[2]);

   // ("x y")
   if ((elementCount == 2) && (argc == 3))
      srcPoint = Utility::mGetStringElementVector(argv[2]);

   // (x, y)
   else if ((elementCount == 1) && (argc == 4))
      srcPoint = Vector2(dAtof(argv[2]), dAtof(argv[3]));

   // Invalid
   else
   {
      Con::warnf("SceneWindow::getWorldPoint() - Invalid number of parameters!");
      return false;
   }
   
   // Destination Point.
    Vector2 dstPoint;

    // Do Conversion.
    object->windowToScenePoint( srcPoint, dstPoint );

    return dstPoint.scriptThis();
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getWindowPoint, const char*, 3, 4, "(X / Y) - Returns Window coordinate of World coordinate."
              "@param x,y The coordinates in world coordinates you wish to convert to window coordinates. Accepts either (x,y) or (\"x y\")"
              "@return Returns the desired window coordinates as a string formatted as \"x y\"")
{
   // The new position.
   Vector2 srcPoint;

   // Elements in the first argument.
   U32 elementCount =Utility::mGetStringElementCount(argv[2]);

   // ("x y")
   if ((elementCount == 2) && (argc == 3))
      srcPoint = Utility::mGetStringElementVector(argv[2]);

   // (x, y)
   else if ((elementCount == 1) && (argc == 4))
      srcPoint = Vector2(dAtof(argv[2]), dAtof(argv[3]));

   // Invalid
   else
   {
      Con::warnf("SceneWindow::getWindowPoint() - Invalid number of parameters!");
      return NULL;
   }
   
   // Destination Point.
    Vector2 dstPoint;

    // Do Conversion.
    object->sceneToWindowPoint( srcPoint, dstPoint );

    return dstPoint.scriptThis();
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getCanvasPoint, const char*, 3, 4, "(X / Y) - Returns Canvas coordinate of Window coordinate."
               "@param x,y The coordinates in world coordinates you wish to convert to window coordinates. Accepts either (x,y) or (\"x y\")"
              "@return Returns the desired canvas coordinates as a string formatted as \"x y\"")
{
   // The new position.
   Point2I srcPoint;

   // Elements in the first argument.
   U32 elementCount =Utility::mGetStringElementCount(argv[2]);

   // ("x y")
   if ((elementCount == 2) && (argc == 3))
   {
      srcPoint.x = dAtoi(Utility::mGetStringElement(argv[2], 0));
      srcPoint.y = dAtoi(Utility::mGetStringElement(argv[2], 1));
   }

   // (x, y)
   else if ((elementCount == 1) && (argc == 4))
      srcPoint = Point2I(dAtoi(argv[2]), dAtoi(argv[3]));

   // Invalid
   else
   {
      Con::warnf("SceneWindow::getCanvasPoint() - Invalid number of parameters!");
      return NULL;
   }

    // Do Conversion.
    Point2I dstPoint = object->localToGlobalCoord( srcPoint );

    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer(32);
    // Format Buffer.
    dSprintf(pBuffer, 32, "%d %d", dstPoint.x, dstPoint.y);
    // Return buffer.
    return pBuffer;
}

//-----------------------------------------------------------------------------

ConsoleMethod(SceneWindow, getIsWindowPoint, bool, 3, 4, "(X / Y) Checks if World coordinate is inside Window."
               "@param x,y The coordinates in world coordinates you wish to check. Accepts either (x,y) or (\"x y\")"
              "@return Returns true if the coordinates are within the window, and false otherwise.")
{
   // The new position.
   Vector2 srcPoint;

   // Elements in the first argument.
   U32 elementCount =Utility::mGetStringElementCount(argv[2]);

   // ("x y")
   if ((elementCount == 2) && (argc == 3))
      srcPoint = Utility::mGetStringElementVector(argv[2]);

   // (x, y)
   else if ((elementCount == 1) && (argc == 4))
      srcPoint = Vector2(dAtof(argv[2]), dAtof(argv[3]));

   // Invalid
   else
   {
      Con::warnf("SceneWindow::getIsWindowPoint() - Invalid number of parameters!");
      return false;
   }

   // Destination Point.
    Vector2 dstPoint;
    // Do Conversion.
    object->sceneToWindowPoint( srcPoint, dstPoint );

    // Check if point is in window bounds.
    return object->mBounds.pointInRect( Point2I( S32(mFloor(dstPoint.x)+object->mBounds.point.x), S32(mFloor(dstPoint.y)+object->mBounds.point.y )) );
}
