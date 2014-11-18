//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function initializeProjectManager(%scopeSet)
{
    //-----------------------------------------------------------------------------
    // Load scripts
    //-----------------------------------------------------------------------------
    exec("./scripts/projectEvents.cs");
    exec("./scripts/projectInternalInterface.cs");    
    exec("./scripts/applicationClose.cs");
    exec("./scripts/dragDrop.cs");
    exec("./scripts/t2dProject.cs");
    
    //-----------------------------------------------------------------------------
    // Load GUIs
    //-----------------------------------------------------------------------------
    %scopeSet.add( TamlRead("./gui/openProjectDlg.gui.taml") );
    %scopeSet.add( TamlRead("./gui/projectOptionsDlg.gui.taml") );
    
    //-----------------------------------------------------------------------------
    // Initialization
    //-----------------------------------------------------------------------------
    $ProjectFilesLocation = expandPath("^tool/templates/projectFiles/");
    $ExercisesLocation = expandPath("^tool/exercises/");
    $UserGamesLocation = getUserHomeDirectory() @ "/3StepStudio";
}

function destroyProjectManager()
{
}