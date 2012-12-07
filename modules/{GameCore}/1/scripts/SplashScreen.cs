//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// Presents the Splash Screen and schedules the transition to the game
/// </summary>
function ShowSplashScreen()
{
    Canvas.pushDialog(SplashScreenGui);
   
    $SplashScreenToGameEvent = schedule(3000, 0, TransitionSplashToGame);
}

/// <summary>
/// Starts the Game and Pops the Splash Screen
/// </summary>
function TransitionSplashToGame()
{
    // This is where the game starts. Right now, we are just starting the first level. You will
    // want to expand this to load up a splash screen followed by a main menu depending on the
    // specific needs of your game. Most likely, a menu button will start the actual game, which
    // is where startGame should be called from.
    startGame(expandPath($Game::DefaultScene));

    Canvas.popDialog(SplashScreenGui);
}

/// <summary>
/// Allows the Player to Skip Out Early
/// </summary>
function SplashScreen::onClick(%this)
{
    cancel($SplashScreenToGameEvent);

    TransitionSplashToGame();
}