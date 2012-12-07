//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$MaxTables = 5;
$GameDir = "";

$GeneralSettingsEditorName = "General Settings";

$GeneralSettingsStartingBankMin = 1;
$GeneralSettingsStartingBankMax = 999999;

/// <summary>
/// Called when another tab is selected
/// </summary>
function GeneralSettingsTabBook::onTabSelected(%this, %tab)
{
   alxStop($SoundContainerCurrentlyPlayingSound);
}

/// <summary>
/// Called when the control object is registered with the system after the control has been created. 
/// </summary>
function GeneralSettingsEditorGui::onAdd(%this)
{
   // load up all of our gui's so we can access the relevant resources
   //$GameDir = LBProjectObj.gamePath;
   //exec($GameDir @ "/gui/tableSelect.gui");
}

/// <summary>
/// Called when the control is woken up. 
/// </summary>
function GeneralSettingsEditorGui::onWake(%this)
{
   if(!isObject(TableSelectGui))
      exec("^project/gui/tableSelect.gui");
      
   GeneralSettingsTabBook.selectPage(0);
   
   %this.resetAllSoundContainers();   
   
   %this.refresh();
      
   // Validate any active textEdits
   GeneralSettingsStartingBankTextEdit.onValidate();
   
   // Serialized the starting state of the gui
   $CurrentGuiSerialString = %this.serialize();
   %this.SetDirtyValue(false);
   
   // Start the update loop for checking the dirty state
   %this.updateDirty();
}

/// <summary>
/// Called when the control is put to sleep. 
/// </summary>
function GeneralSettingsEditorGui::onSleep(%this)
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
function GeneralSettingsEditorGui::updateDirty(%this)
{
    if (!%this.isDirty())
    {
        if ($CurrentGuiSerialString !$= %this.serialize())
            %this.SetDirtyValue(true); 
    }
   
   %this.updateDirtyHandle = %this.schedule(200, updateDirty);
}


/// <summary>
/// Set the 'dirty' state of the window.
/// </summary>
function GeneralSettingsEditorGui::SetDirtyValue(%this, %dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      GeneralSettingsEditorWindow.setText($GeneralSettingsEditorName SPC "*");
   else
      GeneralSettingsEditorWindow.setText($GeneralSettingsEditorName);
}

/// <return>true if this window is 'dirty', otherwise false.</return>
function GeneralSettingsEditorGui::IsDirty(%this)
{
   return $Tools::TemplateToolDirty;
}

/// <summary>
/// Updates the GUI state from the game data.
/// </summary>
function GeneralSettingsEditorGui::refresh(%this)
{
   //GeneralSettingsFontSheetTextEdit
   GeneralSettingsFontSheetTextEdit.setText(bitmapFontTemplate.imageMap);
   GeneralSettingsFontSheetTextEdit.setActive(false);
   
   //GeneralSettingsStartingBankTextEdit
   GeneralSettingsStartingBankTextEdit.setText($Save::GeneralSettings::StartingBankCash);
   
   //GeneralSettingsNumberOfTablesPopup
   GeneralSettingsNumberOfTablesPopup.clear();
   for (%i = 0; %i < $MaxTables; %i++)
   {
      GeneralSettingsNumberOfTablesPopup.add(%i+1, %i);
      
      if (%i == $Save::GeneralSettings::NumberOfTables - 1)
         GeneralSettingsNumberOfTablesPopup.setSelected(%i);
   }
   
   // Set table mapping container visibility
   %this.updateTableMappings();
     
   // Table 1
    if (isFile(expandPath("^project/data/levels/" @ $Save::GeneralSettings::Table1Map @ ".t2d")))
        GeneralSettingsTable1MapTextEdit.setText($Save::GeneralSettings::Table1Map);
    else
        GeneralSettingsTable1MapTextEdit.setText("");
            
   GeneralSettingsTable1MenuImageTextEdit.setText(fileName(TableSelectTable1.bitmap));
   GeneralSettingsTable1MenuImagePreview.setBitmap(GetFullFileName(TableSelectTable1.bitmap));
   GeneralSettingsTable1MapTextEdit.setActive(false);
   GeneralSettingsTable1MenuImageTextEdit.setActive(false);
   
   // Table 2
    if (isFile(expandPath("^project/data/levels/" @ $Save::GeneralSettings::Table2Map @ ".t2d")))
        GeneralSettingsTable2MapTextEdit.setText($Save::GeneralSettings::Table2Map);
    else
        GeneralSettingsTable2MapTextEdit.setText("");
   
   GeneralSettingsTable2MenuImageTextEdit.setText(fileName(TableSelectTable2.bitmap));
   GeneralSettingsTable2MenuImagePreview.setBitmap(GetFullFileName(TableSelectTable2.bitmap));
   GeneralSettingsTable2MapTextEdit.setActive(false);
   GeneralSettingsTable2MenuImageTextEdit.setActive(false);
   
   // Table 3
    if (isFile(expandPath("^project/data/levels/" @ $Save::GeneralSettings::Table3Map @ ".t2d")))
        GeneralSettingsTable3MapTextEdit.setText($Save::GeneralSettings::Table3Map);
    else
        GeneralSettingsTable3MapTextEdit.setText("");

   GeneralSettingsTable3MenuImageTextEdit.setText(fileName(TableSelectTable3.bitmap));
   GeneralSettingsTable3MenuImagePreview.setBitmap(GetFullFileName(TableSelectTable3.bitmap));
   GeneralSettingsTable3MapTextEdit.setActive(false);
   GeneralSettingsTable3MenuImageTextEdit.setActive(false);
   
   // Table 4 
    if (isFile(expandPath("^project/data/levels/" @ $Save::GeneralSettings::Table4Map @ ".t2d")))
        GeneralSettingsTable4MapTextEdit.setText($Save::GeneralSettings::Table4Map);
    else
        GeneralSettingsTable4MapTextEdit.setText("");

   GeneralSettingsTable4MenuImageTextEdit.setText(fileName(TableSelectTable4.bitmap));
   GeneralSettingsTable4MenuImagePreview.setBitmap(GetFullFileName(TableSelectTable4.bitmap)); 
   GeneralSettingsTable4MapTextEdit.setActive(false);
   GeneralSettingsTable4MenuImageTextEdit.setActive(false);  
      
   // Table 5
    if (isFile(expandPath("^project/data/levels/" @ $Save::GeneralSettings::Table5Map @ ".t2d")))
        GeneralSettingsTable5MapTextEdit.setText($Save::GeneralSettings::Table5Map);
    else
        GeneralSettingsTable5MapTextEdit.setText("");

   GeneralSettingsTable5MenuImageTextEdit.setText(fileName(TableSelectTable5.bitmap));
   GeneralSettingsTable5MenuImagePreview.setBitmap(GetFullFileName(TableSelectTable5.bitmap));
   GeneralSettingsTable5MapTextEdit.setActive(false);
   GeneralSettingsTable5MenuImageTextEdit.setActive(false);
   
   // Update sounds
   setSoundContainer(GeneralSettingsSoundsArray.getObject(0), $Save::GeneralSettings::Sound::Shuffling); 
   setSoundContainer(GeneralSettingsSoundsArray.getObject(1), $Save::GeneralSettings::Sound::ChipStacking);
   setSoundContainer(GeneralSettingsSoundsArray.getObject(2), $Save::GeneralSettings::Sound::ChipSelect);
   setSoundContainer(GeneralSettingsSoundsArray.getObject(3), $Save::GeneralSettings::Sound::MenuMusic);
   //setSoundContainer(GeneralSettingsSoundsArray.getObject(4), $Save::GeneralSettings::Sound::WonHand);
   setSoundContainer(GeneralSettingsSoundsArray.getObject(4), $Save::GeneralSettings::Sound::CardDealt);
   setSoundContainer(GeneralSettingsSoundsArray.getObject(5), $Save::GeneralSettings::Sound::ButtonClick);
   setSoundContainer(GeneralSettingsSoundsArray.getObject(6), $Save::GeneralSettings::Sound::LostHand);
   setSoundContainer(GeneralSettingsSoundsArray.getObject(7), $Save::GeneralSettings::Sound::Bankrupt);
}

/// <summary>
/// Called when the save button is pressed.
/// </summary>
function GeneralSettingsEditorGui::doSave(%this)
{
   // Validate text edit fields
   GeneralSettingsStartingBankTextEdit.onValidate();   

   //GeneralSettingsFontSheetTextEdit
   bitmapFontTemplate.imageMap = GeneralSettingsFontSheetTextEdit.getText();
   
   //GeneralSettingsStartingBankTextEdit
   $Save::GeneralSettings::StartingBankCash = GeneralSettingsStartingBankTextEdit.getText();
   
   //GeneralSettingsNumberOfTablesPopup
   $Save::GeneralSettings::NumberOfTables = GeneralSettingsNumberOfTablesPopup.getText();
   
   //GeneralSettingsTable1MapTextEdit
   $Save::GeneralSettings::Table1Map = GeneralSettingsTable1MapTextEdit.getText();
   %bitmap = GetGuiImageFileRelativePath(GeneralSettingsTable1MenuImageTextEdit.getText());
   TableSelectTable1.setBitmap(%bitmap);

   
   //GeneralSettingsTable2MapTextEdit
   $Save::GeneralSettings::Table2Map = GeneralSettingsTable2MapTextEdit.getText();
   %bitmap = GetGuiImageFileRelativePath(GeneralSettingsTable2MenuImageTextEdit.getText());
   TableSelectTable2.setBitmap(%bitmap);
   
   //GeneralSettingsTable3MapTextEdit
   $Save::GeneralSettings::Table3Map = GeneralSettingsTable3MapTextEdit.getText();
   %bitmap = GetGuiImageFileRelativePath(GeneralSettingsTable3MenuImageTextEdit.getText());
   TableSelectTable3.setBitmap(%bitmap);
   
   //GeneralSettingsTable4MapTextEdit
   $Save::GeneralSettings::Table4Map = GeneralSettingsTable4MapTextEdit.getText();
   %bitmap = GetGuiImageFileRelativePath(GeneralSettingsTable4MenuImageTextEdit.getText());
   TableSelectTable4.setBitmap(%bitmap);
   
   //GeneralSettingsTable5MapTextEdit   
   $Save::GeneralSettings::Table5Map = GeneralSettingsTable5MapTextEdit.getText();
   %bitmap = GetGuiImageFileRelativePath(GeneralSettingsTable5MenuImageTextEdit.getText());
   TableSelectTable5.setBitmap(%bitmap);
   
   // Save Sounds
   $Save::GeneralSettings::Sound::Shuffling = getAudioProfileFromSoundContainer(GeneralSettingsSoundsArray.getObject(0)); 
   $Save::GeneralSettings::Sound::ChipStacking = getAudioProfileFromSoundContainer(GeneralSettingsSoundsArray.getObject(1));
   $Save::GeneralSettings::Sound::ChipSelect = getAudioProfileFromSoundContainer(GeneralSettingsSoundsArray.getObject(2));
   $Save::GeneralSettings::Sound::MenuMusic = getAudioProfileFromSoundContainer(GeneralSettingsSoundsArray.getObject(3));
   //$Save::GeneralSettings::Sound::WonHand = getAudioProfileFromSoundContainer(GeneralSettingsSoundsArray.getObject(4));
   $Save::GeneralSettings::Sound::CardDealt = getAudioProfileFromSoundContainer(GeneralSettingsSoundsArray.getObject(4));
   $Save::GeneralSettings::Sound::ButtonClick = getAudioProfileFromSoundContainer(GeneralSettingsSoundsArray.getObject(5));
   $Save::GeneralSettings::Sound::LostHand = getAudioProfileFromSoundContainer(GeneralSettingsSoundsArray.getObject(6));
   $Save::GeneralSettings::Sound::Bankrupt = getAudioProfileFromSoundContainer(GeneralSettingsSoundsArray.getObject(7));
 
   // Save guis
   SaveGui(TableSelectGui);         
      
   export("$Save::GeneralSettings::*","^project/data/files/export_generalSettings.cs", true, false); 

    AddAssetToLevelDatablocks(bitmapFontTemplate.imageMap);

   LBProjectObj.saveLevel();   
   SaveAllLevelDatablocks();

   $CurrentGuiSerialString = %this.serialize();
   %this.SetDirtyValue(false);
}

/// <summary>
/// Called when the save button is pressed.
/// </summary>
function GeneralSettingsEditorGui::saveButtonPressed(%this)
{
   GeneralSettingsEditorGui.doSave();
   Canvas.popDialog(%this);
}

/// <summary>
/// Called when the cancel button is pressed.
/// </summary>
function GeneralSettingsEditorGui::cancelButtonPressed(%this)
{
   // Validate text edit fields
   GeneralSettingsStartingBankTextEdit.onValidate();
   if ($CurrentGuiSerialString !$= %this.serialize())
       %this.SetDirtyValue(true);    
   
   if (%this.IsDirty())
   {
      ConfirmDialog.setupAndShow(GeneralSettingsEditorWindow.getGlobalCenter(), "Save Changes?", 
            "Save", "GeneralSettingsEditorGui.saveButtonPressed();", 
            "Don't Save", "Canvas.popDialog(GeneralSettingsEditorGui);", 
            "Cancel", "");
   }
   else
   {
      Canvas.popDialog(%this);
   }
}

/// <summary>
/// Resets the state of all sound containers in the sound tab.
/// </summary>
function GeneralSettingsEditorGui::resetAllSoundContainers(%this)
{
   for (%i = 0; %i < GeneralSettingsSoundsArray.getCount(); %i++)
   {
      %container = GeneralSettingsSoundsArray.getObject(%i);
      //setSoundButtonBitmap(%container, false);
      //%container.isPlayingSound = false;
   }
}

//-----------------------------------------------------------------------------
// Done / Save / Help
//-----------------------------------------------------------------------------

function GeneralSettingsEditorDoneButton::onClick(%this)
{
   GeneralSettingsEditorGui.cancelButtonPressed();
}

function GeneralSettingsToolSaveButton::onClick(%this)
{
   GeneralSettingsEditorGui.doSave();
}

/// <summary>
/// Launch help page.
/// </summary>
function GeneralSettingsHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/blackjack/generalsettings/");
}

/// <summary>
/// onSelect callback for GeneralSettingsNumberOfTablesPopup
/// </summary>
function GeneralSettingsNumberOfTablesPopup::onSelect(%this, %id, %text)
{
   GeneralSettingsEditorGui.updateTableMappings();
}

/// <summary>
/// Updates the visibility states of the table mapping containers
/// based on the numberOfTablesPopup selection.
/// </summary>
function GeneralSettingsEditorGui::updateTableMappings(%this)
{
   GeneralSettingsTable1Container.setVisible(GeneralSettingsNumberOfTablesPopup.getValue() >= 1 ? true : false);
   GeneralSettingsTable2Container.setVisible(GeneralSettingsNumberOfTablesPopup.getValue() >= 2 ? true : false);
   GeneralSettingsTable3Container.setVisible(GeneralSettingsNumberOfTablesPopup.getValue() >= 3 ? true : false);
   GeneralSettingsTable4Container.setVisible(GeneralSettingsNumberOfTablesPopup.getValue() >= 4 ? true : false);
   GeneralSettingsTable5Container.setVisible(GeneralSettingsNumberOfTablesPopup.getValue() >= 5 ? true : false);
   
   GeneralSettingsTable1Label.setVisible(GeneralSettingsNumberOfTablesPopup.getValue() >= 1 ? true : false);
   GeneralSettingsTable2Label.setVisible(GeneralSettingsNumberOfTablesPopup.getValue() >= 2 ? true : false);
   GeneralSettingsTable3Label.setVisible(GeneralSettingsNumberOfTablesPopup.getValue() >= 3 ? true : false);
   GeneralSettingsTable4Label.setVisible(GeneralSettingsNumberOfTablesPopup.getValue() >= 4 ? true : false);
   GeneralSettingsTable5Label.setVisible(GeneralSettingsNumberOfTablesPopup.getValue() >= 5 ? true : false);
   
   GeneralSettingsWireframeTable1.setVisible(GeneralSettingsNumberOfTablesPopup.getValue() >= 1 ? true : false);
   GeneralSettingsWireframeTable2.setVisible(GeneralSettingsNumberOfTablesPopup.getValue() >= 2 ? true : false);
   GeneralSettingsWireframeTable3.setVisible(GeneralSettingsNumberOfTablesPopup.getValue() >= 3 ? true : false);
   GeneralSettingsWireframeTable4.setVisible(GeneralSettingsNumberOfTablesPopup.getValue() >= 4 ? true : false);
   GeneralSettingsWireframeTable5.setVisible(GeneralSettingsNumberOfTablesPopup.getValue() >= 5 ? true : false);
}

/// <summary>
/// onValidate callback.
/// </summary>
function GeneralSettingsStartingBankTextEdit::onValidate(%this)
{  
   ValidateTextEditInteger(%this, $GeneralSettingsStartingBankMin, $GeneralSettingsStartingBankMax);
   %this.validatedText = %this.getText();  
}


/// <summary>
/// Handle click on the spin up button.
/// </summary>
function GeneralSettingsStartingBankSpinUp::onClick(%this)
{
   GeneralSettingsStartingBankTextEdit.setValue(GeneralSettingsStartingBankTextEdit.getValue() + 1);
   GeneralSettingsStartingBankTextEdit.onValidate();
}

/// <summary>
/// Handle click on the spin down button.
/// </summary>
function GeneralSettingsStartingBankSpinDown::onClick(%this)
{
   GeneralSettingsStartingBankTextEdit.setValue(GeneralSettingsStartingBankTextEdit.getValue() - 1);
   GeneralSettingsStartingBankTextEdit.onValidate();
}

//-----------------------------------------------------------------------------
// Select assets
//-----------------------------------------------------------------------------

function GeneralSettingsTable1MenuImageSelect::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function GeneralSettingsTable1MenuImageSelect::setSelectedAsset(%this, %asset)
{
    %imageFile = expandPath("^project/gui/images/") @ %asset;

    GeneralSettingsTable1MenuImagePreview.asset = %asset;
    GeneralSettingsTable1MenuImagePreview.setBitmap(%imageFile);
    GeneralSettingsTable1MenuImageTextEdit.text = fileName(%asset);
}

function GeneralSettingsTable2MenuImageSelect::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function GeneralSettingsTable2MenuImageSelect::setSelectedAsset(%this, %asset)
{
    %imageFile = expandPath("^project/gui/images/") @ %asset;

    GeneralSettingsTable2MenuImagePreview.asset = %asset;
    GeneralSettingsTable2MenuImagePreview.setBitmap(%imageFile);
    GeneralSettingsTable2MenuImageTextEdit.text = fileName(%asset);
}

function GeneralSettingsTable3MenuImageSelect::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function GeneralSettingsTable3MenuImageSelect::setSelectedAsset(%this, %asset)
{
    %imageFile = expandPath("^project/gui/images/") @ %asset;

    GeneralSettingsTable3MenuImagePreview.asset = %asset;
    GeneralSettingsTable3MenuImagePreview.setBitmap(%imageFile);
    GeneralSettingsTable3MenuImageTextEdit.text = fileName(%asset);
}

function GeneralSettingsTable4MenuImageSelect::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function GeneralSettingsTable4MenuImageSelect::setSelectedAsset(%this, %asset)
{
    %imageFile = expandPath("^project/gui/images/") @ %asset;

    GeneralSettingsTable4MenuImagePreview.asset = %asset;
    GeneralSettingsTable4MenuImagePreview.setBitmap(%imageFile);
    GeneralSettingsTable4MenuImageTextEdit.text = fileName(%asset);
}

function GeneralSettingsTable5MenuImageSelect::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

function GeneralSettingsTable5MenuImageSelect::setSelectedAsset(%this, %asset)
{
    %imageFile = expandPath("^project/gui/images/") @ %asset;

    GeneralSettingsTable5MenuImagePreview.asset = %asset;
    GeneralSettingsTable5MenuImagePreview.setBitmap(%imageFile);
    GeneralSettingsTable5MenuImageTextEdit.text = fileName(%asset);
}

//-----------------------------------------------------------------------------
// Serialize
//-----------------------------------------------------------------------------

/// <summary>
/// Serializes the state of the GUI into a space-delimited string.
/// </summary>
/// <return>A space-delimited string.</return>
function GeneralSettingsEditorGui::serialize(%this)
{
   %serialString = GeneralSettingsFontSheetTextEdit.getText();
   
   //Ben: %serialString = %serialString SPC GeneralSettingsStartingBankTextEdit.getText();
   %serialString = %serialString SPC GeneralSettingsStartingBankTextEdit.validatedText;
   
   %serialString = %serialString SPC GeneralSettingsNumberOfTablesPopup.getText();
   
   %serialString = %serialString SPC GeneralSettingsTable1MapTextEdit.getText();
   %serialString = %serialString SPC GeneralSettingsTable1MenuImageTextEdit.getText();

   %serialString = %serialString SPC GeneralSettingsTable2MapTextEdit.getText();
   %serialString = %serialString SPC GeneralSettingsTable2MenuImageTextEdit.getText();
   
   %serialString = %serialString SPC GeneralSettingsTable3MapTextEdit.getText();
   %serialString = %serialString SPC GeneralSettingsTable3MenuImageTextEdit.getText();
   
   %serialString = %serialString SPC GeneralSettingsTable4MapTextEdit.getText();
   %serialString = %serialString SPC GeneralSettingsTable4MenuImageTextEdit.getText();
   
   %serialString = %serialString SPC GeneralSettingsTable5MapTextEdit.getText();
   %serialString = %serialString SPC GeneralSettingsTable5MenuImageTextEdit.getText();
   
   for (%i = 0; %i < GeneralSettingsSoundsArray.getCount(); %i++)
   {
      %serialString = %serialString SPC getAudioProfileFromSoundContainer(GeneralSettingsSoundsArray.getObject(%i));  
   }
 
   return %serialString;
}


// Asset selector callback functions

/// <summary>Callback from AssetLibrary.open</summary>
function GeneralSettingsFontSheetTextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}

/// <summary>Callback from AssetLibrary.open</summary>
function GeneralSettingsTable1MapTextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}

/// <summary>Callback from AssetLibrary.open</summary>
function GeneralSettingsTable2MapTextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}

/// <summary>Callback from AssetLibrary.open</summary>
function GeneralSettingsTable3MapTextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}

/// <summary>Callback from AssetLibrary.open</summary>
function GeneralSettingsTable4MapTextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}

/// <summary>Callback from AssetLibrary.open</summary>
function GeneralSettingsTable5MapTextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}

/// <summary>Callback from AssetLibrary.open</summary>
function GeneralSettingsSound1TextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}

/// <summary>Callback from AssetLibrary.open</summary>
function GeneralSettingsSound2TextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}

/// <summary>Callback from AssetLibrary.open</summary>
function GeneralSettingsSound3TextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}

/// <summary>Callback from AssetLibrary.open</summary>
function GeneralSettingsSound4TextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}

/// <summary>Callback from AssetLibrary.open</summary>
function GeneralSettingsSound5TextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}

/// <summary>Callback from AssetLibrary.open</summary>
function GeneralSettingsSound6TextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}

/// <summary>Callback from AssetLibrary.open</summary>
function GeneralSettingsSound7TextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}

/// <summary>Callback from AssetLibrary.open</summary>
function GeneralSettingsSound8TextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}

/// <summary>Callback from AssetLibrary.open</summary>
function GeneralSettingsSound9TextEdit::setSelectedAsset(%this, %asset)
{
   %this.setText(%asset);
}
