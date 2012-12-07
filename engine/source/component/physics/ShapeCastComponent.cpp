//-----------------------------------------------------------------------------
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "ShapeCastComponent.h"

IMPLEMENT_CO_NETOBJECT_V1(ShapeCastComponent);

bool ShapeCastComponent::onAdd()
{
    if (!Parent::onAdd())
        return false;
    
    return true;
}

void ShapeCastComponent::onRemove()
{
    Parent::onRemove();
}

void ShapeCastComponent::initPersistFields()
{
    Parent::initPersistFields();

    addProtectedField("Owner", TypeSimObjectPtr, Offset(mOwner, ShapeCastComponent), &defaultProtectedSetFn, &defaultProtectedGetFn, "" );
}

bool ShapeCastComponent::onComponentAdd(SimComponent* target)
{
    if (!Parent::onComponentAdd(target))
        return false;

    SceneObject *owner = dynamic_cast<SceneObject*>(target);
    if (!owner)
    {
        Con::warnf("ShapeCastComponent::onComponentAdd - Must be added to a SceneObject.");
        return false;
    }

    // Store our owner
    mOwner = owner;

    return true;
}

void ShapeCastComponent::onComponentRemove(SimComponent* target)
{
    Parent::onComponentRemove(target);
}

void ShapeCastComponent::castShape(U32 shapeIndex, U32 childIndex, Vector2 endPoint, ShapeCastOutput* shapeCastOutput)
{
    // Initialize output
    shapeCastOutput->shapeCastContactVector.clear();

    /////////////////
    // Calculate normalized AABB.//TEMP: replace
    b2AABB aabb;
    aabb.lowerBound.x = -20;
    aabb.lowerBound.y = -20;
    aabb.upperBound.x = 20;
    aabb.upperBound.y = 20;
    /////////////

    // Get the object's scene
    Scene* pScene = mOwner->getScene();

    // Fetch world query and clear results.
    WorldQuery* pWorldQuery = pScene->getWorldQuery(true);

    // Set filter.
    WorldQueryFilter queryFilter(MASK_ALL, MASK_ALL, true, false, true, true);
    pWorldQuery->setQueryFilter(queryFilter);

    // Perform query.
    pWorldQuery->fixtureQueryArea(aabb);    

    // Finish if no results.
    if (pWorldQuery->getQueryResultsCount() == 0)
        return;

    // Fetch results.
    typeWorldQueryResultVector& queryResults = pWorldQuery->getQueryResults();

    // Get fixture for casted object's shape
    b2Fixture* fixtureA = mOwner->getBody()->GetFixtureList();
    for (U32 i = 0; i < mOwner->getCollisionShapeCount(); i++)
    {
        if (i == shapeIndex)
            break;

        fixtureA = fixtureA->GetNext();
    }

    // Get the casted object shape and body
    b2Shape* pShapeA = fixtureA->GetShape();
    b2Body* pBodyA = fixtureA->GetBody();


    // Create sweeps to be used for objects in TOI calculations
    // These are not the actual sweeps of the objects in the sim
    // as we are performing the shape-cast instantaneously
    
    // Set sweepA - This is the sweep for the casted-shape
    const b2Transform& xfA = pBodyA->GetTransform();
    b2Sweep sweepA;
    sweepA.localCenter.SetZero();
    sweepA.c0 = xfA.p;
    sweepA.c.Set(endPoint.x, endPoint.y);
    sweepA.a0 = xfA.q.GetAngle();
    sweepA.a = xfA.q.GetAngle();

    // Initialize sweepB - This is used for all shapes we are casting against
    b2Sweep sweepB;
    sweepB.localCenter.SetZero();
    sweepB.c0.SetZero();
    sweepB.c.SetZero();
    sweepB.a0 = 0;
    sweepB.a = 0;
    
    
    SceneObject* pSceneObjectB = NULL;
    b2Shape* pShapeB = NULL;
    b2Body* pBodyB = NULL;

    // Perform TOI calculation on eached picked object
    for (U32 n = 0; n < (U32)queryResults.size(); n++)
    {
        // Get a picked object
        pSceneObjectB = queryResults[n].mpSceneObject;

        // Ignore the casted object
        if (pSceneObjectB == mOwner)
            continue;

        // Get that object's body
        pBodyB = pSceneObjectB->getBody();

        // Set the sweep for the shape we are casting against
        const b2Transform& xfB = pBodyB->GetTransform();
        sweepB.c0 = xfB.p;
        sweepB.c = xfB.p;
        sweepB.a0 = xfB.q.GetAngle();
        sweepB.a = xfB.q.GetAngle();
        
        // Iterate through all collision shapes on the body
        for (b2Fixture* fixtureB = pBodyB->GetFixtureList(); fixtureB; fixtureB = fixtureB->GetNext())
        {
            pShapeB = fixtureB->GetShape();

            // Loop through each child shape (>1 only for edge and chain shapes) on both shapes
            for (S32 childIndexA = 0; childIndexA < pShapeA->GetChildCount(); childIndexA++)
            {
                for (S32 childIndexB = 0; childIndexB < pShapeB->GetChildCount(); childIndexB++)
                {
                    // Compute the time of impact in interval [0, 1]
                    b2TOIInput input;
                    input.proxyA.Set(pShapeA, childIndexA);
                    input.proxyB.Set(pShapeB, childIndexB);
                    input.sweepA = sweepA;
                    input.sweepB = sweepB;
                    input.tMax = 1.0f;

                    b2TOIOutput output;
                    b2TimeOfImpact(&output, &input);

                    F32 beta = output.t;
                    U32 contactShapeIndex = pSceneObjectB->getCollisionShapeIndex(fixtureB);

                    if (output.state == b2TOIOutput::e_touching)
                    {
                        // Add to output
                        shapeCastOutput->addContact(pSceneObjectB, contactShapeIndex, beta);
                    }
                }
            }
        }
    }

    // Clear world query.
    pWorldQuery->clearQuery();

    // Sort the output in order of fraction
    shapeCastOutput->sort();
}

//-----------------------------------------------------------------------------

ConsoleMethod(ShapeCastComponent, castShape, const char*, 4, 6, "(endX/Y, shapeIndex, [childIndex]) - .\n"
              "@param endx/y The coordinates of the end point of the center of the object as either (\"x y\") or (x,y)\n"
              "@param object SceneObject's ID\n"
              "@param shapeIndex Index of collision shape.\n"
              "@param childIndex (Optional) For chain/edge shapes, specifies the child fixture (set to zero for other shape types).\n"
              "@return Returns a list of contacted object shapes in blocks of detail items where each block represents a single contact detail in the format:"
                "<ObjectId ShapeIndex Fraction> <ObjectId ShapeIndex Fraction> etc. (sorted by Fraction) \n")
{
    Vector2 endPoint;

    // Grab the number of elements in the first parameter.
    U32 elementCount =Utility::mGetStringElementCount(argv[2]);

    // ("x y")
    S32 nextArg = 2;
    if (elementCount == 2)
    {
        endPoint = Utility::mGetStringElementVector(argv[nextArg]);
        nextArg++;
    }
    // (x, y)
    else if (elementCount == 1)
    {
        endPoint = Vector2(dAtof(argv[nextArg]), dAtof(argv[nextArg+1]));
        nextArg += 2;
    }
    // Invalid
    else
    {
        Con::warnf("Scene::castShape() - Invalid number of parameters!");
        return NULL;
    }

    // Fetch shape index.
    const S32 shapeIndex = dAtoi(argv[nextArg++]);

    // Validate shape index
    const S32 shapeCount = object->getOwner()->getCollisionShapeCount();
    if (shapeIndex >= shapeCount)
    {
        Con::warnf("Scene::castShape() - Invalid shape index of %d.", shapeIndex);
        return false;
    }

    // Check for child index
    const U32 childIndex = (argc == nextArg+1) ? dAtoi(argv[nextArg]) : 0;

    // Perform the cast-shape operation
    ShapeCastComponent::ShapeCastOutput output;
    object->castShape(shapeIndex, childIndex, endPoint, &output);

    // Check if the contact list is empty
    if (output.shapeCastContactVector.size() == 0)
        return StringTable->EmptyString;

    // Set Max Buffer Size.
    const U32 maxBufferSize = 4096;

    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer(maxBufferSize);
    pBuffer[0] = 0;

    // Set Buffer Counter.
    U32 bufferCount = 0;

    // Add contacts to List.
    ShapeCastComponent::typeShapeCastContactVector contactVector = output.shapeCastContactVector;
    for ( S32 n = 0; n < contactVector.size(); n++ )
    {
        // Fetch the contact struct
        ShapeCastComponent::ShapeCastContact contact = contactVector[n];

        bufferCount += dSprintf( pBuffer + bufferCount, maxBufferSize-bufferCount, "%d %d %g ",
            contact.pSceneObject->getId(),
            contact.shapeIndex,
            contact.fraction );

        // Finish early if we run out of buffer space.
        if ( bufferCount >= maxBufferSize )
        {
            // Warn.
            Con::warnf("Scene::castShape() - Too many items to return to scripts!");
            break;
        }
    }

    // Return buffer.
    return pBuffer;
}
