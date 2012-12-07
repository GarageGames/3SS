//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _GRID_H_
#define _GRID_H_

#ifndef _HASHTABLE_H
#include "collection/hashTable.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

#ifndef _IMAGE_ASSET_H_
#include "2d/assets/ImageAsset.h"
#endif

#ifndef _ASSET_PTR_H_
#include "assets/assetPtr.h"
#endif

//-----------------------------------------------------------------------------

class Path;

//-----------------------------------------------------------------------------

class Grid : public SceneObject
{
    typedef SceneObject Parent;

protected:

    /// Defines a cell-to-cell edge for navigation
    class GraphEdge
    {
    protected:

        /// The two nodes this graph connects
        S32 mFromIndex;
        S32 mToIndex;

        /// The cost of this edge
        F32 mCost;

        /// Used to group all edges from the same cell together
        GraphEdge* mPrev;
        GraphEdge* mNext;

    public:
        GraphEdge() :
            mFromIndex(0),
            mToIndex(-1),
            mCost(0.0f),
            mPrev(NULL),
            mNext(NULL)
        {}

        GraphEdge(S32 from, S32 to, F32 cost) : 
            mFromIndex(from),
            mToIndex(to),
            mCost(cost),
            mPrev(NULL),
            mNext(NULL)
        {}

        virtual ~GraphEdge() {}

        S32 getFrom() { return mFromIndex; }
        void setFrom(S32 index) { mFromIndex = index; }

        S32 getTo() { return mToIndex; }
        void setTo(S32 index) { mToIndex = index; }

        F32 getCost() { return mCost; }
        void setCost(F32 cost) { mCost = cost; }

        GraphEdge* getPrevEdge() { return mPrev; }
        GraphEdge* getNextEdge() { return mNext; }
        void setPrevEdge(GraphEdge* edge) { mPrev = edge; }
        void setNextEdge(GraphEdge* edge) { mNext = edge; }

        bool operator==(const GraphEdge& rhs)
        {
            return rhs.mFromIndex == this->mFromIndex &&
                rhs.mToIndex == this->mToIndex &&
                rhs.mCost == this->mCost;
        }

        bool operator!=(const GraphEdge& rhs)
        {
            return !(*this == rhs);
        }
    };

public:
    struct SearchAStarData
    {
        /// The real accumulative cost of the node
        Vector<F32> mGCosts;

        /// The G cost plus the heuristic cost of the node
        Vector<F32> mFCosts;

        Vector<GraphEdge*> mShortestPath;
        Vector<GraphEdge*> mSearchFrontier;

        SearchAStarData(U32 nodeCount)
        {
            mGCosts.increment(nodeCount);
            mFCosts.increment(nodeCount);
            for (Vector<F32>::iterator itr = mGCosts.begin(); itr != mGCosts.end(); ++itr)
            {
            (*itr) = 0.0f;
            }
            for (Vector<F32>::iterator itr = mFCosts.begin(); itr != mFCosts.end(); ++itr)
            {
            (*itr) = 0.0f;
            }

            mShortestPath.increment(nodeCount);
            mSearchFrontier.increment(nodeCount);
            for (Vector<GraphEdge*>::iterator itr = mShortestPath.begin(); itr != mShortestPath.end(); ++itr)
            {
            (*itr) = NULL;
            }
            for (Vector<GraphEdge*>::iterator itr = mSearchFrontier.begin(); itr != mSearchFrontier.end(); ++itr)
            {
            (*itr) = NULL;
            }
        }
    };

protected:
    /// Image lists used for rendering
    HashTable<S32, StringTableEntry>      mImageNameDictionary;
    HashTable<S32, ImageAsset*>           mImageDataBlockDictionary;
    HashTable<S32, U32>                   mImageFrameDictionary;
    HashTable<S32, U32>                   mImageRotationDictionary;

    /// List used for cell integer searches
    Vector<Point2I>   mCellIntegerSearch;
    S32               mNextCellIntegerSearchIndex;

    /// Indicates that the grid's image list should be serialized when the
    /// scene is saved.
    bool mImageListSerialize;

    /// Indicates that cells should not be serialized when the scene
    /// is saved.
    bool mNoCellSerialize;

    /// Number of cells in x direction
    U32 mCellCountX;

    /// Number of cells in y direction
    U32 mCellCountY;

    /// Default weight for each cell
    F32 mDefaultCellWeight;

    /// List of cell weights of size mCellCountX*mCellCountY
    Vector<F32> mCellWeights;

    /// List of cell normalized (-1..1) positions
    Vector<Vector2> mCellNormalizedPositions;

    /// Is the cell cost dirty
    Vector<bool> mCellCostDirty;

    /// Default integer value for each cell
    S32 mDefaultCellInteger;

    /// An arbitrary integer that may be stored against each cell.
    /// Also used for image look-up if grid cell rendering is enabled
    Vector<S32> mCellInteger;

    /// Should a node graph be automatically built?
    bool mBuildGraph;

    /// Should diagonal edges be allowed on the node graph?
    bool mAllowDiagonalEdges;

    /// Is the node graph dirty?
    bool mGraphDirty;

    /// List of edges for the node graph, ordered by cell index
    Vector<GraphEdge*> mEdgeList;

    /// The fraction of cell size for nodes to be considered in a line
    F32 mOptimisePathNodeVariance;

    /// Should cell images be rendered
    bool mRenderCellImages;

    /// Is the cell rendering buffer dirty?
    bool mRenderBufferDirty;

    /// Should a grid be rendered?
    bool mRenderGrid;

    /// Color for the rendered grid
    ColorI mRenderGridColor;

    /// Determines the number of steps between drawing a grid line.
    S32 mRenderGridStep;

    bool isGridValid() const;
    void buildCells(bool clearCellList=false);

    /// Is the given cell and cell delta valid within the grid
    bool isCellValid(S32 x, S32 deltaX, S32 y, S32 deltaY, bool allowDiagonal);

    /// Delete all cell edges and clear the edge vector
    void clearEdgeList();

    /// Clear all edges from the given cell
    void clearEdges(U32 cellIndex);
    void clearEdges(U32 x, U32 y);

    /// Update all edges that go into the given cell
    /// @param x Cell's x index
    /// @param y Cell's y index
    /// @param allowDiagonal Calculate edges that are diagonal from the given cell
    /// @param calculateCost Calculate the cost with both distance and cell weight
    /// @param forceCalculation Do the calculation even if calculateCost is false, which
    ///                         means that the cost will be distance only.
    void updateGraphEdgesForCell(U32 x, U32 y, bool allowDiagonal, bool calculateCost, bool forceCalculation=false);

    F32 heuristicEuclid(U32 index1, U32 index2);

    void getCellListToTarget(U32 sx, U32 sy, U32 ex, U32 ey, Vector<U32>& pathCells);

    /// Taml callbacks.
    virtual void onTamlPreWrite( void );
    virtual void onTamlPostWrite( void );
    virtual void onTamlPreRead( void );
    virtual void onTamlPostRead( const TamlCollection& customCollection );

public:
    Grid();
    virtual ~Grid();

    static void initPersistFields();

    virtual bool onAdd();

    virtual void onDeleteNotify( SimObject* object );

    virtual void write( Stream &stream, U32 tabStop, U32 flags = 0  );

    U32 getCellCountX() { return mCellCountX; }
    U32 getCellCountY() { return mCellCountY; }

    void setCellCount(U32 x, U32 y);
    void setCellCountX(U32 x);
    void setCellCountY(U32 y);

    /// Calculate the normalized coordinates of the given cell.
    /// @note Normalized coordinates are in the range of -1.0 to 1.0
    void getCellNormalizedPosition(U32 x, U32 y, Vector2& outPos);

    /// Calculate the cell index for the given normalized coordinates.
    /// @note Normalized coordinates are in the range of -1.0 to 1.0
    void getCellFromNormalizedPosition(Vector2& inPos, S32& x, S32& y);

    /// Obtain the world position of the given cell
    void getCellWorldPosition(S32& x, S32& y, Vector2& outPos);

    /// Calculate the cell index for the given world coordinates.
    void getCellFromWorldPosition(Vector2& inPos, S32& x, S32& y);

    /// Provides the cell's integer value
    S32 getCellInteger(U32 x, U32 y);

    /// Set the given cell's integer value
    /// @param x The x index of the cell
    /// @param y The y index of the cell
    /// @param value The integer value of the cell
    void setCellInteger(U32 x, U32 y, S32 value);

    /// Does at least one cell have the requested value?
    /// @param value The integer value to look up
    /// @return True if the given integer value exists on at least one cell
    bool gridHasCellInteger(S32 value);

    /// Find the first cell that contains the requested integer value.
    /// @param value The integer value to search for
    /// @param cellIndex A Point2I that will contain the cell's index, if any
    /// @return True if at least one cell with the requested index is found
    /// @note: A single search list is used per grid and calling this method will
    /// reset the search list.
    bool findFirstCellWithInteger(S32 value, Point2I** cell);

    /// Find the next cell that contains the integer value requested using findFirstCellWithInteger()
    /// @param cellIndex A Point2I that will contain the cell's index, if any
    /// @return True if at least one cell with the requested index is found
    bool findNextCellWithInteger(Point2I** cell);

    /// Gives the weight of the requested cell
    F32 getCellWeight(U32 x, U32 y);

    /// Set the given cell's weight
    /// @param x The x index of the cell
    /// @param y The y index of the cell
    /// @param weight The weight of the cell
    /// @param rebuildGraph If true then immediately rebuild the node graph
    /// @note Setting rebuildGraph to true still respects the mBuildGraph setting.  If you 
    /// want to force a node graph rebuild regardless of mBuildGraph, use buildNodeGraph(true).
    void setCellWeight(U32 x, U32 y, F32 weight, bool rebuildGraph=false);

    /// Indicates if the requested cell is blocked.  A blocked cell has a weight of zero.
    bool isCellBlocked(U32 x, U32 y);

    /// Indicates if the given world position within a blocked cell.  A blocked cell has 
    /// a weight of zero.
    bool isWorldPositionBlocked(Vector2 worldPosition);

    /// Indicates if the box alinged to our grid contains any blocked cells.  A blocked
    /// cell has a weight of zero.
    bool isAlignedBoxBlocked(Vector2 boxWorldStart, Vector2 boxWorldEnd);

    /// Clear all cells to the default integer value
    void clearCellIntegers();

    /// Clear all cells to the given integer value
    void clearCellsToInteger(S32 value);

    /// Finds all cells that have an integer value of 'fromValue' and
    /// converts them to an integer value of 'toValue'.
    /// @param fromValue The integer value to modify
    /// @param toValue The integer value to set the cells to
    void setCellsFromIntegerToValue(S32 fromValue, S32 toValue);

    /// Copy all cell integers from this grid to the given grid.
    /// @param copyTo The grid to copy to
    /// @return True if the copy was successful
    /// @note: Will change the number of cells in copyTo to make this grid
    bool copyCellIntegersToGrid(Grid* copyTo);

    /// Clear all cells to the default weight value
    void clearCellWeights();

    /// Clear all cells to the given weight value
    void clearCellsToWeight(F32 weight);

    /// Copy all cell weights from this grid to the given grid.
    /// @param copyTo The grid to copy to
    /// @return True if the copy was successful
    /// @note: Will change the number of cells in copyTo to make this grid
    bool copyCellWeightsToGrid(Grid* copyTo);

    /// Build the node graph based on the cell weights
    /// @param force Force the generation of the node graph even if mBuildGraph is false
    void buildNodeGraph(bool force=false);

    /// Create a path with the given world start and end points.
    /// @param worldStart The path start location, in world coordinates
    /// @param worldEnd The path end location, in world coordinates
    /// @param optimise If true then optimise the path's nodes to the minimum required
    /// @param path The generated path
    /// @return True if the generated path is valid
    /// @note The worldStart and worldEnd points must be within the bounds of the grid.
    /// @note A valid path object is always returned.  However, if false is returned then the 
    /// path will contain no nodes.
    bool createPath(Vector2 worldStart, Vector2 worldEnd, bool optimise, Path** path);

    /// Add an image datablock to the image dictionary under the given key.
    /// @param key The integer key for this image
    /// @param datablock The image datablock to add
    /// @param frame The frame within the datablock to use
    /// @param rotation The rotation of the image [optional]
    /// @return True if the image path has been added to the dictionary. False if 
    /// the image could not be added (because the imagePath is an empty string).
    /// @note Will replace an existing key with this image path.
    bool addImage(S32 key, const char* datablock, U32 frame, U32 rotation=0);

    /// Remove an image from the image list based on the given key
    /// @param key The integer key of the image to remove
    void removeImage(S32 key);

    /// Remove all image frames that belong to the given datablock.
    /// @param datablock The datablock to remove
    /// @returns true if the datablock was removed, false if it wasn't found.
    bool removeImageDatablock(ImageAsset* datablock);

    /// Provides the total number of images in the image dictionary
    /// @return The number of images in the grid's image list
    S32 getImageCount();

    /// Get the image at the given index into the image dictionary
    /// @param index The index into the image dictionary
    /// @return The image path at the requested index
    const char* getImageByIndex(S32 index);

    /// Get the image at the given key within the image dictionary
    /// @param key The integer key of the image to retrieve
    /// @return The image path at the requested key, or an empty string
    /// if that key doesn't exist.
    const char* getImageByKey(S32 key);

    /// Finds the key of the requested datablock, frame and rotation
    /// @param datablock The image datablock to search for
    /// @param frame The frame within the datablock
    /// @param rotation The rotation of the image in the range of 0-3
    /// @param key The found key
    /// @return True if the key was found, false if not.
    bool findImage(const char* datablock, U32 frame, U32 rotation, S32& key);

    /// Finds the next free image key starting at the given value
    /// @param start The number to start searching from
    /// @return The next free key value
    S32 findNextFreeImageKey(S32 start);

    virtual void setAngle( const F32 radians ) { Parent::setAngle( 0.0f ); }; // Stop angle being changed.
    virtual void setFixedAngle( const bool fixed ) { Parent::setFixedAngle( true ); } // Always fixed angle.
    virtual void setFlip( const bool flipX, bool flipY ) { Parent::setFlip( false, false ); }; // Stop flip being changed.
    virtual void setFlipX( const bool flipX ) { Parent::setFlipX( false ); }; // Stop flip being changed.
    virtual void setFlipY( bool flipY ) { Parent::setFlipY( false ); }; // Stop flip being changed.

    virtual bool canRender( void ) const { return isGridValid() && mRenderCellImages; }
    virtual void sceneRender( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer );
    virtual void sceneRenderOverlay( const SceneRenderState* sceneRenderState );

    DECLARE_CONOBJECT(Grid);

protected:
    static bool writeImageListSerialize( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(Grid); return pCastObject->mImageListSerialize == true; }
    static bool writeNoCellSerialize( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(Grid); return pCastObject->mNoCellSerialize == true; }
    static bool setCellCountField(void* obj, const char* data);
    static const char* getCellCountField(void* obj, const char* data);
    static bool writeDefaultCellWeight( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(Grid); return mNotEqual(pCastObject->mDefaultCellWeight, 1.0f); }
    static bool writeDefaultCellInteger( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(Grid); return pCastObject->mDefaultCellInteger != 0; }
    static bool writeBuildGraph( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(Grid); return pCastObject->mBuildGraph == true; }
    static bool writeAllowDiagonalEdges( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(Grid); return pCastObject->mAllowDiagonalEdges == true; }
    static bool writeOptimisePathNodeVariance( void* obj, StringTableEntry pFieldName ) { return mNotEqual(static_cast<Grid*>(obj)->mOptimisePathNodeVariance, 0.25f); }
    static bool writeRenderCellImages( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(Grid); return pCastObject->mRenderCellImages == true; }
    static bool writeRenderGrid( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(Grid); return pCastObject->mRenderGrid == true; }
    static bool writeRenderGridColor( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(Grid); return pCastObject->mRenderGridColor != ColorI(255,255,255); }
    static bool writeRenderGridStep( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(Grid); return pCastObject->mRenderGridStep != 1; }  
};

//-----------------------------------------------------------------------------

inline bool Grid::isGridValid() const
{
    if (mCellCountX == 0 || mCellCountY == 0)
    {
        // Cannot have a grid with these dimensions.
        return false;
    }

    return true;
}

//-----------------------------------------------------------------------------

inline F32 Grid::heuristicEuclid(U32 index1, U32 index2)
{
    return (mCellNormalizedPositions[index1] - mCellNormalizedPositions[index2]).Length();
}

//-----------------------------------------------------------------------------

inline bool Grid::isCellValid(S32 x, S32 deltaX, S32 y, S32 deltaY, bool allowDiagonal)
{
    // Skip the center of the 3x3 grid, which is our cell
    if (deltaX==0 && deltaY==0)
        return false;

    S32 cellX = x + deltaX;
    S32 cellY = y + deltaY;

    // Make sure this is a valid cell index
    if (cellX < 0 || cellX >= (S32)mCellCountX || cellY < 0 || cellY >= (S32)mCellCountY)
        return false;

    // Do we not consider diagonal cells?
    if (!allowDiagonal)
    {
        if ((deltaX==-1 && deltaY==-1) || (deltaX==1 && deltaY==-1) || (deltaX==-1 && deltaY==1) || (deltaX==1 && deltaY==1))
            return false;
    }

    return true;
}

#endif   // _GRID_H_
