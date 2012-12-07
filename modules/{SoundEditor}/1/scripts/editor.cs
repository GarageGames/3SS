//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//--------------------------------
// Sound Tool Help
//--------------------------------
function SoundEditorHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/" @ $LoadedTemplate.Name @ "/soundimporter/");
}

// --------------------------------------------------------------------
$musicAudioType = 1;
$effectsAudioType = 2;
// --------------------------------------------------------------------

function launchSoundImporter( %soundProfile )
{
    activatePackage(TSSSoundEditorPackage);
    //trace ( true );
    EditorSoundPlayer.soundDuration = "";
    EditorSoundPlayer.bufferPos = "";
    SoundEditor.assetSaved = false;
    SoundEditor.soundAssetID = "";    
    SoundEditor.previewSoundHandle = 0;

    // Is this a sound profile?
    %soundAsset = AssetDatabase.acquireAsset(%soundProfile);
    if ( !isObject(%soundAsset) )
    {
        // No, so create a new one.
        SoundEditor.isNewProfile = true;
        SoundEditor.soundAssetID = SoundEditor.createPrivateAsset("", "NewSound");
    }
    else
    {
        SoundEditor.isNewProfile = false;
        SoundEditor.createAssetSnapshot(%soundProfile);
        SoundEditor.createProxyProfile(%soundAsset.AssetName);
    }

    SoundImporterSoundName.initialize($SIAssetNameTextEditMessageString);
    SoundImporterFilename.initialize($SIAssetLocationTextEditMessageString);

    if (SoundEditor.proxyProfile.name !$= "")
        SoundImporterSoundName.setText( SoundEditor.proxyProfile.name );

    // Update importer GUI.   
    SoundEditor.UpdateImporterGui();

    // Reset tag list.
    SoundImporterTagList.refresh(0);
    SoundImporterTagContainer.deleteContents(true);

    // Populdate tag container.   
    SoundImporterTagContainer.populateTagList(SoundEditor.soundAssetID);

    // Auto-apply Tag if we are Filtered 
    if (AssetLibrary.tagFilter !$= "Any")
        SoundEditor.applyTag(AssetLibrary.tagFilter);

    // Show importer.
    Canvas.pushDialog( SoundImporterGui );  
}

// --------------------------------------------------------------------

function cancelSoundImporter()
{
    // Stop any preview sound.
    Si_SoundStopButton.onClick();

    if (SoundEditor.isNewProfile)
        cleanTempAssets();

    // Delete proxy sound profile.
    if ( isObject(SoundEditor.proxyProfile) )
        SoundEditor.proxyProfile.delete();   

    // Remove GUI.
    Canvas.popDialog( SoundImporterGui );
    //trace ( false );
}

// --------------------------------------------------------------------

function SoundImporterGui::onSleep(%this)
{
    Si_SoundStopButton.onClick();
    SoundEditor.close();
    //trace ( false );
}

function SoundEditor::createProxyProfile(%this, %proxyName)
{
    %soundAsset = AssetDatabase.acquireAsset(%this.soundAssetID);
    %proxyProfile = new ScriptObject()
    {   
        name = %proxyName;
        filename = %soundAsset.AudioFile;
        Looping = %soundAsset.Looping;
        volume = (%soundAsset.Volume > 0 ? %soundAsset.Volume : 1);
        volumeChannel = (%soundAsset.VolumeChannel > 0 ? %soundAsset.VolumeChannel : $effectsAudioType);
        streaming = %soundAsset.Streaming;
    };
    AssetDatabase.releaseAsset(%this.soundAssetID);
    %this.proxyProfile = %proxyProfile;
}

function SoundEditor::createAssetSnapshot(%this, %originalAssetID)
{
    %this.soundAssetID = %originalAssetID;
    %this.assetSnapshot = new AssetSnapshot();
    AssetDatabase.getAssetSnapshot(%this.assetSnapshot, %originalAssetID);
    
    // not really part of the snapshot, but we should keep a list of tags so 
    // that any we add are removed if we cancel the edit.

    %assetTagList = "";    
    %assetTagManifest = AssetDatabase.getAssetTags();
    %assetTagCount = %assetTagManifest.getAssetTagCount(%originalAssetID);
    for (%i = 0; %i < %assetTagCount; %i++)
    {
        %tag = %assetTagManifest.getAssetTag(%originalAssetID, %i);
        %assetTagList = %assetTagList @ " " @ %tag;
    }
    %this.holdTagList = trim(%assetTagList);
}

function SoundEditor::restoreAssetSnapshot(%this)
{
    AssetDatabase.setAssetSnapshot(%this.assetSnapshot, %this.soundAssetID);

    // restore original object tags.
    %assetTagManifest = AssetDatabase.getAssetTags();
    %assetTagCount = %assetTagManifest.getAssetTagCount(%this.soundAssetID);
    for (%i = 0; %i < %assetTagCount; %i++)
    {
        // clear all tags.
        %tag = %assetTagManifest.getAssetTag(%this.soundAssetID, %i);
        %assetTagManifest.unTag(%this.soundAssetID, %tag);
    }
    %count = getWordCount(%this.holdTagList);
    for ( %i = 0; %i < %count; %i++ )
    {
        // restore original tags
        %tag = getWord(%this.holdTagList, %i);
        %assetTagManifest.tag(%this.soundAssetID, %tag);
    }
}

function SoundEditor::addTag(%this, %tag)
{
    if (%tag $= "")
        return;

    %assetTagManifest = AssetDatabase.getAssetTags();
    %assetTagCount = %assetTagManifest.getAssetTagCount(%this.soundAssetID);
    for (%i = 0; %i < %assetTagCount; %i++)
    {
        %tempTag = %assetTagManifest.getAssetTag(%this.soundAssetID, %i);
        if (%tag $= %tempTag)
        {
            //echo(" @@@ asset already has tag " @ %tag);
            return;
        }
    }
    %assetTagManifest.tag(%this.soundAssetID, %tag);
    SoundImporterTagContainer.addTagItem(%tag);
    SoundImporterTagContainer.populateTagList(%this.soundAssetID);
}

function SoundEditor::copyTags(%this, %sourceAsset, %targetAsset)
{
    %assetTagManifest = AssetDatabase.getAssetTags();
    %assetTagCount = %assetTagManifest.getAssetTagCount(%sourceAsset);
    %targetTags = %assetTagManifest.getAssetTagCount(%targetAsset);
    for (%i = 0; %i < %assetTagCount; %i++)
    {
        %tag = %assetTagManifest.getAssetTag(%sourceAsset, %i);
        for (%j = 0; %j <= %targetTags; %j++)
        {
            if (%targetTags > 0)
            {
                %tempTargetTag = %assetTagManifest.getAssetTag(%targetAsset, %j);
                if (%tempTargetTag $= %tag)
                {
                    //echo(" @@@ asset already has tag " @ %tag);
                    continue;
                }
            }
            else
                %assetTagManifest.tag(%targetAsset, %tag);
        }
    }
}

function SoundEditor::clearTags(%this, %assetID)
{
    %assetTagManifest = AssetDatabase.getAssetTags();
    %assetTagCount = %assetTagManifest.getAssetTagCount(%assetID);
    for (%i = 0; %i < %assetTagCount; %i++)
    {
        %tag = %assetTagManifest.getAssetTag(%sourceAsset, %i);
        %assetTagManifest.untag(%assetID, %tag);
    }
}

function SoundEditor::applyTag(%this, %tag)
{
}

function SoundEditor::removeTag(%this, %tag)
{
    %assetTagManifest = AssetDatabase.getAssetTags();
    %assetTagManifest.untag(%this.soundAssetID, %tag);
    SoundImporterTagContainer.removeTagItem(%tag);
    %this.UpdateImporterGui();
}

function SoundEditor::deleteTagFromList(%this, %tag)
{
}

// --------------------------------------------------------------------

function SoundEditor::SoundImporterValidateSoundName( %this )
{  
    %oldName = SoundImporterSoundName.getText();

    // User typed something, but ended up using the original name
    if(%oldName $= %this.proxyProfile.name)
        return;

    if (%oldname $= "")
        %newName = %this.getValidSoundName("NewSound");
    else
        %newName = %this.getValidSoundName(%oldName);

    // Set name.
    %this.proxyProfile.name = %newName;

    // Update name.
    if (%this.proxyProfile.name !$= "")
        SoundImporterSoundName.setText( %this.proxyProfile.name );
}

// --------------------------------------------------------------------

function SoundEditor::UpdateImporterGui( %this )
{
    SoundImporterTagContainer.populateTagList(%this.soundAssetID);
    // Fetch proxy profile.
    %proxyProfile = %this.proxyProfile;

    %proxyAsset = AssetDatabase.acquireAsset(%this.soundAssetID);
    
    if (%proxyAsset !$= "")
    {
        // Update filename.
        if (%proxyAsset.AudioFile !$= "")
            SoundImporterFilename.setText( %proxyAsset.audioFile );

        // Update proxy asset name.
        if (%proxyProfile.name !$= "")
            SoundImporterSoundName.setText( %proxyProfile.name );

        // Update "looping".
        SoundImporterLooping.setValue( %proxyAsset.Looping );

        // Update "music".
        %isMusic = (%proxyAsset.VolumeChannel == $musicAudioType);
        SoundImporterChannel.setValue(%isMusic);
    }
    
    AssetDatabase.releaseAsset(%this.soundAssetID);
    
    Si_SaveButton.update(); 
}

// --------------------------------------------------------------------

function SoundEditor::soundFileBrowser( %this )
{
    // Fetch proxy profile.
    %proxyProfile = %this.proxyProfile;

    // Create file dialog.
    %fileDialog = new OpenFileDialog()
    {
        Filters = $T2D::SoundSpec;
        DefaultFile = "";
        ChangePath = false;
        MustExist = true;
        MultipleFiles = false;
    };

    // Update filename.        
    if( %fileDialog.Execute() )
    {
        %fileName = %fileDialog.FileName;

        %proxyProfile.filename = %fileName;

        // Calculate target filename.
        %targetFilename = expandPath("^{UserAssets}/audio/" @ fileName(%proxyProfile.filename) );

        // Is the file in the correct target location?
        if ( %targetFilename !$= %proxyProfile.filename )
        {
            // No, so copy.
            if ( !pathCopy( %proxyProfile.filename, %targetFilename, true ) )
            {
                warn( " @@@ Target file '" @ %targetFilename @ " already exists.  File not copied." );
            }
        }

        %proxyProfile.filename = %targetFilename;

        %proxyAsset = AssetDatabase.acquireAsset(%this.soundAssetID);
        %proxyAsset.AudioFile = %this.stripExtension( %targetFilename );
        AssetDatabase.releaseAsset(%this.soundAssetID);

        EditorSoundPlayer.stop();
    }
    else
    {
        if (%proxyProfile.filename !$= "")
            SoundImporterFilename.initialize($SIAssetLocationTextEditMessageString);
        else
            SoundImporterFilename.setText(%proxyProfile.filename);
    }
    %fileDialog.delete();

    // Update importer GUI.   
    %this.UpdateImporterGui();
}

// --------------------------------------------------------------------

function SoundEditor::createPrivateAsset(%this, %fileName, %assetName)
{
    %privateAsset = new AudioAsset()
    {
        AudioFile = %fileName;
        Looping = false;
        Volume = 1.0;
        VolumeChannel = $effectsAudioType;
        Streaming = false;
    };
    %this.createProxyProfile(%this.getValidSoundName("NewSound"));
    %newAssetID = AssetDatabase.addPrivateAsset(%privateAsset);
    return %newAssetID;
}

// --------------------------------------------------------------------

function SoundEditor::createSoundAsset(%this, %name)
{
    %proxyAsset = AssetDatabase.acquireAsset(%this.soundAssetID);

    %fileName = expandPath("^{UserAssets}/audio/" @ fileName(%proxyAsset.AudioFile));

    %soundAsset = new AudioAsset();
    %soundAsset.AssetName = %name;
    %soundAsset.AudioFile = %fileName;
    %soundAsset.Looping = %proxyAsset.Looping;
    %soundAsset.Volume = %proxyAsset.Volume;
    %soundAsset.VolumeChannel = %proxyAsset.VolumeChannel;
    %soundAsset.Streaming = %proxyAsset.Streaming;

    AssetDatabase.releaseAsset(%this.soundAssetID);

    TamlWrite(%soundAsset, expandPath("^{UserAssets}/audio/" @ %soundAsset.AssetName @ ".asset.taml"));

    %userAssetModule = ModuleDatabase.findModule("{UserAssets}", 1);
    AssetDatabase.addSingleDeclaredAsset(%userAssetModule, expandPath("^{UserAssets}/audio/" @ %soundAsset.AssetName @ ".asset.taml"));
    %newAssetID = "{UserAssets}:" @ %soundAsset.AssetName;

    return %newAssetID;
}

// --------------------------------------------------------------------

function SoundEditor::getValidSoundName(%this, %nameBase)
{
    %number = 0;
    %tempName = %nameBase;
    %query = new AssetQuery();
    AssetDatabase.findAssetName(%query, %tempName);
    while ( %query.getCount() > 0 )
    {
        %tempName = %nameBase @ %number++;
        AssetDatabase.findAssetName(%query, %tempName);
    }
    return %tempName;
}

// --------------------------------------------------------------------

function SoundEditor::updateLooping( %this )
{
    %this.SoundImporterValidateSoundName();

    // Fetch proxy profile.
    %proxyProfile = %this.proxyProfile;

    // Update profile description.
    %proxyProfile.Looping = SoundImporterLooping.getValue();

    %proxyAsset = AssetDatabase.acquireAsset(%this.soundAssetID);
    %proxyAsset.Looping = %proxyProfile.Looping;
    AssetDatabase.releaseAsset(%this.soundAssetID);

    // Update importer GUI.   
    %this.UpdateImporterGui();   
}

function SoundEditor::updateChannel( %this )
{
    %this.SoundImporterValidateSoundName();

    // Fetch proxy profile.
    %proxyProfile = %this.proxyProfile;

    // Update profile description.
    %isMusic = SoundImporterChannel.getValue();
    if (%isMusic)
        %proxyProfile.VolumeChannel = $musicAudioType;
    else
        %proxyProfile.VolumeChannel = $effectsAudioType;

    %proxyAsset = AssetDatabase.acquireAsset(%this.soundAssetID);
    %proxyAsset.VolumeChannel = %proxyProfile.VolumeChannel;
    AssetDatabase.releaseAsset(%this.soundAssetID);

    // Update importer GUI.
    %this.UpdateImporterGUI();
}

// --------------------------------------------------------------------

function SoundEditor::updateProxyProfile( %this )
{
    if (!%this.proxyProfile)
        %this.proxyProfile = new AudioAsset();
    if (%this.soundAssetID !$= "")
    {
        %temp = AssetDatabase.acquireAsset(%this.soundAssetID);
        %this.proxyProfile.name = %temp.AssetName;
        %this.proxyProfile.VolumeChannel = %temp.VolumeChannel;
        %this.proxyProfile.Looping = %temp.Looping;
        %this.proxyProfile.Volume = %temp.Volume;
        %this.proxyProfile.Streaming = %temp.Streaming;
        %this.proxyProfile.filename = %temp.AudioFile;
        AssetDatabase.releaseAsset(%this.soundAssetID);
    }
    else
    {
        %this.proxyProfile.name = SoundImporterSoundName.getText();
        %this.proxyProfile.filename = collapsePath(SoundImporterFilename.getText());
    }
    %this.updateChannel();
    %this.updateLooping();
}

function SoundEditor::updateSoundAsset( %this )
{
    %previewProfile = AssetDatabase.acquireAsset(%this.soundAssetID);
    %proxyProfile = %this.proxyProfile;

    // Configure preview profile.
    if (%previewProfile.AssetName !$= %proxyProfile.name && !AssetDatabase.isDeclaredAsset(%previewProfile.AssetName))
    {
        %newID = strreplace(%this.soundAssetID, %previewProfile.AssetName, %proxyProfile.name);
        AssetDatabase.renameDeclaredAsset(%this.soundAssetID, %newID);
    }
    %previewProfile.AssetName = %proxyProfile.name;
    %previewProfile.AudioFile = %proxyProfile.filename;
    %previewProfile.Looping = %proxyProfile.Looping;
    %previewProfile.VolumeChannel = %proxyProfile.VolumeChannel;
    %previewProfile.Volume = %proxyProfile.volume;
    %previewProfile.Streaming = %proxyProfile.streaming;
    AssetDatabase.releaseAsset(%this.soundAssetID);
}

// --------------------------------------------------------------------
function SoundEditor::stripExtension(%this, %fileName)
{
    %noExt = strreplace(%fileName, ".jpg", "");
    %noExt = strreplace(%noExt, ".jpeg", "");
    %noExt = strreplace(%noExt, ".png", "");
    %noExt = strreplace(%noExt, ".pvr", "");

    return %noExt;
}

function SoundEditor::saveSounds( %this )
{   
    // Fetch proxy / profile.
    %proxyProfile = %this.proxyProfile;
    %tempAssetName = %this.proxyProfile.AssetName;
    
    // Stop any preview sound.
    Si_SoundStopButton.onClick();

    %proxyProfile.name = SoundImporterSoundName.getText();

    // Calculate target filename.
    %targetFilename = expandPath("^{UserAssets}/audio/" @ fileName(%proxyProfile.filename) );

    // Is the file in the correct target location?
    if ( %targetFilename !$= %proxyProfile.filename )
    {
        // No, so copy.
        if ( !pathCopy( %proxyProfile.filename, %targetFilename, true ) )
        {
            warn( " @@@ Target file '" @ %targetFilename @ " already exists.  File not copied." );
        }

        // Update sound profile filename.
        %proxyProfile.AudioFile = %this.stripExtension( %targetFilename );
    }

    // Update the profile.
    if ( %this.isNewProfile )
    {
        %soundAsset = %this.createSoundAsset(%proxyProfile.name);

        %this.copyTags(%this.soundAssetID, %soundAsset);
        %this.clearTags(%this.soundAssetID);

        AssetDatabase.removeSingleDeclaredAsset(%this.soundAssetID);
    }
    else
    {
        %this.updateSoundAsset();
        %this.soundAssetID = "";
    }
    AssetDatabase.saveAssetTags();

    // Delete proxy sound profile.
    if ( isObject(%this.proxyProfile) )
        %this.proxyProfile.delete();

    %this.assetSaved = true;
    // Remove GUI.   
    Canvas.popDialog( SoundImporterGui );

    AssetLibrary.schedule(100, "updateGui");
    //trace ( false );
}

function SoundEditor::close(%this)
{
    if ( %this.isNewProfile )
    {
        if (!%this.assetSaved)
        {
            %targetFilename = expandPath("^{UserAssets}/audio/" @ fileName(%this.proxyProfile.filename) );
            fileDelete(%targetFilename);
        }
        AssetDatabase.removeSingleDeclaredAsset(%this.soundAssetID);
    }
    else
    {
        if ( %this.soundAssetID !$= "" )
        {
            %originalFile = %this.assetSnapshot.AudioFile;
            %tempAsset = AssetDatabase.acquireAsset(%this.soundAssetID);
            %currentFile = %tempAsset.AudioFile;
            AssetDatabase.releaseAsset(%this.soundAssetID);
            
            %this.restoreAssetSnapshot();
            AssetDatabase.releaseAsset(%this.soundAssetID);
            %this.soundAssetID = "";
            if ( !%this.assetSaved && %originalFile !$= %currentFile )
            {
                %targetFilename = expandPath("^{UserAssets}/audio/" @ fileName(%currentFile) );
                fileDelete(%targetFilename);
            }
        }
    }
    deactivatePackage(TSSSoundEditorPackage);
}

function cleanTempAssets(%tempAssetName)
{
    AssetDatabase.removeSingleDeclaredAsset( SoundEditor.soundAssetID );
}

// --------------------------------------------------------------------
function Si_ChangeFileClick::onMouseEnter(%this)
{
    SoundImporterFileBrowse.NormalImageCache = SoundImporterFileBrowse.NormalImage;
    SoundImporterFileBrowse.setNormalImage(SoundImporterFileBrowse.HoverImage);
}

function Si_ChangeFileClick::onMouseLeave(%this)
{
    SoundImporterFileBrowse.setNormalImage(SoundImporterFileBrowse.NormalImageCache);
}

function Si_ChangeFileClick::onMouseDown(%this)
{
    SoundImporterFileBrowse.setNormalImage(SoundImporterFileBrowse.DownImage);
}

function Si_ChangeFileClick::onMouseUp(%this)
{
    SoundEditor.soundFileBrowser();
    SoundImporterFileBrowse.setNormalImage(SoundImporterFileBrowse.HoverImage);
}

function SoundImporterAddTagButton::onClick(%this)
{
    SoundEditor.addTag(SoundImporterTagList.getText());
}

function Si_SoundStopButton::onClick( %this )
{
    EditorSoundPlayer.stop();
    ////trace(false);
}

function Si_SoundPlayButton::onClick( %this )
{
    // pause sound if already playing.
    if ( alxIsPlaying( EditorSoundPlayer.previewSoundHandle ) )
        SoundEditorEventManager.postEvent("_PauseRequest", "");
    else
        SoundEditorEventManager.postEvent("_PlayRequest", "");
    ////trace(true);
}

function SoundImporterSoundName::updateGui(%this)
{
    Si_SaveButton.update(); 
}

function Si_SaveButton::update(%this)
{
    %active = true;    
    
    // Don't allow saving if the name is blank
    if (SoundImporterSoundName.isEmpty()) 
        %active = false;
        
    // Don't allow saving if the image location is blank
    if (SoundImporterFilename.isEmpty()) 
        %active = false;
      
    %this.setActive(%active);
}

function Si_SoundIconDisplay::update(%this)
{
    if (SoundEditor.proxyProfile.Looping)
    {
        if (%this.state > 3)
            %this.state = 1;
        if (%this.state < 1)
            %this.state = 1;
        %this.setImage("{EditorAssets}:speaker_On0" @ %this.state @ "ImageMap");
        %this.state++;
        %this.updateSchedule = %this.schedule(750, update);
    }
    else
    {
        if (%this.Image $= "{EditorAssets}:speaker_On03ImageMap")
            %this.setImage("{EditorAssets}:speaker_OffImageMap");
        else
            %this.setImage("{EditorAssets}:speaker_On03ImageMap");
    }
}

function Si_SoundIconDisplay::stop(%this)
{
    %this.state = 1;
    cancel(%this.updateSchedule);
}

// ----------------------------------------------------------------------------
new ScriptObject(EditorSoundPlayer);

function EditorSoundPlayer::pause(%this)
{
    //echo(" @@@ Pausing playback");
    Si_SoundIconDisplay.stop();
    Si_SoundPlayButton.NormalImage = "{EditorAssets}:playImageMap";
    Si_SoundPlayButton.HoverImage = "{EditorAssets}:play_hImageMap";
    Si_SoundPlayButton.DownImage = "{EditorAssets}:play_dImageMap";
    Si_SoundPlayButton.InactiveImage = "{EditorAssets}:play_iImageMap";

    alxPause( %this.previewSoundHandle );

    %sound = AssetDatabase.acquireAsset(SoundEditor.soundAssetID);
    if (!%sound.Looping)
    {
        %this.timeRemaining = getEventTimeLeft(%this.playPreviewSchedule);
        %this.bufferPos = %this.soundDuration - %this.timeRemaining;
        Si_SoundIconDisplay.setImage("{EditorAssets}:speaker_On01ImageMap");
    }
    AssetDatabase.releaseAsset(SoundEditor.soundAssetID);

    cancel(%this.playPreviewSchedule);
}

function EditorSoundPlayer::play(%this)
{
    // Fetch proxy profile.
    //echo(" @@@ Playing....");
    %proxyProfile = SoundEditor.proxyProfile;
    if (!isObject(%proxyProfile) || %proxyProfile.filename $= "")
        return;

    // set the "playing" image first to give visual feedback that we are trying
    // to perform the requested action.  This is especially important with larger
    // audio files because the copy-then-load proceedure can be long.
    if (Si_SoundIconDisplay.updateSchedule !$= "")
    {
        cancel(Si_SoundIconDisplay.updateSchedule);
        Si_SoundIconDisplay.updateSchedule = "";
    }
    Si_SoundIconDisplay.update();

    Si_SoundPlayButton.NormalImage = "{EditorAssets}:pauseImageMap";
    Si_SoundPlayButton.HoverImage = "{EditorAssets}:pause_hImageMap";
    Si_SoundPlayButton.DownImage = "{EditorAssets}:pause_dImageMap";
    Si_SoundPlayButton.InactiveImage = "{EditorAssets}:pause_iImageMap";

    // Create preview profile.
    SoundEditor.updateSoundAsset();
    %sound = AssetDatabase.acquireAsset(SoundEditor.soundAssetID);
    if (!%sound.Looping)
    {
        if ( %this.soundDuration $= "" )
            %this.soundDuration = alxGetAudioLength(SoundEditor.soundAssetID);

        if ( %this.timeRemaining > 0 )
            %time = %this.timeRemaining >= 32 ? %this.timeRemaining : 32;
        else
        {
            %time = alxGetAudioLength(SoundEditor.soundAssetID) + 32;
            %this.timeRemaining = %this.soundDuration;
        }

        %this.bufferPos = %this.soundDuration - %this.timeRemaining;
        %this.playPreviewSchedule = Si_SoundStopButton.schedule(%time, onClick);
        Si_SoundIconDisplay.setImage("{EditorAssets}:speaker_On03ImageMap");
    }
    AssetDatabase.releaseAsset(SoundEditor.soundAssetID);
    
    // Play the preview sound.
    if ( %this.previewSoundHandle != 0 && !alxIsPlaying(%this.previewSoundHandle) )
        alxUnPause( %this.previewSoundHandle );
    else if ( SoundEditor.soundAssetID !$= "" && %this.previewSoundHandle == 0 )
        %this.previewSoundHandle = alxPlay( SoundEditor.soundAssetID );
}

function EditorSoundPlayer::stop(%this)
{
    // Stop sound if already playing.
    //echo(" @@@ Stopping playback");
    if ( %this.previewSoundHandle != 0 )
        alxStop( %this.previewSoundHandle );
    %this.previewSoundHandle = 0;

    cancel( %this.playPreviewSchedule );
    %this.playPreviewSchedule = "";
    %this.soundDuration = "";
    %this.bufferPos = "";
    %this.timeRemaining = "";

    Si_SoundIconDisplay.stop();

    Si_SoundIconDisplay.setImage("{EditorAssets}:speaker_OffImageMap");
    Si_SoundPlayButton.NormalImage = "{EditorAssets}:playImageMap";
    Si_SoundPlayButton.HoverImage = "{EditorAssets}:play_hImageMap";
    Si_SoundPlayButton.DownImage = "{EditorAssets}:play_dImageMap";
    Si_SoundPlayButton.InactiveImage = "{EditorAssets}:play_iImageMap";
}

package TSSSoundEditorPackage
{

function AudioAsset::copy(%this, %sourceAssetID)
{
    %sourceAsset = AssetDatabase.acquireAsset(%sourceAssetID);

    %this.AudioFile = %sourceAsset.AudioFile;
    %this.Looping = %sourceAsset.Looping;
    %this.VolumeChannel = %sourceAsset.VolumeChannel;
    %this.Volume = %sourceAsset.Volume;
    %this.Streaming = %sourceAsset.Streaming;

    AssetDatabase.releaseAsset(%sourceAssetID);

    %assetTagManifest = AssetDatabase.getAssetTags();
    %assetTagCount = %assetTagManifest.getAssetTagCount(%sourceAssetID);
    for (%i = 0; %i < %assetTagCount; %i++)
    {
        %tag = %assetTagManifest.getAssetTag(%sourceAssetID, %i);
        // ::getAssetId() does not work if this asset was not acquired from 
        // the asset database.
        %assetTagManifest.tag(%this.getAssetId(), %tag);
    }
}

};