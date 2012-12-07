//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function showUserPopup()
{
    refreshUserPop();
    Canvas.pushDialog("UserPopupGui");
}

function closeUserPopup()
{
    $WebServices::RememberUserName = UPRememberUser.getValue();
    $WebServices::AutoLogIn = UPAutoLogin.getValue();

    exportWebServicesPrefs();
    Canvas.popDialog("UserPopupGui");
}

function refreshUserPop()
{
    UPUserName.text = $WebServices::UserFullName;
    UPProfileText.setText($WebServices::UserProfileInfo);
    UPAutoLogin.setValue($WebServices::AutoLogIn);
    UPRememberUser.setValue($WebServices::RememberUserName);
    
    if ($UpdateReadyToInstall)
    {
        %versionText = "Version " @ getVersionString();
        UPUpdateNow.setVisible(true);
    }
    else
    {
        UPUpdateNow.setVisible(false);
        %versionText = "Version " @ getVersionString() SPC "  (You are up to date)";
    }
    
    UPVersionText.text = %versionText;
    
    if ($WebServicess::AvatarAsset !$= "")
        UPProfileImage.Image = $WebServicess::AvatarAsset;
}

function UserPopupGui::changePicture(%this)
{
   echo("@@@ Changing user picture");
}

function UserPopupGui::editProfile(%this)
{
   echo("@@@ Changing user profile");
}

function UserPopupGui::logout(%this)
{
    $WebServices::RememberUserName = UPRememberUser.getValue();
    $WebServices::AutoLogIn = UPAutoLogin.getValue();

    exportWebServicesPrefs();
    EditorEventManager.schedule(0, postEvent, "_StartUserLogOut");
}

function UserPopupGui::updateNow(%this)
{
   EditorEventManager.postEvent("_UpdateInstallStart", "");
}