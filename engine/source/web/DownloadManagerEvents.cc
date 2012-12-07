//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "web/DownloadManagerEvents.h"
#include "web/DownloadManager.h"

DownloadManagerCallbackEvent::DownloadManagerCallbackEvent(CallbackType type, CallbackData* data)
{
    m_CallbackType = type;
    m_pCallbackData = data;
}

DownloadManagerCallbackEvent::~DownloadManagerCallbackEvent()
{
    if(m_pCallbackData)
        delete m_pCallbackData;
}

void DownloadManagerCallbackEvent::process(SimObject *object)
{
    // Make sure we've been sent to the correct object type
    DownloadManager* manager = dynamic_cast<DownloadManager*>(object);
    if(!manager)
        return;

    switch(m_CallbackType)
    {
        case CALLBACK_URL_WRONG_TYPE:
            manager->onWrongURLTypeCallback();
            break;

        case CALLBACK_FILE_BAD_PATH:
            manager->onBadFilePathCallback();
            break;

        case CALLBACK_DOWNLOAD_PROGRESS:
            if(m_pCallbackData)
            {
                manager->onDownloadProgressCallback(m_pCallbackData->m_U32Args[0], m_pCallbackData->m_U32Args[1]);
            }
            break;

        case CALLBACK_DOWNLOAD_ERROR:
            if(m_pCallbackData)
            {
                manager->onDownloadErrorCallback(m_pCallbackData->m_StringData);
            }
            break;

        case CALLBACK_DOWNLOAD_FINISHED:
            if(m_pCallbackData)
            {
                manager->onDownloadFinishedCallback(m_pCallbackData->m_U32Args[0]);
            }
            break;
    }
}
