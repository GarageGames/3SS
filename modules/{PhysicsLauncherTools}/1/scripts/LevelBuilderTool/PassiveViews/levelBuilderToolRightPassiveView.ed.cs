//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------



//--------------------------------------------------------------------------------------------------------------------------------
// Level Builder Tool Right Passive View
//--------------------------------------------------------------------------------------------------------------------------------

/// <summary>
/// Posts Event on Level Builder Tool Right Passive View Wake
/// </summary>
function LevelBuilderToolRightPassiveView::onWake(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::Wake");
}

/// <summary>
/// Posts Event on Level Builder Tool Right Passive View Sleep 
/// </summary>
function LevelBuilderToolRightPassiveView::onSleep(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::Sleep");
}


//------------------------------------------------------------------------------------------------
// Scene Tab Book
//------------------------------------------------------------------------------------------------

/// <summary>
/// Enables/Disables the Scene Tab Book
/// </summary>
/// <param="enable">True if the Scene Tab Book should be Enabled</param>
function LevelBuilderToolRightPassiveView::enableSceneTabBook(%this, %enable)
{
    Lbtrpv_Vb_SceneTabBook.callOnChildren(setActive, %enable);
}

/// <summary>
/// Selects the given Tab in the Scene Tab Book
/// </summary>
/// <param="tab">The Tab to Select</param>
function LevelBuilderToolRightPassiveView::selectSceneTab(%this, %tab)
{
    switch$ (%tab)
    {
        case "Objects":
            Lbtrpv_Vb_SceneTabBook.selectPage(0);
        
        case "Gravity":
            Lbtrpv_Vb_SceneTabBook.selectPage(1);
        
        case "Backgrounds":
            Lbtrpv_Vb_SceneTabBook.selectPage(2);
        
        default:
    }
}


//----------------------------------------------------------------
// Objects Tab Page
//----------------------------------------------------------------

//------------------------------------------------
// Tools
//------------------------------------------------

//--------------------------------
// Selection Tool
//--------------------------------

/// <summary>
/// Sets the Selection Tool as Active/Inactive
/// </summary>
/// <param="active">True if the Selection Tool should be Active</param>
function LevelBuilderToolRightPassiveView::setSelectionToolActive(%this, %active)
{
    Lbtrpv_Vb_Stb_Otp_SelectionToolButton.setActive(%active); 
}

/// <summary>
/// Selects the Selection Tool
/// </summary>
function LevelBuilderToolRightPassiveView::selectSelectionTool(%this)
{
    Lbtrpv_Vb_Stb_Otp_SelectionToolButton.setStateOn(true);
}

/// <summary>
/// Posts Event on Selection Tool Selected
/// </summary>
function Lbtrpv_Vb_Stb_Otp_SelectionToolButton::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::SelectionToolSelected");
}

//--------------------------------
// Rotate
//--------------------------------

/// <summary>
/// Sets Rotate as Active/Inactive
/// </summary>
/// <param="active">True if Rotate should be Active</param>
function LevelBuilderToolRightPassiveView::setRotateActive(%this, %active)
{
    Lbtrpv_Vb_Stb_Otp_RotateButton.setActive(%active); 
}

/// <summary>
/// Posts Event on Rotate Selected
/// </summary>
function Lbtrpv_Vb_Stb_Otp_RotateButton::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::RotateSelected");
}

//--------------------------------
// Duplicate
//--------------------------------

/// <summary>
/// Sets Duplicate as Active/Inactive
/// </summary>
/// <param="active">True if Duplicate should be Active</param>
function LevelBuilderToolRightPassiveView::setDuplicateActive(%this, %active)
{
    Lbtrpv_Vb_Stb_Otp_DuplicateButton.setActive(%active);
}

/// <summary>
/// Posts Event on Duplicate Selected
/// </summary>
function Lbtrpv_Vb_Stb_Otp_DuplicateButton::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::DuplicateSelected");
}

//--------------------------------
// Bring to Front
//--------------------------------

/// <summary>
/// Sets Bring to Front as Active/Inactive
/// </summary>
/// <param="active">True if Brint to Front should be Active</param>
function LevelBuilderToolRightPassiveView::setBringToFrontActive(%this, %active)
{
    Lbtrpv_Vb_Stb_Otp_BringToFrontButton.setActive(%active);
}

/// <summary>
/// Posts Event on Bring to Front Selected
/// </summary>
function Lbtrpv_Vb_Stb_Otp_BringToFrontButton::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::BringToFrontSelected");
}

//--------------------------------
// Send to Back
//--------------------------------

/// <summary>
/// Sets Send to Back as Active/Inactive
/// </summary>
/// <param="active">True if Send to Back should be Active</param>
function LevelBuilderToolRightPassiveView::setSendToBackActive(%this, %active)
{
    Lbtrpv_Vb_Stb_Otp_SendToBackButton.setActive(%active);
}

/// <summary>
/// Posts Event on Send to Back Selected
/// </summary>
function Lbtrpv_Vb_Stb_Otp_SendToBackButton::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::SendToBackSelected");
}

//--------------------------------
// Delete
//--------------------------------

/// <summary>
/// Sets Delete as Active/Inactive
/// </summary>
/// <param="active">True if Delete should be Active</param>
function LevelBuilderToolRightPassiveView::setDeleteActive(%this, %active)
{
    Lbtrpv_Vb_Stb_Otp_DeleteButton.setActive(%active); 
}

/// <summary>
/// Posts Event on Delete Selected
/// </summary>
function Lbtrpv_Vb_Stb_Otp_DeleteButton::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::DeleteSelected");
}


//------------------------------------------------
// Selected Object Details
//------------------------------------------------

/// <summary>
/// Sets the Display of the Name of the Selected Object
/// </summary>
/// <param="name">The Name to Display</param>
function LevelBuilderToolRightPassiveView::setSelectedObjectNameDisplay(%this, %name)
{
    Lbtrpv_Vb_Stb_Otp_Odc_NameDisplay.setText(%name);   
}

//--------------------------------
// Selected Object Preview
//--------------------------------

/// <summary>
/// Sets Up the Selected Object Preview
/// </summary>
/// <param="object">The Selected Object to Preview</param>
function LevelBuilderToolRightPassiveView::setupSelectedObjectPreview(%this, %object)
{
    Lbtrpv_Vb_Stb_Otp_Odc_ObjectPreview.setup(%object);
}

/// <summary>
/// Clears the Selected Object Preview
/// </summary>
function LevelBuilderToolRightPassiveView::clearSelectedObjectPreview(%this)
{
    Lbtrpv_Vb_Stb_Otp_Odc_ObjectPreview.setSceneObject("");
}

//--------------------------------
// Launcher Background Preview
//--------------------------------

/// <summary>
/// Sets Up the Launcher Background Preview (used when the Launcher is the Selected Object)
/// </summary>
/// <param="object">The Launcher Background to Preview</param>
function LevelBuilderToolRightPassiveView::setupLauncherBackgroundPreview(%this, %background)
{
    Lbtrpv_Vb_Stb_Otp_Odc_LauncherBackgroundPreview.setup(%background);
}

/// <summary>
/// Clears the Launcher Background Preview
/// </summary>
function LevelBuilderToolRightPassiveView::clearLauncherBackgroundPreview(%this)
{
    Lbtrpv_Vb_Stb_Otp_Odc_LauncherBackgroundPreview.setSceneObject("");
}

//--------------------------------
// Selected Object Stats
//--------------------------------

/// <summary>
/// Sets whether the Selected Object Stats are Visible/Invisible
/// </summary>
/// <param="visible">True if the Selected Object Stats should be Visible</param>
function LevelBuilderToolRightPassiveView::setSelectedObjectStatsVisible(%this, %visible)
{
    Lbtrpv_Vb_Stb_Otp_Odc_ObjectStatsContainer.setVisible(%visible);
}

/// <summary>
/// Sets the Display of the Mass of the Selected Object
/// </summary>
/// <param="mass">The Mass of the Selected Object</param>
function LevelBuilderToolRightPassiveView::setSelectedObjectMassDisplay(%this, %mass)
{
    Lbtrpv_Vb_Stb_Otp_Odc_Osc_MassDisplay.setText(%mass);
}

/// <summary>
/// Sets the Display of the Friction of the Selected Object
/// </summary>
/// <param="friction">The Friction of the Selected Object</param>
function LevelBuilderToolRightPassiveView::setSelectedObjectFrictionDisplay(%this, %friction)
{
    Lbtrpv_Vb_Stb_Otp_Odc_Osc_FrictionDisplay.setText(%friction);
}

/// <summary>
/// Sets the Display of the Bounce of the Selected Object
/// </summary>
/// <param="bounce">The Bounce of the Selected Object</param>
function LevelBuilderToolRightPassiveView::setSelectedObjectBounceDisplay(%this, %bounce)
{
    Lbtrpv_Vb_Stb_Otp_Odc_Osc_BounceDisplay.setText(%bounce);
}

/// <summary>
/// Sets the Display of the Hit Points of the Selected Object
/// </summary>
/// <param="hitPoints">The Hit Points of the Selected Object</param>
function LevelBuilderToolRightPassiveView::setSelectedObjectHitPointsDisplay(%this, %hitPoints)
{
    Lbtrpv_Vb_Stb_Otp_Odc_Osc_HitPointsDisplay.setText(%hitPoints);
}

//--------------------------------
// Selected Object Edit
//--------------------------------

/// <summary>
/// Sets the Text on the Selected Object Edit control
/// </summary>
/// <param="text">The Text for the Edit control</param>
function LevelBuilderToolRightPassiveView::setSelectedObjectEditText(%this, %text)
{
    Lbtrpv_Vb_Stb_Otp_EditObjectButton.setText(%text);
}

/// <summary>
/// Sets the Selected Object Edit control as Active/Inactive
/// </summary>
/// <param="active">True if the control should be Active</param>
function LevelBuilderToolRightPassiveView::setSelectedObjectEditActive(%this, %active)
{
    Lbtrpv_Vb_Stb_Otp_EditObjectButton.setActive(%active);
}

/// <summary>
/// Handles a Click event on the Selected Object Edit control
/// </summary>
function Lbtrpv_Vb_Stb_Otp_EditObjectButton::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::SelectedObjectEditSelected");
}


//------------------------------------------------
// Object List
//------------------------------------------------

/// <summary>
/// Adds the given Object to the List
/// </summary>
/// <param="object">The Object to Add</param>
function LevelBuilderToolRightPassiveView::addToObjectList(%this, %object)
{
    Lbtrpv_Vb_Stb_Otp_Os_ObjectsArray.addGuiControl(%object);
}

/// <summary>
/// Refreshes the Scroll of the Object list
/// </summary>
function LevelBuilderToolRightPassiveView::refreshObjectScroll(%this)
{
    %objectsScrollerExtent = Lbtrpv_Vb_Stb_Otp_ObjectsScroller.getExtent();
    Lbtrpv_Vb_Stb_Otp_ObjectsScroller.setExtent(getWord(%objectsScrollerExtent, 0), getWord(%objectsScrollerExtent, 1));
}

/// <summary>
/// Clears the Object List
/// </summary>
function LevelBuilderToolRightPassiveView::clearObjectList(%this)
{
    Lbtrpv_Vb_Stb_Otp_Os_ObjectsArray.deleteContents();
    Lbtrpv_Vb_Stb_Otp_Os_ObjectsArray.clear();
}


//----------------------------------------------------------------
// Gravity Tab Page
//----------------------------------------------------------------

//------------------------------------------------
// Gravity Strength
//------------------------------------------------

/// <summary>
/// Gets whether the Gravity Strength List is Active
/// </summary>
function LevelBuilderToolRightPassiveView::getGravityStrengthListActive(%this)
{
    return Lbtrpv_Vb_Stb_Gtp_StrengthDropdown.isActive();
}

/// <summary>
/// Sets whether the Gravity Strength List is Active
/// </summary>
function LevelBuilderToolRightPassiveView::setGravityStrengthListActive(%this, %active)
{
    Lbtrpv_Vb_Stb_Gtp_StrengthDropdown.setActive(%active);
}

/// <summary>
/// Adds the given Strength to the Gravity Strength List
/// </summary>
/// <param="strength">The Strength to Add</param>
function LevelBuilderToolRightPassiveView::addToGravityStrengthList(%this, %strength)
{
    Lbtrpv_Vb_Stb_Gtp_StrengthDropdown.add(%strength, Lbtrpv_Vb_Stb_Gtp_StrengthDropdown.size());
}

/// <summary>
/// Gets the Selected Gravity Strength
/// </summary>
function LevelBuilderToolRightPassiveView::getSelectedGravityStrength(%this)
{
    return Lbtrpv_Vb_Stb_Gtp_StrengthDropdown.getText();
}

/// <summary>
/// Selects the given Gravity Strength
/// </summary>
/// <param="strength">The desired Strength</param>
/// <param="callback">True if the Callback should be triggered</param>
function LevelBuilderToolRightPassiveView::selectGravityStrength(%this, %strength, %callback)
{
    Lbtrpv_Vb_Stb_Gtp_StrengthDropdown.setSelected(Lbtrpv_Vb_Stb_Gtp_StrengthDropdown.findText(%strength), %callback);
}

/// <summary>
/// Handles a Selection in the Gravity Strength List
/// </summary>
function Lbtrpv_Vb_Stb_Gtp_StrengthDropdown::onSelect(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::GravityStrengthSelect");  
}


//------------------------------------------------
// Gravity Direction
//------------------------------------------------

//--------------------------------
// Gravity Up
//--------------------------------

/// <summary>
/// Selects Gravity Up direction
/// </summary>
function LevelBuilderToolRightPassiveView::selectGravityUp(%this)
{
    Lbtrpv_Vb_Stb_Gtp_UpButton.setStateOn(true);
}

/// <summary>
/// Handles a Click event on the Gravity Up control
/// </summary>
function Lbtrpv_Vb_Stb_Gtp_UpButton::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::GravityUpSelected");
}

//--------------------------------
// Gravity None
//--------------------------------

/// <summary>
/// Selects Gravity None direction
/// </summary>
function LevelBuilderToolRightPassiveView::selectGravityNone(%this)
{
    Lbtrpv_Vb_Stb_Gtp_NoneButton.setStateOn(true);
}

/// <summary>
/// Handles a Click event on the Gravity None control
/// </summary>
function Lbtrpv_Vb_Stb_Gtp_NoneButton::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::GravityNoneSelected");
}

//--------------------------------
// Gravity Down
//--------------------------------

/// <summary>
/// Selects Gravity Down direction
/// </summary>
function LevelBuilderToolRightPassiveView::selectGravityDown(%this)
{
    Lbtrpv_Vb_Stb_Gtp_DownButton.setStateOn(true);
}

/// <summary>
/// Handles a Click event on the Gravity Down control
/// </summary>
function Lbtrpv_Vb_Stb_Gtp_DownButton::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::GravityDownSelected");
}


//----------------------------------------------------------------
// Backgrounds Tab Page
//----------------------------------------------------------------

//------------------------------------------------
// Format
//------------------------------------------------

//--------------------------------
// Level Size
//--------------------------------

/// <summary>
/// Selects the given Level Size
/// </summary>
/// <param="levelSize">The Level Size to Select</param>
function LevelBuilderToolRightPassiveView::selectLevelSize(%this, %levelSize)
{
    Lbtrpv_Vb_Stb_Btp_Fc_LevelSizeDropdown.setSelected(Lbtrpv_Vb_Stb_Btp_Fc_LevelSizeDropdown.findText(%levelSize));
}

/// <summary>
/// Adds the given Size to the Level Size List
/// </summary>
/// <param="size">The Size to Add</param>
function LevelBuilderToolRightPassiveView::addToLevelSizeList(%this, %size)
{
    Lbtrpv_Vb_Stb_Btp_Fc_LevelSizeDropdown.add(%size, Lbtrpv_Vb_Stb_Btp_Fc_LevelSizeDropdown.size());
}

/// <summary>
/// Gets the Selected Level Size
/// </summary>
function LevelBuilderToolRightPassiveView::getSelectedLevelSize(%this)
{
    return Lbtrpv_Vb_Stb_Btp_Fc_LevelSizeDropdown.getText();
}

/// <summary>
/// Handles a Selection in the Level Size List
/// </summary>
function Lbtrpv_Vb_Stb_Btp_Fc_LevelSizeDropdown::onSelect(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::LevelSizeSelect");
}

//--------------------------------
// Stretch
//--------------------------------

/// <summary>
/// Selects the Stretch Background Format
/// </summary>
function LevelBuilderToolRightPassiveView::selectBackgroundFormatStretch(%this)
{
    Lbtrpv_Vb_Stb_Btp_Fc_StretchRadio.setStateOn(true);
}

/// <summary>
/// Handles the Stretch Background Format Selection
/// </summary>
function Lbtrpv_Vb_Stb_Btp_Fc_StretchRadio::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::BackgroundFormatStretchSelect");
}

//--------------------------------
// Tile
//--------------------------------

/// <summary>
/// Selects the Tile Background Format
/// </summary>
function LevelBuilderToolRightPassiveView::selectBackgroundFormatTile(%this)
{
    Lbtrpv_Vb_Stb_Btp_Fc_TileRadio.setStateOn(true);
}

/// <summary>
/// Queries whether the Background Format is Tile
/// </summary>
function LevelBuilderToolRightPassiveView::getBackgroundFormatIsTile(%this)
{
    return Lbtrpv_Vb_Stb_Btp_Fc_TileRadio.getStateOn();
}

/// <summary>
/// Handles the Tile Background Format Selection
/// </summary>
function Lbtrpv_Vb_Stb_Btp_Fc_TileRadio::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::BackgroundFormatTileSelect");
}


//------------------------------------------------
// Foreground
//------------------------------------------------

//--------------------------------
// Asset
//--------------------------------

/// <summary>
/// Sets the Foreground Asset Display to the given Asset
/// </summary>
/// <param="asset">The Asset to which to Set the Display</param>
function LevelBuilderToolRightPassiveView::setForegroundAssetDisplay(%this, %asset)
{
    Lbtrpv_Vb_Stb_Btp_Fc_ForegroundAssetDisplay.setText(%asset);
}

/// <summary>
/// Handles a Click event on the Foreground Asset Library control
/// </summary>
function Lbtrpv_Vb_Stb_Btp_Fc_AssetLibraryButton::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::ForegroundAssetSelect");
}

//--------------------------------
// Parallax Speed
//--------------------------------

/// <summary>
/// Adds the given Speed to the Foreground Parallax Speed List
/// </summary>
/// <param="asset">The Speed to Add</param>
function LevelBuilderToolRightPassiveView::addToForegroundParallaxSpeedList(%this, %speed)
{
    Lbtrpv_Vb_Stb_Btp_Fc_ParallaxSpeedDropdown.add(%speed, Lbtrpv_Vb_Stb_Btp_Fc_ParallaxSpeedDropdown.size());
}

/// <summary>
/// Gets the Selected Foreground Parallax Speed
/// </summary>
function LevelBuilderToolRightPassiveView::getSelectedForegroundParallaxSpeed(%this)
{
    return Lbtrpv_Vb_Stb_Btp_Fc_ParallaxSpeedDropdown.getText();
}

/// <summary>
/// Selects the given Foreground Parallax Speed
/// </summary>
/// <param="speed">The Speed to Select</param>
function LevelBuilderToolRightPassiveView::selectForegroundParallaxSpeed(%this, %speed)
{
    Lbtrpv_Vb_Stb_Btp_Fc_ParallaxSpeedDropdown.setSelected(Lbtrpv_Vb_Stb_Btp_Fc_ParallaxSpeedDropdown.findText(%speed));
}

/// <summary>
/// Handles a Selection in the Foreground Parallax Speed List
/// </summary>
function Lbtrpv_Vb_Stb_Btp_Fc_ParallaxSpeedDropdown::onSelect(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::ForegroundParallaxSpeedSelect");
}


//------------------------------------------------
// Background 1
//------------------------------------------------

//--------------------------------
// Asset
//--------------------------------

/// <summary>
/// Sets the Background 1 Asset Display to the given Asset
/// </summary>
/// <param="asset">The Asset to which to Set the Display</param>
function LevelBuilderToolRightPassiveView::setBackground1AssetDisplay(%this, %asset)
{
    Lbtrpv_Vb_Stb_Btp_B1c_Background1AssetDisplay.setText(%asset);
}

/// <summary>
/// Handles a Click event on the Background 1 Asset Library control
/// </summary>
function Lbtrpv_Vb_Stb_Btp_B1c_AssetLibraryButton::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::Background1AssetSelect");
}

//--------------------------------
// Parallax Speed
//--------------------------------

/// <summary>
/// Adds the given Speed to the Background 1 Parallax Speed List
/// </summary>
/// <param="asset">The Speed to Add</param>
function LevelBuilderToolRightPassiveView::addToBackground1ParallaxSpeedList(%this, %speed)
{
    Lbtrpv_Vb_Stb_Btp_B1c_ParallaxSpeedDropdown.add(%speed, Lbtrpv_Vb_Stb_Btp_B1c_ParallaxSpeedDropdown.size());
}

/// <summary>
/// Gets the Selected Background 1 Parallax Speed
/// </summary>
function LevelBuilderToolRightPassiveView::getSelectedBackground1ParallaxSpeed(%this)
{
    return Lbtrpv_Vb_Stb_Btp_B1c_ParallaxSpeedDropdown.getText();
}

/// <summary>
/// Selects the given Background 1 Parallax Speed
/// </summary>
/// <param="speed">The Speed to Select</param>
function LevelBuilderToolRightPassiveView::selectBackground1ParallaxSpeed(%this, %speed)
{
    Lbtrpv_Vb_Stb_Btp_B1c_ParallaxSpeedDropdown.setSelected(Lbtrpv_Vb_Stb_Btp_B1c_ParallaxSpeedDropdown.findText(%speed));
}

/// <summary>
/// Handles a Selection in the Background 1 Parallax Speed List
/// </summary>
function Lbtrpv_Vb_Stb_Btp_B1c_ParallaxSpeedDropdown::onSelect(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::Background1ParallaxSpeedSelect");
}


//------------------------------------------------
// Background 2
//------------------------------------------------

//--------------------------------
// Asset
//--------------------------------

/// <summary>
/// Sets the Background 2 Asset Display to the given Asset
/// </summary>
/// <param="asset">The Asset to which to Set the Display</param>
function LevelBuilderToolRightPassiveView::setBackground2AssetDisplay(%this, %asset)
{
    Lbtrpv_Vb_Stb_Btp_B2c_Background2AssetDisplay.setText(%asset);
}

/// <summary>
/// Handles a Click event on the Background 2 Asset Library control
/// </summary>
function Lbtrpv_Vb_Stb_Btp_B2c_AssetLibraryButton::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::Background2AssetSelect");
}

//--------------------------------
// Parallax Speed
//--------------------------------

/// <summary>
/// Adds the given Speed to the Background 2 Parallax Speed List
/// </summary>
/// <param="asset">The Speed to Add</param>
function LevelBuilderToolRightPassiveView::addToBackground2ParallaxSpeedList(%this, %speed)
{
    Lbtrpv_Vb_Stb_Btp_B2c_ParallaxSpeedDropdown.add(%speed, Lbtrpv_Vb_Stb_Btp_B2c_ParallaxSpeedDropdown.size());
}

/// <summary>
/// Gets the Selected Background 2 Parallax Speed
/// </summary>
function LevelBuilderToolRightPassiveView::getSelectedBackground2ParallaxSpeed(%this)
{
    return Lbtrpv_Vb_Stb_Btp_B2c_ParallaxSpeedDropdown.getText();
}

/// <summary>
/// Selects the given Background 2 Parallax Speed
/// </summary>
/// <param="speed">The Speed to Select</param>
function LevelBuilderToolRightPassiveView::selectBackground2ParallaxSpeed(%this, %speed)
{
    Lbtrpv_Vb_Stb_Btp_B2c_ParallaxSpeedDropdown.setSelected(Lbtrpv_Vb_Stb_Btp_B2c_ParallaxSpeedDropdown.findText(%speed));
}

/// <summary>
/// Handles a Selection in the Background 2 Parallax Speed List
/// </summary>
function Lbtrpv_Vb_Stb_Btp_B2c_ParallaxSpeedDropdown::onSelect(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::Background2ParallaxSpeedSelect");
}


//------------------------------------------------
// Sky
//------------------------------------------------

//--------------------------------
// Asset
//--------------------------------

/// <summary>
/// Sets the Sky Asset Display to the given Asset
/// </summary>
/// <param="asset">The Asset to which to Set the Display</param>
function LevelBuilderToolRightPassiveView::setSkyAssetDisplay(%this, %asset)
{
    Lbtrpv_Vb_Stb_Btp_Sc_SkyAssetDisplay.setText(%asset);
}

/// <summary>
/// Handles a Click event on the Sky Asset Library control
/// </summary>
function Lbtrpv_Vb_Stb_Btp_Sc_AssetLibraryButton::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::SkyAssetSelect");
}

//--------------------------------
// Parallax Speed
//--------------------------------

/// <summary>
/// Adds the given Speed to the Sky Parallax Speed List
/// </summary>
/// <param="asset">The Speed to Add</param>
function LevelBuilderToolRightPassiveView::addToSkyParallaxSpeedList(%this, %speed)
{
    Lbtrpv_Vb_Stb_Btp_Sc_ParallaxSpeedDropdown.add(%speed, Lbtrpv_Vb_Stb_Btp_Sc_ParallaxSpeedDropdown.size());
}

/// <summary>
/// Gets the Selected Sky Parallax Speed
/// </summary>
function LevelBuilderToolRightPassiveView::getSelectedSkyParallaxSpeed(%this)
{
    return Lbtrpv_Vb_Stb_Btp_Sc_ParallaxSpeedDropdown.getText();
}

/// <summary>
/// Selects the given Sky Parallax Speed
/// </summary>
/// <param="speed">The Speed to Select</param>
function LevelBuilderToolRightPassiveView::selectSkyParallaxSpeed(%this, %speed)
{
    Lbtrpv_Vb_Stb_Btp_Sc_ParallaxSpeedDropdown.setSelected(Lbtrpv_Vb_Stb_Btp_Sc_ParallaxSpeedDropdown.findText(%speed));
}

/// <summary>
/// Handles a Selection in the Sky Parallax Speed List
/// </summary>
function Lbtrpv_Vb_Stb_Btp_Sc_ParallaxSpeedDropdown::onSelect(%this)
{    
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolRightPassiveView::SkyParallaxSpeedSelect");
}