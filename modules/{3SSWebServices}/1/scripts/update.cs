//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function UserUpdateServices::onUpdateStartCheck(%this, %messageData)
{
    %this.totalAlreadyDownloadedModules = 0;
    %this.totalModules = 0;
    %this.totalDownloadSize = 0;    
    %this.downloadDelay = 0;
    
    // Clear any download records.
    %this.clear();
        
    // Get the paths
    %paths = PrepareSynchronizeStaging();
    %this.downloadPath = getRecord(%paths, 0);
    %this.modulesPath = getRecord(%paths, 1);
        
    // Get the manifest from the user services.  This manifest was obtained
    // when the user logged in.
    %manifest = UserServices.GetManifestSimObjectId();
    
    // Load up the previous download results.
    %this.readPreviousDownloads();
    
    // Go through each module
    %result = %manifest.pushFirstChildElement("Module");
    while(%result)
    {
        %this.processModuleManifest(%manifest, %this.downloadPath);
        %result = %manifest.nextSiblingElement("Module");
    }
    
    // If there are any modules to download start that now
    if(%this.downloadManager.GetQueueCount() > 0)
    {
        EditorEventManager.postEvent("_UpdateDownloadingStart");
        %this.downloadNextModule();
    }
    else
    {
        // Check if we have modules that are already downloaded and just need
        // to be installed.  In this case we'll post an event as if all downloading
        // is complete, because it is!
        if(%this.totalAlreadyDownloadedModules > 0)
        {
            EditorEventManager.postEvent("_UpdateDownloadingEnd");
        }
        else
        {
            // There are no modules to update
            EditorEventManager.postEvent("_UpdateNoUpdates");
        }
    }   
}

//-----------------------------------------------------------------------------

function UserUpdateServices::onUpdateInstallStart(%this, %messageData)
{   
    // Fetch downloads.
    %downloads = %this.previousDownloads;
    %downloadCount = %downloads.getCount();

    // Iterate modules.
    for( %index = 0; %index < %downloadCount; %index++ )
    {
        // Fetch module.
        %module = %downloads.getObject( %index );
                      
        // Create module path.
        %modulePath = CreateModuleStagingPath( %module.moduleId, %module.versionId );
        
        if(%modulePath $= "")
        {
            echo( "Could not create staging directory for module " @ %module.moduleId @ " version " @ %module.version );
            continue;
        }        

        // Unpacking the module.
        %zip = new ZipObject();
        %zipPath = %this.downloadPath @ %module.internalName @ ".zip";
        %result = %zip.openArchive(%zipPath);
        
        // Did we open the zip?        
        if(%result)
        {
            // Yes, so unzip it.
            %count = %zip.getFileEntryCount();
            for(%i=0; %i<%count; %i++)
            {
                %entry = %zip.getFileEntry(%i);
                %internalPath = getField(%entry, 0);
                %filepath = pathConcat(%modulePath, %internalPath);
                %result = %zip.extractFile(%internalPath, %filepath);
            }
        }
        else
        {
            // No, so warn.
            echo( "Could not open module zip file for processing:" SPC %zipPath );
        }

        // Remove the zip.
        %zip.delete();
    }
        
    // Create the module merge definition file.  This will either be used
    // immediately, or following a restart
    %merge = new moduleMergeDefinition();
    %merge.MergePath = %this.modulesPath;
    TamlWrite( %merge, "module.merge", xml );

    // Can we merge the modules now without a restart?
    %canMergeModules = ModuleDatabase.canMergeModules(%this.modulesPath);
    if ( %canMergeModules )
    {
        // Yes, so merge modules.
        %result = ModuleDatabase.mergeModules("modules", true, true);
                
        // Clean-up the staging area.
        CleanSynchronizeStaging();        
    }
    else
    {
        // No, so this will automatically perform a critical merge.
        %result = ModuleDatabase.mergeModules("modules", true, true);
    }
    
    // Check if we need to restart due to an OS level module being updated
    if( %this.processOSModules() )
    {
        // The file patcher module has already been loaded ready to go.
        OSFilesPatcher.LaunchPatcher();
        return;
    }    
        
    // Restart if we couldn't merge modules.
    if ( !%canMergeModules )
    {
        restartInstance();
        return;
    }
    
    // Finished install.
    EditorEventManager.postEvent("_UpdateInstallEnd", "");    
}

//-----------------------------------------------------------------------------

function UserUpdateServices::processOSModules(%this)
{    
    // Fetch downloads.
    %downloads = %this.previousDownloads;
    %downloadCount = %downloads.getCount();

    // Check if there are any OS files modules to be updated
    %hasOSModules = false;
    for( %index = 0; %index < %downloadCount; %index++ )
    {
        // Fetch module.
        %module = %downloads.getObject( %index );

        if(%module.osFiles == true)
        {
             %hasOSModules = true;
             break;
        }
    }

    // Finish if no OS modules.   
    if(!%hasOSModules)
        return false;
        
    // Load the patcher module
    ModuleDatabase.LoadExplicit( "{3SSFilesPatcher}" );

    // Start the file manifest
    OSFilesPatcher.StartFileManifest(getExecutableName());

    // Go through each OS file module
    for( %index = 0; %index < %downloadCount; %index++ )
    {
        // Fetch module.
        %module = %downloads.getObject( %index );

        if(%module.osFiles == true)
        {
            // Loading an OS module will automatically register its files with the patcher
            ModuleDatabase.loadExplicit( %module.moduleId );
        }
    }

    // Close the manifest
    OSFilesPatcher.EndFileManifest();
      
    return true;
}

//-----------------------------------------------------------------------------

function UserUpdateServices::onError(%this, %error)
{
    echo("UserUpdateServices ERROR: " @ %error);
}

//-----------------------------------------------------------------------------

function UserUpdateServices::downloadNextModule(%this)
{
    // Check if there is at least one more file to download
    if(%this.downloadManager.GetQueueCount() > 0)
    {
        %this.downloadManager.DownloadNextFile();
    }
    else
    {
        // Finished downloading all files
        EditorEventManager.postEvent("_UpdateDownloadingEnd");
    }
}

//-----------------------------------------------------------------------------

function UserUpdateServices::findModuleAndVersion(%this, %moduleId, %version)
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

//-----------------------------------------------------------------------------

function UserUpdateServices::readPreviousDownloads(%this)
{
    %this.clearPreviousDownloads();
    
    %this.previousDownloads = new ScriptGroup()
                                {
                                    internalName = "PreviousDownloads";
                                };
                                    
    addResPath(%this.downloadPath, true);
    %file = findFirstFile(%this.downloadPath @ "*.zip");
    while(%file !$= "")
    {
        %size = fileSize(%file);
        %crc = getFileCRC(%file);
        
        %this.addModuleIsDownloaded( fileBase(%file), %size, %crc, false );
        
        %file = findNextFile(%this.downloadPath @ "*.zip");
    }
    removeResPath(%this.downloadPath);
}

//-----------------------------------------------------------------------------

function UserUpdateServices::clearPreviousDownloads(%this)
{
    if(%this.previousDownloads !$= "")
    {
        %this.previousDownloads.delete();
        %this.previousDownloads = "";
    }
}

//-----------------------------------------------------------------------------

// Returns true if the requested module has been fully downloaded
function UserUpdateServices::findModuleIsDownloaded(%this, %moduleSignature, %size, %crc, %osFiles)
{
    // Find the module in the previous downloaded list
    %module = %this.previousDownloads.findObjectByInternalName(%moduleSignature);
    if(%module == 0)
        return false;

    // Check if the module has been completely downloaded, or if it is only a
    // partial download
    if(%module.size != %size || %module.crc != %crc)
    {
        echo(%moduleSignature @ " mismatch (actual/manifest): size=" @ %module.size @ "/" @ %size @ " crc=" @ %module.crc @ "/" @ %crc);
        return false;
    }
    
    // Yes, so update details.
    // NOTE: This is essential to registering any extra details from the manifest not encoded directly in the filename.
    // For now this is just the "osFiles" flag.
    %this.addModuleIsDownloaded( %moduleSignature, %size, %crc, %osFiles );

    return true;
}

//-----------------------------------------------------------------------------

function UserUpdateServices::addModuleIsDownloaded(%this, %moduleSignature, %size, %crc, %osFiles)
{
    // Expand the signature.
    %moduleId = getUnit( %moduleSignature, 0, "_" );
    %versionId = getUnit( %moduleSignature, 1, "_" );
    %buildId = getUnit( %moduleSignature, 2, "_" );                
    
    // Find the module in the previous downloaded list
    %module = %this.previousDownloads.findObjectByInternalName(%moduleSignature);
    if(%module != 0)
    {
        // Module is in the list, so update its properties
        %module.size = %size;
        %module.crc = %crc;
        %module.osFiles = %osFiles;
    }
    else
    {        
        // Module is not in the list so add it.
        %module = new ScriptObject()
                    {
                        moduleId = %moduleId;
                        versionId = %versionId;
                        buildId = %buildId;
                        internalName = %moduleSignature;
                        size = %size;
                        crc = %crc;
                        osFiles = %osFiles;
                    };
        
        // Put the downloaded module in the list
        %this.previousDownloads.add(%module);
    }
}

//-----------------------------------------------------------------------------

function UserUpdateServices::processModuleManifest(%this, %xml, %stagingPath)
{
    // Get some of the attributes
    %moduleId = %xml.attribute("Id");
    %downloadPath = %xml.attribute("DownloadPath");
    %downloadName = %xml.attribute("DownloadName");
    
    // Make sure all of the attributes are valid.  Use the string version of
    // the attributes to determine if the attribute exists on the module element.
    if(%moduleId $= "" || 
       %xml.attribute("Version") $= "" ||
       %xml.attribute("Build") $= "" ||
       %xml.attribute("LocalSize") $= "" ||
       %downloadPath $= "" ||
       %downloadName $= "" ||
       %xml.attribute("DownloadSize") $= "" ||
       %xml.attribute("CRC") $= "" ||
       %xml.attribute("Depreciated") $= "")
    {
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
    
    // Has this module already been downloaded and is just waiting to be installed?
    %signature = %moduleId @ "_" @ %version @ "_" @ %build;
    if(%this.findModuleIsDownloaded(%signature, %downloadSize, %crc))
    {
        // The module has already been fully downloaded
        echo(%signature @ " has already been downloaded");
        %this.totalAlreadyDownloadedModules++;
        return;
    }
    
    // Build up the module record for the download
    %localPath = pathConcat(%stagingPath, %signature @ ".zip");
    %moduleRecord = new ScriptObject()
                    {
                        class = UserUpdateServicesModuleDownloadClass;
                        
                        moduleId = %moduleId;
                        version = %version;
                        build = %build;
                        signature = %signature;
                        localSize = %localSize;
                        localPath = %localPath;
                        downloadPath = %downloadPath;
                        downloadName = %downloadName;
                        downloadSize = %downloadSize;
                        crc = %crc;
                        
                        owner = %this;
                        osFiles = %containsOSFiles;
                    };
    %this.add(%moduleRecord);

    // Have the module download
    %this.downloadManager.AddFileToQueue(%downloadPath, %localPath, %moduleRecord, false);
    
    %this.totalModules++;
    %this.totalDownloadSize += %downloadSize;
}

//-----------------------------------------------------------------------------

function UserUpdateServices::increaseDownloadDelay(%this)
{
    if(%this.downloadDelay < 10000)
    {
        %this.downloadDelay += 1000;
    }
    else if(%this.downloadDelay < 30000)
    {
        %this.downloadDelay += 5000;
    }
    else if(%this.downloadDelay < 60000)
    {
        %this.downloadDelay += 10000;
    }
}

//-----------------------------------------------------------------------------

function UserUpdateServices::clearDownloadDelay(%this)
{
    %this.downloadDelay = 0;
}

//-----------------------------------------------------------------------------
// UserUpdateServicesModuleDownloadClass
//-----------------------------------------------------------------------------

function UserUpdateServicesModuleDownloadClass::onWrongURLType(%this, %downloadManager)
{
    warn("UserUpdateServicesModuleDownloadClass::onWrongURLType()");
    %error = "Download error: Wrong URL type for " @ %downloadManager.GetCurrentFileURL();
    %this.owner.schedule(0, onError, %error);
}

function UserUpdateServicesModuleDownloadClass::onBadFilePath(%this, %downloadManager)
{
    warn("UserUpdateServicesModuleDownloadClass::onBadFilePath()");
    %error = "Download error: Bad file path for " @ %downloadManager.GetCurrentExpandedFilePath();
    %this.owner.schedule(0, onError, %error);
}

function UserUpdateServicesModuleDownloadClass::onDownloadProgress(%this, %downloadManager, %bytesRead, %bytesTotal)
{
    if (isDebugBuild())
    {
        echo("UserUpdateServicesModuleDownloadClass::onDownloadProgress(): " @ %bytesRead @ " / " @ %bytesTotal);
    }
}

function UserUpdateServicesModuleDownloadClass::onDownloadError(%this, %downloadManager, %errorText)
{
    error("UserUpdateServicesModuleDownloadClass::onDownloadError(): " @ %errorText);

    //%this.owner.schedule(0, onError, "Download error: " @ %errorText);
    
    // Increase the delay between download attempts
    %this.owner.increaseDownloadDelay();

    // Try again    
    %this.owner.schedule(%this.downloadDelay, downloadNextModule);
}

function UserUpdateServicesModuleDownloadClass::onDownloadFinished(%this, %downloadManager, %fileSize)
{
    echo("UserUpdateServicesModuleDownloadClass::onDownloadFinished(): " @ %this.signature @ "  Final size: " @ %fileSize);
    
    // Make sure that the file has been downloaded OK
    %file = %downloadManager.GetCurrentExpandedFilePath();
    %size = fileSize(%file);
    %crc = getFileCRC(%file);
    if ( false )
    //if(%this.downloadSize != %size || %this.crc != %crc)
    {
        // There was a problem downloading this file as the results don't match.
        warn("There was a problem downloading the module " @ %this.signature @ ".  Will try again.");
        fileDelete(%file);

        // Increase the delay between download attempts
        %this.owner.increaseDownloadDelay();
    }
    else
    {
        // Remove the file from the queue
        %downloadManager.RemoveCurrentFileFromQueue();
        
        // Add the module to the downloaded list and write out that list
        %this.owner.addModuleIsDownloaded(%this.signature, %this.downloadSize, %this.crc, %this.osFiles );
        
        // Clear any download delay with a good download
        %this.owner.clearDownloadDelay();
    }
    
    // Move on to the next module
    %this.owner.schedule(%this.downloadDelay, downloadNextModule);
}
