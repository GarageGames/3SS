//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "qt/QtCommandPump.h"
#include "qt/QtFileDownloader.h"

#include <QFocusEvent>

QtCommandPump::QtCommandPump(IQtAppService* appService, QtFileDownloader* downloader, QtHTTPRequestHandler* httphandler)
{
    m_pAppService = appService;
    m_pFileDownloader = downloader;
    m_pHttpRequestHandler = httphandler;
    m_CurrentCommandBuffer = 0;
}

QtCommandPump::~QtCommandPump()
{
    // Clear out command buffers
    for(U32 i=0; i<2; ++i)
    {
        while(m_Commands[i].size() > 0)
        {
            Command* c = m_Commands[i][0];
            m_Commands[i].pop_front();
            delete c;
        }
    }
}

void QtCommandPump::sendCommand(IQtCommandService::Command* c)
{
    m_CommandsMutex.lock();

    m_Commands[m_CurrentCommandBuffer].push_back(c);

    m_CommandsMutex.unlock();
}

void QtCommandPump::tick(Mutex* commandsMutex)
{
    Command* c;

    // Determine if we should used the passed in mutex, or our local one.
    Mutex* cmdMutex = commandsMutex;
    if(!cmdMutex)
        cmdMutex = &m_CommandsMutex;

    // Swtich command buffers.  This allows commands to be sent
    // to the current buffer while we process the other one.
    cmdMutex->lock();
    S32 index = m_CurrentCommandBuffer;
    m_CurrentCommandBuffer = 1 - m_CurrentCommandBuffer;
    cmdMutex->unlock();

    // Step through the command buffer (non-current one)
    while(m_Commands[index].size() > 0)
    {
        // Get the next command
        c = m_Commands[index][0];
        m_Commands[index].pop_front();

        switch(c->m_Id)
        {
            case C_WEBRENDER_CREATE:
                CmdWebRenderCreate(c);
                break;

            case C_WEBRENDER_DESTROY:
                CmdWebRenderDestroy(c);
                break;

            case C_WEBRENDER_SETSIZE:
                CmdWebRenderSetSize(c);
                break;

            case C_WEBRENDER_SETURL:
                CmdWebRenderSetUrl(c);
                break;

            case C_WEBRENDER_RELOAD_ACTION:
                CmdWebRenderReloadAction(c);
                break;

            case C_WEBRENDER_BACK_ACTION:
                CmdWebRenderBackAction(c);
                break;

            case C_WEBRENDER_FORWARD_ACTION:
                CmdWebRenderForwardAction(c);
                break;

            case C_WEBRENDER_STOP_ACTION:
                CmdWebRenderStopAction(c);
                break;

            case C_WEBRENDER_UPDATEVIEW:
                CmdWebRenderUpdateView(c);
                break;

            case C_WEBRENDER_MOUSEMOVE:
            case C_WEBRENDER_MOUSEDOWN:
            case C_WEBRENDER_MOUSEUP:
            case C_WEBRENDER_MOUSEDRAGGED:
                CmdWebRenderMouseEvent(c);
                break;

            case C_WEBRENDER_MOUSEWHEELDOWN:
            case C_WEBRENDER_MOUSEWHEELUP:
                CmdWebRenderMouseWheelEvent(c);
                break;

            case C_WEBRENDER_KEYBOARD_INPUT:
                CmdWebRenderKeyboardInput(c);
                break;

            case C_DOWNLOAD_FILE:
                CmdDownloadFile(c);
                break;

            case C_DOWNLOAD_CANCEL:
                CmdDownloadCancel(c);
                break;

            case C_DOWNLOAD_PAUSE:
                CmdDownloadPause(c);
                break;

            case C_DOWNLOAD_RESUME:
                CmdDownloadResume(c);
                break;

            case C_HTTPREQUEST_POST:
                CmdHttpRequestPost(c);
                break;
        }

        // Delete the processed command
        delete c;
    }
}

//-----------------------------------------------------------------------------

void QtCommandPump::CmdWebRenderCreate(Command* c)
{
    S32 id = c->m_S32Args[0];

    WebRender* r = new WebRender();
    r->SetId(id);
    r->SetSize(c->m_S32Args[1], c->m_S32Args[2]);

    // Store the SimObjectId of the owner of this web render object
    r->SetOwnerId(c->m_S32Args[3]);

    m_WebRenderList.insertEqual(id, r);
}

void QtCommandPump::CmdWebRenderDestroy(Command* c)
{
    S32 id = c->m_S32Args[0];

    WebRender* r = m_WebRenderList.find(id).getValue();
    if(!r)
        return;

    m_WebRenderList.erase(id);

    delete r;
}

void QtCommandPump::CmdWebRenderSetSize(Command* c)
{
    S32 id = c->m_S32Args[0];

    WebRender* r = m_WebRenderList.find(id).getValue();
    if(!r)
        return;

    r->SetSize(c->m_S32Args[1], c->m_S32Args[2]);
}

void QtCommandPump::CmdWebRenderSetUrl(Command* c)
{
    S32 id = c->m_S32Args[0];

    WebRender* r = m_WebRenderList.find(id).getValue();
    if(!r)
        return;

    r->SetURL(c->m_StringArgs[0].toLatin1());
}

void QtCommandPump::CmdWebRenderReloadAction(Command* c)
{
    S32 id = c->m_S32Args[0];

    WebRender* r = m_WebRenderList.find(id).getValue();
    if(!r)
        return;

    r->ReloadAction();
}

void QtCommandPump::CmdWebRenderBackAction(Command* c)
{
    S32 id = c->m_S32Args[0];

    WebRender* r = m_WebRenderList.find(id).getValue();
    if(!r)
        return;

    r->BackAction();
}

void QtCommandPump::CmdWebRenderForwardAction(Command* c)
{
    S32 id = c->m_S32Args[0];

    WebRender* r = m_WebRenderList.find(id).getValue();
    if(!r)
        return;

    r->ForwardAction();
}

void QtCommandPump::CmdWebRenderStopAction(Command* c)
{
    S32 id = c->m_S32Args[0];

    WebRender* r = m_WebRenderList.find(id).getValue();
    if(!r)
        return;

    r->StopAction();
}

void QtCommandPump::CmdWebRenderUpdateView(Command* c)
{
    S32 id = c->m_S32Args[0];

    WebRender* r = m_WebRenderList.find(id).getValue();
    if(!r)
        return;

    r->UpdateView((bool)c->m_S32Args[1]);
}

void QtCommandPump::CmdWebRenderMouseEvent(Command* c)
{
    S32 id = c->m_S32Args[0];

    WebRender* r = m_WebRenderList.find(id).getValue();
    if(!r)
        return;

    if(c->m_Id != C_WEBRENDER_MOUSEMOVE)
    {
        QFocusEvent focus(QEvent::FocusIn);
        m_pAppService->getQtApp()->sendEvent(r, &focus);
    }

    switch(c->m_Id)
    {
        case C_WEBRENDER_MOUSEMOVE:
            {
                QMouseEvent mouseEvent(QEvent::MouseMove, QPoint(c->m_S32Args[1], c->m_S32Args[2]), Qt::NoButton, Qt::NoButton,  Qt::NoModifier);
                m_pAppService->getQtApp()->sendEvent(r, &mouseEvent);
            }
            break;

        case C_WEBRENDER_MOUSEDOWN:
            {
                QMouseEvent mouseEvent(QEvent::MouseButtonPress, QPoint(c->m_S32Args[1], c->m_S32Args[2]), Qt::LeftButton , Qt::LeftButton, Qt::NoModifier);
                m_pAppService->getQtApp()->sendEvent(r, &mouseEvent);
            }
            break;

        case C_WEBRENDER_MOUSEUP:
            {
                QMouseEvent mouseEvent(QEvent::MouseButtonRelease, QPoint(c->m_S32Args[1], c->m_S32Args[2]), Qt::LeftButton , Qt::NoButton, Qt::NoModifier);
                m_pAppService->getQtApp()->sendEvent(r, &mouseEvent);
            }
            break;

        case C_WEBRENDER_MOUSEDRAGGED:
            {
                QMouseEvent mouseEvent(QEvent::MouseMove, QPoint(c->m_S32Args[1], c->m_S32Args[2]), Qt::LeftButton , Qt::NoButton, Qt::NoModifier);
                m_pAppService->getQtApp()->sendEvent(r, &mouseEvent);
            }
            break;
    }
}

void QtCommandPump::CmdWebRenderMouseWheelEvent(Command* c)
{
    const S32 wheelSpeed = 66;

    S32 id = c->m_S32Args[0];

    WebRender* r = m_WebRenderList.find(id).getValue();
    if(!r)
        return;

    if(c->m_Id == C_WEBRENDER_MOUSEWHEELDOWN)
    {
        QWheelEvent wheelEvent(QPoint(c->m_S32Args[1], c->m_S32Args[2]), -wheelSpeed, Qt::NoButton, Qt::NoModifier);
        m_pAppService->getQtApp()->sendEvent(r, &wheelEvent);
    }
    else
    {
        QWheelEvent wheelEvent(QPoint(c->m_S32Args[1], c->m_S32Args[2]), wheelSpeed, Qt::NoButton, Qt::NoModifier);
        m_pAppService->getQtApp()->sendEvent(r, &wheelEvent);
    }
}

void QtCommandPump::CmdWebRenderKeyboardInput(Command* c)
{
    S32 id = c->m_S32Args[0];

    WebRender* r = m_WebRenderList.find(id).getValue();
    if(!r)
        return;

    QFocusEvent focus(QEvent::FocusIn);
    m_pAppService->getQtApp()->sendEvent(r, &focus);

    QKeyEvent keyEvent(QEvent::KeyPress, (U32)c->m_S32Args[1], Qt::NoModifier, QChar((U16)c->m_S32Args[2]));
    m_pAppService->getQtApp()->sendEvent(r, &keyEvent);
}

//-----------------------------------------------------------------------------

void QtCommandPump::CmdDownloadFile(Command* c)
{
    S32 callbackObjId = c->m_S32Args[0];

    m_pFileDownloader->FileDownload(c->m_StringArgs[0].toLatin1(), c->m_StringArgs[1].toLatin1(), callbackObjId, (bool)c->m_S32Args[1]);
}

void QtCommandPump::CmdDownloadCancel(Command* c)
{
    m_pFileDownloader->CancelDownload();
}

void QtCommandPump::CmdDownloadPause(Command* c)
{
    m_pFileDownloader->PauseDownload();
}

void QtCommandPump::CmdDownloadResume(Command* c)
{
    m_pFileDownloader->ResumeDownload();
}

//-----------------------------------------------------------------------------

void QtCommandPump::CmdHttpRequestPost(Command* c)
{
    m_pHttpRequestHandler->Post(c->m_StringArgs[0], &(c->m_KeyValueMap), c->m_S32Args[1], c->m_S32Args[0]);
}
