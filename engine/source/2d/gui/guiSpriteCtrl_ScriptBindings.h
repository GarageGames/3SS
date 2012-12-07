//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

ConsoleMethod( GuiSpriteCtrl, isStaticMode, bool, 2, 2, "() - Gets whether the control is in static or dynamic (animated)mode.\n"
                                                        "@return Returns whether the control is in static or dynamic (animated)mode.")
{
    return object->isStaticMode();
}

//-----------------------------------------------------------------------------

ConsoleMethod( GuiSpriteCtrl, setImage, void, 3, 3, "(imageAssetId) Sets the image-map asset Id to use as the image.\n"
                                                    "@param imageAssetId The image-map asset Id to use as the image.\n"
                                                    "@return No return value.")
{
   object->setImage( argv[2] );
}

//------------------------------------------------------------------------------

ConsoleMethod( GuiSpriteCtrl, getImage, const char*, 2, 2,  "() - Gets current imageMap asset Id.\n"
                                                            "@return (string imageAssetId) The imagemap being displayed.")
{
    // Are we in static mode?
    if ( !object->isStaticMode() )
    {
        // No, so warn.
        Con::warnf( "GuiSpriteCtrl::getImage() - Method invalid, not in static mode." );
        return StringTable->EmptyString;
    }

    // Get image.
    return object->getImage();
}

//-----------------------------------------------------------------------------

ConsoleMethod( GuiSpriteCtrl, setImageFrame, void, 3, 3,    "(int imageFrame) Sets the image-map frame to use as the image.\n"
                                                            "@param imageFrame The image-map frame to use as the image.\n"
                                                            "@return No return value.")
{
   object->setImageFrame( dAtoi(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod( GuiSpriteCtrl, getImageFrame, S32, 2, 2, "() - Gets current imageMap Frame.\n"
                                                        "@return (int frame) The frame currently being displayed.")
{
    // Are we in static mode?
    if ( !object->isStaticMode() )
    {
        // No, so warn.
        Con::warnf( "GuiSpriteCtrl::getFrame() - Method invalid, not in static mode." );
        return -1;
    }

    // Get image frame.
    return object->getImageFrame();
}

//------------------------------------------------------------------------------

ConsoleMethod( GuiSpriteCtrl, setAnimation, void, 3, 3, "(string animationAssetId) - Sets the animation asset Id to display.\n"
                                                        "@param animationAssetId The animation asset Id to play\n"
                                                        "@return No return value.")
{    
    // Set animation.
    object->setAnimation( argv[2] );
}

//------------------------------------------------------------------------------

ConsoleMethod( GuiSpriteCtrl, getAnimation, const char*, 2, 2,  "() - Gets the current animation asset Id.\n"
                                                                "@return (string ianimationAssetId) The animation being displayed.")
{
    // Are we in static mode?
    if ( object->isStaticMode() )
    {
        // Yes, so warn.
        Con::warnf( "GuiSpriteCtrl::getAnimation() - Method invalid, in static mode." );
        return StringTable->EmptyString;
    }

    // Get animation.
    return object->getAnimation();
}

//------------------------------------------------------------------------------

ConsoleMethod( GuiSpriteCtrl, play, void, 2, 2,  "() - plays the control's current animation.  Resumes playback if paused.\n")
{
    // Are we in static mode?
    if ( object->isStaticMode() )
    {
        // Yes, so warn.
        Con::warnf( "GuiSpriteCtrl::play() - Method invalid, in static mode." );
        return;
    }

    // play animation.
    object->play();
}

//------------------------------------------------------------------------------

ConsoleMethod( GuiSpriteCtrl, pause, void, 3, 3,  "(bool flag) - pauses/unpauses the control's current animation.\n"
                                                    "@param flag true to pause, false to resume.")
{
    // Are we in static mode?
    if ( object->isStaticMode() )
    {
        // Yes, so warn.
        Con::warnf( "GuiSpriteCtrl::pause() - Method invalid, in static mode." );
        return;
    }

    // pause animation.
    object->pause( dAtob(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod( GuiSpriteCtrl, stop, void, 2, 2,  "() - stops the control's current animation.\n")
{
    // Are we in static mode?
    if ( object->isStaticMode() )
    {
        // Yes, so warn.
        Con::warnf( "GuiSpriteCtrl::stop() - Method invalid, in static mode." );
        return;
    }

    // stop animation.
    object->stop();
}
