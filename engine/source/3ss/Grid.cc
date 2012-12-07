//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "collection/vectorHeap.h"
#include "math/mathUtils.h"
#include "graphics/dgl.h"
#include "2d/sceneobject/Path.h"
#include "2d/core/CoreMath.h"
#include "3ss/Grid.h"

// Script bindings.
#include "Grid_ScriptBinding.h"

//-----------------------------------------------------------------------------

static U32 VertexLookupTable[8] = {
   0, 1, 2, 3, 0, 1, 2, 3
};

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(Grid);

//-----------------------------------------------------------------------------

Grid::Grid()
{
   mImageListSerialize = false;

   mNoCellSerialize = false;

   mCellCountX = 10;
   mCellCountY = 10;

   mDefaultCellWeight = 1.0f;

   mDefaultCellInteger = 0;

   mBuildGraph = false;

   mAllowDiagonalEdges = false;

   mOptimisePathNodeVariance = 0.25f;

   mGraphDirty = true;

   mRenderCellImages = false;

   mRenderBufferDirty = true;

   mRenderGrid = false;

   mRenderGridColor.set(255, 255, 255);

   mRenderGridStep = 1;

   // Use a static body by default.
   mBodyDefinition.type = b2_staticBody;

   // Use fixed rotation by default.
   mBodyDefinition.fixedRotation = true;
}

//-----------------------------------------------------------------------------

Grid::~Grid()
{
   clearEdgeList();
}

//-----------------------------------------------------------------------------

void Grid::initPersistFields()
{
   addField("imageListSerialize", TypeBool, Offset(mImageListSerialize, Grid), &writeImageListSerialize,
      "Indicates that the grid's image list should be serialized when the scene is saved");

   addField("noCellSerialize", TypeBool, Offset(mNoCellSerialize, Grid), &writeNoCellSerialize,
      "Indicates that cells should not be serialized when the scene is saved");

   addProtectedField( "cellCount", TypePoint2I, Offset(mCellCountX, Grid), &setCellCountField, &getCellCountField, 
      "The number of cells in the x and y directions." );

   addField("defaultCellWeight", TypeF32, Offset(mDefaultCellWeight, Grid), &writeDefaultCellWeight,
      "Default weight to set all cells to when the grid is created.");

   addField("defaultCellInteger", TypeS32, Offset(mDefaultCellInteger, Grid), &writeDefaultCellInteger,
      "Default integer value to set all cells to when the grid is created.");

   addGroup("Path Finding");

      addField("buildGraph", TypeBool, Offset(mBuildGraph, Grid), &writeBuildGraph,
         "Should a node graph be automatically built?");

      addField("allowDiagonalEdges", TypeBool, Offset(mAllowDiagonalEdges, Grid), &writeAllowDiagonalEdges,
         "Should diagonal edges be allowed on the node graph?");

      addField("optimisePathNodeVariance", TypeBool, Offset(mOptimisePathNodeVariance, Grid), &writeOptimisePathNodeVariance,
         "When optimising a path how close to a straight line should the nodes be.  Expressed as a fraction of a cell size.");

   endGroup("Path Finding");

   addField("renderCellImages", TypeBool, Offset(mRenderCellImages, Grid), &writeRenderCellImages,
      "Should cell images be rendered?");

   addField("renderGrid", TypeBool, Offset(mRenderGrid, Grid), &writeRenderGrid,
      "Should a grid be rendered?");

   addField("renderGridColor", TypeColorI, Offset(mRenderGridColor, Grid), &writeRenderGridColor,
      "Color for the rendered grid");

   addField("renderGridStep", TypeS32, Offset(mRenderGridStep, Grid), &writeRenderGridStep,
      "Determines the number of steps between drawing a grid line");

   Parent::initPersistFields();
}

//-----------------------------------------------------------------------------

bool Grid::setCellCountField(void* obj, const char* data)
{
   Grid *object = static_cast<Grid *>(obj);

   S32 x=0, y=0;
   dSscanf(data, "%d %d", &x, &y);
   object->setCellCount(x,y);

   // We return false since we don't want the console to mess with the data
   return false;
}

//-----------------------------------------------------------------------------

const char * Grid::getCellCountField(void* obj, const char* data)
{
   Grid *object = static_cast<Grid *>(obj);

   S32 x = object->getCellCountX();
   S32 y = object->getCellCountY();

   char* pBuffer = Con::getReturnBuffer(256);
   dSprintf(pBuffer, 256, "%d %d", x, y);

   return pBuffer;
}

//-----------------------------------------------------------------------------

bool Grid::onAdd()
{
   if(!Parent::onAdd())
      return false;

   // Build out all cells
   buildCells(true);

   // Read our custom cell fields.
   if (!mNoCellSerialize)
   {
      const char *bField = "";
      char buffer[256];
      StringTableEntry string = "";
      for (U32 x=0; x<mCellCountX; ++x)
      {
         for (U32 y=0; y<mCellCountY; ++y)
         {
            U32 index = x + (mCellCountX * y);

            dSprintf(buffer, sizeof(buffer), "_weight%d", index);
            string = StringTable->insert( buffer );
            bField = getDataField( string, NULL );
            if (bField)
            {
               setCellWeight(x, y, dAtof(bField), false);
               setDataField( string, NULL, "" );
            }

            dSprintf(buffer, sizeof(buffer), "_integer%d", index);
            string = StringTable->insert( buffer );
            bField = getDataField( string, NULL );
            if (bField)
            {
               setCellInteger(x, y, dAtoi(bField));
               setDataField( string, NULL, "" );
            }
         }
      }
   }

   // Read our custom image list fields
   if (mImageListSerialize)
   {
      const char *bField = "";

      char keybuffer[32];
      StringTableEntry keystring = "";
      char dbbuffer[256];
      StringTableEntry dbstring = "";
      char framebuffer[32];
      StringTableEntry framestring = "";
      char rotationbuffer[32];
      StringTableEntry rotationstring = "";
      U32 index = 0;

      dSprintf(keybuffer, sizeof(keybuffer), "_imageKey%d", index);
      keystring = StringTable->insert( keybuffer );
      bField = getDataField( keystring, NULL );
      while (bField && bField[0])
      {
         // The key for this image
         S32 key = dAtoi(bField);
         setDataField( keystring, NULL, "" );

         dSprintf(dbbuffer, sizeof(dbbuffer), "_imageDB%d", index);
         dbstring = StringTable->insert( dbbuffer );
         const char* datablock = getDataField( dbstring, NULL );

         dSprintf(framebuffer, sizeof(framebuffer), "_imageFrame%d", index);
         framestring = StringTable->insert( framebuffer );
         const char* frame = getDataField( framestring, NULL );

         dSprintf(rotationbuffer, sizeof(rotationbuffer), "_imageRotation%d", index);
         rotationstring = StringTable->insert( rotationbuffer );
         const char* rotation = getDataField( rotationstring, NULL );

         if (datablock && datablock[0] && frame && frame[0])
         {
            // Check if the datablock is valid.  If not, then don't bother to
            // add the image.
            ImageAsset* db = dynamic_cast<ImageAsset*>(Sim::findObject( datablock ));
            if (db)
            {
               if (rotation && rotation[0])
               {
                  addImage(key, datablock, dAtoi(frame), dAtoi(rotation));
               }
               else
               {
                  // Original serialization format didn't include rotation
                  addImage(key, datablock, dAtoi(frame));
               }
            }
            else
            {
               // This datablock is not valid, so let's remove all cell integers that reference it.
               setCellsFromIntegerToValue(key, 0);
            }
         }

         if (datablock)
            setDataField( dbstring, NULL, "" );

         if (frame)
            setDataField( framestring, NULL, "" );

         if (rotation)
            setDataField( rotationstring, NULL, "" );

         // Get the next index
         ++index;
         dSprintf(keybuffer, sizeof(keybuffer), "_imageKey%d", index);
         keystring = StringTable->insert( keybuffer );
         bField = getDataField( keystring, NULL );
      }
   }

   return true;
}

//-----------------------------------------------------------------------------

void Grid::onDeleteNotify( SimObject* object )
{
   ImageAsset* db = dynamic_cast<ImageAsset*>(object);
   if (!db)
      return;

   removeImageDatablock(db);
}

//-----------------------------------------------------------------------------

void Grid::onTamlPreWrite( void )
{
    // Call parent.
    Parent::onTamlPreWrite();

    // Write our custom image list fields
    if (mImageListSerialize)
    {
        U32 index = 0;
        for (HashTable<S32, StringTableEntry>::iterator itr=mImageNameDictionary.begin(); itr != mImageNameDictionary.end(); ++itr)
        {
            // Only save out images that are still being used by the grid.
            S32 key = itr->key;
            if ( gridHasCellInteger(key) )
            {
                // Image key.
                StringTableEntry pFieldName = StringTable->insert( avar( "_imageKey%d", index ) );
                setDataField( pFieldName, NULL, avar( "%d", key ) );

                // Image datablock.
                pFieldName = StringTable->insert( avar( "_imageDB%d", index ) );
                const char* datablock = itr.getValue();
                setDataField( pFieldName, NULL, avar( "%s", datablock ) );

                // Image frame.
                pFieldName = StringTable->insert( avar( "_imageFrame%d", index ) );
                HashTable<S32, U32>::iterator frameItr = mImageFrameDictionary.find(key);
                const U32 frame = frameItr.getValue();
                setDataField( pFieldName, NULL, avar( "%d", frame ) );

                // Image rotation.
                pFieldName = StringTable->insert( avar( "_imageRotation%d", index ) );
                HashTable<S32, U32>::iterator rotItr = mImageRotationDictionary.find(key);
                const U32 rot = rotItr.getValue();
                setDataField( pFieldName, NULL, avar( "%d", rot ) );

                ++index;
            }
        }
    }

    // Write our custom cell fields.
    if (!mNoCellSerialize)
    {
        for (U32 x=0; x<mCellCountX; ++x)
        {
            for (U32 y=0; y<mCellCountY; ++y)
            {
                U32 index = x + (mCellCountX * y);

                if (mCellWeights[index] != mDefaultCellWeight)
                {
                    // Weight.
                    StringTableEntry pFieldName = StringTable->insert( avar( "_weight%d", index ) );
                    setDataField( pFieldName, NULL, avar( "%g",  mCellWeights[index] ) );
                }

                if (mCellInteger[index] != mDefaultCellInteger)
                {
                    // Weight.
                    StringTableEntry pFieldName = StringTable->insert( avar( "_integer%d", index ) );
                    setDataField( pFieldName, NULL, avar( "%d",  mCellInteger[index] ) );
                }
            }
        }
    }

}

//-----------------------------------------------------------------------------

void Grid::onTamlPostWrite( void )
{
    // Call parent.
    Parent::onTamlPostWrite();
}

//-----------------------------------------------------------------------------

void Grid::onTamlPreRead( void )
{
    // Call parent.
    Parent::onTamlPreRead();
}

//-----------------------------------------------------------------------------

void Grid::onTamlPostRead( const TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlPostRead( customCollection );
}

//-----------------------------------------------------------------------------

void Grid::write(Stream &stream, U32 tabStop, U32 flags)
{
    // Only output selected objects if they want that.
    if((flags & SelectedOnly) && !isSelected())
        return;

    // The work we want to perform here is in the Taml callback.
    onTamlPreWrite();

    // Write object.
    Parent::write( stream, tabStop, flags );

    // The work we want to perform here is in the Taml callback.
    onTamlPostWrite();
}

//-----------------------------------------------------------------------------

void Grid::setCellCount(U32 x, U32 y)
{
   mCellCountX = x;
   mCellCountY = y;

   mGraphDirty = true;

   buildCells(true);
}

//-----------------------------------------------------------------------------

void Grid::setCellCountX(U32 x)
{
   mCellCountX = x;

   mGraphDirty = true;

   buildCells(true);
}

//-----------------------------------------------------------------------------

void Grid::setCellCountY(U32 y)
{
   mCellCountY = y;

   mGraphDirty = true;

   buildCells(true);
}

//-----------------------------------------------------------------------------

void Grid::getCellNormalizedPosition(U32 x, U32 y, Vector2& outPos)
{
   if (!isGridValid())
   {
      outPos.setZero();
      return;
   }

   F32 xShift = 2.0f / mCellCountX;
   F32 yShift = 2.0f / mCellCountY;

   // x goes from -1..1 from left to right
   outPos.x = (x * xShift) + (xShift / 2.0f) - 1.0f;

   // y goes from 1..-1 from top to bottom
   outPos.y = -((y * yShift) + (yShift / 2.0f) - 1.0f);
}

//-----------------------------------------------------------------------------

void Grid::getCellFromNormalizedPosition(Vector2& inPos, S32& x, S32& y)
{
   if (!isGridValid())
   {
      x = 0;
      y = 0;
      return;
   }

   F32 xShift = 2.0f / mCellCountX;
   F32 yShift = 2.0f / mCellCountY;

   // x goes from -1..1 from left to right
   x = (S32)mFloor((inPos.x + 1.0f) / xShift);

   // y goes from 1..-1 from top to bottom
   y = (S32)mFloor(((-inPos.y) + 1.0f) / yShift);
}

//-----------------------------------------------------------------------------

void Grid::getCellWorldPosition(S32& x, S32& y, Vector2& outPos)
{
   if (!isGridValid())
   {
      outPos.setZero();
      return;
   }

   // Normalized -1..1 position
   Vector2 normalized;
   getCellNormalizedPosition(x, y, normalized);

   // Calculate local position based on grid's size
   Vector2 halfSize = getHalfSize();
   
   Vector2 local = normalized * halfSize;

   outPos = getWorldPoint(local);
}

//-----------------------------------------------------------------------------

void Grid::getCellFromWorldPosition(Vector2& inPos, S32& x, S32& y)
{
   if (!isGridValid())
   {
      x = 0;
      y = 0;
      return;
   }

   Vector2 localPosition = getLocalPoint(inPos);

   // Convert from local to normalized position
   Vector2 normalPosition;
   Vector2 halfSize = getHalfSize();
   normalPosition.x = localPosition.x / halfSize.x;
   normalPosition.y = localPosition.y / halfSize.y;

   getCellFromNormalizedPosition(normalPosition, x, y);
}

//-----------------------------------------------------------------------------

S32 Grid::getCellInteger(U32 x, U32 y)
{
   if (!isGridValid())
   {
      return 0;
   }

   if ((S32)x < 0 || x >= mCellCountX ||
       (S32)y < 0 || y >= mCellCountY)
   {
      // Invalid cell reference
      return 0;
   }

   return mCellInteger[x + (mCellCountX * y)];
}

//-----------------------------------------------------------------------------

void Grid::setCellInteger(U32 x, U32 y, S32 value)
{
   if (!isGridValid())
   {
      return;
   }

   if ((S32)x < 0 || x >= mCellCountX ||
       (S32)y < 0 || y >= mCellCountY)
   {
      // Invalid cell reference
      return;
   }

   U32 index = x + (mCellCountX * y);
   mCellInteger[index] = value;

   // Mark the render buffer as dirty in case it is being used
   mRenderBufferDirty = true;
}

//-----------------------------------------------------------------------------

bool Grid::gridHasCellInteger(S32 value)
{
   if (!isGridValid())
   {
      return false;
   }

   for (Vector<S32>::iterator itr=mCellInteger.begin(); itr != mCellInteger.end(); ++itr)
   {
      if ((*itr) == value)
         return true;
   }

   return false;
}

//-----------------------------------------------------------------------------

bool Grid::findFirstCellWithInteger(S32 value, Point2I** cell)
{
   mCellIntegerSearch.clear();
   mNextCellIntegerSearchIndex = 1;

   if (!isGridValid())
   {
      return false;
   }

   for (U32 x=0; x<mCellCountX; ++x)
   {
      for (U32 y=0; y<mCellCountY; ++y)
      {
         if (getCellInteger(x, y) == value)
         {
            Point2I index(x, y);
            mCellIntegerSearch.push_back(index);
         }
      }
   }

   if (mCellIntegerSearch.size() > 0)
   {
      (*cell) = (Point2I*)&(mCellIntegerSearch[0]);
      return true;
   }
   else
   {
      return false;
   }
}

//-----------------------------------------------------------------------------

bool Grid::findNextCellWithInteger(Point2I** cell)
{
   if (mNextCellIntegerSearchIndex >= mCellIntegerSearch.size())
      return false;

   (*cell) = &(mCellIntegerSearch[mNextCellIntegerSearchIndex]);
   ++mNextCellIntegerSearchIndex;

   return true;
}

//-----------------------------------------------------------------------------

F32 Grid::getCellWeight(U32 x, U32 y)
{
   if (!isGridValid())
   {
      return 0.0f;
   }

   if ((S32)x < 0 || x >= mCellCountX ||
       (S32)y < 0 || y >= mCellCountY)
   {
      // Invalid cell reference
      return 0.0f;
   }

   return mCellWeights[x + (mCellCountX * y)];
}

//-----------------------------------------------------------------------------

void Grid::setCellWeight(U32 x, U32 y, F32 weight, bool rebuildGraph)
{
   if (!isGridValid())
   {
      return;
   }

   if ((S32)x < 0 || x >= mCellCountX ||
       (S32)y < 0 || y >= mCellCountY)
   {
      // Invalid cell reference
      return;
   }

   U32 index = x + (mCellCountX * y);
   mCellWeights[index] = weight;
   mCellCostDirty[index] = true;

   mGraphDirty = true;

   if (rebuildGraph)
   {
      buildNodeGraph();
   }
}

//-----------------------------------------------------------------------------

bool Grid::isCellBlocked(U32 x, U32 y)
{
   if (!isGridValid())
   {
      return true;
   }

   if ((S32)x < 0 || x >= mCellCountX ||
       (S32)y < 0 || y >= mCellCountY)
   {
      // Invalid cell reference
      return true;
   }

   return mIsZero(mCellWeights[x + (mCellCountX * y)]);
}

//-----------------------------------------------------------------------------

bool Grid::isWorldPositionBlocked(Vector2 worldPosition)
{
   if (!isGridValid())
   {
      return true;
   }

   Vector2 localPosition = getLocalPoint(worldPosition);

   // Convert to normalized position
   Vector2 normalizedPosition;
   Vector2 halfSize = getHalfSize();
   normalizedPosition.x = localPosition.x / halfSize.x;
   normalizedPosition.y = localPosition.y / halfSize.y;

   S32 x,y;
   getCellFromNormalizedPosition(normalizedPosition, x, y);

   return isCellBlocked(x, y);
}

//-----------------------------------------------------------------------------

bool Grid::isAlignedBoxBlocked(Vector2 boxWorldStart, Vector2 boxWorldEnd)
{
   if (!isGridValid())
   {
      return true;
   }

   Vector2 localStart = getLocalPoint(boxWorldStart);
   Vector2 localEnd = getLocalPoint(boxWorldEnd);

   // Make sure we have a properly oriented box
   if (localStart.x > localEnd.x)
   {
      F32 temp = localStart.x;
      localStart.x = localEnd.x;
      localEnd.x = temp;
   }
   if (localStart.y > localEnd.y)
   {
      F32 temp = localStart.y;
      localStart.y = localEnd.y;
      localEnd.y = temp;
   }

   Vector2 normalizedPosition;
   Vector2 halfSize = getHalfSize();

   // Get the cells that we start and end in
   S32 sx,sy;
   normalizedPosition.x = localStart.x / halfSize.x;
   normalizedPosition.y = localStart.y / halfSize.y;
   getCellFromNormalizedPosition(normalizedPosition, sx, sy);
   S32 ex,ey;
   normalizedPosition.x = localEnd.x / halfSize.x;
   normalizedPosition.y = localEnd.y / halfSize.y;
   getCellFromNormalizedPosition(normalizedPosition, ex, ey);

   // Now go through each cell and check if it is a blocker
   for (S32 i=sx; i<=ex; ++i)
   {
      for (S32 j=sy; j<=ey; ++j)
      {
         S32 cell = i + (mCellCountX * j);
         if (cell < 0 || cell >= mCellWeights.size() || mIsZero(mCellWeights[cell]))
            return true;
      }
   }

   return false;
}

//-----------------------------------------------------------------------------

void Grid::buildCells(bool clearCellList)
{
   if (clearCellList || !mCellWeights.size())
   {
      // Rebuild the entire cell list from scratch
      mCellWeights.clear();
      mCellNormalizedPositions.clear();
      mCellCostDirty.clear();
      mCellInteger.clear();
      clearEdgeList();

      if (!isGridValid())
      {
         return;
      }

      // Populate the grid with the default weights
      // NOTE: Need to count y then x here as the rest of the
      // code calculates the index based on row, then column
      Vector2 localPos;
      for (U32 y=0; y<mCellCountY; ++y)
      {
         for (U32 x=0; x<mCellCountX; ++x)
         {
            mCellWeights.push_back(mDefaultCellWeight);

            getCellNormalizedPosition(x, y, localPos);
            mCellNormalizedPositions.push_back(localPos);

            mCellCostDirty.push_back(true);

            // Start with no edges
            mEdgeList.push_back(NULL);

            mCellInteger.push_back(mDefaultCellInteger);
         }
      }
   }
   else
   {
      if (!isGridValid())
      {
         return;
      }

      // Only need to set the weights
      for (U32 x=0; x<mCellCountX; ++x)
      {
         for (U32 y=0; y<mCellCountY; ++y)
         {
            U32 index = x + (mCellCountX * y);
            mCellWeights[index] = mDefaultCellWeight;
            mCellCostDirty[index] = true;
         }
      }
   }

   mGraphDirty = true;

   buildNodeGraph();
}

//-----------------------------------------------------------------------------

void Grid::clearEdgeList()
{
   for (Vector<GraphEdge*>::iterator itr = mEdgeList.begin(); itr != mEdgeList.end(); ++itr)
   {
      GraphEdge* edge = (*itr);
      while (edge)
      {
         GraphEdge* nextEdge = edge->getNextEdge();
         delete edge;
         edge = nextEdge;
      }
   }

   mEdgeList.clear();

   mGraphDirty = true;
}

//-----------------------------------------------------------------------------

void Grid::clearEdges(U32 cellIndex)
{
   GraphEdge* edge = mEdgeList[cellIndex];
   while (edge)
   {
      GraphEdge* nextEdge = edge->getNextEdge();
      delete edge;
      edge = nextEdge;
   }

   mEdgeList[cellIndex] = NULL;

   mCellCostDirty[cellIndex] = true;
   mGraphDirty = true;
}

//-----------------------------------------------------------------------------

void Grid::clearEdges(U32 x, U32 y)
{
   U32 cellIndex = x + (mCellCountX * y);
   clearEdges(cellIndex);
}

//-----------------------------------------------------------------------------

void Grid::clearCellIntegers()
{
   if (!isGridValid())
   {
      return;
   }

   for (Vector<S32>::iterator itr=mCellInteger.begin(); itr != mCellInteger.end(); ++itr)
   {
      (*itr) = mDefaultCellInteger;
   }

   // Mark the render buffer as dirty in case it is being used
   mRenderBufferDirty = true;
}

//-----------------------------------------------------------------------------

void Grid::clearCellsToInteger(S32 value)
{
   if (!isGridValid())
   {
      return;
   }

   for (Vector<S32>::iterator itr=mCellInteger.begin(); itr != mCellInteger.end(); ++itr)
   {
      (*itr) = value;
   }

   // Mark the render buffer as dirty in case it is being used
   mRenderBufferDirty = true;
}

//-----------------------------------------------------------------------------

void Grid::setCellsFromIntegerToValue(S32 fromValue, S32 toValue)
{
   if (!isGridValid())
   {
      return;
   }

   for (Vector<S32>::iterator itr=mCellInteger.begin(); itr != mCellInteger.end(); ++itr)
   {
      if ((*itr) == fromValue)
      {
         (*itr) = toValue;
      }
   }

   // Mark the render buffer as dirty in case it is being used
   mRenderBufferDirty = true;
}

//-----------------------------------------------------------------------------

bool Grid::copyCellIntegersToGrid(Grid* copyTo)
{
   if (!copyTo)
      return false;

   // Make sure the grid to copy to has the correct number of cells.
   if (getCellCountX() != copyTo->getCellCountX() || getCellCountY() != copyTo->getCellCountY())
      copyTo->setCellCount(getCellCountX(), getCellCountY());

   // Now copy the cells over
   for (U32 x=0; x<mCellCountX; ++x)
   {
      for (U32 y=0; y<mCellCountY; ++y)
      {
         copyTo->setCellInteger(x, y, getCellInteger(x, y));
      }
   }

   return true;
}

//-----------------------------------------------------------------------------

void Grid::clearCellWeights()
{
   // Clear all cells to the default weight value
   mGraphDirty = true;
   buildCells();
}

//-----------------------------------------------------------------------------

void Grid::clearCellsToWeight(F32 weight)
{
   if (!isGridValid())
   {
      return;
   }

   // Populate the grid with the given weight
   for (U32 x=0; x<mCellCountX; ++x)
   {
      for (U32 y=0; y<mCellCountY; ++y)
      {
         U32 index = x + (mCellCountX * y);
         mCellWeights[index] = weight;
         mCellCostDirty[index] = true;
      }
   }

   mGraphDirty = true;

   buildNodeGraph();
}

//-----------------------------------------------------------------------------

bool Grid::copyCellWeightsToGrid(Grid* copyTo)
{
   if (!copyTo)
      return false;

   // Make sure the grid to copy to has the correct number of cells.
   if (getCellCountX() != copyTo->getCellCountX() || getCellCountY() != copyTo->getCellCountY())
      copyTo->setCellCount(getCellCountX(), getCellCountY());

   // Now copy the cells over
   for (U32 x=0; x<mCellCountX; ++x)
   {
      for (U32 y=0; y<mCellCountY; ++y)
      {
         copyTo->setCellWeight(x, y, getCellWeight(x, y));
      }
   }

   return true;
}

//-----------------------------------------------------------------------------

void Grid::buildNodeGraph(bool force)
{
   if (!force && !mBuildGraph)
      return;

   for (U32 x=0; x<mCellCountX; ++x)
   {
      for (U32 y=0; y<mCellCountY; ++y)
      {
         U32 index = x + (mCellCountX * y);
         if (mCellCostDirty[index])
         {
            updateGraphEdgesForCell(x, y, mAllowDiagonalEdges, true);
            mCellCostDirty[index] = false;
         }
      }
   }

   mGraphDirty = false;
}

//-----------------------------------------------------------------------------

void Grid::updateGraphEdgesForCell(U32 x, U32 y, bool allowDiagonal, bool calculateCost, bool forceCalculation)
{
   U32 cellIndex = x + (mCellCountX * y);

   // Special case: If the cell's weight is zero and we're to calculate weighted cost
   // then delete all edges to and from this cell.  No other cell can reach it.
   if (calculateCost && mIsZero(mCellWeights[cellIndex]))
   {
      // Start with the given cell
      clearEdges(cellIndex);

      // Move on to the surrounding cells
      for (S32 i=-1; i<2; ++i)
      {
         for(S32 j=-1; j<2; ++j)
         {
            if (!isCellValid(x, i, y, j, allowDiagonal))
               continue;

            S32 cellX = x + i;
            S32 cellY = y + j;
            U32 toIndex = cellX + (mCellCountX * cellY);

            GraphEdge* edge = mEdgeList[toIndex];
            while (edge)
            {
               if (edge->getTo() == (S32)cellIndex)
               {
                  // This edge points to our cell.  Delete this edge.
                  GraphEdge* prev = edge->getPrevEdge();
                  GraphEdge* next = edge->getNextEdge();
                  if (prev)
                  {
                     prev->setNextEdge(next);
                     if (next)
                        next->setPrevEdge(prev);
                  }
                  else
                  {
                     mEdgeList[toIndex] = next;
                     if (next)
                        next->setPrevEdge(NULL);
                  }

                  delete edge;

                  break;
               }

               edge = edge->getNextEdge();
            }
         }
      }

      return;
   }

   Vector2& cellPos = mCellNormalizedPositions[cellIndex];

   // If we're to calculate cost then we need to update all edges that point to us.
   // While we're at it, we also need to update all edges that go from us
   // to neighbouring cells.
   if (calculateCost || forceCalculation)
   {
      for (S32 i=-1; i<2; ++i)
      {
         for(S32 j=-1; j<2; ++j)
         {
            if (!isCellValid(x, i, y, j, allowDiagonal))
               continue;

            S32 cellX = x + i;
            S32 cellY = y + j;
            U32 fromIndex = cellX + (mCellCountX * cellY);

            GraphEdge* edge = mEdgeList[fromIndex];
            GraphEdge* lastEdge = edge;
            while (edge)
            {
               if (edge->getTo() == (S32)cellIndex)
               {
                  break;
               }

               lastEdge = edge;
               edge = edge->getNextEdge();
            }

            if (!edge)
            {
               // We didn't find an edge that points to us, so create one.
               GraphEdge* newEdge = new GraphEdge();
               newEdge->setFrom(fromIndex);
               newEdge->setTo(cellIndex);

               if (lastEdge)
               {
                  // Add to the end of the cell's edge list
                  lastEdge->setNextEdge(newEdge);
                  newEdge->setPrevEdge(lastEdge);
               }
               else
               {
                  // This is the first edge for the cell
                  mEdgeList[fromIndex] = newEdge;
               }

               // Pass along to the edge cost calculation
               edge = newEdge;
            }

            // Now calculate the edge cost
            Vector2& fromPos = mCellNormalizedPositions[fromIndex];
            Vector2 v = cellPos - fromPos;
            F32 cost = v.Length();

            if (calculateCost)
            {
               // Calculate the cost to travel to our cell
               cost = mCellWeights[cellIndex] * cost;
            }

            edge->setCost(cost);

            // Now reverse our calculation to go from us to the other
            // cell.  When we're to calculate weighted costs, only
            // create or update this edge if the other cell doesn't
            // have a weighted cost of zero.
            if (!calculateCost || !mIsZero(mCellWeights[fromIndex]))
            {
               // Find the edge that goes from us to the neighbouring cell
               edge = mEdgeList[cellIndex];
               lastEdge = edge;
               while (edge)
               {
                  if (edge->getTo() == (S32)fromIndex)
                  {
                     break;
                  }

                  lastEdge = edge;
                  edge = edge->getNextEdge();
               }

               if (!edge)
               {
                  // We didn't find an edge that from us to the neighbour, so create one.
                  GraphEdge* newEdge = new GraphEdge();
                  newEdge->setFrom(cellIndex);
                  newEdge->setTo(fromIndex);

                  if (lastEdge)
                  {
                     // Add to the end of the cell's edge list
                     lastEdge->setNextEdge(newEdge);
                     newEdge->setPrevEdge(lastEdge);
                  }
                  else
                  {
                     // This is the first edge for our cell
                     mEdgeList[cellIndex] = newEdge;
                  }

                  // Pass along to the edge cost calculation
                  edge = newEdge;
               }

               // Now calculate the edge cost
               fromPos = mCellNormalizedPositions[fromIndex];
               v = cellPos - fromPos;
               cost = v.Length();

               if (calculateCost)
               {
                  // Calculate the cost to travel to our neighbour cell
                  cost = mCellWeights[fromIndex] * cost;
               }

               edge->setCost(cost);
            }
         }
      }
   }
}

//-----------------------------------------------------------------------------

bool Grid::createPath(Vector2 worldStart, Vector2 worldEnd, bool optimise, Path** path)
{
   *path = new Path();
   (*path)->registerObject();

   // Convert the start and end point to local coordinates
   Vector2 size = getSize();
   Vector2 localStart = getLocalPoint(worldStart);
   Vector2 localEnd = getLocalPoint(worldEnd);

   // Now calculate normalized coordinates in the range of -1...1
   Vector2 normalizedPosition;
   Vector2 halfSize = getHalfSize();

   S32 sx,sy;
   normalizedPosition.x = localStart.x / halfSize.x;
   normalizedPosition.y = localStart.y / halfSize.y;
   getCellFromNormalizedPosition(normalizedPosition, sx, sy);

   S32 ex,ey;
   normalizedPosition.x = localEnd.x / halfSize.x;
   normalizedPosition.y = localEnd.y / halfSize.y;
   getCellFromNormalizedPosition(normalizedPosition, ex, ey);

   // Make sure the cell indices are within range
   if (sx < 0 || sx >= (S32)mCellCountX || sy < 0 || sy >= (S32)mCellCountY ||
       ex < 0 || ex >= (S32)mCellCountX || ey < 0 || ey >= (S32)mCellCountY)
   {
      return false;
   }

   // Is the node graph dirty?
   if (mGraphDirty)
   {
      buildNodeGraph(true);
   }

   Vector<U32> pathCells;
   getCellListToTarget(sx, sy, ex, ey, pathCells);
   if (!pathCells.size())
   {
      // No path
      return false;
   }

   // Optimise the path?
   if (optimise && pathCells.size() >= 3)
   {
      // This distance is in local space
      F32 avgSize = ((1.0f/mCellCountX) + (1.0f/mCellCountY)) / 2.0f;
      F32 maxDistance = avgSize * mOptimisePathNodeVariance;
      F32 maxDistanceSq = maxDistance * maxDistance;

      U32 a = pathCells[0];
      Vector2 aPos = mCellNormalizedPositions[a];

      U32 b = pathCells[1];
      Vector2 bPos = mCellNormalizedPositions[b];
      Vector<U32>::iterator bPtr = &pathCells[1];

      for (Vector<U32>::iterator itr = pathCells.begin()+2; itr != pathCells.end(); )
      {
         U32 c = (*itr);
         Vector2 cPos = mCellNormalizedPositions[c];

         // Calculate the distance from c to line ab, in local space
         Vector2 p = CoreMath::mGetClosestPointOnLine(aPos, bPos, cPos);
         F32 distanceSQ = (cPos - p).LengthSquared();

         // If node c is within our tolerances then we have straight line abc
         if (distanceSQ <= maxDistanceSq)
         {
            // Safe to remove node b
            pathCells.erase(bPtr);

            // c becomes the new b.  Careful with pointers as we've just done an erase()!
            // This means bPtr stays the same as node c has just slid into its position.
            b = c;
            bPos = mCellNormalizedPositions[b];
         }
         else
         {
            // Node c is not within our tolerances so we cannot remove node b.  Move along.
            a = b;
            aPos = mCellNormalizedPositions[b];

            b = c;
            bPos = mCellNormalizedPositions[c];
            bPtr = itr;

            ++itr;
         }
      }
   }

   // Build the path
   (*path)->setPosition(getPosition());
   (*path)->setAngle(getAngle());
   for (Vector<U32>::iterator itr = pathCells.begin(); itr != pathCells.end(); ++itr)
   {
      Vector2& pos = mCellNormalizedPositions[(*itr)];
      normalizedPosition = pos * halfSize;
      Vector2 wp = getWorldPoint(normalizedPosition);
      (*path)->addNode(wp, 0.0f, 1.0f);
   }

   return true;
}

//-----------------------------------------------------------------------------

static Grid::SearchAStarData* sCurrentSearchData = NULL;
static S32 fCostCompare(U32 left, U32 right)
{
   F32 lF = sCurrentSearchData->mFCosts[left];
   F32 rF = sCurrentSearchData->mFCosts[right];

   if (lF < rF)
      return 1;
   if (lF < rF)
      return -1;
   return 0;
}

//-----------------------------------------------------------------------------

void Grid::getCellListToTarget(U32 sx, U32 sy, U32 ex, U32 ey, Vector<U32>& pathCells)
{
   U32 nodeCount = mCellWeights.size();
   
   SearchAStarData data(nodeCount);

   sCurrentSearchData = &data;

   Heap<U32> priorityQueue(nodeCount, fCostCompare);

   U32 startIndex = sx + mCellCountX * sy;
   U32 endIndex = ex + mCellCountX * ey;

   // Place the source node on the queue
   priorityQueue.enqueue(startIndex);

   // Process while the queue is not empty
   while (priorityQueue.size())
   {
      // Get the lowest cost cell from the queue
      U32 nextClosestNode = priorityQueue.item();
      priorityQueue.dequeue();

      // Move this node from the frontier
      data.mShortestPath[nextClosestNode] = data.mSearchFrontier[nextClosestNode];

      // Have we reached the end?
      if (nextClosestNode == endIndex)
      {
         // Build the path
         U32 index = endIndex;
         pathCells.push_front(index);
         while ((index != startIndex) && (data.mShortestPath[index] != 0))
         {
            index = data.mShortestPath[index]->getFrom();
            pathCells.push_front(index);
         }
         return;
      }

      // Work with all edges that leave this cell
      GraphEdge* edge = mEdgeList[nextClosestNode];
      while (edge)
      {
         U32 to = edge->getTo();

         // Calculate the heuristic from this node to the target
         F32 costH = heuristicEuclid(endIndex, to);

         // Calculate the real cost to this node from the source
         F32 costG = data.mGCosts[nextClosestNode] + edge->getCost();

         // If the node has not been added to the frontier then add it and
         // update its G and F costs.
         if (!data.mSearchFrontier[to])
         {
            data.mFCosts[to] = costG + costH;
            data.mGCosts[to] = costG;

            priorityQueue.enqueue(to);

            data.mSearchFrontier[to] = edge;
         }
         else if ((costG < data.mGCosts[to]) && !data.mShortestPath[to])
         {
            // The node is already on the frontier but the current cost to
            // get here is cheaper than what is currently on the frontier.
            // Update costs and the frontier.
            data.mFCosts[to] = costG + costH;
            data.mGCosts[to] = costG;

            for (Heap<U32>::iterator itr = priorityQueue.begin(); itr != priorityQueue.end(); ++itr)
            {
               if ((*itr) == to)
               {
                  priorityQueue.erase(itr);
                  break;
               }
            }
            priorityQueue.enqueue(to);

            data.mSearchFrontier[to] = edge;
         }

         // Move on to the next edge that leaves this cell
         edge = edge->getNextEdge();
      }
   }
}

//-----------------------------------------------------------------------------

bool Grid::addImage(S32 key, const char* datablock, U32 frame, U32 rotation)
{
   if (!datablock || !datablock[0])
   {
      Con::warnf("Grid::addImage(): No datablock name given");
      return false;
   }

   StringTableEntry name = StringTable->insert(datablock);

   // Find the image datablock
   ImageAsset* db = dynamic_cast<ImageAsset*>(Sim::findObject( datablock ));

   // Make sure the frame is valid
   if ( frame >= db->getFrameCount() )
   {
      Con::warnf("Grid::addImage(): Invalid frame %d for datablock %s", frame, datablock);
      return false;
   }

   // First erase any existing key and then insert this key/value pair
   mImageNameDictionary.erase(key);
   mImageDataBlockDictionary.erase(key);
   mImageFrameDictionary.erase(key);
   mImageRotationDictionary.erase(key);
   mImageNameDictionary.insertUnique(key, name);
   mImageDataBlockDictionary.insertUnique(key, db);
   mImageFrameDictionary.insertUnique(key, frame);
   mImageRotationDictionary.insertUnique(key, rotation);

   // We want to know if this image datablock is ever deleted out from under us.
   deleteNotify(db);

   return true;
}

//-----------------------------------------------------------------------------

void Grid::removeImage(S32 key)
{
   mImageNameDictionary.erase(key);
   mImageDataBlockDictionary.erase(key);
   mImageFrameDictionary.erase(key);
   mImageRotationDictionary.erase(key);
}

//-----------------------------------------------------------------------------

bool Grid::removeImageDatablock(ImageAsset* datablock)
{

   // Vector to hold list of keys to delete
   Vector<S32> keys;

   // Go through the image datablock dictionary and find all keys that
   // match the deleting datablock.  This datablock may be referenced by
   // multiple images (same datablock, different frame, etc.) so we
   // have to find them all.
   for (HashTable<S32, ImageAsset*>::iterator imageItr=mImageDataBlockDictionary.begin(); imageItr != mImageDataBlockDictionary.end(); ++imageItr)
   {
      if (imageItr.getValue())
      {
         ImageAsset* image = imageItr.getValue();
         if (image == datablock)
         {
            keys.push_back(imageItr->key);
         }
      }
   }

   // Go through the list of keys to delete and remove them
   // from our image lists.
   for (U32 i=0; i < (U32)keys.size(); ++i)
   {
      removeImage(keys[i]);
   }

   if (keys.size() > 0)
      return true;

   return false;
}

//-----------------------------------------------------------------------------

S32 Grid::getImageCount()
{
   return mImageNameDictionary.size();
}

//-----------------------------------------------------------------------------

const char* Grid::getImageByIndex(S32 index)
{
   if (mImageNameDictionary.size() == 0)
      return "";

   S32 count = 0;
   for (HashTable<S32, StringTableEntry>::iterator itr=mImageNameDictionary.begin(); itr != mImageNameDictionary.end(); ++itr)
   {
      if (count == index)
      {
         return (itr.getValue());
      }

      ++ count;
   }

   return "";
}

//-----------------------------------------------------------------------------

const char* Grid::getImageByKey(S32 key)
{
   const char* name = mImageNameDictionary.find(key).getValue();

   if (name)
      return name;
   else
      return "";
}

//-----------------------------------------------------------------------------

bool Grid::findImage(const char* datablock, U32 frame, U32 rotation, S32& key)
{
   if (mImageNameDictionary.size() == 0)
      return false;

   StringTableEntry name = StringTable->insert(datablock);

   for (HashTable<S32, StringTableEntry>::iterator itr=mImageNameDictionary.begin(); itr != mImageNameDictionary.end(); ++itr)
   {
      // Check for the correct name
      if (itr.getValue() == name)
      {
         S32 searchkey = itr->key;
         
         // Check if this is the correct frame and rotation
         HashTable<S32, U32>::iterator frameItr = mImageFrameDictionary.find(searchkey);
         if (frameItr.getValue() == frame)
         {
            HashTable<S32, U32>::iterator rotItr = mImageRotationDictionary.find(searchkey);
            if (rotItr.getValue() == rotation)
            {
               key = searchkey;
               return true;
            }
         }
      }
   }

   return false;
}

//-----------------------------------------------------------------------------

S32 Grid::findNextFreeImageKey(S32 start)
{
   S32 searchValue = start;
   while (1)
   {
      HashTable<S32, StringTableEntry>::iterator itr=mImageNameDictionary.find(searchValue);
      if (itr.getValue() == 0)
      {
         // Found a free key value
         break;
      }

      ++searchValue;
   }

   return searchValue;
}

//-----------------------------------------------------------------------------

void Grid::sceneRender( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer )
{
   if ( !mRenderCellImages || !isGridValid() )
       return;

    // Calculate the size of a rendered cell.
    Vector2 size = getSize();
    Vector2 width( size.x / mCellCountX, 0 );
    Vector2 height( 0, size.y / mCellCountY );

    Vector2 cellVertices[4];

    // Iterate each cell.
    for ( U32 x=0; x < mCellCountX; ++x )
    {
        for ( U32 y=0; y < mCellCountY; ++y )
        {
            S32 key = getCellInteger(x, y);
            HashTable<S32, ImageAsset*>::iterator imageItr = mImageDataBlockDictionary.find(key);

            if ( imageItr.getValue() )
            {
                ImageAsset* imageMap = imageItr.getValue();

                // Fetch the frame area
                HashTable<S32, U32>::iterator frameItr = mImageFrameDictionary.find(key);
                U32 frame = frameItr.getValue();

                // Fetch current frame area.
                const ImageAsset::FrameArea::TexelArea& texelFrameArea = imageMap->getImageFrameArea( frame ).mTexelArea;

                // Fetch lower/upper texture coordinates.
                const Vector2& texLower = texelFrameArea.mTexelLower;
                const Vector2& texUpper = texelFrameArea.mTexelUpper;
   
                // Look up rotation
                HashTable<S32, U32>::iterator rotItr = mImageRotationDictionary.find(key);
                U32 rot = rotItr.getValue();

                cellVertices[VertexLookupTable[0+rot]] = mRenderOOBB[3] + width * (F32)x - height * (F32)y;
                cellVertices[VertexLookupTable[1+rot]] = cellVertices[VertexLookupTable[0+rot]] + width;
                cellVertices[VertexLookupTable[2+rot]] = cellVertices[VertexLookupTable[1+rot]] - height;
                cellVertices[VertexLookupTable[3+rot]] = cellVertices[VertexLookupTable[2+rot]] - width;

                // Submit batched quad.
                pBatchRenderer->SubmitQuad(
                    cellVertices[0],
                    cellVertices[1],
                    cellVertices[2],
                    cellVertices[3],
                    Vector2( texLower.x, texLower.y ),
                    Vector2( texUpper.x, texLower.y ),
                    Vector2( texUpper.x, texUpper.y ),
                    Vector2( texLower.x, texUpper.y ),
                    imageMap->getImageTexture() );
            }
        }
    }
}

//-----------------------------------------------------------------------------

void Grid::sceneRenderOverlay( const SceneRenderState* sceneRenderState )
{
   if ( !mRenderGrid || !isGridValid())
       return;

    // Calculate the size of a rendered cell.
    Vector2 size = getSize();
    Vector2 width( size.x / mCellCountX, 0 );
    Vector2 height( 0, size.y / mCellCountY );

    glEnable(GL_BLEND);
    glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
    glDisable(GL_TEXTURE_2D);

    glColor4ub(mRenderGridColor.red, mRenderGridColor.green, mRenderGridColor.blue, mRenderGridColor.alpha);
    glBegin(GL_LINES);

    Vector2 start;
    Vector2 end;
    for (U32 x=0; x<=mCellCountX; x+=mRenderGridStep)
    {
        start = mRenderOOBB[0] + width * (F32)x;
        end = mRenderOOBB[3] + width * (F32)x;
        glVertex2f(start.x, start.y);
        glVertex2f(end.x, end.y);
    }
    for (U32 y=0; y<=mCellCountY; y+=mRenderGridStep)
    {
        start = mRenderOOBB[0] + height * (F32)y;
        end = mRenderOOBB[1] + height * (F32)y;
        glVertex2f(start.x, start.y);
        glVertex2f(end.x, end.y);
    }
    glEnd();
}
