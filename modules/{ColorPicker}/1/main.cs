//-----------------------------------------------------------------------------
// Torque2D Packaging Utility
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

function buildProject()
{
    Canvas.pushDialog(ColorPickerGui);
}

function initializeColorPicker(%scopeSet)
{
    //-----------------------------------------------------------------------------
    // Execute scripts
    //-----------------------------------------------------------------------------
    exec("./scripts/gui.cs");
    exec("./scripts/profiles.cs");
    
    //-----------------------------------------------------------------------------
    // Load GUIs
    //-----------------------------------------------------------------------------
    %scopeSet.add( TamlRead("./gui/colorPicker.gui.taml") );
    
    //-----------------------------------------------------------------------------
    // Initialization
    //----------------------------------------------------------------------------- 
    // Control that we need to update
    $ColorPickerCallback = ""; 
    
    // ColorI
    $ColorCallbackType   = 1;  
}

function destroyColorPicker()
{
}