//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod(Trigger, setEnterCallback, void, 2, 3, "([setting]) Set whether trigger checks onEnter events\n"
              "@param setting Default is true.\n"
              "@return No return value.")
{
   // If the value isn't specified, the default is true.
   bool callback = true;
   if (argc > 2)
      callback = dAtob(argv[2]);

   object->setEnterCallback(callback);
}

//-----------------------------------------------------------------------------

ConsoleMethod(Trigger, setStayCallback, void, 2, 3, "([setting]) Set whether trigger checks onStay events\n"
              "@param setting Default is true.\n"
              "@return No return value.")
{
   // If the value isn't specified, the default is true.
   bool callback = true;
   if (argc > 2)
      callback = dAtob(argv[2]);

   object->setStayCallback(callback);
}

//-----------------------------------------------------------------------------

ConsoleMethod(Trigger, setLeaveCallback, void, 2, 3, "([setting]) Set whether trigger checks onLeave events\n"
              "@param setting Default is true.\n"
              "@return No return value.")
{
   // If the value isn't specified, the default is true.
   bool callback = true;
   if (argc > 2)
      callback = dAtob(argv[2]);

   object->setLeaveCallback(callback);
}

//-----------------------------------------------------------------------------

ConsoleMethod(Trigger, getEnterCallback, bool, 2, 2, "() \n @return Returns whether trigger checks onEnter events")
{
   return object->getEnterCallback();
}

//-----------------------------------------------------------------------------

ConsoleMethod(Trigger, getStayCallback, bool, 2, 2, "() \n @return Returns whether trigger checks onStay events")
{
   return object->getStayCallback();
}

//-----------------------------------------------------------------------------

ConsoleMethod(Trigger, getLeaveCallback, bool, 2, 2, "() \n @return Returns whether trigger checks onLeave events")
{
   return object->getLeaveCallback();
}

