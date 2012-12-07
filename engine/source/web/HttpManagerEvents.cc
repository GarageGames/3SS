//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "web/HttpManagerEvents.h"
#include "web/HttpManager.h"

HttpManagerCallbackEvent::HttpManagerCallbackEvent(CallbackType type, CallbackData* data)
{
    m_CallbackType = type;
    m_pCallbackData = data;
}

HttpManagerCallbackEvent::~HttpManagerCallbackEvent()
{
    if(m_pCallbackData)
        delete m_pCallbackData;
}

void HttpManagerCallbackEvent::process(SimObject *object)
{
    // Make sure we've been sent to the correct object type
    HTTPManager* manager = dynamic_cast<HTTPManager*>(object);
    if(!manager)
        return;

    switch(m_CallbackType)
    {
        case CALLBACK_URL_WRONG_TYPE:
            if(m_pCallbackData)
            {
                manager->onWrongURLTypeCallback(m_pCallbackData->m_Id);
            }
            break;

        case CALLBACK_REQUEST_PROGRESS:
            if(m_pCallbackData)
            {
                manager->onRequestProgressCallback(m_pCallbackData->m_Id, m_pCallbackData->m_U32Args[0], m_pCallbackData->m_U32Args[1]);
            }
            break;

        case CALLBACK_REQUEST_ERROR:
            if(m_pCallbackData)
            {
                manager->onRequestErrorCallback(m_pCallbackData->m_Id, m_pCallbackData->m_StringData);
            }
            break;

        case CALLBACK_REQUEST_FINISHED:
            if(m_pCallbackData)
            {
                manager->onRequestFinishedCallback(m_pCallbackData->m_Id, m_pCallbackData->m_Response, m_pCallbackData->m_ResponseLength, m_pCallbackData->m_ResponseCode);
            }
            break;
    }
}
