//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "qt/QtCommandPump.h"
#include "qt/QtThread.h"

#include <QApplication>
#include <QTextCodec>
#include <QDir>

QtThread::QtThread() : 
    m_pApp(NULL),
    m_pCommandPump(NULL),
    m_pFileDownloader(NULL),
    Thread()
{
}

QtThread::~QtThread()
{
    // m_pApp will be destoryed when ~Thread() stops
    // the platform thread.
}

void QtThread::run(void *arg)
{
    S32 argc = 0;
    m_pApp = new QApplication(argc, NULL);

#ifdef __MACOSX__
    // Set up the plugin directory for OSX within the bundle
    QDir dir(QApplication::applicationDirPath());
    dir.cdUp();
    dir.cd("PlugIns");
    QApplication::setLibraryPaths(QStringList(dir.absolutePath()));
#endif
   
    QTextCodec::setCodecForTr(QTextCodec::codecForName("utf8"));
    QTextCodec::setCodecForCStrings(QTextCodec::codecForName("UTF-8"));

    // Hold on to the stop semaphore until the thread is done.
    m_StopSemaphore.acquire();

    // Start up the file downloader
    m_pFileDownloader = new QtFileDownloader();

    // Start up the HTTP request handler
    m_pHttpRequestHandler = new QtHTTPRequestHandler();

    // Start up the command pump
    m_pCommandPump = new QtCommandPump(this, m_pFileDownloader, m_pHttpRequestHandler);

    while(true)
    {
        // Allow the command pump to run
        m_pCommandPump->tick();

        if(checkForStop())
        {
            // Stop the command pump
            m_CommandsMutex.lock();
            delete m_pCommandPump;
            m_pCommandPump = NULL;
            m_CommandsMutex.unlock();

            // Stop the file downloader
            delete m_pFileDownloader;
            m_pFileDownloader = NULL;

            // Stop the HTTP request handler
            delete m_pHttpRequestHandler;
            m_pHttpRequestHandler = NULL;

            // Stop the Qt application
            delete m_pApp;
            m_pApp = NULL;
            break;
        }

        // Process all pending Qt events
        m_pApp->processEvents();

        // Pause before performing another event loop, unless
        // a new command comes in (which releases this semaphore).
        m_Semaphore.acquire(true, 50);
    }

    // Indicate that the thread has stopped
    m_StopSemaphore.release();
}

//-----------------------------------------------------------------------------

void QtThread::stopAndWait()
{
    stop();

    // Wait up to 10 seconds for the thread to stop running
    m_StopSemaphore.acquire(true, 10000);
}

//-----------------------------------------------------------------------------

void QtThread::sendCommand(IQtCommandService::Command* c)
{
    m_CommandsMutex.lock();

    if(m_pCommandPump)
        m_pCommandPump->sendCommand(c);

    m_CommandsMutex.unlock();

    m_Semaphore.release();
}
