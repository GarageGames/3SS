//-----------------------------------------------------------------------------
// Torque2D Packaging Utility
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

function initializePublisher( %scopeSet )
{
    //-----------------------------------------------------------------------------
    // Load scripts
    //-----------------------------------------------------------------------------
    exec("./scripts/gui.cs");
    exec("./scripts/publishWindows.cs");
    exec("./scripts/publishOSX.cs");
    exec("./scripts/publishIOS.cs");
    
    //-----------------------------------------------------------------------------
    // Load GUIs
    //-----------------------------------------------------------------------------
    %scopeSet.add( TamlRead("./gui/publisher.gui.taml") );
    %scopeSet.add( TamlRead("./gui/publisherWorking.gui.taml") );

    //-----------------------------------------------------------------------------
    // Initialization
    //-----------------------------------------------------------------------------
    PublisherTextBoxDestination.setText($UserGamesLocation);
}

function destroyPublisher( %scopeSet )
{
}

function buildProject()
{
    Canvas.pushDialog(PublisherGui);
}