//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function initializeUtilityGui(%scopeSet)
{
    //-----------------------------------------------------------------------------
    // Load scripts
    //-----------------------------------------------------------------------------
    exec("./scripts/profiles.cs");
    exec("./scripts/assetViewer.cs");
    exec("./scripts/fileDialogBase.cs");
    exec("./scripts/messageBox.cs");
    exec("./scripts/openFileDialog.cs");
    exec("./scripts/saveFileDialog.cs");
    exec("./scripts/warningDialog.cs");
    exec("./scripts/confirmDialog.cs");
    exec("./scripts/utilityFunctions.cs");
    exec("./scripts/colorPicker.cs");
    exec("./scripts/toolNoticeDialogGui.cs");
    exec("./scripts/toolConfirmDeleteGui.cs");
    exec("./scripts/toolConfirmOverwriteGui.cs");
    exec("./scripts/testGameWindow.cs");
    exec("./scripts/tagDropdownList.cs");
    exec("./scripts/tagListContainer.cs");
    exec("./scripts/confirmActionDialog.cs");
    exec("./scripts/imagePreview.cs");
    exec("./scripts/guiPreview.cs");

    //-----------------------------------------------------------------------------
    // Load GUIs
    //-----------------------------------------------------------------------------
    %scopeSet.add( TamlRead("./gui/colorPicker.gui.taml") );
    %scopeSet.add( TamlRead("./gui/messageBoxOK.gui.taml") );
    %scopeSet.add( TamlRead("./gui/messageBoxOKCancelDetails.gui.taml") );
    %scopeSet.add( TamlRead("./gui/messageBoxSaveChanges.gui.taml") );
    %scopeSet.add( TamlRead("./gui/messageBoxYesNo.gui.taml") );
    %scopeSet.add( TamlRead("./gui/messageBoxYesNoCancel.gui.taml") );
    %scopeSet.add( TamlRead("./gui/messagePopup.gui.taml") );
    %scopeSet.add( TamlRead("./gui/confirmDialog.gui.taml") );
    %scopeSet.add( TamlRead("./gui/warningDialog.gui.taml") );
    %scopeSet.add( TamlRead("./gui/toolNoticeDialogGui.gui.taml") );
    %scopeSet.add( TamlRead("./gui/toolConfirmDeleteGui.gui.taml") );
    %scopeSet.add( TamlRead("./gui/toolConfirmOverwriteGui.gui.taml") );
    %scopeSet.add( TamlRead("./gui/testGameWindow.gui.taml") );
    %scopeSet.add( TamlRead("./gui/toolConfirmActionGui.gui.taml") );
    %scopeSet.add( TamlRead("./gui/imagePreview.gui.taml") );
    %scopeSet.add( TamlRead("./gui/guiPreview.gui.taml") );
    
    //-----------------------------------------------------------------------------
    // Initialization
    //-----------------------------------------------------------------------------
    $Gui::clipboardFile = expandPath("./clipboard.gui");
}

function destroyUtilityGui()
{
}