//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$LevelBuilder::Version = "Alpha";
$InTemplateMode = true;

function toggleUIMode()
{
   if($InTemplateMode)
      LoadStandardUI();
   else
      LoadTemplateUI();
   
   $InTemplateMode = !$InTemplateMode;
}

function LoadTemplateUI()
{
   $AdvancedMode = false;
   
   if (isObject(LevelEditorSelectionTool))
   {
      ToolManager.removeTool(LevelEditorSelectionTool);
      LevelEditorSelectionTool.delete();
   }
      
   levelEditorMap.pop();
         
   // Set Creator Panel Content
   %contentObj = GuiFormManager::FindFormContent( "LevelBuilder", "TemplateSideBar" );
   
   if( %contentObj == 0 )
      GuiFormClass::ClearControlContent( sideBarContentContainer, "LevelBuilder" );
   else
      GuiFormClass::SetControlContent( sideBarContentContainer, "LevelBuilder", %contentObj );     
}

function initializeLevelEditor()
{   
   // Load Preferences.
   exec("./preferences/defaultPrefs.ed.cs");
   execPrefs("levelEditorPrefs.cs");   

   // Create Form Content Library.
   GuiFormManager::RegisterLibrary("LevelBuilder", "levelBuilder/layouts" );
   GuiFormManager::RegisterLibrary("LevelBuilderToolProperties", "levelBuilder/layouts" );
   GuiFormManager::RegisterLibrary("LevelBuilderSidebar", "levelBuilder/layouts" );
   GuiFormManager::RegisterLibrary("LevelBuilderTemplateSidebar", "levelBuilder/layouts" );
   GuiFormManager::RegisterLibrary("LevelBuilderSidebarEdit", "levelBuilder/layouts" );
   GuiFormManager::RegisterLibrary("LevelBuilderSidebarCreate", "levelBuilder/layouts" );
   GuiFormManager::RegisterLibrary("LevelBuilderQuickEditClasses" );

   // Make Sure Default Layout always exists.
   exec("./data/layouts/Default.cs");
   
   // Load Up Available Layouts.
   GuiFormManager::InitLayouts( "LevelBuilder" );

   // Form Contents.
   loadDirectory( expandPath("./scripts") , "ed.cs", "edso" );
   loadDirectory( expandPath("./scripts") , "ed.gui", "edso" );

    // MICHTODO - This was where we loaded the old template tools and scripts. 
    // This process should be replaced elsewhere
   
   // Load guis.
   exec("./gui/LevelBuilderBase.ed.gui");
   exec("./gui/options.ed.gui");
   exec("./gui/TGBInsider.ed.gui");
   exec("./gui/TGBInsider.ed.cs");
   exec("./gui/LevelDatablocksBuilder.ed.gui");    //level datablock editor
   
   exec("./gui/playPlatformDlg.ed.cs");
   exec("./gui/playPlatformDlg.gui");

   OptionsTabBook.add(LevelEditorOptionsPage);
   OptionsTabBook.selectPage(0);
 
   // Create Tool Manager.
   if (!isObject(ToolManager))
   {
      new LevelBuilderSceneEdit( ToolManager );
      LevelBuilderToolManager::initializeTools( ToolManager );
   }
   
   // Build LevelBuilder Menu
   new BehaviorComponent(LevelBuilderMenu);
 
   applyLevelEdOptions();

   // Activate Default Layout
   if( GuiFormManager::FindLayout( "LevelBuilder", "User" ) )
      GuiFormManager::ActivateLayout( "LevelBuilder", "User", levelBuilderViewContainer );
   else
      GuiFormManager::ActivateLayout( "LevelBuilder", "Default", levelBuilderViewContainer );
   
   // Set Creator Panel Content
   %contentObj = GuiFormManager::FindFormContent( "LevelBuilder", "SideBar" );
   
   if( %contentObj == 0 )
      GuiFormClass::ClearControlContent( sideBarContentContainer, "LevelBuilder" );
   else
      GuiFormClass::SetControlContent( sideBarContentContainer, "LevelBuilder", %contentObj );
      
   // Level editor hotkey.
   GlobalActionMap.bindCmd(keyboard, "f9", "toggleLevelEditor();", "");
   
   // Show Start Page
   //Canvas.setContent(EditorShellGui);
   Canvas.setContent( TGBStartPage );
   //Canvas.setContent( MainStartGui );
}

function destroyLevelEditor()
{
  
   // Export Preferences.
   echo("Exporting Scene Editor preferences.");
   $Game::CompanyName = "GarageGames";
   $Game::ProductName = "3StepStudioAlpha";   
   export("$levelEditor::*", "levelEditorPrefs.cs", false, false);
   
}

// --------------------------------------------------------------------
// Show Scene Editor.
// --------------------------------------------------------------------

$LevelEditorWindow::lockMask = -1;
$LevelEditorWindow::hideMask = -1;
$LevelBuilder::lastCameraPosition = "";
$LevelBuilder::lastCameraSize = "";
$LevelBuilder::lastCameraZoom = "";

function showMainEditor()
{  
   Canvas.setContent(LevelBuilderBase);
      
   applyLevelEdOptions();
   //levelEditorMap.push();
   
   // Set Scene-Editor State.
   $LevelEditorActive = true;
     
   LevelBuilderToolManager::setTool(LevelEditorSelectionTool);
     
   %sceneWindow = ToolManager.getLastWindow();
   
   if( !isObject( %sceneWindow ) )
      return;
      
   %scene = %sceneWindow.getScene();
   %cameraPosition = "0 0";
   %cameraSize = $levelEditor::DefaultCameraSize;
   %cameraZoom = 0.8;
   if (%scene.cameraPosition)
      %cameraPosition = %scene.cameraPosition;
   if (%scene.cameraSize)
      %cameraSize = %scene.cameraSize;
   if( $LevelBuilder::lastCameraPosition !$= "" )
      %cameraPosition = $LevelBuilder::lastCameraPosition;
   if( $LevelBuilder::lastCameraSize !$= "" )
      %cameraSize = $LevelBuilder::lastCameraSize;
   if( $LevelBuilder::lastCameraZoom !$= "" )
      %cameraZoom = $LevelBuilder::lastCameraZoom;
      
   %sceneWindow.setCurrentCameraPosition(%cameraPosition, %cameraSize);
   %sceneWindow.setCurrentCameraZoom(%cameraZoom);
   
   %sceneWindow.setLayerMask( $LevelEditorWindow::lockMask );
   %sceneWindow.setRenderMasks( $LevelEditorWindow::hideMask );
   
   GuiFormManager::SendContentMessage( $LBQuickEdit, 0, "inspectUpdate" );
}

// --------------------------------------------------------------------
// Hide Scene Editor.
// --------------------------------------------------------------------
function hideLevelEditor()
{
   if(! isObject(mainScreenGui))
      return;
      
   %window = ToolManager.getLastWindow();
   $LevelEditorWindow::lockMask = %window.getLayerMask();
   $LevelEditorWindow::hideMask = %window.getRenderLayerMask();
        
   // Set Scene-Editor State.
   $LevelEditorActive = false;

   levelEditorMap.pop();
}

// --------------------------------------------------------------------
// Toggle Scene Editor.
// --------------------------------------------------------------------
$LevelEditorActive = false;
function toggleLevelEditor()
{
   // Toggle Scene-Editor.
   if ($LevelEditorActive)
      hideLevelEditor();
   else
      showMainEditor();
}




//--------------------------------
// Template Tool Dialog Callbacks
//--------------------------------
function TemplateTool::onDialogPush(%this)
{
   // Disable "Open Level" and "Publish" items in File Menu
   FileMenu.enableItem(1, false);
   FileMenu.enableItem(5, false);   
   
   %index = 0;

   while (ToolsMenu.item[%index] !$= "")
   {
      ToolsMenu.enableItem(%index, false);
      %index++;
   }
}

function TemplateTool::onDialogPop(%this)
{
   // Check if any other Template Tools are still open before re-enabling
   // the menu items
   %numPushedTemplateTools = 0;   
   
   for (%i = 0; %i < Canvas.getCount(); %i++)
   {
      if (Canvas.getObject(%i).getSuperClassNamespace() $= "TemplateTool")
      {
         %numPushedTemplateTools++;
         
         if (%numPushedTemplateTools > 1)
            return;
      }
   }
   
   %index = 0;

   while (ToolsMenu.item[%index] !$= "")
   {
      ToolsMenu.enableItem(%index, true);
      %index++;
   }
   
   // Enable "Open Level" and "Publish" items in File Menu
   FileMenu.enableItem(1, true);
   FileMenu.enableItem(5, true);
}