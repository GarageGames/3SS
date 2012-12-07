//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function initialize3SSEditor(%scopeSet)
{
    //-----------------------------------------------------------------------------
    // Load scripts
    //-----------------------------------------------------------------------------
    exec("./scripts/events.cs");
    exec("./scripts/newProjectDlg.cs");
    exec("./scripts/assetTextEdit.cs");
    exec("./scripts/guiUtils.cs");
    exec("./scripts/projectOptionsDlg.cs");
    exec("./scripts/3SSConfirmDeleteProject.cs");
    exec("./scripts/verticalScrollContainer.cs");
    
    //-----------------------------------------------------------------------------
    // Load GUIs
    //-----------------------------------------------------------------------------
    %scopeSet.add( TamlRead("./gui/newProjectDlg.gui.taml") );
    %scopeSet.add( TamlRead("./gui/projectOptionsDlg.gui.taml") );
    %scopeSet.add( TamlRead("./gui/3SSConfirmDeleteProjectGui.gui.taml") );

    //-----------------------------------------------------------------------------
    // Register Collision Editor open and close events
    //-----------------------------------------------------------------------------
    if (!EditorEventManager.isRegisteredEvent("_TSSCollisionEditorOpen"))
        EditorEventManager.registerEvent("_TSSCollisionEditorOpen");

    if (!EditorEventManager.isRegisteredEvent("_TSSCollisionEditorClose"))
        EditorEventManager.registerEvent("_TSSCollisionEditorClose");
}

function destroy3SSEditor()
{
}

function onExecuteDone(%thing)
{
   restoreWindow();
}