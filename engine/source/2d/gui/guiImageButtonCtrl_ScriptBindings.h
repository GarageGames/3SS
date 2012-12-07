//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

ConsoleMethod( GuiImageButtonCtrl, setNormalImage, void, 3, 3,  "(imageAssetId) Sets the asset Id the button \"up\" state.\n"
                                                                "@return No return value.")
{
   object->setNormalImage( argv[2] );
}

//-----------------------------------------------------------------------------

ConsoleMethod( GuiImageButtonCtrl, setHoverImage, void, 3, 3,       "(imageAssetId) Sets the asset Id the button \"hover\" state.\n"
                                                                    "@return No return value.")
{
   object->setHoverImage( argv[2]  );
}

//-----------------------------------------------------------------------------

ConsoleMethod( GuiImageButtonCtrl, setDownImage, void, 3, 3,   "(imageAssetId) Sets the asset Id the button \"down\" state.\n"
                                                                    "@return No return value.")
{
   object->setDownImage( argv[2]  );
}

//-----------------------------------------------------------------------------

ConsoleMethod( GuiImageButtonCtrl, setInactiveImage, void, 3, 3,    "(imageAssetId) Sets the asset Id the button \"inactive\" state.\n"
                                                                    "@return No return value.")
{
   object->setInactiveImage( argv[2]  );
}

//-----------------------------------------------------------------------------

ConsoleMethod( GuiImageButtonCtrl, setActive, void, 3, 3,    "(imageAssetId) Sets the asset Id the button \"inactive\" state.\n"
                                                                    "@return No return value.")
{
    bool flag = dAtob( argv[2] );
    object->setActive( flag  );
}
