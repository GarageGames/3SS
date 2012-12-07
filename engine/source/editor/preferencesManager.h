//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// PREFERENCESMANAGER.H
//
// Allows for the creation and management of user preferences.
//

#ifndef _PREFERENCESMANAGER_H_
#define _PREFERENCESMANAGER_H_

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif
#ifndef _COLOR_H_
#include "graphics/color.h"
#endif

#include "collection/simpleHashTable.h"

class PreferencesManager : public SimObject
{
   typedef SimObject Parent;

public:
   enum StorageType {
      stString = 0,
      stF32,
      stS32,
      stBool,
      stColorI,
      stColorF,
   };

   struct Preference {
      S32 mID;                         // Hack: ID for fast lookup through hash table

      bool mIsGroup;                   // True if this is a group

      StringTableEntry mName;	         // The name of the preference as used by the get/set methods
      StringTableEntry mDesc;	         // A user friendly description

      StringTableEntry mType;	         // The dynamic GUI control type of the preference
      StringTableEntry mVariable;	   // The name of the TGE global variable that actually stores the preference.  Usually it begins with "$pref::Constructor::" so that it persists, but doesn't have to.

      bool   mBroadcastChange;	      // Should a change in this preference be broadcast through the script messaging system
      S32 mSelector;                   // Used to limit what prefs are returned in an enumeration

      StringTableEntry mSetupCtrlCB;   // Script callback to setup a dynamic control for this pref
      StringTableEntry mCtrlLabel;     // Label for dynamic control

      //*** Optimized storage for C++ access
      S32    mStorageType;
      F32    mValueF32;
      S32    mValueS32;
      bool   mValueBool;
      ColorI mValueColorI;
      ColorF mValueColorF;
   };

   // [tom, 9/19/2006] I'm keeping the preferences vector to allow guaranteed order
   // enumeration of the prefs values for automatic GUI generation. This wouldn't be
   // so easy with the hash table alone.
   VectorPtr<Preference*> mPreferences;
   SimpleHashTable<Preference> mPrefsHash;

   // [tom, 9/19/2006] Current group if one is begun, otherwise NULL
   Preference *mCurrentGroup;

   void doEnumCallback(const char *callback, const char *cbData, Preference *pref);

public:
   static PreferencesManager* gPreferencesManager;

   DECLARE_CONOBJECT(PreferencesManager);

   PreferencesManager();
   virtual ~PreferencesManager();

   //*** Add a preference to the list
   void addPref(const char* name, const char* desc, const char* type, const char* var, const char* defaultval, S32 storageType = stString, S32 selector = -1, const char *label = NULL);

   void addPrefGroup(const char *name, const char *desc, S32 selector = -1);
   void endPrefGroup();

   //*** Set the preference's broadcast state
   bool getPrefBroadcastState(const char* name);
   void setPrefBroadcastState(const char* name, bool state);

   void setPrefSetupCtrlCB(const char *name, const char *cb);
   const char *getPrefSetupCtrlCB(const char *name);

   void setPrefCtrlLabel(const char *name, const char *label);
   const char *getPrefCtrlLabel(const char *name);

   //*** Get and set a preference
   const char* getPref(const char* name, const char* defaultChar="");
   void setPref(const char* name, const char* value);

   //*** Specialized get functions
   F32 getPrefF32(const char* name, F32 defaultF32=0.f);
   S32 getPrefS32(const char* name, S32 defaultS32=0);
   bool getPrefBool(const char* name, bool defaultBool=false);
   ColorI getPrefColorI(const char* name, ColorI defaultColor=ColorI(0,0,0,255));
   ColorF getPrefColorF(const char* name, ColorF defaultColor=ColorF(0.f,0.f,0.f,1.f));

   //*** Enumeration
   bool enumeratePrefGroup(const char *name, S32 selector, const char *callback, const char *cbData);
   bool enumeratePrefs(S32 selector, const char *callback, const char *cbData);
};

#endif // _PREFERENCESMANAGER_H_
