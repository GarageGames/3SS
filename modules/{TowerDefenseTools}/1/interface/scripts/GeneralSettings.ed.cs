//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$GeneralSettingsToolInitialized = false;

//--------------------------------
// General Settings Help
//--------------------------------
/// <summary>
/// This function opens a browser and displays the Tower Defense Template General
/// Settings help page.
/// </summary>
function GenSettingsHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/towerdefense/generalsettings/");
}


//-----------------------------------------------------------------------------
// General Settings Globals
//-----------------------------------------------------------------------------
$LevelFileSpec = "/data/levels/*.t2d";

/// <summary>
/// This function handles setting the tool's dirty state.
/// </summary>
/// <param name="dirty">Set to true if the tool's data needs to be saved, false if not.</param>
function SetGeneralToolDirtyState(%dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      GeneralSettingsWindow.setText("General Settings *");
   else
      GeneralSettingsWindow.setText("General Settings");
}

/// <summary>
/// This function handles opening GUI bitmap images.  Deprecated.
/// </summary>
/// <param name="preview">The preview window to assign the image to.</param>
function GenSettingsOpenPreviewImageFile(%preview)
{
    //echo(" -- opening image for " @ %preview);
   if (!$GameImageDir)
      $GameImageDir = $GameDir @ "/gui/images/";
   %dlg = new OpenFileDialog()
   {
      Filters = $T2D::ImageMapSpec;
      ChangePath = false;
      MustExist = true;
      MultipleFiles = true;
   };
   
   %dlg.DefaultPath = $GameImageDir;
      
   if(%dlg.Execute())
   {
      %fileName     = %dlg.files[0];
      %fileOnlyName = fileName( %fileName );         
      
      // [neo, 5/15/2007 - #3117]
      // If the image is already in a sub dir of images don't copy it just use
      // the same path and update the image map to use it.
      %checkPath    = expandPath( "^project/gui/images/" );
      %fileOnlyPath = filePath( expandPath( %fileName ) );
      %fileBasePath = getSubStr( %fileOnlyPath, 0, strlen( %checkPath ) );           

      if( %checkPath !$= %fileBasePath )         
      {                     
         %newFileLocation = expandPath("^project/gui/images/" @ %fileOnlyName );
         
         addResPath( filePath( %newFileLocation ) );
               
         pathCopy( %fileName, %newFileLocation );
      }            
      else
      {
         // Already exists in data/images or sub dir so just point to it
         %newFileLocation = %fileName;
      }
      
      // Error of some sort, skip it.
      if( !isFile( %newFileLocation ) )
      {
         %dlg.delete();
         return;
      }
      
      if (isObject(%preview))
         %preview.setBitmap(%fileName);
    //echo(" -- file name: " @ %fileName);
   }
   %dlg.delete();
}

//-----------------------------------------------------------------------------
// General Settings Tool
//-----------------------------------------------------------------------------
/// <summary>
/// This function registers the tool with the Asset Library so that it can react
/// to asset deletion.
/// </summary>
function GeneralSettings::onAdd(%this)
{
    AssetLibrary.registerTemplateToolForCallbacks(%this);
}

/// <summary>
/// This function initializes the General Settings Tool on wake.
/// </summary>
function GeneralSettings::onWake(%this)
{
   // load up our audio profiles for access from the tool.
   if (!$GameDir)
      $GameDir = LBProjectObj.gamePath;

   %this.clearLists();

   if (!$GeneralSettingsToolInitialized)
   {
      if (!isObject(mainMenuGui))
         exec($GameDir @ "/gui/mainMenu.gui");   
      if (!isObject(winGui))   
         exec($GameDir @ "/gui/win.gui");
      if (!isObject(loseGui))
         exec($GameDir @ "/gui/lose.gui");
      if (!isObject(levelSelectGui))
         exec($GameDir @ "/gui/levelSelect.gui");
         
      exec($GameDir @ "/scripts/AudioDescriptions.cs");
      exec($GameDir @ "/scripts/AudioProfiles.cs");
         
      $GeneralSettingsToolInitialized = true;
   }

   %levelFile = LBProjectObj.currentLevelFile;
   %this.levelPath = makeRelativePath( $GameDir, %levelFile );
   %this.imagePath = "gui/images/";

   %musicFile = GlobalBehaviorObject.callOnBehaviors("getMusic");
   %this.musicPath = filePath(%musicFile);

   GenSettingsTabBook.selectPage(0);

   %this.getLevelFileList();
   
   %this.populateLevelPane();
   
   %this.fontSheet = mainMenuGui.fontSheet;
   %this.titleMusic = mainMenuGui.music;
   %this.buttonSound = mainMenuGui.buttonSound;
   %this.winMusic = winGui.music;
   %this.loseMusic = loseGui.music;
   %this.towerPlace = mainMenuGui.towerPlace;
   %this.towerMisplace = mainMenuGui.towerMisplace;
   %this.towerUpgrade = mainMenuGui.towerUpgrade;
   %this.towerSell = mainMenuGui.towerSell;
   %this.damagedSound = GlobalBehaviorObject.callOnBehaviors("getDamageSound");
   
   GenSettingsTitleMusicEdit.text = %this.titleMusic;
   GenSettingsTitleMusicEdit.setActive(false);
   GenSettingsClickSoundEdit.text = %this.buttonSound;
   GenSettingsClickSoundEdit.setActive(false);
   GenSettingsFontSheetEdit.text = %this.fontSheet;
   GenSettingsFontSheetEdit.setActive(false);
   GenSettingsWinMusicEdit.text = %this.winMusic;
   GenSettingsWinMusicEdit.setActive(false);
   GenSettingsLoseMusicEdit.text = %this.loseMusic;
   GenSettingsLoseMusicEdit.setActive(false);
   GenSettingsPlacementEdit.text = %this.towerPlace;
   GenSettingsPlacementEdit.setActive(false);
   GenSettingsMisplacementEdit.text = %this.towerMisplace;
   GenSettingsMisplacementEdit.setActive(false);
   GenSettingsUpgradeEdit.text = %this.towerUpgrade;
   GenSettingsUpgradeEdit.setActive(false);
   GenSettingsSellEdit.text = %this.towerSell;
   GenSettingsSellEdit.setActive(false);
   GenSettingsDamageSoundEdit.text = %this.damagedSound;
   GenSettingsDamageSoundEdit.setActive(false);
}

/// <summary>
/// This function shuts down our playback if we close the tool while a sound is playing.
/// </summary>
function GeneralSettings::onSleep(%this)
{
   alxStopAll();
}

/// <summary>
/// This function handles deletion of a level file.  It checks for the deleted 
/// level in our list of levels for the game and removes it from the list if it
/// has been deleted.
/// </summary>
/// <param name="object">The name of the deleted asset, in this case our level file.</param>
/// <param name="type">The type of asset that was deleted.  If it is 'Level' then we have work to do.</param>
function GeneralSettings::onAssetDeleted(%this, %object, %type)
{
    if ($LoadedTemplate.Name !$= "TowerDefense")
        return;

    if (%type $= "Level")
    {
        %this.imagePath = "gui/images/";

        if (!$GameDir)
            $GameDir = LBProjectObj.gamePath;

        if (!isObject(levelSelectGui))
            exec($GameDir @ "/gui/levelSelect.gui");

        %this.getLevelFileList();
        %index = %this.findName(%object);
        echo(" -- level " @ %object @ " deleted.  Index : " @ %index);
        if (%index >= 0)
        {
            %this.removeLevel(%index);
            %this.saveLevelList();
        }
    }
}

/// <summary>
/// This function handles closing the General Settings Tool.
/// </summary>
function GeneralSettings::close(%this)
{
   if ($Tools::TemplateToolDirty)
   {
      ConfirmDialog.setupAndShow(GeneralSettingsWindow.getGlobalCenter(), "Save General Settings Changes?", 
            "Save", "GeneralSettings.saveSettings(); SetGeneralToolDirtyState(false); Canvas.popDialog(GeneralSettings);", 
            "Don't Save", "SetGeneralToolDirtyState(false); Canvas.popDialog(GeneralSettings);", 
            "Cancel", "");
   }
   else
      Canvas.popDialog(GeneralSettings);
}

/// <summary>
/// This function attaches the list of selected levels to the levelSelectGui and 
/// then saves the gui file.
/// </summary>
function GeneralSettings::saveLevelList(%this)
{
   for (%i = 0; %i < %this.LevelListSize; %i++)
   {
      levelSelectGui.LevelList[%i] = %this.LevelList[%i];
      levelSelectGui.LevelImageList[%i] = %this.imagePath @ fileName(%this.LevelImageList[%i]);
   }
   
   // Remove level settings that are no longer needed
   while (levelSelectGui.LevelList[%i] !$= "" && levelSelectGui.LevelImageList[%i] !$= "")
   {
      levelSelectGui.LevelList[%i] = "";
      levelSelectGui.LevelImageList[%i] = "";
      %i++;
   }
   GuiToolSaveGui(levelSelectGui);
}

/// <summary>
/// This function saves the general settings to the project.
/// </summary>
function GeneralSettings::saveSettings(%this)
{
   // Store current level settings
   for (%i = 0; %i < %this.LevelListSize; %i++)
   {
      levelSelectGui.LevelList[%i] = %this.LevelList[%i];
      levelSelectGui.LevelImageList[%i] = %this.imagePath @ fileName(%this.LevelImageList[%i]);
   }
   
   // Remove level settings that are no longer needed
   while (levelSelectGui.LevelList[%i] !$= "")
   {
      levelSelectGui.LevelList[%i] = "";
      levelSelectGui.LevelImageList[%i] = "";
      %i++;
   }
   
   mainMenuGui.fontSheet = %this.fontSheet;
   AddAssetToLevelDatablocks(%this.fontSheet);
   
   waveCountdownBitmapFont.setImageMap(%this.fontSheet);
   waveTotalBitmapFont.setImageMap(%this.fontSheet);
   waveCountSlashBitmapFont.setImageMap(%this.fontSheet);
   currentWaveBitmapFont.setImageMap(%this.fontSheet);
   scoreBitmapFont.setImageMap(%this.fontSheet);
   livesBitmapFont.setImageMap(%this.fontSheet);
   fundsBitmapFont.setImageMap(%this.fontSheet);

   mainMenuGui.buttonSound = %this.musicPath @ fileName(%this.buttonSound);
   mainMenuGui.music = %this.musicPath @ fileName(%this.titleMusic);
   loseGui.music = %this.musicPath @ fileName(%this.loseMusic);
   winGui.music = %this.musicPath @ fileName(%this.winMusic);
   mainMenuGui.towerPlace = %this.towerPlace;
   mainMenuGui.towerMisplace = %this.towerMisplace;
   mainMenuGui.towerUpgrade = %this.towerUpgrade;
   mainMenuGui.towerSell = %this.towerSell;
   
   GlobalBehaviorObject.callOnBehaviors("setDamageSound", %this.damagedSound);
   
   // call save gui function
   GuiToolSaveGui(mainMenuGui);
   GuiToolSaveGui(levelSelectGui);
   GuiToolSaveGui(winGui);
   GuiToolSaveGui(loseGui);
   
   LDEonApply();
   SaveAllLevelDatablocks();

   LBProjectObj.persistToDisk(true, true, true, true, true, true);
   
   SetGeneralToolDirtyState(false);
}

/// <summary>
/// This function clears the Level and Level Image lists.
/// </summary>
function GeneralSettings::clearLists(%this)
{
   for (%i = 0; %i < %this.LevelListSize; %i++)
   {
      %this.LevelList[%i] = "";
      %this.LevelImageList[%i] = "";
   }
   %this.LevelListSize = 0;
}

/// <summary>
/// This function populates the level display pane.
/// </summary>
function GeneralSettings::populateLevelPane(%this)
{
   // loop through levels folder and create preview panes for all levels
   GenSettingsLevelPane.clear();

   %count = 0;
   while ( %this.LevelList[%count] !$= "" )
   {
      //echo(" -- LevelList[" @ %count @ "] = " @ %this.LevelList[%count]);
      //echo(" -- LevelImageList[" @ %count @ "] = " @ %this.LevelImageList[%count]);
      %preview = %this.createPreviewPane(%count);
      %preview.LLPLevelFileEdit.text = %this.LevelList[%count];
      %preview.LLPLevelFileEdit.setActive(false);
      if (%this.LevelImageList[%count] !$= "")
      {
         %bitmap = $GameDir @ "/" @ %this.LevelImageList[%count];
      }
      else
      {
         %this.LevelImageList[%count] = "gui/images/levelIcon.png";
         %bitmap = $GameDir @ "/" @ "gui/images/levelIcon.png";
      }
      
      %preview.LLPLevelImageEdit.text = fileName(%bitmap);
      %preview.LLPLevelImageEdit.setActive(false);
      %preview.LLPIcon.isLegacyVersion = "0";
      %preview.LLPIcon.bitmapNormal = %bitmap;
      %preview.LLPLevelImageSelect.icon = %preview.LLPIcon;
      %preview.LLPLevelImageSelect.edit = %preview.LLPLevelImageEdit;
      
      GenSettingsLevelPane.add(%preview);
      
      %count++;
   }
   %this.LevelListSize = %count;
   
   %addPane = %this.createPreviewPane();
   GenSettingsLevelPane.add(%addPane);
   
   %position = GenSettingsLevelPane.Position;
   %width = GenSettingsLevelPane.colCount * 400;
   %rowCount = mFloor(((GenSettingsLevelPane.getCount() - 1) / 2)) + 1;
   GenSettingsLevelPane.resize(%position.x, %position.y, %width, %rowCount * 160);
   GenSettingsLevelPane.refresh();
   GenSettingsLevelScroller.scrollToTop();
}

/// <summary>
/// This function gets the level list off of the levelSelectGui and verifies that
/// the list only contains levels that actually exist.
/// </summary>
function GeneralSettings::getLevelFileList(%this)
{
    // get and clean the level file list from levelSelectGui
    %count = 0;
    while ( levelSelectGui.LevelList[%count] !$= "" )
    {
        // if the file exists, add it and its associated image to the clean list
        if (isFile($GameDir @ "/data/levels/" @ levelSelectGui.LevelList[%count] @ ".t2d"))
        {
            %levelList[%count] = levelSelectGui.LevelList[%count];
            %levelImageList[%count] = levelSelectGui.LevelImageList[%count];
        }
        else
        {
            // otherwise add a blank
            %levelList[%count] = "";
            %levelImageList[%count] = "";
        }
        %count++;
    }
    %k = 0;
    for (%i = 0; %i < %count; %i++)
    {
        // loop through and drop the blanks.
        if (%levelList[%i] !$= "")
        {
            %this.LevelList[%k] = %levelList[%i];
            %this.LevelImageList[%k] = %levelImageList[%i];
            %k++;
        }
    }
    %this.LevelListSize = %k;
}

/// <summary>
/// This function finds the specified name in the level list.
/// </summary>
/// <param name="name">The name of the level we're looking for.</param>
/// <return>The index of the name in the list, or -1 if the name is not in the list.</param>
function GeneralSettings::findName(%this, %name)
{
   for (%i = 0; %i < %this.LevelListSize; %i++)
   {
      if (%this.LevelList[%i] $= %name)
      {
         //echo(" -- Found " @ %name @ " at " @ %i);
         return %i;
      }
   }
   return -1;
}

/// <summary>
/// This function removes a level and its associated image from the lists attached
/// to the levelSelectGui.
/// </summary>
/// <param name="index">The index of the entry to remove from the list.</param>
function GeneralSettings::removeLevel(%this, %index)
{
    //echo(" -- removing " @ %this.LevelList[%index] @ " from play list");
    if (%index >= 0)
    {
        %this.LevelList[%index] = "";
        %this.LevelImageList[%index] = "";

        %i = %index;
        while (%this.LevelList[%i + 1] !$= "")
        {
            %this.LevelList[%i] = %this.LevelList[%index + 1];
            %this.LevelImageList[%i] = %this.LevelImageList[%index + 1];
            %index++;
        }
        %this.LevelListSize = %i;
        %this.LevelList[%i] = "";
        %this.LevelImageList[%i] = "";
    }
}

/// <summary>
/// This function dynamically creates a GUI control containing level name and a level 
/// select button image to be added to the level list display.
/// </summary>
/// <param name="index">The index of the list item used to create the pane.  This is also used in 
/// button callbacks to determine which pane we're dealing with.</param>
function GeneralSettings::createPreviewPane(%this, %index)
{
   %pane = new GuiControl() {
      canSaveDynamicFields = "0";
         isContainer = "1";
      Profile = "EditorPanelMedium";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "400 160";
      MinExtent = "8 2";
      canSave = "0";
      Visible = "1";
      hovertime = "1000";
         index = %index;
   };
   
   if (%index !$= "")
   {

      %pane.LLPLevelEditBg = new GuiControl() {
         canSaveDynamicFields = "0";
          isContainer = "0";
         Profile = "EditorPanelDark";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "131 45";
         Extent = "250 42";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
      };
      %pane.addGuiControl(%pane.LLPLevelEditBg);
      
      %pane.LLPLevelImageEditBg = new GuiControl() {
         canSaveDynamicFields = "0";
          isContainer = "0";
         Profile = "EditorPanelDark";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "131 94";
         Extent = "250 42";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
      };
      %pane.addGuiControl(%pane.LLPLevelImageEditBg);
      
      %pane.LLPLevelEditText = new GuiTextCtrl() {
         canSaveDynamicFields = "0";
          isContainer = "0";
         Profile = "GuiTextProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "139 44";
         Extent = "104 18";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         text = "Select Level to Use";
         maxLength = "1024";
      };
      %pane.addGuiControl(%pane.LLPLevelEditText);
      
      %pane.LLPLevelImageText = new GuiTextCtrl() {
         canSaveDynamicFields = "0";
          isContainer = "0";
         Profile = "GuiTextProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "139 94";
         Extent = "104 18";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         text = "Select Image to Use";
         maxLength = "1024";
      };
      %pane.addGuiControl(%pane.LLPLevelImageText);
      
      %pane.LLPLevelSelect = new GuiButtonCtrl() {
         canSaveDynamicFields = "0";
          class = "LLPLevelSelectButton";
         isContainer = "0";
         Profile = "GuiButtonProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "310 58";
         Extent = "64 24";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         text = "Select";
         groupNum = "-1";
         buttonType = "PushButton";
         useMouseEvents = "0";
            index = %index;
      };
      %pane.addGuiControl(%pane.LLPLevelSelect);
      
      %pane.LLPIcon = new GuiBitmapButtonCtrl() {
         canSaveDynamicFields = "0";
          class = "LLPIconButton";
         isContainer = "0";
         Profile = "GuiDefaultProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "18 40";
         Extent = "100 100";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         groupNum = "-1";
         buttonType = "PushButton";
         useMouseEvents = "0";
         bitmap = "";
            index = %index;
      };
      %pane.addGuiControl(%pane.LLPIcon);
      
      %pane.LLPLevelFileEdit = new GuiTextEditCtrl() {
         canSaveDynamicFields = "0";
          isContainer = "0";
         Profile = "GuiTextEditProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "138 58";
         Extent = "166 24";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         maxLength = "1024";
         historySize = "0";
         password = "0";
         tabComplete = "0";
         sinkAllKeyEvents = "0";
         password = "0";
         passwordMask = "*";
            index = %index;
      };
      %pane.addGuiControl(%pane.LLPLevelFileEdit);
      
      %pane.LLPLevelImageEdit = new GuiTextEditCtrl() {
         canSaveDynamicFields = "0";
          isContainer = "0";
         Profile = "GuiTextEditProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "138 108";
         Extent = "166 24";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         maxLength = "1024";
         historySize = "0";
         password = "0";
         tabComplete = "0";
         sinkAllKeyEvents = "0";
         password = "0";
         passwordMask = "*";
            index = %index;
      };
      %pane.addGuiControl(%pane.LLPLevelImageEdit);

      %pane.LLPLevelImageSelect = new GuiButtonCtrl() {
         canSaveDynamicFields = "0";
          class = "LLPImageSelectBtn";
         isContainer = "0";
         Profile = "GuiButtonProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "310 108";
         Extent = "64 24";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         text = "Select";
         groupNum = "-1";
         buttonType = "PushButton";
         useMouseEvents = "0";
            index = %index;
      };
      %pane.addGuiControl(%pane.LLPLevelImageSelect);

      %pane.LLPRemoveButton = new GuiBitmapButtonCtrl() {
         canSaveDynamicFields = "0";
          class = "LLPRemoveBtn";
         isContainer = "0";
         Profile = "GuiDefaultProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "357 16";
         Extent = "24 24";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         groupNum = "-1";
         buttonType = "PushButton";
         useMouseEvents = "1";
         isLegacyVersion = "0";
         bitmap = "templates/commoninterface/gui/images/removeButton_normal.png";
         bitmapNormal = "templates/commoninterface/gui/images/removeButton_normal.png";
         bitmapHilight = "templates/commoninterface/gui/images/removeButton_highlight.png";
         bitmapDepressed = "templates/commoninterface/gui/images/removeButton_depressed.png";
         bitmapInactive = "templates/commoninterface/gui/images/removeButton_inactive.png";
            index = %index;
      };
      %pane.addGuiControl(%pane.LLPRemoveButton);
   }
   else
   {
      %pane.LLPIcon = new GuiBitmapButtonCtrl() {
         canSaveDynamicFields = "0";
          class = "LLPAddButton";
         isContainer = "0";
         Profile = "GuiDefaultProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "18 40";
         Extent = "100 100";
         MinExtent = "8 2";
         canSave = "0";
         Visible = "1";
         hovertime = "1000";
         groupNum = "-1";
         buttonType = "PushButton";
         useMouseEvents = "0";
            index = %index;
         isLegacyVersion = "0";
         bitmap = "templates/TowerDefense/interface/gui/images/addLevel";
         bitmapNormal = "templates/TowerDefense/interface/gui/images/addLevel";
      };
      %pane.addGuiControl(%pane.LLPIcon);
   }
   
   return %pane;
}

/// <summary>
/// This function opens the Asset Library to select a level for the pane that
/// this callback was triggered from.
/// </summary>
function LLPLevelSelectButton::onClick(%this)
{
   GeneralSettings.tempLevel = GeneralSettings.LevelList[%this.index];
   GeneralSettings.tempImage = GeneralSettings.LevelImageList[%this.index];
   AssetLibrary.open(%this, $LevelsPage);
}

/// <summary>
/// This function assigns the level selected in the Asset Library to the pane
/// from which the select callback was triggered from.
/// </summary>
/// <param name="asset">The returned level file name from the Asset Library.</param>
function LLPLevelSelectButton::setSelectedAsset(%this, %asset)
{
    if (%asset $= GeneralSettings.tempLevel)
        return;

    %index = GeneralSettings.findName(%asset);
    //echo(" -- Adding " @ %asset @ " to file list");
    if (%index >= 0)
    {
        //echo(" -- Found duplicate at " @ %index @ ", swapping with " @ %this.index);
        GeneralSettings.LevelList[%this.index] = GeneralSettings.LevelList[%index];
        GeneralSettings.LevelImageList[%this.index] = GeneralSettings.LevelImageList[%index];

        GeneralSettings.LevelList[%index] = GeneralSettings.tempLevel;
        GeneralSettings.LevelImageList[%index] = GeneralSettings.tempImage;
    }
    else
        GeneralSettings.LevelList[%this.index] = %asset;

    SetGeneralToolDirtyState(true);
    GeneralSettings.populateLevelPane();
}

/// <summary>
/// This function opens the Asset Library to select an image for the level in the
/// pane from which the callback was triggered.
/// </summary>
function LLPImageSelectBtn::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the image from the Asset Library to the level image slot in 
/// the pane from which the select callback was triggered.
/// </summary>
/// <param name="asset">The image file name returned from the Asset Library.</param>
function LLPImageSelectBtn::setSelectedAsset(%this, %asset)
{
    %this.icon.setBitmap(expandPath("^project/gui/images/") @ %asset);
    GeneralSettings.LevelImageList[%this.index] = GeneralSettings.imagePath @ %asset;
    %this.edit.text = fileName(%this.icon.bitmap);
   SetGeneralToolDirtyState(true);
}

function LLPIconButton::onClick(%this)
{
   //echo(" -- LLPIconButton::onClick() - " @ %this.index);
}

/// <summary>
/// This function opens the Asset Library to select a new level to add to the
/// game's level list.
/// </summary>
function LLPAddButton::onClick(%this)
{
   AssetLibrary.open(%this, $LevelsPage);
}

/// <summary>
/// This function assigns the level to the next available slot in the level list.
/// </summary>
/// <param name="asset">The name of the level selected in the Asset Library.</param>
function LLPAddButton::setSelectedAsset(%this, %asset)
{
    %index = GeneralSettings.findName(%asset);
    if (%index >= 0)
    {
        //echo(" -- Found duplicate at " @ %index @ ", moving file to end of list : size - " @ GeneralSettings.LevelListSize);
        %size = GeneralSettings.LevelListSize;
        %tempLevel = GeneralSettings.LevelList[%index];
        %tempImage = GeneralSettings.LevelImageList[%index];

        for (%i = %index; %i < %size; %i++)
        {
            GeneralSettings.LevelList[%i] = GeneralSettings.LevelList[%i + 1];
            GeneralSettings.LevelImageList[%i] = GeneralSettings.LevelImageList[%i + 1];
        }
        GeneralSettings.LevelList[%size - 1] = %tempLevel;
        GeneralSettings.LevelImageList[%size - 1] = %tempImage;
    }
    else
    {
        %index = (GeneralSettings.LevelListSize);
        //echo(" @@@ " @ %index @ " levels in list - adding " @ %asset);

        GeneralSettings.LevelList[%index] = %asset;
        SetGeneralToolDirtyState(true);
    }

    GeneralSettings.populateLevelPane();
}

/// <summary>
/// This function removes the level in the pane from which this callback was triggered
/// from the game's level list.
/// </summary>
function LLPRemoveBtn::onClick(%this)
{
   GeneralSettings.removeLevel(%this.index);

   SetGeneralToolDirtyState(true);
   GeneralSettings.populateLevelPane();
}

/// <summary>
/// This function opens the Asset Library to select a new bitmap font sheet.
/// </summary>
function GenSettingsFontSheetBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected bitmap font sheet to the game.
/// </summary>
/// <param name="asset">The name of the bitmap font sheet from the Asset Library.</param>
function GenSettingsFontSheetBrowse::setSelectedAsset(%this, %asset)
{
   //echo("@@@ Got " @ %asset @ " back from the Asset Library");
   GeneralSettings.fontSheet = %asset;
   GenSettingsFontSheetEdit.text = GeneralSettings.fontSheet;
   SetGeneralToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an audio profile for the game's 
/// title music.
/// </summary>
function GenSettingsTitleMusicSelectBtn::onClick(%this)
{
   AssetLibrary.open(%this, $SoundsPage);
}

/// <summary>
/// This function assigns the title music audio profile
/// </summary>
/// <param name="asset">The name of the audio profile returned from the Asset Library.</param>
function GenSettingsTitleMusicSelectBtn::setSelectedAsset(%this, %asset)
{
   //echo("@@@ Got " @ %asset @ " back from the Asset Library");
   alxStopAll();
   GeneralSettings.titleMusic = %asset;
   GenSettingsTitleMusicEdit.text = GeneralSettings.titleMusic;
   SetGeneralToolDirtyState(true);
}

/// <summary>
/// This function plays the title music audio profile
/// </summary>
function GenSettingsTitleMusicPlayButton::onClick(%this)
{
   // temped in a sound file to see if this works
   // have to exec("scripts/game/audio.cs"); from the console in the editor.
   alxStopAll();
   %temp = alxPlay(GeneralSettings.titleMusic);
   //echo(" -- Play button clicked, handle : " @ %temp);
}

/// <summary>
/// This function stops the title music audio profile
/// </summary>
function GenSettingsTitleMusicStopButton::onClick(%this)
{
   alxStopAll();
}

/// <summary>
/// This function opens the Asset Library to select an audio profile for the game's 
/// button sounds.
/// </summary>
function GenSettingsClickSoundSelectButton::onClick(%this)
{
   AssetLibrary.open(%this, $SoundsPage);
}

/// <summary>
/// This function assigns the button sounds audio profile
/// </summary>
/// <param name="asset">The name of the audio profile returned from the Asset Library.</param>
function GenSettingsClickSoundSelectButton::setSelectedAsset(%this, %asset)
{
   //echo("@@@ Got " @ %asset @ " back from the Asset Library");
   alxStopAll();
   GeneralSettings.buttonSound = %asset;
   GenSettingsClickSoundEdit.text = GeneralSettings.buttonSound;
   SetGeneralToolDirtyState(true);
}

/// <summary>
/// This function plays the button sounds audio profile
/// </summary>
function GenSettingsClickSoundPlayButton::onClick(%this)
{
   // temped in a sound file to see if this works
   // have to exec("scripts/game/audio.cs"); from the console in the editor.
   alxStopAll();
   %temp = alxPlay(GeneralSettings.buttonSound);
   //echo(" -- Play button clicked, handle : " @ %temp);
}

/// <summary>
/// This function stops the button sounds audio profile
/// </summary>
function GenSettingsClickSoundStopButton::onClick(%this)
{
   alxStopAll();
}

/// <summary>
/// This function opens the Asset Library to select an audio profile for the game's 
/// win music.
/// </summary>
function GenSettingsWinMusicSelectButton::onClick(%this)
{
   AssetLibrary.open(%this, $SoundsPage);
}

/// <summary>
/// This function assigns the win music audio profile
/// </summary>
/// <param name="asset">The name of the audio profile returned from the Asset Library.</param>
function GenSettingsWinMusicSelectButton::setSelectedAsset(%this, %asset)
{
   //echo("@@@ Got " @ %asset @ " back from the Asset Library");
   alxStopAll();
   GeneralSettings.winMusic = %asset;
   GenSettingsWinMusicEdit.text = GeneralSettings.winMusic;
   SetGeneralToolDirtyState(true);
}

/// <summary>
/// This function plays the win music audio profile
/// </summary>
function GenSettingsWinMusicPlayButton::onClick(%this)
{
   // temped in a sound file to see if this works
   // have to exec("scripts/game/audio.cs"); from the console in the editor.
   alxStopAll();
   %temp = alxPlay(GeneralSettings.winMusic);
   //echo(" -- Play button clicked, handle : " @ %temp);
}

/// <summary>
/// This function stops the win music audio profile
/// </summary>
function GenSettingsWinMusicStopButton::onClick(%this)
{
   alxStopAll();
}

/// <summary>
/// This function opens the Asset Library to select an audio profile for the game's 
/// lose music.
/// </summary>
function GenSettingsLoseMusicSelectButton::onClick(%this)
{
   AssetLibrary.open(%this, $SoundsPage);
}

/// <summary>
/// This function assigns the lose music audio profile
/// </summary>
/// <param name="asset">The name of the audio profile returned from the Asset Library.</param>
function GenSettingsLoseMusicSelectButton::setSelectedAsset(%this, %asset)
{
   //echo("@@@ Got " @ %asset @ " back from the Asset Library");
   alxStopAll();
   GeneralSettings.loseMusic = %asset;
   GenSettingsLoseMusicEdit.text = GeneralSettings.loseMusic;
   SetGeneralToolDirtyState(true);
}

/// <summary>
/// This function plays the lose music audio profile
/// </summary>
function GenSettingsLoseMusicPlayButton::onClick(%this)
{
   // temped in a sound file to see if this works
   // have to exec("scripts/game/audio.cs"); from the console in the editor.
   alxStopAll();
   %temp = alxPlay(GeneralSettings.loseMusic);
   //echo(" -- Play button clicked, handle : " @ %temp);
}

/// <summary>
/// This function stops the lose music audio profile
/// </summary>
function GenSettingsLoseMusicStopButton::onClick(%this)
{
   alxStopAll();
}

// GenSettingsPlacementSelectBtn
/// <summary>
/// This function opens the Asset Library to select an audio profile for the game's 
/// placement sound.
/// </summary>
function GenSettingsPlacementSelectBtn::onClick(%this)
{
   AssetLibrary.open(%this, $SoundsPage);
}

/// <summary>
/// This function assigns the placement sound audio profile
/// </summary>
/// <param name="asset">The name of the audio profile returned from the Asset Library.</param>
function GenSettingsPlacementSelectBtn::setSelectedAsset(%this, %asset)
{
   //echo("@@@ Got " @ %asset @ " back from the Asset Library");
   alxStopAll();
   GeneralSettings.towerPlace = %asset;
   GenSettingsPlacementEdit.text = GeneralSettings.towerPlace;
   SetGeneralToolDirtyState(true);
}

/// <summary>
/// This function plays the placement sound audio profile
/// </summary>
function GenSettingsPlacementPlayButton::onClick(%this)
{
   // temped in a sound file to see if this works
   // have to exec("scripts/game/audio.cs"); from the console in the editor.
   alxStopAll();
   %temp = alxPlay(GeneralSettings.towerPlace);
   //echo(" -- Play button clicked, handle : " @ %temp);
}

/// <summary>
/// This function stops the placement sound audio profile
/// </summary>
function GenSettingsPlacementStopButton::onClick(%this)
{
   alxStopAll();
}

// GenSettingsMisplacementSelectBtn
/// <summary>
/// This function opens the Asset Library to select an audio profile for the game's 
/// misplacement sound.
/// </summary>
function GenSettingsMisplacementSelectBtn::onClick(%this)
{
   AssetLibrary.open(%this, $SoundsPage);
}

/// <summary>
/// This function assigns the misplacement sound audio profile
/// </summary>
/// <param name="asset">The name of the audio profile returned from the Asset Library.</param>
function GenSettingsMisplacementSelectBtn::setSelectedAsset(%this, %asset)
{
   //echo("@@@ Got " @ %asset @ " back from the Asset Library");
   alxStopAll();
   GeneralSettings.towerMisplace = %asset;
   GenSettingsMisplacementEdit.text = GeneralSettings.towerMisplace;
   SetGeneralToolDirtyState(true);
}

/// <summary>
/// This function plays the misplacement sound audio profile
/// </summary>
function GenSettingsMisplacementPlayButton::onClick(%this)
{
   // temped in a sound file to see if this works
   // have to exec("scripts/game/audio.cs"); from the console in the editor.
   alxStopAll();
   %temp = alxPlay(GeneralSettings.towerMisplace);
   //echo(" -- Play button clicked, handle : " @ %temp);
}

/// <summary>
/// This function stops the misplacement sound audio profile
/// </summary>
function GenSettingsMisplacementStopButton::onClick(%this)
{
   alxStopAll();
}

// GenSettingsUpgradeSelectBtn
/// <summary>
/// This function opens the Asset Library to select an audio profile for the game's 
/// upgrade sound.
/// </summary>
function GenSettingsUpgradeSelectBtn::onClick(%this)
{
   AssetLibrary.open(%this, $SoundsPage);
}

/// <summary>
/// This function assigns the upgrade sound audio profile
/// </summary>
/// <param name="asset">The name of the audio profile returned from the Asset Library.</param>
function GenSettingsUpgradeSelectBtn::setSelectedAsset(%this, %asset)
{
   //echo("@@@ Got " @ %asset @ " back from the Asset Library");
   alxStopAll();
   GeneralSettings.towerUpgrade = %asset;
   GenSettingsUpgradeEdit.text = GeneralSettings.towerUpgrade;
   SetGeneralToolDirtyState(true);
}

/// <summary>
/// This function plays the upgrade sound audio profile
/// </summary>
function GenSettingsUpgradePlayButton::onClick(%this)
{
   // temped in a sound file to see if this works
   // have to exec("scripts/game/audio.cs"); from the console in the editor.
   alxStopAll();
   %temp = alxPlay(GeneralSettings.towerUpgrade);
   //echo(" -- Play button clicked, handle : " @ %temp);
}

/// <summary>
/// This function stops the upgrade sound audio profile
/// </summary>
function GenSettingsUpgradeStopButton::onClick(%this)
{
   alxStopAll();
}

// GenSettingsSellSelectBtn
/// <summary>
/// This function opens the Asset Library to select an audio profile for the game's 
/// sell sound.
/// </summary>
function GenSettingsSellSelectBtn::onClick(%this)
{
   AssetLibrary.open(%this, $SoundsPage);
}

/// <summary>
/// This function assigns the sell sound audio profile
/// </summary>
/// <param name="asset">The name of the audio profile returned from the Asset Library.</param>
function GenSettingsSellSelectBtn::setSelectedAsset(%this, %asset)
{
   //echo("@@@ Got " @ %asset @ " back from the Asset Library");
   alxStopAll();
   GeneralSettings.towerSell = %asset;
   GenSettingsSellEdit.text = GeneralSettings.towerSell;
   SetGeneralToolDirtyState(true);
}

/// <summary>
/// This function plays the sell sound audio profile
/// </summary>
function GenSettingsSellPlayButton::onClick(%this)
{
   // temped in a sound file to see if this works
   // have to exec("scripts/game/audio.cs"); from the console in the editor.
   alxStopAll();
   %temp = alxPlay(GeneralSettings.towerSell);
   //echo(" -- Play button clicked, handle : " @ %temp);
}

/// <summary>
/// This function stops the sell sound audio profile
/// </summary>
function GenSettingsSellStopButton::onClick(%this)
{
   alxStopAll();
}

/// <summary>
/// This function opens the Asset Library to select an audio profile for the game's 
/// damage player sound.
/// </summary>
function GenSettingsDamageSoundSelectBtn::onClick(%this)
{
   AssetLibrary.open(%this, $SoundsPage);
}

/// <summary>
/// This function assigns the damage player sound audio profile
/// </summary>
/// <param name="asset">The name of the audio profile returned from the Asset Library.</param>
function GenSettingsDamageSoundSelectBtn::setSelectedAsset(%this, %asset)
{
   //echo("@@@ Got " @ %asset @ " back from the Asset Library");
   alxStopAll();
   GeneralSettings.damagedSound = %asset;
   GenSettingsDamageSoundEdit.text = GeneralSettings.damagedSound;
   SetGeneralToolDirtyState(true);
}

/// <summary>
/// This function plays the damage player sound audio profile
/// </summary>
function GenSettingsDamageSoundPlayButton::onClick(%this)
{
   // temped in a sound file to see if this works
   // have to exec("scripts/game/audio.cs"); from the console in the editor.
   alxStopAll();
   %temp = alxPlay(GeneralSettings.damagedSound);
   echo(" -- Play button clicked, handle : " @ %temp);
}

/// <summary>
/// This function stops the damage player sound audio profile
/// </summary>
function GenSettingsDamageSoundStopButton::onClick(%this)
{
   alxStopAll();
}

/// <summary>
/// This function stops all playing sounds on tab selection.
/// </summary>
function GenSettingsTabBook::onTabSelected(%this)
{
    alxStopAll();
}