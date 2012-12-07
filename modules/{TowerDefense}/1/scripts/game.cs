//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// Global Game Variables
$Lives = 0;
$WaveTotal = 0;
$CurrentWave = 0;
$Funds = 0;
$Score = 0;
$TotalCreepsInLevel = 0;
$EnemiesGone = 0;
$TouchHandled = false;

function initializeGame()
{
    // Execute our game-specific scripts here

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
    $SelectedLevel = "level.t2d";
    
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

    Canvas.pushDialog(mainMenuGui);

    // Execute the following code on iOS only
    if($platform $= "iOS")
    {
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
    %new = "Unknown";
    
    if(%newOrientation == $iOS::constant::OrientationLandscapeLeft)
        %new = "Landscape Left (Home Button on the right)";
    else if(%newOrientation == $iOS::constant::OrientationLandscapeRight)
        %new = "Landscape Right (Home Button on the left)";
    else if(%newOrientation == $iOS::constant::OrientationPortrait)
        %new = "Portrait (Home Button on the bottom)";
    else if(%newOrientation == $iOS::constant::OrientationPortraitUpsideDown)
        %new = "Portrait Upside Down (Home Button on the top)";
}

function loadPersistentObjects(%newScene)
{
    //---------------------------------------------------------------------------
    // Managed Persistent Objects
    //---------------------------------------------------------------------------
    if (!isObject($persistentObjectSet) || $persistentObjectSet.getCount() < 1)
    {
        %persistFile = expandPath("^TowerDefenseTemplate/managed/persistent.cs");
        
        if (isFile(%persistFile))
            $persistentObjectSet = TamlRead(%persistFile);

        if (isObject($persistentObjectSet) )
        {
            // Now we need to move all the persistent objects into the scene.
            %count = $persistentObjectSet.getCount();
            for (%i = 0; %i < %count; %i++)
            {
                %object = $persistentObjectSet.getObject(%i);
                %sg = %object.getScene();
                if (%sg)
                    %sg.removeFromScene(%object);

                %newScene.addToScene(%object);
                %object.setPosition(%object.getPosition());
            }
        }
    }
}