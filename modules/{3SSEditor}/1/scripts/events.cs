//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
function Editor::onStartUpComplete(%this, %messageData)
{
    $UpdateReadyToInstall = false;
    
    EditorEventManager.schedule(0, postEvent, "_StartUserLogInCheck");
}

function Editor::onProjectToolsLoaded(%this, %messageData)
{
    // Show the editor shell
    Canvas.setContent(EditorShellGui);
    
    // Clear all the views from the shell
    EditorShellGui.clearViews();
    
    // Push the template selector view
    EditorShellGui.addView(TemplateListGui, "");
    EditorShellGui.addView(AnnouncementsGui, "");
    EditorShellGui.setToolBar(HomeScreenToolbar);
}

function Editor::onProjectLoaded(%this, %messageData)
{
    EditorShellGui.clearViews();
    %module = ModuleDatabase.findLoadedModule(LBProjectObj.templateModule);
    %toolsGroup = %module.ToolsGroup;
    
    ModuleDatabase.loadGroup(%toolsGroup);
    EditorShellGui.setCommonToolBar(CommonToolbar);
}

function Editor::onCoreShutdown(%this, %messageData)
{
}

function Editor::onUserRequiresLogIn(%this, %messageData)
{
    OpenLogInScreen(%messageData);
}

function Editor::onUserLoggedIn(%this, %messageData)
{
    EditorEventManager.schedule(0, postEvent, "_GetUserProfile");
}

function Editor::onStartUserRequestOfflineMode(%this, %messageData)
{
    // Present the user with the option of going offline
    OpenOfflineModeRequest();
}

function Editor::onUpdateAvailable(%this, %messageData)
{
    $UpdateReadyToInstall = true;
    EditorShellGui.displayUpdateBadge($UpdateReadyToInstall);
    AnnouncementsGui.refresh();
}

function Editor::onUpdateInstallStart(%this, %messageData)
{
    echo("@@@ Update service is installing, push GUI and stop input");
}

function Editor::onUpdateInstallEnd(%this, %messageData)
{
    $UpdateReadyToInstall = false;
    EditorShellGui.displayUpdateBadge($UpdateReadyToInstall);
    AnnouncementsGui.refresh();
}

function Editor::onContactingLoginServer(%this, %messageData)
{
    OpenConnectingToServerScreen();
}

function Editor::onUserProfileAcquired(%this, %messageData)
{
    if (isFile($WebServices::ProfileImagePath))
    {
        if($WebServicess::AvatarAsset $= "")
        {
            %avatarAsset = new ImageAsset()
            {
                AssetInternal = "1";
                ImageFile = $WebServices::ProfileImagePath;
            };
        
            $WebServicess::AvatarAsset = AssetDatabase.addPrivateAsset(%avatarAsset);
        }
        else
        {
            // Re-load the image from the disk
            AssetDatabase.reloadAsset($WebServicess::AvatarAsset);
        }
    }
    else
    {
        %avatarAsset = "";
        error("% - User profile image not found");
    }

    %loadSuccess = ModuleDatabase.loadGroup("projectTools");
    
    if (%loadSuccess)
        EditorEventManager.postEvent("_ProjectToolsLoaded");
    
    // Check if there are any files to download based on the user's manifest
    EditorEventManager.schedule(0, postEvent, "_UpdateStartCheck");
}

// THIS IS A VERY UNIQUE CALLBACK. It is only used when the user profile
// picture is finished downloading. The call is made in:
// {3SSWebServices/1/scripts/user.cs, UserServices::onUserInfoAcquired
function Editor::onDownloadFinished(%this, %downloadManager, %filesize)
{
    %downloadManager.RemoveCurrentFileFromQueue();
    EditorEventManager.postEvent("_UserProfileAcquired", %data);
}

function Editor::onDownloadProgress(%this, %downloadManager, %bytesRead, %bytesTotal)
{
    if (isDebugBuild())
    {
        %currentFile = %downloadManager.GetCurrentFileURL();
        echo("Editor::onDownloadProgress(): " @ %currentFile SPC %bytesRead @ " / " @ %bytesTotal);
    }
}

function Editor::onDownloadError(%this, %downloadManager, %errorText)
{
    error("Editor::onDownloadError(): " @ %errorText);
}

function showMainEditor()
{
    Canvas.setContent(EditorShellGui);
}