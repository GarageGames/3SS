//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Utility methods for VerticalScrollCtrl
//-----------------------------------------------------------------------------

/// <summary> These adjust the scroll indicator arrow position relative to the 
/// target control.
/// </summary>
$VerticalScrollCtrl::ScrollIndicatorYOffset = 2;
$VerticalScrollCtrl::ScrollIndicatorXOffset = -3;
/// <summary>
/// This handles the scroll up button mouse down event.  It also sets the button
/// image on the underlying button object.
/// </summary>
function Vscg_UpBTN::onMouseDown(%this)
{
    cancel(%this.scrollScheduleID);
   
    %this.scrollScheduleID = -1;
    %this.scrollUp();
    %this.button.setNormalImage(%this.button.DownImage);
}

/// <summary>
/// This handles the scroll up button mouse enter event.  It also sets the button
/// image on the underlying button object.
/// </summary>
function Vscg_UpBTN::onMouseEnter(%this)
{
   %this.button.setNormalImage(%this.button.HoverImage);
}

/// <summary>
/// This handles the scroll up button mouse leave event.  It also sets the button
/// image on the underlying button object.
/// </summary>
function Vscg_UpBTN::onMouseLeave(%this)
{
   %this.button.setNormalImage(%this.button.normalImageCache);
}

/// <summary>
/// This handles scrolling the contents of the container when the scroll up button is
/// clicked and/or held down.
/// </summary>
function Vscg_UpBTN::scrollUp(%this)
{
    %this.container.scrollPosition = %this.container.scrollCtrl.getScrollPositionY();
    %this.container.scrollCount = mRound(%this.container.scrollPosition / (%this.container.buttonHeight + %this.container.contentPane.rowHeight));
    %this.container.scrollCount --;
    if (%this.container.scrollCount < 0)
    {
        %this.container.scrollCount = 0;
        %this.container.scrollCtrl.setScrollPosition(0, 0);
        cancel(%this.scrollScheduleID);
        %this.scrollUpScheduleID = -1;
        return;
    }
    %this.container.scrollPosition -= (%this.container.buttonHeight + %this.container.contentPane.rowSpacing) * %this.container.scrollSpeed;
    if (%this.container.scrollPosition < 0)
        %this.container.scrollPosition = 0;
    %this.container.scrollCtrl.setScrollPosition(0, %this.container.scrollPosition);

   %this.scrollScheduleID = %this.schedule(%this.container.scrollRepeat, scrollUp);
}

/// <summary>
/// This handles the scroll up button mouse up event.  It also sets the button
/// image on the underlying button object.
/// </summary>
function Vscg_UpBTN::onMouseUp(%this, %modifier, %mousePoint, %mouseClickCount)
{
    %this.button.setNormalImage(%this.button.HoverImage);
    if (%this.scrollScheduleID == -1)
        return;

    cancel(%this.scrollScheduleID);
   
    %this.scrollScheduleID = -1;
}

/// <summary>
/// This handles the scroll down button mouse down event.  It also sets the button
/// image on the underlying button object.
/// </summary>
function Vscg_DownBTN::onMouseDown(%this)
{
    cancel(%this.scrollScheduleID);
   
    %this.scrollScheduleID = -1;
    %this.scrollDown();
    %this.button.setNormalImage(%this.button.DownImage);
}

/// <summary>
/// This handles the scroll down button mouse enter event.  It also sets the button
/// image on the underlying button object.
/// </summary>
function Vscg_DownBTN::onMouseEnter(%this)
{
   %this.button.setNormalImage(%this.button.HoverImage);
}

/// <summary>
/// This handles the scroll down button mouse leave event.  It also sets the button
/// image on the underlying button object.
/// </summary>
function Vscg_DownBTN::onMouseLeave(%this)
{
   %this.button.setNormalImage(%this.button.normalImageCache);
}

/// <summary>
/// This handles scrolling the contents of the container down when the down button is 
/// clicked and/or held down.
/// </summary>
function Vscg_DownBTN::scrollDown(%this)
{
    %itemCount = %this.container.itemCount;
    %paneSize = %this.container.paneSize;
    %spacing = %this.container.contentPane.rowSpacing;
    %buttonHeight = %this.container.contentPane.rowSize;
    %maxScroll = (%buttonHeight * %itemCount) + (%itemCount * %spacing) - %paneSize;
    %this.container.scrollPosition = %this.container.scrollCtrl.getScrollPositionY();
    %this.container.scrollCount = mRound(%this.container.scrollPosition / (%this.container.buttonHeight + %this.container.contentPane.rowHeight));
    %this.container.scrollCount ++;
    if (%this.container.scrollCount > %this.container.itemCount)
    {
        %this.container.scrollCount = %this.container.itemCount;
        %this.container.scrollCtrl.setScrollPosition(0, %maxScroll);
        cancel(%this.scrollScheduleID);
        %this.scrollScheduleID = -1;
        return;
    }
    %this.container.scrollPosition += (%buttonHeight + %spacing) * %this.container.scrollSpeed;
    if (%this.container.scrollPosition > %maxScroll)
        %this.container.scrollPosition = %maxScroll;
    %this.container.scrollCtrl.setScrollPosition(0, %this.container.scrollPosition);

   %this.scrollScheduleID = %this.schedule(%this.container.scrollRepeat, scrollDown);
}

/// <summary>
/// This handles the scroll down button mouse up event.  It also sets the button
/// image on the underlying button object.
/// </summary>
function Vscg_DownBTN::onMouseUp(%this, %modifier, %mousePoint, %mouseClickCount)
{
   %this.button.setNormalImage(%this.button.HoverImage);
    if (%this.scrollScheduleID == -1)
        return;

    cancel(%this.scrollScheduleID);
   
    %this.scrollScheduleID = -1;
}

/// <summary>
/// This clears the control's contents
/// </summary>
function VerticalScrollCtrl::clear(%this)
{
    if (!isObject(%this.deletionSet))
        %this.deletionSet = new SimSet();

    for(%i = 0; %i < %this.deletionSet.getCount(); %i++)
    {
        %obj = %this.deletionSet.getObject(0);
        %obj.delete();
    }
    
    for(%i = 0; %i < %this.contentPane.getCount(); %i++)
    {
        %obj = %this.contentPane.getObject(%i);
        %this.deletionSet.add(%obj);        
    }
    %this.contentPane.clear();
    %this.buttonHeight = 0;
}

/// <summary>
/// This scrolls the control to the desired button
/// </summary>
/// <param name="index">The index of the desired button</param>
function VerticalScrollCtrl::scrollToButton(%this, %index)
{
    if (%index < 0)
        %index = 0;

    if (%index > %this.contentPane.getCount())
        %index = %this.contentPane.getCount();
    %itemCount = %this.contentPane.getCount();
    %spacing = %this.contentPane.rowSpacing;
    %maxScroll = (%this.buttonHeight * %itemCount) + (%itemCount * %spacing) - %this.buttonHeight;

    %position = %index * (%spacing + %this.buttonHeight);
    if (%position > %maxScroll)
        %position = %maxScroll;
    %this.scrollCtrl.setScrollPosition(0, %position);
}

/// <summary>
/// This gets the vertical position of the desired button
/// </summary>
/// <param name="index">The index of the desired button</param>
/// <return>Returns the vertical position of the button</return>
function VerticalScrollCtrl::getButtonPosition(%this, %index)
{
    if (%index < 0)
        %index = 0;

    if (%index > %this.contentPane.getCount())
        %index = %this.contentPane.getCount();
    %itemCount = %this.contentPane.getCount();
    %spacing = %this.contentPane.rowSpacing;
    %maxScroll = (%this.buttonHeight * %itemCount) + (%itemCount * %spacing) - %this.buttonHeight;

    %position = %index * (%spacing + %this.buttonHeight);
    if (%position > %maxScroll)
        %position = %maxScroll;
    return %position;
}

/// <summary>
/// This returns the button at the desired index.
/// </summary>
/// <param name="index">The index of the desired button</param>
/// <return>Returns the button at the specified index</return>
function VerticalScrollCtrl::getButton(%this, %index)
{
    if ( %index < 0 )
        %index = 0;

    %count = %this.contentPane.getCount();

    if (%index > %count )
        %index = %count;

    return %this.contentPane.getObject(%index);
}

/// <summary>
/// This function adds a header to the container.  The header can be any collection of gui controls
/// contained within a parent control.
/// </summary>
/// <param name="guiControl">The GUI control to create the header from</param>
function VerticalScrollCtrl::addHeader(%this, %guiControl)
{
    // take the provided gui control and place it on a 
    // VsDynamicButton in VsContentPane and assign it an index.
    if (%this.headerWidth $= "")
        %this.resizeCtrl = true;
    %this.headerCtrl = %guiControl;

    %this.headerWidth = getWord(%guiControl.extent, 0);
    %this.headerHeight = getWord(%guiControl.extent, 1);

    %this.addGuiControl(%guiControl);

    if (%this.resizeCtrl)
    {
        %this.resizeCtrl = false;
        %this.resizeContainer();
    }
}

/// <summary>
/// This function adds a footer to the container.  The footer can be any collection of gui controls
/// contained within a parent control.
/// </summary>
/// <param name="guiControl">The GUI control to create the footer from</param>
function VerticalScrollCtrl::addFooter(%this, %guiControl)
{
    // take the provided gui control and place it on a 
    // VsDynamicButton in VsContentPane and assign it an index.
    if (%this.footerWidth $= "")
        %this.resizeCtrl = true;
    %this.footerCtrl = %guiControl;

    %this.footerWidth = getWord(%guiControl.extent, 0);
    %this.footerHeight = getWord(%guiControl.extent, 1);

    %this.addGuiControl(%guiControl);

    if (%this.resizeCtrl)
    {
        %this.resizeCtrl = false;
        %this.resizeContainer();
    }

    %parent = %this.getParent();
    %parentHeight = %parent.Extent.y - 13;
    %this.footerCtrl.setPosition(0, %parentHeight - %this.footerHeight - 18);
}

/// <summary>
/// This function adds a button to the container with a target object and method to 
/// call, along with other data potentially.  The container will be sized to the width 
/// of the assigned button's parent control unless the container has been assigned a
/// header.
/// </summary>
/// <param name="guiControl">The GUI control to create the button from</param>
/// <param name="object">The object that contains the method to be called</param>
/// <param name="handler">The method on %object to call to handle the button click</param>
/// <param name="data">Additional information that needs to be passed on to %handler</param>
function VerticalScrollCtrl::addButton(%this, %guiControl, %object, %handler, %data)
{
    // take the provided gui control and place it on a 
    // VsDynamicButton in VsContentPane and assign it an index.
    if (%this.buttonWidth $= "")
        %this.resizeCtrl = true;

    %this.buttonWidth = %guiControl.Extent.x;
    %this.buttonHeight = %guiControl.Extent.y;

    if ( %this.buttonProfile $= "" && %guiControl.Profile !$= "GuiTransparentProfile" )
    {
        %this.buttonProfile = %guiControl.Profile;
        %this.useProfile = true;
    }
    else
        %this.useProfile = false;

    if ( %this.buttonProfile $= "" )
        %this.buttonProfile = "GuiTransparentProfile";

    %baseProfile = (%this.useProfile == true ? %this.buttonProfile : %guiControl.Profile);

    %extent = %guiControl.Extent;
	%button = new GuiControl()
	{
		canSaveDynamicFields="0";
		isContainer="0";
		Profile=%baseProfile;
        HorizSizing="left";
        VertSizing="top";
		Position="0 0";
		Extent=%extent;
		MinExtent="8 2";
		canSave="1";
		Visible="1";
		hovertime="1000";
            index = %this.contentPane.getCount();
    };
	%clickEvent = new GuiMouseEventCtrl()
	{
	    class="VsDynamicButton";
		canSaveDynamicFields="0";
		isContainer="0";
		Profile="GuiTransparentProfile";
        HorizSizing="left";
        VertSizing="top";
		Position="0 0";
		Extent=%extent;
		MinExtent="8 2";
		canSave="1";
		Visible="1";
		hovertime="1000";
        toolTipProfile=%guiControl.toolTipProfile;
        toolTip=%guiControl.toolTip;
		groupNum="-1";
            index = %this.contentPane.getCount();
            object = %object;
            handler = %handler;
            data = %data;
            button = %button;
            container = %this;
	};

	// If the incoming gui control set has any buttons in it, assume that we want
	// to be able to click them.  Sort through the control set and hold buttons out
	// until the mouse event control has been added, then add the other child buttons.
	// Additionally, hold out pretty much any control that we might want to be able to
	// interact with, such as edit boxes or dropdown menus.
	%itemCount = %guiControl.getCount();
	%k = 0;
	while (%itemCount > 0)
	{
	    %temp = %guiControl.getObject(0);
	    if (isObject(%temp))
	    {
            %type = %temp.getClassName();
            if (%type $= "guiButtonBaseCtrl" || %type $= "guiBitmapButtonCtrl" || 
            %type $= "guiBitmapButtonTextCtrl" || %type $= "guiBorderButtonCtrl" ||
            %type $= "guiPopUpMenuCtrl" || %type $= "guiPopUpMenuCtrlEx" ||
            %type $= "guiTextEditCtrl" || %type $= "guiTextEditSliderCtrl" ||
            %type $= "guiSliderCtrl" || %type $= "guiRadioCtrl" || %type $= "guiCheckBoxCtrl" ||
            %type $= "guiIconButtonCtrl" || %type $= "guiImageButtonCtrl")
            {
                %buttons[%k] =  %temp;
                %guiControl.remove(%temp);
                %k++;
            }
            else
                %button.addGuiControl(%temp);
	    }
	    %itemCount=%guiControl.getCount();
	}
    %button.addGuiControl(%clickEvent);
	if (%k > 0)
	{
	    for (%i = 0; %i < %k; %i++)
	    {
	        %button.addGuiControl(%buttons[%i]);
	    }
	}

    %this.contentPane.add(%button);

    if (%this.batch)
        return;

    if (%this.resizeCtrl)
    {
        %this.resizeCtrl = false;
        %this.resizeContainer();
    }

    %this.resizeContentPane();

    if (%this.contentPane.Extent.y > %this.scrollCtrl.Extent.y)
    {
        %this.upButton.button.setActive(true);
        %this.downButton.button.setActive(true);
    }
    else
    {
        %this.upButton.button.setActive(false);
        %this.downButton.button.setActive(false);
    }
}

function VerticalScrollCtrl::onAdd(%this)
{
    if (isObject(%this.getParent))
    {
        %this.resizeContainer();
        %this.resizeContentPane();
        %this.resizeCtrl = false;
    }
}

function VerticalScrollCtrl::onSleep(%this)
{
    if (isObject(%this.indicatorArrow))
    {
        %this.indicatorArrow.delete();
    }
}

function VerticalScrollCtrl::onRemove(%this)
{
    if (isObject(%this.indicatorArrow))
    {
        %this.indicatorArrow.delete();
    }
}

/// <summary>
/// This function sets the container to add buttons without resizing the control.
/// </summary>
/// <param name="flag">The desired container batch add state</param>
function VerticalScrollCtrl::toggleBatch(%this, %flag)
{
    %this.batch = %flag;
    if ( %this.scrollCallbacks && %flag )
        %this.scrollCtrl.setUseScrollEvents(false);
    if ( %this.scrollCallbacks && !%flag )
        %this.scrollCtrl.setUseScrollEvents(%this.scrollCallbacks);

    if (!%flag)
    {
        %this.resizeContentPane();

        if (%this.contentPane.Extent.y > %this.scrollCtrl.Extent.y)
        {
            %this.upButton.button.setActive(true);
            %this.downButton.button.setActive(true);
        }
        else
        {
            %this.upButton.button.setActive(false);
            %this.downButton.button.setActive(false);
        }
    }
}

function VerticalScrollCtrl::resizeContainer(%this)
{
    %parent = %this.getParent();
    %parentWidth = %parent.Extent.x - 5;
    %parentHeight = %parent.Extent.y - 14;
    %buttonWidth = %this.buttonWidth;
    %buttonHeight = %this.buttonHeight;

    %this.resize(0, 0, %parentWidth, %parentHeight);

    if (isObject(%this.headerCtrl))
        %this.headerCtrl.setPosition(0, 18);
    
    if (isObject(%this.footerCtrl))
        %this.footerCtrl.setPosition(0, %parentHeight - %this.footerHeight - 18);

    %scrollContainerWidth = %this.Extent.x - 32;
    %scrollContainerPosX = %this.Position.x + 18;
    %scrollContainerHeight = %this.Extent.y - %this.headerHeight - %this.footerHeight - 32;
    %scrollContainerPosY = %this.Position.y + 18 + %this.headerHeight;

    %this.scrollContainer.resize(%scrollContainerPosX, %scrollContainerPosY, %scrollContainerWidth, %scrollContainerHeight);

    %this.upButton.resize(4, 6, %scrollContainerWidth - 8, %this.upButton.Extent.y);
    %btnPosX = (%this.upButton.Extent.x / 2) - (%this.upButton.button.Extent.x / 2) + 4;
    %this.upButton.button.setPosition(%btnPosX, %this.upButton.Position.y);

    %this.downButton.resize(4, %scrollContainerHeight - %this.downButton.Extent.y - 3, %scrollContainerWidth - 8, %this.downButton.Extent.y);
    %btnPosX = (%this.downButton.Extent.x / 2) - (%this.downButton.button.Extent.x / 2) + 4;
    %this.downButton.button.setPosition(%btnPosX, %this.downButton.Position.y);

    %scrollPosY = %this.upButton.Position.y + %this.upButton.Extent.y + 2;
    %scrollLength = %this.scrollContainer.Extent.y - (%this.upButton.Extent.y * 2) - 12;

    %this.scrollCtrl.resize(5, %scrollPosY, %scrollContainerWidth - 10, %scrollLength);
}

function VerticalScrollCtrl::resizeContentPane(%this)
{
    %count = %this.contentPane.getCount();
    %spacing = %this.contentPane.rowSpacing;
    %height = (%count * %this.buttonHeight) + (%count * %spacing);

    %position = %this.contentPane.Position;
    %this.contentPane.resize(0, 0, %this.scrollCtrl.Extent.x, %height);
    %this.contentPane.rowSize = %this.buttonHeight;
    %this.contentPane.colSize = %this.scrollCtrl.Extent.x;

    %this.contentPane.refresh();
    %this.paneSize = getWord(%this.scrollCtrl.Extent, 1);
    %this.itemCount = %this.contentPane.getCount();    

    %this.scrollCtrl.scrollToTop();
}

/// <summary>
/// This sets the spacing between buttons in the container
/// </summary>
/// <param name="spacing">The desired space in pixels between buttons</param>
function VerticalScrollCtrl::setSpacing(%this, %spacing)
{
    %this.contentPane.rowSpacing = %spacing;
}

/// <summary>
/// This sets a profile to use for highlighting the currently selected button.
/// </summary>
/// <param name="profile">The profile to use for the selected button.</param>
function VerticalScrollCtrl::setHighlightProfile(%this, %profile)
{
    %this.highlightProfile = %profile;
}

/// <summary>
/// This sets an asset ID for the selected item scroll position indicator.
/// </summary>
/// <param name="profile">The asset ID to use for the indicator arrow.</param>
function VerticalScrollCtrl::setIndicatorImage(%this, %assetID)
{
    %this.indicatorImage = %assetID;
}

/// <summary>
/// This sets a base profile for all buttons.  If this is not set, the first button
/// added to the container sets the base button profile.
/// </summary>
/// <param name="profile">The profile to use for the selected button</param>
function VerticalScrollCtrl::setNormalProfile(%this, %profile)
{
    %this.buttonProfile = %profile;
}

/// <summary>
/// This gets the current container button spacing
/// </summary>
/// <return>Returns the current button spacing</return>
function VerticalScrollCtrl::getSpacing(%this)
{
    if (%this.contentPane.rowSpacing !$= "")
        return %this.contentPane.rowSpacing;
    else
        return 0;
}

/// <summary>
/// This "clicks" the desired contained button.
/// </summary>
/// <param name="index">The index of the desired button</param>
function VerticalScrollCtrl::setSelected(%this, %index)
{
    if (%index < 0 || %index > %this.contentPane.getCount())
        return;

    %control = %this.contentPane.getObject(%index);
    if (isObject(%control))
    {
        for (%i = 0; %i < %control.getCount(); %i++)
        {
            %obj = %control.getObject(%i);
            %type = %obj.getClassName();
            if (%type $= "GuiMouseEventCtrl")
            {
                %obj.onMouseUp();
                break;
            }
        }
    }
}

/// <summary>
/// This gets the number of buttons in the container.
/// </summary>
/// <return>Returns the number of buttons in the content pane.</return>
function VerticalScrollCtrl::getCount(%this)
{
    return %this.contentPane.getCount();
}

/// <summary>
/// Sets the distance that the container scrolls per repeat cycle that the scroll button is held down.
/// The value is a fraction of the button height, so 0.5 is half button height, 2 is twice button height.
/// The default is 0.3.
/// </summary>
/// <param name="multiplier">The fraction of the current button height to scroll.</param>
function VerticalScrollCtrl::setScrollSpeed(%this, %multiplier)
{
    %this.scrollSpeed = %multiplier;
}

/// <summary>
/// Sets the repeat rate for continuous scrolling in milliseconds.  The default is 100 ms.
/// </summary>
/// <param name="rate">Milliseconds between scroll actions.</param>
function VerticalScrollCtrl::setScrollRepeat(%this, %rate)
{
    %this.scrollRepeat = %rate;
}

/// <summary>
/// Enables or disables onScroll callbacks.
/// </summary>
/// <param name="%flag">Set to true to enable onScroll callbacks, false to disable them.</param>
function VerticalScrollCtrl::setScrollCallbacks(%this, %flag)
{
    %this.scrollCallbacks = %flag;
    %this.scrollCtrl.setUseScrollEvents(%flag);
}

/// <summary>
/// This is to set the handler for scrollCtrl's onScroll callback.  This is to allow any given instance
/// of a VerticalScrollContainer to have its own way of handling this callback.
/// </summary>
/// <param name="funcName">The method that the onScroll() callback will pass control to.</param>
function VerticalScrollCtrl::setScrollHandler(%this, %funcName)
{
    %this.scrollCtrl.scrollHandler = %funcName;
}

/// <summary>
/// This function manually sets the item indicator arrow to point to the item at the 
/// specified index.
/// </summary>
/// <param name="index">The index of the item to point to.</param>
function VerticalScrollCtrl::setIndicatorToButton(%this, %index)
{
    %this.selectedButton = %this.getButton(%index);
    %this.updateIndicatorPosition();
}

/// <summary>
/// This function handles updating the selected item indicator arrow position when the scroll
/// control state changes.
/// </summary>
/// <param name="childPos">Position information passed from the onScroll() callback.</param>
/// <param name="childRelPos">Position information passed from the onScroll() callback.</param>
function VerticalScrollCtrl::updateIndicatorPosition(%this, %childPos, %childRelPos)
{
    if ( %childPos $= "" )
        %childPos = %this.scrollCtrl.childPos;
    if ( %childRelPos $= "" )
        %childRelPos = %this.scrollCtrl.childRelPos;

    if ( !isObject ( %this.indicatorArrow ) && %this.indicatorImage !$= "" )
    {
        %tempAsset = AssetDatabase.acquireAsset(%this.indicatorImage);
        %this.indicatorArrow = new GuiSpriteCtrl()
        {
            canSaveDynamicFields="0";
            isContainer="0";
            Profile="GuiTransparentProfile";
            HorizSizing="left";
            VertSizing="top";
            Position="0 0";
            Extent=%tempAsset.getImageSize();
            MinExtent="8 2";
            canSave = "0";
            Visible="1";
            hovertime="1000";
            wrap="0";
            useSourceRect="0";
            sourceRect="0 0 0 0";
            Image=%this.indicatorImage;
        };
        AssetDatabase.releaseAsset(%this.indicatorImage);
        EditorShellGui.add(%this.indicatorArrow);
    }
    EditorShellGui.pushToBack(%this.indicatorArrow);
    %basePos = %this.getParent().Position;
    %containerPos = %this.Position.x + %basePos.x SPC %this.Position.y + %basePos.y;
    %scrollContainerPos = %containerPos.x + %this.scrollContainer.Position.x SPC %containerPos.y + %this.scrollContainer.Position.y;
    %scrollOffset = %this.scrollContainer.Extent.x + %scrollContainerPos.x + $VerticalScrollCtrl::ScrollIndicatorXOffset;

    if ( isObject(%this.selectedButton) )
        %button = %this.getButton(%this.selectedButton.index);
    else
        %button = %this.getButton(0);
        
    %buttonOffset = %button.Position.y + (%button.Extent.y / 2) + %childPos.y + $VerticalScrollCtrl::ScrollIndicatorYOffset;
    %yPos = %scrollContainerPos.y + %buttonOffset;
    if (%yPos < %scrollContainerPos.y + $VerticalScrollCtrl::ScrollIndicatorYOffset)
        %yPos = %scrollContainerPos.y + $VerticalScrollCtrl::ScrollIndicatorYOffset;
    if (%yPos > (%scrollContainerPos.y + %this.scrollCtrl.Extent.y) + $VerticalScrollCtrl::ScrollIndicatorYOffset)
        %yPos = %scrollContainerPos.y + %this.scrollCtrl.Extent.y + $VerticalScrollCtrl::ScrollIndicatorYOffset;
    %this.indicatorArrow.setPosition(%scrollOffset, %yPos);
}

/// <summary>
/// This is the container's scroll control onScroll callback function.  If onScroll events are enabled 
/// with setScrollCallbacks, this callback will handle all changes to %this.scrollCtrl scroll position.
/// </summary>
/// <param name="childStart">The position of the scroll control's 'contained' control before scrolling.</param>
/// <param name="childRelStart">The relative position of the scroll control's 'contained' control before scrolling.</param>
/// <param name="childPos">The position of the scroll control's 'contained' control after scrolling.</param>
/// <param name="childRelPos">The relative position of the scroll control's 'contained' control after scrolling.</param>
function VsScrollCtrl::onScroll(%this, %childStart, %childRelStart, %childPos, %childRelPos)
{
    %this.childPos = %childPos;
    %this.childRelPos = %childRelPos;
    // take care of updating the selection indicator if we have one.
    if (%this.container.indicatorImage !$= "")
        %this.container.updateIndicatorPosition(%childPos, %childRelPos);

    if ( %this.scrollHandler !$= "" )
        eval(%this.scrollHandler@"(\""@%childStart@"\",\""@%childRelStart@"\",\""@%childPos@"\",\""@%childRelPos@"\");");
}

/// <summary>
/// This handles button clicks for contained buttons.  It checks to ensure that 
/// the object has the assigned method, then calls that method with the assigned
/// data.
/// </summary>
function VsDynamicButton::onMouseUp(%this)
{
    if ( %this.container.selectedButton !$= "" && isObject(%this.container.selectedButton) )
        %this.container.selectedButton.setProfile(%this.container.buttonProfile);
    %this.container.selectedButton = %this.button;
    %this.button.setProfile(%this.container.highlightProfile);
    %this.container.updateIndicatorPosition();
    %object = %this.object;
    if (%object.isMethod(%this.handler))
        %object.call(%this.handler, %this.data);
}

/// <summary>
/// This "factory" function creates a vertical scroll container and returns a 
/// reference to it.  Use the returned reference to work with the container.
/// </summary>
/// <param name="profile">Profile for the scroll container.  Default is GuiSunkenContainerProfile</param>
/// <return>A new VerticalScrollCtrl container object</return>
function createVerticalScrollContainer(%profile)
{
    %container = new GuiControl()
    {
        class="VerticalScrollCtrl";
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
		HorizSizing="left";
		VertSizing="top";
        Position="0 0";
        Extent="160 558";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
    };

    %scrollContainer = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile=(%profile !$= "" ? %profile : "GuiSunkenContainerProfile");
		HorizSizing="left";
		VertSizing="top";
        Position="0 0";
        Extent="160 558";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
    };
    %container.addGuiControl(%scrollContainer);
    %container.scrollContainer = %scrollContainer;

	%upButton = new GuiImageButtonCtrl()
	{
		canSaveDynamicFields="0";
		isContainer="0";
		Profile="GuiDefaultProfile";
		HorizSizing="left";
		VertSizing="top";
		Position="0 0";
		Extent="69 23";
		MinExtent="8 2";
		canSave="1";
		Visible="1";
		hovertime="1000";
		groupNum="-1";
		buttonType="PushButton";
		useMouseEvents="1";
		isLegacyVersion="0";
		NormalImage="{EditorAssets}:scrollUpImageMap";
		HoverImage="{EditorAssets}:scrollUp_hImageMap";
		DownImage="{EditorAssets}:scrollUp_dImageMap";
		InactiveImage="{EditorAssets}:scrollUp_iImageMap";
		    normalImageCache="{EditorAssets}:scrollUpImageMap";
	};
	%scrollContainer.addGuiControl(%upButton);

	%upClickEvent = new GuiMouseEventCtrl()
	{
		class="Vscg_UpBTN";
		canSaveDynamicFields="0";
		isContainer="0";
		Profile="GuiTransparentProfile";
		HorizSizing="left";
		VertSizing="top";
		Position="0 0";
		Extent="140 23";
		MinExtent="8 2";
		canSave="1";
		Visible="1";
		hovertime="1000";
		groupNum="-1";
            button = %upButton;
		    container = %container;
	};
	%scrollContainer.addGuiControl(%upClickEvent);
	%container.upButton = %upClickEvent;

	%downButton = new GuiImageButtonCtrl()
	{
		canSaveDynamicFields="0";
		isContainer="0";
		Profile="GuiDefaultProfile";
		HorizSizing="left";
		VertSizing="top";
		Position="0 505";
		Extent="69 23";
		MinExtent="8 2";
		canSave="1";
		Visible="1";
		hovertime="1000";
		groupNum="-1";
		buttonType="PushButton";
		useMouseEvents="1";
		isLegacyVersion="0";
		NormalImage="{EditorAssets}:scrollDownImageMap";
		HoverImage="{EditorAssets}:scrollDown_hImageMap";
		DownImage="{EditorAssets}:scrollDown_dImageMap";
		InactiveImage="{EditorAssets}:scrollDown_iImageMap";
		    normalImageCache="{EditorAssets}:scrollDownImageMap";
	};
	%scrollContainer.addGuiControl(%downButton);

	%downClickEvent = new GuiMouseEventCtrl()
	{
		class="Vscg_DownBTN";
		canSaveDynamicFields="0";
		isContainer="0";
		Profile="GuiTransparentProfile";
		HorizSizing="left";
		VertSizing="top";
		Position="0 505";
		Extent="140 23";
		MinExtent="8 2";
		canSave="1";
		Visible="1";
		hovertime="1000";
		groupNum="-1";
            button = %downButton;
		    container = %container;
	};
	%scrollContainer.addGuiControl(%downClickEvent);
	%container.downButton = %downClickEvent;

	%scroll = new GuiScrollCtrl()
	{
		class="VsScrollCtrl";
		canSaveDynamicFields="0";
		isContainer="1";
		Profile="GuiTransparentScrollProfile";
		HorizSizing="left";
		VertSizing="top";
		Position="0 53";
		Extent="140 444";
		MinExtent="8 2";
		canSave="1";
		Visible="1";
		hovertime="1000";
		willFirstRespond="1";
		hScrollBar="alwaysOff";
		vScrollBar="alwaysOff";
		constantThumbHeight="0";
		childMargin="0 0";
	};
	%scrollContainer.addGuiControl(%scroll);
	%container.scrollCtrl = %scroll;
	%scroll.container = %container;
	
    %contentPane = new GuiDynamicCtrlArrayControl()
    {
        class="VsContent";
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
		HorizSizing="left";
		VertSizing="top";
        Position="1 1";
        Extent="140 442";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
        colCount="1";
        colSize="1024";
        rowSize="106";
        rowSpacing="0";
        colSpacing="0";
    };
    %scroll.addGuiControl(%contentPane);
    %container.contentPane = %contentPane;

    %container.scrollSpeed = 0.3;
    %container.scrollRepeat = 100;
    %container.resizeContainer = true;

    return %container;
}