//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod(BehaviorInstance, getTemplateName, const char *, 2, 2, "() - Get the template name of this behavior\n"
                                                                     "@return (string name) The name of the template this behavior was created from")
{
   const char* pName = object->getTemplateName();
   return pName ? pName : StringTable->EmptyString;
}
