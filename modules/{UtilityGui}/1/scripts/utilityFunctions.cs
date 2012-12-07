
function SaveGui(%guiObject)
{
   %currentObject = %guiObject;
   
   if( %currentObject == -1 )
      return;
   
   if( %currentObject.getName() !$= "" )
      %name =  %currentObject.getName() @ ".gui";
   else
      %name = "Untitled.gui";
      
   %currentFile = %currentObject.getScriptFile();
   if( %currentFile $= GuiEditor.blankgui.getScriptFile())
      %currentFile = "";
   
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
   }
   else
      MessageBox("Torque Game Builder", "There was an error writing to file '" @ %currentFile @ "'. The file may be read-only.", "Ok", "Error" );
   
}

function GetRelativeFileName(%fileName)
{
   %tempPath = filePath(%fileName);
   %relPath = makeRelativePath(%tempPath, LBProjectObj.gamePath);
   return %relPath @ "/" @ fileName(%fileName);
}

function GetFullFileName(%fileName)
{
   %tempPath = makeFullPath(filePath(%fileName), LBProjectObj.gamePath);
   return %tempPath @ "/" @ fileName(%fileName);
}

function GetGuiImageFileRelativePath(%fileName)
{
    return "gui/images/" @ fileName(%fileName);
}

function OpenImageFileForFields(%previewImage, %textEdit, %callBack, %gameGuiDir)
{   
   if (%gameGuiDir $= "")
      %gameGuiDir = LBProjectObj.gamePath @ "/gui/images/";
   else
      %gameGuiDir = LBProjectObj.gamePath @ %gameGuiDir;   
      
   %dlg = new OpenFileDialog()
   {
      Filters = $T2D::ImageMapSpec;
      ChangePath = false;
      MustExist = true;
      MultipleFiles = true;
   };
   
   %dlg.DefaultPath = %gameGuiDir;
      
   if(%dlg.Execute())
   {
      %fileName     = %dlg.files[0];
      %fileOnlyName = fileName( %fileName );         
      
      // [neo, 5/15/2007 - #3117]
      // If the image is already in a sub dir of images don't copy it just use
      // the same path and update the image map to use it.
      //%checkPath    = expandPath( "^project/gui/images/" );
      %checkPath    = %gameGuiDir;
      %fileOnlyPath = filePath( expandPath( %fileName ) );
      // (2012/03/28) - %fileBasePath = getSubStr( %fileOnlyPath, 0, strlen( %checkPath ) );           

      // (2012/03/28) - if( %checkPath !$= %fileBasePath )         
      // (2012/03/28) - {                     
         %newFileLocation = %gameGuiDir @ %fileOnlyName;
         
         addResPath( filePath( %newFileLocation ) );
               
         pathCopy( %fileName, %newFileLocation );
      // (2012/03/28) - }            
      // (2012/03/28) - else
      // (2012/03/28) - {
         // Already exists in data/images or sub dir so just point to it
      // (2012/03/28) -    %newFileLocation = %fileName;
      // (2012/03/28) - }
      
      // Error of some sort, skip it.
      if( !isFile( %newFileLocation ) )
      {
         %dlg.delete();
         return;
      }
      
      if (isObject(%previewImage))
         %previewImage.bitmap = %fileName;
         
      if (isObject(%textEdit))
         %textEdit.text = GuiToolGetRelativeFileName(%fileName);
         
      // Fire callback
      eval(%callBack);   

   }
   %dlg.delete();
}

function OpenTextFile(%fieldName, %gameGuiDir)
{
   // %fieldName should be the name of the text edit box to populate.
   %textEdit = %fieldName;
   
   if (!%gameGuiDir)
      %gameGuiDir = LBProjectObj.gamePath @ "/gui/";
   %dlg = new OpenFileDialog()
   {
      Filters = $T2D::TextFileSpec;
      ChangePath = false;
      MustExist = true;
      MultipleFiles = true;
   };
   
   %dlg.DefaultPath = %gameGuiDir;
      
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
/// Convert a TextEdit value into an integer clamped between min and max.
/// </summary>
function ValidateTextEditInteger(%textEdit, %min, %max)
{
   %value = %textEdit.getText();
   
   if (%min !$= "Inf")
   {
      if (%value < %min)
         %value = %min;
   }
      
   if (%max !$= "Inf")
   {
      if (%value > %max)
         %value = %max;  
   }
   
   %textEdit.setText(%value);
}

/// <summary>
/// Converts a given sprite (static or animated) to the opposite type
/// </summary>
function ConvertSpriteToOtherType(%spriteToConvert)
{
   %convertedSprite = "";
   
   if (%spriteToConvert.getClassName() $= "t2dAnimatedSprite")
      %convertedSprite = new t2dStaticSprite();
   else
      %convertedSprite = new t2dAnimatedSprite();

   %convertedSprite.BodyType = "static";
             
   %fieldCount = %spriteToConvert.getFieldCount();
   
   for (%i = 0; %i < %fieldCount; %i++)
   {
      %fieldName = %spriteToConvert.getField(%i);
      
      if ((%fieldName $= "imageMap") || (%fieldName $= "animationName") || (%fieldName $= "frame") )
         continue;
      
      %convertedSprite.setFieldValue(%fieldName, %spriteToConvert.getFieldValue(%fieldName));
   }
   
   %dynamicFieldCount = %spriteToConvert.getDynamicFieldCount();
   
   for (%j = 0; %j < %dynamicFieldCount; %j++)
   {
      %dynamicFieldName = %spriteToConvert.getDynamicField(%j);
      
      // We now want to maintain this through conversions following the move of Animation Sets from
      // Asset Library to template tool
      //if (%dynamicFieldName $= "animationSet")
         //continue;
      
      %convertedSprite.setFieldValue(%dynamicFieldName, %spriteToConvert.getFieldValue(%dynamicFieldName));
   }
   
   while (%spriteToConvert.getBehaviorCount())
   {
      %behaviorInstance = %spriteToConvert.getBehaviorByIndex(0);
      %convertedSprite.addBehavior(%behaviorInstance);
      %spriteToConvert.removeBehavior(%behaviorInstance, false);
   }
      
   %name = %spriteToConvert.getName();
   
   %spriteToConvert.delete();
   
   $persistentObjectSet.remove(%spriteToConvert);

   %convertedSprite.setName(%name);
      
   $persistentObjectSet.add(%convertedSprite);
}

function renameCurrentLevelFile(%fileName)
{
    %newLevelFile = expandPath($LevelSaveDirectory @ %fileName @ ".t2d");  
    
    // Check if the new name is different than the old one
    if (LBProjectObj.currentLevelFile !$= %newLevelFile)
    {
        %oldLevelFile = LBProjectObj.currentLevelFile;
        LBProjectObj.currentLevelFile = %newLevelFile;
        $levelEditor::LastLevel = %newLevelFile;
        LBProjectObj.doSave();
        
        // Delete the old file
        echo("Removing file: " @ %oldLevelFile);
        fileDelete(%oldLevelFile);
        
        // Delete the dso file
        echo("Removing dso file: " @ %oldLevelFile @ ".dso");
        fileDelete(%oldLevelFile @ ".dso");
        
        // Delete the old datablock
        %levelFileName = fileName(%oldLevelFile);
        %levelFileName = strreplace( %levelFileName, ".t2d", "" );
        %dbFileName = strreplace(%oldLevelFile, %levelFileName @ ".t2d", "datablocks/" @ %levelFileName @ "_datablocks.cs"); 
        echo("Removing file: " @ %dbFileName);
        fileDelete(%dbFileName);
        
        return true;
    }
    else
    {
        return false;
    }
}

/// <summary>
/// Strips the module name from a fully-qualified assetID.
/// </summary>
/// <param name="assetName">  The asset ID to strip.</param>
/// <return> The unqualified asset name - not the same as the AssetName field!</return>
function stripModule(%assetName)
{
    %temp = strchr(%assetName, ":");
    %final = strreplace(%temp, ":", "");
    return %final;
}

/// <summary>
/// Checks an asset ID to determine if it is a User Asset or not.
/// </summary>
/// <param name="assetID"> The asset ID to check.</param>
/// <return> True if this is a stock asset, false if it is a user asset.</return>
function getIsStockAsset(%assetID)
{
    // This asset is stock if the asset resides anywhere except the {UserAssets} module.
    %module = AssetDatabase.getAssetModule(%assetID);
    if ( %module.ModuleId !$= "{UserAssets}" )
        return true;
    else
        return false;
}