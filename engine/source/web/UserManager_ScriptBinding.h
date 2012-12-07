//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod(UserManager, IsUserLoggedIn, bool, 2, 2,  "() - Is the user currently logged in.\n"
                                                        "@return True if the user is currently logged in.\n")
{
    return object->IsUserLoggedIn();
}

//-----------------------------------------------------------------------------

ConsoleMethod(UserManager, IsUserOffline, bool, 2, 2,   "() - Is the user operating in offline mode.\n"
                                                        "@return True if the user is operating in offline mode.\n")
{
    return object->IsUserOffline();
}

//-----------------------------------------------------------------------------

ConsoleMethod(UserManager, CanAccessModule, bool, 3, 3, "(moduleId) - Does the user have permission to access the requested module.\n"
                                                        "@moduleId The moduleId to check permissions for.\n"
                                                        "@return True if the user has permission to access the requested module.\n")
{
    return object->CanAccessModule(argv[2]);
}

//-----------------------------------------------------------------------------

ConsoleMethod(UserManager, GetModuleAccessList, const char*, 2, 2,    "() - Provides a list of modules the user may access.\n"
                                                                "@return Space delimited list of modules.\n")
{
    // Get the list of modules
    Vector<StringTableEntry> moduleList;
    object->GetModuleAccessList(moduleList);
    const S32 moduleListSize = moduleList.size();

    // Calculate the size of the required buffer
    U32 bufferSize = 0;
    for(S32 i=0; i<moduleListSize; ++i)
    {
        bufferSize += dStrlen(moduleList[i]) + 1;
    }

    // Build out the list for the console
    char* buffer = Con::getReturnBuffer(bufferSize);
    buffer[0] = '\0';
    for(S32 i=0; i<moduleListSize; ++i)
    {
            dStrcat(buffer, moduleList[i]);
            dStrcat(buffer, " ");
    }

    // Terminate the buffer
    buffer[bufferSize-1] = '\0';

    return buffer;
}

//-----------------------------------------------------------------------------

ConsoleMethod(UserManager, StartUserLogInCheck, void, 2, 2,  "() - Start checking if the user is already logged in.\n")
{
    object->StartUserLogInCheck();
}

//-----------------------------------------------------------------------------

ConsoleMethod(UserManager, LogInUser, void, 3, 3,   "(password) - Attempt to log in the user.\n"
                                                    "@password The password to use.\n")
{
    object->LogInUser(argv[2]);
}

//-----------------------------------------------------------------------------

ConsoleMethod(UserManager, LogOutUser, void, 2, 2,  "() - Log out the current user.\n")
{
    object->LogOutUser();
}

//-----------------------------------------------------------------------------

ConsoleMethod(UserManager, OfflineUser, void, 2, 2,  "() - Make the user offline.\n")
{
    object->OfflineUser();
}
//-----------------------------------------------------------------------------

ConsoleMethod(UserManager, Update, void, 2, 2,  "() - Update the state machine.\n")
{
    object->Update();
}

//-----------------------------------------------------------------------------

ConsoleMethod(UserManager, GetManifestSimObjectId, S32, 2, 2,  "() - Get the SimObjectId of the internal module manifest.\n")
{
    return object->GetManifestSimObjectId();
}

//-----------------------------------------------------------------------------

ConsoleMethod(UserManager, GetUserInfo, void, 2, 2, "() - Attempt to get the user's profile information.\n")
{
	object->GetUserInfo();
}

//-----------------------------------------------------------------------------

ConsoleMethod(UserManager, GetUserProfileSimObjectId, S32, 2, 2, "() - Get the SimObjectId of the internal user info manifest.\n")
{
	return object->GetUseProfileSmObjectId();
}

//-----------------------------------------------------------------------------

ConsoleMethod(UserManager, SetHTTPManager, void, 3, 3,  "(httpManager) - Set the HTTP manager to use for all requests.\n"
                                                        "@httpManager The HTTPManager to use.\n")
{
    HTTPManager* manager = dynamic_cast<HTTPManager*>(Sim::findObject(argv[2]));

    object->SetHTTPManager(manager);
}

//-----------------------------------------------------------------------------

ConsoleFunction(getTSSWebAddress, const char*,  1, 1, "() - Gets the 3 Step Studio web address\n"
													  "Used for logging in, acquiring profiles, and downloading.\n"
													  "This will point to the live site when TORQUE_SHIPPING is defined")
{
	return TSS_WEB_ACCESS;
}