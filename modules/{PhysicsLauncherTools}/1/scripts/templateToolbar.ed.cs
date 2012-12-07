//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------



/// <summary>
/// Loads the Level Builder Tool upon selection in the Template Toolbar
/// </summary>
function Tt_LevelBuilderToolButton::onClick(%this)
{
    if (!LevelBuilderToolLeftPassiveView.isAwake())
        LevelBuilderToolPresenter.load();
    Ct_AssetLibraryButton.setStateOn(false);
}

/// <summary>
/// Loads the Launcher Tool upon selection in the Template Toolbar
/// </summary>
function Tt_LauncherToolButton::onClick(%this)
{
    if (!LauncherToolGui.isAwake())
        LauncherTool.load();
    Ct_AssetLibraryButton.setStateOn(false);
}

/// <summary>
/// Loads the Projectile Tool upon selection in the Template Toolbar
/// </summary>
function Tt_ProjectileToolButton::onClick(%this)
{
    if (!ProjectileToolForm.isAwake())
        ProjectileTool.load();
    Ct_AssetLibraryButton.setStateOn(false);
}

/// <summary>
/// Loads the World Object Tool upon selection in the Template Toolbar
/// </summary>
function Tt_WorldObjectToolButton::onClick(%this)
{
    if (!WorldObjectToolGui.isAwake())
        WorldObjectTool.load();
    Ct_AssetLibraryButton.setStateOn(false);
}

/// <summary>
/// Loads the World Tool upon selection in the Template Toolbar
/// </summary>
function Tt_WorldToolButton::onClick(%this)
{
    if (!WorldTool.isAwake())
        WorldTool.load();
    Ct_AssetLibraryButton.setStateOn(false);
}

/// <summary>
/// Loads the Collision Editor upon selection in the Template Toolbar
/// </summary>
function Tt_CollisionButton::onClick(%this)
{
    CollisionSidebar.load(0, false);
    Ct_AssetLibraryButton.setStateOn(false);
}

/// <summary>
/// Loads the Interface Editor upon selection in the Template Toolbar
/// </summary>
function Tt_InterfaceButton::onClick(%this)
{
    if (!InterfaceTool.isAwake())
        InterfaceTool.load();
    Ct_AssetLibraryButton.setStateOn(false);
}

function TemplateToolbar::clearButtonSelect(%this)
{
    Tt_CollisionButton.setStateOn(false);
    Tt_InterfaceButton.setStateOn(false);
    Tt_LauncherToolButton.setStateOn(false);
    Tt_LevelBuilderToolButton.setStateOn(false);
    Tt_ProjectileToolButton.setStateOn(false);
    Tt_WorldObjectToolButton.setStateOn(false);
    Tt_WorldToolButton.setStateOn(false);
}