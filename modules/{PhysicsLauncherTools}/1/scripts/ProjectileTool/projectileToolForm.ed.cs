//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This function sets the initial state of the Projectile Tool view.
/// </summary>
function ProjectileToolForm::onWake(%this)
{
    if (isObject(ProjectileTool.selectedObject))
    {
        %this.selectedSound = 0;
        %this.selectSound();
        %this.selectedPreview = 0;
        %this.projectile = ProjectileTool.selectedObject;
        %this.tutorialData = ProjectileBuilder::getTutorialImage(%this.projectile);
        %this.selectPreview();
        Pt_PreviewPlayBtn.onClick();
        %this.updateStateDropdown();
        %this.refresh();
    }
    
    PhysicsLauncherTools::audioButtonInitialize(Pt_PreviewSoundPlayBtn);
}

/// <summary>
/// This function ensures that all projectile data has been saved when the tool
/// closes.
/// </summary>
function ProjectileToolForm::onSleep(%this)
{
    Pt_ProjectileFrictionEdit.onValidate();
    Pt_ProjectileRestitutionEdit.onValidate();
    Pt_ProjectilePointValueEdit.onValidate();
    Pt_ProjectileMassEdit.onValidate();
    PhysicsLauncherTools::writePrefabs();
    alxStopAll();
    ProjectileTool.helpManager.stop();
    ProjectileTool.helpManager.delete();
}

/// <summary>
/// This function plays the currently selected projectile's animation
/// </summary>
function ProjectileToolForm::displayPreview(%this, %index)
{
    %this.selectedPreview = %index;
    %this.selectPreview();

    %isStaticImage = AssetDatabase.getAssetType(%this.previewName) $= "ImageAsset";

    // Refresh play/stop buttons
    Pt_PreviewPlayBtn.setVisible(!%isStaticImage);
    Pt_PreviewStopBtn.setVisible(!%isStaticImage);

    if (%isStaticImage)
    {
        Pt_PreviewWindow.setImage(%this.previewName);
        %frame = 0;
        switch(Pt_PreviewSelectDropdown.getSelected())
        {
            case 0:
                %frame = ProjectileBuilder::getIdleInLauncherAnimFrame(%this.projectile);
            case 1:
                %frame = ProjectileBuilder::getInAirAnimFrame(%this.projectile);
            case 2:
                %frame = ProjectileBuilder::getHitAnimFrame(%this.projectile);
            case 3:
                %frame = ProjectileBuilder::getVanishAnimFrame(%this.projectile);
            case 4:
                %frame = ProjectileBuilder::getImageTrailFrame(%this.projectile);
        }
        Pt_PreviewWindow.setImageFrame(%frame);
    }
    else
        Pt_PreviewWindow.Animation = %this.previewName;
}

/// <summary>
/// This function refreshes the data and view for the currently selected projectile.
/// </summary>
function ProjectileToolForm::refresh(%this)
{
    %this.projectile = ProjectileTool.selectedObject;
    
    if (isObject(%this.projectile))
    {
        %this.idleAnim = ProjectileBuilder::getIdleInLauncherAnim(%this.projectile);
        %this.inAirAnim = ProjectileBuilder::getInAirAnim(%this.projectile);
        %this.hitObjectAnim = ProjectileBuilder::getHitAnim(%this.projectile);
        %this.disappearAnim = ProjectileBuilder::getVanishAnim(%this.projectile);
        %this.trailAnim = ProjectileBuilder::getImageTrailAnim(%this.projectile);

        %this.idleSound = ProjectileBuilder::getIdleInLauncherSound(%this.projectile);
        %this.inAirSound = ProjectileBuilder::getInAirSound(%this.projectile);
        %this.hitObjectSound = ProjectileBuilder::getHitObjectSound(%this.projectile);
        %this.disappearSound = ProjectileBuilder::getDisappearSound(%this.projectile);

        %this.updateStateDropdown();

        Pt_ProjectileNameEdit.setText(%this.projectile.getInternalName());
        Pt_ProjectileMassEdit.setText(ProjectileBuilder::getProjectileMass(%this.projectile));
        Pt_ProjectilePointValueEdit.setText(ProjectileBuilder::getProjectilePointValue(%this.projectile));
        Pt_ProjectileFrictionEdit.setText(ProjectileBuilder::getProjectileFriction(%this.projectile));
        Pt_ProjectileRestitutionEdit.setText(ProjectileBuilder::getProjectileRestitution(%this.projectile));

        %tempAsset = ProjectileBuilder::getIdleInLauncherAnim(%this.projectile);
        %temp = AssetDatabase.acquireAsset(%tempAsset);
        Pt_PreviewFileEdit.setText(%temp.AssetName);
        AssetDatabase.releaseAsset(%tempAsset);

        %this.tutorialData = ProjectileBuilder::getTutorialImage(%this.projectile);
        %temp = AssetDatabase.acquireAsset(%this.tutorialData);
        Pt_TutorialFileEdit.setText(%temp.AssetName);
        AssetDatabase.releaseAsset(%this.tutorialData);

        %this.selectorButton = ProjectileBuilder::getSwitchButton(%this.projectile);
        if (getWordCount(%this.selectorButton) > 1)
            %preview = getWord(%this.selectorButton, 0);
        else
            %preview = %this.selectorButton;
        %temp = AssetDatabase.acquireAsset(%preview);
        Pt_ProjectileButtonFileEdit.setText(%temp.AssetName);
        AssetDatabase.releaseAsset(%preview);

        %this.selectedSound = 0;
        %this.selectSound();

        %tempSnd = AssetDatabase.acquireAsset(%this.soundName);
        Pt_PreviewSoundFileEdit.setText(%tempSnd.AssetName);
        AssetDatabase.releaseAsset(%this.soundName);

        %this.displayPreview(0);
    }
    else
        echo(" @@@ no projectile selected");
}

/// <summary>
/// This function prepares the projectile state dropdown with the correct entries.
/// </summary>
function ProjectileToolForm::updateStateDropdown(%this)
{
    Pt_PreviewSelectDropdown.clear();
    Pt_PreviewSelectDropdown.add("Idle in Launcher", 0);
    Pt_PreviewSelectDropdown.add("In Air", 1);
    Pt_PreviewSelectDropdown.add("Hit Object", 2);
    Pt_PreviewSelectDropdown.add("Disappear", 3);
    Pt_PreviewSelectDropdown.add("In Air Trail", 4);
    Pt_PreviewSelectDropdown.setFirstSelected();
}

/// <summary>
/// This function resizes an image and maintains correct aspect ratio for placement
/// on the projecitle selection buttons.
/// </summary>
/// <param name="image">The image or animation to place on the button.</param>
/// <param name="preview">The target gui element that will display the image.</param>
/// <param name="container">The gui element that defines the maximum extent of the preview.</param>
function ProjectileToolForm::resizeButtonPreview(%this, %image, %preview, %container)
{
    %asset = strchr(%image, "{");
    %temp = AssetDatabase.acquireAsset(%asset);
    if (!isObject(%temp))
        return;
    %size = %temp.getFrameSize(0);

    %previewExtent = %container.Extent;
    %sizeX = getWord(%size, 0);
    %sizeY = getWord(%size, 1);
    %previewSizeX = getWord(%previewExtent, 0);
    %previewSizeY = getWord(%previewExtent, 1);
    if (%sizeX > %sizeY)
    {
        %scaleFactor = %previewSizeX / %sizeX;
    }
    else
        %scaleFactor = %previewSizeY / %sizeY;

    %assetSizeX = %sizeX * %scaleFactor;
    %assetSizeY = %sizeY * %scaleFactor;
    
    %previewPosX = (%previewSizeX / 2) - (%assetSizeX / 2);
    %previewPosY = (%previewSizeY / 2) - (%assetSizeY / 2);

    %preview.Extent = %assetSizeX SPC %assetSizeY;
    %preview.Position = %previewPosX SPC %previewPosY;
}

/// <summary>
/// This function identifies an asset as either an image or an animation.
/// </summary>
/// <param name="datablock">The asset instance to identify.</param>
/// <return>Returns a string that contains the classname of the asset instance.</return>
function ProjectileToolForm::getPreviewType(%this, %datablock)
{
    if (!isObject(%datablock))
        return;

    %class = %datablock.getClassName();
    switch$(%class)
    {
        case "ImageAsset":
            return "t2dStaticSprite";

        case "AnimationAsset":
            return "t2dAnimatedSprite";
    }
}

/// <summary>
/// This function sets the currently selected sound name field for use in 
/// the state preview.  Also sets the name label for display.
/// </summary>
function ProjectileToolForm::selectSound(%this, %index)
{
    %tag = "IDLE IN LAUNCHER: ";
    %this.selectedSound = %index;

    switch(%index)
    {
        case 0:
            %this.soundName = %this.idleSound;
            %tag = "Idle Sound";

        case 1:
            %this.soundName = %this.inAirSound;
            %tag = "In air sound";

        case 2:
            %this.soundName = %this.hitObjectSound;
            %tag = "Hit object sound";

        case 3:
            %this.soundName = %this.disappearSound;
            %tag = "Disappear sound";

        case 4:
            %this.soundName = "";
            %tag = "No Sound";
    }
    Pt_SoundLabel.text = %tag;
}

/// <summary>
/// This function sets the currently selected image or animation name to display.
/// </summary>
function ProjectileToolForm::selectPreview(%this)
{
    switch(%this.selectedPreview)
    {
        case 0:
            %this.previewName = %this.idleAnim;

        case 1:
            %this.previewName = %this.inAirAnim;

        case 2:
            %this.previewName = %this.hitObjectAnim;

        case 3:
            %this.previewName = %this.disappearAnim;

        case 4:
            %this.previewName = %this.trailAnim;
    }
}

/// <summary>
/// This function assigns the selected asset to the required slot in the projectile
/// animation and sound effect system.
/// </summary>
/// <param name="asset">The asset to assign to the projectile.</param>
/// <param name="type">The type of asset received.</param>
function ProjectileToolForm::setAssetSlot(%this, %asset, %type, %frame)
{
    %slot = Pt_PreviewSelectDropdown.getSelected();
    switch(%slot)
    {
        case 0:
            if (%type $= "sound")
                ProjectileBuilder::setIdleInLauncherSound(%this.projectile, %asset);
            if (%type $= "AnimationAsset")
                ProjectileBuilder::setIdleInLauncherAnim(%this.projectile, %asset);
            if (%type $= "ImageAsset")
                ProjectileBuilder::setIdleInLauncherAnim(%this.projectile, %asset, %frame);

        case 1:
            if (%type $= "sound")
                ProjectileBuilder::setInAirSound(%this.projectile, %asset);
            if (%type $= "AnimationAsset")
                ProjectileBuilder::setInAirAnim(%this.projectile, %asset);
            if (%type $= "ImageAsset")
                ProjectileBuilder::setInAirAnim(%this.projectile, %asset, %frame);

        case 2:
            if (%type $= "sound")
                ProjectileBuilder::setHitObjectSound(%this.projectile, %asset);
            if (%type $= "AnimationAsset")
                ProjectileBuilder::setHitAnim(%this.projectile, %asset);
            if (%type $= "ImageAsset")
                ProjectileBuilder::setHitAnim(%this.projectile, %asset, %frame);

        case 3:
            if (%type $= "sound")
                ProjectileBuilder::setDisappearSound(%this.projectile, %asset);
            if (%type $= "AnimationAsset")
                ProjectileBuilder::setVanishAnim(%this.projectile, %asset);
            if (%type $= "ImageAsset")
                ProjectileBuilder::setVanishAnim(%this.projectile, %asset, %frame);

        case 4:
            if (%type $= "sound")
                return;

            ProjectileBuilder::setImageTrailAnim(%this.projectile, %asset, %frame);
    }
}

function Pt_ProjectileNameEdit::onReturn(%this)
{
    %this.onValidate();
}

/// <summary>
/// This function ensures that the projectile name is both valid and unique.
/// </summary>
function Pt_ProjectileNameEdit::onValidate(%this)
{
    %projectileSet = $PrefabSet.findObjectByInternalName("ProjectileSet");
    %selectedName = ProjectileTool.selectedObject.getInternalName();
    
    %name = %this.getText();
    
    if ( %name $= %selectedName )
        return;

    if (!isObject(%projectileSet.findObjectByInternalName(%name)))
    {
        ProjectileTool.selectedObject.setInternalName(%name);
    }
    else
    {
        %this.setText(ProjectileTool.selectedObject.getInternalName());
        NoticeGui.display(%name @ " already exists.  Please pick another name.");
    }
    
    // Refresh the scroll view
    ProjectileTool.refreshProjectileView();  
    ProjectileTool.ProjectileContainer.setSelected(ProjectileTool.selectedIndex);
    ProjectileTool.ProjectileContainer.scrollToButton(ProjectileTool.selectedIndex);
}

/// <summary>
/// This function decrements the projectile's mass.
/// </summary>
function Pt_ProjectileMassDownBtn::onClick(%this)
{
    Pt_ProjectileMassEdit.profile = "GuiSpinnerProfile";
    %projectile = ProjectileTool.selectedObject;
    %mass = Pt_ProjectileMassEdit.getText();
    %mass -= 1;
    ProjectileBuilder::setProjectileMass(%projectile, %mass);
    Pt_ProjectileMassEdit.setText(ProjectileBuilder::getProjectileMass(%projectile));
}

/// <summary>
/// This function increments the projectile's mass.
/// </summary>
function Pt_ProjectileMassUpBtn::onClick(%this)
{
    Pt_ProjectileMassEdit.profile = "GuiSpinnerProfile";
    %projectile = ProjectileTool.selectedObject;
    %mass = Pt_ProjectileMassEdit.getText();
    %mass += 1;
    ProjectileBuilder::setProjectileMass(%projectile, %mass);
    Pt_ProjectileMassEdit.setText(ProjectileBuilder::getProjectileMass(%projectile));
}

/// <summary>
/// This function sets the projectile's mass when directly set in the 
/// value edit box.
/// </summary>
function Pt_ProjectileMassEdit::onValidate(%this)
{
    %this.profile = "GuiSpinnerProfile";
    %mass = %this.getText();
    if (%mass > $ProjectileBuilder::ToolMassMax || %mass < $ProjectileBuilder::ToolMassMin)
    {
        %this.profile = "GuiInvalidSpinnerProfile";
    }
    else
        ProjectileBuilder::setProjectileMass(ProjectileTool.selectedObject, %mass);
}

/// <summary>
/// This function decrements the projectile's point value.
/// </summary>
function Pt_ProjectileValueDownBtn::onClick(%this)
{
    %projectile = ProjectileTool.selectedObject;
    %value = Pt_ProjectilePointValueEdit.getText();
    %value -= 1;
    ProjectileBuilder::setProjectilePointValue(%projectile, %value);
    Pt_ProjectilePointValueEdit.setText(ProjectileBuilder::getProjectilePointValue(%projectile));
}

/// <summary>
/// This function increments the projectile's point value.
/// </summary>
function Pt_ProjectileValueUpBtn::onClick(%this)
{
    %projectile = ProjectileTool.selectedObject;
    %value = Pt_ProjectilePointValueEdit.getText();
    %value += 1;
    ProjectileBuilder::setProjectilePointValue(%projectile, %value);
    Pt_ProjectilePointValueEdit.setText(ProjectileBuilder::getProjectilePointValue(%projectile));
}

/// <summary>
/// This function sets the projectile's point value when it is entered in the value
/// edit box.
/// </summary>
function Pt_ProjectilePointValueEdit::onValidate(%this)
{
    %this.profile = "GuiSpinnerProfile";
    %value = %this.getText();
    if (%value > $ProjectileBuilder::MaxPointValue || %value < 0)
    {
        %this.profile = "GuiInvalidSpinnerProfile";
    }
    else
        ProjectileBuilder::setProjectilePointValue(ProjectileTool.selectedObject, %value);
}

/// <summary>
/// This function decrements the projectile's friction value.
/// </summary>
function Pt_ProjectileFrictionDownBtn::onClick(%this)
{
    Pt_ProjectileFrictionEdit.profile = "GuiSpinnerProfile";
    %projectile = ProjectileTool.selectedObject;
    %friction = Pt_ProjectileFrictionEdit.getText();
    %friction -= 1;
    ProjectileBuilder::setProjectileFriction(%projectile, %friction);
    Pt_ProjectileFrictionEdit.setText(ProjectileBuilder::getProjectileFriction(%projectile));
}

/// <summary>
/// This function increments the projectile's friction value.
/// </summary>
function Pt_ProjectileFrictionUpBtn::onClick(%this)
{
    Pt_ProjectileFrictionEdit.profile = "GuiSpinnerProfile";
    %projectile = ProjectileTool.selectedObject;
    %friction = Pt_ProjectileFrictionEdit.getText();
    %friction += 1;
    ProjectileBuilder::setProjectileFriction(%projectile, %friction);
    Pt_ProjectileFrictionEdit.setText(ProjectileBuilder::getProjectileFriction(%projectile));
}

/// <summary>
/// This function sets the projectile's friction value when it is entered in the 
/// friction edit box.
/// </summary>
function Pt_ProjectileFrictionEdit::onValidate(%this)
{
    %this.profile = "GuiSpinnerProfile";
    %friction = %this.getText();
    if (%friction > $ProjectileBuilder::ToolFrictionMax || %friction < $ProjectileBuilder::ToolFrictionMin)
    {
        %this.profile = "GuiInvalidSpinnerProfile";
    }
    else
        ProjectileBuilder::setProjectileFriction(ProjectileTool.selectedObject, %friction);
}

/// <summary>
/// This function decrements the projectile's restitution value.
/// </summary>
function Pt_ProjectileRestitutionDownBtn::onClick(%this)
{
    Pt_ProjectileRestitutionEdit.profile = "GuiSpinnerProfile";
    %projectile = ProjectileTool.selectedObject;
    %restitution = Pt_ProjectileRestitutionEdit.getText();
    %restitution -= 1;
    ProjectileBuilder::setProjectileRestitution(%projectile, %restitution);
    Pt_ProjectileRestitutionEdit.setText(ProjectileBuilder::getProjectileRestitution(%projectile));
}

/// <summary>
/// This function increments the projectile's restitution value.
/// </summary>
function Pt_ProjectileRestitutionUpBtn::onClick(%this)
{
    Pt_ProjectileRestitutionEdit.profile = "GuiSpinnerProfile";
    %projectile = ProjectileTool.selectedObject;
    %restitution = Pt_ProjectileRestitutionEdit.getText();
    %restitution += 1;
    ProjectileBuilder::setProjectileRestitution(%projectile, %restitution);
    Pt_ProjectileRestitutionEdit.setText(ProjectileBuilder::getProjectileRestitution(%projectile));
}

/// <summary>
/// This function sets the restitution value when it is entered directly in the 
/// value edit box.
/// </summary>
function Pt_ProjectileRestitutionEdit::onValidate(%this)
{
    %this.profile = "GuiSpinnerProfile";
    %restitution = %this.getText();
    if (%restitution > $ProjectileBuilder::ToolRestitutionMax || %restitution < $ProjectileBuilder::ToolRestitutionMin)
    {
        %this.profile = "GuiInvalidSpinnerProfile";
    }
    else
        ProjectileBuilder::setProjectileRestitution(ProjectileTool.selectedObject, %restitution);
}

/// <summary>
/// This function plays the projectile state animation if the selected state is animated.
/// </summary>
function Pt_PreviewPlayBtn::onClick(%this)
{
    %isStaticImage = AssetDatabase.getAssetType(ProjectileToolForm.previewName) $= "ImageAsset";
    if (%isStaticImage)
    {
        if (Pt_PreviewWindow.Image $= "")
            Pt_PreviewWindow.setImage(ProjectileToolForm.previewName);
    }
    else
    {
        if (Pt_PreviewWindow.Animation $= "")
            Pt_PreviewWindow.Animation = ProjectileToolForm.previewName;
    }
    Pt_PreviewWindow.pause(false);
}

/// <summary>
/// This function stops playback of the current state animation.
/// </summary>
function Pt_PreviewStopBtn::onClick(%this)
{
    Pt_PreviewWindow.pause(true);
}

/// <summary>
/// This function selects the projectile state and displays the appropriate data.
/// </summary>
function Pt_PreviewSelectDropdown::onSelect(%this)
{
    %index = %this.getSelected();
    ProjectileToolForm.selectSound(%index);
    ProjectileToolForm.displayPreview(%index);
    Pt_PreviewPlayBtn.onClick();

    %tempImg = AssetDatabase.acquireAsset(ProjectileToolForm.previewName);
    Pt_PreviewFileEdit.setText(%tempImg.AssetName);
    AssetDatabase.releaseAsset(ProjectileToolForm.previewName);

    %tempSnd = AssetDatabase.acquireAsset(ProjectileToolForm.soundName);
    Pt_PreviewSoundFileEdit.setText(%tempSnd.AssetName);
    AssetDatabase.releaseAsset(ProjectileToolForm.soundName);

    if (%index == 4)
    {
        Pt_PreviewSoundFileEdit.setActive(false);
        Pt_PreviewSoundFileEvent.setActive(false);
        Pt_PreviewSoundSelectBtn.setActive(false);
        Pt_PreviewSoundStopBtn.setActive(false);
        Pt_PreviewSoundPlayBtn.setActive(false);
    }
    else
    {
        Pt_PreviewSoundFileEdit.setActive(true);
        Pt_PreviewSoundFileEvent.setActive(true);
        Pt_PreviewSoundSelectBtn.setActive(true);
        Pt_PreviewSoundStopBtn.setActive(true);
        Pt_PreviewSoundPlayBtn.setActive(true);
    }
}

/// <summary>
/// This function opens the asset library to select an image or animation for
/// the current projectile's selected state.
/// </summary>
function Pt_PreviewFileSelectBtn::onClick(%this)
{
    %this.selectedState = Pt_PreviewSelectDropdown.getSelected();
    AssetPicker.open("AnimationAsset ImageAsset", "projectile", "", %this);
}

/// <summary>
/// This function sets the asset to the selected projectile's current state.
/// </summary>
/// <param name="assetID">The asset ID returned from the asset library.</param>
function Pt_PreviewFileSelectBtn::setSelectedAsset(%this, %assetID, %frame)
{
    %temp = AssetDatabase.acquireAsset(%assetID);
    %type = %temp.getClassName();
    Pt_PreviewFileEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(%assetID);
    ProjectileToolForm.setAssetSlot(%assetID, %type, %frame);
    ProjectileTool.refreshProjectileView();
    ProjectileTool.selectProjectile(ProjectileTool.selectedIndex);
    Pt_PreviewSelectDropdown.setSelected(%this.selectedState);
}

/// <summary>
/// This function opens the asset library to select an image to use for the
/// current projectile's tutorial image.
/// </summary>
function Pt_TutorialImageSelectBtn::onClick(%this)
{
    AssetPicker.open("ImageAsset", "", "gui", %this);
}

/// <summary>
/// This function sets the current projectile's tutorial image.
/// </summary>
/// <param name="assetID">The asset ID of the tutorial image.</param>
function Pt_TutorialImageSelectBtn::setSelectedAsset(%this, %assetID)
{
    %tutorialName = ProjectileToolForm.projectile.getName();
    ProjectileBuilder::setTutorialImage(%tutorialName, %assetID);
    ProjectileToolForm.refresh();
}

/// <summary>
/// This function opens the asset library to select an image to use for the
/// current projectile's selection button on the in-game HUD.
/// </summary>
function Pt_ButtonImageSelectBtn::onClick(%this)
{
    AssetPicker.open("ImageAsset", "", "gui", %this);
}

/// <summary>
/// This function assigns the selected asset to the current projectile's 
/// selection button slot in the in-game HUD.
/// Currently, the images are expected to have identical names ending with Up, Hover, Down and
/// Inactive respectively and the Up button should be selected for the image.  The others will be
/// added automatically if they exist.
/// </summary>
/// <param name="assetID">The asset ID of the image to use as the selection button.</param>
function Pt_ButtonImageSelectBtn::setSelectedAsset(%this, %assetID)
{
    ProjectileBuilder::setSwitchButton(ProjectileToolForm.projectile, %assetID, Pt_ButtonStateDropdown.getSelected());
    ProjectileToolForm.refresh();
}

/// <summary>
/// This function opens the asset library to select the sound for the current
/// projectile's selected state.
/// </summary>
function Pt_PreviewSoundSelectBtn::onClick(%this)
{
    %this.selectedState = Pt_PreviewSelectDropdown.getSelected();
    AssetPicker.open("AudioAsset", "", "", %this);
}

/// <summary>
/// This function assigns the selected sound to the current projectile's selected
/// state.
/// </summary>
/// <param name="assetID">The asset ID of the selected sound.</param>
function Pt_PreviewSoundSelectBtn::setSelectedAsset(%this, %assetID)
{
    ProjectileToolForm.setAssetSlot(%assetID, "sound");
    ProjectileToolForm.refresh();
    Pt_PreviewSelectDropdown.setSelected(%this.selectedState);
}

/// <summary>
/// This function plays the sound associated with the current projectile's 
/// selected state.
/// </summary>
function Pt_PreviewSoundPlayBtn::onClick(%this)
{
    PhysicsLauncherTools::audioButtonPairClicked(%this, ProjectileToolForm.soundName);
}

function Pt_PreviewSoundStopBtn::onClick(%this)
{
    PhysicsLauncherTools::audioButtonPairStop(Pt_PreviewSoundPlayBtn);
}
/// <summary>
/// This function deletes the selected projectile from the projectile set.
/// </summary>
function Pt_ProjectileRemove::onClick(%this)
{
    %projectileSet = $PrefabSet.findObjectByInternalName("ProjectileSet");
    %obj = %projectileSet.getObject(%this.index);
    %index = %this.index;
    %data = %index @ " " @ %obj.getName() @ " " @ ProjectileBuilder::findProjectileInAllLevels(%obj.getName());
    Plt_ConfirmDeleteGui.display("", "ProjectileTool", "removeProjectile", %data);
}

/// <summary>
/// This function displays a preview of the tutorial image in a dialog.
/// </summary>
function Pt_TutorialImagePreviewBtn::onClick(%this)
{
    %asset = ProjectileToolForm.tutorialData;
    %temp = AssetDatabase.acquireAsset(%asset);
    ImagePreviewGui.display(%asset);
}

/// <summary>
/// This function displays a preview of the button image in a dialog.
/// </summary>
function Pt_ButtonImagePreviewBtn::onClick(%this)
{
    %asset = getWord(ProjectileToolForm.selectorButton, Pt_ButtonStateDropdown.getSelected());
    %temp = AssetDatabase.acquireAsset(%asset);
    ImagePreviewGui.display(%asset);
}

//------------------------------------------------------------------------------
// Collision Editor
//------------------------------------------------------------------------------
/// <summary>
/// This function opens the Collision Editor for the selected projectile.
/// </summary>
function Pt_EditCollisionBtn::onClick(%this)
{
    ProjectileBuilder::openCollisionEditor(ProjectileTool.selectedObject, %this);
}

/// <summary>
/// This function requests the ProjectileBuilder to set up the projectile's 
/// new collision shapes from the editor.
/// </summary>
function Pt_EditCollisionBtn::onCollisionEditSave(%this, %proxyObject)
{
    //echo ("@@@ Pt_EditCollisionBtn::onCollisionEditSave called..."); 
      
    ProjectileBuilder::setCollisionShapesFromProxy(ProjectileTool.selectedObject, %proxyObject);
}

function Pt_ButtonStateDropdown::onSelect(%this)
{
    %index = %this.getSelected();

    %asset = getWord(ProjectileToolForm.selectorButton, %this.getSelected());
    %temp = AssetDatabase.acquireAsset(%asset);
    Pt_ProjectileButtonFileEdit.setText(%temp.AssetName);
    AssetDatabase.releaseAsset(%asset);
}