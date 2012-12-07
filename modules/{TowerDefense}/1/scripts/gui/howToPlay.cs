$CurrentHowToPlayScreen = 1;

/// <summary>
/// This function initializes the help screen GUI.
/// </summary>
function howToPlayGui::onWake(%this)
{
    if (%this.numHowToPlayScreens <= 1)
        helpNextButton.Visible = false;
    
    helpPreviousButton.Visible = false;

    howToPlayBackground.Image = %this.howToPlayScreen1;
}

/// <summary>
/// This function handles the help screen close button.
/// </summary>
function helpCloseButton::onClick(%this)
{
   $CurrentHowToPlayScreen = 1;
   
   howToPlayBackground.Image = howToPlayGui.howToPlayScreen1;
   
   if (helpPreviousButton.Visible) 
      helpPreviousButton.Visible = false;
      
   if (!helpNextButton.Visible) 
      helpNextButton.Visible = true;
   
   Canvas.popDialog(howToPlayGui);
}

/// <summary>
/// This function handles the help screen next button - it advances the background
/// image to the next in the series and handles displaying the back button if needed.
/// </summary>
function helpNextButton::onClick(%this)
{
   $CurrentHowToPlayScreen++;
   
   if ($CurrentHowToPlayScreen >= howToPlayGui.numHowToPlayScreens)
      helpNextButton.Visible = false;
   
   if (!helpPreviousButton.Visible) 
      helpPreviousButton.Visible = true; 
      
      
   switch$ (howToPlayBackground.Image)
   {
      case howToPlayGui.howToPlayScreen1:
         howToPlayBackground.Image = howToPlayGui.howToPlayScreen2;
      
      case howToPlayGui.howToPlayScreen2:
         howToPlayBackground.Image = howToPlayGui.howToPlayScreen3;
      
      case howToPlayGui.howToPlayScreen3:
         howToPlayBackground.Image = howToPlayGui.howToPlayScreen4;
      
      case howToPlayGui.howToPlayScreen4:
         howToPlayBackground.Image = howToPlayGui.howToPlayScreen5;
      
      case howToPlayGui.howToPlayScreen5:
         howToPlayBackground.Image = howToPlayGui.howToPlayScreen6;
   }
   //echo(" -- NextButton - " @ $CurrentHowToPlayScreen @ " : " @ howToPlayBackground.bitmap);
}

/// <summary>
/// This function handles the help screen back button - it sets the background 
/// to the previous image in the series and handles updating the next button if needed.
/// </summary>
function helpPreviousButton::onClick(%this)
{
   $CurrentHowToPlayScreen--;
   
   if ($CurrentHowToPlayScreen <= 1)
      helpPreviousButton.Visible = false;   
   
   if (!helpNextButton.Visible) 
      helpNextButton.Visible = true;
      
   switch$ (howToPlayBackground.Image)
   {
      case howToPlayGui.howToPlayScreen6:
         howToPlayBackground.Image = howToPlayGui.howToPlayScreen5;
      
      case howToPlayGui.howToPlayScreen5:
         howToPlayBackground.Image = howToPlayGui.howToPlayScreen4;
      
      case howToPlayGui.howToPlayScreen4:
         howToPlayBackground.Image = howToPlayGui.howToPlayScreen3;
      
      case howToPlayGui.howToPlayScreen3:
         howToPlayBackground.Image = howToPlayGui.howToPlayScreen2;
      
      case howToPlayGui.howToPlayScreen2:
         howToPlayBackground.Image = howToPlayGui.howToPlayScreen1;
   }
   //echo(" -- PreviousButton - " @ $CurrentHowToPlayScreen @ " : " @ howToPlayBackground.bitmap);
}