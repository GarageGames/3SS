//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "qt/QtNetworkManager.h"
#include "qt/QtCookieJar.h"
#include "console/console.h"

QtNetworkManager::QtNetworkManager()
{
    m_pCookieJar = NULL;

    connect(this, SIGNAL(authenticationRequired(QNetworkReply*,QAuthenticator*)),  this, SLOT(authenticationRequiredSlot(QNetworkReply*,QAuthenticator*)));
}

QtNetworkManager::~QtNetworkManager()
{
}

//-----------------------------------------------------------------------------

void QtNetworkManager::init()
{
    if(!m_pCookieJar)
    {
        // Create the cookie jar and assign it to us.  The jar's parent
        // will automatically become us so we don't need to worry about
        // deleting the jar later.
        m_pCookieJar = new QtCookieJar();
        setCookieJar(m_pCookieJar);
    }
}

//-----------------------------------------------------------------------------

bool QtNetworkManager::saveCookies(const char* path)
{
    if(!m_pCookieJar)
        return false;

    return m_pCookieJar->save(path);
}

bool QtNetworkManager::loadCookies(const char* path)
{
    if(!m_pCookieJar)
        return false;

    return m_pCookieJar->load(path);
}

void QtNetworkManager::clearCookies()
{
    if(!m_pCookieJar)
        return;

    m_pCookieJar->clearAllCookies();
}

//-----------------------------------------------------------------------------

void QtNetworkManager::authenticationRequiredSlot(QNetworkReply* reply,QAuthenticator* auth)
{
    Con::printf("QtFileDownloader::authenticationRequiredSlot()");
}
