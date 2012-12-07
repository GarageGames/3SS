//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function TestLogin()
{
    new ScriptObject(LoginTestObject)
    {
        stateStartUp = 0;
        stateCheckLogIn = 1;
        stateAttemptLogIn = 2;
        
        msgNotLoggedIn = "User not logged in.";
        msgInvalidCredentials = "";
        
        currentState = 0;
    };

    LoginTestObject.schedule(0, tick);
    
    //HttpQueue.addPostRequest("http://dev2.3stepstudio.com/client/login/post", "login[username]=davew@garagegames.com" TAB "login[password]=Garage1", LoginTestObject);
}

function LoginTestObject::tick(%this)
{
    switch(%this.currentState)
    {
        case %this.stateStartUp:
            // Check if we're already logged in
            HttpQueue.addPostRequest("http://dev2.3stepstudio.com/client", "", LoginTestObject);
            %this.currentState = %this.stateCheckLogIn;
    }
}

function LoginTestObject::onRequestError(%this, %manager, %error)
{
    switch(%this.currentState)
    {
        case %this.stateCheckLogIn:
            echo("LoginTestObject: ERR Check if logged in error: " @ %error);
            
        case %this.stateAttemptLogIn:
            echo("LoginTestObject: ERR Attempted to log in error: " @ %error);
    }
}

function LoginTestObject::onRequestFinished(%this, %manager, %response, %responseCode)
{
    switch(%this.currentState)
    {
        case %this.stateCheckLogIn:
            if(%response $= %this.msgNotLoggedIn)
            {
                echo("LoginTestObject: User is not logged in");
                
                // Attempt a log in with credentials
                HttpQueue.addPostRequest("http://dev2.3stepstudio.com/client/login/post", "login[username]=davew@garagegames.com" TAB "login[password]=Garage1", LoginTestObject);
                %this.currentState = %this.stateAttemptLogIn;
            }
            else
            {
                // User is alread logged in with the manifest in the response
                echo("LoginTestObject: User is already logged in: '" @ %response @ "'");
            }
            
        case %this.stateAttemptLogIn:
            if(%response $= %this.msgInvalidCredentials)
            {
                echo("LoginTestObject: Invalid credentials sent");
            }
            else
            {
                // The user has logged in with the manifest in the response
                echo("LoginTestObject: User is logged in with manifest: " @ %response);
            }
    }
}
