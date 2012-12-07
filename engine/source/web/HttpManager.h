//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _HTTPMANAGER_H
#define _HTTPMANAGER_H

#include "sim/simBase.h"
#include "string/stringTable.h"
#include "web/IHttpServicesProvider.h"

class HTTPManager : public SimObject
{
    typedef SimObject Parent;

    friend class HttpManagerCallbackEvent;

public:
    struct HTTPEntry
    {
        // The Id of this request
        S32 m_Id;

        // The URL of the request
        StringTableEntry m_pURL;

        // Optional SimObject Id of object to call back.  If 0 then
        // the DownloadManager will handle the callback.
        SimObjectId m_CallbackObject;

        HTTPEntry()
        {
            m_Id = -1;
            m_pURL = StringTable->EmptyString;
        }
    };

public:
    HTTPManager();
    virtual ~HTTPManager();

    // SimObject overrides
    virtual bool onAdd();
    virtual void onRemove();

    void SetHTTPProvider(IHttpServicesProvider* provider);

    // Add a post request to the queue
    S32 addPostRequest(const char* url, IHttpServicesProvider::PostDictionary* postArray, SimObjectId callbackObject);

    DECLARE_CONOBJECT(HTTPManager);

protected:
    typedef HashTable<S32, HTTPEntry*> HttpRequestHash;
    typedef HttpRequestHash::iterator HttpRequestHashIterator;

    HttpRequestHash m_HttpRequests;

    IHttpServicesProvider* m_pHtpServicesProvider;

protected:
    void onWrongURLTypeCallback(S32 id);
    void onRequestProgressCallback(S32 id, U32 bytesRead, U32 bytesTotal);
    void onRequestErrorCallback(S32 id, const char* errorText);
    void onRequestFinishedCallback(S32 id, char* response, U32 responseLength, S32 responseCode);
};

#endif  // _HTTPMANAGER_H
