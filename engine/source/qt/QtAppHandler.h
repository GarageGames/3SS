//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _QT_APPHANDLER_H
#define _QT_APPHANDLER_H

#include "platform/Tickable.h"
#include "qt/IQtAppService.h"
#include "qt/IQtCommandService.h"
#include "qt/QtCommandPump.h"
#include "qt/QtFileDownloader.h"
#include "qt/QtHttpRequestHandler.h"

class QApplication;

class QtAppHandler : public IQtAppService, public IQtCommandService, public virtual Tickable
{
public:
    QtAppHandler();
    virtual ~QtAppHandler();

    // From IQtAppService
    virtual QApplication* getQtApp() {return m_pApp;}

    // From IQtCommandService
    virtual void sendCommand(IQtCommandService::Command* c);

    void startUp();

    void shutDown();

protected:
    // Qt application
    QApplication* m_pApp;

    QtCommandPump* m_pCommandPump;

    // Used for downloading files
    QtFileDownloader* m_pFileDownloader;

    // Used for Http requests
    QtHTTPRequestHandler* m_pHttpRequestHandler;

protected:
    // From Tickable.
    virtual void interpolateTick( F32 delta ) {}
    virtual void processTick();
    virtual void advanceTime( F32 timeDelta ) {}
};

#endif // _QT_APPHANDLER_H
