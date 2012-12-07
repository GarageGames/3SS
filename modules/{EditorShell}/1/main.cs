//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
function initializeEditorShell(%scopeSet)
{
    //-----------------------------------------------------------------------------
    // Execute scripts
    //-----------------------------------------------------------------------------
    exec("./scripts/gui.cs");
    exec("./scripts/commonToolbar.cs");
    
    //-----------------------------------------------------------------------------
    // Load GUIs
    //-----------------------------------------------------------------------------
    %scopeSet.add( TamlRead("./gui/commonToolbar.gui.taml") );
    %scopeSet.add( TamlRead("./gui/mainEditorShell.gui.taml") );
    
    //-----------------------------------------------------------------------------
    // Initialization
    //----------------------------------------------------------------------------- 
    $BaseEditorRefreshState::All = 0;
    $BaseEditorRefreshState::ToolView = 1;
    $BaseEditorRefreshState::ToolBar = 2;
    $BaseEditorRefreshState::CommonToolBar = 3;
    $BaseEditorRefreshState::HintsAndTips = 4;
    $BaseEditorSpacing = 12;
    $EditorShellGui::ViewHeight = 560;
}

function destroyEditorShell()
{
   EditorShellGui.destroy();
}
