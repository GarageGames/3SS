//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
$LBSelectionToolPropertiesContent = GuiFormManager::AddFormContent( "LevelBuilderToolProperties", "Selection Tool", "LBSelectionToolProperties::CreateForm", "LBSelectionToolProperties::SaveForm", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBSelectionToolProperties::CreateForm( %formCtrl )
{    
   %base = new GuiStackControl() 
   {
      class = "LevelBuilderSelectionToolProperties";
      StackingType = "Horizontal";
      HorizStacking = "Left to Right";
      VertStacking = "Top to Bottom";
      Padding = "2";
      canSaveDynamicFields = "0";
      Profile = "GuiTransparentProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "89 62";
      Extent = "20 35";
      MinExtent = "24 24";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
   };

   //
   // Full Box Selection Option
   //
   %spacer = new GuiSeparatorCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "GuiSeparatorProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "114 0";
      Extent = "12 35";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      type = "Vertical";
      BorderMargin = "3";
      LeftMargin = "0";
      Invisible = "1";
   };
   %base.add(%spacer);
   %fullBoxSelect = new GuiCheckBoxCtrl() 
   {
      canSaveDynamicFields = "0";
      internalName = "fullBoxSelect";
      Profile = "EditorCheckBox";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "114 35";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      command = %base @ ".setFullContainSelect();";
      text = "Full Box Selection";
      groupNum = "-1";
      buttonType = "ToggleButton";
   };  
   %fullBoxSelect.setValue( LevelEditorSelectionTool.getFullContainSelect() );
   %base.add( %fullBoxSelect );


   %spacer = new GuiSeparatorCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "GuiSeparatorProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "149 0";
      Extent = "12 35";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      type = "Vertical";
      BorderMargin = "3";
      LeftMargin = "0";
      Invisible = "1";
   };
   %base.add(%spacer);

   %snapToGrid = new GuiCheckBoxCtrl() 
   {
      canSaveDynamicFields = "0";
      internalName = "snapToGrid";
      Profile = "EditorCheckBox";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "114 35";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      command = %base @ ".setGridSnap();";
      text = "Snap to Grid";
      groupNum = "-1";
      buttonType = "ToggleButton";
   };
   %snapToGrid.setValue( ToolManager.getSnapToGrid() );
   %base.add( %snapToGrid );
   
   %base.updateStack();
   
   %formCtrl.add(%base);

   // Return back the base control to indicate we were successful
   return %base;
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBSelectionToolProperties::SaveForm( %formCtrl )
{
   // Nothing.
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LevelBuilderSelectionToolProperties::onContentMessage( %this, %sender, %message )
{
   %command = GetWord( %message, 0 );
   %value   = GetWord( %message, 1 );

   switch$( %command )
   {
      case "updateToolProperties":
         if (%this.toolProperties)
         {
            schedule(0, 0, "updateToolProperties", %this.parentForm);
         }
      
      case "refreshToolProperties":
         %this.refresh();
   }
}

function LevelBuilderSelectionToolProperties::refresh( %this )
{
   %fullBoxSelect = %this.findObjectByInternalName( "fullBoxSelect" );
   %snapCheck = %this.findObjectByInternalname( "snapToGrid" );
   
   if( isObject( %fullBoxSelect ) )
      %fullBoxSelect.setValue( $levelEditor::FullContainSelection );
      
   if( isObject( %snapCheck ) )
      %snapCheck.setValue( $levelEditor::snapX && $levelEditor::snapY );
}

function LevelBuilderSelectionToolProperties::setFullContainSelect( %this )
{
   %fullBoxSelect = %this.findObjectByInternalName( "fullBoxSelect" );
   if( isObject( %fullBoxSelect ) )
   {
      $levelEditor::FullContainSelection = %fullBoxSelect.getValue();
      applyLevelEdOptions();
   }
}

function LevelBuilderSelectionToolProperties::setGridSnap(%this)
{
   %snapCheck = %this.findObjectByInternalname("snapToGrid");
   if (isObject(%snapCheck))
   {
      $levelEditor::snapX = %snapCheck.getValue();
      $levelEditor::snapY = %snapCheck.getValue();
      applyLevelEdOptions();
   }
}

function LevelBuilderSelectionToolProperties::setDeviceDesign(%this)
{
   %deviceType = %this.findObjectByInternalname("editorDeviceSelection");
   
   if (isObject(%deviceType))
   {
      if(%deviceType.getSelected() == $iOS::constant::iPad)
      {
         echo("Setting editor to iPad mode");
      }
      else
      {
         echo("Setting editor to iPhone mode");
      }
   }
}

function LevelBuilderSelectionTool::getPropertyFormName(%this)
{
   return "Selection Tool Properties";
}
