//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$CreditsInitialized = false;

/// <summary>
/// This function heats up the GUI when it is awakened.
/// </summary>
function creditsGui::onWake(%this)
{
   if (!$CreditsInitialized)
   {
      if (%this.useText)
      {
         CreditsMLText.Visible = true;
         CreditsGuiCreditsImage.Visible = false;
         %this.readCreditsFile();
      }
      else
      {
         CreditsMLText.Visible = false;
         CreditsGuiCreditsImage.Visible = true;
      }
      
      $CreditsInitialized = true;
   }
   
   if (%this.useText)
      creditsMLText.forceReflow();
}

/// <summary>
/// This function loads the credits text file.
/// </summary>
function creditsGui::readCreditsFile(%this)
{
   %file = new FileObject();
   // temporary hard-coded file name - will be superceded by the GUI Tool credits
   // screen selection.
   %check = %file.openForRead(creditsGui.creditsTxtFile);
   
   if (%check)
   {
      while (!%file.isEOF())
      {
         %textLine = %file.readLine() @ "<br>";
         creditsMLText.addText(%textLine, false);
      }
   }
   
   %file.close();
}

/// <summary>
/// This function handles the close button.
/// </summary>
function closeCreditsButton::onClick(%this)
{
   Canvas.popDialog(creditsGui);
}