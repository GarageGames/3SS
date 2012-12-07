//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod(RenderProxy, isStaticMode, bool, 2, 2,    "() - Gets whether the render-proxy is in static or dynamic (animated)mode.\n"
                                                        "@return Returns whether the render-proxy is in static or dynamic (animated)mode.")
{
    return object->isStaticMode();
}

//------------------------------------------------------------------------------

ConsoleMethod(RenderProxy, setImageMap, bool, 3, 4,  "(string imageAssetId, [int frame]) - Sets imageAssetId/Frame.\n"
                                                "@param imageAssetId The imagemap asset Id to display\n"
                                                "@param frame The frame of the imagemap to display\n"
                                                "@return Returns true on success.")
{
    // Fetch image asset Id.
    const char* pImageAssetId = argv[2];

    // Calculate Frame.
    const U32 frame = argc >= 4 ? dAtoi(argv[3]) : 0;

    // Set ImageMap.
    return static_cast<SpriteProxyBase*>(object)->setImage(pImageAssetId, frame );
}   

//------------------------------------------------------------------------------

ConsoleMethod(RenderProxy, getImageMap, const char*, 2, 2,   "() - Gets current imageMap asset Id.\n"
                                                        "@return (string imageAssetId) The imagemap being displayed")
{
    // Are we in static mode?
    if ( !object->isStaticMode() )
    {
        // No, so warn.
        Con::warnf( "RenderProxy::getImageMap() - Method invalid, not in static mode." );
        return StringTable->EmptyString;
    }

    // Get ImageMap.
    return static_cast<SpriteProxyBase*>(object)->getImage();
}   

//------------------------------------------------------------------------------

ConsoleMethod(RenderProxy, setFrame, bool, 3, 3,    "(int frame) - Sets imageMap frame.\n"
                                                    "@param frame The frame to display\n"
                                                    "@return Returns true on success.")
{
    // Are we in static mode?
    if ( !object->isStaticMode() )
    {
        // No, so warn.
        Con::warnf( "RenderProxy::setFrame() - Method invalid, not in static mode." );
        return false;
    }

    // Set ImageMap Frame.
    return static_cast<SpriteProxyBase*>(object)->setImageFrame( dAtoi(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(RenderProxy, getFrame, S32, 2, 2, "() - Gets current imageMap Frame.\n"
                                                "@return (int frame) The frame currently being displayed")
{
    // Are we in static mode?
    if ( !object->isStaticMode() )
    {
        // No, so warn.
        Con::warnf( "RenderProxy::getFrame() - Method invalid, not in static mode." );
        return -1;
    }

    // Get ImageMap Frame.
    return object->getImageFrame();
}

//------------------------------------------------------------------------------

ConsoleMethod(RenderProxy, playAnimation, bool, 3, 4,    "(string animationAssetId, [bool autoRestore]) - Plays an animation.\n"
                                                    "@param animationAssetId The animation asset Id to play\n"
                                                    "@param autoRestore If true, the previous animation will be played when this new animation finishes.\n"
                                                    "@return Returns true on success.")
{    
    // Fetch Auto-Restore Flag.
    const bool autoRestore = (argc >= 4) ? dAtob(argv[3]) : false;

    // Play Animation.
    return static_cast<SpriteProxyBase*>(object)->setAnimation( argv[2], autoRestore );
}   

//-----------------------------------------------------------------------------

ConsoleMethod(RenderProxy, pauseAnimation, void, 3, 3, "(bool enable) - Pause the current animation\n"
                                                             "@param enable If true, pause the animation. If false, continue animating\n")
{
    // Are we in static mode?
    if ( object->isStaticMode() )
    {
        // Yes, so warn.
        Con::warnf( "RenderProxy::pauseAnimation() - Method invalid, not in dynamic (animated) mode." );
        return;
    }

    static_cast<SpriteProxyBase*>(object)->pauseAnimation(dAtob(argv[2]));
}

//-----------------------------------------------------------------------------

ConsoleMethod(RenderProxy, stopAnimation, void, 2, 2,   "() - Stop the current animation\n"
                                                        "@return No return value.")
{
    // Are we in static mode?
    if ( object->isStaticMode() )
    {
        // Yes, so warn.
        Con::warnf( "RenderProxy::stopAnimation() - Method invalid, not in dynamic (animated) mode." );
        return;
    }

    object->getAnimationController()->stopAnimation();
}

//-----------------------------------------------------------------------------

ConsoleMethod(RenderProxy, setAnimationFrame, void, 3, 3, "(int frame) - Sets the current animation frame.\n"
                                                                "@param frame Which frame of the animation to display\n"
                                                                "@return No return value.")
{
    // Are we in static mode?
    if ( object->isStaticMode() )
    {
        // Yes, so warn.
        Con::warnf( "RenderProxy::setAnimationFrame() - Method invalid, not in dynamic (animated) mode." );
        return;
    }

    // Set Animation Frame
    object->getAnimationController()->setAnimationFrame( dAtoi(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(RenderProxy, getAnimationFrame, S32, 2, 2, "() - Gets current animation frame.\n"
                                                               "@return (int frame) The current animation frame")
{
    // Are we in static mode?
    if ( object->isStaticMode() )
    {
        // Yes, so warn.
        Con::warnf( "RenderProxy::getAnimationFrame() - Method invalid, not in dynamic (animated) mode." );
        return -1;
    }

    // Get Animation Frame.
    return object->getAnimationController()->getCurrentFrame();
}

//-----------------------------------------------------------------------------

ConsoleMethod(RenderProxy, getAnimation, const char*, 2, 2,  "() - Gets current animation asset Id.\n"
                                                        "@return (string AnimationAssetId) The current animation asset Id.")
{
    // Are we in static mode?
    if ( object->isStaticMode() )
    {
        // Yes, so warn.
        Con::warnf( "RenderProxy::getAnimation() - Method invalid, not in dynamic (animated) mode." );
        return StringTable->EmptyString;
    }


    // Get Current Animation.
    return object->getAnimationController()->getCurrentAnimation();
}

//-----------------------------------------------------------------------------

ConsoleMethod(RenderProxy, getAnimationTime, F32, 2, 2,  "() - Gets current animation time.\n"
                                                    "@return (float time) The current animation time")
{
    // Are we in static mode?
    if ( object->isStaticMode() )
    {
        // Yes, so warn.
        Con::warnf( "RenderProxy::getAnimationTime() - Method invalid, not in dynamic (animated) mode." );
        return 0.0f;
    }


    // Get Animation Time.
    return object->getAnimationController()->getCurrentTime();
}

//-----------------------------------------------------------------------------

ConsoleMethod(RenderProxy, getIsAnimationFinished, bool, 2, 2,   "() - Checks animation status.\n"
                                                            "@return (bool finished) Whether or not the animation is finished")
{
    // Are we in static mode?
    if ( object->isStaticMode() )
    {
        // Yes, so warn.
        Con::warnf( "RenderProxy::getIsAnimationFinished() - Method invalid, not in dynamic (animated) mode." );
        return true;
    }

    // Return Animation Finished Status.
    return object->getAnimationController()->isAnimationFinished();
}

//-----------------------------------------------------------------------------

ConsoleMethod(RenderProxy, setAnimationTimeScale, void, 3, 3,   "(float timeScale) - Change the rate of animation.\n"
                                                            "@param timeScale Value which will scale the frame animation speed. 1 by default.\n")
{
    // Are we in static mode?
    if ( object->isStaticMode() )
    {
        // Yes, so warn.
        Con::warnf( "RenderProxy::setAnimationTimeScale() - Method invalid, not in dynamic (animated) mode." );
        return;
    }

    object->getAnimationController()->setAnimationTimeScale(dAtof(argv[2]));
}

//-----------------------------------------------------------------------------

ConsoleMethod(RenderProxy, getAnimationTimeScale, F32, 2, 2,     "() - Get the animation time scale for this render-proxy.\n"
                                                            "@return (float) Returns the animation time scale for this render-proxy.\n")
{
    // Are we in static mode?
    if ( object->isStaticMode() )
    {
        // Yes, so warn.
        Con::warnf( "RenderProxy::getSpeedFactor() - Method invalid, not in dynamic (animated) mode." );
        return 1.0f;
    }

    return object->getAnimationController()->getAnimationTimeScale();
}

