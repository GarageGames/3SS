//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
function Editor::onStartUpComplete(%this, %messageData)
{
    // This is where we will push the login screen, but since we do not have
    // one yet, go straight to the project tools
    %loadSuccess = ModuleDatabase.loadGroup("projectTools");
    
    if (%loadSuccess)
        EditorEventManager.postEvent("_ProjectToolsLoaded");
}

function Editor::onProjectToolsLoaded(%this, %messageData)
{
    // Present the shell GUI
    Canvas.setContent(T2DStartPage);
}

function Editor::onProjectLoaded(%this, %messageData)
{
}

function Editor::onAssetToolsLoaded(%this, %messageData)
{
}

function Editor::onAdvancedToolsLoaded(%this, %messageData)
{
}

function Editor::onCoreShutdown(%this, %messageData)
{
}

function Editor::onUserRequiresLogIn(%this, %messageData)
{
}

function Editor::onUserLoggedIn(%this, %messageData)
{
}

function Editor::onStartUserRequestOfflineMode(%this, %messageData)
{
}

function Editor::onUpdateAvailable(%this, %messageData)
{
}

function Editor::onContentDownloadComplete(%this, %messageData)
{
}

function Editor::onContactingLoginServer(%this, %messageData)
{
}