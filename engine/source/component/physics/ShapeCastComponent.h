//-----------------------------------------------------------------------------
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _SHAPECASTCOMPONENT_H_
#define _SHAPECASTCOMPONENT_H_

#ifndef _VECTOR2_H_
#include "2d/core/Vector2.h"
#endif

#include "component/dynamicConsoleMethodComponent.h"
#include "2d/sceneobject/SceneObject.h"



class ShapeCastComponent : public DynamicConsoleMethodComponent
{
    typedef DynamicConsoleMethodComponent Parent;

public:

    struct ShapeCastContact
    {
        SceneObject*     pSceneObject;
        U32                 shapeIndex;
        F32                 fraction;
    };

    typedef Vector<ShapeCastContact> typeShapeCastContactVector;

    struct ShapeCastOutput
    {
        typeShapeCastContactVector shapeCastContactVector;

        // And a new contact
        void addContact(SceneObject* pSceneObject, U32 shapeIndex, F32 fraction)
        {
            ShapeCastContact newContact;
            newContact.pSceneObject = pSceneObject;
            newContact.shapeIndex = shapeIndex;
            newContact.fraction = fraction;

            shapeCastContactVector.push_back(newContact);
        }

        // Sorts the contact list by fraction
        void sort()
        {
            dQsort( shapeCastContactVector.address(), shapeCastContactVector.size(), sizeof(ShapeCastContact), shapeCastFractionSort );
        }

        static S32 QSORT_CALLBACK shapeCastFractionSort(const void* a, const void* b)
        {
            // Fetch scene objects.
            ShapeCastContact* pContactA  = (ShapeCastContact*)a;
            ShapeCastContact* pContactB  = (ShapeCastContact*)b;

            // Fetch fractions.
            const F32 fractionA = pContactA->fraction;
            const F32 fractionB = pContactB->fraction;

            if ( fractionA < fractionB )
                return -1;

            if ( fractionA > fractionB )
                return 1;

            return 0;
        }
    };
  
protected:
    SimObjectPtr<SceneObject> mOwner;

public:

    DECLARE_CONOBJECT(ShapeCastComponent);

    bool onAdd();
    void onRemove();

    static void initPersistFields();

    virtual bool onComponentAdd(SimComponent *target);
    virtual void onComponentRemove(SimComponent *target);

    void castShape(U32 shapeIndex, U32 childIndex, Vector2 endPoint, ShapeCastOutput* output);

    SimObjectPtr<SceneObject> getOwner() { return mOwner; }
};


#endif // _SHAPECASTCOMPONENT_H_