//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _GRAPH_FIELD_H_
#define _GRAPH_FIELD_H_

#ifndef _VECTOR_H_
#include "collection/vector.h"
#endif

#ifndef _SERIALISATION_H_
#include "2d/core/Serialise.h"
#endif

///-----------------------------------------------------------------------------

class GraphField
{
public:
    /// Data Key Node.
    struct tDataKeyNode
    {
        F32     mTime;
        F32     mValue;
    };

private:
    Vector<tDataKeyNode>    mDataKeys;

    F32                     mValueScale;
    F32                     mTimeScale;
    F32                     mMaxTime;
    F32                     mMinValue;
    F32                     mMaxValue;
    F32                     mDefaultValue;

public:
    GraphField();
    virtual ~GraphField();

    virtual void copyTo(GraphField& graph);

    const tDataKeyNode getDataKeyNode( const U32 index ) const;
    void resetDataKeys( void );
    bool setTimeRepeat( const F32 timeRepeat );
    bool setValueScale( const F32 valueScale );
    void setValueBounds( F32 maxTime, F32 minValue, F32 maxValue, F32 defaultValue );
    S32 addDataKey( const F32 time, const F32 value );
    bool removeDataKey( const U32 index );
    void clearDataKeys( void );
    bool setDataKeyValue( const U32 index, const F32 value );
    F32 getDataKeyValue( const U32 index ) const;
    F32 getDataKeyTime( const U32 index ) const;
    U32 getDataKeyCount( void ) const;
    F32 getGraphValue( F32 time ) const;
    F32 getMinValue( void ) const { return mMinValue; };
    F32 getMaxValue( void ) const { return mMaxValue; };
    F32 getMinTime( void ) const { return 0.0f; }
    F32 getMaxTime( void ) const { return mMaxTime; };
    F32 getTimeRepeat( void ) const { return mTimeScale - 1.0f; };
    F32 getValueScale( void ) const { return mValueScale; };

    // Calculate Graph Variants.
    static F32 calcGraphBV( const GraphField& base, const GraphField& variation, const F32 effectAge, const bool modulate = false, const F32 modulo = 0.0f );
    static F32 calcGraphBVE( const GraphField& base, const GraphField& variation, const GraphField& effect, const F32 effectAge, const bool modulate = false, const F32 modulo = 0.0f );
    static F32 calcGraphBVLE( const GraphField& base, const GraphField& variation, const GraphField& overlife, const GraphField& effect, const F32 effectTime, const F32 particleAge, const bool modulate = false, const F32 modulo = 0.0f );

    /// Declare Serialise Object.
    DECLARE_T2D_SERIALISE( GraphField );
    /// Declare Serialise Objects.
    DECLARE_2D_LOADSAVE_METHOD( GraphField, 1 );
    DECLARE_2D_LOADSAVE_METHOD( GraphField, 2 );
};


///-----------------------------------------------------------------------------
/// Base-Only Graph.
///-----------------------------------------------------------------------------
struct GraphField_B
{
   GraphField       GraphField_Base;

   virtual void copyTo(GraphField_B& graph)
   {
      GraphField_Base.copyTo(graph.GraphField_Base);
   };

};

///-----------------------------------------------------------------------------
/// Life-Only Graph.
///-----------------------------------------------------------------------------
struct GraphField_L
{
   GraphField       GraphField_OverLife;

   void copyTo(GraphField_L& graph)
   {
      GraphField_OverLife.copyTo(graph.GraphField_OverLife);
   };

};

///-----------------------------------------------------------------------------
/// Base & Variation Graphs.
///-----------------------------------------------------------------------------
struct GraphField_BV
{
    GraphField       GraphField_Base;
    GraphField       GraphField_Variation;

    void copyTo(GraphField_BV& graph)
    {
       GraphField_Base.copyTo(graph.GraphField_Base);
       GraphField_Variation.copyTo(graph.GraphField_Variation);
    };

};

///-----------------------------------------------------------------------------
/// Base, Variation and Over-Time Graphs.
///-----------------------------------------------------------------------------
struct GraphField_BVL
{
    GraphField       GraphField_Base;
    GraphField       GraphField_Variation;
    GraphField       GraphField_OverLife;

    void copyTo(GraphField_BVL& graph)
    {
       GraphField_Base.copyTo(graph.GraphField_Base);
       GraphField_Variation.copyTo(graph.GraphField_Variation);
       GraphField_OverLife.copyTo(graph.GraphField_OverLife);
    };

};

#endif // _GRAPH_FIELD_H_
