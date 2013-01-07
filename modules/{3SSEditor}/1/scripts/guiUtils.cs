//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This function duplicates a gui control.  Simply pass in 
/// the control to duplicate (mainMenuGui for example) and it will make a full copy
/// of the original - minus dynamic fields.
/// Passing the %data parameter causes the function to copy the names of the original 
/// controls and append the contents of %data to the name to allow manipulation of the 
/// copy's members.
/// </summary>
/// <param name="control">The control to duplicate.</param>
/// <param name="data">Used to mutate names from the original gui - optional.</param>
/// <param name="class">Assigns a script class to all controls in the object to be duplicated - optional.</param>
function GuiUtils::duplicateGuiObject(%control, %data, %class)
{
    %type = %control.getClassName();
    %filter = strreplace(%type, "Button", "");
    if (%filter $= %type)
        eval("%newControl = new "@%type@"();");
    else
        eval("%newControl = new "@%type@"(){class = \""@%class@"\";};");

    if (%data !$= "")
        %newControl.setName(%control.getName() @ %data);

    %newControl.Extent = %control.Extent;
    %newControl.MinExtent = "2 2";
    %newControl.Position = %control.Position;
    %newControl.setProfile(%control.Profile);
    %newControl.canSave = "0";
    %newControl.Visible = %control.Visible;
    %newControl.Text = %control.Text;
    GuiUtils::copyAssets(%control, %newControl);
    %newControl.data = %data;

    %numChildren = %control.getCount();
    %index = 0;
    while (%index < %numChildren)
    {
        %obj = %control.getObject(%index);
        %newControl.add(GuiUtils::duplicateChildGuiObject(%obj, %data, %class));
        %index++;
    }

    return %newControl;
}

function GuiUtils::duplicateChildGuiObject(%control, %data, %class)
{
    %type = %control.getClassName();
    %filter = strreplace(%type, "Button", "");
    if (%filter $= %type)
        eval("%newControl = new "@%type@"();");
    else
        eval("%newControl = new "@%type@"(){class = \""@%class@"\";};");

    if (%data !$= "")
        %newControl.setName(%control.getName() @ %data);

    %newControl.Extent = %control.Extent;
    %newControl.MinExtent = "2 2";
    %newControl.Position = %control.Position;
    %newControl.setProfile(%control.Profile);
    %newControl.canSave = "0";
    %newControl.Visible = %control.Visible;
    %newControl.Text = %control.Text;
    GuiUtils::copyAssets(%control, %newControl);
    %newControl.data = %data;

    %numChildren = %control.getCount();
    %index = 0;
    while (%index < %numChildren)
    {
        %obj = %control.getObject(%index);
        %newControl.add(GuiUtils::duplicateChildGuiObject(%obj, %data, %class));
        %index++;
    }
    return %newControl;
}

/// <summary>
/// This function copys the asset fields from one control to another of the same 
/// type.
/// </summary>
/// <param name="control">The source control.</param>
/// <param name="target">The control to copy asset fields to.</param>
function GuiUtils::copyAssets(%control, %target)
{
    %type = %control.getClassName();
    switch$(%type)
    {
        case "GuiSpriteCtrl":
            %target.Image = %control.Image;

        case "GuiImageButtonCtrl":
            %target.NormalImage = %control.NormalImage;
            %target.HoverImage = %control.HoverImage;
            %target.DownImage = %control.DownImage;
            %target.InactiveImage = %control.InactiveImage;
    }
}

/// <summary>
/// This function resizes a gui control.
/// Simply pass in the control to resize (mainMenuGui for example) and it will resize the control
/// and all children, maintaining relative positioning and aspect.
/// </summary>
/// <param name="control">The control to resize.</param>
/// <param name="originalSize">The gui's original Extent.</param>
/// <param name="newSize">The desired Extent of the gui.</param>
function GuiUtils::resizeGuiObject(%control, %originalSize, %newSize)
{
    %xRatio = %newSize.x / %originalSize.x;
    %yRatio = %newSize.y / %originalSize.y;
    %fontScale = 1.0 * %yRatio;
    %control.resize(0, 0, %newSize.x, %newSize.y);
    %type = %control.getClassName();
    if (%type $= "GuiTextCtrl" || %type $= "GuiTextEditCtrl")
        %control.setProfile( GuiUtils::createScaledTextProfile(%control.Profile, (%control.Profile.FontSize * %fontScale)) );

    %numChildren = %control.getCount();
    %index = 0;
    while (%index < %numChildren)
    {
        %obj = %control.getObject(%index);
        GuiUtils::resizeChildGuiObject(%obj, %xRatio, %yRatio);
        %index++;
    }
}

function GuiUtils::resizeChildGuiObject(%control, %xRatio, %yRatio)
{
    %fontScale = 1.0 * %yRatio;
    %type = %control.getClassName();
    %objXPos = %control.position.x * %xRatio;
    %objYPos = %control.position.y * %yRatio;
    %objXSize = %control.extent.x * %xRatio;
    %objYSize = %control.extent.y * %yRatio;
    %control.resize(%objXPos, %objYPos, %objXSize, %objYSize);
    %type = %control.getClassName();
    if (%type $= "GuiTextCtrl" || %type $= "GuiTextEditCtrl")
        %control.setProfile( GuiUtils::createScaledTextProfile(%control.Profile, (%control.Profile.FontSize * %fontScale)) );

    %numChildren = %control.getCount();
    %index = 0;
    while (%index < %numChildren)
    {
        %obj = %control.getObject(%index);
        GuiUtils::resizeChildGuiObject(%obj, %xRatio, %yRatio);
        %index++;
    }
}

function GuiUtils::createScaledTextProfile(%profile, %size)
{
    %name = %profile.getName();
    %scaledName = %name @ "Scaled" @ %size;
    %newName = strreplace(%scaledName, ".", "");

    if (!isObject(%newName))
    {
        new GuiControlProfile(%newName);
        %count = %profile.getFieldCount();

        // fill color
        %newName.opaque = %profile.opaque;
        %newName.fillColor = %profile.fillColor;
        %newName.fillColorHL = %profile.fillColorHL;
        %newName.fillColorNA = %profile.fillColorNA;

        // border color
        %newName.border = %profile.border;
        %newName.borderColor = %profile.borderColor;
        %newName.borderColorHL = %profile.borderColorHL;
        %newName.borderColorNA = %profile.borderColorNA;

        // font
        %newName.fontType = %profile.fontType;
        %newName.fontSize = %size;

        %newName.fontColor = %profile.fontColor;
        %newName.fontColorHL = %profile.fontColorHL;
        %newName.fontColorNA = %profile.fontColorNA;
        %newName.fontColorSEL = %profile.fontColorSEL;
        
        %newName.justify = %profile.justify;
        %newName.textOffset = %profile.textOffset;
        %newName.numbersOnly = %profile.numbersOnly;
    }

    %newName.FontSize = %size;

    return %newName;
}
