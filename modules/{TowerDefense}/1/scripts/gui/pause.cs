/// <summary>
/// This function loads the main menu.
/// </summary>
function pauseMainMenuButton::onClick(%this)
{
   sceneWindow2D.endLevel();

   // Ensure that the ScheduleManager is reinitialized before next level
   ScheduleManager.initialize();   
   Canvas.popDialog(pauseGui);
   
   Canvas.pushDialog(mainMenuGui);
}