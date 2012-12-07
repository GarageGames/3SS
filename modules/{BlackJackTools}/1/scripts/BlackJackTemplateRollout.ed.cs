//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
$LBTemplateProjectContent = GuiFormManager::AddFormContent("LevelBuilderBlackJackTemplate", "BlackJack Template Rollout", "LBBlackJackTemplateRollout::CreateForm", "LBBlackJackTemplateRollout::SaveForm", 2);
$LBTemplateCommonContent = GuiFormManager::AddFormContent("LevelBuilderBlackJackTemplate", "Common Template Rollout", "LBCommonTemplateRollout::CreateForm", "LBCommonTemplateRollout::SaveForm", 2);

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBBlackJackTemplateRollout::CreateForm(%formCtrl)
{   
   //Project manager base container rollout.
   //%base = new GuiRolloutCtrl() 
   //{
      //canSaveDynamicFields = "0";
      //Profile = "EditorRolloutProfile";
      //HorizSizing = "width";
      //VertSizing = "height";
      //Position = "0 0";
      //Extent = "400 500";
      //MinExtent = "8 2";
      //canSave = "1";
      //Visible = "1";
      //hovertime = "1000";
      //Caption = "BlackJack Template Tools";
      //Margin = "6 4";
      //DragSizable = true;
      //DefaultHeight = "260";
   //};
   
   //Project manager scroll container.
   %base = new GuiScrollCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorTransparentScrollProfile";
      HorizSizing = "width";
      VertSizing = "bottom";
      position = "0 0";
      Extent = "400 500";
      MinExtent = "72 260";
      canSave = "1";
      visible = "1";
      hovertime = "1000";
      willFirstRespond = "1";
      hScrollBar = "alwaysOff";
      vScrollBar = "alwaysOn";
      constantThumbHeight = "0";
      childMargin = "0 0";
   };
   
   //%base.add(%scroll);
   
   // Load the BlackJack Template Rollout Contents
   exec("^{BlackjackTools}/gui/BlackJackTemplateRollout.ed.gui");
   
   if (isObject(BlackJackRolloutContents))
      %base.add(BlackJackRolloutContents);
   
   %formCtrl.add(%base);

   // Specify Message Control (Override getObject(0) on new Content which is default message control)

   // Load Gui Profiles   
   // PROFILES TODO exec("^{BlackjackTools}/gui/profiles.ed.cs");
      
   // Load BlackJack Template Tool GUIs
   exec("^{BlackjackTools}/gui/generalSettingsEditor.ed.gui");
   exec("^{BlackjackTools}/gui/tableEditor.ed.gui");
   exec("^{BlackjackTools}/gui/aiEditor.ed.gui");
   exec("^{BlackjackTools}/gui/aiCardEditor.ed.gui");
   exec("^{BlackjackTools}/gui/customDeckTool.ed.gui");
   exec("^{BlackjackTools}/gui/interfaceEditor.ed.gui");
   exec("^{BlackjackTools}/gui/cardGeneration.ed.gui");
   exec("^{BlackjackTools}/gui/MessageDialog.ed.gui");

   //*** Return back the base control to indicate we were successful
   return %base;
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBBlackJackTemplateRollout::SaveForm(%formCtrl)
{
   // Nothing.
}

//-----------------------------------------------------------------------------
// Form Content Functionality
//-----------------------------------------------------------------------------
