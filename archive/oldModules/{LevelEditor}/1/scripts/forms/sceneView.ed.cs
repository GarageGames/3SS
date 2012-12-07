
//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
$LBSceneViewContent = GuiFormManager::AddFormContent( "LevelBuilder", "Scene View", "LBSceneWindow::CreateForm", "LBSceneWindow::SaveForm", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBSceneWindow::CreateForm( %formCtrl )
{    
   %base = new LevelBuilderSceneWindow() 
   {
      class = "LevelBuilderSceneView";
      canSaveDynamicFields = "0";
      Profile = "LBSceneWindow";
      HorizSizing = "width";
      VertSizing = "height";
      position = "0 0";
      Extent = "668 1000";
      MinExtent = "10 10";
      canSave = "0";
      visible = "1";
      internalName = "LevelBuilderSceneView";
      tooltipprofile = "GuiToolTipProfile";
      hovertime = "0";
      lockMouse = "0";
      UseWindowInputEvents = "1";
      UseObjectInputEvents = "0";
      bob = "your uncle";
   };
   %formCtrl.add(%base);
   %formCtrl.canSaveDynamicFields = 0;

   // Propagate Saved State Settings.
   if( %formCtrl.lastCameraZoom !$= "" )
      %base.setCurrentCameraZoom( %formCtrl.lastCameraZoom );
   if( %formCtrl.lastCameraPosition !$= "" )
      %base.setCurrentCameraPosition( %formCtrl.lastCameraPosition );

   if( %formCtrl.lastScene !$= "" && isObject( %formCtrl.lastScene ) && %formCtrl.lastScene.getClassName() $= "Scene" )
      %base.setScene( %formCtrl.lastScene );
   else if (isObject($currentScene))
      %base.setScene($currentScene);
   else
      // Request Notification of Active scne
      GuiFormManager::SendContentMessage( $LBSceneViewContent, %base, "getScene" );
      
  if (!isObject(ToolManager.getActiveTool()))
      LevelBuilderToolManager::setTool(LevelEditorSelectionTool);
   
   // Resize as appropriate.
   if( %formCtrl.isMethod("sizeContentsToFit") )
      %formctrl.sizeContentsToFit(%base, %formCtrl.contentID.margin);

   $SceneViewID = %base;
   
   //*** Return back the base control to indicate we were successful
   return %base;

}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBSceneWindow::SaveForm( %formCtrl, %contentObj )
{
   %formCtrl.lastCameraZoom = %contentObj.getCurrentCameraZoom();
   %formCtrl.lastCameraPosition = %contentObj.getCurrentCameraPosition();
   %formCtrl.lastScene = %contentObj.getScene();
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LevelBuilderSceneView::onContentMessage( %this, %sender, %message )
{

   %command = GetWord( %message, 0 );
   %value   = GetWord( %message, 1 );

   switch$( %command )
   {
      case "updateScene":
         // Update scene to %value's scene.
         if( isObject( %value ) )
            %this.setScene( %value );
      case "getScene":
         // Update sibling view with our scene.
         if( %this.getScene() )
            %sender.onContentMessage( %this, "updateScene " @ %this.getScene() );
      case "playScene":
         %this.testScene();
      case "pauseScene":
         %this.endTestScene();
   }
}

//-----------------------------------------------------------------------------
// Form Content Functionality
//-----------------------------------------------------------------------------
function LevelBuilderSceneView::onAdd(%this)
{
   if (!isObject(ToolManager))
      error("LevelBuilderSceneWindow::onAdd() - No ToolManager Found!");
   else
      %this.setSceneEdit(ToolManager);
}

function LevelBuilderSceneView::onRightMouseDown(%this, %modifier, %position, %clicks)
{
   %this.rightMouseDownPosition = %position;
}

function LevelBuilderSceneView::onRightMouseDragged(%this, %modifier, %position, %clicks)
{
   // NewPosition = OldPosition + (MouseDownPosition - MousePosition)
   %movement = Vector2Sub(%this.rightMouseDownPosition, %position);
   %newPosition = Vector2Add(%this.getCurrentCameraPosition(), %movement);
   
   if($AdvancedMode)
      %this.setCurrentCameraPosition(%newPosition);
}

function LevelBuilderSceneView::onMouseWheelDown(%this, %modifier, %position, %clicks)
{
   %zoom = %this.getCurrentCameraZoom();
   %amount = -120 * 0.0005 * %zoom;
   
   if($AdvancedMode)
      %this.setCurrentCameraZoom(%zoom + %amount);
}

function LevelBuilderSceneView::onMouseWheelUp(%this, %modifier, %position, %clicks)
{
   %zoom = %this.getCurrentCameraZoom();
   %amount = 120 * 0.0005 * %zoom;
   
   if($AdvancedMode)
      %this.setCurrentCameraZoom(%zoom + %amount);
}

function LevelBuilderSceneView::onControlDropped(%this, %control, %position)
{
   if (isObject(%control.sceneObject))
   {
      %datablockName = %control.datablockName;
      %toolType      = %control.toolType;
   
      switch$( %toolType )
      {
         case "t2dStaticSprite":
               %object = %control.getSceneObject();
              %currentFrame = %object.getFrame();
              levelEditorStaticSpriteTool.setImageMap( %datablockName, %currentFrame );   
              
               $selectedStaticSprite = %datablockName;
         case "BitmapFontObject":
            %object = %control.getSceneObject();
            %text = %object.getText();
            LevelEditorBitmapFontTool.setImageMap( %datablockName );
            LevelEditorBitmapFontTool.setText( %text );
            $selectedBitmapFontObject = %datablockName;
         case "t2dAnimatedSprite":
            $selectedAnimatedSprite = %datablockName;
            levelEditorAnimatedSpriteTool.setAnimation( %datablockName );
         case "Scroller":
            $selectedScroller = %datablockName;
            levelEditorScrollerTool.setImageMap( %datablockName );
         case "TileLayer":
            $selectedTileMap = %datablockName;
            levelEditorTileMapTool.setTileLayerFile( %datablockName );
         case "ParticleEffect":
            $selectedParticleEffect = %datablockName;
            levelEditorParticleTool.setEffect( %datablockName );
         case "t2dShape3D":
            $selectedShape3D = %datablockName;
            levelEditor3DShapeTool.setShape( %datablockName );
         case "SceneObject":
         case "TextObject":
            LevelEditorTextObjectTool.setFontDB(defaultFontDB);
         case "Trigger":
         case "t2dObjectTemplate":
            LevelBuilder::loadObjectTemplate( %datablockName, %this.getWorldPoint(Vector2Sub(%position, %this.getGlobalPosition())) );
            return;
      }
   
      // Set the proper tool for this object.
      LevelBuilderToolManager::setCreateTool( %toolType );
      
      ToolManager.getActiveTool().createObject(%this, %this.getWorldPoint(Vector2Sub(%position, %this.getGlobalPosition())));
   }
}
