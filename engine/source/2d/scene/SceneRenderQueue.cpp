//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SCENE_RENDER_QUEUE_H_
#include "2d/scene/SceneRenderQueue.h"
#endif

#ifndef _SCENE_RENDER_OBJECT_H_
#include "2d/scene/SceneRenderObject.h"
#endif

//-----------------------------------------------------------------------------

static EnumTable::Enums renderSortLookup[] =
                {
                { SceneRenderQueue::RENDER_SORT_OFF,            "Off" },
                { SceneRenderQueue::RENDER_SORT_NEWEST,         "New" },
                { SceneRenderQueue::RENDER_SORT_OLDEST,         "Old" },
                { SceneRenderQueue::RENDER_SORT_BATCH,          "Batch" },
                { SceneRenderQueue::RENDER_SORT_XAXIS,          "X Axis" },
                { SceneRenderQueue::RENDER_SORT_YAXIS,          "Y Axis" },
                { SceneRenderQueue::RENDER_SORT_ZAXIS,          "Z Axis" },
                { SceneRenderQueue::RENDER_SORT_INVERSE_XAXIS,  "-X Axis" },
                { SceneRenderQueue::RENDER_SORT_INVERSE_YAXIS,  "-Y Axis" },
                { SceneRenderQueue::RENDER_SORT_INVERSE_ZAXIS,  "-Z Axis" },
                };

EnumTable SceneRenderQueue::renderSortTable(sizeof(renderSortLookup) /  sizeof(EnumTable::Enums), &renderSortLookup[0]);

//-----------------------------------------------------------------------------

SceneRenderQueue::RenderSort SceneRenderQueue::getRenderSortEnum(const char* label)
{
    // Search for Mnemonic.
    for(U32 i = 0; i < (sizeof(renderSortLookup) / sizeof(EnumTable::Enums)); i++)
        if( dStricmp(renderSortLookup[i].label, label) == 0)
            return((RenderSort)renderSortLookup[i].index);

    // Error.
    return SceneRenderQueue::RENDER_SORT_INVALID;
}

//-----------------------------------------------------------------------------

const char* SceneRenderQueue::getRenderSortDescription( const RenderSort& sortMode )
{
    // Search for Mnemonic.
    for (U32 i = 0; i < (sizeof(renderSortLookup) / sizeof(EnumTable::Enums)); i++)
    {
        if( renderSortLookup[i].index == sortMode )
            return renderSortLookup[i].label;
    }

    // Fatal!
    AssertFatal(false, "SceneRenderQueue::getRenderSortDescription() - Invalid render sort mode.");

    // Error.
    return StringTable->EmptyString;
}

//-----------------------------------------------------------------------------

S32 QSORT_CALLBACK SceneRenderQueue::layeredNewFrontSort(const void* a, const void* b)
{
    // Fetch scene render requests.
    SceneRenderRequest* pSceneRenderRequestA  = *((SceneRenderRequest**)a);
    SceneRenderRequest* pSceneRenderRequestB  = *((SceneRenderRequest**)b);

    // Use serial Id,
    return pSceneRenderRequestA->mSerialId - pSceneRenderRequestB->mSerialId;
}
    
//-----------------------------------------------------------------------------

S32 QSORT_CALLBACK SceneRenderQueue::layeredOldFrontSort(const void* a, const void* b)
{
    // Fetch scene render requests.
    SceneRenderRequest* pSceneRenderRequestA  = *((SceneRenderRequest**)a);
    SceneRenderRequest* pSceneRenderRequestB  = *((SceneRenderRequest**)b);

    // Use reverse serial Id,
    return pSceneRenderRequestB->mSerialId - pSceneRenderRequestA->mSerialId;
}

//-----------------------------------------------------------------------------

S32 QSORT_CALLBACK SceneRenderQueue::layeredDepthSort(const void* a, const void* b)
{
    // Fetch scene render requests.
    SceneRenderRequest* pSceneRenderRequestA  = *((SceneRenderRequest**)a);
    SceneRenderRequest* pSceneRenderRequestB  = *((SceneRenderRequest**)b);

    // Fetch depths.
    const F32 depthA = pSceneRenderRequestA->mDepth;
    const F32 depthB = pSceneRenderRequestB->mDepth;

    return depthA < depthB ? 1 : depthA > depthB ? -1 : pSceneRenderRequestA->mSerialId - pSceneRenderRequestB->mSerialId;;
}

//-----------------------------------------------------------------------------

S32 QSORT_CALLBACK SceneRenderQueue::layeredInverseDepthSort(const void* a, const void* b)
{
    // Fetch scene render requests.
    SceneRenderRequest* pSceneRenderRequestA  = *((SceneRenderRequest**)a);
    SceneRenderRequest* pSceneRenderRequestB  = *((SceneRenderRequest**)b);

    // Fetch depths.
    const F32 depthA = pSceneRenderRequestA->mDepth;
    const F32 depthB = pSceneRenderRequestB->mDepth;

    return depthA < depthB ? -1 : depthA > depthB ? 1 : pSceneRenderRequestA->mSerialId - pSceneRenderRequestB->mSerialId;;
}

//-----------------------------------------------------------------------------

S32 QSORT_CALLBACK SceneRenderQueue::layerBatchOrderSort(const void* a, const void* b)
{
    // Fetch scene render requests.
    SceneRenderRequest* pSceneRenderRequestA  = *((SceneRenderRequest**)a);
    SceneRenderRequest* pSceneRenderRequestB  = *((SceneRenderRequest**)b);

    // Fetch scene render objects.
    SceneRenderObject* pSceneRenderObjectA = pSceneRenderRequestA->mpSceneRenderObject;
    SceneRenderObject* pSceneRenderObjectB = pSceneRenderRequestB->mpSceneRenderObject;

    // Fetch batch render isolation.
    const bool renderIsolatedA = pSceneRenderObjectA->getBatchIsolated();
    const bool renderIsolatedB = pSceneRenderObjectB->getBatchIsolated();

    if ( !renderIsolatedA && !renderIsolatedB )
        return 0;

    if ( renderIsolatedA && !renderIsolatedB )
        return -1;

    if ( !renderIsolatedA && renderIsolatedB )
        return 1;

    // Use serial Id,
    return pSceneRenderRequestA->mSerialId - pSceneRenderRequestB->mSerialId;
}

//-----------------------------------------------------------------------------

S32 QSORT_CALLBACK SceneRenderQueue::layeredXSortPointSort(const void* a, const void* b)
{
    // Fetch scene render requests.
    SceneRenderRequest* pSceneRenderRequestA  = *((SceneRenderRequest**)a);
    SceneRenderRequest* pSceneRenderRequestB  = *((SceneRenderRequest**)b);

    const F32 x1 = pSceneRenderRequestA->mWorldPosition.x + pSceneRenderRequestA->mSortPoint.x;
    const F32 x2 = pSceneRenderRequestB->mWorldPosition.x + pSceneRenderRequestB->mSortPoint.x;

    // We sort lower x values before higher values.
    return x1 < x2 ? -1 : x1 > x2 ? 1 : pSceneRenderRequestA->mSerialId - pSceneRenderRequestB->mSerialId;
}

//-----------------------------------------------------------------------------

S32 QSORT_CALLBACK SceneRenderQueue::layeredYSortPointSort(const void* a, const void* b)
{
    // Fetch scene render requests.
    SceneRenderRequest* pSceneRenderRequestA  = *((SceneRenderRequest**)a);
    SceneRenderRequest* pSceneRenderRequestB  = *((SceneRenderRequest**)b);

    const F32 y1 = pSceneRenderRequestA->mWorldPosition.y + pSceneRenderRequestA->mSortPoint.y;
    const F32 y2 = pSceneRenderRequestB->mWorldPosition.y + pSceneRenderRequestB->mSortPoint.y;

    // We sort lower y values before higher values.
    return y1 < y2 ? -1 : y1 > y2 ? 1 : pSceneRenderRequestA->mSerialId - pSceneRenderRequestB->mSerialId;
}

//-----------------------------------------------------------------------------

S32 QSORT_CALLBACK SceneRenderQueue::layeredInverseXSortPointSort(const void* a, const void* b)
{
    // Fetch scene render requests.
    SceneRenderRequest* pSceneRenderRequestA  = *((SceneRenderRequest**)a);
    SceneRenderRequest* pSceneRenderRequestB  = *((SceneRenderRequest**)b);

    const F32 x1 = pSceneRenderRequestA->mWorldPosition.x + pSceneRenderRequestA->mSortPoint.x;
    const F32 x2 = pSceneRenderRequestB->mWorldPosition.x + pSceneRenderRequestB->mSortPoint.x;

    // We sort higher x values before lower values.
    return x1 < x2 ? 1 : x1 > x2 ? -1 : pSceneRenderRequestA->mSerialId - pSceneRenderRequestB->mSerialId;
}

//-----------------------------------------------------------------------------

S32 QSORT_CALLBACK SceneRenderQueue::layeredInverseYSortPointSort(const void* a, const void* b)
{
    // Fetch scene render requests.
    SceneRenderRequest* pSceneRenderRequestA  = *((SceneRenderRequest**)a);
    SceneRenderRequest* pSceneRenderRequestB  = *((SceneRenderRequest**)b);

    const F32 y1 = pSceneRenderRequestA->mWorldPosition.y + pSceneRenderRequestA->mSortPoint.y;
    const F32 y2 = pSceneRenderRequestB->mWorldPosition.y + pSceneRenderRequestB->mSortPoint.y;

    // We sort higher y values before lower values.
    return y1 < y2 ? 1 : y1 > y2 ? -1 : pSceneRenderRequestA->mSerialId - pSceneRenderRequestB->mSerialId;
}
