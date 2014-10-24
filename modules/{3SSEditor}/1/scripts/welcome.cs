//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2014
//-----------------------------------------------------------------------------

function WelcomeGui::onWake(%this)
{
    WG_GamesLocationDisplay.setText($UserGamesLocation);
}

function WG_ChangeGamesLocationButton::onClick(%this)
{
    %currentFile = WG_GamesLocationDisplay.getText();

    %dlg = new OpenFolderDialog()
    {
        DefaultPath = %currentFile;
    };

    if (%dlg.Execute())
    {
        $UserGamesLocation = %dlg.FileName;
        WG_GamesLocationDisplay.setText($UserGamesLocation);
    }

    %dlg.delete();
}

function WG_StartButton::onClick(%this)
{
    GamesLocationGui.refresh();
    
    Canvas.popDialog(WelcomeGui);   
}