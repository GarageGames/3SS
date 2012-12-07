//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _QT_MANAGER_H
#define _QT_MANAGER_H

#include "sim/simBase.h"
#include "qt/IQtAppService.h"
#include "qt/IQtCommandService.h"
#include "qt/QtNetworkManager.h"
#include "web/IDownloadProvider.h"
#include "web/IHttpServicesProvider.h"
#include "platform/Tickable.h"

class QtThread;
class QtAppHandler;
class QtCommandPump;
class QtFileDownloader;
class QApplication;

class QtManager : public SimObject, public IDownloadProvider, public IHttpServicesProvider
{
    typedef SimObject Parent;

public:
    static QtManager* s_Manager;

public:
    QtManager();
    virtual ~QtManager();

    // SimObject overrides
    virtual bool onAdd();
    virtual void onRemove();

    QtNetworkManager& getNetworkManager() { return m_NetManager; }

    // Network Manager Cookie Support
    bool saveCookies(const char* path);
    bool loadCookies(const char* path);
    void clearCookies();

    S32 CreateWebRender(SimObjectId caller, S32 width, S32 height);

    void DestroyWebRender(S32 id);

    void WebRenderSetSize(S32 id, S32 width, S32 height);

    void WebRenderSetUrl(S32 id, const char* url);

    void WebRenderReloadAction(S32 id);
    void WebRenderBackAction(S32 id);
    void WebRenderForwardAction(S32 id);
    void WebRenderStopAction(S32 id);

    void WebRenderUpdateView(S32 id, bool force);

    void WebRenderMouseMove(S32 id, Point2I point);
    void WebRenderMouseDown(S32 id, Point2I point);
    void WebRenderMouseUp(S32 id, Point2I point);
    void WebRenderMouseDragged(S32 id, Point2I point);

    void WebRenderMouseWheelDown(S32 id, Point2I point);
    void WebRenderMouseWheelUp(S32 id, Point2I point);

    void WebRenderKeyboardInput(S32 id, U32 key, U32 keyASCII);

    // From IDownloadProvider
    virtual void DownloadFile(S32 callback, const char* url, const char* path, bool deleteExisting);
    virtual void CancelDownload();
    virtual void PauseDownload();
    virtual void ResumeDownload();

    // From IHttpServicesProvider
    virtual S32 HttpPost(S32 callback, const char* url, PostDictionary* postArray);

    DECLARE_CONOBJECT(QtManager);

    static QtManager* singleton();

protected:
    // Indicates that the Qt system should be multithreaded
    bool m_UseMultithread;

    // QApplication thread when running multithreaded
    QtThread* m_pAppThread;

    // Our network manager
    QtNetworkManager m_NetManager;

    // Qt application used when running singlethreaded
    QtAppHandler* m_pAppHandler;

    // Points to entity to send commands to
    IQtCommandService* m_pCommandService;

    // Tracks the next available web view Id
    S32 m_WebRenderCounter;

    // Tracks the next Http Request Id
    S32 m_HttpRequestCounter;

protected:
    // Start QtThread if multithreaded
    bool startUpThread();

    // Start QtAppHandler if single threaded
    bool startUpQtAppHandler();

    // Initialize the network manager
    void initNetworkManager();

    S32 getNextWebRenderId() { return m_WebRenderCounter++; }

    // Convert from Torque key mapping to Qt key mapping
    U32 convertTorqueKeyToQT(U32 torqueKey);

    // Create all file path expandos
    void createPathExpandos();
};

#endif // _QT_MANAGER_H
