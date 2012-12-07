//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$Gui::defaultGridSize = 8;

//-----------------------------------------------------------------------------------------
// Buttons
//-----------------------------------------------------------------------------------------
function GuiEditorPrefsDlgOkBtn::onAction(%this)
{
    $pref::guiEditor::snap2gridsize = GuiEditorPrefsDlgGridEdit.getValue();
    
    if ($pref::guiEditor::snap2grid)
        GuiEditor.setSnapToGrid($pref::guiEditor::snap2gridsize);
    
    Canvas.popDialog(GuiEditorPrefsDlg);
}

function GuiEditorPrefsDlgCancelBtn::onAction(%this)
{
    Canvas.popDialog(GuiEditorPrefsDlg);
}

function GuiEditorPrefsDlgDefaultsBtn::onAction(%this)
{
    GuiEditorPrefsDlgGridSlider.setValue($Gui::defaultGridSize);
}

//-----------------------------------------------------------------------------------------
// Grid
//-----------------------------------------------------------------------------------------
function GuiEditorPrefsDlgGridEdit::onWake(%this)
{
    %this.setValue($pref::guiEditor::snap2gridsize);
}

function GuiEditorPrefsDlgGridSlider::onWake(%this)
{
    %this.setValue($pref::guiEditor::snap2gridsize);
}

function GuiEditorPrefsDlgGridSlider::onAction(%this)
{
    %val = %this.value;
    
    if(%val == 0)
        %val = 1;
        
    GuiEditorPrefsDlgGridEdit.setvalue(%val);
}