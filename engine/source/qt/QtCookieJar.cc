//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "qt/QtCookieJar.h"
#include "console/console.h"
#include "io/fileStream.h"
#include "io/bitStream.h"
#include "platform/platform.h"

#include <QDateTime>

QtCookieJar::QtCookieJar()
{
}

QtCookieJar::~QtCookieJar()
{
}

//-----------------------------------------------------------------------------

QList<QNetworkCookie> QtCookieJar::cookiesForUrl( const QUrl& url ) const
{
    // Convert the URL to a string
    //QString str = url.toString(); 
    //QByteArray ba = str.toAscii();
    //char* text = new char[ba.size()+1];
    //dMemcpy(text, ba, ba.size());
    //text[ba.size()] = '\0';

    //Con::printf("Cookie request from: %s", text);
    //delete[] text;

    return Parent::cookiesForUrl(url);
}

//-----------------------------------------------------------------------------

bool QtCookieJar::setCookiesFromUrl( const QList<QNetworkCookie>& cookieList, const QUrl& url )
{
    // Convert the URL to a string
    //QString str = url.toString(); 
    //QByteArray ba = str.toAscii();
    //char* text = new char[ba.size()+1];
    //dMemcpy(text, ba, ba.size());
    //text[ba.size()] = '\0';

    //Con::printf("Cookies sent from: %s [%d]", text, cookieList.size());
    //delete[] text;

    return Parent::setCookiesFromUrl(cookieList, url);
}

//-----------------------------------------------------------------------------

bool QtCookieJar::save(const char* path)
{
    char fullPath[1024];
    bool result = Con::expandPath(fullPath, 1024, path);
    if(!result)
    {
        Con::errorf("QtCookieJar::save(): Could not expand path '%s'", path);
        return false;
    }

    // Make sure the path exists
    result = Platform::createPath(fullPath);
    if(!result)
    {
        Con::errorf("QtCookieJar::save(): Could not create path '%s'", path);
        return false;
    }

    // Attempt to open the file for writing
    FileStream stream;
    result = stream.open(fullPath, FileStream::Write);
    if(!result)
    {
        Con::errorf("QtCookieJar::save(): Could not open file for writing '%s'", path);
        return false;
    }

    // Begin the bit stream to save the cookies to, including a header
    InfiniteBitStream cookieStream;
    // Header to make sure the data doesn't start on a byte boundry
    cookieStream.writeFlag(true);
    cookieStream.writeFlag(false);
    cookieStream.writeFlag(true);
    // Version information
    cookieStream.writeInt(1, 6);
    cookieStream.writeInt(0, 6);

    // Calculate the number of cookies that will be saved
    QList<QNetworkCookie> cookies = allCookies();
    U32 count = 0;
    for(QList<QNetworkCookie>::iterator cookie = cookies.begin(); cookie != cookies.end(); ++cookie)
    {
        if(cookie->isSessionCookie())
        {
            // Don't save a session cookie
            continue;
        }

        ++count;
    }

    // Write out the cookie count
    cookieStream.writeInt(count, 8);

    // Write the cookies to a bit stream
    for(QList<QNetworkCookie>::iterator cookie = cookies.begin(); cookie != cookies.end(); ++cookie)
    {
        if(cookie->isSessionCookie())
        {
            // Don't save a session cookie
            continue;
        }

        // Cookie name
        QByteArray name = cookie->name();
        cookieStream.writeInt(name.size(), 8);
        cookieStream._write(name.size(), name.data());

        // Secure setting
        cookieStream.writeFlag(cookie->isSecure());

        // Http setting
        cookieStream.writeFlag(cookie->isHttpOnly());

        // Cookie value
        QByteArray value = cookie->value();
        cookieStream.writeInt(value.size(), 8);
        cookieStream._write(value.size(), value.data());

        // Expires setting
        QDateTime expires = cookie->expirationDate();
        cookieStream.writeInt(expires.toTime_t(), 8);

        // Domain setting
        QString domain = cookie->domain();
        QByteArray domainBytes = domain.toLatin1();
        char* domainBuffer = new char[domainBytes.size()];
        dMemcpy(domainBuffer, domainBytes.data(), domainBytes.size());
        cookieStream.writeInt(domainBytes.size(), 8);
        cookieStream.write(domainBytes.size(), domainBuffer);
        delete[] domainBuffer;

        // Path setting
        QString cookiePath = cookie->path();
        QByteArray cookiePathBytes = cookiePath.toLatin1();
        char* cookiePathBuffer = new char[cookiePathBytes.size()];
        dMemcpy(cookiePathBuffer, cookiePathBytes.data(), cookiePathBytes.size());
        cookieStream.writeInt(cookiePathBytes.size(), 8);
        cookieStream.write(cookiePathBytes.size(), cookiePathBuffer);
        delete[] cookiePathBuffer;
    }

    // Write the bit stream to the file
    cookieStream.setPosition(0);
    stream.copyFrom(&cookieStream);

    // Close the file
    stream.close();

    return true;
}

bool QtCookieJar::load(const char* path)
{
    char fullPath[1024];
    bool result = Con::expandPath(fullPath, 1024, path);
    if(!result)
    {
        Con::errorf("QtCookieJar::load(): Could not expand path '%s'", path);
        return false;
    }

    // Attempt to open the file for reading.  If it doesn't exist then that means
    // we don't have any cookies to load.
    FileStream stream;
    result = stream.open(fullPath, FileStream::Read);
    if(!result)
    {
        return false;
    }

    // Load in the file to our bit stream
    InfiniteBitStream cookieStream;
    cookieStream.copyFrom(&stream);
    stream.close();

    // Make sure the header and version is correct
    cookieStream.setPosition(0);
    if(cookieStream.readFlag() != true)
    {
        return false;
    }
    if(cookieStream.readFlag() != false)
    {
        return false;
    }
    if(cookieStream.readFlag() != true)
    {
        return false;
    }

    S32 ver1 = cookieStream.readInt(6);
    S32 ver2 = cookieStream.readInt(6);
    if(ver1 != 1 || ver2 != 0)
    {
        return false;
    }

    // Read in the cookie count
    U32 count = cookieStream.readInt(8);

    QList<QNetworkCookie> cookies;

    // Read in each cookie
    for(U32 i=0; i<count; ++i)
    {
        // Cookie name
        const U32 nameSize = cookieStream.readInt(8);
        char* nameBuffer = new char[nameSize];
        cookieStream._read(nameSize, nameBuffer);
        QByteArray name(nameBuffer, nameSize);
        delete[] nameBuffer;

        // Cookie is secure
        bool secure = cookieStream.readFlag();

        // Cookie is HTTP
        bool http = cookieStream.readFlag();

        // Cookie value
        const S32 valueSize = cookieStream.readInt(8);
        char* valueBuffer = new char[valueSize];
        cookieStream._read(valueSize, valueBuffer);
        QByteArray value(valueBuffer, valueSize);
        delete[] valueBuffer;

        // Cookie expires
        U32 expiresRaw = cookieStream.readInt(8);
        QDateTime expires;
        expires.fromTime_t(expiresRaw);

        // Cookie domain
        const U32 domainSize = cookieStream.readInt(8);
        char* domainBuffer = new char[domainSize+1];
        cookieStream.read(domainSize, domainBuffer);
        domainBuffer[domainSize] = '\0';
        QString domain(domainBuffer);
        delete[] domainBuffer;

        // Cookie path
        const U32 cookiePathSize = cookieStream.readInt(8);
        char* cookiePathBuffer = new char[cookiePathSize+1];
        cookieStream.read(cookiePathSize, cookiePathBuffer);
        cookiePathBuffer[cookiePathSize] = '\0';
        QString cookiePath(cookiePathBuffer);
        delete[] cookiePathBuffer;

        // Create the actual cookie
        QNetworkCookie cookie;
        cookie.setName(name);
        cookie.setValue(value);
        cookie.setDomain(domain);
        cookie.setPath(cookiePath);
        cookie.setSecure(secure);
        cookie.setHttpOnly(http);
        cookie.setExpirationDate(expires);

        // Add the cookie to the jar
        cookies.append(cookie);
    }

    // Set all cookies on the jar
    setAllCookies(cookies);

    return true;
}

//-----------------------------------------------------------------------------

void QtCookieJar::clearAllCookies()
{
    QList<QNetworkCookie> cookies;

    // Clear all cookies by setting to an empty list
    setAllCookies(cookies);
}
