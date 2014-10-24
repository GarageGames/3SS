//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2014
//-----------------------------------------------------------------------------

function GamesLocationGui::onWake(%this)
{
    %this.refresh();
}

function GLG_ChangeGamesLocationButton::onClick(%this)
{
    %currentFile = GLG_GamesLocationDisplay.getText();

    %dlg = new OpenFolderDialog()
    {
        DefaultPath = %currentFile;
    };

    if (%dlg.Execute())
    {
        $UserGamesLocation = %dlg.FileName;
        GLG_GamesLocationDisplay.setText($UserGamesLocation);
        AG_GamesList.Refresh();
        GMGamesList.Refresh();
    }

    %dlg.delete();
}

function GamesLocationGui::refresh(%this)
{
    GLG_GamesLocationDisplay.setText($UserGamesLocation);
}