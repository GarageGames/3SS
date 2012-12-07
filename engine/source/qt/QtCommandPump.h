//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _QT_COMMANDPUMP_H
#define _QT_COMMANDPUMP_H

#include "collection/hashTable.h"
#include "platform/threads/mutex.h"
#include "qt/IQtAppService.h"
#include "qt/IQtCommandService.h"
#include "qt/QtHttpRequestHandler.h"
#include "qt/WebRender.h"

#include <QString>

class QtFileDownloader;

class QtCommandPump : public IQtCommandService
{
public:
    QtCommandPump(IQtAppService* appService, QtFileDownloader* downloader, QtHTTPRequestHandler* httphandler);
    virtual ~QtCommandPump();

    virtual void sendCommand(IQtCommandService::Command* c);

    // Tick the command pump
    void tick(Mutex* commandsMutex=NULL);

protected:
    typedef HashTable<S32, WebRender*> WebRenderHash;
    typedef WebRenderHash::iterator WebRenderHashIterator;

    // Points to the QApplication service provider
    IQtAppService* m_pAppService;

    // Used for downloading files
    QtFileDownloader* m_pFileDownloader;

    // Used for HTTP requests
    QtHTTPRequestHandler* m_pHttpRequestHandler;

    // List of commands to execute (double buffered)
    Vector< IQtCommandService::Command* > m_Commands[2];

    // Current command buffer
    S32 m_CurrentCommandBuffer;

    // List of WebRenders
    WebRenderHash m_WebRenderList;

    // Commands list locking
    Mutex m_CommandsMutex;

protected:
    void CmdWebRenderCreate(Command* c);
    void CmdWebRenderDestroy(Command* c);
    void CmdWebRenderSetSize(Command* c);
    void CmdWebRenderSetUrl(Command* c);
    void CmdWebRenderReloadAction(Command* c);
    void CmdWebRenderBackAction(Command* c);
    void CmdWebRenderForwardAction(Command* c);
    void CmdWebRenderStopAction(Command* c);
    void CmdWebRenderUpdateView(Command* c);
    void CmdWebRenderMouseEvent(Command* c);
    void CmdWebRenderMouseWheelEvent(Command* c);
    void CmdWebRenderKeyboardInput(Command* c);

    void CmdDownloadFile(Command* c);
    void CmdDownloadCancel(Command* c);
    void CmdDownloadPause(Command* c);
    void CmdDownloadResume(Command* c);

    void CmdHttpRequestPost(Command* c);
};

#endif // _QT_COMMANDPUMP_H
