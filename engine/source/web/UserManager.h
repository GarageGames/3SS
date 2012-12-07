//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _USERMANAGER_H
#define _USERMANAGER_H

#include "sim/simBase.h"
#include "collection/hashTable.h"
#include "persistence/SimXMLDocument.h"
#include "web/IHttpCallback.h"

class HTTPManager;

class UserManager : public SimObject, public IHttpCallback
{
    typedef SimObject Parent;

public:
    enum UserStates
    {
        UserState_Startup,
        UserState_CheckUserLoggedIn,
        UserState_GetUserCredentials,
        UserState_AttemptLogIn,
        UserState_UserLoggedIn,
        UserState_RequestOfflineMode,
        UserState_UserOfflineMode,
		UserState_GettingUserInfo
    };

public:
    UserManager();
    virtual ~UserManager();

    // SimObject overrides
    virtual bool onAdd();
    virtual void onRemove();

    static void initPersistFields();

    // Is the user currently logged in
    bool IsUserLoggedIn();

    // Is the user operating under offline mode
    bool IsUserOffline();

    // Can the user access the requested module (DRM)
    bool CanAccessModule(const char* moduleId);

    // Get the full list of modules that the user may access.
    void GetModuleAccessList(Vector<StringTableEntry>& list);

    // Start checking if the user is already logged in
    void StartUserLogInCheck();

    // Log in the user explicitly
    void LogInUser(const char* password);

    // Log out the current user
    void LogOutUser(const char* message="");

    // Make the user offline
    void OfflineUser();

	// Get the user info
	void GetUserInfo();

    // Have the manager perform an update
    void Update();

	// Get the stored ID for the internal XML manifest parser
    S32 GetManifestSimObjectId() { return m_XMLDocument.getId(); }

	// Get the store ID for the internal usre XML profile parser
	S32 GetUseProfileSmObjectId() { return m_UserXMLDocument.getId(); }

    // IHttpCallback
    virtual void onRequestErrorCallback(S32 id, const char* errorText);
    virtual void onRequestFinishedCallback(S32 id, S32 responseCode, const char* response);

    void SetHTTPManager(HTTPManager* manager) { m_pHTTPManager = manager; }

    DECLARE_CONOBJECT(UserManager);

protected:
    // The current user state
    UserStates m_UserState;

    // The current user name
    StringTableEntry m_UserName;

    // The URL used when attempting auto login
    StringTableEntry m_AutoLoginURL;

    // The URL used when attempting to log in with credentials
    StringTableEntry m_CredentialsLoginURL;

	// The URL used when grabbing the user information from their web profile
	StringTableEntry m_UserInfoURL;

    // The HTTP Post key to use for the user name
    StringTableEntry m_PostUsernameKey;

    // The HTTP Post key to use for the password
    StringTableEntry m_PostPasswordKey;

    // The HTTPManager we use for our HTTP requests
    HTTPManager* m_pHTTPManager;

    // The Id of the current HTTP request
    S32 m_CurrentHTTPRequestId;

    // Used when parsing the XML manifest
    SimXMLDocument m_XMLDocument;

    // The module manifest in XML form
    char* m_pXMLManifest;

    // The user name as read in from the manifest
    StringTableEntry m_ManifestUserName;

	// Used when parsing the user XML profile
	SimXMLDocument m_UserXMLDocument;

	// The user profile in XML form
	char* m_pUserXMLProfile;

    // The list of modules available to this user
    typedef HashMap<StringTableEntry, bool> ModuleList;
    ModuleList m_ModuleList;

protected:
    // Check if the user can go offline
    bool checkCanGoOffline();

    // Save the module manifest
    bool writeModuleManifest();

    // Read in the module manifest
    bool readModuleManifest();

    // Delete the module manifest file
    bool deleteModuleManifest();

    // Check that the manifest is good
    bool checkManifest(const char* manifest);

    // Internally store a XML module manifest
    void storeXMLManifest(const char* xml);

    // Process the internal XML manifest
    bool processXMLManifest();

    // Clear the module list for the user
    void clearModuleList();

	// Read the user profile
	bool readUserProfile();

	// Save the user profile
	bool writeUserProfile();

	// Delete the user profile
	bool deleteUserProfile();

	// Check that the user profile is good
	bool checkUserProfile(const char* userProfile);

	// Internally store a user XML file
	void storeUserXMLProfile(const char* userXml);

	// Process the internal user XML file
	bool processUserXMLProfile();
};

#endif  // _USERMANAGER_H
