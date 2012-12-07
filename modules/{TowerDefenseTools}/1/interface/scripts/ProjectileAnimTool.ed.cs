//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------



//--------------------------------
// Projectile Animation Set Tool Help
//--------------------------------
/// <summary>
/// This function opens a browser and navigates to the Tower Defense Template 
/// Projectile Animation Set Tool help page.
/// </summary>
function PAToolHelpButton::onClick(%this)
{
   gotoWebPage("http://docs.3stepstudio.com/towerdefense/projectileanimationset/");
}


//-----------------------------------------------------------------------------
// Projectile Animation Tool Globals
//-----------------------------------------------------------------------------

$PAToolInitialized = false;

/// <summary>
/// This function handles the tool's dirty state to keep track of the need to save data.
/// </summary>
/// <param name="dirty">True if data has been changed, false if not.</param>
function SetProjectileAnimToolDirtyState(%dirty)
{
   $Tools::TemplateToolDirty = %dirty;
   
   if ($Tools::TemplateToolDirty)
      ProjectileAnimToolWindow.setText("Projectile Animation Set Tool *");
   else
      ProjectileAnimToolWindow.setText("Projectile Animation Set Tool");
}
//-----------------------------------------------------------------------------
// Projectile Animation Tool
//-----------------------------------------------------------------------------

/// <summary>
/// This function handles closing the tool correctly.
/// </summary>
function ProjectileAnimTool::handleClose(%this)
{
   if ($Tools::TemplateToolDirty)
   {
      ConfirmDialog.setupAndShow(ProjectileAnimToolWindow.getGlobalCenter(), "Save Projectile Animation Set Changes?", 
            "Save", "if (ProjectileAnimTool.saveAnimSet()){ SetProjectileAnimToolDirtyState(false); Canvas.popDialog(ProjectileAnimTool);}", 
            "Don't Save", "ProjectileAnimTool.initializePreviews(); SetProjectileAnimToolDirtyState(false); Canvas.popDialog(ProjectileAnimTool);", 
            "Cancel", "");
   }
   else
      Canvas.popDialog(ProjectileAnimTool);
}

/// <summary>
/// This function handles closing the tool correctly.
/// </summary>
function ProjectileAnimTool::confirmChanges(%this)
{
   if ($Tools::TemplateToolDirty)
   {
      ConfirmDialog.setupAndShow(ProjectileAnimToolWindow.getGlobalCenter(), "Save Projectile Animation Set Changes?", 
            "Save", "ProjectileAnimTool.saveAnimSet(); SetProjectileAnimToolDirtyState(false);", 
            "Don't Save", "ProjectileAnimTool.initializePreviews(); SetProjectileAnimToolDirtyState(false);", 
            "Cancel", "");
   }
}

/// <summary>
/// This function handles cancelling the tool's save process.
/// </summary>
function ProjectileAnimTool::cancel(%this)
{
   %this.initializePreviews();
   SetEnemyAnimToolDirtyState(false);
}

/// <summary>
/// This function checks that a name is valid for use as an animation set name.
/// </summary>
/// <param name="name">The name to be validated.</param>
/// <return>Returns true if the name is valid, false if not.</return>
function ProjectileAnimTool::checkValidName(%this, %name)
{
    // Check for empty name 
    if (strreplace(%name, " ", "") $= "")  
    {
        // Show message dialog
        WarningDialog.setupAndShow(ProjectileAnimToolWindow.getGlobalCenter(), "Notice!", "Animation Set name cannot be empty!", 
         "", "", "", "", "Okay", "return false;");         

        
        return false;   
    }

    // Check for spaces
    if (strreplace(%name, " ", "") !$= %name)
    {
        // Show message dialog
        WarningDialog.setupAndShow(ProjectileAnimToolWindow.getGlobalCenter(), "Notice!", "Animation Set name cannot contain spaces!", 
         "", "", "", "", "Okay", "return false;");         
        
        return false;
    }
    
    // Check for invalid characters
    %invalidCharacters = "-+*/%$&§=()[].?\"#,;!~<>|°^{}";
    %strippedName = stripChars(%name, %invalidCharacters);
    if (%strippedName !$= %name)
    {
        // Show message dialog
        WarningDialog.setupAndShow(ProjectileAnimToolWindow.getGlobalCenter(), "Notice!", "Animation Set name contains invalid symbols!", 
         "", "", "", "", "Okay", "return false;"); 
         
        return false;   
    }
    
    // check for all numbers 
    %invalidCharacters = "0123456789";
    %strippedName = stripChars(%name, %invalidCharacters);
    if (%strippedName $= "")
    {
        // Show message dialog
        WarningDialog.setupAndShow(ProjectileAnimToolWindow.getGlobalCenter(), "Notice!", "Animation Set name must have at least one non-numeric character!", 
         "", "", "", "", "Okay", "return false;"); 
         
        return false;   
    }
    
    %invalidCharacters = "0123456789";
    %firstChar = getSubStr(%name, 0, 1);
    %strippedName = stripChars(%firstChar, %invalidCharacters);
    if (%strippedName $= "")
    {
        // Show message dialog
        WarningDialog.setupAndShow(ProjectileAnimToolWindow.getGlobalCenter(), "Notice!", "Animation Set name must not begin with a number!", 
         "", "", "", "", "Okay", "return false;"); 
         
        return false;   
    }

    return true;
}

/// <summary>
/// This function checks that all animation set fields have been populated correctly and
/// that the animation set name is a valid SimObject name.
/// </summary>
/// <return>Returns true if the animation set is valid, false if not.</return>
function ProjectileAnimTool::checkValid(%this)
{
   // check for empty fields
   if (isObject(PAToolTravelAnimPreview.sprite))
      return true;
   else
      return false;
}

/// <summary>
/// This function saves the projectile animation set.
/// </summary>
/// <return>Returns true if the save operation completes, or false if not.</return>
function ProjectileAnimTool::saveAnimSet(%this)
{
   %valid = %this.checkValid();
   if (!%valid)
   {
      // messagebox - set has an empty animation
      return false;
   }
   
   %animSetName = PAToolAnimSetName.getText();
   
    if (!%this.checkValidName(%animSetName))
        return false;
    
   %noConflict = false;

   for (%i = 0; %i < $animationSets.getCount();%i++)
   {
      %temp = $animationSets.getObject(%i);

      if (%temp.getName() $= %animSetName)
      {
         if (%this.animationSet == %temp)
            %noConflict = true;

         else
         {
            // messagebox - name conflict
            WarningDialog.setupAndShow(ProjectileAnimToolWindow.getGlobalCenter(), "Duplicate Name", "Another Animation Set Object in your Game Already has this Name", "", "", "OK");      
            echo(" -- ProjectileAnimTool::saveAnimSet() - name conflict " @ %temp @ " : " @ %this.animationSet);
            return false;
         }
      }
      if (%noConflict)
         break;
   }
   
   if (!%noConflict)
      %this.animationSet.setName(PAToolAnimSetName.getText());
   
   
   %this.animationSet.TravelAnim = PAToolTravelAnimPreview.sprite.getAnimation();
   AddAssetToLevelDatablocks(%this.animationSet.TravelAnim);
   //echo(" -- ProjectileAnimTool::saveAnimSet() - TravelAnim = "@%this.animationSet.TravelAnim);
   %this.animationSet.HitAnim = PAToolHitAnimPreview.sprite.getAnimation();
   AddAssetToLevelDatablocks(%this.animationSet.HitAnim);
   //echo(" -- ProjectileAnimTool::saveAnimSet() - HitAnim = "@%this.animationSet.HitAnim);
   
   LDEonApply();
   LBProjectObj.persistToDisk(true, true, true, true, true, true);
   //%this.animationSet.save($AnimSetFile);
   //echo(" -- EnemyAnimTool::saveAnimSet() File - " @ $AnimSetFile);
   
   ProjectileUpdateAnimSet(%this.animationSet.getName());
   
   
   return true;
}

/// <summary>
/// This function creates a new, blank projectile animation set and gives it a
/// unique temporary name.
/// </summary>
function ProjectileAnimTool::createAnimationSet(%this)
{
   %this.animationSet = new ScriptObject() {
      canSaveDynamicFields = "1";
         class = "animationSet";
         NameTags = "4";

         TravelAnim = "";
         HitAnim = "";
   };
   %name = "NewAnimationSet";
   %number = 1;
   
   while (isObject(%name))
   {
      %name = "NewAnimationSet" @ %number;
      %number++;
   }

   %this.animationSet.setName(%name);
   LBProjectObj.addAnimationSet(%this.animationSet.getName(), true);
}

/// <summary>
/// This function initializes the tool on wake.
/// </summary>
function ProjectileAnimTool::onWake(%this)
{
   $IsProjectileAnimToolAwake = true;   
   
   if ($PAToolInitialized == false)
   {
      $GameDir = LBProjectObj.gamePath;

      
      $PAToolInitialized = true;   
   }
   
   PAToolTabBook.selectPage(0);

   %this.initializePreviews();
   %this.awake = true;
   $Tools::TemplateToolDirty = false;
}

/// <summary>
/// This function handles setting up the animation previews.
/// </summary>
function ProjectileAnimTool::initializePreviews(%this)
{
   if (!isObject(%this.animationSet))
   {
      %this.createAnimationSet();
      %this.setIsNew = true;
   }
   else
      %this.setIsNew = false;

   PAToolAnimSetName.text = %this.animationSet.getName();
   
   PAToolTravelAnimPreview.display(%this.animationSet.TravelAnim);
   PAToolTravelAnimEdit.text = %this.animationSet.TravelAnim;

   PAToolHitAnimPreview.display(%this.animationSet.HitAnim);
   PAToolHitAnimEdit.text = %this.animationSet.HitAnim;
}

/// <summary>
/// This function handles putting the tool to sleep.
/// </summary>
function ProjectileAnimTool::onSleep(%this)
{
   $IsProjectileAnimToolAwake = false;   
   
    %this.awake = false;
}

/// <summary>
/// This function clears the contents of the PAToolAnimSetName text box.
/// </summary>
function PAAnimSetNameClearButton::onClick(%this)
{
   PAToolAnimSetName.text = "";
}

/// <summary>
/// This function opens the Asset Library to select an animation for the projectile's 
/// travel animation.
/// </summary>
function PAToolTravelAnimSelectButton::onClick(%this)
{
   // Access animations via the Asset Selector
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation returned from the Asset Library to the projectile's 
/// travel animation.
/// </summary>
/// <param name="asset">The asset passed back from the Asset Library.</param>
function PAToolTravelAnimSelectButton::setSelectedAsset(%this, %asset)
{
   PAToolTravelAnimPreview.display(%asset, t2dAnimatedSprite);
   PAToolTravelAnimEdit.text = %asset;
   if (ProjectileAnimTool.awake)
      SetProjectileAnimToolDirtyState(true);
}

/// <summary>
/// This function opens the Asset Library to select an animation for the projectile's 
/// hit animation.
/// </summary>
function PAToolHitAnimSelectButton::onClick(%this)
{
   // Access animations via the Asset Selector
   AssetLibrary.open(%this, $AnimatedSpritePage);
}

/// <summary>
/// This function assigns the animation returned from the Asset Library to the projectile's 
/// hit animation.
/// </summary>
/// <param name="asset">The asset passed back from the Asset Library.</param>
function PAToolHitAnimSelectButton::setSelectedAsset(%this, %asset)
{
   PAToolHitAnimPreview.display(%asset, t2dAnimatedSprite);
   PAToolHitAnimEdit.text = %asset;
   if (ProjectileAnimTool.awake)
        SetProjectileAnimToolDirtyState(true);
}

/// <summary>
/// This function handles playing/pausing the travel animation when clicking over
/// the preview window.
/// </summary>
function PAToolTravelAnimClick::onMouseUp(%this)
{
    PAToolTravelAnimPlayBtn.onClick();
}

/// <summary>
/// This function handles playing/pausing the hit animation when clicking over
/// the preview window.
/// </summary>
function PAToolHitAnimClick::onMouseUp(%this)
{
   PAToolHitAnimPlayBtn.onClick();
}

/// <summary>
/// This function saves the projectile animation set.
/// </summary>
function ProjectileAnimSaveButton::onClick(%this)
{
   // Save all the things!
   ProjectileAnimTool.saveAnimSet();
   SetProjectileAnimToolDirtyState(false);
}

/// <summary>
/// This function updates the animation preview play buttons.
/// </summary>
function PAToolTabBook::onTabSelected(%this)
{
   // Return is the tool is not active
   if (!$IsProjectileAnimToolAwake)   
      return;    
   
   if (isObject(PAToolHitAnimPreview))
   {
		PAToolTravelAnimPreview.playBtn = PAToolTravelAnimPlayBtn;
		PAToolHitAnimPreview.playBtn = EAToolNDiePlayBtn;
		PAToolHitAnimPreview.play();
        PAToolHitAnimPreview.pause();
        PAToolHitAnimPreview.sprite.setAnimationFrame(0);
   }
}

/// <summary>
/// This function handles playing/pausing the travel animation via the play/pause button.
/// </summary>
function PAToolTravelAnimPlayBtn::onClick(%this)
{
    if (PAToolTravelAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        PAToolTravelAnimPreview.play();
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        PAToolTravelAnimPreview.pause();
    }
}

/// <summary>
/// This function handles playing/pausing the hit animation via the play/pause button.
/// </summary>
function PAToolHitAnimPlayBtn::onClick(%this)
{
    if (PAToolHitAnimPreview.sprite.paused)
    {
        %this.setBitmap("templates/commonInterface/gui/images/pauseButton.png");
        PAToolHitAnimPreview.play();
        %anim = PAToolHitAnimPreview.sprite.getAnimation();
        %frames = getWordCount(%anim.animationFrames);
        %currentFrame = PAToolHitAnimPreview.sprite.getAnimationFrame();
        %remainingFrames = %frames - %currentFrame;
        %frameTime = %anim.animationTime / %frames;
        %playTime = %remainingFrames * %frameTime * 1000;
        %this.resetSchedule = %this.schedule(%playTime, resetBitmap, "templates/commonInterface/gui/images/playButton.png");
    }
    else
    {
        %this.setBitmap("templates/commonInterface/gui/images/playButton.png");
        cancel(%this.resetSchedule);
        PAToolHitAnimPreview.pause();
    }
}

/// <summary>
/// This function resets the button image on a non-looping animation's play/pause button.
/// </summary>
/// <param name="imageFile">The name of the image to set the button state to.</param>
function PAToolHitAnimPlayBtn::resetBitmap(%this, %imageFile)
{
    %this.setBitmap(%imageFile);
    PAToolHitAnimPreview.sprite.setAnimationFrame(0);
    PAToolHitAnimPreview.pause();
}