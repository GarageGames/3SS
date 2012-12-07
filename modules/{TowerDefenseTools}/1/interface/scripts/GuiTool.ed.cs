//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------



//--------------------------------
// Interface Tool Help
//--------------------------------
/// <summary>
/// This function opens a browser and navigates to the Tower Defense Template 
/// Interface Tool help page.
/// </summary>
function GuiToolHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/towerdefense/interface/");
}


//-----------------------------------------------------------------------------
// GUI Tool Globals
//-----------------------------------------------------------------------------

$GuiToolInitialized = false;
$GameDir = "";
$GameGuiDir = "";
$T2D::TextFileSpec = "Text Files (*.txt)|*.txt|";
$HowToPlayScreens = 0;

/// <summary>
/// This function handles the tool's dirty state to keep track of the need to save data.
/// </summary>
/// <param name="dirty">True if data has been changed, false if not.</param>
function SetGuiToolDirtyState(%dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      GuiToolWindow.setText("Interface Tool *");
   else
      GuiToolWindow.setText("Interface Tool");
}

/// <summary>
/// This function saves a gui file to disk.
/// </summary>
/// <param name="guiObject">The GUI object to save to disk.</param>
function GuiToolSaveGui(%guiObject)
{
   %currentObject = %guiObject;
   
   if( %currentObject == -1 )
      return;

   %currentObject.canSaveDynamicFields = "1";
   
   if( %currentObject.getName() !$= "" )
      %name =  %currentObject.getName() @ ".gui";
   else
      %name = "Untitled.gui";
      
   %currentFile = %currentObject.getScriptFile();
   //if( %currentFile $= GuiEditor.blankgui.getScriptFile())
      //%currentFile = "";
   
   // get the filename
   %filename = %currentFile;
   
   if(%filename $= "")
      return;
      
   // Save the Gui
   if( isWriteableFileName( %filename ) )
   {
      //
      // Extract any existent TorqueScript before writing out to disk
      //
      %fileObject = new FileObject();
      %fileObject.openForRead( %filename );      
      %lines = 0;
      %skipLines = true;
      while( !%fileObject.isEOF() )
      {
         %line = %fileObject.readLine();
         if( %line $= "//--- OBJECT WRITE BEGIN ---" )
            %skipLines = true;
         else if( %line $= "//--- OBJECT WRITE END ---" )
            %skipLines = false;
         else if( %skipLines == false )
            %newFileLines[ %lines++ ] = %line;
      }      
      %fileObject.close();
      %fileObject.delete();
     
      %fo = new FileObject();
      %fo.openForWrite(%filename);
      %fo.writeLine("//--- OBJECT WRITE BEGIN ---");
      %fo.writeObject(%currentObject, "%guiContent = ");
      %fo.writeLine("//--- OBJECT WRITE END ---");
      
      // Write out captured TorqueScript below Gui object
      for( %i = 1; %i <= %lines; %i++ )
         %fo.writeLine( %newFileLines[ %i ] );
               
      %fo.close();
      %fo.delete();
      
      // set the script file name
      %currentObject.setScriptFile(%filename);
      
      // Clear the blank gui if we save it
      if( GuiEditor.blankGui == %currentObject )
         GuiEditor.blankGui = new GuiControl();
         
      $GuiDirty = false;
      
      if (isFile(%filename @ ".dso"))
         fileDelete(%filename @ ".dso");
   }
   else
      MessageBox("Torque Game Builder", "There was an error writing to file '" @ %currentFile @ "'. The file may be read-only.", "Ok", "Error" );
   
}

/// <summary>
/// This function gets a relative file name based on a fully qualified filename.
/// </summary>
/// <param name="fileName">The name of the file to find a relative path for.</param>
function GuiToolGetRelativeFileName(%fileName)
{
   %tempPath = filePath(%fileName);
   %relPath = makeRelativePath(%tempPath, $GameDir);
   return %relPath @ "/" @ fileName(%fileName);
}

/// <summary>
/// This function opens an image file to assign to a set of controls that share
/// a 'field name' - a common name portion from which the control names can be
/// generated.
/// </summary>
/// <param name="fieldName">The common portion of the control names to update.</param>
/// <example>
/// If you pass in GUIMenuTabTitle, the GUIMenuTabTitlePreview control will be assigned the image, 
/// the GUIMenuTabTitleEdit control will be assigned the name of the image and the 
/// GUIMenuTabTitleDropdown control will have a field added with the image name attached for 
/// button state selection.
/// </example>
function GUIToolOpenImageFile(%fieldName)
{
   // %fieldName should be the field that the image will be attached to.  For
   // example, pass in GUIMenuTabTitle to get image and file name for GUIMenuTabTitlePreview
   // and GUIMenuTabTitleEdit objects from the GUIMenuTabTitleBrowse button onClick() event.
   %previewImage = %fieldName @ "Preview";
   %textEdit = %fieldName @ "Edit";
   %dropdown = %fieldName @ "Dropdown";
   if (isObject(%dropdown))
      %dropState = %dropdown.getText();
   
   //echo(" -- Preview : " @ %previewImage);
   //echo(" -- Edit : " @ %textEdit);
   //echo(" -- Dropdown : " @ %dropdown @ " : " @ %dropState);
   
   if (!$GameGuiDir)
      $GameGuiDir = $GameDir @ "/gui/images/";
   %dlg = new OpenFileDialog()
   {
      Filters = $T2D::ImageMapSpec;
      ChangePath = false;
      MustExist = true;
      MultipleFiles = true;
   };
   
   %dlg.DefaultPath = $GameGuiDir;
      
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
      
      if (isObject(%dropdown))
      {
         /*
         bitmapNormal = ""; - Up
         bitmapHilight = ""; - Over
         bitmapDepressed = ""; - Down
         */
         %imageField = "";
         switch$(%dropState)
         {
            case "Up":
               %previewImage.setBitmap(%fileName);
               %previewImage.BitmapNormal = %fileName;
               %dropdown.bitmapNormal = %fileName;
               
            case "Over":
               %previewImage.setBitmap(%fileName);
               %previewImage.BitmapHilight = %fileName;
               %dropdown.bitmapHilight = %fileName;
               
            case "Down":
               %previewImage.setBitmap(%fileName);
               %previewImage.BitmapDepressed = %fileName;
               %dropdown.bitmapDepressed = %fileName;
         }
      }
      else
      {
         %previewImage.bitmap = %fileName;
      }
            
      %textEdit.text = fileName(%fileName);
      %dlg.delete();
      return true;
   }
   else
   {
       %dlg.delete();
       return false;
   }
}

/// <summary>
/// This function opens a text file for use in a text field
/// </summary>
/// <param name="fieldName">The name of the text edit control to assign the text file's contents to.</param>
function GUIToolOpenTextFile(%fieldName)
{
   // %fieldName should be the name of the text edit box to populate.
   %textEdit = %fieldName;
   
   if (!$GameGuiDir)
      $GameGuiDir = $GameDir @ "/gui/";
   %dlg = new OpenFileDialog()
   {
      Filters = $T2D::TextFileSpec;
      ChangePath = false;
      MustExist = true;
      MultipleFiles = true;
   };
   
   %dlg.DefaultPath = $GameGuiDir;
      
   if(%dlg.Execute())
   {
      %fileName     = %dlg.files[0];
      %fileOnlyName = fileName( %fileName );         
      
      // [neo, 5/15/2007 - #3117]
      // If the image is already in a sub dir of images don't copy it just use
      // the same path and update the image map to use it.
      %checkPath    = expandPath( "^project/gui/" );
      %fileOnlyPath = filePath( expandPath( %fileName ) );
      %fileBasePath = getSubStr( %fileOnlyPath, 0, strlen( %checkPath ) );           

      if( %checkPath !$= %fileBasePath )         
      {                     
         %newFileLocation = expandPath("^project/gui/" @ %fileOnlyName );
         
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
      
      %textEdit.text = %fileName;
   }
   %dlg.delete();
}

/// <summary>
/// This function clears and refreshes the contents of the passed dropdown control.
/// </summary>
/// <param name="dropDown">The control to refresh.</param>
function GUIToolRefreshStateDropdown(%dropDown)
{
   %dropDown.clear();
   %dropDown.add("Up", 0);
   %dropDown.add("Over", 1);
   %dropDown.add("Down", 2);
   
   %dropDown.setFirstSelected();
}

/// <summary>
/// This function clears and refreshes the contents of the passed dropdown control.
/// </summary>
/// <param name="dropDown">The control to refresh.</param>
function GUIToolSetupNumHelpScreensDropdown(%dropDown)
{
   %dropDown.clear();
   %dropDown.add("1", 0);
   %dropDown.add("2", 1);
   %dropDown.add("3", 2);
   %dropDown.add("4", 3);
   %dropDown.add("5", 4);
   %dropDown.add("6", 5);
   
   %screenCount = howToPlayGui.numHowToPlayScreens;
   %dropDown.setSelected(%screenCount > -1 ? (%screenCount - 1) : 0);
}

//-----------------------------------------------------------------------------
// GUI Tool
//-----------------------------------------------------------------------------
/// <summary>
/// This function initializes the Interface Tool when it wakes.
/// </summary>
function GuiTool::onWake(%this)
{
    $GameDir = "^project/";
    %this.guiDir = $GameDir @ "gui/";
    %this.guiImageDir = %this.guiDir @ "images/";
    %this.guiImageRelDir = "gui/images/";

    // load up all of our gui's so we can access the relevant resources
    if (!isObject(mainMenuGui))
      exec($GameDir @ "gui/mainMenu.gui"); 
    if (!isObject(mainScreenGui))
      exec($GameDir @ "gui/mainScreen.gui");
    if (!isObject(winGui))
      exec($GameDir @ "gui/win.gui");
    if (!isObject(loseGui))
      exec($GameDir @ "gui/lose.gui");
    if (!isObject(pauseGui))
      exec($GameDir @ "gui/pause.gui");
    if (!isObject(creditsGui))
      exec($GameDir @ "gui/credits.gui");
    if (!isObject(howToPlayGui))
      exec($GameDir @ "gui/howToPlay.gui");
    if (!isObject(levelSelectGui))
      exec($GameDir @ "gui/levelSelect.gui");

    // set up our tabs
    %this.initMenuTab();
    %this.initLevelSelectTab();
    %this.initLevelTab();
    %this.initWinTab();
    %this.initPauseTab();
    %this.initHelpTab();
    %this.initCreditsTab();

    // start at the beginning....
    GUITabBook.selectPage(0);

    $HowToPlayScreens = howToPlayGui.numHowToPlayScreens;

    $GuiToolInitialized = true;
    %this.awake = true;
    SetGuiToolDirtyState(false);
}

/// <summary>
/// This function handles some admin when the tool is put to sleep.
/// </summary>
function GuiTool::onSleep(%this)
{
    $GuiToolInitialized = false;
    %this.awake = false;
}

/// <summary>
/// This function assigns an asset from the Asset Library to a set of controls determined
/// by a common name element.
/// </summary>
/// <param name="asset">The asset returned from the Asset Library.</param>
/// <param name="fieldName">The common portion of the control names to update.</param>
/// <example>
/// If you pass in GUIMenuTabTitle, the GUIMenuTabTitlePreview control will be assigned the image, 
/// the GUIMenuTabTitleEdit control will be assigned the name of the image and the 
/// GUIMenuTabTitleDropdown control will have a field added with the image name attached for 
/// button state selection.
/// </example>
function GuiTool::assignAsset(%this, %asset, %fieldName)
{
    %imageFile = expandPath("^project/gui/images/") @ %asset;

    %previewImage = %fieldName @ "Preview";
    %textEdit = %fieldName @ "Edit";
    %dropdown = %fieldName @ "Dropdown";

    if (isObject(%dropdown))
    {
        %dropState = %dropdown.getText();
        %imageField = "";
        switch$(%dropState)
        {
        case "Up":
            %previewImage.setBitmap(%imageFile);
            %previewImage.bitmapNormal = %imageFile;
            %dropdown.bitmapNormal = %imageFile;

        case "Over":
            %previewImage.setBitmap(%imageFile);
            %previewImage.bitmapHilight = %imageFile;
            %dropdown.bitmapHilight = %imageFile;

        case "Down":
            %previewImage.setBitmap(%imageFile);
            %previewImage.bitmapDepressed = %imageFile;
            %dropdown.bitmapDepressed = %imageFile;
        }
    }
    else
    {
        %previewImage.bitmap = %imageFile;
    }

    %textEdit.text = %asset;
}

/// <summary>
/// This function assigns an asset from the Asset Library to the selected help
/// screen slot.
/// </summary>
/// <param name="asset">The asset name returned from the Asset Library.</param>
/// <param name="fieldName">The common portion of the controls that will be updated.</param>
/// <return>Returns the relative filename of the image file from the asset.</return>
function GuiTool::assignHelpScreen(%this, %asset, %fieldName)
{
    %imageFile = expandPath("^project/gui/images/") @ %asset;

    %previewImage = %fieldName @ "Preview";
    %textEdit = %fieldName @ "Edit";
    %previewImage.bitmap = %imageFile;
    %textEdit.text = %asset;
    return GuiToolGetRelativeFileName(%previewImage.bitmap);
}

/// <summary>
/// This function sets the button image preview to the image for its button state
/// as selected by the passed dropdown control.
/// </summary>
/// <param name="dropdown">The dropdown control requesting the update.</param>
/// <param name="buttonTag">The common portion of the name of the controls to be updated.</param>
function GuiTool::setButtonPreview(%this, %dropdown, %buttonTag)
{
    %button = %buttonTag @ "Preview";
    %edit = %buttonTag @ "Edit";
    
    switch$(%dropdown.getText())
    {
        case "Up":
            %button.setBitmap(%button.bitmapNormal);
            %edit.text = fileName(%button.bitmap);
            
        case "Over":
            %button.setBitmap(%button.bitmapHilight);
            %edit.text = fileName(%button.bitmap);

        case "Down":
            %button.setBitmap(%button.bitmapDepressed);
            %edit.text = fileName(%button.bitmap);
    }
}

/// <summary>
/// This function saves all interface changes made in the tool.
/// </summary>
function GuiTool::saveGuiSet(%this)
{
   // Main Menu
   mainMenuBackground.bitmap = %this.guiImageRelDir @ fileName(GUIMenuTabTitlePreview.bitmap);

   menuStartButton.bitmap = %this.guiImageRelDir @ fileName(GUIMenuTabPlayBtnPreview.bitmap);
   menuStartButton.bitmapNormal = %this.guiImageRelDir @ fileName(GUIMenuTabPlayBtnPreview.bitmapNormal);
   menuStartButton.bitmapHilight = %this.guiImageRelDir @ fileName(GUIMenuTabPlayBtnPreview.bitmapHilight);
   menuStartButton.bitmapDepressed = %this.guiImageRelDir @ fileName(GUIMenuTabPlayBtnPreview.bitmapDepressed);
   
   mainMenuHowToPlayButton.bitmap = %this.guiImageRelDir @ fileName(GUIMenuTabHowToPlayBtnPreview.bitmap);
   mainMenuHowToPlayButton.bitmapNormal = %this.guiImageRelDir @ fileName(GUIMenuTabHowToPlayBtnPreview.bitmapNormal);
   mainMenuHowToPlayButton.bitmapHilight = %this.guiImageRelDir @ fileName(GUIMenuTabHowToPlayBtnPreview.bitmapHilight);
   mainMenuHowToPlayButton.bitmapDepressed = %this.guiImageRelDir @ fileName(GUIMenuTabHowToPlayBtnPreview.bitmapDepressed);
   
   menuCreditsButton.bitmap = %this.guiImageRelDir @ fileName(GUIMenuTabCreditsBtnPreview.bitmap);
   menuCreditsButton.bitmapNormal = %this.guiImageRelDir @ fileName(GUIMenuTabCreditsBtnPreview.bitmapNormal);
   menuCreditsButton.bitmapHilight = %this.guiImageRelDir @ fileName(GUIMenuTabCreditsBtnPreview.bitmapHilight);
   menuCreditsButton.bitmapDepressed = %this.guiImageRelDir @ fileName(GUIMenuTabCreditsBtnPreview.bitmapDepressed);
   
   
   // Level Select Screen
   LevelSelectBackground.bitmap = %this.guiImageRelDir @ fileName(GUILevelSelectBackgroundPreview.bitmap);

   LevelSelectNextBtn.bitmap = %this.guiImageRelDir @ fileName(GUILevelSelectNextBtnPreview.bitmap);
   LevelSelectNextBtn.bitmapNormal = %this.guiImageRelDir @ fileName(GUILevelSelectNextBtnPreview.bitmapNormal);
   LevelSelectNextBtn.bitmapHilight = %this.guiImageRelDir @ fileName(GUILevelSelectNextBtnPreview.bitmapHilight);
   LevelSelectNextBtn.bitmapDepressed = %this.guiImageRelDir @ fileName(GUILevelSelectNextBtnPreview.bitmapDepressed);
   
   LevelSelectBackBtn.bitmap = %this.guiImageRelDir @ fileName(GUILevelSelectBackBtnPreview.bitmap);
   LevelSelectBackBtn.bitmapNormal = %this.guiImageRelDir @ fileName(GUILevelSelectBackBtnPreview.bitmapNormal);
   LevelSelectBackBtn.bitmapHilight = %this.guiImageRelDir @ fileName(GUILevelSelectBackBtnPreview.bitmapHilight);
   LevelSelectBackBtn.bitmapDepressed = %this.guiImageRelDir @ fileName(GUILevelSelectBackBtnPreview.bitmapDepressed);
   
   LevelSelectHomeBtn.bitmap = %this.guiImageRelDir @ fileName(GUILevelSelectHomeBtnPreview.bitmap);
   LevelSelectHomeBtn.bitmapNormal = %this.guiImageRelDir @ fileName(GUILevelSelectHomeBtnPreview.bitmapNormal);
   LevelSelectHomeBtn.bitmapHilight = %this.guiImageRelDir @ fileName(GUILevelSelectHomeBtnPreview.bitmapHilight);
   LevelSelectHomeBtn.bitmapDepressed = %this.guiImageRelDir @ fileName(GUILevelSelectHomeBtnPreview.bitmapDepressed);
   
   
   // Level
   waveInfoContainer.imageMap = GUILevelTabLargeIconPreview.asset;
   AddAssetToLevelDatablocks(GUILevelTabLargeIconPreview.asset);

   scoreContainer.imageMap = GUILevelTabSmallIconPreview.asset;
   AddAssetToLevelDatablocks(GUILevelTabSmallIconPreview.asset);

   livesContainer.imageMap = GUILevelTabSmallIconPreview.asset;
   AddAssetToLevelDatablocks(GUILevelTabSmallIconPreview.asset);

   fundsContainer.imageMap = GUILevelTabSmallIconPreview.asset;
   AddAssetToLevelDatablocks(GUILevelTabSmallIconPreview.asset);

   pauseButton.imageMap = GUILevelTabPauseBtnPreview.asset;
   AddAssetToLevelDatablocks(GUILevelTabPauseBtnPreview.asset);

   livesIcon.imageMap = GUILevelTabHealthPreview.asset;
   AddAssetToLevelDatablocks(GUILevelTabHealthPreview.asset);

   fundsIcon.imageMap = GUILevelTabCurrencyPreview.asset;   
   AddAssetToLevelDatablocks(GUILevelTabCurrencyPreview.asset);

   waveInfoIcon.imageMap = GUILevelTabWavePreview.asset;
   AddAssetToLevelDatablocks(GUILevelTabWavePreview.asset);

   scoreIcon.imageMap = GUILevelTabPointsPreview.asset;
   AddAssetToLevelDatablocks(GUILevelTabPointsPreview.asset);

   rangeCircle.imageMap = GUILevelTabRangePreview.asset;
   AddAssetToLevelDatablocks(GUILevelTabRangePreview.asset);

   cancelTower.imageMap = GUILevelTabCancelTowerPreview.asset;
   AddAssetToLevelDatablocks(GUILevelTabCancelTowerPreview.asset);

   upgradeButton.imageMap = GUILevelTabUpgradePreview.asset;
   AddAssetToLevelDatablocks(GUILevelTabUpgradePreview.asset);

   sellButton.imageMap = GUILevelTabSellPreview.asset;
   AddAssetToLevelDatablocks(GUILevelTabSellPreview.asset);

   confirmButton.imageMap = GUILevelTabAcceptPreview.asset;
   AddAssetToLevelDatablocks(GUILevelTabAcceptPreview.asset);

   towerCancelButton.imageMap = GUILevelTabCancelPreview.asset;
   AddAssetToLevelDatablocks(GUILevelTabCancelPreview.asset);

   startNextWaveButton.imageMap = GUILevelTabStartPreview.asset;
   AddAssetToLevelDatablocks(GUILevelTabStartPreview.asset);

   LDEonApply();
   SaveAllLevelDatablocks();

   // Win/Lose
   winScreen.bitmap = %this.guiImageRelDir @ fileName(GUIWinTabWinScreenPreview.bitmap);
   loseScreen.bitmap = %this.guiImageRelDir @ fileName(GUIWinTabLoseScreenPreview.bitmap);

   winReplayButton.bitmap = %this.guiImageRelDir @ fileName(GUIWinTabReplayBtnPreview.bitmap);
   winReplayButton.bitmapNormal = %this.guiImageRelDir @ fileName(GUIWinTabReplayBtnPreview.bitmapNormal);
   winReplayButton.bitmapHilight = %this.guiImageRelDir @ fileName(GUIWinTabReplayBtnPreview.bitmapHilight);
   winReplayButton.bitmapDepressed = %this.guiImageRelDir @ fileName(GUIWinTabReplayBtnPreview.bitmapDepressed);

   winMainMenuButton.bitmap = %this.guiImageRelDir @ fileName(GUIWinTabMenuBtnPreview.bitmap);
   winMainMenuButton.bitmapNormal = %this.guiImageRelDir @ fileName(GUIWinTabMenuBtnPreview.bitmapNormal);
   winMainMenuButton.bitmapHilight = %this.guiImageRelDir @ fileName(GUIWinTabMenuBtnPreview.bitmapHilight);
   winMainMenuButton.bitmapDepressed = %this.guiImageRelDir @ fileName(GUIWinTabMenuBtnPreview.bitmapDepressed);
   
   winNextLevelButton.bitmap = %this.guiImageRelDir @ fileName(GUIWinTabLevelBtnPreview.bitmap);
   winNextLevelButton.bitmapNormal = %this.guiImageRelDir @ fileName(GUIWinTabLevelBtnPreview.bitmapNormal);
   winNextLevelButton.bitmapHilight = %this.guiImageRelDir @ fileName(GUIWinTabLevelBtnPreview.bitmapHilight);
   winNextLevelButton.bitmapDepressed = %this.guiImageRelDir @ fileName(GUIWinTabLevelBtnPreview.bitmapDepressed);
   
   loseReplayButton.bitmap = %this.guiImageRelDir @ fileName(GUIWinTabReplayBtnPreview.bitmap);
   loseReplayButton.bitmapNormal = %this.guiImageRelDir @ fileName(GUIWinTabReplayBtnPreview.bitmapNormal);
   loseReplayButton.bitmapHilight = %this.guiImageRelDir @ fileName(GUIWinTabReplayBtnPreview.bitmapHilight);
   loseReplayButton.bitmapDepressed = %this.guiImageRelDir @ fileName(GUIWinTabReplayBtnPreview.bitmapDepressed);

   loseMainMenuButton.bitmap = %this.guiImageRelDir @ fileName(GUIWinTabMenuBtnPreview.bitmap);
   loseMainMenuButton.bitmapNormal = %this.guiImageRelDir @ fileName(GUIWinTabMenuBtnPreview.bitmapNormal);
   loseMainMenuButton.bitmapHilight = %this.guiImageRelDir @ fileName(GUIWinTabMenuBtnPreview.bitmapHilight);
   loseMainMenuButton.bitmapDepressed = %this.guiImageRelDir @ fileName(GUIWinTabMenuBtnPreview.bitmapDepressed);
   
   // Pause
   pauseBackground.bitmap = %this.guiImageRelDir @ fileName(GUIPauseTabBackgroundPreview.bitmap);

   pauseHowToPlayButton.bitmap = %this.guiImageRelDir @ fileName(GUIPauseTabHowToPlayBtnPreview.bitmap);
   pauseHowToPlayButton.bitmapNormal = %this.guiImageRelDir @ fileName(GUIPauseTabHowToPlayBtnPreview.bitmapNormal);
   pauseHowToPlayButton.bitmapHilight = %this.guiImageRelDir @ fileName(GUIPauseTabHowToPlayBtnPreview.bitmapHilight);
   pauseHowToPlayButton.bitmapDepressed = %this.guiImageRelDir @ fileName(GUIPauseTabHowToPlayBtnPreview.bitmapDepressed);
   
   pauseResumeButton.bitmap = %this.guiImageRelDir @ fileName(GUIPauseTabResumeBtnPreview.bitmap);
   pauseResumeButton.bitmapNormal = %this.guiImageRelDir @ fileName(GUIPauseTabResumeBtnPreview.bitmapNormal);
   pauseResumeButton.bitmapHilight = %this.guiImageRelDir @ fileName(GUIPauseTabResumeBtnPreview.bitmapHilight);
   pauseResumeButton.bitmapDepressed = %this.guiImageRelDir @ fileName(GUIPauseTabResumeBtnPreview.bitmapDepressed);

   pauseMainMenuButton.bitmap = %this.guiImageRelDir @ fileName(GUIPauseTabMenuBtnPreview.bitmap);
   pauseMainMenuButton.bitmapNormal = %this.guiImageRelDir @ fileName(GUIPauseTabMenuBtnPreview.bitmapNormal);
   pauseMainMenuButton.bitmapHilight = %this.guiImageRelDir @ fileName(GUIPauseTabMenuBtnPreview.bitmapHilight);
   pauseMainMenuButton.bitmapDepressed = %this.guiImageRelDir @ fileName(GUIPauseTabMenuBtnPreview.bitmapDepressed);

   // Help
   howToPlayGui.howToPlayScreen1 = %this.guiImageRelDir @ fileName(GUIHelpTabScreen1Preview.bitmap);
   howToPlayGui.howToPlayScreen2 = %this.guiImageRelDir @ fileName(GUIHelpTabScreen2Preview.bitmap);
   howToPlayGui.howToPlayScreen3 = %this.guiImageRelDir @ fileName(GUIHelpTabScreen3Preview.bitmap);
   howToPlayGui.howToPlayScreen4 = %this.guiImageRelDir @ fileName(GUIHelpTabScreen4Preview.bitmap);
   howToPlayGui.howToPlayScreen5 = %this.guiImageRelDir @ fileName(GUIHelpTabScreen5Preview.bitmap);
   howToPlayGui.howToPlayScreen6 = %this.guiImageRelDir @ fileName(GUIHelpTabScreen6Preview.bitmap);
   howToPlayGui.numHowToPlayScreens = $HowToPlayScreens;

   helpNextButton.bitmap = %this.guiImageRelDir @ fileName(GUIHelpTabForwardBtnPreview.bitmap);
   helpNextButton.bitmapNormal = %this.guiImageRelDir @ fileName(GUIHelpTabForwardBtnPreview.bitmapNormal);
   helpNextButton.bitmapHilight = %this.guiImageRelDir @ fileName(GUIHelpTabForwardBtnPreview.bitmapHilight);
   helpNextButton.bitmapDepressed = %this.guiImageRelDir @ fileName(GUIHelpTabForwardBtnPreview.bitmapDepressed);

   helpPreviousButton.bitmap = %this.guiImageRelDir @ fileName(GUIHelpTabBackBtnPreview.bitmap);
   helpPreviousButton.bitmapNormal = %this.guiImageRelDir @ fileName(GUIHelpTabBackBtnPreview.bitmapNormal);
   helpPreviousButton.bitmapHilight = %this.guiImageRelDir @ fileName(GUIHelpTabBackBtnPreview.bitmapHilight);
   helpPreviousButton.bitmapDepressed = %this.guiImageRelDir @ fileName(GUIHelpTabBackBtnPreview.bitmapDepressed);

   helpCloseButton.bitmap = %this.guiImageRelDir @ fileName(GUIHelpTabCloseBtnPreview.bitmap);
   helpCloseButton.bitmapNormal = %this.guiImageRelDir @ fileName(GUIHelpTabCloseBtnPreview.bitmapNormal);
   helpCloseButton.bitmapHilight = %this.guiImageRelDir @ fileName(GUIHelpTabCloseBtnPreview.bitmapHilight);
   helpCloseButton.bitmapDepressed = %this.guiImageRelDir @ fileName(GUIHelpTabCloseBtnPreview.bitmapDepressed);

   // Credits
   creditsBackground.bitmap = %this.guiImageRelDir @ fileName(GUICreditsTabBackgroundPreview.bitmap);
   
   creditsGuiCreditsImage.bitmap = %this.guiImageRelDir @ fileName(GUICreditsTabCreditsPreview.bitmap);

   closeCreditsButton.bitmap = %this.guiImageRelDir @ fileName(GUICreditsTabCloseBtnPreview.bitmap);
   closeCreditsButton.bitmapNormal = %this.guiImageRelDir @ fileName(GUICreditsTabCloseBtnPreview.bitmapNormal);
   closeCreditsButton.bitmapHilight = %this.guiImageRelDir @ fileName(GUICreditsTabCloseBtnPreview.bitmapHilight);
   closeCreditsButton.bitmapDepressed = %this.guiImageRelDir @ fileName(GUICreditsTabCloseBtnPreview.bitmapDepressed);

   //creditsGui.creditsTxtFile = %this.guiDir @ fileName(GUICreditsTabCreditsTextFileEdit.getText());
   mainMenuGui.showCreditsButton = GUICreditsTabMenuBtnCheckbox.getValue();
   
   // Save the gui files
   GuiToolSaveGui(howToPlayGui);
   GuiToolSaveGui(mainMenuGui);
   GuiToolSaveGui(mainScreenGui);
   GuiToolSaveGui(winGui);
   GuiToolSaveGui(loseGui);
   GuiToolSaveGui(pauseGui);
   GuiToolSaveGui(creditsGui);
   GuiToolSaveGui(levelSelectGui);

   LBProjectObj.persistToDisk(true, true, true, true, true, true);
   LBProjectObj.saveLevel();
   
   return true;
}

/// <summary>
/// This function handles the close button.
/// </summary>
function GuiTool::close(%this)
{
   if ($Tools::TemplateToolDirty)
   {
      ConfirmDialog.setupAndShow(GuiToolWindow.getGlobalCenter(), "Save Interface Changes?", 
            "Save", "if (GuiTool.saveGuiSet()){ SetGuiToolDirtyState(false); Canvas.popDialog(GuiTool);}", 
            "Don't Save", "SetGuiToolDirtyState(false); Canvas.popDialog(GuiTool);", 
            "Cancel", "");
   }
   else
      Canvas.popDialog(GuiTool);
}

//-----------------------------------------------------------------------------
// GUI Tool
// ** Main Menu Tab
//-----------------------------------------------------------------------------
/// <summary>
/// This function initializes the controls on the Main Menu Tab.
/// </summary>
function GuiTool::initMenuTab(%this)
{
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(mainMenuBackground.bitmap));
    GUIMenuTabTitlePreview.setBitmap(GetFullFileName(%tempBmpPath));
    GUIMenuTabTitleEdit.text = fileName(%tempBmpPath);
    GUIMenuTabTitleEdit.setActive(false);

    %tempBmpPath = GetGuiImageFileRelativePath(fileName(menuStartButton.bitmapNormal));
    GUIMenuTabPlayBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(menuStartButton.bitmapHilight));
    GUIMenuTabPlayBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(menuStartButton.bitmapDepressed));
    GUIMenuTabPlayBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(menuStartButton.bitmap));
    GUIMenuTabPlayBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
    GUIMenuTabPlayBtnEdit.text = fileName(%tempBmpPath);
    GUIMenuTabPlayBtnEdit.setActive(false);   

    %tempBmpPath = GetGuiImageFileRelativePath(fileName(mainMenuHowToPlayButton.bitmapNormal));
    GUIMenuTabHowToPlayBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(mainMenuHowToPlayButton.bitmapHilight));
    GUIMenuTabHowToPlayBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(mainMenuHowToPlayButton.bitmapDepressed));
    GUIMenuTabHowToPlayBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(mainMenuHowToPlayButton.bitmap));
    GUIMenuTabHowToPlayBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
    GUIMenuTabHowToPlayBtnEdit.text = fileName(%tempBmpPath);
    GUIMenuTabHowToPlayBtnEdit.setActive(false);

    %tempBmpPath = GetGuiImageFileRelativePath(fileName(menuCreditsButton.bitmapNormal));
    GUIMenuTabCreditsBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(menuCreditsButton.bitmapHilight));
    GUIMenuTabCreditsBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(menuCreditsButton.bitmapDepressed));
    GUIMenuTabCreditsBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(menuCreditsButton.bitmap));
    GUIMenuTabCreditsBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
    GUIMenuTabCreditsBtnEdit.text = fileName(%tempBmpPath);
    GUIMenuTabCreditsBtnEdit.setActive(false);

    GUIToolRefreshStateDropdown(GUIMenuTabPlayBtnDropdown);
    GUIToolRefreshStateDropdown(GUIMenuTabHowToPlayBtnDropdown);
    GUIToolRefreshStateDropdown(GUIMenuTabCreditsBtnDropdown);
}

/// <summary>
/// This function opens the Asset Library to select an image for the main menu
/// background.
/// </summary>
function GUIMenuTabTitleBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the asset selected in the Asset Library to the
/// main menu background.
/// </summary>
/// <param name="asset">The asset returned from the Asset Library.</param>
function GUIMenuTabTitleBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIMenuTabTitle);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the selected
/// play button state.
/// </summary>
function GUIMenuTabPlayBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the asset selected in the Asset Library to the 
/// play button's selected button state.
/// </summary>
/// <param name="asset">The asset returned from the Asset Library.</param>
function GUIMenuTabPlayBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIMenuTabPlayBtn);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function sets the play button's button state image to that of the selected state.
/// </summary>
function GUIMenuTabPlayBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUIMenuTabPlayBtn);
}

/// <summary>
/// This function opens the Asset Library to select an image for the selected
/// how to play button state.
/// </summary>
function GUIMenuTabHowToPlayBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the asset selected in the Asset Library to the 
/// how to play button's selected button state.
/// </summary>
/// <param name="asset">The asset returned from the Asset Library.</param>
function GUIMenuTabHowToPlayBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIMenuTabHowToPlayBtn);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function sets the how to play button's button state image to that of the selected state.
/// </summary>
function GUIMenuTabHowToPlayBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUIMenuTabHowToPlayBtn);
}

/// <summary>
/// This function opens the Asset Library to select an image for the selected
/// credits button state.
/// </summary>
function GUIMenuTabCreditsBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the asset selected in the Asset Library to the 
/// credits button's selected button state.
/// </summary>
/// <param name="asset">The asset returned from the Asset Library.</param>
function GUIMenuTabCreditsBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIMenuTabCreditsBtn);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function sets the credits button's button state image to that of the selected state.
/// </summary>
function GUIMenuTabCreditsBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUIMenuTabCreditsBtn);
}

//-----------------------------------------------------------------------------
// GUI Tool
// ** Level Select Screen Tab
//-----------------------------------------------------------------------------
/// <summary>
/// This function initializes the controls on the Level Select Screen Tab.
/// </summary>
function GuiTool::initLevelSelectTab(%this)
{
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(LevelSelectBackground.bitmap));
   GUILevelSelectBackgroundPreview.setBitmap(GetFullFileName(%tempBmpPath));
   GUILevelSelectBackgroundEdit.text = fileName(%tempBmpPath);
   GUILevelSelectBackgroundEdit.setActive(false);
   
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(LevelSelectNextBtn.bitmapNormal));
   GUILevelSelectNextBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(LevelSelectNextBtn.bitmapHilight));
   GUILevelSelectNextBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(LevelSelectNextBtn.bitmapDepressed));
   GUILevelSelectNextBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(LevelSelectNextBtn.bitmap));
   GUILevelSelectNextBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
   GUILevelSelectNextBtnEdit.text = fileName(%tempBmpPath);
   GUILevelSelectNextBtnEdit.setActive(false);   

   %tempBmpPath = GetGuiImageFileRelativePath(fileName(LevelSelectBackBtn.bitmapNormal));
   GUILevelSelectBackBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(LevelSelectBackBtn.bitmapHilight));
   GUILevelSelectBackBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(LevelSelectBackBtn.bitmapDepressed));
   GUILevelSelectBackBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(LevelSelectBackBtn.bitmap));
   GUILevelSelectBackBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
   GUILevelSelectBackBtnEdit.text = fileName(%tempBmpPath);
   GUILevelSelectBackBtnEdit.setActive(false);

   %tempBmpPath = GetGuiImageFileRelativePath(fileName(LevelSelectHomeBtn.bitmapNormal));
   GUILevelSelectHomeBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(LevelSelectHomeBtn.bitmapHilight));
   GUILevelSelectHomeBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(LevelSelectHomeBtn.bitmapDepressed));
   GUILevelSelectHomeBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(LevelSelectHomeBtn.bitmap));
   GUILevelSelectHomeBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
   GUILevelSelectHomeBtnEdit.text = fileName(%tempBmpPath);
   GUILevelSelectHomeBtnEdit.setActive(false);
   
   GUIToolRefreshStateDropdown(GUILevelSelectNextBtnDropdown);
   GUIToolRefreshStateDropdown(GUILevelSelectBackBtnDropdown);
   GUIToolRefreshStateDropdown(GUILevelSelectHomeBtnDropdown);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// Level Select screen background.
/// </summary>
function GUILevelSelectBackgroundBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the Level Select background.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelSelectBackgroundBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUILevelSelectBackground);

    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// next button's selected state.
/// </summary>
function GUILevelSelectNextBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the next button's selected 
/// button state image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelSelectNextBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUILevelSelectNextBtn);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns the next button's selected button state image to the preview.
/// </summary>
function GUILevelSelectNextBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUILevelSelectNextBtn);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// back button's selected state.
/// </summary>
function GUILevelSelectBackBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the back button's selected 
/// button state image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelSelectBackBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUILevelSelectBackBtn);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns the back button's selected button state image to the preview.
/// </summary>
function GUILevelSelectBackBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUILevelSelectBackBtn);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// home button's selected state.
/// </summary>
function GUILevelSelectHomeBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the home button's selected 
/// button state image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelSelectHomeBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUILevelSelectHomeBtn);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns the home button's selected button state image to the preview.
/// </summary>
function GUILevelSelectHomeBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUILevelSelectHomeBtn);
}

//-----------------------------------------------------------------------------
// ** Level Screen Tab
//-----------------------------------------------------------------------------
/// <summary>
/// This function initializes the controls on the Level Screen Tab.
/// </summary>
function GuiTool::initLevelTab(%this)
{
   // These are all scene objects.
   if (isObject(waveInfoContainer.imageMap) && isObject(GUILevelTabLargeIconPreview))
   {
       GUILevelTabLargeIconPreview.setBitmap(waveInfoContainer.imageMap.imageFile);
       GUILevelTabLargeIconEdit.text = fileName(waveInfoContainer.imageMap.imageFile);
       GUILevelTabLargeIconEdit.setActive(false);
   }

   if (isObject(scoreContainer.imageMap) && isObject(GUILevelTabSmallIconPreview))
   {
       GUILevelTabSmallIconPreview.setBitmap(scoreContainer.imageMap.imageFile);
       GUILevelTabSmallIconEdit.text = fileName(scoreContainer.imageMap.imageFile);
       GUILevelTabSmallIconEdit.setActive(false);
   }

   if (isObject(waveInfoContainer.imageMap) && isObject(GUILevelTabBottomPreview))
   {
       GUILevelTabBottomPreview.setBitmap(waveInfoContainer.imageMap.imageFile);
       GUILevelTabBottomEdit.text = fileName(waveInfoContainer.imageMap.imageFile);
       GUILevelTabBottomEdit.setActive(false);
   }
   
   GUILevelTabPauseBtnPreview.setBitmap(pauseButton.getImageMap().imageFile);
   GUILevelTabPauseBtnEdit.text = fileName(pauseButton.getImageMap().imageFile);
   GUILevelTabPauseBtnEdit.setActive(false);

   GUILevelTabHealthPreview.setBitmap(livesIcon.getImageMap().imageFile);
   GUILevelTabHealthEdit.text = fileName(livesIcon.getImageMap().imageFile);
   GUILevelTabHealthEdit.setActive(false);

   GUILevelTabCurrencyPreview.setBitmap(fundsIcon.getImageMap().imageFile);
   GUILevelTabCurrencyEdit.text = fileName(fundsIcon.getImageMap().imageFile);
   GUILevelTabCurrencyEdit.setActive(false);

   GUILevelTabWavePreview.setBitmap(waveInfoIcon.getImageMap().imageFile);
   GUILevelTabWaveEdit.text = fileName(waveInfoIcon.getImageMap().imageFile);
   GUILevelTabWaveEdit.setActive(false);

   GUILevelTabPointsPreview.setBitmap(scoreIcon.getImageMap().imageFile);
   GUILevelTabPointsEdit.text = fileName(scoreIcon.getImageMap().imageFile);
   GUILevelTabPointsEdit.setActive(false);

   GUILevelTabRangePreview.setBitmap(rangeCircle.getImageMap().imageFile);
   GUILevelTabRangeEdit.text = fileName(rangeCircle.getImageMap().imageFile);
   GUILevelTabRangeEdit.setActive(false);

   GUILevelTabCancelTowerPreview.setBitmap(cancelTower.getImageMap().imageFile);
   GUILevelTabCancelTowerEdit.text = fileName(cancelTower.getImageMap().imageFile);
   GUILevelTabCancelTowerEdit.setActive(false);

   GUILevelTabUpgradePreview.setBitmap(upgradeButton.getImageMap().imageFile);
   GUILevelTabUpgradeEdit.text = fileName(upgradeButton.getImageMap().imageFile);
   GUILevelTabUpgradeEdit.setActive(false);

   GUILevelTabSellPreview.setBitmap(sellButton.getImageMap().imageFile);
   GUILevelTabSellEdit.text = fileName(sellButton.getImageMap().imageFile);
   GUILevelTabSellEdit.setActive(false);

   GUILevelTabAcceptPreview.setBitmap(confirmButton.getImageMap().imageFile);
   GUILevelTabAcceptEdit.text = fileName(confirmButton.getImageMap().imageFile);
   GUILevelTabAcceptEdit.setActive(false);

   GUILevelTabCancelPreview.setBitmap(towerCancelButton.getImageMap().imageFile);
   GUILevelTabCancelEdit.text = fileName(towerCancelButton.getImageMap().imageFile);
   GUILevelTabCancelEdit.setActive(false);

   GUILevelTabStartPreview.setBitmap(startNextWaveButton.getImageMap().imageFile);
   GUILevelTabStartEdit.text = fileName(startNextWaveButton.getImageMap().imageFile);
   GUILevelTabStartEdit.setActive(false);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// large level info container
/// </summary>
function GUILevelTabLargeBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the large level info container.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabLargeBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabLargeIconPreview.asset = %asset;
    GUILevelTabLargeIconPreview.setBitmap(%asset.imageFile);
    GUILevelTabLargeIconEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// small level info container.
/// </summary>
function GUILevelTabSmallBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the small level info container.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabSmallBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabSmallIconPreview.asset = %asset;
    GUILevelTabSmallIconPreview.setBitmap(%asset.imageFile);
    GUILevelTabSmallIconEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// level bottom container. (not currently used).
/// </summary>
function GUILevelTabBottomBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the level bottom container - not used.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabBottomBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabBottomPreview.asset = %asset;
    GUILevelTabBottomPreview.setBitmap(%asset.imageFile);
    GUILevelTabBottomEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// wave container image.
/// </summary>
function GUILevelTabLargeIconBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the wave container image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabLargeIconBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabLargeIconPreview.asset = %asset;
    GUILevelTabLargeIconPreview.setBitmap(%asset.imageFile);
    GUILevelTabLargeIconEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// lives/currency/points container image.
/// </summary>
function GUILevelTabSmallIconBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the lives/currency/points container image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabSmallIconBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabSmallIconPreview.asset = %asset;
    GUILevelTabSmallIconPreview.setBitmap(%asset.imageFile);
    GUILevelTabSmallIconEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// pause button image.
/// </summary>
function GUILevelTabPauseBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the pause button image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabPauseBtnBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabPauseBtnPreview.asset = %asset;
    GUILevelTabPauseBtnPreview.setBitmap(%asset.imageFile);
    GUILevelTabPauseBtnEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// health icon image.
/// </summary>
function GUILevelTabHealthBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the health icon image
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabHealthBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabHealthPreview.asset = %asset;
    GUILevelTabHealthPreview.setBitmap(%asset.imageFile);
    GUILevelTabHealthEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// currency icon image.
/// </summary>
function GUILevelTabCurrencyBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the currency icon image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabCurrencyBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabCurrencyPreview.asset = %asset;
    GUILevelTabCurrencyPreview.setBitmap(%asset.imageFile);
    GUILevelTabCurrencyEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// wave icon image.
/// </summary>
function GUILevelTabWaveBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the wave icon image
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabWaveBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabWavePreview.asset = %asset;
    GUILevelTabWavePreview.setBitmap(%asset.imageFile);
    GUILevelTabWaveEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// points icon image.
/// </summary>
function GUILevelTabPointsBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the points icon image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabPointsBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabPointsPreview.asset = %asset;
    GUILevelTabPointsPreview.setBitmap(%asset.imageFile);
    GUILevelTabPointsEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// range circle image.
/// </summary>
function GUILevelTabRangeBrowse::OnClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the range circle image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabCancelTowerBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabCancelTowerPreview.asset = %asset;
    GUILevelTabCancelTowerPreview.setBitmap(%asset.imageFile);
    GUILevelTabCancelTowerEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// cancel tower button image.
/// </summary>
function GUILevelTabCancelTowerBrowse::OnClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the cancel tower button image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabRangeBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabRangePreview.asset = %asset;
    GUILevelTabRangePreview.setBitmap(%asset.imageFile);
    GUILevelTabRangeEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// upgrade button.
/// </summary>
function GUILevelTabUpgradeBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the upgrade button image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabUpgradeBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabUpgradePreview.asset = %asset;
    GUILevelTabUpgradePreview.setBitmap(%asset.imageFile);
    GUILevelTabUpgradeEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// sell button image.
/// </summary>
function GUILevelTabSellBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the sell button image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabSellBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabSellPreview.asset = %asset;
    GUILevelTabSellPreview.setBitmap(%asset.imageFile);
    GUILevelTabSellEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// accept button image.
/// </summary>
function GUILevelTabAcceptBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the accept button image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabAcceptBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabAcceptPreview.asset = %asset;
    GUILevelTabAcceptPreview.setBitmap(%asset.imageFile);
    GUILevelTabAcceptEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// cancel button image.
/// </summary>
function GUILevelTabCancelBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the cancel button image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabCancelBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabCancelPreview.asset = %asset;
    GUILevelTabCancelPreview.setBitmap(%asset.imageFile);
    GUILevelTabCancelEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// wave start button image.
/// </summary>
function GUILevelTabStartBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $SpriteSheetPage);
}

/// <summary>
/// This function assigns the selected asset to the wave start button image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUILevelTabStartBrowse::setSelectedAsset(%this, %asset)
{
    GUILevelTabStartPreview.asset = %asset;
    GUILevelTabStartPreview.setBitmap(%asset.imageFile);
    GUILevelTabStartEdit.text = fileName(%asset.imageFile);
    SetGuiToolDirtyState(true);
}
//-----------------------------------------------------------------------------
// ** Win/Lose Screen Tab
//-----------------------------------------------------------------------------
/// <summary>
/// This function initializes the controls on the Win/Lose Screen Tab.
/// </summary>
function GuiTool::initWinTab(%this)
{
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(winScreen.bitmap));
   GUIWinTabWinScreenPreview.setBitmap(GetFullFileName(%tempBmpPath));
   GUIWinTabWinScreenEdit.text = fileName(%tempBmpPath);
   GUIWinTabWinScreenEdit.setActive(false);
   
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(loseScreen.bitmap));
   GUIWinTabLoseScreenPreview.setBitmap(GetFullFileName(%tempBmpPath));
   GUIWinTabLoseScreenEdit.text = fileName(%tempBmpPath);
   GUIWinTabLoseScreenEdit.setActive(false);
   
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(winReplayButton.bitmapNormal));
   GUIWinTabReplayBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(winReplayButton.bitmapHilight));
   GUIWinTabReplayBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(winReplayButton.bitmapDepressed));
   GUIWinTabReplayBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(winReplayButton.bitmap));
   GUIWinTabReplayBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
   GUIWinTabReplayBtnEdit.text = fileName(%tempBmpPath);
   GUIWinTabReplayBtnEdit.setActive(false);
   
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(winMainMenuButton.bitmapNormal));
   GUIWinTabMenuBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(winMainMenuButton.bitmapHilight));
   GUIWinTabMenuBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(winMainMenuButton.bitmapDepressed));
   GUIWinTabMenuBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(winMainMenuButton.bitmap));
   GUIWinTabMenuBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
   GUIWinTabMenuBtnEdit.text = fileName(%tempBmpPath);
   GUIWinTabMenuBtnEdit.setActive(false);

   %tempBmpPath = GetGuiImageFileRelativePath(fileName(winNextLevelButton.bitmapNormal));
   GUIWinTabLevelBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(winNextLevelButton.bitmapHilight));
   GUIWinTabLevelBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(winNextLevelButton.bitmapDepressed));
   GUIWinTabLevelBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(winNextLevelButton.bitmap));
   GUIWinTabLevelBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
   GUIWinTabLevelBtnEdit.text = fileName(%tempBmpPath);
   GUIWinTabLevelBtnEdit.setActive(false);

   GUIToolRefreshStateDropdown(GUIWinTabReplayBtnDropdown);
   GUIToolRefreshStateDropdown(GUIWinTabMenuBtnDropdown);
   GUIToolRefreshStateDropdown(GUIWinTabLevelBtnDropdown);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// win screen background.
/// </summary>
function GUIWinTabWinScreenBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the win screen background
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIWinTabWinScreenBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIWinTabWinScreen);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// lose screen background.
/// </summary>
function GUIWinTabLoseScreenBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the lose screen background.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIWinTabLoseScreenBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIWinTabLoseScreen);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// replay button's selected state.
/// </summary>
function GUIWinTabReplayBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the replay button's selected 
/// button state image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIWinTabReplayBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIWinTabReplayBtn);

    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns the replay button's selected button state image to the preview.
/// </summary>
function GUIWinTabReplayBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUIWinTabReplayBtn);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// main menu button's selected state.
/// </summary>
function GUIWinTabMenuBtnBrowse::onClick(%this)
{
    AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the main menu button's selected 
/// button state image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIWinTabMenuBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIWinTabMenuBtn);

    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns the main menu button's selected button state image to the preview.
/// </summary>
function GUIWinTabMenuBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUIWinTabMenuBtn);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// next level button's selected state.
/// </summary>
function GUIWinTabLevelBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the next level button's selected 
/// button state image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIWinTabLevelBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIWinTabLevelBtn);

    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns the next level button's selected button state image to the preview.
/// </summary>
function GUIWinTabLevelBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUIWinTabLevelBtn);
}
//-----------------------------------------------------------------------------
// ** Pause Screen Tab
//-----------------------------------------------------------------------------
/// <summary>
/// This function initializes the Pause Screen tab controls.
/// </summary>
function GuiTool::initPauseTab(%this)
{
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(pauseBackground.bitmap));
    GUIPauseTabBackgroundPreview.setBitmap(GetFullFileName(%tempBmpPath));
    GUIPauseTabBackgroundEdit.text = fileName(%tempBmpPath);
    GUIPauseTabBackgroundEdit.setActive(false);

    %tempBmpPath = GetGuiImageFileRelativePath(fileName(pauseHowToPlayButton.bitmapNormal));
    GUIPauseTabHowToPlayBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(pauseHowToPlayButton.bitmapHilight));
    GUIPauseTabHowToPlayBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(pauseHowToPlayButton.bitmapDepressed));
    GUIPauseTabHowToPlayBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(pauseHowToPlayButton.bitmap));
    GUIPauseTabHowToPlayBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
    GUIPauseTabHowToPlayBtnEdit.text = fileName(%tempBmpPath);
    GUIPauseTabHowToPlayBtnEdit.setActive(false);

    %tempBmpPath = GetGuiImageFileRelativePath(fileName(pauseResumeButton.bitmapNormal));
    GUIPauseTabResumeBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(pauseResumeButton.bitmapHilight));
    GUIPauseTabResumeBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(pauseResumeButton.bitmapDepressed));
    GUIPauseTabResumeBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(pauseResumeButton.bitmap));
    GUIPauseTabResumeBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
    GUIPauseTabResumeBtnEdit.text = fileName(%tempBmpPath);
    GUIPauseTabResumeBtnEdit.setActive(false);

    %tempBmpPath = GetGuiImageFileRelativePath(fileName(pauseMainMenuButton.bitmapNormal));
    GUIPauseTabMenuBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(pauseMainMenuButton.bitmapHilight));
    GUIPauseTabMenuBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(pauseMainMenuButton.bitmapDepressed));
    GUIPauseTabMenuBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(pauseMainMenuButton.bitmap));
    GUIPauseTabMenuBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
    GUIPauseTabMenuBtnEdit.text = fileName(%tempBmpPath);
    GUIPauseTabMenuBtnEdit.setActive(false);
/*
    %tempBmpPath = winReplayButton.bitmapNormal;
    GUIPauseTabRestartBtnPreview.setBitmapNormal(%tempBmpPath);
    %tempBmpPath = winReplayButton.bitmapHilight;
    GUIPauseTabRestartBtnPreview.setBitmapHilight(%tempBmpPath);
    %tempBmpPath = winReplayButton.bitmapDepressed;
    GUIPauseTabRestartBtnPreview.setBitmapDepressed(%tempBmpPath);
    %tempBmpPath = winReplayButton.bitmap;
    GUIPauseTabRestartBtnPreview.setBitmap(%tempBmpPath);
    GUIPauseTabRestartBtnEdit.text = fileName(%tempBmpPath);
    GUIPauseTabRestartBtnEdit.setActive(false);
*/   
    GUIToolRefreshStateDropdown(GUIPauseTabHowToPlayBtnDropdown);
    GUIToolRefreshStateDropdown(GUIPauseTabResumeBtnDropdown);
    GUIToolRefreshStateDropdown(GUIPauseTabMenuBtnDropdown);
    //GUIToolRefreshStateDropdown(GUIPauseTabRestartBtnDropdown);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// pause screen background.
/// </summary>
function GUIPauseTabBackgroundBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the pause screen background.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIPauseTabBackgroundBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIPauseTabBackground);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// how to play button's selected state.
/// </summary>
function GUIPauseTabHowToPlayBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the how to play button's selected 
/// button state image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIPauseTabHowToPlayBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIPauseTabHowToPlayBtn);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns the how to play button's selected button state image to the preview.
/// </summary>
function GUIPauseTabHowToPlayBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUIPauseTabHowToPlayBtn);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// resume button's selected state.
/// </summary>
function GUIPauseTabResumeBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the resume button's selected 
/// button state image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIPauseTabResumeBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIPauseTabResumeBtn);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns the resume button's selected button state image to the preview.
/// </summary>
function GUIPauseTabResumeBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUIPauseTabResumeBtn);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// main menu button's selected state.
/// </summary>
function GUIPauseTabMenuBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the main menu button's selected 
/// button state image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIPauseTabMenuBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIPauseTabMenuBtn);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns the main menu button's selected button state image to the preview.
/// </summary>
function GUIPauseTabMenuBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUIPauseTabMenuBtn);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// restart button's selected state.
/// </summary>
function GUIPauseTabRestartBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the restart button's selected 
/// button state image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIPauseTabRestartBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIPauseTabRestartBtn);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns the restart button's selected button state image to the preview.
/// </summary>
function GUIPauseTabRestartBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUIPauseTabRestartBtn);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// close button's selected state.
/// </summary>
function GUIPauseTabCloseBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the close button's selected 
/// button state image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIPauseTabCloseBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIPauseTabCloseBtn);

    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns the close button's selected button state image to the preview.
/// </summary>
function GUIPauseTabCloseBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUIPauseTabCloseBtn);
}
//-----------------------------------------------------------------------------
// ** Help Screen Tab
//-----------------------------------------------------------------------------
/// <summary>
/// This function initializes the Help Screen tab controls.
/// </summary>
function GuiTool::initHelpTab(%this)
{
    %tempBmpPath = makeFullPath(howToPlayGui.howToPlayScreen1, getCurrentDirectory());
    GUIHelpTabScreen1Preview.bitmap = %tempBmpPath;
    GUIHelpTabScreen1Preview.setBitmap(%tempBmpPath);
    GUIHelpTabScreen1Edit.text = fileName(howToPlayGui.howToPlayScreen1);
    GUIHelpTabScreen1Edit.setActive(false);

    %tempBmpPath = makeFullPath(howToPlayGui.howToPlayScreen2, getCurrentDirectory());
    GUIHelpTabScreen2Preview.bitmap = %tempBmpPath;
    GUIHelpTabScreen2Preview.setBitmap(%tempBmpPath);
    GUIHelpTabScreen2Edit.text = fileName(howToPlayGui.howToPlayScreen2);
    GUIHelpTabScreen2Edit.setActive(false);
      
    %tempBmpPath = makeFullPath(howToPlayGui.howToPlayScreen3, getCurrentDirectory());
    GUIHelpTabScreen3Preview.bitmap = %tempBmpPath;
    GUIHelpTabScreen3Preview.setBitmap(%tempBmpPath);
    GUIHelpTabScreen3Edit.text = fileName(howToPlayGui.howToPlayScreen3);
    GUIHelpTabScreen3Edit.setActive(false);

    %tempBmpPath = makeFullPath(howToPlayGui.howToPlayScreen4, getCurrentDirectory());
    GUIHelpTabScreen4Preview.bitmap = %tempBmpPath;
    GUIHelpTabScreen4Preview.setBitmap(%tempBmpPath);
    GUIHelpTabScreen4Edit.text = fileName(howToPlayGui.howToPlayScreen4);
    GUIHelpTabScreen4Edit.setActive(false);

    %tempBmpPath = makeFullPath(howToPlayGui.howToPlayScreen5, getCurrentDirectory());
    GUIHelpTabScreen5Preview.bitmap = %tempBmpPath;
    GUIHelpTabScreen5Preview.setBitmap(%tempBmpPath);
    GUIHelpTabScreen5Edit.text = fileName(howToPlayGui.howToPlayScreen5);
    GUIHelpTabScreen5Edit.setActive(false);

    %tempBmpPath = makeFullPath(howToPlayGui.howToPlayScreen6, getCurrentDirectory());
    GUIHelpTabScreen6Preview.bitmap = %tempBmpPath;
    GUIHelpTabScreen6Preview.setBitmap(%tempBmpPath);
    GUIHelpTabScreen6Edit.text = fileName(howToPlayGui.howToPlayScreen6);
    GUIHelpTabScreen6Edit.setActive(false);

    %tempBmpPath = GetGuiImageFileRelativePath(fileName(helpNextButton.bitmapNormal));
    GUIHelpTabForwardBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(helpNextButton.bitmapHilight));
    GUIHelpTabForwardBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(helpNextButton.bitmapDepressed));
    GUIHelpTabForwardBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(helpNextButton.bitmap));
    GUIHelpTabForwardBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
    GUIHelpTabForwardBtnEdit.text = fileName(%tempBmpPath);
    GUIHelpTabForwardBtnEdit.setActive(false);

    %tempBmpPath = GetGuiImageFileRelativePath(fileName(helpPreviousButton.bitmapNormal));
    GUIHelpTabBackBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(helpPreviousButton.bitmapHilight));
    GUIHelpTabBackBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(helpPreviousButton.bitmapDepressed));
    GUIHelpTabBackBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(helpPreviousButton.bitmap));
    GUIHelpTabBackBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
    GUIHelpTabBackBtnEdit.text = fileName(%tempBmpPath);
    GUIHelpTabBackBtnEdit.setActive(false);

    %tempBmpPath = GetGuiImageFileRelativePath(fileName(helpCloseButton.bitmapNormal));
    GUIHelpTabCloseBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(helpCloseButton.bitmapHilight));
    GUIHelpTabCloseBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(helpCloseButton.bitmapDepressed));
    GUIHelpTabCloseBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
    %tempBmpPath = GetGuiImageFileRelativePath(fileName(helpCloseButton.bitmap));
    GUIHelpTabCloseBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
    GUIHelpTabCloseBtnEdit.text = fileName(%tempBmpPath);
    GUIHelpTabCloseBtnEdit.setActive(false);

    GUIToolSetupNumHelpScreensDropdown(GUIHelpTabNumScreensDropdown);
    GUIToolRefreshStateDropdown(GUIHelpTabForwardBtnDropdown);
    GUIToolRefreshStateDropdown(GUIHelpTabBackBtnDropdown);
    GUIToolRefreshStateDropdown(GUIHelpTabCloseBtnDropdown);
}

/// <summary>
/// This function hides help screen previews based on the number passed in.
/// </summary>
/// <param name="num">The number of previews to leave unhidden.</param>
function GUIToolHelpHider(%num)
{
   %label = "GUIToolHelpTabLabel1";
   %preview = "GUIHelpTabScreen1Preview";
   %edit = "GUIHelpTabScreen1Edit";
   %browse = "GUIHelpTabScreen1Browse";
   
   for (%i = 5; %i > %num; %i--)
   {
      %hideLabel = strreplace(%label, "1", %i + 1);
      %hidePreview = strreplace(%preview, "1", %i + 1);
      %hideEdit = strreplace(%edit, "1", %i + 1);
      %hideBrowse = strreplace(%browse, "1", %i + 1);
      
      %hideLabel.setVisible(false);
      %hidePreview.setVisible(false);
      %hideEdit.setVisible(false);
      %hideBrowse.setVisible(false);
   }
}

/// <summary>
/// This function shows help screen previews based on the number passed in.
/// </summary>
/// <param name="num">The number of previews to show.</param>
function GUIToolHelpUnHider(%num)
{
   %label = "GUIToolHelpTabLabel1";
   %preview = "GUIHelpTabScreen1Preview";
   %edit = "GUIHelpTabScreen1Edit";
   %browse = "GUIHelpTabScreen1Browse";
   
   for (%i = 0; %i <= %num; %i++)
   {
      %hideLabel = strreplace(%label, "1", %i + 1);
      %hidePreview = strreplace(%preview, "1", %i + 1);
      %hideEdit = strreplace(%edit, "1", %i + 1);
      %hideBrowse = strreplace(%browse, "1", %i + 1);
      
      %hideLabel.setVisible(true);
      %hidePreview.setVisible(true);
      %hideEdit.setVisible(true);
      %hideBrowse.setVisible(true);
   }
}

/// <summary>
/// This function sets the number of help screens that the game will use.
/// </summary>
function GUIHelpTabNumScreensDropdown::onSelect(%this)
{
    %selected = GUIHelpTabNumScreensDropdown.getSelected();
    GUIToolHelpHider(%selected);
    GUIToolHelpUnHider(%selected);
    $HowToPlayScreens = (%selected + 1);

    if (GuiTool.awake)
        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// first help screen
/// </summary>
function GUIHelpTabScreen1Browse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the first help screen's image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIHelpTabScreen1Browse::setSelectedAsset(%this, %asset)
{
    howToPlayGui.howToPlayScreen1 = GuiTool.assignHelpScreen(%asset, GUIHelpTabScreen1);

    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// second help screen.
/// </summary>
function GUIHelpTabScreen2Browse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the second help screen's image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIHelpTabScreen2Browse::setSelectedAsset(%this, %asset)
{
    howToPlayGui.howToPlayScreen2 = GuiTool.assignHelpScreen(%asset, GUIHelpTabScreen2);

    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// third help screen.
/// </summary>
function GUIHelpTabScreen3Browse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the third help screen's image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIHelpTabScreen3Browse::setSelectedAsset(%this, %asset)
{
    howToPlayGui.howToPlayScreen3 = GuiTool.assignHelpScreen(%asset, GUIHelpTabScreen3);

    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// fourth help screen.
/// </summary>
function GUIHelpTabScreen4Browse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the fourth help screen's image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIHelpTabScreen4Browse::setSelectedAsset(%this, %asset)
{
    howToPlayGui.howToPlayScreen4 = GuiTool.assignHelpScreen(%asset, GUIHelpTabScreen4);

    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// fifth help screen.
/// </summary>
function GUIHelpTabScreen5Browse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the fifth help screen's image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIHelpTabScreen5Browse::setSelectedAsset(%this, %asset)
{
    howToPlayGui.howToPlayScreen5 = GuiTool.assignHelpScreen(%asset, GUIHelpTabScreen5);

    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// sixth help screen.
/// </summary>
function GUIHelpTabScreen6Browse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the sixth help screen's image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIHelpTabScreen6Browse::setSelectedAsset(%this, %asset)
{
    howToPlayGui.howToPlayScreen6 = GuiTool.assignHelpScreen(%asset, GUIHelpTabScreen6);

    SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// next button's selected state.
/// </summary>
function GUIHelpTabForwardBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the next button's selected 
/// button state image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIHelpTabForwardBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIHelpTabForwardBtn);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns the next button's selected button state image to the preview.
/// </summary>
function GUIHelpTabForwardBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUIHelpTabForwardBtn);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// back button's selected state.
/// </summary>
function GUIHelpTabBackBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the back button's selected 
/// button state image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIHelpTabBackBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIHelpTabBackBtn);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns the back button's selected button state image to the preview.
/// </summary>
function GUIHelpTabBackBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUIHelpTabBackBtn);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// close button's selected state.
/// </summary>
function GUIHelpTabCloseBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the close button's selected 
/// button state image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUIHelpTabCloseBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUIHelpTabCloseBtn);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns the close button's selected button state image to the preview.
/// </summary>
function GUIHelpTabCloseBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUIHelpTabCloseBtn);
}
//-----------------------------------------------------------------------------
// ** Credits Tab
//-----------------------------------------------------------------------------
/// <summary>
/// This function initializes the Credits Screen tab controls.
/// </summary>
function GuiTool::initCreditsTab(%this)
{
   GUICreditsTabMenuBtnCheckbox.setStateOn(mainMenuGui.showCreditsButton);
   
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(creditsBackground.bitmap));
   GUICreditsTabBackgroundPreview.setBitmap(GetFullFileName(%tempBmpPath));
   GUICreditsTabBackgroundEdit.text = fileName(%tempBmpPath);
   GUICreditsTabBackgroundEdit.setActive(false);
   
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(closeCreditsButton.bitmapNormal));
   GUICreditsTabCloseBtnPreview.BitmapNormal = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(closeCreditsButton.bitmapHilight));
   GUICreditsTabCloseBtnPreview.BitmapHilight = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(closeCreditsButton.bitmapDepressed));
   GUICreditsTabCloseBtnPreview.BitmapDepressed = GetFullFileName(%tempBmpPath);
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(closeCreditsButton.bitmap));
   GUICreditsTabCloseBtnPreview.setBitmap(GetFullFileName(%tempBmpPath));
   GUICreditsTabCloseBtnEdit.text = fileName(%tempBmpPath);
   GUICreditsTabCloseBtnEdit.setActive(false);
   
   %tempBmpPath = GetGuiImageFileRelativePath(fileName(creditsGuiCreditsImage.bitmap));
   GUICreditsTabCreditsPreview.setBitmap(GetFullFileName(%tempBmpPath));
   GUICreditsTabCreditsEdit.text = fileName(%tempBmpPath);
   GUICreditsTabCreditsEdit.setActive(false);

   //GUICreditsTabCreditsTextFileEdit.text = creditsGui.creditsTxtFile;
   //GUICreditsTabCreditsTextFileEdit.setActive(false);

   GUIToolRefreshStateDropdown(GUICreditsTabCloseBtnDropdown);
}

/// <summary>
/// This function sets the flag to show or hide the credits button on the main menu.
/// </summary>
function GUICreditsTabMenuBtnCheckbox::onClick(%this)
{
   SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns a text file for the credits screen (unused currently).
/// </summary>
function GUICreditsTabCreditsTextFileBrowse::onClick(%this)
{
    if (GUIToolOpenTextFile(GUICreditsTabCreditsTextFileEdit))
        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// credits screen background.
/// </summary>
function GUICreditsTabBackgroundBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the credits screen background.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUICreditsTabBackgroundBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUICreditsTabBackground);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// close button's selected state.
/// </summary>
function GUICreditsTabCloseBtnBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the close button's selected 
/// button state image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUICreditsTabCloseBtnBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUICreditsTabCloseBtn);

        SetGuiToolDirtyState(true);
}

/// <summary>
/// This function assigns the close button's selected button state image to the preview.
/// </summary>
function GUICreditsTabCloseBtnDropdown::onSelect(%this)
{
    GuiTool.setButtonPreview(%this, GUICreditsTabCloseBtn);
   }

/// <summary>
/// This function opens the Asset Library to select an image for the 
/// credits.
/// </summary>
function GUICreditsTabCreditsBrowse::onClick(%this)
{
   AssetLibrary.open(%this, $GuisPage);
}

/// <summary>
/// This function assigns the selected asset to the credits image.
/// </summary>
/// <param name="asset">The name of the selected image asset.</param>
function GUICreditsTabCreditsBrowse::setSelectedAsset(%this, %asset)
{
    GuiTool.assignAsset(%asset, GUICreditsTabCredits);

        SetGuiToolDirtyState(true);
}