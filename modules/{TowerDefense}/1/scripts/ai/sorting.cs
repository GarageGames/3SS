//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------


/// <summary>
/// Takes a list of objects and sorts it according the the specified comparator function.
/// </summary>
/// <param name="%objectList">List of object ids to be sorted</param>
/// <param name="%comparatorFunctionName">The name of the function that returns a value to be used for comparison.</param>
/// <param name="%comparatorFunctionParams">String of parameters to the function.</param>
/// <param name="%increasing">If true (default), sorts in increasing order</param>
/// <return>A sorted list of objects</return>
function sortObjectList(%objectList, %comparatorFunctionName, %comparatorFunctionParams, %increasing)
{
   if (%increasing $= "")
      %increasing = true;   
   
   %sortedList = "";
     
   // Cache a map of comparator values for each item in list
   for (%i = 0; %i < getWordCount(%objectList); %i++)
   {
       %object = getWord(%objectList, %i);
       %valueMap[%object] = %object.call(%comparatorFunctionName, %comparatorFunctionParams);
   }     
      
   // Generate a sorted list of objects   
   while (getWordCount(%objectList) > 0)
   {
      %index = 0;
      %value = %valueMap[getWord(%objectList, 0)];   
   
      for (%i = 1; %i < getWordCount(%objectList); %i++)
      {
         %newValue = %valueMap[getWord(%objectList, %i)];
         if (%newValue < %value)
         {
            %index = %i;
            %value = %newValue;
         }
      }
      
      // Save that target in the sorted list and remove it from the objectList
      if (%sortedList $= "")   
      {
         %sortedList = getWord(%objectList, %index);
      }
      else if (%increasing) 
      {     
         %sortedList = %sortedList SPC getWord(%objectList, %index);
      }
      else
      { 
         %sortedList = getWord(%objectList, %index) SPC %sortedList;
      }
      
      %objectList = removeWord(%objectList, %index);      
   }
   
   return %sortedList;
}

/// <summary>
/// Takes a list of objects returns all objects that have the lowest value as specified by a comparator function.
/// </summary>
/// <param name="%objectList">List of object ids</param>
/// <param name="%comparatorFunctionName">The name of the function that returns a value to be used for comparison.</param>
/// <param name="%comparatorFunctionParams">String of parameters to the function.</param>
/// <return>A list of objects tied for the lowest values</return>
function getLowestObjectsFromList(%objectList, %comparatorFunctionName, %comparatorFunctionParams)
{
   // Sort the objects in increasing order
   %sortedList = sortObjectList(%objectList, %comparatorFunctionName, %comparatorFunctionParams, true);
   
   %lowestList = getWord(%sortedList, 0);

   if (!isObject(%lowestList))
      return "";   
   
   %lowestValue = %lowestList.call(%comparatorFunctionName, %comparatorFunctionParams);
   
   for (%i = 1; %i < getWordCount(%sortedList); %i++)
   {
      %obj = getWord(%sortedList, %i);
      %objValue = %obj.call(%comparatorFunctionName, %comparatorFunctionParams);
      if (%objValue == %lowestValue)
      {
         %lowestList = %lowestList SPC %obj;
      }
   }
   
   return %lowestList;
}

function getHighestObjectsFromList(%objectList, %comparatorFunctionName, %comparatorFunctionParams)
{
   // Sort the objects in increasing order
   %sortedList = sortObjectList(%objectList, %comparatorFunctionName, %comparatorFunctionParams, false);
   
   %highestList = getWord(%sortedList, 0);
   
   if (!isObject(%highestList))
      return "";     
   
   %highestValue = %highestList.call(%comparatorFunctionName, %comparatorFunctionParams);
   
   for (%i = 1; %i < getWordCount(%sortedList); %i++)
   {
      %obj = getWord(%sortedList, %i);
      %objValue = %obj.call(%comparatorFunctionName, %comparatorFunctionParams);
      if (%objValue == %highestValue)
      {
         %highestList = %highestList SPC %obj;
      }
   }
   
   return %highestList;   
}

/// Test functions

function testSortObjectList()
{
   echo ("Starting sort object list tests");   
   
   %enemy1 = new ScriptObject() { class = SortTestObject; testVar = 3; };
   %enemy2 = new ScriptObject() { class = SortTestObject; testVar = 2; };
   %enemy3 = new ScriptObject() { class = SortTestObject; testVar = 1; };
   %enemy4 = new ScriptObject() { class = SortTestObject; testVar = 10; };
   %enemy5 = new ScriptObject() { class = SortTestObject; testVar = 1; };

   %list = %enemy1 SPC %enemy2 SPC %enemy3 SPC %enemy4 SPC %enemy5;
   
   echo(%list);
   
   %increasingList = sortObjectList(%list, "getTestVar", "", true);
   
   echo(%increasingList);
   
   %decreasingList = sortObjectList(%list, "getTestVar", "", false);
   
   echo(%decreasingList);
   
   %lowest = getLowestObjectsFromList(%list, "getTestVar", "");
   
   echo(%lowest);
   
   %highest = getHighestObjectsFromList(%list, "getTestVar", "");
   
   echo(%highest);
}

function SortTestObject::getTestVar(%this, %params)
{
   return %this.testVar;
}

