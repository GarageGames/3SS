//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _IAIMOVEMENT_H_
#define _IAIMOVEMENT_H_

class IAIMovement
{
protected:

   F32 mWeight;

public:

   IAIMovement() { mWeight = 1.0f; }

   virtual F32 getWeight() { return mWeight; }
   virtual void setWeight(F32 weight) { mWeight = weight; }

   virtual Vector2 calculateForce(AIMovementComponent::MovementParameters& params) = 0;
};

#endif   // _IAIMOVEMENT_H_
