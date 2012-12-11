//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

if(!isObject(GuiLoginTextProfile)) new GuiControlProfile (GuiLoginTextProfile)
{
   fontColor = "0 0 0";
   
   fontType = "Arial Bold";
   fontSize = 16;
};

//-----------------------------------------------------------------------------

// Display the connecting to server screen.
function OpenConnectingToServerScreen()
{
    if(Canvas.getContent() != LoginScreenGui.getId())
        Canvas.setContent(ConnectingToServerScreenGui);
}

//-----------------------------------------------------------------------------

// Display the log in screen.  If %errorMessage is not empty then present
// the error to the user as well.
function OpenLogInScreen(%errorMessage)
{
    /**********************************************************Temporary Hack to Skip Login for Usability Testing*********************************************************/
    EditorEventManager.schedule(0, postEvent, "_UserProfileAcquired");
    
    
    // Set up the log in screen
    if (isFile($WebServices::ProfileImagePath) && $WebServices::RememberUserName)
    {
        if($WebServicess::AvatarAsset $= "")
        {
            %avatarAsset = new ImageAsset()
            {
                AssetInternal = true;
                ImageFile = $WebServices::ProfileImagePath;
            };
        
            $WebServicess::AvatarAsset = AssetDatabase.addPrivateAsset(%avatarAsset);
        }
        LoginScreenGuiWindow-->LoginScreenGuiAvatar.Image = $WebServicess::AvatarAsset;
    }
    
    LoginScreenGuiWindow-->UsernameTextEdit.setText($WebServices::UserName);
    LoginScreenGuiWindow-->PasswordTextEdit.setText("");
    LoginScreenGuiWindow-->AutoLogInCheckBox.setValue($WebServices::AutoLogIn);
    LoginScreenGuiWindow-->RememberUserCheckBox.setValue($WebServices::RememberUserName);
    LoginScreenGuiWindow-->LoggingInText.setVisible(false);
    LoginScreenGuiWindow-->ConnectingToServerArea.setVisible(false);
    LoginScreenGuiWindow-->LoginButton.setVisible(true);
    LoginScreenGuiWindow-->PasswordTextEdit.setActive(true);
    LoginScreenGuiWindow-->UsernameTextEdit.setActive(true);
    
    Canvas.setContent(LoginScreenGui);
    
    LoginScreenGuiWindow-->UsernameTextEdit.setFirstResponder();
    
    // Display any error message
    if(%errorMessage $= "Invalid user name or password.")
    {
        exportWebServicesPrefs();
        LoginScreenGuiWindow-->InvalidMessageText.setVisible(true);
        LoginScreenGuiWindow-->NameAsterix.setVisible(true);
        LoginScreenGuiWindow-->PassAsterix.setVisible(true);
        return;
    }
    else
    {
        LoginScreenGuiWindow-->InvalidMessageText.setVisible(false);
        LoginScreenGuiWindow-->NameAsterix.setVisible(false);
        LoginScreenGuiWindow-->PassAsterix.setVisible(false);
    }
    
    if (%errorMessage !$= "")
    {
        MessageBoxOK("Login Error", %errorMessage, "");
    }
}

//-----------------------------------------------------------------------------

// Ask the user if they would like to go offline
function OpenOfflineModeRequest()
{
    MessageBoxYesNo("Offline Mode", "Cannot conact the 3 Step Studio server.  Would you like to go into offline mode?", "EditorEventManager.schedule(0, postEvent, \"_StartUserOfflineMode\");", "EditorEventManager.schedule(0, postEvent, \"_StartUserLogOut\");");
}

//-----------------------------------------------------------------------------

function LoginScreenGuiWindow::onCreateNewAccount(%this)
{
    // If we are running in debug, this button will skip the login-process
    if (isDebugBuild())
    {
        EditorEventManager.schedule(0, postEvent, "_UserProfileAcquired");
    }
    else
    {
        Canvas.pushDialog(WebWindowGui);
    }
}

function LoginScreenGuiWindow::onUsernameText(%this)
{
    $WebServices::UserName = LoginScreenGuiWindow-->UsernameTextEdit.getText();
}

function LoginScreenGuiWindow::onAutoLogIn(%this)
{
    %state = LoginScreenGuiWindow-->AutoLogInCheckBox.getValue();
    
    $WebServices::AutoLogIn = %state;
}

function LoginScreenGuiWindow::onRememberUserName(%this)
{
    %state = LoginScreenGuiWindow-->RememberUserCheckBox.getValue();
    
    $WebServices::RememberUserName = %state;
}

function LoginScreenGuiWindow::onLogInButton(%this)
{
    LoginScreenGuiWindow-->InvalidMessageText.setVisible(false);
    LoginScreenGuiWindow-->NameAsterix.setVisible(false);
    LoginScreenGuiWindow-->PassAsterix.setVisible(false);
    LoginScreenGuiWindow-->LoggingInText.setVisible(true);
    LoginScreenGuiWindow-->LoginButton.setVisible(false);
    LoginScreenGuiWindow-->PasswordTextEdit.setActive(false);
    LoginScreenGuiWindow-->UsernameTextEdit.setActive(false);
    
    $WebServices::UserName = LoginScreenGuiWindow-->UsernameTextEdit.getText();
    %password = LoginScreenGuiWindow-->PasswordTextEdit.getText();
    LoginScreenGuiWindow-->PasswordTextEdit.setText("");

    exportWebServicesPrefs();
    $WebServices::UserName = LoginScreenGuiWindow-->UsernameTextEdit.getText();
    
    EditorEventManager.schedule(0, postEvent, "_StartUserLogIn", %password);
}

//-----------------------------------------------------------------------------

function LoginScreenGuiWindowUsername::onTabComplete(%this)
{
    LoginScreenGuiWindowPassword.setFirstResponder();
}

function LoginScreenGuiWindowUsername::onReturn(%this)
{
    if (LoginScreenGuiWindowPassword.getText() !$= "")
        LoginScreenGuiWindow.onLogInButton();
    else
        LoginScreenGuiWindowPassword.setFirstResponder();
}

function LoginScreenGuiWindowPassword::onTabComplete(%this)
{
    LoginScreenGuiWindowUsername.setFirstResponder();
}

function LoginScreenGuiWindowPassword::onReturn(%this)
{
    if (LoginScreenGuiWindowUsername.getText() !$= "")
        LoginScreenGuiWindow.onLogInButton();
    else
        LoginScreenGuiWindowUsername.setFirstResponder();
}
