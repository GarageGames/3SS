//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _TAML_CALLBACKS_H_
#define _TAML_CALLBACKS_H_

//-----------------------------------------------------------------------------

class TamlCollection;
class SimObject;

//-----------------------------------------------------------------------------

class TamlCallbacks
{
    friend class Taml;

private:
    /// Called prior to Taml writing the object.
    virtual void onTamlPreWrite( void ) = 0;

    /// Called after Taml has finished writing the object.
    virtual void onTamlPostWrite( void ) = 0;

    /// Called prior to Taml reading the object.
    virtual void onTamlPreRead( void ) = 0;

    /// Called after Taml has finished reading the object.
    /// The custom collection is additionally passed here for object who want to process it at the end of reading.
    virtual void onTamlPostRead( const TamlCollection& customCollection ) = 0;

    /// Called after Taml has finished reading the object and has added the object to any parent.
    virtual void onTamlAddParent( SimObject* pParentObject ) = 0;

    /// Called during the writing of the object to allow custom collection properties to be written.
    virtual void onTamlCustomWrite( TamlCollection& customCollection ) = 0;

    /// Called during the reading of the object to allow custom collection properties to be read.
    virtual void onTamlCustomRead( const TamlCollection& customCollection ) = 0;
};

#endif // _TAML_CALLBACKS_H_