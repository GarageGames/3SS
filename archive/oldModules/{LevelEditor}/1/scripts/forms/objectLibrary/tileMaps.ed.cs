//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
$LBCTileMap = GuiFormManager::AddFormContent( "LevelBuilderSidebarCreate", "TileMaps", "LBCTileMap::CreateForm", "LBCTileMap::SaveForm", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBCTileMap::CreateForm( %contentCtrl )
{    

   // Create necessary objects.
   %base = ObjectLibraryBaseType::CreateContent( %contentCtrl, "LBOTTileMap" );
   %base.sortOrder = 5;

   // Set Caption.
   %base.caption = "Tilemaps";

   // Return Base.
   return %base;

}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBCTileMap::SaveForm( %formCtrl, %contentObj )
{
   %formCtrl.sSpritesExpanded = %contentObj.sSpritesExpanded;
}

function LBOTTileMap::refresh( %this, %resize )
{
   %this.destroy();
   
   %this.scene = new Scene();
   %this.tilemap = new TileMap() { scene = %this.scene; };
   
   $LB::ObjectLibraryGroup.add( %this.scene );
   $LB::ObjectLibraryGroup.add( %this.tilemap );
   
   // Find objectList
   %objectList = %this.findObjectByInternalName("ObjectList");
   %objectsAdded = 0;
   // Add Empty Layer
   %defaultLayerFile = expandPath("./newLayer.lyr");
   %tileLayer = new t2dStaticSprite()
   {
      scene = %this.Scene;
      imageMap = tileLayerIconImageMap;
   };
   %objectList.AddT2DObject( %tileLayer, %defaultLayerFile, "TileLayer", fileName( %defaultLayerFile ));
   
   
   // Refresh in resource manager
   %path = expandPath( "^project/data/tilemaps/" );
   removeResPath( %path @ "*" );
   addResPath( %path );
   
   %fileSpec = %path @ "*.lyr";
   
   for (%file = findFirstFile(%fileSpec); %file !$= ""; %file = findNextFile(%fileSpec))
   {
      %tilelayer = %this.tilemap.createTileLayer(1, 1, 1, 1);
      %tilelayer.loadTileLayer(%file);
      %tileLayer.setSize( %tileLayer.getTileCountX() * %tileLayer.getTileSizeX(),
                          %tileLayer.getTileCountY() * %tileLayer.getTileSizeY() );

      %objectList.AddT2DObject( %tilelayer, %file, "TileLayer", fileName( %file ) );
      %objectsAdded ++;
   }
   
   if( %resize )
   {
      if( %objectsAdded > 0 )
         %this.sizeToContents();
      else
         %this.instantCollapse();
   }
}


function LBOTTileMap::onRemove( %this )
{
   // I don't understand this, but sometimes in onSleep 
   // for this content, the %this object is bad. :(
   if( !isObject( %this ) )
      return;

   %this.destroy();
}

function LBOTTileMap::destroy(%this)
{
   if (isObject(%this.scene))
   {
      %this.tilemap.delete();
      %this.scene.delete();
   }
      
   // Find objectList
   %objectList = %this.findObjectByInternalName("ObjectList");

   if( !isObject( %objectList ) )
      return;

   while( %objectList.getCount() > 0 )
   {
      %object = %objectList.getObject( 0 );
      if( isObject( %object ) )
         %object.delete();
      else
         %objectList.remove( %object );
   }
}

function LBOTTileMap::onContentMessage(%this, %sender, %message)
{
   %messageCommand = GetWord( %message, 0 );
   switch$( %messageCommand )
   {
      case "refresh":
         %this.refresh();
         
      case "destroy":
         %this.destroy();
   }
}
