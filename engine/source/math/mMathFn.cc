//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "math/mMathFn.h"
#include "math/mPlane.h"
#include "math/mMatrix.h"


void mTransformPlane(const MatrixF& mat,
                     const Point3F& scale,
                     const PlaneF&  plane,
                     PlaneF*        result)
{
   m_matF_x_scale_x_planeF(mat, &scale.x, &plane.x, &result->x);
}

//--------------------------------------

U32 getNextPow2(U32 io_num)
{
   S32 oneCount   = 0;
   S32 shiftCount = -1;
   while (io_num) {
      if(io_num & 1)
         oneCount++;
      shiftCount++;
      io_num >>= 1;
   }
   if(oneCount > 1)
      shiftCount++;

   return U32(1 << shiftCount);
}

//--------------------------------------

U32 getBinLog2(U32 io_num)
{
   AssertFatal(io_num != 0 && isPow2(io_num) == true,
               "Error, this only works on powers of 2 > 0");

   S32 shiftCount = 0;
   while (io_num) {
      shiftCount++;
      io_num >>= 1;
   }

   return U32(shiftCount - 1);
}


