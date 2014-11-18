//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function initialize3SSHomeScreens(%scopeSet)
{
    //-----------------------------------------------------------------------------
    // Execute scripts
    //-----------------------------------------------------------------------------
    exec("./scripts/gui.cs");
    exec("./scripts/templateListGui.cs");
    exec("./scripts/exerciseListGui.cs");
    exec("./scripts/gameManagerPage.cs");
    exec("./scripts/exerciseManagerPage.cs");
    exec("./scripts/homeScreenToolbar.cs");
    exec("./scripts/allGamesPage.cs");
    exec("./scripts/gamesLocation.cs");
    exec("./scripts/helpMarquee.cs");

    //-----------------------------------------------------------------------------
    // Load GUIs
    //----------------------------------------------------------------------------- 
    %scopeSet.add(TamlRead("./gui/gameManagerGui.gui.taml"));
    %scopeSet.add(TamlRead("./gui/templateListGui.gui.taml"));
    %scopeSet.add(TamlRead("./gui/exerciseManagerGui.gui.taml"));
    %scopeSet.add(TamlRead("./gui/exerciseListGui.gui.taml"));
    %scopeSet.add(TamlRead("./gui/gamesLocation.gui.taml"));
    %scopeSet.add(TamlRead("./gui/homeScreenToolbar.gui.taml"));
    %scopeSet.add(TamlRead("./gui/allGamesGui.gui.taml"));
}

function destroy3SSHomeScreens()
{
}