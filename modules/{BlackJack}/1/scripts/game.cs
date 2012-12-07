//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$Game::StartingScene = "^BlackjackTemplate/data/levels/startLevel.scene.taml";

function initializeGame()
{
    //  Execute our game-specific scripts here

    // Show a 3-second Splash Screen for the Product, skippable with a click or touch
    ShowSplashScreen();
}

//------------------------------------------------------------------------------
// startGame
// All game logic should be set up here. This will be called by the level
// builder when you select "Run Game" or by the startup process of your game
// to load the first level.
//------------------------------------------------------------------------------
function startGame(%level)
{
    // Initialize PlayerArray values.
    $NumberOfPlayers = 0;
    $CurrentPlayer = 0;

    // Create the player profile
    $DefaultPlayerProfile = PlayerProfileBehavior.createInstance();
    $DefaultPlayerProfile.startingCash = $Save::GeneralSettings::StartingBankCash;
    $DefaultPlayerProfile.init();

    // Push the parent GUI for the game (found in game/gui/mainScreen.gui)
    Canvas.setContent(mainScreenGui);

    // Report the time it took to load the game to this point
    %runTime = getRealTime() - $Debug::loadStartTime;
    echo(" % - Game load time : " @ %runTime @ " ms");

    // Create a new ActionMap for input binding
    new ActionMap(moveMap);

    // Enable DirectInput for Windows development
    $enableDirectInput = true;
    activateDirectInput();

    // Load the level file for this game
    sceneWindow2D.loadLevel($Game::StartingScene);

    // Execute the following code on iOS only
    if($platform $= "iOS")
    {
        // Bind core touch reactions to functions
        // Function names are completely up to you
        moveMap.bind(touchdevice, touchdown, "touchesDown");
        moveMap.bind(touchdevice, touchmove, "touchesMove");
        moveMap.bind(touchdevice, touchup, "touchesUp");

        // Hide the "mouse" cursor on iOS
        hideCursor();

        // Push the moveMap bindings
        moveMap.push();
    }
}

//------------------------------------------------------------------------------
// endGame
// Game cleanup should be done here.
//------------------------------------------------------------------------------
function endGame()
{
    sceneWindow2D.endLevel();
    moveMap.pop();
    moveMap.delete();
}

//------------------------------------------------------------------------------
// oniPhoneResignActive
// Callback sent from the engine when the device has triggered an exit,
// like pressing the home button. This is where you want to execute emergency
// code, like game saving, data storage and general cleanup
//------------------------------------------------------------------------------
function oniPhoneResignActive()
{
    echo("oniPhoneResignActive called...");

    // Save the default player profile
    if (isObject($DefaultPlayerProfile))
    {
        echo("Creating xml file");
        $DefaultPlayerProfile.updateFromPlayer($UserControlledPlayer);
        $DefaultPlayerProfile.createProfileXMLFile();
    }
}

//------------------------------------------------------------------------------
// oniPhoneBecomeActive
// Callback sent from the engine when the device has triggered a resume to app
// This is where you want to execute resume code, like showing a paused screen,
// rescheduling, etc.
//------------------------------------------------------------------------------
function oniPhoneBecomeActive()
{
}

//------------------------------------------------------------------------------
// oniPhoneChangeOrientation
// Callback sent from the engine when the device's orientation has adjusted
//------------------------------------------------------------------------------
function oniPhoneChangeOrientation(%newOrientation)
{
    %new = "Unkown";
    if (%newOrientation == $iOS::constant::OrientationLandscapeLeft)
    {
        %new = "Landscape Left (Home Button on the right)";
    }
    else if (%newOrientation == $iOS::constant::OrientationLandscapeRight)
    {
        %new = "Landscape Right (Home Button on the left)";
    }
    else if (%newOrientation == $iOS::constant::OrientationPortrait)
    {
        %new = "Portrait (Home Button on the bottom)";
    }
    else if (%newOrientation == $iOS::constant::OrientationPortraitUpsideDown)
    {
        %new = "Portrait Upside Down (Home Button on the top)";
    }

    echo("newOrientation: " @ %new);
}

//------------------------------------------------------------------------------
// touchesDown
// Called whenever a finger touches the screen, if moveMap is pushed
//
// %touchIDs - List of unique ID numbers, one per-finger
// %touchesX - List of coordinates in the X axis
// %touchesY - List of coordinates in the Y axis
//------------------------------------------------------------------------------
function touchesDown(%touchIDs, %touchesX, %touchesY)
{

}

//------------------------------------------------------------------------------
// touchesMove
// Called whenever a finger drags across the screen, if moveMap is pushed
//
// %touchIDs - List of unique ID numbers, one per-finger
// %touchesX - List of coordinates in the X axis
// %touchesY - List of coordinates in the Y axis
//------------------------------------------------------------------------------
function touchesMove(%touchIDs, %touchesX, %touchesY)
{

}

//------------------------------------------------------------------------------
// touchesUp
// Called whenever a finger is lifted from the screen, if moveMap is pushed
//
// %touchIDs - List of unique ID numbers, one per-finger
// %touchesX - List of coordinates in the X axis
// %touchesY - List of coordinates in the Y axis
//------------------------------------------------------------------------------
function touchesUp(%touchIDs, %touchesX, %touchesY)
{
   
}
