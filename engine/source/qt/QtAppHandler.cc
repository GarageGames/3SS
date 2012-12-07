//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "qt/QtAppHandler.h"

#include <QApplication>
#include <QTextCodec>
#include <QDir>

QtAppHandler::QtAppHandler() : 
    m_pApp(NULL),
    m_pCommandPump(NULL),
    m_pFileDownloader(NULL)
{
}

QtAppHandler::~QtAppHandler()
{
}

//-----------------------------------------------------------------------------

void QtAppHandler::startUp()
{
    // Start up QApplication
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

    // Start up the file downloader
    m_pFileDownloader = new QtFileDownloader();

    // Start up the HTTP request handler
    m_pHttpRequestHandler = new QtHTTPRequestHandler();

    // Start up the command pump
    m_pCommandPump = new QtCommandPump(this, m_pFileDownloader, m_pHttpRequestHandler);

    setProcessTicks(true);
}

void QtAppHandler::shutDown()
{
    setProcessTicks(false);

    delete m_pCommandPump;
    m_pCommandPump = NULL;

    delete m_pFileDownloader;
    m_pFileDownloader = NULL;

    delete m_pHttpRequestHandler;
    m_pHttpRequestHandler = NULL;

    delete m_pApp;
    m_pApp = NULL;
}

//-----------------------------------------------------------------------------

void QtAppHandler::processTick()
{
    if(m_pCommandPump)
        m_pCommandPump->tick();

    //if(m_pApp)
    //    m_pApp->processEvents();
}

//-----------------------------------------------------------------------------

void QtAppHandler::sendCommand(IQtCommandService::Command* c)
{
    if(m_pCommandPump)
        m_pCommandPump->sendCommand(c);
}
