//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------



//--------------------------------------------------------------------------------------------------------------------------------
// Level Builder Tool Right Passive View Interface
//--------------------------------------------------------------------------------------------------------------------------------

/// <summary>
/// Level Builder Tool Right Passive View Interface Object
/// </summary>
new GuiControl(LevelBuilderToolRightPassiveViewInterface)
{
    
};


//------------------------------------------------------------------------------------------------
// Scene Tab Book
//------------------------------------------------------------------------------------------------

/// <summary>
/// Enables/Disables the Scene Tab Book
/// </summary>
/// <param="enable">True if the Scene Tab Book should be Enabled</param>
function LevelBuilderToolRightPassiveViewInterface::enableSceneTabBook(%this, %enable)
{
    error("@@@" SPC %this @ "::enableSceneTabBook" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Selects the given Tab in the Scene Tab Book
/// </summary>
/// <param="tab">The Tab to Select</param>
function LevelBuilderToolRightPassiveViewInterface::selectSceneTab(%this, %tab)
{
    error("@@@" SPC %this @ "::selectSceneTab" SPC "- NOT IMPLEMENTED" SPC "@@@");
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
function LevelBuilderToolRightPassiveViewInterface::setSelectionToolActive(%this, %active)
{
    error("@@@" SPC %this @ "::setSelectionToolActive" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Selects the Selection Tool
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::selectSelectionTool(%this)
{
    error("@@@" SPC %this @ "::selectSelectionTool" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Rotate
//--------------------------------

/// <summary>
/// Sets Rotate as Active/Inactive
/// </summary>
/// <param="active">True if Rotate should be Active</param>
function LevelBuilderToolRightPassiveViewInterface::setRotateActive(%this, %active)
{
    error("@@@" SPC %this @ "::setRotateActive" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Duplicate
//--------------------------------

/// <summary>
/// Sets Duplicate as Active/Inactive
/// </summary>
/// <param="active">True if Duplicate should be Active</param>
function LevelBuilderToolRightPassiveViewInterface::setDuplicateActive(%this, %active)
{
    error("@@@" SPC %this @ "::setDuplicateActive" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Bring to Front
//--------------------------------

/// <summary>
/// Sets Bring to Front as Active/Inactive
/// </summary>
/// <param="active">True if Brint to Front should be Active</param>
function LevelBuilderToolRightPassiveViewInterface::setBringToFrontActive(%this, %active)
{
    error("@@@" SPC %this @ "::setBringToFrontActive" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Send to Back
//--------------------------------

/// <summary>
/// Sets Send to Back as Active/Inactive
/// </summary>
/// <param="active">True if Send to Back should be Active</param>
function LevelBuilderToolRightPassiveViewInterface::setSendToBackActive(%this, %active)
{
    error("@@@" SPC %this @ "::setSendToBackActive" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Delete
//--------------------------------

/// <summary>
/// Sets Delete as Active/Inactive
/// </summary>
/// <param="active">True if Delete should be Active</param>
function LevelBuilderToolRightPassiveViewInterface::setDeleteActive(%this, %active)
{
    error("@@@" SPC %this @ "::setDeleteActive" SPC "- NOT IMPLEMENTED" SPC "@@@");
}


//------------------------------------------------
// Selected Object Details
//------------------------------------------------

/// <summary>
/// Sets the Display of the Name of the Selected Object
/// </summary>
/// <param="name">The Name to Display</param>
function LevelBuilderToolRightPassiveViewInterface::setSelectedObjectNameDisplay(%this, %name)
{
    error("@@@" SPC %this @ "::setSelectedObjectNameDisplay" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Selected Object Preview
//--------------------------------

/// <summary>
/// Sets Up the Selected Object Preview
/// </summary>
/// <param="object">The Selected Object to Preview</param>
function LevelBuilderToolRightPassiveViewInterface::setupSelectedObjectPreview(%this, %object)
{
    error("@@@" SPC %this @ "::setupSelectedObjectPreview" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Clears the Selected Object Preview
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::clearSelectedObjectPreview(%this)
{
    error("@@@" SPC %this @ "::clearSelectedObjectPreview" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Launcher Background Preview
//--------------------------------

/// <summary>
/// Sets Up the Launcher Background Preview (used when the Launcher is the Selected Object)
/// </summary>
/// <param="object">The Launcher Background to Preview</param>
function LevelBuilderToolRightPassiveViewInterface::setupLauncherBackgroundPreview(%this, %background)
{
    error("@@@" SPC %this @ "::setupLauncherBackgroundPreview" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Clears the Launcher Background Preview
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::clearLauncherBackgroundPreview(%this)
{
    error("@@@" SPC %this @ "::clearLauncherBackgroundPreview" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Selected Object Stats
//--------------------------------

/// <summary>
/// Sets whether the Selected Object Stats are Visible/Invisible
/// </summary>
/// <param="visible">True if the Selected Object Stats should be Visible</param>
function LevelBuilderToolRightPassiveViewInterface::setSelectedObjectStatsVisible(%this, %visible)
{
    error("@@@" SPC %this @ "::setSelectedObjectStatsVisible" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Sets the Display of the Mass of the Selected Object
/// </summary>
/// <param="mass">The Mass of the Selected Object</param>
function LevelBuilderToolRightPassiveViewInterface::setSelectedObjectMassDisplay(%this, %mass)
{
    error("@@@" SPC %this @ "::setSelectedObjectMass" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Sets the Display of the Friction of the Selected Object
/// </summary>
/// <param="friction">The Friction of the Selected Object</param>
function LevelBuilderToolRightPassiveViewInterface::setSelectedObjectFrictionDisplay(%this, %friction)
{
    error("@@@" SPC %this @ "::setSelectedObjectFriction" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Sets the Display of the Bounce of the Selected Object
/// </summary>
/// <param="bounce">The Bounce of the Selected Object</param>
function LevelBuilderToolRightPassiveViewInterface::setSelectedObjectBounceDisplay(%this, %bounce)
{
    error("@@@" SPC %this @ "::setSelectedObjectBounce" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Sets the Display of the Hit Points of the Selected Object
/// </summary>
/// <param="hitPoints">The Hit Points of the Selected Object</param>
function LevelBuilderToolRightPassiveViewInterface::setSelectedObjectHitPointsDisplay(%this, %hitPoints)
{
    error("@@@" SPC %this @ "::setSelectedObjectHitPoints" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Selected Object Edit
//--------------------------------

/// <summary>
/// Sets the Text on the Selected Object Edit control
/// </summary>
/// <param="text">The Text for the Edit control</param>
function LevelBuilderToolRightPassiveViewInterface::setSelectedObjectEditText(%this, %text)
{
    error("@@@" SPC %this @ "::setSelectedObjectEditText" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Sets the Selected Object Edit control as Active/Inactive
/// </summary>
/// <param="active">True if the control should be Active</param>
function LevelBuilderToolRightPassiveViewInterface::setSelectedObjectEditActive(%this, %active)
{
    error("@@@" SPC %this @ "::setSelectedObjectEditActive" SPC "- NOT IMPLEMENTED" SPC "@@@");
}


//------------------------------------------------
// Object List
//------------------------------------------------

/// <summary>
/// Adds the given Object to the List
/// </summary>
/// <param="object">The Object to Add</param>
function LevelBuilderToolRightPassiveViewInterface::addToObjectList(%this, %object)
{
    error("@@@" SPC %this @ "::addToObjectList" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Refreshes the Scroll of the Object list
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::refreshObjectScroll(%this)
{
    error("@@@" SPC %this @ "::refreshObjectScroll" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Clears the Object List
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::clearObjectList(%this)
{
    error("@@@" SPC %this @ "::clearObjectList" SPC "- NOT IMPLEMENTED" SPC "@@@");
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
function LevelBuilderToolRightPassiveViewInterface::getGravityStrengthListActive(%this)
{
    error("@@@" SPC %this @ "::getGravityStrengthListActive" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Sets whether the Gravity Strength List is Active
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::setGravityStrengthListActive(%this, %active)
{
    error("@@@" SPC %this @ "::setGravityStrengthListActive" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Adds the given Strength to the Gravity Strength List
/// </summary>
/// <param="strength">The Strength to Add</param>
function LevelBuilderToolRightPassiveViewInterface::addToGravityStrengthList(%this, %strength)
{
    error("@@@" SPC %this @ "::addToGravityStrengthList" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Gets the Selected Gravity Strength
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::getSelectedGravityStrength(%this)
{
    error("@@@" SPC %this @ "::getSelectedGravityStrength" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Selects the given Gravity Strength
/// </summary>
/// <param="strength">The desired Strength</param>
/// <param="callback">True if the Callback should be triggered</param>
function LevelBuilderToolRightPassiveViewInterface::selectGravityStrength(%this, %strength, %callback)
{
    error("@@@" SPC %this @ "::selectGravityStrength" SPC "- NOT IMPLEMENTED" SPC "@@@");
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
function LevelBuilderToolRightPassiveViewInterface::selectGravityUp(%this)
{
    error("@@@" SPC %this @ "::selectGravityUp" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Gravity None
//--------------------------------

/// <summary>
/// Selects Gravity None direction
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::selectGravityNone(%this)
{
    error("@@@" SPC %this @ "::selectGravityNone" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Gravity Down
//--------------------------------

/// <summary>
/// Selects Gravity Down direction
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::selectGravityDown(%this)
{
    error("@@@" SPC %this @ "::selectGravityDown" SPC "- NOT IMPLEMENTED" SPC "@@@");
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
function LevelBuilderToolRightPassiveViewInterface::selectLevelSize(%this, %levelSize)
{
    error("@@@" SPC %this @ "::selectLevelSize" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Adds the given Size to the Level Size List
/// </summary>
/// <param="size">The Size to Add</param>
function LevelBuilderToolRightPassiveViewInterface::addToLevelSizeList(%this, %size)
{
    error("@@@" SPC %this @ "::addToLevelSizeList" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Gets the Selected Level Size
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::getSelectedLevelSize(%this)
{
    error("@@@" SPC %this @ "::getSelectedLevelSize" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Stretch
//--------------------------------

/// <summary>
/// Selects the Stretch Background Format
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::selectBackgroundFormatStretch(%this)
{
    error("@@@" SPC %this @ "::selectBackgroundFormatStretch" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Tile
//--------------------------------

/// <summary>
/// Selects the Tile Background Format
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::selectBackgroundFormatTile(%this)
{
    error("@@@" SPC %this @ "::selectBackgroundFormatTile" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Queries whether the Background Format is Tile
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::getBackgroundFormatIsTile(%this)
{
    error("@@@" SPC %this @ "::getBackgroundFormatIsTile" SPC "- NOT IMPLEMENTED" SPC "@@@");
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
function LevelBuilderToolRightPassiveViewInterface::setForegroundAssetDisplay(%this, %asset)
{
    error("@@@" SPC %this @ "::setForegroundAssetDisplay" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Parallax Speed
//--------------------------------

/// <summary>
/// Adds the given Speed to the Foreground Parallax Speed List
/// </summary>
/// <param="asset">The Speed to Add</param>
function LevelBuilderToolRightPassiveViewInterface::addToForegroundParallaxSpeedList(%this, %speed)
{
    error("@@@" SPC %this @ "::addToForegroundParallaxSpeedList" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Gets the Selected Foreground Parallax Speed
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::getSelectedForegroundParallaxSpeed(%this)
{
    error("@@@" SPC %this @ "::getSelectedForegroundParallaxSpeed" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Selects the given Foreground Parallax Speed
/// </summary>
/// <param="speed">The Speed to Select</param>
function LevelBuilderToolRightPassiveViewInterface::selectForegroundParallaxSpeed(%this, %speed)
{
    error("@@@" SPC %this @ "::selectForegroundParallaxSpeed" SPC "- NOT IMPLEMENTED" SPC "@@@");
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
function LevelBuilderToolRightPassiveViewInterface::setBackground1AssetDisplay(%this, %asset)
{
    error("@@@" SPC %this @ "::setBackground1AssetDisplay" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Parallax Speed
//--------------------------------

/// <summary>
/// Adds the given Speed to the Background 1 Parallax Speed List
/// </summary>
/// <param="asset">The Speed to Add</param>
function LevelBuilderToolRightPassiveViewInterface::addToBackground1ParallaxSpeedList(%this, %speed)
{
    error("@@@" SPC %this @ "::addToBackground1ParallaxSpeedList" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Gets the Selected Background 1 Parallax Speed
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::getSelectedBackground1ParallaxSpeed(%this)
{
    error("@@@" SPC %this @ "::getSelectedBackground1ParallaxSpeed" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Selects the given Background 1 Parallax Speed
/// </summary>
/// <param="speed">The Speed to Select</param>
function LevelBuilderToolRightPassiveViewInterface::selectBackground1ParallaxSpeed(%this, %speed)
{
    error("@@@" SPC %this @ "::selectBackground1ParallaxSpeed" SPC "- NOT IMPLEMENTED" SPC "@@@");
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
function LevelBuilderToolRightPassiveViewInterface::setBackground2AssetDisplay(%this, %asset)
{
    error("@@@" SPC %this @ "::setBackground2AssetDisplay" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Parallax Speed
//--------------------------------

/// <summary>
/// Adds the given Speed to the Background 2 Parallax Speed List
/// </summary>
/// <param="asset">The Speed to Add</param>
function LevelBuilderToolRightPassiveViewInterface::addToBackground2ParallaxSpeedList(%this, %speed)
{
    error("@@@" SPC %this @ "::addToBackground2ParallaxSpeedList" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Gets the Selected Background 2 Parallax Speed
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::getSelectedBackground2ParallaxSpeed(%this)
{
    error("@@@" SPC %this @ "::getSelectedBackground2ParallaxSpeed" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Selects the given Background 2 Parallax Speed
/// </summary>
/// <param="speed">The Speed to Select</param>
function LevelBuilderToolRightPassiveViewInterface::selectBackground2ParallaxSpeed(%this, %speed)
{
    error("@@@" SPC %this @ "::selectBackground2ParallaxSpeed" SPC "- NOT IMPLEMENTED" SPC "@@@");
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
function LevelBuilderToolRightPassiveViewInterface::setSkyAssetDisplay(%this, %asset)
{
    error("@@@" SPC %this @ "::setSkyAssetDisplay" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

//--------------------------------
// Parallax Speed
//--------------------------------

/// <summary>
/// Adds the given Speed to the Sky Parallax Speed List
/// </summary>
/// <param="asset">The Speed to Add</param>
function LevelBuilderToolRightPassiveViewInterface::addToSkyParallaxSpeedList(%this, %speed)
{
    error("@@@" SPC %this @ "::addToSkyParallaxSpeedList" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Gets the Selected Sky Parallax Speed
/// </summary>
function LevelBuilderToolRightPassiveViewInterface::getSelectedSkyParallaxSpeed(%this)
{
    error("@@@" SPC %this @ "::getSelectedSkyParallaxSpeed" SPC "- NOT IMPLEMENTED" SPC "@@@");
}

/// <summary>
/// Selects the given Sky Parallax Speed
/// </summary>
/// <param="speed">The Speed to Select</param>
function LevelBuilderToolRightPassiveViewInterface::selectSkyParallaxSpeed(%this, %speed)
{
    error("@@@" SPC %this @ "::selectSkyParallaxSpeed" SPC "- NOT IMPLEMENTED" SPC "@@@");
}