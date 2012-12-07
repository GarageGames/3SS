//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _IHTTPCALLBACK_H
#define _IHTTPCALLBACK_H

class IHttpCallback
{
public:
    virtual void onRequestErrorCallback(S32 id, const char* errorText) = 0;
    virtual void onRequestFinishedCallback(S32 id, S32 responseCode, const char* response) = 0;
};

#endif // _IHTTPCALLBACK_H
