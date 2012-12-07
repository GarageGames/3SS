//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function initializeSoundEditor(%scopeSet)
{   
    //-----------------------------------------------------------------------------
    // Load scripts
    //-----------------------------------------------------------------------------
    exec("./scripts/editor.cs");
    exec("./scripts/soundEventManager.cs");
   
    //-----------------------------------------------------------------------------
    // Load GUIs
    //-----------------------------------------------------------------------------
    %scopeSet.add( TamlRead("./gui/soundImporter.gui.taml") );
   
    //-----------------------------------------------------------------------------
    // Initialization
    //-----------------------------------------------------------------------------
    // Create the editor object for namespace usage
    new ScriptObject(SoundEditor);
    %scopeSet.add(SoundEditor);
    
    $SIAssetNameTextEditMessageString = "Enter asset name...";
    $SIAssetLocationTextEditMessageString = "Select an asset...";
    $T2D::SoundSpec = "All Supported Sounds (*.wav)|*.wav|";
    initializeSoundEditorEventManager();
}

function destroySoundEditor()
{
    destroySoundEditorEventManager();
}