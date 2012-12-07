//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function initializeFontEditor()
{
    //-----------------------------------------------------------------------------
    // Execute scripts
    //-----------------------------------------------------------------------------
    exec("./gui.cs");
    
    //-----------------------------------------------------------------------------
    // Load GUIs
    //-----------------------------------------------------------------------------
    exec("./gui/FontTool.ed.gui");
    TamlWrite(FontToolGui, "./gui/fontTool.gui.taml");
    //%scopeSet.add( TamlRead("./gui/fontTool.gui.gaml") );
    
    //-----------------------------------------------------------------------------
    // Initialization
    //----------------------------------------------------------------------------- 
    if (!isObject(FontTool)) 
    {
        new ScriptObject(FontTool);
        %scopeSet.add(FontTool);
    }
}

function destroyFontEditor()
{
    if (isObject(FontTool))
        FontTool.destroy();
}

function FontTool::onAdd(%this)
{   
    %this.scene = new Scene();
}

function FontTool::onRemove(%this)
{
    %this.destroy();
}