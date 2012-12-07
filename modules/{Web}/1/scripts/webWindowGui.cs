//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// This function displays a web page in the web view.  If the parameter is left
/// blank http://www.3stepstudio.com is set by default.
/// </summary>
/// <param name="url">The web page to display.</param>
function WebWindowGui::display(%this, %url)
{
    Canvas.pushDialog(%this);
    if (%url $= "")
        %url = "http://www.3stepstudio.com";
    Wwg_WebViewCtrl.setURL(%url);
}

function Wwg_CloseButton::onClick(%this)
{
    // set the URL back to default.
    Wwg_WebViewCtrl.setURL("http://www.3stepstudio.com");
    Canvas.popDialog(WebWindowGui);
}