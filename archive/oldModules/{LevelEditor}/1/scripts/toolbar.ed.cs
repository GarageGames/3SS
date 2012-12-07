//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
//
$LB::ToolBarGroup = "LBToolBarGroup";


if( !isObject( $LB::ToolBarGroup ) )
   new SimGroup( $LB::ToolBarGroup );
   
function LevelBuilderToolManager::onRemove(%this)
{
   %this.destroyTools();
}


function LevelBuilderToolManager::addButtonToBar( %bar, %texture, %command, %toolTip, %addSpacer, %spacerInvisible, %className )
{
   // Create a button for this tool
   %button = new GuiIconButtonCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorButtonToolbar";
      HorizSizing = "right";
      VertSizing = "bottom";
      position = "0 0";
      Extent = "24 24";
      MinExtent = "8 2";
      canSave = "1";
      visible = "1";
      Command = "ToolManager.getLastWindow().setFirstResponder();" SPC %command;
      tooltipprofile = "GuiToolTipProfile";
      ToolTip = %toolTip;
      hovertime = "100";
      class = %className;
      textLocation = "None";
      textMargin = "4";
      buttonMargin = "4 4";     
      groupNum = "1338";
      buttonType = "Button";
      iconBitmap = $LevelBuilder::GuiPath @ "/images/" @ %texture;
   };

   // Add to toolbar
   %bar.add( %button );

   // Link tool to button
   %tool.button = %button;
   
   // Find Properties Spacer
   %propertiesSpacer = %bar.findObjectByInternalName("propertiesSpacer");

   if( isObject( %propertiesSpacer ))
   {
      // Place After Spacer
      %bar.reorderChild( %button, %propertiesSpacer );
   }

   // Return the button.
   if( !%addSpacer )
      return %button;

   // Add Spacer 
   %spacer = new GuiSeparatorCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "GuiSeparatorProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "14 24";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      type = "Vertical";
      Invisible = %spacerInvisible;
      BorderMargin = "3";
      LeftMargin = "0";
   };

   // Add spacer to toolbar
   %bar.add( %spacer );

   // Place Before Button
   %bar.reorderChild( %spacer, %button );

   // Place After Button
   %bar.reorderChild( %button, %spacer );

   // Force Stack to Reposition it's children according to our new order.
   %bar.updateStack();

}


function LevelBuilderToolManager::addToolToBar( %tool, %addSpacer, %spacerInvisible )
{
   if( !isObject( %tool ) || %tool.getToolName() $= "" )
   {
      error("LevelBuilderToolManager::addToolToBar - Invalid Tool or Nameless Tool!");
      return;
   }

   // Create a button for this tool
   %button = new GuiIconButtonCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorButtonToolbar";
      HorizSizing = "right";
      VertSizing = "bottom";
      position = "0 0";
      Extent = "24 24";
      MinExtent = "8 2";
      canSave = "1";
      visible = "1";
      Command = "LevelBuilderToolManager::setTool(" @ %tool.getID() @ ");";
      tooltipprofile = "GuiToolTipProfile";
      ToolTip = %tool.getToolName();
      hovertime = "100";
      text = %tool.getToolName();
      textLocation = "None";
      textMargin = "4";
      buttonMargin = "4 4";     
      groupNum = "1338";
      buttonType = "RadioButton";
      iconBitmap = $LevelBuilder::GuiPath @ "/images/" @ %tool.getToolTexture();
   };

   // Add to toolbar
   LevelBuilderToolBar.add( %button );

   // Link tool to button
   %tool.button = %button;
   
   // Find Properties Spacer
   %propertiesSpacer = LevelBuilderToolBar.findObjectByInternalName("propertiesSpacer");

   if( isObject( %propertiesSpacer ))
   {
      // Place After Spacer
      LevelBuilderToolBar.reorderChild( %button, %propertiesSpacer );
   }

   // Return the button.
   if( !%addSpacer )
      return %button;

   // Add Spacer 
   %spacer = new GuiSeparatorCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "GuiSeparatorProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "12 24";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      type = "Vertical";
      Invisible = %spacerInvisible;
      BorderMargin = "3";
      LeftMargin = "0";
   };

   // Add spacer to toolbar
   LevelBuilderToolBar.add( %spacer );

   // Place Before Button
   LevelBuilderToolBar.reorderChild( %spacer, %button );

   // Place After Button
   LevelBuilderToolBar.reorderChild( %button, %spacer );

   // Force Stack to Reposition it's children according to our new order.
   LevelBuilderToolBar.updateStack();

}

function LevelBuilderToolManager::setToolPropertiesOnBar( %toolID )
{
   // Find Properties Spacer
   %propertiesSpacer = LevelBuilderToolBar.findObjectByInternalName("propertiesSpacer");

   if( !isObject( %propertiesSpacer ))
   {
      error("LevelBuilderToolManager::setToolPropertiesOnBar - Unable to find properties Spacer!");
      return;
   }

   %contentObj = GuiFormManager::FindFormContent( "LevelBuilderToolProperties", %toolID.getToolName() );
   if( %contentObj == 0 )
      GuiFormClass::ClearControlContent( LevelBuilderToolBar, "LevelBuilderToolProperties" );
   else
      GuiFormClass::SetControlContent( LevelBuilderToolBar, "LevelBuilderToolProperties", %contentObj );

}

function LevelBuilderSceneEdit::onToolActivate(%this, %toolObject)
{  
   //LevelBuilderToolManager::setToolPropertiesOnBar( %toolObject );

   if (isObject(%toolObject.button))
      %toolObject.button.setStateOn( true );
      
   if (%toolObject.isMethod("onActivate"))
      %toolObject.onActivate();
}

function LevelBuilderSceneEdit::onToolDeactivate(%this, %tool)
{
   if (%tool.isMemberOfClass("LevelBuilderCreateTool"))
      GuiFormManager::SendContentMessage($LBCreateSiderBar, %this, "clearSelections");
   
   if (%tool.isMethod("onDeactivate"))
      %tool.onDeactivate();
}


function LevelBuilderToolManager::initializeTools(%toolManager)
{
   //---------------------------------------------------------------------------------------------
   // Toolbar Buttons - Utility
   //---------------------------------------------------------------------------------------------
   // New Level
   //LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconNew", "LBPRojectObj.newLevel();",  "Create a New Scene" );
   //// Open Level
   //LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconOpen", "LBPRojectObj.openLevel();",  "Open an Existing Scene" );
   //// Save Level
   //LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconSave", "LBPRojectObj.saveLevel();",  "Save Changes to Current Scene",true);
   //
   ////Level datablocks
   //LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconSortTool", "levelBuilderEditLevelDatablocks(true);",  "Edit level datablocks", true);
   //
   ///*// Build Project Button
   //// MP - Deprecated these outdated, non-functional buttons
   //if($platform $= "macos")
   //{
      //LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconCompile", "openXCodeProject();", "Build Project",true );
   //}
   //else if($platform $= "windows")
   //{
      //LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconCompile2005", "openVS2005Project();", "Open Visual Studio 2005",true );      
      //LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconCompile2008", "openVS2008Project();", "Open Visual Studio 2008",true );      
   //}*/
//
   //// Cut
   //LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconCut", "levelBuilderCut(1);",  "Move Selection to Clipboard" );
   //// Copy
   //LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconCopy", "levelBuilderCopy(1);",  "Copy Selection to Clipboard" );
   //// Paste
   //LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconPaste", "levelBuilderPaste(1);",  "Paste Objects in Clipboard", true );
//
//
   //// Add Undo Button
   //LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconUndo", "levelBuilderUndo(1);",  "Undo Last Change" );
   //// Add Redo Button
   //LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconRedo", "levelBuilderRedo(1);",  "Redo Last Change", true );
//
   //// Add Layer Up Buitton
   //LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconLayerUp", "layerSelectionUp(1);", "Move Selection Up one Layer" );
   //// Add Layer Down Buitton
   //LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconLayerDown", "layerSelectionDown(1);", "Move Selection Down one Layer", true );
//
   //// Add Play Level Buitton
   ////LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconPlay", "runGame();", "Play Game" );
   //LevelBuilderToolManager::addButtonToBar( LevelBuilderToolBar, $LevelBuilder::GuiPath @ "/images/iconPlay", "showPlayPlatform();", "Play Game" );


   //---------------------------------------------------------------------------------------------
   // Tools
   //---------------------------------------------------------------------------------------------
   if (!isObject(LevelEditorSelectionTool))
   {
      %tool = new LevelBuilderSelectionTool(LevelEditorSelectionTool);
      %tool.setToolTexture("iconSelectionTool");
      %toolManager.addTool(LevelEditorSelectionTool, true);
      //LevelBuilderToolManager::addToolToBar( %tool, true, true );
   
      $LB::ToolBarGroup.add( %tool );
      
      // Buttons for display when mousing over a selected object.
      %widget = new SelectionToolWidget() { priority = 5; position = 0; showClasses = false;
                                          callback = "editCollisionPoly";
                                          tooltip = "Edit this objects collision polygon"; };
      %widget.setTexture($LevelBuilder::GuiPath @ "/images/iconPathTool");
      LevelEditorSelectionTool.addWidget(%widget);
      
      $LB::ToolBarGroup.add( %widget );
         
      %widget = new SelectionToolWidget() { priority = 5; position = 0; showClasses = false;
                                          callback = "editSortPoint";
                                          tooltip = "Edit this objects sort point"; };
      %widget.setTexture($LevelBuilder::GuiPath @ "/images/iconSortTool");
      LevelEditorSelectionTool.addWidget(%widget);
      
      $LB::ToolBarGroup.add( %widget );
                                                                                 
      %widget = new SelectionToolWidget() { priority = 5; position = 1; showClasses = false;
                                          callback = "mountObject";
                                          tooltip = "Mount this object to another object"; };
      %widget.setTexture($LevelBuilder::GuiPath @ "/images/iconMount");
      %widget.setDisplayRule("Unmounted");
      %widget.setDisplayRule("Unpathed");
      LevelEditorSelectionTool.addWidget(%widget);
      
      $LB::ToolBarGroup.add( %widget );
                                          
      %widget = new SelectionToolWidget() { priority = 5; position = 1; showClasses = false;
                                          callback = "dismountObject";
                                          tooltip = "Dismount this object from what it's mounted to"; };
      %widget.setTexture($LevelBuilder::GuiPath @ "/images/iconDismount");
      %widget.setDisplayRule("Mounted");
      LevelEditorSelectionTool.addWidget(%widget);
      
      $LB::ToolBarGroup.add( %widget );
                                          
      %widget = new SelectionToolWidget() { priority = 5; position = 1; showClasses = false;
                                          callback = "dismountObject";
                                          tooltip = "Dismount this object from what it's mounted to"; };
      %widget.setTexture($LevelBuilder::GuiPath @ "/images/iconDismount");
      %widget.setDisplayRule("Pathed");
      LevelEditorSelectionTool.addWidget(%widget);
      
      $LB::ToolBarGroup.add( %widget );
                                             
      %widget = new SelectionToolWidget() { priority = 10; position = 1; showClasses = true;
                                          callback = "editPath";
                                          tooltip = "Edit this Path"; };
      %widget.addClass("Path");
      %widget.setTexture($LevelBuilder::GuiPath @ "/images/iconPathTool");
      LevelEditorSelectionTool.addWidget(%widget);
      
      $LB::ToolBarGroup.add( %widget );
                                          
      %widget = new SelectionToolWidget() { priority = 10; position = 1; showClasses = true;
                                          callback = "editImageMap";
                                          tooltip = "Edit this ImageMap"; };
      %widget.addClass("t2dStaticSprite");
      %widget.setTexture($LevelBuilder::GuiPath @ "/images/iconImage");
      LevelEditorSelectionTool.addWidget(%widget);
      
      $LB::ToolBarGroup.add( %widget );
      
      %widget = new SelectionToolWidget() { priority = 10; position = 1; showClasses = true;
                                          callback = "editAnimation";
                                          tooltip = "Edit this Animation"; };                                       
      %widget.addClass("t2dAnimatedSprite");
      %widget.setTexture($LevelBuilder::GuiPath @ "/images/iconViewStoryboard");
      LevelEditorSelectionTool.addWidget(%widget);
      
      $LB::ToolBarGroup.add( %widget );
      
      %widget = new SelectionToolWidget() { priority = 10; position = 1; showClasses = true;
                                          callback = "editTileLayer";
                                          tooltip = "Edit this Tile map"; };                                       
      %widget.addClass("TileLayer");
      %widget.setTexture($LevelBuilder::GuiPath @ "/images/iconImage");
      LevelEditorSelectionTool.addWidget(%widget);
      
      $LB::ToolBarGroup.add( %widget );
      
      %widget = new SelectionToolWidget() { priority = 10; position = 1; showClasses = true;
                                          callback = "editTextObject";
                                          tooltip = "Edit this Text"; };                                       
      %widget.addClass("TextObject");
      %widget.setTexture($LevelBuilder::GuiPath @ "/images/iconTextEditTool");
      LevelEditorSelectionTool.addWidget(%widget);
      
      $LB::ToolBarGroup.add( %widget );
      
      %widget = new SelectionToolWidget() { priority = 10; position = 1; showClasses = true;
                                          callback = "editShapeVector";
                                          tooltip = "Edit this Polygon"; };
      %widget.addClass("ShapeVector");
      %widget.setTexture($LevelBuilder::GuiPath @ "/images/iconShapeVectorTool");
      LevelEditorSelectionTool.addWidget(%widget);
      
      $LB::ToolBarGroup.add( %widget);
   
   }
   
   if( !isObject( LevelEditorTextEditTool ) )
   {
      %tool = new LevelBuilderTextEditTool(LevelEditorTextEditTool);
      %tool.setToolTexture("iconTextEditTool");
      %toolManager.addTool(LevelEditorTextEditTool);
      //LevelBuilderToolManager::addToolToBar( %tool, false, true );
   }
   
   //if (!isObject(LevelEditorPolyTool))
   //{
      //%tool = new LevelBuilderPolyTool(LevelEditorPolyTool);
      //%toolManager.addTool(LevelEditorPolyTool);
      //$LB::ToolBarGroup.add( %tool );
   //}

   if (!isObject(LevelEditorSortPointTool))
   {
      %tool = new LevelBuilderSortPointTool(LevelEditorSortPointTool);
      %toolManager.addTool(LevelEditorSortPointTool);
      $LB::ToolBarGroup.add( %tool );
   }

   if (!isObject(LevelEditorCameraTool))
   {
      %tool = new LevelBuilderCameraTool(LevelEditorCameraTool);
      %toolManager.addTool(LevelEditorCameraTool);
      %tool.setToolTexture("iconCameraTool");
      //LevelBuilderToolManager::addToolToBar( %tool );
      $LB::ToolBarGroup.add( %tool );
   }
   
   if (!isObject(LevelEditorPathEditTool))
   {
      %tool = new LevelBuilderPathEditTool(LevelEditorPathEditTool);
      %toolManager.addTool(LevelEditorPathEditTool);
      $LB::ToolBarGroup.add( %tool );
   }
   
   // Non Visual (No Toolbar Button) Tools.
   if (!isObject(LevelEditorBitmapFontTool))
   {
      %tool = new LevelBuilderBitmapFontTool(LevelEditorBitmapFontTool);
      %toolManager.addTool(LevelEditorBitmapFontTool);
      $LB::ToolBarGroup.add( %tool );
   }
       
   if (!isObject(LevelEditorStaticSpriteTool))
   {
      %tool = new LevelBuilderStaticSpriteTool(LevelEditorStaticSpriteTool);
      %toolManager.addTool(LevelEditorStaticSpriteTool);
      $LB::ToolBarGroup.add( %tool );
   }
    
   if (!isObject(LevelEditorSceneObjectTool))
   {
      %tool = new LevelBuilderSceneObjectTool(LevelEditorSceneObjectTool);
      %toolManager.addTool(LevelEditorSceneObjectTool);
      $LB::ToolBarGroup.add( %tool );
   }
   
   if (!isObject(LevelEditorTextObjectTool))
   {
      %tool = new LevelBuilderTextObjectTool(LevelEditorTextObjectTool);
      %toolManager.addTool(LevelEditorTextObjectTool);
      $LB::ToolBarGroup.add( %tool );
   }
    
   if (!isObject(LevelEditorTriggerTool))
   {
      %tool = new LevelBuilderTriggerTool(LevelEditorTriggerTool);
      %toolManager.addTool(LevelEditorTriggerTool);
      $LB::ToolBarGroup.add( %tool );
   }

   if (!isObject(LevelEditorShapeVectorTool))
   {
      %tool = new LevelBuilderShapeVectorTool(LevelEditorShapeVectorTool);
      %toolManager.addTool(LevelEditorShapeVectorTool);
      $LB::ToolBarGroup.add( %tool );
   }
    
    
   if (!isObject(LevelEditorParticleTool))
   {
      %tool = new LevelBuilderParticleTool(LevelEditorParticleTool);
      %toolManager.addTool(LevelEditorParticleTool);
      $LB::ToolBarGroup.add( %tool );
   }
    
   if (!isObject(LevelEditorTileMapTool))
   {
      %tool = new LevelBuilderTileMapTool(LevelEditorTileMapTool);
      %toolManager.addTool(LevelEditorTileMapTool);
      $LB::ToolBarGroup.add( %tool );
   }
    
   if (!isObject(LevelEditorScrollerTool))
   {
      %tool = new LevelBuilderScrollerTool(LevelEditorScrollerTool);
      %toolManager.addTool(LevelEditorScrollerTool);
      $LB::ToolBarGroup.add( %tool );
   }
    
   if (!isObject(LevelEditorAnimatedSpriteTool))
   {
      %tool = new LevelBuilderAnimatedSpriteTool(LevelEditorAnimatedSpriteTool);
      %toolManager.addTool(LevelEditorAnimatedSpriteTool);
      $LB::ToolBarGroup.add( %tool );
   }    
    
   if (!isObject(LevelEditorPathTool))
   {
      %tool = new LevelBuilderPathTool(LevelEditorPathTool);
      %toolManager.addTool(LevelEditorPathTool);
      
      $LB::ToolBarGroup.add( %tool );
   }
   
   if (!isObject(LevelEditorTileMapEditTool))
   {
      %tool = new LevelBuilderTileMapEditTool(LevelEditorTileMapEditTool);
      %toolManager.addTool(LevelEditorTileMapEditTool);
      LevelEditorTileMapEditTool.setIconBitmap(expandPath("^{TileLayerEditor}/tileicons"));
      
      $LB::ToolBarGroup.add( %tool );
   }     
}

function LevelBuilderToolManager::destroyTools(%toolManager)
{
   if (isObject(LevelEditorSelectionTool))
   {
      %toolManager.removeTool(LevelEditorSelectionTool);
      LevelEditorSelectionTool.delete();
   }
    
   if (isObject(LevelEditorPathEditTool))
   {
      %toolManager.removeTool(LevelEditorPathEditTool);
      LevelEditorPathEditTool.delete();
   }
   
   if (isObject(LevelEditorBitmapFontTool))
   {
      %toolManager.removeTool(LevelEditorBitmapFontTool);
      LevelEditorBitmapFontTool.delete();
   }
   
   if (isObject(LevelEditorStaticSpriteTool))
   {
      %toolManager.removeTool(LevelEditorStaticSpriteTool);
      LevelEditorStaticSpriteTool.delete();
   }
    
   if (isObject(LevelEditorParticleTool))
   {
      %toolManager.removeTool(LevelEditorParticleTool);
      LevelEditorParticleTool.delete();
   }
    
   if (isObject(LevelEditorTileMapTool))
   {
      %toolManager.removeTool(LevelEditorTileMapTool);
      LevelEditorTileMapTool.delete();
   }
    
   if (isObject(LevelEditorTriggerTool))
   {
      %toolManager.removeTool(LevelEditorTriggerTool);
      LevelEditorTriggerTool.delete();
   }
    
   if (isObject(LevelEditorSceneObjectTool))
   {
      %toolManager.removeTool(LevelEditorSceneObjectTool);
      LevelEditorSceneObjectTool.delete();
   }
   
   if (isObject(LevelEditorTextObjectTool))
   {
      %toolManager.removeTool(LevelEditorTextObjectTool);
      LevelEditorTextObjectTool.delete();
   }
    
   if (isObject(LevelEditor3DShapeTool))
   {
      %toolManager.removeTool(LevelEditor3DShapeTool);
      LevelEditor3DShapeTool.delete();
   }
    
   if (isObject(LevelEditorPathTool))
   {
      %toolManager.removeTool(LevelEditorPathTool);
      LevelEditorPathTool.delete();
   }
    
   if (isObject(LevelEditorScrollerTool))
   {
      %toolManager.removeTool(LevelEditorScrollerTool);
      LevelEditorScrollerTool.delete();
   }
    
   if (isObject(LevelEditorAnimatedSpriteTool))
   {
      %toolManager.removeTool(LevelEditorAnimatedSpriteTool);
      LevelEditorAnimatedSpriteTool.delete();
   }

   //if (isObject(LevelEditorPolyTool))
   //{
      //%toolManager.removeTool(LevelEditorPolyTool);
      //LevelEditorPolyTool.delete();
   //}
   
   if (isObject(LevelEditorCameraTool))
   {
      %toolManager.removeTool(LevelEditorCameraTool);
      LevelEditorCameraTool.delete();
   }
}

function LevelBuilderToolManager::setTool( %toolId )
{
   if ( isObject(%toolId) && !ToolManager.setActiveTool(%toolId) && ToolManager.getActiveTool() != 0 )
   {
      if (isObject(ToolManager.getActiveTool().button))
         ToolManager.getActiveTool().button.setStateOn( true );
   }
}

function LevelBuilderToolManager::setCreateTool( %type )
{
   $selectedCreateType = %type;
   switch$ ($selectedCreateType)
   {
      case "BitmapFontObject":
         ToolManager.setActiveTool(LevelEditorBitmapFontTool);
      case "t2dStaticSprite":
         ToolManager.setActiveTool(LevelEditorStaticSpriteTool);
      case "t2dAnimatedSprite":
         ToolManager.setActiveTool(LevelEditorAnimatedSpriteTool);
      case "Scroller":
         ToolManager.setActiveTool(LevelEditorScrollerTool);
      case "TileLayer":
         ToolManager.setActiveTool(LevelEditorTileMapTool);
         LevelEditorTileMapTool.setTileMap(LevelBuilderToolManager::getGlobalTileMap());
      case "ParticleEffect":
         ToolManager.setActiveTool(LevelEditorParticleTool);
      case "t2dShape3D":
         ToolManager.setActiveTool(LevelEditor3DShapeTool);
      case "SceneObject":
         ToolManager.setActiveTool(LevelEditorSceneObjectTool);
      case "TextObject":
         ToolManager.setActiveTool(LevelEditorTextObjectTool);
      case "Trigger":
         ToolManager.setActiveTool(LevelEditorTriggerTool);
      case "ShapeVector":
         ToolManager.setActiveTool(LevelEditorShapeVectorTool);
      case "Path":
         ToolManager.setActiveTool(LevelEditorPathTool);
   }
   if (isObject(ToolManager.getActiveTool().button))
      ToolManager.getActiveTool().button.setStateOn( true );
      
   // Update Class/SuperClass Links
   LevelBuilderToolManager::updateClassLink( ToolManager.classLink );
   LevelBuilderToolManager::updateSuperClassLink( ToolManager.superClassLink );  
}

function LevelBuilderToolManager::getGlobalTileMap()
{
   %scene = ToolManager.getLastWindow().getScene();
   if (isObject(%scene))
      return %scene.getGlobalTileMap();
   
   return 0;
}

//---------------------------------------------------------------------------------------------
// Update link of Class Namespace with Active Tool
//---------------------------------------------------------------------------------------------
function LevelBuilderToolManager::updateClassLink( %newLink )
{
   ToolManager.classLink = %newLink;
   
   %tool = ToolManager.getActiveTool();
   
   // Update Create Tool Link
   if( %tool.isMemberOfClass("LevelBuilderCreateTool") )
      %tool.setClassName( %newLink );
}

//---------------------------------------------------------------------------------------------
// Update link of SuperClass Namespace with Active Tool
//---------------------------------------------------------------------------------------------
function LevelBuilderToolManager::updateSuperClassLink( %newLink )
{
   ToolManager.superClassLink = %newLink;
   
   %tool = ToolManager.getActiveTool();
   
   // Update Create Tool Link
   if( %tool.isMemberOfClass("LevelBuilderCreateTool") )
      %tool.setSuperClassName( %newLink );
}

function LevelBuilderSceneWindow::onEscapeTool( %this )
{
   ToolManager.setActiveTool( LevelEditorSelectionTool );
} 