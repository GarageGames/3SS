//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// PREFERENCESMANAGER.H
//
// Allows for the creation and management of user preferences.
//

#include "editor/preferencesManager.h"

PreferencesManager* PreferencesManager::gPreferencesManager = 0;

//**********************************************************************************
//*** CUserActionManager Implementation
//**********************************************************************************

IMPLEMENT_CONOBJECT( PreferencesManager );

PreferencesManager::PreferencesManager() : mPrefsHash(64, false)
{
   mCurrentGroup = NULL;
}

PreferencesManager::~PreferencesManager()
{
   mCurrentGroup = NULL;

   mPrefsHash.clearTables();

   // [tom, 9/19/2006] Hash table will free the prefs

   //    S32 count = mPreferences.size();
   //    for(S32 i=0; i<count; ++i)
   //    {
   //       if(mPreferences[i])
   //          delete mPreferences[i];
   //    }
   mPreferences.clear();
}

//*** Add a preference to the list.  The given TGE global variable should start with '$'.
void PreferencesManager::addPref(const char* name, const char* desc, const char* type, const char* var, const char* defaultval, S32 storageType /* = stString */, S32 selector /* = -1 */, const char *label /* = NULL */)
{
   //Con::errorf("*** Adding preference %s", name);

   Preference* pref = new Preference();

   //*** Build up the preference
   pref->mID = -1;
   pref->mIsGroup = false;

   pref->mName = StringTable->insert(name);
   pref->mDesc = StringTable->insert(desc, true);
   pref->mType = StringTable->insert(type, true);
   pref->mCtrlLabel = label ? StringTable->insert(label, true) : NULL;

   pref->mSelector = mCurrentGroup && selector == -1 ? mCurrentGroup->mSelector : selector;
   
   pref->mSetupCtrlCB = NULL;

   //*** By default do not broadcast a change to this preference.
   pref->mBroadcastChange = false;

   //*** Give the preference the appropriate storage type
   pref->mStorageType = storageType;

   //*** Before adding the TGE global variable to the string table, check if it
   //*** is already there.  This is used to determine if the default value needs
   //*** to be set for it.
   const char* varcheck = StringTable->lookup(var);
   pref->mVariable = StringTable->insert(var);

   //*** Add it to the list
   mPreferences.push_back(pref);

   pref->mID = mPreferences.size() - 1;
   mPrefsHash.insert(pref, pref->mName);

   //*** If the TGE global variable doesn't already exist, the create it with the default value
   //Con::errorf("*** Looking up pref %s variable %s",name,var);
   const char* value;
   if(!varcheck)
   {
      Con::setVariable(pref->mVariable, defaultval);
      value = defaultval;
      //Con::errorf("*** Giving pref %s (%s) a default value of %s", name, var, defaultval);

   } else
   {
      //*** It does exist, so obtain its value for possible storage in our type-specific area
      value = Con::getVariable(pref->mVariable);
   }

   //*** If neccessary, convert the value to the correct type
   S32 r,g,b,a;
   F32 fr,fg,fb,fa;
   switch(storageType)
   {
      case stF32:
         pref->mValueF32 = dAtof(value);
         break;

      case stS32:
         pref->mValueS32 = dAtoi(value);
         break;

      case stBool:
         pref->mValueBool = dAtob(value);
         break;

      case stColorI:
         a=255;
         dSscanf(value, "%d %d %d %d",&r,&g,&b,&a);
         pref->mValueColorI = ColorI(r,g,b,a);
         pref->mValueColorF = ColorF(pref->mValueColorI);
         break;

      case stColorF:
         fa=1.f;
         dSscanf(value, "%g %g %g %g",&fr,&fg,&fb,&fa);
         pref->mValueColorF = ColorF(fr,fg,fb,fa);
         pref->mValueColorI = ColorI(pref->mValueColorF);
         break;
   }

}

//*** Prefs Groups
void PreferencesManager::addPrefGroup(const char *name, const char *desc, S32 selector /* = -1 */)
{
   if(mCurrentGroup)
   {
      Con::errorf("Attempting to add group \"%s\": Nesting preference groups not supported", name);
      return;
   }

   Preference* pref = new Preference();

   //*** Build up the preference
   pref->mID = -1;
   pref->mIsGroup = true;

   pref->mName = StringTable->insert(name);
   pref->mDesc = StringTable->insert(desc, true);
   pref->mCtrlLabel = pref->mDesc;
   pref->mType = NULL;

   pref->mSelector = selector;
   pref->mSetupCtrlCB = NULL;

   //*** By default do not broadcast a change to this preference.
   pref->mBroadcastChange = false;

   //*** Give the preference the appropriate storage type
   pref->mStorageType = -1;

   pref->mVariable = NULL;

   //*** Add it to the list
   mPreferences.push_back(pref);

   pref->mID = mPreferences.size() - 1;
   mPrefsHash.insert(pref, pref->mName);

   mCurrentGroup = pref;
}

void PreferencesManager::endPrefGroup()
{
   if(mCurrentGroup == NULL)
   {
      Con::errorf("Attempting to end group: No group has been begun");
      return;
   }

   Preference* pref = new Preference();

   //*** Build up the preference
   pref->mID = -1;
   pref->mIsGroup = true;

   // [tom, 9/19/2006] We must have a unique name to add it to the hash table
   char nameBuf[512];
   dSprintf(nameBuf, sizeof(nameBuf), "%s_EndGroup", mCurrentGroup->mName);
   pref->mName = StringTable->insert(nameBuf);
   pref->mDesc = NULL;
   pref->mCtrlLabel = NULL;
   pref->mType = NULL;
   pref->mSetupCtrlCB = NULL;

   //*** By default do not broadcast a change to this preference.
   pref->mBroadcastChange = false;

   //*** Give the preference the appropriate storage type
   pref->mStorageType = -1;

   pref->mVariable = NULL;

   //*** Add it to the list
   mPreferences.push_back(pref);

   pref->mID = mPreferences.size() - 1;
   mPrefsHash.insert(pref, pref->mName);

   mCurrentGroup = NULL;
}

//*** Get a preference's broadcast state
bool PreferencesManager::getPrefBroadcastState(const char* name)
{
   //*** Find the preference
   Preference *pref = mPrefsHash.retrieve(name);
   if(pref && ! pref->mIsGroup)
      return pref->mBroadcastChange;

   return false;
}

//*** Set a preference's broadcast state
void PreferencesManager::setPrefBroadcastState(const char* name, bool state)
{
   //*** Find the preference
   Preference *pref = mPrefsHash.retrieve(name);
   if(pref && ! pref->mIsGroup)
      pref->mBroadcastChange = state;
}

//*** Get a preference's control setup callback
const char *PreferencesManager::getPrefSetupCtrlCB(const char *name)
{
   Preference *pref = mPrefsHash.retrieve(name);
   if(pref && pref->mSetupCtrlCB)
      return pref->mSetupCtrlCB;

   return "";
}

//*** Set a preference's control setup callback
void PreferencesManager::setPrefSetupCtrlCB(const char *name, const char *cb)
{
   Preference *pref = mPrefsHash.retrieve(name);
   if(pref && ! pref->mIsGroup)
      pref->mSetupCtrlCB = StringTable->insert(cb);
}

//*** Get a preference's control label
const char *PreferencesManager::getPrefCtrlLabel(const char *name)
{
   Preference *pref = mPrefsHash.retrieve(name);
   if(pref && pref->mCtrlLabel)
      return pref->mCtrlLabel;

   return "";
}

//*** Set a preference's control label
void PreferencesManager::setPrefCtrlLabel(const char *name, const char *label)
{
   Preference *pref = mPrefsHash.retrieve(name);
   if(pref && ! pref->mIsGroup)
      pref->mCtrlLabel = StringTable->insert(label);
}


//*** Get the value of a preference
const char* PreferencesManager::getPref(const char* name, const char* defaultChar)
{
   //*** Find the preference
   Preference *pref = mPrefsHash.retrieve(name);
   if(pref && ! pref->mIsGroup)
      return Con::getVariable(pref->mVariable);

   return defaultChar;
}

//*** Set the value of a preference
void PreferencesManager::setPref(const char* name, const char* value)
{
   //*** Find the preference
   Preference *pref = mPrefsHash.retrieve(name);
   if(pref && ! pref->mIsGroup)
   {
      //*** Set the value according to the storage type, if necessary.
      S32 r,g,b,a;
      F32 fr,fg,fb,fa;
      switch(pref->mStorageType)
      {
         case stF32:
            pref->mValueF32 = dAtof(value);
            break;

         case stS32:
            pref->mValueS32 = dAtoi(value);
            break;

         case stBool:
            pref->mValueBool = dAtob(value);
            break;

         case stColorI:
            a=255;
            dSscanf(value, "%d %d %d %d",&r,&g,&b,&a);
            pref->mValueColorI = ColorI(r,g,b,a);
            pref->mValueColorF = ColorF(pref->mValueColorI);
            break;

         case stColorF:
            fa=1.f;
            dSscanf(value, "%g %g %g %g",&fr,&fg,&fb,&fa);
            pref->mValueColorF = ColorF(fr,fg,fb,fa);
            pref->mValueColorI = ColorI(pref->mValueColorF);
            break;
      }

      //*** Always set the script variable regardless of storage
      //*** type to keep it in sync.
      Con::setVariable(pref->mVariable, value);

      //*** Do we broadcast our change to the script message router?
      if(pref->mBroadcastChange)
      {
         Con::executef(this, 2, "broadcastPrefChange", pref->mName);
      }
   }
}

//*** Get the value of a preference as a F32
F32 PreferencesManager::getPrefF32(const char* name, F32 defaultF32)
{
   //*** Find the preference
   Preference *pref = mPrefsHash.retrieve(name);
   if(pref && ! pref->mIsGroup)
   {
      //*** If this preference is defined as a F32 storage type, then
      //*** return the appropriate value.  Otherwise return using the
      //*** console function which performs a string conversion.
      if(pref->mStorageType == stF32)
      {
         return pref->mValueF32;

      } else
      {
         return Con::getFloatVariable(pref->mVariable);
      }
   }

   return defaultF32;
}

//*** Get the value of a preference as a S32
S32 PreferencesManager::getPrefS32(const char* name, S32 defaultS32)
{
   //*** Find the preference
   Preference *pref = mPrefsHash.retrieve(name);
   if(pref && ! pref->mIsGroup)
   {
      //*** If this preference is defined as a S32 storage type, then
      //*** return the appropriate value.  Otherwise return using the
      //*** console function which performs a string conversion.
      if(pref->mStorageType == stS32)
      {
         return pref->mValueS32;

      } else
      {
         return Con::getIntVariable(pref->mVariable);
      }
   }

   return defaultS32;
}

//*** Get the value of a preference as a bool
bool PreferencesManager::getPrefBool(const char* name, bool defaultBool)
{
   //*** Find the preference
   Preference *pref = mPrefsHash.retrieve(name);
   if(pref && ! pref->mIsGroup)
   {
      //*** If this preference is defined as a Bool storage type, then
      //*** return the appropriate value.  Otherwise return using the
      //*** console function which performs a string conversion.
      if(pref->mStorageType == stBool)
      {
         return pref->mValueBool;

      } else
      {
         return Con::getBoolVariable(pref->mVariable);
      }
   }

   return defaultBool;
}

//*** Get the value of a preference as a ColorI
ColorI PreferencesManager::getPrefColorI(const char* name, ColorI defaultColor)
{
   //*** Find the preference
   Preference *pref = mPrefsHash.retrieve(name);
   if(pref && ! pref->mIsGroup)
   {
      //*** If this preference is defined as a ColorI storage type, then
      //*** return the appropriate value.  This also works if the storage type
      //*** is a ColorF
      if(pref->mStorageType == stColorI || pref->mStorageType == stColorF)
      {
         return pref->mValueColorI;

      }
   }

   return defaultColor;
}

//*** Get the value of a preference as a ColorF
ColorF PreferencesManager::getPrefColorF(const char* name, ColorF defaultColor)
{
   //*** Find the preference
   Preference *pref = mPrefsHash.retrieve(name);
   if(pref && ! pref->mIsGroup)
   {
      //*** If this preference is defined as a ColorF storage type, then
      //*** return the appropriate value.  This also works if the storage type
      //*** is a ColorI
      if(pref->mStorageType == stColorF || pref->mStorageType == stColorI)
      {
         return pref->mValueColorF;

      }
   }

   return defaultColor;
}

//**********************************************************************************
//*** Enumeration
//**********************************************************************************

void PreferencesManager::doEnumCallback(const char *callback, const char *cbData, Preference *pref)
{
   Con::executef(10, callback, 
                    pref->mName,
                    pref->mDesc ? pref->mDesc : "",
                    Con::getIntArg(pref->mIsGroup),
                    pref->mType ? pref->mType : "",
                    Con::getIntArg(pref->mBroadcastChange),
                    cbData,
                    pref->mCtrlLabel ? pref->mCtrlLabel : "",
                    pref->mSetupCtrlCB ? pref->mSetupCtrlCB : "",
                    Con::getIntArg(pref->mSelector));
}

bool PreferencesManager::enumeratePrefGroup(const char *name, S32 selector, const char *callback, const char *cbData)
{
   Preference *group = mPrefsHash.retrieve(name);
   if(group == NULL || !group->mIsGroup || group->mID < 0 || group->mID >= mPreferences.size()-1)
      return false;

   for(S32 i = group->mID + 1;i < mPreferences.size() && mPreferences[i]->mDesc != NULL;i++)
   {
      Preference *pref = mPreferences[i];
      if(selector == -1 || pref->mSelector == selector)
         doEnumCallback(callback, cbData, pref);
   }

   return true;
}

bool PreferencesManager::enumeratePrefs(S32 selector, const char *callback, const char *cbData)
{
   for(S32 i = 0;i < mPreferences.size();i++)
   {
      Preference *pref = mPreferences[i];
      if(selector == -1 || pref->mSelector == selector)
         doEnumCallback(callback, cbData, pref);
   }

   return true;
}

//**********************************************************************************
//*** Console Functions
//**********************************************************************************

//*** Register the Preferences Manager
ConsoleFunction(registerPreferencesManager, bool, 2, 2, "registerPreferencesManager(SimObjectID) - register the script created Preferences Manager with C++ code.")
{
   if(PreferencesManager::gPreferencesManager)
   {
      Con::errorf("registerPreferencesManager(): Preferences Manager already registered!");
      return false;
   }

   PreferencesManager::gPreferencesManager = dynamic_cast<PreferencesManager*>(Sim::findObject(argv[1]));
   if(PreferencesManager::gPreferencesManager)
   {
      return true;
   }
   else
   {
      Con::errorf("registerPreferencesManagerManager(): Could not register Preferences Manager");
      return false;
   }
}

//**********************************************************************************
//*** Console Methods -- Add, Get, Set
//**********************************************************************************

//*** Add a preference to the list
ConsoleMethod(PreferencesManager, addPref, void, 7, 9, "addPref(prefname, description, type, tgevariable, defaultvalue[, selector][, label]) - Add a preference to the list.")
{
   S32 selector = argc > 7 ? dAtoi(argv[7]) : -1;
   object->addPref(argv[2], argv[3], argv[4], argv[5], argv[6], object->stString, selector, argc > 8 ? argv[8] : NULL);
}

ConsoleMethod(PreferencesManager, addPrefGroup, void, 4, 5, "addPrefGroup(groupname, description[, selector]) - Add a preference group to the list.")
{
   S32 selector = argc > 4 ? dAtoi(argv[4]) : -1;
   object->addPrefGroup(argv[2], argv[3], selector);
}

ConsoleMethod(PreferencesManager, endPrefGroup, void, 2, 2, "endPrefGroup() - End the current preference group.")
{
   object->endPrefGroup();
}

//**********************************************************************************

//*** Add a preference to the list as a F32
ConsoleMethod(PreferencesManager, addPrefF32, void, 7, 9, "addPrefF32(prefname, description, type, tgevariable, defaultvalue[, selector][, label]) - Add a preference to the list as a F32.")
{
   S32 selector = argc > 7 ? dAtoi(argv[7]) : -1;
   object->addPref(argv[2], argv[3], argv[4], argv[5], argv[6], object->stF32, selector, argc > 8 ? argv[8] : NULL);
}

//*** Add a preference to the list as a S32
ConsoleMethod(PreferencesManager, addPrefS32, void, 7, 9, "addPrefS32(prefname, description, type, tgevariable, defaultvalue[, selector][, label]) - Add a preference to the list as a S32.")
{
   S32 selector = argc > 7 ? dAtoi(argv[7]) : -1;
   object->addPref(argv[2], argv[3], argv[4], argv[5], argv[6], object->stS32, selector, argc > 8 ? argv[8] : NULL);
}

//*** Add a preference to the list as a Bool
ConsoleMethod(PreferencesManager, addPrefBool, void, 7, 9, "addPrefBool(prefname, description, type, tgevariable, defaultvalue[, selector][, label]) - Add a preference to the list as a Bool.")
{
   S32 selector = argc > 7 ? dAtoi(argv[7]) : -1;
   object->addPref(argv[2], argv[3], argv[4], argv[5], argv[6], object->stBool, selector, argc > 8 ? argv[8] : NULL);
}

//*** Add a preference to the list as a ColorI
ConsoleMethod(PreferencesManager, addPrefColorI, void, 7, 9, "addPrefColorI(prefname, description, type, tgevariable, defaultvalue[, selector][, label]) - Add a preference to the list as a ColorI.")
{
   S32 selector = argc > 7 ? dAtoi(argv[7]) : -1;
   object->addPref(argv[2], argv[3], argv[4], argv[5], argv[6], object->stColorI, selector, argc > 8 ? argv[8] : NULL);
}

//*** Add a preference to the list as a ColorF
ConsoleMethod(PreferencesManager, addPrefColorF, void, 7, 9, "addPrefColorF(prefname, description, type, tgevariable, defaultvalue[, selector][, label]) - Add a preference to the list as a ColorF.")
{
   S32 selector = argc > 7 ? dAtoi(argv[7]) : -1;
   object->addPref(argv[2], argv[3], argv[4], argv[5], argv[6], object->stColorF, selector, argc > 8 ? argv[8] : NULL);
}

//**********************************************************************************

//*** Get a preference's value
ConsoleMethod(PreferencesManager, get, const char*, 3, 3, "get(prefname) - Get a preference's value.")
{
   return object->getPref(argv[2]);
}

//*** Get a preference's value as a float
ConsoleMethod(PreferencesManager, getFloat, F32, 3, 3, "getFloat(prefname) - Get a preference's value as a floating point number.")
{
   return object->getPrefF32(argv[2]);
}

//*** Get a preference's value as an int
ConsoleMethod(PreferencesManager, getInt, S32, 3, 3, "getInt(prefname) - Get a preference's value as an integer.")
{
   return object->getPrefS32(argv[2]);
}

//*** Get a preference's value as a bool
ConsoleMethod(PreferencesManager, getBool, bool, 3, 3, "getBool(prefname) - Get a preference's value as a boolean value.")
{
   return object->getPrefBool(argv[2]);
}

//*** Get a preference's value as a ColorI
ConsoleMethod(PreferencesManager, getColorI, const char*, 3, 3, "getColorI(prefname) - Get a preference's value as a ColorI.")
{
   ColorI col = object->getPrefColorI(argv[2]);

   char* returnBuffer = Con::getReturnBuffer(32);
   dSprintf(returnBuffer,sizeof(32),"%d %d %d",col.red,col.green,col.blue);

   return returnBuffer;
}

//*** Get a preference's value as a ColorF
ConsoleMethod(PreferencesManager, getColorF, const char*, 3, 3, "getColorF(prefname) - Get a preference's value as a ColorF.")
{
   ColorF col = object->getPrefColorF(argv[2]);

   char* returnBuffer = Con::getReturnBuffer(32);
   dSprintf(returnBuffer,sizeof(32),"%g %g %g",col.red,col.green,col.blue);

   return returnBuffer;
}

//*** Set a preference's value
ConsoleMethod(PreferencesManager, set, void,  4, 4, "set(prefname, value) - Set a preference's value.")
{
   object->setPref(argv[2], argv[3]);
}

//**********************************************************************************
//*** Console Methods -- Script Message Router Broadcasting
//**********************************************************************************

//*** Get a preference's broadcast state
ConsoleMethod(PreferencesManager, getPrefBroadcastState, bool, 3, 3, "getPrefBroadcastState(prefname) - Get a preference's broadcast state.")
{
   return object->getPrefBroadcastState(argv[2]);
}

//*** Set a preference's broadcast state
ConsoleMethod(PreferencesManager, setPrefBroadcastState, void, 4, 4, "setPrefBroadcastState(prefname, bool state) - Set a preference's broadcast state.")
{
   return object->setPrefBroadcastState(argv[2], dAtob(argv[3]));
}

//*** Get a preference's control setup callback
ConsoleMethod(PreferencesManager, getPrefSetupCtrlCB, bool, 3, 3, "getPrefSetupCtrlCB(prefname) - Get a preference's control setup callback.")
{
   return object->getPrefSetupCtrlCB(argv[2]);
}

//*** Set a preference's control setup callback
ConsoleMethod(PreferencesManager, setPrefSetupCtrlCB, void, 4, 4, "setPrefSetupCtrlCB(prefname, callback) - Set a preference's control setup callback.")
{
   return object->setPrefSetupCtrlCB(argv[2], argv[3]);
}

//*** Get a preference's control setup label
ConsoleMethod(PreferencesManager, getPrefCtrlLabel, bool, 3, 3, "getPrefCtrlLabel(prefname) - Get a preference's control label.")
{
   return object->getPrefCtrlLabel(argv[2]);
}

//*** Set a preference's control setup label
ConsoleMethod(PreferencesManager, setPrefCtrlLabel, void, 4, 4, "setPrefCtrlLabel(prefname, label) - Set a preference's control label.")
{
   return object->setPrefCtrlLabel(argv[2], argv[3]);
}

//**********************************************************************************
//*** Console Methods -- Enumeration
//**********************************************************************************

ConsoleMethod(PreferencesManager, enumeratePrefGroup, bool, 6, 6, "enumeratePrefGroup(group, selector, callback, callbackData) - Enumerate a preference group")
{
   return object->enumeratePrefGroup(argv[2], dAtoi(argv[3]), argv[4], argv[5]);
}

ConsoleMethod(PreferencesManager, enumeratePrefs, bool, 5, 5, "enumeratePrefs(selector, callback, callbackData) - Enumerate all preferences")
{
   return object->enumeratePrefs(dAtoi(argv[2]), argv[3], argv[4]);
}
