//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _QT_NETWORKMANAGER_H
#define _QT_NETWORKMANAGER_H

#include <QNetworkAccessManager>

class QtCookieJar;

class QtNetworkManager : public QNetworkAccessManager
{
    Q_OBJECT

public slots:
    void authenticationRequiredSlot(QNetworkReply*,QAuthenticator *);

public:
    QtNetworkManager();
    virtual ~QtNetworkManager();

    void init();

    bool saveCookies(const char* path);

    bool loadCookies(const char* path);

    void clearCookies();

protected:
    QtCookieJar* m_pCookieJar;
};

#endif  // _QT_NETWORKMANAGER_H
