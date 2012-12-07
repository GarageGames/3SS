//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// This is the basic editor for "local points", those points that are specified in 
/// terms of the -1 to 1 "local space" of an object, such as collision polygon points.
///
/// Modes!
/// showEditObject - Display a representation of the object you are editing
/// showPolygon - Display a polygon linking the local points
/// showHull - Display a polygon showing the convex hull of the local points
/// showConvexViolations - Flag points that aren't on the convex hull
/// clampToBounds - Only allow points in the -1 to +1 range
/// insertBetweenMode - Insert new points between the endpoints of the closest segment
/// showBackground - Display the scene background behind the editable object
/// zoomEnable - Whether to allow zoom or not
/// polyControlsEnable - Whether to allow the use of the special polygon controls
///
// Internal fields!
/// baseObject - the object we are editing
/// localPoints - Data structure that holds all the local point data structures
/// convexHull - Data structure that holds the local points on the convex hull
/// displayScene - The scene that holds all of the sceneobjects for the editor
/// editObject - The representation of the object we're editing inside the editor
/// displayPolygon - A ShapeVector that shows a polygon connecting the local points
/// hullPoly - A ShapeVector that shows a polygon of the convex hull of the local points
/// sceneAspect - a 2D vector that gives the aspect ratio of the base object
/// aspect - a 2D vector with the aspect ratio in the editor

function LocalPointEditor::onAdd(%this)
{
   // the scene to use to edit local points
   %this.displayScene = new Scene();
   
   // a polygon to show the convex hull.
   %this.hullPoly = new ShapeVector()
   {
      graphGroup = $LocalPointEditor::ignoreGroup;
      layer = $LocalPointEditor::polygonLayer;
   };
   %this.hullPoly.setLineColor(0 SPC 1 SPC 0 SPC 1);   
   %this.displayScene.add(%this.hullPoly);

   // a polygon connecting all the points.
   %this.displayPolygon = new ShapeVector() {
      graphGroup = $LocalPointEditor::ignoreGroup;
      layer = $LocalPointEditor::polygonLayer;
   };
   %this.displayPolygon.setLineColor(0 SPC 1 SPC 1 SPC 1);
   %this.displayScene.add(%this.displayPolygon);   
   
   // we want to track undo/redo for this editor separate from the global undo system.
   %this.undoManager = new UndoManager()
   {
      class = LPUndoManager;
   };
   
   // and we need an action map to keybind undo/redo
   %this.actionMap = new ActionMap();
}

function LocalPointEditor::onRemove(%this)
{
   // clean up the scene object, since we're tearing everything down.
   if (isObject(%this.displayScene))
   {
      %this.displayScene.delete();
   }
   
   if (isObject(%this.displayPolygon))
   {
      %this.displayPolygon.delete();
   }
   
   if (isObject(%this.hullPoly))
   {
      %this.hullPoly.delete();
   }
   
   if (isObject(%this.undoManager))
   {
      %this.undoManager.clearAll();
      %this.undoManager.delete();
   }
   
   if (isObject(%this.actionMap))
   {
      %this.actionMap.delete();
   }
}

function LocalPointEditor::open(%this, %object)
{
   if (%this.open)
   {
      // already open!  Close it first?
      %this.close();
   }
   
   if (LocalPointEditorGui.editorObject.open)
   {
      // another local point editor is open!  Close it!
      LocalPointEditorGui.editorObject.close();
   }
   
   // keep track of whether we're open or not.
   LocalPointEditorGui.editorObject = %this;
   %this.open = true;
   
   // Connect GUI to this editor object.
   LocalPointEditorWindowGui.closeCommand = %this @ ".cancel();";
   LPZoomSlider.altcommand = %this @ ".setZoom(LPZoomSlider.value);";
   LPRadioSquare.Command = %this @ ".updateAspect();";
   LPRadioPreserve.Command = %this @ ".updateAspect();";
   LPDisplayBackground.Command = %this @ ".setZoom(LPZoomSlider.value);";
   LPNewPointButton.Command = %this @ ".newLocalPoint(NewLPX.getText() SPC NewLPY.getText());";
   LPESaveButton.Command =  %this @ ".save();" @ " ToolManager.getLastWindow().setFirstResponder();";
   LPECancelButton.Command = %this @ ".cancel();" @ "ToolManager.getLastWindow().setFirstResponder();";
   LocalPointEditorWindowGui.text = %this.title @ "      -      Left click to create, Drag to move, Right click to delete. Drag and drop in scrolling area to reorder.";
      
   LPZoomLabel.setVisible(%this.zoomEnable);
   LPZoomSlider.setVisible(%this.zoomEnable);
   LPPolygonControls.setVisible(%this.polyControlsEnable);
   LPDisplayBackground.setValue(%this.showBackground);

   $LocalPointEditor::showHull = %this.showHull;
   LPShowHull.Command = %this @ ".updateHullPolygon();";
   $LocalPointEditor::showConvexViolations = %this.showConvexViolations;
   LPShowViolations.Command = %this @ ".updateDots();";
   LPInsertBetween.setValue(%this.insertBetweenMode);
   LPInsertAtEnd.setValue(!%this.insertBetweenMode);
   LPConvertToConvexButton.Command = %this @ ".replacePointsWithConvexHull();";

   LPUndo.Command = %this @ ".undoManager.undo();";
   LPRedo.Command = %this @ ".undoManager.redo();";

   NewLPX.setText("0.000");
   NewLPY.setText("0.000");

   Canvas.pushDialog(LocalPointEditorGui);
   LocalPointEditorWindowGui.setFirstResponder();
   %this.bindKeys();
   %this.actionMap.push();
      
   // need data structures to hold gridlines.
   if (!isObject(%this.vertLines))
      %this.vertLines = new SimSet();
   if (!isObject(%this.horizLines))
      %this.horizLines = new SimSet();


   // initialize the aspect ratio radio buttons.
   if (!LPRadioSquare.getValue() && !LPRadioPreserve.getValue())
   {
      LPRadioPreserve.setValue(true);
   }
   
   
   LEVisualEdit.setScene(%this.displayScene);
   
   %sceneWindow = ToolManager.getLastWindow();
   if (!isObject(%sceneWindow))
      return;
      
   %selectedObjects = ToolManager.getAcquiredObjects();
   %count = %selectedObjects.getCount();
   if (%count < 1 || %count > 1)
   {
      if (isObject(%object))
      {
         %this.editLocalPoints(%object);
      }
   }
   else
   {
      %object = %selectedObjects.getObject(0);
      %this.editLocalPoints(%object);
   }

   /*if (isObject(%object))
   {
      %this.editLocalPoints(%object);
   }*/
      
   // default zoom to 1.1, just a little bit bigger than the bounding box.
   LPZoomSlider.setValue(1.1);
   %this.setZoom(LPZoomSlider.getValue());
}

function LocalPointEditor::close(%this)
{
   // keep track of whether we're open or not.
   %this.open = false;
   LocalPointEditorGui.editorObject = null;
   
   // clean up the objects we may have created.
   %cleanupList = %this.localPoints;
   for(%i=0; %i<getWordCount(%cleanupList); %i++)
   {
      %obj = getWord(%cleanupList, %i);
      %obj.cleanUp();
   }
   %this.localPoints = "";

   // clean up the object display.
   if (isObject(%this.editObject))
   {
      %this.editObject.class = %this.baseObject.class;
      %this.editObject.superclass = %this.baseObject.superclass;
      %this.editObject.delete();
   }

   // unbind the keys
   %this.actionMap.pop();

   // clean up the undo manager.
   %this.undoManager.clearAll();
      
   // get rid of the window.
   Canvas.popDialog(LocalPointEditorGui);
}

function LocalPointEditor::editLocalPoints(%this, %object)
{
   %this.baseObject = %object;
   if (%object.getSizeX() > %object.GetSizeY())
   {
      // wide!
      %this.sceneAspect = 1 SPC (%object.getSizeY() / %object.getSizeX());
   }
   else
   {
      // tall or square.
      %this.sceneAspect = (%object.getSizeX() / %object.getSizeY()) SPC 1;
   }
   

   // determine what aspect ratio to use, to figure out size of object in the editor.
   if (LPRadioSquare.getValue())
   {
      // use a square aspect ratio!
      %this.aspect = 1 SPC 1;
   }
   else
   {
      // use the scene aspect ratio!
      %this.aspect = %this.sceneAspect;
   }
   
   // create representation of the object in the editor based on the object we're editing.
   %this.editObject = %object.clone();
   %this.editObject.class = "";
   %this.editObject.superclass = "";
   %this.editObject.clearBehaviors();
   %this.displayScene.add(%this.editObject);
   %this.editObject.setDebugOn(1);
   %this.editObject.setLinearVelocity(0 SPC 0);
   //%this.editObject.setAutoRotation(0);
   %this.editObject.setAngularVelocity(0);
   %this.editObject.setAngle(0);
   %this.editObject.setFlipX(false);
   %this.editObject.setFlipY(false);
   //%this.editObject.setCollisionActive(false, false);
   //%this.editObject.setCollisionPhysics(true, true);
   //%this.editObject.setPhysicsSuppress(true);
   %this.editObject.setSceneLayer($LocalPointEditor::editObjectLayer);
   %this.editObject.setSceneGroup($LocalPointEditor::ignoreGroup);
   %this.editObject.setPosition(0 SPC 0);
   //%this.editObject.setSize(Vector2Scale(%this.aspect, 2));
   %this.updateAspect();
   %this.editObject.setVisible(%this.showEditObject);
   %this.displayPolygon.setSize(Vector2Scale(%this.aspect, 2));
   %this.hullPoly.setSize(Vector2Scale(%this.aspect, 2));
   
   // Now load in the local points to edit. This will be overridden for each specific editor.
   %this.getLocalPoints();   

   %this.updateBackgroundCam();
}


function LocalPointEditor::cancel(%this)
{
   %this.close();
}


function LocalPointEditor::newLocalPoint(%this, %position)
{
   %this.insertBetweenMode = LPInsertBetween.getValue();
   if (%this.insertBetweenMode)
   {
      %seg = %this.findClosestSegment(%position);
      %newPoint = %this.createNewLocalPoint(%position);
      %this.movePoint(%newPoint.index, getWord(%seg, 0)+1);
   }
   else
   {
      %newPoint = %this.createNewLocalPoint(%position);
   }

   // update the scrolling control so the newly added point is visible.
   %scrollToPos = %newPoint.holder.getPosition();
   LETextScroll.setScrollPosition(getWord(%scrollToPos, 0), getWord(%scrollToPos, 1));

   // register into undo system
   %undo = new UndoScriptAction()
   {
      class = LocalPointUndo;
      actionName = "Create New Point";
      editor = %this;
      point = %newPoint;
      position = %newPoint.position;
      index = %newPoint.index;
   };
   %undo.addToManager(%this.undoManager);
   
   // give the created point back to whoever called us.
   return %newPoint;
}


function LocalPointEditor::createNewLocalPoint(%this, %position)
{
   %x = mFloatLength(getWord(%position, 0), $LocalPointEditor::precision);
   %y = mFloatLength(getWord(%position, 1), $LocalPointEditor::precision);

   %zoom = LPZoomSlider.getValue();

   %lpIndex = getWordCount(%this.localPoints);
   %lp = new ScriptObject() {
      class = LocalPointObject;
      index = %lpIndex;
      editor = %this;
   };

   // add the object to the list of active points.
   %this.localPoints = setWord(%this.localPoints, %lpIndex, %lp);

   // Create a visual representation of the local point.
   %lp.dot = new ShapeVector()
   {
      class = LocalPointDot;
      lpobject = %lp;
      graphGroup = $LocalPointEditor::selectGroup;
      layer = $LocalPointEditor::dotLayer;
   };
   %lp.dot.setSize(Vector2Scale($LocalPointEditor::dotSize, %zoom));
   %lp.dot.setPolyPrimitive(4);
   %lp.dot.setLineColor($LocalPointEditor::dotBorderColor);
   %lp.dot.setFillColor($LocalPointEditor::dotMainColor);
   %lp.dot.setFillMode(true);


   // Create a text label so you can tell the dots apart.
   %lp.label = new TextObject()
   {
      canSaveDynamicFields = "1";
      font = "Arial";
      wordWrap = "0";
      hideOverflow = "0";
      textAlign = "CENTER";
      aspectRatio = "1";
      lineSpacing = "0";
      characterSpacing = "0";
      fontSizes = "14";
      textColor = $LocalPointEditor::labelTextColor;
      size = 20 SPC 20; // this will be updated by the auto-sizer
      graphGroup = $LocalPointEditor::ignoreGroup;
      layer = $LocalPointEditor::labelLayer;
   };
   %lp.label.lineHeight = $LocalPointEditor::labelLineHeight * 2 * %zoom;
   %lp.label.text = %lpIndex;

   // add the dot and label to the scene.
   LEVisualEdit.getScene().add(%lp.dot);   
   LEVisualEdit.getScene().add(%lp.label);

   // connect them together.
   %lp.label.mount(%lp.dot);



   // Create a GUI representation of the local points.
   %lp.holder = new GuiStackControl()
   {
      extent = 364 SPC 32;
      profile = "EditorPanelLightModeless";
   };
   
   %container = new GuiMouseEventCtrl() 
   {
      class = LocalPointHolder;
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "264 32";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      localPoint = %lp;
   };

   %dotLabelBG = new GuiControl() 
   {
      canSaveDynamicFields = "0";
      Profile = LocalPointEditorDotBGProfile;
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "8 8";
      Extent = "16 16";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
   };
      
   %dotLabel = new GuiTextCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = LocalPointEditorDotTextProfile;
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "8 8";
      Extent = "16 16";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      text = "X";
      justify = "Center";
   };
   %dotLabel.text = %lpIndex;
   %lp.guiLabel = %dotLabel;
   %lp.guiBG = %dotLabelBG;
   
   %labelX = new GuiTextCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "LocalPointText";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "32 8";
      Extent = "18 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      text = "X";
      maxLength = "1024";
   };
   %entryX = new GuiTextEditCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorTextEdit";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "58 5";
      Extent = "64 22";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      maxLength = "1024";
      historySize = "0";
      password = "0";
      tabComplete = "0";
      sinkAllKeyEvents = "0";
      password = "0";
      passwordMask = "*";
   };
   %lp.guiX = %entryX;
   
   %labelY = new GuiTextCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "LocalPointText";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "130 8";
      Extent = "18 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      text = "Y";
      maxLength = "1024";
   };
   %entryY = new GuiTextEditCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorTextEdit";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "156 5";
      Extent = "64 22";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      maxLength = "1024";
      historySize = "0";
      password = "0";
      tabComplete = "0";
      sinkAllKeyEvents = "0";
      password = "0";
      passwordMask = "*";
   };
   %lp.guiY = %entryY;
   
   %deleteButton = new GuiIconButtonCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorButtonToolbar";
      HorizSizing = "right";
      VertSizing = "bottom";
      position = "236 4";
      Extent = "24 24";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      Command = %this @ ".deletePoint(" @ %lp @ ");";
      hovertime = "1000";
      tooltip = "Delete This Local Point";
      tooltipProfile = "EditorToolTipProfile";
      groupNum = "-1";
      buttonType = "PushButton";
      iconBitmap = "^{LevelEditor}/gui/images/iconCancel.png";
      sizeIconToButton = "0";
      textLocation = "Center";
      textMargin = "4";
      buttonMargin = "4 4";
      accelerator = "escape";
   };
   
   // bundle all the GUI elements we just created together.
   %container.add(%dotLabelBG);
   %container.add(%dotLabel);
   %container.add(%labelX);
   %container.add(%entryX);
   %container.add(%labelY);
   %container.add(%entryY);
   %container.add(%deleteButton);
   %lp.holder.add(%container);
   
   // put the GUI representation into the editor window.
   LETextEditor.add(%lp.holder);
      
   // Connect GUI elements to main object.
   %entryX.validate = %lp @ ".setPosition(" @ %entryX @ ".getText() SPC " @ %entryY @ ".getText());";
   %entryY.validate = %lp @ ".setPosition(" @ %entryX @ ".getText() SPC " @ %entryY @ ".getText());";
      
   // Now that everything is created, put it in the right place.
   %lp.setPosition(%x SPC %y);
   %lp.previousPosition = %x SPC %y; // it's never been anywhere before, so set this loc as previous.
   
   // give the object back to whoever called us.
   return %lp;   
}

function LocalPointEditor::deletePoint(%this, %lp)
{
   %undo = new UndoScriptAction()
   {
      class = LocalPointUndo;
      actionName = "Delete Point";
      editor = %this;
      position = %lp.position;
      index = %lp.index;
   };
   %undo.addToManager(%this.undoManager);
   
   %lp.cleanUp();
}


function LocalPointEditor::replacePointsWithConvexHull(%this)
{
   %oldPoints = %this.localPoints;
   %convex = %this.convexHull;
   for (%i=0; %i<getWordCount(%convex); %i++)
   {
      %cPoint = getWord(%convex, %i);
      %c[%cPoint] = true;
   }
   for (%i=0; %i<getWordCount(%oldPoints); %i++)
   {
      %oPoint = getWord(%oldPoints, %i);
      if (!%c[%oPoint])
      {
         %this.deletePoint(%oPoint);
      }
   }
   %this.localPoints = %convex;
   %this.updateIndices();
   %this.updateGeometry();
}


function LocalPointEditor::movePoint(%this, %oldIndex, %newIndex, %permanentChange)
{
   if (%oldIndex == %newIndex)
      return;
      
   %numPoints = getWordCount(%this.localPoints);
   
   if (%newIndex >= %numPoints)
      %newIndex = %numPoints - 1;

   if (%permanentChange)
   {
      %undo = new UndoScriptAction()
      {
         class = LocalPointUndo;
         actionName = "Reorder Point";
         editor = %this;
         oldIndex = %oldIndex;
         newIndex = %newIndex;
      };
      %undo.addToManager(%this.undoManager);
   }
   
   for (%i=0; %i<%numPoints; %i++)
   {
      if (((%i<%oldIndex) && (%i<%newIndex)) || ((%i>%oldIndex) && (%i>%newIndex)))
         %oldPoint = getWord(%this.localPoints, %i);
      else if (%i == %newIndex)
         %oldPoint = getWord(%this.localPoints, %oldIndex);
      else if (%newIndex < %oldIndex)
         %oldPoint = getWord(%this.localPoints, %i-1);
      else
         %oldPoint = getWord(%this.localPoints, %i+1);
         
      %newPoints = setWord(%newPoints, %i, %oldPoint);
   }

   %this.localPoints = %newPoints;
   
   // update the GUI to reflect the new order of the points.
   for (%i=0; %i<%numPoints; %i++)
   {
      %point = getWord(%this.localPoints, %i);
      LETextEditor.pushToBack(%point.holder);
   }
   
   // we changed the order, so update the indices.
   %this.updateIndices();
   
   // since the order changed, the polygons changed.
   %this.updateGeometry();
}

function LocalPointEditor::updateIndices(%this)
{
   // update the indices, and the index labels!
   %numPoints = getWordCount(%this.localPoints);
   for(%i=0; %i<%numPoints; %i++)
   {
      %obj = getWord(%this.localPoints, %i);
      if (%obj.index != %i)
      {
         %obj.index = %i;
         %obj.label.text = %i;
         %obj.guiLabel.setText(%i);
      }
   }
}


//////////////////////////////////////////////////////////////////////
// The scenewindow "gui" stuff
//////////////////////////////////////////////////////////////////////

function LocalPointEditor::updateHullPolygon(%this)
{
   %this.showHull = LPShowHull.getValue();
   
   if (!%this.showHull)
   {
      %this.hullPoly.setVisible(false);
      return;
   }
   else
   {
      %this.hullPoly.setVisible(true);
   }
   
   %hull = %this.convexHull;
   for (%i=0; %i<getWordCount(%hull); %i++)
   {
      %point = getWord(%hull, %i);
      %hullPoly = setWord(%hullPoly, 2*%i, getWord(%point.position, 0));
      %hullPoly = setWord(%hullPoly, 2*%i+1, getWord(%point.position, 1));
   }
   
   %numHullPoints = getWordCount(%hull);
   if (%hull >= 2)
      %this.hullPoly.setPolyCustom(getWordCount(%hull), %hullPoly);
   else
      %this.hullPoly.setPolyPrimitive(4);
}


function LocalPointEditor::updateDisplayPolygon(%this)
{
   if (!%this.showPolygon)
      return;
   
   %length = getWordCount(%this.localPoints);
   %newPoints = "";
   for (%i=0; %i<%length; %i++)
   {
      %obj = getWord(%this.localPoints, %i);
      %objX = getWord(%obj.position, 0);
      %objY = getWord(%obj.position, 1);
      %newPoints = setWord(%newPoints, %i*2, %objX);
      %newPoints = setWord(%newPoints, %i*2+1, %objY);
   }

   if (%length >= 2)
      %this.displayPolygon.setPolyCustom(%length, %newPoints);
   else
      %this.displayPolygon.setPolyPrimitive(4);
}

function LocalPointEditor::updateDots(%this)
{
   %this.showConvexViolations = LPShowViolations.getValue();
   
   if (!%this.showConvexViolations)
   {
      // make sure the colors are normal, whether we violate convexity or not.
      %numP = getWordCount(%this.localPoints);
      for (%i=0; %i<%numP; %i++)
      {
         %iPoint = getWord(%this.localPoints, %i);
         if (%iPoint.guiBG.Profile !$= LocalPointEditorDotBGProfile)
         {
            %iPoint.dot.setFillColor($LocalPointEditor::dotMainColor);
            %iPoint.guiBG.setProfile(LocalPointEditorDotBGProfile);
         }
      }
      return;
   }
   
   // Figure out which points are on the convex hull, and which aren't.
   %convex = %this.convexHull;
   %numC = getWordCount(%convex);
   %numP = getWordCount(%this.localPoints);

   // need to get points and convex in same order.
   %startP = getWord(%convex, 0);
   for (%offset=0; %offset<%numP; %offset++)
   {
      if (getWord(%this.localPoints, %offset) == %startP)
         break;
   }
   for (%i=0; %i<%numP; %i++)
   {
      %points = setWord(%points, %i, getWord(%this.localPoints, ((%offset+%i) % %numP)));
   }

   // make the convex points normal, and the non-convex ones errors
   %p = 0;
   for (%c=0; %c<%numC; %c++)
   {
      %cPoint = getWord(%convex, %c);
      %pPoint = getWord(%points, %p);
      while ((%pPoint != %cPoint) && (%p < %numP))
      {
         if (isObject(%pPoint))
         {
            %pPoint.dot.setFillColor($LocalPointEditor::dotErrorMainColor);
            %pPoint.guiBG.setProfile(LocalPointEditorDotBGErrorProfile);
         }
         %p++;
         %pPoint = getWord(%points, %p);
      }

      if (%pPoint == %cPoint)
      {
         %pPoint.dot.setFillColor($LocalPointEditor::dotMainColor);
         %pPoint.guiBG.setProfile(LocalPointEditorDotBGProfile);
         %p++;
      }
   }
   
   if (%p<%numP)
   {
      for (%i=%p; %i<%numP; %i++)
      {
         %iPoint = getWord(%points, %i);
         %iPoint.dot.setFillColor($LocalPointEditor::dotErrorMainColor);
         %iPoint.guiBG.setProfile(LocalPointEditorDotBGErrorProfile);
      }
   }
}

function LocalPointEditor::updateGridLines(%this)
{
   // find a visually pleasing grid spacing.
   %gridSpacingX = 0.2;
   %gridSpacingY = 0.2;
   
   %pos = LEVisualEdit.getCurrentCameraPosition();
   %size = LEVisualEdit.getCurrentCameraSize();
   %halfSize = Vector2Scale(%size, 0.5);
   %halfSize = %this.sceneSpaceToLocalSpace(%halfSize);

   
   %min = Vector2Sub(%pos, %halfSize);
   %max = Vector2Add(%pos, %halfSize);

   %minX = getWord(%min, 0);
   %minY = getWord(%min, 1);
   %maxX = getWord(%max, 0);
   %maxY = getWord(%max, 1);
   
   %width = %maxX - %minX;
   %height = %maxY - %minY;
   %midPointX = (%minX + %maxX)/2;
   %midPointY = (%minY + %maxY)/2;
   
   %grid[0] = 0.1;
   %grid[1] = 0.25;
   %grid[2] = 0.5;
   
   
   %gIndex = 0;
   %gScale = 1;
   %foundIt = 0;
   
   if (%height < %width)
      %limitSize = %height;
   else
      %limitSize = %width;
   
   while(!%foundIt)
   {
      %numLines = %limitSize / (%grid[%gIndex]*%gScale);
      
      if (%numLines >= 20) // 15
      {
         // increase grid size.
         %gIndex++;
         if (%gIndex >= 3)
         {
            %gIndex = 0;
            %gScale *= 10;
         }
      }
      else if (%numLines < 8) // 5
      {
         // decrease grid size.
         %gIndex--;
         if (%gIndex < 0)
         {
            %gIndex = 3;
            %gScale *= 0.1;
         }
      }
      else
      {
         %foundIt = 1;
      }
   }
   
   %gridSpacing = %grid[%gIndex] * %gScale;
   
   // vert lines;
   %i = 0;
   for (%x=%gridSpacing*mFloor(%minX/%gridSpacing); %x<%maxX; %x+= %gridSpacing)
   {
      // get an object to use.  Re-use if we've already got some in the scene.
      if (%i >= %this.vertLines.getCount())
      {
         %line = new ShapeVector() {
            graphGroup = $LocalPointEditor::ignoreGroup;
            layer = $LocalPointEditor::gridLayer; //10;
         };
         %line.setPolyCustom(2, 0.0 SPC -1.0 SPC 0.0 SPC 1.0);
         %this.vertLines.add(%line);
         LEVisualEdit.getScene().add(%line);
      }
      else
      {
         %line = %this.vertLines.getObject(%i);
      }

      // keep track of the line index.
      %i++;
      
      // orient the line.
      %line.setSize(0.05 SPC %height);
      %line.setPosition(%this.localSpaceToSceneSpace(%x SPC %midpointY));
      
      // color the gridlines.
      if (%x == 0)
         %line.setLineColor(0 SPC 0 SPC 0 SPC 1);  // dark black line.
      else
         %line.setLineColor(0 SPC 0 SPC 0 SPC 0.1); // light gray line.
   }

   // get rid of leftovers.
   while (%this.vertLines.getCount() > %i)
      %this.vertLines.getObject(%i).delete();


   // horiz lines;
   %j = 0;
   for (%y=%gridSpacing*mFloor(%minY/%gridSpacing); %y<%maxY; %y+= %gridSpacing)
   {
      // get an object to use.  Re-use if we've already got some in the scene.
      if (%j >= %this.horizLines.getCount())
      {
         %line = new ShapeVector() {
            graphGroup = $LocalPointEditor::ignoreGroup;
            layer = $LocalPointEditor::gridLayer;
         };
         %line.setPolyCustom(2, -1.0 SPC 0.0 SPC 1.0 SPC 0.0);
         %this.horizLines.add(%line);
         LEVisualEdit.getScene().add(%line);
      }
      else
      {
         %line = %this.horizLines.getObject(%j);
      }

      // keep track of the line index.
      %j++;
      
      // orient the line.
      %line.setSize(%width SPC 0.05);
      %line.setPosition(%this.localSpaceToSceneSpace(%midPointX SPC %y));

      if (%y == 0)
         %line.setLineColor(0 SPC 0 SPC 0 SPC 1);  // dark black line.
      else
         %line.setLineColor(0 SPC 0 SPC 0 SPC 0.1); // light gray line.
   }

   // get rid of leftovers.
   while (%this.horizLines.getCount() > %j)
      %this.horizLines.getObject(%j).delete();

}


function LocalPointEditor::updateAspect(%this)
{
   if (LPRadioSquare.getValue())
   {
      // make it square!
      %this.aspect = 1 SPC 1;
   }
   else
   {
      // use scene aspect ratio!
      %this.aspect = %this.sceneAspect;
   }
   
   // reflect new values in the visual editor.
   %this.editObject.setSize(Vector2Scale(%this.aspect, 2));

   if (%this.editObject.getClassName() $= "TileLayer")
   {
      %mySize = %this.editObject.getSize();
      %baseSize = %this.baseObject.getSize();
      %tileScale = getWord(%mySize, 0) / getWord(%baseSize, 0) SPC getWord(%mySize, 1) / getWord(%baseSize, 1);
      %this.editObject.setTileSize(Vector2Mult(%this.baseObject.getTileSize(), %tileScale));
   }
   else if (%this.editObject.getClassName() $= "Scroller")
   {
      %mySize = %this.editObject.getSize();
      %baseSize = %this.baseObject.getSize();
      %scrollerScale = getWord(%mySize, 0) / getWord(%baseSize, 0) SPC getWord(%mySize, 1) / getWord(%baseSize, 1);
      %this.editObject.setScroll(Vector2Mult(%this.baseObject.getScrollX() SPC %this.baseObject.getScrollY(), %scrollerScale));
   }

   %this.displayPolygon.setSize(Vector2Scale(%this.aspect, 2));
   %this.hullPoly.setSize(Vector2Scale(%this.aspect, 2));
   
   for (%i=0; %i<getWordCount(%this.localPoints); %i++)
   {
      %obj = getWord(%this.localPoints, %i);
      %obj.setPosition(%obj.position);
   }
   %this.setZoom(LPZoomSlider.value);
}


function LocalPointEditor::setZoom(%this, %zoom)
{
   %camSize = 2*%zoom SPC 2*%zoom;
   LEVisualEdit.setCurrentCameraPosition(0 SPC 0, %camSize);
   %this.updateGridLines();

   // resize the dots, so they stay the same size on the screen.
   %dotSize = Vector2Scale($LocalPointEditor::dotSize, %zoom);
   %charSize = $LocalPointEditor::labelLineHeight * 2 * %zoom;
   
   for (%i=0; %i<getWordCount(%this.localPoints); %i++)
   {
      %lp = getWord(%this.localPoints, %i);
      %lp.dot.setSize(%dotSize);
      %lp.label.lineHeight = %charSize;
      %lp.label.text = %lp.label.text;
   }

   %this.updateBackgroundCam();
}

function LocalPointEditor::updateBackgroundCam(%this)
{
   %updateBG = LPDisplayBackground.getValue();
   if (%updateBG)
   {
      LEBackgroundDisplay.setScene(%this.baseObject.getScene());
   
      %size = Vector2Scale(%this.baseObject.getSize(), LPZoomSlider.value);
      %size = (getWord(%size, 0) / getWord(%this.aspect, 0)) SPC 
              (getWord(%size, 1) / getWord(%this.aspect, 1));
      LEBackgroundDisplay.setCurrentCameraPosition(%this.baseobject.getPosition(), %size);
   }
   LEBackgroundDisplay.setVisible(%updateBG);
}

function LocalPointObject::cleanUp(%this)
{
   // first, remove it from the list.
   %numPoints = getWordCount(%this.editor.localPoints);
   %newPoints = "";
   for (%i=0; %i<%numPoints; %i++)
   {
      if (%i<%this.index)
         %newPoints = setWord(%newPoints, %i, getWord(%this.editor.localPoints, %i));
      if (%i > %this.index)
         %newPoints = setWord(%newPoints, %i-1, getWord(%this.editor.localPoints, %i));
   }
   %this.editor.localPoints = %newPoints;
               
   // now eliminate the object's editor representations.
   %this.dot.delete();
   %this.holder.delete();
   
   // now that a point is gone, update the display.
   %this.editor.updateIndices();
   %this.editor.updateGeometry();

   // now finalize the cleanup by deleting this object itself.
   %this.delete();
}

function LocalPointEditor::updateGeometry(%this)
{
   %this.convexHull = %this.calculateConvexHull();
   %this.updateDisplayPolygon();
   %this.updateHullPolygon();
   %this.updateDots();
}

function LocalPointObject::setPosition(%this, %pos, %permanent)
{
   %x = mFloatLength(getWord(%pos, 0), $LocalPointEditor::precision);
   %y = mFloatLength(getWord(%pos, 1), $LocalPointEditor::precision);

   if (%this.editor.clampToBounds)
   {
      if (%x > 1.0) %x  = mFloatLength(1,  $LocalPointEditor::precision);
      if (%x < -1.0) %x = mFloatLength(-1, $LocalPointEditor::precision);
      if (%y > 1.0) %y  = mFloatLength(1,  $LocalPointEditor::precision);
      if (%y < -1.0) %y = mFloatLength(-1, $LocalPointEditor::precision);
   }

   if ((%permanent) && (%this.previousPosition !$= (%x SPC %y)))
   {
      // log with the undo system.
      %undo = new UndoScriptAction(){
         class = LocalPointUndo;
         actionName = "Move Point"; 
         localPoint = %this;
         oldPosition = %this.previousPosition;
         newPosition = %x SPC %y;
      };
      %undo.addToManager(%this.editor.undoManager);       

      // since this is a permanent move, this is the new original point for the next undo.
      %this.previousPosition = %x SPC %y;
   }

   %this.position = %x SPC %y;
   
   // calculate where to display the dot, graphically.
   %scenePosition = %this.editor.LocalSpaceToSceneSpace(%x SPC %y);
   %this.dot.setPosition(%scenePosition);
   %this.guiX.setText(%x);
   %this.guiY.setText(%y);

   // we changed a points position, so make sure the polygons, etc., are also updated.
   %this.editor.updateGeometry();

   LocalPointEditorWindowGui.setFirstResponder();
}

function LocalPointUndo::undo(%this)
{
   if (%this.actionName $= "Move Point")
   {
      %this.localPoint.setPosition(%this.oldPosition);
   }
   else if (%this.actionName $= "Create New Point")
   {
      %lp = getWord(%this.editor.localPoints, %this.index);
      %lp.cleanUp();
   }
   else if (%this.actionName $= "Delete Point")
   {
      %lp = %this.editor.createNewLocalPoint(%this.position);
      %this.editor.movePoint(%lp.index, %this.index);
   }
   else if (%this.actionName $= "Reorder Point")
   {
      %this.editor.movePoint(%this.newIndex, %this.oldIndex);
   }
}

function LocalPointUndo::redo(%this)
{
   if (%this.actionName $= "Move Point")
   {
      %this.localPoint.setPosition(%this.newPosition);
   }
   else if (%this.actionName $= "Create New Point")
   {
      %lp = %this.editor.createNewLocalPoint(%this.position);
      %this.editor.movePoint(%lp.index, %this.index);
   }
   else if (%this.actionName $= "Delete Point")
   {
      %lp = getWord(%this.editor.localPoints, %this.index);
      %lp.cleanUp();
   }
   else if (%this.actionName $= "Reorder Point")
   {
      %this.editor.movePoint(%this.oldIndex, %this.newIndex);
   }
}

function LPUndoManager::onClear(%this)
{
}

function LocalPointEditorKeybindUndo(%val)
{
   if (%val > 0)
      LocalPointEditorGui.editorObject.undoManager.undo();
}

function LocalPointEditorKeybindRedo(%val)
{
   if (%val > 0)
      LocalPointEditorGui.editorObject.undoManager.redo();
}

function LocalPointEditor::bindKeys(%this)
{
   if($platform $= "macos")
   {
      //%this.actionMap.bind(keyboard, "cmd z",       LocalPointEditorKeybindUndo, "Undo The Last Action");
      //%this.actionMap.bind(keyboard, "cmd-shift z", LocalPointEditorKeybindRedo, "Redo an Undone Action");
      %this.actionMap.bind(keyboard, "cmd z",       LocalPointEditorKeybindUndo);
      %this.actionMap.bind(keyboard, "cmd-shift z", LocalPointEditorKeybindRedo);
   }
   else
   {
      //%this.actionMap.bind(keyboard, "ctrl z", LocalPointEditorKeybindUndo, "Undo The Last Action");
      //%this.actionMap.bind(keyboard, "ctrl y", LocalPointEditorKeybindRedo, "Redo an Undone Action");
      %this.actionMap.bind(keyboard, "ctrl z", LocalPointEditorKeybindUndo);
      %this.actionMap.bind(keyboard, "ctrl y", LocalPointEditorKeybindRedo);
   }
}

//////////////////////////////////////////////////////////////////////////
// Geometry Stuff
//////////////////////////////////////////////////////////////////////////

function LocalPointEditor::SceneSpaceToLocalSpace(%this, %pos)
{
   // return the local space coordinates, based on the scene we're using to display.
   %localVector = (getWord(%pos, 0) / getWord(%this.aspect, 0)) SPC
                  (getWord(%pos, 1) / getWord(%this.aspect, 1));
   return %localVector;
}

function LocalPointEditor::LocalSpaceToSceneSpace(%this, %Pos)
{
   // return the scene coordinates in the display, based on local space coordinates.
   %sceneVector = (getWord(%pos, 0) * getWord(%this.aspect, 0)) SPC
                  (getWord(%pos, 1) * getWord(%this.aspect, 1));
   return %sceneVector;
}

function LocalPointEditor::calculateConvexHull(%this)
{
   // Use a gift-wrapping algorithm to calculate the convex hull of the local points.
   
   %numPoints = getWordCount(%this.localPoints);
   if (%numPoints == 0)
      return "";

   // find the local point with the minimum X value. This extreme point is 
   // guaranteed to be on the convex hull.
   %lpMinX = getWord(%this.localPoints, 0);
   %minX = getWord(%lpMinX.position, 0);
   for (%i=1; %i<%numPoints; %i++)
   {
      %lp = getWord(%this.localPoints, %i);
      %x = getWord(%lp.position, 0);
      if (%x<%minX)
      {
         %lpMinX = %lp;
         %minX = %x;
      }
   }
   
   // add it to the hull, and make it the starting point.
   %convexHull = %lpMinX;
   %currentPoint = %lpMinX;
   
   // make bogus "last angle" so small than any real angle will be greater than it.
   %lastAngle = -1000; 

   while (true)
   {
      // set dummy values so any concievable real value will be "better".
      %bestCandidate = %currentPoint;
      %bestAngle = 1000.0;
   
      for (%i=0; %i<%numPoints; %i++)
      {
         %lp = getWord(%this.localPoints, %i);
         if (%lp == %currentPoint)
            continue;
         
         %angle = t2dAngleToPoint(%currentPoint.position, %lp.position);
         if (%angle < 0) 
            %angle += 360.0;

         if ((%angle < %bestAngle) && (%angle > %lastAngle))
         {
            %bestCandidate = %lp;
            %bestAngle = %angle;
         }
      }

      // scan current convex hull, see if we've completed a circuit.
      for (%j=0; %j<getWordCount(%convexHull); %j++)
      {
         %point = getWord(%convexHull, %j);
         if (%point == %bestCandidate)
            return %convexHull;
      }

      // otherwise, add the best candidate point to the convex hull and continue.
      %convexHull = %convexHull SPC %bestCandidate;
      %currentPoint = %bestCandidate;
      %lastAngle = %bestAngle;
   }
}

function LocalPointEditor::findClosestSegment(%this, %pos)
{
   %numVertices = getWordCount(%this.localPoints);
   %bestSeg = %numVertices - 1 SPC %numVertices;
   
   if (%numVertices < 3)
      return %bestSeg;
      
   %bestLength = 10000000;
   for (%i=0; %i<%numVertices; %i++)
   {
      %nextVertex = (%i+1) % %numVertices;
      %curPos = getWord(%this.localPoints, %i).position;
      %nextPos = getWord(%this.localPoints, %nextVertex).position;

      // calculate distance from point to segment
      %x1 = getWord(%curPos, 0);
      %y1 = getWord(%curPos, 1);
      %x2 = getWord(%nextPos, 0);
      %y2 = getWord(%nextPos, 1);
      %x3 = getWord(%pos, 0);
      %y3 = getWord(%pos, 1);
      
      %segLength = Vector2Distance(%curPos, %nextPos);
      
      %u = ((%x3-%x1)*(%x2-%x1) + (%y3-%y1)*(%y2-%y1))/((%x2-%x1)*(%x2-%x1) + (%y2-%y1)*(%y2-%y1));
      if ((%u>0) && (%u<1))
      {
         %px = %x1 + %u * (%x2 - %x1);
         %py = %y1 + %u * (%y2 - %y1);
         
         %length = Vector2Distance(%pos, %px SPC %py);   
      }
      else
      {
         %l1 = Vector2Distance(%curPos, %pos);
         %l2 = Vector2Distance(%nextPos, %pos);
         
         if (%l1<%l2)
            %length = %l1;
         else
            %length = %l2;
      }

      if (%length < %bestLength)
      {
         %bestSeg = %i SPC %nextVertex;
         %bestLength = %length;
      }

   }
   return %bestSeg;
}
