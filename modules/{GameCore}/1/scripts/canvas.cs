//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function iOSResolutionFromSetting( %deviceType, %deviceScreenOrientation )
{
    // A helper function to get a string based resolution from the settings given.
    %x = 0;
    %y = 0;

    switch(%deviceType)
    {
        case $iOS::constant::iPhone:
            if(%deviceScreenOrientation == $iOS::constant::Landscape)
            {
                %x =  $iOS::constant::iPhoneWidth;
                %y =  $iOS::constant::iPhoneHeight;
            }
            else
            {
                %x =  $iOS::constant::iPhoneHeight;
                %y =  $iOS::constant::iPhoneWidth;
            }

        case $iOS::constant::iPad:
            if(%deviceScreenOrientation == $iOS::constant::Landscape)
            {
                %x =  $iOS::constant::iPadWidth;
                %y =  $iOS::constant::iPadHeight;
            }
            else
            {
                %x =  $iOS::constant::iPadHeight;
                %y =  $iOS::constant::iPadWidth;
            }
    }
   
    return %x @ " " @ %y;
}

//------------------------------------------------------------------------------
// initializeCanvas
// Constructs and initializes the default canvas window.
//------------------------------------------------------------------------------
$canvasCreated = false;
function initializeCanvas(%windowName)
{
    // Don't duplicate the canvas.
    if($canvasCreated)
    {
        error("Cannot instantiate more than one canvas!");
        return;
    }

    videoSetGammaCorrection($pref::OpenGL::gammaCorrection);

    if (!createCanvas(%windowName))
    {
        error("Canvas creation failed. Shutting down.");
        quit();
    }

    $pref::iOS::ScreenDepth = 32;

    if ($pref::iOS::DeviceType $= "")
        %res = $pref::Video::defaultResolution;
    else
        %res = iOSResolutionFromSetting($pref::iOS::DeviceType, $pref::iOS::ScreenOrientation);

    if ($platform $= "windows" || $platform $= "macos")
        setScreenMode( GetWord( %res , 0 ), GetWord( %res, 1 ), $pref::iOS::ScreenDepth, $pref::Video::fullScreen);
    else
        setScreenMode( GetWord( %res , 0 ), GetWord( %res, 1 ), $pref::iOS::ScreenDepth, false);

    $canvasCreated = true;
}

//------------------------------------------------------------------------------
// resetCanvas
// Forces the canvas to redraw itself.
//------------------------------------------------------------------------------
function resetCanvas()
{
    if (isObject(Canvas))
        Canvas.repaint();
}

new GuiCursor(DefaultCursor)
{
    hotSpot = "4 4";
    renderOffset = "0 0";
    bitmapName = "^{GameCore}/gui/images/defaultCursor";
};