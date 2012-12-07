
function creditsGui::onWake(%this)
{
   %this.readCreditsFile($Save::Interface::creditsTextFile);
}

function creditsGui::readCreditsFile(%this, %textFile)
{
   %file = new FileObject();

   %check = %file.openForRead(%textFile);
   
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

function closeCreditsButton::onClick(%this)
{
   alxPlay($Save::GeneralSettings::Sound::ButtonClick);
   Canvas.popDialog(creditsGui);
}