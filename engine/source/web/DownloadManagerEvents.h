//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _DOWNLOADMANAGEREVENTS_H
#define _DOWNLOADMANAGEREVENTS_H

#include "sim/simBase.h"

class DownloadManagerCallbackEvent : public SimEvent
{
public:
    enum CallbackType {
        CALLBACK_URL_WRONG_TYPE,
        CALLBACK_FILE_BAD_PATH,
        CALLBACK_DOWNLOAD_PROGRESS,
        CALLBACK_DOWNLOAD_ERROR,
        CALLBACK_DOWNLOAD_FINISHED,
    };

    struct CallbackData {
        char* m_StringData;
        U32 m_U32Args[2];

        CallbackData()
        {
            m_StringData = NULL;
        }

        ~CallbackData()
        {
            if(m_StringData)
                delete m_StringData;
        }
    };

public:
    DownloadManagerCallbackEvent(CallbackType type, CallbackData* data=NULL);
    virtual ~DownloadManagerCallbackEvent();

    virtual void process(SimObject *object);

protected:
    CallbackType m_CallbackType;
    CallbackData* m_pCallbackData;
};

#endif // _DOWNLOADMANAGEREVENTS_H
