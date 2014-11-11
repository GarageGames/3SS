/// <summary>
/// This function is called from the win and lose screens' replay buttons.  
/// It restarts the last level.
/// </summary>
/// <param name="dialogToPop">The dialog we're clearing to restart the level.</param>
function replay(%dialogToPop)
{
    if(!$levelLoading)
    {
        $levelLoading = true;
        schedule(100, 0, loadLevel);
    }

    Canvas.schedule(1000, popDialog, %dialogToPop);
}

/// <summary>
/// This function is called from the win, lose and pause screens's main menu buttons.
/// It loads the Main Menu.
/// </summary>
/// <param name="dialogToPop">This is the dialog we're clearing to show the main menu.</param>
function goToMainMenu(%dialogToPop)
{
    // Ensure that the ScheduleManager is reinitialized before next level
    if (isObject(MainScene))
        MainScene.clear();
    
    if (isObject(ScheduleManager))
        ScheduleManager.initialize();
        
    Canvas.schedule(50, popDialog, %dialogToPop);
    Canvas.schedule(50, pushDialog, mainMenuGui);
}