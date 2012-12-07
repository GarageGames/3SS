//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//
//  iOSSocialNetworkingManager.mm
//  Torque2D for iOS
//
//  Created by Ian Eborn
//  Copyright 2010 Luma Arcade. All rights reserved.
//

#if defined(_USE_SOCIAL_NETWORKING)

#include "platformiOS/iOSSocialNetworkingManager.h"

iOSSocialNetworkingManager* socialNetworkingManager = NULL;

void socialNetworkingDeleteManager()
{
    if(socialNetworkingManager != NULL)
    {
        delete socialNetworkingManager;
        socialNetworkingManager = NULL;
    }
}

//////////////////// SocialNetworkingManager functions

iOSSocialNetworkingManager::iOSSocialNetworkingManager()
{
}

iOSSocialNetworkingManager::~iOSSocialNetworkingManager()
{
}

bool iOSSocialNetworkingManager::socialNetworkingShow(int view, bool allowUserSwitching)
{
    Con::printf("This is the base social networking manager - a more specific version (AGON, OpenFeint, Scoreloop, etc.) should probably be used instead.");
    return false;
}

bool iOSSocialNetworkingManager::socialNetworkingHide(bool animatedIfApplicable)
{
    Con::printf("This is the base social networking manager - a more specific version (AGON, OpenFeint, Scoreloop, etc.) should probably be used instead.");
    return false;
}

bool iOSSocialNetworkingManager::socialNetworkingShowProfilePicker()
{
    Con::printf("This is the base social networking manager - a more specific version (AGON, OpenFeint, Scoreloop, etc.) should probably be used instead.");
    return false;
}

bool iOSSocialNetworkingManager::socialNetworkingShowProfile()
{
    Con::printf("This is the base social networking manager - a more specific version (AGON, OpenFeint, Scoreloop, etc.) should probably be used instead.");
    return false;
}

bool iOSSocialNetworkingManager::socialNetworkingShowFriends()
{
    Con::printf("This is the base social networking manager - a more specific version (AGON, OpenFeint, Scoreloop, etc.) should probably be used instead.");
    return false;
}

bool iOSSocialNetworkingManager::socialNetworkingShowAwards()
{
    Con::printf("This is the base social networking manager - a more specific version (AGON, OpenFeint, Scoreloop, etc.) should probably be used instead.");
    return false;
}

bool iOSSocialNetworkingManager::socialNetworkingShowScores(const char* scoreBoardID)
{
    Con::printf("This is the base social networking manager - a more specific version (AGON, OpenFeint, Scoreloop, etc.) should probably be used instead.");
    return false;
}

bool iOSSocialNetworkingManager::socialNetworkingSubmitScore(S32 score, const char* metaData, const char* scoreBoardID)
{
    Con::printf("This is the base social networking manager - a more specific version (AGON, OpenFeint, Scoreloop, etc.) should probably be used instead.");
    return false;
}

bool iOSSocialNetworkingManager::socialNetworkingUnlockAward(const char* awardID)
{
    Con::printf("This is the base social networking manager - a more specific version (AGON, OpenFeint, Scoreloop, etc.) should probably be used instead.");
    return false;
}

bool iOSSocialNetworkingManager::socialNetworkingToResignActive()
{
    Con::printf("This is the base social networking manager - a more specific version (AGON, OpenFeint, Scoreloop, etc.) should probably be used instead.");
    return false;
}

bool iOSSocialNetworkingManager::socialNetworkingBecameActive()
{
    Con::printf("This is the base social networking manager - a more specific version (AGON, OpenFeint, Scoreloop, etc.) should probably be used instead.");
    return false;
}

bool iOSSocialNetworkingManager::socialNetworkingSetInterfaceOrientation(UIInterfaceOrientation newOrientation)
{
    Con::printf("This is the base social networking manager - a more specific version (AGON, OpenFeint, Scoreloop, etc.) should probably be used instead.");
    return false;
}


//////////////////// Console functions

ConsoleFunction(socialNetworkingShow, bool, 3, 3, "socialNetworkingShow( view, allowUserSwitching )")
{
    if(socialNetworkingManager != NULL)
    {
        return socialNetworkingManager->socialNetworkingShow(dAtoi(argv[1]), dAtob(argv[2]));
    }
    return false;
}

ConsoleFunction(socialNetworkingHide, bool, 1, 1, "socialNetworkingHide( animatedIfApplicable )")
{
    if(socialNetworkingManager != NULL)
    {
        return socialNetworkingManager->socialNetworkingHide(dAtob(argv[1]));
    }
    return false;
}

ConsoleFunction(socialNetworkingShowProfilePicker, bool, 1, 1, "socialNetworkingShowProfilePicker")
{
    if(socialNetworkingManager != NULL)
    {
        return socialNetworkingManager->socialNetworkingShowProfilePicker();
    }
    return false;
}

ConsoleFunction(socialNetworkingShowProfile, bool, 1, 1, "socialNetworkingShowProfile")
{
    if(socialNetworkingManager != NULL)
    {
        return socialNetworkingManager->socialNetworkingShowProfile();
    }
    return false;
}

ConsoleFunction(socialNetworkingShowFriends, bool, 1, 1, "socialNetworkingShowFriends")
{
    if(socialNetworkingManager != NULL)
    {
        return socialNetworkingManager->socialNetworkingShowFriends();
    }
    return false;
}

ConsoleFunction(socialNetworkingShowAwards, bool, 1, 1, "socialNetworkingShowAwards")
{
    if(socialNetworkingManager != NULL)
    {
        return socialNetworkingManager->socialNetworkingShowAwards();
    }
    return false;
}

ConsoleFunction(socialNetworkingShowScores, bool, 2, 2, "socialNetworkingShowScores( scoreBoardID )")
{
    if(socialNetworkingManager != NULL)
    {
        return socialNetworkingManager->socialNetworkingShowScores(argv[1]);
    }
    return false;
}

ConsoleFunction(socialNetworkingSubmitScore, bool, 4, 4, "socialNetworkingSubmitScore( score, metaData, scoreBoardID )")
{
    if(socialNetworkingManager != NULL)
    {
        return socialNetworkingManager->socialNetworkingSubmitScore(dAtoi(argv[1]), argv[2],  argv[3]);
    }
    return false;
}

ConsoleFunction(socialNetworkingUnlockAward, bool, 2, 2, "socialNetworkingUnlockAward( awardID )")
{
    if(socialNetworkingManager != NULL)
    {
        return socialNetworkingManager->socialNetworkingUnlockAward(argv[1]);
    }
    return false;
}

ConsoleFunction(socialNetworkingSetInterfaceOrientation, bool, 2, 2, "socialNetworkingSetInterfaceOrientation( newOrientation )")
{
    if(socialNetworkingManager != NULL)
    {
        return socialNetworkingManager->socialNetworkingSetInterfaceOrientation((UIInterfaceOrientation)dAtoi(argv[1]));
    }
    return false;
}

#endif // _USE_SOCIAL_NETWORKING
