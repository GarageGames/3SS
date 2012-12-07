//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function TGBInsiderDlg::addAddon( %this, %addonName )
{
   %addonList = %this.findObjectByInternalName("addonList",true);

   // Generate resource file path
   %addonLocation = $Tools::resourcePath @ "resources/" @ %addonName;

   %addonList.addItem( %addonName @ " - " @ %addonLocation );

}
function TGBInsiderDlg::closeDialog( %this )
{
   Canvas.popDialog( TGBInsiderDlg );
}

function TGBInsiderDlg::showAbout( %this ) 
{
   Canvas.pushDialog( TGBInsiderDlg );
   %book = %this.findObjectByInternalName("insiderTabBook", true );
   %book.selectPage( 2 );
}

function TGBInsiderDlg::showFirstRun( %this )
{
   Canvas.pushDialog( TGBInsiderDlg );

   %book = %this.findObjectByInternalName("insiderTabBook", true );
   %book.selectPage( 0 );  
}

function TGBInsiderDlg::onWake( %this )
{
   // Find Credits ML Text Control
   // Note : -> is equivelent to .findObjectByInternalName(CreditsMLText, false)
   //      :--> is equivelent to .findObjectByInternalName(CreditsMLText, true)
   %cml = %this-->CreditsMLText;

   // Credits Styling (Platform Specific changes go here)
   
   %headerFontBold = "Arial Bold";
   %headerFont = "Arial";
   %headerFontSize = 14;
   %headerFontColor = "FFFFFF";
   %headerFontBoldStyle = "<font:" @ %headerFontBold @ ":" @ %headerFontSize @ "><color:" @ %headerFontColor @ ">";
   %headerFontStyle = "<font:" @ %headerFont @ ":" @ %headerFontSize @ "><color:" @ %headerFontColor @ ">";
   %pageBreak = "<br><br><br><br><br><br><br><br>";
   %br = " <br>";

   //May 16, 1996 - September 4, 2011 Alyssa Otremba

   // Header Font
   %cml.addText( %headerFontStyle ,false );
   %cml.addText( "<just:right>" , false );
   %cml.addText( %headerFontStyle @ "3 Step Studio Version:" SPC getThreeStepStudioVersion() @ %br ,false);
   %cml.addText( "<br><br><br>", false );
   //%cml.addText( %headerFontStyle @ "<just:left>iTorque 2D  is dedicated to " @
                  //"the GarageGames community for standing by us " @
                  //"through thick and thin. Your support and enthusiasm brought us back " @
                  //"to life and drove the release of this engine." @ %br ,false);
   //%cml.addText( %br @ %br, false );

   //%cml.addText( %headerFontStyle @ "<just:left>v1.5 is dedicated to " @
                  //"Alyssa Otremba (5/16/1996 - 8/4/2011). May she rest in peace." @ %br ,false);
   //%cml.addText( %pageBreak, false );

   ////
   //// Dedication (Thanks GG)
   ////
   //%cml.addText( %headerFontStyle @ "<just:center>iTorque 2D  is dedicated to " @
                  //"the GarageGames community for standing by us " @
                  //"through thick and thin. Your support and enthusiasm brought us back " @
                  //"to life and drove the release of this engine." @ %br ,false);
   //%cml.addText( %br @ %br, false );                 
   
   //
   // Producer
   //     
   %cml.addText( %headerFontBoldStyle @ "<just:center>Producer" @ %br ,false);
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "Derek Bronson" @ %br, false );
   %cml.addText( %br @ %br, false );
   
   //
   // Project Management
   //
   %cml.addText( %headerFontBoldStyle @ "Project Management" @ %br ,false);
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "James Dickinson" @ %br, false );
   %cml.addText( %br @ %br, false );
   
   //
   // Core Team
   //   
   %cml.addText( %headerFontBoldStyle @ "Core Programming Lead" @ %br ,false);   
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "Michael Perry" @ %br, false );
   %cml.addText( %br @ %br, false );

   %cml.addText( %headerFontBoldStyle @ "Core Programming Team" @ %br ,false);   
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "Melv May" @ %br, false );
   %cml.addText( "Joseph Thomas" @ %br, false );
   %cml.addText( %br @ %br, false );
   
   //
   // Template Teams
   //   
   %cml.addText( %headerFontBoldStyle @ "Template Programming Leads" @ %br ,false);   
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "Scott Burns" @ %br, false );
   %cml.addText( "Kyle Miller" @ %br, false );
   %cml.addText( %br @ %br, false );
   
   %cml.addText( %headerFontBoldStyle @ "Template Programming Team" @ %br ,false);   
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "Doug Poston" @ %br, false );
   %cml.addText( "Richard Ranft" @ %br, false );
   %cml.addText( "Ben Steffan" @ %br, false );
   %cml.addText( "Dave Wyand" @ %br, false );
   %cml.addText( %br @ %br, false );

   //
   // Art
   //
   %cml.addText( %headerFontBoldStyle @ "Art Director" @ %br ,false);   
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "Elie Arabian" @ %br, false );
   %cml.addText( %br @ %br, false );
   
   %cml.addText( %headerFontBoldStyle @ "Art Team" @ %br ,false);   
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "Phill Belanger" @ %br, false );
   %cml.addText( "Jesse Oliger" @ %br, false );
   %cml.addText( "Matt Ostgard" @ %br, false );
   %cml.addText( "John Shuman" @ %br, false );
   %cml.addText( %headerFontBoldStyle @ "Icons by <a:www.famfamfam.com>FamFamFam </a>" @ %br , false );
   %cml.addText( %br @ %br, false );   
   
   //
   // Design
   //
   %cml.addText( %headerFontBoldStyle @ "Design Team" @ %br ,false);   
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "Derek Bronson" @ %br, false );
   %cml.addText( "Jon Williams" @ %br, false );
   %cml.addText( %br @ %br, false );
   
   //
   // Documentation
   //
   %cml.addText( %headerFontBoldStyle @ "Documentation Lead" @ %br ,false);   
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "Geoff Beckstrom" @ %br, false );
   %cml.addText( %br @ %br, false );

   //
   // QA
   //   
   %cml.addText( %headerFontBoldStyle @ "Quality Assurance Team" @ %br ,false);
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "Scott Burns" @ %br, false );
   %cml.addText( "Masaki Oyata" @ %br, false );
   %cml.addText( "Ben Steffen" @ %br, false );
   %cml.addText( "Jon Williams" @ %br, false );
   %cml.addText( %br @ %br, false );

   //
   // Marketing
   //
   %cml.addText( %headerFontBoldStyle @ "Marketing Lead" @ %br ,false);
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "David Montgomery-Blake" @ %br, false );
   %cml.addText( %br @ %br, false );
   
   %cml.addText( %headerFontBoldStyle @ "Marketing Team" @ %br ,false);
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "Dexter Chow" @ %br, false );
   %cml.addText( "Chris Tauscher" @ %br, false );
   %cml.addText( %br @ %br, false );
   
   //
   // Web Team
   //
   %cml.addText( %headerFontBoldStyle @ "Web Team" @ %br, false );
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "Zach Gambino" @ %br, false );
   %cml.addText( "Jehnean Jablonski" @ %br, false );
   %cml.addText( "Matt Wood" @ %br, false );
   %cml.addText( %br @ %br, false );

   //
   // Special Thanks
   //   
   %cml.addText( %headerFontBoldStyle @ "Special Thanks" @ %br ,false);
   
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "The GarageGames Community" @ %br, false );
   %cml.addText( "The original Torque Game Builder team" @ %br, false );
   %cml.addText( "All the usability testers that put in late nights" @ %br, false );
   %cml.addText( "Erik Graham" @ %br, false );
   %cml.addText( "Justin Head" @ %br, false );   
   %cml.addText( "Gabe Huntington" @ %br, false );
   %cml.addText( "Chris Schoenfeldt" @ %br, false );
   %cml.addText( "Edward Maurina" @ %br, false );
   %cml.addText( "Ronny \"orb\" Bangsund" @ %br, false );
   %cml.addText( "Pedro Vicente" @ %br, false );
   %cml.addText( "Jonas Dahlman" @ %br, false );
   %cml.addText( "Justin Mosiman" @ %br, false );
   %cml.addText( "Dean Parker" @ %br, false );
   %cml.addText( "Everyone that contributed and is not listed" @ %br @ %br, false );
   %cml.addText( %br @ %br, false );
   
   //
   // GarageGames
   //   
   %cml.addText( %headerFontBoldStyle @ "GarageGames" @ %br ,false);
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "Erik Graham" @ %br, false );
   %cml.addText( "Justin Head" @ %br, false );
   %cml.addText( "Eric Preisz" @ %br, false );
   %cml.addText( "Geoff Beckstrom" @ %br, false );
   %cml.addText( "Phill Belanger" @ %br, false );
   %cml.addText( "Michael Blenden" @ %br, false );
   %cml.addText( "Dennis Booth" @ %br, false );
   %cml.addText( "Derek Bronson" @ %br, false );
   %cml.addText( "Scott Burns" @ %br, false );
   %cml.addText( "Dexter Chow" @ %br, false );
   %cml.addText( "James Dickinson" @ %br, false );
   %cml.addText( "Cassondra Forbes" @ %br, false );
   %cml.addText( "Zach Gambino" @ %br, false );
   %cml.addText( "Kyle Miller" @ %br, false );
   %cml.addText( "David Montgomery-Blake" @ %br, false );
   %cml.addText( "Jesse Oliger" @ %br, false );
   %cml.addText( "Matt Ostgard" @ %br, false );
   %cml.addText( "Michael \"Mich\" Perry" @ %br, false );
   %cml.addText( "Doug Poston" @ %br, false );
   %cml.addText( "Richard Ranft" @ %br, false );
   %cml.addText( "Dmitry Shtainer" @ %br, false );
   %cml.addText( "John Shuman" @ %br, false );
   %cml.addText( "Ben Steffen" @ %br, false );
   %cml.addText( "Joseph Thomas" @ %br, false );
   %cml.addText( "Jon Williams" @ %br, false );
   %cml.addText( "Matthew Wood" @ %br, false );
   %cml.addText( %br @ %br, false );
   
   //
   // Torque Game Builder
   //   
   %cml.addText( %headerFontBoldStyle @ "Original Torque Game Builder Team" @ %br ,false);   
   %cml.addText( %headerFontStyle, false );
   %cml.addText( "Justin DuJardin" @ %br, false );
   %cml.addText( "Adam Larson" @ %br, false );
   %cml.addText( "Matt Langley" @ %br, false );
   %cml.addText( "Paul Scott" @ %br, false );
   %cml.addText( "Tom Bampton" @ %br, false );
   %cml.addText( "Josh Williams" @ %br, false );
   %cml.addText( "Melvyn May" @ %br, false );
   %cml.addText( "Ben Garney" @ %br, false );
   %cml.addText( "Robert Blanchet Jr." @ %br, false );
   %cml.addText( "Joe Maruschak" @ %br, false );
   %cml.addText( "Thomas Eastman" @ %br, false );
   %cml.addText( "Alex Swanson" @ %br, false );
   %cml.addText( "Nate Feyma" @ %br, false );
   %cml.addText( "Mark McCoy" @ %br, false );
   %cml.addText( "Thomas Buscaglia" @ %br, false );
   %cml.addText( "Dan Maruschak" @ %br, false );
   %cml.addText( "Ivan 'Spider' DelSol" @ %br, false );
   %cml.addText( "Ken Holst" @ %br, false );
   %cml.addText( "Eric Fritz" @ %br, false );
   
   %cml.addText( %pageBreak, false );

   // Flow the Credits
   %cml.forceReflow();
}

