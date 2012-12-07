//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$InterfaceEditorName = "Interface Tool";

$BitmapButtonStateOn = 0;
$BitmapButtonStateOver = 1;
$BitmapButtonStateDown = 2;
$BitmapButtonStateInactive = 3;

/// <summary>
/// Called when the control object is registered with the system after the control has been created. 
/// </summary>
function InterfaceEditorGui::onAdd(%this)
{
   // load up all of our gui's so we can access the relevant resources
   //$GameDir = LBProjectObj.gamePath;
   //exec($GameDir @ "/gui/tablePlay.gui");
   //exec($GameDir @ "/gui/tableSelect.gui");
   //exec($GameDir @ "/gui/helpScreen.gui");
   //exec($GameDir @ "/gui/credits.gui");
}

/// <summary>
/// Called when the control is woken up. 
/// </summary>
function InterfaceEditorGui::onWake(%this)
{
   if(!isObject(TablePlayGui))
      exec("^project/gui/tablePlay.gui");
      
   if(!isObject(TableSelectGui))
      exec("^project/gui/tableSelect.gui");
      
   if(!isObject(HelpScreenGui))
      exec("^project/gui/helpScreen.gui");
      
   if(!isObject(creditsGui))
      exec("^project/gui/credits.gui");      
         
   InterfaceEditorTabBook.selectPage(0);   
   
   %this.refresh();
   
   // Validate any active textEdits
    // none to validate in this gui
   
   // Serialized the starting state of the gui
   $CurrentGuiSerialString = %this.serialize();
   %this.SetDirtyValue(false);
   
    // Start the update loop for checking the dirty state
   %this.updateDirty();
}

/// <summary>
/// Called when the control is put to sleep. 
/// </summary>
function InterfaceEditorGui::onSleep(%this)
{
   if (isEventPending(%this.updateDirtyHandle))  
   {
      cancel(%this.updateDirtyHandle);
   }
}

/// <summary>
/// Update the dirty state of the GUI by comparing the current serialized state
/// with an initial state. Schedules the next update.
/// </summary>
function InterfaceEditorGui::updateDirty(%this)
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
function InterfaceEditorGui::SetDirtyValue(%this, %dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      InterfaceEditorWindow.setText($InterfaceEditorName SPC "*");
   else
      InterfaceEditorWindow.setText($InterfaceEditorName);
}

/// <return>true if this window is 'dirty', otherwise false.</return>
function InterfaceEditorGui::IsDirty(%this)
{
   return $Tools::TemplateToolDirty;
}

/// <summary>
/// Initilizes a popupMenu for a button with "up", "over", and "down"
/// items.
/// </summary>
/// <param name="popupMenu">The popupMenu to be initialized.</param>
function InterfaceEditorGui::initializeButtonPopup(%this, %popupMenu)
{
   %popupMenu.clear();   
   
   %popupMenu.add("Up", 0);
   %popupMenu.add("Over", 1);
   %popupMenu.add("Down", 2); 
   %popupMenu.add("Inactive", 3);  

   %popupMenu.setSelected(0);
}

/// <summary>
/// Get the bitmap corresponding to the current popup menu selection
/// </summary>
/// <param name="container">The gui container object.</param>
/// <param name="button">The gui button.</param>
/// <return>A bitmap.</return>
function InterfaceEditorGui::getButtonBitmap(%this, %container)
{
    %popupMenu = %container.findObjectByInternalName("PopupMenu", true);

    switch(%popupMenu.getSelected())
    {
    case $BitmapButtonStateOn:
        return %container.tempBitmapNormal;
        
    case $BitmapButtonStateOver:
        return %container.tempBitmapHilight;
    
    case $BitmapButtonStateDown:
        return %container.tempBitmapDepressed;
    
    case $BitmapButtonStateInactive:
        return %container.tempBitmapInactive;
        
    default:
        warn("Failed to get button bitmap.");
    }
}

/// <summary>
/// Set the bitmap corresponding to the current popup menu selection
/// </summary>
/// <param name="container">The gui container object.</param>
/// <param name="button">The gui button.</param>
/// <param name="bitmap">The bitmap being set for the button.</param>
/// <return>A bitmap.</return>
function InterfaceEditorGui::setButtonBitmap(%this, %container, %bitmap)
{
    %popupMenu = %container.findObjectByInternalName("PopupMenu", true);
    
    switch(%popupMenu.getSelected())
    {
    case $BitmapButtonStateOn:
        %container.tempBitmapNormal = %bitmap;
        
    case $BitmapButtonStateOver:
        %container.tempBitmapHilight = %bitmap;
    
    case $BitmapButtonStateDown:
        %container.tempBitmapDepressed = %bitmap;
    
    case $BitmapButtonStateInactive:
        %container.tempBitmapInactive = %bitmap;
        
    default:
        warn("Failed to set button bitmap.");
    }
}

/// <summary>
/// Initializes a gui container for a gui button.
/// </summary>
/// <param name="container">The gui container object that is initialized.</param>
/// <param name="button">The gui button to initializes the container from.</param>
function InterfaceEditorGui::initializeButtonContainer(%this, %container, %button)
{
   %imagePreview = %container.findObjectByInternalName("ImagePreview", true);
   %textEdit = %container.findObjectByInternalName("imageText", true);
   %popupMenu = %container.findObjectByInternalName("PopupMenu", true);
   
   // Save a reference to the container's associated button
   %container.linkedButton = %button;
   
    if (isObject(%popupMenu))
    {
        %this.initializeButtonPopup(%popupMenu);
        
        // Create temporary variables for storing button bitmaps
        %container.tempBitmapNormal = GetGuiImageFileRelativePath(%button.bitmapNormal);
        %container.tempBitmapHilight = GetGuiImageFileRelativePath(%button.bitmapHilight);
        %container.tempBitmapDepressed = GetGuiImageFileRelativePath(%button.bitmapDepressed);
        %container.tempBitmapInactive = GetGuiImageFileRelativePath(%button.bitmapInactive);
   
        %this.updateButtonContainer(%container, %button); 
        
        %textEdit.setActive(false);       
    }
}

/// <summary>
/// Updates a gui container for a gui button.
/// </summary>
/// <param name="container">The gui container object that is updated.</param>
function InterfaceEditorGui::updateButtonContainer(%this, %container)
{
    %imagePreview = %container.findObjectByInternalName("ImagePreview", true);
    %textEdit = %container.findObjectByInternalName("imageText", true);
    %popupMenu = %container.findObjectByInternalName("PopupMenu", true);
   
    if (isObject(%popupMenu))
    {
        %currentButtonBitmap = %this.getButtonBitmap(%container);
        %imagePreview.setBitmap(GetFullFileName(%currentButtonBitmap));
        %textEdit.setText(fileName(%currentButtonBitmap));
    }
}

/// <summary>
/// Saves a gui container for a gui button.
/// </summary>
/// <param name="container">The gui container object that is saved.</param>
/// <param name="button">The gui button the container state is saved to.</param>
function InterfaceEditorGui::saveButtonContainer(%this, %container, %button)
{
   %textEdit = %container.findObjectByInternalName("imageText", true);
   
    // Set button from temporay variables
    %button.bitmapNormal = %container.tempBitmapNormal;
    %button.bitmapHilight = %container.tempBitmapHilight;
    %button.bitmapDepressed = %container.tempBitmapDepressed;
    %button.bitmapInactive = %container.tempBitmapInactive;   
}

/// <summary>
/// Serializes a button container into a space-delimited string.
/// </summary>
/// <param name="container">The gui container.</param>
/// <return>A space-delimited string.</return>
function InterfaceEditorGui::serializeButtonContainer(%this, %container, %button)
{
   %serialString = %container.tempBitmapNormal;
   %serialString = %serialString SPC %container.tempBitmapHilight;
   %serialString = %serialString SPC %container.tempBitmapDepressed;
   %serialString = %serialString SPC %container.tempBitmapInactive;
   
   return %serialString;
}

/// <summary>
/// Initializes a gui container.
/// </summary>
function InterfaceEditorGui::initializeIconContainer(%this, %preview, %iconText, %frameText, %imageMap, %frame)
{
   %iconText.setText(%imageMap);
   %frameText.setValue(%frame);
   %preview.display(%imageMap, "t2dStaticSprite");
   %preview.sprite.setFrame(%frame);
}

/// <summary>
/// Saves a gui container with an imageText field.
/// </summary>
/// <param name="object">The object to save the container state to.</param>
function InterfaceEditorGui::saveIconContainer(%this, %iconText, %frameText, %object)
{
   if (isObject(%object))
   {
      %object.setImageMap(%iconText.getText());
      %object.setFrame(%frameText.getValue());
   }
}

/// <summary>
/// Serializes a container into a space-delimited string.
/// </summary>
/// <return>A space-delimited string.</return>
function InterfaceEditorGui::serializeIconContainer(%this, %iconText, %frameText)
{
   return %iconText.getText() SPC %frameText.getValue();
}

/// <summary>
/// Updates the GUI state from the game data.
/// </summary>
function InterfaceEditorGui::refresh(%this)
{
   //------------------------------
   // General Tab
   //------------------------------
   
   // Casino Carpet
   InterfaceEditorCasinoCarpetText.setText(CasinoCarpetBackground.getImageMap());
   InterfaceEditorCasinoCarpetFrameEditBox.setValue(CasinoCarpetBackground.getFrame());
   InterfaceEditorCasinoCarpetPreview.refresh();
   
   // Bank Container
   InterfaceEditorBankContainerText.setText(BankBackgroundObject.getImageMap());
   InterfaceEditorBankContainerFrameEditBox.setValue(BankBackgroundObject.getFrame());
   InterfaceEditorBankContainerPreview.refresh();
   
   // Player Avatar
   InterfaceEditorPlayerAvatarText.setText(userAi.getImageMap());
   InterfaceEditorPlayerAvatarFrameEditBox.setValue(userAi.getFrame());
   InterfaceEditorPlayerAvatarPreview.refresh();
   
   // Indication Arrow
   InterfaceEditorIndicationArrowText.setText(currentHandIcon.getImageMap());
   InterfaceEditorIndicationArrowFrameEditBox.setValue(currentHandIcon.getFrame());
   InterfaceEditorIndicationArrowPreview.refresh();
   
   // Npc Bank Image
   InterfaceEditorNpcBankImageText.setText(BottomBarAIBackgroundTemplate.getImageMap());
   InterfaceEditorNpcBankFrameEditBox.setValue(BottomBarAIBackgroundTemplate.getFrame());
   InterfaceEditorNpcBankImagePreview.refresh();

   
   //------------------------------
   // Buttons Tab
   //------------------------------

   %this.initializeButtonContainer(InterfaceEditorDealButtonContainer, TablePlayDealButton);   
   %this.initializeButtonContainer(InterfaceEditorRebetButtonContainer, TablePlayRebetButton);  
   %this.initializeButtonContainer(InterfaceEditorHitButtonContainer, TablePlayHitButton);     
   %this.initializeButtonContainer(InterfaceEditorSplitButtonContainer, TablePlaySplitButton);
   %this.initializeButtonContainer(InterfaceEditorStandButtonContainer, TablePlayStandButton);
   %this.initializeButtonContainer(InterfaceEditorDoubleButtonContainer, TablePlayDoubleButton);
   %this.initializeButtonContainer(InterfaceEditorHowtoButtonContainer, TableSelectHowtoButton);
   %this.initializeButtonContainer(InterfaceEditorLeaveButtonContainer, TablePlayCloseButton);
   %this.initializeButtonContainer(InterfaceEditorCreditsButtonContainer, TableSelectCreditsButton);
   %this.initializeButtonContainer(InterfaceEditorResetBankButtonContainer, TableSelectResetBankButton);
   
   
   //------------------------------
   // Icons Tab
   //------------------------------
   %this.initializeIconContainer(InterfaceEditorBlackjackIconPreview, InterfaceEditorBlackjackIconText, 
                                 InterfaceEditorBlackjackIconFrameEditBox, blackjackIcon.getImageMap(), blackjackIcon.getFrame()); 
   %this.initializeIconContainer(InterfaceEditorBustIconPreview, InterfaceEditorBustIconText, 
                                 InterfaceEditorBustIconFrameEditBox, bustIcon.getImageMap(), bustIcon.getFrame()); 
   %this.initializeIconContainer(InterfaceEditorWinIconPreview, InterfaceEditorWinIconText, 
                                 InterfaceEditorWinIconFrameEditBox, winIcon.getImageMap(), winIcon.getFrame()); 
   %this.initializeIconContainer(InterfaceEditorLoseIconPreview, InterfaceEditorLoseIconText, 
                                 InterfaceEditorLoseIconFrameEditBox, loseIcon.getImageMap(), loseIcon.getFrame()); 
   %this.initializeIconContainer(InterfaceEditorShufflingIconPreview, InterfaceEditorShufflingIconText, 
                                 InterfaceEditorShufflingIconFrameEditBox, shufflingIcon.getImageMap(), shufflingIcon.getFrame()); 
   %this.initializeIconContainer(InterfaceEditorPushIconPreview, InterfaceEditorPushIconText, 
                                 InterfaceEditorPushIconFrameEditBox, pushIcon.getImageMap(), pushIcon.getFrame()); 
   %this.initializeIconContainer(InterfaceEditorBankruptIconPreview, InterfaceEditorBankruptIconText, 
                                 InterfaceEditorBankruptIconFrameEditBox, BankruptIcon.getImageMap(), BankruptIcon.getFrame()); 
   InterfaceEditorBlackjackIconFrameEditBox.onValidate();
   InterfaceEditorBustIconFrameEditBox.onValidate();
   InterfaceEditorWinIconFrameEditBox.onValidate();
   InterfaceEditorLoseIconFrameEditBox.onValidate();
   InterfaceEditorShufflingIconFrameEditBox.onValidate();
   InterfaceEditorPushIconFrameEditBox.onValidate();
   InterfaceEditorBankruptIconFrameEditBox.onValidate();
   
   //------------------------------
   // How to Play Tab
   //------------------------------
   
   // Popup menu
   InterfaceEditorNumberOfHelpScreensPopup.clear();
   for (%i = 0; %i <= InterfaceEditorHelpScreenArray.getCount(); %i++)
   { 
      InterfaceEditorNumberOfHelpScreensPopup.add(%i, %i);
   }  
   InterfaceEditorNumberOfHelpScreensPopup.setSelected($Save::Interface::NumberOfHelpScreens);
   
   // Array
   for (%i = 0; %i < InterfaceEditorHelpScreenArray.getCount(); %i++)
   { 
      %container = InterfaceEditorHelpScreenArray.getObject(%i);
      %imagePreview = %container.findObjectByInternalName("ImagePreview", true);
      %textEdit = %container.findObjectByInternalName("ImageText", true);
      
      %imagePreview.bitmap = GetFullFileName($Save::Interface::HelpScreens[%i]);
      %textEdit.text = fileName($Save::Interface::HelpScreens[%i]);
      %textEdit.setActive(false);
   }

   // Buttons 
   %this.initializeButtonContainer(InterfaceEditorHowtoForwardButtonContainer, helpScreenNextButton);   
   %this.initializeButtonContainer(InterfaceEditorHowtoBackButtonContainer, helpScreenPreviousButton);
   %this.initializeButtonContainer(InterfaceEditorHowtoCloseButtonContainer, helpScreenCloseButton);

   //------------------------------
   // Credits Tab
   //------------------------------
   
   // Credits button on menu screen
   %buttonVisible = TableSelectCreditsButton.visible;
   InterfaceEditorCreditsButtonCheckBox.setStateOn(%buttonVisible);    
   
   // Background
   %imagePreview = InterfaceEditorCreditsBackgroundContainer.findObjectByInternalName("ImagePreview", true);
   %imageText = InterfaceEditorCreditsBackgroundContainer.findObjectByInternalName("imageText", true);
   %bitmap = CreditsBackground.bitmap;
   %imagePreview.setBitmap(GetFullFileName(%bitmap));
   %imageText.setText(filename(%bitmap));

   
   // Credits Image
   %imagePreview = InterfaceEditorCreditsImageContainer.findObjectByInternalName("ImagePreview", true);
   %imageText = InterfaceEditorCreditsImageContainer.findObjectByInternalName("imageText", true);
   %bitmap = CreditsImage.bitmap;
   %imagePreview.setBitmap(GetFullFileName(%bitmap));
   %imageText.setText(fileName(%bitmap));
   
   // Close Button
   %this.initializeButtonContainer(InterfaceEditorCreditsCloseButtonContainer, closeCreditsButton);
}

/// <summary>
/// Called when the save button is pressed.
/// </summary>
function InterfaceEditorGui::doSave(%this)
{
   //------------------------------
   // General Tab
   //------------------------------
   
   // Casino Carpet
   %imageMap = InterfaceEditorCasinoCarpetText.getText();
   CasinoCarpetBackground.setImageMap(%imageMap);   
   CasinoCarpetBackground.setFrame(InterfaceEditorCasinoCarpetFrameEditBox.getValue());
   
   // Bank Container
   %imageMap = InterfaceEditorBankContainerText.getText();
   BankBackgroundObject.setImageMap(%imageMap);
   BankBackgroundObject.setFrame(InterfaceEditorBankContainerFrameEditBox.getValue());
   
   // Player Avatar
   %imageMap = InterfaceEditorPlayerAvatarText.getText();
   %frame = InterfaceEditorPlayerAvatarFrameEditBox.getValue();
   userAi.setImageMap(%imageMap);  
   userAi.setFrame(%frame);
   BottomBarPlayerAvatarImage.setImageMap(%imageMap);
   BottomBarPlayerAvatarImage.setFrame(%frame);
   
   // Indication Arrow
   %imageMap = InterfaceEditorIndicationArrowText.getText();
   currentHandIcon.setImageMap(%imageMap); 
   currentHandIcon.setFrame(InterfaceEditorIndicationArrowFrameEditBox.getValue());
      
   // Npc Bank Image
   %imageMap = InterfaceEditorNpcBankImageText.getText();
   %frame = InterfaceEditorNpcBankFrameEditBox.getValue();
   
   BottomBarAIBackgroundTemplate.setImageMap(%imageMap);
   BottomBarAIBackgroundTemplate.setFrame(%frame);
   
   $BottomBarBackground[0].setImageMap(%imageMap);
   $BottomBarBackground[0].setFrame(%frame);
   $BottomBarBackground[1].setImageMap(%imageMap);
   $BottomBarBackground[1].setFrame(%frame);
   
   //------------------------------
   // Buttons Tab
   //------------------------------ 
   %this.saveButtonContainer(InterfaceEditorDealButtonContainer, TablePlayDealButton);   
   %this.saveButtonContainer(InterfaceEditorRebetButtonContainer, TablePlayRebetButton);  
   %this.saveButtonContainer(InterfaceEditorHitButtonContainer, TablePlayHitButton);     
   %this.saveButtonContainer(InterfaceEditorSplitButtonContainer, TablePlaySplitButton);
   %this.saveButtonContainer(InterfaceEditorStandButtonContainer, TablePlayStandButton);
   %this.saveButtonContainer(InterfaceEditorDoubleButtonContainer, TablePlayDoubleButton);
   %this.saveButtonContainer(InterfaceEditorHowtoButtonContainer, TableSelectHowtoButton);
   %this.saveButtonContainer(InterfaceEditorLeaveButtonContainer, TablePlayCloseButton);
   %this.saveButtonContainer(InterfaceEditorCreditsButtonContainer, TableSelectCreditsButton);
   %this.saveButtonContainer(InterfaceEditorResetBankButtonContainer, TableSelectResetBankButton);
   
   //------------------------------
   // Icons Tab
   //------------------------------
   %this.saveIconContainer(InterfaceEditorBlackjackIconText, InterfaceEditorBlackjackIconFrameEditBox, blackjackIcon); 
   %this.saveIconContainer(InterfaceEditorBustIconText, InterfaceEditorBustIconFrameEditBox, bustIcon); 
   %this.saveIconContainer(InterfaceEditorWinIconText, InterfaceEditorWinIconFrameEditBox, winIcon); 
   %this.saveIconContainer(InterfaceEditorLoseIconText, InterfaceEditorLoseIconFrameEditBox, loseIcon); 
   %this.saveIconContainer(InterfaceEditorShufflingIconText, InterfaceEditorShufflingIconFrameEditBox, shufflingIcon);
   %this.saveIconContainer(InterfaceEditorPushIconText, InterfaceEditorPushIconFrameEditBox, pushIcon);  
   %this.saveIconContainer(InterfaceEditorBankruptIconText, InterfaceEditorBankruptIconFrameEditBox, BankruptIcon); 
   
   //------------------------------
   // How to Play Tab
   //------------------------------
   
   $Save::Interface::NumberOfHelpScreens = InterfaceEditorNumberOfHelpScreensPopup.getValue();
   
   // Array
   for (%i = 0; %i < InterfaceEditorHelpScreenArray.getCount(); %i++)
   { 
      %container = InterfaceEditorHelpScreenArray.getObject(%i);
      %textEdit = %container.findObjectByInternalName("imageText", true);      
      $Save::Interface::HelpScreens[%i] = "gui/images/" @ %textEdit.getText();
   } 
   
   // Buttons 
   %this.saveButtonContainer(InterfaceEditorHowtoForwardButtonContainer, helpScreenNextButton);   
   %this.saveButtonContainer(InterfaceEditorHowtoBackButtonContainer, helpScreenPreviousButton);
   %this.saveButtonContainer(InterfaceEditorHowtoCloseButtonContainer, helpScreenCloseButton);   
   
   //------------------------------
   // Credits Tab
   //------------------------------
   
   // Credits button on menu screen
   TableSelectCreditsButton.Visible = InterfaceEditorCreditsButtonCheckBox.getValue();     
   
   // Background
   %textEdit = InterfaceEditorCreditsBackgroundContainer.findObjectByInternalName("imageText", true);
   %bitmap = GetGuiImageFileRelativePath("gui/images/" @ %textEdit.getText());
   CreditsBackground.setBitmap(%bitmap);
   
   // Credits Image
   %textEdit = InterfaceEditorCreditsImageContainer.findObjectByInternalName("imageText", true);
   %bitmap = GetGuiImageFileRelativePath("gui/images/" @ %textEdit.getText());
   CreditsImage.setBitmap(%bitmap);
   
   // Close Button
   %this.saveButtonContainer(InterfaceEditorCreditsCloseButtonContainer, closeCreditsButton);   
   
   //------------------------------

   // Save guis
   SaveGui(TablePlayGui); 
   SaveGui(TableSelectGui);     
   SaveGui(HelpScreenGui);
   SaveGui(creditsGui);
      
   export("$Save::Interface::*","^project/data/files/export_interface.cs", true, false);  

   LBProjectObj.saveLevel();
   SaveAllLevelDatablocks();   
   
   $CurrentGuiSerialString = %this.serialize();
   %this.SetDirtyValue(false);
}

/// <summary>
/// Called when the save button is pressed.
/// </summary>
function InterfaceEditorGui::saveButtonPressed(%this)
{
   InterfaceEditorGui.doSave();
   Canvas.popDialog(%this);
}

/// <summary>
/// Called when the cancel button is pressed.
/// </summary>
function InterfaceEditorGui::cancelButtonPressed(%this)
{
   // final check for dirty
   if ($CurrentGuiSerialString !$= %this.serialize())
       %this.SetDirtyValue(true);    

   if (%this.IsDirty())
   {
      ConfirmDialog.setupAndShow(InterfaceEditorWindow.getGlobalCenter(), "Save Changes?", 
            "Save", "InterfaceEditorGui.saveButtonPressed();", 
            "Don't Save", "Canvas.popDialog(InterfaceEditorGui);", 
            "Cancel", "");
   }
   else
   {
      Canvas.popDialog(%this);
   }
}


//-----------------------------------------------------------------------------
// Done / Save / Help / Done
//-----------------------------------------------------------------------------

function InterfaceEditorDoneButton::onClick(%this)
{
   InterfaceEditorGui.cancelButtonPressed();
}

function InterfaceEditorToolSaveButton::onClick(%this)
{
   InterfaceEditorGui.doSave();
}

/// <summary>
/// Launch help page.
/// </summary>
function InterfaceEditorHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/blackjack/interface/");
}

//-----------------------------------------------------------------------------
// Select assets
//-----------------------------------------------------------------------------

function InterfaceEditorSetContainerFields(%container, %asset)
{
    %imageFile = expandPath("^project/gui/images/") @ %asset;
    
    %imagePreview = %container.findObjectByInternalName("ImagePreview", true);
    if (isObject(%imagePreview))
    {
        %imagePreview.asset = %asset;
        %imagePreview.setBitmap(%imageFile);
    }

    %textEdit = %container.findObjectByInternalName("imageText", true);    
    if (isObject(%textEdit))
        %textEdit.text = fileName(%asset);  
        
    %popup = %container.findObjectByInternalName("PopupMenu", true);
    if (isObject(%popup))
    {
        InterfaceEditorGui.setButtonBitmap(%container, GetGuiImageFileRelativePath(%textEdit.getText()));
    }
}

function HelpScreen1Select::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function HelpScreen1Select::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(HelpScreen1Container, %asset);
}

function HelpScreen2Select::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function HelpScreen2Select::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(HelpScreen2Container, %asset);
}

function HelpScreen3Select::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function HelpScreen3Select::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(HelpScreen3Container, %asset);
}

function HelpScreen4Select::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function HelpScreen4Select::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(HelpScreen4Container, %asset);
}

function HelpScreen5Select::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function HelpScreen5Select::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(HelpScreen5Container, %asset);
}

function HelpScreen6Select::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function HelpScreen6Select::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(HelpScreen6Container, %asset);
}

function InterfaceEditorCreditsBackgroundButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorCreditsBackgroundButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorCreditsBackgroundContainer, %asset);
}

function InterfaceEditorCreditsImageButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorCreditsImageButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorCreditsImageContainer, %asset);
}

// Buttons

function InterfaceEditorDealButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorDealButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorDealButtonContainer, %asset);
}

function InterfaceEditorHitButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorHitButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorHitButtonContainer, %asset);
}

function InterfaceEditorStandButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorStandButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorStandButtonContainer, %asset);
}

function InterfaceEditorRebetButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorRebetButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorRebetButtonContainer, %asset);
}

function InterfaceEditorSplitButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorSplitButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorSplitButtonContainer, %asset);
}

function InterfaceEditorDoubleButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorDoubleButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorDoubleButtonContainer, %asset);
}

function InterfaceEditorHowtoButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorHowtoButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorHowtoButtonContainer, %asset);
}

function InterfaceEditorLeaveButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorLeaveButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorLeaveButtonContainer, %asset);
}

function InterfaceEditorCreditsButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorCreditsButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorCreditsButtonContainer, %asset);
}

function InterfaceEditorResetBankButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorResetBankButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorResetBankButtonContainer, %asset);
}

function InterfaceEditorHowtoForwardButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorHowtoForwardButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorHowtoForwardButtonContainer, %asset);
}

function InterfaceEditorHowtoCloseButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorHowtoCloseButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorHowtoCloseButtonContainer, %asset);
}

function InterfaceEditorHowtoBackButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorHowtoBackButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorHowtoBackButtonContainer, %asset);
}

function InterfaceEditorCreditsCloseButton::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function InterfaceEditorCreditsCloseButton::setSelectedAsset(%this, %asset)
{
    InterfaceEditorSetContainerFields(InterfaceEditorCreditsCloseButtonContainer, %asset);
}

//- End Buttons ------------------------------------

/// <summary>
/// Called when an item is selected in a popup menu for a button container.
/// </summary>
function InterfaceEditorGui::buttonPopupSelected(%this, %container)
{
    //%container = $ThisControl.getParent().getName();
    %this.updateButtonContainer(%container);
}

/// <summary>
/// Updates the visibility of the how-to screen containers based on the
/// InterfaceEditorNumberOfHelpScreensPopup.
/// </summary>
function InterfaceEditorGui::updateHowtoArrayVisibility(%this)
{
   %numVisible = InterfaceEditorNumberOfHelpScreensPopup.getValue();
      
   for (%i = 0; %i < InterfaceEditorHelpScreenArray.getCount(); %i++)
   {
      %container = InterfaceEditorHelpScreenArray.getObject(%i);
      
      if (%i + 1 > %numVisible)
         %container.visible = false;
      else
         %container.visible = true;
   }
}

/// <summary>
/// onSelect callback.
/// </summary>
function InterfaceEditorNumberOfHelpScreensPopup::onSelect(%this)
{
   InterfaceEditorGui.updateHowtoArrayVisibility();
}

/// <summary>
/// Serializes the state of the GUI into a space-delimited string.
/// </summary>
/// <return>A space-delimited string.</return>
function InterfaceEditorGui::serialize(%this)
{
   %serialString = InterfaceEditorCasinoCarpetText.getText();
   %serialString = %serialString SPC InterfaceEditorBankContainerText.getText();
   %serialString = %serialString SPC InterfaceEditorBankContainerFrameEditBox.getText();
   %serialString = %serialString SPC InterfaceEditorPlayerAvatarText.getText();
   %serialString = %serialString SPC InterfaceEditorPlayerAvatarFrameEditBox.getText();
   %serialString = %serialString SPC InterfaceEditorIndicationArrowText.getText();
   %serialString = %serialString SPC InterfaceEditorIndicationArrowFrameEditBox.getText();
   %serialString = %serialString SPC InterfaceEditorNpcBankImageText.getText();
   %serialString = %serialString SPC InterfaceEditorNpcBankFrameEditBox.getText();
   
   %serialString = %serialString SPC %this.serializeButtonContainer(InterfaceEditorDealButtonContainer, TablePlayDealButton);
   %serialString = %serialString SPC %this.serializeButtonContainer(InterfaceEditorRebetButtonContainer, TablePlayRebetButton);
   %serialString = %serialString SPC %this.serializeButtonContainer(InterfaceEditorHitButtonContainer, TablePlayHitButton);
   %serialString = %serialString SPC %this.serializeButtonContainer(InterfaceEditorSplitButtonContainer, TablePlaySplitButton);
   %serialString = %serialString SPC %this.serializeButtonContainer(InterfaceEditorStandButtonContainer, TablePlayStandButton);
   %serialString = %serialString SPC %this.serializeButtonContainer(InterfaceEditorDoubleButtonContainer, TablePlayDoubleButton);
   %serialString = %serialString SPC %this.serializeButtonContainer(InterfaceEditorHowtoButtonContainer, TableSelectHowtoButton);
   %serialString = %serialString SPC %this.serializeButtonContainer(InterfaceEditorLeaveButtonContainer, TablePlayCloseButton);
   %serialString = %serialString SPC %this.serializeButtonContainer(InterfaceEditorCreditsButtonContainer, TableSelectCreditsButton);
   %serialString = %serialString SPC %this.serializeButtonContainer(InterfaceEditorResetBankButtonContainer, TableSelectResetBankButton);
   
   %serialString = %serialString SPC %this.serializeIconContainer(InterfaceEditorBlackjackIconText, InterfaceEditorBlackjackIconFrameEditBox);
   %serialString = %serialString SPC %this.serializeIconContainer(InterfaceEditorBustIconText, InterfaceEditorBustIconFrameEditBox); 
   %serialString = %serialString SPC %this.serializeIconContainer(InterfaceEditorWinIconText, InterfaceEditorWinIconFrameEditBox); 
   %serialString = %serialString SPC %this.serializeIconContainer(InterfaceEditorLoseIconText, InterfaceEditorLoseIconFrameEditBox);
   %serialString = %serialString SPC %this.serializeIconContainer(InterfaceEditorShufflingIconText, InterfaceEditorShufflingIconFrameEditBox);
   %serialString = %serialString SPC %this.serializeIconContainer(InterfaceEditorPushIconText, InterfaceEditorPushIconFrameEditBox); 
   %serialString = %serialString SPC %this.serializeIconContainer(InterfaceEditorBankruptIconText, InterfaceEditorBankruptIconFrameEditBox); 
   
   %serialString = %serialString SPC InterfaceEditorNumberOfHelpScreensPopup.getValue(); 
   
   // Array
   for (%i = 0; %i < InterfaceEditorHelpScreenArray.getCount(); %i++)
   { 
      %container = InterfaceEditorHelpScreenArray.getObject(%i);
      %textEdit = %container.findObjectByInternalName("imageText", true);      
      %serialString = %serialString SPC %textEdit.getText();
   } 
   
   %serialString = %serialString SPC %this.serializeButtonContainer(InterfaceEditorHowtoForwardButtonContainer);  
   %serialString = %serialString SPC %this.serializeButtonContainer(InterfaceEditorHowtoBackButtonContainer);
   %serialString = %serialString SPC %this.serializeButtonContainer(InterfaceEditorHowtoCloseButtonContainer);   
   
   %serialString = %serialString SPC InterfaceEditorCreditsButtonCheckBox.getValue();     
   %serialString = %serialString SPC InterfaceEditorCreditsBackgroundContainer-->imageText.getText();
   %serialString = %serialString SPC InterfaceEditorCreditsImageContainer-->imageText.getText();
   %serialString = %serialString SPC %this.serializeButtonContainer(InterfaceEditorCreditsCloseButtonContainer);  
   
   return %serialString;
}

// Asset selector callback functions

/// <summary>Callback from AssetLibrary.open</summary>
function TitleScreenContainer::setSelectedAsset(%this, %asset)
{
   InterfaceEditorCasinoCarpetText.setText(%asset);
   // todo (2012/03/22): use actual frame when available
   InterfaceEditorCasinoCarpetFrameEditBox.setValue(0);
   
   InterfaceEditorCasinoCarpetPreview.refresh();
}
   
/// <summary>Callback from AssetLibrary.open</summary>
function BankContainer::setSelectedAsset(%this, %asset)
{
   InterfaceEditorBankContainerText.setText(%asset);
   // todo (2012/03/22): use actual frame when available
   InterfaceEditorBankContainerFrameEditBox.setValue(0);
   
   InterfaceEditorBankContainerPreview.refresh();
}

/// <summary>Callback from AssetLibrary.open</summary>
function PlayerAvatarContainer::setSelectedAsset(%this, %asset)
{
   InterfaceEditorPlayerAvatarText.setText(%asset);
   // todo (2012/03/22): use actual frame when available
   InterfaceEditorPlayerAvatarFrameEditBox.setValue(0);
   
   InterfaceEditorPlayerAvatarPreview.refresh();
}

/// <summary>Callback from AssetLibrary.open</summary>
function IndicationArrowContainer::setSelectedAsset(%this, %asset)
{
   InterfaceEditorIndicationArrowText.setText(%asset);
   // todo (2012/03/22): use actual frame when available
   InterfaceEditorBankIndicationArrowEditBox.setValue(0);
   
   InterfaceEditorIndicationArrowPreview.refresh();
}

/// <summary>Callback from AssetLibrary.open</summary>
function NpcBankImageContainer::setSelectedAsset(%this, %asset)
{
   InterfaceEditorNpcBankImageText.setText(%asset);
   // todo (2012/03/22): use actual frame when available
   InterfaceEditorNpcBankFrameEditBox.setValue(0);
   
   InterfaceEditorNpcBankImagePreview.refresh();
}

/// <summary>Callback from AssetLibrary.open</summary>   
function InterfaceEditorBlackjackIconContainer::setSelectedAsset(%this, %asset)
{
   // todo (2012/03/20): When AssetLibrary returns a frame number, use that instead of 0.
   InterfaceEditorGui.initializeIconContainer(InterfaceEditorBlackjackIconPreview, InterfaceEditorBlackjackIconText, 
                                              InterfaceEditorBlackjackIconFrameEditBox, %asset, 0);
   InterfaceEditorBlackjackIconFrameEditBox.onValidate();
}

/// <summary>Callback from AssetLibrary.open</summary>
function InterfaceEditorBustIconContainer::setSelectedAsset(%this, %asset)
{
   // todo (2012/03/20): When AssetLibrary returns a frame number, use that instead of 0.
   InterfaceEditorGui.initializeIconContainer(InterfaceEditorBustIconPreview, InterfaceEditorBustIconText, 
                                              InterfaceEditorBustIconFrameEditBox, %asset, 0);
   InterfaceEditorBustIconFrameEditBox.onValidate();
}

/// <summary>Callback from AssetLibrary.open</summary>
function InterfaceEditorWinIconContainer::setSelectedAsset(%this, %asset)
{
   // todo (2012/03/20): When AssetLibrary returns a frame number, use that instead of 0.
   InterfaceEditorGui.initializeIconContainer(InterfaceEditorWinIconPreview, InterfaceEditorWinIconText, 
                                              InterfaceEditorWinIconFrameEditBox, %asset, 0);
   InterfaceEditorWinIconFrameEditBox.onValidate();
}

/// <summary>Callback from AssetLibrary.open</summary>
function InterfaceEditorLoseIconContainer::setSelectedAsset(%this, %asset)
{
   // todo (2012/03/20): When AssetLibrary returns a frame number, use that instead of 0.
   InterfaceEditorGui.initializeIconContainer(InterfaceEditorLoseIconPreview, InterfaceEditorLoseIconText, 
                                              InterfaceEditorLoseIconFrameEditBox, %asset, 0);
   InterfaceEditorLoseIconFrameEditBox.onValidate();
}

/// <summary>Callback from AssetLibrary.open</summary>
function InterfaceEditorShufflingIconContainer::setSelectedAsset(%this, %asset)
{
   // todo (2012/03/20): When AssetLibrary returns a frame number, use that instead of 0.
   InterfaceEditorGui.initializeIconContainer(InterfaceEditorShufflingIconPreview, InterfaceEditorShufflingIconText, 
                                              InterfaceEditorShufflingIconFrameEditBox, %asset, 0);
   InterfaceEditorShufflingIconFrameEditBox.onValidate();
}

/// <summary>Callback from AssetLibrary.open</summary>
function InterfaceEditorBankruptIconContainer::setSelectedAsset(%this, %asset)
{
   // todo (2012/03/20): When AssetLibrary returns a frame number, use that instead of 0.
   InterfaceEditorGui.initializeIconContainer(InterfaceEditorBankruptIconPreview, InterfaceEditorBankruptIconText, 
                                              InterfaceEditorBankruptIconFrameEditBox, %asset, 0);
   InterfaceEditorBankruptIconFrameEditBox.onValidate();
}

/// <summary>Callback from AssetLibrary.open</summary>
function InterfaceEditorPushIconContainer::setSelectedAsset(%this, %asset)
{
   // todo (2012/03/20): When AssetLibrary returns a frame number, use that instead of 0.
   InterfaceEditorGui.initializeIconContainer(InterfaceEditorPushIconPreview, InterfaceEditorPushIconText, 
                                              InterfaceEditorPushIconFrameEditBox, %asset, 0);
   InterfaceEditorPushIconFrameEditBox.onValidate();
}

//--------------------------------
// InterfaceEditorCasinoCarpetImage
//--------------------------------

/// <summary>
/// Refresh the t2dStaticSprite Gui.
/// </summary>
function InterfaceEditorCasinoCarpetPreview::refresh(%this)
{ 
   if (InterfaceEditorCasinoCarpetText.getText() $= "")
   {
      InterfaceEditorCasinoCarpetPreview.display("");
      return;  
   }

   InterfaceEditorCasinoCarpetPreview.display(InterfaceEditorCasinoCarpetText.getText(), "t2dStaticSprite");
   InterfaceEditorCasinoCarpetPreview.sprite.setFrame(InterfaceEditorCasinoCarpetFrameEditBox.getValue());

   // Toggle frame select visibility based on number of frames.
   InterfaceEditorCasinoCarpetFrameContainer.Visible = (InterfaceEditorCasinoCarpetText.getText().getFrameCount() > 1);
}

/// <summary>
/// Refresh the Gui.
/// </summary>
function InterfaceEditorCasinoCarpetFrameEditBox::refresh(%this)
{
   if (isObject(CasinoCarpetBackground.getImageMap()))
      %this.text = CasinoCarpetBackground.getImageMap().getFrame();
   else
      %this.text = "0";
}

/// <summary>
/// Validate frame number.
/// </summary>
function InterfaceEditorCasinoCarpetFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (InterfaceEditorCasinoCarpetText.getText().getFrameCount() - 1))
      %this.setValue(InterfaceEditorCasinoCarpetText.getText().getFrameCount() - 1);
      
   InterfaceEditorCasinoCarpetPreview.refresh();
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function InterfaceEditorCasinoCarpetFrameSpinLeft::onClick(%this)
{
   InterfaceEditorCasinoCarpetFrameEditBox.setValue(InterfaceEditorCasinoCarpetFrameEditBox.getValue() - 1);
   InterfaceEditorCasinoCarpetFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function InterfaceEditorCasinoCarpetFrameSpinRight::onClick(%this)
{
   InterfaceEditorCasinoCarpetFrameEditBox.setValue(InterfaceEditorCasinoCarpetFrameEditBox.getValue() + 1);
   InterfaceEditorCasinoCarpetFrameEditBox.onValidate();
}

//--------------------------------
// InterfaceEditorBankContainerImage
//--------------------------------

/// <summary>
/// Refresh the t2dStaticSprite Gui.
/// </summary>
function InterfaceEditorBankContainerPreview::refresh(%this)
{ 
   if (InterfaceEditorBankContainerText.getText() $= "")
   {
      InterfaceEditorBankContainerPreview.display("");
      return;  
   }

   InterfaceEditorBankContainerPreview.display(InterfaceEditorBankContainerText.getText(), "t2dStaticSprite");
   InterfaceEditorBankContainerPreview.sprite.setFrame(InterfaceEditorBankContainerFrameEditBox.getValue());

   // Toggle frame select visibility based on number of frames.
   InterfaceEditorBankContainerFrameContainer.Visible = (InterfaceEditorBankContainerText.getText().getFrameCount() > 1);
}

/// <summary>
/// Refresh the Gui.
/// </summary>
function InterfaceEditorBankContainerFrameEditBox::refresh(%this)
{
   if (isObject(BankBackgroundObject.getImageMap()))
      %this.text = BankBackgroundObject.getImageMap().getFrame();
   else
      %this.text = "0";
}

/// <summary>
/// Validate frame number.
/// </summary>
function InterfaceEditorBankContainerFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (InterfaceEditorBankContainerText.getText().getFrameCount() - 1))
      %this.setValue(InterfaceEditorBankContainerText.getText().getFrameCount() - 1);
      
   InterfaceEditorBankContainerPreview.refresh();
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function InterfaceEditorBankContainerFrameSpinLeft::onClick(%this)
{
   InterfaceEditorBankContainerFrameEditBox.setValue(InterfaceEditorBankContainerFrameEditBox.getValue() - 1);
   InterfaceEditorBankContainerFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function InterfaceEditorBankContainerFrameSpinRight::onClick(%this)
{
   InterfaceEditorBankContainerFrameEditBox.setValue(InterfaceEditorBankContainerFrameEditBox.getValue() + 1);
   InterfaceEditorBankContainerFrameEditBox.onValidate();
}

//--------------------------------
// InterfaceEditorPlayerAvatarImage
//--------------------------------

/// <summary>
/// Refresh the t2dStaticSprite Gui.
/// </summary>
function InterfaceEditorPlayerAvatarPreview::refresh(%this)
{ 
   if (InterfaceEditorPlayerAvatarText.getText() $= "")
   {
      InterfaceEditorPlayerAvatarPreview.display("");
      return;  
   }

   InterfaceEditorPlayerAvatarPreview.display(InterfaceEditorPlayerAvatarText.getText(), "t2dStaticSprite");
   InterfaceEditorPlayerAvatarPreview.sprite.setFrame(InterfaceEditorPlayerAvatarFrameEditBox.getValue());

   // Toggle frame select visibility based on number of frames.
   InterfaceEditorPlayerAvatarFrameContainer.Visible = (InterfaceEditorPlayerAvatarText.getText().getFrameCount() > 1);
}

/// <summary>
/// Refresh the Gui.
/// </summary>
function InterfaceEditorPlayerAvatarFrameEditBox::refresh(%this)
{
   if (isObject(userAi.getImageMap()))
      %this.text = userAi.getImageMap().getFrame();
   else
      %this.text = "0";
}

/// <summary>
/// Validate frame number.
/// </summary>
function InterfaceEditorPlayerAvatarFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (InterfaceEditorPlayerAvatarText.getText().getFrameCount() - 1))
      %this.setValue(InterfaceEditorPlayerAvatarText.getText().getFrameCount() - 1);
      
   InterfaceEditorPlayerAvatarPreview.refresh();
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function InterfaceEditorPlayerAvatarFrameSpinLeft::onClick(%this)
{
   InterfaceEditorPlayerAvatarFrameEditBox.setValue(InterfaceEditorPlayerAvatarFrameEditBox.getValue() - 1);
   InterfaceEditorPlayerAvatarFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function InterfaceEditorPlayerAvatarFrameSpinRight::onClick(%this)
{
   InterfaceEditorPlayerAvatarFrameEditBox.setValue(InterfaceEditorPlayerAvatarFrameEditBox.getValue() + 1);
   InterfaceEditorPlayerAvatarFrameEditBox.onValidate();
}

//-------------------------------------
// InterfaceEditorIndicationArrowImage
//-------------------------------------

/// <summary>
/// Refresh the t2dStaticSprite Gui.
/// </summary>
function InterfaceEditorIndicationArrowPreview::refresh(%this)
{ 
   if (InterfaceEditorIndicationArrowText.getText() $= "")
   {
      InterfaceEditorIndicationArrowPreview.display("");
      return;  
   }

   InterfaceEditorIndicationArrowPreview.display(InterfaceEditorIndicationArrowText.getText(), "t2dStaticSprite");
   InterfaceEditorIndicationArrowPreview.sprite.setFrame(InterfaceEditorIndicationArrowFrameEditBox.getValue());

   // Toggle frame select visibility based on number of frames.
   InterfaceEditorIndicationArrowFrameContainer.Visible = (InterfaceEditorIndicationArrowText.getText().getFrameCount() > 1);
}

/// <summary>
/// Refresh the Gui.
/// </summary>
function InterfaceEditorIndicationArrowFrameEditBox::refresh(%this)
{
   if (isObject(currentHandIcon.getImageMap()))
      %this.text = currentHandIcon.getImageMap().getFrame();
   else
      %this.text = "0";
}

/// <summary>
/// Validate frame number.
/// </summary>
function InterfaceEditorIndicationArrowFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (InterfaceEditorIndicationArrowText.getText().getFrameCount() - 1))
      %this.setValue(InterfaceEditorIndicationArrowText.getText().getFrameCount() - 1);
      
   InterfaceEditorIndicationArrowPreview.refresh();
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function InterfaceEditorIndicationArrowFrameSpinLeft::onClick(%this)
{
   InterfaceEditorIndicationArrowFrameEditBox.setValue(InterfaceEditorIndicationArrowFrameEditBox.getValue() - 1);
   InterfaceEditorIndicationArrowFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function InterfaceEditorIndicationArrowFrameSpinRight::onClick(%this)
{
   InterfaceEditorIndicationArrowFrameEditBox.setValue(InterfaceEditorIndicationArrowFrameEditBox.getValue() + 1);
   InterfaceEditorIndicationArrowFrameEditBox.onValidate();
}

//--------------------------------
// InterfaceEditorNpcBankImage
//--------------------------------

/// <summary>
/// Refresh the t2dStaticSprite Gui.
/// </summary>
function InterfaceEditorNpcBankImagePreview::refresh(%this)
{ 
   if (InterfaceEditorNpcBankImageText.getText() $= "")
   {
      InterfaceEditorNpcBankImagePreview.display("");
      return;  
   }

   InterfaceEditorNpcBankImagePreview.display(InterfaceEditorNpcBankImageText.getText(), "t2dStaticSprite");
   InterfaceEditorNpcBankImagePreview.sprite.setFrame(InterfaceEditorNpcBankFrameEditBox.getValue());

   // Toggle frame select visibility based on number of frames.
   InterfaceEditorNpcBankFrameContainer.Visible = (InterfaceEditorNpcBankImageText.getText().getFrameCount() > 1);
}

/// <summary>
/// Refresh the Gui.
/// </summary>
function InterfaceEditorNpcBankFrameEditBox::refresh(%this)
{
   if (isObject(BottomBarAIBackgroundTemplate.getImageMap()))
      %this.text = BottomBarAIBackgroundTemplate.getImageMap().getFrame();
   else
      %this.text = "0";
}

/// <summary>
/// Validate frame number.
/// </summary>
function InterfaceEditorNpcBankFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (InterfaceEditorNpcBankImageText.getText().getFrameCount() - 1))
      %this.setValue(InterfaceEditorNpcBankImageText.getText().getFrameCount() - 1);
      
   InterfaceEditorNpcBankImagePreview.refresh();
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function InterfaceEditorNpcBankFrameSpinLeft::onClick(%this)
{
   InterfaceEditorNpcBankFrameEditBox.setValue(InterfaceEditorNpcBankFrameEditBox.getValue() - 1);
   InterfaceEditorNpcBankFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function InterfaceEditorNpcBankFrameSpinRight::onClick(%this)
{
   InterfaceEditorNpcBankFrameEditBox.setValue(InterfaceEditorNpcBankFrameEditBox.getValue() + 1);
   InterfaceEditorNpcBankFrameEditBox.onValidate();
}

//-----------------------------------
// InterfaceEditorBlackjackIconFrame
//-----------------------------------

/// <summary>
/// Validate frame number.
/// </summary>
function InterfaceEditorBlackjackIconFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (InterfaceEditorBlackjackIconText.getText().getFrameCount() - 1))
      %this.setValue(InterfaceEditorBlackjackIconText.getText().getFrameCount() - 1);
      
   InterfaceEditorBlackjackIconPreview.sprite.setFrame(%this.getValue());

   // Toggle frame select visibility based on number of frames.
   InterfaceEditorBlackjackIconFrameContainer.Visible = (InterfaceEditorBlackjackIconText.getText().getFrameCount() > 1);
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function InterfaceEditorBlackjackIconFrameSpinLeft::onClick(%this)
{
   InterfaceEditorBlackjackIconFrameEditBox.setValue(InterfaceEditorBlackjackIconFrameEditBox.getValue() - 1);
   InterfaceEditorBlackjackIconFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function InterfaceEditorBlackjackIconFrameSpinRight::onClick(%this)
{
   InterfaceEditorBlackjackIconFrameEditBox.setValue(InterfaceEditorBlackjackIconFrameEditBox.getValue() + 1);
   InterfaceEditorBlackjackIconFrameEditBox.onValidate();
}
//--------------------------------
// InterfaceEditorBustIconFrame
//--------------------------------

/// <summary>
/// Validate frame number.
/// </summary>
function InterfaceEditorBustIconFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (InterfaceEditorBustIconText.getText().getFrameCount() - 1))
      %this.setValue(InterfaceEditorBustIconText.getText().getFrameCount() - 1);
      
   InterfaceEditorBustIconPreview.sprite.setFrame(%this.getValue());

   // Toggle frame select visibility based on number of frames.
   InterfaceEditorBustIconFrameContainer.Visible = (InterfaceEditorBustIconText.getText().getFrameCount() > 1);
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function InterfaceEditorBustIconFrameSpinLeft::onClick(%this)
{
   InterfaceEditorBustIconFrameEditBox.setValue(InterfaceEditorBustIconFrameEditBox.getValue() - 1);
   InterfaceEditorBustIconFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function InterfaceEditorBustIconFrameSpinRight::onClick(%this)
{
   InterfaceEditorBustIconFrameEditBox.setValue(InterfaceEditorBustIconFrameEditBox.getValue() + 1);
   InterfaceEditorBustIconFrameEditBox.onValidate();
}

//--------------------------------
// InterfaceEditorWinIconFrame
//--------------------------------

/// <summary>
/// Validate frame number.
/// </summary>
function InterfaceEditorWinIconFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (InterfaceEditorWinIconText.getText().getFrameCount() - 1))
      %this.setValue(InterfaceEditorWinIconText.getText().getFrameCount() - 1);
      
   InterfaceEditorWinIconPreview.sprite.setFrame(%this.getValue());

   // Toggle frame select visibility based on number of frames.
   InterfaceEditorWinIconFrameContainer.Visible = (InterfaceEditorWinIconText.getText().getFrameCount() > 1);
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function InterfaceEditorWinIconFrameSpinLeft::onClick(%this)
{
   InterfaceEditorWinIconFrameEditBox.setValue(InterfaceEditorWinIconFrameEditBox.getValue() - 1);
   InterfaceEditorWinIconFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function InterfaceEditorWinIconFrameSpinRight::onClick(%this)
{
   InterfaceEditorWinIconFrameEditBox.setValue(InterfaceEditorWinIconFrameEditBox.getValue() + 1);
   InterfaceEditorWinIconFrameEditBox.onValidate();
}

//--------------------------------
// InterfaceEditorLoseIconFrame
//--------------------------------

/// <summary>
/// Validate frame number.
/// </summary>
function InterfaceEditorLoseIconFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (InterfaceEditorLoseIconText.getText().getFrameCount() - 1))
      %this.setValue(InterfaceEditorLoseIconText.getText().getFrameCount() - 1);
      
   InterfaceEditorLoseIconPreview.sprite.setFrame(%this.getValue());

   // Toggle frame select visibility based on number of frames.
   InterfaceEditorLoseIconFrameContainer.Visible = (InterfaceEditorLoseIconText.getText().getFrameCount() > 1);
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function InterfaceEditorLoseIconFrameSpinLeft::onClick(%this)
{
   InterfaceEditorLoseIconFrameEditBox.setValue(InterfaceEditorLoseIconFrameEditBox.getValue() - 1);
   InterfaceEditorLoseIconFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function InterfaceEditorLoseIconFrameSpinRight::onClick(%this)
{
   InterfaceEditorLoseIconFrameEditBox.setValue(InterfaceEditorLoseIconFrameEditBox.getValue() + 1);
   InterfaceEditorLoseIconFrameEditBox.onValidate();
}

//--------------------------------
// InterfaceEditorShufflingIconFrame
//--------------------------------

/// <summary>
/// Validate frame number.
/// </summary>
function InterfaceEditorShufflingIconFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (InterfaceEditorShufflingIconText.getText().getFrameCount() - 1))
      %this.setValue(InterfaceEditorShufflingIconText.getText().getFrameCount() - 1);
      
   InterfaceEditorShufflingIconPreview.sprite.setFrame(%this.getValue());

   // Toggle frame select visibility based on number of frames.
   InterfaceEditorShufflingIconFrameContainer.Visible = (InterfaceEditorShufflingIconText.getText().getFrameCount() > 1);
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function InterfaceEditorShufflingIconFrameSpinLeft::onClick(%this)
{
   InterfaceEditorShufflingIconFrameEditBox.setValue(InterfaceEditorShufflingIconFrameEditBox.getValue() - 1);
   InterfaceEditorShufflingIconFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function InterfaceEditorShufflingIconFrameSpinRight::onClick(%this)
{
   InterfaceEditorShufflingIconFrameEditBox.setValue(InterfaceEditorShufflingIconFrameEditBox.getValue() + 1);
   InterfaceEditorShufflingIconFrameEditBox.onValidate();
}

//--------------------------------
// InterfaceEditorPushIconFrame
//--------------------------------

/// <summary>
/// Validate frame number.
/// </summary>
function InterfaceEditorPushIconFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (InterfaceEditorPushIconText.getText().getFrameCount() - 1))
      %this.setValue(InterfaceEditorPushIconText.getText().getFrameCount() - 1);
      
   InterfaceEditorPushIconPreview.sprite.setFrame(%this.getValue());

   // Toggle frame select visibility based on number of frames.
   InterfaceEditorPushIconFrameContainer.Visible = (InterfaceEditorPushIconText.getText().getFrameCount() > 1);
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function InterfaceEditorPushIconFrameSpinLeft::onClick(%this)
{
   InterfaceEditorPushIconFrameEditBox.setValue(InterfaceEditorPushIconFrameEditBox.getValue() - 1);
   InterfaceEditorPushIconFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function InterfaceEditorPushIconFrameSpinRight::onClick(%this)
{
   InterfaceEditorPushIconFrameEditBox.setValue(InterfaceEditorPushIconFrameEditBox.getValue() + 1);
   InterfaceEditorPushIconFrameEditBox.onValidate();
}

//--------------------------------
// InterfaceEditorBankruptIconFrame
//--------------------------------

/// <summary>
/// Validate frame number.
/// </summary>
function InterfaceEditorBankruptIconFrameEditBox::onValidate(%this)
{
   if (%this.getValue() < 0)
      %this.setValue(0);
   else if (%this.getValue() > (InterfaceEditorBankruptIconText.getText().getFrameCount() - 1))
      %this.setValue(InterfaceEditorBankruptIconText.getText().getFrameCount() - 1);
      
   InterfaceEditorBankruptIconPreview.sprite.setFrame(%this.getValue());

   // Toggle frame select visibility based on number of frames.
   InterfaceEditorBankruptIconFrameContainer.Visible = (InterfaceEditorBankruptIconText.getText().getFrameCount() > 1);
}

/// <summary>
/// Handle click on left spinned button.
/// </summary>
function InterfaceEditorBankruptIconFrameSpinLeft::onClick(%this)
{
   InterfaceEditorBankruptIconFrameEditBox.setValue(InterfaceEditorBankruptIconFrameEditBox.getValue() - 1);
   InterfaceEditorBankruptIconFrameEditBox.onValidate();
}

/// <summary>
/// Handle click on right spinned button.
/// </summary>
function InterfaceEditorBankruptIconFrameSpinRight::onClick(%this)
{
   InterfaceEditorBankruptIconFrameEditBox.setValue(InterfaceEditorBankruptIconFrameEditBox.getValue() + 1);
   InterfaceEditorBankruptIconFrameEditBox.onValidate();
}
