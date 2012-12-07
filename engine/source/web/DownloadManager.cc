//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "web/DownloadManager.h"

// Script bindings.
#include "web/DownloadManager_ScriptBinding.h"

IMPLEMENT_CONOBJECT( DownloadManager );

DownloadManager::DownloadManager()
{
    m_pDownloadProvider = NULL;
    m_pCurrentFile = NULL;
}

DownloadManager::~DownloadManager()
{
    ClearQueue();

    m_pDownloadProvider = NULL;
}

//-----------------------------------------------------------------------------

bool DownloadManager::onAdd()
{
    if( !Parent::onAdd() )
        return false;

    return true;
}

void DownloadManager::onRemove()
{
    // Call parent.
    Parent::onRemove();
}

//-----------------------------------------------------------------------------

void DownloadManager::SetDownloadProvider(IDownloadProvider* provider)
{
    m_pDownloadProvider = provider;
}

//-----------------------------------------------------------------------------

bool DownloadManager::AddFileToQueue(const char* url, const char* path, SimObjectId callbackObject, bool deleteExisting)
{
    if(!url || !path)
        return false;

    FileEntry* entry = new FileEntry();
    entry->m_pURL = StringTable->insert(url, true);
    entry->m_pPath = StringTable->insert(path, true);
    entry->m_CallbackObject = callbackObject;
    entry->m_DeleteExisting = deleteExisting;

    m_FileQueue.insertEqual(entry->m_pURL, entry);

    return true;
}

void DownloadManager::RemoveFileFromQueue(const char* url)
{
    if(!url)
        return;

    StringTableEntry key = StringTable->insert(url, true);

    FileEntry* entry = m_FileQueue.find(key).getValue();

    if(!entry)
        return;

    m_FileQueue.erase(key);

    if(m_pCurrentFile == entry)
        m_pCurrentFile = NULL;

    delete entry;
}

void DownloadManager::RemoveCurrentFileFromQueue()
{
    if(!m_pCurrentFile)
        return;

    m_FileQueue.erase(m_pCurrentFile->m_pURL);

    delete m_pCurrentFile;
    m_pCurrentFile = NULL;
}

void DownloadManager::ClearQueue()
{
    for(FileQueueHashIterator itr=m_FileQueue.begin(); itr != m_FileQueue.end(); ++itr)
    {
        FileEntry* entry = itr.getValue();
        if(entry)
            delete entry;
    }

    m_FileQueue.clear();

    m_pCurrentFile = NULL;
}

S32 DownloadManager::GetQueueCount()
{
    return m_FileQueue.size();
}

//-----------------------------------------------------------------------------

bool DownloadManager::DownloadNextFile()
{
	if( m_pCurrentFile )
	{
		Con::warnf("DownloadManager::DownloadNextFile(): Currently downloading - %s", m_pCurrentFile->m_pPath);
		return false;
	}

    if(m_FileQueue.size() == 0)
        return false;

    if(!m_pDownloadProvider)
    {
        Con::warnf("DownloadManager::DownloadNextFile(): No download provider set");
        return false;
    }

    FileEntry* entry = NULL;
    for(FileQueueHashIterator itr=m_FileQueue.begin(); itr != m_FileQueue.end();)
    {
        entry = itr.getValue();
        if(!entry)
        {
            // Something is wrong with this entry.  Delete it and move on.
            m_FileQueue.erase(itr);
            continue;
        }

        // Found a valid entry
        break;
    }

    m_pCurrentFile = entry;

    m_pDownloadProvider->DownloadFile(getId(), entry->m_pURL, entry->m_pPath, entry->m_DeleteExisting);

    return true;
}

//-----------------------------------------------------------------------------

DownloadManager::FileEntry* DownloadManager::GetCurrentFile()
{
    return m_pCurrentFile;
}

//-----------------------------------------------------------------------------

void DownloadManager::PauseDownload()
{
    m_pDownloadProvider->PauseDownload();
}

void DownloadManager::ResumeDownload()
{
    m_pDownloadProvider->ResumeDownload();
}

//-----------------------------------------------------------------------------

void DownloadManager::CancelDownload()
{
    if(!m_pDownloadProvider)
    {
        Con::warnf("DownloadManager::CancelDownload(): No download provider set");
        return;
    }
}

//-----------------------------------------------------------------------------

void DownloadManager::onWrongURLTypeCallback()
{
    if(m_pCurrentFile)
    {
        SimObject* obj = Sim::findObject(m_pCurrentFile->m_CallbackObject);

        if(!obj)
        {
            // Callback on self
            Con::executef(this, 1, "onWrongURLType");
        }
        else
        {
            // Callback on object
            Con::executef(obj, 2, "onWrongURLType", this->getIdString());
        }
    }
}

void DownloadManager::onBadFilePathCallback()
{
    if(m_pCurrentFile)
    {
        SimObject* obj = Sim::findObject(m_pCurrentFile->m_CallbackObject);

        if(!obj)
        {
            // Callback on self
            Con::executef(this, 1, "onBadFilePath");
        }
        else
        {
            // Callback on object
            Con::executef(obj, 2, "onBadFilePath", this->getIdString());
        }
    }
}

void DownloadManager::onDownloadProgressCallback(U32 bytesRead, U32 bytesTotal)
{
    if(m_pCurrentFile)
    {
        SimObject* obj = Sim::findObject(m_pCurrentFile->m_CallbackObject);

        if(!obj)
        {
            // Callback on self
            Con::executef(this, 3, "onDownloadProgress", Con::getIntArg(bytesRead), Con::getIntArg(bytesTotal));
        }
        else
        {
            // Callback on object
            Con::executef(obj, 4, "onDownloadProgress", this->getIdString(), Con::getIntArg(bytesRead), Con::getIntArg(bytesTotal));
        }
    }
}

void DownloadManager::onDownloadErrorCallback(const char* errorText)
{
    if(m_pCurrentFile)
    {
        SimObject* obj = Sim::findObject(m_pCurrentFile->m_CallbackObject);

        if(!obj)
        {
            // Callback on self
            Con::executef(this, 2, "onDownloadError", errorText);
        }
        else
        {
            // Callback on object
            Con::executef(obj, 3, "onDownloadError", this->getIdString(), errorText);
        }
    }
}

void DownloadManager::onDownloadFinishedCallback(U32 fileSize)
{
    if(m_pCurrentFile)
    {
        SimObject* obj = Sim::findObject(m_pCurrentFile->m_CallbackObject);

        if(!obj)
        {
            // Callback on self
            Con::executef(this, 2, "onDownloadFinished", Con::getIntArg(fileSize));
        }
        else
        {
            // Callback on object
            Con::executef(obj, 3, "onDownloadFinished", this->getIdString(), Con::getIntArg(fileSize));
        }
    }
}
