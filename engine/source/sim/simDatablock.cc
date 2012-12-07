//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "sim/simBase.h"
#include "console/consoleTypes.h"
#include "sim/scriptObject.h"
#include "sim/simDatablock.h"

//-----------------------------------------------------------------------------

IMPLEMENT_CO_DATABLOCK_V1(SimDataBlock);
SimObjectId SimDataBlock::sNextObjectId = DataBlockObjectIdFirst;
S32 SimDataBlock::sNextModifiedKey = 0;

//---------------------------------------------------------------------------

SimDataBlock::SimDataBlock()
{
   setModDynamicFields(true);
   setModStaticFields(true);
}

bool SimDataBlock::onAdd()
{
   Parent::onAdd();

   // This initialization is done here, and not in the constructor,
   // because some jokers like to construct and destruct objects
   // (without adding them to the manager) to check what class
   // they are.
   modifiedKey = ++sNextModifiedKey;
   AssertFatal(sNextObjectId <= DataBlockObjectIdLast,
               "Exceeded maximum number of data blocks");

   // add DataBlock to the DataBlockGroup unless it is client side ONLY DataBlock
   if (getId() >= DataBlockObjectIdFirst && getId() <= DataBlockObjectIdLast)
      if (SimGroup* grp = Sim::getDataBlockGroup())
         grp->addObject(this);

   return true;
}

void SimDataBlock::onRemove()
{
    Parent::onRemove();
}

void SimDataBlock::assignId()
{
   // We don't want the id assigned by the manager, but it may have
   // already been assigned a correct data block id.
   if (getId() < DataBlockObjectIdFirst || getId() > DataBlockObjectIdLast)
      setId(sNextObjectId++);
}

void SimDataBlock::onStaticModified(const char* slotName, const char* newValue)
{
   modifiedKey = sNextModifiedKey++;

}

/*void SimDataBlock::setLastError(const char*)
{
} */

void SimDataBlock::packData(BitStream*)
{
}

void SimDataBlock::unpackData(BitStream*)
{
}

bool SimDataBlock::preload(bool, char[256])
{
   return true;
}

ConsoleFunction(deleteDataBlocks, void, 1, 1, "() Use the deleteDataBlocks function to cause a server to delete all datablocks that have thus far been loaded and defined.\n"
                                                                "This is usually done in preparation of downloading a new set of datablocks, such as occurs on a mission change, but it's also good post-mission cleanup\n"
                                                                "@return No return value.")
{
   // delete from last to first:
   SimGroup *grp = Sim::getDataBlockGroup();
   for(S32 i = grp->size() - 1; i >= 0; i--)
   {
      SimObject *obj = (*grp)[i];
      obj->deleteObject();
   }
   SimDataBlock::sNextObjectId = DataBlockObjectIdFirst;
   SimDataBlock::sNextModifiedKey = 0;
}
