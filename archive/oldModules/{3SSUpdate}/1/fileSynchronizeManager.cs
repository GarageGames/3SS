//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function FileSynchronizeManager::init(%this)
{
//    EditorEventManager.subscribe(%this, "_UpdateStartCheck", "start");
//    EditorEventManager.subscribe(%this, "_UpdateMergeFiles", "synchronizeModules");
    
    // Set up the application name that will be restarted when OS files
    // need updating.
//    if($platform $= "windows")
//    {
//       %this.osApp = "3StepStudio.exe";
//    }
//    else if($platform $= "macos")
//    {
//       %this.osApp = "3StepStudio.app";
//    }
    %this.osApp = getExecutableName();
}

function FileSynchronizeManager::cleanUp(%this)
{
//    EditorEventManager.remove(%this, "_UpdateStartCheck");
//    EditorEventManager.remove(%this, "_UpdateMergeFiles");
}

//-----------------------------------------------------------------------------

function FileSynchronizeManager::start(%this)
{
    // Create the log file
    %this.createLogFile();
    %this.writeLog("Update started: " @ getLocalTime());
    %this.writeLogSeparator();
    
    // Reset some properties
    %this.totalModules = 0;
    %this.processedModules = 0;
    %this.restartRequired = false;
    
    // Prepare and clean up the staging directory
    CleanSynchronizeStaging();
    %paths = PrepareSynchronizeStaging();
    %this.path = getRecord(%paths, 0);
    %this.modulesPath = getRecord(%paths, 1);
    if(%this.path $= "" || %this.modulesPath $= "")
    {
        %this.onError("Could not create synchronization staging path");
        return;
    }
    
    echo("Synchronization staging location: " @ %this.path);
    
    // Download the user specific manifest from the web service
    %this.downloadingManifest = true;
    %this.writeLog("Downloading manifest...");
    %this.downloadQueue.AddFileToQueue("http://www.gnometech.com/3SS/user/dave/usermanifest.xml", %this.path @ "manifest.xml", %this);
    %this.downloadQueue.DownloadNextFile();
}

function FileSynchronizeManager::clearModuleDownloadRecords(%this)
{
    while(%this.getCount() > 0)
    {
        %obj = %this.getObject(0);
        %obj.delete();
    }
}

function FileSynchronizeManager::onError(%this, %error)
{
    // Clear out the download queue
    %this.downloadQueue.ClearQueue();
    
    // Delete all pending module downloads
    %this.clearModuleDownloadRecords();
    
    // Close log file
    %this.closeLogFile();
    
    EditorEventManager.postEvent("_UpdateGeneralError", %error);
}

function FileSynchronizeManager::onDone(%this)
{
    %this.writeLogSeparator();
    %this.writeLog("File synchronization process complete: " @ getLocalTime());
    
    // Delete all previous module downloads
    %this.clearModuleDownloadRecords();
    
    // Close log file
    %this.closeLogFile();
}

function FileSynchronizeManager::processRemoteManifest(%this, %path)
{
    %this.writeLogSeparator();
    %this.writeLog("Processing manifest...");
    
    %xml = new SimXMLDocument();
    %result = %xml.loadFile(%path);
    if(!%result)
    {
        %this.writeLog("Could not open manifest file");
        %this.onError("Could not open manifest file");
        return;
    }
    
    // Make sure this manifest is formed correctly
    %result = %xml.pushFirstChildElement("Manifest");
    if(!%result)
    {
        %this.writeLog("Manifest file is not formed correctly");
        %this.onError("Manifest file is not formed correctly");
        return;
    }
    
    %version = %xml.attribute("Version");
    if(%version $= "" || %version !$= "1")
    {
        %this.writeLog("Manifest file is the wrong version");
        %this.onError("Manifest file is the wrong version");
        return;
    }
    
    %manifestType = %xml.attribute("Type");
    if(%manifestType $= "" || %manifestType !$= "UserSpecific")
    {
        %this.writeLog("Manifest file is the wrong type");
        %this.onError("Manifest file is the wrong type");
        return;
    }
    
    %result = %xml.pushFirstChildElement("Product");
    if(!%result)
    {
        %this.writeLog("Manifest file is missing product information");
        %this.onError("Manifest file is missing product information");
        return;
    }
    
    %result = %xml.pushFirstChildElement("Modules");
    if(!%result)
    {
        %this.writeLog("Manifest file is missing module information");
        %this.onError("Manifest file is missing module information");
        return;
    }
    
    // Go through each module
    %moduleDownloadPath = %this.path;
    %result = %xml.pushFirstChildElement("Module");
    while(%result)
    {
        %this.processModuleManifest(%xml, %moduleDownloadPath);
        %result = %xml.nextSiblingElement("Module");
    }
    
    %this.writeLog("Done processing manifest.");
    
    // If there are any modules to download start that now
    if(%this.downloadQueue.GetQueueCount() > 0)
    {
        EditorEventManager.postEvent("_UpdateDownloadingStart");
        %this.downloadNextModule();
    }
    else
    {
        // There are no modules to update
        %this.writeLogSeparator();
        %this.writeLog("All modules are up to date.");
        EditorEventManager.postEvent("_UpdateNoUpdates");
        %this.onDone();
    }
}

function FileSynchronizeManager::processModuleManifest(%this, %xml, %path)
{
    // Get some of the attributes
    %moduleId = %xml.attribute("Id");
    %downloadPath = %xml.attribute("DownloadPath");
    
    // Make sure all of the attributes are valid.  Use the string version of
    // the attributes to determine if the attribute exists on the module element.
    if(%moduleId $= "" || 
       %xml.attribute("Version") $= "" ||
       %xml.attribute("Build") $= "" ||
       %xml.attribute("LocalSize") $= "" ||
       %downloadPath $= "" ||
       %xml.attribute("DownloadSize") $= "" ||
       %xml.attribute("CRC") $= "" ||
       %xml.attribute("Depreciated") $= "")
    {
        %this.writeLog("Module is missing information [" @ %moduleId @ "]");
        %this.onError("Module is missing information [" @ %moduleId @ "]");
        return;
    }

    // Get the rest of the attributes
    %version = %xml.attributeS32("Version");
    %build = %xml.attributeS32("Build");
    %localSize = %xml.attributeS32("LocalSize");
    %downloadSize = %xml.attributeS32("DownloadSize");
    %crc = %xml.attributeS32("CRC");
    %depreciated = %xml.attributeS32("Depreciated");
    
    // Is this module depreciated?
    if(%depreciated $= "1")
    {
        // Skip this module
        return;
    }
    
    // Is this an OS specific module?
    %containsOSFiles = false;
    if(%xml.attributeExists("OS"))
    {
       %os = %xml.attribute("OS");
       if($platform $= "windows" && %os $= "Windows")
       {
          %containsOSFiles = true;
       }
       else if($platform $= "macos" && %os $= "OSX")
       {
          %containsOSFiles = true;
       }
       else
       {
          // This is an OS specific module but not for our current OS.
          // We can skip this module.
          return;
       }
    }
    
    // Does this module Id and version already exist?
    %foundModule = %this.findModuleAndVersion(%moduleId, %version);
    if(%foundModule > 0 && %foundModule.BuildId == %build)
    {
        // This module is up to date
        return;
    }
    
    // Build up the module record for the download
    %localPath = pathConcat(%path, %moduleId @ "_" @ %version @ "_" @ %build @ ".zip");
    %moduleRecord = new ScriptObject()
                    {
                        class = ModuleDownloadRecordClass;
                        
                        moduleId = %moduleId;
                        version = %version;
                        build = %build;
                        localSize = %localSize;
                        localPath = %localPath;
                        downloadPath = %downloadPath;
                        downloadSize = %downloadSize;
                        crc = %crc;
                        
                        owner = %this;
                        osFiles = %containsOSFiles;
                    };
    %this.add(%moduleRecord);

    // Have the module download
    %this.downloadQueue.AddFileToQueue(%downloadPath, %localPath, %moduleRecord);
    
    %this.writeLog(">> Will download: " @ %moduleId @ "_" @ %version @ "_" @ %build);
    
    %this.totalModules++;
}

//-----------------------------------------------------------------------------

function FileSynchronizeManager::synchronizeModules(%this)
{
    %this.writeLogSeparator();
    %this.writeLog("Synchronizing modules.");
    
    // Create the module merge definition file.  This will either be used
    // immediately, or following a restart
    %merge = new moduleMergeDefinition();
    %merge.MergePath = %this.modulesPath;
    TamlWrite( %merge, "module.merge", xml );
    
    // Check if the module merge can happen right now
    %restartRequired = false;
    if(ModuleDatabase.canMergeModules(%this.modulesPath))
    {
        EditorEventManager.postEvent("_UpdateModuleMergeStart");
        %result = ModuleDatabase.mergeModules("modules", true, true);
        EditorEventManager.postEvent("_UpdateModuleMergeEnd");
    }
    else
    {
        // Requires that we restart.  However, we still need to merge those
        // modules marked as critical.  
        %this.writeLog("Restart required for module synchronization");

        // Only critical modules will be affected by this next command
        %this.writeLog("Merging any critical modules...");
        %result = ModuleDatabase.mergeModules("modules", true, true);
        %this.writeLog("Finished merging any critical modules");
        
        %this.writeLog("Restarting instance now...");
        %restartRequired = true;
    }
    
    // Check if we need to restart due to an OS level module being updated
    %osFilesPresent = %this.processOSModules();
    
    if(%osFilesPresent)
    {
        // The {3SSFilesPatcher} module has already been loaded by processOSModules()
        OSFilesPatcher.LaunchPatcher();
    }
    else if(%restartRequired)
    {
       restartInstance();
    }
    
    %this.onDone();
}

function FileSynchronizeManager::processOSModules(%this)
{
   // Check if there are any OS files modules to be updated
   %hasOSModules = false;
   for(%i=0; %i<%this.getCount(); %i++)
   {
      %moduleRecord = getObject(%i);
      if(%moduleRecord.osFiles == true)
      {
         %hasOSModules = true;
         break;
      }
   }
   
   if(%hasOSModules)
   {
      // Load the patcher module
      ModuleDatabase.LoadExplicit( "{3SSFilesPatcher}" );
      
      // Start the file manifest
      OSFilesPatcher.StartFileManifest(%this.osApp);
      
      // Go through each OS file module
      for(%i=0; %i<%this.getCount(); %i++)
      {
         %moduleRecord = getObject(%i);
         if(%moduleRecord.osFiles == true)
         {
            %this.addOSModuleToManifest(%moduleRecord);
         }
      }
      
      // Close the manifest
      OSFilesPatcher.EndFileManifest();
      
      return true;
   }
   
   return false;
}

function FileSynchronizeManager::addOSModuleToManifest(%this, %moduleRecord)
{
   // Loading an OS module will automatically register its files with the patcher
   ModuleDatabase.loadExplicit(%moduleRecord.moduleId);
}

//-----------------------------------------------------------------------------

function FileSynchronizeManager::findModuleAndVersion(%this, %moduleId, %version)
{
    %moduleList = ModuleDatabase.findModules(false);
    
    %size = getWordCount(%moduleList);
    for(%i=0; %i<%size; %i++)
    {
        %module = getWord(%moduleList, %i);
        if(%module.ModuleId $= %moduleId && %module.VersionId == %version)
        {
            return %module;
        }
    }
    
    return 0;
}

function FileSynchronizeManager::isModuleLoaded(%this, %moduleObject)
{
    %moduleList = ModuleDatabase.findModules(true);

    %size = getWordCount(%moduleList);
    for(%i=0; %i<%size; %i++)
    {
        %module = getWord(%moduleList, %i);
        if(%module.ModuleId $= %moduleObject.ModuleId && %module.VersionId == %moduleObject.VersionId)
        {
            return true;
        }
    }
    
    return false;
}

//-----------------------------------------------------------------------------

function FileSynchronizeManager::downloadNextModule(%this)
{
    if(%this.downloadQueue.GetQueueCount() > 0)
    {
        %this.downloadQueue.DownloadNextFile();

        %this.writeLogSeparator();
        %this.writeLog("Downloading " @ %this.downloadQueue.GetCurrentFileURL() @ " to " @ %this.downloadQueue.GetCurrentExpandedFilePath() @ "...");
    }
    else
    {
        %this.writeLogSeparator();
        %this.writeLog("All modules have been downloaded.");
        EditorEventManager.postEvent("_UpdateDownloadingEnd");
        //%this.synchronizeModules();
    }
}

//-----------------------------------------------------------------------------

function FileSynchronizeManager::onWrongURLType(%this, %downloadManager)
{
    warn("FileSynchronizeManager::onWrongURLType()");
    %error = "Download error: Wrong URL type for " @ %downloadManager.GetCurrentFileURL();
    %this.writeLog(%error);
    %this.onError(%error);
}

function FileSynchronizeManager::onBadFilePath(%this, %downloadManager)
{
    warn("FileSynchronizeManager::onBadFilePath()");
    %error = "Download error: Bad file path for " @ %downloadManager.GetCurrentExpandedFilePath();
    %this.writeLog(%error);
    %this.onError(%error);
}

function FileSynchronizeManager::onDownloadProgress(%this, %downloadManager, %bytesRead, %bytesTotal)
{
    echo("FileSynchronizeManager::onDownloadProgress(): " @ %bytesRead @ " / " @ %bytesTotal);
}

function FileSynchronizeManager::onDownloadError(%this, %downloadManager, %errorText)
{
    error("FileSynchronizeManager::onDownloadError(): " @ %errorText);
    %this.writeLog("Download error: " @ %errorText);
    %this.onError("Download error: " @ %errorText);
}

function FileSynchronizeManager::onDownloadFinished(%this, %downloadManager, %fileSize)
{
    if(%this.downloadingManifest)
    {
        // The manifest has downloaded
        %this.downloadingManifest = false;
        %path = %downloadManager.GetCurrentExpandedFilePath();
        %this.writeLog("Manifest downloaded: " @ %path);
        %downloadManager.RemoveCurrentFileFromQueue();
        
        // Process the web service manifest on the next tick rather than during this callback
        %this.schedule(0, processRemoteManifest, %path);
        return;
    }
    
    echo("FileSynchronizeManager::onDownloadFinished(): " @ %downloadManager.GetCurrentExpandedFilePath() @ "  Final size: " @ %fileSize);
    %downloadManager.RemoveCurrentFileFromQueue();
    
    %downloadManager.DownloadNextFile();
}

//-----------------------------------------------------------------------------

function FileSynchronizeManager::createLogFile(%this)
{
    %this.log = new FileObject();
    %logPath = pathConcat(getMainDotCsDir(), "update.log");
    %result = %this.log.openForWrite(%logPath);
    if(!%result)
    {
        // Cannot open log file
        %this.log.delete();
        %this.log = 0;
        return;
    }
}

function FileSynchronizeManager::closeLogFile(%this)
{
    // Make sure the log file is valid
    if(!%this.log)
        return;
    
    %this.log.close();
    %this.log.delete();
    %this.log = 0;
}

function FileSynchronizeManager::writeLog(%this, %line)
{
    // Make sure the log file is valid
    if(!%this.log)
        return;
    
    %this.log.writeLine(%line);
    echo(">>> " @ %line);
}

function FileSynchronizeManager::writeLogSeparator(%this)
{
    // Make sure the log file is valid
    if(!%this.log)
        return;
    
    %this.log.writeLine("--------------------------------------------------------------------------------");

    echo(">>> - - - - - -");
}

//-----------------------------------------------------------------------------
// ModuleDownloadRecordClass
//-----------------------------------------------------------------------------

function ModuleDownloadRecordClass::onWrongURLType(%this, %downloadManager)
{
    warn("ModuleDownloadRecordClass::onWrongURLType()");
    %error = "Download error: Wrong URL type for " @ %downloadManager.GetCurrentFileURL();
    %this.owner.writeLog(%error);
    %this.owner.schedule(0, onError, %error);
}

function ModuleDownloadRecordClass::onBadFilePath(%this, %downloadManager)
{
    warn("ModuleDownloadRecordClass::onBadFilePath()");
    %error = "Download error: Bad file path for " @ %downloadManager.GetCurrentExpandedFilePath();
    %this.owner.writeLog(%error);
    %this.owner.schedule(0, onError, %error);
}

function ModuleDownloadRecordClass::onDownloadProgress(%this, %downloadManager, %bytesRead, %bytesTotal)
{
    echo("ModuleDownloadRecordClass::onDownloadProgress(): " @ %bytesRead @ " / " @ %bytesTotal);
}

function ModuleDownloadRecordClass::onDownloadError(%this, %downloadManager, %errorText)
{
    error("ModuleDownloadRecordClass::onDownloadError(): " @ %errorText);
    %this.owner.writeLog("Download error: " @ %errorText);
    %this.owner.schedule(0, onError, "Download error: " @ %errorText);
}

function ModuleDownloadRecordClass::onDownloadFinished(%this, %downloadManager, %fileSize)
{
    %zipPath = %downloadManager.GetCurrentExpandedFilePath();
    
    echo("ModuleDownloadRecordClass::onDownloadFinished(): " @ %zipPath @ "  Final size: " @ %fileSize);
    %this.owner.writeLog("finished downloading module");
    %downloadManager.RemoveCurrentFileFromQueue();
    
    %path = CreateModuleStagingPath(%this.moduleId, %this.version);
    if(%path $= "")
    {
        %error = "Could not create staging directory for module " @ %this.moduleId @ " version " @ %this.version;
        %this.owner.writeLog(%error);
        %this.owner.schedule(0, onError, %error);
        return;
    }
    
    // Unzip the module
    %this.owner.writeLog("Uncompressing module to: " @ %path);
    %zip = new ZipObject();
    %result = %zip.openArchive(%zipPath);
    if(!%result)
    {
        %error = "Could not open zip file for processing";
        %this.owner.writeLog(%error);
        %this.owner.schedule(0, onError, %error);
        return;
    }
    
    %count = %zip.getFileEntryCount();
    for(%i=0; %i<%count; %i++)
    {
        %entry = %zip.getFileEntry(%i);
        %internalPath = getField(%entry, 0);
        echo("Found file in zip: " @ %internalPath);
        %filepath = pathConcat(%path, %internalPath);
        %result = %zip.extractFile(%internalPath, %filepath);
    }
    
    %zip.delete();
    
    // Move on to the next module
    %this.owner.schedule(0, downloadNextModule);
}
