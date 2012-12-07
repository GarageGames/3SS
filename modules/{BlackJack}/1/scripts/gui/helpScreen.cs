//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
// Script for handling the helpScreen GUI
//-----------------------------------------------------------------------------

//$Save::Interface::HelpScreens[0] = "data/images/1blackjackbasicspage1.png";
//$Save::Interface::HelpScreens[1] = "data/images/2blackjacktermspage2.png";
//$Save::Interface::HelpScreens[2] = "data/images/3blackjackflowpage3.png";
//$Save::Interface::HelpScreens[3] = "data/images/4blackjackvariationspage4.png";
//$Save::Interface::HelpScreens[4] = "data/images/5blackjackstrategypage5.png";
//$Save::Interface::HelpScreens[5] = "data/images/6blackjackcountingpage6.png";

$CurrentHelpScreen = 0;
//$LastHelpScreen = 5;


function helpScreenCloseButton::onClick(%this)
{
   alxPlay($Save::GeneralSettings::Sound::ButtonClick);

   $CurrentHelpScreen = 0;
   
   HelpScreenBackground.bitmap = $Save::Interface::HelpScreens[$CurrentHelpScreen];
   
   if (helpScreenPreviousButton.Visible) 
      helpScreenPreviousButton.Visible = false;
      
   if (!helpScreenNextButton.Visible) 
      helpScreenNextButton.Visible = true;
   
   Canvas.popDialog(HelpScreenGui);
}

function helpScreenNextButton::onClick(%this)
{
   alxPlay($Save::GeneralSettings::Sound::ButtonClick);

   $CurrentHelpScreen++;
   
   if ($CurrentHelpScreen >= $Save::Interface::NumberOfHelpScreens - 1)
      helpScreenNextButton.Visible = false;
   
   if (!helpScreenPreviousButton.Visible) 
      helpScreenPreviousButton.Visible = true; 
      
   HelpScreenBackground.bitmap = $Save::Interface::HelpScreens[$CurrentHelpScreen];
}

function helpScreenPreviousButton::onClick(%this)
{
   alxPlay($Save::GeneralSettings::Sound::ButtonClick);

   $CurrentHelpScreen--;
   
   if ($CurrentHelpScreen == 0)
      helpScreenPreviousButton.Visible = false;   
   
   if (!helpScreenNextButton.Visible) 
      helpScreenNextButton.Visible = true;
      
   HelpScreenBackground.bitmap = $Save::Interface::HelpScreens[$CurrentHelpScreen];
}

