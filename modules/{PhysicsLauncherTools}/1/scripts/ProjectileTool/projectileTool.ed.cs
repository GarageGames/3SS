//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$ProjectileDefaultName = "New Projectile";
$ProjectileObjDefaultName = "NewProjectile";

new ScriptObject(ProjectileTool)
{};

/// <summary>
/// This function saves the current list of projectiles to the game project's
/// prefabs list.
/// </summary>
function ProjectileTool::saveProjectiles(%this)
{
    // Write out templateObjectSet to taml
    %prefabFile = expandPath("^PhysicsLauncherTemplate/managed/prefabs.taml");

    if (isFile(%prefabFile))
        TamlWrite($PrefabSet, %prefabFile); 
}

/// <summary>
/// This function sets up the Projectile Tool and loads the current projectile
/// data for editing.
/// </summary>
function ProjectileTool::load(%this)
{
    %this.helpManager = createHelpMarqueeObject("ProjectileToolHints", 10000, "{PhysicsLauncherTools}");
    %this.helpManager.openHelpSet("projectileToolHelp");
    %this.helpManager.start();

    EditorShellGui.clearViews();

    %this.ProjectileContainer = createVerticalScrollContainer();
    EditorShellGui.addView(%this.ProjectileContainer, "medium");

    EditorShellGui.addView(ProjectileToolForm, "large");

    %this.validateProjectileSet();
    // Setup first view
    %this.ProjectileContainer.setSpacing(2);
    %this.ProjectileContainer.setScrollCallbacks(true);
    %this.ProjectileContainer.setIndicatorImage("{EditorAssets}:indicationArrowImage");
    %this.ProjectileContainer.setNormalProfile(GuiLargePanelContainer);
    %this.ProjectileContainer.setHighlightProfile(GuiLargePanelContainerHighlight);
    %this.ProjectileContainer.addHeader(%this.createHeader("Projectile List"));
    %this.refreshProjectileView();
    %this.ProjectileContainer.setSelected(0);
    
    initializeButtonStateDropdown(Pt_ButtonStateDropdown);
}

/// <summary>
/// This creates a simple header gui control for use in the list containers.
/// </summary>
/// <param name="text">The text to assign to the header's label.</param>
function ProjectileTool::createHeader(%this, %text)
{
    %control = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="266 24";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
    };
    
    %label = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="center";
        VertSizing="center";
        Position="18 2";
        Extent="230 18";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
        maxLength="1024";
        text=%text;
    };
    %control.addGuiControl(%label);
    
    return %control;
}

/// <summary>
/// This function selects the projectile data set to edit.  It saves the current 
/// data before changing to the new projectile.
/// </summary>
/// <param name="data">The index of the projectile to display.</param>
function ProjectileTool::selectProjectile(%this, %data)
{
    // save values to current projectile before changing to new one
    if (isObject(%this.selectedObject))
    {
        %this.selectedObject.setInternalName(Pt_ProjectileNameEdit.getText());
        Pt_ProjectileFrictionEdit.onValidate();
        Pt_ProjectileRestitutionEdit.onValidate();
        Pt_ProjectilePointValueEdit.onValidate();
        Pt_ProjectileMassEdit.onValidate();
        PhysicsLauncherTools::writePrefabs();
    }
    %projectileSet = $PrefabSet.findObjectByInternalName("ProjectileSet"); 

    %this.selectedObject = %projectileSet.getObject(%data);
    %this.selectedIndex = %data;
    ProjectileToolForm.refresh();
    %this.SetSelectedProjectileButton(%data);
}

/// <summary>
/// This function handles creating a new projectile from a template object.
/// It adds the new projectile directly to the prefab file.
/// </summary>
function ProjectileTool::createProjectile(%this, %index)
{
    if (isObject(%this.selectedObject))
    {
        %this.selectedObject.setInternalName(Pt_ProjectileNameEdit.getText());
        Pt_ProjectileFrictionEdit.onValidate();
        Pt_ProjectileRestitutionEdit.onValidate();
        Pt_ProjectilePointValueEdit.onValidate();
        Pt_ProjectileMassEdit.onValidate();
        PhysicsLauncherTools::writePrefabs();
    }

    %projectileSet = $PrefabSet.findObjectByInternalName("ProjectileSet"); 
    
    // Create a new projectile
    %newProjectile = ProjectileBuilder::createDefaultProjectile();

    if (isObject(%newProjectile))
    {
        // Assign a default name to the new object
        %nameCount = 1;
        %newProjectileName = $ProjectileDefaultName @ %nameCount;
        while (isObject(%projectileSet.findObjectByInternalName(%newProjectileName)))
        {
            %newProjectileName = $ProjectileDefaultName @ %nameCount;
            %nameCount++;
        }
        
        %newProjectile.setInternalName(%newProjectileName);
        %nameCount = 1;
        %newObjName = ($ProjectileObjDefaultName @ %nameCount);
        while (isObject(%newObjName))
        {
            %newObjName = $ProjectileObjDefaultName @ %nameCount++;
        }
        %newProjectile.setName(%newObjName);

        // Add the object to the ProjectileSet
        %projectileSet.add(%newProjectile);
    }
    
    //TamlWrite(%newProjectile, "./newProjectilePrefab.taml");
    %this.refreshProjectileView();
    %this.ProjectileContainer.setSelected(%index);
    %this.ProjectileContainer.scrollToButton(%index);
}

/// <summary>
/// This function refreshes the current tool view's data from the projectile list.
/// </summary>
function ProjectileTool::refreshProjectileView(%this)
{
    %projectileSet = $PrefabSet.findObjectByInternalName("ProjectileSet");
    
    %this.ProjectileContainer.clear();
    
    for (%i = 0; %i < %projectileSet.getCount(); %i++)
    {
        %projectile = %projectileSet.getObject(%i); 
        
        // Get the image from the idle state of the projectile
        %asset = ProjectileBuilder::getIdleInLauncherAnim(%projectile);
        %frame = ProjectileBuilder::getIdleInLauncherAnimFrame(%projectile);     
        
        %control = new GuiControl(){
            canSaveDynamicFields="0";
            isContainer="1";
            Profile="GuiLargePanelContainer";
            HorizSizing="right";
            VertSizing="bottom";
            Position="0 0";
            Extent="217 78";
            MinExtent="8 2";
            canSave = "0";
            Visible="1";
            hovertime="1000";
        };

        %image = new GuiSpriteCtrl(){
            canSaveDynamicFields="0";
            isContainer="0";
            Profile="GuiDefaultProfile";
            HorizSizing="right";
            VertSizing="bottom";
            Position="9 9";
            Extent="60 60";
            MinExtent="8 2";
        };
        
        if (AssetDatabase.getAssetType(%asset) $= "AnimationAsset")
        {
            %image.Animation = %asset;
        }
        else  
        {  
            %image.Image = %asset;
            %image.Frame = %frame;
        }        
        
        %aspectRatio = %projectile.getWidth()/%projectile.getHeight();
        PhysicsLauncherTools::setGuiControlAspectRatio(%image, %aspectRatio);
        
        %control.addGuiControl(%image);
        
        %text = new GuiTextCtrl(){
            canSaveDynamicFields="0";
            isContainer="0";
            Profile="GuiTextCenterProfile";
            HorizSizing="right";
            VertSizing="bottom";
            Position="72 30";
            Extent="133 18";
            MinExtent="8 2";
            canSave = "0";
            Visible="1";
            hovertime="1000";
            text=%projectile.getInternalName();
            maxLength="1024";
        };
        %control.addGuiControl(%text);

        %this.ProjectileContainer.addButton(%control, "ProjectileTool", "selectProjectile", %i);
    }
    
    // Add new button at end
    %control = new GuiControl(){
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="217 78";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select to add a new Projectile.";
    };

    %btnShape = new GuiControl(){
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLargePanelContainer";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="217 53";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
    };
    %control.addGuiControl(%btnShape);

    %image = new GuiSpriteCtrl(){
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="25 5";
        Extent="43 43";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
        Image="{EditorAssets}:addButton_normalImageMap";
        wrap="0";
        useSourceRect="0";
        sourceRect="0 0 0 0";
    };
    %control.addGuiControl(%image);
    
    %text = new GuiTextCtrl(){
        Name="DynamicButtonText";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="70 15";
        Extent="140 18";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
        text="Add New Projectile";
        maxLength="1024";
    };
    %control.addGuiControl(%text);

    %this.ProjectileContainer.addButton(%control, "ProjectileTool", "createProjectile", %i);
}

/// <summary>
/// This function checks that the ProjectileSet exists, and creates a new empty 
/// set if it does not exist.
/// </summary>
function ProjectileTool::validateProjectileSet()
{
    %projectileSet = $PrefabSet.findObjectByInternalName("ProjectileSet");
    if (!isObject(%projectileSet))
    {
        %projectileSet = new SimSet();
        %projectileSet.setInternalName("ProjectileSet");
        %projectileSet.setName("ProjectileSet");
        $PrefabSet.add(%projectileSet);
    }
}

function ProjectileTool::SetSelectedProjectileButton(%this, %position)
{
    %targetButton = %this.ProjectileContainer.getButton(%position);
    //%levelName = %this.currentWorlds[%this.worldIndex].LevelList[%position];

    if ( isObject ( %this.projectileSelectBtnCtrl ) )
    {
        if (%this.ProjectileContainer.scrollCtrl.isMember(%this.projectileSelectBtnCtrl))
            %this.ProjectileContainer.scrollCtrl.remove(%this.projectileSelectBtnCtrl);
        %this.projectileSelectBtnCtrl.delete();
    }
    %this.projectileSelectBtnCtrl = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfileModeless";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="122 500";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };
    %size = %this.ProjectileContainer.contentPane.Extent;
    %this.projectileSelectBtnCtrl.setExtent(%size.x, %size.y);
    %posY = %this.ProjectileContainer.scrollCtrl.getScrollPositionY();
    %this.projectileSelectBtnCtrl.setPosition(0, 0 - %posY);
    %this.ProjectileContainer.scrollCtrl.add(%this.projectileSelectBtnCtrl);

    %this.CreateProjectileHighlightButton(%position);

    %this.selectedProjectileBtn.SetVisible(true);
}

function ProjectileTool::CreateProjectileHighlightButton(%this, %i)
{
    if ( isObject( %this.selectedProjectileBtn ) )
        %this.selectedProjectileBtn.delete();

    %projectileSet = $PrefabSet.findObjectByInternalName("ProjectileSet");

    %projectile = %projectileSet.getObject(%i); 

    %posY = %this.ProjectileContainer.getButtonPosition(%i) + 2;

    // Get the image from the idle state of the projectile
    %asset = ProjectileBuilder::getIdleInLauncherAnim(%projectile);
    %frame = ProjectileBuilder::getIdleInLauncherAnimFrame(%projectile);     
    
    %control = new GuiControl(){
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLargePanelContainerHighlight";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0" SPC %posY;
        Extent="218 78";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
    };

    %image = new GuiSpriteCtrl(){
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="9 9";
        Extent="60 60";
        MinExtent="8 2";
    };
    
    if (AssetDatabase.getAssetType(%asset) $= "AnimationAsset")
    {
        %image.Animation = %asset;
    }
    else  
    {  
        %image.Image = %asset;
        %image.Frame = %frame;
    }        
    
    %aspectRatio = %projectile.getWidth()/%projectile.getHeight();
    PhysicsLauncherTools::setGuiControlAspectRatio(%image, %aspectRatio);
    
    %control.addGuiControl(%image);
    
    %text = new GuiTextCtrl(){
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="72 30";
        Extent="133 18";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
        text=%projectile.getInternalName();
        maxLength="1024";
        truncate="1";
    };
    %control.addGuiControl(%text);

    if (%projectileSet.getCount() > 1)
    {
        %remove = new GuiImageButtonCtrl()
        {
            canSaveDynamicFields="0";
            class="Pt_ProjectileRemove";
            isContainer="0";
            Profile="GuiTransparentProfile";
            HorizSizing="right";
            VertSizing="bottom";
            Position="189 5";
            Extent="25 25";
            MinExtent="8 2";
            canSave = "0";
            Visible="1";
            hovertime="1000";
            toolTipProfile="GuiToolTipProfile";
            toolTip="Delete this Projectile.";
            groupNum="-1";
            buttonType="PushButton";
            useMouseEvents="0";
            isLegacyVersion="0";
            NormalImage="{EditorAssets}:redCloseImageMap";
            HoverImage="{EditorAssets}:redClose_hImageMap";
            DownImage="{EditorAssets}:redClose_dImageMap";
                index = %i;
        };
        %control.addGuiControl(%remove);
    }

    %this.selectedProjectileBtn = %control;
    %control.setPosition(0, %posY);
    %this.projectileSelectBtnCtrl.addGuiControl(%control);
}

function ProjectileTool::removeProjectile(%this, %index)
{
    %projectileSet = $PrefabSet.findObjectByInternalName("ProjectileSet");
    %obj = %projectileSet.getObject(%index);
    
    %projectileSet.remove(%obj);

    // Replace any references to the removed projectile in all levels 
    ProjectileBuilder::removeProjectileFromAllLevels(%obj); 
    
    PhysicsLauncherToolsEventManager.postEvent("_Cleanup", %obj);
    ProjectileTool.refreshProjectileView();
    ProjectileToolForm.refresh();
    PhysicsLauncherToolsEventManager.postEvent("_ProjectileButtonUpdateComplete", 0);
}