//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
$LBTemplateProjectContent = GuiFormManager::AddFormContent("LevelBuilderTowerDefenseTemplate", "Tower Defense Template Rollout", "LBTowerDefenseTemplateRollout::CreateForm", "LBTowerDefenseTemplateRollout::SaveForm", 2);
$LBTemplateCommonContent = GuiFormManager::AddFormContent("LevelBuilderTowerDefenseTemplate", "Common Template Rollout", "LBCommonTemplateRollout::CreateForm", "LBCommonTemplateRollout::SaveForm", 2);

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
/// <summary>
/// This function creates the Tower Defense Tool rollout and adds it to the main tool palette.
/// </summary>
/// <param name="formCtrl">The parent form to add the Tower Defense tools to.</param>
function LBTowerDefenseTemplateRollout::CreateForm(%formCtrl)
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
      //Caption = "Tower Defense Template Tools";
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
      hScrollBar = "dynamic";
      vScrollBar = "alwaysOn";
      constantThumbHeight = "0";
      childMargin = "0 0";
   };
   
   //%base.add(%scroll);
   
   // Load the Tower Defense Template Rollout Contents
   exec("../gui/TowerDefenseTemplateRollout.ed.gui");
   
   if (isObject(TowerDefenseRolloutContents))
      %base.add(TowerDefenseRolloutContents);
   
   %formCtrl.add(%base);

   // Specify Message Control (Override getObject(0) on new Content which is default message control)

   // Load Tower Defense Template Tool GUIs
   exec("../gui/GeneralSettings.ed.gui");
   exec("../gui/GuiTool.ed.gui");
   exec("../gui/TerrainTool.ed.gui");
   exec("../gui/EnemyAnimTool.ed.gui");
   exec("../gui/EnemyTool.ed.gui");
   exec("../gui/WaveTool.ed.gui");
   exec("../gui/ProjectileTool.ed.gui");
   exec("../gui/ProjectileAnimTool.ed.gui");
   exec("../gui/TowerTool.ed.gui");
   exec("../gui/TowerAnimTool.ed.gui");
   exec("../gui/LevelTool.ed.gui");
   exec("../gui/CreateAnimationSetTypeDialog.ed.gui");
   exec("../gui/TerrainToolPathDialog.ed.gui");


   //*** Return back the base control to indicate we were successful
   return %base;
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
/// <summary>
/// This function is for saving dynamic form states to the parent form - not used.
/// </summary>
/// <param name="formCtrl">The parent form containing the Tower Defense Tool rollout.</param>
function LBTowerDefenseTemplateRollout::SaveForm(%formCtrl)
{
   // Nothing.
}

//-----------------------------------------------------------------------------
// Form Content Functionality
//-----------------------------------------------------------------------------
