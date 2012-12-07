//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------


//--------------------------------
// Enemy Animation Set Tool Help
//--------------------------------
/// <summary>
/// This function handles the help button in the Enemy Animation Set Tool.  It 
/// opens a browser and links to the correct online help section.
/// </summary>
function EAToolHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/towerdefense/enemyanimationset/");
}

//-----------------------------------------------------------------------------
// Enemy Animation Tool Globals
//-----------------------------------------------------------------------------
/// <summary>
/// This function keeps track of whether the tool has changed something that needs to 
/// be saved.
/// </summary>
/// <param name="dirty">Set true if the tool needs to save data, false if not.</param>
function SetEnemyAnimToolDirtyState(%dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      EnemyAnimToolWindow.setText("Enemy Animation Set Tool *");
   else
      EnemyAnimToolWindow.setText("Enemy Animation Set Tool");
}

$EAToolInitialized = false;

$EAToolMirroredColor = "1.0 1.0 1.0 0.4";
$EAToolOriginalColor = "1.0 1.0 1.0 1.0";

/// <summary>
/// This function is used to set the flip state of the specified sprite object.
/// </summary>
/// <param name="sprite">The sprite to flip.</param>
/// <param name="direction">The desired flip direction, Horizontal, Vertical or Diagonal.</param>
function EAToolSetFlip(%sprite, %direction)
{
   switch$(%direction)
   {
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
/// <param name="state">If this is non-null, this actually sets the flip state to the state specified, else it resets the sprite's flip state.</param>
function EAToolResetFlip(%sprite, %state)
{
   if (%state !$= "")
   {
      EAToolSetFlip(%sprite, %state);
   }
   else
   {
      %sprite.setFlipX(false);
      %sprite.setFlipY(false);
   }
   //echo(" -- Sprite " @ %sprite @ " reset flip");
}

/// <summary>
/// This function is used to set the text of the edit box associated with the passed 
/// animation preview window to the name of the animation currently displayed by the
/// preview passed for the walk animation tab.
/// </summary>
/// <param name="animPreview">The name of the animation preview window that needs its label updated.</param>
function EAToolUpdateEdit(%animPreview)
{
   if (%animPreview $= EAToolWalkNorthPreview)
      EAToolWalkNorthEdit.text = %animPreview.sprite.getAnimation();

   if (%animPreview $= EAToolWalkEastPreview)
      EAToolWalkEastEdit.text = %animPreview.sprite.getAnimation();

   if (%animPreview $= EAToolWalkSouthPreview)
      EAToolWalkSouthEdit.text = %animPreview.sprite.getAnimation();

   if (%animPreview $= EAToolWalkWestPreview)
      EAToolWalkWestEdit.text = %animPreview.sprite.getAnimation();

   if (%animPreview $= EAToolDieNorthPreview)
      EAToolDieNorthEdit.text = %animPreview.sprite.getAnimation();

   if (%animPreview $= EAToolDieEastPreview)
      EAToolDieEastEdit.text = %animPreview.sprite.getAnimation();

   if (%animPreview $= EAToolDieSouthPreview)
      EAToolDieSouthEdit.text = %animPreview.sprite.getAnimation();

   if (%animPreview $= EAToolDieWestPreview)
      EAToolDieWestEdit.text = %animPreview.sprite.getAnimation();
}

/// <summary>
/// This function is used to set the visibility of the mirror checkbox in a 
/// mirrored animation preview display and to set the correct animation in the
/// destination preview for the walk animation tab.
/// </summary>
/// <param name="checkbox">The checkbox that is being checked/unchecked.</param>
function EAToolMirrorHide(%checkbox)
{
   %state = !%checkbox.getValue();
   
   // -------------------------------------------------------------------------
   // Flip North to South
   // -------------------------------------------------------------------------
   if (%checkbox == EAToolWalkNorthMirror.getId())
   {
      EAToolWalkSouthMirror.setValue(false);
      EAToolWalkSouthMirror.Visible = %state;
      if (%state == false)
      {
         EAToolWalkSouthPreview.originalAnim = EAToolWalkSouthPreview.sprite.getAnimation();
         EAToolWalkSouthPreview.display(EAToolWalkNorthPreview.sprite.getAnimation(), t2dAnimatedSprite);
         EAToolSetFlip(EAToolWalkSouthPreview.sprite, "Vertical");
         EAToolWalkSouthPreview.mirrorState = "Vertical";
         EAToolUpdateEdit(EAToolWalkSouthPreview);
      }
      else
      {
         // temporary code - restores original test animation to the preview window
         EAToolWalkSouthPreview.display(EAToolWalkSouthPreview.originalAnim, t2dAnimatedSprite);
         EAToolResetFlip(EAToolWalkSouthPreview.sprite, "");
         EAToolWalkSouthPreview.mirrorState = "";
         EAToolUpdateEdit(EAToolWalkSouthPreview);
      }
      EAToolWalkSouthPreview.sprite.BlendColor = (%state == 0 ? $EAToolMirroredColor : $EAToolOriginalColor);
   }

   // -------------------------------------------------------------------------
   // Flip East to West
   // -------------------------------------------------------------------------
   if (%checkbox == EAToolWalkEastMirror.getId())
   {
      EAToolWalkWestMirror.setValue(false);
      EAToolWalkWestMirror.Visible = %state;
      if (%state == false)
      {
         EAToolWalkWestPreview.originalAnim = EAToolWalkWestPreview.sprite.getAnimation();
         EAToolWalkWestPreview.display(EAToolWalkEastPreview.sprite.getAnimation(), t2dAnimatedSprite);
         EAToolSetFlip(EAToolWalkWestPreview.sprite, "Horizontal");
         EAToolWalkWestPreview.mirrorState = "Horizontal";
         EAToolUpdateEdit(EAToolWalkWestPreview);
      }
      else
      {
         // temporary code - restores original test animation to the preview window
         EAToolWalkWestPreview.display(EAToolWalkWestPreview.originalAnim, t2dAnimatedSprite);
         EAToolResetFlip(EAToolWalkWestPreview.sprite, "");
         EAToolWalkWestPreview.mirrorState = "";
         EAToolUpdateEdit(EAToolWalkWestPreview);
      }
      EAToolWalkWestPreview.sprite.BlendColor = (%state == 0 ? $EAToolMirroredColor : $EAToolOriginalColor);
   }

   // -------------------------------------------------------------------------
   // Flip South to North
   // -------------------------------------------------------------------------
   if (%checkbox == EAToolWalkSouthMirror.getId())
   {
      EAToolWalkNorthMirror.setValue(false);
      EAToolWalkNorthMirror.Visible = %state;
      if (%state == false)
      {
         EAToolWalkNorthPreview.originalAnim = EAToolWalkNorthPreview.sprite.getAnimation();
         EAToolWalkNorthPreview.display(EAToolWalkSouthPreview.sprite.getAnimation(), t2dAnimatedSprite);
         EAToolSetFlip(EAToolWalkNorthPreview.sprite, "Vertical");
         EAToolWalkNorthPreview.mirrorState = "Vertical";
         EAToolUpdateEdit(EAToolWalkNorthPreview);
      }
      else
      {
         // temporary code - restores original test animation to the preview window
         EAToolWalkNorthPreview.display(EAToolWalkNorthPreview.originalAnim, t2dAnimatedSprite);
         EAToolResetFlip(EAToolWalkNorthPreview.sprite, "");
         EAToolWalkNorthPreview.mirrorState = "";
         EAToolUpdateEdit(EAToolWalkNorthPreview);
      }
      EAToolWalkNorthPreview.sprite.BlendColor = (%state == 0 ? $EAToolMirroredColor : $EAToolOriginalColor);
   }

   // -------------------------------------------------------------------------
   // Flip West to East
   // -------------------------------------------------------------------------
   if (%checkbox == EAToolWalkWestMirror.getId())
   {
      EAToolWalkEastMirror.setValue(false);
      EAToolWalkEastMirror.Visible = %state;
      if (%state == false)
      {
         EAToolWalkEastPreview.originalAnim = EAToolWalkEastPreview.sprite.getAnimation();
         EAToolWalkEastPreview.display(EAToolWalkWestPreview.sprite.getAnimation(), t2dAnimatedSprite);
         EAToolSetFlip(EAToolWalkEastPreview.sprite, "Horizontal");
         EAToolWalkEastPreview.mirrorState = "Horizontal";
         EAToolUpdateEdit(EAToolWalkEastPreview);
      }
      else
      {
         // temporary code - restores original test animation to the preview window
         EAToolWalkEastPreview.display(EAToolWalkEastPreview.originalAnim, t2dAnimatedSprite);
         EAToolResetFlip(EAToolWalkEastPreview.sprite, "");
         EAToolWalkEastPreview.mirrorState = "";
         EAToolUpdateEdit(EAToolWalkEastPreview);
      }
      EAToolWalkEastPreview.sprite.BlendColor = (%state == 0 ? $EAToolMirroredColor : $EAToolOriginalColor);
   }
}

/// <summary>
/// This function is used to set the text of the edit box associated with the passed 
/// animation preview window to the name of the animation currently displayed by the
/// preview passed for the death animation tab.
/// </summary>
/// <param name="animPreview">The animation preview that needs its label updated.</param>
function EAToolUpdateDeathEdit(%animPreview)
{
   if (%animPreview $= EAToolDieNorthPreview)
      EAToolDieNorthEdit.text = %animPreview.sprite.getAnimation();

   if (%animPreview $= EAToolDieEastPreview)
      EAToolDieEastEdit.text = %animPreview.sprite.getAnimation();

   if (%animPreview $= EAToolDieSouthPreview)
      EAToolDieSouthEdit.text = %animPreview.sprite.getAnimation();

   if (%animPreview $= EAToolDieWestPreview)
      EAToolDieWestEdit.text = %animPreview.sprite.getAnimation();
}

/// <summary>
/// This function is used to set the visibility of the mirror checkbox in a 
/// mirrored animation preview display and to set the correct animation in the
/// destination preview for the death animation tab.
/// </summary>
/// <param name="checkbox">The checkbox that is being checked/unchecked.</param>
function EAToolDieMirrorHide(%checkbox)
{
   %state = !%checkbox.getValue();
   
   // -------------------------------------------------------------------------
   // Flip North to South
   // -------------------------------------------------------------------------
   if (%checkbox == EAToolDieNorthMirror.getId())
   {
      EAToolDieSouthMirror.setValue(false);
      EAToolDieSouthMirror.Visible = %state;
      if (%state == false)
      {
         EAToolDieSouthPreview.originalAnim = EAToolDieSouthPreview.sprite.getAnimation();
         EAToolDieSouthPreview.display(EAToolDieNorthPreview.sprite.getAnimation(), t2dAnimatedSprite);
         EAToolSetFlip(EAToolDieSouthPreview.sprite, "Vertical");
         EAToolDieSouthPreview.mirrorState = "Vertical";
         EAToolUpdateEdit(EAToolDieSouthPreview);
      }
      else
      {
         // temporary code - restores original test animation to the preview window
         EAToolDieSouthPreview.display(EAToolDieSouthPreview.originalAnim, t2dAnimatedSprite);
         EAToolResetFlip(EAToolDieSouthPreview.sprite, "");
         EAToolDieSouthPreview.mirrorState = "";
         EAToolUpdateEdit(EAToolDieSouthPreview);
      }
      EAToolDieSouthPreview.sprite.BlendColor = (%state == 0 ? $EAToolMirroredColor : $EAToolOriginalColor);
   }

   // -------------------------------------------------------------------------
   // Flip East to West
   // -------------------------------------------------------------------------
   if (%checkbox == EAToolDieEastMirror.getId())
   {
      EAToolDieWestMirror.setValue(false);
      EAToolDieWestMirror.Visible = %state;
      if (%state == false)
      {
         EAToolDieWestPreview.originalAnim = EAToolDieWestPreview.sprite.getAnimation();
         EAToolDieWestPreview.display(EAToolDieEastPreview.sprite.getAnimation(), t2dAnimatedSprite);
         EAToolSetFlip(EAToolDieWestPreview.sprite, "Horizontal");
         EAToolDieWestPreview.mirrorState = "Horizontal";
         EAToolUpdateEdit(EAToolDieWestPreview);
      }
      else
      {
         // temporary code - restores original test animation to the preview window
         EAToolDieWestPreview.display(EAToolDieWestPreview.originalAnim, t2dAnimatedSprite);
         EAToolResetFlip(EAToolDieWestPreview.sprite, "");
         EAToolDieWestPreview.mirrorState = "";
         EAToolUpdateEdit(EAToolDieWestPreview);
      }
      EAToolDieWestPreview.sprite.BlendColor = (%state == 0 ? $EAToolMirroredColor : $EAToolOriginalColor);
   }

   // -------------------------------------------------------------------------
   // Flip South to North
   // -------------------------------------------------------------------------
   if (%checkbox == EAToolDieSouthMirror.getId())
   {
      EAToolDieNorthMirror.setValue(false);
      EAToolDieNorthMirror.Visible = %state;
      if (%state == false)
      {
         EAToolDieNorthPreview.originalAnim = EAToolDieNorthPreview.sprite.getAnimation();
         EAToolDieNorthPreview.display(EAToolDieSouthPreview.sprite.getAnimation(), t2dAnimatedSprite);
         EAToolSetFlip(EAToolDieNorthPreview.sprite, "Vertical");
         EAToolDieNorthPreview.mirrorState = "Vertical";
         EAToolUpdateEdit(EAToolDieNorthPreview);
      }
      else
      {
         // temporary code - restores original test animation to the preview window
         EAToolDieNorthPreview.display(EAToolDieNorthPreview.originalAnim, t2dAnimatedSprite);
         EAToolResetFlip(EAToolDieNorthPreview.sprite, "");
         EAToolDieNorthPreview.mirrorState = "";
         EAToolUpdateEdit(EAToolDieNorthPreview);
      }
      EAToolDieNorthPreview.sprite.BlendColor = (%state == 0 ? $EAToolMirroredColor : $EAToolOriginalColor);
   }

   // -------------------------------------------------------------------------
   // Flip West to East
   // -------------------------------------------------------------------------
   if (%checkbox == EAToolDieWestMirror.getId())
   {
      EAToolDieEastMirror.setValue(false);
      EAToolDieEastMirror.Visible = %state;
      if (%state == false)
      {
         EAToolDieEastPreview.originalAnim = EAToolDieEastPreview.sprite.getAnimation();
         EAToolDieEastPreview.display(EAToolDieWestPreview.sprite.getAnimation(), t2dAnimatedSprite);
         EAToolSetFlip(EAToolDieEastPreview.sprite, "Horizontal");
         EAToolDieEastPreview.mirrorState = "Horizontal";
         EAToolUpdateEdit(EAToolDieEastPreview);
      }
      else
      {
         // temporary code - restores original test animation to the preview window
         EAToolDieEastPreview.display(EAToolDieEastPreview.originalAnim, t2dAnimatedSprite);
         EAToolResetFlip(EAToolDieEastPreview.sprite, "");
         EAToolDieEastPreview.mirrorState = "";
         EAToolUpdateEdit(EAToolDieEastPreview);
      }
      EAToolDieEastPreview.sprite.BlendColor = (%state == 0 ? $EAToolMirroredColor : $EAToolOriginalColor);
   }
}

//-----------------------------------------------------------------------------
// Enemy Animation Tool
//-----------------------------------------------------------------------------
/// <summary>
/// This function initializes the Enemy Animation Set Tool
/// </summary>
function EnemyAnimTool::onWake(%this)
{  
   $IsEnemyAnimToolAwake = true;
   EAToolTabBook.selectPage(0);

   %this.initializePreviews();

   SetEnemyAnimToolDirtyState(false);
}

/// <summary>
/// This function cleans up when the Enemy Animation Set Tool is hidden.
/// </summary>
function EnemyAnimTool::onSleep(%this)
{
   $IsEnemyAnimToolAwake = false;
    %this.clearStates();
}

/// <summary>
/// This function handles the close button.
/// </summary>
function EnemyAnimTool::handleClose(%this)
{
   if ($Tools::TemplateToolDirty)
   {
      ConfirmDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(), "Save Enemy Animation Set Changes?", 
            "Save", "if (EnemyAnimTool.saveAnimSet()){ SetEnemyAnimToolDirtyState(false); Canvas.popDialog(EnemyAnimTool);}", 
            "Don't Save", "EnemyAnimTool.clearStates(); SetWaveToolDirtyState(false); Canvas.popDialog(EnemyAnimTool);", 
            "Cancel", "");
   }
   else
      Canvas.popDialog(EnemyAnimTool);
}

/// <summary>
/// This function confirms that any pending changes should be saved before closing the tool
/// </summary>
function EnemyAnimTool::confirmChanges(%this)
{
   if ($Tools::TemplateToolDirty)
   {
      ConfirmDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(), "Save Enemy Animation Set Changes?", 
            "Save", "EnemyAnimTool.saveAnimSet(); SetEnemyAnimToolDirtyState(false);", 
            "Don't Save", "EnemyAnimTool.initializePreviews(); SetEnemyAnimToolDirtyState(false);", 
            "Cancel", "");
   }
}

/// <summary>
/// This function resets the Enemy Animation Set Tool if changes are canceled by 
/// reloading from the original source.
/// </summary>
function EnemyAnimTool::cancel(%this)
{
   %this.initializePreviews();
   SetEnemyAnimToolDirtyState(false);
}

/// <summary>
/// This function filters names to ensure that they are valid for use as animation
/// set names.
/// </summary>
/// <param name="name">The name we wish to validate.</param>
function EnemyAnimTool::checkValidName(%this, %name)
{
    // Check for empty name 
    if (strreplace(%name, " ", "") $= "")  
    {
        // Show message dialog
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(), "Notice!", "Animation Set name cannot be empty!", 
         "", "", "", "", "Okay", "return false;");         

        
        return false;   
    }

    // Check for spaces
    if (strreplace(%name, " ", "") !$= %name)
    {
        // Show message dialog
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(), "Notice!", "Animation Set name cannot contain spaces!", 
         "", "", "", "", "Okay", "return false;");         
        
        return false;
    }
    
    // Check for invalid characters
    %invalidCharacters = "-+*/%$&§=()[].?\"#,;!~<>|°^{}";
    %strippedName = stripChars(%name, %invalidCharacters);
    if (%strippedName !$= %name)
    {
        // Show message dialog
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(), "Notice!", "Animation Set name contains invalid symbols!", 
         "", "", "", "", "Okay", "return false;"); 
         
        return false;   
    }
    
    // check for all numbers 
    %invalidCharacters = "0123456789";
    %strippedName = stripChars(%name, %invalidCharacters);
    if (%strippedName $= "")
    {
        // Show message dialog
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(), "Notice!", "Animation Set name must have at least one non-numeric character!", 
         "", "", "", "", "Okay", "return false;"); 
         
        return false;   
    }
    
    %invalidCharacters = "0123456789";
    %firstChar = getSubStr(%name, 0, 1);
    %strippedName = stripChars(%firstChar, %invalidCharacters);
    if (%strippedName $= "")
    {
        // Show message dialog
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(), "Notice!", "Animation Set name must not begin with a number!", 
         "", "", "", "", "Okay", "return false;"); 
         
        return false;   
    }

    return true;
}

/// <summary>
/// This function checks to ensure that the enemy animation set has an animation 
/// assigned for all directions and that they are of the correct type before allowing
/// the animation set to be saved.
/// </summary>
function EnemyAnimTool::checkValid(%this)
{
    // check for empty fields
    if (!isObject(EAToolWalkNorthPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "North walk animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = EAToolWalkNorthPreview.sprite.getAnimation();
    if (%datablock.animationCycle == false)
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "North walk animation is not a looping animation.  Please make this a looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }

    if (!isObject(EAToolWalkEastPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "East walk animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = EAToolWalkEastPreview.sprite.getAnimation();
    if (%datablock.animationCycle == false)
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "East walk animation is not a looping animation.  Please make this a looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }

    if (!isObject(EAToolWalkSouthPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "South walk animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = EAToolWalkSouthPreview.sprite.getAnimation();
    if (%datablock.animationCycle == false)
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "South walk animation is not a looping animation.  Please make this a looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }

    if (!isObject(EAToolWalkWestPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "West walk animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = EAToolWalkWestPreview.sprite.getAnimation();
    if (%datablock.animationCycle == false)
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "West walk animation is not a looping animation.  Please make this a looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }

    if (!isObject(EAToolDieNorthPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "North death animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = EAToolDieNorthPreview.sprite.getAnimation();
    if (%datablock.animationCycle == true)
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "North death animation is a looping animation.  Please make this a non-looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }

    if (!isObject(EAToolDieEastPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "East death animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = EAToolDieEastPreview.sprite.getAnimation();
    if (%datablock.animationCycle == true)
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "East death animation is a looping animation.  Please make this a non-looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }

    if (!isObject(EAToolDieSouthPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "South death animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = EAToolDieSouthPreview.sprite.getAnimation();
    if (%datablock.animationCycle == true)
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "South death animation is a looping animation.  Please make this a non-looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }

    if (!isObject(EAToolDieWestPreview.sprite.getAnimation()))
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "West death animation is unassigned.  Please assign an animation before saving.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }
    %datablock = EAToolDieWestPreview.sprite.getAnimation();
    if (%datablock.animationCycle == true)
    {
        WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(),
            "Notice!", 
            "West death animation is a looping animation.  Please make this a non-looping animation.", 
            "", "", 
            "OK", "return;", 
            "", "");
        return false;
    }

    return true;
}

/// <summary>
/// This function validates and then saves the current animation set.
/// </summary>
/// <return>Returns true if successful, or false if not.</param>
function EnemyAnimTool::saveAnimSet(%this)
{
   %valid = %this.checkValid();
   if (!%valid)
   {
      return false;
   }
   
   %animSetName = EAToolAnimSetNameEdit.getText();
   
    if (!%this.checkValidName(%animSetName))
        return false;

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
            WarningDialog.setupAndShow(EnemyAnimToolWindow.getGlobalCenter(), "Duplicate Name", "Another Animation Set Object in your Game Already has this Name", "", "", "OK");      
            echo(" -- EnemyAnimTool::saveAnimSet() - name conflict " @ %temp @ " : " @ %this.animationSet);
            return false;
         }
      }
      if (%noConflict)
         break;
   }
   
   if (!%noConflict)
      %this.animationSet.setName(EAToolAnimSetNameEdit.getText());

   %this.animationSet.MoveNorthAnim = EAToolWalkNorthEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.MoveNorthAnim);
   %this.animationSet.MoveNorthMirror = EAToolWalkNorthPreview.mirrorState;

   %this.animationSet.MoveEastAnim = EAToolWalkEastEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.MoveEastAnim);
   %this.animationSet.MoveEastMirror = EAToolWalkEastPreview.mirrorState;

   %this.animationSet.MoveSouthAnim = EAToolWalkSouthEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.MoveSouthAnim);
   %this.animationSet.MoveSouthMirror = EAToolWalkSouthPreview.mirrorState;

   %this.animationSet.MoveWestAnim = EAToolWalkWestEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.MoveWestAnim);
   %this.animationSet.MoveWestMirror = EAToolWalkWestPreview.mirrorState;

   %this.animationSet.DeathNorthAnim = EAToolDieNorthEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.DeathNorthAnim);
   %this.animationSet.DeathNorthMirror = EAToolDieNorthPreview.mirrorState;

   %this.animationSet.DeathEastAnim = EAToolDieEastEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.DeathEastAnim);
   %this.animationSet.DeathEastMirror = EAToolDieEastPreview.mirrorState;

   %this.animationSet.DeathSouthAnim = EAToolDieSouthEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.DeathSouthAnim);
   %this.animationSet.DeathSouthMirror = EAToolDieSouthPreview.mirrorState;

   %this.animationSet.DeathWestAnim = EAToolDieWestEdit.getText();
   AddAssetToLevelDatablocks(%this.animationSet.DeathWestAnim);
   %this.animationSet.DeathWestMirror = EAToolDieWestPreview.mirrorState;
   
   LDEonApply();
   LBProjectObj.persistToDisk(true, true, true, true, true, true);
   //%this.animationSet.save($AnimSetFile);
   //echo(" -- EnemyAnimTool::saveAnimSet() File - " @ $AnimSetFile);

   EnemyUpdateAnimSet(%this.animationSet.getName());
   
   
   return true;
}

/// <summary>
/// This function creates a new, empty animation set object and adds it to 
/// the project's list of animation sets.
/// </summary>
function EnemyAnimTool::createAnimationSet(%this)
{
   %this.animationSet = new ScriptObject() {
      canSaveDynamicFields = "1";
         class = "animationSet";
         NameTags = "2";

         MoveNorthAnim = "";
         MoveNorthMirror = "";

         MoveEastAnim = "";
         MoveEastMirror = "";

         MoveSouthAnim = "";
         MoveSouthMirror = "";

         MoveWestAnim = "";
         MoveWestMirror = "";

         DeathEastAnim = "";
         DeathEastMirror = "";

         DeathNorthAnim = "";
         DeathNorthMirror = "";

         DeathSouthAnim = "";
         DeathSouthMirror = "";

         DeathWestAnim = "";
         DeathWestMirror = "";
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
/// This function cleans up all preview states.
/// </summary>
function EnemyAnimTool::clearStates(%this)
{
   SetEnemyAnimToolDirtyState(false);
   
   EAToolWalkNorthEdit.text = "";
   EAToolWalkNorthPreview.mirrorState = "";
   EAToolWalkNorthPreview.originalAnim = "";
   EAToolWalkNorthPreview.clear();
   EAToolResetFlip(EAToolWalkNorthPreview.sprite, "");
   EAToolWalkNorthMirror.setStateOn(false);
   EAToolWalkNorthMirror.setValue(false);

   EAToolWalkEastEdit.text = "";
   EAToolWalkEastPreview.mirrorState = "";
   EAToolWalkEastPreview.originalAnim = "";
   EAToolWalkEastPreview.clear();
   EAToolResetFlip(EAToolWalkEastPreview.sprite, "");
   EAToolWalkEastMirror.setStateOn(false);
   EAToolWalkEastMirror.setValue(false);

   EAToolWalkSouthEdit.text = "";
   EAToolWalkSouthPreview.mirrorState = "";
   EAToolWalkSouthPreview.originalAnim = "";
   EAToolWalkSouthPreview.clear();
   EAToolResetFlip(EAToolWalkSouthPreview.sprite, "");
   EAToolWalkSouthMirror.setStateOn(false);
   EAToolWalkSouthMirror.setValue(false);

   EAToolWalkWestEdit.text = "";
   EAToolWalkWestPreview.mirrorState = "";
   EAToolWalkWestPreview.originalAnim = "";
   EAToolWalkWestPreview.clear();
   EAToolResetFlip(EAToolWalkWestPreview.sprite, "");
   EAToolWalkWestMirror.setStateOn(false);
   EAToolWalkWestMirror.setValue(false);

   EAToolDieNorthEdit.text = "";
   EAToolDieNorthPreview.mirrorState = "";
   EAToolDieNorthPreview.originalAnim = "";
   EAToolDieNorthPreview.clear();
   EAToolResetFlip(EAToolDieNorthPreview.sprite, "");
   EAToolDieNorthMirror.setStateOn(false);
   EAToolDieNorthMirror.setValue(false);

   EAToolDieEastEdit.text = "";
   EAToolDieEastPreview.mirrorState = "";
   EAToolDieEastPreview.originalAnim = "";
   EAToolDieEastPreview.clear();
   EAToolResetFlip(EAToolDieEastPreview.sprite, "");
   EAToolDieEastMirror.setStateOn(false);
   EAToolDieEastMirror.setValue(false);

   EAToolDieSouthEdit.text = "";
   EAToolDieSouthPreview.mirrorState = "";
   EAToolDieSouthPreview.originalAnim = "";
   EAToolDieSouthPreview.clear();
   EAToolResetFlip(EAToolDieSouthPreview.sprite, "");
   EAToolDieSouthMirror.setStateOn(false);
   EAToolDieSouthMirror.setValue(false);

   EAToolDieWestEdit.text = "";
   EAToolDieWestPreview.mirrorState = "";
   EAToolDieWestPreview.originalAnim = "";
   EAToolDieWestPreview.clear();
   EAToolResetFlip(EAToolDieWestPreview.sprite, "");
   EAToolDieWestMirror.setStateOn(false);
   EAToolDieWestMirror.setValue(false);
}

/// <summary>
/// This function loads up all animations and sets up all of the animation
/// previews.
/// </summary>
function EnemyAnimTool::initializePreviews(%this)
{
   // Need to modify this to properly handle mirrored animations on load,
   // such that if an animation is mirrored, it cannot be "unmirrored" until 
   // a valid animation is selected for the mirrored animation.
   echo(" -- EnemyAnimTool::initializePreviews() - animation set: " @%this.animationSet);
   
   if (!isObject(%this.animationSet))
      %this.createAnimationSet();
      
   EAToolAnimSetNameEdit.text = %this.animationSet.getName();
   
   %this.loadPreview("EAToolWalkNorth", "North", "Move");
   %this.loadPreview("EAToolWalkEast", "East", "Move");
   %this.loadPreview("EAToolWalkSouth", "South", "Move");
   %this.loadPreview("EAToolWalkWest", "West", "Move");
   
   %this.loadPreview("EAToolDieNorth", "North", "Death");
   %this.loadPreview("EAToolDieEast", "East", "Death");
   %this.loadPreview("EAToolDieSouth", "South", "Death");
   %this.loadPreview("EAToolDieWest", "West", "Death");
}

/// <summary>
/// This function loads up the specified animation and sets the mirror state.
/// </summary>
/// <param name="previewTag">The base name of the control without the control type at the end.</param>
/// <param name="direction">The direction tag for this animation preview - North, South, East or West.</param>
/// <param name="type">The type of animation - Move or Death.</param>
function EnemyAnimTool::loadPreview(%this, %previewTag, %direction, %type)
{
   // %previewTag is the name of the control without the identifying part at the end.
   // For example, EAToolDieWest is the tag for EAToolDieWestPreview, EAToolDieWestEdit and
   // EAToolDieWestMirror.
   // %direction should be North, South, East or West
   // %type should be Move or Death

   %preview = %previewTag @ "Preview";
   %edit = %previewTag @ "Edit";
   %mirror = %previewTag @ "Mirror";
   %anim = %type @ %direction @ "Anim";
   %edit.setActive(false);

   %animMirrorState = "";
   eval("%animMirrorState = %this.animationSet." @ %type @ %direction @ "Mirror;");

   %displayObject = %this.animationSet.GetAnimationDatablock(%anim);
   if (isObject(%displayObject))
   {
       %preview.display(%displayObject, t2dAnimatedSprite);
       %edit.text = %preview.sprite.getAnimation();
       %preview.mirrorState = %animMirrorState;
       %preview.originalMirrorState = %animMirrorState;
       
       if (%preview.mirrorState $= "")
          %preview.originalAnim = %preview.sprite.getAnimation();
       else
       {
          %mirror.visible = false;
          %preview.sprite.BlendColor = $EAToolMirroredColor;
          if (%direction $= "North")
          {
             %opposite = strreplace(%previewTag, "North", "South");
             %oppositeMirror = %opposite @ "Mirror";
             %oppositeMirror.setStateOn(true);
          }
          if (%direction $= "East")
          {
             %opposite = strreplace(%previewTag, "East", "West");
             %oppositeMirror = %opposite @ "Mirror";
             %oppositeMirror.setStateOn(true);
          }
          if (%direction $= "South")
          {
             %opposite = strreplace(%previewTag, "South", "North");
             %oppositeMirror = %opposite @ "Mirror";
             %oppositeMirror.setStateOn(true);
          }
          if (%direction $= "West")
          {
             %opposite = strreplace(%previewTag, "West", "East");
             %oppositeMirror = %opposite @ "Mirror";
             %oppositeMirror.setStateOn(true);
          }
       }

       EAToolSetFlip(%preview.sprite, %animMirrorState);
   }
   else
   {
       %preview.display("", "");
       %edit.text = "";
       %preview.mirrorState = "";
       %preview.originalMirrorState = "";
       %mirror.Visible = true;
   }
}

/// <summary>
/// This function plays the animation in the North Walk preview.
/// </summary>
function EAToolWalkNorthClick::onMouseUp(%this)
{
   EAToolNWalkPlayBtn.onClick();
}

/// <summary>
/// This function plays the animation in the East Walk preview.
/// </summary>
function EAToolWalkEastClick::onMouseUp(%this)
{
   EAToolEWalkPlayBtn.onClick();
}

/// <summary>
/// This function plays the animation in the South Walk preview.
/// </summary>
function EAToolWalkSouthClick::onMouseUp(%this)
{
   EAToolSWalkPlayBtn.onClick();
}

/// <summary>
/// This function plays the animation in the West Walk preview.
/// </summary>
function EAToolWalkWestClick::onMouseUp(%this)
{
   EAToolWWalkPlayBtn.onClick();
}

/// <summary>
/// This function plays the animation in the North Death preview.
/// </summary>
function EAToolDieNorthClick::onMouseUp(%this)
{
   EAToolNDiePlayBtn.onClick();
}

/// <summary>
/// This function plays the animation in the East Death preview.
/// </summary>
function EAToolDieEastClick::onMouseUp(%this)
{
   EAToolEDiePlayBtn.onClick();
}

/// <summary>
/// This function plays the animation in the South Death preview.
/// </summary>
function EAToolDieSouthClick::onMouseUp(%this)
{
   EAToolSDiePlayBtn.onClick();
}

/// <summary>
/// This function plays the animation in the West Death preview.
/// </summary>
function EAToolDieWestClick::onMouseUp(%this)
{
   EAToolWDiePlayBtn.onClick();
}

/// <summary>
/// This function opens the Asset Library to select an animated sprite for 
/// this preview.
/// </summary>
function EAToolWalkNorthSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function sets this preview to the selected asset.
/// </summary>
/// <param name="asset">The name of the asset passed back from the Asset Library.</param>
function EAToolWalkNorthSelectButton::setSelectedAsset(%this, %asset)
{
   EAToolWalkNorthPreview.display(%asset, t2dAnimatedSprite);
   EAToolWalkNorthEdit.text = %asset;

   if (EAToolWalkNorthPreview.mirrorState !$= "")
   {
      EAToolWalkSouthMirror.setStateOn(false);
      EAToolWalkNorthMirror.Visible = true;
      EAToolWalkNorthPreview.mirrorState = "";
      EAToolResetFlip(EAToolWalkNorthPreview.sprite);
   }

   if (EAToolWalkSouthPreview.mirrorState !$= "")
   {
      EAToolWalkSouthPreview.display(%asset, t2dAnimatedSprite);
      EAToolWalkSouthEdit.text = %asset;
      EAToolSetFlip(EAToolWalkSouthPreview.sprite, EAToolWalkSouthPreview.mirrorState);
      EAToolWalkSouthPreview.sprite.BlendColor = $EAToolMirroredColor;
   }

   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animated sprite for 
/// this preview.
/// </summary>
function EAToolWalkEastSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function sets this preview to the selected asset.
/// </summary>
/// <param name="asset">The name of the asset passed back from the Asset Library.</param>
function EAToolWalkEastSelectButton::setSelectedAsset(%this, %asset)
{
   EAToolWalkEastPreview.display(%asset, t2dAnimatedSprite);
   EAToolWalkEastEdit.text = %asset;

   if (EAToolWalkEastPreview.mirrorState !$= "")
   {
      EAToolWalkWestMirror.setStateOn(false);
      EAToolWalkEastMirror.Visible = true;
      EAToolWalkEastPreview.mirrorState = "";
      EAToolResetFlip(EAToolWalkEastPreview.sprite);
   }

   if (EAToolWalkWestPreview.mirrorState !$= "")
   {
      EAToolWalkWestPreview.display(%asset, t2dAnimatedSprite);
      EAToolWalkWestEdit.text = %asset;
      EAToolSetFlip(EAToolWalkWestPreview.sprite, EAToolWalkWestPreview.mirrorState);
      EAToolWalkWestPreview.sprite.BlendColor = $EAToolMirroredColor;
   }

   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animated sprite for 
/// this preview.
/// </summary>
function EAToolWalkSouthSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function sets this preview to the selected asset.
/// </summary>
/// <param name="asset">The name of the asset passed back from the Asset Library.</param>
function EAToolWalkSouthSelectButton::setSelectedAsset(%this, %asset)
{
   EAToolWalkSouthPreview.display(%asset, t2dAnimatedSprite);
   EAToolWalkSouthEdit.text = %asset;

   if (EAToolWalkSouthPreview.mirrorState !$= "")
   {
      EAToolWalkNorthMirror.setStateOn(false);
      EAToolWalkSouthMirror.Visible = true;
      EAToolWalkSouthPreview.mirrorState = "";
      EAToolResetFlip(EAToolWalkSouthPreview.sprite);
   }

   if (EAToolWalkNorthPreview.mirrorState !$= "")
   {
      EAToolWalkNorthPreview.display(%asset, t2dAnimatedSprite);
      EAToolWalkNorthEdit.text = %asset;
      EAToolSetFlip(EAToolWalkNorthPreview.sprite, EAToolWalkNorthPreview.mirrorState);
      EAToolWalkNorthPreview.sprite.BlendColor = $EAToolMirroredColor;
   }

   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animated sprite for 
/// this preview.
/// </summary>
function EAToolWalkWestSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function sets this preview to the selected asset.
/// </summary>
/// <param name="asset">The name of the asset passed back from the Asset Library.</param>
function EAToolWalkWestSelectButton::setSelectedAsset(%this, %asset)
{
   EAToolWalkWestPreview.display(%asset, t2dAnimatedSprite);
   EAToolWalkWestEdit.text = %asset;

   if (EAToolWalkWestPreview.mirrorState !$= "")
   {
      EAToolWalkEastMirror.setStateOn(false);
      EAToolWalkWestMirror.Visible = true;
      EAToolWalkWestPreview.mirrorState = "";
      EAToolResetFlip(EAToolWalkWestPreview.sprite);
   }

   if (EAToolWalkEastPreview.mirrorState !$= "")
   {
      EAToolWalkEastPreview.display(%asset, t2dAnimatedSprite);
      EAToolWalkEastEdit.text = %asset;
      EAToolSetFlip(EAToolWalkEastPreview.sprite, EAToolWalkEastPreview.mirrorState);
      EAToolWalkEastPreview.sprite.BlendColor = $EAToolMirroredColor;
   }

   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function sets the mirror state of this animation.
/// </summary>
function EAToolWalkNorthMirror::onClick(%this)
{
   EAToolMirrorHide(%this);
   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function sets the mirror state of this animation.
/// </summary>
function EAToolWalkEastMirror::onClick(%this)
{
   EAToolMirrorHide(%this);
   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function sets the mirror state of this animation.
/// </summary>
function EAToolWalkSouthMirror::onClick(%this)
{
   EAToolMirrorHide(%this);
   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function sets the mirror state of this animation.
/// </summary>
function EAToolWalkWestMirror::onClick(%this)
{
   EAToolMirrorHide(%this);
   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animated sprite for 
/// this preview.
/// </summary>
function EAToolDieNorthSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function sets this preview to the selected asset.
/// </summary>
/// <param name="asset">The name of the asset passed back from the Asset Library.</param>
function EAToolDieNorthSelectButton::setSelectedAsset(%this, %asset)
{
   EAToolDieNorthPreview.display(%asset, t2dAnimatedSprite);
   EAToolDieNorthEdit.text = %asset;
   
   if (EAToolDieNorthPreview.mirrorState !$= "")
   {
      EAToolDieSouthMirror.setStateOn(false);
      EAToolDieNorthMirror.Visible = true;
      EAToolDieNorthPreview.mirrorState = "";
      EAToolResetFlip(EAToolDieNorthPreview.sprite);
   }

   if (EAToolDieSouthPreview.mirrorState !$= "")
   {
      EAToolDieSouthPreview.display(%asset, t2dAnimatedSprite);
      EAToolDieSouthEdit.text = %asset;
      EAToolSetFlip(EAToolDieSouthPreview.sprite, EAToolDieSouthPreview.mirrorState);
      EAToolDieSouthPreview.sprite.BlendColor = $EAToolMirroredColor;
   }
   
   EAToolNDiePlayBtn.onClick();

   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animated sprite for 
/// this preview.
/// </summary>
function EAToolDieEastSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function sets this preview to the selected asset.
/// </summary>
/// <param name="asset">The name of the asset passed back from the Asset Library.</param>
function EAToolDieEastSelectButton::setSelectedAsset(%this, %asset)
{
   EAToolDieEastPreview.display(%asset, t2dAnimatedSprite);
   EAToolDieEastEdit.text = %asset;

   if (EAToolDieEastPreview.mirrorState !$= "")
   {
      EAToolDieWestMirror.setStateOn(false);
      EAToolDieEastMirror.Visible = true;
      EAToolDieEastPreview.mirrorState = "";
      EAToolResetFlip(EAToolDieEastPreview.sprite);
   }

   if (EAToolDieWestPreview.mirrorState !$= "")
   {
      EAToolDieWestPreview.display(%asset, t2dAnimatedSprite);
      EAToolDieWestEdit.text = %asset;
      EAToolSetFlip(EAToolDieWestPreview.sprite, EAToolDieWestPreview.mirrorState);
      EAToolDieWestPreview.sprite.BlendColor = $EAToolMirroredColor;
   }
   
   EAToolEDiePlayBtn.onClick();

   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animated sprite for 
/// this preview.
/// </summary>
function EAToolDieSouthSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function sets this preview to the selected asset.
/// </summary>
/// <param name="asset">The name of the asset passed back from the Asset Library.</param>
function EAToolDieSouthSelectButton::setSelectedAsset(%this, %asset)
{
   EAToolDieSouthPreview.display(%asset, t2dAnimatedSprite);
   EAToolDieSouthEdit.text = %asset;

   if (EAToolDieSouthPreview.mirrorState !$= "")
   {
      EAToolDieNorthMirror.setStateOn(false);
      EAToolDieSouthMirror.Visible = true;
      EAToolDieSouthPreview.mirrorState = "";
      EAToolResetFlip(EAToolDieSouthPreview.sprite);
   }

   if (EAToolDieNorthPreview.mirrorState !$= "")
   {
      EAToolDieNorthPreview.display(%asset, t2dAnimatedSprite);
      EAToolDieNorthEdit.text = %asset;
      EAToolSetFlip(EAToolDieNorthPreview.sprite, EAToolDieNorthPreview.mirrorState);
      EAToolDieNorthPreview.sprite.BlendColor = $EAToolMirroredColor;
   }
   
   EAToolSDiePlayBtn.onClick();

   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animated sprite for 
/// this preview.
/// </summary>
function EAToolDieWestSelectButton::onClick(%this)
{
   // Open the Asset Selector and pick an animation set for this direction
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function sets this preview to the selected asset.
/// </summary>
/// <param name="asset">The name of the asset passed back from the Asset Library.</param>
function EAToolDieWestSelectButton::setSelectedAsset(%this, %asset)
{
   EAToolDieWestPreview.display(%asset, t2dAnimatedSprite);
   EAToolDieWestEdit.text = %asset;

   if (EAToolDieWestPreview.mirrorState !$= "")
   {
      EAToolDieEastMirror.setStateOn(false);
      EAToolDieWestMirror.Visible = true;
      EAToolDieWestPreview.mirrorState = "";
      EAToolResetFlip(EAToolDieWestPreview.sprite);
   }

   if (EAToolDieEastPreview.mirrorState !$= "")
   {
      EAToolDieEastPreview.display(%asset, t2dAnimatedSprite);
      EAToolDieEastEdit.text = %asset;
      EAToolSetFlip(EAToolDieEastPreview.sprite, EAToolDieEastPreview.mirrorState);
      EAToolDieEastPreview.sprite.BlendColor = $EAToolMirroredColor;
   }
   
   EAToolWDiePlayBtn.onClick();

   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function sets the mirror state of this animation.
/// </summary>
function EAToolDieNorthMirror::onClick(%this)
{
   EAToolDieMirrorHide(%this);
   EAToolSDiePlayBtn.onClick();
   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function sets the mirror state of this animation.
/// </summary>
function EAToolDieEastMirror::onClick(%this)
{
   EAToolDieMirrorHide(%this);
   EAToolWDiePlayBtn.onClick();
   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function sets the mirror state of this animation.
/// </summary>
function EAToolDieSouthMirror::onClick(%this)
{
   EAToolDieMirrorHide(%this);
   EAToolNDiePlayBtn.onClick();
   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function sets the mirror state of this animation.
/// </summary>
function EAToolDieWestMirror::onClick(%this)
{
   EAToolDieMirrorHide(%this);
   EAToolEDiePlayBtn.onClick();
   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function clears the animation set name edit box.
/// </summary>
function EAToolAnimSetNameClearButton::onClick(%this)
{
   EAToolAnimSetNameEdit.text = "";
   SetEnemyAnimToolDirtyState(true);
}

/// <summary>
/// This function handles saving the animation set when the save button is clicked.
/// </summary>
function EnemyAnimSaveButton::onClick(%this)
{
   EnemyAnimTool.saveAnimSet();
   SetEnemyAnimToolDirtyState(false);
}

/// <summary>
/// This function handles the close button.
/// </summary>
function EnemyAnimCloseButton::onClick(%this)
{
   EnemyAnimTool.handleClose();
}

/// <summary>
/// This function updates the preview play button states on tab page change.
/// </summary>
function EAToolTabBook::onTabSelected(%this)
{    
   // Return is the tool is not active
   if (!$IsEnemyAnimToolAwake)   
      return;
   
    EAToolWalkNorthPreview.playBtn = EAToolNWalkPlayBtn;
    EAToolWalkEastPreview.playBtn = EAToolEWalkPlayBtn;
    EAToolWalkSouthPreview.playBtn = EAToolSWalkPlayBtn;
    EAToolWalkWestPreview.playBtn = EAToolWWalkPlayBtn;

    if (isObject(EAToolDieNorthPreview))
	{
    	EAToolDieNorthPreview.playBtn = EAToolNDiePlayBtn;
      	EAToolDieNorthPreview.play();
    	EAToolDieNorthPreview.pause();
    	EAToolDieNorthPreview.sprite.setAnimationFrame(0);
	}    
	if (isObject(EAToolDieEastPreview))
	{
    	EAToolDieEastPreview.playBtn = EAToolEDiePlayBtn;
      	EAToolDieEastPreview.play();
   	 	EAToolDieEastPreview.pause();
    	EAToolDieEastPreview.sprite.setAnimationFrame(0);
	}    
	if (isObject(EAToolDieSouthPreview))
	{
    	EAToolDieSouthPreview.playBtn = EAToolSDiePlayBtn;
      	EAToolDieSouthPreview.play();
    	EAToolDieSouthPreview.pause();
    	EAToolDieSouthPreview.sprite.setAnimationFrame(0);
	}    
	if (isObject(EAToolDieWestPreview))
	{
    	EAToolDieWestPreview.playBtn = EAToolWDiePlayBtn;
        EAToolDieWestPreview.play();
        EAToolDieWestPreview.pause();
        EAToolDieWestPreview.sprite.setAnimationFrame(0);
	}
}

/// <summary>
/// This function plays or pauses this preview animation.
/// </summary>
function EAToolNWalkPlayBtn::onClick(%this)
{
    if (EAToolWalkNorthPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        EAToolWalkNorthPreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        EAToolWalkNorthPreview.pause();
    }
}

/// <summary>
/// This function plays or pauses this preview animation.
/// </summary>
function EAToolEWalkPlayBtn::onClick(%this)
{
    if (EAToolWalkEastPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        EAToolWalkEastPreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        EAToolWalkEastPreview.pause();
    }
}

/// <summary>
/// This function plays or pauses this preview animation.
/// </summary>
function EAToolSWalkPlayBtn::onClick(%this)
{
    if (EAToolWalkSouthPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        EAToolWalkSouthPreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        EAToolWalkSouthPreview.pause();
    }
}

/// <summary>
/// This function plays or pauses this preview animation.
/// </summary>
function EAToolWWalkPlayBtn::onClick(%this)
{
    if (EAToolWalkWestPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        EAToolWalkWestPreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        EAToolWalkWestPreview.pause();
    }
}

/// <summary>
/// This function plays or pauses this preview animation.
/// </summary>
function EAToolNDiePlayBtn::onClick(%this)
{
    if (EAToolDieNorthPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        EAToolDieNorthPreview.play();
        %anim = EAToolDieNorthPreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = EAToolDieNorthPreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        EAToolDieNorthPreview.pause();
    }
}

/// <summary>
/// This function plays or pauses this preview animation.
/// </summary>
function EAToolEDiePlayBtn::onClick(%this)
{
    if (EAToolDieEastPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        EAToolDieEastPreview.play();
        %anim = EAToolDieEastPreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = EAToolDieEastPreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        EAToolDieEastPreview.pause();
    }
}

/// <summary>
/// This function plays or pauses this preview animation.
/// </summary>
function EAToolSDiePlayBtn::onClick(%this)
{
    if (EAToolDieSouthPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        EAToolDieSouthPreview.play();
        %anim = EAToolDieSouthPreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = EAToolDieSouthPreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        EAToolDieSouthPreview.pause();
    }
}

/// <summary>
/// This function plays or pauses this preview animation.
/// </summary>
function EAToolWDiePlayBtn::onClick(%this)
{
    if (EAToolDieWestPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        EAToolDieWestPreview.play();
        %anim = EAToolDieWestPreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = EAToolDieWestPreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        EAToolDieWestPreview.pause();
    }
}

/// <summary>
/// This function resets the play/pause button image to its former state.
/// </summary>
function EAToolNDiePlayBtn::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    EAToolDieNorthPreview.sprite.setAnimationFrame(0);
    EAToolDieNorthPreview.pause();
}

/// <summary>
/// This function resets the play/pause button image to its former state.
/// </summary>
function EAToolEDiePlayBtn::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    EAToolDieEastPreview.sprite.setAnimationFrame(0);
    EAToolDieEastPreview.pause();
}

/// <summary>
/// This function resets the play/pause button image to its former state.
/// </summary>
function EAToolSDiePlayBtn::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    EAToolDieSouthPreview.sprite.setAnimationFrame(0);
    EAToolDieSouthPreview.pause();
}

/// <summary>
/// This function resets the play/pause button image to its former state.
/// </summary>
function EAToolWDiePlayBtn::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    EAToolDieWestPreview.sprite.setAnimationFrame(0);
    EAToolDieWestPreview.pause();
}