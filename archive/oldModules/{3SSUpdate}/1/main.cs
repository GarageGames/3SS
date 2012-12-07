//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function initialize3SSUpdate()
{
    exec("./generateManifest.cs");
    exec("./fileSynchronizeManager.cs");
    exec("./updateTest.cs");
    
    new ScriptGroup(FileSynchronizeManager)
    {
        log = 0;
        downloadingManifest = false;
        processedModules = 0;
        totalModules = 0;
        
        restartRequired = false;
        
        downloadQueue = DownloadQueue;
    };
    FileSynchronizeManager.init();
}

function destroy3SSUpdate()
{
    FileSynchronizeManager.cleanUp();
    FileSynchronizeManager.delete();
}
