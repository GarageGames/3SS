//-----------------------------------------------------------------------------
// Torque Card Template
// Copyright © GarageGames, LLC.
//-----------------------------------------------------------------------------

function TableSelectGui::onWake(%this)
{
   %numTables = $Save::GeneralSettings::NumberOfTables;
   
   TableSelectTable1.setVisible( %numTables >= 1 );
   TableSelectTable2.setVisible( %numTables >= 2 );
   TableSelectTable3.setVisible( %numTables >= 3 );
   TableSelectTable4.setVisible( %numTables >= 4 );
   TableSelectTable5.setVisible( %numTables >= 5 );
   
   if($Save::Interface::NumberOfHelpScreens <= 0)
      TableSelectHowtoButton.setVisible(false);
}

function TableSelectGui::loadLevel(%this, %level)
{
   // Reset values used by the levels
   $NumberOfPlayers = 0;
   $SeatSelectingPlayer = 0;
   $PlayInputPlayer = null; 
   
   // Extract the file name from the full path 
   %levelFileName = %level @ ".scene.taml";
   %levelFilePath = "^BlackjackTemplate/data/levels/" @ %levelFileName;
   %levelFilePathDSO = %levelFilePath @ ".dso";
   
   // Check if the file exists
   if (!isFile(%levelFilePath) && !isFile(%levelFilePathDSO))
   {
        echo("Could not find level file: " @ %levelFilePath);  
        return; 
   }
      
   // Schedule the level to load
   sceneWindow2D.schedule(200, "loadLevel", %levelFilePath);  
   
   // Schedule the gui to be removed
   Canvas.schedule(200, "popDialog", TableSelectGui);
}

function TableSelectTable1::onClick(%this)
{
   TableSelectGui.loadLevel($Save::GeneralSettings::Table1Map);
}

function TableSelectTable2::onClick(%this)
{
   TableSelectGui.loadLevel($Save::GeneralSettings::Table2Map);
}

function TableSelectTable3::onClick(%this)
{
   TableSelectGui.loadLevel($Save::GeneralSettings::Table3Map);
}

function TableSelectTable4::onClick(%this)
{
   TableSelectGui.loadLevel($Save::GeneralSettings::Table4Map);
}

function TableSelectTable5::onClick(%this)
{
   TableSelectGui.loadLevel($Save::GeneralSettings::Table5Map);
}

function TableSelectGuiObject::onLevelLoaded(%this)
{
   Canvas.pushDialog(TableSelectGui);
}

function TableSelectHowtoButton::onClick()
{
   howtoButtonClicked();
}

function TableSelectResetBankButton::onClick()
{
   resetBankButtonClicked();
}

function TableSelectCreditsButton::onClick()
{
   creditsButtonClicked();
}