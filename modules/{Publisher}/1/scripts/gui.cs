//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function PublisherGui::onWake(%this)
{
    //PublisherXcodeText.setVisible(false);

    // If we are on Windows
    if ($platform $= "windows")
    {
        // Select windows by default.
        PublisherRadioButtonPC.performClick();
        
        // Disable iOS publishing
        PublisherRadioButtoniOS.setActive(0);
        PublisherRadioButtoniOS.ToolTip="You cannot publish to iOS from Windows.";
    }
    else
    {
        // Select OSX by default.
        PublisherRadioButtonOSX.performClick();
        
        // If we are on OS X, but there is no Xcode installed
        // Disable iOS publishing
        if (!isDirectory("/Developer/Applications/Xcode.app") && !isDirectory("/Applications/Xcode.app"))
        {
            echo("@@@ No Xcode");
            PublisherRadioButtoniOS.setActive(0);
        }
    }
}

//-----------------------------------------------------------------------------

function PublisherRadioButtonPC::onClick(%this)
{
    $PublishPlatform = "Windows";
    PublisherBrowseButton.setVisible(true);
}

//-----------------------------------------------------------------------------

function PublisherRadioButtonOSX::onClick(%this)
{
    $PublishPlatform = "OSX";
    PublisherBrowseButton.setVisible(true);
}

//-----------------------------------------------------------------------------

function PublisherRadioButtoniOS::onClick(%this)
{
    $PublishPlatform = "iOS";
    PublisherBrowseButton.setVisible(false);
}

//-----------------------------------------------------------------------------

function PublisherBrowseButton::onClick(%this)
{
    %currentFile = PublisherTextBoxDestination.getText();

    %dlg = new OpenFolderDialog()
    {
        DefaultPath = %currentFile;
    };

    if (%dlg.Execute())
        PublisherTextBoxDestination.setText(%dlg.FileName);

    %dlg.delete();
}

function Publisher_ChangeDestClick::onMouseEnter(%this)
{
    PublisherBrowseButton.setNormalImage(PublisherBrowseButton.HoverImage);
}

function Publisher_ChangeDestClick::onMouseLeave(%this)
{
    PublisherBrowseButton.setNormalImage(PublisherBrowseButton.NormalImageCache);
}

function Publisher_ChangeDestClick::onMouseDown(%this)
{
    PublisherBrowseButton.setNormalImage(PublisherBrowseButton.DownImage);
}

function Publisher_ChangeDestClick::onMouseUp(%this)
{
    PublisherBrowseButton.onClick();
    PublisherBrowseButton.setNormalImage(PublisherBrowseButton.HoverImage);
}

//-----------------------------------------------------------------------------

function PublisherBuildButton::onClick(%this)
{
    if ($PublishPlatform $= "")
    {
        NoticeGui.display("The platform to publish to was not specified!");
        return;
    }
    
    // Ensure the game has been synchronized.
    synchronizeGame();

    // Are we publishing to iOS?
    if ($PublishPlatform $= "iOS")
    {
        publishIOS();
        return;
    }

    // Do we have a destination?
    if ( PublisherTextBoxDestination.getText() $= "" )
    {
        // No, so warn.
        NoticeGui.display("An invalid path was specified for the 'Output Directory'!");
        return;
    }

    // Fetch the game name (remove any spaces).
    %gameName = strreplace(LBProjectObj.projectName, " ", "");
    
    // Fetch output directory.
    %outputPath = PublisherTextBoxDestination.getText() @ "/" @ %gameName;   

    // Does the output site already exist?
    if (isFile(%outputPath) || isDirectory(%outputPath))
    {
        // Yes, so confirm overwrite.
        ConfirmOverwriteGui.display("The output directory already exists. Overwrite?", PublisherGui, publish, "\""@%outputPath@"\"" TAB "\""@%gameName@"\"" );
        return;
    }
    
    // No, so just publish.
    PublisherGui.publish( %outputPath, %gameName );
}

function PublisherGui::publish( %this, %outputPath, %gameName )
{
    // Remove publisher dialog.
    Canvas.popDialog(PublisherGui);
    
    // Add publisher working dialog.
    Canvas.pushDialog(PublisherWorkingGui);
    
    // Start publishing the product.
    schedule( 10, 0, PublishProductStart, %outputPath, %gameName );
}

//-----------------------------------------------------------------------------

function PublishProductStart( %outputPath, %gameName )
{
    // Delete the target site.
    if ( isFile(%outputPath) || isDirectory(%outputPath) )
    {
        directoryDelete(%outputPath);
    }
    
    // Set staging site.
    %stagingPath = %outputPath @ "/_staging";
    
    // Copy to the target site.
    pathCopy( LBProjectObj.gamePath, %stagingPath, false );

    // Compile the target files.
    compilePublishedFiles( %stagingPath );
    
    // Are we publishing to windows?
    if ($PublishPlatform $= "Windows")
    {
        // Yes, so fetch windows exectuable.
        %executableFilePath = %stagingPath @ "/" @ %gameName @ ".exe";
        
        // Publish to Windows.
        publishWindows( %outputPath, %stagingPath, %executableFilePath, %gameName );
    }
    // Are we publishing to OSX?
    else if ($PublishPlatform $= "OSX")
    {
        // Yes, so fetch OSX executable.
        %executableFilePath = %stagingPath @ "/" @ %gameName @ ".app";
        
        // Publish to OSX.
        publishOSX( %outputPath, %stagingPath, %executableFilePath, %gameName );
    }
    
    // Do we have a staging path?
    if ( %stagingPath !$= "" )
    {
        // Yes, so delete it.
        directoryDelete(%stagingPath);
    }
    
    // Remove publisher dialog.
    Canvas.popDialog(PublisherWorkingGui); 
}

//-----------------------------------------------------------------------------

function compilePublishedFiles(%targetDir)
{  
    // Save compiled script state.
    %saveDSO    = $Scripts::OverrideDSOPath;
    %saveIgnore = $Scripts::ignoreDSOs;

    // Set new compiled script state.
    $Scripts::OverrideDSOPath  = %targetDir;
    $Scripts::ignoreDSOs       = false;

    // Calculate script pattern.
    %scriptPattern = %targetDir @ "/*.cs";
    
    // Calculate root script.
    %rootScript = %targetDir @ "/main.cs";

    // Add resource path.
    addresPath(%targetDir);

    // Compile and delete the script files
    for (%file = findFirstFile(%scriptPattern); %file !$= ""; %file = findNextFile(%scriptPattern))
    {
        // Compile if the file is not the root script.
        if ( %file !$= %rootScript )
        {
            compile(%file);
            fileDelete( %file );
        }
    }   
            
    // Remove resource path.
    removeResPath(%targetDir);

    // Restore compiled script state.
    $Scripts::OverrideDSOPath  = %saveDSO;
    $Scripts::ignoreDSOs       = %saveIgnore;
}

//-----------------------------------------------------------------------------

function openProjectFolder()
{
   echo("Opening Project Folder At " @ expandPath("^project/")) ;
   openFolder(expandPath("^project/"));   
}

//-----------------------------------------------------------------------------

function openXCodeProject()
{
    if($platform $= "macos")
    {
        %pathToProject = expandPath("^project/../../buildFiles/XCode/3StepStudio.xcodeproj");
        %batchFile = expandPath("./openXcode.sh");
        %cmds = expandPath("./openXcode.sh") @ "; " @ %pathToProject;

        echo("Doing : " @ %cmds);

        runBatchFile("sh", %cmds, true);
        echo("Finished running batchfile");   
    }
    else
    {
        messageBox("Error", "Cannot open XCode projects on Windows, Sorry!");
    }
}

