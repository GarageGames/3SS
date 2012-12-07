//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function initializeWebStore(%scopeSet)
{
    //-----------------------------------------------------------------------------
    // Load scripts
    //-----------------------------------------------------------------------------
    exec("./scripts/profiles.cs");
    
    //-----------------------------------------------------------------------------
    // Load GUIs
    //-----------------------------------------------------------------------------
    %scopeSet.add( TamlRead("./gui/webStore.gui.taml") );
}

function destroyWebStore()
{
}

function WebStoreCtrl::onLoadStarted(%this)
{
   echo("WEB: Loading page '" @ %this.URL @ "'");
   
   WebStoreDlgURLField.setText(%this.URL);
   
   WebStoreLoadingCtrl.setVisible(true);
   WebStoreLoadingAnimation.display(WebPageLoadingAnimation);
}

function WebStoreCtrl::onFinishLoading(%this, %state)
{
   echo("WEB: Finished loading page '" @ %this.URL @ "' with status: " @ %state);
   WebStoreDlgURLField.setText(%this.URL);
   WebStoreLoadingCtrl.setVisible(false);
}

function WebStoreCtrl::onLinkClicked(%this, %url)
{
   echo("WEB: Linked clicked to '" @ %url @ "'");
   
   return true;
}

function WebStoreCtrl::onURLChanged(%this, %url)
{
   echo("WEB: URL changed to '" @ %url @ "'");
   WebStoreDlgURLField.setText(%url);
   
   return true;
}

function WebStoreCtrl::onConnectionError(%this, %error)
{
   echo("WEB: Connection error to '" @ %url @ "' with error: " @ %error);
}

//-----------------------------------------------------------------------------

function WebStoreDlgURLField::validateURL(%this)
{
   echo("WEB: WebStoreDlgURLField::validateURL(): " @ %this.getText());
   WebStoreCtrl.setURL(%this.getText());
}
