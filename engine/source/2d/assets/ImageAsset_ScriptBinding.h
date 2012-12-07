//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, setImageFile, void, 3, 3,     "(ImageFile) Sets the image file (bitmap file).\n"
                                                        "@return No return value.")
{
    object->setImageFile( argv[2] );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getImageFile, const char*, 2, 2,  "() Gets the image file.\n"
                                                            "@return Returns the bitmap image file.")
{
    return object->getImageFile();
}

//------------------------------------------------------------------------------

ConsoleMethod(ImageAsset, setFilterMode, void, 3, 3,            "(mode) Sets the filter mode.\n"
                                                                "@return No return value.")
{
    // Fetch Texture Filter Mode.
    const ImageAsset::TextureFilterMode filterMode = getFilterModeEnum( argv[2] );

    // Valid Filter?
    if ( filterMode == ImageAsset::FILTER_INVALID )
    {
        // Warn.
        Con::warnf("ImageAsset::setFilterMode() - Invalid Filter Mode Specified! (%s)", argv[2] );
        // Finish Here.
        return;
    }

    // Set Filter Mode.
    object->setFilterMode( filterMode );
}

//------------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getFilterMode, const char*, 2, 2,   "() Gets the filter mode.\n"
                                                                        "@return The filter mode.")
{
    return getFilterModeDescription( object->getFilterMode() );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, setForce16Bit, void, 3, 3,          "(force16Bit?) Sets whether 16-bit image is forced or not.\n"
                                                                        "@return No return value.")
{
    object->setForce16Bit( dAtob(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getForce16Bit, bool, 2, 2,          "() Gets whether 16-bit image is forced or not.\n"
                                                                        "@return Whether 16-bit image is forced or not.")
{
    return object->getForce16Bit();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, setCellRowOrder, void, 3, 3,        "(rowOrder?) Sets whether CELL row order should be used or not.\n"
                                                                        "@return No return value.")
{
    object->setCellRowOrder( dAtob(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getCellRowOrder, bool, 2, 2,        "() Gets whether CELL row order should be used or not.\n"
                                                                        "@return Whether CELL row order should be used or not.")
{
    return object->getCellRowOrder();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, setCellOffsetX, void, 3, 3,         "(offsetX) Sets the CELL offset X.\n"
                                                                        "@return No return value.")
{
    object->setCellOffsetX( dAtoi(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getCellOffsetX, S32, 2, 2,          "() Gets the CELL offset X.\n"
                                                                        "@return The CELL offset X.")
{
    return object->getCellOffsetX();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, setCellOffsetY, void, 3, 3,         "(offsetY) Sets the CELL offset Y.\n"
                                                                        "@return No return value.")
{
    object->setCellOffsetY( dAtoi(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getCellOffsetY, S32, 2, 2,          "() Gets the CELL offset Y.\n"
                                                                        "@return The CELL offset Y.")
{
    return object->getCellOffsetY();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, setCellStrideX, void, 3, 3,         "(strideX) Sets the CELL stride X.\n"
                                                                        "@return No return value.")
{
    object->setCellStrideX( dAtoi(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getCellStrideX, S32, 2, 2,          "() Gets the CELL stride X.\n"
                                                                        "@return The CELL stride X.")
{
    return object->getCellStrideX();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, setCellStrideY, void, 3, 3,         "(strideY) Sets the CELL stride Y.\n"
                                                                        "@return No return value.")
{
    object->setCellStrideY( dAtoi(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getCellStrideY, S32, 2, 2,          "() Gets the CELL stride Y.\n"
                                                                        "@return The CELL stride Y.")
{
    return object->getCellStrideY();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, setCellCountX, void, 3, 3,         "(countX) Sets the CELL count X.\n"
                                                                        "@return No return value.")
{
    object->setCellCountX( dAtoi(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getCellCountX, S32, 2, 2,          "() Gets the CELL count X.\n"
                                                                        "@return The CELL count X.")
{
    return object->getCellCountX();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, setCellCountY, void, 3, 3,         "(countY) Sets the CELL count Y.\n"
                                                                        "@return No return value.")
{
    object->setCellCountY( dAtoi(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getCellCountY, S32, 2, 2,          "() Gets the CELL count Y.\n"
                                                                        "@return The CELL count Y.")
{
    return object->getCellCountY();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, setCellWidth, void, 3, 3,           "(Width) Sets the CELL width.\n"
                                                                        "@return No return value.")
{
    object->setCellWidth( dAtoi(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getCellWidth, S32, 2, 2,            "() Gets the CELL width.\n"
                                                                        "@return The CELL width.")
{
    return object->getCellWidth();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, setCellHeight, void, 3, 3,          "(Height) Sets the CELL height.\n"
                                                                        "@return No return value.")
{
    object->setCellHeight( dAtoi(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getCellHeight, S32, 2, 2,           "() Gets the CELL height.\n"
                                                                        "@return The CELL height.")
{
    return object->getCellHeight();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getImageWidth, S32, 2, 2,           "() Gets the image width.\n"
                                                                        "@return The image width.")
{
    return object->getImageWidth();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getImageHeight, S32, 2, 2,          "() Gets the image height.\n"
                                                                        "@return The image width.")
{
    return object->getImageHeight();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getImageSize, const char*, 2, 2,    "() Gets the image size.\n"
                                                                        "@return The image size.")
{
    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer(32);

    // Format Buffer.
    dSprintf(pBuffer, 32, "%d %d", object->getImageWidth(), object->getImageHeight());

    // Return Buffer.
    return pBuffer;
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getIsImagePOT, bool, 2, 2,          "() Gets whether the image has a power-of-two dimensions or not.\n"
                                                                        "@return Whether the image has a power-of-two dimensions or not." )
{
    return isPow2( object->getImageWidth() ) && isPow2( object->getImageHeight() );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getFrameCount, S32, 2, 2,           "() Gets the frame count.\n"
                                                                        "@return The frame count.")
{
    return object->getFrameCount(); 
}

//-----------------------------------------------------------------------------

ConsoleMethod(ImageAsset, getFrameSize, const char*, 3, 3,    "(int frame) - Gets the specified frames size.\n"
                                                                        "@param frame The frame number to use.\n"
                                                                        "@return The specified frames size.")
{
    // Fetch Frame.
    const S32 frame = dAtoi(argv[2]);

    // Check Frame.
    if ( frame < 0 || frame >= (S32)object->getFrameCount() )
    {
        // Warn.
        Con::warnf("ImageAsset::getFrameSize() - Invalid Frame; Allowed range is 0 to %d", object->getFrameCount()-1 );
        // Finish Here.
        return NULL;
    }

    // Fetch Selected Frame Pixel Area.
    const ImageAsset::FrameArea::PixelArea& framePixelArea = object->getImageFrameArea( frame ).mPixelArea;

    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer(32);

    // Format Buffer.
    dSprintf(pBuffer, 32, "%d %d", framePixelArea.mPixelWidth, framePixelArea.mPixelHeight );

    // Return Velocity.
    return pBuffer;
}

