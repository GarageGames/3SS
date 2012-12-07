//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _HTTPMANAGEREVENTS_H
#define _HTTPMANAGEREVENTS_H

#include "sim/simBase.h"

class HttpManagerCallbackEvent : public SimEvent
{
public:
    enum CallbackType {
        CALLBACK_URL_WRONG_TYPE,
        CALLBACK_REQUEST_PROGRESS,
        CALLBACK_REQUEST_ERROR,
        CALLBACK_REQUEST_FINISHED,
    };

    struct CallbackData {
        S32 m_Id;
        char* m_Response;
        U32 m_ResponseLength;
        S32 m_ResponseCode;
        char* m_StringData;
        U32 m_U32Args[2];

        CallbackData()
        {
            m_Id = -1;
            m_Response = NULL;
            m_ResponseLength = 0;
            m_ResponseCode = -1;
            m_StringData = NULL;
        }

        ~CallbackData()
        {
            if(m_Response)
                delete m_Response;

            if(m_StringData)
                delete m_StringData;
        }
    };

public:
    HttpManagerCallbackEvent(CallbackType type, CallbackData* data);
    virtual ~HttpManagerCallbackEvent();

    virtual void process(SimObject *object);

protected:
    CallbackType m_CallbackType;
    CallbackData* m_pCallbackData;
};

#endif  // _HTTPMANAGEREVENTS_H
