//--------------------------------
// Projectile Tool Help
//--------------------------------
/// <summary>
/// This function opens a browser and navigates to the Tower Defense Template 
/// Projectile Tool help page.
/// </summary>
function ProjectileToolHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/towerdefense/projectile/");
}



//--------------------------------
// Projectile Globals
//--------------------------------

$ProjectileToolInitialized = false;

$ProjectilePreviewDefaultExtent = "256 256";

$SelectedProjectile = "";
$ProjectileToSelect = "";

$ProjectileAttackMin = 0;
$ProjectileAttackMax = 1000;

$SelectedProjectileEffect = "";

$ProjectileEffectPiercingDamageFalloffPercentDefault = 25;
$ProjectileEffectPiercingDamageFalloffPercentMin = 0;
$ProjectileEffectPiercingDamageFalloffPercentMax = 99;

$ProjectileEffectPoisonDurationDefault = 3;
$ProjectileEffectPoisonDurationMin = 1;
$ProjectileEffectPoisonDurationMax = 60;
$ProjectileEffectPoisonDPSDefault = 5;
$ProjectileEffectPoisonDPSMin = 0;
$ProjectileEffectPoisonDPSMax = 1000;

$ProjectileEffectSlowPercentDefault = 50;
$ProjectileEffectSlowPercentMin = 1;
$ProjectileEffectSlowPercentMax = 99;

$ProjectileEffectSlowDurationDefault = 3;
$ProjectileEffectSlowDurationMin = 1;
$ProjectileEffectSlowDurationMax = 60;

$ProjectileEffectSplashDamageDefault = 10;
$ProjectileEffectSplashDamageMin = 1;
$ProjectileEffectSplashDamageMax = 100;
$ProjectileEffectSplashRadiusDefault = 2;

$ProjectilePreviewZoomed = false;

$ProjectileSpriteRadioSelection = "";


//--------------------------------
// Projectile Tool
//--------------------------------

/// <summary>
/// This function initializes the tool on wake.
/// </summary>
function ProjectileTool::onWake(%this)
{
   if (!$ProjectileToolInitialized)
      %this.initialize();

   if ($SelectedProjectile !$= "")
      RefreshProjectileData();

    if (ProjectilePreview.sprite.getClassName() !$= "t2dAnimatedSprite")
        ProjectilePreviewPlayButton.Visible = false;

   if (!ProjectilePreview.sprite.paused)
    ProjectilePreviewPlayButton.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
}

/// <summary>
/// This function performs one-time-only initialization tasks.
/// </summary>
function ProjectileTool::initialize(%this)
{
   ProjectileSpriteDisplay.setActive(false);      
   
   ProjectileSelectedDropdown.refresh();
   ProjectileSelectedDropdown.setFirstSelected();
   
   ProjectileEffectDropdown.initialize();

   ProjectileEffectSplashRadiusDropdown.initialize();   
      
   
   $ProjectileToolInitialized = true;
}

/// <summary>
/// This function handles changing away from this tool window.
/// </summary>
function ProjectileTool::onDialogPop(%this)
{
   TemplateTool::onDialogPop();
   
   SetProjectileToolDirtyState(false);
   
   if ($TowerToolLaunchedProjectileTool)
   {
      ProjectileRemoveButton.setActive(true);
      ProjectileAddButton.setActive(true);
      ProjectileSelectedDropdown.setActive(true);
      $Tools::TemplateToolDirty = $TowerToolDirtyBeforeEditingProjectile;
      TowerProjectileDropdown.initialize();
      TowerProjectileDropdown.refresh();
   }
}


//--------------------------------
// Projectile Selection
//--------------------------------

/// <summary>
/// This function refreshes the contents of the ProjectileSelectedDropdown.
/// </summary>
function ProjectileSelectedDropdown::refresh(%this)
{
   %this.clear();
   
   %count = $persistentObjectSet.getCount();
   
   %index = 0;

   for (%i = 0; %i < %count; %i++)
   {
      %object = $persistentObjectSet.getObject(%i);
      
      if (%object.Type !$= "Projectile")
         continue;

      %this.add(%object.getName().getInternalName(), %index++);
   }
   
   %this.sort();
}

/// <summary>
/// This function handles selecting another projectile to edit.  It first checks 
/// for changes to the currently selected projectile and prompts to save if appropriate.
/// It then requests the data for the new projectile.
/// </summary>
function ProjectileSelectedDropdown::onSelect(%this)
{
   if ($SelectedProjectile && ($SelectedProjectile.getInternalName() !$= %this.getText()))
      ValidateProjectileInfo();
      
   if ($Tools::TemplateToolDirty && !ConfirmDialog.isAwake() && ($SelectedProjectile.getInternalName() !$= %this.getText()))
   {
      $ProjectileToSelect = $persistentObjectSet.findObjectByInternalName(ProjectileSelectedDropdown.getText(), true);
      
      ConfirmDialog.setupAndShow(ProjectileToolWindow.getGlobalCenter(), "Save Projectile Changes?", 
         "Save", "if (SaveProjectileData()) { ProjectileSelectedDropdown.setSelected(ProjectileSelectedDropdown.findText($ProjectileToSelect.getInternalName())); }", 
         "Don't Save", "SetProjectileToolDirtyState(false); ReactToProjectileSelection();", 
         "Cancel", "ProjectileSelectedDropdown.setSelected(ProjectileSelectedDropdown.findText($SelectedProjectile.getInternalName()));");
   }
   else
      ReactToProjectileSelection();
}

/// <summary>
/// This function gets the newly selected projectile name and calls to refresh the 
/// projectile data.
/// </summary>
function ReactToProjectileSelection()
{
   $SelectedProjectile = $persistentObjectSet.findObjectByInternalName(ProjectileSelectedDropdown.getText(), true);
   
   if (!$Tools::TemplateToolDirty)
      RefreshProjectileData();
}


//--------------------------------
// Projectile Addition
//--------------------------------
/// <summary>
/// This function handles the Add Projectile button.
/// </summary>
function ProjectileAddButton::onClick(%this)
{
   if ($Tools::TemplateToolDirty)
   {
      ConfirmDialog.setupAndShow(ProjectileToolWindow.getGlobalCenter(), "Save Projectile Changes?", 
            "Save", "if (SaveProjectileData()) { AddProjectile(); }", 
            "Don't Save", "SetProjectileToolDirtyState(false); AddProjectile();", 
            "Cancel", "");
   }
   else
      AddProjectile();
}

/// <summary>
/// This function creates a new projectile from a persistent template object.
/// It then refreshes the projectile display.
/// </summary>
function AddProjectile()
{
   if (!ProjectileSelectedDropdown.size())
      ToggleProjectileControlsActiveState(true);   
      
   %name = "Projectile0";
   %number = 0;
   
   %count = $persistentObjectSet.getCount();
   
   %index = 0;

   for (%i = 0; %i < %count; %i++)
   {
      %object = $persistentObjectSet.getObject(%i);
      
      if (%object.Type !$= "Projectile")
         continue;

      if (%object.getName() $= %name)
      {
         %number++;
         %name = "Projectile" @ %number;
         %i = 0;
      }
   }
   
   %newProjectile = DefaultProjectile.clone();
   
   %newProjectile.setName(%name);
   
   %internalName = "NewProjectile" @ %number;
   
   %newProjectile.setInternalName(%internalName);
   
   %newProjectile.setPosition(DefaultProjectile.getPositionX() + 128, DefaultProjectile.getPositionY());
   
   %newProjectile.setFieldValue("Type", "Projectile");
   
   %animationSet = new ScriptObject(%internalName @ "AnimSet") {
      canSaveDynamicFields = "1";
      PlatformTarget = "UNIVERSAL";
      class = "animationSet";
         NameTags = "4";
   };
   
   LBProjectObj.addAnimationSet(%animationSet.getName(), true);
   
   %newProjectile.AnimationSet = %animationSet.getName();
   
   $persistentObjectSet.add(%newProjectile);
   
   LBProjectObj.saveLevel();
   
   ProjectileSelectedDropdown.refresh();
   
   $SelectedProjectile = "";
   
   ProjectileSelectedDropdown.setSelected(ProjectileSelectedDropdown.findText(%newProjectile.getInternalName()));
}


//--------------------------------
// Projectile Removal
//--------------------------------
/// <summary>
/// This function confirms and then requests removal of the currently selected projectile.
/// </summary>
function ProjectileRemoveButton::onClick(%this)
{
   ConfirmDialog.setupAndShow(ProjectileToolWindow.getGlobalCenter(), "Remove Projectile?", 
      "Delete", "DeleteSelectedProjectile();", "", "", "Cancel", "");
}


//--------------------------------
// Projectile Name
//--------------------------------

/// <summary>
/// This function refreshes the contents of the ProjectileNameEditBox from the 
/// currently selected projectile's internal name field.
/// </summary>
function ProjectileNameEditBox::refresh(%this)
{
   %this.text = $SelectedProjectile.getInternalName();
}

/// <summary>
/// This function validates any change to the ProjectileNameEditBox.
/// </summary>
function ProjectileNameEditBox::onValidate(%this)
{
   if (%this.getText() !$= $SelectedProjectile.getInternalName())
      SetProjectileToolDirtyState(true);
}

/// <summary>
/// This function clears the contents of the ProjectileNameEditBox.
/// </summary>
function ProjectileNameClearButton::onClick(%this)
{
   ProjectileNameEditBox.text = "";
   SetProjectileToolDirtyState(true);
}


//--------------------------------
// Projectile Attack
//--------------------------------

/// <summary>
/// This function refreshes the value in the ProjectileAttackEditBox from the
/// currently selected projectile.
/// </summary>
function ProjectileAttackEditBox::refresh(%this)
{
   %this.text = $SelectedProjectile.callOnBehaviors(getAttack);
}

/// <summary>
/// This function validates changes to the contents of the ProjectileAttackEditBox.
/// </summary>
function ProjectileAttackEditBox::onValidate(%this)
{
   if ($SelectedProjectile $= "")
      return;
      
   if (ProjectileAttackEditBox.getValue() < $ProjectileAttackMin)
      ProjectileAttackEditBox.setValue($ProjectileAttackMin);
   else if (ProjectileAttackEditBox.getValue() > $ProjectileAttackMax)
      ProjectileAttackEditBox.setValue($ProjectileAttackMax);
      
   if (%this.getText() !$= $SelectedProjectile.callOnBehaviors(getAttack))
      SetProjectileToolDirtyState(true);
}

/// <summary>
/// This function increments the ProjectileAttackEditBox value.
/// </summary>
function ProjectileAttackSpinUp::onClick(%this)
{
   if (ProjectileAttackEditBox.getValue() < $ProjectileAttackMax)
   {
      ProjectileAttackEditBox.setValue(ProjectileAttackEditBox.getValue() + 1);
      SetProjectileToolDirtyState(true);
   }
}

/// <summary>
/// This function decrements the ProjectileAttackEditBox value.
/// </summary>
function ProjectileAttackSpinDown::onClick(%this)
{
   if (ProjectileAttackEditBox.getValue() > $ProjectileAttackMin)
   {
      ProjectileAttackEditBox.setValue(ProjectileAttackEditBox.getValue() - 1);
      SetProjectileToolDirtyState(true);
   }
}


//--------------------------------
// Projectile Effects
//--------------------------------

/// <summary>
/// This function initializes the contents of the ProjectileEffectDropdown.
/// </summary>
function ProjectileEffectDropdown::initialize(%this)
{
   %this.add("None", 0);
   
   %this.add("Piercing", 1);
   %this.add("Poison", 2);
   %this.add("Slow", 3);
   %this.add("Splash", 4);
   
   %this.setSelected(0);
}

/// <summary>
/// This function refreshes the contents of the ProjectileEffectDropdown and 
/// displays the selected effect's controls.
/// </summary>
function ProjectileEffectDropdown::refresh(%this)
{
   %effect = "None";
   
   %behaviorCount = $SelectedProjectile.getBehaviorCount();
   
   for (%i = 0; %i < %behaviorCount; %i++)
   {
      %behaviorTemplate = $SelectedProjectile.getBehaviorByIndex(%i).template;
      
      if (%behaviorTemplate.behaviorType $= "AttackEffect")
      {
         %effect = %behaviorTemplate.friendlyName;
         break;
      }
   }
   
   %this.setSelected(%this.findText(%effect), false);
   
   ReactToProjectileEffectSelection();
}

/// <summary>
/// This function handles changing the selected effect.
/// </summary>
function ProjectileEffectDropdown::onSelect(%this)
{
   if (($SelectedProjectileEffect !$= "") && ($SelectedProjectileEffect !$= %this.getText()))
      SetProjectileToolDirtyState(true);
      
   ReactToProjectileEffectSelection();
}

/// <summary>
/// This function switches the effect control set based on the selected projectile effect.
/// </summary>
function ReactToProjectileEffectSelection()
{
   switch$ ($SelectedProjectileEffect)
   {
      case "Piercing":
         ProjectileEffectPiercingContainer.setVisible(false);
      case "Poison":
         ProjectileEffectPoisonContainer.setVisible(false);
      case "Slow":
         ProjectileEffectSlowContainer.setVisible(false);
      case "Splash":
         ProjectileEffectSplashContainer.setVisible(false);
   }
      
   $SelectedProjectileEffect = ProjectileEffectDropdown.getText();
      
   switch$ ($SelectedProjectileEffect)
   {
      case "Piercing":
         ProjectileEffectPiercingDamageFalloffPercentEditBox.refresh();
         ProjectileEffectPiercingContainer.setVisible(true);
      case "Poison":
         ProjectileEffectPoisonDurationEditBox.refresh();
         ProjectileEffectPoisonDPSEditBox.refresh();
         ProjectileEffectPoisonContainer.setVisible(true);         
      case "Slow":
         ProjectileEffectSlowPercentEditBox.refresh();
         ProjectileEffectSlowDurationEditBox.refresh();
         ProjectileEffectSlowContainer.setVisible(true);
      case "Splash":
         ProjectileEffectSplashRadiusDropdown.refresh();
         ProjectileEffectSplashDamageEditBox.refresh();
         ProjectileEffectSplashContainer.setVisible(true);
   }
}


//--------------------------------
// Projectile Piercing Effect
//--------------------------------

/// <summary>
/// This function populates the control for the piercing effect damage falloff.
/// </summary>
function ProjectileEffectPiercingDamageFalloffPercentEditBox::refresh(%this)
{
   %this.text = $SelectedProjectile.callOnBehaviors(getEffectDamageFalloff);
   
   if (%this.text $= "ERR_CALL_NOT_HANDLED")
      %this.text = $ProjectileEffectPiercingDamageFalloffPercentDefault;
}

/// <summary>
/// This function validates changes to the piercing effect damage falloff.
/// </summary>
function ProjectileEffectPiercingDamageFalloffPercentEditBox::onValidate(%this)
{
   if (%this.getValue() < $ProjectileEffectPiercingDamageFalloffPercentMin)
      %this.setValue($ProjectileEffectPiercingDamageFalloffPercentMin);
   else if (%this.getValue() > $ProjectileEffectPiercingDamageFalloffPercentMax)
      %this.setValue($ProjectileEffectPiercingDamageFalloffPercentMax);
      
   if (%this.getText() !$= $SelectedProjectile.callOnBehaviors(getEffectDamageFalloff))
      SetProjectileToolDirtyState(true);
}

/// <summary>
/// This function increments the piercing effect damage falloff value.
/// </summary>
function ProjectileEffectPiercingDamageFalloffPercentSpinUp::onClick(%this)
{
   if (ProjectileEffectPiercingDamageFalloffPercentEditBox.getValue() < $ProjectileEffectPiercingDamageFalloffPercentMax)
   {
      ProjectileEffectPiercingDamageFalloffPercentEditBox.setValue(ProjectileEffectPiercingDamageFalloffPercentEditBox.getValue() + 1);
      SetProjectileToolDirtyState(true);
   }
}

/// <summary>
/// This function decrements the piercing effect damage falloff value.
/// </summary>
function ProjectileEffectPiercingDamageFalloffPercentSpinDown::onClick(%this)
{
   if (ProjectileEffectPiercingDamageFalloffPercentEditBox.getValue() > $ProjectileEffectPiercingDamageFalloffPercentMin)
   {
      ProjectileEffectPiercingDamageFalloffPercentEditBox.setValue(ProjectileEffectPiercingDamageFalloffPercentEditBox.getValue() - 1);
      SetProjectileToolDirtyState(true);
   }
}


//--------------------------------
// Projectile Poison Effect
//--------------------------------

/// <summary>
/// This function populates the Poison Effect duration control.
/// </summary>
function ProjectileEffectPoisonDurationEditBox::refresh(%this)
{
   %this.text = $SelectedProjectile.callOnBehaviors(getEffectPoisonDuration);
   
   if (%this.text $= "ERR_CALL_NOT_HANDLED")
      %this.text = $ProjectileEffectPoisonDurationDefault;
}

/// <summary>
/// This function validates changes to the poison duration value.
/// </summary>
function ProjectileEffectPoisonDurationEditBox::onValidate(%this)
{
   if (%this.getValue() < $ProjectileEffectPoisonDurationMin)
      %this.setValue($ProjectileEffectPoisonDurationMin);
   else if (%this.getValue() > $ProjectileEffectPoisonDurationMax)
      %this.setValue($ProjectileEffectPoisonDurationMax);
      
   if (%this.getText() !$= $SelectedProjectile.callOnBehaviors(getEffectPoisonDuration))
      SetProjectileToolDirtyState(true);
}

/// <summary>
/// This function increments the poison duration value.
/// </summary>
function ProjectileEffectPoisonDurationSpinUp::onClick(%this)
{
   if (ProjectileEffectPoisonDurationEditBox.getValue() < $ProjectileEffectPoisonDurationMax)
   {
      ProjectileEffectPoisonDurationEditBox.setValue(ProjectileEffectPoisonDurationEditBox.getValue() + 1);
      SetProjectileToolDirtyState(true);
   }
}

/// <summary>
/// This function decrements the poison duration value.
/// </summary>
function ProjectileEffectPoisonDurationSpinDown::onClick(%this)
{
   if (ProjectileEffectPoisonDurationEditBox.getValue() > $ProjectileEffectPoisonDurationMin)
   {
      ProjectileEffectPoisonDurationEditBox.setValue(ProjectileEffectPoisonDurationEditBox.getValue() - 1);
      SetProjectileToolDirtyState(true);
   }
}

/// <summary>
/// This function populates the Poison Effect damage per second value.
/// </summary>
function ProjectileEffectPoisonDPSEditBox::refresh(%this)
{
   %this.text = $SelectedProjectile.callOnBehaviors(getEffectPoisonDamagePerSecond);
   
   if (%this.text $= "ERR_CALL_NOT_HANDLED")
      %this.text = $ProjectileEffectPoisonDPSDefault;
}

/// <summary>
/// This function validates changes to the poison damage per second value.
/// </summary>
function ProjectileEffectPoisonDPSEditBox::onValidate(%this)
{
   if (%this.getValue() < $ProjectileEffectPoisonDPSMin)
      %this.setValue($ProjectileEffectPoisonDPSMin);
   else if (%this.getValue() > $ProjectileEffectPoisonDPSMax)
      %this.setValue($ProjectileEffectPoisonDPSMax);
      
   if (%this.getText() !$= $SelectedProjectile.callOnBehaviors(getEffectPoisonDamagePerSecond))
      SetProjectileToolDirtyState(true);
}

/// <summary>
/// This function increments the poison damage per second value.
/// </summary>
function ProjectileEffectPoisonDPSSpinUp::onClick(%this)
{
   if (ProjectileEffectPoisonDPSEditBox.getValue() < $ProjectileEffectPoisonDPSMax)
   {
      ProjectileEffectPoisonDPSEditBox.setValue(ProjectileEffectPoisonDPSEditBox.getValue() + 1);
      SetProjectileToolDirtyState(true);
   }
}

/// <summary>
/// This function decrements the poison damage per second value.
/// </summary>
function ProjectileEffectPoisonDPSSpinDown::onClick(%this)
{
   if (ProjectileEffectPoisonDPSEditBox.getValue() > $ProjectileEffectPoisonDPSMin)
   {
      ProjectileEffectPoisonDPSEditBox.setValue(ProjectileEffectPoisonDPSEditBox.getValue() - 1);
      SetProjectileToolDirtyState(true);
   }
}

//--------------------------------
// Projectile Slow Effect
//--------------------------------

/// <summary>
/// This function populates the Slow Effect slow percentage control.
/// </summary>
function ProjectileEffectSlowPercentEditBox::refresh(%this)
{
   %this.text = $SelectedProjectile.callOnBehaviors(getEffectSlowPercent);
   
   if (%this.text $= "ERR_CALL_NOT_HANDLED")
      %this.text = $ProjectileEffectSlowPercentDefault;
}

/// <summary>
/// This function validates changes to the slow percentage value.
/// </summary>
function ProjectileEffectSlowPercentEditBox::onValidate(%this)
{
   if (%this.getValue() < $ProjectileEffectSlowPercentMin)
      %this.setValue($ProjectileEffectSlowPercentMin);
   else if (%this.getValue() > $ProjectileEffectSlowPercentMax)
      %this.setValue($ProjectileEffectSlowPercentMax);
      
   if (%this.getText() !$= $SelectedProjectile.callOnBehaviors(getEffectSlowPercent))
      SetProjectileToolDirtyState(true);
}

/// <summary>
/// This function increments the slow percentage value.
/// </summary>
function ProjectileEffectSlowPercentSpinUp::onClick(%this)
{
   if (ProjectileEffectSlowPercentEditBox.getValue() < $ProjectileEffectSlowPercentMax)
   {
      ProjectileEffectSlowPercentEditBox.setValue(ProjectileEffectSlowPercentEditBox.getValue() + 1);
      SetProjectileToolDirtyState(true);
   }
}

/// <summary>
/// This function decrements the slow percentage value.
/// </summary>
function ProjectileEffectSlowPercentSpinDown::onClick(%this)
{
   if (ProjectileEffectSlowPercentEditBox.getValue() > $ProjectileEffectSlowPercentMin)
   {
      ProjectileEffectSlowPercentEditBox.setValue(ProjectileEffectSlowPercentEditBox.getValue() - 1);
      SetProjectileToolDirtyState(true);
   }
}

/// <summary>
/// This function populates the Slow Effect slow duration control.
/// </summary>
function ProjectileEffectSlowDurationEditBox::refresh(%this)
{
   %this.text = $SelectedProjectile.callOnBehaviors(getEffectSlowDuration);
   
   if (%this.text $= "ERR_CALL_NOT_HANDLED")
      %this.text = $ProjectileEffectSlowDurationDefault;
}

/// <summary>
/// This function validates changes to the slow duration value.
/// </summary>
function ProjectileEffectSlowDurationEditBox::onValidate(%this)
{
   if (%this.getValue() < $ProjectileEffectSlowDurationMin)
      %this.setValue($ProjectileEffectSlowDurationMin);
   else if (%this.getValue() > $ProjectileEffectSlowDurationMax)
      %this.setValue($ProjectileEffectSlowDurationMax);
      
   if (%this.getText() !$= $SelectedProjectile.callOnBehaviors(getEffectSlowDuration))
      SetProjectileToolDirtyState(true);
}

/// <summary>
/// This function increments the slow duration value.
/// </summary>
function ProjectileEffectSlowDurationSpinUp::onClick(%this)
{
   if (ProjectileEffectSlowDurationEditBox.getValue() < $ProjectileEffectSlowDurationMax)
   {
      ProjectileEffectSlowDurationEditBox.setValue(ProjectileEffectSlowDurationEditBox.getValue() + 1);
      SetProjectileToolDirtyState(true);
   }
}

/// <summary>
/// This function decrements the slow duration value.
/// </summary>
function ProjectileEffectSlowDurationSpinDown::onClick(%this)
{
   if (ProjectileEffectSlowDurationEditBox.getValue() > $ProjectileEffectSlowDurationMin)
   {
      ProjectileEffectSlowDurationEditBox.setValue(ProjectileEffectSlowDurationEditBox.getValue() - 1);
      SetProjectileToolDirtyState(true);
   }
}


//--------------------------------
// Projectile Splash Effect
//--------------------------------

/// <summary>
/// This function initializes the Splash Effect splash radius dropdown control.
/// </summary>
function ProjectileEffectSplashRadiusDropdown::initialize(%this)
{
   %this.add("1", 0);
   %this.add("2", 1);
   %this.add("3", 2);
   %this.add("4", 3);
   %this.add("5", 4);
   
   %this.setSelected(2, false);
}

/// <summary>
/// This function populates the Splash Effect splash radius value.
/// </summary>
function ProjectileEffectSplashRadiusDropdown::refresh(%this)
{
    %radius = $SelectedProjectile.callOnBehaviors(getEffectSplashRadius);

    if (%radius $= "ERR_CALL_NOT_HANDLED")
        %radius = $ProjectileEffectSplashRadiusDefault;
        
    %this.setSelected(%this.findText(%radius));
}

/// <summary>
/// This function updates the splash effect radius based on the dropdown selection.
/// </summary>
function ProjectileEffectSplashRadiusDropdown::onSelect(%this)
{
   if (%this.getText() !$= $SelectedProjectile.callOnBehaviors(getEffectSplashRadius))
      SetProjectileToolDirtyState(true);  
}

/// <summary>
/// This function populates the Splash Effect splash damage control.
/// </summary>
function ProjectileEffectSplashDamageEditBox::refresh(%this)
{
   %this.text = $SelectedProjectile.callOnBehaviors(getEffectSplashDamage);
   
   if (%this.text $= "ERR_CALL_NOT_HANDLED")
      %this.text = $ProjectileEffectSplashDamageDefault;
}

/// <summary>
/// This function validates the splash damage value.
/// </summary>
function ProjectileEffectSplashDamageEditBox::onValidate(%this)
{
   if (%this.getValue() < $ProjectileEffectSplashDamageMin)
      %this.setValue($ProjectileEffectSplashDamageMin);
   else if (%this.getValue() > $ProjectileEffectSplashDamageMax)
      %this.setValue($ProjectileEffectSplashDamageMax);
      
   if (%this.getText() !$= $SelectedProjectile.callOnBehaviors(getEffectSplashDamage))
      SetProjectileToolDirtyState(true);
}

/// <summary>
/// This function increments the splash damage value.
/// </summary>
function ProjectileEffectSplashDamageSpinUp::onClick(%this)
{
   if (ProjectileEffectSplashDamageEditBox.getValue() < $ProjectileEffectSplashDamageMax)
   {
      ProjectileEffectSplashDamageEditBox.setValue(ProjectileEffectSplashDamageEditBox.getValue() + 1);
      SetProjectileToolDirtyState(true);
   }
}

/// <summary>
/// This function decrements the splash damage value.
/// </summary>
function ProjectileEffectSplashDamageSpinDown::onClick(%this)
{
   if (ProjectileEffectSplashDamageEditBox.getValue() > $ProjectileEffectSplashDamageMin)
   {
      ProjectileEffectSplashDamageEditBox.setValue(ProjectileEffectSplashDamageEditBox.getValue() - 1);
      SetProjectileToolDirtyState(true);
   }
}


//--------------------------------
// Projectile Preview
//--------------------------------

/// <summary>
/// This function displays the animations for the currently selected projectile.
/// </summary>
function ProjectilePreview::refresh(%this)
{  
   if (ProjectileSpriteDisplay.getText() $= "")
   {
      ProjectilePreview.display("");
      
      return;  
   }
   
   if ($ProjectileSpriteRadioSelection $= "Static")
      %asset = ProjectileSpriteDisplay.getText();
   else
   {
      switch (ProjectilePreviewDropdown.getSelected())
      {
         case 0:
            %asset = ProjectileSpriteDisplay.getText().GetAnimationDatablock("TravelAnim");
         case 1:
            %asset = ProjectileSpriteDisplay.getText().GetAnimationDatablock("HitAnim");
      }
   }
   
   if ($ProjectileSpriteRadioSelection $= "Static")
   {
      ProjectilePreview.display(%asset, "t2dStaticSprite");
      
      ProjectilePreview.sprite.setFrame(ProjectileStaticSpriteFrameEditBox.getValue());
   }
   else
      ProjectilePreview.display(%asset, "t2dAnimatedSprite");
   
   %this.format();
}

/// <summary>
/// This function handles 'zooming' the preview display.
/// </summary>
function ProjectilePreview::format(%this)
{
   %frameSize = "";
   
   if ($ProjectileSpriteRadioSelection $= "Static")
      %frameSize = ProjectilePreview.resource.getFrameSize(0);
   else
      %frameSize = ProjectilePreview.resource.imageMap.getFrameSize(0);
      
   %previewCenter = ProjectilePreview.getCenter();
   
   if ($ProjectilePreviewZoomed)
   {
      if (%frameSize.x < %frameSize.y)
         ProjectilePreview.setExtent($ProjectilePreviewDefaultExtent.x * (%frameSize.x / %frameSize.y), $ProjectilePreviewDefaultExtent.y);
      else if (%frameSize.x > %frameSize.y)
         ProjectilePreview.setExtent($ProjectilePreviewDefaultExtent.x, $ProjectilePreviewDefaultExtent.y * (%frameSize.x / %frameSize.y));
      else
         ProjectilePreview.setExtent($ProjectilePreviewDefaultExtent.x, $ProjectilePreviewDefaultExtent.y);
   }
   else
   {
      if ((%frameSize.x > $ProjectilePreviewDefaultExtent.x) || (%frameSize.y > $ProjectilePreviewDefaultExtent.y))
      {
         if (%frameSize.x >= %frameSize.y)
            ProjectilePreview.setExtent($ProjectilePreviewDefaultExtent.x, %frameSize.y * ($ProjectilePreviewDefaultExtent.x / %frameSize.x));
         else
            ProjectilePreview.setExtent(%frameSize.x * ($ProjectilePreviewDefaultExtent.y / %frameSize.y), $ProjectilePreviewDefaultExtent.y);
      }
      else
         ProjectilePreview.setExtent(%frameSize.x, %frameSize.y);      
   }
   
   ProjectilePreview.setPosition(%previewCenter.x - ProjectilePreview.getExtent().x / 2,
      %previewCenter.y - ProjectilePreview.getExtent().y / 2);
}

/// <summary>
/// This function requests toggling of the preview display's 'zoom' state.
/// </summary>
function ProjectilePreviewZoomButton::onClick(%this)
{
   $ProjectilePreviewZoomed = !$ProjectilePreviewZoomed;
   
   ProjectilePreview.format();
}

/// <summary>
/// This function sets up the preview animation display selection dropdown.
/// </summary>
function ProjectilePreviewDropdown::refresh(%this)
{
   // If projectile is static, reset to that.  If animated, reset to Travel Animation.     
   
   if ($ProjectileSpriteRadioSelection $= "Static")
   {
      %this.clear();
      
      %this.add("Static", 0);
   
      %this.setSelected(0);
   }
   else
   {
      %this.clear();
        
      %this.add("Travel Animation", 0);
      %this.add("Hit Animation", 1);
      
      %this.setSelected(0);
   }
}

/// <summary>
/// This function selects the projectile animation to display.
/// </summary>
function ProjectilePreviewDropdown::onSelect(%this)
{
    ProjectilePreview.refresh();
    ProjectilePreviewPlayButton.onClick();

    switch(%this.getSelected())
    {
        case 0:
            cancel(ProjectilePreviewPlayButton.resetSchedule);
            ProjectilePreviewPlayButton.loopClick();

        case 1:
            ProjectilePreviewPlayButton.playOnceClick();
    }
}


//--------------------------------
// Projectile Sprite
//--------------------------------

/// <summary>
/// This function sets the currently selected projectile to use a static sprite object.
/// </summary>
function ProjectileStaticRadioButton::onClick(%this)
{
   if ($ProjectileSpriteRadioSelection $= "Static")
      return; 
      
   $ProjectileSpriteRadioSelection = "Static";
   
   ToggleProjectileStaticSpriteControls(true);
   
   ProjectileSpriteDisplay.setText($SelectedProjectile.getImageMap());
   
   ProjectilePreviewDropdown.refresh();
   
   ProjectilePreviewPlayButton.Visible = false;
   
   if ($SelectedProjectile.getClassName() $= "t2dAnimatedSprite")
      SetProjectileToolDirtyState(true);
}

/// <summary>
/// This function sets the currently selected projectile to use an animated sprite object.
/// </summary>
function ProjectileAnimatedRadioButton::onClick(%this)
{
   if ($ProjectileSpriteRadioSelection $= "Animated")
      return; 
      
   $ProjectileSpriteRadioSelection = "Animated";
      
   ToggleProjectileStaticSpriteControls(false);
   
   ProjectileSpriteDisplay.setText($SelectedProjectile.AnimationSet);
   
   ProjectilePreviewDropdown.refresh();
   
   ProjectilePreviewPlayButton.Visible = true;
      
   if ($SelectedProjectile.getClassName() $= "t2dStaticSprite")
      SetProjectileToolDirtyState(true);
}

/// <summary>
/// This function updates the display of the sprite type of the projectile.
/// </summary>
function ProjectileSpriteDisplay::refresh(%this)
{
   if ($SelectedProjectile.getClassName() $= "t2dStaticSprite")
      %this.setText($SelectedProjectile.getImageMap());
   else
      %this.setText($SelectedProjectile.animationSet);
}

/// <summary>
/// This function sets the visibility of the static sprite controls.
/// </summary>
/// <param name="visible">Flag to set the visibility of the static sprite tools, true to show, false to hide.</param>
function ToggleProjectileStaticSpriteControls(%visible)
{
   ProjectileStaticSpriteFrameLabel.setVisible(%visible);
   ProjectileStaticSpriteFrameSpinLeft.setVisible(%visible);
   ProjectileStaticSpriteFrameEditBox.setVisible(%visible);
   ProjectileStaticSpriteFrameSpinRight.setVisible(%visible);
   ProjectileSpriteSelectButton.setVisible(%visible);
   ProjectileAnimSetEditButton.setVisible(!%visible);
   
   if (%visible)
      SetProjectileStaticSpriteFrameAvailability((ProjectileSpriteDisplay.getText() !$= "") && (ProjectileSpriteDisplay.getText().getFrameCount() > 1));
   
    ProjectilePreviewPlayButton.Visible = !%visible;
    if (!%visible)
    {
        switch(ProjectilePreviewDropdown.getSelected())
        {
            case 0:
                ProjectilePreviewPlayButton.setBitmap("templates/commonInterface/gui/images/pauseButton.png");

            case 1:
                ProjectilePreviewPlayButton.setBitmap("templates/commonInterface/gui/images/playButton.png");
                
        }
        ProjectilePreview.refresh();
    }
}

/// <summary>
/// This function opens the Asset Library to select a static sprite for the projectile.
/// </summary>
function ProjectileSpriteSelectButton::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage, "");
}

/// <summary>
/// This function assigns the selected asset to the projectile.
/// </summary>
/// <param name="asset">The asset returned from the Asset Library.</param>
function ProjectileSpriteSelectButton::setSelectedAsset(%this, %asset)
{
   if (%asset !$= ProjectileSpriteDisplay.getText())
      SetProjectileToolDirtyState(true);
      
   ProjectileSpriteDisplay.setText(%asset);
   
   SetProjectileStaticSpriteFrameAvailability(ProjectileSpriteDisplay.getText().getFrameCount() > 1);
   
   ProjectilePreview.refresh();
}

/// <summary>
/// This function opens the Projectile Animation Set Tool.
/// </summary>
function ProjectileAnimSetEditButton::onClick(%this)
{
   ProjectileAnimTool.animationSet = $SelectedProjectile.AnimationSet.getId();
   Canvas.pushDialog(ProjectileAnimTool);
}

/// <summary>
/// This function assigns an animation set to the projectile.
/// </summary>
/// <param name="newAnimSetName">The new animation set to assign to the projectile.</param>
function ProjectileUpdateAnimSet(%newAnimSetName)
{
   if (%newAnimSetName !$= ProjectileSpriteDisplay.getText())
   {
      ProjectileSpriteDisplay.setText(%newAnimSetName);
      SaveProjectileData();
   }
   
   ProjectilePreview.refresh();
}

/// <summary>
/// This function populates the static sprite frame display.
/// </summary>
function ProjectileStaticSpriteFrameEditBox::refresh(%this)
{
   if ($SelectedProjectile.getClassName() !$= "t2dStaticSprite")
      return;   
   
   if ($SelectedProjectile.getImageMap().getFrameCount())
      %this.text = $SelectedProjectile.getFrame();
   else
      %this.text = 0;
   
   SetProjectileStaticSpriteFrameAvailability($SelectedProjectile.getImageMap().getFrameCount() > 1);
}

/// <summary>
/// This function makes the frame selection tools available or unavailable based on the flag passed in.
/// </summary>
/// <param name="available">This enables the frame selection buttons if true, or disables them if false.</param>
function SetProjectileStaticSpriteFrameAvailability(%available)
{
   ProjectileStaticSpriteFrameEditBox.setActive(%available);
   ProjectileStaticSpriteFrameEditBox.setProfile(%available ? "GuiTextEditProfile" : "GuiTextEditInactiveProfile");

   if (!%available)
      ProjectileStaticSpriteFrameEditBox.setText(0);
   
   ProjectileStaticSpriteFrameSpinLeft.setActive(%available);
   ProjectileStaticSpriteFrameSpinRight.setActive(%available);
}

/// <summary>
/// This function validates the ProjectileStaticSpriteFrameEditBox value.
/// </summary>
function ProjectileStaticSpriteFrameEditBox::onValidate(%this)
{
   if ($SelectedProjectile $= "")
      return;
      
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if ((ProjectileSpriteDisplay.getText() !$= "") && (%this.getValue() > (ProjectileSpriteDisplay.getText().getFrameCount() - 1)))
      %this.setValue(ProjectileSpriteDisplay.getText().getFrameCount() - 1);
      
   ProjectilePreview.refresh();
   
   if (%this.getText() !$= $SelectedProjectile.getFrame())
      SetProjectileToolDirtyState(true);
}

/// <summary>
/// This function decrements the frame value.
/// </summary>
function ProjectileStaticSpriteFrameSpinLeft::onClick(%this)
{
   if (ProjectileStaticSpriteFrameEditBox.getValue() > 0)
   {
      ProjectileStaticSpriteFrameEditBox.setValue(ProjectileStaticSpriteFrameEditBox.getValue() - 1);
      ProjectilePreview.refresh();
      SetProjectileToolDirtyState(true);
   }
}

/// <summary>
/// This function increments the frame value.
/// </summary>
function ProjectileStaticSpriteFrameSpinRight::onClick(%this)
{
   if (ProjectileStaticSpriteFrameEditBox.getValue() < (ProjectileSpriteDisplay.getText().getFrameCount() - 1))
   {
      ProjectileStaticSpriteFrameEditBox.setValue(ProjectileStaticSpriteFrameEditBox.getValue() + 1);
      ProjectilePreview.refresh();
      SetProjectileToolDirtyState(true);
   }
}


//--------------------------------
// Projectile Save
//--------------------------------

/// <summary>
/// This function requests a save of the projectile data, and sets the selected projectile if 
/// it successfully saves.
/// </summary>
function ProjectileSaveButton::onClick(%this)
{
   if (SaveProjectileData())
      ProjectileSelectedDropdown.setSelected(ProjectileSelectedDropdown.findText($SelectedProjectile.getInternalName()));
}

/// <summary>
/// This function saves the projectile data.
/// </summary>
/// <return>Returns true if successful, false if not.</return>
function SaveProjectileData()
{ 
   if ($SelectedProjectile $= "")
      return false;
      
   if (ProjectileNameEditBox.getText() $= "")
   {
      WarningDialog.setupAndShow(ProjectileToolWindow.getGlobalCenter(), "Invalid Name", "You cannot Save a Projectile without a Name", "", "", "OK");
            
      return false;
   }
   
   // Check for Duplicate Internal Names
   %scene = ToolManager.getLastWindow().getScene();
   
   %sceneObjectCount = %scene.getSceneObjectCount();
   
   for (%i = 0; %i < %sceneObjectCount; %i++)
   {
      %sceneObject = %scene.getSceneObject(%i);
      
      if (%sceneObject == $SelectedProjectile)
         continue;
      
      if (%sceneObject.getInternalName() $= ProjectileNameEditBox.getText())
      {
         WarningDialog.setupAndShow(ProjectileToolWindow.getGlobalCenter(), "Duplicate Name", "Another Object in your Game Already has this Name", "", "", "OK");      
      
         return false;
      }
   }
      
   $SelectedProjectile.setInternalName(ProjectileNameEditBox.getText());
   
   ProjectileAttackEditBox.onValidate();
   $SelectedProjectile.callOnBehaviors(setAttack, ProjectileAttackEditBox.getText());
   
   switch$ ($SelectedProjectileEffect)
   {
      case "Piercing":
         if (!$SelectedProjectile.getBehavior(AppliesPiercingEffectBehavior))
         {
            $SelectedProjectile.addBehavior(AppliesPiercingEffectBehavior.createInstance());
            $SelectedProjectile.callOnBehaviors(setDeleteOnHit, 0);
            $SelectedProjectile.callOnBehaviors(setDeleteOnDealDamage, 0);
            $SelectedProjectile.callOnBehaviors(setHitAllTowerEnemies, 1);
            $SelectedProjectile.callOnBehaviors(setLimitRange, 0);
            $SelectedProjectile.callOnBehaviors(setContinueWithoutTarget, 1);
         }
   
         ProjectileEffectPiercingDamageFalloffPercentEditBox.onValidate();
         $SelectedProjectile.callOnBehaviors(setEffectDamageFalloff, ProjectileEffectPiercingDamageFalloffPercentEditBox.getText());
      case "Poison":
         if (!$SelectedProjectile.getBehavior(AppliesPoisonEffectBehavior))      
            $SelectedProjectile.addBehavior(AppliesPoisonEffectBehavior.createInstance());
         
         ProjectileEffectPoisonDurationEditBox.onValidate();
         $SelectedProjectile.callOnBehaviors(setEffectPoisonDuration, ProjectileEffectPoisonDurationEditBox.getText());
         ProjectileEffectPoisonDPSEditBox.onValidate();
         $SelectedProjectile.callOnBehaviors(setEffectPoisonDamagePerSecond, ProjectileEffectPoisonDPSEditBox.getText());
      case "Slow":
         if (!$SelectedProjectile.getBehavior(AppliesSlowEffectBehavior))      
            $SelectedProjectile.addBehavior(AppliesSlowEffectBehavior.createInstance());
   
         ProjectileEffectSlowPercentEditBox.onValidate();
         $SelectedProjectile.callOnBehaviors(setEffectSlowPercent, ProjectileEffectSlowPercentEditBox.getText());
         
         ProjectileEffectSlowDurationEditBox.onValidate();
         $SelectedProjectile.callOnBehaviors(setEffectSlowDuration, ProjectileEffectSlowDurationEditBox.getText());
      case "Splash":
         if (!$SelectedProjectile.getBehavior(AppliesSplashEffectBehavior))      
            $SelectedProjectile.addBehavior(AppliesSplashEffectBehavior.createInstance());
   
         $SelectedProjectile.callOnBehaviors(setEffectSplashRadius, ProjectileEffectSplashRadiusDropdown.getText());
         
         ProjectileEffectSplashDamageEditBox.onValidate();
         $SelectedProjectile.callOnBehaviors(setEffectSplashDamage, ProjectileEffectSplashDamageEditBox.getText());
   }  
      
   if (($SelectedProjectileEffect !$= "Piercing") && ($SelectedProjectile.getBehavior(AppliesPiercingEffectBehavior)))
   {
      $SelectedProjectile.callOnBehaviors(setContinueWithoutTarget, 0);
      $SelectedProjectile.callOnBehaviors(setLimitRange, 1);
      $SelectedProjectile.callOnBehaviors(setHitAllTowerEnemies, 0);
      $SelectedProjectile.callOnBehaviors(setDeleteOnDealDamage, 1);
      $SelectedProjectile.callOnBehaviors(setDeleteOnHit, 1);
      $SelectedProjectile.removeBehavior($SelectedProjectile.getBehavior(AppliesPiercingEffectBehavior));
   }
   else if (($SelectedProjectileEffect !$= "Poison") && ($SelectedProjectile.getBehavior(AppliesPoisonEffectBehavior)))
      $SelectedProjectile.removeBehavior($SelectedProjectile.getBehavior(AppliesPoisonEffectBehavior));
   else if (($SelectedProjectileEffect !$= "Slow") && ($SelectedProjectile.getBehavior(AppliesSlowEffectBehavior)))
      $SelectedProjectile.removeBehavior($SelectedProjectile.getBehavior(AppliesSlowEffectBehavior));
   else if (($SelectedProjectileEffect !$= "Splash") && ($SelectedProjectile.getBehavior(AppliesSplashEffectBehavior)))
      $SelectedProjectile.removeBehavior($SelectedProjectile.getBehavior(AppliesSplashEffectBehavior));
      
   if ($ProjectileSpriteRadioSelection $= "Static")
   {
      if ($SelectedProjectile.getClassName() $= "t2dAnimatedSprite")
      {
         %projectileInternalName = $SelectedProjectile.getInternalName();
         
         ConvertSpriteToOtherType($SelectedProjectile);
         
         $SelectedProjectile = $persistentObjectSet.findObjectByInternalName(%projectileInternalName);
      }
      
      $SelectedProjectile.setImageMap(ProjectileSpriteDisplay.getText());
      
      ProjectileStaticSpriteFrameEditBox.onValidate();
      $SelectedProjectile.setFrame(ProjectileStaticSpriteFrameEditBox.getValue());
      
      %size = $SelectedProjectile.getImageMap().getFrameSize(0);
      %size = Vector2Scale(%size, $TDMetersPerPixel);
      $SelectedProjectile.setSize(%size);
   }
   else
   {
      if ($SelectedProjectile.getClassName() $= "t2dStaticSprite")
      {
         %projectileInternalName = $SelectedProjectile.getInternalName();
         
         ConvertSpriteToOtherType($SelectedProjectile);
         
         $SelectedProjectile = $persistentObjectSet.findObjectByInternalName(%projectileInternalName);
      }
      
      $SelectedProjectile.animationSet = ProjectileSpriteDisplay.getText();
      $SelectedProjectile.animationName = $SelectedProjectile.animationSet.TravelAnim;
      
      %size = $SelectedProjectile.getAnimation().imageMap.getFrameSize(0);
      %size = Vector2Scale(%size, $TDMetersPerPixel);
      $SelectedProjectile.setSize(%size);
   }
   
   AddAssetToLevelDatablocks(ProjectileSpriteDisplay.getText());
   
   LDEonApply();
   SaveAllLevelDatablocks();
   LBProjectObj.saveLevel();
   
   SetProjectileToolDirtyState(false);
   
   ProjectileSelectedDropdown.refresh();
   ProjectileSelectedDropdown.setSelected(ProjectileSelectedDropdown.findText($SelectedProjectile.getInternalName()), false);
   
   
   return true;
}

/// <summary>
/// This function handles the tool's dirty state to keep track of the need to save data.
/// </summary>
/// <param name="dirty">True if data has been changed, false if not.</param>
function SetProjectileToolDirtyState(%dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      ProjectileToolWindow.setText("Projectile Tool *");
   else
      ProjectileToolWindow.setText("Projectile Tool");
}


//--------------------------------
// Projectile Close
//--------------------------------

/// <summary>
/// This function handles closing the tool, prompting to save changes if needed.
/// </summary>
function ProjectileCloseButton::onClick(%this)
{
   if ($SelectedProjectile !$= "")
      ValidateProjectileInfo();
      
   if ($Tools::TemplateToolDirty)
   {
      ConfirmDialog.setupAndShow(ProjectileToolWindow.getGlobalCenter(), "Save Projectile Changes?", 
            "Save", "if (SaveProjectileData()) { Canvas.popDialog(ProjectileTool); }", 
            "Don't Save", "SetProjectileToolDirtyState(false); Canvas.popDialog(ProjectileTool);", 
            "Cancel", "");
   }
   else
      Canvas.popDialog(ProjectileTool);
}


//--------------------------------
// Projectile Miscellaneous
//--------------------------------

/// <summary>
/// This function refreshes the projectile's display data.
/// </summary>
function RefreshProjectileData()
{
   ProjectileNameEditBox.refresh();
   ProjectileAttackEditBox.refresh();
   ProjectileEffectDropdown.refresh();
   
   if ($SelectedProjectile.getClassName() $= "t2dStaticSprite")
   {
      $ProjectileSpriteRadioSelection = "Static";
      ProjectileStaticRadioButton.setStateOn(true);
      ToggleProjectileStaticSpriteControls(true);
   }
   else
   {
      $ProjectileSpriteRadioSelection = "Animated";
      ProjectileAnimatedRadioButton.setStateOn(true);
      ToggleProjectileStaticSpriteControls(false);
   }
      
   ProjectileSpriteDisplay.refresh();
   
   ProjectilePreviewDropdown.refresh();
   
   ProjectileStaticSpriteFrameEditBox.refresh();
}

/// <summary>
/// This function deletes the selected projectile.
/// </summary>
function DeleteSelectedProjectile()
{
   // Remove deleted projectiles that are attached to current towers
    SetProjectileAnimToolDirtyState(false);   
   
   %count = $persistentObjectSet.getCount();
   
   %index = 0;

   for (%i = 0; %i < %count; %i++)
   {
      %object = $persistentObjectSet.getObject(%i);
      
      if (%object.Type !$= "Tower")
         continue;

      if (%object.callOnBehaviors(getProjectile) $= $SelectedProjectile.getName())
         %object.callOnBehaviors(setProjectile, "None");
   }
   
   $SelectedProjectile.safeDelete();
   
   $persistentObjectSet.remove($SelectedProjectile);
   
   LBProjectObj.saveLevel();
   
   ProjectileSelectedDropdown.refresh();
   
   $SelectedProjectile = "";
   
   ProjectileSelectedDropdown.setFirstSelected();

   if (!ProjectileSelectedDropdown.size())
   {
      ClearProjectileFields();
      ToggleProjectileControlsActiveState(false);
   }
   else
   {
      ToggleProjectileControlsActiveState(true);
   }
   
   SetProjectileToolDirtyState(false);
}

/// <summary>
/// This function clears the projectile data fields in the tool.
/// </summary>
function ClearProjectileFields()
{
   ProjectileNameEditBox.setText("");
   ProjectileAttackEditBox.setText("");
   ProjectileEffectDropdown.setSelected("None");
   ProjectileStaticRadioButton.onClick();
   ProjectileStaticSpriteFrameEditBox.setText("0");
}

/// <summary>
/// This function sets the tool's control active state.
/// </summary>
/// <param name="active">Sets the tool's controls to active if true, inactive if false.</param>
function ToggleProjectileControlsActiveState(%active)
{
   ProjectileSaveButton.setActive(%active);
   ProjectileSelectedDropdown.setActive(%active);
   ProjectileRemoveButton.setActive(%active);
   ProjectileNameEditBox.setActive(%active);
   ProjectileNameClearButton.setActive(%active);
   ProjectileAttackEditBox.setActive(%active);
   ProjectileAttackSpinUp.setActive(%active);
   ProjectileAttackSpinDown.setActive(%active);
   ProjectileEffectDropdown.setActive(%active);
   ProjectilePreviewZoomButton.setActive(%active);
   ProjectilePreviewDropdown.setActive(%active);
   ProjectileStaticRadioButton.setActive(%active);
   ProjectileAnimatedRadioButton.setActive(%active);
   ProjectileSpriteSelectButton.setActive(%active);
   ProjectileAnimSetEditButton.setActive(%active);
   ProjectileStaticSpriteFrameSpinLeft.setActive(%active);
   ProjectileStaticSpriteFrameEditBox.setActive(%active);
   ProjectileStaticSpriteFrameSpinRight.setActive(%active);
   
   ProjectileNameEditBox.setProfile(%active ? "GuiTextEditProfile" : "GuiTextEditInactiveProfile");
   ProjectileAttackEditBox.setProfile(%active ? "GuiTextEditProfile" : "GuiTextEditInactiveProfile");
   ProjectileStaticSpriteFrameEditBox.setProfile(%active ? "GuiTextEditProfile" : "GuiTextEditInactiveProfile");
}

/// <summary>
/// This function handles ensuring that the projectile's data is within acceptable parameters.
/// </summary>
function ValidateProjectileInfo()
{
   ProjectileNameEditBox.onValidate();   
   
   ProjectileAttackEditBox.onValidate();
   
   switch$ ($SelectedProjectileEffect)
   {
      case "Piercing":
         ProjectileEffectPiercingDamageFalloffPercentEditBox.onValidate();
      case "Poison":
         ProjectileEffectPoisonDurationEditBox.onValidate();
      case "Slow":
         ProjectileEffectSlowPercentEditBox.onValidate();
         ProjectileEffectSlowDurationEditBox.onValidate();
      case "Splash":
         ProjectileEffectSplashDamageEditBox.onValidate();
   }
      
   if ($ProjectileSpriteRadioSelection $= "Static")
      ProjectileStaticSpriteFrameEditBox.onValidate();
}

/// <summary>
/// This function toggles the play/pause state of the projectile preview.
/// </summary>
function ProjectilePreviewPlayButton::onClick(%this)
{
    %anim = ProjectilePreview.sprite.getAnimation();
    if (%anim.animationCycle)
    {
        %this.loopClick();
    }
    else
    {
        %this.playOnceClick();
    }
}

/// <summary>
/// This function handles playing/pausing of a looping animation.
/// </summary>
function ProjectilePreviewPlayButton::loopClick(%this)
{
    if (ProjectilePreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        ProjectilePreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        ProjectilePreview.pause();
    }
}

/// <summary>
/// This function handles playing/pausing non-looping animations.
/// </summary>
function ProjectilePreviewPlayButton::playOnceClick(%this)
{
    if (ProjectilePreview.sprite.getIsAnimationFinished())
        ProjectilePreviewDropdown.onSelect();

    if (ProjectilePreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        ProjectilePreview.play();
        %anim = ProjectilePreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = ProjectilePreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        ProjectilePreview.pause();
    }
}

/// <summary>
/// This function handles setting the play/pause button image for non-looping animations.
/// </summary>
/// <param name="imageFile">The image to set the button state to.</param>
function ProjectilePreviewPlayButton::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    if (ProjectilePreview.sprite.getIsAnimationFinished())
        ProjectilePreviewDropdown.onSelect();
    ProjectilePreview.sprite.setAnimationFrame(0);
    ProjectilePreview.pause();
}
