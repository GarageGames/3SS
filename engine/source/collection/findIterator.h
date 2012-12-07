//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _FIND_ITERATOR_H_
#define _FIND_ITERATOR_H_

template <class Iterator, class Value>
Iterator find(Iterator first, Iterator last, Value value)
{
   while (first != last && *first != value)
      ++first;
   return first;
}

#endif //_FIND_ITERATOR_H_
