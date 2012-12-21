//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

if($platform $= "macos")
{
    new GuiCursor(DefaultCursor)
    {
        hotSpot = "4 4";
        renderOffset = "0 0";
        bitmapName = "^{EditorAssets}/data/images/macCursor";
    };
} 
else 
{
    new GuiCursor(DefaultCursor)
    {
        hotSpot = "1 1";
        renderOffset = "0 0";
        bitmapName = "^{EditorAssets}/data/images/macCursor";
    };
}

new GuiCursor(LeftRightCursor)
{
    hotSpot = "0.5 0";
    renderOffset = "0.5 0";
    bitmapName = "./Images/leftRight";
};

new GuiCursor(UpDownCursor)
{
    hotSpot = "1 1";
    renderOffset = "0 1";
    bitmapName = "./Images/upDown";
};

new GuiCursor(NWSECursor)
{
    hotSpot = "1 1";
    renderOffset = "0.5 0.5";
    bitmapName = "./Images/NWSE";
};

new GuiCursor(NESWCursor)
{
    hotSpot = "1 1";
    renderOffset = "0.5 0.5";
    bitmapName = "./Images/NESW";
};

new GuiCursor(MoveCursor)
{
    hotSpot = "1 1";
    renderOffset = "0.5 0.5";
    bitmapName = "./Images/move";
};

new GuiCursor(TextEditCursor)
{
    hotSpot = "1 1";
    renderOffset = "0.5 0.5";
    bitmapName = "./Images/textEdit";
};

//---------------------------------------------------------------------------------------------
// Cursor toggle functions.
//---------------------------------------------------------------------------------------------
$cursorControlled = true;
function showCursor()
{
    if ($cursorControlled)
        lockMouse(false);
        
    Canvas.cursorOn();
}

function hideCursor()
{
    if ($cursorControlled)
        lockMouse(true);
    Canvas.cursorOff();
}

//---------------------------------------------------------------------------------------------
// In the CanvasCursor package we add some additional functionality to the built-in GuiCanvas
// class, of which the global Canvas object is an instance. In this case, the behavior we want
// is for the cursor to automatically display, except when the only guis visible want no
// cursor - usually the in game interface.
//---------------------------------------------------------------------------------------------
package CanvasCursorPackage
{

//---------------------------------------------------------------------------------------------
// checkCursor
// The checkCursor method iterates through all the root controls on the canvas checking each
// ones noCursor property. If the noCursor property exists as anything other than false or an
// empty string on every control, the cursor will be hidden.
//---------------------------------------------------------------------------------------------
function GuiCanvas::checkCursor(%this)
{
    %count = %this.getCount();
    for (%i = 0; %i < %count; %i++)
    {
        %control = %this.getObject(%i);
        
        if ((%control.noCursor $= "") || !%control.noCursor)
        {
            showCursor();
            return;
        }
    }
    
    // If we get here, every control requested a hidden cursor, so we oblige.
    hideCursor();
}

//---------------------------------------------------------------------------------------------
// The following functions override the GuiCanvas defaults that involve changing the content
// of the Canvas. Basically, all we are doing is adding a call to checkCursor to each one.
//---------------------------------------------------------------------------------------------
function GuiCanvas::setContent(%this, %ctrl)
{
    Parent::setContent(%this, %ctrl);
    %this.checkCursor();
}

function GuiCanvas::pushDialog(%this, %ctrl, %layer)
{
    Parent::pushDialog(%this, %ctrl, %layer);
    %this.checkCursor();
}

function GuiCanvas::popDialog(%this, %ctrl)
{
    Parent::popDialog(%this, %ctrl);
    %this.checkCursor();
}

function GuiCanvas::popLayer(%this, %layer)
{
    Parent::popLayer(%this, %layer);
    %this.checkCursor();
}

};

activatePackage(CanvasCursorPackage);
