//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _QT_COOKIEJAR_H
#define _QT_COOKIEJAR_H

#include <QNetworkCookieJar>

class QtCookieJar : public QNetworkCookieJar
{
    typedef QNetworkCookieJar Parent;

    Q_OBJECT

public:
    QtCookieJar();
    virtual ~QtCookieJar();

    // From QNetworkCookieJar
    virtual QList<QNetworkCookie> cookiesForUrl( const QUrl& url ) const;
    virtual bool setCookiesFromUrl( const QList<QNetworkCookie>& cookieList, const QUrl& url );

    bool save(const char* path);
    bool load(const char* path);

    void clearAllCookies();
};

#endif  // _QT_COOKIEJAR_H
