//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function initializeT2DStartPage()
{
    addResPath("^{T2DStartPage}");
    
    //-----------------------------------------------------------------------------
    // Load start page scripts
    //-----------------------------------------------------------------------------
    exec("./scripts/t2dStartPage.ed.cs");
    
    //-----------------------------------------------------------------------------
    // Load start page GUIs
    //-----------------------------------------------------------------------------
    TamlRead("./gui/t2dStartPage.gui.taml");
    
    Canvas.setContent(T2DStartPage);
}

function destroyTgbStartPage()
{
}