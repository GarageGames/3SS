//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2014
//-----------------------------------------------------------------------------

// Open the test WebViewGui.  This does the Qt initialization as we don't
// want it to normally happen during this testing.
function WVG()
{
    // Start up the QtManager if needed
    if (!isObject(QtPlatformManager))
    {
        echo("@@@ Starting QtManager");
        new QtManager(QtPlatformManager);
    }
    
    // Load in the test web view if needed
    if (!isObject(WebViewGui))
    {
        echo("@@@ Loading test web view");
        TamlRead("^{3SSEditor}/gui/webview.gui.taml");
    }
    
    // Set the URL to load
    WV_WebView.setURL("http://devpro.gginteractive.com/");
    
    Canvas.pushDialog(WebViewGui);
}

function WV_CloseButton::onClick(%this)
{
    Canvas.popDialog(WebViewGui);
}

function WV_RefreshButton::onClick(%this)
{
    WV_WebView.reloadAction();
}
