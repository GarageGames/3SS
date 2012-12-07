//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$AiCardCurrentIndex = 0;
$AiCardAdvancedMode = false;
$AiCardEditorTitleStr = "NPC Strategy Tool";

/// <summary>
/// Called by the engine when gui is first set.
/// </summary>
function AiCardEditorWindow::onWake(%this)
{
   AiCardSelectPopUp.refresh();
   AiCardTabBook.selectPage(0);
   AiCardEditorWindow.refresh();
}

/// <summary>
/// Save the sub-guis values back to the current object.
/// </summary>
function AiCardEditorWindow::onSave(%this)
{
   AiCardNameEditBox.onSave();
   
   AiCardSoftArray.onSave();
   AiCardHardArray.onSave();
   AiCardSplitArray.onSave();
   AiCardBettingArray.onSave();
   
   if (isObject(AiCardSelectPopUp))
      AiCardSelectPopUp.refresh();
}

function AiCardEditorWindow::onClose(%this)
{
   AiCardEditorWindow.checkChangesDialog("exit aiEditor 0");
}

/// <summary>
/// Refresh all the sub-guis.
/// </summary>
function AiCardEditorWindow::refresh(%this)
{
   AiCardCopyTablePopUp.refresh();

   AiCardNameEditBox.refresh();
   AiCardSoftArray.refresh();
   AiCardHardArray.refresh();
   AiCardSplitArray.refresh();
   AiCardBettingArray.refresh();
   AiCardAiUsingList::refresh();
   AiCardEditorWindow.SetDirtyValue(false);
}

/// <summary>
/// Save the grid and exit the gui.
/// </summary>
function AiCardEditorWindow::doSave(%this)
{
   AiCardEditorWindow.onSave();
   export("$StrategyGrid::*", "^project/data/files/strategyGrids.cs", true, false);
   AiCardEditorWindow.SetDirtyValue(false);
}

/// <summary>
/// Run all the validation steps.
/// </summary>
function AiCardEditorWindow::doValidate(%this)
{
   AiCardNameEditBox.onValidate();
}

/// <summary>
/// Select a card.
/// </summary>
function AiCardEditorWindow::doSelectCard(%this, %id)
{
   if (%id < 0)
      $AiCardCurrentIndex = 0;
   else
      $AiCardCurrentIndex = %id;
      
   // refresh all guis
   AiCardSelectPopUp.setSelected(%id);
   AiCardEditorWindow.refresh();
}

/// <summary>
/// Open up the ai editor.
/// </summary>
function AiCardEditorWindow::doAiEditor(%this, %aiObject)
{
   if (isObject(%aiObject))
      $AiEditorCurrentObject = %aiObject;
      
   Canvas.pushDialog(AiEditorGui);
}

/// <summary>
/// Preform the actions in the str.
/// </summary>
function AiCardEditorWindow::doActions(%this, %actionStr)
{
   %count = getWordCount(%actionStr);
   for (%i = 0; %i < %count; %i++)
   {
      %action = getWord(%actionStr, %i);
      switch$ (%action)
      {
         case "exit":
            Canvas.popDialog(AiCardEditorGui);
         case "save":
            AiCardEditorWindow.doSave();
         case "card":
            %i++;
            if (%i < %count)
            {
               AiCardEditorWindow.doSelectCard(getWord(%actionStr, %i));
            }
            else
            {
               AiCardEditorWindow.doSelectCard(0);
            }
         case "aiEditor":
            %i++;
            if (%i < %count)
            {
               AiCardEditorWindow.doAiEditor(getWord(%actionStr, %i));
            }
            else
            {
               AiCardEditorWindow.doAiEditor(0);
            }
         case "help":
            gotoWebPage("http://docs.3stepstudio.com/blackjack/npcstrategy/");
      }
   }
}

/// <summary>
/// Set the 'dirty' state of the window.
/// </summary>
function AiCardEditorWindow::SetDirtyValue(%this, %dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      %this.setText($AiCardEditorTitleStr SPC "*");
   else
      %this.setText($AiCardEditorTitleStr);
}

/// <return>true if this window is 'dirty', otherwise false.</return>
function AiCardEditorWindow::IsDirty(%this)
{
   return $Tools::TemplateToolDirty;
}

/// <summary>
/// Display the 'Save Changes' dialog if dirty, then preform actions.
/// </summary>
function AiCardEditorWindow::checkChangesDialog(%this, %actionStr)
{
   AiCardEditorWindow.doValidate();
   if ($Tools::TemplateToolDirty)
   {
      $AiCardEditorActionStr = %actionStr;
      $AiCardEditorSaveActionStr = "save" SPC %actionStr;
      
      ConfirmDialog.setupAndShow(%this.getGlobalCenter(), "Save changes?", 
         "Save", "AiCardEditorWindow.doActions($AiCardEditorSaveActionStr);",
         "Don't Save", "AiCardEditorWindow.doActions($AiCardEditorActionStr);",
         "Cancel", "");
   }
   else
      AiCardEditorWindow.doActions(%actionStr);
}

//-----------------------------------------------------------------------------
// Done / Save / Help
//-----------------------------------------------------------------------------

/// <summary>
/// Handle user saves.
/// </summary>
function AiCardDoneButton::onClick(%this)
{
   AiCardEditorWindow.checkChangesDialog("exit aiEditor 0");
}

function AiCardToolSaveButton::onClick(%this)
{
   AiCardEditorWindow.doActions("save");
}

function AiCardHelpButton::onClick(%this)
{
   AiCardEditorWindow.doActions("help");
}

//-----------------------------------------------------------------------------
// AiCardClearButton
//-----------------------------------------------------------------------------

/// <summary>
/// Clear the visible array(s) to the selected action.
/// </summary>
function AiCardClearButton::onClick(%this)
{
   %action = AiCardRadioGetActive();
   if (%action $= "None")
      %action = "Hit"; 
 
   if (AiCardSoftTabPage.isVisible())
      AiCardSoftArray.clearTo(%action);
      
   if (AiCardHardTabPage.isVisible())
      AiCardHardArray.clearTo(%action);
      
   if (AiCardSplitTabPage.isVisible())
      AiCardSplitArray.clearTo(%action);

   if (AiCardBettingTabPage.isVisible())
      AiCardBettingArray.clearTo(%action);
}

//-----------------------------------------------------------------------------
// AiCardCopyTableButton
//-----------------------------------------------------------------------------

/// <summary>
/// Copy values from the card selected in the AiCardCopyTablePopUp to the visible array(s).
/// </summary>
function AiCardCopyTableButton::onClick(%this)
{
   %copyFromIndex = AiCardCopyTablePopUp.getSelected();   
   
   if (AiCardSoftTabPage.isVisible())
      AiCardSoftArray.copyFrom(%copyFromIndex);
      
   if (AiCardHardTabPage.isVisible())
      AiCardHardArray.copyFrom(%copyFromIndex);
      
   if (AiCardSplitTabPage.isVisible())
      AiCardSplitArray.copyFrom(%copyFromIndex);
      
   if (AiCardBettingTabPage.isVisible())
      AiCardBettingArray.copyFrom(%copyFromIndex);
      
   AiCardEditorWindow.SetDirtyValue(true);
}

//-----------------------------------------------------------------------------
// Helper Functions
//-----------------------------------------------------------------------------

/// <summary>
/// Get the value of the card at an index.
/// </summary>
/// <param name="%v">Card value (1-11)</param>
/// <return>Value of the card as 2-9, T, or A (0 if not a valid card).</return>
function AiCardGetCardFromValue(%v)
{
   if ((%v >= 2) && (%v <= 9))
      return %v;
      
   if ((%v == 1) || (%v == 11))
      return "A";
   
   if (%v == 10)
      return "T";
   
   return 0;
}

/// <summary>
/// Get the Profile name for the action given.
/// </summary>
/// <param name="%action">Action given (Hit, Stand, Double, Split).</param>
/// <return>Profile for action (GuiTabBookProfile if not valid).</return>
function AiCardGetProfileForAction(%action)
{
   switch$ (%action)
   {
      case "Hit":
         return "GuiBJHitProfile";
      case "Double":
         return "GuiBJDoubleProfile";
      case "Split":
         return "GuiBJSplitProfile";
      case "Stand":
         return "GuiBJStandProfile";
   }
   
   return "GuiTabBookProfile";
}

//-----------------------------------------------------------------------------
// AiSelectPopUp
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the dropdown list of available AI card styles.
/// </summary>
function AiCardSelectPopUp::refresh(%this)
{
   %this.clear();
   %index = 0;

   for (%i = 0; %i < getFieldCount($StrategyGrid::PlayStyles); %i++)
   {
      %name = getField($StrategyGrid::PlayStyles, %i) @ " (" @ %i @ ")";
      %this.add(%name, %index);
      %index++;
   }
   
   %this.setSelected($AiCardCurrentIndex);
}

/// <param="%str">String we are getting the index from.</param>
/// <return>Game strategy index.</return>
function AiCardSelectPopUp::getStrategyIndex(%this, %str)
{
   %count = getFieldCount($StrategyGrid::PlayStyles);
   for (%i = 0; %i < %count; %i++)
   {
      if (%str $= getField($StrategyGrid::PlayStyles, %i))
         return %i;      
   }
   
   return 0;
}

/// <summary>
/// Called when user selects something from popup.
/// </summary>
function AiCardSelectPopUp::onSelect(%this, %id, %text)
{
   if ($AiCardCurrentIndex != %id)
   {
      AiCardEditorWindow.checkChangesDialog("card" SPC %id);
   }
}

//-----------------------------------------------------------------------------
// AiCardCopyTablePopUp
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the dropdown list of available AI card styles.
/// </summary>
function AiCardCopyTablePopUp::refresh(%this)
{
   %this.clear();
   %index = 0;

   for (%i = 0; %i < getFieldCount($StrategyGrid::PlayStyles); %i++)
   {
      %name = getField($StrategyGrid::PlayStyles, %i) @ " (" @ %i @ ")";
      %this.add(%name, %index);
      %index++;
   }
   
   %this.setFirstSelected();
}

//-----------------------------------------------------------------------------
// AiNameEditBox
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function AiCardNameEditBox::refresh(%this)
{
   %this.text = getField($StrategyGrid::PlayStyles, $AiCardCurrentIndex);
}

/// <summary>
/// Save the Gui value back to the current object.
/// </summary>
function AiCardNameEditBox::onSave(%this)
{
   $StrategyGrid::PlayStyles = setField($StrategyGrid::PlayStyles, $AiCardCurrentIndex, %this.getValue());   
}

/// <summary>
/// Validate.
/// </summary>
function AiCardNameEditBox::onValidate(%this)
{
   // check for bad characters
   %temp = stripChars(%this.getText(), $AiNameBadChars);
   if (%temp !$= %this.getText())
   {
      // todo: alert user
      %this.text = getField($StrategyGrid::PlayStyles, $AiCardCurrentIndex);
   }
   
   if (%this.getText() !$= getField($StrategyGrid::PlayStyles, $AiCardCurrentIndex))
      AiCardEditorWindow.SetDirtyValue(true);
}

/// <summary>
/// Handle click on the 'Clear Text' button.
/// </summary>
function AiCardNameClearButton::onClick(%this)
{
   AiCardNameEditBox.text = "";
   AiCardNameEditBox.onValidate();
}

//-----------------------------------------------------------------------------
// AiCardSoftArray
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function AiCardSoftArray::refresh(%this)
{
   %this.copyFrom($AiCardCurrentIndex);  
}

/// <summary>
/// Set all the values in the array to those in a $StrategyGrid.
/// </summary>
/// <param="%copyFromIndex">Index of the $StrategyGrid to copy from.</param>
function AiCardSoftArray::copyFrom(%this, %copyFromIndex)
{
   if (%copyFromIndex < 0)
      %copyFromIndex = 0;
      
   for (%i = 12; %i < 21; %i++)
   {
      for (%j = 1; %j <= 10; %j++)
      {
         %c = AiCardGetCardFromValue(%j);
         %grid = "GridSoft_"@%i@"_"@%c;
         %action = $StrategyGrid::Soft[%copyFromIndex, %i, %c];
                  
         AiCardGridSetControlToAction(%grid, %action);
      }
   }
}

/// <summary>
/// Save the Gui value back to the current object.
/// </summary>
function AiCardSoftArray::onSave(%this)
{
   for (%i = 12; %i < 21; %i++)
   {
      for (%j = 1; %j <= 10; %j++)
      {
         %c = AiCardGetCardFromValue(%j);
         %grid = "GridSoft_"@%i@"_"@%c;
         
         $StrategyGrid::Soft[$AiCardCurrentIndex, %i, %c] = %grid.text;
      }
   }
}

/// <summary>
/// Set all the values in the array to a single action.
/// </summary>
/// <param="%action">Action to set the array to.</param>
function AiCardSoftArray::clearTo(%this, %action)
{
   for (%i = 12; %i < 21; %i++)
   {
      for (%j = 1; %j <= 10; %j++)
      {
         %c = AiCardGetCardFromValue(%j);
         %grid = "GridSoft_"@%i@"_"@%c;
         
         AiCardGridSetControlToAction(%grid, %action);
      }
   }
}

//-----------------------------------------------------------------------------
// AiCardHardArray
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function AiCardHardArray::refresh(%this)
{
   %this.copyFrom($AiCardCurrentIndex);
}

/// <summary>
/// Set all the values in the array to those in a $StrategyGrid.
/// </summary>
/// <param="%copyFromIndex">Index of the $StrategyGrid to copy from.</param>
function AiCardHardArray::copyFrom(%this, %copyFromIndex)
{
   if (%copyFromIndex < 0)
      %copyFromIndex = 0;
      
   for (%i = 8; %i < 21; %i++)
   {
      for (%j = 1; %j <= 10; %j++)
      {
         %c = AiCardGetCardFromValue(%j);
         %grid = "GridHard_"@%i@"_"@%c;
         %action = $StrategyGrid::Hard[%copyFromIndex, %i, %c];
      
         AiCardGridSetControlToAction(%grid, %action);
      }
   }
}

/// <summary>
/// Save the gui value back to the current object.
/// </summary>
function AiCardHardArray::onSave(%this)
{
   // "8 or less"
   for (%i = 2; %i < 8; %i++)
   {
      for (%j = 1; %j <= 10; %j++)
      {
         %c = AiCardGetCardFromValue(%j);
         %grid = "GridHard_8_"@%c;
         
         $StrategyGrid::Hard[$AiCardCurrentIndex, %i, %c] = %grid.text;
      }
   }   
   
   for (%i = 8; %i < 21; %i++)
   {
      for (%j = 1; %j <= 10; %j++)
      {
         %c = AiCardGetCardFromValue(%j);
         %grid = "GridHard_"@%i@"_"@%c;
         
         $StrategyGrid::Hard[$AiCardCurrentIndex, %i, %c] = %grid.text;
      }
   }
}

/// <summary>
/// Set all the values in the array to a single action.
/// </summary>
/// <param="%action">Action to set the array to.</param>
function AiCardHardArray::clearTo(%this, %action)
{
   for (%i = 2; %i < 21; %i++)
   {
      for (%j = 1; %j <= 10; %j++)
      {
         %c = AiCardGetCardFromValue(%j);
         %grid = "GridHard_"@%i@"_"@%c;
         
         AiCardGridSetControlToAction(%grid, %action);
      }
   }
}

//-----------------------------------------------------------------------------
// AiCardSplitArray
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function AiCardSplitArray::refresh(%this)
{
   %this.copyFrom($AiCardCurrentIndex);
}

/// <summary>
/// Set all the values in the array to those in a $StrategyGrid.
/// </summary>
/// <param="%copyFromIndex">Index of the $StrategyGrid to copy from.</param>
function AiCardSplitArray::copyFrom(%this, %copyFromIndex)
{   
   for (%i = 1; %i <= 10; %i++)
   {
      for (%j = 1; %j <= 10; %j++)
      {
         %o = AiCardGetCardFromValue(%i);
         %c = AiCardGetCardFromValue(%j);
         %grid = "GridSplit_"@%o@"_"@%c;
         %action = $StrategyGrid::Pairs[%copyFromIndex, %o, %c];
         
         AiCardGridSetControlToAction(%grid, %action);
      }
   }
}

/// <summary>
/// Save the gui value back to the current object.
/// </summary>
function AiCardSplitArray::onSave(%this)
{
   for (%i = 1; %i <= 10; %i++)
   {
      for (%j = 1; %j <= 10; %j++)
      {
         %o = AiCardGetCardFromValue(%i);
         %c = AiCardGetCardFromValue(%j);
         %grid = "GridSplit_"@%o@"_"@%c;
         
         $StrategyGrid::Pairs[$AiCardCurrentIndex, %o, %c] = %grid.text;
      }
   }
}

/// <summary>
/// Set all the values in the array to a single action.
/// </summary>
/// <param="%action">Action to set the array to.</param>
function AiCardSplitArray::clearTo(%this, %action)
{
   for (%i = 1; %i <= 10; %i++)
   {
      for (%j = 1; %j <= 10; %j++)
      {
         %o = AiCardGetCardFromValue(%i);
         %c = AiCardGetCardFromValue(%j);
         %grid = "GridSplit_"@%o@"_"@%c;
         
         AiCardGridSetControlToAction(%grid, %action);
      }
   }
}

//-----------------------------------------------------------------------------
// AiCardBettingArray
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the gui.
/// </summary>
function AiCardBettingArray::refresh(%this)
{
   %this.copyFrom($AiCardCurrentIndex);
}

/// <summary>
/// Set all the values in the array to those in a $StrategyGrid.
/// </summary>
/// <param="%copyFromIndex">Index of the $StrategyGrid to copy from.</param>
function AiCardBettingArray::copyFrom(%this, %copyFromIndex)
{
   %this.clearTo(0);
   
   for (%i = 0; %i < $StrategyGrid::BettingCount[%copyFromIndex]; %i++)
   {
      %grid = "GridBet_"@%i;
      %grid.text = $StrategyGrid::Betting[%copyFromIndex, %i];
   }
}

/// <summary>
/// Save the gui value back to the current object.
/// </summary>
function AiCardBettingArray::onSave(%this)
{
   %count = 0;

   for (%i = 0; %i < 10; %i++)
   {
      %grid = "GridBet_"@%i;
      %value = mRound(%grid.getValue());
      if (%value > 0)
      {
         $StrategyGrid::Betting[$AiCardCurrentIndex, %count] = %value;
         %count++;
      }      
   }
   
   $StrategyGrid::BettingCount[$AiCardCurrentIndex] = %count;
}

/// <summary>
/// Set all the values in the array to a single action.
/// </summary>
/// <param="%action">unused</param>
function AiCardBettingArray::clearTo(%this, %action)
{
   for (%i = 0; %i < 10; %i++)
   {
      %grid = "GridBet_"@%i;
      %grid.text = "";
   }
}

function AiCardBettingArray::validateGrid()
{
   ValidateTextEditInteger($ThisControl, 0, 1000);
   
   AiCardEditorWindow.SetDirtyValue(true); 
}

//-----------------------------------------------------------------------------
// AiCardGrid (helpers)
//-----------------------------------------------------------------------------

/// <return>Return the active action radio button ("None" if no action button is pressed).</return>
function AiCardRadioGetActive()
{
   if (AiCardHitRadio.getValue())
   {
      return "Hit";
   }

   if (AiCardStandRadio.getValue())
   {
      return "Stand";
   }

   if (AiCardSplitRadio.getValue())
   {
      return "Split";
   }

   if (AiCardDoubleRadio.getValue())
   {
      return "Double";
   }
   
   return "None";
}

/// <summary>
/// Set control to the profile & text of an action.
/// </summary>
/// <param="%control">Control who's values are being set.</param>
/// <param="%action">Action used to set the control.</param>
function AiCardGridSetControlToAction(%control, %action)
{
   %control.Profile = AiCardGetProfileForAction(%action);
   %control.text = %action;
      
   AiCardEditorWindow.SetDirtyValue(true);   
}

/// <summary>
/// Set $ThisControl text and profile to the current selected AiCard Radio button.
/// </summary>
function AiCardGrid::setGridToCurrentAction()
{
   %ctrlName = $ThisControl.getName();
   
   %action = AiCardRadioGetActive();
   if (%action !$= "None")
   {
      // don't paint 'split' on anything except the Split array.
      if ((strstr(%ctrlName, "GridSplit_") == 0)
        || (%action !$= "Split"))
      {
         AiCardGridSetControlToAction($ThisControl, %action);
         return;
      }
   }
   
   // fallback, just go to the next action
   switch$ ($ThisControl.text)
   {
      case "Hit":
         AiCardGridSetControlToAction($ThisControl, "Stand");
      
      case "Stand":
         if (strstr(%ctrlName, "GridSplit_") == 0)
            AiCardGridSetControlToAction($ThisControl, "Split");
         else
            AiCardGridSetControlToAction($ThisControl, "Double");
      
      case "Split":
         AiCardGridSetControlToAction($ThisControl, "Double");
      
      default:
         AiCardGridSetControlToAction($ThisControl, "Hit");
   }
}

//-----------------------------------------------------------------------------
// AiCardAiUsingList
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the list of AI using the current strategy.
/// </summary>
function AiCardAiUsingList::refresh(%this)
{
   %index = 1;
   
   if(isObject($persistentObjectSet))
   {
      %count = $persistentObjectSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %object = $persistentObjectSet.getObject(%i);

         if (isObject(%object.getBehavior("BlackjackAiPlayerBehavior"))
             && (%object.callOnBehaviors(getIsAvailable))
             && (%object.callOnBehaviors(getStrategy) == $AiCardCurrentIndex))
         {
            (AiCardAiUsingContainer@%index).Visible = true;
            (AiCardAiUsingPreviewBitmap@%index).display(%object.getImageMap(), "t2dStaticSprite");
            (AiCardAiUsingPreviewBitmap@%index).sprite.setFrame(%object.getFrame());            
            (AiCardAiUsingButton@%index).text = "Edit " @ %object.getInternalName();
            (AiCardAiUsingButton@%index).object = %object;
            %index++;
         }
      }
      
      while (%index <= 10)
      {
         (AiCardAiUsingContainer@%index).Visible = false;
         %index++;
      }
   }
}

//-----------------------------------------------------------------------------
// AiCardAiUsingButton
//-----------------------------------------------------------------------------

/// <summary>
/// Close this editor and open the AiEditor with the selected AI.
/// </summary>
function AiCardAiUsingButton::editAi()
{
   if (!isObject($ThisControl) || !isObject($ThisControl.object))
      return;
      
   AiCardEditorWindow.checkChangesDialog("exit aiEditor" SPC $ThisControl.object);
}

//-----------------------------------------------------------------------------
// AiCardTabBook
//-----------------------------------------------------------------------------

/// <summary>
/// Handle tab selections.
/// </summary>
function AiCardTabBook::onTabSelected(%this, %text, %index)
{
    switch$ (%text)
    {
        case "NPC Selection":
            AiCardCopyGroup.Visible = false;
        default:
            AiCardCopyGroup.Visible = true;        
    }
}
