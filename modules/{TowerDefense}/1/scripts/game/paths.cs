//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// This function is used to find the world position of the start cell in a path grid.
/// </summary>
/// <param name="grid">The name of the path grid.</param>
/// <return>Returns the grid's start point world coordinates in the form of 'x y'.  Or
/// returns an empty string if no start point is found.</return>
function getGridStartPoint(%grid)
{
   %cell = %grid.findFirstCellWithInteger(2);
   if (%cell $= "")
      return "";
   
   %x = getWord(%cell, 0);
   %y = getWord(%cell, 1);
   return %grid.getCellWorldPosition(%x, %y);
}

/// <summary>
/// This function is used to find the world position of the end cell in a path grid.
/// </summary>
/// <param name="grid">The name of the path grid.</param>
/// <return>Returns the grid's end point world coordinates in the form of 'x y'.  Or
/// returns an empty string if no end point is found.</return>
function getGridEndPoint(%grid)
{
   %cell = %grid.findFirstCellWithInteger(4);
   if (%cell $= "")
      return "";
   
   %x = getWord(%cell, 0);
   %y = getWord(%cell, 1);
   return %grid.getCellWorldPosition(%x, %y);
}