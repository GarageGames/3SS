//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function initialize3SSWebServices(%scopeSet)
{
    //-----------------------------------------------------------------------------
    // Execute scripts
    //-----------------------------------------------------------------------------
    exec("./scripts/defaultPrefs.cs");
    execPrefs("webServicesPrefs.cs");
    
    exec("./scripts/download.cs");
    exec("./scripts/http.cs");
    exec("./scripts/update.cs");
    exec("./scripts/user.cs");
    
    //-----------------------------------------------------------------------------
    // Initialization
    //-----------------------------------------------------------------------------
    if(!isObject(DownloadQueue))
    {
        new DownloadManager(DownloadQueue);
        DownloadQueue.SetDownloadProvider(QtPlatformManager);
    }
    
    if(!isObject(HttpQueue))
    {
        new HttpManager(HttpQueue);
        HttpQueue.SetHTTPProvider(QtPlatformManager);
    }
    
    if(!isObject(UserServices))
    {
        %siteString = getTSSWebAddress();
        
        new UserManager(UserServices)
        {
            autoLoginURL = %siteString @ "client";
            credentialsLoginURL = %siteString @ "client/login/post";
            userInfoURL = %siteString @ "client/user";
            postUsernameKey = "login[username]";
            postPasswordKey = "login[password]";
        };
        
        UserServices.SetHTTPManager(HttpQueue);
        EditorEventManager.subscribe(UserServices, "_StartUserLogInCheck", "onStartUserLogInCheckEvent");
        EditorEventManager.subscribe(UserServices, "_StartUserLogIn",      "onStartUserLogInEvent");
        EditorEventManager.subscribe(UserServices, "_StartUserLogOut",     "onStartUserLogOutEvent");
        EditorEventManager.subscribe(UserServices, "_StartUserOfflineMode","onStartUserOfflineModeEvent");
        EditorEventManager.subscribe(UserServices, "_GetUserProfile", "onStartUserGetInfo");
    }
    
    if(!isObject(UserUpdateServices))
    {
        new ScriptGroup(UserUpdateServices)
        {
            downloadManager = DownloadQueue;
        };
        EditorEventManager.subscribe(UserUpdateServices, "_UpdateStartCheck", "onUpdateStartCheck");
        EditorEventManager.subscribe(UserUpdateServices, "_UpdateDownloadingEnd", "onUpdateDownloadingEnd");
        EditorEventManager.subscribe(UserUpdateServices, "_UpdateInstallStart", "onUpdateInstallStart");
        EditorEventManager.subscribe(UserUpdateServices, "_UpdateInstallEnd", "onUpdateEnd");
    }
    
    %scopeSet.add(DownloadQueue);
    %scopeSet.add(UserUpdateServices);
    %scopeSet.add(HttpQueue);
    %scopeSet.add(UserServices);
}

function destroy3SSWebServices()
{
    // Export preferences
    exportWebServicesPrefs();
    
    if(isObject(UserUpdateServices))
    {
        EditorEventManager.remove(UserUpdateServices, "_UpdateStartCheck");
        EditorEventManager.remove(UserUpdateServices, "_UpdateDownloadingEnd");
        EditorEventManager.remove(UserUpdateServices, "_UpdateInstallStart");
        EditorEventManager.remove(UserUpdateServices, "_UpdateInstallEnd");
    }
    
    if(isObject(UserServices))
    {
        EditorEventManager.remove(UserServices, "_StartUserLogInCheck");
        EditorEventManager.remove(UserServices, "_StartUserLogIn");
        EditorEventManager.remove(UserServices, "_StartUserLogOut");
        EditorEventManager.remove(UserServices, "_StartUserOfflineMode");
        EditorEventManager.remove(UserServices, "_GetUserProfile");
    }
}

function exportWebServicesPrefs()
{
    if(!$WebServices::RememberUserName)
        $WebServices::UserName = "";
    
    %prefsPath = getPrefsPath("webServicesPrefs.cs");
            
    export("$WebServices::*", %prefsPath, false, false);


    if($WebServices::AutoLogIn == false)
    {
        QtPlatformManager.clearCookies();
    }
    QtPlatformManager.saveCookies("^CacheFileLocation/3SSData/c.dat");
}