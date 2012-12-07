//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This function displays a full-screen confirmation dialog with custom message and 
/// callback functionality.
/// </summary>
/// <param name="message">The message to display to the user.</param>
function NoticeGui::display(%this, %message)
{
    NoticeMessageText.setText(%message);
    Canvas.pushDialog(%this);
}