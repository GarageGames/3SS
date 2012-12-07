//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$TableEditorName = "Table Tool";

$TableEditorGUIMaxSeats = 5;

/// Contains the current object being edited.
$ShoeEditorCurrentObject = 0;

$LevelSaveDirectory = "^project/data/levels/";

/// <summary>
/// Called when another tab is selected
/// </summary>
function TableEditorTabBook::onTabSelected(%this, %tab)
{
   alxStop($SoundContainerCurrentlyPlayingSound);
}

/// <summary>
/// Called when the control object is registered with the system after the control has been created. 
/// </summary>
function TableEditorGUI::onAdd(%this)
{
   $GameDir = LBProjectObj.gamePath;
}

/// <summary>
/// Called when the control is woken up. 
/// </summary>
function TableEditorGUI::onWake(%this)
{
   TableEditorTabBook.selectPage(0);
   
   %this.currentTable = $CurrentTable;
   %this.currentTableObject = $CurrentTable.owner; 
   
   $ShoeEditorCurrentObject = $CurrentShoe.owner;
   
   %this.refresh();
   
   // Validate any active textEdits
   %this.validateAll();
   
   // Serialized the starting state of the gui
   $CurrentGuiSerialString = %this.serialize();
   %this.SetDirtyValue(false);
   
    // Start the update loop for checking the dirty state
   %this.updateDirty();

   if (!isObject($CurrentTable))
   {
      TableEditorWindow.Visible = false;
      
      ConfirmDialog.setupAndShow(GeneralSettingsEditorWindow.getGlobalCenter(), "Open a Table level to use this tool.", 
      "", "", 
      "", "", 
      "Okay", "Canvas.popDialog(TableEditorGui);"); 
   }
   else
      TableEditorWindow.Visible = true;
}

/// <summary>
/// Called when the control is put to sleep. 
/// </summary>
function TableEditorGUI::onSleep(%this)
{
   if (isEventPending(%this.updateDirtyHandle))  
   {
      cancel(%this.updateDirtyHandle);
   }
   
   alxStop($SoundContainerCurrentlyPlayingSound);
}

/// <summary>
/// Update the dirty state of the GUI by comparing the current serialized state
/// with an initial state. Schedules the next update.
/// </summary>
function TableEditorGUI::updateDirty(%this)
{
    if(!%this.IsDirty())
    {
        if ($CurrentGuiSerialString !$= %this.serialize())
            %this.SetDirtyValue(true);
    }
   
   %this.updateDirtyHandle = %this.schedule(400, updateDirty);
}

/// <summary>
/// Set the 'dirty' state of the window.
/// </summary>
function TableEditorGUI::SetDirtyValue(%this, %dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      TableEditorWindow.setText($TableEditorName SPC "*");
   else
      TableEditorWindow.setText($TableEditorName);
}

/// <return>true if this window is 'dirty', otherwise false.</return>
function TableEditorGUI::IsDirty(%this)
{
   return $Tools::TemplateToolDirty;
}

/// <summary>
/// Updates the GUI state from the game data.
/// </summary>
function TableEditorGUI::refresh(%this)
{
    if (!isObject($CurrentTable))
        return;   
    
   %scene = ToolManager.getLastWindow().getScene();
   
   // Update Table Name
   %levelFile = LBProjectObj.currentLevelFile;
   TableEditorTableNameEditText.setText(fileBase(%levelFile));
   TableEditorTableNameEditText.setActive(true);
   
   // Update Table Image
   TableEditorTableImageEditText.setText(%this.currentTableObject.getImageMap());
   TableEditorTableImageFrameEditBox.setValue(%this.currentTableObject.getFrame());
   TableEditorTableImageEditText.refreshDisplay();
   
   // Update Table Rules Image
   if (isObject(TableRulesImage))
   {
      TableEditorRulesImageEditText.setText(TableRulesImage.getImageMap());
      TableEditorRulesImageFrameEditBox.setValue(TableRulesImage.getFrame());
      TableEditorRulesImageEditText.refreshDisplay();
   }
   
   // Update Betting Circle Image
   if (isObject(BetArea0))
   {
      TableEditorBetCircleImageEditText.setText(BetArea0.getImageMap());
      TableEditorBetCircleImageFrameEditBox.setValue(BetArea0.getFrame());
      TableEditorBetCircleImageEditText.refreshDisplay();
   }

   // Update Card Box Image
   if (isObject($TableSeatArray[0]))
   {
      TableEditorCardBoxImageEditText.setText($TableSeatArray[0].owner.getImageMap());
      TableEditorCardBoxImageFrameEditBox.setValue($TableSeatArray[0].owner.getFrame());
      TableEditorCardBoxImageEditText.refreshDisplay();
   }
   
   // Update Dealer Chip Rack
   if (isObject(DealerChipRack))
   {
      TableEditorDealerChipRackImageEditText.setText(DealerChipRack.getImageMap());
      TableEditorDealerChipRackImageFrameEditBox.setValue(DealerChipRack.getFrame());
      TableEditorDealerChipRackImageEditText.refreshDisplay();
   }
   
   // Update Number of seats popup
   TableEditorNumSeatsPopup.clear();
   %count = 0;
   for (%i = 0; %i < $TableSeatCount; %i++)
   {
      TableEditorNumSeatsPopup.add(%i+1, %i);
      
      if ($TableSeatArray[%i].owner.getVisible())
         %count++;
   }
   TableEditorNumSeatsPopup.setSelected(%count);
   
   %seatBehaviorList = %this.getBehaviorList("TableSeatBehavior");
   %count = 0;
   for (%i = 0; %i < getWordCount(%seatBehaviorList); %i++)
   {
      %seatObject = getWord(%seatBehaviorList, %i).owner;
      if (%seatObject.getVisible())
         %count++;
   }
   TableEditorNumSeatsPopup.setSelected(%count - 1);
   
   // Update Table Music
   %backgroundMusicBehavior = $CurrentTable.owner.getBehavior("BackgroundMusicBehavior");
   if (isObject(%backgroundMusicBehavior))
   {
      %songProfile = %backgroundMusicBehavior.songProfile;
      setSoundContainer(TableEditorTableMusicContainer, %songProfile); 
   }
   
   // Update Standard Payout
   %standardPayout = $CurrentBetRules.getWinPayout();
   TableEditorStandardPayoutEdit1.setText(getWord(%standardPayout, 0));
   TableEditorStandardPayoutEdit2.setText(getWord(%standardPayout, 1));
   
   // Update Blackjack Payout
   %blackjackPayout = $CurrentBetRules.getBlackjackPayout();
   TableEditorBlackjackPayoutEdit1.setText(getWord(%blackjackPayout, 0));
   TableEditorBlackjackPayoutEdit2.setText(getWord(%blackjackPayout, 1));
   
   // Update Minimum Bet
   %minBet = $CurrentBetRules.minBet;
   TableEditorMinBetEdit.setText(%minBet);
   
   // Update Maximum Bet
   %maxBet = $CurrentBetRules.maxBet;
   TableEditorMaxBetEdit.setText(%maxBet);    
   
   // Update Hand Count Visible
   %showHandValues = $CurrentTable.showHandValues;
   TableEditorHandCountCheckBox.setStateOn(%showHandValues);
   
   // Update Split any Pair
   %canSplit = $CurrentTable.canSplit;
   TableEditorSplitAnyPairCheckBox.setStateOn(%canSplit);
   
   // Update Split 10's and Royals
   %canSplitTens = $CurrentTable.canSplitTens;
   TableEditorSplit10CheckBox.setStateOn(%canSplitTens);
   TableEditorSplitTensContainer.setVisible(%canSplit);
   
   // Update Basic Double Down
   %canDoubleDown = $CurrentTable.canDoubleDown;
   TableEditorDoubleDownCheckBox.setStateOn(%canDoubleDown);
   
   // Update Double Down after Split
   %canDoubleAfterSplit = $CurrentTable.canDoubleAfterSplit;
   TableEditorDoubleOnSplitCheckBox.setStateOn(%canDoubleAfterSplit);
   TableEditorDoubleOnSplitContainer.setVisible(%canDoubleDown);
   
   //------------------------------
   // Shoe
   //------------------------------
   ShoeBodyImageEditBox.refresh();
   ShoeDiscardImageEditBox.refresh();
   ShoePenetrationImageEditBox.refresh();   
   ShoeShuffleTimeEditBox.refresh();
   ShoeDeckImageEditBox.refresh();
   ShoeNumberOfDecksSlider.refresh();
   ShoePenetrationPercentSlider.refresh();
   
   //------------------------------
   // Player
   //------------------------------
   TableEditorPlayerList::refresh();   
}

/// <summary>
/// Handle click on the 'Clear Text' button.
/// </summary>
function TableEditorTableNameClearButton::onClick(%this)
{
   TableEditorTableNameEditText.text = "";
}

//-----------------------------------------------------------------------------
// Done / Save / Help / Done
//-----------------------------------------------------------------------------

/// <summary>
/// onClick callback.
/// </summary>
function TableEditorDoneButton::onClick(%this)
{
   TableEditorGUI.cancelButtonPressed();
}

/// <summary>
/// onClick callback.
/// </summary>
function TableEditorToolSaveButton::onClick(%this)
{
   TableEditorGUI.doSave();
}

/// <summary>
/// Launch help page.
/// </summary>
function TableEditorHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/blackjack/table/");
}

// -----------------------------------------------------------------------------
// AssetLibrary buttons
//------------------------------------------------------------------------------

/// <summary>
/// AssetLibrary callback.
/// </summary>
function TableEditorTableImageEditText::setSelectedAsset(%this, %asset)
{
   %this.text = %asset;
   // todo (2012/03/20): use actual frame when available
   TableEditorTableImageFrameEditBox.setValue(0);  
    
   %this.refreshDisplay();
}

/// <summary>
/// Refresh the display.
/// </summary>
function TableEditorTableImageEditText::refreshDisplay(%this)
{
   TableEditorTableImagePreview.display(%this.getText(), "t2dStaticSprite");
   TableEditorTableImagePreview.sprite.setFrame(TableEditorTableImageFrameEditBox.getValue());

   // Toggle frame select visibility based on number of frames.
   TableEditorTableImageFrameContainer.Visible = (%this.getText().getFrameCount() > 1);
}

/// <summary>
/// onClick callback.
/// </summary>
function TableEditorTableImageButton::onClick(%this)
{
   AssetLibrary.open(TableEditorTableImageEditText, $SpriteSheetPage, "Table");
}

/// <summary>
/// Validate frame number.
/// </summary>
function TableEditorTableImageFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (TableEditorTableImageEditText.getText().getFrameCount() - 1))
      %this.setValue(TableEditorTableImageEditText.getText().getFrameCount() - 1);
      
   TableEditorTableImagePreview.sprite.setFrame(%this.getValue());
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function TableEditorTableImageFrameSpinLeft::onClick(%this)
{
   TableEditorTableImageFrameEditBox.setValue(TableEditorTableImageFrameEditBox.getValue() - 1);
   TableEditorTableImageFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function TableEditorTableImageFrameSpinRight::onClick(%this)
{
   TableEditorTableImageFrameEditBox.setValue(TableEditorTableImageFrameEditBox.getValue() + 1);
   TableEditorTableImageFrameEditBox.onValidate();
}

//-----

/// <summary>
/// AssetLibrary callback.
/// </summary>
function TableEditorRulesImageEditText::setSelectedAsset(%this, %asset)
{
   %this.text = %asset;
   // todo (2012/03/20): use actual frame when available
   TableEditorRulesImageFrameEditBox.setValue(0);  
    
   %this.refreshDisplay();
}

/// <summary>
/// Refresh the display.
/// </summary>
function TableEditorRulesImageEditText::refreshDisplay(%this)
{
   TableEditorRulesImagePreview.display(%this.getText(), "t2dStaticSprite");
   TableEditorRulesImagePreview.sprite.setFrame(TableEditorRulesImageFrameEditBox.getValue());

   // Toggle frame select visibility based on number of frames.
   TableEditorRulesImageFrameContainer.Visible = (%this.getText().getFrameCount() > 1);
}

/// <summary>
/// onClick callback.
/// </summary>
function TableEditorRulesImageButton::onClick(%this)
{
   AssetLibrary.open(TableEditorRulesImageEditText, $SpriteSheetPage, "Table");
}

/// <summary>
/// Validate frame number.
/// </summary>
function TableEditorRulesImageFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (TableEditorRulesImageEditText.getText().getFrameCount() - 1))
      %this.setValue(TableEditorRulesImageEditText.getText().getFrameCount() - 1);
      
   TableEditorRulesImagePreview.sprite.setFrame(%this.getValue());
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function TableEditorRulesImageFrameSpinLeft::onClick(%this)
{
   TableEditorRulesImageFrameEditBox.setValue(TableEditorRulesImageFrameEditBox.getValue() - 1);
   TableEditorRulesImageFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function TableEditorRulesImageFrameSpinRight::onClick(%this)
{
   TableEditorRulesImageFrameEditBox.setValue(TableEditorRulesImageFrameEditBox.getValue() + 1);
   TableEditorRulesImageFrameEditBox.onValidate();
}

//-----

/// <summary>
/// AssetLibrary callback.
/// </summary>
function TableEditorBetCircleImageEditText::setSelectedAsset(%this, %asset)
{
   %this.text = %asset;
   // todo (2012/03/20): use actual frame when available
   TableEditorBetCircleImageFrameEditBox.setValue(0);  
    
   %this.refreshDisplay();
}

/// <summary>
/// Refresh the display.
/// </summary>
function TableEditorBetCircleImageEditText::refreshDisplay(%this)
{
   TableEditorBetCircleImagePreview.display(%this.getText(), "t2dStaticSprite");
   TableEditorBetCircleImagePreview.sprite.setFrame(TableEditorBetCircleImageFrameEditBox.getValue());

   // Toggle frame select visibility based on number of frames.
   TableEditorBetCircleImageFrameContainer.Visible = (%this.getText().getFrameCount() > 1);
}

/// <summary>
/// onClick callback.
/// </summary>
function TableEditorBetCircleImageButton::onClick(%this)
{
   AssetLibrary.open(TableEditorBetCircleImageEditText, $SpriteSheetPage, "Table");
}

/// <summary>
/// Validate frame number.
/// </summary>
function TableEditorBetCircleImageFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (TableEditorBetCircleImageEditText.getText().getFrameCount() - 1))
      %this.setValue(TableEditorBetCircleImageEditText.getText().getFrameCount() - 1);
      
   TableEditorBetCircleImagePreview.sprite.setFrame(%this.getValue());
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function TableEditorBetCircleImageFrameSpinLeft::onClick(%this)
{
   TableEditorBetCircleImageFrameEditBox.setValue(TableEditorBetCircleImageFrameEditBox.getValue() - 1);
   TableEditorBetCircleImageFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function TableEditorBetCircleImageFrameSpinRight::onClick(%this)
{
   TableEditorBetCircleImageFrameEditBox.setValue(TableEditorBetCircleImageFrameEditBox.getValue() + 1);
   TableEditorBetCircleImageFrameEditBox.onValidate();
}

//-----

/// <summary>
/// AssetLibrary callback.
/// </summary>
function TableEditorCardBoxImageEditText::setSelectedAsset(%this, %asset)
{
   %this.text = %asset;
   // todo (2012/03/20): use actual frame when available
   TableEditorCardBoxImageFrameEditBox.setValue(0);  
    
   %this.refreshDisplay();
}

/// <summary>
/// Refresh the display.
/// </summary>
function TableEditorCardBoxImageEditText::refreshDisplay(%this)
{
   TableEditorCardBoxImagePreview.display(%this.getText(), "t2dStaticSprite");
   TableEditorCardBoxImagePreview.sprite.setFrame(TableEditorCardBoxImageFrameEditBox.getValue());

   // Toggle frame select visibility based on number of frames.
   TableEditorCardBoxImageFrameContainer.Visible = (%this.getText().getFrameCount() > 1);
}

/// <summary>
/// onClick callback. 
/// </summary>
function TableEditorCardBoxImageButton::onClick(%this)
{
   AssetLibrary.open(TableEditorCardBoxImageEditText, $SpriteSheetPage, "Table");
}

/// <summary>
/// Validate frame number.
/// </summary>
function TableEditorCardBoxImageFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (TableEditorCardBoxImageEditText.getText().getFrameCount() - 1))
      %this.setValue(TableEditorCardBoxImageEditText.getText().getFrameCount() - 1);
      
   TableEditorCardBoxImagePreview.sprite.setFrame(%this.getValue());
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function TableEditorCardBoxImageFrameSpinLeft::onClick(%this)
{
   TableEditorCardBoxImageFrameEditBox.setValue(TableEditorCardBoxImageFrameEditBox.getValue() - 1);
   TableEditorCardBoxImageFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function TableEditorCardBoxImageFrameSpinRight::onClick(%this)
{
   TableEditorCardBoxImageFrameEditBox.setValue(TableEditorCardBoxImageFrameEditBox.getValue() + 1);
   TableEditorCardBoxImageFrameEditBox.onValidate();
}

//-----

/// <summary>
/// AssetLibrary callback.
/// </summary>
function TableEditorDealerChipRackImageEditText::setSelectedAsset(%this, %asset)
{
   %this.text = %asset;
   // todo (2012/03/20): use actual frame when available
   TableEditorDealerChipRackImageFrameEditBox.setValue(0);  
    
   %this.refreshDisplay();
}

/// <summary>
/// Refresh the display.
/// </summary>
function TableEditorDealerChipRackImageEditText::refreshDisplay(%this)
{
   TableEditorDealerChipRackImagePreview.display(%this.getText(), "t2dStaticSprite");
   TableEditorDealerChipRackImagePreview.sprite.setFrame(TableEditorDealerChipRackImageFrameEditBox.getValue());

   // Toggle frame select visibility based on number of frames.
   TableEditorDealerChipRackImageFrameContainer.Visible = (%this.getText().getFrameCount() > 1);
}

/// <summary>
/// onClick callback.
/// </summary>
function TableEditorDealerChipRackImageButton::onClick(%this)
{
   AssetLibrary.open(TableEditorDealerChipRackImageEditText, $SpriteSheetPage, "Table");
}

/// <summary>
/// Validate frame number.
/// </summary>
function TableEditorDealerChipRackImageFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (TableEditorDealerChipRackImageEditText.getText().getFrameCount() - 1))
      %this.setValue(TableEditorDealerChipRackImageEditText.getText().getFrameCount() - 1);
      
   TableEditorDealerChipRackImagePreview.sprite.setFrame(%this.getValue());
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function TableEditorDealerChipRackImageFrameSpinLeft::onClick(%this)
{
   TableEditorDealerChipRackImageFrameEditBox.setValue(TableEditorDealerChipRackImageFrameEditBox.getValue() - 1);
   TableEditorDealerChipRackImageFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function TableEditorDealerChipRackImageFrameSpinRight::onClick(%this)
{
   TableEditorDealerChipRackImageFrameEditBox.setValue(TableEditorDealerChipRackImageFrameEditBox.getValue() + 1);
   TableEditorDealerChipRackImageFrameEditBox.onValidate();
}

// -----------------------------------------------------------------------------
// Save/Cancel buttons
//------------------------------------------------------------------------------

/// <summary>
/// Called when the cancel button is pressed.
/// </summary>
function TableEditorGUI::cancelButtonPressed(%this)
{
   // final check for dirty
   %this.validateAll();
   if ($CurrentGuiSerialString !$= %this.serialize())
       %this.SetDirtyValue(true);    

   if (%this.IsDirty())
   {
      ConfirmDialog.setupAndShow(TableEditorWindow.getGlobalCenter(), "Save Changes?", 
            "Save", "TableEditorGUI.saveButtonPressed();", 
            "Don't Save", "Canvas.popDialog(TableEditorGUI);", 
            "Cancel", "");
   }
   else
   {
      Canvas.popDialog(%this);
   }
}

/// <summary>
/// Preform the action of saving.
/// </summary>
/// <return>Returns true if the save was successful, false otherwise</return>
function TableEditorGUI::doSave(%this)
{
    // Check if the table name is valid 
    %tableName = TableEditorTableNameEditText.getText();
    if (!%this.checkValidLevelFileName(%tableName))
        return false;
    
   // Make sure all text fields are validated
   %this.validateAll();   
   
   // Set Table Image
   if (isObject(TableEditorTableImageEditText.getText()))
   {
      %this.currentTableObject.setImageMap(TableEditorTableImageEditText.getText());
      %this.currentTableObject.setFrame(TableEditorTableImageFrameEditBox.getValue());
   }
   
   // Set Table Rules Image
   if (isObject(TableRulesImage) && isObject(TableEditorRulesImageEditText.getText()))
   {
      TableRulesImage.setImageMap(TableEditorRulesImageEditText.getText());
      TableRulesImage.setFrame(TableEditorRulesImageFrameEditBox.getValue());
   }
   
   // Set Betting Circle Image
   %bettingCircleImageMap = TableEditorBetCircleImageEditText.getText();
   %bettingCircleImageFrame = TableEditorBetCircleImageFrameEditBox.getValue();
   %seatBehaviorList = %this.getBehaviorList("TableSeatBehavior");
   for (%i = 0; %i < getWordCount(%seatBehaviorList); %i++)
   {
      %seat = getWord(%seatBehaviorList, %i);
      %mainBetArea = %seat.mainBetArea;
      %mainBetArea.setImageMap(%bettingCircleImageMap);
      %mainBetArea.setFrame(%bettingCircleImageFrame);
   }
   
   // Set Card Box Image
   %cardBoxImageMap = TableEditorCardBoxImageEditText.getText();
   %cardBoxImageFrame = TableEditorCardBoxImageFrameEditBox.getValue();
   %seatBehaviorList = %this.getBehaviorList("TableSeatBehavior");
   for (%i = 0; %i < getWordCount(%seatBehaviorList); %i++)
   {
      %seatObject = getWord(%seatBehaviorList, %i).owner;
      %seatObject.setImageMap(%cardBoxImageMap);
      %seatObject.setFrame(%cardBoxImageFrame);
   }
   
   // Set Dealer Chip Rack   
   if (isObject(DealerChipRack))
   {
      DealerChipRack.setImageMap(TableEditorDealerChipRackImageEditText.getText());
      DealerChipRack.setFrame(TableEditorDealerChipRackImageFrameEditBox.getValue());
   }
   
   // Set Number of Seats
   %this.setSeatsVisible(TableEditorNumSeatsPopup.getText());
   
   // Set Table Music
   %backgroundMusicBehavior = $CurrentTable.owner.getBehavior("BackgroundMusicBehavior");
   if (isObject(%backgroundMusicBehavior))
   {
      %backgroundMusicBehavior.songProfile = getAudioProfileFromSoundContainer(TableEditorTableMusicContainer); 
   }
   
   // Set Standard Payout
   %numerator = TableEditorStandardPayoutEdit1.getText();
   %denominator = TableEditorStandardPayoutEdit2.getText();
   $CurrentBetRules.setWinPayout(%numerator, %denominator);
   
   // Set Blackjack Payout
   %numerator = TableEditorBlackjackPayoutEdit1.getText();
   %denominator = TableEditorBlackjackPayoutEdit2.getText();
   $CurrentBetRules.setBlackjackPayout(%numerator, %denominator);
   
   // Set Minimum Bet
   $CurrentBetRules.minBet = TableEditorMinBetEdit.getText();
   
   // Set Maximum Bet
   $CurrentBetRules.maxBet = TableEditorMaxBetEdit.getText(); 
   
   // Set Hand Count Visibility
   $CurrentTable.showHandValues = TableEditorHandCountCheckBox.getValue();
   
   // Set Split any Pair
   $CurrentTable.canSplit = TableEditorSplitAnyPairCheckBox.getValue();
   
   // Set Split Tens
   $CurrentTable.canSplitTens = TableEditorSplit10CheckBox.getValue();
   
   // Set Double Down
   $CurrentTable.canDoubleDown = TableEditorDoubleDownCheckBox.getValue();
   
   // Set Double after Split
   $CurrentTable.canDoubleAfterSplit = TableEditorDoubleOnSplitCheckBox.getValue();
   
  
   //------------------------------
   // Shoe
   //------------------------------
   ShoeBodyImageEditBox.onSave();
   ShoeDiscardImageEditBox.onSave();
   ShoePenetrationImageEditBox.onSave();
   ShoeShuffleTimeEditBox.onSave();
   ShoeDeckImageEditBox.onSave();
   ShoeNumberOfDecksSlider.onSave();
   ShoePenetrationPercentSlider.onSave();
   
   //------------------------------
   // Currency
   //------------------------------
   for (%i = 0; %i < $CurrencyEditorMaximumElements; %i++)
   { 
      %currencyElement = CurrencyEditorCurrencyArray.getObject(%i);     
      
      if (isObject(CurrencyEditorGUI.bankStack[%i]))
      {
         
         // Set visibility
         %isEnabledCheckBox = %currencyElement.findObjectByInternalName("IsEnabledBox", true);
         CurrencyEditorGUI.bankStack[%i].setEnabled(%isEnabledCheckBox.getValue());
         CurrencyEditorGUI.bankStack[%i].setVisible(%isEnabledCheckBox.getValue());
         
         // Set imageMap
         %imageMapName = %currencyElement.findObjectByInternalName("ImagePathTextEdit", true).getText();
         CurrencyEditorGUI.bankStack[%i].setImageMap(%imageMapName);

         %imageFrameValue = %currencyElement.findObjectByInternalName("ImageFrameTextEdit", true).getValue();
         CurrencyEditorGUI.bankStack[%i].setFrame(%imageFrameValue);
         
         // Set value
         %valueText = %currencyElement.findObjectByInternalName("ValueTextEdit", true);
         %behavior = CurrencyEditorGUI.bankStack[%i].getBehavior("BankStackBehavior");
         %behavior.denomination = %valueText.getText();
      }
   }
   
   //------------------------------
   // AI Player List
   //------------------------------
   TableEditorPlayerList::save();
   
   // Rename the level file
   %tableName = TableEditorTableNameEditText.getText();
   
   if (%tableName !$= fileBase(LBProjectObj.currentLevelFile))
   {
        %this.updateLevelSelect(%tableName);
        renameCurrentLevelFile(%tableName);
   }
   else
   {
        LBProjectObj.saveLevel(); 
        SaveAllLevelDatablocks();  
   }
   
   $CurrentGuiSerialString = %this.serialize();
   %this.SetDirtyValue(false);
   
   return true;
}

function TableEditorGUI::updateLevelSelect(%this, %fileName)
{
    if ($Save::GeneralSettings::Table1Map $= fileBase(LBProjectObj.currentLevelFile))
    {
        $Save::GeneralSettings::Table1Map = %fileName;
    }
 
    if ($Save::GeneralSettings::Table2Map $= fileBase(LBProjectObj.currentLevelFile))
    {
        $Save::GeneralSettings::Table2Map = %fileName;
    }
 
    if ($Save::GeneralSettings::Table3Map $= fileBase(LBProjectObj.currentLevelFile))
    {
        $Save::GeneralSettings::Table3Map = %fileName;
    }
 
    if ($Save::GeneralSettings::Table4Map $= fileBase(LBProjectObj.currentLevelFile))
    {
        $Save::GeneralSettings::Table4Map = %fileName;
    }
 
    if ($Save::GeneralSettings::Table5Map $= fileBase(LBProjectObj.currentLevelFile))
    {
        $Save::GeneralSettings::Table5Map = %fileName;
    }
   
    export("$Save::GeneralSettings::*","^project/data/files/export_generalSettings.cs", true, false); 
}

function TableEditorGUI::checkValidLevelFileName(%this, %fileName)
{
    // Check for empty filename 
    if (strreplace(%fileName, " ", "") $= "")  
    {
        // Show message dialog
        MessageDialog.setupAndShow(TableEditorWindow.getGlobalCenter(), "Table name cannot be empty!", 
         "", "", "", "", "Okay", "");         

        
        return false;   
    }

    // Check for spaces
    if (strreplace(%fileName, " ", "") !$= %fileName)
    {
        // Show message dialog
        MessageDialog.setupAndShow(TableEditorWindow.getGlobalCenter(), "Table name cannot contain spaces!", 
         "", "", "", "", "Okay", "");         
        
        return false;
    }
    
    // Check for invalid characters
    %invalidCharacters = "-+*/%$&§=()[].?\"#,;!~<>|°^{}@";
    %strippedName = stripChars(%fileName, %invalidCharacters);
    if (%strippedName !$= %fileName)
    {
        // Show message dialog
        MessageDialog.setupAndShow(TableEditorWindow.getGlobalCenter(), "Table name contains invalid symbols!", 
         "", "", "", "", "Okay", ""); 
         
        return false;   
    }
    
    // Check if file already exists
    %currentFileName = strreplace(fileName(LBProjectObj.currentLevelFile), ".t2d","");
    if (isFile($LevelSaveDirectory @ %fileName @ ".t2d") && %fileName !$= %currentFileName)
    {
        // Show message dialog
        MessageDialog.setupAndShow(TableEditorWindow.getGlobalCenter(), "Table name already exists!", 
         "", "", "", "", "Okay", "");
         
        return false;
    }
    
    return true;
}

/// <summary>
/// Called when the save button is pressed.
/// </summary>
function TableEditorGUI::saveButtonPressed(%this)
{   
    %success = TableEditorGUI.doSave();
   
   if (%success)
    Canvas.popDialog(%this);
}

/// <summary>
/// Gets a space-separated list of all behaviors in the scene with the given behavior name.
/// </summary>
/// <param name="behaviorName">The behavior name.</param>
/// <return>A space-separated string of behavior objects.</return>
function TableEditorGUI::getBehaviorList(%this, %behaviorName)
{
   %scene = ToolManager.getLastWindow().getScene();
   
   %objectList = "";
   
   for (%i = 0; %i < %scene.getSceneObjectCount(); %i++)
   {
      %sceneObject = %scene.getSceneObject(%i);
      %behaviorHandle = %sceneObject.getBehavior(%behaviorName);
      
      if (isObject(%behaviorHandle))
      {
         if (%objectList $= "")
            %objectList = %behaviorHandle;
         else
            %objectList = %objectList SPC %behaviorHandle;
      }
   }  
   
   return %objectList;
}

/// <summary>
/// Sets the visibility of seats in the scene based on the number of
/// seats set for the table.
/// </summary>
/// <param name="visibleSeatCount">The number of visible seats.</param>
function TableEditorGUI::setSeatsVisible(%this, %visibleSeatCount)
{
   // Find the middle value(s) of the TableSeatCount
   %right = mCeil(($TableSeatCount-1)/2);
   %left = mFloor(($TableSeatCount-1)/2);
  
   %count = %visibleSeatCount;

   // Clear all seats
   for (%i = 0; %i < $TableSeatCount; %i++)
   {
      $TableSeatArray[%i].setSeatVisibility(false);
   }

   // Set seats visible according to visibleSeatCount
   while (%count > 0)
   {
      if (%left == %right)
      {
         if (%count % 2 != 0)
         {
            $TableSeatArray[%right].setSeatVisibility(true);
            %count--;
         }
         %right++;
         %left--;
      }
      else
      {
         if (%count % 2 != 0)
         {
            $TableSeatArray[%right].setSeatVisibility(true);
            %count--;
            %right++;
         }
         else
         {
            $TableSeatArray[%right].setSeatVisibility(true);
            $TableSeatArray[%left].setSeatVisibility(true);
            %count -= 2;
            %right++;
            %left--;
         }
      }
   }
}

/// <summary>
/// AssetLibrary callback.
/// </summary>
function TableEditorTableMusicTextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}

/// <summary>
/// onAction callback.
/// </summary>
function TableEditorSplitAnyPairCheckBox::onAction(%this)
{
   TableEditorSplitTensContainer.setVisible(%this.getValue());
}

/// <summary>
/// onAction callback.
/// </summary>
function TableEditorDoubleDownCheckBox::onAction(%this)
{
   TableEditorDoubleOnSplitContainer.setVisible(%this.getValue());
}

/// <summary>
/// Validates all text fields in the table editor.
/// </summary>
function TableEditorGUI::validateAll(%this)
{
   // Shoe
   ShoeShuffleTimeEditBox.onValidate();   
   
   // Currency
   CurrencyEditorGUI.validateAllValueTextFields();   
   
   // Rules and Payouts
   TableEditorStandardPayoutEdit1.onValidate();
   TableEditorStandardPayoutEdit2.onValidate();
   TableEditorBlackjackPayoutEdit1.onValidate();
   TableEditorBlackjackPayoutEdit2.onValidate();
   TableEditorMinBetEdit.onValidate();
   TableEditorMaxBetEdit.onValidate();
}

/// <summary>
/// Validate the numeric value.
/// </summary>
function TableEditorStandardPayoutEdit1::onValidate(%this)
{
   %min = TableEditorStandardPayoutEdit2.getText();
   %max = 1000;
   
   if (%min < 1)
      %min = 1;
      
   ValidateTextEditInteger(%this, %min, %max);
   
   %this.validatedText = %this.getText();
}

/// <summary>
/// Handle click on the spin up button.
/// </summary>
function TableEditorStandardPayout1SpinUp::onClick(%this)
{
   TableEditorStandardPayoutEdit1.setValue(TableEditorStandardPayoutEdit1.getValue() + 1);
   TableEditorStandardPayoutEdit1.onValidate();
}

/// <summary>
/// Handle click on the spin down button.
/// </summary>
function TableEditorStandardPayout1SpinDown::onClick(%this)
{
   TableEditorStandardPayoutEdit1.setValue(TableEditorStandardPayoutEdit1.getValue() - 1);
   TableEditorStandardPayoutEdit1.onValidate();
}

/// <summary>
/// Validate the numeric value.
/// </summary>
function TableEditorStandardPayoutEdit2::onValidate(%this)
{
   %min = 1;
   %max = TableEditorStandardPayoutEdit1.getText();
   
   if (%max < 1)
      %max = 1; 
      
   ValidateTextEditInteger(%this, %min, %max);
   
   %this.validatedText = %this.getText();
}

/// <summary>
/// Handle click on the spin up button.
/// </summary>
function TableEditorStandardPayout2SpinUp::onClick(%this)
{
   TableEditorStandardPayoutEdit2.setValue(TableEditorStandardPayoutEdit2.getValue() + 1);
   TableEditorStandardPayoutEdit2.onValidate();
}

/// <summary>
/// Handle click on the spin down button.
/// </summary>
function TableEditorStandardPayout2SpinDown::onClick(%this)
{
   TableEditorStandardPayoutEdit2.setValue(TableEditorStandardPayoutEdit2.getValue() - 1);
   TableEditorStandardPayoutEdit2.onValidate();
}

/// <summary>
/// Validate the numeric value.
/// </summary>
function TableEditorBlackjackPayoutEdit1::onValidate(%this)
{
   %min = TableEditorBlackjackPayoutEdit2.getText();
   %max = 1000;
   
   if (%min < 1)
      %min = 1;
      
   ValidateTextEditInteger(%this, %min, %max);
   
   %this.validatedText = %this.getText();
}

/// <summary>
/// Handle click on the spin up button.
/// </summary>
function TableEditorBlackjackPayout1SpinUp::onClick(%this)
{
   TableEditorBlackjackPayoutEdit1.setValue(TableEditorBlackjackPayoutEdit1.getValue() + 1);
   TableEditorBlackjackPayoutEdit1.onValidate();
}

/// <summary>
/// Handle click on the spin down button.
/// </summary>
function TableEditorBlackjackPayout1SpinDown::onClick(%this)
{
   TableEditorBlackjackPayoutEdit1.setValue(TableEditorBlackjackPayoutEdit1.getValue() - 1);
   TableEditorBlackjackPayoutEdit1.onValidate();
}

/// <summary>
/// Validate the numeric value.
/// </summary>
function TableEditorBlackjackPayoutEdit2::onValidate(%this)
{
   %min = 1;
   %max = TableEditorBlackjackPayoutEdit1.getText();

   if (%max < 1)
      %max = 1;   
   
   ValidateTextEditInteger(%this, %min, %max);
   
   %this.validatedText = %this.getText();
}

/// <summary>
/// Handle click on the spin up button.
/// </summary>
function TableEditorBlackjackPayout2SpinUp::onClick(%this)
{
   TableEditorBlackjackPayoutEdit2.setValue(TableEditorBlackjackPayoutEdit2.getValue() + 1);
   TableEditorBlackjackPayoutEdit2.onValidate();
}

/// <summary>
/// Handle click on the spin down button.
/// </summary>
function TableEditorBlackjackPayout2SpinDown::onClick(%this)
{
   TableEditorBlackjackPayoutEdit2.setValue(TableEditorBlackjackPayoutEdit2.getValue() - 1);
   TableEditorBlackjackPayoutEdit2.onValidate();
}

/// <summary>
/// Validate the numeric value.
/// </summary>
function TableEditorMinBetEdit::onValidate(%this)
{
   %min = 1;
   %max = TableEditorMaxBetEdit.getText();

   if (%max < 1)
      %max = 1;   
   
   ValidateTextEditInteger(%this, %min, %max);
   
   %this.validatedText = %this.getText();
}

/// <summary>
/// Handle click on the spin up button.
/// </summary>
function TableEditorMinBetSpinUp::onClick(%this)
{
   TableEditorMinBetEdit.setValue(TableEditorMinBetEdit.getValue() + 1);
   TableEditorMinBetEdit.onValidate();
}

/// <summary>
/// Handle click on the spin down button.
/// </summary>
function TableEditorMinBetSpinDown::onClick(%this)
{
   TableEditorMinBetEdit.setValue(TableEditorMinBetEdit.getValue() - 1);
   TableEditorMinBetEdit.onValidate();
}

/// <summary>
/// Validate the numeric value.
/// </summary>
function TableEditorMaxBetEdit::onValidate(%this)
{
   %min = TableEditorMinBetEdit.getText();
   %max = 10000000;

   if (%min < 1)
      %min = 1;   
   
   ValidateTextEditInteger(%this, %min, %max);
   
   %this.validatedText = %this.getText();
}

/// <summary>
/// Handle click on the spin up button.
/// </summary>
function TableEditorMaxBetSpinUp::onClick(%this)
{
   TableEditorMaxBetEdit.setValue(TableEditorMaxBetEdit.getValue() + 1);
   TableEditorMaxBetEdit.onValidate();
}

/// <summary>
/// Handle click on the spin down button.
/// </summary>
function TableEditorMaxBetSpinDown::onClick(%this)
{
   TableEditorMaxBetEdit.setValue(TableEditorMaxBetEdit.getValue() - 1);
   TableEditorMaxBetEdit.onValidate();
}


//------------------------------------------------------------------------------
// Shoe
//------------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// ShoeBodyImageEditBox
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function ShoeBodyImageEditBox::refresh(%this)
{
   if (isObject($ShoeEditorCurrentObject))
   {
      %this.text = $ShoeEditorCurrentObject.getImageMap();
      ShoeBodyImageFrameEditBox.setValue($ShoeEditorCurrentObject.getFrame());
   }
   else
   {
      ShoeBodyImageBitmap.display("");
      ShoeBodyImageFrameEditBox.setValue(0);
   }
   
   %this.refreshDisplay();
}

/// <summary>
/// Refresh the display.
/// </summary>
function ShoeBodyImageEditBox::refreshDisplay(%this)
{
   ShoeBodyImageBitmap.display(%this.getText(), "t2dStaticSprite");
   ShoeBodyImageBitmap.sprite.setFrame(ShoeBodyImageFrameEditBox.getValue());

   // Toggle frame select visibility based on number of frames.
   ShoeBodyImageFrameContainer.Visible = (%this.getText().getFrameCount() > 1);
}

/// <summary>
/// Save value back to the object.
/// </summary>
function ShoeBodyImageEditBox::onSave(%this)
{
   if (isObject($ShoeEditorCurrentObject))
   {
      $ShoeEditorCurrentObject.setImageMap(%this.getText());
      $ShoeEditorCurrentObject.setFrame(ShoeBodyImageFrameEditBox.getValue());
   }
}

/// <summary>
/// AssectSelector callback.
/// </summary>
function ShoeBodyImageEditBox::setSelectedAsset(%this, %asset)
{
   %this.text = %asset;
   // todo (2012/03/20): use actual frame when available
   ShoeBodyImageFrameEditBox.setValue(0);  
   
   %this.refreshDisplay();
}

/// <summary>
/// Launch the AssetLibrary.
/// </summary>
function ShoeBodyImageButton::onClick(%this)
{
   AssetLibrary.open(ShoeBodyImageEditBox, $SpriteSheetPage, "Shoe");
}

/// <summary>
/// Validate frame number.
/// </summary>
function ShoeBodyImageFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (ShoeBodyImageEditBox.getText().getFrameCount() - 1))
      %this.setValue(ShoeBodyImageEditBox.getText().getFrameCount() - 1);
      
   ShoeBodyImageBitmap.sprite.setFrame(%this.getValue());
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function ShoeBodyImageFrameSpinLeft::onClick(%this)
{
   ShoeBodyImageFrameEditBox.setValue(ShoeBodyImageFrameEditBox.getValue() - 1);
   ShoeBodyImageFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function ShoeBodyImageFrameSpinRight::onClick(%this)
{
   ShoeBodyImageFrameEditBox.setValue(ShoeBodyImageFrameEditBox.getValue() + 1);
   ShoeBodyImageFrameEditBox.onValidate();
}

//-----------------------------------------------------------------------------
// ShoeShuffleTimeEditBox
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function ShoeShuffleTimeEditBox::refresh(%this)
{
   %this.text = (isObject($ShoeEditorCurrentObject)) ? 
                  $ShoeEditorCurrentObject.callOnBehaviors(getShuffleTime) :
                  "";
}

/// <summary>
/// Save the gui value back to the current object.
/// </summary>
function ShoeShuffleTimeEditBox::onSave(%this)
{
   if (isObject($ShoeEditorCurrentObject))
   {
      %this.onValidate();
      $ShoeEditorCurrentObject.callOnBehaviors(setShuffleTime, %this.getText());
   }
}

/// <summary>
/// Validate gui value.
/// </summary>
function ShoeShuffleTimeEditBox::onValidate(%this)
{
   ValidateTextEditInteger(%this, 0, 30);
   %this.validatedText = %this.getText();
}

/// <summary>
/// Handle click on the spin up button.
/// </summary>
function ShoeShuffleTimeSpinUp::onClick(%this)
{
   ShoeShuffleTimeEditBox.setValue(ShoeShuffleTimeEditBox.getValue() + 1);
   ShoeShuffleTimeEditBox.onValidate();
}

/// <summary>
/// Handle click on the spin down button.
/// </summary>
function ShoeShuffleTimeSpinDown::onClick(%this)
{
   ShoeShuffleTimeEditBox.setValue(ShoeShuffleTimeEditBox.getValue() - 1);
   ShoeShuffleTimeEditBox.onValidate();
}

//-----------------------------------------------------------------------------
// ShoePenetrationImageEditBox
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function ShoePenetrationImageEditBox::refresh(%this)
{
   if (isObject(penetrationCard))
   {
      %this.text = penetrationCard.getImageMap();
      ShoePenetrationImageFrameEditBox.setValue(penetrationCard.getFrame());
   }
   else
   {
      %this.text = "";
      ShoePenetrationImageFrameEditBox.setValue(0);
   }
   
   %this.refreshDisplay();
}

/// <summary>
/// Refresh the display.
/// </summary>
function ShoePenetrationImageEditBox::refreshDisplay(%this)
{
   ShoePenetrationImageBitmap.display(%this.getText(), "t2dStaticSprite");
   ShoePenetrationImageBitmap.sprite.setFrame(ShoePenetrationImageFrameEditBox.getValue());

   // Toggle frame select visibility based on number of frames.
   ShoePenetrationImageFrameContainer.Visible = (%this.getText().getFrameCount() > 1);
}

/// <summary>
/// Save the gui value back to the current object.
/// </summary>
function ShoePenetrationImageEditBox::onSave(%this)
{
   if (isObject(penetrationCard))
   {
      penetrationCard.setImageMap(%this.getText());
      penetrationCard.setFrame(ShoePenetrationImageFrameEditBox.getValue());
   }
}

/// <summary>
/// AssectSelector callback.
/// </summary>
function ShoePenetrationImageEditBox::setSelectedAsset(%this, %asset)
{
   %this.text = %asset;
   // todo (2012/03/20): use actual frame when available
   ShoePenetrationImageFrameEditBox.setValue(0);  
   
   %this.refreshDisplay();
}

/// <summary>
/// Launch the AssetLibrary.
/// </summary>
function ShoePenetrationImageButton::onClick(%this)
{
   AssetLibrary.open(ShoePenetrationImageEditBox, $SpriteSheetPage, "Shoe");
}

/// <summary>
/// Validate frame number.
/// </summary>
function ShoePenetrationImageFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (ShoePenetrationImageEditBox.getText().getFrameCount() - 1))
      %this.setValue(ShoePenetrationImageEditBox.getText().getFrameCount() - 1);
      
   ShoePenetrationImageBitmap.sprite.setFrame(%this.getValue());
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function ShoePenetrationImageFrameSpinLeft::onClick(%this)
{
   ShoePenetrationImageFrameEditBox.setValue(ShoePenetrationImageFrameEditBox.getValue() - 1);
   ShoePenetrationImageFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function ShoePenetrationImageFrameSpinRight::onClick(%this)
{
   ShoePenetrationImageFrameEditBox.setValue(ShoePenetrationImageFrameEditBox.getValue() + 1);
   ShoePenetrationImageFrameEditBox.onValidate();
}

//-----------------------------------------------------------------------------
// ShoeDiscardImageEditBox
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function ShoeDiscardImageEditBox::refresh(%this)
{
   if (isObject(DiscardArea))
   {
      %this.text = DiscardArea.getImageMap();
      ShoeDiscardImageFrameEditBox.setValue(DiscardArea.getFrame());
   }
   else
   {
      %this.text = "";
      ShoeDiscardImageFrameEditBox.setValue(0);
   }

    %this.refreshDisplay();
}
   
/// <summary>
/// Refresh the display.
/// </summary>
function ShoeDiscardImageEditBox::refreshDisplay(%this)
{
   ShoeDiscardImageBitmap.display(%this.getText(), "t2dStaticSprite");
   ShoeDiscardImageBitmap.sprite.setFrame(ShoeDiscardImageFrameEditBox.getValue());

   // Toggle frame select visibility based on number of frames.
   ShoeDiscardImageFrameContainer.Visible = (%this.getText().getFrameCount() > 1);
}

/// <summary>
/// Save the gui value back to the current object.
/// </summary>
function ShoeDiscardImageEditBox::onSave(%this)
{
   if (isObject(DiscardArea))
   {
      DiscardArea.setImageMap(%this.getText());
      DiscardArea.setFrame(ShoeDiscardImageFrameEditBox.getValue());
   }
}

/// <summary>
/// AssectSelector callback.
/// </summary>
function ShoeDiscardImageEditBox::setSelectedAsset(%this, %asset)
{
   %this.text = %asset;
   // todo (2012/03/20): use actual frame when available
   ShoeDiscardImageFrameEditBox.setValue(0);  

   %this.refreshDisplay();
}

/// <summary>
/// Launch the AssetLibrary.
/// </summary>
function ShoeDiscardImageButton::onClick(%this)
{
   AssetLibrary.open(ShoeDiscardImageEditBox, $SpriteSheetPage, "Shoe");
}

/// <summary>
/// Validate frame number.
/// </summary>
function ShoeDiscardImageFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (ShoeDiscardImageEditBox.getText().getFrameCount() - 1))
      %this.setValue(ShoeDiscardImageEditBox.getText().getFrameCount() - 1);
      
   ShoeDiscardImageBitmap.sprite.setFrame(%this.getValue());
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function ShoeDiscardImageFrameSpinLeft::onClick(%this)
{
   ShoeDiscardImageFrameEditBox.setValue(ShoeDiscardImageFrameEditBox.getValue() - 1);
   ShoeDiscardImageFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function ShoeDiscardImageFrameSpinRight::onClick(%this)
{
   ShoeDiscardImageFrameEditBox.setValue(ShoeDiscardImageFrameEditBox.getValue() + 1);
   ShoeDiscardImageFrameEditBox.onValidate();
}

//-----------------------------------------------------------------------------
// ShoeDeckImageEditBox
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function ShoeDeckImageEditBox::refresh(%this)
{
   if (isObject(tableCards))
   {
      %this.text = tableCards.getImageMap();
   }
   else
   {
      %this.text = "";
   }
   
   %this.setActive(false);
}

/// <summary>
/// Refresh the display.
/// </summary>
function ShoeDeckImageEditBox::refreshDisplay(%this)
{
   ShoeDeckImageBitmap.display(%this.getText(), "t2dStaticSprite");
   ShoeDeckImageBitmap.sprite.setFrame(ShoePenetrationImageFrameEditBox.getValue());

   // Toggle frame select visibility based on number of frames.
   ShoeDeckFrameContainer.Visible = (%this.getText().getFrameCount() > 1);
}

/// <summary>
/// Save the gui value back to the current object.
/// </summary>
function ShoeDeckImageEditBox::onSave(%this)
{
   if (isObject(tableCards))
      tableCards.setImageMap(%this.getText());
}

/// <summary>
/// AssectSelector callback.
/// </summary>
function ShoeDeckImageEditBox::setSelectedAsset(%this, %asset)
{
   %this.text = %asset;
}

/// <summary>
/// Launch the AssetLibrary.
/// </summary>
function ShoeDeckImageButton::onClick(%this)
{
   AssetLibrary.open(ShoeDeckImageEditBox, $BlackjackDecksPage);
}

//-----------------------------------------------------------------------------
// ShoeNumberOfDecksSlider
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function ShoeNumberOfDecksSlider::refresh(%this)
{
   %value = $ShoeEditorCurrentObject.callOnBehaviors(getDeckCount);
   %this.setValue(%value);
   ShoeNumberOfDecksTextEdit.setValue(%value);
}

/// <summary>
/// Refresh the Gui.
/// </summary>
function ShoeNumberOfDecksSlider::onMouseDragged(%this)
{
   ShoeNumberOfDecksTextEdit.setValue(%this.value);
}

/// <summary>
/// Save the gui value back to the current object.
/// </summary>
function ShoeNumberOfDecksSlider::onSave(%this)
{
   $ShoeEditorCurrentObject.callOnBehaviors(setDeckCount, %this.value);
}

//-----------------------------------------------------------------------------
// ShoeNumberOfDecksTextEdit
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function ShoeNumberOfDecksTextEdit::refresh(%this)
{
   %this.text = ShoeNumberOfDecksSlider.getValue();
}

/// <summary>
/// Save the gui value back to the current object.
/// </summary>
function ShoeNumberOfDecksTextEdit::onSave(%this)
{
   // Do nothing, slider handles saving.
}

/// <summary>
/// Validate gui value.
/// </summary>
function ShoeNumberOfDecksTextEdit::onValidate(%this)
{
   ValidateTextEditInteger(%this, 1, 16);
   %this.validatedText = %this.getText();
   
   ShoeNumberOfDecksSlider.setValue(%this.getValue());
}

//-----------------------------------------------------------------------------
// ShoePenetrationPercentSlider
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function ShoePenetrationPercentSlider::refresh(%this)
{
   %value = $ShoeEditorCurrentObject.callOnBehaviors(getShoePenetrationPercent);
   %this.setValue(%value);
   ShoePenetrationPercentTextEdit.setValue(%value);
}

/// <summary>
/// Refresh the Gui.
/// </summary>
function ShoePenetrationPercentSlider::onMouseDragged(%this)
{
   ShoePenetrationPercentTextEdit.setValue(%this.value);
}

/// <summary>
/// Save the gui value back to the current object.
/// </summary>
function ShoePenetrationPercentSlider::onSave(%this)
{
   $ShoeEditorCurrentObject.callOnBehaviors(setShoePenetrationPercent, %this.value);
}

//-----------------------------------------------------------------------------
// ShoePenetrationPercentTextEdit
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the Gui.
/// </summary>
function ShoePenetrationPercentTextEdit::refresh(%this)
{
   %this.text = ShoePenetrationPercentSlider.getValue();
}

/// <summary>
/// Save the gui value back to the current object.
/// </summary>
function ShoePenetrationPercentTextEdit::onSave(%this)
{
   // Do nothing, slider handles saving.
}

/// <summary>
/// Validate gui value.
/// </summary>
function ShoePenetrationPercentTextEdit::onValidate(%this)
{
   ValidateTextEditInteger(%this, 0, 100);
   %this.validatedText = %this.getText();
   
   ShoePenetrationPercentSlider.setValue(%this.getValue());
}

//------------------------------------------------------------------------------
// Player
//------------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// TableEditorPlayerList
//-----------------------------------------------------------------------------

/// <summary>
/// Refresh the list of AI
/// </summary>
function TableEditorPlayerList::refresh(%this)
{
   %index = 1;
   
   if(isObject($persistentObjectSet))
   {
      %count = $persistentObjectSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %object = $persistentObjectSet.getObject(%i);

         if (isObject(%object.getBehavior("BlackjackAiPlayerBehavior"))
             && (%object.callOnBehaviors(getIsAvailable)))
         {
            (TableEditorPlayerContainer@%index).Visible = true;
            (TableEditorPlayerPreviewBitmap@%index).display(%object.getImageMap(), "t2dStaticSprite");
            (TableEditorPlayerPreviewBitmap@%index).sprite.setFrame(%object.getFrame());            
            
            %button = (TableEditorPlayerButton@%index);
            
            %button.text = %object.getInternalName();
            %button.object = %object;
          
            // check is AI is at this table
            echo("******** "@$CurrentTable.tableIndex);
            %isAvailable = %object.callOnBehaviors(getIsAvailableOn, $CurrentTable.tableIndex);
            if (%isAvailable !$= "ERR_CALL_NOT_HANDLED" && %isAvailable)
            {
               %button.Profile = GuiBJActiveProfile;
            }
            else
            {
               %button.Profile = GuiBJInactiveProfile;               
            }
            
            %index++;
         }
      }
      
      while (%index <= 10)
      {
         (TableEditorPlayerContainer@%index).Visible = false;
         %index++;
      }
   }
}

/// <summary>
/// Sets the availability of ai players for the current level from the gui.
/// </summary>
function TableEditorPlayerList::save(%this)
{
   %index = 1;   
   
   if(isObject($persistentObjectSet))
   {
      %count = $persistentObjectSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %object = $persistentObjectSet.getObject(%i);

         if (isObject(%object.getBehavior("BlackjackAiPlayerBehavior"))
             && (%object.callOnBehaviors(getIsAvailable)))
         {            
            %button = (TableEditorPlayerButton@%index);
            %button.object = %object;

            if (%button.Profile $= GuiBJActiveProfile)
               %object.callOnBehaviors(setIsAvailableOn, $CurrentTable.tableIndex, true);
            else
               %object.callOnBehaviors(setIsAvailableOn, $CurrentTable.tableIndex, false);
               
            %index++;
         }
      }
   }
}

/// <summary>
/// Serializes the state of the GUI into a space-delimited string.
/// </summary>
/// <return>A space-delimited string.</return>
function TableEditorPlayerList::serialize(%this)
{
   %serialString = "";
   %index = 1; 
   
   if(isObject($persistentObjectSet))
   {
      %count = $persistentObjectSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %object = $persistentObjectSet.getObject(%i);

         if (isObject(%object.getBehavior("BlackjackAiPlayerBehavior"))
             && (%object.callOnBehaviors(getIsAvailable)))
         {            
            %button = (TableEditorPlayerButton@%index);
            %button.object = %object;

            %serialString = %button.Profile SPC %serialString;      

            %index++;
         }
      }
   }
   
   return %serialString;
}

//-----------------------------------------------------------------------------
// TableEditorPlayerButton
//-----------------------------------------------------------------------------

/// <summary>
/// Toggle 'isAvailable' value for the AI on the current table.
/// </summary>
function TableEditorPlayerButton::toggleAi()
{
   if ( $ThisControl.Profile $= GuiBJActiveProfile)
      $ThisControl.Profile = GuiBJInactiveProfile;
   else
      $ThisControl.Profile = GuiBJActiveProfile;
}

/// <summary>
/// Serializes the state of the GUI into a space-delimited string.
/// </summary>
/// <return>A space-delimited string.</return>
function TableEditorGUI::serialize(%this)
{
   %serialString = TableEditorTableNameEditText.getText();
    
   %serialString = %serialString SPC TableEditorTableImageEditText.getText() SPC TableEditorTableImageFrameEditBox.getValue();
   %serialString = %serialString SPC TableEditorRulesImageEditText.getText() SPC TableEditorRulesImageFrameEditBox.getValue();
   %serialString = %serialString SPC TableEditorBetCircleImageEditText.getText() SPC TableEditorBetCircleImageFrameEditBox.getValue();
   %serialString = %serialString SPC TableEditorCardBoxImageEditText.getText() SPC TableEditorCardBoxImageFrameEditBox.getValue();
   %serialString = %serialString SPC TableEditorDealerChipRackImageEditText.getText() SPC TableEditorDealerChipRackImageFrameEditBox.getValue();
   %serialString = %serialString SPC TableEditorNumSeatsPopup.getValue();
   
   %serialString = %serialString SPC getAudioProfileFromSoundContainer(TableEditorTableMusicContainer);

   //Ben: %serialString = %serialString SPC TableEditorStandardPayoutEdit1.getText();
   %serialString = %serialString SPC TableEditorStandardPayoutEdit1.validatedText;
   //Ben: %serialString = %serialString SPC TableEditorStandardPayoutEdit2.getText();
   %serialString = %serialString SPC TableEditorStandardPayoutEdit2.validatedText;
   //Ben: %serialString = %serialString SPC TableEditorBlackjackPayoutEdit1.getText();
   %serialString = %serialString SPC TableEditorBlackjackPayoutEdit1.validatedText;
   //Ben: %serialString = %serialString SPC TableEditorBlackjackPayoutEdit2.getText();
   %serialString = %serialString SPC TableEditorBlackjackPayoutEdit2.validatedText;
   //Ben: %serialString = %serialString SPC TableEditorMinBetEdit.getText();
   %serialString = %serialString SPC TableEditorMinBetEdit.validatedText;
   //Ben: %serialString = %serialString SPC TableEditorMaxBetEdit.getText();
   %serialString = %serialString SPC TableEditorMaxBetEdit.validatedText;
   
   %serialString = %serialString SPC TableEditorHandCountCheckBox.getValue();
   %serialString = %serialString SPC TableEditorSplitAnyPairCheckBox.getValue();
   %serialString = %serialString SPC TableEditorSplit10CheckBox.getValue();
   %serialString = %serialString SPC TableEditorDoubleDownCheckBox.getValue();
   %serialString = %serialString SPC TableEditorDoubleOnSplitCheckBox.getValue();
   
   // Shoe
   %serialString = %serialString SPC ShoeBodyImageEditBox.getText() SPC ShoeBodyImageFrameEditBox.getValue();
   %serialString = %serialString SPC ShoeDiscardImageEditBox.getText() SPC ShoeDiscardImageFrameEditBox.getValue();
   %serialString = %serialString SPC ShoePenetrationImageEditBox.getText() SPC ShoePenetrationImageFrameEditBox.getValue();

   %serialString = %serialString SPC ShoeShuffleTimeEditBox.validatedText;
   %serialString = %serialString SPC ShoeDeckImageEditBox.getText();
   
   %serialString = %serialString SPC ShoeNumberOfDecksSlider.getValue();
   %serialString = %serialString SPC ShoePenetrationPercentSlider.getValue();

   // Currency
   %serialString = %serialString SPC CurrencyEditorGUI.serialize();
   
   // AI Player List
   %serialString = %serialString SPC TableEditorPlayerList::serialize();
}
