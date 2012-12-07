//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "qt/QtManager.h"
#include "qt/QtFileDownloader.h"
#include "platform/platform.h"
#include "console/console.h"
#include "web/DownloadManagerEvents.h"

#include <QNetworkRequest>
#include <QNetworkReply>
#include <QFileInfo>
#include <QDir>
#include <QSslError>

QtFileDownloader::QtFileDownloader()
{
    m_pNetReply = NULL;
    m_pFile = NULL;

    m_CallbackObject = 0;

    m_DownloadCanceled = false;

    m_DownloadPaused = false;
    m_DownloadPausedFileSize = 0;
}

QtFileDownloader::~QtFileDownloader()
{
    if(m_pFile)
        delete m_pFile;
}

//-----------------------------------------------------------------------------

bool QtFileDownloader::FileDownload(const char* url, const char* path, SimObjectId callback, bool deleteExisting)
{
    // Make sure that the URL is of the correct type
    if(dStrnicmp("HTTP://", url, 7) && dStrnicmp("HTTPS://", url, 8))
    {
        Con::warnf("QtFileDownloader::FileDownload(): URL is of wrong type [%s]", url);

        // Post the error
        Sim::postCurrentEvent(callback, new DownloadManagerCallbackEvent(DownloadManagerCallbackEvent::CALLBACK_URL_WRONG_TYPE));

        return false;
    }

    m_CallbackObject = callback;

    m_URL = url;

    char buffer[1024];
    Con::expandPath(buffer, sizeof(buffer), path);
    QString filePath = buffer;

    // Delete the existing file?
    bool continueExistingFile = false;
    if(QFile::exists(filePath))
    {
        if(deleteExisting)
        {
            QFile::remove(filePath);
        }
        else
        {
            continueExistingFile = true;
        }
    }

    // Make sure that the path to the file exists
    QFileInfo info(filePath);
    QString absolutePath(info.absolutePath());
    if(!QFile::exists(absolutePath))
    {
        QByteArray ba(absolutePath.toLatin1());
        Con::printf("QtFileDownloader::FileDownload(): Had to create path '%s'", ba.data());
        QDir::root().mkpath(absolutePath);
    }

    QIODevice::OpenMode accessType = QIODevice::WriteOnly;
    if(continueExistingFile)
    {
        accessType = QIODevice::Append;
    }
    m_pFile = new QFile(filePath);
    if(!m_pFile->open(accessType))
    {
        QByteArray ba(filePath.toLatin1());
        Con::warnf("QtFileDownloader::FileDownload(): Could not open file for writing [%s]", ba.data());
        delete m_pFile;
        m_pFile = NULL;

        // Post the error
        Sim::postCurrentEvent(callback, new DownloadManagerCallbackEvent(DownloadManagerCallbackEvent::CALLBACK_FILE_BAD_PATH));

        return false;
    }

    m_DownloadCanceled = false;
    m_DownloadPaused = false;

    if(continueExistingFile)
    {
        // If we're to continue an existing file then pass along
        // the file's current size as if we're paused.
        m_DownloadPausedFileSize = m_pFile->size();
    }
    else
    {
        m_DownloadPausedFileSize = 0;
    }

    startRequest(m_URL);

    return true;
}

void QtFileDownloader::CancelDownload()
{
    m_DownloadCanceled = true;
    if(m_pNetReply)
    {
        m_pNetReply->abort();
    }
}

void QtFileDownloader::PauseDownload()
{
    m_DownloadPaused = true;
    if(m_pNetReply)
    {
        m_pNetReply->abort();
    }
}

void QtFileDownloader::ResumeDownload()
{
    m_DownloadPaused = false;

    if(m_DownloadCanceled)
        return;

    // Make sure there is a file to resume.
    if(!m_pFile)
        return;

    m_DownloadPausedFileSize = m_pFile->size();

    startRequest(m_URL);
}

//-----------------------------------------------------------------------------

void QtFileDownloader::startRequest(QUrl url)
{
    //QByteArray ba(url.toString().toLatin1());
    //Con::printf("QtFileDownloader::startRequest(): %s", ba.data());

    QNetworkRequest request(url);

    // Are we resuming a download?
    if(m_DownloadPausedFileSize > 0)
    {
        QByteArray rangeHeaderValue = "bytes=" + QByteArray::number(m_DownloadPausedFileSize) + "-";
        request.setRawHeader("Range", rangeHeaderValue);
    }

    m_pNetReply = QtManager::singleton()->getNetworkManager().get(request);

    connectNetworkReplySignals();
}

void QtFileDownloader::connectNetworkReplySignals()
{
    if(!m_pNetReply)
        return;

    connect(m_pNetReply, SIGNAL(finished()),                        this, SLOT(downloadFinishedSlot()));
    connect(m_pNetReply, SIGNAL(readyRead()),                       this, SLOT(dataReadyReadSlot()));
    connect(m_pNetReply, SIGNAL(downloadProgress(qint64, qint64)),  this, SLOT(downloadProgressSlot(qint64, qint64)));
    connect(m_pNetReply, SIGNAL(sslErrors(const QList<QSslError>&)),this, SLOT(sslErrorsSlot(const QList<QSslError>&)));
}

void QtFileDownloader::disconnectNetworkReplySignals()
{
    if(!m_pNetReply)
        return;

    disconnect(m_pNetReply, SIGNAL(finished()),                        this, SLOT(downloadFinishedSlot()));
    disconnect(m_pNetReply, SIGNAL(readyRead()),                       this, SLOT(dataReadyReadSlot()));
    disconnect(m_pNetReply, SIGNAL(downloadProgress(qint64, qint64)),  this, SLOT(downloadProgressSlot(qint64, qint64)));
    disconnect(m_pNetReply, SIGNAL(sslErrors(const QList<QSslError>&)),this, SLOT(sslErrorsSlot(const QList<QSslError>&)));
}

//-----------------------------------------------------------------------------

void QtFileDownloader::downloadFinishedSlot()
{
    //Con::printf("QtFileDownloader::httpFinishedSlot()");

    // Check if the download has been canceled
    if(m_DownloadCanceled || m_DownloadPaused)
    {
        // Only delete the open file if we're not paused
        if(!m_DownloadPaused && m_pFile)
        {
            m_pFile->close();
            m_pFile->remove();
            delete m_pFile;
            m_pFile = NULL;
        }

        disconnectNetworkReplySignals();

        m_pNetReply->deleteLater();
        m_pNetReply = NULL;

        return;
    }

    m_pFile->flush();
    m_pFile->close();

    // Check for error
    if(m_pNetReply->error())
    {
        QByteArray ba(m_pNetReply->errorString().toLatin1());
        Con::warnf("QtFileDownloader::httpFinishedSlot(): Download failed: %s", ba.data());
        m_pFile->remove();

        // Copy the error string into the callback data
        DownloadManagerCallbackEvent::CallbackData* data = new DownloadManagerCallbackEvent::CallbackData();
        QByteArray errorBA = m_pNetReply->errorString().toLatin1();
        char* text = new char[errorBA.size()+1];
        dMemcpy(text, errorBA, errorBA.size());
        text[errorBA.size()] = '\0';
        data->m_StringData = text;

        // Post the error
        Sim::postCurrentEvent(m_CallbackObject, new DownloadManagerCallbackEvent(DownloadManagerCallbackEvent::CALLBACK_DOWNLOAD_ERROR, data));
    }
    else
    {
        // Post the success
        DownloadManagerCallbackEvent::CallbackData* data = new DownloadManagerCallbackEvent::CallbackData();
        data->m_U32Args[0] = m_pFile->size();
        Sim::postCurrentEvent(m_CallbackObject, new DownloadManagerCallbackEvent(DownloadManagerCallbackEvent::CALLBACK_DOWNLOAD_FINISHED, data));
    }

    disconnectNetworkReplySignals();

    m_pNetReply->deleteLater();
    m_pNetReply = NULL;

    delete m_pFile;
    m_pFile = NULL;
}

void QtFileDownloader::dataReadyReadSlot()
{
    //Con::printf("QtFileDownloader::httpReadyReadSlot()");

    if(m_DownloadPaused)
        return;

    // Called whenever there is new data to write.
    if(m_pFile)
    {
        m_pFile->write(m_pNetReply->readAll());
    }
}

void QtFileDownloader::downloadProgressSlot(qint64 bytesRead, qint64 totalBytes)
{
    if(m_DownloadCanceled || m_DownloadPaused)
        return;

    //Con::printf("QtFileDownloader::updateDataReadProgressSlot(): %d / %d", bytesRead, totalBytes);

    // Post the progress
    DownloadManagerCallbackEvent::CallbackData* data = new DownloadManagerCallbackEvent::CallbackData();
    data->m_U32Args[0] = bytesRead + m_DownloadPausedFileSize;
    data->m_U32Args[1] = totalBytes + m_DownloadPausedFileSize;
    Sim::postCurrentEvent(m_CallbackObject, new DownloadManagerCallbackEvent(DownloadManagerCallbackEvent::CALLBACK_DOWNLOAD_PROGRESS, data));
}

void QtFileDownloader::sslErrorsSlot(const QList<QSslError>& errors)
{
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
    m_pNetReply->ignoreSslErrors();
}
