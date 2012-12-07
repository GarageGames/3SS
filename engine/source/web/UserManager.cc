//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "web/UserManager.h"
#include "web/HttpManager.h"
#include "console/consoleTypes.h"
#include "io/bitStream.h"
#include "algorithm/hashFunction.h"

#include "web/UserManager_ScriptBinding.h"

#define HTTP_USER_NOT_LOGGED_IN "User not logged in."
#define HTTP_INVALID_USER_CREDENTIALS1 "Invalid login or password."
#define HTTP_INVALID_USER_CREDENTIALS2 ""

#define MANIFEST_FILE_PATH "^CacheFileLocation/3SSData/m.dat"
#define USER_PROFILE_FILE_PATH "^CacheFileLocation/3SSData/u.dat"

IMPLEMENT_CONOBJECT( UserManager );

UserManager::UserManager()
{
    m_UserState = UserState_Startup;

    m_UserName = StringTable->EmptyString;

    m_AutoLoginURL = StringTable->EmptyString;
    m_CredentialsLoginURL = StringTable->EmptyString;
    m_UserInfoURL = StringTable->EmptyString;
    m_PostUsernameKey = StringTable->EmptyString;
    m_PostPasswordKey = StringTable->EmptyString;

    m_pHTTPManager = NULL;
    m_CurrentHTTPRequestId = -1;

    m_pXMLManifest = NULL;
    m_pUserXMLProfile = NULL;
    m_ManifestUserName = StringTable->EmptyString;
}

UserManager::~UserManager()
{
}

//-----------------------------------------------------------------------------

void UserManager::initPersistFields()
{
   Parent::initPersistFields();

   addField("autoLoginURL",         TypeCaseString,       Offset(m_AutoLoginURL, UserManager), "");
   addField("credentialsLoginURL",  TypeCaseString,       Offset(m_CredentialsLoginURL, UserManager), "");
   addField("userInfoURL",			TypeCaseString,		  Offset(m_UserInfoURL, UserManager), "");
   addField("postUsernameKey",      TypeCaseString,       Offset(m_PostUsernameKey, UserManager), "");
   addField("postPasswordKey",      TypeCaseString,       Offset(m_PostPasswordKey, UserManager), "");
}

//-----------------------------------------------------------------------------

bool UserManager::onAdd()
{
    if( !Parent::onAdd() )
        return false;

    m_XMLDocument.registerObject();

    m_UserXMLDocument.registerObject();

    return true;
}

void UserManager::onRemove()
{
    if(m_pXMLManifest)
        delete[] m_pXMLManifest;

    if(m_pUserXMLProfile)
        delete[] m_pUserXMLProfile;

    m_XMLDocument.unregisterObject();

    m_UserXMLDocument.unregisterObject();

    // Call parent.
    Parent::onRemove();
}

//-----------------------------------------------------------------------------

bool UserManager::IsUserLoggedIn()
{
    return m_UserState == UserState_UserLoggedIn;
}

bool UserManager::IsUserOffline()
{
    return m_UserState == UserState_UserOfflineMode;
}

//-----------------------------------------------------------------------------

bool UserManager::CanAccessModule(const char* moduleId)
{
    if(m_UserState != UserState_UserLoggedIn && m_UserState != UserState_UserOfflineMode)
    {
        // Cannot use any modules if the user is not in the correct state
        return false;
    }

    // Is the module in the list?
    ModuleList::iterator module = m_ModuleList.find(StringTable->insert(moduleId, true));
    if(module.getValue())
        return true;

    // By default, no module may be used
    return false;
}

void UserManager::GetModuleAccessList(Vector<StringTableEntry>& list)
{
    for(ModuleList::iterator itr = m_ModuleList.begin(); itr != m_ModuleList.end(); ++itr)
    {
        if(itr.getValue())
        {
            list.push_back(itr->key);
        }
    }
}

//-----------------------------------------------------------------------------

void UserManager::StartUserLogInCheck()
{
    if(m_UserState != UserState_Startup)
    {
        // In wrong state
        return;
    }

    // Clear the valid manifest list
    clearModuleList();

    bool autoLogin = Con::getBoolVariable("$WebServices::AutoLogIn", false);
    if(autoLogin)
    {
        if(m_pHTTPManager)
        {
            // Move on to checking if the user is already logged in
            m_UserState = UserState_CheckUserLoggedIn;
            IHttpServicesProvider::PostDictionary* parameters = new IHttpServicesProvider::PostDictionary();
            m_CurrentHTTPRequestId = m_pHTTPManager->addPostRequest(m_AutoLoginURL, parameters, getId());
        }
        else
        {
            Con::errorf("Cannot log in as no HTTP manager has been defined");
        }
    }
    else
    {
        // Move on to asking the user to log in
        m_UserState = UserState_GetUserCredentials;
        Con::executef(this, 1, "onUserCredentialsRequired");
    }
}

//-----------------------------------------------------------------------------

void UserManager::LogInUser(const char* password)
{
    if(m_UserState != UserState_GetUserCredentials)
    {
        // In wrong state
        return;
    }

    const char* username = Con::getVariable("$WebServices::UserName");
    m_UserName = StringTable->insert(username, true);

    if(m_pHTTPManager)
    {
        // Move on to logging in the user
        m_UserState = UserState_AttemptLogIn;
        IHttpServicesProvider::PostDictionary* parameters = new IHttpServicesProvider::PostDictionary();

        parameters->insertEqual(m_PostUsernameKey, m_UserName);
        parameters->insertEqual(m_PostPasswordKey, StringTable->insert(password, true));

        m_CurrentHTTPRequestId = m_pHTTPManager->addPostRequest(m_CredentialsLoginURL, parameters, getId());
    }
    else
    {
        Con::errorf("Cannot log in as no HTTP manager has been defined");
    }
}

//-----------------------------------------------------------------------------

void UserManager::LogOutUser(const char* message)
{
    clearModuleList();
    deleteModuleManifest();
    deleteUserProfile();

    // After a log out the only valid state is to get user credentials
    m_UserState = UserState_GetUserCredentials;
    Con::executef(this, 2, "onUserCredentialsRequired", message);
}

//-----------------------------------------------------------------------------

void UserManager::OfflineUser()
{
    // Can only go offline if currently in the offline request mode
    if(m_UserState == UserState_RequestOfflineMode)
    {
        // Make sure we can still go offline
        if(checkCanGoOffline())
        {
            // Precoess the manifest that was read in checkCanGoOffline()
            processXMLManifest();

            // Process the user profile that was read in checkCanGoOffline()
            processUserXMLProfile();

            // Log in the user
            m_UserState = UserState_UserLoggedIn;
            Con::executef(this, 1, "onUserLoggedIn");
        }
    }

    // Cannot handle the error so log out
    LogOutUser("Cannot go offline at this time.  Please try to log in again.");
}

//-----------------------------------------------------------------------------

void UserManager::GetUserInfo()
{
    // Do not proceed if the user is not logged in
    if(m_UserState != UserState_UserLoggedIn)
    {
        Con::errorf("Cannot get user profile because they are not logged in");
        return;
    }

    // Make sure the HTTP Manager is valid
    if(m_pHTTPManager)
    {
        // Move on to logging in the user
        IHttpServicesProvider::PostDictionary* parameters = new IHttpServicesProvider::PostDictionary();

        m_UserState = UserState_GettingUserInfo;

        m_CurrentHTTPRequestId = m_pHTTPManager->addPostRequest(m_UserInfoURL, parameters, getId());
    }
    else
    {
        Con::errorf("Cannot get user info because no HTTP manager has been defined");
    }
}
//-----------------------------------------------------------------------------

void UserManager::Update()
{
    switch(m_UserState)
    {
        case UserState_Startup:
            {
                // Clear the valid manifest list
                clearModuleList();

                bool autoLogin = Con::getBoolVariable("$WebServices::AutoLogIn", false);
                if(autoLogin)
                {
                    if(m_pHTTPManager)
                    {
                        // Move on to checking if the user is already logged in
                        m_UserState = UserState_CheckUserLoggedIn;
                        IHttpServicesProvider::PostDictionary* parameters = new IHttpServicesProvider::PostDictionary();
                        m_CurrentHTTPRequestId = m_pHTTPManager->addPostRequest(m_AutoLoginURL, parameters, getId());
                    }
                    else
                    {
                        Con::errorf("Cannot log in as no HTTP manager has been defined");
                    }
                }
                else
                {
                    // Move on to asking the user to log in
                    m_UserState = UserState_GetUserCredentials;
                    Con::executef(this, 1, "onUserCredentialsRequired");
                }
            }
            break;
    }
}

//-----------------------------------------------------------------------------

void UserManager::onRequestErrorCallback(S32 id, const char* errorText)
{
    if(id != m_CurrentHTTPRequestId)
    {
        // Don't handle a request we're not ready for
        return;
    }

    Con::printf("UserManager::onRequestErrorCallback(): %s", errorText);

    if(m_UserState == UserState_CheckUserLoggedIn)
    {
        // Cannot contact the server while attempting to log in.  Check if offline mode
        // is an option.
        if(checkCanGoOffline())
        {
            // This user may go offline.
            m_UserState = UserState_RequestOfflineMode;
            Con::executef(this, 1, "onRequestOfflineMode");
            return;
        }
    }

    // Cannot handle the error so log out
    LogOutUser(errorText);
}

void UserManager::onRequestFinishedCallback(S32 id, S32 responseCode, const char* response)
{
    if(id != m_CurrentHTTPRequestId)
    {
        // Don't handle a request we're not ready for
        return;
    }

    Con::printf("UserManager::onRequestFinishedCallback(): %d", responseCode);

    switch(m_UserState)
    if(m_UserState == UserState_CheckUserLoggedIn)
    {
        case UserState_CheckUserLoggedIn:
            {
                // Check if the user is not currently logged in
                if(dStricmp(response, HTTP_USER_NOT_LOGGED_IN) == 0)
                {
                    // User is not logged in so get credentials
                    LogOutUser();
                }
                else
                {
                    // User is logged in.  Make sure the module manifest is good.
                    bool result = checkManifest(response);
                    if(!result)
                    {
                        Con::errorf("Manifest obtained from web services is not usable.");
                        LogOutUser("Manifest received from server is invalid.  Please try again.");
                        return;
                    }

                    // The manifest is good.  Store it for our use.
                    storeXMLManifest(response);

                    // Save the manifest for later
                    writeModuleManifest();

#ifdef TORQUE_DEBUG
                    // In a debug build, write out the manifest as an XML file
                    char buffer[1024];
                    Platform::makeFullPathName("debug_manifest.xml", buffer, 1024);
                    FileStream fstream;
                    if( fstream.open(buffer, FileStream::Write) )
					{
						fstream.writeStringBuffer(response);
						fstream.close();
					}
#endif

                    // Process it
                    processXMLManifest();

                    m_UserState = UserState_UserLoggedIn;
                    Con::executef(this, 1, "onUserLoggedIn");
                }
            }
            break;

        case UserState_AttemptLogIn:
            {
                if(dStricmp(response, HTTP_INVALID_USER_CREDENTIALS1) == 0 || 
                   dStricmp(response, HTTP_INVALID_USER_CREDENTIALS2) == 0)
                {
                    // Attempted to log in with wrong user name or password
                    LogOutUser("Invalid user name or password.");
                }
                else
                {
                    // User is logged in.  Make sure the module manifest is good.
                    bool result = checkManifest(response);
                    if(!result)
                    {
                        Con::errorf("Manifest obtained from web services is not usable.");
                        LogOutUser("Manifest received from server is invalid.  Please try again.");
                        return;
                    }

                    // The manifest is good.  Store it for our use.
                    storeXMLManifest(response);

                    // Save the manifest for later
                    writeModuleManifest();

                    // Process it
                    processXMLManifest();

                    m_UserState = UserState_UserLoggedIn;
                    Con::executef(this, 1, "onUserLoggedIn");
                }
            }
            break;

        case UserState_GettingUserInfo:
            {
                // Check if the user is not currently logged in
                if(dStricmp(response, HTTP_USER_NOT_LOGGED_IN) == 0)
                {
                    // User is not logged in so get credentials
                    LogOutUser();
                }
                else
                {
                    // User is logged in.  Make sure the module manifest is good.
                    bool result = checkUserProfile(response);
                    if(!result)
                    {
                        Con::errorf("User info obtained from web services is not usable.");
                        //LogOutUser("User info received from server is invalid.  Please try again.");
                        return;
                    }

                    // The manifest is good.  Store it for our use.
                    storeUserXMLProfile(response);

                    // Save the manifest for later
                    writeUserProfile();

#ifdef TORQUE_DEBUG
                    // In a debug build, write out the manifest as an XML file
                    char buffer[1024];
                    Platform::makeFullPathName("debug_user_profile.xml", buffer, 1024);
                    FileStream fstream;
                    if( fstream.open(buffer, FileStream::Write) )
					{
						fstream.writeStringBuffer(response);
						fstream.close();
					}
#endif

                    // Process it
                    processUserXMLProfile();

                    m_UserState = UserState_UserLoggedIn;
                    Con::executef(this, 1, "onUserInfoAcquired");
                }
            }
            break;

    }
}

//-----------------------------------------------------------------------------

bool UserManager::checkCanGoOffline()
{
    // Load up any previously saved manifest
    bool result = readModuleManifest();
    if(result)
    {
        // A previous module manifest exists on the system.  Does the user name
        // match the current user?
        const char* username = Con::getVariable("$WebServices::UserName");
        if(dStrcmp(username, m_ManifestUserName) == 0)
        {
            // We have a user name match so offline mode is possible
            // Try to get the user profile before continuing
            result = readUserProfile();
            if(result)
            {
                Con::printf("Found user profile before going offline");
            }
            
            return true;
        }
    }

    return false;
}

//-----------------------------------------------------------------------------

bool UserManager::checkManifest(const char* manifest)
{
    m_XMLDocument.parse(manifest);

    // Make sure the header is correct
    if(!m_XMLDocument.pushFirstChildElement("Manifest"))
    {
        Con::errorf("Module manifest has wrong header type");
        return false;
    }

    if(!m_XMLDocument.attributeExists("Version"))
    {
        Con::errorf("Module manifest is missing version");
        return false;
    }

    const char* version = m_XMLDocument.attribute("Version");
    if(dAtoi(version) != 1)
    {
        Con::errorf("Module manifest is the wrong version");
        return false;
    }

    if(!m_XMLDocument.pushFirstChildElement("Product"))
    {
        Con::errorf("Module manifest is missing product information");
        return false;
    }

    if(!m_XMLDocument.pushFirstChildElement("Modules"))
    {
        Con::errorf("Module manifest is missing module list");
        return false;
    }

    return true;
}

//-----------------------------------------------------------------------------

void UserManager::storeXMLManifest(const char* xml)
{
    if(m_pXMLManifest)
    {
        delete[] m_pXMLManifest;
        m_pXMLManifest = NULL;
    }

    dsize_t manifestSize = dStrlen(xml);
    m_pXMLManifest = new char[manifestSize+1];
    dMemcpy(m_pXMLManifest, xml, manifestSize);
    m_pXMLManifest[manifestSize] = '\0';
}

//-----------------------------------------------------------------------------
bool UserManager::processXMLManifest()
{
    if(!m_pXMLManifest)
        return false;

    m_XMLDocument.parse(m_pXMLManifest);

    // Make sure the header is correct
    if(!m_XMLDocument.pushFirstChildElement("Manifest"))
    {
        Con::errorf("Module manifest has wrong header type");
        return false;
    }

    if(!m_XMLDocument.attributeExists("Version"))
    {
        Con::errorf("Module manifest is missing version");
        return false;
    }

    const char* version = m_XMLDocument.attribute("Version");
    if(dAtoi(version) != 1)
    {
        Con::errorf("Module manifest is the wrong version");
        return false;
    }

    if(!m_XMLDocument.pushFirstChildElement("Product"))
    {
        Con::errorf("Module manifest is missing product information");
        return false;
    }

    if(!m_XMLDocument.pushFirstChildElement("Modules"))
    {
        Con::errorf("Module manifest is missing module list");
        return false;
    }

    // Go through each module and build the list
    bool hasModule = m_XMLDocument.pushFirstChildElement("Module");
    while(hasModule)
    {
        // Get the module Id
        if(m_XMLDocument.attributeExists("Id"))
        {
            const char* moduleId = m_XMLDocument.attribute("Id");
            m_ModuleList.insert(StringTable->insert(moduleId, true), true);
        }

        hasModule = m_XMLDocument.nextSiblingElement("Module");
    }

    // Go back to the Modules level
    m_XMLDocument.popElement();

    return true;
}

//-----------------------------------------------------------------------------

bool UserManager::writeModuleManifest()
{
    if(!m_pXMLManifest)
    {
        Con::errorf("UserManager::writeModuleManifest(): Missing manifest");
        return false;
    }

    char fullPath[1024];
    bool result = Con::expandPath(fullPath, 1024, MANIFEST_FILE_PATH);
    if(!result)
    {
        Con::errorf("UserManager::writeModuleManifest(): Could not expand path '%s'", MANIFEST_FILE_PATH);
        return false;
    }

    // Make sure the path exists
    result = Platform::createPath(fullPath);
    if(!result)
    {
        Con::errorf("UserManager::writeModuleManifest(): Could not create path '%s'", MANIFEST_FILE_PATH);
        return false;
    }

    // Attempt to open the file for writing
    FileStream stream;
    result = stream.open(fullPath, FileStream::Write);
    if(!result)
    {
        Con::errorf("UserManager::writeModuleManifest(): Could not open file for writing '%s'", MANIFEST_FILE_PATH);
        return false;
    }

    // Begin the bit stream to save the manifest to, including a header
    InfiniteBitStream manifestStream;
    // Header to make sure the data doesn't start on a byte boundry
    manifestStream.writeFlag(true);
    manifestStream.writeFlag(true);
    manifestStream.writeFlag(false);
    // Version information
    manifestStream.writeInt(1, 6);
    manifestStream.writeInt(0, 6);

    // Write out the user name with this manifest
    U32 userLength = dStrlen(m_UserName);
    manifestStream.writeInt(userLength, 32);
    manifestStream._write(userLength, m_UserName);

    // Write out the XML manifest
    U32 xmlLength = dStrlen(m_pXMLManifest);
    manifestStream.writeInt(xmlLength, 32);
    manifestStream._write(xmlLength, m_pXMLManifest);

    // Build a hash for everything.  Start the hash off
    // with an arbitrary number
    U32 h = 1971;
    h = hash((U8*)m_UserName, userLength, h);
    h = hash((U8*)m_pXMLManifest, xmlLength, h);

    // Write out the hash
    manifestStream.writeInt(h, 32);

    // Write the bit stream to the file
    manifestStream.setPosition(0);
    stream.copyFrom(&manifestStream);

    // Close the file
    stream.close();

    return true;
}

bool UserManager::readModuleManifest()
{
    if(m_pXMLManifest)
        delete[] m_pXMLManifest;
    m_ManifestUserName = StringTable->EmptyString;

    char fullPath[1024];
    bool result = Con::expandPath(fullPath, 1024, MANIFEST_FILE_PATH);
    if(!result)
    {
        Con::errorf("UserManager::readModuleManifest(): Could not expand path '%s'", MANIFEST_FILE_PATH);
        return false;
    }

    // Attempt to open the file for reading.
    FileStream stream;
    result = stream.open(fullPath, FileStream::Read);
    if(!result)
    {
        return false;
    }

    // Load in the file to our bit stream
    InfiniteBitStream manifestStream;
    manifestStream.copyFrom(&stream);
    stream.close();

    // Make sure the header and version is correct
    manifestStream.setPosition(0);
    if(manifestStream.readFlag() != true)
    {
        return false;
    }
    if(manifestStream.readFlag() != true)
    {
        return false;
    }
    if(manifestStream.readFlag() != false)
    {
        return false;
    }

    S32 ver1 = manifestStream.readInt(6);
    S32 ver2 = manifestStream.readInt(6);
    if(ver1 != 1 || ver2 != 0)
    {
        return false;
    }

    // Read in the user name with this manifest
    const U32 nameSize = manifestStream.readInt(32);
    char* nameBuffer = new char[nameSize+1];
    manifestStream._read(nameSize, nameBuffer);
    nameBuffer[nameSize] = '\0';
    m_ManifestUserName = StringTable->insert(nameBuffer, true);
    delete[] nameBuffer;

    // Read in the XML manifest
    const U32 manifestSize = manifestStream.readInt(32);
    m_pXMLManifest = new char[manifestSize+1];
    manifestStream._read(manifestSize, m_pXMLManifest);
    m_pXMLManifest[manifestSize] = '\0';

    // Read in and check the hash
    U32 fileH = manifestStream.readInt(32);
    U32 h = 1971;
    h = hash((U8*)m_ManifestUserName, nameSize, h);
    h = hash((U8*)m_pXMLManifest, manifestSize, h);
    if(h != fileH)
    {
        // Hashes don't match up
        delete[] m_pXMLManifest;
        m_pXMLManifest = NULL;
        return false;
    }

    return true;
}

bool UserManager::deleteModuleManifest()
{
    char expandedPath[1024];
    bool result = Con::expandPath(expandedPath, 1024, MANIFEST_FILE_PATH);
    if(!result)
    {
        Con::errorf("UserManager::deleteModuleManifest(): Could not expand path '%s'", MANIFEST_FILE_PATH);
        return false;
    }

    char fullPath[1024];
    Platform::makeFullPathName(expandedPath, fullPath, sizeof(fullPath));

    // Delete the file
    return Platform::fileDelete(fullPath);
}

//-----------------------------------------------------------------------------

bool UserManager::readUserProfile()
{
    Con::errorf("UserManager::readUserProfile() not yet implemented");
    return false;
}

//-----------------------------------------------------------------------------

bool UserManager::writeUserProfile()
{
    Con::errorf("UserManager::writeUserProfile() not yet implemented");
    return false;
}

//-----------------------------------------------------------------------------

bool UserManager::deleteUserProfile()
{
    Con::errorf("UserManager::deleteUserProfile() not yet implemented");
    return false;
}

//-----------------------------------------------------------------------------

bool UserManager::checkUserProfile(const char* userManifest)
{
    m_UserXMLDocument.parse(userManifest);

    // Make sure the header is correct
    if(!m_UserXMLDocument.pushFirstChildElement("User"))
    {
        Con::errorf("User profile has wrong header type");
        return false;
    }

    // Make sure the user id attribute was included
    if(!m_UserXMLDocument.attributeExists("userId"))
    {
        Con::errorf("User profile ID is missing");
        return false;
    }

    // Check the user id for validity. Should be over 0
    const char* id = m_XMLDocument.attribute("userId");
    /*if(dAtoi(id) <= 0)
    {
        Con::errorf("User profile id is invalid");
        return false;
    }*/

    // Make sure the active attribute was included
    if(!m_UserXMLDocument.attributeExists("active"))
    {
        Con::errorf("User active status is missing");
        return false;
    }

    // If the user account is no longer active, do not proceed
    const char* userActive = m_XMLDocument.attribute("active");
    /*if(!dAtob(userActive))
    {
        Con::errorf("User account is no longer active");
        return false;
    }*/

    // Make sure the details exist
    if(!m_UserXMLDocument.pushFirstChildElement("Details"))
    {
        Con::errorf("User details are missing");
        return false;
    }

    // Go back to the user level
    m_UserXMLDocument.popElement();

    // Make sure the contact info exists
    if(!m_UserXMLDocument.pushFirstChildElement("Contact"))
    {
        Con::errorf("User contact information is missing");
        return false;
    }

    // Go back to the user level
    m_UserXMLDocument.popElement();

    return true;
}

//-----------------------------------------------------------------------------

void UserManager::storeUserXMLProfile(const char* userXml)
{
    if(m_pUserXMLProfile)
    {
        delete[] m_pUserXMLProfile;
        m_pUserXMLProfile = NULL;
    }

    dsize_t profileSize = dStrlen(userXml);
    m_pUserXMLProfile = new char[profileSize+1];
    dMemcpy(m_pUserXMLProfile, userXml, profileSize);
    m_pUserXMLProfile[profileSize] = '\0';
}

//-----------------------------------------------------------------------------

bool UserManager::processUserXMLProfile()
{
    if(!m_pUserXMLProfile)
        return false;

    m_UserXMLDocument.parse(m_pUserXMLProfile);

    // Make sure the header is correct
    if(!m_UserXMLDocument.pushFirstChildElement("User"))
    {
        Con::errorf("User profile has wrong header type");
        return false;
    }

    // Make sure the user id attribute was included
    if(!m_UserXMLDocument.attributeExists("userId"))
    {
        Con::errorf("User profile ID is missing");
        return false;
    }

    // Check the user id for validity. Should be over 0
    const char* id = m_XMLDocument.attribute("userId");
    if(dAtoi(id) <= 0)
    {
        Con::errorf("User profile id is invalid");
        return false;
    }

    // Make sure the active attribute was included
    if(!m_UserXMLDocument.attributeExists("active"))
    {
        Con::errorf("User active status is missing");
        return false;
    }

    // If the user account is no longer active, do not proceed
    const char* userActive = m_XMLDocument.attribute("active");
    if(!dAtob(userActive))
    {
        Con::errorf("User account is no longer active");
        return false;
    }

    // Make sure the details exist
    if(!m_UserXMLDocument.pushFirstChildElement("Details"))
    {
        Con::errorf("User details are missing");
        return false;
    }

    // Make sure there is a first name
    if(!m_UserXMLDocument.pushFirstChildElement("FirstName"))
    {
        Con::errorf("User info first name is missing");
        return false;
    }
    m_UserXMLDocument.popElement();

    // Make sure there is a last name
    if(!m_UserXMLDocument.pushFirstChildElement("LastName"))
    {
        Con::errorf("User info last name is missing");
        return false;
    }
    m_UserXMLDocument.popElement();

    // Make sure there is an avatar image
    if(!m_UserXMLDocument.pushFirstChildElement("Avatar"))
    {
        Con::errorf("User avatar link is missing");
        return false;
    }
    m_UserXMLDocument.popElement();

    // Make sure there is a bio
    if(!m_UserXMLDocument.pushFirstChildElement("Bio"))
    {
        Con::errorf("User bio is missing");
        return false;
    }
    m_UserXMLDocument.popElement();

    // Go back to the user level
    m_UserXMLDocument.popElement();

    // Make sure the contact info exists
    if(!m_UserXMLDocument.pushFirstChildElement("Contact"))
    {
        Con::errorf("User contact information is missing");
        return false;
    }

    if(!m_UserXMLDocument.pushFirstChildElement("Email"))
    {
        Con::errorf("User email is missing");
        return false;
    }
    m_UserXMLDocument.popElement();

    // Go back to the user level
    m_UserXMLDocument.popElement();

    return true;
}

//-----------------------------------------------------------------------------

void UserManager::clearModuleList()
{
    m_ModuleList.clear();
}
