//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "web/HttpManager.h"
#include "web/IHttpCallback.h"

#include "web/HttpManager_ScriptBinding.h"

IMPLEMENT_CONOBJECT( HTTPManager );

HTTPManager::HTTPManager()
{
}

HTTPManager::~HTTPManager()
{
}

//-----------------------------------------------------------------------------

bool HTTPManager::onAdd()
{
    if( !Parent::onAdd() )
        return false;

    return true;
}

void HTTPManager::onRemove()
{
    // Call parent.
    Parent::onRemove();
}

//-----------------------------------------------------------------------------

void HTTPManager::SetHTTPProvider(IHttpServicesProvider* provider)
{
    m_pHtpServicesProvider = provider;
}

//-----------------------------------------------------------------------------

S32 HTTPManager::addPostRequest(const char* url, IHttpServicesProvider::PostDictionary* postArray, SimObjectId callbackObject)
{
    if(!m_pHtpServicesProvider)
    {
        Con::errorf("HTTPManager::addPostRequest(): No Http Services Provider");
        return -1;
    }

    // Send the post request to our provider.  Deleting the postArray is the responsibility of the provider.
    S32 id = m_pHtpServicesProvider->HttpPost(getId(), url, postArray);

    if(id == -1)
    {
        // There was a problem with generating the request
        Con::errorf("HTTPManager::addPostRequest(): Could not submit request to Heep Services Provider");
        return -1;
    }

    // Create our record of the request
    HTTPEntry* entry  = new HTTPEntry();
    entry->m_Id = id;
    entry->m_pURL = StringTable->insert(url, true);
    entry->m_CallbackObject = callbackObject;

    // Store the request for later look up
    m_HttpRequests.insertEqual(id, entry);

    return id;
}

//-----------------------------------------------------------------------------

void HTTPManager::onWrongURLTypeCallback(S32 id)
{
}

void HTTPManager::onRequestProgressCallback(S32 id, U32 bytesRead, U32 bytesTotal)
{
}

void HTTPManager::onRequestErrorCallback(S32 id, const char* errorText)
{
    // Find the entry in our table
    HttpRequestHashIterator itr = m_HttpRequests.find(id);
    if(!itr.getValue())
        return;

    HTTPEntry* entry = itr.getValue();

    SimObject* obj = Sim::findObject(entry->m_CallbackObject);

    if(!obj)
    {
        // Callback on self
        Con::executef(this, 2, "onRequestError", errorText);
    }
    else
    {
        IHttpCallback* callbackObj = dynamic_cast<IHttpCallback*>(obj);
        if(callbackObj)
        {
            // Callback on object in C++
            callbackObj->onRequestErrorCallback(id, errorText);
        }
        else
        {
            // Callback on object using script
            Con::executef(obj, 3, "onRequestError", this->getIdString(), errorText);
        }
    }

    // Remove this entry from the table
    delete entry;
    m_HttpRequests.erase(id);
}

void HTTPManager::onRequestFinishedCallback(S32 id, char* response, U32 responseLength, S32 responseCode)
{
    // Find the entry in our table
    HttpRequestHashIterator itr = m_HttpRequests.find(id);
    if(!itr.getValue())
        return;

    HTTPEntry* entry = itr.getValue();

    SimObject* obj = Sim::findObject(entry->m_CallbackObject);

    if(!obj)
    {
        // Callback on self
        Con::executef(this, 2, "onRequestFinished", response, Con::getIntArg(responseCode));
    }
    else
    {
        IHttpCallback* callbackObj = dynamic_cast<IHttpCallback*>(obj);
        if(callbackObj)
        {
            // Callback on object in C++
            callbackObj->onRequestFinishedCallback(id, responseCode, response);
        }
        else
        {
            // Callback on object using script
            Con::executef(obj, 3, "onRequestFinished", this->getIdString(), response, Con::getIntArg(responseCode));
        }
    }

    // Remove this entry from the table
    delete entry;
    m_HttpRequests.erase(id);
}
