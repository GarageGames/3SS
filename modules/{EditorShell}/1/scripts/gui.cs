//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function EditorShellGui::onWake(%this)
{
    // Check to see if initialization occurred before doing this
    if (%this.isInitialized != true)
        %this.initialize();
}

function EditorShellGui::initialize(%this)
{
    %this.lastRefreshState = $BaseEditorRefreshState::All;
    
    if ($WebServicess::AvatarAsset !$= "")
    {
        ESShowAvatarButton.NormalImage = $WebServicess::AvatarAsset;
        ESShowAvatarButton.DownImage = $WebServicess::AvatarAsset;
        ESShowAvatarButton.HoverImage = $WebServicess::AvatarAsset;
        ESShowAvatarButton.InactiveImage = $WebServicess::AvatarAsset;
    }
    
    if (!isObject(ToolViewSet))
        new SimSet(ToolViewSet);
      
    %this.lastToolView = "";
    %this.lastToolBar = "";
    %this.lastCommonToolBar = "";
    %this.lastHintAndTips = "";
}

function EditorShellGui::destroy(%this)
{
    %this.isInitialized = false;

    %this.lastRefreshState = -1;

    if (isObject(ToolViewSet))
        ToolViewSet.delete();

    %this.lastToolView = "";
    %this.lastToolBar = "";
    %this.lastCommonToolBar = "";
    %this.lastHintAndTips = "";
}

function EditorShellGui::refresh(%this)
{
    // Deprecated until it has a use
    
    //switch (%this.lastRefreshState)
    //{
        //case $BaseEditorRefreshState::ToolView:

        //// This is where I'm going to adjust the positions of the views
        //// based on the actions that have taken place, mainly remove and set
        //// The following code was from a previous approach, mainly for reference

        ////// Need to offset the other views to fit properly
        ////%lastIndex = ToolViewSet.getCount() - 1;
        ////
        ////%lastToolView = ToolViewSet.getObject(%lastIndex);
        ////
        ////// Get the left border of the last tool view added
        ////%toolViewXPosition = getWord(%lastToolView.getPosition(), 0);
        ////
        ////// Store the width of the last tool view added
        ////%toolViewWidth = getWord(%lastToolView.getExtent(), 0);
        ////
        ////// Retain the Y position of the tool content shell
        ////%toolContentYPosition = getWord(ToolContentShell.getPosition(), 1);
        ////
        ////%toolContentXPosition = %toolViewWidth + %toolViewXPosition;
        ////
        ////// Nudge the tool content shell to maintain a consistent spacing
        ////ToolContentShell.setPosition(%toolContentXPosition + 17, %toolContentYPosition);

        //case $BaseEditorRefreshState::ToolBar:
            //echo("@@@ Refreshed editor Tool Bar");

        //case $BaseEditorRefreshState::CommonToolBar:
            //echo("@@@ Refreshed editor Common Tool Bar");

        //case $BaseEditorRefreshState::HintsAndTips:
            //echo("@@@ Refreshed editor Hints and tips");

        //case $BaseEditorRefreshState::All:
            //echo("@@@ Refreshed entire editor");
    //}
}

/// <summary>
/// Creates a new gui tool view and adds it to the editor display area.  The available sizes are:
/// smallest    - 100px wide
/// small       - 127px wide
/// smallMedium - 170px wide
/// medium      - 266px wide
/// large       - 710px wide
/// largest     - 806px wide
/// ""          - A blank string sizes the new view to the size of %guiControl
/// </summary>
/// <param name="guiControl">The control to display in the view</param>
/// <param name="size">The size of the container - optional</param>
function EditorShellGui::addView(%this, %guiControl, %size)
{
    %xPosition = 0;
   
    // If previous tool views were added
    if (ToolViewSet.getCount() > 0)
    {
        %lastIndex = ToolViewSet.getCount() - 1;

        %lastToolView = ToolViewSet.getObject(%lastIndex);

        // The x location of the new tool view will be 20 pixels to the right
        // of the last control that was added
        %lastToolXPosition = getWord(%lastToolView.getPosition(), 0);
        %lastToolWidth = getWord(%lastToolView.getExtent(), 0);
        %xPosition = %lastToolXPosition + %lastToolWidth + $BaseEditorSpacing;
    }
    else
        %xPosition = 18;
   
    %position = %xPosition SPC "110";

    // smallest
    // small    
    // medium
    // large
    // largest
    if (%size $="")
    {
        %newToolBorderCtrl = new GuiControl()
        {
            Profile = "GuiTransparentProfile";
            Extent = %guiControl.Extent;
            Position = %position;
            HorizSizing = "relative";
            VertSizing = "relative";
            MinExtent = "8 2";
            canSave = false;
            isContainer = "0";
        };
        %newToolBorderCtrl.add(%guiControl);
    }
    else
    {
        %borderImage = "{EditorShell}:" @ %size @ "_ContainerImageMap";
    
        %newToolBorderCtrl = new GuiSpriteCtrl()
        {
            canSaveDynamicFields = "0";
            isContainer = "0";
            Profile = "GuiTransparentProfile";
            HorizSizing = "relative";
            VertSizing = "relative";
            Position = %position;
            Extent = "1 1";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
            wrap = "0";
            useSourceRect = "0";
            sourceRect = "0 0 0 0";
            Image = %borderImage;
        };
        
        %imageAsset = AssetDatabase.acquireAsset(%borderImage);
        
        %height = %imageAsset.getImageHeight();
        %width = %imageAsset.getImageWidth();
        
        AssetDatabase.releaseAsset(%borderImage);
        
        %newToolBorderCtrl.extent = %width SPC %height;
        %newToolBorderCtrl.add(%guiControl);
    }
    
    EditorShellGui.add(%newToolBorderCtrl);

    ToolViewSet.add(%newToolBorderCtrl);
   
    %this.lastRefreshState = $BaseEditorRefreshState::ToolView;

    %this.refresh();
}

function EditorShellGui::setView(%this, %oldView, %newView)
{
    %toolViewContainer = "";
    %oldToolContent = "";

    // Get the base container
    for (%i = 0; %i < ToolViewSet.getCount(); %i++)
    {
        %container = ToolViewSet.getObject(%i);

        %contents = %container.getObject(0);

        if (%contents $= %oldView)
        {
            %oldToolContent = %contents;
            %toolViewContainer = %container;
            break;
        }
    }
   
    // If it doesn't exist, a bad index was used
    if (!isObject(%toolViewContainer))
    {
        error("Could not find the old view: " @ %oldView);
        return;
    }
   
    // Remove the tool content from the container
    %toolViewContainer.remove(%oldToolContent);

    // Get the size of the new view contents
    %borderWidth = getWord(%newView.getExtent(), 0) + 10;
    %borderHeight = $EditorShellGui::ViewHeight;

    // Resize the container appropriately
    //%toolViewContainer.setExtent(%borderWidth, %borderHeight);

    // Add the new tool contents
    %toolViewContainer.add(%newView);
   
   // Offset the new contents slightly
   //%newView.setPosition(5, 5);
   
   // Refresh
   %this.lastRefreshState = $BaseEditorRefreshState::ToolView;
   %this.refresh();
}

function EditorShellGui::removeView(%this, %oldView)
{
    %toolViewContainer = "";
    %toolContent = "";

    // Get the base container
    for (%i = 0; %i < ToolViewSet.getCount(); %i++)
    {
        %container = ToolViewSet.getObject(%i);

        %contents = %toolViewContainer.getObject(0);

        if (%contents $= %oldView)
        {
            %toolContent = %contents;
            %toolViewContainer = %container;
            break;
        }
    }

    // If it doesn't exist, a bad index was used
    if (!isObject(%toolViewContainer))
    {
        error("Could not find view to remove: " @ %oldView);
        return;
    }

    // Remove the tool content from the container
    %toolViewContainer.remove(%toolContent);

    // Remove the container from the ToolViewSet
    ToolViewSet.remove(%toolViewContainer);

    // Delete the tool view container, since we generate these on the fly
    %toolViewContainer.delete();

    %this.lastRefreshState = $BaseEditorRefreshState::ToolView;
    %this.refresh();
}

function EditorShellGui::clearViews(%this)
{
    // Go through the list of tool views until there are none left
    while (ToolViewSet.getCount() > 0)
    {
        // Get the tool view container, which is the control with a black
        // border and white filler
        %toolViewContainer = ToolViewSet.getObject(%i);

        // Get the template tool content, which should always be the
        // first entry
        %toolContent = %toolViewContainer.getObject(0);

        %exists = isObject(%toolContent);

        // Remove the tool content from the container
        %toolViewContainer.remove(%toolContent);

        %exists = isObject(%toolContent);

        // Remove the container from the ToolViewSet
        ToolViewSet.remove(%toolViewContainer);

        // Delete the tool view container, since we generate these on the fly
        %toolViewContainer.delete();
    }
}

function EditorShellGui::setToolBar(%this, %toolBar)
{
    if (isObject(%this.lastToolBar))
        ESMainToolbarGui.remove(%this.lastToolBar);

    if (!isObject(%toolBar))
    {
        %this.lastToolBar = "";
        return;
    }
    
    %this.lastToolBar = %toolBar;
    
    %toolBarExtent = %this.lastToolBar.getExtent();
    
    ESMainToolbarGui.setExtent(getWord(%toolBarExtent, 0), getWord(%toolBarExtent, 1));
    
    ESMainToolbarGui.addGuiControl(%toolBar);

    %this.lastRefreshState = $BaseEditorRefreshState::ToolBar;

    %this.refresh();
}

function EditorShellGui::getToolBar(%this)
{
    return %this.lastToolBar;
}

function EditorShellGui::setCommonToolBar(%this, %commonToolBar)
{
    Esg_3SSLogo.setVisible(false);
    
    if (isObject(%this.lastCommonToolBar))
        ESCommonToolbarGui.remove(%this.lastCommonToolBar);

    if (!isObject(%commonToolBar))
    {
        %this.lastCommonToolBar = "";
        Esg_3SSLogo.setVisible(true);
        return;
    }
        
    %this.lastCommonToolBar = %commonToolBar;

    %toolBarExtent = %this.lastCommonToolBar.getExtent();
    
    ESCommonToolbarGui.setExtent(getWord(%toolBarExtent, 0), getWord(%toolBarExtent, 1));
    
    ESCommonToolbarGui.addGuiControl(%commonToolBar);

    %this.lastRefreshState = $BaseEditorRefreshState::CommonToolBar;
    %this.refresh();
}

function EditorShellGui::getCommonToolBar(%this)
{
    return %this.lastCommonToolBar;
}

function EditorShellGui::setHintsAndTips(%this, %hintsAndTips)
{   
    HintsAndTipsGui.setText(%hintsAndTips);

    %this.lastRefreshState = $BaseEditorRefreshState::HintsAndTips;

    %this.refresh();
}

function EditorShellGui::setToolContent(%this, %toolContent)
{
    if (isObject(%this.lastToolContent))
        ToolContentShell.remove(%this.lastToolContent);

    %this.lastToolContent = %toolContent;

    ToolContentShell.addGuiControl(%toolContent);

    %this.lastRefreshState = $BaseEditorRefreshState::ToolContent;
    %this.refresh();
}

function EditorShellGui::getToolContent(%this)
{
    return %this.lastToolContent;
}

function ESPreviousTipButton::onClick(%this)
{   
}

function ESNextTipButton::onClick(%this)
{
}

function ESMoreHelpButton::onClick(%this)
{
}