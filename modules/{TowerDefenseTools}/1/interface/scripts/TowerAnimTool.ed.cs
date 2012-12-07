//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------



//--------------------------------
// Tower Animation Set Tool Help
//--------------------------------
/// <summary>
/// This function opens a browser and navigates to the Tower Defense Template 
/// Tower Animation Set Tool help page.
/// </summary>
function TAToolHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/towerdefense/toweranimationset/");
}


/// <summary>
/// This function adds an onMouseUp callback handler to the AnimationPreviewWindow that 
/// plays the window's animation.
/// </summary>
function AnimationPreviewWindow::onMouseUp(%this, %modifier, %worldPosition, %clicks)
{
   //echo(" -- " @ %this @ " : AnimationPreviewWindow::onMouseUp()");
   %this.play();
}

//-----------------------------------------------------------------------------
// Tower Animation Tool Globals
//-----------------------------------------------------------------------------
/// <summary>
/// This function handles the tool's dirty state to keep track of the need to save data.
/// </summary>
/// <param name="dirty">True if data has been changed, false if not.</param>
function SetTowerAnimToolDirtyState(%dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      TowerAnimToolWindow.setText("Tower Animation Set Tool *");
   else
      TowerAnimToolWindow.setText("Tower Animation Set Tool");
}

$TAToolInitialized = false;

$TAToolMirroredColor = "1.0 1.0 1.0 0.4";
$TAToolOriginalColor = "1.0 1.0 1.0 1.0";

/// <summary>
/// This function refreshes the contents of the requested mirror dropdown control.
/// </summary>
/// <param name="dropdown">The control to refresh.</param>
function TAToolRefreshMirrorDropdown(%dropdown)
{
      %dropdown.clear();
      %dropdown.add("Diagonal", 0);
      %dropdown.add("Horizontal", 1);
      %dropdown.add("Vertical", 2);
      
      %dropdown.setFirstSelected();
}

/// <summary>
/// This function sets the visibility of the requested dropdown control based on the associated checkbox's value.
/// </summary>
/// <param name="dropdown">The dropdown to set visibility on.</param>
/// <param name="checkbox">The checkbox that will determine the visibility of the associated dropdown control.</param>
function TAToolSetDropdownVisibility(%dropdown, %checkbox)
{
   %dropdown.Visible = %checkbox.getValue();
}

/// <summary>
/// This function is used to set the flip state of the specified sprite object.
/// </summary>
/// <param name="sprite">The sprite to flip.</param>
/// <param name="direction">The desired flip direction, Horizontal, Vertical or Diagonal.</param>
function TAToolSetFlip(%sprite, %direction)
{
   switch$(%direction)
   {
      case "Diagonal":
         %sprite.setFlipX(true);
         %sprite.setFlipY(true);
      
      case "Horizontal":
         %sprite.setFlipX(true);
      
      case "Vertical":
         %sprite.setFlipY(true);
   }
   //echo(" -- Sprite " @ %sprite @ " flip " @ %direction);
}

/// <summary>
/// This function is used to clear the flip state of the desired sprite.
/// </summary>
/// <param name="sprite">The sprite to reset.</param>
function TAToolResetFlip(%sprite)
{
   %sprite.setFlipX(false);
   %sprite.setFlipY(false);
   //echo(" -- Sprite " @ %sprite @ " reset flip");
}

/// <summary>
/// This function is used to set the text of the edit box associated with the passed 
/// animation preview window to the name of the animation currently displayed by the
/// preview passed for the fire animation tab.
/// </summary>
/// <param name="animPreview">The name of the animation preview window that needs its label updated.</param>
function TAToolUpdateEdit(%animPreview)
{
   if (%animPreview $= TAToolNAnimPreview)
      TAToolNAnimEdit.text = fileName(%animPreview.animation);

   if (%animPreview $= TAToolNEAnimPreview)
      TAToolNEAnimEdit.text = fileName(%animPreview.animation);

   if (%animPreview $= TAToolEAnimPreview)
      TAToolEAnimEdit.text = fileName(%animPreview.animation);

   if (%animPreview $= TAToolSEAnimPreview)
      TAToolSEAnimEdit.text = fileName(%animPreview.animation);

   if (%animPreview $= TAToolSAnimPreview)
      TAToolSAnimEdit.text = fileName(%animPreview.animation);

   if (%animPreview $= TAToolSWAnimPreview)
      TAToolSWAnimEdit.text = fileName(%animPreview.animation);

   if (%animPreview $= TAToolWAnimPreview)
      TAToolWAnimEdit.text = fileName(%animPreview.animation);

   if (%animPreview $= TAToolNWAnimPreview)
      TAToolNWAnimEdit.text = fileName(%animPreview.animation);
}

/// <summary>
/// This function is used to set the visibility of the mirror checkbox and associated dropdown in a 
/// mirrored animation preview display and to set the correct animation in the
/// destination preview for the walk animation tab.
/// </summary>
/// <param name="checkbox">The checkbox that is being checked/unchecked.</param>
/// <param name="dropdown">The associated dropdown control.</param>
function TAToolMirrorHide(%checkbox, %dropdown)
{
   %state = !%checkbox.getValue();
   
   // -------------------------------------------------------------------------
   // Flip North to South
   // -------------------------------------------------------------------------
   if (%checkbox == TAToolNMirrorCheckBox.getId())
   {
      TAToolSMirrorCheckBox.setValue(false);
      TAToolSMirrorCheckBox.Visible = %state;
      if (%state == false)
      {
         TAToolSAnimPreview.display(TAToolNAnimPreview.animation);
         TAToolSetFlip(TAToolSAnimPreview.sprite, "Vertical");
         TAToolSAnimPreview.mirrorState = "Vertical";
         TAToolUpdateEdit(TAToolSAnimPreview);
      }
      else
      {
         // temporary code - restores original test animation to the preview window
         TAToolSAnimPreview.display(TAToolSAnimPreview.originalAnim);
         TAToolResetFlip(TAToolSAnimPreview.sprite);
         TAToolSAnimPreview.mirrorState = "";
         TAToolUpdateEdit(TAToolSAnimPreview);
      }
      TAToolSAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
   }

   // -------------------------------------------------------------------------
   // Flip Northeast to South, SE, NW
   // -------------------------------------------------------------------------
   if (%checkbox == TAToolNEMirrorCheckBox.getId())
   {
      if (isObject(%dropdown.hiddenControl))
      {
         %dropdown.hiddenControl.setValue(false);
         %dropdown.hiddenControl.Visible = %state;
      }
      else
      {
         TAToolSWMirrorCheckBox.setValue(false);
         TAToolSWMirrorCheckBox.Visible = %state;
      }
      
      if (isObject(%dropdown.animPreview))
      {
         switch (%state)
         {
            case 0:
               %dropdown.animPreview.sprite.BlendColor = $TAToolMirroredColor;
               %dropdown.animPreview.mirrorState = %dropdown.getValue();
            
            case 1:
               %dropdown.animPreview.sprite.BlendColor = $TAToolOriginalColor;
               TAToolResetFlip(TAToolSWAnimPreview.sprite);
               TAToolSWAnimPreview.display(TAToolSWAnimPreview.originalAnim);
               TAToolSWAnimPreview.mirrorState = "";
               TAToolUpdateEdit(TAToolSWAnimPreview);

               TAToolResetFlip(TAToolSEAnimPreview.sprite);
               TAToolSEAnimPreview.display(TAToolSEAnimPreview.originalAnim);
               TAToolSEAnimPreview.mirrorState = "";
               TAToolUpdateEdit(TAToolSEAnimPreview);

               TAToolResetFlip(TAToolNWAnimPreview.sprite);
               TAToolNWAnimPreview.display(TAToolNWAnimPreview.originalAnim);
               TAToolNWAnimPreview.mirrorState = "";
               TAToolUpdateEdit(TAToolNWAnimPreview);
         }
      }
      else
      {
         TAToolSWAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
      }
   }

   // -------------------------------------------------------------------------
   // Flip East to West
   // -------------------------------------------------------------------------
   if (%checkbox == TAToolEMirrorCheckBox.getId())
   {
      TAToolWMirrorCheckBox.setValue(false);
      TAToolWMirrorCheckBox.Visible = %state;
      if (%state == false)
      {
         TAToolWAnimPreview.display(TAToolEAnimPreview.animation);
         TAToolWAnimPreview.mirrorState = "";
         TAToolSetFlip(TAToolWAnimPreview.sprite, "Horizontal");
         TAToolUpdateEdit(TAToolWAnimPreview);
      }
      else
      {
         // temporary code - restores original test animation to the preview window
         TAToolWAnimPreview.display(TAToolWAnimPreview.originalAnim);
         TAToolResetFlip(TAToolWAnimPreview.sprite);
         TAToolWAnimPreview.mirrorState = "";
         TAToolUpdateEdit(TAToolWAnimPreview);
      }
      TAToolWAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
   }

   // -------------------------------------------------------------------------
   // Flip Southeast to Northwest, NE, SW
   // -------------------------------------------------------------------------
   if (%checkbox == TAToolSEMirrorCheckBox.getId())
   {
      if (isObject(%dropdown.hiddenControl))
      {
         %dropdown.hiddenControl.setValue(false);
         %dropdown.hiddenControl.Visible = %state;
      }
      else
      {
         TAToolNWMirrorCheckBox.setValue(false);
         TAToolNWMirrorCheckBox.Visible = %state;
      }

      if (isObject(%dropdown.animPreview))
      {
         switch (%state)
         {
            case 0:
               %dropdown.animPreview.sprite.BlendColor = $TAToolMirroredColor;
               %dropdown.animPreview.mirrorState = %dropdown.getValue();
            
            case 1:
               %dropdown.animPreview.sprite.BlendColor = $TAToolOriginalColor;
               TAToolResetFlip(TAToolSWAnimPreview.sprite);
               TAToolSWAnimPreview.display(TAToolSWAnimPreview.originalAnim);
               TAToolSWAnimPreview.mirrorState = "";
               TAToolUpdateEdit(TAToolSWAnimPreview);

               TAToolResetFlip(TAToolNEAnimPreview.sprite);
               TAToolNEAnimPreview.display(TAToolNEAnimPreview.originalAnim);
               TAToolNEAnimPreview.mirrorState = "";
               TAToolUpdateEdit(TAToolNEAnimPreview);

               TAToolResetFlip(TAToolNWAnimPreview.sprite);
               TAToolNWAnimPreview.display(TAToolNWAnimPreview.originalAnim);
               TAToolNWAnimPreview.mirrorState = "";
               TAToolUpdateEdit(TAToolNWAnimPreview);
         }
      }
      else
      {
         TAToolNWAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
      }
   }

   // -------------------------------------------------------------------------
   // Flip South to North
   // -------------------------------------------------------------------------
   if (%checkbox == TAToolSMirrorCheckBox.getId())
   {
      TAToolNMirrorCheckBox.setValue(false);
      TAToolNMirrorCheckBox.Visible = %state;
      if (%state == false)
      {
         TAToolNAnimPreview.display(TAToolSAnimPreview.animation);
         TAToolNAnimPreview.mirrorState = "Vertical";
         TAToolSetFlip(TAToolNAnimPreview.sprite, "Vertical");
         TAToolUpdateEdit(TAToolNAnimPreview);
      }
      else
      {
         // temporary code - restores original test animation to the preview window
         TAToolNAnimPreview.display(TAToolNAnimPreview.originalAnim);
         TAToolResetFlip(TAToolNAnimPreview.sprite);
         TAToolNAnimPreview.mirrorState = "";
         TAToolUpdateEdit(TAToolNAnimPreview);
      }
      TAToolNAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
   }

   // -------------------------------------------------------------------------
   // Flip Southwest to Northeast, NW, SE
   // -------------------------------------------------------------------------
   if (%checkbox == TAToolSWMirrorCheckBox.getId())
   {
      if (isObject(%dropdown.hiddenControl))
      {
         %dropdown.hiddenControl.setValue(false);
         %dropdown.hiddenControl.Visible = %state;
      }
      else
      {
         TAToolNEMirrorCheckBox.setValue(false);
         TAToolNEMirrorCheckBox.Visible = %state;
      }
      
      if (isObject(%dropdown.animPreview))
      {
         switch (%state)
         {
            case 0:
               %dropdown.animPreview.sprite.BlendColor = $TAToolMirroredColor;
               %dropdown.animPreview.mirrorState = %dropdown.getValue();
            
            case 1:
               %dropdown.animPreview.sprite.BlendColor = $TAToolOriginalColor;
               TAToolResetFlip(TAToolNEAnimPreview.sprite);
               TAToolNEAnimPreview.display(TAToolNEAnimPreview.originalAnim);
               TAToolNEAnimPreview.mirrorState = "";
               TAToolUpdateEdit(TAToolNEAnimPreview);

               TAToolResetFlip(TAToolSEAnimPreview.sprite);
               TAToolSEAnimPreview.display(TAToolSEAnimPreview.originalAnim);
               TAToolSEAnimPreview.mirrorState = "";
               TAToolUpdateEdit(TAToolSEAnimPreview);

               TAToolResetFlip(TAToolNWAnimPreview.sprite);
               TAToolNWAnimPreview.display(TAToolNWAnimPreview.originalAnim);
               TAToolNWAnimPreview.mirrorState = "";
               TAToolUpdateEdit(TAToolNWAnimPreview);
         }
      }
      else
      {
         TAToolNEAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
      }
   }

   // -------------------------------------------------------------------------
   // Flip West to East
   // -------------------------------------------------------------------------
   if (%checkbox == TAToolWMirrorCheckBox.getId())
   {
      TAToolEMirrorCheckBox.setValue(false);
      TAToolEMirrorCheckBox.Visible = %state;
      if (%state == false)
      {
         TAToolEAnimPreview.display(TAToolWAnimPreview.animation);
         TAToolEAnimPreview.mirrorState = "Horizontal";
         TAToolSetFlip(TAToolEAnimPreview.sprite, "Horizontal");
         TAToolUpdateEdit(TAToolEAnimPreview);
      }
      else
      {
         // temporary code - restores original test animation to the preview window
         TAToolEAnimPreview.display(TAToolEAnimPreview.originalAnim);
         TAToolResetFlip(TAToolEAnimPreview.sprite);
         TAToolEAnimPreview.mirrorState = "";
         TAToolUpdateEdit(TAToolEAnimPreview);
      }
      TAToolEAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
   }

   // -------------------------------------------------------------------------
   // Flip Northwest to Southeast, NE, SW
   // -------------------------------------------------------------------------
   if (%checkbox == TAToolNWMirrorCheckBox.getId())
   {
      if (isObject(%dropdown.hiddenControl))
      {
         %dropdown.hiddenControl.setValue(false);
         %dropdown.hiddenControl.Visible = %state;
      }
      else
      {
         TAToolSEMirrorCheckBox.setValue(false);
         TAToolSEMirrorCheckBox.Visible = %state;
      }
      
      if (isObject(%dropdown.animPreview))
      {
         switch (%state)
         {
            case 0:
               %dropdown.animPreview.sprite.BlendColor = $TAToolMirroredColor;
               %dropdown.animPreview.mirrorState = %dropdown.getValue();
            
            case 1:
               %dropdown.animPreview.sprite.BlendColor = $TAToolOriginalColor;
               TAToolResetFlip(TAToolSWAnimPreview.sprite);
               TAToolSWAnimPreview.display(TAToolSWAnimPreview.originalAnim);
               TAToolSWAnimPreview.mirrorState = "";
               TAToolUpdateEdit(TAToolSWAnimPreview);

               TAToolResetFlip(TAToolSEAnimPreview.sprite);
               TAToolSEAnimPreview.display(TAToolSEAnimPreview.originalAnim);
               TAToolSEAnimPreview.mirrorState = "";
               TAToolUpdateEdit(TAToolSEAnimPreview);

               TAToolResetFlip(TAToolNEAnimPreview.sprite);
               TAToolNEAnimPreview.display(TAToolNEAnimPreview.originalAnim);
               TAToolNEAnimPreview.mirrorState = "";
               TAToolUpdateEdit(TAToolNEAnimPreview);
         }
      }
      else
      {
         TAToolSEAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
      }
   }
   SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function is used to set the text of the edit box associated with the passed 
/// animation preview window to the name of the animation currently displayed by the
/// preview passed for the idle animation tab.
/// </summary>
/// <param name="animPreview">The animation preview that needs its label updated.</param>
function TAToolUpdateIdleEdit(%animPreview)
{
   if (%animPreview $= TAToolNIdleAnimPreview)
      TAToolNIdleAnimEdit.text = fileName(%animPreview.animation);

   if (%animPreview $= TAToolNEIdleAnimPreview)
      TAToolNEIdleAnimEdit.text = fileName(%animPreview.animation);

   if (%animPreview $= TAToolEIdleAnimPreview)
      TAToolEIdleAnimEdit.text = fileName(%animPreview.animation);

   if (%animPreview $= TAToolSEIdleAnimPreview)
      TAToolSEIdleAnimEdit.text = fileName(%animPreview.animation);

   if (%animPreview $= TAToolSIdleAnimPreview)
      TAToolSIdleAnimEdit.text = fileName(%animPreview.animation);

   if (%animPreview $= TAToolSWIdleAnimPreview)
      TAToolSWIdleAnimEdit.text = fileName(%animPreview.animation);

   if (%animPreview $= TAToolWIdleAnimPreview)
      TAToolWIdleAnimEdit.text = fileName(%animPreview.animation);

   if (%animPreview $= TAToolNWIdleAnimPreview)
      TAToolNWIdleAnimEdit.text = fileName(%animPreview.animation);
}

//-----------------------------------------------------------------------------
// Tower Animation Tool
//-----------------------------------------------------------------------------

/// <summary>
/// This function handles initialization of the Tower Animation Set Tool.
/// </summary>
function TowerAnimTool::onWake(%this)
{
   $IsTowerAnimToolAwake = true;   
   
   if ($TAToolInitialized == false)
   {
      $GameDir = LBProjectObj.gamePath;

      TAToolAnimDefaultAnimDropdown.clear();
      TAToolAnimDefaultAnimDropdown.add("North", 0);
      TAToolAnimDefaultAnimDropdown.add("Northeast", 1);
      TAToolAnimDefaultAnimDropdown.add("East", 2);
      TAToolAnimDefaultAnimDropdown.add("Southeast", 3);
      TAToolAnimDefaultAnimDropdown.add("South", 4);
      TAToolAnimDefaultAnimDropdown.add("Southwest", 5);
      TAToolAnimDefaultAnimDropdown.add("West", 6);
      TAToolAnimDefaultAnimDropdown.add("Northwest", 7); 
      
      if(isObject(%this.animationSet))
      {
        TAToolAnimDefaultAnimDropdown.setSelected(%this.animationSet.defaultAnimation);
        SetTowerAnimToolDirtyState(false);
      }
      else
      {
      TAToolAnimDefaultAnimDropdown.setFirstSelected();
        SetTowerAnimToolDirtyState(false);
      }

      TAToolRefreshMirrorDropdown(TAToolNEIdleMirrorDropDown);
      TAToolRefreshMirrorDropdown(TAToolSEIdleMirrorDropDown);
      TAToolRefreshMirrorDropdown(TAToolSWIdleMirrorDropDown);
      TAToolRefreshMirrorDropdown(TAToolNWIdleMirrorDropDown);
      
      TAToolSetDropdownVisibility(TAToolNEIdleMirrorDropDown, TAToolNEIdleMirrorCheckBox);
      TAToolSetDropdownVisibility(TAToolSEIdleMirrorDropDown, TAToolSEIdleMirrorCheckBox);
      TAToolSetDropdownVisibility(TAToolSWIdleMirrorDropDown, TAToolSWIdleMirrorCheckBox);
      TAToolSetDropdownVisibility(TAToolNWIdleMirrorDropDown, TAToolNWIdleMirrorCheckBox);
      
      TAToolRefreshMirrorDropdown(TAToolNEMirrorDropDown);
      TAToolRefreshMirrorDropdown(TAToolSEMirrorDropDown);
      TAToolRefreshMirrorDropdown(TAToolSWMirrorDropDown);
      TAToolRefreshMirrorDropdown(TAToolNWMirrorDropDown);
      
      TAToolSetDropdownVisibility(TAToolNEMirrorDropDown, TAToolNEMirrorCheckBox);
      TAToolSetDropdownVisibility(TAToolSEMirrorDropDown, TAToolSEMirrorCheckBox);
      TAToolSetDropdownVisibility(TAToolSWMirrorDropDown, TAToolSWMirrorCheckBox);
      TAToolSetDropdownVisibility(TAToolNWMirrorDropDown, TAToolNWMirrorCheckBox);
      
      $TAToolInitialized = true;   
   }
   %this.clearPreviews();

   TAToolNEIdleMirrorDropDown.active = true;
   TAToolSEIdleMirrorDropDown.active = true;
   TAToolSWIdleMirrorDropDown.active = true;
   TAToolNWIdleMirrorDropDown.active = true;

   TAToolNEMirrorDropDown.active = true;
   TAToolSEMirrorDropDown.active = true;
   TAToolSWMirrorDropDown.active = true;
   TAToolNWMirrorDropDown.active = true;
   
   TAToolTabBook.selectPage(0);
   
   $TAToolFireStateInit = false;
   %this.initializePreviews();
   SetTowerAnimToolDirtyState(false);
}

/// <summary>
/// This function clears the animations from the animation preview windows.
/// It also clears the associated edit controls.
/// </summary>
function TowerAnimTool::clearPreviews(%this)
{
    TAToolNIdleAnimPreview.mirrorState = "";
    TAToolNIdleAnimPreview.originalMirrorState = "";
    TAToolNIdleAnimPreview.originalAnim = "";

    TAToolNEIdleAnimPreview.mirrorState = "";
    TAToolNEIdleAnimPreview.originalMirrorState = "";
    TAToolNEIdleAnimPreview.originalAnim = "";

    TAToolEIdleAnimPreview.mirrorState = "";
    TAToolEIdleAnimPreview.originalMirrorState = "";
    TAToolEIdleAnimPreview.originalAnim = "";

    TAToolSEIdleAnimPreview.mirrorState = "";
    TAToolSEIdleAnimPreview.originalMirrorState = "";
    TAToolSEIdleAnimPreview.originalAnim = "";

    TAToolSIdleAnimPreview.mirrorState = "";
    TAToolSIdleAnimPreview.originalMirrorState = "";
    TAToolSIdleAnimPreview.originalAnim = "";

    TAToolSWIdleAnimPreview.mirrorState = "";
    TAToolSWIdleAnimPreview.originalMirrorState = "";
    TAToolSWIdleAnimPreview.originalAnim = "";

    TAToolWIdleAnimPreview.mirrorState = "";
    TAToolWIdleAnimPreview.originalMirrorState = "";
    TAToolWIdleAnimPreview.originalAnim = "";

    TAToolNWIdleAnimPreview.mirrorState = "";
    TAToolNWIdleAnimPreview.originalMirrorState = "";
    TAToolNWIdleAnimPreview.originalAnim = "";

    TAToolNAnimPreview.mirrorState = "";
    TAToolNAnimPreview.originalMirrorState = "";
    TAToolNAnimPreview.originalAnim = "";

    TAToolNEAnimPreview.mirrorState = "";
    TAToolNEAnimPreview.originalMirrorState = "";
    TAToolNEAnimPreview.originalAnim = "";

    TAToolEAnimPreview.mirrorState = "";
    TAToolEAnimPreview.originalMirrorState = "";
    TAToolEAnimPreview.originalAnim = "";

    TAToolSEAnimPreview.mirrorState = "";
    TAToolSEAnimPreview.originalMirrorState = "";
    TAToolSEIdleAnimPreview.originalAnim = "";

    TAToolSAnimPreview.mirrorState = "";
    TAToolSAnimPreview.originalMirrorState = "";
    TAToolSAnimPreview.originalAnim = "";

    TAToolSWAnimPreview.mirrorState = "";
    TAToolSWAnimPreview.originalMirrorState = "";
    TAToolSWAnimPreview.originalAnim = "";

    TAToolWAnimPreview.mirrorState = "";
    TAToolWAnimPreview.originalMirrorState = "";
    TAToolWAnimPreview.originalAnim = "";

    TAToolNWAnimPreview.mirrorState = "";
    TAToolNWAnimPreview.originalMirrorState = "";
    TAToolNWAnimPreview.originalAnim = "";

    TAToolNIdleMirrorCheckBox.setValue(false);
    TAToolNEIdleMirrorCheckBox.setValue(false);
    TAToolEIdleMirrorCheckBox.setValue(false);
    TAToolSEIdleMirrorCheckBox.setValue(false);
    TAToolSIdleMirrorCheckBox.setValue(false);
    TAToolSWIdleMirrorCheckBox.setValue(false);
    TAToolWIdleMirrorCheckBox.setValue(false);
    TAToolNWIdleMirrorCheckBox.setValue(false);

    TAToolNMirrorCheckBox.setValue(false);
    TAToolNEMirrorCheckBox.setValue(false);
    TAToolEMirrorCheckBox.setValue(false);
    TAToolSEMirrorCheckBox.setValue(false);
    TAToolSMirrorCheckBox.setValue(false);
    TAToolSWMirrorCheckBox.setValue(false);
    TAToolWMirrorCheckBox.setValue(false);
    TAToolNWMirrorCheckBox.setValue(false);

    TAToolNEIdleMirrorDropDown.Visible = false;
    TAToolSEIdleMirrorDropDown.Visible = false;
    TAToolSWIdleMirrorDropDown.Visible = false;
    TAToolNWIdleMirrorDropDown.Visible = false;

    TAToolNEMirrorDropDown.Visible = false;
    TAToolSEMirrorDropDown.Visible = false;
    TAToolSWMirrorDropDown.Visible = false;
    TAToolNWMirrorDropDown.Visible = false;
}

/// <summary>
/// This function asks the user to save any changes if needed and handles closing
/// the tool.
/// </summary>
function TowerAnimTool::close(%this)
{
   if ($Tools::TemplateToolDirty)
   {
      ConfirmDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(), "Save Tower Animation Set Changes?", 
            "Save", "if (TowerAnimTool.saveAnimSet()) { SetTowerAnimToolDirtyState(false); Canvas.popDialog(TowerAnimTool);}", 
            "Don't Save", "SetTowerAnimToolDirtyState(false); Canvas.popDialog(TowerAnimTool);", 
            "Cancel", "");
   }
   else
      Canvas.popDialog(TowerAnimTool);
}

/// <summary>
/// This function retrieves the animation set information and updates the tool to 
/// display the data.
/// </summary>
function TowerAnimTool::initializePreviews(%this)
{
   if (!isObject(%this.animationSet))
      %this.createAnimationSet();
      
   TAToolAnimSetName.text = %this.animationSet.getName();

   %this.loadPreview("IdleAnim", "N");
   %this.loadPreview("IdleAnim", "NE");
   %this.loadPreview("IdleAnim", "E");
   %this.loadPreview("IdleAnim", "SE");
   %this.loadPreview("IdleAnim", "S");
   %this.loadPreview("IdleAnim", "SW");
   %this.loadPreview("IdleAnim", "W");
   %this.loadPreview("IdleAnim", "NW");
   
   %this.loadPreview("Anim", "N");
   %this.loadPreview("Anim", "NE");
   %this.loadPreview("Anim", "E");
   %this.loadPreview("Anim", "SE");
   %this.loadPreview("Anim", "S");
   %this.loadPreview("Anim", "SW");
   %this.loadPreview("Anim", "W");
   %this.loadPreview("Anim", "NW");

   TAToolFireOnStartRBtn.setValue(%this.animationSet.fireOnStart);
   TAToolFireOnEndRBtn.setValue(!%this.animationSet.fireOnStart);
   $TAToolFireStateInit = true;
}

/// <summary>
/// This function loads a preview animation associated with the requested tag and 
/// direction.
/// </summary>
/// <param name="previewTag">The undecorated name of the control group.</param>
/// <param name="direction">The requested animation direction.</param>
/// <example>
/// %previewTag is the name of the control without the identifying part at the end.
/// For example, IdleAnim is the tag for TAToolNWIdleAnimPreview, TAToolNWIdleAnimEdit, 
/// TAToolNWIdleAnimMirrorCheckBox and TAToolNWIdleAnimMirrorDropDown.
/// %direction should be N, NE, E, SE, S, SW, W, NW.
/// The function will then assemble all of the object names as needed from the tags passed
/// and load the data to the appropriate controls.
/// </example>
function TowerAnimTool::loadPreview(%this, %previewTag, %direction)
{
   // TATool N Anim Preview
   // TATool N IdleAnim Preview
   %tag = "TATool" @ %direction @ %previewTag;
   %preview = %tag @ "Preview";
   %edit = %tag @ "Edit";

   %animMirrorState = "";
   %type = "";
   %dir = "";
   switch$(%previewTag)
   {
      case "IdleAnim":
         %type = "Idle";
         %mType = "Idle";
      
      case "Anim":
         %type = "Firing";
         %mType = "";
   }

   %mirror = "TATool" @ %direction @ %mType;
   %mirror = strreplace(%mirror, "Anim", "");
   %mirrorDrop = %mirror;

   switch$(%direction)
   {
      case "N":
         %dir = "North";
      case "NE":
         %dir = "Northeast";
      case "E":
         %dir = "East";
      case "SE":
         %dir = "Southeast";
      case "S":
         %dir = "South";
      case "SW":
         %dir = "Southwest";
      case "W":
         %dir = "West";
      case "NW":
         %dir = "Northwest";
   }
   
   %anim = %type @ %dir @ "Anim";
   eval("%animMirrorState = %this.animationSet." @ %type @ %dir @ "Mirror;");
   
   //echo(" -- %animMirrorState = " @ %animMirrorState);
   if (%animMirrorState !$= "" && %animMirrorState !$= "Diagonal" && %animMirrorState !$= "Horizontal" && %animMirrorState !$= "Vertical")
      %animMirrorState = "";
    echo(" -- TATool::LoadPreview() - %animMirrorState : " @ %animMirrorState);
   %displayObject = %this.animationSet.GetAnimationDatablock(%anim);
   
   if (isObject(%displayObject))
   {
       %preview.display(%displayObject, t2dAnimatedSprite);
       %edit.text = %preview.sprite.getAnimation();
       %edit.setActive(false);
       %preview.mirrorState = %animMirrorState;
       %preview.originalMirrorState = %animMirrorState;
       
       if (%preview.mirrorState $= "")
          %preview.originalAnim = %preview.sprite.getAnimation();
       else
       {
          %mirror.visible = false;
          %preview.sprite.BlendColor = $TAToolMirroredColor;
          if (%direction $= "N")
          {
             %opposite = strreplace(%mirror, "N", "S");
             %oppositeMirror = %opposite @ "MirrorCheckBox";
             %oppositeMirror.setStateOn(true);
          }
          if (%direction $= "NE")
          {
             switch$(%animMirrorState)
             {
                case "Diagonal":
                   %opposite = strreplace(%mirror, "NE", "SW");
                case "Horizontal":
                   %opposite = strreplace(%mirror, "NE", "NW");
                case "Vertical":
                   %opposite = strreplace(%mirror, "NE", "SE");
             }
             %oppositeMirror = %opposite @ "MirrorCheckBox";
             %oppositeDropdown = %opposite @ "MirrorDropDown";
             %oppositeMirror.setStateOn(true);
          }
          if (%direction $= "E")
          {
             %opposite = strreplace(%mirror, "E", "W");
             %oppositeMirror = %opposite @ "MirrorCheckBox";
             %oppositeMirror.setStateOn(true);
          }
          if (%direction $= "SE")
          {
             switch$(%animMirrorState)
             {
                case "Diagonal":
                   %opposite = strreplace(%mirror, "SE", "NW");
                case "Horizontal":
                   %opposite = strreplace(%mirror, "SE", "SW");
                case "Vertical":
                   %opposite = strreplace(%mirror, "SE", "NE");
             }
             %oppositeMirror = %opposite @ "MirrorCheckBox";
             %oppositeDropdown = %opposite @ "MirrorDropDown";
             %oppositeMirror.setStateOn(true);
          }
          if (%direction $= "S")
          {
             %opposite = strreplace(%mirror, "S", "N");
             %oppositeMirror = %opposite @ "MirrorCheckBox";
             %oppositeMirror.setStateOn(true);
          }
          if (%direction $= "SW")
          {
             switch$(%animMirrorState)
             {
                case "Diagonal":
                   %opposite = strreplace(%mirror, "SW", "NE");
                case "Horizontal":
                   %opposite = strreplace(%mirror, "SW", "SE");
                case "Vertical":
                   %opposite = strreplace(%mirror, "SW", "NW");
             }
             %oppositeMirror = %opposite @ "MirrorCheckBox";
             %oppositeDropdown = %opposite @ "MirrorDropDown";
             %oppositeMirror.setStateOn(true);
          }
          if (%direction $= "W")
          {
             %opposite = strreplace(%mirror, "W", "E");
             %oppositeMirror = %opposite @ "MirrorCheckBox";
             %oppositeMirror.setStateOn(true);
          }
          if (%direction $= "NW")
          {
             switch$(%animMirrorState)
             {
                case "Diagonal":
                   %opposite = strreplace(%mirror, "NW", "SE");
                case "Horizontal":
                   %opposite = strreplace(%mirror, "NW", "SE");
                case "Vertical":
                   %opposite = strreplace(%mirror, "NW", "SE");
             }
             %oppositeMirror = %opposite @ "MirrorCheckBox";
             %oppositeDropdown = %opposite @ "MirrorDropDown";
             %oppositeMirror.setStateOn(true);
          }
       }
    echo(" -- TATool::LoadPreview() - %opposite : " @ %oppositeMirror);

       TAToolSetFlip(%preview.sprite, %animMirrorState);
   }
   else
   {
       %preview.display("", "");
       %edit.text = "";
       %edit.setActive(false);
       %preview.mirrorState = "";
       %preview.originalMirrorState = "";
       %mirror.Visible = true;
   }
}

/// <summary>
/// This function checks that all animation set fields have been populated correctly and
/// that the animation set name is a valid SimObject name.
/// </summary>
/// <return>Returns true if the animation set is valid, false if not.</return>
function TowerAnimTool::checkValid(%this)
{
    if (!isObject(TAToolNIdleAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "North idle animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolNIdleAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == false)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "North idle animation is not a looping animation.  Please make this a looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    if (!isObject(TAToolNEIdleAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Northeast idle animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolNEIdleAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == false)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Northeast idle animation is not a looping animation.  Please make this a looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    if (!isObject(TAToolEIdleAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "East idle animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolEIdleAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == false)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "East idle animation is not a looping animation.  Please make this a looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    if (!isObject(TAToolSEIdleAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Southeast idle animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolSEIdleAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == false)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Southeast idle animation is not a looping animation.  Please make this a looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    if (!isObject(TAToolSIdleAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "South idle animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolSIdleAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == false)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "South idle animation is not a looping animation.  Please make this a looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    if (!isObject(TAToolSWIdleAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Southwest idle animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolSWIdleAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == false)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Southwest idle animation is not a looping animation.  Please make this a looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    if (!isObject(TAToolWIdleAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "West idle animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolWIdleAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == false)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "West idle animation is not a looping animation.  Please make this a looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    if (!isObject(TAToolNWIdleAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Northwest idle animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolNWIdleAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == false)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Northwest idle animation is not a looping animation.  Please make this a looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }

    if (!isObject(TAToolNAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "North firing animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolNAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == true)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "North Firing animation is a looping animation.  Please make this a non-looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    if (!isObject(TAToolNEAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Northeast firing animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolNEAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == true)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Northeast Firing animation is a looping animation.  Please make this a non-looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    if (!isObject(TAToolEAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "East firing animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolEAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == true)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "East Firing animation is a looping animation.  Please make this a non-looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    if (!isObject(TAToolSEAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Southeast firing animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolSEAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == true)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Southeast Firing animation is a looping animation.  Please make this a non-looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    if (!isObject(TAToolSAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "South firing animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolSAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == true)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "South Firing animation is a looping animation.  Please make this a non-looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    if (!isObject(TAToolSWAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Southwest firing animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolSWAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == true)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Southwest Firing animation is a looping animation.  Please make this a non-looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    if (!isObject(TAToolWAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "West firing animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolWAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == true)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "West Firing animation is a looping animation.  Please make this a non-looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    if (!isObject(TAToolNWAnimPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Northwest firing animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = TAToolNWAnimPreview.sprite.getAnimation();
    if (%datablock.animationCycle == true)
    {
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "Northwest Firing animation is a looping animation.  Please make this a non-looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }

    return true;
}

/// <summary>
/// This function creates a new, blank tower animation set and gives it a
/// unique temporary name.
/// </summary>
function TowerAnimTool::createAnimationSet(%this)
{
   %this.animationSet = new ScriptObject() {
	  canSaveDynamicFields = "1";
         class = "animationSet";
	  
         IdleNorthAnim = "";
         IdleNorthMirror = "";

         IdleNortheastAnim = "";
         IdleNortheastMirror = "";

         IdleEastAnim = "";
         IdleEastMirror = "";

         IdleSoutheastAnim = "";
         IdleSoutheastMirror = "";

         IdleSouthAnim = "";
         IdleSouthMirror = "";

         IdleSouthwestAnim = "";
         IdleSouthwestMirror = "";

         IdleWestAnim = "";
         IdleWestMirror = "";

         IdleNorthwestAnim = "";
         IdleNorthwestMirror = "";

         FiringNorthAnim = "";
         FiringNorthMirror = "";

         FiringNortheastAnim = "";
         FiringNortheastMirror = "";

         FiringEastAnim = "";
         FiringEastMirror = "";

         FiringSoutheastAnim = "";
         FiringSoutheastMirror = "";

         FiringSouthAnim = "";
         FiringSouthMirror = "";

         FiringSouthwestAnim = "";
         FiringSouthwestMirror = "";

         FiringWestAnim = "";
         FiringWestMirror = "";

         FiringNorthwestAnim = "";
         FiringNorthwestMirror = "";

         NameTags = "1";
   };

   %name = "NewAnimationSet";
   %number = 1;
   
   while (isObject(%name))
   {
      %name = "NewAnimationSet" @ %number;
      %number++;
   }

   %this.animationSet.setName(%name);
   LBProjectObj.addAnimationSet(%this.animationSet.getName(), true);
}

/// <summary>
/// This function checks that a name is valid for use as an animation set name.
/// </summary>
/// <param name="name">The name to be validated.</param>
/// <return>Returns true if the name is valid, false if not.</return>
function TowerAnimTool::checkValidName(%this, %name)
{
    // Check for empty name 
    if (strreplace(%name, " ", "") $= "")  
    {
        // Show message dialog
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(), "Notice!", "Animation Set name cannot be empty!", 
         "", "", "", "", "Okay", "return false;");         

        
        return false;   
    }

    // Check for spaces
    if (strreplace(%name, " ", "") !$= %name)
    {
        // Show message dialog
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(), "Notice!", "Animation Set name cannot contain spaces!", 
         "", "", "", "", "Okay", "return false;");         
        
        return false;
    }
    
    // Check for invalid characters
    %invalidCharacters = "-+*/%$&§=()[].?\"#,;!~<>|°^{}";
    %strippedName = stripChars(%name, %invalidCharacters);
    if (%strippedName !$= %name)
    {
        // Show message dialog
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(), "Notice!", "Animation Set name contains invalid symbols!", 
         "", "", "", "", "Okay", "return false;"); 
         
        return false;   
    }
    
    // check for all numbers 
    %invalidCharacters = "0123456789";
    %strippedName = stripChars(%name, %invalidCharacters);
    if (%strippedName $= "")
    {
        // Show message dialog
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(), "Notice!", "Animation Set name must have at least one non-numeric character!", 
         "", "", "", "", "Okay", "return false;"); 
         
        return false;   
    }
    
    %invalidCharacters = "0123456789";
    %firstChar = getSubStr(%name, 0, 1);
    %strippedName = stripChars(%firstChar, %invalidCharacters);
    if (%strippedName $= "")
    {
        // Show message dialog
        WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(), "Notice!", "Animation Set name must not begin with a number!", 
         "", "", "", "", "Okay", "return false;"); 
         
        return false;   
    }

    return true;
}

/// <summary>
/// This function saves the tower animation set.
/// </summary>
/// <return>Returns true if the save operation completes, or false if not.</return>
function TowerAnimTool::saveAnimSet(%this)
{
   %valid = %this.checkValid();
   if (!%valid)
   {
      return;
   }
   
   %animSetName = TAToolAnimSetName.getText();
   
    if (!%this.checkValidName(%animSetName))
        return;
    
   %noConflict = false;

   for (%i = 0; %i < $animationSets.getCount();%i++)
   {
      %temp = $animationSets.getObject(%i);

      if (%temp.getName() $= %animSetName)
      {
         if (%this.animationSet == %temp)
            %noConflict = true;

         else
         {
            // messagebox - name conflict
            WarningDialog.setupAndShow(TowerAnimToolWindow.getGlobalCenter(), "Duplicate Name", "Another Animation Set Object in your Game Already has this Name", "", "", "OK");
            
            return;
         }
      }
      if (%noConflict)
         break;
   }
   
   if (!%noConflict)
      %this.animationSet.setName(TAToolAnimSetName.getText());

    %this.animationSet.defaultAnimation = TAToolAnimDefaultAnimDropdown.getSelected();

   %this.animationSet.FiringNorthAnim = TAToolNAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.FiringNorthAnim);
   %this.animationSet.FiringNorthMirror = TAToolNAnimPreview.mirrorState;

   %this.animationSet.FiringNortheastAnim = TAToolNEAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.FiringNortheastAnim);
   %this.animationSet.FiringNortheastMirror = TAToolNEAnimPreview.mirrorState;

   %this.animationSet.FiringEastAnim = TAToolEAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.FiringEastAnim);
   %this.animationSet.FiringEastMirror = TAToolEAnimPreview.mirrorState;

   %this.animationSet.FiringSoutheastAnim = TAToolSEAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.FiringSoutheastAnim);
   %this.animationSet.FiringSoutheastMirror = TAToolSEAnimPreview.mirrorState;

   %this.animationSet.FiringSouthAnim = TAToolSAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.FiringSouthAnim);
   %this.animationSet.FiringSouthMirror = TAToolSAnimPreview.mirrorState;

   %this.animationSet.FiringSouthwestAnim = TAToolSWAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.FiringSouthwestAnim);
   %this.animationSet.FiringSouthwestMirror = TAToolSWAnimPreview.mirrorState;

   %this.animationSet.FiringWestAnim = TAToolWAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.FiringWestAnim);
   %this.animationSet.FiringWestMirror = TAToolWAnimPreview.mirrorState;

   %this.animationSet.FiringNorthwestAnim = TAToolNWAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.FiringNorthwestAnim);
   %this.animationSet.FiringNorthwestMirror = TAToolNWAnimPreview.mirrorState;

   %this.animationSet.IdleNorthAnim = TAToolNIdleAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.IdleNorthAnim);
   %this.animationSet.IdleNorthMirror = TAToolNIdleAnimPreview.mirrorState;

   %this.animationSet.IdleNorthEastAnim = TAToolNEIdleAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.IdleNorthEastAnim);
   %this.animationSet.IdleNorthEastMirror = TAToolNEIdleAnimPreview.mirrorState;

   %this.animationSet.IdleEastAnim = TAToolEIdleAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.IdleEastAnim);
   %this.animationSet.IdleEastMirror = TAToolEIdleAnimPreview.mirrorState;

   %this.animationSet.IdleSoutheastAnim = TAToolSEIdleAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.IdleSoutheastAnim);
   %this.animationSet.IdleSoutheastMirror = TAToolSEIdleAnimPreview.mirrorState;

   %this.animationSet.IdleSouthAnim = TAToolSIdleAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.IdleSouthAnim);
   %this.animationSet.IdleSouthMirror = TAToolSIdleAnimPreview.mirrorState;

   %this.animationSet.IdleSouthwestAnim = TAToolSWIdleAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.IdleSouthwestAnim);
   %this.animationSet.IdleSouthwestMirror = TAToolSWIdleAnimPreview.mirrorState;

   %this.animationSet.IdleWestAnim = TAToolWIdleAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.IdleWestAnim);
   %this.animationSet.IdleWestMirror = TAToolWIdleAnimPreview.mirrorState;
   
   %this.animationSet.IdleNorthwestAnim = TAToolNWIdleAnimEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.IdleNorthwestAnim);
   %this.animationSet.IdleNorthwestMirror = TAToolNWIdleAnimPreview.mirrorState;
   
   %this.animationSet.fireOnStart = TAToolFireOnStartRBtn.getValue();
   
   LDEonApply();
   LBProjectObj.persistToDisk(true, true, true, true, true, true, true);
   SaveAllLevelDatablocks();
   LBProjectObj.saveLevel();
   //%this.animationSet.save($AnimSetFile);
   //echo(" -- EnemyAnimTool::saveAnimSet() File - " @ $AnimSetFile);

   SetTowerAnimToolDirtyState(false);
   
   TowerUpdateAnimSet(%this.animationSet.getName());
}

/// <summary>
/// This function handles putting the tool to sleep.
/// </summary>
function TowerAnimTool::onSleep(%this)
{
   $IsTowerAnimToolAwake = false;   
   
   TAToolNEIdleMirrorDropDown.active = false;
   TAToolSEIdleMirrorDropDown.active = false;
   TAToolSWIdleMirrorDropDown.active = false;
   TAToolNWIdleMirrorDropDown.active = false;

   TAToolNEMirrorDropDown.active = false;
   TAToolSEMirrorDropDown.active = false;
   TAToolSWMirrorDropDown.active = false;
   TAToolNWMirrorDropDown.active = false;
}

/// <summary>
/// This function handles the Fire On Start radio button.
/// </summary>
function TAToolFireOnStartRBtn::onClick(%this)
{
    //echo(" -- Fire On Start value changed");
    if ($TAToolFireStateInit)
        SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles the Fire On End radio button.
/// </summary>
function TAToolFireOnEndRBtn::onClick(%this)
{
    //echo(" -- Fire On End value changed");
    if ($TAToolFireStateInit)
        SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function clears the TAToolAnimSetName edit box.
/// </summary>
function TAAnimSetNameClearButton::onClick(%this)
{
   TAToolAnimSetName.text = "";
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolNAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolNAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolNAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolNAnimPreview.originalAnim = %asset;
   TAToolNAnimEdit.text = %asset;

   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolNAnimPreview.mirrorState !$= "")
   {
      TAToolSMirrorCheckBox.setStateOn(false);
      TAToolNMirrorCheckBox.Visible = true;
      TAToolNAnimPreview.mirrorState = "";
      TAToolNAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
      TAToolResetFlip(TAToolNAnimPreview.sprite);
   }

   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolSAnimPreview.mirrorState !$= "")
   {
      TAToolSAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolSAnimEdit.text = %asset;
   }
   
   SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolNEAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolNEAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolNEAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolNEAnimPreview.originalAnim = %asset;
   TAToolNEAnimEdit.text = %asset;
   
   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolNEAnimPreview.mirrorState !$= "")
   {
      switch$(TAToolNEAnimPreview.mirrorState)
      {
         case "Diagonal":
            TAToolSWMirrorCheckBox.setStateOn(false);
            TAToolSWMirrorDropDown.Visible = false;
         
         case "Horizontal":
            TAToolNWMirrorCheckBox.setStateOn(false);
            TAToolNWMirrorDropDown.Visible = false;
         
         case "Vertical":
            TAToolSEMirrorCheckBox.setStateOn(false);
            TAToolSEMirrorDropDown.Visible = false;
         
      }
      TAToolNEMirrorCheckBox.Visible = true;
      TAToolNEAnimPreview.mirrorState = "";
      TAToolNEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
      TAToolResetFlip(TAToolNEAnimPreview.sprite);
   }

   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolSWAnimPreview.mirrorState $= "Diagonal")
   {
      TAToolSWAnimPreview.originalAnim = TAToolSWAnimPreview.sprite.getAnimation();
      TAToolSWAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolSWAnimEdit.text = %asset;
   }
   if (TAToolSEAnimPreview.mirrorState $= "Vertical")
   {
      TAToolSEAnimPreview.originalAnim = TAToolSEAnimPreview.sprite.getAnimation();
      TAToolSEAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolSEAnimEdit.text = %asset;
   }
   if (TAToolNWAnimPreview.mirrorState $= "Horizontal")
   {
      TAToolNWAnimPreview.originalAnim = TAToolNWAnimPreview.sprite.getAnimation();
      TAToolNWAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolNWAnimEdit.text = %asset;
   }
   
   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolEAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolEAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolEAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolEAnimPreview.originalAnim = %asset;
   TAToolEAnimEdit.text = %asset;
   
   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolEIdleAnimPreview.mirrorState !$= "")
   {
      TAToolWMirrorCheckBox.setStateOn(false);
      TAToolEMirrorCheckBox.Visible = true;
      TAToolEAnimPreview.mirrorState = "";
      TAToolResetFlip(TAToolEAnimPreview.sprite);
      TAToolEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
   }

   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolWAnimPreview.mirrorState !$= "")
   {
      TAToolWAnimPreview.originalAnim = TAToolWAnimPreview.sprite.getAnimation();
      TAToolWAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolWAnimEdit.text = %asset;
   }

   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolSEAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolSEAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolSEAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolSEAnimPreview.originalAnim = %asset;
   TAToolSEAnimEdit.text = %asset;

   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolSEAnimPreview.mirrorState !$= "")
   {
      switch$(TAToolSEAnimPreview.mirrorState)
      {
         case "Diagonal":
            TAToolNWMirrorCheckBox.setStateOn(false);
            TAToolNWMirrorDropDown.Visible = false;
         
         case "Horizontal":
            TAToolSWMirrorCheckBox.setStateOn(false);
            TAToolSWMirrorDropDown.Visible = false;
         
         case "Vertical":
            TAToolNEMirrorCheckBox.setStateOn(false);
            TAToolNEMirrorDropDown.Visible = false;
         
      }
      TAToolSEMirrorCheckBox.Visible = true;
      TAToolSEAnimPreview.mirrorState = "";
      TAToolResetFlip(TAToolSEAnimPreview.sprite);
      TAToolSEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
   }

   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolNWAnimPreview.mirrorState $= "Diagonal")
   {
      TAToolNWAnimPreview.originalAnim = TAToolNWAnimPreview.sprite.getAnimation();
      TAToolNWAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolNWAnimEdit.text = %asset;
   }
   if (TAToolNEAnimPreview.mirrorState $= "Vertical")
   {
      TAToolNEMirrorCheckBox.originalAnim = TAToolNEMirrorCheckBox.sprite.getAnimation();
      TAToolNEAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolNEAnimEdit.text = %asset;
   }
   if (TAToolSWAnimPreview.mirrorState $= "Horizontal")
   {
      TAToolSWAnimPreview.originalAnim = TAToolSWAnimPreview.sprite.getAnimation();
      TAToolSWAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolSWAnimEdit.text = %asset;
   }

   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolSAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolSAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolSAnimPreview.originalAnim = TAToolSAnimPreview.sprite.getAnimation();
   TAToolSAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolSAnimEdit.text = %asset;

   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolSAnimPreview.mirrorState !$= "")
   {
      TAToolNMirrorCheckBox.setStateOn(false);
      TAToolSMirrorCheckBox.Visible = true;
      TAToolSAnimPreview.mirrorState = "";
      TAToolResetFlip(TAToolEAnimPreview.sprite);
      TAToolSAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
   }

   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolSAnimPreview.mirrorState !$= "")
   {
      TAToolNAnimPreview.originalAnim = TAToolNAnimPreview.sprite.getAnimation();
      TAToolNAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolNAnimEdit.text = %asset;
   }

   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolSWAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolSWAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolSWAnimPreview.originalAnim = TAToolSWAnimPreview.sprite.getAnimation();
   TAToolSWAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolSWAnimEdit.text = %asset;

   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolSWAnimPreview.mirrorState !$= "")
   {
      switch$(TAToolSWAnimPreview.mirrorState)
      {
         case "Diagonal":
            TAToolNEMirrorCheckBox.setStateOn(false);
            TAToolNEMirrorDropDown.Visible = false;
         
         case "Horizontal":
            TAToolSEMirrorCheckBox.setStateOn(false);
            TAToolSEMirrorDropDown.Visible = false;
         
         case "Vertical":
            TAToolNWMirrorCheckBox.setStateOn(false);
            TAToolNWMirrorDropDown.Visible = false;
         
      }
      TAToolSWMirrorCheckBox.Visible = true;
      TAToolSWAnimPreview.mirrorState = "";
      TAToolSWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
      TAToolResetFlip(TAToolSWAnimPreview.sprite);
   }

   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolNWAnimPreview.mirrorState $= "Vertical")
   {
      TAToolNWAnimPreview.originalAnim = TAToolNWAnimPreview.sprite.getAnimation();
      TAToolNWAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolNWAnimEdit.text = %asset;
   }
   if (TAToolNEAnimPreview.mirrorState $= "Diagonal   ")
   {
      TAToolNEAnimPreview.originalAnim = TAToolNEAnimPreview.sprite.getAnimation();
      TAToolNEAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolNEAnimEdit.text = %asset;
   }
   if (TAToolSEAnimPreview.mirrorState $= "Horizontal")
   {
      TAToolSEAnimPreview.originalAnim = TAToolSEAnimPreview.sprite.getAnimation();
      TAToolSEAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolSEAnimEdit.text = %asset;
   }

   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolWAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolWAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolWAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolWAnimEdit.text = %asset;
   
   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolWAnimPreview.mirrorState !$= "")
   {
      TAToolEMirrorCheckBox.setStateOn(false);
      TAToolWMirrorCheckBox.Visible = true;
      TAToolWAnimPreview.mirrorState = "";
      TAToolResetFlip(TAToolWAnimPreview.sprite);
      TAToolWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
   }

   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolEAnimPreview.mirrorState !$= "")
   {
      TAToolWAnimPreview.originalAnim = TAToolWAnimPreview.sprite.getAnimation();
      TAToolWAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolWAnimEdit.text = %asset;
   }

   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolNWAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolNWAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolNWAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolWAnimPreview.originalAnim = %asset;
   TAToolNWAnimEdit.text = %asset;

   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolNWAnimPreview.mirrorState !$= "")
   {
      switch$(TAToolNWAnimPreview.mirrorState)
      {
         case "Diagonal":
            TAToolSEMirrorCheckBox.setStateOn(false);
            TAToolSEMirrorDropDown.Visible = false;
         
         case "Horizontal":
            TAToolNEMirrorCheckBox.setStateOn(false);
            TAToolNEMirrorDropDown.Visible = false;
         
         case "Vertical":
            TAToolSWMirrorCheckBox.setStateOn(false);
            TAToolSWMirrorDropDown.Visible = false;
         
      }
      TAToolNWMirrorCheckBox.Visible = true;
      TAToolNWAnimPreview.mirrorState = "";
      TAToolNWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
      TAToolResetFlip(TAToolNWAnimPreview.sprite);
   }

   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolNEAnimPreview.mirrorState $= "Horizontal")
   {
      TAToolNEAnimPreview.originalAnim = TAToolNEAnimPreview.sprite.getAnimation();
      TAToolNEAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolNEAnimEdit.text = %asset;
   }
   if (TAToolSWAnimPreview.mirrorState $= "Vertical")
   {
      TAToolSWAnimPreview.originalAnim = TAToolSWAnimPreview.sprite.getAnimation();
      TAToolSWAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolSWAnimEdit.text = %asset;
   }
   if (TAToolSEAnimPreview.mirrorState $= "Diagonal")
   {
      TAToolSEAnimPreview.originalAnim = TAToolSEAnimPreview.sprite.getAnimation();
      TAToolSEAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolSEAnimEdit.text = %asset;
   }

   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring an animation from North to South.
/// </summary>
function TAToolNMirrorCheckBox::onClick(%this)
{
    // -------------------------------------------------------------------------
    // Flip North to South
    // -------------------------------------------------------------------------
    %state = !%this.getValue();
    TAToolSMirrorCheckBox.setValue(false);
    TAToolSMirrorCheckBox.Visible = %state;
    if (%state == false)
    {
        TAToolSAnimPreview.display(TAToolNAnimPreview.animation);
        TAToolSetFlip(TAToolSAnimPreview.sprite, "Vertical");
        TAToolSAnimPreview.mirrorState = "Vertical";
        TAToolUpdateEdit(TAToolSAnimPreview);
    }
    else
    {
        // temporary code - restores original test animation to the preview window
        TAToolSAnimPreview.display(TAToolSAnimPreview.originalAnim);
        TAToolResetFlip(TAToolSAnimPreview.sprite);
        TAToolSAnimPreview.mirrorState = "";
        TAToolUpdateEdit(TAToolSAnimPreview);
    }
    TAToolSAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
    SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring an animation from Northeast to Southwest, Southeast or Northwest.
/// </summary>
function TAToolNEMirrorCheckBox::onClick(%this)
{
   TAToolNEMirrorDropDown.onSelect();
}

/// <summary>
/// This function handles mirroring an animation from East to West.
/// </summary>
function TAToolEMirrorCheckBox::onClick(%this)
{
    // -------------------------------------------------------------------------
    // Flip East to West
    // -------------------------------------------------------------------------
    %state = !%this.getValue();
    TAToolWMirrorCheckBox.setValue(false);
    TAToolWMirrorCheckBox.Visible = %state;
    if (%state == false)
    {
        TAToolWAnimPreview.display(TAToolEAnimPreview.animation);
        TAToolWAnimPreview.mirrorState = "Horizontal";
        TAToolSetFlip(TAToolWAnimPreview.sprite, "Horizontal");
        TAToolUpdateEdit(TAToolWAnimPreview);
    }
    else
    {
        // temporary code - restores original test animation to the preview window
        TAToolWAnimPreview.display(TAToolWAnimPreview.originalAnim);
        TAToolResetFlip(TAToolWAnimPreview.sprite);
        TAToolWAnimPreview.mirrorState = "";
        TAToolUpdateEdit(TAToolWAnimPreview);
    }
    TAToolWAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
    SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring an animation from Southeast to Northwest, Northeast or Southwest.
/// </summary>
function TAToolSEMirrorCheckBox::onClick(%this)
{
   TAToolSEMirrorDropDown.onSelect();
}

/// <summary>
/// This function handles mirroring an animation from South to North.
/// </summary>
function TAToolSMirrorCheckBox::onClick(%this)
{
    // -------------------------------------------------------------------------
    // Flip South to North
    // -------------------------------------------------------------------------
    %state = !%this.getValue();
    TAToolNMirrorCheckBox.setValue(false);
    TAToolNMirrorCheckBox.Visible = %state;
    if (%state == false)
    {
        TAToolNAnimPreview.display(TAToolSAnimPreview.animation);
        TAToolNAnimPreview.mirrorState = "Vertical";
        TAToolSetFlip(TAToolNAnimPreview.sprite, "Vertical");
        TAToolUpdateEdit(TAToolNAnimPreview);
    }
    else
    {
        // temporary code - restores original test animation to the preview window
        TAToolNAnimPreview.display(TAToolNAnimPreview.originalAnim);
        TAToolResetFlip(TAToolNAnimPreview.sprite);
        TAToolNAnimPreview.mirrorState = "";
        TAToolUpdateEdit(TAToolNAnimPreview);
    }
    TAToolNAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
    SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring an animation from Southwest to Northeast, Northwest or Southeast.
/// </summary>
function TAToolSWMirrorCheckBox::onClick(%this)
{
   TAToolSWMirrorDropDown.onSelect();
}

/// <summary>
/// This function handles mirroring an animation from West to East.
/// </summary>
function TAToolWMirrorCheckBox::onClick(%this)
{
    // -------------------------------------------------------------------------
    // Flip West to East
    // -------------------------------------------------------------------------
    %state = !%this.getValue();
    TAToolEMirrorCheckBox.setValue(false);
    TAToolEMirrorCheckBox.Visible = %state;
    if (%state == false)
    {
        TAToolEAnimPreview.display(TAToolWAnimPreview.animation);
        TAToolEAnimPreview.mirrorState = "Horizontal";
        TAToolSetFlip(TAToolEAnimPreview.sprite, "Horizontal");
        TAToolUpdateEdit(TAToolEAnimPreview);
    }
    else
    {
        // temporary code - restores original test animation to the preview window
        TAToolEAnimPreview.display(TAToolEAnimPreview.originalAnim);
        TAToolResetFlip(TAToolEAnimPreview.sprite);
        TAToolEAnimPreview.mirrorState = "";
        TAToolUpdateEdit(TAToolEAnimPreview);
    }
    TAToolEAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
    SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring an animation from Northwest to Southeast, Southwest or Northeast.
/// </summary>
function TAToolNWMirrorCheckBox::onClick(%this)
{
   TAToolNWMirrorDropDown.onSelect();
}

/// <summary>
/// This function handles mirroring based on the Northeast mirror dropdown selection.
/// </summary>
function TAToolNEMirrorDropDown::onSelect(%this)
{
    if (%this.resetting)
    {
        %this.resetting = false;
        return;
    }
        
   if (%this.active)
   {
      %mirrorState = !TAToolNEMirrorCheckBox.getValue();
      %this.Visible = TAToolNEMirrorCheckBox.getValue();
      %state = %this.getText();
      switch$ (%state)
      {
         case "Diagonal": // Diagonal Mirror
            if (!%mirrorState && (TAToolSWAnimPreview.mirrorState !$= "" || TAToolSWMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolSWMirrorCheckBox.getValue() == false && TAToolSWAnimPreview.mirrorState $= "")
            {
                TAToolSWMirrorCheckBox.Visible = false;
                TAToolSWMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolSWMirrorCheckBox;
                %this.animPreview = TAToolSWAnimPreview;
                TAToolSetFlip(TAToolSWAnimPreview.sprite, %state);
                TAToolSWAnimPreview.originalAnim = TAToolSWAnimPreview.sprite.getAnimation();
                TAToolSWAnimPreview.display(TAToolNEAnimPreview.animation);
                TAToolSWAnimPreview.mirrorState = %state;
                TAToolUpdateEdit(TAToolSWAnimPreview);
                TAToolSWAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolSWMirrorCheckBox.Visible = true;
                TAToolSWMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolSWAnimPreview.sprite);
                TAToolSWAnimPreview.display(TAToolSWAnimPreview.originalAnim);
                TAToolSWAnimPreview.mirrorState = "";
                TAToolUpdateEdit(TAToolSWAnimPreview);
                TAToolSWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }

            if (TAToolNWAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolNWMirrorCheckBox.Visible = true;
                TAToolNWMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNWAnimPreview.sprite);
                TAToolNWAnimPreview.display(TAToolNWAnimPreview.originalAnim);
                TAToolNWAnimPreview.mirrorState = "";
                TAToolNWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolNWAnimPreview);
            }

            if (TAToolSEAnimPreview.mirrorState $= "Vertical")
            {
                TAToolSEMirrorCheckBox.Visible = true;
                TAToolSEMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSEAnimPreview.sprite);
                TAToolSEAnimPreview.display(TAToolSEAnimPreview.originalAnim);
                TAToolSEAnimPreview.mirrorState = "";
                TAToolSEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolSEAnimPreview);
            }

         case "Horizontal": // Horizontal Mirror
            if (!%mirrorState && (TAToolNWAnimPreview.mirrorState !$= "" || TAToolNWMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolSWAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolSWMirrorCheckBox.Visible = true;
                TAToolSWMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSWAnimPreview.sprite);
                TAToolSWAnimPreview.display(TAToolSWAnimPreview.originalAnim);
                TAToolSWAnimPreview.mirrorState = "";
                TAToolSWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolSWAnimPreview);
            }
            
            if (TAToolNWMirrorCheckBox.getValue() == false && TAToolNWAnimPreview.mirrorState $= "")
            {
                TAToolNWMirrorCheckBox.Visible = false;
                TAToolNWMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolNWMirrorCheckBox;
                %this.animPreview = TAToolNWAnimPreview;
                TAToolSetFlip(TAToolNWAnimPreview.sprite, %state);
                TAToolNWAnimPreview.originalAnim = TAToolNWAnimPreview.sprite.getAnimation();
                TAToolNWAnimPreview.display(TAToolNEAnimPreview.animation);
                TAToolNWAnimPreview.mirrorState = %state;
                TAToolUpdateEdit(TAToolNWAnimPreview);
                TAToolNWAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolNWMirrorCheckBox.Visible = true;
                TAToolNWMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolNWAnimPreview.sprite);
                TAToolNWAnimPreview.display(TAToolNWAnimPreview.originalAnim);
                TAToolNWAnimPreview.mirrorState = "";
                TAToolUpdateEdit(TAToolNWAnimPreview);
                TAToolNWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }

            if (TAToolSEAnimPreview.mirrorState $= "Vertical")
            {
                TAToolSEMirrorCheckBox.Visible = true;
                TAToolSEMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSEAnimPreview.sprite);
                TAToolSEAnimPreview.display(TAToolSEAnimPreview.originalAnim);
                TAToolSEAnimPreview.mirrorState = "";
                TAToolSEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolSEAnimPreview);
            }
            
         case "Vertical": // Vertical Mirror
            if (!%mirrorState && (TAToolSEAnimPreview.mirrorState !$= "" || TAToolSEMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolSWAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolSWMirrorCheckBox.Visible = true;
                TAToolSWMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSWAnimPreview.sprite);
                TAToolSWAnimPreview.display(TAToolSWAnimPreview.originalAnim);
                TAToolSWAnimPreview.mirrorState = "";
                TAToolSWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolSWAnimPreview);
            }
            
            if (TAToolNWAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolNWMirrorCheckBox.Visible = true;
                TAToolNWMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNWAnimPreview.sprite);
                TAToolNWAnimPreview.display(TAToolNWAnimPreview.originalAnim);
                TAToolNWAnimPreview.mirrorState = "";
                TAToolNWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolNWAnimPreview);
            }

            if (TAToolSEMirrorCheckBox.getValue() == false && TAToolSEAnimPreview.mirrorState $= "")
            {
                TAToolSEMirrorCheckBox.Visible = false;
                TAToolSEMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolSEMirrorCheckBox;
                %this.animPreview = TAToolSEAnimPreview;
                TAToolSetFlip(TAToolSEAnimPreview.sprite, %state);
                TAToolSEAnimPreview.originalAnim = TAToolSEAnimPreview.sprite.getAnimation();
                TAToolSEAnimPreview.display(TAToolNEAnimPreview.animation);
                TAToolSEAnimPreview.mirrorState = %state;
                TAToolUpdateEdit(TAToolSEAnimPreview);
                TAToolSEAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolSEMirrorCheckBox.Visible = true;
                TAToolSEMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolSetFlip(TAToolSEAnimPreview.sprite, %state);
                TAToolSEAnimPreview.display(TAToolSEAnimPreview.originalAnim);
                TAToolSEAnimPreview.mirrorState = "";
                TAToolUpdateEdit(TAToolSEAnimPreview);
                TAToolSEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }
      }
   }

    if (TAToolNWMirrorCheckBox.getValue())
        TAToolNWMirrorDropDown.onSelect();
    
    if (TAToolSWMirrorCheckBox.getValue())
        TAToolSWMirrorDropDown.onSelect();
    
    if (TAToolSEMirrorCheckBox.getValue())
        TAToolSEMirrorDropDown.onSelect();
    
   %this.previous = %this.getText();
   SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring based on the Southeast mirror dropdown selection.
/// </summary>
function TAToolSEMirrorDropDown::onSelect(%this)
{
    if (%this.resetting)
    {
        %this.resetting = false;
        return;
    }
        
   if (%this.active)
   {
      %mirrorState = !TAToolSEMirrorCheckBox.getValue();
      %this.Visible = TAToolSEMirrorCheckBox.getValue();
      %state = %this.getText();
      switch$ (%state)
      {
         case "Diagonal": // Diagonal Mirror
            if (!%mirrorState && (TAToolNWAnimPreview.mirrorState !$= "" || TAToolNWMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolNWMirrorCheckBox.getValue() == false && TAToolNWAnimPreview.mirrorState $= "")
            {
                TAToolNWMirrorCheckBox.Visible = false;
                TAToolNWMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolNWMirrorCheckBox;
                %this.animPreview = TAToolNWAnimPreview;
                TAToolSetFlip(TAToolNWAnimPreview.sprite, %state);
                TAToolNWAnimPreview.originalAnim = TAToolNWAnimPreview.sprite.getAnimation();
                TAToolNWAnimPreview.display(TAToolSEAnimPreview.animation);
                TAToolNWAnimPreview.mirrorState = %state;
                TAToolUpdateEdit(TAToolNWAnimPreview);
                TAToolNWAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolNWMirrorCheckBox.Visible = true;
                TAToolNWMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolNWAnimPreview.sprite);
                TAToolNWAnimPreview.display(TAToolNWAnimPreview.originalAnim);
                TAToolNWAnimPreview.mirrorState = "";
                TAToolUpdateEdit(TAToolNWAnimPreview);
                TAToolNWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }

            if (TAToolSWAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolSWMirrorCheckBox.Visible = true;
                TAToolSWMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSWAnimPreview.sprite);
                TAToolSWAnimPreview.display(TAToolSWAnimPreview.originalAnim);
                TAToolSWAnimPreview.mirrorState = "";
                TAToolSWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolSWAnimPreview);
            }
            
            if (TAToolNEAnimPreview.mirrorState $= "Vertical")
            {
                TAToolNEMirrorCheckBox.Visible = true;
                TAToolNEMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNEAnimPreview.sprite);
                TAToolNEAnimPreview.display(TAToolNEAnimPreview.originalAnim);
                TAToolNEAnimPreview.mirrorState = "";
                TAToolNEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolNEAnimPreview);
            }

         case "Horizontal": // Horizontal Mirror
            if (!%mirrorState && (TAToolSWAnimPreview.mirrorState !$= "" || TAToolSWMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolNWAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolNWMirrorCheckBox.Visible = true;
                TAToolNWMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNWAnimPreview.sprite);
                TAToolNWAnimPreview.display(TAToolNWAnimPreview.originalAnim);
                TAToolNWAnimPreview.mirrorState = "";
                TAToolNWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolNWAnimPreview);
            }

            if (TAToolSWMirrorCheckBox.getValue() == false && TAToolSWAnimPreview.mirrorState $= "")
            {
                TAToolSWMirrorCheckBox.Visible = false;
                TAToolSWMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolSWMirrorCheckBox;
                %this.animPreview = TAToolSWAnimPreview;
                TAToolSetFlip(TAToolSWAnimPreview.sprite, %state);
                TAToolSWAnimPreview.originalAnim = TAToolSWAnimPreview.sprite.getAnimation();
                TAToolSWAnimPreview.display(TAToolSEAnimPreview.animation);
                TAToolSWAnimPreview.mirrorState = %state;
                TAToolUpdateEdit(TAToolSWAnimPreview);
                TAToolSWAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolSWMirrorCheckBox.Visible = true;
                TAToolSWMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolSWAnimPreview.sprite);
                TAToolSWAnimPreview.display(TAToolSWAnimPreview.originalAnim);
                TAToolSWAnimPreview.mirrorState = "";
                TAToolUpdateEdit(TAToolSWAnimPreview);
                TAToolSWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }

            if (TAToolNEAnimPreview.mirrorState $= "Vertical")
            {
                TAToolNEMirrorCheckBox.Visible = true;
                TAToolNEMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNEAnimPreview.sprite);
                TAToolNEAnimPreview.display(TAToolNEAnimPreview.originalAnim);
                TAToolNEAnimPreview.mirrorState = "";
                TAToolNEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolNEAnimPreview);
            }

         case "Vertical": // Vertical Mirror
            if (!%mirrorState && (TAToolNEAnimPreview.mirrorState !$= "" || TAToolNEMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolNWAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolNWMirrorCheckBox.Visible = true;
                TAToolNWMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNWAnimPreview.sprite);
                TAToolNWAnimPreview.display(TAToolNWAnimPreview.originalAnim);
                TAToolNWAnimPreview.mirrorState = "";
                TAToolNWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolNWAnimPreview);
            }

            if (TAToolSWAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolSWMirrorCheckBox.Visible = true;
                TAToolSWMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSWAnimPreview.sprite);
                TAToolSWAnimPreview.display(TAToolSWAnimPreview.originalAnim);
                TAToolSWAnimPreview.mirrirState = "";
                TAToolSWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolSWAnimPreview);
            }

            if (TAToolNEMirrorCheckBox.getValue() == false && TAToolNEAnimPreview.mirrorState $= "")
            {
                TAToolNEMirrorCheckBox.Visible = false;
                TAToolNEMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolNEMirrorCheckBox;
                %this.animPreview = TAToolNEAnimPreview;
                TAToolSetFlip(TAToolNEAnimPreview.sprite, %state);
                TAToolNEAnimPreview.originalAnim = TAToolNEAnimPreview.sprite.getAnimation();
                TAToolNEAnimPreview.display(TAToolSEAnimPreview.animation);
                TAToolNEAnimPreview.mirrorState = %state;
                TAToolUpdateEdit(TAToolNEAnimPreview);
                TAToolNEAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolNEMirrorCheckBox.Visible = true;
                TAToolNEMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolNEAnimPreview.sprite);
                TAToolNEAnimPreview.display(TAToolNEAnimPreview.originalAnim);
                TAToolNEAnimPreview.mirrorState = "";
                TAToolUpdateEdit(TAToolNEAnimPreview);
                TAToolNEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }
      }
   }

    if (TAToolNEMirrorCheckBox.getValue())
        TAToolNEMirrorDropDown.onSelect();
    
    if (TAToolNWMirrorCheckBox.getValue())
        TAToolNWMirrorDropDown.onSelect();
        
    if (TAToolSWMirrorCheckBox.getValue())
        TAToolSWMirrorDropDown.onSelect();

   %this.previous = %this.getText();
   SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring based on the Southwest mirror dropdown selection.
/// </summary>
function TAToolSWMirrorDropDown::onSelect(%this)
{
    if (%this.resetting)
    {
        %this.resetting = false;
        return;
    }
        
   if (%this.active)
   {
      %mirrorState = !TAToolSWMirrorCheckBox.getValue();
      %this.Visible = TAToolSWMirrorCheckBox.getValue();
      %state = %this.getText();
      switch$ (%state)
      {
         case "Diagonal": // Diagonal Mirror
            if (!%mirrorState && (TAToolNEAnimPreview.mirrorState !$= "" || TAToolNEMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolNEMirrorCheckBox.getValue() == false && TAToolNEAnimPreview.mirrorState $= "")
            {
                TAToolNEMirrorCheckBox.Visible = false;
                TAToolNEMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolNEMirrorCheckBox;
                %this.animPreview = TAToolNEAnimPreview;
                TAToolSetFlip(TAToolNEAnimPreview.sprite, %state);
                TAToolNEAnimPreview.originalAnim = TAToolNEAnimPreview.sprite.getAnimation();
                TAToolNEAnimPreview.display(TAToolSWAnimPreview.animation);
                TAToolNEAnimPreview.mirrorState = %state;
                TAToolUpdateEdit(TAToolNEAnimPreview);
                TAToolNEAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolNEMirrorCheckBox.Visible = true;
                TAToolNEMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolNEAnimPreview.sprite);
                TAToolNEAnimPreview.display(TAToolNEAnimPreview.originalAnim);
                TAToolNEAnimPreview.mirrorState = "";
                TAToolUpdateEdit(TAToolNEAnimPreview);
                TAToolNEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }
            
            if (TAToolSEAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolSEMirrorCheckBox.Visible = true;
                TAToolSEMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSEAnimPreview.sprite);
                TAToolSEAnimPreview.display(TAToolSEAnimPreview.originalAnim);
                TAToolSEAnimPreview.mirrorState = "";
                TAToolSEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolSEAnimPreview);
            }
            
            if (TAToolNWAnimPreview.mirrorState $= "Vertical")
            {
                TAToolNWMirrorCheckBox.Visible = true;
                TAToolNWMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNWAnimPreview.sprite);
                TAToolNWAnimPreview.display(TAToolNWAnimPreview.originalAnim);
                TAToolNWAnimPreview.mirrorState = "";
                TAToolNWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolNWAnimPreview);
            }

         case "Horizontal": // Horizontal Mirror
            if (!%mirrorState && (TAToolSEAnimPreview.mirrorState !$= "" || TAToolSEMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolNEAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolNEMirrorCheckBox.Visible = true;
                TAToolNEMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNEAnimPreview.sprite);
                TAToolNEAnimPreview.display(TAToolNEAnimPreview.originalAnim);
                TAToolNEAnimPreview.mirrorState = "";
                TAToolNEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolNEAnimPreview);
            }

            if (TAToolSEMirrorCheckBox.getValue() == false && TAToolSEAnimPreview.mirrorState $= "")
            {
                TAToolSEMirrorCheckBox.Visible = false;
                TAToolSEMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolSEMirrorCheckBox;
                %this.animPreview = TAToolSEAnimPreview;
                TAToolSetFlip(TAToolSEAnimPreview.sprite, %state);
                TAToolSEAnimPreview.originalAnim = TAToolSEAnimPreview.sprite.getAnimation();
                TAToolSEAnimPreview.display(TAToolSWAnimPreview.animation);
                TAToolSEAnimPreview.mirrorState = %state;
                TAToolUpdateEdit(TAToolSEAnimPreview);
                TAToolSEAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolSEMirrorCheckBox.Visible = true;
                TAToolSEMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolSEAnimPreview.sprite);
                TAToolSEAnimPreview.display(TAToolSEAnimPreview.originalAnim);
                TAToolSEAnimPreview.mirrorState = "";
                TAToolUpdateEdit(TAToolSEAnimPreview);
                TAToolSEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }
            
            if (TAToolNWAnimPreview.mirrorState $= "Vertical")
            {
                TAToolNWMirrorCheckBox.Visible = true;
                TAToolNWMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNWAnimPreview.sprite);
                TAToolNWAnimPreview.display(TAToolNWAnimPreview.originalAnim);
                TAToolNWAnimPreview.mirrorState = "";
                TAToolNWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolNWAnimPreview);
            }

         case "Vertical": // Vertical Mirror
            if (!%mirrorState && (TAToolNWAnimPreview.mirrorState !$= "" || TAToolNWMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolNEAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolNEMirrorCheckBox.Visible = true;
                TAToolNEMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNEAnimPreview.sprite);
                TAToolNEAnimPreview.display(TAToolNEAnimPreview.originalAnim);
                TAToolNEAnimPreview.mirrorState = "";
                TAToolNEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolNEAnimPreview);
            }
            
            if (TAToolSEAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolSEMirrorCheckBox.Visible = true;
                TAToolSEMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSEAnimPreview.sprite);
                TAToolSEAnimPreview.display(TAToolSEAnimPreview.originalAnim);
                TAToolSEAnimPreview.mirrorState = "";
                TAToolSEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolSEAnimPreview);
            }
            
            if (TAToolNWMirrorCheckBox.getValue() == false && TAToolNWAnimPreview.mirrorState $= "")
            {
                TAToolNWMirrorCheckBox.Visible = false;
                TAToolNWMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolNWMirrorCheckBox;
                %this.animPreview = TAToolNWAnimPreview;
                TAToolSetFlip(TAToolNWAnimPreview.sprite, %state);
                TAToolNWAnimPreview.originalAnim = TAToolNWAnimPreview.sprite.getAnimation();
                TAToolNWAnimPreview.display(TAToolSWAnimPreview.animation);
                TAToolNWAnimPreview.mirrorState = %state;
                TAToolUpdateEdit(TAToolNWAnimPreview);
                TAToolNWAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolNWMirrorCheckBox.Visible = true;
                TAToolNWMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolNWAnimPreview.sprite);
                TAToolNWAnimPreview.display(TAToolNWAnimPreview.originalAnim);
                TAToolNWAnimPreview.mirrorState = "";
                TAToolUpdateEdit(TAToolNWAnimPreview);
                TAToolNWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }
      }
   }

    if (TAToolNWMirrorCheckBox.getValue())
        TAToolNWMirrorDropDown.onSelect();
        
    if (TAToolNEMirrorCheckBox.getValue())
        TAToolNEMirrorDropDown.onSelect();
        
    if (TAToolSEMirrorCheckBox.getValue())
        TAToolSEMirrorDropDown.onSelect();

   %this.previous = %this.getText();
   SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring based on the Northwest mirror dropdown selection.
/// </summary>
function TAToolNWMirrorDropDown::onSelect(%this)
{
    if (%this.resetting)
    {
        %this.resetting = false;
        return;
    }
        
   if (%this.active)
   {
      %mirrorState = !TAToolNWMirrorCheckBox.getValue();
      %this.Visible = TAToolNWMirrorCheckBox.getValue();
      %state = %this.getText();
      switch$ (%state)
      {
         case "Diagonal": // Diagonal Mirror
            if (!%mirrorState && (TAToolSEAnimPreview.mirrorState !$= "" || TAToolSEMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolSEMirrorCheckBox.getValue() == false && TAToolSEAnimPreview.mirrorState $= "")
            {
                TAToolSEMirrorCheckBox.Visible = false;
                TAToolSEMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolSEMirrorCheckBox;
                %this.animPreview = TAToolSEAnimPreview;
                TAToolSetFlip(TAToolSEAnimPreview.sprite, %state);
                TAToolSEAnimPreview.originalAnim = TAToolSEAnimPreview.sprite.getAnimation();
                TAToolSEAnimPreview.display(TAToolNWAnimPreview.animation);
                TAToolSEAnimPreview.mirrorState = %state;
                TAToolUpdateEdit(TAToolSEAnimPreview);
                TAToolSEAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolSEMirrorCheckBox.Visible = true;
                TAToolSEMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolSEAnimPreview.sprite);
                TAToolSEAnimPreview.display(TAToolSEAnimPreview.originalAnim);
                TAToolSEAnimPreview.mirrorState = "";
                TAToolUpdateEdit(TAToolSEAnimPreview);
                TAToolSEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }
            
            if (TAToolNEAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolNEMirrorCheckBox.Visible = true;
                TAToolNEMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNEAnimPreview.sprite);
                TAToolNEAnimPreview.display(TAToolNEAnimPreview.originalAnim);
                TAToolNEAnimPreview.mirrorState = "";
                TAToolNEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolNEAnimPreview);
            }

            if (TAToolSWAnimPreview.mirrorState $= "Vertical")
            {
                TAToolSWMirrorCheckBox.Visible = true;
                TAToolSWMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSWAnimPreview.sprite);
                TAToolSWAnimPreview.display(TAToolSWAnimPreview.originalAnim);
                TAToolSWAnimPreview.mirrorState = "";
                TAToolSWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolSWAnimPreview);
            }

         case "Horizontal": // Horizontal Mirror
            if (!%mirrorState && (TAToolNEAnimPreview.mirrorState !$= "" || TAToolNEMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolSEAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolSEMirrorCheckBox.Visible = true;
                TAToolSEMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSEAnimPreview.sprite);
                TAToolSEAnimPreview.display(TAToolSEAnimPreview.originalAnim);
                TAToolSEAnimPreview.mirrorState = "";
                TAToolSEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolSEAnimPreview);
            }
            
            if (TAToolNEMirrorCheckBox.getValue() == false && TAToolNEAnimPreview.mirrorState $= "")
            {
                TAToolNEMirrorCheckBox.Visible = false;
                TAToolNEMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolNEMirrorCheckBox;
                %this.animPreview = TAToolNEAnimPreview;
                TAToolSetFlip(TAToolNEAnimPreview.sprite, %state);
                TAToolNEAnimPreview.originalAnim = TAToolNEAnimPreview.sprite.getAnimation();
                TAToolNEAnimPreview.display(TAToolNWAnimPreview.animation);
                TAToolNEAnimPreview.mirrorState = %state;
                TAToolUpdateEdit(TAToolNEAnimPreview);
                TAToolNEAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolNEMirrorCheckBox.Visible = true;
                TAToolNEMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolNEAnimPreview.sprite);
                TAToolNEAnimPreview.display(TAToolNEAnimPreview.originalAnim);
                TAToolNEAnimPreview.mirrorState = "";
                TAToolUpdateEdit(TAToolNEAnimPreview);
                TAToolNEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }
            
            if (TAToolSWAnimPreview.mirrorState $= "Vertical")
            {
                TAToolSWMirrorCheckBox.Visible = true;
                TAToolSWMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSWAnimPreview.sprite);
                TAToolSWAnimPreview.display(TAToolSWAnimPreview.originalAnim);
                TAToolSWAnimPreview.mirrorState = "";
                TAToolSWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolSWAnimPreview);
            }

         case "Vertical": // Vertical Mirror
            if (!%mirrorState && (TAToolSWAnimPreview.mirrorState !$= "" || TAToolSWMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolSEAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolSEMirrorCheckBox.Visible = true;
                TAToolSEMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSEAnimPreview.sprite);
                TAToolSEAnimPreview.display(TAToolSEAnimPreview.originalAnim);
                TAToolSEAnimPreview.mirrorState = "";
                TAToolSEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolSEAnimPreview);
            }
            
            if (TAToolNEAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolNEMirrorCheckBox.Visible = true;
                TAToolNEMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNEAnimPreview.sprite);
                TAToolNEAnimPreview.display(TAToolNEAnimPreview.originalAnim);
                TAToolNEAnimPreview.mirrorState = "";
                TAToolNEAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateEdit(TAToolNEAnimPreview);
            }

            if (TAToolSWMirrorCheckBox.getValue() == false && TAToolSWAnimPreview.mirrorState $= "")
            {
                TAToolSWMirrorCheckBox.Visible = false;
                TAToolSWMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolSWMirrorCheckBox;
                %this.animPreview = TAToolSWAnimPreview;
                TAToolSetFlip(TAToolSWAnimPreview.sprite, %state);
                TAToolSWAnimPreview.originalAnim = TAToolSWAnimPreview.sprite.getAnimation();
                TAToolSWAnimPreview.display(TAToolNWAnimPreview.animation);
                TAToolSWAnimPreview.mirrorState = %state;
                TAToolUpdateEdit(TAToolSWAnimPreview);
                TAToolSWAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolSWMirrorCheckBox.Visible = true;
                TAToolSWMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolSWAnimPreview.sprite);
                TAToolSWAnimPreview.display(TAToolSWAnimPreview.originalAnim);
                TAToolSWAnimPreview.mirrorState = "";
                TAToolUpdateEdit(TAToolSWAnimPreview);
                TAToolSWAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }
      }
   }
    
    if (TAToolNEMirrorCheckBox.getValue())
        TAToolNEMirrorDropDown.onSelect();
        
    if (TAToolSEMirrorCheckBox.getValue())
        TAToolSEMirrorDropDown.onSelect();
    
    if (TAToolSWMirrorCheckBox.getValue())
        TAToolSWMirrorDropDown.onSelect();

   %this.previous = %this.getText();
   SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolNIdleAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolNIdleAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolNIdleAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolNIdleAnimPreview.originalAnim = %asset;
   TAToolNIdleAnimEdit.text = %asset;

   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolNIdleAnimPreview.mirrorState !$= "")
   {
      TAToolSIdleMirrorCheckBox.setStateOn(false);
      TAToolNIdleMirrorCheckBox.Visible = true;
      TAToolNIdleAnimPreview.mirrorState = "";
      TAToolResetFlip(TAToolNIdleAnimPreview.sprite);
      TAToolNIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
   }
   
   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolSIdleAnimPreview.mirrorState !$= "")
   {
      TAToolNIdleAnimPreview.originalAnim = TAToolNIdleAnimPreview.sprite.getAnimation();
      TAToolNIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolNIdleAnimEdit.text = %asset;
   }

   SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolNEIdleAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolNEIdleAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolNEIdleAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolNEIdleAnimPreview.originalAnim = %asset;
   TAToolNEIdleAnimEdit.text = %asset;
   
   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolNEIdleAnimPreview.mirrorState !$= "")
   {
      switch$(TAToolNEIdleAnimPreview.mirrorState)
      {
         case "Diagonal":
            TAToolSWIdleMirrorCheckBox.setStateOn(false);
            TAToolSWIdleMirrorDropDown.Visible = false;
         
         case "Horizontal":
            TAToolNWIdleMirrorCheckBox.setStateOn(false);
            TAToolNWIdleMirrorDropDown.Visible = false;
         
         case "Vertical":
            TAToolSEIdleMirrorCheckBox.setStateOn(false);
            TAToolSEIdleMirrorDropDown.Visible = false;
         
      }
      TAToolNEIdleMirrorCheckBox.Visible = true;
      TAToolNEIdleAnimPreview.mirrorState = "";
      TAToolNEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
      TAToolResetFlip(TAToolNEIdleAnimPreview.sprite);
   }

   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolSWIdleAnimPreview.mirrorState $= "Diagonal")
   {
      TAToolSWIdleAnimPreview.originalAnim = TAToolSWIdleAnimPreview.sprite.getAnimation();
      TAToolSWIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolSWIdleAnimEdit.text = %asset;
   }
   if (TAToolSEIdleAnimPreview.mirrorState $= "Vertical")
   {
      TAToolSEIdleAnimPreview.originalAnim = TAToolSEIdleAnimPreview.sprite.getAnimation();
      TAToolSEIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolSEIdleAnimEdit.text = %asset;
   }
   if (TAToolNWIdleAnimPreview.mirrorState $= "Horizontal")
   {
      TAToolNWIdleAnimPreview.originalAnim = TAToolNWIdleAnimPreview.sprite.getAnimation();
      TAToolNWIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolNWIdleAnimEdit.text = %asset;
   }
   
   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolEIdleAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolEIdleAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolEIdleAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolEIdleAnimPreview.originalAnim = %asset;
   TAToolEIdleAnimEdit.text = %asset;

   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolEIdleAnimPreview.mirrorState !$= "")
   {
      TAToolWIdleMirrorCheckBox.setStateOn(false);
      TAToolEIdleMirrorCheckBox.Visible = true;
      TAToolEIdleAnimPreview.mirrorState = "";
      TAToolResetFlip(TAToolEIdleAnimPreview.sprite);
      TAToolEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
   }
   
   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolWIdleAnimPreview.mirrorState !$= "")
   {
      TAToolWIdleAnimPreview.originalAnim = TAToolWIdleAnimPreview.sprite.getAnimation();
      TAToolWIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolWIdleAnimEdit.text = %asset;
   }

   SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolSEIdleAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolSEIdleAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolSEIdleAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolSEIdleAnimPreview.originalAnim = %asset;
   TAToolSEIdleAnimEdit.text = %asset;
   
   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolSEIdleAnimPreview.mirrorState !$= "")
   {
      switch$(TAToolSEIdleAnimPreview.mirrorState)
      {
         case "Diagonal":
            TAToolNWIdleMirrorCheckBox.setStateOn(false);
            TAToolNWIdleMirrorDropDown.Visible = false;
         
         case "Horizontal":
            TAToolSWIdleMirrorCheckBox.setStateOn(false);
            TAToolSWIdleMirrorDropDown.Visible = false;
         
         case "Vertical":
            TAToolNEIdleMirrorCheckBox.setStateOn(false);
            TAToolNEIdleMirrorDropDown.Visible = false;
         
      }
      TAToolSEIdleMirrorCheckBox.Visible = true;
      TAToolSEIdleAnimPreview.mirrorState = "";
      TAToolSEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
      TAToolResetFlip(TAToolSEIdleAnimPreview.sprite);
   }

   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolSWIdleAnimPreview.mirrorState $= "Horizontal")
   {
      TAToolSWIdleAnimPreview.originalAnim = TAToolSWIdleAnimPreview.sprite.getAnimation();
      TAToolSWIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolSWIdleAnimEdit.text = %asset;
   }
   if (TAToolNEIdleAnimPreview.mirrorState $= "Vertical")
   {
      TAToolNEIdleAnimPreview.originalAnim = TAToolNEIdleAnimPreview.sprite.getAnimation();
      TAToolNEIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolNEIdleAnimEdit.text = %asset;
   }
   if (TAToolNWIdleAnimPreview.mirrorState $= "Diagonal")
   {
      TAToolNWIdleAnimPreview.originalAnim = TAToolNWIdleAnimPreview.sprite.getAnimation();
      TAToolNWIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolNWIdleAnimEdit.text = %asset;
   }
   
   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolSIdleAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolSIdleAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolSIdleAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolSIdleAnimPreview.originalAnim = %asset;
   TAToolSIdleAnimEdit.text = %asset;

   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolSIdleAnimPreview.mirrorState !$= "")
   {
      TAToolNIdleMirrorCheckBox.setStateOn(false);
      TAToolSIdleMirrorCheckBox.Visible = true;
      TAToolSIdleAnimPreview.mirrorState = "";
      TAToolSIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
      TAToolResetFlip(TAToolSIdleAnimPreview.sprite);
   }
   
   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolNIdleAnimPreview.mirrorState !$= "")
   {
      TAToolNIdleAnimPreview.originalAnim = TAToolNIdleAnimPreview.sprite.getAnimation();
      TAToolNIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolNIdleAnimEdit.text = %asset;
   }

   SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolSWIdleAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolSWIdleAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolSWIdleAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolSWIdleAnimPreview.originalAnim = %asset;
   TAToolSWIdleAnimEdit.text = %asset;
   
   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolSWIdleAnimPreview.mirrorState !$= "")
   {
      switch$(TAToolSWIdleAnimPreview.mirrorState)
      {
         case "Diagonal":
            TAToolNEIdleMirrorCheckBox.setStateOn(false);
            TAToolNEIdleMirrorDropDown.Visible = false;
         
         case "Horizontal":
            TAToolSEIdleMirrorCheckBox.setStateOn(false);
            TAToolSEIdleMirrorDropDown.Visible = false;
         
         case "Vertical":
            TAToolNWIdleMirrorCheckBox.setStateOn(false);
            TAToolNWIdleMirrorDropDown.Visible = false;
         
      }
      TAToolSWIdleMirrorCheckBox.Visible = true;
      TAToolSWIdleAnimPreview.mirrorState = "";
      TAToolSWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
      TAToolResetFlip(TAToolSWIdleAnimPreview.sprite);
   }

   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolNEIdleAnimPreview.mirrorState $= "Diagonal")
   {
      TAToolNEIdleAnimPreview.originalAnim = TAToolNEIdleAnimPreview.sprite.getAnimation();
      TAToolNEIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolNEIdleAnimEdit.text = %asset;
   }
   if (TAToolSEIdleAnimPreview.mirrorState $= "Horizontal")
   {
      TAToolSEIdleAnimPreview.originalAnim = TAToolSEIdleAnimPreview.sprite.getAnimation();
      TAToolSEIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolSEIdleAnimEdit.text = %asset;
   }
   if (TAToolNWIdleAnimPreview.mirrorState $= "Vertical")
   {
      TAToolNWIdleAnimPreview.originalAnim = TAToolNWIdleAnimPreview.sprite.getAnimation();
      TAToolNWIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolNWIdleAnimEdit.text = %asset;
   }
   
   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolWIdleAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolWIdleAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolWIdleAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolWIdleAnimPreview.originalAnim = %asset;
   TAToolWIdleAnimEdit.text = %asset;

   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolWIdleAnimPreview.mirrorState !$= "")
   {
      TAToolEIdleMirrorCheckBox.setStateOn(false);
      TAToolWIdleMirrorCheckBox.Visible = true;
      TAToolWIdleAnimPreview.mirrorState = "";
      TAToolWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
      TAToolResetFlip(TAToolWIdleAnimPreview.sprite);
   }
   
   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolEIdleAnimPreview.mirrorState !$= "")
   {
      TAToolEIdleAnimPreview.originalAnim = TAToolEIdleAnimPreview.sprite.getAnimation();
      TAToolEIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolEIdleAnimEdit.text = %asset;
   }

   SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the 
/// requesting animation slot.
/// </summary>
function TAToolNWIdleAnimSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation fromt the Asset Library to the requesting
/// animation slot.  If it is mirrored to another slot, that slot is also updated.
/// </summary>
/// <param name="asset">The animation returned from the Asset Library.</param>
function TAToolNWIdleAnimSelectButton::setSelectedAsset(%this, %asset)
{
   TAToolNWIdleAnimPreview.display(%asset, t2dAnimatedSprite);
   TAToolNWIdleAnimPreview.originalAnim = %asset;
   TAToolNWIdleAnimEdit.text = %asset;
   
   // if this animation is mirrored from another direction clear this 
   // mirrored state and update the mirroring control's state
   if (TAToolNWIdleAnimPreview.mirrorState !$= "")
   {
      switch$(TAToolNWIdleAnimPreview.mirrorState)
      {
         case "Diagonal":
            TAToolSEIdleMirrorCheckBox.setStateOn(false);
            TAToolSEIdleMirrorDropDown.Visible = false;
         
         case "Horizontal":
            TAToolNEIdleMirrorCheckBox.setStateOn(false);
            TAToolNEIdleMirrorDropDown.Visible = false;
         
         case "Vertical":
            TAToolSWIdleMirrorCheckBox.setStateOn(false);
            TAToolSWIdleMirrorDropDown.Visible = false;
         
      }
      TAToolNWIdleMirrorCheckBox.Visible = true;
      TAToolNWIdleAnimPreview.mirrorState = "";
      TAToolNWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
      TAToolResetFlip(TAToolNWIdleAnimPreview.sprite);
   }

   // if this animation is mirrored to another direction update the 
   // mirrored animation
   if (TAToolSWIdleAnimPreview.mirrorState $= "Vertical")
   {
      TAToolSWIdleAnimPreview.originalAnim = TAToolSWIdleAnimPreview.sprite.getAnimation();
      TAToolSWIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolSWIdleAnimEdit.text = %asset;
   }
   if (TAToolSEIdleAnimPreview.mirrorState $= "Diagonal")
   {
      TAToolSEIdleAnimPreview.originalAnim = TAToolSEIdleAnimPreview.sprite.getAnimation();
      TAToolSEIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolSEIdleAnimEdit.text = %asset;
   }
   if (TAToolNEIdleAnimPreview.mirrorState $= "Horizontal")
   {
      TAToolNEIdleAnimPreview.originalAnim = TAToolNEIdleAnimPreview.sprite.getAnimation();
      TAToolNEIdleAnimPreview.display(%asset, t2dAnimatedSprite);
      TAToolNEIdleAnimEdit.text = %asset;
   }
   
   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring an animation from North to South.
/// </summary>
function TAToolNIdleMirrorCheckBox::onClick(%this)
{
    // -------------------------------------------------------------------------
    // Flip North to South
    // -------------------------------------------------------------------------
    %state = !%this.getValue();
    TAToolSIdleMirrorCheckBox.setValue(false);
    TAToolSIdleMirrorCheckBox.Visible = %state;
    if (%state == false)
    {
        // if %state is false, then the box is checked so mirror the animation.
        TAToolSIdleAnimPreview.display(TAToolNIdleAnimPreview.animation);
        TAToolSetFlip(TAToolSIdleAnimPreview.sprite, "Vertical");
        TAToolSIdleAnimPreview.mirrorState = "Vertical";
        TAToolUpdateIdleEdit(TAToolSIdleAnimPreview);
    }
    else
    {
        // temporary code - restores original test animation to the preview window
        TAToolSIdleAnimPreview.display(TAToolSIdleAnimPreview.originalAnim);
        TAToolResetFlip(TAToolSIdleAnimPreview.sprite);
        TAToolSIdleAnimPreview.mirrorState = "";
        TAToolUpdateIdleEdit(TAToolSIdleAnimPreview);
    }
    TAToolSIdleAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
    SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring an animation from Northeast to Southwest, Southeast or Northwest.
/// </summary>
function TAToolNEIdleMirrorCheckBox::onClick(%this)
{
    TAToolNEIdleMirrorDropDown.onSelect();
}

/// <summary>
/// This function handles mirroring an animation from East to West.
/// </summary>
function TAToolEIdleMirrorCheckBox::onClick(%this)
{
    // -------------------------------------------------------------------------
    // Flip East to West
    // -------------------------------------------------------------------------
    %state = !%this.getValue();
    TAToolWIdleMirrorCheckBox.setValue(false);
    TAToolWIdleMirrorCheckBox.Visible = %state;
    if (%state == false)
    {
        TAToolWIdleAnimPreview.display(TAToolEIdleAnimPreview.animation);
        TAToolWIdleAnimPreview.mirrorState = "Horizontal";
        TAToolSetFlip(TAToolWIdleAnimPreview.sprite, "Horizontal");
        TAToolUpdateIdleEdit(TAToolWIdleAnimPreview);
    }
    else
    {
        // temporary code - restores original test animation to the preview window
        TAToolWIdleAnimPreview.display(TAToolWIdleAnimPreview.originalAnim);
        TAToolResetFlip(TAToolWIdleAnimPreview.sprite);
        TAToolWIdleAnimPreview.mirrorState = "";
        TAToolUpdateIdleEdit(TAToolWIdleAnimPreview);
    }
    TAToolWIdleAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
    SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring an animation from Southeast to Northwest, Northeast or Southwest.
/// </summary>
function TAToolSEIdleMirrorCheckBox::onClick(%this)
{
    TAToolSEIdleMirrorDropDown.onSelect();
}

/// <summary>
/// This function handles mirroring an animation from South to North.
/// </summary>
function TAToolSIdleMirrorCheckBox::onClick(%this)
{
    // -------------------------------------------------------------------------
    // Flip South to North
    // -------------------------------------------------------------------------
    %state = !%this.getValue();
    TAToolNIdleMirrorCheckBox.setValue(false);
    TAToolNIdleMirrorCheckBox.Visible = %state;
    if (%state == false)
    {
        TAToolNIdleAnimPreview.display(TAToolSIdleAnimPreview.animation);
        TAToolNIdleAnimPreview.mirrorState = "Vertical";
        TAToolSetFlip(TAToolNIdleAnimPreview.sprite, "Vertical");
        TAToolUpdateIdleEdit(TAToolNIdleAnimPreview);
    }
    else
    {
        // temporary code - restores original test animation to the preview window
        TAToolNIdleAnimPreview.display(TAToolNIdleAnimPreview.originalAnim);
        TAToolResetFlip(TAToolNIdleAnimPreview.sprite);
        TAToolNIdleAnimPreview.mirrorState = "";
        TAToolUpdateIdleEdit(TAToolNIdleAnimPreview);
    }
    TAToolNIdleAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
    SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring an animation from Southwest to Northeast, Northwest or Southeast.
/// </summary>
function TAToolSWIdleMirrorCheckBox::onClick(%this)
{
    TAToolSWIdleMirrorDropDown.onSelect();
}

/// <summary>
/// This function handles mirroring an animation from West to East.
/// </summary>
function TAToolWIdleMirrorCheckBox::onClick(%this)
{
    // -------------------------------------------------------------------------
    // Flip West to East
    // -------------------------------------------------------------------------
    %state = !%this.getValue();
    TAToolEIdleMirrorCheckBox.setValue(false);
    TAToolEIdleMirrorCheckBox.Visible = %state;
    if (%state == false)
    {
        TAToolEIdleAnimPreview.display(TAToolWIdleAnimPreview.animation);
        TAToolEIdleAnimPreview.mirrorState = "Horizontal";
        TAToolSetFlip(TAToolEIdleAnimPreview.sprite, "Horizontal");
        TAToolUpdateIdleEdit(TAToolEIdleAnimPreview);
    }
    else
    {
        // temporary code - restores original test animation to the preview window
        TAToolEIdleAnimPreview.display(TAToolEIdleAnimPreview.originalAnim);
        TAToolResetFlip(TAToolEIdleAnimPreview.sprite);
        TAToolEIdleAnimPreview.mirrorState = "";
        TAToolUpdateIdleEdit(TAToolEIdleAnimPreview);
    }
    TAToolEIdleAnimPreview.sprite.BlendColor = (%state == 0 ? $TAToolMirroredColor : $TAToolOriginalColor);
    SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring an animation from Northwest to Southeast, Southwest or Northeast.
/// </summary>
function TAToolNWIdleMirrorCheckBox::onClick(%this)
{
    TAToolNWIdleMirrorDropDown.onSelect();
}

/// <summary>
/// This function handles mirroring based on the Northeast mirror dropdown selection.
/// </summary>
function TAToolNEIdleMirrorDropDown::onSelect(%this)
{
    if (%this.resetting)
    {
        %this.resetting = false;
        return;
    }
        
   if (%this.active)
   {
      %mirrorState = !TAToolNEIdleMirrorCheckBox.getValue();
      %this.Visible = TAToolNEIdleMirrorCheckBox.getValue();
      %state = %this.getText();
      switch$ (%state)
      {
         case "Diagonal": // Diagonal Mirror
            if (!%mirrorState && (TAToolSWIdleAnimPreview.mirrorState !$= "" || TAToolSWIdleMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolSWIdleMirrorCheckBox.getValue() == false && TAToolSWIdleAnimPreview.mirrorState $= "")
            {
                TAToolSWIdleMirrorCheckBox.Visible = false;
                TAToolSWIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolSWIdleMirrorCheckBox;
                %this.animPreview = TAToolSWIdleAnimPreview;
                TAToolSetFlip(TAToolSWIdleAnimPreview.sprite, %state);
                TAToolSWIdleAnimPreview.originalAnim = TAToolSWIdleAnimPreview.sprite.getAnimation();
                TAToolSWIdleAnimPreview.display(TAToolNEIdleAnimPreview.animation);
                TAToolSWIdleAnimPreview.mirrorState = %state;
                TAToolUpdateIdleEdit(TAToolSWIdleAnimPreview);
                TAToolSWIdleAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolSWIdleMirrorCheckBox.Visible = true;
                TAToolSWIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolSWIdleAnimPreview.sprite);
                TAToolSWIdleAnimPreview.display(TAToolSWIdleAnimPreview.originalAnim);
                TAToolSWIdleAnimPreview.mirrorState = "";
                TAToolUpdateIdleEdit(TAToolSWIdleAnimPreview);
                TAToolSWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }

            if (TAToolNWIdleAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolNWIdleMirrorCheckBox.Visible = true;
                TAToolNWIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNWIdleAnimPreview.sprite);
                TAToolNWIdleAnimPreview.display(TAToolNWIdleAnimPreview.originalAnim);
                TAToolNWIdleAnimPreview.mirrorState = "";
                TAToolNWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolNWIdleAnimPreview);
            }

            if (TAToolSEIdleAnimPreview.mirrorState $= "Vertical")
            {
                TAToolSEIdleMirrorCheckBox.Visible = true;
                TAToolSEIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSEIdleAnimPreview.sprite);
                TAToolSEIdleAnimPreview.display(TAToolSEIdleAnimPreview.originalAnim);
                TAToolSEIdleAnimPreview.mirrorState = "";
                TAToolSEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolSEIdleAnimPreview);
            }

         case "Horizontal": // Horizontal Mirror
            if (!%mirrorState && (TAToolNWIdleAnimPreview.mirrorState !$= "" || TAToolNWIdleMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolSWIdleAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolSWIdleMirrorCheckBox.Visible = true;
                TAToolSWIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSWIdleAnimPreview.sprite);
                TAToolSWIdleAnimPreview.display(TAToolSWIdleAnimPreview.originalAnim);
                TAToolSWIdleAnimPreview.mirrorState = "";
                TAToolSWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolSWIdleAnimPreview);
            }
            
            if (TAToolNWIdleMirrorCheckBox.getValue() == false && TAToolNWIdleAnimPreview.mirrorState $= "")
            {
                TAToolNWIdleMirrorCheckBox.Visible = false;
                TAToolNWIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolNWIdleMirrorCheckBox;
                %this.animPreview = TAToolNWIdleAnimPreview;
                TAToolSetFlip(TAToolNWIdleAnimPreview.sprite, %state);
                TAToolNWIdleAnimPreview.originalAnim = TAToolNWIdleAnimPreview.sprite.getAnimation();
                TAToolNWIdleAnimPreview.display(TAToolNEIdleAnimPreview.animation);
                TAToolNWIdleAnimPreview.mirrorState = %state;
                TAToolUpdateIdleEdit(TAToolNWIdleAnimPreview);
                TAToolNWIdleAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolNWIdleMirrorCheckBox.Visible = true;
                TAToolNWIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolNWIdleAnimPreview.sprite);
                TAToolNWIdleAnimPreview.display(TAToolNWIdleAnimPreview.originalAnim);
                TAToolNWIdleAnimPreview.mirrorState = "";
                TAToolUpdateIdleEdit(TAToolNWIdleAnimPreview);
                TAToolNWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }

            if (TAToolSEIdleAnimPreview.mirrorState $= "Vertical")
            {
                TAToolSEIdleMirrorCheckBox.Visible = true;
                TAToolSEIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSEIdleAnimPreview.sprite);
                TAToolSEIdleAnimPreview.display(TAToolSEIdleAnimPreview.originalAnim);
                TAToolSEIdleAnimPreview.mirrorState = "";
                TAToolSEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolSEIdleAnimPreview);
            }
            
         case "Vertical": // Vertical Mirror
            if (!%mirrorState && (TAToolSEIdleAnimPreview.mirrorState !$= "" || TAToolSEIdleMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolSWIdleAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolSWIdleMirrorCheckBox.Visible = true;
                TAToolSWIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSWIdleAnimPreview.sprite);
                TAToolSWIdleAnimPreview.display(TAToolSWIdleAnimPreview.originalAnim);
                TAToolSWIdleAnimPreview.mirrorState = "";
                TAToolSWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolSWIdleAnimPreview);
            }
            
            if (TAToolNWIdleAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolNWIdleMirrorCheckBox.Visible = true;
                TAToolNWIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNWIdleAnimPreview.sprite);
                TAToolNWIdleAnimPreview.display(TAToolNWIdleAnimPreview.originalAnim);
                TAToolNWIdleAnimPreview.mirrorState = "";
                TAToolNWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolNWIdleAnimPreview);
            }

            if (TAToolSEIdleMirrorCheckBox.getValue() == false && TAToolSEIdleAnimPreview.mirrorState $= "")
            {
                TAToolSEIdleMirrorCheckBox.Visible = false;
                TAToolSEIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolSEIdleMirrorCheckBox;
                %this.animPreview = TAToolSEIdleAnimPreview;
                TAToolSetFlip(TAToolSEIdleAnimPreview.sprite, %state);
                TAToolSEIdleAnimPreview.originalAnim = TAToolSEIdleAnimPreview.sprite.getAnimation();
                TAToolSEIdleAnimPreview.display(TAToolNEIdleAnimPreview.animation);
                TAToolSEIdleAnimPreview.mirrorState = %state;
                TAToolUpdateIdleEdit(TAToolSEIdleAnimPreview);
                TAToolSEIdleAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolSEIdleMirrorCheckBox.Visible = true;
                TAToolSEIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolSEIdleAnimPreview.sprite);
                TAToolSEIdleAnimPreview.display(TAToolSEIdleAnimPreview.originalAnim);
                TAToolSEIdleAnimPreview.mirrorState = "";
                TAToolUpdateIdleEdit(TAToolSEIdleAnimPreview);
                TAToolSEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }
      }
   }

    if (TAToolNWIdleMirrorCheckBox.getValue())
        TAToolNWIdleMirrorDropDown.onSelect();
    
    if (TAToolSWIdleMirrorCheckBox.getValue())
        TAToolSWIdleMirrorDropDown.onSelect();
    
    if (TAToolSEIdleMirrorCheckBox.getValue())
        TAToolSEIdleMirrorDropDown.onSelect();
        
   %this.previous = %this.getText();
   SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring based on the Southeast mirror dropdown selection.
/// </summary>
function TAToolSEIdleMirrorDropDown::onSelect(%this)
{
    if (%this.resetting)
    {
        %this.resetting = false;
        return;
    }
        
   if (%this.active)
   {
      %mirrorState = !TAToolSEIdleMirrorCheckBox.getValue();
      %this.Visible = TAToolSEIdleMirrorCheckBox.getValue();
      %state = %this.getText();
      switch$ (%state)
      {
         case "Diagonal": // Diagonal Mirror
            if (!%mirrorState && (TAToolNWIdleAnimPreview.mirrorState !$= "" || TAToolNWIdleMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolNWIdleMirrorCheckBox.getValue() == false && TAToolNWIdleAnimPreview.mirrorState $= "")
            {
                TAToolNWIdleMirrorCheckBox.Visible = false;
                TAToolNWIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolNWIdleMirrorCheckBox;
                %this.animPreview = TAToolNWIdleAnimPreview;
                TAToolSetFlip(TAToolNWIdleAnimPreview.sprite, %state);
                TAToolNWIdleAnimPreview.originalAnim = TAToolNWIdleAnimPreview.sprite.getAnimation();
                TAToolNWIdleAnimPreview.display(TAToolSEIdleAnimPreview.animation);
                TAToolNWIdleAnimPreview.mirrorState = %state;
                TAToolUpdateIdleEdit(TAToolNWIdleAnimPreview);
                TAToolNWIdleAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolNWIdleMirrorCheckBox.Visible = true;
                TAToolNWIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolNWIdleAnimPreview.sprite);
                TAToolNWIdleAnimPreview.display(TAToolNWIdleAnimPreview.originalAnim);
                TAToolNWIdleAnimPreview.mirrorState = "";
                TAToolUpdateIdleEdit(TAToolNWIdleAnimPreview);
                TAToolNWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }

            if (TAToolSWIdleAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolSWIdleMirrorCheckBox.Visible = true;
                TAToolSWIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSWIdleAnimPreview.sprite);
                TAToolSWIdleAnimPreview.display(TAToolSWIdleAnimPreview.originalAnim);
                TAToolSWIdleAnimPreview.mirrorState = "";
                TAToolSWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolSWIdleAnimPreview);
            }

            if (TAToolNEIdleAnimPreview.mirrorState $= "Vertical")
            {
                TAToolNEIdleMirrorCheckBox.Visible = true;
                TAToolNEIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNEIdleAnimPreview.sprite);
                TAToolNEIdleAnimPreview.display(TAToolNEIdleAnimPreview.originalAnim);
                TAToolNEIdleAnimPreview.mirrorState = "";
                TAToolNEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolNEIdleAnimPreview);
            }

         case "Horizontal": // Horizontal Mirror
            if (!%mirrorState && (TAToolSWIdleAnimPreview.mirrorState !$= "" || TAToolSWIdleMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolNWIdleAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolNWIdleMirrorCheckBox.Visible = true;
                TAToolNWIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNWIdleAnimPreview.sprite);
                TAToolNWIdleAnimPreview.display(TAToolNWIdleAnimPreview.originalAnim);
                TAToolNWIdleAnimPreview.mirrorState = "";
                TAToolNWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolNWIdleAnimPreview);
            }

            if (TAToolSWIdleMirrorCheckBox.getValue() == false && TAToolSWIdleAnimPreview.mirrorState $= "")
            {
                TAToolSWIdleMirrorCheckBox.Visible = false;
                TAToolSWIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolSWIdleMirrorCheckBox;
                %this.animPreview = TAToolSWIdleAnimPreview;
                TAToolSetFlip(TAToolSWIdleAnimPreview.sprite, %state);
                TAToolSWIdleAnimPreview.originalAnim = TAToolSWIdleAnimPreview.sprite.getAnimation();
                TAToolSWIdleAnimPreview.display(TAToolSEIdleAnimPreview.animation);
                TAToolSWIdleAnimPreview.mirrorState = %state;
                TAToolUpdateIdleEdit(TAToolSWIdleAnimPreview);
                TAToolSWIdleAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolSWIdleMirrorCheckBox.Visible = true;
                TAToolSWIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolSWIdleAnimPreview.sprite);
                TAToolSWIdleAnimPreview.display(TAToolSWIdleAnimPreview.originalAnim);
                TAToolSWIdleAnimPreview.mirrorState = "";
                TAToolUpdateIdleEdit(TAToolSWIdleAnimPreview);
                TAToolSWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }

            if (TAToolNEIdleAnimPreview.mirrorState $= "Vertical")
            {
                TAToolNEIdleMirrorCheckBox.Visible = true;
                TAToolNEIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNEIdleAnimPreview.sprite);
                TAToolNEIdleAnimPreview.display(TAToolNEIdleAnimPreview.originalAnim);
                TAToolNEIdleAnimPreview.mirrorState = "";
                TAToolNEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolNEIdleAnimPreview);
            }

         case "Vertical": // Vertical Mirror
            if (!%mirrorState && (TAToolNEIdleAnimPreview.mirrorState !$= "" || TAToolNEIdleMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolNWIdleAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolNWIdleMirrorCheckBox.Visible = true;
                TAToolNWIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNWIdleAnimPreview.sprite);
                TAToolNWIdleAnimPreview.display(TAToolNWIdleAnimPreview.originalAnim);
                TAToolNWIdleAnimPreview.mirrorState = "";
                TAToolNWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolNWIdleAnimPreview);
            }
            
            if (TAToolSWIdleAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolSWIdleMirrorCheckBox.Visible = true;
                TAToolSWIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSWIdleAnimPreview.sprite);
                TAToolSWIdleAnimPreview.display(TAToolSWIdleAnimPreview.originalAnim);
                TAToolSWIdleAnimPreview.mirrorState = "";
                TAToolSWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolSWIdleAnimPreview);
            }

            if (TAToolNEIdleMirrorCheckBox.getValue() == false && TAToolNEIdleAnimPreview.mirrorState $= "")
            {
                TAToolNEIdleMirrorCheckBox.Visible = false;
                TAToolNEIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolNEIdleMirrorCheckBox;
                %this.animPreview = TAToolNEIdleAnimPreview;
                TAToolSetFlip(TAToolNEIdleAnimPreview.sprite, %state);
                TAToolNEIdleAnimPreview.originalAnim = TAToolNEIdleAnimPreview.sprite.getAnimation();
                TAToolNEIdleAnimPreview.display(TAToolSEIdleAnimPreview.animation);
                TAToolNEIdleAnimPreview.mirrorState = %state;
                TAToolUpdateIdleEdit(TAToolNEIdleAnimPreview);
                TAToolNEIdleAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolNEIdleMirrorCheckBox.Visible = true;
                TAToolNEIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolNEIdleAnimPreview.sprite);
                TAToolNEIdleAnimPreview.display(TAToolNEIdleAnimPreview.originalAnim);
                TAToolNEIdleAnimPreview.mirrorState = "";
                TAToolUpdateIdleEdit(TAToolNEIdleAnimPreview);
                TAToolNEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }
      }
   }

    if (TAToolNEIdleMirrorCheckBox.getValue())
        TAToolNEIdleMirrorDropDown.onSelect();
    
    if (TAToolNWIdleMirrorCheckBox.getValue())
        TAToolNWIdleMirrorDropDown.onSelect();
    
    if (TAToolSWIdleMirrorCheckBox.getValue())
        TAToolSWIdleMirrorDropDown.onSelect();

   %this.previous = %this.getText();
   SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring based on the Southwest mirror dropdown selection.
/// </summary>
function TAToolSWIdleMirrorDropDown::onSelect(%this)
{
    if (%this.resetting)
    {
        %this.resetting = false;
        return;
    }
        
   if (%this.active)
   {
      %mirrorState = !TAToolSWIdleMirrorCheckBox.getValue();
      %this.Visible = TAToolSWIdleMirrorCheckBox.getValue();
      %state = %this.getText();
      switch$ (%state)
      {
         case "Diagonal": // Diagonal Mirror
            if (!%mirrorState && (TAToolNEIdleAnimPreview.mirrorState !$= "" || TAToolNEIdleMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolNEIdleMirrorCheckBox.getValue() == false && TAToolNEIdleAnimPreview.mirrorState $= "")
            {
                TAToolNEIdleMirrorCheckBox.Visible = false;
                TAToolNEIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolNEIdleMirrorCheckBox;
                %this.animPreview = TAToolNEIdleAnimPreview;
                TAToolSetFlip(TAToolNEIdleAnimPreview.sprite, %state);
                TAToolNEIdleAnimPreview.originalAnim = TAToolNEIdleAnimPreview.sprite.getAnimation();
                TAToolNEIdleAnimPreview.display(TAToolSWIdleAnimPreview.animation);
                TAToolNEIdleAnimPreview.mirrorState = %state;
                TAToolUpdateIdleEdit(TAToolNEIdleAnimPreview);
                TAToolNEIdleAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolNEIdleMirrorCheckBox.Visible = true;
                TAToolNEIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolNEIdleAnimPreview.sprite);
                TAToolNEIdleAnimPreview.display(TAToolNEIdleAnimPreview.originalAnim);
                TAToolNEIdleAnimPreview.mirrorState = "";
                TAToolUpdateIdleEdit(TAToolNEIdleAnimPreview);
                TAToolNEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }

            if (TAToolSEIdleAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolSEIdleMirrorCheckBox.Visible = true;
                TAToolSEIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSEIdleAnimPreview.sprite);
                TAToolSEIdleAnimPreview.display(TAToolSEIdleAnimPreview.originalAnim);
                TAToolSEIdleAnimPreview.mirrorState = "";
                TAToolSEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolSEIdleAnimPreview);
            }
            
            if (TAToolNWIdleAnimPreview.mirrorState $= "Vertical")
            {
                TAToolNWIdleMirrorCheckBox.Visible = true;
                TAToolNWIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNWIdleAnimPreview.sprite);
                TAToolNWIdleAnimPreview.display(TAToolNWIdleAnimPreview.originalAnim);
                TAToolNWIdleAnimPreview.mirrorState = "";
                TAToolNWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolNWIdleAnimPreview);
            }

         case "Horizontal": // Horizontal Mirror
            if (!%mirrorState && (TAToolSEIdleAnimPreview.mirrorState !$= "" || TAToolSEIdleMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolNEIdleAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolNEIdleMirrorCheckBox.Visible = true;
                TAToolNEIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNEIdleAnimPreview.sprite);
                TAToolNEIdleAnimPreview.display(TAToolNEIdleAnimPreview.originalAnim);
                TAToolNEIdleAnimPreview.mirrorState = "";
                TAToolNEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolNEIdleAnimPreview);
            }
            
            if (TAToolSEIdleMirrorCheckBox.getValue() == false && TAToolSEIdleAnimPreview.mirrorState $= "")
            {
                TAToolSEIdleMirrorCheckBox.Visible = false;
                TAToolSEIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolSEIdleMirrorCheckBox;
                %this.animPreview = TAToolSEIdleAnimPreview;
                TAToolSetFlip(TAToolSEIdleAnimPreview.sprite, %state);
                TAToolSEIdleAnimPreview.originalAnim = TAToolSEIdleAnimPreview.sprite.getAnimation();
                TAToolSEIdleAnimPreview.display(TAToolSWIdleAnimPreview.animation);
                TAToolSEIdleAnimPreview.mirrorState = %state;
                TAToolUpdateIdleEdit(TAToolSEIdleAnimPreview);
                TAToolSEIdleAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolSEIdleMirrorCheckBox.Visible = true;
                TAToolSEIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolSEIdleAnimPreview.sprite);
                TAToolSEIdleAnimPreview.display(TAToolSEIdleAnimPreview.originalAnim);
                TAToolSEIdleAnimPreview.mirrorState = "";
                TAToolUpdateIdleEdit(TAToolSEIdleAnimPreview);
                TAToolSEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }

            if (TAToolNWIdleAnimPreview.mirrorState $= "Vertical")
            {
                TAToolNWIdleMirrorCheckBox.Visible = true;
                TAToolNWIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNWIdleAnimPreview.sprite);
                TAToolNWIdleAnimPreview.display(TAToolNWIdleAnimPreview.originalAnim);
                TAToolNWIdleAnimPreview.mirrorState = "";
                TAToolNWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolNWIdleAnimPreview);
            }

         case "Vertical": // Vertical Mirror
            if (!%mirrorState && (TAToolNWIdleAnimPreview.mirrorState !$= "" || TAToolNWIdleMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolNEIdleAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolNEIdleMirrorCheckBox.Visible = true;
                TAToolNEIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNEIdleAnimPreview.sprite);
                TAToolNEIdleAnimPreview.display(TAToolNEIdleAnimPreview.originalAnim);
                TAToolNEIdleAnimPreview.mirrorState = "";
                TAToolNEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolNEIdleAnimPreview);
            }
            
            if (TAToolSEIdleAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolSEIdleMirrorCheckBox.Visible = true;
                TAToolSEIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSEIdleAnimPreview.sprite);
                TAToolSEIdleAnimPreview.display(TAToolSEIdleAnimPreview.originalAnim);
                TAToolSEIdleAnimPreview.mirrorState = "";
                TAToolSEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolSEIdleAnimPreview);
            }
            
            if (TAToolNWIdleMirrorCheckBox.getValue() == false && TAToolNWIdleAnimPreview.mirrorState $= "")
            {
                TAToolNWIdleMirrorCheckBox.Visible = false;
                TAToolNWIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolNWIdleMirrorCheckBox;
                %this.animPreview = TAToolNWIdleAnimPreview;
                TAToolSetFlip(TAToolNWIdleAnimPreview.sprite, %state);
                TAToolNWIdleAnimPreview.originalAnim = TAToolNWIdleAnimPreview.sprite.getAnimation();
                TAToolNWIdleAnimPreview.display(TAToolSWIdleAnimPreview.animation);
                TAToolNWIdleAnimPreview.mirrorState = %state;
                TAToolUpdateIdleEdit(TAToolNWIdleAnimPreview);
                TAToolNWIdleAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolNWIdleMirrorCheckBox.Visible = true;
                TAToolNWIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolNWIdleAnimPreview.sprite);
                TAToolNWIdleAnimPreview.display(TAToolNWIdleAnimPreview.originalAnim);
                TAToolNWIdleAnimPreview.mirrorState = "";
                TAToolUpdateIdleEdit(TAToolNWIdleAnimPreview);
                TAToolNWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }
      }
   }

    if (TAToolNWIdleMirrorCheckBox.getValue())
        TAToolNWIdleMirrorDropDown.onSelect();
    
    if (TAToolNEIdleMirrorCheckBox.getValue())
        TAToolNEIdleMirrorDropDown.onSelect();
        
    if (TAToolSEIdleMirrorCheckBox.getValue())
        TAToolSEIdleMirrorDropDown.onSelect();

   %this.previous = %this.getText();
   SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles mirroring based on the Northwest mirror dropdown selection.
/// </summary>
function TAToolNWIdleMirrorDropDown::onSelect(%this)
{
    if (%this.resetting)
    {
        %this.resetting = false;
        return;
    }
        
   if (%this.active)
   {
      %mirrorState = !TAToolNWIdleMirrorCheckBox.getValue();
      %this.Visible = TAToolNWIdleMirrorCheckBox.getValue();
      %state = %this.getText();
      switch$ (%state)
      {
         case "Diagonal": // Diagonal Mirror
            if (!%mirrorState && (TAToolSEIdleAnimPreview.mirrorState !$= "" || TAToolSEIdleMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolSEIdleMirrorCheckBox.getValue() == false && TAToolSEIdleAnimPreview.mirrorState $= "")
            {
                TAToolSEIdleMirrorCheckBox.Visible = false;
                TAToolSEIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolSEIdleMirrorCheckBox;
                %this.animPreview = TAToolSEIdleAnimPreview;
                TAToolSetFlip(TAToolSEIdleAnimPreview.sprite, %state);
                TAToolSEIdleAnimPreview.originalAnim = TAToolSEIdleAnimPreview.sprite.getAnimation();
                TAToolSEIdleAnimPreview.display(TAToolNWIdleAnimPreview.animation);
                TAToolSEIdleAnimPreview.mirrorState = %state;
                TAToolUpdateIdleEdit(TAToolSEIdleAnimPreview);
                TAToolSEIdleAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolSEIdleMirrorCheckBox.Visible = true;
                TAToolSEIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolSEIdleAnimPreview.sprite);
                TAToolSEIdleAnimPreview.display(TAToolSEIdleAnimPreview.originalAnim);
                TAToolSEIdleAnimPreview.mirrorState = "";
                TAToolUpdateIdleEdit(TAToolSEIdleAnimPreview);
                TAToolSEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }
            
            if (TAToolNEIdleAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolNEIdleMirrorCheckBox.Visible = true;
                TAToolNEIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNEIdleAnimPreview.sprite);
                TAToolNEIdleAnimPreview.display(TAToolNEIdleAnimPreview.originalAnim);
                TAToolNEIdleAnimPreview.mirrorState = "";
                TAToolNEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolNEIdleAnimPreview);
            }

            if (TAToolSWIdleAnimPreview.mirrorState $= "Vertical")
            {
                TAToolSWIdleMirrorCheckBox.Visible = true;
                TAToolSWIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSWIdleAnimPreview.sprite);
                TAToolSWIdleAnimPreview.display(TAToolSWIdleAnimPreview.originalAnim);
                TAToolSWIdleAnimPreview.mirrorState = "";
                TAToolSWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolSWIdleAnimPreview);
            }

         case "Horizontal": // Horizontal Mirror
            if (!%mirrorState && (TAToolNEIdleAnimPreview.mirrorState !$= "" || TAToolNEIdleMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolSEIdleAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolSEIdleMirrorCheckBox.Visible = true;
                TAToolSEIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSEIdleAnimPreview.sprite);
                TAToolSEIdleAnimPreview.display(TAToolSEIdleAnimPreview.originalAnim);
                TAToolSEIdleAnimPreview.mirrorState = "";
                TAToolSEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolSEIdleAnimPreview);
            }
            
            if (TAToolNEIdleMirrorCheckBox.getValue() == false && TAToolNEIdleAnimPreview.mirrorState $= "")
            {
                TAToolNEIdleMirrorCheckBox.Visible = false;
                TAToolNEIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolNEIdleMirrorCheckBox;
                %this.animPreview = TAToolNEIdleAnimPreview;
                TAToolSetFlip(TAToolNEIdleAnimPreview.sprite, %state);
                TAToolNEIdleAnimPreview.originalAnim = TAToolNEIdleAnimPreview.sprite.getAnimation();
                TAToolNEIdleAnimPreview.display(TAToolNWIdleAnimPreview.animation);
                TAToolNEIdleAnimPreview.mirrorState = %state;
                TAToolUpdateIdleEdit(TAToolNEIdleAnimPreview);
                TAToolNEIdleAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolNEIdleMirrorCheckBox.Visible = true;
                TAToolNEIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolNEIdleAnimPreview.sprite);
                TAToolNEIdleAnimPreview.display(TAToolNEIdleAnimPreview.originalAnim);
                TAToolNEIdleAnimPreview.mirrorState = "";
                TAToolUpdateIdleEdit(TAToolNEIdleAnimPreview);
                TAToolNEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }
            
            if (TAToolSWIdleAnimPreview.mirrorState $= "Vertical")
            {
                TAToolSWIdleMirrorCheckBox.Visible = true;
                TAToolSWIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSWIdleAnimPreview.sprite);
                TAToolSWIdleAnimPreview.display(TAToolSWIdleAnimPreview.originalAnim);
                TAToolSWIdleAnimPreview.mirrorState = "";
                TAToolSWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolSWIdleAnimPreview);
            }

         case "Vertical": // Vertical Mirror
            if (!%mirrorState && (TAToolSWIdleAnimPreview.mirrorState !$= "" || TAToolSWIdleMirrorCheckBox.getValue() == true))
            {
                %prevSelected = %this.findText(%this.previous);
                %this.resetting = true;
                %this.setSelected(%prevSelected);
                return;
            }

            if (TAToolSEIdleAnimPreview.mirrorState $= "Diagonal")
            {
                TAToolSEIdleMirrorCheckBox.Visible = true;
                TAToolSEIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolSEIdleAnimPreview.sprite);
                TAToolSEIdleAnimPreview.display(TAToolSEIdleAnimPreview.originalAnim);
                TAToolSEIdleAnimPreview.mirrorState = "";
                TAToolSEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolSEIdleAnimPreview);
            }
            
            if (TAToolNEIdleAnimPreview.mirrorState $= "Horizontal")
            {
                TAToolNEIdleMirrorCheckBox.Visible = true;
                TAToolNEIdleMirrorDropDown.Visible = false;
                TAToolResetFlip(TAToolNEIdleAnimPreview.sprite);
                TAToolNEIdleAnimPreview.display(TAToolNEIdleAnimPreview.originalAnim);
                TAToolNEIdleAnimPreview.mirrorState = "";
                TAToolNEIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
                TAToolUpdateIdleEdit(TAToolNEIdleAnimPreview);
            }

            if (TAToolSWIdleMirrorCheckBox.getValue == false && TAToolSWIdleAnimPreview.mirrorState $= "")
            {
                TAToolSWIdleMirrorCheckBox.Visible = false;
                TAToolSWIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = TAToolSWIdleMirrorCheckBox;
                %this.animPreview = TAToolSWIdleAnimPreview;
                TAToolSetFlip(TAToolSWIdleAnimPreview.sprite, %state);
                TAToolSWIdleAnimPreview.originalAnim = TAToolSWIdleAnimPreview.sprite.getAnimation();
                TAToolSWIdleAnimPreview.display(TAToolNWIdleAnimPreview.animation);
                TAToolSWIdleAnimPreview.mirrorState = %state;
                TAToolUpdateIdleEdit(TAToolSWIdleAnimPreview);
                TAToolSWIdleAnimPreview.sprite.BlendColor = $TAToolMirroredColor;
            }

            else if (%mirrorState)
            {
                TAToolSWIdleMirrorCheckBox.Visible = true;
                TAToolSWIdleMirrorDropDown.Visible = false;
                %this.hiddenControl = "";
                %this.animPreview = "";
                TAToolResetFlip(TAToolSWIdleAnimPreview.sprite);
                TAToolSWIdleAnimPreview.display(TAToolSWIdleAnimPreview.originalAnim);
                TAToolSWIdleAnimPreview.mirrorState = "";
                TAToolUpdateIdleEdit(TAToolSWIdleAnimPreview);
                TAToolSWIdleAnimPreview.sprite.BlendColor = $TAToolOriginalColor;
            }
      }
   }

    if (TAToolNEIdleMirrorCheckBox.getValue())
        TAToolNEIdleMirrorDropDown.onSelect();
    
    if (TAToolSEIdleMirrorCheckBox.getValue())
        TAToolSEIdleMirrorDropDown.onSelect();
    
    if (TAToolSWIdleMirrorCheckBox.getValue())
        TAToolSWIdleMirrorDropDown.onSelect();

   %this.previous = %this.getText();
   SetTowerAnimToolDirtyState(true);
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolNorthIdleClick::onMouseUp(%this)
{
   TAToolNIdleAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolNorthEastIdleClick::onMouseUp(%this)
{
   TAToolNEIdleAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolEastIdleClick::onMouseUp(%this)
{
   TAToolEIdleAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolSouthEastIdleClick::onMouseUp(%this)
{
   TAToolSEIdleAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolSouthIdleClick::onMouseUp(%this)
{
   TAToolSIdleAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolSouthWestIdleClick::onMouseUp(%this)
{
   TAToolSWIdleAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolWestIdleClick::onMouseUp(%this)
{
   TAToolWIdleAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolNorthWestIdleClick::onMouseUp(%this)
{
   TAToolNWIdleAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolNorthFireClick::onMouseUp(%this)
{
   TAToolNFireAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolNorthEastFireClick::onMouseUp(%this)
{
   TAToolNEFireAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolEastFireClick::onMouseUp(%this)
{
   TAToolEFireAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolSouthEastFireClick::onMouseUp(%this)
{
   TAToolSEFireAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolSouthFireClick::onMouseUp(%this)
{
   TAToolSFireAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolSouthWestFireClick::onMouseUp(%this)
{
   TAToolSWFireAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolWestFireClick::onMouseUp(%this)
{
   TAToolWFireAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the clicked animation preview.
/// </summary>
function TAToolNorthWestFireClick::onMouseUp(%this)
{
   TAToolNWFireAnimPlayBtn.onClick();
}

/// <summary>
/// This function resets the fire tab animations on tab change.  It also assigns
/// the play/pause buttons to their respective previews.
/// </summary>
function TAToolTabBook::onTabSelected(%this)
{
    // Return is the tool is not active
    if (!$IsTowerAnimToolAwake)   
        return;   

    TAToolNIdleAnimPreview.playBtn = TAToolNIdleAnimPlayBtn;
    TAToolNEIdleAnimPreview.playBtn = TAToolNEIdleAnimPlayBtn;
    TAToolEIdleAnimPreview.playBtn = TAToolEIdleAnimPlayBtn;
    TAToolSEIdleAnimPreview.playBtn = TAToolSEIdleAnimPlayBtn;
    TAToolSIdleAnimPreview.playBtn = TAToolSIdleAnimPlayBtn;
    TAToolSWIdleAnimPreview.playBtn = TAToolSWIdleAnimPlayBtn;
    TAToolWIdleAnimPreview.playBtn = TAToolWIdleAnimPlayBtn;
    TAToolNWIdleAnimPreview.playBtn = TAToolNWIdleAnimPlayBtn;

	if (isObject(TAToolNAnimPreview))
	{
    	TAToolNAnimPreview.playBtn = TAToolNFireAnimPlayBtn;
      	TAToolNAnimPreview.play();
    	TAToolNAnimPreview.pause();
 		TAToolNAnimPreview.sprite.setAnimationFrame(0);
	}

    if (isObject(TAToolNEAnimPreview))   
	{
		TAToolNEAnimPreview.playBtn = TAToolNEFireAnimPlayBtn;
		TAToolNEAnimPreview.play();
		TAToolNEAnimPreview.pause();
		TAToolNEAnimPreview.sprite.setAnimationFrame(0);
	}
   if (isObject(TAToolEAnimPreview))
   {
		TAToolEAnimPreview.playBtn = TAToolEFireAnimPlayBtn;
		TAToolEAnimPreview.play();
    	TAToolEAnimPreview.pause();
   		TAToolEAnimPreview.sprite.setAnimationFrame(0);
   }
   if (isObject(TAToolSEAnimPreview))
   {
    	TAToolSEAnimPreview.playBtn = TAToolSEFireAnimPlayBtn;
      	TAToolSEAnimPreview.play();
    	TAToolSEAnimPreview.pause();
    	TAToolSEAnimPreview.sprite.setAnimationFrame(0);
   }
   if (isObject(TAToolSAnimPreview))
   {
    	TAToolSAnimPreview.playBtn = TAToolSFireAnimPlayBtn;
      	TAToolSAnimPreview.play();
    	TAToolSAnimPreview.pause();
    	TAToolSAnimPreview.sprite.setAnimationFrame(0);
   }
   if (isObject(TAToolSWAnimPreview))
   {
    	TAToolSWAnimPreview.playBtn = TAToolSWFireAnimPlayBtn;
      	TAToolSWAnimPreview.play();
    	TAToolSWAnimPreview.pause();
    	TAToolSWAnimPreview.sprite.setAnimationFrame(0);
   }
   if (isObject(TAToolWAnimPreview))
   {
    	TAToolWAnimPreview.playBtn = TAToolWFireAnimPlayBtn;
      	TAToolWAnimPreview.play();
    	TAToolWAnimPreview.pause();
    	TAToolWAnimPreview.sprite.setAnimationFrame(0);
   }
   if (isObject(TAToolNWAnimPreview))
   {
    	TAToolNWAnimPreview.playBtn = TAToolNWFireAnimPlayBtn;
        TAToolNWAnimPreview.play();
        TAToolNWAnimPreview.pause();
        TAToolNWAnimPreview.sprite.setAnimationFrame(0);
   }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolNIdleAnimPlayBtn::onClick(%this)
{
    if (TAToolNIdleAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolNIdleAnimPreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        TAToolNIdleAnimPreview.pause();
    }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolNEIdleAnimPlayBtn::onClick(%this)
{
    if (TAToolNEIdleAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolNEIdleAnimPreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        TAToolNEIdleAnimPreview.pause();
    }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolEIdleAnimPlayBtn::onClick(%this)
{
    if (TAToolEIdleAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolEIdleAnimPreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        TAToolEIdleAnimPreview.pause();
    }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolSEIdleAnimPlayBtn::onClick(%this)
{
    if (TAToolSEIdleAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolSEIdleAnimPreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        TAToolSEIdleAnimPreview.pause();
    }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolSIdleAnimPlayBtn::onClick(%this)
{
    if (TAToolSIdleAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolSIdleAnimPreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        TAToolSIdleAnimPreview.pause();
    }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolSWIdleAnimPlayBtn::onClick(%this)
{
    if (TAToolSWIdleAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolSWIdleAnimPreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        TAToolSWIdleAnimPreview.pause();
    }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolWIdleAnimPlayBtn::onClick(%this)
{
    if (TAToolWIdleAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolWIdleAnimPreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        TAToolWIdleAnimPreview.pause();
    }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolNWIdleAnimPlayBtn::onClick(%this)
{
    if (TAToolNWIdleAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolNWIdleAnimPreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        TAToolNWIdleAnimPreview.pause();
    }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolNFireAnimPlayBtn::onClick(%this)
{
    if (TAToolNAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolNAnimPreview.play();
        %anim = TAToolNAnimPreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = TAToolNAnimPreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        TAToolNAnimPreview.pause();
    }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolNEFireAnimPlayBtn::onClick(%this)
{
    if (TAToolNEAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolNEAnimPreview.play();
        %anim = TAToolNEAnimPreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = TAToolNEAnimPreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        TAToolNEAnimPreview.pause();
    }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolEFireAnimPlayBtn::onClick(%this)
{
    if (TAToolEAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolEAnimPreview.play();
        %anim = TAToolEAnimPreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = TAToolEAnimPreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        TAToolEAnimPreview.pause();
    }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolSEFireAnimPlayBtn::onClick(%this)
{
    if (TAToolSEAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolSEAnimPreview.play();
        %anim = TAToolSEAnimPreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = TAToolSEAnimPreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        TAToolSEAnimPreview.pause();
    }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolSFireAnimPlayBtn::onClick(%this)
{
    if (TAToolSAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolSAnimPreview.play();
        %anim = TAToolSAnimPreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = TAToolSAnimPreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        TAToolSAnimPreview.pause();
    }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolSWFireAnimPlayBtn::onClick(%this)
{
    if (TAToolSWAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolSWAnimPreview.play();
        %anim = TAToolSWAnimPreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = TAToolSWAnimPreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        TAToolSWAnimPreview.pause();
    }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolWFireAnimPlayBtn::onClick(%this)
{
    if (TAToolWAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolWAnimPreview.play();
        %anim = TAToolWAnimPreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = TAToolWAnimPreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        TAToolWAnimPreview.pause();
    }
}

/// <summary>
/// This function toggles playing/pausing the assigned animation preview.
/// </summary>
function TAToolNWFireAnimPlayBtn::onClick(%this)
{
    if (TAToolNWAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        TAToolNWAnimPreview.play();
        %anim = TAToolNWAnimPreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = TAToolNWAnimPreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        TAToolNWAnimPreview.pause();
    }
}

/// <summary>
/// This function sets the button state image of the assigned button to the passed image.
/// </summary>
/// <param name="imageFile">The desired button state image.</param>
function TAToolNFireAnimPlayBtn::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    TAToolNAnimPreview.sprite.setAnimationFrame(0);
    TAToolNAnimPreview.pause();
}

/// <summary>
/// This function sets the button state image of the assigned button to the passed image.
/// </summary>
/// <param name="imageFile">The desired button state image.</param>
function TAToolNEFireAnimPlayBtn::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    TAToolNEAnimPreview.sprite.setAnimationFrame(0);
    TAToolNEAnimPreview.pause();
}

/// <summary>
/// This function sets the button state image of the assigned button to the passed image.
/// </summary>
/// <param name="imageFile">The desired button state image.</param>
function TAToolEFireAnimPlayBtn::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    TAToolEAnimPreview.sprite.setAnimationFrame(0);
    TAToolEAnimPreview.pause();
}

/// <summary>
/// This function sets the button state image of the assigned button to the passed image.
/// </summary>
/// <param name="imageFile">The desired button state image.</param>
function TAToolSEFireAnimPlayBtn::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    TAToolSEAnimPreview.sprite.setAnimationFrame(0);
    TAToolSEAnimPreview.pause();
}

/// <summary>
/// This function sets the button state image of the assigned button to the passed image.
/// </summary>
/// <param name="imageFile">The desired button state image.</param>
function TAToolSFireAnimPlayBtn::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    TAToolSAnimPreview.sprite.setAnimationFrame(0);
    TAToolSAnimPreview.pause();
}

/// <summary>
/// This function sets the button state image of the assigned button to the passed image.
/// </summary>
/// <param name="imageFile">The desired button state image.</param>
function TAToolSWFireAnimPlayBtn::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    TAToolSWAnimPreview.sprite.setAnimationFrame(0);
    TAToolSWAnimPreview.pause();
}

/// <summary>
/// This function sets the button state image of the assigned button to the passed image.
/// </summary>
/// <param name="imageFile">The desired button state image.</param>
function TAToolWFireAnimPlayBtn::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    TAToolWAnimPreview.sprite.setAnimationFrame(0);
    TAToolWAnimPreview.pause();
}

/// <summary>
/// This function sets the button state image of the assigned button to the passed image.
/// </summary>
/// <param name="imageFile">The desired button state image.</param>
function TAToolNWFireAnimPlayBtn::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    TAToolNWAnimPreview.sprite.setAnimationFrame(0);
    TAToolNWAnimPreview.pause();
}

/// <summary>
/// This function sets default animation selection and sets the tool's dirty state.
/// </summary>
function TAToolAnimDefaultAnimDropdown::onSelect(%this)
{
    SetTowerAnimToolDirtyState(true);
}