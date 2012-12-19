//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

new ScriptObject(CollisionSidebar)
{};

function CollisionSidebar::load(%this, %selectedType, %fromOtherTool)
{
    EditorEventManager.subscribe(%this, "_TSSCollisionEditorOpen", "onCEOpen");
    EditorEventManager.subscribe(%this, "_TSSCollisionEditorClose", "onCEClose");

    %this.projectileSet = $PrefabSet.findObjectByInternalName("ProjectileSet");
    %this.launcherSet = $PrefabSet.findObjectByInternalName("LauncherSet");
    %this.worldObjectSet = $PrefabSet.findObjectByInternalName("WorldObjectSet");
    
    EditorShellGui.clearViews();

    %this.CollisionObjectContainer = createVerticalScrollContainer();
    EditorShellGui.addView(%this.CollisionObjectContainer, "smallMedium");

    %this.CollisionObjectContainer.setSpacing(2);
    %this.CollisionObjectContainer.setScrollRepeat(70);
    %this.CollisionObjectContainer.setScrollCallbacks(true);
    %this.CollisionObjectContainer.setScrollHandler("CollisionSidebarScrollHandler");
    %this.CollisionObjectContainer.setIndicatorImage("{EditorAssets}:indicationArrowImage");
    %this.CollisionObjectContainer.setNormalProfile(GuiLargePanelContainer);
    %this.CollisionObjectContainer.setHighlightProfile(GuiLargePanelContainerHighlight);
    %this.CollisionObjectContainer.addHeader(%this.createHeader());

    EditorShellGui.addView(CollisionEditorGui, "largest");
    
    %this.ObjectDropdown.setSelected(%selectedType);
    
    if (!%fromOtherTool)
    {
        %this.CollisionObjectContainer.setSelected(0);
    }
    else
    {
        // find correct button and set it as selected
        %this.schedule(64, "selectButton");
    }
}

function CollisionSidebar::onCEOpen(%this, %msgData)
{
    if ( isObject($CollisionSidebar::helpManager) )
    {
        $CollisionSidebar::helpManager.stop();
        $CollisionSidebar::helpManager.delete();
    }
    $CollisionSidebar::helpManager = createHelpMarqueeObject("CollisionToolTips", 10000, "{PhysicsLauncherTools}");
    $CollisionSidebar::helpManager.openHelpSet("collisionToolHelp");
    $CollisionSidebar::helpManager.start();
}

function CollisionSidebar::onCEClose(%this, %msdData)
{
    if ( isObject($CollisionSidebar::helpManager) )
    {
        $CollisionSidebar::helpManager.stop();
        $CollisionSidebar::helpManager.delete();
    }
}

function CollisionSidebarScrollHandler(%childStart, %childRelStart, %childPos, %childRelPos)
{
}

function CollisionSidebar::selectButton(%this)
{
    %name = CollisionEditor.sourceObject.objectName;
    %this.findButton(%name);
}

function CollisionSidebar::findButton(%this, %name)
{
    if (%name $= "")
    {
        echo(" @@@ CollisionSidebar::findButton() : parameter blank");
        return;
    }
    for (%i = 0; %i < %this.CollisionObjectContainer.contentPane.getCount(); %i++)
    {
        %button = %this.CollisionObjectContainer.getButton(%i);
        %count = %button.getCount();
        %k = 0;
        %obj = %button.getObject(%k);
        while( isObject(%obj) )
        {
            if (%obj.objectName $= %name)
            {
                %this.select(%i);
                return;
            }
            %k++;
            %obj = %button.getObject(%k);
        }
    }
}

function CollisionSidebar::clearObjectList()
{
    %this.CollisionObjectContainer.clear();
}

function CollisionSidebar::select(%this, %index)
{
    %this.CollisionObjectContainer.setSelected(%index);
}

function CollisionSidebar::populateObjectList(%this, %type)
{
    %this.CollisionObjectContainer.clear();

    %this.CollisionObjectContainer.toggleBatch(true);
    switch$ (%type)
    {
        case "All":
            for (%i = 0; %i < %this.projectileSet.getCount(); %i++)
            {
                %object = %this.projectileSet.getObject(%i);
                %button = %this.createObjectButton(%object, "Projectiles", %i);
                %objCommand = "ProjectileBuilder::openCollisionEditor";
                %buttonData = %i TAB %object TAB %type TAB %objCommand;
                %this.CollisionObjectContainer.addButton(%button, "CollisionSidebar", "selectObject", %buttonData);
            }
            for (%i = 0; %i < %this.worldObjectSet.getCount(); %i++)
            {
                %object = %this.worldObjectSet.getObject(%i);
                %button = %this.createObjectButton(%object, "World Objects", %i);
                %objCommand = "WorldObjectBuilder::openCollisionEditor";
                %buttonData = %i TAB %object TAB %type TAB %objCommand;
                %this.CollisionObjectContainer.addButton(%button, "CollisionSidebar", "selectObject", %buttonData);
            }
            for (%i = 0; %i < %this.launcherSet.getCount(); %i++)
            {
                %object = %this.launcherSet.getObject(%i);
                %button = %this.createObjectButton(%object, "Launchers", %i);
                %objCommand = "SlingshotLauncherBuilder::openCollisionEditor";
                %buttonData = %i TAB %object TAB %type TAB %objCommand;
                %this.CollisionObjectContainer.addButton(%button, "CollisionSidebar", "selectObject", %buttonData);
            }

        case "Projectiles":
            for (%i = 0; %i < %this.projectileSet.getCount(); %i++)
            {
                %object = %this.projectileSet.getObject(%i);
                %button = %this.createObjectButton(%object, "Projectiles", %i);
                %objCommand = "ProjectileBuilder::openCollisionEditor";
                %buttonData = %i TAB %object TAB %type TAB %objCommand;
                %this.CollisionObjectContainer.addButton(%button, "CollisionSidebar", "selectObject", %buttonData);
            }

        case "World Objects":
            for (%i = 0; %i < %this.worldObjectSet.getCount(); %i++)
            {
                %object = %this.worldObjectSet.getObject(%i);
                %button = %this.createObjectButton(%object, "World Objects", %i);
                %objCommand = "WorldObjectBuilder::openCollisionEditor";
                %buttonData = %i TAB %object TAB %type TAB %objCommand;
                %this.CollisionObjectContainer.addButton(%button, "CollisionSidebar", "selectObject", %buttonData);
            }

        case "Launchers":
            for (%i = 0; %i < %this.launcherSet.getCount(); %i++)
            {
                %object = %this.launcherSet.getObject(%i);
                %button = %this.createObjectButton(%object, "Launchers", %i);
                %objCommand = "SlingshotLauncherBuilder::openCollisionEditor";
                %buttonData = %i TAB %object TAB %type TAB %objCommand;
                %this.CollisionObjectContainer.addButton(%button, "CollisionSidebar", "selectObject", %buttonData);
            }

    }
    %this.CollisionObjectContainer.toggleBatch(false);
}

function CollisionSidebar::selectObject(%this, %data)
{
    %object = getField(%data, 1);
    
    %type = getField(%data, 2);

    %command = getField(%data, 3);

    if (%this.selectedObject !$= "")
        %this.lastObject = %this.selectedObject;
    else
        %this.lastObject = %object;
        
    %this.selectedObject = %object;

    // Get temp object to replicate what the prefab is, since we are not editing the prefab itself
    // need the dimensions, collision shapes, and visual representation (imageMap or animation)
    eval(%command @ "(" @ %object @ ", " @ %this @ ");");
    
    %this.lastObject = %object;
}

function CollisionSidebar::createObjectButton(%this, %object, %type, %index)
{
    %buttonText = "";
    
    if (isObject(%object)) 
        %buttonText = %object.getInternalName();
    else
        return;

    %control = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiNarrowPanelContainer";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="122 96";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
            index = %index;
    };

    switch$ (%type)
    {
        case "Projectiles":
            %anim = ProjectileBuilder::getIdleInLauncherAnim(%object);

        case "World Objects":
            %anim = getWord(WorldObjectBuilder::getAnimationList(%object), 1);

        case "Launchers":
            %anim = SlingshotLauncherBuilder::getForkBackgroundAsset(%object);
    }

    %previewAsset = AssetDatabase.acquireAsset(%anim);
    %previewClass = %previewAsset.getClassName();
    AssetDatabase.releaseAsset(%anim);

    %preview = new GuiSpriteCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="33 10";
        Extent="55 55";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
            objectName = %object.getInternalName();
    };
    if (%previewClass $= "AnimationAsset")
        %preview.Animation= %anim;
    else
        %preview.Image=%anim;
    %control.addGuiControl(%preview);

    %text = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextCenterProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="5 74";
        Extent="112 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
        text=%buttonText;
        maxLength="1024";
        truncate="1";
    };
    
    %control.addGuiControl(%text);
        
    return %control;
}

function CollisionSidebar::createHeader(%this)
{
    %control = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="left";
        VertSizing="top";
        Position="0 0";
        Extent="152 50";
        MinExtent="8 2";
        canSave = "0";
        Visible="1";
        hovertime="1000";
    };

    %label = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="relative";
        VertSizing="relative";
        Position="70 0";
        Extent="32 18";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text="Type";
        maxLength="1024";
    };
    %control.addGuiControl(%label);

    %dropDown = new GuiPopUpMenuCtrl()
    {
        class="CEObjectDropdown";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiPopUpMenuProfile";
        HorizSizing="relative";
        VertSizing="relative";
        Position="18 20";
        Extent="134 25";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Choose the Type of Collisions you want to edit between Projectiles, Objects, Launchers or display All.";
        text="All";
        maxLength="1024";
        maxPopupHeight="200";
        sbUsesNAColor="0";
        reverseTextList="0";
        bitmapBounds="16 16";
    };
    %control.addGuiControl(%dropDown);
    %this.ObjectDropdown = %dropDown;

    %dropDown.clear();
    %dropDown.add("All", 0);
    %dropDown.add("Projectiles", 1);
    %dropDown.add("World Objects", 2);
    %dropDown.add("Launchers", 3);

    return %control;
}

// Callback that is executed when the collision editor saves any changes to
// the object that was passed in for editing
function CollisionSidebar::onCollisionEditSave(%this, %object)
{
    if ( %this.lastObject.getClassName() $= "SceneObjectGroup" )
        %collisionObject = %this.lastObject.findObjectByInternalName($SlingshotLauncherBuilder::CollisionObjectInternalName);
    else
        %collisionObject = %this.lastObject;
    PhysicsLauncherTools::copyCollisionShapes(%object, %collisionObject);
    PhysicsLauncherTools::writePrefabs();
}

function CEObjectDropdown::onSelect(%this)
{
    %selectionText = %this.getValue();

    CollisionSidebar.populateObjectList(%selectionText);
    CollisionSidebar.select(0);
}