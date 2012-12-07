//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _QT_HTTPREQUESTHANDLER_H
#define _QT_HTTPREQUESTHANDLER_H

#include "sim/simBase.h"
#include "qt/IQtCommandService.h"

#include <QObject>
#include <QMap>
#include <QNetworkAccessManager>
#include <QNetworkReply>

class QtHTTPRequestHandler : public QObject
{
    Q_OBJECT

public slots:
    void finishedSlot();
    void readyReadSlot();
    void downloadProgressSlot(qint64 bytesRead, qint64 totalBytes);
    void sslErrorsSlot(const QList<QSslError>& errors);

public:
    struct RequestData
    {
        SimObjectId mCallbackObject;
        S32 mRequestId;
        QByteArray mParameters;
    };

public:
    QtHTTPRequestHandler();
    virtual ~QtHTTPRequestHandler();

    bool Post(QString url, IQtCommandService::KeyValueMap* keyValueMap, S32 id, SimObjectId callback);

protected:
    typedef QMap<QNetworkReply*, RequestData*> NetworkReplyMap;

    NetworkReplyMap m_NetworkReplies;

protected:
    // Clean up a network reply
    void cleanNetworkReply(QNetworkReply* reply);

    // Clean up all of the network replies
    void cleanNetworkReplies();

    // Connect up the given QNetworkReply to our slots
    void connectNetworkReplySignals(QNetworkReply* reply);

    // Disconnect the given QNetworkReply from our slots
    void disconnectNetworkReplySignals(QNetworkReply* reply);

    // Repost a request
    bool repost(QString url, QByteArray& data, S32 id, SimObjectId callback);
};

#endif  // _QT_HTTPREQUESTHANDLER_H
