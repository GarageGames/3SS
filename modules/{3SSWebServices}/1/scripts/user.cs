//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function TestCheckUserLoggedIn()
{
    // Kicks off the process of checking if the user is already logged in.
    // If the user is not already logged in then start that process.
    // Normally this event would be posted after 3SS starts up.
    EditorEventManager.schedule(0, postEvent, "_StartUserLogInCheck");
}

function TestUserLogOut()
{
    // Kicks off the process of logging the user out.  Normally this event
    // would be posted by a button press or some other means to indicate the
    // user wants to log out.
    EditorEventManager.schedule(0, postEvent, "_StartUserLogOut");
}

//-----------------------------------------------------------------------------

// Called when 3SS first starts up to check if the user is already logged in
function UserServices::onStartUserLogInCheckEvent(%this, %messageData)
{
    if(!UserServices.IsUserLoggedIn())
    {
        // Allow the GUI to update
        EditorEventManager.schedule(0, postEvent, "_ContactingLoginServer");
        
        // Start checking if the user is already logged in by calling
        // the C++ console method.
        UserServices.schedule(0, "StartUserLogInCheck");
    }
}

// Called when the user is trying to log in
function UserServices::onStartUserLogInEvent(%this, %messageData)
{
    if(UserServices.IsUserLoggedIn())
    {
        // User is already logged in.  We should not be here.
        return;
    }
    
    // Allow the GUI to update
    EditorEventManager.schedule(0, postEvent, "_ContactingLoginServer");
    
    // Start to log in the user by calling the C++ console method.
    // The password is in the messageData.
    %this.schedule(0, "LogInUser", %messageData);
}

// Called when the user should be logged out
function UserServices::onStartUserLogOutEvent(%this, %messageData)
{
    Projects::GetEventManager().schedule(0, postEvent, "_ProjectClose");
    
    ModuleDatabase.unloadGroup(projectTools);
    
    // Log out the user by calling the C++ console method.
    %this.LogOutUser();
}

// Called when the user is attempting to go into offline mode
function UserServices::onStartUserOfflineModeEvent(%this, %messageData)
{
    // Attempt to go in offline mode by calling the C++ console method.
    %this.schedule(0, "OfflineUser");
}

// Called when the user is attempting to see their profile through the editor
function UserServices::onStartUserGetInfo(%this, %messageData)
{
    // Call an internal function that will post to a private URL
    // and attempt to get the user's public profile information
    %this.schedule(0, "GetUserInfo");
}

// Called when the system has grabbed the user's profile information
function UserServices::onUserInfoAcquired(%this, %messageData)
{
    // Get the XML document that contains the profile data
    %profile = UserServices.GetUserProfileSimObjectId();
    
    %user = %profile.pushFirstChildElement("User");

    %details = %profile.pushFirstChildElement("Details");
    
    %profile.pushFirstChildElement("FirstName");
    %firstName = %profile.getData();
    %profile.popElement();

    %profile.pushFirstChildElement("LastName");
    %lastName = %profile.getData();
    %profile.popElement();
    
    $WebServices::UserFullName = %firstName SPC %lastName;
    
    %profile.pushFirstChildElement("AvatarImage");
    $WebServices::UserAvatarLink = %profile.getData();
    %profile.popElement();
    
    %extension = fileExt($WebServices::UserAvatarLink);
    
    %baseImagePath = "^PicturesFileLocation/" @ $WebServices::UserFullName @ "/userImage" @ %extension;
    $WebServices::ProfileImagePath = expandPath(%baseImagePath);
    
    %downloadResult = DownloadQueue.AddFileToQueue($WebServices::UserAvatarLink, $WebServices::ProfileImagePath, EditorListener, true);
    DownloadQueue.DownloadNextFile();
     
    %profile.pushFirstChildElement("Bio");
    $WebServices::UserProfileInfo = %profile.getData();
    %profile.popElement();
    
    %profile.popElement();
    
    %profile.pushChildElement("Contact");
    
    %profile.pushChildElement("Email");
    $WebServices::UserEmail = %profile.getData();
    %profile.popElement();

    %profile.popElement();
    %profile.popElement();
}

//-----------------------------------------------------------------------------

// C++ callback when credentials are required from the user before we
// can log in.
function UserServices::onUserCredentialsRequired(%this, %errorMessage)
{
    // We need to request credentials from the user
    // If %errorMessage contains text then there was a problem with the last
    // log in attempt, such as an incorrect password.
    EditorEventManager.schedule(0, postEvent, "_UserRequiresLogIn", %errorMessage);
}

// C++ callback when the user is logged in, either off line or on line.
function UserServices::onUserLoggedIn(%this)
{
    // The user has logged in so allow the GUI to update
    EditorEventManager.schedule(0, postEvent, "_UserLoggedIn");
}

// C++ callback when there is an error contacting the log in server and it is
// possible for the user to run in an offline mode.
function UserServices::onRequestOfflineMode(%this)
{
    // Inform the GUI that the user has a choice of running in offline mode
    EditorEventManager.schedule(0, postEvent, "_StartUserRequestOfflineMode");
}
