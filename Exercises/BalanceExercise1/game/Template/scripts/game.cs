//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function initializeGame()
{
   // Execute our game-specific scripts here
   exec("./game/scheduleManager.cs");
   exec("./game/input.cs");
   exec("./game/inputTracker.cs");
   exec("./game/worldLimits.cs");
   
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
    // Push the parent GUI for the game (found in game/gui/mainScreen.gui)
    Canvas.setContent(mainScreenGui);
    Canvas.pushDialog(mainMenuGui);

    // Report the time it took to load the game to this point
    %runTime = getRealTime() - $Debug::loadStartTime;
    echo(" % - Game load time : " @ %runTime @ " ms");

    initializeInputSystem();
    
    // Execute the following code on iOS only
    if($platform $= "iphone" || $platform $= "ipad" || $platform $= "iphone4")
    {
        // Hide the "mouse" cursor on iOS
        hideCursor();
    }

    GameEventManager.postEvent("_StartGame", "");
}

//------------------------------------------------------------------------------
// endGame
// Game cleanup should be done here.
//------------------------------------------------------------------------------
function endGame()
{
   sceneWindow2D.endLevel();
   cleanInputSystem();
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
	%new = "Unkown";
	if(%newOrientation == $iOS::constant::OrientationLandscapeLeft)
	{
		%new = "Landscape Left (Home Button on the right)";
	}
	else if(%newOrientation == $iOS::constant::OrientationLandscapeRight)
	{
		%new = "Landscape Right (Home Button on the left)";
	}
	else if(%newOrientation == $iOS::constant::OrientationPortrait)
	{
		%new = "Portrait (Home Button on the bottom)";
	}
	else if(%newOrientation == $iOS::constant::OrientationPortraitUpsideDown)
	{
		%new = "Portrait Upside Down (Home Button on the top)";
	}
		
	echo("newOrientation: " @ %new);
}

function Game::onStartGame(%this, %messageData)
{
    
}

function Game::onLevelLoaded(%this, %scene)
{
    if (%scene.getName() $= "MainScene")
    {
        checkTutorials();
    }
    
    %levelSize = sceneWindow2D.getScene().levelSize;
    %halfWidth = getWord(%levelSize, 0) / 2;
    %halfHeight = getWord(%levelSize, 1) / 2;
    
    sceneWindow2D.setViewLimitOn(%halfWidth * -1, %halfHeight * -1, %halfWidth, %halfHeight);
    gameActionMap.push();
    schedule(2000, 0, "panCameraToStart");
}

// -----------------------------------------------------------------------------

/// <summary>
/// This function checks for new tutorials with the current level.  If tutorials are
/// available for the current level and they have not been seen before the helpGui
/// will be pushed and it will handle displaying all of the available tutorials before
/// allowing the player to begin the level.
/// </summary>
function checkTutorials()
{
    if (MainScene.Tutorial[0] !$= "")
    {
        %tutorial = MainScene.Tutorial[0];
        if (!%tutorial.TutorialRead)
            Canvas.pushDialog(helpGui);
    }
}

function loadPersistentObjects(%newScene)
{
    //---------------------------------------------------------------------------
    // Managed Persistent Objects
    //---------------------------------------------------------------------------
    if (!isObject($persistentObjectSet) || $persistentObjectSet.getCount() < 1)
    {
        %persistFile = expandPath("^PhysicsLauncherTemplate/managed/persistent.taml");
        
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