//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function testDownloadManager()
{
    // Add files to the queue
    DownloadQueue.AddFileToQueue("http://www.gnometech.com/gnomecam/pic.jpg", "^DocumentsFileLocation/3SSTesting/gnomepic1.jpg", 0, true);
    DownloadQueue.AddFileToQueue("http://www.gnometech.com/gnomecam/gnomecam0001.jpg", "^DocumentsFileLocation/3SSTesting/gnomepic2.jpg", 0, true);
    
    // Start downloading the first file
    DownloadQueue.DownloadNextFile();
}
//-----------------------------------------------------------------------------

function DownloadQueue::onWrongURLType(%this)
{
    warn("DownloadQueue::onWrongURLType()");
}

function DownloadQueue::onBadFilePath(%this)
{
    warn("DownloadQueue::onBadFilePath()");
}

function DownloadQueue::onDownloadProgress(%this, %bytesRead, %bytesTotal)
{
    if (isDebugBuild())
    {
        %currentFile = %this.GetCurrentFileURL();
        echo("DownloadQueue::onDownloadProgress(): " @ %currentFile SPC %bytesRead @ " / " @ %bytesTotal);
        echo("DownloadQueue::onDownloadProgress(): " @ %bytesRead @ " / " @ %bytesTotal);
    }
}

function DownloadQueue::onDownloadError(%this, %errorText)
{
    error("DownloadQueue::onDownloadError(): " @ %errorText);
}

function DownloadQueue::onDownloadFinished(%this, %fileSize)
{
    echo("DownloadQueue::onDownloadFinished(): " @ %this.GetCurrentExpandedFilePath() @ "  Final size: " @ %fileSize);
    %this.RemoveCurrentFileFromQueue();
    
    %this.DownloadNextFile();
}
