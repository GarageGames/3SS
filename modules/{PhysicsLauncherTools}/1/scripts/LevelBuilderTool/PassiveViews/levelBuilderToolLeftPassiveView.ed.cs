//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------



//--------------------------------------------------------------------------------------------------------------------------------
// Level Builder Tool Left Passive View
//--------------------------------------------------------------------------------------------------------------------------------

/// <summary>
/// Posts Event on Level Builder Tool Left Passive View Wake
/// </summary>
function LevelBuilderToolLeftPassiveView::onWake(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolLeftPassiveView::Wake");
}

/// <summary>
/// Posts Event on Level Builder Tool Left Passive View Sleep 
/// </summary>
function LevelBuilderToolLeftPassiveView::onSleep(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolLeftPassiveView::Sleep");
}

/// <summary>
/// Handles a Control Dropped Event
/// </summary>
/// <param="control">Control being Dropped</param>
/// <param="position">Position at which the Control is being Dropped</param>
function LevelBuilderToolLeftPassiveView::onControlDropped(%this, %control, %position)
{
    %data = new ScriptObject()
    {
        control = %control;
        position = %position;
    };
    
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolLeftPassiveView::ControlDropped", %data);
}


//------------------------------------------------------------------------------------------------
// World List
//------------------------------------------------------------------------------------------------

/// <summary>
/// Sets the World List as Active/Inactive
/// </summary>
/// <param="active">True if the World List should be Active</param>
function LevelBuilderToolLeftPassiveView::setWorldListActive(%this, %active)
{
    Lbtlpv_WorldDropdown.setActive(%active);
}

/// <summary>
/// Adds the given World to the World List
/// </summary>
/// <param="world">The World to Add</param>
function LevelBuilderToolLeftPassiveView::addToWorldList(%this, %world)
{
    Lbtlpv_WorldDropdown.add(%world, Lbtlpv_WorldDropdown.size());
}

/// <summary>
/// Clears the World List
/// </summary>
function LevelBuilderToolLeftPassiveView::clearWorldList(%this)
{
    Lbtlpv_WorldDropdown.clear();
}

/// <summary>
/// Gets the Selected World
/// </summary>
function LevelBuilderToolLeftPassiveView::getSelectedWorld(%this)
{
    return Lbtlpv_WorldDropdown.getText();
}

/// <summary>
/// Selects the given World
/// </summary>
/// <param="world">The World to Select</param>
/// <param="callback">True if the Callback should be triggered</param>
function LevelBuilderToolLeftPassiveView::selectWorld(%this, %world, %callback)
{
    Lbtlpv_WorldDropdown.setSelected(Lbtlpv_WorldDropdown.findText(%world), %callback);
} 

/// <summary>
/// Handles a Selection in the World List
/// </summary>
function Lbtlpv_WorldDropdown::onSelect(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolLeftPassiveView::WorldSelect");
}


//------------------------------------------------------------------------------------------------
// Level List
//------------------------------------------------------------------------------------------------

/// <summary>
/// Sets the Level List as Active/Inactive
/// </summary>
/// <param="active">True if the Level List should be Active</param>
function LevelBuilderToolLeftPassiveView::setLevelListActive(%this, %active)
{
    Lbtlpv_LevelDropdown.setActive(%active);
}

/// <summary>
/// Adds the given Level to the Level List
/// </summary>
/// <param="world">The Level to Add</param>
function LevelBuilderToolLeftPassiveView::addToLevelList(%this, %level)
{
    Lbtlpv_LevelDropdown.add(%level, Lbtlpv_LevelDropdown.size());
}

/// <summary>
/// Clears the Level List
/// </summary>
function LevelBuilderToolLeftPassiveView::clearLevelList(%this)
{
    Lbtlpv_LevelDropdown.clear();
}

/// <summary>
/// Gets the Selected Level
/// </summary>
function LevelBuilderToolLeftPassiveView::getSelectedLevel(%this)
{
    return Lbtlpv_LevelDropdown.getText();
}

/// <summary>
/// Selects the given Level
/// </summary>
/// <param="world">The Level to Select</param>
/// <param="callback">True if the Callback should be triggered</param>
function LevelBuilderToolLeftPassiveView::selectLevel(%this, %level, %callback)
{
    Lbtlpv_LevelDropdown.setSelected(Lbtlpv_LevelDropdown.findText(%level), %callback);
}

/// <summary>
/// Handles a Selection in the Level List
/// </summary>
function Lbtlpv_LevelDropdown::onSelect(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolLeftPassiveView::LevelSelect");
}


//------------------------------------------------------------------------------------------------
// Preview Physics
//------------------------------------------------------------------------------------------------

/// <summary>
/// Posts Event on Preview Physics toggle
/// </summary>
function Lbtlpv_PreviewPhysicsButton::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolLeftPassiveView::PhysicsPreviewToggle");
}


//------------------------------------------------------------------------------------------------
// Collision
//------------------------------------------------------------------------------------------------

/// <summary>
/// Queries Collision visibility state
/// </summary>
function LevelBuilderToolLeftPassiveView::getCollisionOn(%this)
{
    return Lbtlpv_CollisionCheckBox.getStateOn();
}

/// <summary>
/// Posts Event on Collision toggle
/// </summary>
function Lbtlpv_CollisionCheckBox::onClick(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolLeftPassiveView::CollisionToggle");
}


//------------------------------------------------------------------------------------------------
// Zoom List
//------------------------------------------------------------------------------------------------

/// <summary>
/// Adds the given Option to the Zoom List
/// </summary>
/// <param="zoomOption">The Zoom Option to Add</param>
function LevelBuilderToolLeftPassiveView::addZoomOption(%this, %zoomOption)
{
    Lbtlpv_ZoomDropdown.add(%zoomOption, Lbtlpv_ZoomDropdown.size());
}

/// <summary>
/// Gets the Selected Zoom Option
/// </summary>
function LevelBuilderToolLeftPassiveView::getSelectedZoomOption(%this)
{
    return Lbtlpv_ZoomDropdown.getText();
}

/// <summary>
/// Selects the given Zoom Option
/// </summary>
/// <param="zoomOption">The Zoom Option to Select</param>
function LevelBuilderToolLeftPassiveView::selectZoomOption(%this, %zoomOption)
{
    Lbtlpv_ZoomDropdown.setSelected(Lbtlpv_ZoomDropdown.findText(%zoomOption));
}

/// <summary>
/// Posts Event on Zoom selection
/// </summary>
function Lbtlpv_ZoomDropdown::onSelect(%this)
{
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolLeftPassiveView::ZoomSelect");
}


//------------------------------------------------------------------------------------------------
// Scene View Scroll
//------------------------------------------------------------------------------------------------

/// <summary>
/// Queries the Scene Scroll Size
/// </summary>
function LevelBuilderToolLeftPassiveView::getSceneScrollSize(%this)
{
    return Lbtlpv_Sc_SceneScroll.getExtent();
}

/// <summary>
/// Gets the current Scroll Y-coordinate of the Scene Scroll
/// </summary>
function LevelBuilderToolLeftPassiveView::getSceneScrollScrollPositionY(%this)
{
    return Lbtlpv_Sc_SceneScroll.getScrollPositionY();
}

/// <summary>
/// Scrolls the Scene Scroll to the Bottom
/// </summary>
function LevelBuilderToolLeftPassiveView::scrollSceneScrollToBottom(%this)
{
    Lbtlpv_Sc_SceneScroll.scrollToBottom();
}

/// <summary>
/// Sets the Scroll Position of the Scene Scroll
/// </summary>
/// <param="x">The desired X-coordinate</param>
/// <param="y">The desired Y-coordinate</param>
function LevelBuilderToolLeftPassiveView::setSceneScrollScrollPosition(%this, %x, %y)
{
    Lbtlpv_Sc_SceneScroll.setScrollPosition(%x, %y);
}

/// <summary>
/// Refreshes the Scene Scroll
/// </summary>
function LevelBuilderToolLeftPassiveView::refreshSceneScroll(%this)
{
    Lbtlpv_Sc_SceneScroll.computeSizes();
}


//----------------------------------------------------------------
// Scene View
//----------------------------------------------------------------

/// <summary>
/// Gets the Global Position of the Scene View
/// </summary>
function LevelBuilderToolLeftPassiveView::getSceneViewGlobalPosition(%this)
{
    return Lbtlpv_Sc_Ss_SceneView.getGlobalPosition();
}

/// <summary>
/// Sets the Layers which will be editable in the Scene View
/// </summary>
/// <param="layerMask">The Layers on which to allow edits</param>
function LevelBuilderToolLeftPassiveView::setSceneViewLayerMask(%this, %layerMask)
{
    Lbtlpv_Sc_Ss_SceneView.setLayerMask(%layerMask);
}

/// <summary>
/// Sets the Scene Editor for the Scene View
/// </summary>
/// <param="sceneEdit">The desired Editor</param>
function LevelBuilderToolLeftPassiveView::setSceneViewSceneEdit(%this, %sceneEdit)
{
    Lbtlpv_Sc_Ss_SceneView.setSceneEdit(%sceneEdit);
}

/// <summary>
/// Sets the Size of the Scene View
/// </summary>
/// <param="width">The desired Width</param>
/// <param="width">The desired Height</param>
function LevelBuilderToolLeftPassiveView::setSceneViewSize(%this, %width, %height)
{
    Lbtlpv_Sc_Ss_SceneView.setExtent(%width, %height);
}

/// <summary>
/// Sets the Scene for the Scene View
/// </summary>
/// <param="scene">The desired Scene</param>
function LevelBuilderToolLeftPassiveView::setSceneViewScene(%this, %scene)
{
    Lbtlpv_Sc_Ss_SceneView.setScene(%scene);
}

/// <summary>
/// Sets the Camera Position and Size for the Scene View
/// </summary>
/// <param="x">The X-coordinate of the Camera Position</param>
/// <param="y">The Y-coordinate of the Camera Position</param>
/// <param="width">The Camera width</param>
/// <param="height">The Camera height</param>
function LevelBuilderToolLeftPassiveView::setSceneViewCameraPositionAndSize(%this, %x, %y, %width, %height)
{
    Lbtlpv_Sc_Ss_SceneView.setCurrentCameraPosition(%x, %y, %width, %height);
}

/// <summary>
/// Gets the corresponding World Point in the Scene of the given Position
/// </summary>
/// <param="position">The Position to convert</param>
function LevelBuilderToolLeftPassiveView::getSceneViewWorldPoint(%this, %position)
{
    return Lbtlpv_Sc_Ss_SceneView.getWorldPoint(%position);
}

/// <summary>
/// Gets the corresponding Window Point of the given Position in the Scene
/// </summary>
/// <param="position">The Position to convert</param>
function LevelBuilderToolLeftPassiveView::getSceneViewWindowPoint(%this, %position)
{
    return Lbtlpv_Sc_Ss_SceneView.getWindowPoint(%position);
}

/// <summary>
/// Adds the given object to the Scene View
/// </summary>
/// <param="object">The Object to add</param>
function LevelBuilderToolLeftPassiveView::addToSceneView(%this, %object)
{
    Lbtlpv_Sc_Ss_SceneView.add(%object);
}


//------------------------------------------------
// Scene View Mouse Event Control
//------------------------------------------------

/// <summary>
/// Handles a Mouse Move Event in the Scene View
/// </summary>
/// <param="modifier">Modifier present, if any, during Move</param>
/// <param="mousePoint">The Point at which the Move occurred</param>
/// <param="mouseClickCount">Number of times the Mouse has been Clicked</param>
function Lbtlpv_Sc_Ss_Sv_MouseEventCtrl::onMouseMove(%this, %modifier, %mousePoint, %mouseClickCount)
{
    %data = new ScriptObject()
    {
        mousePoint = %mousePoint;
        globalPosition = %this.getGlobalPosition();
        object = %this.getObject(0);
    };
    
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolLeftPassiveView::DuplicationStampMove", %data);
}

/// <summary>
/// Handles a Mouse Down Event in the Scene View
/// </summary>
/// <param="modifier">Modifier present, if any, during Down event</param>
/// <param="mousePoint">The Point at which the Down event occurred</param>
/// <param="mouseClickCount">Number of times the Mouse has been Clicked</param>
function Lbtlpv_Sc_Ss_Sv_MouseEventCtrl::onMouseDown(%this, %modifier, %mousePoint, %mouseClickCount)
{
    %data = new ScriptObject()
    {
        mousePoint = %mousePoint;
        globalPosition = %this.getGlobalPosition();
        object = %this.getObject(0);
    };
    
    PhysicsLauncherToolsEventManager.postEvent("LevelBuilderToolLeftPassiveView::DuplicationStampDown", %data);
}

//------------------------------------------------
// Duplication Stamp
//------------------------------------------------

/// <summary>
/// Prepares the Scene View Mouse Event Control to Accept the Stamp to Duplicate the
/// currently selected object
/// </summary>
/// <param="accept">True if the control should Accept the Stamp</param>
function LevelBuilderToolLeftPassiveView::setAcceptDuplicationStamp(%this, %accept)
{
    Lbtlpv_Sc_Ss_Sv_MouseEventCtrl.setProfile(%accept ? "GuiDefaultProfile" : "GuiModelessDialogProfile");
}

/// <summary>
/// Adds the given Stamp, which is a Duplicate of the currently selected object,
/// to the Scene
/// </summary>
/// <param="stamp">Duplication Stamp to Add</param>
function LevelBuilderToolLeftPassiveView::addDuplicationStamp(%this, %stamp)
{
    Lbtlpv_Sc_Ss_Sv_MouseEventCtrl.add(%stamp);
}