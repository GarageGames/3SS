//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
GuiFormManager::AddFormContent( "LevelBuilderQuickEditClasses", "t2dAnimatedSprite", "LBQEAnimatedSprite::CreateContent", "LBQEAnimatedSprite::SaveContent", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBQEAnimatedSprite::CreateContent( %contentCtrl, %quickEditObj )
{
   %base = %contentCtrl.createBaseStack("LBQEAnimatedSpriteClass", %quickEditObj);
   %rollout = %base.createRolloutStack("Animated Sprite", true);
   %rollout.createT2DDatablockList("Animation", "Animation", "AnimationAsset", "The Animation to Play");
   
   // Return Ref to Base.
   return %base;

}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBQEAnimatedSprite::SaveContent( %contentCtrl )
{
   // Nothing.
}
