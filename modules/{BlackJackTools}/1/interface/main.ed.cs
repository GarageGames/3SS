//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
GuiFormManager::RegisterLibrary("LevelBuilderBlackJackTemplate", "levelBuilder/layouts");

$LBTemplateSidebarBlackJack = GuiFormManager::AddFormContent("LevelBuilderTemplateSidebar", "BlackJackTemplate", "LBBlackJackToolCreate::CreateContent", "LBBlackJackToolCreate::SaveContent", 2);

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBBlackJackToolCreate::CreateContent(%contentCtrl)
{
   %extent = %contentCtrl.getExtent();
   %extentX = GetWord(%extent, 0);
   %extentY = GetWord(%extent, 1);
   
   %base = new GuiTabPageCtrl() 
   {
      Text = "BlackJack Template Tools";
      internalName = "BlackJackTemplatePage";
      canSaveDynamicFields = "0";
      Profile = "EditorTabPage";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = %extent;
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
   };
   
   %scroll = new GuiScrollCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorTransparentScrollProfile";
      HorizSizing = "width";
      VertSizing = "height";
      position = "0 0";
      Extent = "500 400";
      MinExtent = "72 64";
      canSave = "1";
      visible = "1";
      hovertime = "1000";
      willFirstRespond = "1";
      hScrollBar = "alwaysOff";
      vScrollBar = "alwaysOn";
      constantThumbHeight = "1";
      childMargin = "0 4";
   };
   %base.add(%scroll);
   
   //%stack = new GuiStackControl() 
   //{
      //StackingType = "Vertical";
      //HorizStacking = "Left to Right";
      //VertStacking = "Top to Bottom";
      //Padding = "0";
      //canSaveDynamicFields = "0";
      //internalName = "editStack";
      //class = "LBSideBarBlackJackMessaging";
      //Profile = "EditorTransparentProfile";
      //HorizSizing = "width";
      //VertSizing = "height";
      //Position = "0 0";
      //Extent = "480 400";
      //MinExtent = "24 24";
      //canSave = "1";
      //Visible = "1";
      //hovertime = "1000";
   //};
   //
   //%scroll.add(%stack);
   
   // Add Base to Form
   %contentCtrl.add(%base);
   
   // Load dynamic edit bar panels
   %count = GuiFormManager::GetFormContentCount("LevelBuilderBlackJackTemplate");
   
   %contentCount = 0;
   %lastContentHeight = 0;
   
   for(%i = 0; %i < %count; %i++)
   {
      %contentObj = GuiFormManager::GetFormContentByIndex("LevelBuilderBlackJackTemplate", %i);
	  
      if (%contentObj.CreateFunction !$= "")
      {
         %result = eval(%contentObj.CreateFunction @ "(" @ %scroll @ ");");
         
         if(%contentCount >= 1)
         {
            %xPosition = getWord(%result.getPosition(), 0);
            
            %yPosition = %lastContentHeight;
            
            %result.setPosition(%xPosition, %yPosition+5);
         }
         
         %contentCount++;
         
         %lastContentHeight = getWord(%result.getExtent(), 1);
         
         if (isObject(%result))
            GuiFormManager::AddContentReference("LevelBuilderBlackJackTemplate", %contentObj.Name, %result);               
      }
   }
   
   activatePackage(BlackJackPackage);
   
   LevelBuilderMenu.initialize();
   
   AddBlackjackTypesToAssetLibrary();
   
   
   return %base;
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBBlackJackToolCreate::SaveContent(%contentCtrl)
{
   // Nothing.
}

package BlackJackPackage
{
function CreateNewSpriteSheet(%tag)
{
   switch$ (%tag)
   {
      case "Table":
         echo("@@@ Table creating new sprite sheet");
         launchNewImageMap(true);
      case "Carpet":
         echo("@@@ Carpet creating new sprite sheet");
         launchNewImageMap(true);
      case "Chips":
         echo("@@@ Chips creating new sprite sheet");
         launchNewImageMap(true);
      case "Shoe":
         echo("@@@ Shoe creating new sprite sheet");
         launchNewImageMap(true);
      case "HUD":
         echo("@@@ HUD creating new sprite sheet");
         launchNewImageMap(true);
      default:
         launchNewImageMap(true);
   } 
}

function CreateNewAnimation(%tag)
{
   switch$( %tag )
   {
      case "Cards":
         echo("@@@ Cards creating new animation");
         //AnimationBuilder.createAnimation();
      case "Table":
         echo("@@@ Table creating new animation");
         //AnimationBuilder.createAnimation();
      case "Carpet":
         echo("@@@ Carpet creating new animation");
         //AnimationBuilder.createAnimation();
      case "Chips":
         echo("@@@ Chips creating new animation");
         //AnimationBuilder.createAnimation();
      case "Shoe":
         echo("@@@ Shoe creating new animation");
         //AnimationBuilder.createAnimation();
      case "HUD":
         echo("@@@ HUD creating new animation");
         //AnimationBuilder.createAnimation();
      default:
         //AnimationBuilder.createAnimation();
         
   }
}

function CreateNewGuiImage(%tag)
{
   // Get the game's gui/images directory
   %gameGuiDir = LBProjectObj.gamePath @ "/gui/images/";
   
   %dlg = new OpenFileDialog()
   {
      Filters = $T2D::ImageMapSpec;
      ChangePath = false;
      MustExist = true;
      MultipleFiles = true;
   };
   
   if (%dlg.Execute())
   {
      %file = %dlg.files[0];
      %fileOnlyName = fileName(%file);
      %fileOnlyBase = fileBase(%file);
      %fileOnlyExtension = fileExt(%file);
      
      // Get the new location of the file
      %fileLocation = %gameGuiDir @ %fileOnlyName;
      
      // Check if the file-base already exists at the new location
      if (isFileIgnoreExtension(%fileLocation))
      {
            // Find the next available file name
            %i = 1;
            %newFileOnlyName = %fileOnlyName;
            %newFileOnlyBase = %fileOnlyBase;
            %newFileLocation = %gameGuiDir @ %fileOnlyName;
            while (isFileIgnoreExtension(%gameGuiDir @ %newFileOnlyName))
            {
                  %i++;
                  %newFileOnlyBase = %fileOnlyBase @ "_" @ %i;
                  %newFileOnlyName = %newFileOnlyBase @ %fileOnlyExtension;
                  %newFileLocation = %gameGuiDir @ %newFileOnlyName;
            }      
         
             AssetLibraryDialog.setupAndShow(AssetLibraryWindow.getGlobalCenter(), "File already exists", 
                                             "A file called \"" @ %fileOnlyBase @ "\" already exists."
                                             @ "\n\nWould you like to replace it or create a new file called \""  
                                             @ %newFileOnlyBase @ "\"?", 
                                             "Replace", "copyGuiAssetToPath(\"" @ %file @ "\",\"" @ %fileLocation @ "\");", 
                                             "Create New", "copyGuiAssetToPath(\"" @ %file @ "\",\"" @ %newFileLocation @ "\");", 
                                             "Cancel", "");
      }
      else
      {
         pathCopy( %file, %fileLocation );
         AssetLibrary.refresh(true);
      }
   }
   
   
   
   %dlg.delete();
}

// Checks if a file exists, comparing the file base ignoring file extension
function isFileIgnoreExtension(%file)
{
   %filePath = filePath(%file);
   %fileBase = fileBase(%file);
   
   for (%f = findFirstFile(%filePath @ "*"); %f !$= ""; %f = findNextFile(%filePath @ "*"))
   {
      if(%fileBase $= fileBase(%f))
      {
         return true;
      }
   }
   
   return false;
}

function removeFileIgnoreExtension(%file)
{
   %filePath = filePath(%file);
   %fileBase = fileBase(%file);
   
   %f = findFirstFile(%filePath @ "*");
   while ( isFile(%f) )
   {
      if(%fileBase $= fileBase(%f))
      {
         echo("Removing file: " @ %f);
         fileDelete(%f);
      }
      %f = findNextFile(%filePath @ "*");
   }
}

// Wrapper for pathCopy to ensure it gets called in a tools script
function copyGuiAssetToPath(%oldLoc, %newLoc)
{
   // Remove any previous files with the same file base
   removeFileIgnoreExtension(%newLoc);
   
   // Copy the asset
   pathCopy( %oldLoc, %newLoc );
   AssetLibrary.refresh(true);
}

function CreateNewAudioProfile(%tag)
{
   launchSoundImporter("");
}

function CreateNewLevel(%tag)
{
    %templateName = "tableTemplate";
    %newName = "UntitledLevel";    
    
    // Get the template directory path
    %templateDir = $templatesDirectory @ "/BlackJack/levelTemplates/";
    
    // Get the project levels path
    %projectLevelsDir = expandPath("^project/data/levels/");
    
    // Get the path to the level template and datablock
    %levelTemplate = %templateDir @ %templateName @ ".t2d";
    %levelTemplateDatablock = strreplace(%levelTemplate, %templateName @ ".t2d", "datablocks/" @ %templateName @ "_datablocks.cs");

    if (isFile(%levelTemplate) && isFile(%levelTemplateDatablock))
    {
        // Find the next available file name
        %i = 1;
        %newLevel = %projectLevelsDir @ %newName @ %i @ ".t2d";
        while (isFile(%newLevel))
        {
            %i++;
            %newLevel = %projectLevelsDir @ %newName @ %i @ ".t2d";
        }
        
        // copy level file
        pathCopy(%levelTemplate, %newLevel);
        
        // copy level datablock
        %newDatablock = strreplace(%newLevel, %newName @ %i @ ".t2d", "datablocks/" @ %newName @ %i @ "_datablocks.cs");
        pathCopy(%levelTemplateDatablock, %newDatablock);
    }
    
   AssetLibrary.schedule(100, "updateGui");
   
   return %newLevel;
}
function CreateNewBitmapFont(%tag)
{
   launchFontTool("");
}

function EditSpriteSheet(%datablockName, %tag)
{
   switch$( %tag )
   {
      case "Cards":
         echo("@@@ Cards opening sprite sheet");
         for(%i = 0; %i < GeneratedDecks.getCount(); %i++)
         {
            if(GeneratedDecks.getObject(%i).internalName $= %datablockName)
            {
               $DeckTool::currentDeckEdit = GeneratedDecks.getObject(%i);
               break;
            }
         }
         
         Canvas.pushDialog(CustomDeckToolGui);
         
      case "Table":
         echo("@@@ Table opening sprite sheet");
         launchEditImageMap(%datablockName);
      case "Carpet":
         echo("@@@ Carpet opening sprite sheet");
         launchEditImageMap(%datablockName);
      case "Chips":
         echo("@@@ Chips opening sprite sheet");
         launchEditImageMap(%datablockName);
      case "Shoe":
         echo("@@@ Shoe opening sprite sheet");
         launchEditImageMap(%datablockName);
      case "HUD":
         echo("@@@ HUD opening sprite sheet");
         launchEditImageMap(%datablockName);
      default:
         if(%datablockName.NameTags == 1)
         {
            for(%i = 0; %i < GeneratedDecks.getCount(); %i++)
            {
               if(GeneratedDecks.getObject(%i).internalName $= %datablockName)
               {
                  $DeckTool::currentDeckEdit = GeneratedDecks.getObject(%i);
                  break;
               }
            }
            
            Canvas.pushDialog(CustomDeckToolGui);
         }
         else
            launchEditImageMap(%datablockName);
   } 
}

function EditAnimation(%datablockName, %tag)
{
   switch$( %tag )
   {
      case "Cards":
         echo("@@@ Cards opening animation");
         launchEditAnimation(%datablockName);
      case "Table":
         echo("@@@ Table opening animation");
         launchEditAnimation(%datablockName);
      case "Carpet":
         echo("@@@ Carpet opening animation");
         launchEditAnimation(%datablockName);
      case "Chips":
         echo("@@@ Chips opening animation");
         launchEditAnimation(%datablockName);
      case "Shoe":
         echo("@@@ Shoe opening animation");
         launchEditAnimation(%datablockName);
      case "HUD":
         echo("@@@ HUD opening animation");
         launchEditAnimation(%datablockName);
      default:
         launchEditAnimation(%datablockName);
   }
}

function EditSoundProfile(%datablockName, %tag)
{
   switch$( %tag )
   {
      case "Cards":
         echo("@@@ Cards opening SoundProfile");
         launchSoundImporter(%datablockName);
      case "Table":
         echo("@@@ Table opening SoundProfile");
         launchSoundImporter(%datablockName);
      case "Carpet":
         echo("@@@ Carpet opening SoundProfile");
         launchSoundImporter(%datablockName);
      case "Chips":
         echo("@@@ Chips opening SoundProfile");
         launchSoundImporter(%datablockName);
      case "Shoe":
         echo("@@@ Shoe opening SoundProfile");
         launchSoundImporter(%datablockName);
      case "HUD":
         echo("@@@ HUD opening SoundProfile");
         launchSoundImporter(%datablockName);
      default:
         launchSoundImporter(%datablockName);
   }
}

function EditLevel(%datablockName, %tag)
{
         ToolManager.getLastWindow().setFirstResponder();
         %projectPath = expandPath("^project/data/levels/");
         %file = %projectPath @ %datablockName @ ".t2d";
         LBProjectObj.openLevel(%file);
         AssetLibrary.close();
}

function EditBitmapFont(%datablockName, %tag)
{
   switch$( %tag )
   {
      case "Cards":
         echo("@@@ Cards opening bitmap font");
         launchFontTool(%datablockName);
      case "Table":
         echo("@@@ Table opening bitmap font");
         launchFontTool(%datablockName);
      case "Carpet":
         echo("@@@ Carpet opening bitmap font");
         launchFontTool(%datablockName);
      case "Chips":
         echo("@@@ Chips opening bitmap font");
         launchFontTool(%datablockName);
      case "Shoe":
         echo("@@@ Shoe opening bitmap font");
         launchFontTool(%datablockName);
      case "HUD":
         echo("@@@ HUD opening bitmap font");
         launchFontTool(%datablockName);
      default:
         launchFontTool(%datablockName);
   }
}

function LevelBuilderMenu::initialize( %this )
{
   if( isObject( LevelBuilderBase.menuGroup ) )
      LevelBuilderBase.menuGroup.delete();
      
   LevelBuilderBase.menuGroup = new SimGroup();
   
   //-----------------------------------------------------------------------------
   // File Menu
   //-----------------------------------------------------------------------------    
   %nonMacMenu = 0;
   if( $platform $= "macos" )
      %nonMacMenu = -1000;

   %fileMenu = new PopupMenu(FileMenu)
   {
      superClass = "MenuBuilder";
      
      barPosition = 0;
      barName     = "File";      
      
      item[0] = "New Game..." TAB "" TAB "Canvas.setContent(MainStartGui);";
      item[1] = "Open Level" TAB $cmdCtrl SPC "O" TAB "AssetLibrary.open(\"\", $LevelsPage, \"\");";
      item[2] = "Open Game" TAB "" TAB "Canvas.setContent(MainStartGui);";
      item[3] = "-";
      item[4] = "Test Game" TAB "" TAB "ToolManager.getLastWindow().setFirstResponder(); showPlayPlatform();";
      item[5] = "Publish Game" TAB "" TAB "buildProject();";
      item[6] = "-";
      item[7] = "Web Store" TAB "" TAB "Canvas.pushDialog(WebStoreDlg);";
      item[8] = "Online Help" TAB "" TAB "gotoWebpage(\"http://docs.3stepstudio.com\");";
      item[9] = "3 Step Studio Forums" TAB "" TAB "gotoWebpage(\"http://boards.3stepstudio.com\");";
      item[10] = "About 3 Step Studio" TAB "" TAB "TGBInsiderDlg.showAbout();";
      item[11] = "-";
      item[12] = "Close Game" TAB "" TAB "Canvas.setContent(MainStartGui);";
           
      // the mac os application menu already has a quit item, yay! no need to duplicate it here!
      // we therefore hide this next entry
      item[%nonMacMenu + 14] = "Quit" TAB "" TAB "quit();";
   };

   //-----------------------------------------------------------------------------
   // Tools Menu
   //-----------------------------------------------------------------------------
   %toolsMenu = new PopupMenu(ToolsMenu)
   {
      superClass = "MenuBuilder";
      
      barPosition = 1;
      barName     = "Tools";      

      item[0] = "General Settings" TAB "" TAB "Canvas.pushDialog(GeneralSettingsEditorGui);";
      item[1] = "Interface Tool" TAB "" TAB "Canvas.pushDialog(InterfaceEditorGUI);";
      item[2] = "Table Tool" TAB "" TAB "Canvas.pushDialog(TableEditorGUI);";
      item[3] = "NPC Player Tool" TAB "" TAB "Canvas.pushDialog(AiEditorGui);";
      item[4] = "NPC Strategy Tool" TAB "" TAB "Canvas.pushDialog(AiCardEditorGui);";
      item[5] = "Custom Deck Tool" TAB "" TAB "Canvas.pushDialog(CustomDeckToolGui);";
   };      
      
   // Submenus will be deleted when the menu they are in is deleted
   LevelBuilderBase.menuGroup.add(%fileMenu);
   LevelBuilderBase.menuGroup.add(%toolsMenu);

}

function AddBlackjackTypesToAssetLibrary()
{
   ALTemplateTypesLabel.setText("Blackjack");
   
   $BlackjackDecksPage = 6;
   
   new GuiIconButtonCtrl(ALBlackjackDecksButton) 
   {
      canSaveDynamicFields = "0";
      isContainer = "0";
      Profile = "ALPageButtonProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "7 325";
      Extent = "158 32";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      Command = "AssetLibrary.setPage($BlackjackDecksPage);";
      tooltipprofile = "GuiDefaultProfile";
      hovertime = "1000";
      text = "Decks";
      groupNum = "1";
      buttonType = "RadioButton";
      useMouseEvents = "0";
      buttonMargin = "4 4";
      iconBitmap = "^{AssetLibrary}/gui/";
      iconLocation = "Left";
      sizeIconToButton = "0";
      textLocation = "Center";
      textMargin = "4";
   };
   
   AssetLibraryWindow.add(ALBlackjackDecksButton);
}

function SetTemplateTypeButtonsActiveState(%active)
{
   ALBlackjackDecksButton.setActive(%active);
}

function ActivateTemplateTypeButton(%type)
{
   if (%type == $BlackjackDecksPage)
      ALBlackjackDecksButton.setActive(true);
}

function SelectTemplateTypeButton(%type)
{
   if (%type == $BlackjackDecksPage)
      ALBlackjackDecksButton.setStateOn(true);
}

function GetTemplateTypesTagExclusions()
{
   return "Deck";
}

function GetTemplateTypePageTagFilter(%type)
{
   if (%type == $BlackjackDecksPage)
      return "Deck";
      
      
   return "";
}

function GetTemplateTypeFromPage(%page)
{
   if (%page == $BlackjackDecksPage)
      return "ImageAsset";
      
      
   return "";
}

function CreateNewTemplateType(%type, %tag)
{
   if (%type == $BlackjackDecksPage)
   {
      $DeckTool::currentDeckEdit = $DeckTool::NewDeck;
      Canvas.pushDialog(CustomDeckToolGui);
   }
}

};