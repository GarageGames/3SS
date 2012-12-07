//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$GUI::FileSpec = "Torque Gui Files (*.gui.taml)|*.gui.taml|All Files (*.*)|*.*|";

/// GuiBuilder::getSaveName - Open a Native File dialog and retrieve the
///  location to save the current document.
/// @arg defaultFileName   The FileName to default in the field and to be selected when a path is opened
function GuiBuilder::getSaveName(%defaultFileName)
{
    // if we're editing a game, we want to default to the games dir.
    // if we're not, then we default to the tools directory or the base.
    if (%defaultFileName !$= "" && isPathExpando("project"))
        %prefix = "^project";
    else if (isPathExpando("modules"))
        %prefix = "^modules";
    else
        %prefix = "";

    if (%defaultFileName $= "")
        %defaultFileName = expandPath(%prefix @ "/gui/untitled.gui.taml");
    else 
        %defaultFileName = expandPath(%prefix @ "/" @ %defaultFileName);

    %path = filePath(%defaultFileName) @ "/";
    %dlg = new SaveFileDialog()
    {
        Filters           = $GUI::FileSpec;
        DefaultPath       = %path;
        DefaultFile       = getSaveDefaultFile(%defaultFileName);
        ChangePath        = true;
        OverwritePrompt   = true;
    };

    if (%dlg.Execute())
    {
        $Pref::GuiEditor::LastPath = filePath(%dlg.FileName);
        %filename = %dlg.FileName;

        // make sure we have a .gui.taml extension
        if (fileExt(%dlg.FileName) !$= ".gui.taml")
        {
            // strip the extension just in case
            %temp = fileBase(%dlg.FileName);
            %filename =  $Pref::GuiEditor::LastPath @ "/" @ %temp @ ".gui.taml";
        }
    }
    else
        %filename = "";

    %dlg.delete();

    return %filename;
}

function GuiBuilder::getOpenName(%defaultFileName)
{
    if (%defaultFileName $= "")
        %defaultFileName = expandPath("^project/gui/untitled.gui.taml");

    %dlg = new OpenFileDialog()
    {
        Filters        = $GUI::FileSpec;
        DefaultPath    = $Pref::GuiEditor::LastPath;
        DefaultFile    = %defaultFileName;
        ChangePath     = false;
        MustExist      = true;
    };

    if (%dlg.Execute())
    {
        $Pref::GuiEditor::LastPath = filePath(%dlg.FileName);
        %filename = %dlg.FileName;      
        %dlg.delete();
        return %filename;
    }

    %dlg.delete();
    return "";   
}
