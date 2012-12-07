//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
$LBToolPropertiesContent = GuiFormManager::AddFormContent( "LevelBuilder", "Tool Properties", "LBToolProperties::CreateForm", "LBToolProperties::SaveForm", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBToolProperties::CreateForm( %formCtrl )
{    
   %base = new GuiControl() 
   {
      class = "LevelBuilderToolProperties";
      canSaveDynamicFields = "0";
      Profile = "GuiDefaultProfile";
      HorizSizing = "width";
      VertSizing = "height";
      position = "0 32";
      Extent = "668 543";
      MinExtent = "10 10";
      canSave = "1";
      visible = "1";
      internalName = "LevelBuilderToolProperties";
      tooltipprofile = "GuiDefaultProfile";
      hovertime = "0";
      lockMouse = "0";
   };
   
   %base.parentForm = %formCtrl;
   %base.toolProperties = true;
   %formCtrl.add(%base);

   //*** Now resize as appropriate
   %formctrl.sizeContentsToFit(%base, %formCtrl.contentID.margin);

   //*** Return back the base control to indicate we were successful
   return %base;
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBToolProperties::SaveForm( %formCtrl )
{
   // Nothing.
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LevelBuilderToolProperties::onContentMessage( %this, %sender, %message )
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
   }
}

function updateToolProperties(%form)
{
   %newGui = GuiFormClass::setFormContent(%form, GuiFormManager::FindFormContent("LevelBuilder", ToolManager.getActiveTool().getPropertyFormName()));
   %newGui.parentForm = %form;
   %newGui.toolProperties = true;
}
