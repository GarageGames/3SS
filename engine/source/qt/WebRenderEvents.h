//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _WEBRENDEREVENTS_H
#define _WEBRENDEREVENTS_H

#include "sim/simBase.h"

class WebRenderCallbackEvent : public SimEvent
{
public:
    enum CallbackType {
        CALLBACK_LOAD_STARTED,
        CALLBACK_LINK_CLICKED,
        CALLBACK_FINISH_LOADING,
        CALLBACK_CONNECTION_ERROR,
        CALLBACK_VIEW_UPDATE,
        CALLBACK_URL_CHANGED,
    };

    struct CallbackData {
        bool m_Result;
        char* m_StringData;
        char* m_URL;
        U8* m_pU8Pointer;
        S32 m_S32Args[2];

        CallbackData()
        {
            m_Result = false;
            m_StringData = NULL;
            m_URL = NULL;
            m_pU8Pointer = NULL;
        }

        ~CallbackData()
        {
            if(m_StringData)
                delete m_StringData;

            if(m_URL)
                delete m_URL;
        }
    };

public:
    WebRenderCallbackEvent(CallbackType type, CallbackData* data=NULL);
    virtual ~WebRenderCallbackEvent();

    virtual void process(SimObject *object);

protected:
    CallbackType m_CallbackType;
    CallbackData* m_pCallbackData;
};

#endif // _WEBRENDEREVENTS_H
