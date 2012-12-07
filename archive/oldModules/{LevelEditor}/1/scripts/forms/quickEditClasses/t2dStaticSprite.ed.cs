//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
GuiFormManager::AddFormContent( "LevelBuilderQuickEditClasses", "t2dStaticSprite", "LBQEStaticSprite::CreateContent", "LBQEStaticSprite::SaveContent", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBQEStaticSprite::CreateContent( %contentCtrl, %quickEditObj )
{
   %base = %contentCtrl.createBaseStack("LBQEStaticSpriteClass", %quickEditObj);
   
   //Static sprite options
   %rollout = %base.createRolloutStack("Static Sprite", true);
   %imageMapList = %rollout.createT2DDatablockList("ImageMap", "Image Map", "ImageAsset", "The Image to Display on This Sprite");
   %frameContainer = %rollout.createHideableStack(%base @ ".object.getImageMap().getFrameCount() < 2;");
   %frameContainer.addControlDependency(%imageMapList);
   %frameContainer.createLeftRightEdit("Frame", "0;", %base @ ".object.getImageMap().getFrameCount() - 1;", 1, "Frame", "The Image Map Frame To Display");
   
   // Return Ref to Base.
   return %base;

}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBQEStaticSprite::SaveContent( %contentCtrl )
{
   // Nothing.
}
