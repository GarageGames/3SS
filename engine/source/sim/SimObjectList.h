//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SIM_OBJECT_LIST_H_
#define _SIM_OBJECT_LIST_H_

#ifndef _VECTOR_H_
#include "collection/vector.h"
#endif

//-----------------------------------------------------------------------------

class SimObject;

//-----------------------------------------------------------------------------

class SimObjectList : public VectorPtr<SimObject*>
{
   static S32 QSORT_CALLBACK compareId(const void* a,const void* b);

public:
   void pushBack(SimObject*);       ///< Add the SimObject* to the end of the list, unless it's already in the list.
   void pushBackForce(SimObject*);  ///< Add the SimObject* to the end of the list, moving it there if it's already present in the list.
   void pushFront(SimObject*);      ///< Add the SimObject* to the start of the list.
   void remove(SimObject*);         ///< Remove the SimObject* from the list; may disrupt order of the list.

   inline SimObject* at(S32 index) const {  if(index >= 0 && index < size()) return (*this)[index]; return NULL; }

   /// Remove the SimObject* from the list; guaranteed to preserve list order.
   void removeStable(SimObject* pObject);

   void sortId();                   ///< Sort the list by object ID.
};

#endif // _SIM_OBJECT_LIST_H_