//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function BuildUpdateManifest()
{
    echoSeparator();
    echo("Building Update Manifest");
    
    // Make sure that the staging directory exists.  We need to put this in a
    // location other than one of the 3SS directories.  The ResourceManager is
    // set to scan all directories starting at the executable location.  This
    // process ends up locking all ZIP files, which means we cannot delete them
    // later on.
    %path = CleanUpdateManifestStaging();
    echo("The staging path will be: '" @ %path @ "'");
    
    // Allow the canvas to display the echo        
    resetCanvas();
    
    // Create the XML document
    %xml = new SimXMLDocument();
    %xml.addHeader();
    
    %xml.pushNewElement("Manifest");
    %xml.setAttribute("Version", "1");
    %xml.setAttribute("Type", "ProductInformation");
    
    %xml.pushNewElement("Product");
    %xml.setAttribute("Name", "3SS Shipping");
    
    // Modules
    BuildModuleManifest(%xml, %path);

    // Product Element    
    %xml.popElement();
    
    // Manifest Element
    %xml.popElement();
    
    // Save the manifest
    %manifestPath = pathConcat(%path, "updatemanifest.xml");
    %xml.saveFile(%manifestPath);
    echo("Update manifest file is: '" @ %manifestPath @ "'"); 

    echo("Update Manifest Complete");
    echoSeparator();
}

function BuildModuleManifest(%xml, %path)
{
    // Get the module directory path
    %moduleDirPath = expandPath("^modules");
    
    // Create the ZipObject we will use to build the archives
    %zip = new ZipObject();
    
    // Create the Modules element.  All modules will be children of
    // this element.
    %xml.pushNewElement("Modules");
   
    // Step through all of the modules
    %modules = ModuleDatabase.findModules(false);
    %size = getWordCount(%modules);
    for(%i=0; %i<%size; %i++)
    {
        %module = getWord(%modules, %i);

        %zippath = pathConcat(%path, %module.Signature @ ".zip");
        
        // If the zip archive already exists then delete it
        if(isFile(%zippath))
        {
            fileDelete(%zippath);
        }
        
        // Create the zip archive for the module
        %result = %zip.openArchive(%zippath, "readwrite");
        if(!%result)
        {
            echo("Could not create ZIP file " @ %zippath);
            
            // Allow the canvas to display the echo        
            resetCanvas();
            
            continue;
        }
        
        echo("Building " @ %zippath);

        // Allow the canvas to display the echo        
        resetCanvas();
        
        // Process each file within the module
        %totalFileSize = 0;
        %findPath = %module.ModulePath @ "/*";
        %file = findFirstFile(%findPath);
        while(%file !$= "")
        {
            if(isFile(%file))
            {
                // Track the total file size for the module
                %totalFileSize += fileSize(%file);
                
                // Store the file within the archive
                %relativePath = makeRelativePath(%file, %module.ModulePath);
                %zip.addFile(%file, %relativePath);
            }
            
            %file = findNextFile(%findPath);
        }
        
        // Done with zip archive
        %zip.closeArchive();
        
        // Create the module element
        %xml.pushNewElement("Module");
        
        // Write the attributes to the module element
        %xml.setAttribute("Id", %module.ModuleId);
        %xml.setAttribute("Version", %module.VersionId);
        %xml.setAttribute("Build", %module.BuildId);
        %xml.setAttribute("LocalSize", %totalFileSize);
        %xml.setAttribute("DownloadName", fileName(%zippath));
        %xml.setAttribute("DownloadSize", fileSize(%zippath));
        %xml.setAttribute("CRC", getFileCRC(%zippath));
        %xml.setAttribute("Depreciated", %module.Deprecated);
        
        // Write the optional OS attribute
        if(%module.OS !$= "")
        {
           %xml.setAttribute("OS", %module.OS);
        }

        // Remove the module element from the stack
        %xml.popElement();
    }
   
    // Remove the Modules element from the stack
    %xml.popElement();
    
    %zip.delete();
}
