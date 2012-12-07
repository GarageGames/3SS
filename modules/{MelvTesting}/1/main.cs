//OpenRemoteDebugger( 1, 1010, "abc" );

function startMelvTesting()
{
    setScreenMode( 1920, 1200, 32, false );
    //setScreenMode( 2560, 1600, 32, true );

	$NoImageRenderProxy = new RenderProxy()
	{
		ImageMap = "{PhysicsLauncherAssets}:defaultImageImageMap";
		//Animation = "{PhysicsLauncherAssets}:PL_GorillaFlightProjectileAnim";
	};
	
	testScreen();
	//testQueryAssetName();
	//testQueryAssetType();
	//testAssetAcquisitionAndRelease();
	
	//%sprite1 = new Sprite();
	//testScene.addToScene( %sprite1 );
	//%sprite1.setSize( 15, 15 );
	//%sprite1.ImageMap = "{MelvTesting}:isotiles2";
	//%sprite1.Frame = 0;
	//%sprite1.setSceneLayer( 31 );
	//%sprite1.setSceneLayerDepth( 0 );
//
	//%sprite2 = new Sprite();
	//testScene.addToScene( %sprite2 );
	//%sprite2.setPosition( 4, 4 );
	//%sprite2.setSize( 15, 15 );
	//%sprite2.ImageMap = "{MelvTesting}:isotiles2";
	//%sprite2.Frame = 2;
	//%sprite2.setSceneLayer( 31 );
	//%sprite2.setSceneLayerDepth( 1 );
//
	//%sprite3 = new Sprite();
	//testScene.addToScene( %sprite3 );
	//%sprite3.setPosition( 4, -2 );
	//%sprite3.setSize( 15, 15 );
	//%sprite3.ImageMap = "{MelvTesting}:isotiles2";
	//%sprite3.Frame = 4;
	//%sprite3.setSceneLayer( 31 );
	//%sprite3.setSceneLayerDepth( 15 );
	
	//%sprite3.setSceneLayerDepthForward();
	//%sprite3.setSceneLayerDepthForward();


    //testScene.layerSortMode31 = "-Y Axis";
    //testScene.layerSortMode30 = "-Y Axis";

    //testScene.layerSortMode31 = "z axis";
    //testScene.layerSortMode30 = "z axis";
    
    testScene.setDebugOn( 0, 2, 3 );
    
    
    %asset = new ImageAsset()
    {
        ImageFile = expandPath("^{MelvTesting}/assets/images/Curiosity.jpg");
    };
    
    %assetId = AssetDatabase.addPrivateAsset( %asset );

    %sprite = new Sprite();
    testScene.addToScene( %sprite );    
    %sprite.imageMap = %assetId;
    %sprite.setSize( 100, 75 );
    
    AssetDatabase.removeSingleDeclaredAsset( %assetId );
    //return;
    
    return;    
	
	%composite = new CompositeSprite();
	testScene.addToScene( %composite );
	%composite.BatchIsolated = "true";

    %composite.setDefaultSpriteStride( 5.2345678, 5.2345678 );
    %composite.setDefaultSpriteSize( 4.56789 );
    
    %composite.setAngle( 30.0001 );
	
	%frame = 0;
	
    for ( %y = -3; %y < 3; %y++ )
	{
	    for ( %x = -3; %x < 3; %x++ )
        {
            %composite.addSprite( %x, %y );
            //%composite.setSpriteImage( "{MelvTesting}:isotiles2", getRandom(0,4) );
            %composite.setSpriteImage( "{MelvTesting}:MiniTileMapImage", %frame );
            //%composite.setSpriteAngle( getRandom(0,360) );
            //%composite.setSpriteVisible( getRandom(1,10) < 5 );
            
            %frame++;
            if ( %frame == 16 ) %frame = 0;
        }
	}
	
	testSceneWindow2D.setCurrentCameraPosition( 0, 0 );
	
	//%composite.setAngularVelocity( 20 );
	
    //%assetTags = AssetDatabase.getAssetTags();    
    //%assetTags.createtag( "Test" );
    //%assetTags.createtag( "Image" );
    //%assetTags.createtag( "Animation" );
    
    //%assetTags.tag( "{MelvTesting}:MiniTileMapAnim", "Test" );
    //%assetTags.tag( "{MelvTesting}:MiniTileMapImage", "Test" );    
    //%assetTags.tag( "{MelvTesting}:CuriosityImage", "Test" );    
    //%assetTags.tag( "{MelvTesting}:MiniTileMapImage", "Image" );    
    //%assetTags.tag( "{MelvTesting}:CuriosityImage", "Image" );    
    //%assetTags.tag( "{MelvTesting}:MiniTileMapAnim", "Animation" );
    
    //AssetDatabase.saveAssetTags();
	//testQueryAssetTag();
	//AssetDatabase.deleteAsset( "{MelvTesting}:MiniTileMapImage", true, true );

    //AssetDatabase.IgnoreAutoUnload = true;
    // Scroller.
    //%scroller = new Scroller();
    //%scroller.setSize( 100, 75 );
    //%scroller.scrollX = -30;
    //%scroller.scrollY = 15;
    //%scroller.repeatX = 4;
    //%scroller.repeatY = 3;
    //%scroller.setScrollPositionX( 20 );
    //%scroller.imageMap = "{PhysicsLauncherAssets}:PL_GorillaProjectileImageMap";
    //%scroller.imageMap = "";
    //%scroller.Animation = "{PhysicsLauncherAssets}:MiniTileMapAnim";
    //%scroller.frame = 80;
    //%scroller.imageMap = "{PhysicsLauncherAssets}:CreditsImageImageMap";
    
    //AssetDatabase.IgnoreAutoUnload = false;
    
    //testScene.addToScene( %scroller );        

    //TamlWrite( testScene, "testScene.taml" );
    //%scene = TamlRead( "testScene.taml" );
    //TamlWrite( %scene, "testSceneOut.taml" );
    //quit();

	//%assetTagManifest = AssetDatabase.getAssetTags();

	//%assetTagManifest.createTag( "Backgrounds" );
	//%assetTagManifest.tag( "{PhysicsLauncherAssets}:Level1BackgroundImageMap", "Effects" );
	//%assetTagManifest.untag( "{PhysicsLauncherAssets}:Level1BackgroundImageMap", "Effects" );
	//%assetTagManifest.deleteTag( "Projectiles" );
	//%assetId = "{PhysicsLauncherAssets}:PL_GorillaFlightProjectileAnim";
	//%tagCount = %assetTagManifest.getAssetTagCount(%assetId);
	//for ( %index = 0; %index < %tagCount; %index++ )
	//{
	//    echo( %assetTagManifest.getAssetTag(%assetId, %index) );
	//}

	//AssetDatabase.dumpDeclaredAssets();


	//%asset = AssetDatabase.acquireAsset( "{PhysicsLauncherAssets}:PL_GorillaProjectileImageMap" );
	//%assetId = %asset.getAssetId();
	//testAssetDependsOn( %assetId );
	//testAssetIsDependedOn( %assetId );

	//%assetSnapshot = new AssetSnapshot();
	//AssetDatabase.getAssetSnapshot( %assetSnapshot, %assetId );
	//%asset.assetName = "Should not be able to do this!";
	//%asset.AssetDescription = "New asset description:" SPC getRandom();
	//AssetDatabase.setAssetSnapshot( %assetSnapshot, %assetId );
	//%asset.refreshAsset();
	//AssetDatabase.refreshAllAssets(true);


    //AssetDatabase.purgeAssets();

	//%asset.refreshAsset();

	//AssetDatabase.renameDeclaredAsset( "{PhysicsLauncherAssets}:Background9ImageMap", "{PhysicsLauncherAssets}:Background1ImageMap" );

	//AssetDatabase.saveAssetTags();

	//%scene = TamlRead( "^Modules/{PhysicsLauncher}/1/data/levels/world1level1.scene.taml" );
	//testSceneWindow2D.setScene( %scene );

	//testSpriteCtrl.delete();
	//$NoImageRenderProxy.delete();
	
	//alxPlay( "{PhysicsLauncherAssets}:PL_LaunchSound" );
	//alxPlay( "{PhysicsLauncherAssets}:PL_TitleSound" );


	//%asset = AssetDatabase.acquireAsset( "{PhysicsLauncherAssets}:PL_TitleSound" );
	
	//%asset.AudioFile="#PL_TitleAudio.wav";
    //echo( ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>" SPC %asset.AudioFile );	
	
	//%asset.refreshAsset();
	//refreshTextureManager();
	
}

function testScreen()
{
    new SceneWindow(testSceneWindow2D);
    new Scene(testScene);
    testSceneWindow2D.setScene( testScene );
    Canvas.setContent(testSceneWindow2D);   
    
    //new GuiSceneObjectCtrl(testT2DViewer);    
    //testT2DViewer.position = "0 0";
    //testT2DViewer.extent = "400 400";
    //Canvas.setContent(testT2DViewer);
    
    //new guiSpriteCtrl(testSpriteCtrl);
    //Canvas.setContent(testSpriteCtrl);
    
    //new GuiImageButtonCtrl(testImageButtonCtrl);
    //Canvas.setContent(testImageButtonCtrl);    
}

function testQueryAssetName()
{
    %assetName = "PL_DefaultProjectile";
    %assetQuery = new AssetQuery();
    AssetDatabase.findAssetName( %assetQuery, %assetName );
    
    %assetCount = %assetQuery.Count;

    echo( "Asset Query: Search for asset name:" SPC %assetName SPC "found" SPC %assetCount SPC "asset(s):" );
    
    for( %index = 0; %index < %assetCount; %index++ )
    {
        %assetId = %assetQuery.getAsset( %index );
        
        echo( "> #" @ %index @ ":" SPC %assetId );
    }
    
    %assetQuery.delete();
}

function testQueryAssetType()
{
    %assetType = "ImageAsset";
    //%assetType = "AnimationAsset";
    %assetQuery = new AssetQuery();
    AssetDatabase.findAssetType( %assetQuery, %assetType );

    %assetBase = new AssetQuery();
    %assetBase.set( %assetQuery );    
    
    %assetCount = %assetBase.Count;

    echo( "Asset Query: Search for asset type:" SPC %assetType SPC "found" SPC %assetCount SPC "asset(s):" );
    
    for( %index = 0; %index < %assetCount; %index++ )
    {
        %assetId = %assetBase.getAsset( %index );
        
        echo( "> #" @ %index @ ":" SPC %assetId );
    }    
    
    %assetQuery.delete();
    %assetBase.delete();
}

function testQueryAssetTag()
{
    %assetTag1 = "test";
    %assetTag2 = "animation";
    %assetQuery = new AssetQuery();
    AssetDatabase.findTaggedAssets( %assetQuery, %assetTag1 );
    AssetDatabase.findTaggedAssets( %assetQuery, %assetTag2, true );
    
    %assetCount = %assetQuery.Count;

    echo( "*********************************************************************************************************************************************************" );
    echo( "Asset Query: Search for asset tag found" SPC %assetCount SPC "asset(s):" );
    
    for( %index = 0; %index < %assetCount; %index++ )
    {
        %assetId = %assetQuery.getAsset( %index );
        
        echo( "> #" @ %index @ ":" SPC %assetId );
    }

    echo( "*********************************************************************************************************************************************************" );
    
    %assetQuery.delete();
}

function testAssetDependsOn( %assetId )
{
    %assetQuery = new AssetQuery();
    
    AssetDatabase.findAssetDependsOn( %assetQuery, %assetId );
    
    %assetCount = %assetQuery.Count;

    echo( "Asset Query: Search Asset Id" SPC %assetId SPC "depends-on found" SPC %assetCount SPC "asset(s):" );
    
    for( %index = 0; %index < %assetCount; %index++ )
    {
        %assetId = %assetQuery.getAsset( %index );
        
        echo( "> #" @ %index @ ":" SPC %assetId );
        
        testAssetDependsOn( %assetId );       
    }
    
    %assetQuery.delete();   
}

function testAssetIsDependedOn( %assetId )
{
    %assetQuery = new AssetQuery();
    
    AssetDatabase.findAssetIsDependedOn( %assetQuery, %assetId );
    
    %assetCount = %assetQuery.Count;

    echo( "Asset Query: Search Asset Id" SPC %assetId SPC "is-depended-on found" SPC %assetCount SPC "asset(s):" );
    
    for( %index = 0; %index < %assetCount; %index++ )
    {
        %assetId = %assetQuery.getAsset( %index );
                      
        echo( "> #" @ %index @ ":" SPC %assetId );
        
        testAssetIsDependedOn( %assetId );        
    }
    
    %assetQuery.delete();   
}

function testAssetAcquisitionAndRelease()
{
    //%asset = AssetDatabase.acquireAsset( "{PhysicsLauncherAssets}:BackgroundOneImageMap" );
    //AssetDatabase.releaseAsset( %asset.getAssetId() );

    //AssetDatabase.acquireAsset( "{PhysicsLauncherAssets}:PL_GorillaFlightProjectileAnim" );

    // Static Sprite#1
    //%sprite1 = new Sprite();
    //%sprite1.imageMap = "{PhysicsLauncherAssets}:Background1ImageMap";
    //%sprite1.setImageMap( "{PhysicsLauncherAssets}:Background1ImageMap" );
    //%sprite1.setSize( 100, 75 );
    //echo( "SPRITE >>>>>>>>>>>>>>>>>" SPC %sprite1.imageMap );
    //testScene.addToScene( %sprite1 );    

    // Static Sprite#1
    //%sprite2 = new Sprite();
    //%sprite2.imageMap = "{PhysicsLauncherAssets}:Background2ImageMap";
    //%sprite2.setSize( 100, 75 );
    //testScene.addToScene( %sprite2 );

    //AssetDatabase.acquireAsset( "{PhysicsLauncherAssets}:PL_DefaultProjectileBlue1ImageMap" );
    
    // Animated Sprite.    
    //%sprite3 = new Sprite();
    //%sprite3.playAnimation("{PhysicsLauncherAssets}:PL_GorillaFlightProjectileAnim");
    //%sprite3.setSize( 20, 20 );
    //testScene.addToScene( %sprite3 );

    //TamlWrite( %sprite, "sprite.taml" );
    //%sprite.delete();    
    //%newSprite = TamlRead( "sprite.taml" );
    //%newSprite.delete();

    //%sprite1 = new Sprite();
    //%sprite1.imageMap = "{PhysicsLauncherAssets}:Background1ImageMap";    
    //testT2DViewer.setSceneObject( %sprite1 );
    
    //testSpriteCtrl.Image = "{PhysicsLauncherAssets}:PL_GorillaProjectileImageMap";
    //testSpriteCtrl.Frame = 5;
    //testSpriteCtrl.Animation = "{PhysicsLauncherAssets}:PL_GorillaFlightProjectileAnim";
    //echo( ">>>>>>>>>>>>>>>>>>>>>>" SPC testSpriteCtrl.Animation );
    //testSpriteCtrl.setAnimation( "{PhysicsLauncherAssets}:PL_GorillaFlightProjectileAnim" );
    //testImageButtonCtrl.NormalImage = "{PhysicsLauncherAssets}:PL_DefaultProjectileBlue1ImageMap";
    //testImageButtonCtrl.HoverImage = "{PhysicsLauncherAssets}:PL_DefaultProjectileBlue2ImageMap";
    //testImageButtonCtrl.DownImage = "{PhysicsLauncherAssets}:PL_DefaultProjectileBlue3ImageMap";
    //testImageButtonCtrl.InactiveImage = "{PhysicsLauncherAssets}:PL_DefaultProjectileBlue4ImageMap";

        
    
    //TamlWrite( testSpriteCtrl, "spritectrl.taml" );
    
}
