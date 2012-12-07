//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _IDOWNLOADPROVIDER_H
#define _IDOWNLOADPROVIDER_H

class IDownloadProvider
{
public:
    virtual void DownloadFile(S32 callback, const char* url, const char* path, bool deleteExisting) = 0;

    virtual void CancelDownload() = 0;

    virtual void PauseDownload() = 0;
    
    virtual void ResumeDownload() = 0;
};

#endif // _IDOWNLOADPROVIDER_H
