//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------



/// <summary>
/// Loads the Asset Library
/// </summary>
function Ct_AssetLibraryButton::onClick(%this)
{
    ResetCommonToolButtons();
    %this.setStateOn(true);
    if (!AssetLibraryWindow.isAwake())
        AssetLibrary.open();
    TemplateToolbar.clearButtonSelect();
}

/// <summary>
/// Tests the current project
/// </summary>
function Ct_TestButton::onClick(%this)
{
    ResetCommonToolButtons();
    %this.setStateOn(true);
    Canvas.pushDialog(TestGameWindowGui);
}

/// <summary>
/// Publishes the current project
/// </summary>
function Ct_PublishButton::onClick(%this)
{
    ResetCommonToolButtons();
    %this.setStateOn(true);
    Canvas.pushDialog(PublisherGui);
}

/// <summary>
/// Exits the Editor
/// </summary>
function Ct_ExitButton::onClick(%this)
{
    Projects::GetEventManager().schedule(0, postEvent, "_ProjectClose");
    ModuleDatabase.unloadGroup(projectTools);
    ResetCommonToolButtons();
    %this.setStateOn(true);
    Hs_HomeButton.onClick();
    %this.setStateOn(false);
    EditorShellGui.setCommonToolBar("");
    EditorShellGui.setToolBar(HomeScreenToolbar);
}

function ResetCommonToolButtons()
{
    Ct_AssetLibraryButton.setStateOn(false);
    Ct_TestButton.setStateOn(false);
    Ct_PublishButton.setStateOn(false);
    Ct_ExitButton.setStateOn(false);
}