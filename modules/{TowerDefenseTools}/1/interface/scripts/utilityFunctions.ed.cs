//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// Returns the grid's start point world coordinates in the form of 'x y'.  Or
// returns an empty string if no start point is found.
function getGridStartPoint(%grid)
{
   %cell = %grid.findFirstCellWithInteger(2);
   if (%cell $= "")
      return "";
   
   %x = getWord(%cell, 0);
   %y = getWord(%cell, 1);
   return %grid.getCellWorldPosition(%x, %y);
}

// Returns the grid's end point world coordinates in the form of 'x y'.  Or
// returns an empty string if no end point is found.
function getGridEndPoint(%grid)
{
   %cell = %grid.findFirstCellWithInteger(4);
   if (%cell $= "")
      return "";
   
   %x = getWord(%cell, 0);
   %y = getWord(%cell, 1);
   return %grid.getCellWorldPosition(%x, %y);
}
