//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, addSprite, S32, 4, 8,    "( a, b, [c], [d], [e], [f] ) - Adds a sprite at the specified logical position.\n"
                                                        "The created sprite will be automatically selected.\n"
                                                        "@param a Logical position #1.\n"
                                                        "@param b Logical position #2.\n"
                                                        "@param c Logical position #3.\n"
                                                        "@param d Logical position #4.\n"
                                                        "@param e Logical position #5.\n"
                                                        "@param f Logical position #6.\n"
                                                        "@return The batch Id of the added sprite or zero if not successful." )
{
    return object->addSprite( CompositeSprite::LogicalPosition(argc-2, argv+2) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, removeSprite, bool, 2, 2,    "() - Removes the selected sprite.\n"
                                                            "@return Whether the sprite was removed or not." )
{
    return object->removeSprite();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, clearSprites, void, 2, 2,    "() - Removes all sprites.\n"
                                                            "@return No return value." )
{
    return object->clearSprites();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteCount, S32, 2, 2,   "() - Gets a count of sprites in the composite.\n"
                                                            "@return The count of sprites in the composite." )
{
    return object->getSpriteCount();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setBatchIsolated, void, 3, 3,    "(bool batchIsolated) - Sets whether the sprites are rendered, isolated from other renderings as one batch or not.\n"
                                                                "When in batch isolated mode, the sprites can be optionally sorted.\n"
                                                                "@return No return value." )
{
    // Fetch batch isolated.
    const bool batchIsolated = dAtob(argv[2]);

    object->setBatchIsolated( batchIsolated );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getBatchIsolated, bool, 2, 2,    "() - Gets whether the sprites are rendered, isolated from other renderings as one batch or not.\n"
                                                                "@return Whether the sprites are rendered, isolated from other renderings as one batch or not." )
{
    return object->getBatchIsolated();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSortMode, void, 3, 3,     "(renderSortMode) - Sets the render sort mode.\n"
                                                            "The render sort mode is used when isolated batch mode is on.\n"
                                                            "@return No return value." )
{
    // Fetch render sort mode.
    SceneRenderQueue::RenderSort sortMode = SceneRenderQueue::getRenderSortEnum( argv[2] );

    // Sanity!
    if ( sortMode == SceneRenderQueue::RENDER_SORT_INVALID )
    {
        // Warn.
        Con::warnf( "CompositeSprite::setSortMode() - Unknown sort mode of '%s'.", argv[2] );
        return;
    }

    object->setSortMode( sortMode );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSortMode, const char*, 2, 2,  "() - Gets the render sort mode.\n"
                                                                "@return The render sort mode." )
{
    return SceneRenderQueue::getRenderSortDescription( object->getSortMode() );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setDefaultSpriteStride, void, 3, 4,  "(float strideX, [float strideY]]) - Sets the stride which scales the position at which sprites are created.\n"
                                                                    "@param strideX The default stride of the local X axis.\n"
                                                                    "@param strideY The default stride of the local Y axis.\n"
                                                                    "@return No return value.")
{
    Vector2 stride;

    // Fetch element count.
    const U32 elementCount = Utility::mGetStringElementCount(argv[2]);

    // ("strideX strideY")
    if ( (elementCount == 2) && (argc == 3) )
    {
        stride.x = dAtof(Utility::mGetStringElement(argv[2], 0));
        stride.y = dAtof(Utility::mGetStringElement(argv[2], 1));
    }
    // (strideX, [strideY])
    else if (elementCount == 1)
    {
        stride.x = dAtof(argv[2]);

        if (argc > 3)
            stride.y = dAtof(argv[3]);
        else
            stride.y = stride.x;
    }
    // Invalid
    else
    {
        Con::warnf("CompositeSprite::setDefaultSpriteStride() - Invalid number of parameters!");
        return;
    }

    object->setDefaultSpriteStride( stride );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getDefaultSpriteStride, const char*, 2, 2,   "() - Gets the stride which scales the position at which sprites are created.\n"
                                                                            "@return (float strideX/float strideY) The stride which scales the position at which sprites are created.")
{
    return object->getDefaultSpriteStride().scriptThis();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setDefaultSpriteSize, void, 3, 4,    "(float width, [float height]) - Sets the size at which sprites are created.\n"
                                                                    "@param width The default width of sprites.\n"
                                                                    "@param height The default height of sprites\n"
                                                                    "@return No return value.")
{
    Vector2 size;

    // Fetch element count.
    const U32 elementCount = Utility::mGetStringElementCount(argv[2]);

    // ("width height")
    if ( (elementCount == 2) && (argc == 3) )
    {
        size.x = dAtof(Utility::mGetStringElement(argv[2], 0));
        size.y = dAtof(Utility::mGetStringElement(argv[2], 1));
    }
    // (width, [height])
    else if (elementCount == 1)
    {
        size.x = dAtof(argv[2]);

        if (argc > 3)
            size.y = dAtof(argv[3]);
        else
            size.y = size.x;
    }
    // Invalid
    else
    {
        Con::warnf("CompositeSprite::setDefaultSpriteSize() - Invalid number of parameters!");
        return;
    }

    object->setDefaultSpriteSize( size );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getDefaultSpriteSize, const char*, 2, 2, "() - Gets the size at which sprites are created.\n"
                                                                        "@return (float width/float height) The size at which sprites are created.")
{
    return object->getDefaultSpriteSize().scriptThis();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setDefaultSpriteAngle, void, 3, 3,   "(float angle) - Sets the angle at which sprites are created.\n"
                                                                    "@param angle The angle at which sprites are created.\n"
                                                                    "@return No return value.")
{
    // Fetch angle.
    const F32 angle = mDegToRad( dAtof(argv[2]) );

    static_cast<SpriteBatch*>(object)->setDefaultSpriteAngle( angle );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getDefaultSpriteAngle, F32, 3, 3,   "() - Gets the angle at which sprites are created.\n"
                                                                    "@return (float angle) The angle at which sprites are created.")
{
    return mRadToDeg( static_cast<SpriteBatch*>(object)->getDefaultSpriteAngle() );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, selectSprite, bool, 4, 8,    "( a, b, [c], [d], [e], [f] ) - Selects a sprite at the specified logical position.\n"
                                                            "@param a Logical position #1.\n"
                                                            "@param b Logical position #2.\n"
                                                            "@param c Logical position #3.\n"
                                                            "@param d Logical position #4.\n"
                                                            "@param e Logical position #5.\n"
                                                            "@param f Logical position #6.\n"
                                                            "@return Whether the sprite was selected or not." )
{
    return object->selectSprite( CompositeSprite::LogicalPosition(argc-2, argv+2) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, selectSpriteId, bool, 3, 3,  "( int batchId ) - Selects a sprite with the specified batch Id.\n"
                                                            "@param batchId The batch Id of the sprite to select.\n"
                                                            "@return Whether the sprite was selected or not." )
{
    return object->selectSpriteId( dAtoi(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, deselectSprite, void, 2, 2,  "() - Deselects any selected sprite.\n"
                                                            "This is not required but can be used to stop accidental changes to sprites.\n"
                                                            "@return No return value." )
{
    return object->deselectSprite();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, isSpriteSelected, bool, 2, 2,    "() - Checks whether a sprite is selected or not.\n"
                                                                "@return Whether a sprite is selected or not." )
{
    return object->isSpriteSelected();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteImage, void, 3, 4,  "(imageAssetId, [int imageFrame]) - Sets the sprite image and optional frame.\n"
                                                            "@param imageAssetId The image to set the sprite to.\n"
                                                            "@param imageFrame The image frame of the imageAssetId to set the sprite to.\n"
                                                            "@return No return value." )
{
    // Fetch frame.
    const U32 frame = argc >=4 ? dAtoi(argv[3]) : 0;

    object->setSpriteImage( argv[2], frame );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteImage, const char*, 2, 2,   "() - Gets the sprite image.\n"
                                                                    "@return The sprite image." )
{
    return object->getSpriteImage();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteImageFrame, void, 3, 3,  "(int imageFrame) - Sets the sprite image frame.\n"
                                                            "@param imageFrame The image frame to set the sprite to.\n"
                                                            "@return No return value." )
{
    // Fetch frame.
    const U32 frame = dAtoi(argv[2]);

    object->setSpriteImageFrame( frame );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteImageFrame, S32, 2, 2,  "() - Gets the sprite image frame.\n"
                                                                "@return The sprite image frame." )
{
    return object->getSpriteImageFrame();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteAnimation, void, 3, 4,  "(animationAssetId, [bool autoRestore]) - Sets the sprite animation.\n"
                                                                "@param imageAssetId The animation to set the sprite to.\n"
                                                                "@param autoRestore Whether to restore any previously playing animation or not.\n"
                                                                "@return No return value." )
{
    // Fetch Auto-Restore Flag.
    const bool autoRestore = (argc >= 4) ? dAtob(argv[3]) : false;

    object->setSpriteAnimation( argv[2], autoRestore );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteAnimation, const char*, 2, 2,   "() - Gets the sprite animation.\n"
                                                                        "@return The sprite animation." )
{
    return object->getSpriteAnimation();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, clearSpriteAsset, void, 2, 2,    "() - Clears any image or animation asset from the sprite.\n"
                                                                "@return No return value." )
{
    return object->clearSpriteAsset();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteVisible, void, 3, 3,    "(bool visible) - Sets whether the sprite is visible or not.\n"
                                                                "@param visible Whether the sprite is visible or not.\n"
                                                                "@return No return value." )
{
    // Fetch visible.
    const bool visible = dAtob(argv[2]);

    object->setSpriteVisible( visible );

}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteVisible, bool, 2, 2,    "() - Gets whether the sprite is visible or not.\n"
                                                                "@return Whether the sprite is visible or not." )
{
    return object->getSpriteVisible();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteLocalPosition, void, 3, 4,  "(float localX, float localY) - Sets the sprites local position.\n"
                                                                    "@param localX The local position X.\n"
                                                                    "@param localY The local position Y.\n"
                                                                    "@return No return value." )
{
    Vector2 localPosition;

    // Fetch element count.
    const U32 elementCount = Utility::mGetStringElementCount(argv[2]);

    // ("x y")
    if ( (elementCount == 2) && (argc == 3) )
    {
        localPosition.x = dAtof(Utility::mGetStringElement(argv[2], 0));
        localPosition.y = dAtof(Utility::mGetStringElement(argv[2], 1));
    }
    // (x, y)
    else if ( elementCount == 1 && (argc > 3) )
    {
        localPosition.x = dAtof(argv[2]);
        localPosition.y = dAtof(argv[3]);
    }
    // Invalid
    else
    {
        Con::warnf("CompositeSprite::setSpriteLocalPosition() - Invalid number of parameters!");
        return;
    }

    object->setSpriteLocalPosition( localPosition );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteLocalPosition, const char*, 2, 2,   "() - Gets the sprite local position.\n"
                                                                            "@return The sprite local position." )
{
    return object->getSpriteLocalPosition().scriptThis();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteAngle, void, 3, 3,  "(float localAngle) - Sets the sprites local angle.\n"
                                                            "@param localAngle The sprite local angle.\n"
                                                            "@return No return value." )
{
    // Fetch angle.
    const F32 angle = mDegToRad( dAtof(argv[2]) );

    object->setSpriteAngle( angle );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteAngle, F32, 2, 2,   "() - Gets the sprite local angle.\n"
                                                            "@return The sprite local angle." )
{
    return mRadToDeg( object->getSpriteAngle() );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteDepth, void, 3, 3,  "(float depth) - Sets the sprites depth.\n"
                                                            "@param depth The sprite depth.\n"
                                                            "@return No return value." )
{
    // Fetch depth.
    const F32 depth = dAtof(argv[2]);

    object->setSpriteDepth( depth );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteDepth, F32, 2, 2,   "() - Gets the sprite depth.\n"
                                                            "@return The sprite depth." )
{
    return object->getSpriteDepth();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteSize, void, 3, 4,   "(float width, [float height]) - Sets the sprite size.\n"
                                                            "@param width The sprite width.\n"
                                                            "@param height The sprite height\n"
                                                            "@return No return value.")
{
    Vector2 size;

    // Fetch element count.
    const U32 elementCount = Utility::mGetStringElementCount(argv[2]);

    // ("width height")
    if ( (elementCount == 2) && (argc == 3) )
    {
        size.x = dAtof(Utility::mGetStringElement(argv[2], 0));
        size.y = dAtof(Utility::mGetStringElement(argv[2], 1));
    }
    // (width, [height])
    else if (elementCount == 1)
    {
        size.x = dAtof(argv[2]);

        if (argc > 3)
            size.y = dAtof(argv[3]);
        else
            size.y = size.x;
    }
    // Invalid
    else
    {
        Con::warnf("CompositeSprite::setSpriteSize() - Invalid number of parameters!");
        return;
    }

    object->setSpriteSize( size );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteSize, const char*, 2, 2,    "() - Gets the sprite size.\n"
                                                                    "@return (float width/float height) The sprite size.")
{
    return object->getSpriteSize().scriptThis();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteFlipX, void, 3, 3,  "(bool flipX) - Sets whether the sprite is flipped along its local X axis or not.\n"
                                                            "@param flipX Whether the sprite is flipped along its local X axis or not.\n"
                                                            "@return No return value." )
{
    // Fetch flipX.
    const bool flipX = dAtob(argv[2]);

    object->setSpriteFlipX( flipX );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteFlipX, bool, 2, 2,   "() - Gets whether the sprite is flipped along its local X axis or not.\n"
                                                            "@return Whether the sprite is flipped along its local X axis or not." )
{
    return object->getSpriteFlipX();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteFlipY, void, 3, 3,  "(bool flipY) - Sets whether the sprite is flipped along its local Y axis or not.\n"
                                                            "@param flipY Whether the sprite is flipped along its local Y axis or not.\n"
                                                            "@return No return value." )
{
    const bool flipY = dAtob(argv[2]);

    object->setSpriteFlipY( flipY );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteFlipY, bool, 2, 2,   "() - Gets whether the sprite is flipped along its local Y axis or not.\n"
                                                                "@return Whether the sprite is flipped along its local Y axis or not." )
{
    return object->getSpriteFlipY();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteSortPoint, void, 3, 4,  "(float localX, float localY) - Sets the sprites local position.\n"
                                                                "@param localX The local position X.\n"
                                                                "@param localY The local position Y.\n"
                                                                "@return No return value." )
{
    Vector2 sortPoint;

    // Fetch element count.
    const U32 elementCount = Utility::mGetStringElementCount(argv[2]);

    // ("x y")
    if ( (elementCount == 2) && (argc == 3) )
    {
        sortPoint.x = dAtof(Utility::mGetStringElement(argv[2], 0));
        sortPoint.y = dAtof(Utility::mGetStringElement(argv[2], 1));
    }
    // (x, y)
    else if ( elementCount == 1 && (argc > 3) )
    {
        sortPoint.x = dAtof(argv[2]);
        sortPoint.y = dAtof(argv[3]);
    }
    // Invalid
    else
    {
        Con::warnf("CompositeSprite::setSpriteSortPoint() - Invalid number of parameters!");
        return;
    }

    object->setSpriteSortPoint( sortPoint );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteSortPoint, const char*, 2, 2,   "() - Gets the sprite local position.\n"
                                                                        "@return The sprite local position." )
{
    return object->getSpriteSortPoint().scriptThis();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteBlendMode, void, 3, 3,   "(bool blendMode) - Sets whether sprite blending is on or not.\n"
                                                                "@blendMode Whether sprite blending is on or not.\n"
                                                                "@return No return Value.")
{
    // Fetch blend mode.
    const bool blendMode = dAtob(argv[2]);

    object->setSpriteBlendMode( blendMode );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteBlendMode, bool, 2, 2,  "() - Gets whether sprite blending is on or not.\n"
                                                                "@return (bool blendMode) Whether sprite blending is on or not.")
{
   return object->getSpriteBlendMode();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteSrcBlendFactor, void, 3, 3, "(srcBlend) - Sets the sprite source blend factor.\n"
                                                                    "@param srcBlend The sprite source blend factor.\n"
                                                                    "@return No return Value.")
{
    // Fetch source blend factor.
    GLenum blendFactor = getSrcBlendFactorEnum(argv[2]);

    object->setSpriteSrcBlendFactor( blendFactor );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteSrcBlendFactor, const char*, 2, 2,  "() - Gets the sprite source blend factor.\n"
                                                                            "@return (srcBlend) The sprite source blend factor.")
{
   return getSrcBlendFactorDescription( object->getSpriteSrcBlendFactor() );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteDstBlendFactor, void, 3, 3, "(dstBlend) - Sets the sprite destination blend factor.\n"
                                                                    "@param dstBlend The sprite destination blend factor.\n"
                                                                    "@return No return Value.")
{
    // Fetch destination blend factor.
    GLenum blendFactor = getDstBlendFactorEnum(argv[2]);

    object->setSpriteDstBlendFactor( blendFactor );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteDstBlendFactor, const char*, 2, 2,  "() - Gets the sprite destination blend factor.\n"
                                                                            "@return (dstBlend) The sprite destination blend factor.")
{
   return getDstBlendFactorDescription( object->getSpriteDstBlendFactor() );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteBlendColor, void, 3, 6, "(float red, float green, float blue, [float alpha = 1.0]) - Sets the sprite blend color."
                                                                "@param red The red value.\n"
                                                                "@param green The green value.\n"
                                                                "@param blue The blue value.\n"
                                                                "@param alpha The alpha value.\n"
                                                                "@return No return Value.")
{
    // The colors.
    F32 red;
    F32 green;
    F32 blue;
    F32 alpha = 1.0f;

    // Grab the element count.
    const U32 elementCount = Utility::mGetStringElementCount(argv[2]);

    // Space separated.
    if (argc < 4)
    {
        // ("R G B [A]")
        if ((elementCount == 3) || (elementCount == 4))
        {
            // Extract the color.
            red   = dAtof(Utility::mGetStringElement(argv[2], 0));
            green = dAtof(Utility::mGetStringElement(argv[2], 1));
            blue  = dAtof(Utility::mGetStringElement(argv[2], 2));

            // Grab the alpha if it's there.
            if (elementCount > 3)
            alpha = dAtof(Utility::mGetStringElement(argv[2], 3));
        }

        // Invalid.
        else
        {
            Con::warnf("CompositeSprite::setSpriteBlendColor() - Invalid Number of parameters!");
            return;
        }
    }

    // (R, G, B)
    else if (argc >= 5)
    {
        red   = dAtof(argv[2]);
        green = dAtof(argv[3]);
        blue  = dAtof(argv[4]);

        // Grab the alpha if it's there.
        if (argc > 5)
            alpha = dAtof(argv[5]);
    }

    // Invalid.
    else
    {
        Con::warnf("CompositeSprite::setSpriteBlendColor() - Invalid Number of parameters!");
        return;
    }

    // Set blend color.
    object->setSpriteBlendColor(ColorF(red, green, blue, alpha));
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteBlendColor, const char*, 2, 2,  "Gets the sprite blend color\n"
                                                                        "@return (float red / float green / float blue / float alpha) The sprite blend color.")
{
    // Get Blend Colour.
    ColorF blendColor = object->getSpriteBlendColor();

    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer(64);

    // Format Buffer.
    dSprintf(pBuffer, 64, "%g %g %g %g", blendColor.red, blendColor.green, blendColor.blue, blendColor.alpha );

    // Return buffer.
    return pBuffer;
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteBlendAlpha, void, 3, 3, "(float alpha) - Sets the sprite color alpha (transparency).\n"
                                                                "The alpha value specifies directly the transparency of the image. A value of 1.0 will not affect the object and a value of 0.0 will make the object completely transparent.\n"
                                                                "@param alpha The alpha value.\n"
                                                                "@return No return Value.")
{
    object->setSpriteBlendAlpha( dAtof(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteBlendAlpha, F32, 2, 2,  "() - Gets the sprite color alpha (transparency).\n"
                                                                "@return (float alpha) The alpha value, a range from 0.0 to 1.0.  Less than zero if alpha testing is disabled.")
{
    return object->getSpriteBlendAlpha();
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, setSpriteAlphaTest, void, 3, 3,  "(float alpha) - Set the sprite alpha test.\n"
                                                                "@param value Numeric value of 0.0 to 1.0 to turn on alpha testing. Less than zero to disable alpha testing.")
{
    object->setSpriteAlphaTest(dAtof(argv[2]));
}

//-----------------------------------------------------------------------------

ConsoleMethod(CompositeSprite, getSpriteAlphaTest, F32, 2, 2,   "() - Gets the sprite alpha test.\n"
                                                                "@return (S32) A value of 0 to 255 if alpha testing is enabled. <0 represents disabled alpha testing.")
{
    return object->getSpriteAlphaTest();
}
