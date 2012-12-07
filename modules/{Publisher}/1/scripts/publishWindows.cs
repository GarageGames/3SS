//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$publishingWindowsDirId = 0;
$publishingWindowsFileId = 0;

function publishWindows( %outputPath, %stagingPath, %executableFilePath, %gameName )
{
    // Configure installer arguments.
    %executableFile = fileName(%executableFilePath);
    %upgradeUUID = generateWiXUUID();
    %productName = %gameName;
    %productDescription = "3SS Game";
    %productVersion = "1.0.0.0";
    %productCompany = "GarageGames";
    
    // Generate a directory tree.
    $publishingWindowsDirId = 0;
    $publishingWindowsFileId = 0;
    %rootNode = generateWindowsDirectoryTree( new SimGroup(), %stagingPath, %executableFile );      
    
    // Generate the WiX document according to the WiX schema.
    %wixFile = new SimXMLDocument();
    %wixFile.addHeader();
    
    %wixFile.pushNewElement("Wix");
        %wixFile.setAttribute( "xmlns", "http://schemas.microsoft.com/wix/2006/wi" );
        
            %wixFile.pushNewElement("Product");
                %wixFile.setAttribute( "Id", "*" );
                %wixFile.setAttribute( "UpgradeCode", %upgradeUUID );
                %wixFile.setAttribute( "Language", "1033" );
                %wixFile.setAttribute( "Name", %productName );
                %wixFile.setAttribute( "Version", %productVersion );
                %wixFile.setAttribute( "Manufacturer", %productCompany );
                
                    %wixFile.pushNewElement("Package");
                        %wixFile.setAttribute( "Id", "*" );
                        %wixFile.setAttribute( "InstallerVersion", "300" );
                        %wixFile.setAttribute( "Compressed", "yes" );       
                    %wixFile.popElement(); // Package.
                    
                    %wixFile.pushNewElement("Media");
                        %wixFile.setAttribute( "Id", "1" );
                        %wixFile.setAttribute( "Cabinet", "game.cab" );
                        %wixFile.setAttribute( "EmbedCab", "yes" );       
                    %wixFile.popElement(); // Media.
                    
                    %wixFile.pushNewElement("Property");
                        %wixFile.setAttribute( "Id", "WIXUI_INSTALLDIR" );
                        %wixFile.setAttribute( "Value", "INSTALLDIR" );
                    %wixFile.popElement(); // Property.
                    
                    %wixFile.pushNewElement("UIRef");
                        %wixFile.setAttribute( "Id", "WixUI_InstallDir" );
                    %wixFile.popElement(); // UIRef.
                    
                    %wixFile.pushNewElement("CustomAction");
                        %wixFile.setAttribute( "Id", "LaunchApplication" );
                        %wixFile.setAttribute( "FileKey", %executableFile );
                        %wixFile.setAttribute( "ExeCommand", "" );
                        %wixFile.setAttribute( "Execute", "immediate" );
                        %wixFile.setAttribute( "Impersonate", "yes" );
                        %wixFile.setAttribute( "Return", "asyncNoWait" );
                    %wixFile.popElement(); // CustomAction.

                    %wixFile.pushNewElement("Condition");
                        %wixFile.setAttribute( "Message", "This application is only supported on Windows Vista, Windows Server 2008, or higher." );
                        %wixFile.addData( "Installed OR (VersionNT >= 600)" );
                    %wixFile.popElement(); // Condition.

                    %wixFile.pushNewElement("InstallUISequence");
                        %wixFile.pushNewElement("LaunchConditions");
                            %wixFile.setAttribute( "After", "AppSearch" );
                        %wixFile.popElement(); // LaunchConditions.
                    %wixFile.popElement(); // InstallUISequence.

                    %wixFile.pushNewElement("InstallExecuteSequence");
                        %wixFile.pushNewElement("LaunchConditions");
                            %wixFile.setAttribute( "After", "AppSearch" );
                        %wixFile.popElement(); // LaunchConditions.
                    %wixFile.popElement(); // InstallExecuteSequence.
                   
                    // This is how to include a license file later.            
                    //%wixFile.pushNewElement("WixVariable");
                    //    %wixFile.setAttribute( "Id", "WixUILicenseRtf" );
                    //    %wixFile.setAttribute( "Value", "license.rtf" );
                    //%wixFile.popElement(); // WixVariable.

                    %wixFile.pushNewElement("DirectoryRef");
                        %wixFile.setAttribute( "Id", "ApplicationProgramsFolder" );                        
                            %wixFile.pushNewElement("Component");                           
                                %wixFile.setAttribute( "Id", "ApplicationShortcut" );
                                %wixFile.setAttribute( "Guid", "*" );
                                
                                    %wixFile.pushNewElement("Shortcut");
                                        %wixFile.setAttribute( "Id", "ApplicationStartMenuShortcut" );
                                        %wixFile.setAttribute( "Name", %productName );
                                        %wixFile.setAttribute( "Description", %productDescription );
                                        %wixFile.setAttribute( "Target", "[INSTALLDIR]" @ %executableFile );
                                        %wixFile.setAttribute( "WorkingDirectory", "INSTALLDIR" );
                                    %wixFile.popElement(); // Shortcut.
                                    
                                    %wixFile.pushNewElement("Shortcut");
                                        %wixFile.setAttribute( "Id", "UninstallProduct" );
                                        %wixFile.setAttribute( "Name", "Uninstall" SPC %productName );
                                        %wixFile.setAttribute( "Description", "Uninstalls" SPC %productName );
                                        %wixFile.setAttribute( "Target", "[SystemFolder]msiexec.exe" );
                                        %wixFile.setAttribute( "Arguments", "/x [ProductCode]" );
                                    %wixFile.popElement(); // Shortcut.  
                                    
                                    %wixFile.pushNewElement("RemoveFolder");
                                        %wixFile.setAttribute( "Id", "ApplicationProgramsFolder" );
                                        %wixFile.setAttribute( "On", "uninstall" );
                                    %wixFile.popElement(); // RemoveFolder.  

                                    %wixFile.pushNewElement("RegistryValue");
                                        %wixFile.setAttribute( "Root", "HKCU" );
                                        %wixFile.setAttribute( "Key", "Software/" @ %productCompany @ "/" @ %productName );
                                        %wixFile.setAttribute( "Name", "installed" );
                                        %wixFile.setAttribute( "Type", "integer" );
                                        %wixFile.setAttribute( "Value", "1" );
                                        %wixFile.setAttribute( "KeyPath", "yes" );
                                    %wixFile.popElement(); // RegistryValue.  
                                    
                            %wixFile.popElement(); // Component.                            
                    %wixFile.popElement(); // DirectoryRef.
                    
                    %wixFile.pushNewElement("Directory");
                        %wixFile.setAttribute( "Id", "TARGETDIR" );                        
                        %wixFile.setAttribute( "Name", "SourceDir" );
                        
                            generateWixFonts( %wixFile );
                                                
                            %wixFile.pushNewElement("Directory");
                                %wixFile.setAttribute( "Id", "ProgramFilesFolder" );                        
                                    %wixFile.pushNewElement("Directory");
                                        %wixFile.setAttribute( "Id", "INSTALLDIR" );                        
                                        %wixFile.setAttribute( "Name", %productName );                        
                                            generateWixDirectories( %rootNode, %wixFile );
                                    %wixFile.popElement(); // Directory.
                            %wixFile.popElement(); // Directory.

                            %wixFile.pushNewElement("Directory");
                                %wixFile.setAttribute( "Id", "ProgramMenuFolder" );                        
                                    %wixFile.pushNewElement("Directory");
                                        %wixFile.setAttribute( "Id", "ApplicationProgramsFolder" );                        
                                        %wixFile.setAttribute( "Name", %productName );                        
                                    %wixFile.popElement(); // Directory.
                            %wixFile.popElement(); // Directory.                            
                    %wixFile.popElement(); // Directory.
                                        
                    %wixFile.pushNewElement("DirectoryRef");
                        %wixFile.setAttribute( "Id", "INSTALLDIR" );
                            %wixFile.pushNewElement("Component");                           
                                %wixFile.setAttribute( "Id", %executableFile );
                                %wixFile.setAttribute( "Guid", "*" );
                                    %wixFile.pushNewElement("File");
                                        %wixFile.setAttribute( "Id", %executableFile );
                                        %wixFile.setAttribute( "Source", %executableFile );
                                        %wixFile.setAttribute( "KeyPath", "yes" );
                                        %wixFile.setAttribute( "Vital", "yes" );
                                        %wixFile.setAttribute( "Checksum", "yes" );
                                    %wixFile.popElement(); // File.
                            %wixFile.popElement(); // Component.
                    %wixFile.popElement(); // DirectoryRef.
                    
                    generateWixFiles( %rootNode, %wixFile );                    
                                        
                    %wixFile.pushNewElement("Feature");                    
                        %wixFile.setAttribute( "Id", "MainApplication" );
                        %wixFile.setAttribute( "Title", "Main Application" );
                        %wixFile.setAttribute( "Level", "1" );
                            %wixFile.pushNewElement("ComponentRef");
                                %wixFile.setAttribute( "Id", "ApplicationShortcut" );
                            %wixFile.popElement(); // ComponentRef.                        
                            %wixFile.pushNewElement("ComponentRef");
                                %wixFile.setAttribute( "Id", %executableFile );
                            %wixFile.popElement(); // ComponentRef.                                                        
                            generateWixFileFeatures( %rootNode, %wixFile );
                            generateWixFontFeatures( %wixFile );

                    %wixFile.popElement(); // Feature.
                                        
            %wixFile.popElement(); // Product.
            
    %wixFile.popElement(); // Wix.
    
    // Calculate WiX declaration.
    %wixDeclarationTarget = %outputPath @ "/game.wxs";
    // Save WiX configuration file.
    %wixFile.saveFile( %wixDeclarationTarget );   
    %wixFile.delete();

    // Calculate WiX tool location.
    %wixToolPath = expandPath("^{WiXTool}/WiX");    
    
    // Compile WiX declaration.
    %candleArgs = "\"" @ %wixDeclarationTarget @ "\"" SPC "-out " @ "\"" @ %outputPath @ "/game.wxobj" @ "\"";
    shellExecuteBlocking( %wixToolPath @ "/candle.exe", %candleArgs, %wixToolPath );
    
    // Link WiX object.
    %lightArgs = "\"" @ %outputPath @ "/game.wxobj" @ "\"" SPC "-ext WixUIExtension" SPC "-b" SPC "\"" @ %stagingPath @ "\"" SPC "-out" SPC "\"" @ %outputPath @ "/" @ %gameName @ ".msi" @ "\"";
    shellExecuteBlocking( %wixToolPath @ "/light.exe", %lightArgs, %wixToolPath );
    
    // Delete WiX temporary objects and files.
    %rootNode.delete();
    fileDelete( %wixDeclarationTarget );
    fileDelete( %outputPath @ "/game.wxobj" );
    fileDelete( %outputPath @ "/" @ %gameName @ ".wixpdb" );
}

//-----------------------------------------------------------------------------

function generateWiXUUID()
{
    return strupr( strreplace( createUUID(), "-", "" ) );
}

//-----------------------------------------------------------------------------

// Generate a directory tree using pre-order traversal.  This MUST be in pre-order!
function generateWindowsDirectoryTree( %node, %path, %excludeFile )
{   
    // Do we have a directory Id?
    if ( %node.directoryId $= "" )
    {
        // No, so this must be the install directory.
        %node.directoryId = "INSTALLDIR";       
    }
    
    // Fetch files.
    %files = getFileList( %path );
    
    // Fetch file count.
    %fileCount = getUnitCount( %files, "\t" );

    // Reset file index.
    %fileIndex = 0;
    
    // Do we have any files?
    if ( %fileCount > 0 )
    {
        // Yes, so add them.
        for( %index = 0; %index < %fileCount; %index++ )
        {
            // Fetch file.
            %file = getUnit(%files, %index, "\t");

            // Skip if this is the excluded file.
            if ( %file $= %excludeFile )
                continue;
            
            // Store it.
            %node.file[%fileIndex] = %node.directoryRelative $= "" ? %file : %node.directoryRelative @ "/" @  %file;
            %node.fileId[%fileIndex] = "__file" @ $publishingWindowsFileId++;
            
            // Next file index.
            %fileIndex++;
        }                    
    }
    
    // Set final file count.
    %node.fileCount = %fileIndex;
    
    // Fetch directories.
    %directories = getDirectoryList( %path );
    
    // Fetch directory count.
    %directoryCount = %node.directoryCount = getUnitCount( %directories, "\t" );
    
    // Do we have any directories?
    if ( %directoryCount > 0 )
    {       
        // Yes, so add them and recurse each.
        for( %index = 0; %index < %directoryCount; %index++ )
        {
            // Fetch directory.
            %directory = getUnit(%directories, %index, "\t");
            
            // Create folder node.
            %folderNode = new SimGroup();

            // Set its directory.
            %folderNode.directoryId = "__dir" @ $publishingWindowsDirId++;            
            %folderNode.directoryName = %directory;
            %folderNode.directoryRelative = %node.directoryRelative $= "" ? %directory : %node.directoryRelative @ "/" @ %directory;
                                
            // Add it to the file set.
            %node.add( %folderNode );
                        
            // Generate directory tree.
            generateWindowsDirectoryTree( %folderNode, %path @ "/" @ %directory, %excludeFile );
        }                            
    }
    
    return %node;    
}

//-----------------------------------------------------------------------------

function generateWixDirectories( %node, %wixFile )
{
    // Fetch directory count.
    %directoryCount = %node.getCount();

    // Finish if no directories.
    if ( %directoryCount == 0 )
        return;    
        
    // Generate directories.
    for( %index = 0; %index < %directoryCount; %index++ )
    {
        // Fetch directory node.
        %directoryNode = %node.getObject( %index );
            
        // Generate element.
        %wixFile.pushNewElement("Directory");
            %wixFile.setAttribute( "Id", %directoryNode.directoryId );
            %wixFile.setAttribute( "Name", %directoryNode.directoryName );
                // Generate sub-elements.
                generateWixDirectories( %directoryNode, %wixFile );
        %wixFile.popElement(); // Directory.
    }
}

//-----------------------------------------------------------------------------

function generateWixFiles( %node, %wixFile )
{
    // Fetch file count.
    %fileCount = %node.fileCount;
    
    // Are there any files?
    if ( %fileCount > 0 )
    {
        // Yes, so generate files.
        for( %index = 0; %index < %fileCount; %index++ )
        {
            // Fetch file and Id.
            %file = %node.file[%index];
            %fileId = %node.fileId[%index];
                
            // Generate element.
            %wixFile.pushNewElement("DirectoryRef");
                %wixFile.setAttribute( "Id", %node.directoryId );
                    %wixFile.pushNewElement("Component");
                        %wixFile.setAttribute( "Id", %fileId );
                        %wixFile.setAttribute( "Guid", "*" );
                            %wixFile.pushNewElement("File");
                                %wixFile.setAttribute( "Id", %fileId );
                                %wixFile.setAttribute( "Source", %file );
                            %wixFile.popElement(); // File.
                    %wixFile.popElement(); // Component.
            %wixFile.popElement(); // DirectoryRef.
        }
    }
    else
    {
        // No, so we'll need to create a directory here explicitly.
        %wixFile.pushNewElement("DirectoryRef");
            %wixFile.setAttribute( "Id", %node.directoryId );
                %wixFile.pushNewElement("Component");
                    %wixFile.setAttribute( "Id", %node.directoryId );
                    %wixFile.setAttribute( "Guid", generateWiXUUID() );
                        %wixFile.pushNewElement("CreateFolder");
                        %wixFile.popElement(); // CreateFolder.
                %wixFile.popElement(); // Component.
        %wixFile.popElement(); // DirectoryRef.
    }
    
    // Fetch directory count.
    %directoryCount = %node.getCount();

    // Finish if no directories.
    if ( %directoryCount == 0 )
        return;    
        
    // Generate directories.
    for( %index = 0; %index < %directoryCount; %index++ )
    {
        // Fetch directory node.
        %directoryNode = %node.getObject( %index );

        // Generate WiX Files for directory.
        generateWixFiles( %directoryNode, %wixFile );
    }     
}

//-----------------------------------------------------------------------------

function generateWixFileFeatures( %node, %wixFile )
{
    // Fetch file count.
    %fileCount = %node.fileCount;
    
    // Finish if no files.
    if ( %fileCount > 0 )
    {
        // Yes, so generate file features.
        for( %index = 0; %index < %fileCount; %index++ )
        {
            // Fetch file and Id.
            %fileId = %node.fileId[%index];            
                
            // Generate element.
            %wixFile.pushNewElement("ComponentRef");
                %wixFile.setAttribute( "Id", %fileId );
            %wixFile.popElement(); // ComponentRef.
        }
    }
    else
    {
        // No, so generate folder feature.
        %wixFile.pushNewElement("ComponentRef");
            %wixFile.setAttribute( "Id", %node.directoryId );
        %wixFile.popElement(); // ComponentRef.
    }
    
    // Fetch directory count.
    %directoryCount = %node.getCount();

    // Finish if no directories.
    if ( %directoryCount == 0 )
        return;    
        
    // Generate directories.
    for( %index = 0; %index < %directoryCount; %index++ )
    {
        // Fetch directory node.
        %directoryNode = %node.getObject( %index );

        // Generate WiX Files for directory.
        generateWixFileFeatures( %directoryNode, %wixFile );
    }        
}

//-----------------------------------------------------------------------------

function generateWixFonts( %wixFile )
{   
    // Fetch font count.
    %fontCount = AvailableFonts.getCount();

    // Publish fonts folder.    
    %wixFile.pushNewElement("Directory");
        %wixFile.setAttribute( "Id", "FontsFolder" );
        %wixFile.setAttribute( "Name", "FontsFolder" );

    // Iterate fonts.
    for ( %fontIndex = 0; %fontIndex < %fontCount; %fontIndex++ )
    {    
        // Fetch font object.
        %font = AvailableFonts.getObject( %fontIndex );
 
        // Fetch font name.
        %fontName = %font.getName();
        
        // Publish font.
        %wixFile.pushNewElement("Component");                           
            %wixFile.setAttribute( "Id", %fontName );
            %wixFile.setAttribute( "Guid", generateWiXUUID() );
            %wixFile.setAttribute( "NeverOverwrite", "yes" );
            %wixFile.setAttribute( "Permanent", "yes" );
                %wixFile.pushNewElement("File");
                    %wixFile.setAttribute( "Id", %fontName );
                    %wixFile.setAttribute( "Source", expandPath( %font.File ) );
                    %wixFile.setAttribute( "TrueType", "yes" );
                %wixFile.popElement(); // File.
        %wixFile.popElement(); // Component.
    }
    
    %wixFile.popElement(); // Directory.
}

//-----------------------------------------------------------------------------

function generateWixFontFeatures( %wixFile )
{
    // Fetch font count.
    %fontCount = AvailableFonts.getCount();
    
    // Iterate fonts.
    for ( %fontIndex = 0; %fontIndex < %fontCount; %fontIndex++ )
    {    
        // Fetch font object.
        %font = AvailableFonts.getObject( %fontIndex );

        // Publish font feature.        
        %wixFile.pushNewElement("ComponentRef");                           
            %wixFile.setAttribute( "Id", %font.getName() );
        %wixFile.popElement(); // ComponentRef.                                                        
    }
}

