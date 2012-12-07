//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod(Grid, getCellCount, const char*, 2, 2, 
   "Gives the grid's cell count.\n"
   "@return The cell count as 'x y'." )
{
   U32 x = object->getCellCountX();
   U32 y = object->getCellCountY();

   char* pBuffer = Con::getReturnBuffer(256);
   dSprintf(pBuffer, 256, "%d %d", x, y);

    return pBuffer;
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, getCellCountX, S32, 2, 2, 
   "Gives the grid's cell count in the x direction.\n" )
{
    return object->getCellCountX();
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, getCellCountY, S32, 2, 2, 
   "Gives the grid's cell count in the y direction.\n" )
{
    return object->getCellCountY();
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, setCellCount, void, 3, 3, 
   "(Point2I size) - Set the grid's cell count in each direction.\n" )
{
   S32 x=0, y=0;
   dSscanf(argv[2], "%d %d", &x, &y);
   object->setCellCount(x, y);
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, getCellWorldPosition, const char*, 4, 4, 
   "(U32 x, U32 y) - Obtain the world position of the given cell.\n"
   "@param x The x index of the cell\n"
   "@param y The y index of the cell\n"
   "@return The cell's world position in the form of 'x y'.\n")
{
   S32 x = dAtoi(argv[2]);
   S32 y = dAtoi(argv[3]);
   Vector2 pos;
   object->getCellWorldPosition(x, y, pos);

   char* pBuffer = Con::getReturnBuffer(256);
   dSprintf(pBuffer, 256, "%g %g", pos.x, pos.y);

    return pBuffer;
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, getCellFromWorldPosition, const char*, 3, 3, 
   "(Point2F position) - Gives the cell at the given world position.\n" )
{
   F32 wx=0, wy=0;
   dSscanf(argv[2], "%g %g", &wx, &wy);

   S32 x, y;
   Vector2 v(wx,wy);
   object->getCellFromWorldPosition(v, x, y);

   char* pBuffer = Con::getReturnBuffer(256);
   dSprintf(pBuffer, 256, "%d %d", x, y);

    return pBuffer;
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, getCellInteger, S32, 4, 4, 
   "(U32 x, U32 y) - Provides the cell's integer value.\n"
   "@param x The x index of the cell\n"
   "@param y The y index of the cell\n" )
{
   return object->getCellInteger(dAtoi(argv[2]), dAtoi(argv[3]));
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, setCellInteger, void, 5, 5, 
   "(U32 x, U32 y, S32 value) - Set the given cell's integer value.\n"
   "@param x The x index of the cell\n"
   "@param y The y index of the cell\n"
   "@param value The integer value of the cell\n" )
{
   object->setCellInteger(dAtoi(argv[2]), dAtoi(argv[3]), dAtoi(argv[4]));
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, gridHasCellInteger, bool, 3, 3, 
   "(S32 value) - Does at least one cell have the requested value.\n"
   "@param value The integer value to look up\n"
   "@return True if the given integer value exists on at least one cell\n" )
{
   return object->gridHasCellInteger(dAtoi(argv[2]));
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, findFirstCellWithInteger, const char*, 3, 3, 
   "(S32 value) - Find the first cell that contains the requested integer value.\n"
   "@param value The integer value to search for.\n"
   "@return The cell index for the found cell, or an empty string if none is found.\n"
   "@note: A single search list is used per grid and calling this method will "
   "reset the search list.\n")
{
   Point2I* pnt;
   bool result = object->findFirstCellWithInteger(dAtoi(argv[2]), &pnt);
   if (result)
   {
      char* pBuffer = Con::getReturnBuffer(256);
      dSprintf(pBuffer, 256, "%d %d", pnt->x, pnt->y);
      return pBuffer;
   }
   else
   {
      return "";
   }
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, findNextCellWithInteger, const char*, 2, 2, 
   "Find the next cell that contains the integer value requested using findFirstCellWithInteger().\n"
   "@return The cell index for the found cell, or an empty string if none is found.\n")
{
   Point2I** pnt = NULL;
   bool result = object->findNextCellWithInteger(pnt);
   if (result)
   {
      char* pBuffer = Con::getReturnBuffer(256);
      dSprintf(pBuffer, 256, "%d %d", (*pnt)->x, (*pnt)->y);
      return pBuffer;
   }
   else
   {
      return "";
   }
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, setCellWeight, void, 5, 6, 
   "(U32 x, U32 y, F32 weight, bool rebuildGraph=false) - Set the given cell's weight.\n"
   "@param x The x index of the cell\n"
   "@param y The y index of the cell\n"
   "@param weight The weight of the cell\n"
   "@param rebuildGraph If true then immediately rebuild the node graph [optional]\n"
   "@note Setting rebuildGraph to true still respects the mBuildGraph setting.  If you "
   "want to force a node graph rebuild regardless of mBuildGraph, use buildNodeGraph(true)." )
{
   if (argc == 5)
   {
      object->setCellWeight(dAtoi(argv[2]), dAtoi(argv[3]), dAtof(argv[4]));
   }
   else
   {
      object->setCellWeight(dAtoi(argv[2]), dAtoi(argv[3]), dAtof(argv[4]), dAtob(argv[5]));
   }
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, getCellWeight, F32, 4, 4, 
   "(U32 x, U32 y) - Gives the weight of the requested cell.\n"
   "@param x The x index of the cell\n"
   "@param y The y index of the cell\n" )
{
   return object->getCellWeight(dAtoi(argv[2]), dAtoi(argv[3]));
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, isCellBlocked, bool, 4, 4, 
   "(U32 x, U32 y) - Indicates if the requested cell is blocked.  A blocked cell has a weight of zero.\n" )
{
   return object->isCellBlocked(dAtoi(argv[2]), dAtoi(argv[3]));
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, isWorldPositionBlocked, bool, 3, 3, 
   "(Point2F position) - Indicates if the given world position within a blocked cell.  A blocked cell has a weight of zero.\n" )
{
   F32 x=0, y=0;
   dSscanf(argv[2], "%g %g", &x, &y);
   Vector2 v(x,y);
   return object->isWorldPositionBlocked(v);
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, isAlignedBoxBlocked, bool, 4, 4, 
   "(Point2F start, Point2F end) - Indicates if the given world position box overlaps any blocked cells.  A blocked cell has a weight of zero.\n"
   "@param start The world position of the top left corner of the box to test.\n"
   "@param end The world position of the bottom right corner of the box to test.\n"
   "@return True if the box overlaps at least one cell that is blocked.\n"
   "@note The given box is assumed to already be aligned with the grid.")
{
   F32 sx=0, sy=0;
   dSscanf(argv[2], "%g %g", &sx, &sy);

   F32 ex=0, ey=0;
   dSscanf(argv[3], "%g %g", &ex, &ey);

   Vector2 s(sx,sy);
   Vector2 e(ex,ey);
   return object->isAlignedBoxBlocked(s, e);
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, clearCellIntegers, void, 2, 2, 
   "Clear all cells to the default integer value.\n" )
{
    object->clearCellIntegers();
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, clearCellsToInteger, void, 3, 3, 
   "(S32 value) - Clear all cells to the given integer value.\n" )
{
    object->clearCellsToInteger(dAtoi(argv[2]));
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, setCellsFromIntegerToValue, void, 4, 4, 
   "(S32 fromValue, S32 toValue) - Finds all cells that have an integer value of 'fromValue' and converts them to an integer value of 'toValue'.\n"
   "@param fromValue The integer value to modify\n"
   "@param toValue The integer value to set the cells to\n")
{
    object->setCellsFromIntegerToValue(dAtoi(argv[2]), dAtoi(argv[3]));
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, copyCellIntegersToGrid, bool, 3, 3, 
   "(Grid* copyTo) - Copy all cell integers from this grid to the given grid.\n"
   "@param copyTo The grid to copy values to\n"
   "@return True if the copy was successful\n"
   "@note: Will change the number of cells in copyTo to make this grid" )
{
   Grid* grid = dynamic_cast<Grid*>(Sim::findObject( argv[2] ));

   if (!grid)
   {
      Con::errorf("Grid::copyCellIntegersToGrid(): Could not find given grid %s", argv[2]);
      return false;
   }

   return object->copyCellIntegersToGrid(grid);
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, clearCellWeights, void, 2, 2, 
   "Clear all cells to the default weight value.\n" )
{
    object->clearCellWeights();
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, clearCellsToWeight, void, 3, 3, 
   "(F32 weight) - Clear all cells to the given weight value.\n" )
{
    object->clearCellsToWeight(dAtof(argv[2]));
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, copyCellWeightsToGrid, bool, 3, 3, 
   "(Grid* copyTo) - Copy all cell weights from this grid to the given grid.\n"
   "@param copyTo The grid to copy values to\n"
   "@return True if the copy was successful\n"
   "@note: Will change the number of cells in copyTo to make this grid" )
{
   Grid* grid = dynamic_cast<Grid*>(Sim::findObject( argv[2] ));

   if (!grid)
   {
      Con::errorf("Grid::copyCellWeightsToGrid(): Could not find given grid %s", argv[2]);
      return false;
   }

   return object->copyCellWeightsToGrid(grid);
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, buildNodeGraph, void, 2, 3, 
   "(bool force=false) - Build the node graph based on the cell weights.\n"
   "@param force Force the generation of the node graph even if mBuildGraph is false" )
{
   if (argc == 2)
   {
    object->buildNodeGraph();
   }
   else
   {
    object->buildNodeGraph(dAtob(argv[2]));
   }
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, createPath, S32, 5, 5, 
   "(Vector2 worldStart, Vector2 worldEnd, bool optimise) - Create a path with the given world start and end points.\n"
   "@param worldStart The path start location, in world coordinates\n"
   "@param worldEnd The path end location, in world coordinates\n"
   "@param optimise If true then optimise the path's nodes to the minimum required\n"
   "@return The generated path ID, or 0 if no path could be generated.\n"
   "@note The worldStart and worldEnd points must be within the bounds of the grid.\n" )
{
   Vector2 start, end;
   dSscanf(argv[2], "%g %g", &(start.x), &(start.y));
   dSscanf(argv[3], "%g %g", &(end.x), &(end.y));

   Path* path = NULL;

   bool check = object->createPath(start, end, dAtob(argv[4]), &path);

   if (check)
   {
      return path->getId();
   }
   else
   {
      path->unregisterObject();
      return 0;
   }
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, addImage, bool, 5, 6, 
   "(S32 key, const char* datablock, U32 frame) - Add an image datablock to the image dictionary under the given key.\n"
   "@param key The integer key for this image\n"
   "@param datablock The image datablock to add\n"
   "@param frame The frame within the datablock to use\n"
   "@param rotation The rotation of the image in the range of 0-3 [optional]\n"
   "@return True if the image path has been added to the dictionary. False if "
   "the image could not be added (because the imagePath is an empty string).\n"
   "@note Will replace an existing key with this image path." )
{
   U32 rotation = 0;
   if (argc > 5)
      rotation = dAtoi(argv[5]);

    return object->addImage(dAtoi(argv[2]), argv[3], dAtoi(argv[4]), rotation);
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, removeImage, void, 3, 3, 
   "(S32 key) - Remove an image from the image list based on the given key.\n"
   "@param key The integer key of the image to remove\n" )
{
    object->removeImage(dAtoi(argv[2]));
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, removeImageDatablock, bool, 3, 3, 
   "(SimObject datablock) - Remove all image frames that belong to the given datablock.\n"
   "@param datablock The image map datablock to remove\n"
   "@returns true if the datablock was removed, false if it wasn't found.\n")
{
   ImageAsset* db = dynamic_cast<ImageAsset*>(Sim::findObject(argv[2]));

   if (db)
   {
       return object->removeImageDatablock(db);
   }

   return false;
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, findImage, const char*, 5, 5, 
   "(const char* datablock, U32 frame, U32 rotation) - Finds the key of the requested datablock, frame and rotation.\n"
   "@param datablock The image datablock to find\n"
   "@param frame The frame within the datablock to use\n"
   "@param rotation The rotation of the image in the range of 0-3\n"
   "@return The key if the image was found, or an empty string if it was not found.\n" )
{
   S32 key = -1;
   bool result = object->findImage(argv[2], dAtoi(argv[3]), dAtoi(argv[4]), key);

   if (!result)
   {
      return "";
   }
   else
   {
      char* pBuffer = Con::getReturnBuffer(64);
      dSprintf(pBuffer, 64, "%d", key);

      return pBuffer;
   }
}

//-----------------------------------------------------------------------------

ConsoleMethod(Grid, findNextFreeImageKey, S32, 3, 3, 
   "(S32 start) - Finds the next free image key starting at the given value.\n"
   "@param start The number to start searching from\n"
   "@return The next free key value\n" )
{
    return object->findNextFreeImageKey(dAtoi(argv[2]));
}
