//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _QT_THREAD_H
#define _QT_THREAD_H

#include "platform/threads/thread.h"
#include "platform/threads/semaphore.h"
#include "platform/threads/mutex.h"
#include "qt/IQtAppService.h"
#include "qt/IQtCommandService.h"
#include "qt/QtCommandPump.h"
#include "qt/QtFileDownloader.h"
#include "qt/QtHttpRequestHandler.h"

class QApplication;

class QtThread : public Thread, public IQtAppService, public IQtCommandService
{
public:
    QtThread();
    virtual ~QtThread();

    // From IQtAppService
    virtual QApplication* getQtApp() {return m_pApp;}

    // From IQtCommandService
    virtual void sendCommand(IQtCommandService::Command* c);

    // From Thread
    virtual void run(void *arg = 0);

    // Stop the thread and wait for it to finish
    void stopAndWait();

	// Wait for the thread to start running.  A timeout of 0 means an infinite wait.
	void waitForThreadRun(U32 timeoutMS=0);

protected:
    // Qt application
    QApplication* m_pApp;

    QtCommandPump* m_pCommandPump;

    // Used for downloading files
    QtFileDownloader* m_pFileDownloader;

    // Used for Http requests
    QtHTTPRequestHandler* m_pHttpRequestHandler;

    // Used to make the event loop continue from its wait
    // state.
    Semaphore m_Semaphore;

    // Used to wait for the thread to stop
    Semaphore m_StopSemaphore;

    // Commands list locking
    Mutex m_CommandsMutex;
};

#endif // _QT_THREAD_H
