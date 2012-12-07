//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
GuiFormManager::RegisterLibrary("LevelBuilderTowerDefenseTemplate", "levelBuilder/layouts");

$LBTemplateSidebarTowerDefense = GuiFormManager::AddFormContent("LevelBuilderTemplateSidebar", "TowerDefenseTemplate", "LBTowerDefenseToolCreate::CreateContent", "LBTowerDefenseToolCreate::SaveContent", 2);

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBTowerDefenseToolCreate::CreateContent(%contentCtrl)
{
   %extent = %contentCtrl.getExtent();
   %extentX = GetWord(%extent, 0);
   %extentY = GetWord(%extent, 1);
   
   %base = new GuiTabPageCtrl() 
   {
      Text = "Tower Defense Template Tools";
      internalName = "TowerDefenseTemplatePage";
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
      //class = "LBSideBarTowerDefenseMessaging";
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
   %count = GuiFormManager::GetFormContentCount("LevelBuilderTowerDefenseTemplate");
   
   %contentCount = 0;
   %lastContentHeight = 0;
   
   for(%i = 0; %i < %count; %i++)
   {
      %contentObj = GuiFormManager::GetFormContentByIndex("LevelBuilderTowerDefenseTemplate", %i);
	  
      if (!isObject(%contentObj))
         continue;

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
            GuiFormManager::AddContentReference("LevelBuilderTowerDefenseTemplate", %contentObj.Name, %result);               
      }
   }
   
   activatePackage(TowerDefensePackage);
   
   LevelBuilderMenu.initialize();
   
   return %base;
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBTowerDefenseToolCreate::SaveContent(%contentCtrl)
{
   // Nothing.
}

package TowerDefensePackage
{
   function CreateNewSpriteSheet(%tag)
   {
      //switch$ (%tag)
      //{
         //case "Enemy":
            //echo("@@@ Enemy creating new sprite sheet");
         //case "Tower":
            //echo("@@@ Tower creating new sprite sheet");
         //case "Projectile":
            //echo("@@@ Projectile creating new sprite sheet");
         //case "Terrain":
            //echo("@@@ Terrain creating new sprite sheet");
         //default:
            //launchNewImageMap(true);
      //}
      
      launchNewImageMap(true);
   }

   function CreateNewAnimation(%tag)
   {
      //switch$ (%tag)
      //{
         //case "Enemy":
            //echo("@@@ Enemy creating new animation");
         //case "Tower":
            //echo("@@@ Tower creating new animation");
         //case "Projectile":
            //echo("@@@ Projectile creating new animation");
         //case "Terrain":
            //echo("@@@ Terrain creating new animation");
         //default:
            //AnimationBuilder.createAnimation();
      //}
      
      AnimationBuilder.createAnimation();
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
      %templateName = "towerTemplate";
      %newName = "UntitledLevel";    

      // Get the template directory path
      %templateDir = $templatesDirectory @ "/TowerDefense/levelTemplates/";

      // Get the project levels path
      %projectLevelsDir = expandPath("^project/data/levels/");

      // Get the path to the level template and datablock
      %levelTemplate = %templateDir @ %templateName @ ".t2d";
      %levelTemplateDatablock = %templateDir @ "datablocks/" @ %templateName @ "_datablocks.cs";
      echo(" >< New Level " @ %levelTemplate);
      echo(" >< New Level Datablocks " @ %levelTemplateDatablock);

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
      //switch$ (%tag)
      //{
         //case "Enemy":
            //echo("@@@ Enemy opening sprite sheet");
         //case "Tower":
            //echo("@@@ Tower opening sprite sheet");
         //case "Projectile":
            //echo("@@@ Projectile opening sprite sheet");
         //case "Terrain":
            //echo("@@@ Terrain opening sprite sheet");
         //default:
            //launchEditImageMap(%datablockName);
      //} 
      
      launchEditImageMap(%datablockName);
   }

   function EditAnimation(%datablockName, %tag)
   {
      //switch$( %tag )
      //{
         //case "Enemy":
            //echo("@@@ Enemy opening animation");
         //case "Tower":
            //echo("@@@ Tower opening animation");
         //case "Projectile":
            //echo("@@@ Projectile opening animation");
         //case "Terrain":
            //echo("@@@ Terrain opening animation");
         //default:
            //launchEditAnimation(%datablockName);
      //}
      
      launchEditAnimation(%datablockName);
   }

   function EditSoundProfile(%datablockName, %tag)
   {
      //switch$( %tag )
      //{
         //case "Enemy":
            //echo("@@@ Enemy opening audio profile");
         //case "Tower":
            //echo("@@@ Tower opening audio profile");
         //case "Projectile":
            //echo("@@@ Projectile opening audio profile");
         //case "Terrain":
            //echo("@@@ Terrain opening audio profile");
         //default:
            //launchSoundImporter(%datablockName);
      //}
      
      launchSoundImporter(%datablockName);
   }

   function EditLevel(%datablockName, %tag)
   {
      //switch$( %tag )
      //{
         //case "Enemy":
            //echo("@@@ Enemy opening level");
         //case "Tower":
            //echo("@@@ Tower opening level");
         //case "Projectile":
            //echo("@@@ Enemey opening level");
         //case "Terrain":
            //echo("@@@ Enemey opening level");
         //default:
            //ToolManager.getLastWindow().setFirstResponder();
            //%projectPath = expandPath("^project/data/levels/");
            //%file = %projectPath @ %datablockName @ ".t2d";
            //LBProjectObj.openLevel(%file);
            //AssetLibrary.close();
      //}
      
      ToolManager.getLastWindow().setFirstResponder();
      %projectPath = expandPath("^project/data/levels/");
      %file = %projectPath @ %datablockName @ ".t2d";
      LBProjectObj.openLevel(%file);
      AssetLibrary.close();
   }

   function EditBitmapFont(%datablockName, %tag)
   {
      //switch$( %tag )
      //{
         //case "Enemy":
            //echo("@@@ Enemy opening bitmap font");
         //case "Tower":
            //echo("@@@ Tower opening bitmap font");
         //case "Projectile":
            //echo("@@@ Projectile opening bitmap font");
         //case "Terrain":
            //echo("@@@ Terrain opening bitmap font");
         //default:
            //launchFontTool(%datablockName);
      //}
      
      launchFontTool(%datablockName);
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

         item[0] = "General Settings" TAB "" TAB "Canvas.pushDialog(GeneralSettings);";
         item[1] = "Interface Tool" TAB "" TAB "Canvas.pushDialog(GuiTool);";
         item[2] = "Enemy Tool" TAB "" TAB "Canvas.pushDialog(EnemyTool);";
         item[3] = "Tower Tool" TAB "" TAB "Canvas.pushDialog(TowerTool);";
         item[4] = "Projectile Tool" TAB "" TAB "Canvas.pushDialog(ProjectileTool);";
         item[5] = "Terrain Tool" TAB "" TAB "Canvas.pushDialog(TerrainTool);";
         item[6] = "Wave Tool" TAB "" TAB "Canvas.pushDialog(WaveTool);";
         item[7] = "Level Tool" TAB "" TAB "Canvas.pushDialog(LevelTool);";
      };      
         
      // Submenus will be deleted when the menu they are in is deleted
      LevelBuilderBase.menuGroup.add(%fileMenu);
      LevelBuilderBase.menuGroup.add(%toolsMenu);
   }
};