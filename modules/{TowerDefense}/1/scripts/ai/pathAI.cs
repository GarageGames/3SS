//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function EnemyPathGrid01::onLevelLoaded(%this, %scene)
{
   // Force a rebuild of the node graph based on the weights assigned above.   
   %this.buildNodeGraph(true);
}
