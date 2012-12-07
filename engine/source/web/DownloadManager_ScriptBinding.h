//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod(DownloadManager, SetDownloadProvider, bool, 3, 3, "(downloadProvider) - Set the provider of download services.\n"
                                                                "@downloadProvider The IDownloadProvider class that provides the file download services to the manager.\n")
{
    IDownloadProvider* provider = dynamic_cast<IDownloadProvider*>(Sim::findObject(argv[2]));
    if(!provider)
    {
        Con::warnf("DownloadManager::SetDownloadProvider(): Invalid download provider");
        return false;
    }

    object->SetDownloadProvider( provider );

    return true;
}

//-----------------------------------------------------------------------------

ConsoleMethod(DownloadManager, AddFileToQueue, bool, 6, 6,      "(url, path, callbackObjectId, deleteExisting) - Add a URL to download to the given file path to the download queue.\n"
                                                                "@url The fully qualified URL of the file to download.\n"
                                                                "@path The path to save the file to.  May include special path expandos to save in system standard locations.\n"
                                                                "@callbackObjectId The SimObject Id of the object to receive download callbacks, or 0 for the DownloadManager to handle.\n"
                                                                "@deleteExisting Should any previous file be deleted first?\n"
                                                                "@return Whether adding the file to the download queue was successful or not.")
{
    SimObjectId id = 0;
    SimObject* obj = Sim::findObject(argv[4]);
    if(obj)
        id = obj->getId();
    
    // Add the file to the download queue
    return object->AddFileToQueue( argv[2], argv[3], id, dAtob(argv[5]));
}

//-----------------------------------------------------------------------------

ConsoleMethod(DownloadManager, RemoveFileFromQueue, void, 3, 3, "(url) - Remove the given file's URL from the download queue.\n"
                                                                "@url The fully qualified URL of the file to download.\n")
{
    // Remove the file to the download queue
    object->RemoveFileFromQueue( argv[2] );
}

//-----------------------------------------------------------------------------

ConsoleMethod(DownloadManager, RemoveCurrentFileFromQueue, void, 2, 2,  "() - Remove the currently downloading file from the queue.\n")
{
    // Remove the currently downloading file from the queue
    object->RemoveCurrentFileFromQueue();
}

//-----------------------------------------------------------------------------

ConsoleMethod(DownloadManager, ClearQueue, void, 2, 2,          "() - Clear all pending files from the download queue.\n")
{
    // Clear the download queue
    object->ClearQueue();
}

//-----------------------------------------------------------------------------

ConsoleMethod(DownloadManager, GetQueueCount, S32, 2, 2,        "() - Get the number of files in the download queue.\n"
                                                                "@return The number of files in the download queue.")
{
    // Clear the download queue
    return object->GetQueueCount();
}

//-----------------------------------------------------------------------------

ConsoleMethod(DownloadManager, DownloadNextFile, bool, 2, 2,    "() - Start the downloading of the next file in the queue.\n"
                                                                "@return True if there was a file to download.")
{
    // Trigger the downloading of the next file in the queue
    return object->DownloadNextFile();
}

//-----------------------------------------------------------------------------

ConsoleMethod(DownloadManager, PauseDownload, void, 2, 2,      "() - Pause the current file download.\n")
{
    // Pause the current file download
    object->PauseDownload();
}

//-----------------------------------------------------------------------------

ConsoleMethod(DownloadManager, ResumeDownload, void, 2, 2,      "() - Resume the current file download.\n")
{
    // Resume the current file download
    object->ResumeDownload();
}

//-----------------------------------------------------------------------------

ConsoleMethod(DownloadManager, CancelDownload, void, 2, 2,      "() - Cancel the current file download.\n")
{
    // Cancel the current file download
    object->CancelDownload();
}

//-----------------------------------------------------------------------------

ConsoleMethod(DownloadManager, GetCurrentFileURL, const char*, 2, 2,    "() - Returns the URL of the currently downloading file.\n"
                                                                        "@return The URL of the downloading file.")
{
    DownloadManager::FileEntry* entry = object->GetCurrentFile();
    if(!entry)
    {
        return "";
    }
    else
    {
        return entry->m_pURL;
    }
}

//-----------------------------------------------------------------------------

ConsoleMethod(DownloadManager, GetCurrentFilePath, const char*, 2, 2,   "() - Returns the path of the currently downloading file.\n"
                                                                        "@return The path of the downloading file.")
{
    DownloadManager::FileEntry* entry = object->GetCurrentFile();
    if(!entry)
    {
        return "";
    }
    else
    {
        return entry->m_pPath;
    }
}

//-----------------------------------------------------------------------------

ConsoleMethod(DownloadManager, GetCurrentExpandedFilePath, const char*, 2, 2,   "() - Returns the expanded path of the currently downloading file.\n"
                                                                                "@return The expanded path of the downloading file.")
{
    DownloadManager::FileEntry* entry = object->GetCurrentFile();
    if(!entry)
    {
        return "";
    }
    else
    {
        char* buffer = Con::getReturnBuffer(1024);
        Con::expandPath(buffer, 1024, entry->m_pPath);
        return buffer;
    }
}
