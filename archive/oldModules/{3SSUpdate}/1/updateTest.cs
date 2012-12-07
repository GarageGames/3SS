//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function testUserUpdate()
{
    if(!isObject(UserUpdateTest))
    {
        new ScriptObject(UserUpdateTest)
        {
            class = "UserUpdateTestClass";
        };
        EditorEventManager.subscribe(UserUpdateTest, "_UpdateDownloadingEnd", "onDownloadComplete");
    }
    
    // Start the update
    EditorEventManager.schedule(0, postEvent, "_UpdateStartCheck");
}

function UserUpdateTestClass::onDownloadComplete(%this)
{
    // Everything is downloaded.  Start the merge.
    EditorEventManager.schedule(0, postEvent, "_UpdateMergeFiles");
}
