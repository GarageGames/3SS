//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function GuiPreviewGui::display(%this, %gui)
{
    %guiCopy = GuiUtils::duplicateGuiObject(%gui);
    GuiUtils::resizeGuiObject(%guiCopy, %gui.extent, %this.Extent);
    Gp_GuiPreview.add(%guiCopy);
    Canvas.pushDialog(%this);
}

function Gp_DoneBtn::onClick(%this)
{
    %obj = Gp_GuiPreview.getObject(0);
    if ( isObject( %obj ) )
        %obj.delete();

    Canvas.popDialog(GuiPreviewGui);
}