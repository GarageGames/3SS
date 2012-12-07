//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//
//  iOSSocialNetworkingManager.h
//  Torque2D for iOS
//
//  Created by Ian Eborn
//  Copyright 2010 Luma Arcade. All rights reserved.
//

#include "console/console.h"

#if defined(_USE_SOCIAL_NETWORKING)

#if !defined(IOS_SOCIAL_NETWORKING_MANAGER)
#define IOS_SOCIAL_NETWORKING_MANAGER

#import "TGBAppDelegate.h"

class iOSSocialNetworkingManager
{
    public:
        iOSSocialNetworkingManager();
        
        ~iOSSocialNetworkingManager();

        virtual bool socialNetworkingShow(int view, bool allowUserSwitching);

        virtual bool socialNetworkingHide(bool animatedIfApplicable);

        virtual bool socialNetworkingShowProfilePicker();

        virtual bool socialNetworkingShowProfile();

        virtual bool socialNetworkingShowFriends();

        virtual bool socialNetworkingShowAwards();

        virtual bool socialNetworkingShowScores(const char* scoreBoardID);

        virtual bool socialNetworkingSubmitScore(S32 score, const char* metaData, const char* scoreBoardID);

        virtual bool socialNetworkingUnlockAward(const char* awardID);
    
        virtual bool socialNetworkingToResignActive();
    
        virtual bool socialNetworkingBecameActive();
    
        virtual bool socialNetworkingSetInterfaceOrientation(UIInterfaceOrientation newOrientation);
};

extern iOSSocialNetworkingManager* socialNetworkingManager;

extern bool socialNetworkingInit(TGBAppDelegate* appDelegate);

extern bool socialNetworkingCleanup();

void socialNetworkingDeleteManager();

#endif // IOS_SOCIAL_NETWORKING_MANAGER

#endif // _USE_SOCIAL_NETWORKING
