//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function launchFontTool(%fontDatablock)
{
   FontTool.open(%fontDatablock);
}

function FontTool::open(%this, %fontDatablock)
{
   activatePackage(FontTool);
   
   FontToolFontName.setText("");
   FontToolSpriteSheet.setText("");
   FontToolSpacing.setText("0");
      
   Canvas.pushDialog(FontToolGui);
   
   if(%fontDatablock !$= "")
   {
      %this.newFont = false;
      %this.fontDatablock = %fontDatablock;
   }
   
   %this.updateGui();
}

function FontTool::close(%this)
{
   Canvas.popDialog(FontToolGui);
   
   %this.newFont = true;
   %this.baseDimension = 100;
   %this.fontDatablock = "";
   
   deactivatePackage(FontToolPackage);
}

function FontTool::destroy( %this )
{
   %this.fontDatablock = "";
   
   if(isObject(%this.scene))
      %this.scene.delete();
}

function FontTool::updateGui(%this)
{
   if(isObject(%this.fontDatablock))
   {
      FontToolFontName.setText(%this.fontDatablock.getName());
      FontToolSpriteSheet.setText(%this.fontDatablock.imageFile);
      FontToolSpacing.setText(%this.fontDatablock.spacing);
   }
   
   %this.refresh(true);
}

function FontTool::refresh(%this, %resize)
{
   if(!isObject(%this.fontDatablock))
      return;
   
   FontPreviewWindow.display(%this.fontDatablock, "BitmapFontObject");
   
   FontPreviewWindow.sprite.setText("Hello");
   
   %spacing = FontToolSpacing.getText();
   
   %this.fontDatablock.spacing = %spacing;
   
   FontPreviewWindow.sprite.characterPadding = %spacing;
}

function FontTool::save(%this)
{
   LBProjectObj.addDatablock( %this.fontDatablock, false );
   
   if(!ProjectNameTags.isMember(%this.fontDatablock))
      ProjectNameTags.add(%this.fontDatablock);
      
   %fontTagID = ProjectNameTags.getTagId("Font");
      
   ProjectNameTags.tag(%this.fontDatablock.getID(), %fontTagID);

   LBProjectObj.persistToDisk( true, false, false, false, false, true);
   
   // Update object library
   if(isObject(AssetLibrary))
      AssetLibrary.schedule(100, "updateGui");
   
   FontTool.close();
}

function FontTool::selectSpriteSheet(%this)
{
   AssetLibrary.close();
   
   AssetLibrary.open(%this, $SpriteSheetPage, "");
}

function FontTool::setSelectedAsset(%this, %spriteSheetDatablock)
{
   %this.fontDatablock = %spriteSheetDatablock;
   %this.updateGUI();
}

function FontTool::FontImporterValidateName(%this)
{
   %oldName = FontToolFontName.getText();
   %newName = validateDatablockName( %oldName );

   // Mangle name if conflict.
   %managledName = %newName;
   %mangleIndex = 0;
   while( isObject(%managledName) )
   {
      %managledName = %name @ "_" @ %mangledIndex++;            
   }
      
   // Set name.
   %this.fontDatablock.setname(%managledName);
   
   // Update importer GUI.   
   %this.updateGui();
}

function FontTool::FontImporterValidateSpacing(%this)
{
   if(%this.text < 0)
      %this.text = 0;
}