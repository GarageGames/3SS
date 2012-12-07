//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _MANIFESTWORKER_H
#define _MANIFESTWORKER_H

#include <QMap>
#include <QString>

class QFile;

class ManifestWorker
{
public:
    ManifestWorker();
    virtual ~ManifestWorker();

    // Load the manifest file
    bool LoadManifestFile();

    // Read the manifest file and produce the file list
    bool ReadManifestFile(int& totalBytes);

    // Perform some work on copy files in the manifest
    bool ProcessManifestTick(int& totalBytesCopied);

    // Restart the calling application
    bool RestartApp();

protected:
    QFile*  m_ManifestFile;

    // The name of the application to execute once we're done
    QString m_ApplicationFileName;

    // The number of files we should process according to the manifest
    int m_NumFiles;

    typedef QMap<QString, QString> FilesMap;

    // The list of files in the manifest
    FilesMap m_FileList;

    // The total number of bytes that will be copied
    int m_TotalBytes;

    // The total number of bytes that have been copied so far
    int m_TotalBytesCopied;

    // Points to the current file being copied in the file list
    FilesMap::const_iterator m_CurrentFileItr;

    // The file currently being copied from
    QFile* m_CurrentFile;

    // The copy destination file
    QFile* m_CopyDestination;

protected:
    void CloseManifestFile();

    void CloseCopyFiles();

    void ClearFileList();
};

#endif  // _MANIFESTWORKER_H
