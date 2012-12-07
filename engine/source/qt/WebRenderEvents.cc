//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "qt/WebRenderEvents.h"
#include "qt/guiWebViewCtrl.h"

WebRenderCallbackEvent::WebRenderCallbackEvent(CallbackType type, CallbackData* data)
{
    m_CallbackType = type;
    m_pCallbackData = data;
}

WebRenderCallbackEvent::~WebRenderCallbackEvent()
{
    if(m_pCallbackData)
        delete m_pCallbackData;
}

void WebRenderCallbackEvent::process(SimObject *object)
{
    // Make sure we've been sent to the correct object type
    GuiWebViewCtrl* ctrl = dynamic_cast<GuiWebViewCtrl*>(object);
    if(!ctrl)
        return;

    switch(m_CallbackType)
    {
        case CALLBACK_LOAD_STARTED:
            ctrl->onLoadStarted();
            break;

        case CALLBACK_LINK_CLICKED:
            if(m_pCallbackData)
            {
                ctrl->onLinkClicked(m_pCallbackData->m_URL);
            }
            break;

        case CALLBACK_FINISH_LOADING:
            if(m_pCallbackData)
            {
                ctrl->onFinishedLoading(m_pCallbackData->m_Result);
            }
            break;

        case CALLBACK_CONNECTION_ERROR:
            if(m_pCallbackData)
            {
                ctrl->onConnectionError(m_pCallbackData->m_StringData);
            }
            break;

        case CALLBACK_VIEW_UPDATE:
            if(m_pCallbackData)
            {
                ctrl->onUpdateView(m_pCallbackData->m_Result, m_pCallbackData->m_S32Args[0], m_pCallbackData->m_S32Args[1], m_pCallbackData->m_pU8Pointer);
            }
            break;

        case CALLBACK_URL_CHANGED:
            if(m_pCallbackData)
            {
                ctrl->onURLChanged(m_pCallbackData->m_URL);
            }
            break;
    }
}
