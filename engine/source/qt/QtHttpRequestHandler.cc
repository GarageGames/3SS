//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "qt/QtManager.h"
#include "qt/QtHttpRequesthandler.h"
#include "platform/platform.h"
#include "console/console.h"
#include "web/HttpManagerEvents.h"

#include <QNetworkReply>
#include <QSslError>

QtHTTPRequestHandler::QtHTTPRequestHandler()
{
}

QtHTTPRequestHandler::~QtHTTPRequestHandler()
{
    cleanNetworkReplies();
}

//-----------------------------------------------------------------------------

void QtHTTPRequestHandler::cleanNetworkReply(QNetworkReply* reply)
{
    for(NetworkReplyMap::iterator itr = m_NetworkReplies.begin(); itr != m_NetworkReplies.end(); ++itr)
    {
        QNetworkReply* nr = itr.key();
        if(nr == reply)
        {
            // Delete the network reply
            disconnectNetworkReplySignals(reply);
            reply->deleteLater();

            RequestData* data = itr.value();
            delete data;

            m_NetworkReplies.erase(itr);

            break;
        }
    }
}

void QtHTTPRequestHandler::cleanNetworkReplies()
{
    while(m_NetworkReplies.count() > 0)
    {
        // Delete the network reply key/value
        NetworkReplyMap::iterator itr = m_NetworkReplies.begin();
        QNetworkReply* reply = itr.key();
        disconnectNetworkReplySignals(reply);
        delete reply;
        RequestData* data = itr.value();
        delete data;

        m_NetworkReplies.erase(itr);
    }
}

//-----------------------------------------------------------------------------

bool QtHTTPRequestHandler::Post(QString url, IQtCommandService::KeyValueMap* keyValueMap, S32 id, SimObjectId callback)
{
    // Copy the post parameters
    QByteArray data;
    QUrl params;

    for(IQtCommandService::KeyValueMap::iterator itr = keyValueMap->begin(); itr != keyValueMap->end(); ++itr)
    {
        params.addQueryItem(itr.key(), itr.value());
    }

    data = params.encodedQuery();

    return repost(url, data, id, callback);
}

bool QtHTTPRequestHandler::repost(QString url, QByteArray& data, S32 id, SimObjectId callback)
{
    // Create the request
    QNetworkRequest request(url);

    // Indicate that we're passing encoded data
    request.setHeader(QNetworkRequest::ContentTypeHeader, "application/x-www-form-urlencoded");

    // Generate the post
    QNetworkReply* reply = QtManager::singleton()->getNetworkManager().post(request, data);
    if(!reply)
        return false;

    connectNetworkReplySignals(reply);

    // Store the values in our table
    RequestData* rd = new RequestData();
    rd->mRequestId = id;
    rd->mCallbackObject = callback;
    rd->mParameters = data;
    m_NetworkReplies.insert(reply, rd);

    return true;
}

//-----------------------------------------------------------------------------

void QtHTTPRequestHandler::connectNetworkReplySignals(QNetworkReply* reply)
{
    if(!reply)
        return;

    connect(reply, SIGNAL(finished()),                          this, SLOT(finishedSlot()));
    connect(reply, SIGNAL(readyRead()),                         this, SLOT(readyReadSlot()));
    connect(reply, SIGNAL(downloadProgress(qint64, qint64)),    this, SLOT(downloadProgressSlot(qint64, qint64)));
    connect(reply, SIGNAL(sslErrors(const QList<QSslError>&)),  this, SLOT(sslErrorsSlot(const QList<QSslError>&)));
}

void QtHTTPRequestHandler::disconnectNetworkReplySignals(QNetworkReply* reply)
{
    if(!reply)
        return;

    disconnect(reply, SIGNAL(finished()),                           this, SLOT(finishedSlot()));
    disconnect(reply, SIGNAL(readyRead()),                          this, SLOT(readyReadSlot()));
    disconnect(reply, SIGNAL(downloadProgress(qint64, qint64)),     this, SLOT(downloadProgressSlot(qint64, qint64)));
    disconnect(reply, SIGNAL(sslErrors(const QList<QSslError>&)),   this, SLOT(sslErrorsSlot(const QList<QSslError>&)));
}

//-----------------------------------------------------------------------------

void QtHTTPRequestHandler::finishedSlot()
{
    QNetworkReply* reply = dynamic_cast<QNetworkReply*>(sender());
    if(!reply)
        return;

    // Attempt to retrieve the HTTP response code
    S32 responseCode = -1;
    QVariant responseCodeVariant = reply->attribute(QNetworkRequest::HttpStatusCodeAttribute);
    if(responseCodeVariant.isValid())
    {
        responseCode = responseCodeVariant.toInt();
        Con::printf("QtHTTPRequestHandler::finishedSlot() HTTP Reponse Code: %d", responseCode);
    }

    // Check for error
    if(reply->error())
    {
        QByteArray ba(reply->errorString().toLatin1());

        HttpManagerCallbackEvent::CallbackData* data = new HttpManagerCallbackEvent::CallbackData();

        // Copy the error string into the callback data
        QByteArray errorBA = reply->errorString().toLatin1();
        char* text = new char[errorBA.size()+1];
        dMemcpy(text, errorBA, errorBA.size());
        text[errorBA.size()] = '\0';
        data->m_StringData = text;
        Con::warnf("QtHTTPRequestHandler::httpFinishedSlot(): request failed: %s [%d]", text, reply->error());

        // Look up the request in our table
        if(m_NetworkReplies.contains(reply))
        {
            RequestData* rd = m_NetworkReplies.find(reply).value();

            data->m_Id = rd->mRequestId;

            // Post the error
            Sim::postCurrentEvent(rd->mCallbackObject, new HttpManagerCallbackEvent(HttpManagerCallbackEvent::CALLBACK_REQUEST_ERROR, data));
        }
    }
    else
    {
        bool replyHandled = false;

        // Check for a redirect
        if(responseCode == 301 || responseCode == 302)
        {
            QVariant redirectURL = reply->attribute(QNetworkRequest::RedirectionTargetAttribute);
            if(redirectURL.isValid())
            {
                QUrl newUrl = redirectURL.toUrl();

                // Post to the redirected URL
                if(m_NetworkReplies.contains(reply))
                {
                    RequestData* rd = m_NetworkReplies.find(reply).value();
                    repost(newUrl.toString(), rd->mParameters, rd->mRequestId, rd->mCallbackObject);
                    replyHandled = true;
                }
            }
        }

        // If the reply has not already been handled by the HTTP processing then work
        // with its payload here.
        if(!replyHandled)
        {
            QByteArray requestData = reply->readAll();

            // Look up the request in our table
            if(m_NetworkReplies.contains(reply))
            {
                RequestData* rd = m_NetworkReplies.find(reply).value();

                // Build the callback data
                HttpManagerCallbackEvent::CallbackData* data = new HttpManagerCallbackEvent::CallbackData();

                data->m_Response = new char[requestData.size()+1];
                dMemcpy(data->m_Response, requestData.data(), requestData.size());
                data->m_Response[requestData.size()] = '\0';

                data->m_ResponseLength = requestData.size();

                data->m_Id = rd->mRequestId;
                data->m_ResponseCode = responseCode;

                // Post the success
                Sim::postCurrentEvent(rd->mCallbackObject, new HttpManagerCallbackEvent(HttpManagerCallbackEvent::CALLBACK_REQUEST_FINISHED, data));
            }
        }
    }

    // Clean up the reply
    cleanNetworkReply(reply);
}

void QtHTTPRequestHandler::readyReadSlot()
{
    QNetworkReply* reply = dynamic_cast<QNetworkReply*>(sender());
    if(!reply)
        return;

    //qint64 size = reply->bytesAvailable();
    //Con::printf("QtHTTPRequestHandler::readyReadSlot(): %d", size);
}

void QtHTTPRequestHandler::downloadProgressSlot(qint64 bytesRead, qint64 totalBytes)
{
}

void QtHTTPRequestHandler::sslErrorsSlot(const QList<QSslError>& errors)
{
    QNetworkReply* reply = dynamic_cast<QNetworkReply*>(sender());
    if(!reply)
        return;

    QString errorString;
    foreach (const QSslError &error, errors)
    {
        if(!errorString.isEmpty())
            errorString += ", ";

        errorString += error.errorString();
    }

    QByteArray ba(errorString.toLatin1());
    Con::warnf("QtFileDownloader::sslErrorsSlot() Errors: %s", ba.data());

    // Just ignore SSL errors
    reply->ignoreSslErrors();
}
