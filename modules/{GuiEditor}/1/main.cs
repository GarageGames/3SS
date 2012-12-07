//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function initializeGuiEditor(%scopeSet)
{
   // Load Client Scripts.
   %scopeSet.add( TamlRead("./gui/guiEditorGui.gui.taml") );
   exec("./scripts/guiEditor.cs");
   
   %scopeSet.add( TamlRead("./gui/newGuiDialog.gui.taml") );
   exec("./scripts/fileDialogs.cs");
   
   %scopeSet.add( TamlRead("./gui/guiEditorPrefs.gui.taml") );
   exec("./scripts/guiEditorPrefsDlg.cs");
   
   %scopeSet.add( TamlRead("./gui/guiEditorPalette.gui.taml") );
   exec("./scripts/guiEditorUndo.cs");

   exec("./scripts/guiApplicationClose.cs");
}

function destroyGuiEditor()
{
}
