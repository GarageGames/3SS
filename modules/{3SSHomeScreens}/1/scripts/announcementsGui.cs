//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function AnnouncementsGui::onWake(%this)
{
    %this.downloadStarted = false;
    %this.updateScheduleId = "";
    
    if (!isObject(%this.xmlDocument))
    {
        %this.xmlDocument = new SimXMLDocument();
        %this.announcementSet = new SimSet();
    }
    else
    {
        %this.xmlDocument.clear();
        %this.announcementSet.clear();
    }
    
    HttpQueue.addPostRequest("http://dev.3stepstudio.com/client/announcements", "", %this);
    
    if ($UpdateReadyToInstall)
    {
        AGUpdateButton.NormalImage = "{3SSHomeScreens}:updateAvailable_normal";
		AGUpdateButton.HoverImage = "{3SSHomeScreens}:updateAvailable_hover";
		AGUpdateButton.InactiveImage = "{3SSHomeScreens}:updateAvailable_hover";
		AGUpdateButton.DownImage = "{3SSHomeScreens}:updateAvailable_hover";
    }
    else
    {
        AGUpdateButton.NormalImage = "{3SSHomeScreens}:upToDate";
		AGUpdateButton.HoverImage = "{3SSHomeScreens}:upToDate";
		AGUpdateButton.InactiveImage = "{3SSHomeScreens}:upToDate";
		AGUpdateButton.DownImage = "{3SSHomeScreens}:upToDate";
		AGUpdateButton.setActive(false);
    }
}

function AnnouncementsGui::refresh(%this)
{
    if ($UpdateReadyToInstall)
    {
        AGUpdateButton.setActive(true);
        AGUpdateButton.NormalImage = "{3SSHomeScreens}:updateAvailable_normal";
		AGUpdateButton.HoverImage = "{3SSHomeScreens}:updateAvailable_hover";
		AGUpdateButton.InactiveImage = "{3SSHomeScreens}:updateAvailable_hover";
		AGUpdateButton.DownImage = "{3SSHomeScreens}:updateAvailable_hover";
    }
    else
    {
        AGUpdateButton.NormalImage = "{3SSHomeScreens}:upToDate";
		AGUpdateButton.HoverImage = "{3SSHomeScreens}:upToDate";
		AGUpdateButton.InactiveImage = "{3SSHomeScreens}:upToDate";
		AGUpdateButton.DownImage = "{3SSHomeScreens}:upToDate";
    }
    
    for(%i = 0; %i < %this.announcementSet.getCount(); %i++)
    {
        %pageName = "AnnouncementPage" @ %i;
        %page = AGAnnouncementsContainer.findObjectByInternalName(%pageName);
        
        if (isObject(%page))
            %page.setVisible(true);
    }
    
    %page = AGAnnouncementsContainer.findObjectByInternalName("AnnouncementPage0");
    
    if (isObject(%page))
        %page.performClick();
}

function AnnouncementsGui::setPage(%this, %pageNumber)
{
    cancel(%this.updateScheduleId);
    
    %announcement = %this.announcementSet.getObject(%pageNumber);
    
    if (isObject(%announcement))
    {
        AGTitleText.text = %announcement.title;
    
        if (%announcement.imagePreview !$= "")
            AGPreviewImage.image = %announcement.imagePreview;
        
        AGBodyText.setText(%announcement.body);
    
        %pageName = "AnnouncementPage" @ %pageNumber++;
        %page = AGAnnouncementsContainer.findObjectByInternalName(%pageName);
        
        if (!isObject(%page))
            %page = AGAnnouncementsContainer.findObjectByInternalName("AnnouncementPage0");        
        
        %this.updateScheduleId = %page.schedule(10000, performClick);
    }
}

function AGUpdateButton::onClick(%this)
{    
    EditorEventManager.postEvent("_UpdateInstallStart", "");
}

function AnnouncementsGui::onRequestFinished(%this, %manager, %response, %responseCode)
{
    %this.xmlDocument.parse(%response);
    %this.xmlDocument.pushFirstChildElement("LatestAnnouncements");
    
    %result = %this.xmlDocument.pushFirstChildElement("Announcement");
    
    $DebugTestVariable = 0;
    while(%result)
    {
        %this.creatAnnouncementEntry(%this.xmlDocument);
        %result = %this.xmlDocument.nextSiblingElement("Announcement");
    }
}

function AnnouncementsGui::creatAnnouncementEntry(%this, %xmlDocument)
{
    %xmlDocument.pushFirstChildElement("Date");
    %date = %xmlDocument.getData();
    %xmlDocument.popElement();
    
    %xmlDocument.pushFirstChildElement("Title");
    %title = %xmlDocument.getData();
    %xmlDocument.popElement();
    
    %xmlDocument.pushFirstChildElement("Image");
    %image = %xmlDocument.getData();
    %xmlDocument.popElement();
    
    %xmlDocument.pushFirstChildElement("Body");
    %body = %xmlDocument.getData();
    %xmlDocument.popElement();
    
    %xmlDocument.pushFirstChildElement("MoreInfo");
    %link = %xmlDocument.getData();
    %xmlDocument.popElement();
    
    %baseImageName = "editorImages/" @ fileName(%image);    
    %imagePath = expandPath(getPrefsPath(%baseImageName));
    
    DownloadQueue.AddFileToQueue(%image, %imagePath, %this, true);
    
    if (!%this.downloadStarted)
    {
        DownloadQueue.DownloadNextFile();
        %this.downloadStarted = true;
    }
    
    %newAnnouncement = new ScriptObject()
    {
        date = %date;
        title = %title;
        imagePath = %imagePath;
        imagePreview = "";
        body = %body;
        link = %link;
    };
    
    %this.announcementSet.add(%newAnnouncement);
}

function AnnouncementsGui::onDownloadFinished(%this, %downloadManager, %filesize)
{
    %newFile = %downloadManager.GetCurrentExpandedFilePath();
    
    %imageAsset = new ImageAsset()
    {
        AssetInternal = "1";
        ImageFile = %newFile;
    };
    
    %privateAsset = AssetDatabase.addPrivateAsset(%imageAsset);
    
    for (%i = 0; %i < %this.announcementSet.getCount(); %i++)
    {
        %announcementFile = %this.announcementSet.getObject(%i).imagePath;
        
        if (!stricmp(%newFile, %announcementFile))
        {
            %this.announcementSet.getObject(%i).imagePreview = %privateAsset;
            AGPreviewImage.image = %privateAsset;
            break;
        }
    }
    
    %downloadManager.RemoveCurrentFileFromQueue();
    
    %downloadManager.DownloadNextFile();
    
    %this.refresh();
}

function AnnouncementsGui::onWrongURLType(%this, %downloadManager)
{
    %downloadManager.RemoveCurrentFileFromQueue();
    %downloadManager.DownloadNextFile();
    %this.refresh();
}

function AnnouncementsGui::onBadFilePath(%this, %downloadManager)
{
    warn("AnnouncementsGui::onBadFilePath()");
    %downloadManager.RemoveCurrentFileFromQueue();
    %downloadManager.DownloadNextFile();
    %this.refresh();
}

function AnnouncementsGui::onDownloadProgress(%this, %downloadManager, %bytesRead, %bytesTotal)
{
    if (isDebugBuild())
    {
        %currentFile = %downloadManager.GetCurrentFileURL();
        echo("AnnouncementsGui::onDownloadProgress(): " @ %currentFile SPC %bytesRead @ " / " @ %bytesTotal);
    }
}

function AnnouncementsGui::onDownloadError(%this, %downloadManager, %errorText)
{
    error("AnnouncementsGui::onDownloadError(): " @ %errorText);
    %downloadManager.RemoveCurrentFileFromQueue();
    %downloadManager.DownloadNextFile();
    %this.refresh();
}

function AGStoreButton::onClick(%this)
{
    WebWindowGui.display("http://3stepstudio.com/shop");
}