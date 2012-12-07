//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// Contains the current AI Player being edited.
$AiEditorCurrentObject = 0;

/// Title bar text
$AiEditorTitleStr = "NPC";

/// Characters not allowed in a AI name.
$AiNameBadChars = "/\\*~`!@#$%^&*()-=+[]{}'\";:?,.<>";

/// <summary>
/// Called by the engine when gui is first set.
/// </summary>
function AiEditorWindow::onWake(%this)
{
   %this.objectCount = 0;
   
   AiSelectPopUp.refresh();
}

/// <summary>
/// Save the sub-guis values back to the current object.
/// </summary>
function AiEditorWindow::onSave(%this)
{
   AiNameEditBox.onSave();
   AiAvatarImageEditBox.onSave();   
   AiBankEditBox.onSave();
   AiStrategyPopUp.onSave();
   AiReactionTimeEditBox.onSave();
   
   AiEditorWindow.SetDirtyValue(false);
   if (isObject(AiSelectPopUp))
      AiSelectPopUp.refresh();
      
   LBProjectObj.saveLevel();
   SaveAllLevelDatablocks();
}

function AiEditorWindow::onClose(%this)
{
   AiEditorWindow.checkChangesDialog("exit");
}

/// <summary>
/// Run all the validation steps.
/// </summary>
function AiEditorWindow::doValidate(%this)
{
   AiNameEditBox.onValidate();
   AiAvatarImageEditBox.onValidate();
   AiBankEditBox.onValidate();
   AiReactionTimeEditBox.onValidate();   
}

/// <summary>
/// Refresh all the sub-guis.
/// </summary>
function AiEditorWindow::refresh(%this)
{
   if ($AiEditorCurrentObject == 0)
   {
       AiActiveGroup.Visible = false;
   }
   else
   {
       AiActiveGroup.Visible = true;

       AiNameEditBox.refresh();   
       AiAvatarImageEditBox.refresh();   
       AiBankEditBox.refresh();   
       AiStrategyPopUp.refresh();
       AiReactionTimeEditBox.refresh();
   }
}

/// <summary>
/// Select an AI
/// </summary>
function AiEditorWindow::doSelectAi(%this, %id)
{
   if (%id < 0)
      $AiEditorCurrentObject = 0;
   else
      $AiEditorCurrentObject = AiSelectPopUp.aiObject[%id];
      
   AiEditorWindow.refresh();
   
   AiEditorWindow.SetDirtyValue(false);
}

/// <summary>
/// Open up the ai card editor.
/// </summary>
function AiEditorWindow::doAiCardEditor(%this)
{
   Canvas.pushDialog(AiCardEditorGui);
   AiCardEditorWindow.doSelectCard(AiStrategyPopUp.getSelected());
}

/// <summary>
/// Save all changes and exit the gui.
/// </summary>
function AiEditorWindow::doSave(%this)
{
   AiEditorWindow.onSave();
// TODO: save level.
}

/// <summary>
/// Remove the currently selected object and refresh the guis.
/// </summary>
function AiEditorWindow::removeCurrentObject(%this)
{
   if (!isObject($AiEditorCurrentObject))
      return;
      
   $AiEditorCurrentObject.callOnBehaviors(setIsAvailable, false);
   for (%i = 1; %i <= 5; %i++)
        $AiEditorCurrentObject.callOnBehaviors(setIsAvailableOn, %i, false);
   %this.onSave();
        
   $AiEditorCurrentObject = 0;
   %this.onWake();   // to refresh *everything*
}

/// <summary>
/// Preform the actions in the str.
/// </summary>
function AiEditorWindow::doActions(%this, %actionStr)
{
   %count = getWordCount(%actionStr);
   for (%i = 0; %i < %count; %i++)
   {
      %action = getWord(%actionStr, %i);
      switch$ (%action)
      {
         case "exit":
            Canvas.popDialog(AiEditorGui);
         case "save":
            AiEditorWindow.doSave();
         case "ai":
            %i++;
            if (%i < %count)
            {
               AiEditorWindow.doSelectAi(getWord(%actionStr, %i));
            }
            else
            {
               AiEditorWindow.doSelectAi(0);
            }
         case "aiCardEditor":
            AiEditorWindow.doAiCardEditor();
         case "help":
           gotoWebPage("http://docs.3stepstudio.com/blackjack/npc/");
      }
   }
}

/// <summary>
/// Set the 'dirty' state of the window.
/// </summary>
function AiEditorWindow::SetDirtyValue(%this, %dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      %this.setText($AiEditorTitleStr SPC "*");
   else
      %this.setText($AiEditorTitleStr);
}

/// <return>true if this window is 'dirty', otherwise false.</return>
function AiEditorWindow::IsDirty(%this)
{
   return $Tools::TemplateToolDirty;
}

/// <summary>
/// Display the 'Save Changes' dialog if dirty, then preform actions.
/// </summary>
function AiEditorWindow::checkChangesDialog(%this, %actionStr)
{
   if (isObject($AiEditorCurrentObject))
    AiEditorWindow.doValidate();
    
   if ($Tools::TemplateToolDirty)
   {
      $AiEditorActionStr = %actionStr;
      $AiEditorSaveActionStr = "save" SPC %actionStr;
      
      ConfirmDialog.setupAndShow(%this.getGlobalCenter(), "Save changes?", 
         "Save", "AiEditorWindow.doActions($AiEditorSaveActionStr);",
         "Don't Save", "AiEditorWindow.doActions($AiEditorActionStr);",
         "Cancel", "");
   }
   else
      AiEditorWindow.doActions(%actionStr);
}

//-----------------------------------------------------------------------------
// Done / Save / Help
//-----------------------------------------------------------------------------

function AiDoneButton::onClick(%this)
{
   AiEditorWindow.checkChangesDialog("exit");
}

function AiToolSaveButton::onClick(%this)
{
   AiEditorWindow.doActions("save");
}

function AiHelpButton::onClick(%this)
{
   AiEditorWindow.doActions("help");
}

//-----------------------------------------------------------------------------
// Create / Remove
//-----------------------------------------------------------------------------

/// <summary>
/// Create a new AI (if there is a slot available).
/// </summary>
function AiCreate::onClick(%this)
{
    AiEditorWindow.doValidate();
    if ($Tools::TemplateToolDirty)
    {
        ConfirmDialog.setupAndShow(AiEditorWindow.getGlobalCenter(), "Save changes to '" @ $AiEditorCurrentObject.getInternalName() @ "'?!", 
            "Save", "AiEditorWindow.doSave(); AiEditorWindow.doCreateAi();", 
            "Don't Save", "AiEditorWindow.doCreateAi();", 
            "Cancel", "");
    }
    else
    {
        AiEditorWindow.doCreateAi();  
    }
}

/// <summary>
/// Create the first AI (if there is a slot available).
/// </summary>
function AiCreateFirst::onClick(%this)
{
   AiEditorWindow.doCreateAi(); 
}

function AiEditorWindow::doCreateAi(%this)
{
    if(isObject($persistentObjectSet) && (AiEditorWindow.objectCount < 10))
   {
      %index = 0;
      %count = $persistentObjectSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %object = $persistentObjectSet.getObject(%i);

         if (isObject(%object.getBehavior("BlackjackAiPlayerBehavior")))
         {
            %index++;
            if (!(%object.callOnBehaviors(getIsAvailable)))
            {
               $AiEditorCurrentObject = %object;
               AiEditorSetObjectToDefault($AiEditorCurrentObject);
               $AiEditorCurrentObject.setName("npc"@%index);
               $AiEditorCurrentObject.setInternalName("npc"@%index);
               $AiEditorCurrentObject.callOnBehaviors(setIsAvailable, true);
               AiEditorWindow.onWake();
               
               return;
            }
         }
      }
   }
   
   // Couldn't find an available slot.
   ConfirmDialog.setupAndShow(AiEditorWindow.getGlobalCenter(), "AI slots full!", 
         "", "", "", "", "Okay", ""); 
}

/// <summary>
/// Bring up the "Delete?" dialog.
/// </summary>
function AiRemove::onClick(%this)
{
   ConfirmDialog.setupAndShow(AiEditorWindow.getGlobalCenter(), "Really Delete '" @ $AiEditorCurrentObject.getInternalName() @ "'?!", 
      "Delete", "AiEditorWindow.removeCurrentObject();", "", "", "Cancel", "");   
}


//-----------------------------------------------------------------------------
// Helper functions
//-----------------------------------------------------------------------------

/// <param="%index">Strategy index we want the name for.</param>
/// <return>Strategy name (empty if bad index).</return>
function AiGetStrategyNameForIndex(%index)
{
   %count = getFieldCount($StrategyGrid::PlayStyles);
   if ((%index >= 0) && (%index < %count))
   {
      return getField($StrategyGrid::PlayStyles, %index);
   }
   
   return "";
}

/// <summary>
/// Set object values to starting defaults.
/// </summary>
function AiEditorSetObjectToDefault(%object)
{
   if (!(isObject(%object.getBehavior("BlackjackAiPlayerBehavior"))))
      return;

   %object.setInternalName("npc");
   %object.callOnBehaviors(setStrategy, 1);
   %object.callOnBehaviors(setReactionTime, 1);
   %object.callOnBehaviors(setCash, 1000);
}

//-----------------------------------------------------------------------------
// AiSelectPopUp
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the dropdown list of available AIs.
/// </summary>
function AiSelectPopUp::refresh(%this)
{
   %this.clear();
   AiEditorWindow.objectCount = 0;
   $selectedObject = 0;
   
   %index = 0;
   if(isObject($persistentObjectSet) && ($persistentObjectSet.getCount() > 0))
   {
      %count = $persistentObjectSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %object = $persistentObjectSet.getObject(%i);

         if (isObject(%object.getBehavior("BlackjackAiPlayerBehavior"))
                      && (%object.callOnBehaviors(getIsAvailable)))
         {
            %this.add(%object.getInternalName(), %index);
            %this.aiObject[%index] = %object;
            if (%object == $AiEditorCurrentObject)
               $selectedObject = %index;            
            
            %index++;
         }
      }
   }
   
   if (%index <= 0)
   {
       $selectedObject = -1;
   }
   %this.setSelected($selectedObject);
   
   AiEditorWindow.objectCount = %index;
   AiEditorWindow.doActions("ai" SPC $selectedObject);
}

/// <summary>
/// Handle Ai popup selection event.
/// </summary>
function AiSelectPopUp::onSelect(%this, %id, %text)
{
   if (($AiEditorCurrentObject != 0) && ($AiEditorCurrentObject != %this.aiObject[%id]))
   {
        AiEditorWindow.doValidate();
        if ($Tools::TemplateToolDirty)
        {
            $AiEditorActionStr = "ai" SPC %id;
            $AiEditorSaveActionStr = "save" SPC "ai" SPC %id;
      
            ConfirmDialog.setupAndShow(%this.getGlobalCenter(), "Save changes?", 
                "Save", "AiEditorWindow.doActions($AiEditorSaveActionStr);",
                "Don't Save", "AiEditorWindow.doActions($AiEditorActionStr);",
                "Cancel", "AiSelectPopUp.setSelected($selectedObject);");
        }
        else
        {
            AiEditorWindow.doActions("ai" SPC %id); 
        }
   }
}

//-----------------------------------------------------------------------------
// AiNameEditBox
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function AiNameEditBox::refresh(%this)
{
   if (isObject($AiEditorCurrentObject))
      %this.text = $AiEditorCurrentObject.getInternalName();
}

/// <summary>
/// Save the Gui value back to the current object.
/// </summary>
function AiNameEditBox::onSave(%this)
{
   if (isObject($AiEditorCurrentObject))
   {
      $AiEditorCurrentObject.setInternalName(%this.getValue());
   }
}

/// <summary>
/// Validate.
/// </summary>
function AiNameEditBox::onValidate(%this)
{
   // check for bad characters
   %temp = stripChars(%this.getText(), $AiNameBadChars);
   if (%temp !$= %this.getText())
   {
      // todo: alert user
      %this.text = $AiEditorCurrentObject.getInternalName();
   }
   
   if (%this.getText() !$= $AiEditorCurrentObject.getInternalName())
   {
      AiEditorWindow.SetDirtyValue(true);
   }
}

/// <summary>
/// Handle click on the 'Clear Text' button.
/// </summary>
function AiNameClearButton::onClick(%this)
{
   AiNameEditBox.text = "";
   AiNameEditBox.onValidate();
}

//--------------------------------
// AiPreviewBitmap
//--------------------------------

/// <summary>
/// Refresh the t2dStaticSprite Gui.
/// </summary>
function AiPreviewBitmap::refresh(%this)
{ 
   if (AiAvatarImageEditBox.getText() $= "")
   {
      AiPreviewBitmap.display("");
      return;  
   }

   AiPreviewBitmap.display(AiAvatarImageEditBox.getText(), "t2dStaticSprite");
   AiPreviewBitmap.sprite.setFrame(AiAvatarImageFrameEditBox.getValue());
   
   // Toggle frame select visibility based on number of frames.
   AiAvatarImageFrameContainer.Visible = (AiAvatarImageEditBox.getText().getFrameCount() > 1);
}

//-----------------------------------------------------------------------------
// AiAvatarImageEditBox
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function AiAvatarImageEditBox::refresh(%this)
{
   if (isObject($AiEditorCurrentObject))
   {
      %this.text = $AiEditorCurrentObject.getImageMap();
   }
   else
   {
      %this.text = "";
   }
   AiAvatarImageFrameEditBox.refresh();
      
   %this.setActive(false);
   
   AiPreviewBitmap.refresh();
}

/// <summary>
/// Save the Gui value back to the current object.
/// </summary>
function AiAvatarImageEditBox::onSave(%this)
{
   if (isObject($AiEditorCurrentObject))
   {
      $AiEditorCurrentObject.setImageMap(%this.getText());
      $AiEditorCurrentObject.setFrame(AiAvatarImageFrameEditBox.getValue());
   }
}

/// <summary>
/// Validate.
/// </summary>
function AiAvatarImageEditBox::onValidate(%this)
{
   // This field is set with AssetLibrary, no need to validate.

   if (%this.getText() !$= $AiEditorCurrentObject.getImageMap())
      AiEditorWindow.SetDirtyValue(true);
}

/// <summary>
/// AssectSelector callback.
/// </summary>
function AiAvatarImageEditBox::setSelectedAsset(%this, %asset)
{
   %this.text = %asset;
   AiAvatarImageFrameEditBox.onValidate(); 
   
   AiEditorWindow.SetDirtyValue(true);
}

//-----------------------------------------------------------------------------
// AiAvatarImageButton
//-----------------------------------------------------------------------------

/// <summary>
/// Open AssectSelector.
/// </summary>
function AiAvatarImageButton::onClick(%this)
{
   AssetLibrary.open(AiAvatarImageEditBox, $SpriteSheetPage, "Avatar");
}

//-----------------------------------------------------------------------------
// AiAvatarImage Frame Selector
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function AiAvatarImageFrameEditBox::refresh(%this)
{
   if (isObject($AiEditorCurrentObject))
      %this.text = $AiEditorCurrentObject.getFrame();
   else
      %this.text = "0";
}

/// <summary>
/// Validate frame number.
/// </summary>
function AiAvatarImageFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (AiAvatarImageEditBox.getText().getFrameCount() - 1))
      %this.setValue(AiAvatarImageEditBox.getText().getFrameCount() - 1);
      
   AiPreviewBitmap.refresh();
   
   if (isObject($AiEditorCurrentObject) && (%this.getText() !$= $AiEditorCurrentObject.getFrame()))
      AiEditorWindow.SetDirtyValue(true);
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function AiAvatarImageFrameSpinLeft::onClick(%this)
{
   if (AiAvatarImageFrameEditBox.getValue() > 0)
   {
      AiAvatarImageFrameEditBox.setValue(AiAvatarImageFrameEditBox.getValue() - 1);
      AiPreviewBitmap.refresh();
      AiAvatarImageFrameEditBox.onValidate();
   }
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function AiAvatarImageFrameSpinRight::onClick(%this)
{
   if (AiAvatarImageFrameEditBox.getValue() < (AiAvatarImageEditBox.getText().getFrameCount() - 1))
   {
      AiAvatarImageFrameEditBox.setValue(AiAvatarImageFrameEditBox.getValue() + 1);
      AiPreviewBitmap.refresh();
      AiAvatarImageFrameEditBox.onValidate();
   }
}

//-----------------------------------------------------------------------------
// AiBankEditBox
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function AiBankEditBox::refresh(%this)
{
   if (isObject($AiEditorCurrentObject))
      %this.text = $AiEditorCurrentObject.callOnBehaviors(getCash);
   else
      %this.text = "";
}

/// <summary>
/// Save the currently selected value back to the current object.
/// </summary>
function AiBankEditBox::onSave(%this)
{
   // todo: convert text into int
   $AiEditorCurrentObject.callOnBehaviors(setCash, %this.getText());
}

/// <summary>
/// Validate.
/// </summary>
function AiBankEditBox::onValidate(%this)
{
   ValidateTextEditInteger(%this, $GeneralSettingsStartingBankMin, $GeneralSettingsStartingBankMax);

   if (%this.getText() !$= $AiEditorCurrentObject.callOnBehaviors(getCash))
      AiEditorWindow.SetDirtyValue(true);
}

/// <summary>
/// Handle click on the spin up button.
/// </summary>
function AiBankSpinUp::onClick(%this)
{
   AiBankEditBox.setValue(AiBankEditBox.getValue() + 1);
   AiBankEditBox.onValidate();
}

/// <summary>
/// Handle click on the spin down button.
/// </summary>
function AiBankSpinDown::onClick(%this)
{
   AiBankEditBox.setValue(AiBankEditBox.getValue() - 1);
   AiBankEditBox.onValidate();
}

//-----------------------------------------------------------------------------
// AiStrategyPopUp
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the dropdown box for AiStrategyPopUp
/// </summary>
function AiStrategyPopUp::refresh(%this)
{
   if (isObject($AiEditorCurrentObject))
      %selIndex = $AiEditorCurrentObject.callOnBehaviors(getStrategy);
   else
      %selIndex = 0;
     
   // add/select dropdown values   
   %this.clear();
   for (%i = 0; %i < getFieldCount($StrategyGrid::PlayStyles); %i++)
   {
      %name = getField($StrategyGrid::PlayStyles, %i);
      %this.add(%name, %i);
   }
   
   %this.setSelected(%selIndex);
}

/// <summary>
/// Handle Ai popup selection event.
/// </summary>
function AiStrategyPopUp::onSelect(%this, %id, %text)
{
   if (isObject($AiEditorCurrentObject))
   {
      if (%id != $AiEditorCurrentObject.callOnBehaviors(getStrategy))
      {
         AiEditorWindow.SetDirtyValue(true);
      }
   }
}

/// <summary>
/// Save the currently selected value back to the current object.
/// </summary>
function AiStrategyPopUp::onSave(%this)
{
   if (isObject($AiEditorCurrentObject))
      $AiEditorCurrentObject.callOnBehaviors(setStrategy, %this.getSelected());
}

//-----------------------------------------------------------------------------
// AiStrategyButton
//-----------------------------------------------------------------------------

/// <summary>
/// React to user clicking to Strategy button.
/// </summary>
function AiStrategyButton::onClick(%this)
{
   AiEditorWindow.checkChangesDialog("exit aiCardEditor");
}

//-----------------------------------------------------------------------------
// AiReactionTimeEditBox
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui
/// </summary>
function AiReactionTimeEditBox::refresh(%this)
{
   if (isObject($AiEditorCurrentObject))
      %this.text = $AiEditorCurrentObject.callOnBehaviors(getReactionTime);
   else
      %this.text = "";
}

/// <summary>
/// Save the currently selected value back to the current object.
/// </summary>
function AiReactionTimeEditBox::onSave(%this)
{
   $AiEditorCurrentObject.callOnBehaviors(setReactionTime, %this.getText());
}

/// <summary>
/// Validate.
/// </summary>
function AiReactionTimeEditBox::onValidate(%this)
{
   ValidateTextEditInteger(%this, 1, 10);
   
   if (%this.getText() !$= $AiEditorCurrentObject.callOnBehaviors(getReactionTime))
      AiEditorWindow.SetDirtyValue(true);
}

/// <summary>
/// Handle click on the spin up button.
/// </summary>
function AiReactionTimeSpinUp::onClick(%this)
{
   AiReactionTimeEditBox.setValue(AiReactionTimeEditBox.getValue() + 1);
   AiReactionTimeEditBox.onValidate();
}

/// <summary>
/// Handle click on the spin down button.
/// </summary>
function AiReactionTimeSpinDown::onClick(%this)
{
   AiReactionTimeEditBox.setValue(AiReactionTimeEditBox.getValue() - 1);
   AiReactionTimeEditBox.onValidate();
}
