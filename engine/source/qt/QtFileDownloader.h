//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _QT_FILEDOWNLOADER_H
#define _QT_FILEDOWNLOADER_H

#include "sim/simBase.h"

#include <QObject>
#include <QNetworkAccessManager>
#include <QUrl>
#include <QFile>

class QtFileDownloader : public QObject
{
    Q_OBJECT

public slots:
    void downloadFinishedSlot();
    void dataReadyReadSlot();
    void downloadProgressSlot(qint64 bytesRead, qint64 totalBytes);
    void sslErrorsSlot(const QList<QSslError>& errors);

public:
    QtFileDownloader();
    virtual ~QtFileDownloader();

    // Download a file from the given url and write it to the given path
    bool FileDownload(const char* url, const char* path, SimObjectId callback, bool deleteExisting);

    // Stop the current download
    void CancelDownload();

    // Pause the current download
    void PauseDownload();

    // Resume a paused download
    void ResumeDownload();

protected:
    // The Qt network reply object that notifies on progress
    QNetworkReply* m_pNetReply;

    // URL to download
    QUrl m_URL;

    // File we're downloading to
    QFile* m_pFile;

    // The SimObject to send events to
    SimObjectId m_CallbackObject;

    // Indicates that the download should be canceled
    bool m_DownloadCanceled;

    // Indicates that the download should be paused
    bool m_DownloadPaused;

    // Last size of downloaded file when pause happened
    U32 m_DownloadPausedFileSize;

protected:
    // Start the file download request
    void startRequest(QUrl url);

    // Connect up the current QNetworkReply to our slots
    void connectNetworkReplySignals();

    // Disconnect the current QNetworkReply from our slots
    void disconnectNetworkReplySignals();
};

#endif // _QT_FILEDOWNLOADER_H
