//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function initializeQtManager(%scopeSet)
{
    if(!isObject(QtPlatformManager))
    {
        new QtManager(QtPlatformManager);
        
        %scopeSet.add(QtPlatformManager);
        
        EditorEventManager.subscribe(QtPlatformManager, "_StartUserLogOut", "onUserLogOutEvent");
        
        QtPlatformManager.loadCookies("^CacheFileLocation/3SSData/c.dat");
    }
}

function destroyQtManager()
{
    if(isObject(QtPlatformManager))
    {
        EditorEventManager.remove(QtPlatformManager, "_StartUserLogOut");
        
        if($WebServices::AutoLogIn == false)
        {
            // If the user should not automatically log in, then clear
            // all cookies.
            QtPlatformManager.clearCookies();
        }
        
        QtPlatformManager.saveCookies("^CacheFileLocation/3SSData/c.dat");
    }
}

function QtPlatformManager::onUserLogOutEvent(%this, %messageData)
{
    echo("QtPlatformManager::onUserLogOutEvent");
    
    // Clear all network cookies on log out
    %this.clearCookies();
}
