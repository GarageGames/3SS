//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
function DeleteTagButton::selfDelete(%this)
{
    // The the parent container for the tag
    %container = %this.getParent();

    // Get the text control in the tag container (every instance should have this)
    %tagTextControl = %container.findObjectByInternalName("TagContainerTextControl");

    // Get the name of the tag
    %tagName = %tagTextControl.text;

    %parent = %container;

    // Get to the top parent
    while(%parent !$= "" && %parent != Canvas.getID())
    {
        %root = %parent;
        %parent = %parent.getParent();
    }

    %root.removeTag(%tagName);
}

function CreateTagBar(%parentGui, %position, %tag)
{
    %newTagContainer = new GuiControl() 
    {
        canSaveDynamicFields = "0";
        isContainer = "1";
        Profile = "EditorPanelDark";
        HorizSizing = "right";
        VertSizing = "bottom";
        Position = %position;
        Extent = "118 29";
        MinExtent = "8 2";
        canSave = "1";
        Visible = "1";
        tooltipprofile = "GuiDefaultProfile";
        hovertime = "1000";
    };

    %tagName = new GuiTextCtrl()
    {
        canSaveDynamicFields = "0";
        internalName = "TagContainerTextControl";
        isContainer = "0";
        Profile = "GuiTextProfile";
        HorizSizing = "right";
        VertSizing = "bottom";
        Position = "41 7";
        Extent = "75 17";
        MinExtent = "8 2";
        canSave = "1";
        Visible = "1";
        hovertime = "1000";
        text = %tag;
        maxLength = "1024";
    };
    %tagDeleteButton = new GuiIconButtonCtrl()
    {
        canSaveDynamicFields = "0";
        Class = "DeleteTagButton";
        isContainer = "0";
        Profile = "GuiButtonProfile";
        HorizSizing = "right";
        VertSizing = "bottom";
        Position = "8 5";
        Extent = "20 20";
        MinExtent = "8 2";
        canSave = "1";
        Visible = "1";
        tooltipprofile = "EditorToolTipProfile";
        ToolTip = "Remove tag.";
        hovertime = "100";
        groupNum = "-1";
        buttonType = "PushButton";
        useMouseEvents = "0";
        buttonMargin = "4 4";
        iconBitmap = "^{EditorAssets}/gui/images/iconCancel";
        iconLocation = "Left";
        sizeIconToButton = "1";
        textLocation = "Center";
        textMargin = "4";
    };
   
    %tagDeleteButton.command = %tagDeleteButton @ ".selfDelete();";

    %newTagContainer.add(%tagname);
    %newTagContainer.add(%tagDeleteButton);

    %parentGui.add(%newTagContainer);

    return %newTagContainer;
}