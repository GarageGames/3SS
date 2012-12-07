//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "manifestWorker.h"

#include <QFile>
#include <QFileInfo>
#include <QProcess>

#include "util.h"

extern QString gsLastErrorMessage;

ManifestWorker::ManifestWorker()
{
    m_ManifestFile = NULL;
    m_NumFiles = 0;
    m_TotalBytes = 0;
    m_TotalBytesCopied = 0;
    m_CurrentFile = NULL;
    m_CopyDestination = NULL;
}

ManifestWorker::~ManifestWorker()
{
    CloseManifestFile();
    ClearFileList();
}

void ManifestWorker::CloseManifestFile()
{
    if(m_ManifestFile)
    {
        m_ManifestFile->close();
        delete m_ManifestFile;
        m_ManifestFile = NULL;
    }
}

void ManifestWorker::ClearFileList()
{
    m_FileList.clear();
}

void ManifestWorker::CloseCopyFiles()
{
    if(m_CopyDestination)
    {
        delete m_CopyDestination;
        m_CopyDestination = NULL;
    }

    if(m_CurrentFile)
    {
        delete m_CurrentFile;
        m_CurrentFile = NULL;
    }
}

bool ManifestWorker::LoadManifestFile()
{
    // Close any previous manifest file
    CloseManifestFile();
    ClearFileList();

    m_ApplicationFileName.clear();
    m_NumFiles = 0;

    // Open the file
    m_ManifestFile = new QFile(MANIFEST_FILE);
    if(!m_ManifestFile->open(QIODevice::ReadOnly | QIODevice::Text))
    {
        delete m_ManifestFile;
        m_ManifestFile = NULL;
        gsLastErrorMessage = "Cannot open file manifest.";
        return false;
    }

    // Process the manifest file
    bool headerFound = false;
    bool osFound = false;
    bool applicationFound = false;
    bool fileCountFound = false;
    while(!m_ManifestFile->atEnd() && !(headerFound && applicationFound && fileCountFound))
    {
        QString line = m_ManifestFile->readLine();

        // If we've not found the header line then attempt to process it
        if(!headerFound)
        {
            QString column1 = line.section('\t', 0, 0).trimmed();
            if(QString::compare(column1, "Manifest", Qt::CaseInsensitive) == 0)
            {
                // Found the header line, but is it the correct version?
                QString column2 = line.section('\t', 1, 1).trimmed();
                if(column2.toInt() == 1)
                {
                    // Correct version
                    headerFound = true;

                    continue;
                }
            }

            // Still haven't found the header.  Cannot process other data until we do.
            continue;
        }

        // If the OS name has not been found then attempt to process it
        if(!osFound)
        {
            QString column1 = line.section('\t', 0, 0).trimmed();
            if(QString::compare(column1, "OS", Qt::CaseInsensitive) == 0)
            {
                // Found the application name line.
                QString os = line.section('\t', 1, 1).trimmed();
                if(os.isEmpty())
                {
                    // There is a problem with the application name
                    gsLastErrorMessage = "Could not find application name in manifest.";
                    return false;
                }

#ifdef _WINDOWS
                if(QString::compare(os, "Windows", Qt::CaseInsensitive) != 0)
                {
                    // The manifest file is for the wrong OS
                    gsLastErrorMessage = "Manifest file is meant for a different operating system.";
                    return false;
                }
#endif
                // Have the application name
                osFound = true;
                continue;
            }
        }

        // If the application file name has not been found then attempt to process it
        if(!applicationFound)
        {
            QString column1 = line.section('\t', 0, 0).trimmed();
            if(QString::compare(column1, "Application", Qt::CaseInsensitive) == 0)
            {
                // Found the application name line.
                m_ApplicationFileName = line.section('\t', 1, 1).trimmed();
                if(m_ApplicationFileName.isEmpty())
                {
                    // There is a problem with the application name
                    gsLastErrorMessage = "Could not find application name in manifest.";
                    return false;
                }

                // Have the application name
                applicationFound = true;
                continue;
            }
        }

        // If the file count has not been found then attempt to process it
        if(!fileCountFound)
        {
            QString column1 = line.section('\t', 0, 0).trimmed();
            if(QString::compare(column1, "Files", Qt::CaseInsensitive) == 0)
            {
                // Found the application name line.
                QString fileCountString = line.section('\t', 1, 1).trimmed();
                if(fileCountString.isEmpty())
                {
                    // There is a problem with the file count
                    gsLastErrorMessage = "Could not find file count in manifest.";
                    return false;
                }

                // Have the file count
                m_NumFiles = fileCountString.toInt();
                fileCountFound = true;
                continue;
            }
        }
    }

    if(!headerFound || !applicationFound || !fileCountFound)
    {
        gsLastErrorMessage = "File manifest is malformed.";
        return false;
    }

    return true;
}

bool ManifestWorker::ReadManifestFile(int& totalBytes)
{
    if(!m_ManifestFile)
    {
        // We shouldn't be here
        return false;
    }

    // Read in the file list
    int filesRead = 0;
    while(!m_ManifestFile->atEnd() && (filesRead != m_NumFiles))
    {
        QString line = m_ManifestFile->readLine();

        QString filePath = line.section('\t', 0, 0).trimmed();
        QString copyToPath = line.section('\t', 1, 1).trimmed();

        if(filePath.isEmpty() || copyToPath.isEmpty())
        {
            // Neither of these columns should not be empty
            gsLastErrorMessage = "File manifest is missing data.";
            return false;
        }

        // Push the files to the map
        m_FileList.insert(filePath, copyToPath);
        ++filesRead;
    }

    if(filesRead != m_NumFiles)
    {
        // There is a problem with the file list
        gsLastErrorMessage = "Manifest file list does not match file count";
        return false;
    }

    // Sum up the total number of bytes that we will be copying
    totalBytes = 0;
    FilesMap::const_iterator itr = m_FileList.constBegin();
    while (itr != m_FileList.constEnd())
    {
        QString path = ROOT_PATH + itr.key();
        QFile file(path);
        if(!file.open(QIODevice::ReadOnly))
        {
            // Problem with reading in the file
            gsLastErrorMessage = "Could not open file to copy: " + path;
            return false;
        }

        // Add the file size to the total
        totalBytes += file.size();
        file.close();

        // Next file in the list
        ++itr;
    }

    // Indicate the first file to process in the next step
    m_CurrentFileItr = m_FileList.constBegin();

    // Clear some other values
    m_TotalBytesCopied = 0;

    return true;
}

bool ManifestWorker::ProcessManifestTick(int& totalBytesCopied)
{
    if(m_CurrentFileItr == m_FileList.constEnd())
    {
        // We should have read in the total number of bytes by now
        CloseCopyFiles();
        gsLastErrorMessage = "File list over run while processing manifest.";
        return false;
    }

    // Make sure we have a file to copy from and to
    if(!m_CurrentFile)
    {
        // Open the file to copy from
        QString path = ROOT_PATH + m_CurrentFileItr.key();
        m_CurrentFile = new QFile(path);
        if(!m_CurrentFile->open(QIODevice::ReadOnly))
        {
            // Problem reading the file
            CloseCopyFiles();
            gsLastErrorMessage = "Could not open file to copy: " + path;
            return false;
        }

        // Open the file to copy to.  Delete it if it already exists.
        //QString relativePath = ROOT_PATH + m_CurrentFileItr.value();
        //QFileInfo info(relativePath);
        //path = info.canonicalFilePath();
        path = ROOT_PATH + m_CurrentFileItr.value();
        if(QFile::exists(path))
        {
            bool result = QFile::remove(path);
            if(!result)
            {
                // Problem deleting the old file
                CloseCopyFiles();
                gsLastErrorMessage = "Could not remove old file: " + path;
                return false;
            }
        }
        m_CopyDestination = new QFile(path);
        if(!m_CopyDestination->open(QIODevice::ReadWrite))
        {
            // Problem creating the new file
            CloseCopyFiles();
            gsLastErrorMessage = "Could not create new copy of file: " + path;
            return false;
        }
    }

    // Copy a chunk of data
    static char buffer[MAX_BYTES_TO_COPY];
    qint64 bytesRead = m_CurrentFile->read(buffer, MAX_BYTES_TO_COPY);
    if(bytesRead == -1)
    {
        // Problem reading the file
        CloseCopyFiles();
        gsLastErrorMessage = "Unable to read from file to copy.";
        return false;
    }
    else if(bytesRead == 0 || bytesRead < MAX_BYTES_TO_COPY)
    {
        // Finished reading this file
        if(bytesRead != 0)
        {
            // Write out the data
            bool result = m_CopyDestination->write(buffer, bytesRead);
            if(!result)
            {
                // Error writing the data
                CloseCopyFiles();
                gsLastErrorMessage = "Unable to write to file.";
                return false;
            }

            m_TotalBytesCopied += bytesRead;
        }

        // Move on to the next file
        CloseCopyFiles();
        ++m_CurrentFileItr;
    }
    else
    {
        // Write out the data
        bool result = m_CopyDestination->write(buffer, bytesRead);
        if(!result)
        {
            // Error writing the data
            CloseCopyFiles();
            gsLastErrorMessage = "Unable to write to file.";
            return false;
        }

        m_TotalBytesCopied += bytesRead;
    }

    totalBytesCopied = m_TotalBytesCopied;

    return true;
}

bool ManifestWorker::RestartApp()
{
    if(m_ApplicationFileName.isEmpty())
    {
        gsLastErrorMessage = "Missing application to restart.";
        return false;
    }

    QString path = ROOT_PATH + m_ApplicationFileName;
    bool result = QProcess::startDetached(path, QStringList(), QString(ROOT_PATH));

    if(!result)
    {
        gsLastErrorMessage = "Unable to start up application: " + path;
        return false;
    }

    return true;
}
