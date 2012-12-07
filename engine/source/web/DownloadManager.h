//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _DOWNLOADMANAGER_H
#define _DOWNLOADMANAGER_H

#include "sim/simBase.h"
#include "collection/hashTable.h"
#include "web/IDownloadProvider.h"

class DownloadManagerCallbackEvent;

class DownloadManager : public SimObject
{
    typedef SimObject Parent;

    friend class DownloadManagerCallbackEvent;

public:
    struct FileEntry {
        // URL to download from
        StringTableEntry m_pURL;

        // Path to save downloaded file to
        StringTableEntry m_pPath;

        // Optional SimObject Id of object to call back.  If 0 then
        // the DownloadManager will handle the callback.
        SimObjectId m_CallbackObject;

        // Should any previous file be deleted first, or are we continuing
        // a download?
        bool m_DeleteExisting;

        FileEntry()
        {
            m_pURL = NULL;
            m_pPath = NULL;
            m_CallbackObject = 0;
            m_DeleteExisting = true;
        }
    };

public:
    DownloadManager();
    virtual ~DownloadManager();

    // SimObject overrides
    virtual bool onAdd();
    virtual void onRemove();

    void SetDownloadProvider(IDownloadProvider* provider);

    // Add a file to the download queue
    bool AddFileToQueue(const char* url, const char* path, SimObjectId callbackObject, bool deleteExisting);

    // Remove a file from the download queue
    void RemoveFileFromQueue(const char* url);

    // Remove the current file from the download queue
    void RemoveCurrentFileFromQueue();

    // Clear all files from the download queue
    void ClearQueue();

    // Returns the number of files in the queue
    S32 GetQueueCount();

    // Start the downloads
    bool DownloadNextFile();

    // Retrieve the current file that is downloading
    FileEntry* GetCurrentFile();

    // Pause the downloads
    void PauseDownload();

    // Resume a paused download
    void ResumeDownload();

    // Cancel the current download
    void CancelDownload();

    // Indicates if a file is currently downloading
    bool IsDownloading();

    DECLARE_CONOBJECT(DownloadManager);

protected:
    typedef HashTable<const char*, FileEntry*> FileQueueHash;
    typedef FileQueueHash::iterator FileQueueHashIterator;

    FileQueueHash m_FileQueue;

    IDownloadProvider* m_pDownloadProvider;

    // The file we're currently downloading
    FileEntry* m_pCurrentFile;

protected:
    void onWrongURLTypeCallback();
    void onBadFilePathCallback();
    void onDownloadProgressCallback(U32 bytesRead, U32 bytesTotal);
    void onDownloadErrorCallback(const char* errorText);
    void onDownloadFinishedCallback(U32 fileSize);
};

#endif // _DOWNLOADMANAGER_H
