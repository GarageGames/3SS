//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/*
** Alive and Ticking
** (c) Copyright 2006 Burnt Wasp
**     All Rights Reserved.
**
** Filename:    scriptMsgListener.cc
** Author:      Tom Bampton
** Created:     19/8/2006
** Purpose:
**   Script Message Listener
**
*/

#include "console/consoleTypes.h"
#include "messaging/dispatcher.h"
#include "messaging/scriptMsgListener.h"

//////////////////////////////////////////////////////////////////////////
// Constructor
//////////////////////////////////////////////////////////////////////////

ScriptMsgListener::ScriptMsgListener()
{
   mNSLinkMask = LinkSuperClassName | LinkClassName;
}

IMPLEMENT_CONOBJECT(ScriptMsgListener);

//////////////////////////////////////////////////////////////////////////

bool ScriptMsgListener::onAdd()
{
   if(! Parent::onAdd())
      return false;

   linkNamespaces();
   Con::executef(this, 1, "onAdd");
   return true;
}

void ScriptMsgListener::onRemove()
{
   Con::executef(this, 1, "onRemove");
   unlinkNamespaces();
   
   Parent::onRemove();
}

//////////////////////////////////////////////////////////////////////////
// Public Methods
//////////////////////////////////////////////////////////////////////////

bool ScriptMsgListener::onMessageReceived(StringTableEntry queue, const char* event, const char* data)
{
   return dAtob(Con::executef(this, 4, "onMessageReceived", queue, event, data));
}

bool ScriptMsgListener::onMessageObjectReceived(StringTableEntry queue, Message *msg)
{
   return dAtob(Con::executef(this, 4, "onMessageObjectReceived", queue, Con::getIntArg(msg->getId())));
}

//////////////////////////////////////////////////////////////////////////

void ScriptMsgListener::onAddToQueue(StringTableEntry queue)
{
   Con::executef(this, 2, "onAddToQueue", queue);
   IMLParent::onAddToQueue(queue);
}

void ScriptMsgListener::onRemoveFromQueue(StringTableEntry queue)
{
   Con::executef(this, 2, "onRemoveFromQueue", queue);
   IMLParent::onRemoveFromQueue(queue);
}
