//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This function duplicates a gui control up to four levels deep.  Simply pass in 
/// the control to duplicate (mainMenuGui for example) and it will make a full copy
/// of the original - minus dynamic fields.
/// Passing the %data parameter causes the function to copy the names of the original 
/// controls and append the contents of %data to the name to allow manipulation of the 
/// copy's members.
/// </summary>
/// <param name="control">The control to duplicate.</param>
/// <param name="data">Used to mutate names from the original gui - optional.</param>
/// <param name="class">Assigns a script class to all controls in the object to be duplicated - optional.</param>
function duplicateControl(%control, %data, %class)
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
    copyAssets(%control, %newControl);
    %newControl.data = %data;
    for (%a = 0; %a < %control.getCount(); %a++)
    {
        %obja = %control.getObject(%a);
        %type = %obja.getClassName();
        %filter = strreplace(%type, "Button", "");
        if (%filter $= %type)
            eval("%newObja = new "@%type@"();");
        else
            eval("%newObja = new "@%type@"(){class = \""@%class@"\";};");

        if (%data !$= "")
            %newObja.setName(%obja.getName() @ %data);

        %newObja.Extent = %obja.Extent;
        %newObja.MinExtent = "2 2";
        %newObja.Position = %obja.Position;
        %newObja.setProfile(%obja.Profile);
        %newObja.canSave = "0";
        %newObja.Visible = %obja.Visible;
        %newObja.Text = %obja.Text;
        copyAssets(%obja, %newObja);
        %newObja.data = %data;
        for (%b = 0; %b < %obja.getCount(); %b++)
        {
            %objb = %obja.getObject(%b);
            %type = %objb.getClassName();
            %filter = strreplace(%type, "Button", "");
            if (%filter $= %type)
                eval("%newObjb = new "@%type@"();");
            else
                eval("%newObjb = new "@%type@"(){class = \""@%class@"\";};");

            if (%data !$= "")
                %newObjb.setName(%objb.getName() @ %data);

            %newObjb.Extent = %objb.Extent;
            %newObjb.MinExtent = "2 2";
            %newObjb.Position = %objb.Position;
            %newObjb.setProfile(%objb.Profile);
            %newObjb.canSave = "0";
            %newObjb.Visible = %objb.Visible;
            %newObjb.Text = %objb.Text;
            copyAssets(%objb, %newObjb);
            %newObjb.data = %data;
            for (%c = 0; %c < %objb.getCount(); %c++)
            {
                %objc = %objb.getObject(%c);
                %type = %objc.getClassName();
                %filter = strreplace(%type, "Button", "");
                if (%filter $= %type)
                    eval("%newObjc = new "@%type@"();");
                else
                    eval("%newObjc = new "@%type@"(){class = \""@%class@"\";};");

                if (%data !$= "")
                    %newObjc.setName(%objc.getName ()@ %data);

                %newObjc.Extent = %objc.Extent;
                %newObjcMinExtent = "2 2";
                %newObjc.Position = %objc.Position;
                %newObjc.setProfile(%objc.Profile);
                %newObjc.canSave = "0";
                %newObjc.Visible = %objc.Visible;
                %newObjc.Text = %objc.Text;
                copyAssets(%objc, %newObjc);
                %newObjc.data = %data;
                for (%d = 0; %d < %objc.getCount(); %d++)
                {
                    %objd = %objc.getObject(%c);
                    %type = %objd.getClassName();
                    %filter = strreplace(%type, "Button", "");
                    if (%filter $= %type)
                        eval("%newObjd = new "@%type@"();");
                    else
                        eval("%newObjd = new "@%type@"(){class = \""@%class@"\";};");

                    if (%data !$= "")
                        %newObjd.setName(%objd.getName() @ %data);

                    %newObjd.Extent = %objd.Extent;
                    %newObjd.MinExtent = "2 2";
                    %newObjd.Position = %objd.Position;
                    %newObjd.setProfile(%objd.Profile);
                    %newObjd.canSave = "0";
                    %newObjd.Visible = %objd.Visible;
                    %newObjd.Text = %objd.Text;
                    copyAssets(%objd, %newObjd);
                    %newObjd.data = %data;

                    %newObjc.addGuiControl(%newObjd);
                }
                %newObjb.addGuiControl(%newObjc);
            }
            %newObja.addGuiControl(%newObjb);
        }
        %newControl.addGuiControl(%newObja);
    }
    return %newControl;
}

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
    copyAssets(%control, %newControl);
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
    copyAssets(%control, %newControl);
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
function copyAssets(%control, %target)
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
/// This function resizes a gui control up to four levels deep.  Simply pass in 
/// the control to resize (mainMenuGui for example) and it will resize the control
/// and all children, maintaining relative positioning and aspect.
/// </summary>
/// <param name="control">The control to resize.</param>
/// <param name="originalSize">The gui's original Extent.</param>
/// <param name="newSize">The desired Extent of the gui.</param>
function resizeControl(%control, %originalSize, %newSize)
{
    %xRatio = %newSize.x / %originalSize.x;
    %yRatio = %newSize.y / %originalSize.y;
    %fontScale = 1.0 * %yRatio;
    %control.resize(0, 0, %newSize.x, %newSize.y);
    %type = %control.getClassName();
    if (%type $= "GuiTextCtrl" || %type $= "GuiTextEditCtrl")
        %control.setProfile( createScaledTextProfile(%control.Profile, (%control.Profile.FontSize * %fontScale)) );
    for (%a = 0; %a < %control.getCount(); %a++)
    {
        %obja = %control.getObject(%a);
        %objaXPos = %obja.position.x * %xRatio;
        %objaYPos = %obja.position.y * %yRatio;
        %objaXSize = %obja.extent.x * %xRatio;
        %objaYSize = %obja.extent.y * %yRatio;
        %obja.resize(%objaXPos, %objaYPos, %objaXSize, %objaYSize);
        %type = %obja.getClassName();
        if (%type $= "GuiTextCtrl" || %type $= "GuiTextEditCtrl")
            %obja.setProfile( createScaledTextProfile(%obja.Profile, (%obja.Profile.FontSize * %fontScale)) );
        for (%b = 0; %b < %obja.getCount(); %b++)
        {
            %objb = %obja.getObject(%b);
            %objbXPos = %objb.position.x * %xRatio;
            %objbYPos = %objb.position.y * %yRatio;
            %objbXSize = %objb.extent.x * %xRatio;
            %objbYSize = %objb.extent.y * %yRatio;
            %objb.resize(%objbXPos, %objbYPos, %objbXSize, %objbYSize);
            %type = %objb.getClassName();
            if (%type $= "GuiTextCtrl" || %type $= "GuiTextEditCtrl")
                %objb.setProfile( createScaledTextProfile(%objb.Profile, (%objb.Profile.FontSize * %fontScale)) );
            for (%c = 0; %c < %objb.getCount(); %c++)
            {
                %objc = %objb.getObject(%c);
                %objcXPos = %objc.position.x * %xRatio;
                %objcYPos = %objc.position.y * %yRatio;
                %objcXSize = %objc.extent.x * %xRatio;
                %objcYSize = %objc.extent.y * %yRatio;
                %objc.resize(%objcXPos, %objcYPos, %objcXSize, %objcYSize);
                %type = %objc.getClassName();
                if (%type $= "GuiTextCtrl" || %type $= "GuiTextEditCtrl")
                    %objc.setProfile( createScaledTextProfile(%objc.Profile, (%objc.Profile.FontSize * %fontScale)) );
                for (%d = 0; %d < %objc.getCount(); %d++)
                {
                    %objd = %objc.getObject(%c);
                    %objdXPos = %objd.position.x * %xRatio;
                    %objdYPos = %objd.position.y * %yRatio;
                    %objdXSize = %objd.extent.x * %xRatio;
                    %objdYSize = %objd.extent.y * %yRatio;
                    %objd.resize(%objdXPos, %objdYPos, %objdXSize, %objdYSize);
                    %type = %objd.getClassName();
                    if (%type $= "GuiTextCtrl" || %type $= "GuiTextEditCtrl")
                        %objd.setProfile( createScaledTextProfile(%objd.Profile, (%objd.Profile.FontSize * %fontScale)) );
                }
            }
        }
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
        %control.setProfile( createScaledTextProfile(%control.Profile, (%control.Profile.FontSize * %fontScale)) );

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
        %control.setProfile( createScaledTextProfile(%control.Profile, (%control.Profile.FontSize * %fontScale)) );

    %numChildren = %control.getCount();
    %index = 0;
    while (%index < %numChildren)
    {
        %obj = %control.getObject(%index);
        GuiUtils::resizeChildGuiObject(%obj, %xRatio, %yRatio);
        %index++;
    }
}

function createScaledTextProfile(%profile, %size)
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
