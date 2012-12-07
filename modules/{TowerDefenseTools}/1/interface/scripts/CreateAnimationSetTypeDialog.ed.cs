//--------------------------------
// Create Animation Set Type Dialog
//--------------------------------

/// <summary>
/// This function sets the selected type to the value of the type for which
/// the dialog was requested.
/// </summary>
function CreateAnimationSetTypeDialog::onDialogPush(%this)
{
   switch$ (%this.Type)
   {
      case "Enemy":
         CreateAnimationSetEnemyRadio.setStateOn(true);
      case "Tower":
         CreateAnimationSetTowerRadio.setStateOn(true);
      case "Projectile":
         CreateAnimationSetProjectileRadio.setStateOn(true);
   }
}


//--------------------------------
// Create Animation Set
//--------------------------------

/// <summary>
/// This function sets a blank animation set name on the appropriate animation set tool, 
/// then open the tool.
/// </summary>
function CreateAnimationSetCreateButton::onClick(%this)
{
   if (CreateAnimationSetEnemyRadio.getValue())
   {
      EnemyAnimTool.animationSet = "";
      Canvas.pushDialog(EnemyAnimTool);
      Canvas.popDialog(CreateAnimationSetTypeDialog);
   }
   else if (CreateAnimationSetTowerRadio.getValue())
   {
      TowerAnimTool.animationSet = "";
      Canvas.pushDialog(TowerAnimTool);
      Canvas.popDialog(CreateAnimationSetTypeDialog);
   }
   else if (CreateAnimationSetProjectileRadio.getValue())
   {
      ProjectileAnimTool.animationSet = "";
      Canvas.pushDialog(ProjectileAnimTool);
      Canvas.popDialog(CreateAnimationSetTypeDialog);
   }
}