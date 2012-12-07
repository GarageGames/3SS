//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function Ab_FrameControl::onMouseDown(%this, %modifier, %mousePoint, %mouseClickCount)
{
    if (%mouseClickCount == 2)
        AnimationBuilder.appendFrame(%this.getParent().Frame);
    else
    {
        %position = %mousePoint;
        %halfParentWidth = %this.getParent().getExtent().x / 2;
        %halfParentHeight = %this.getParent().getExtent().y / 2;
        %position.x -= %halfParentWidth;
        %position.y -= %halfParentHeight;
        AnimationBuilder.createDraggingControl(%this.getParent(), %position, %mousePoint, %this.Extent);
    }
}

function Ab_FrameControl::onTouchDragged(%this, %modifier, %mousePoint, %mouseClickCount)
{
    if (!%this.getParent().isActive())
        return;

    %position = %mousePoint;
    %halfParentWidth = %this.getParent().getExtent().x / 2;
    %halfParentHeight = %this.getParent().getExtent().y / 2;
    %position.x -= %halfParentWidth;
    %position.y -= %halfParentHeight;
    AnimationBuilder.createDraggingControl(%this.getParent(), %position, %mousePoint, %this.Extent);
}