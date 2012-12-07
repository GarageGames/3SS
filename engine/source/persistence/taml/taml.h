//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _TAML_H_
#define _TAML_H_

#ifndef _TAML_CALLBACKS_H_
#include "tamlCallbacks.h"
#endif

#ifndef _TAML_COLLECTION_H_
#include "tamlCollection.h"
#endif

#ifndef _TAML_WRITE_NODE_H_
#include "TamlWriteNode.h"
#endif

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

#ifndef _HASHTABLE_H
#include "collection/hashTable.h"
#endif

#ifndef _FILESTREAM_H_
#include "io/fileStream.h"
#endif

//-----------------------------------------------------------------------------

#define TAML_SIGNATURE                  "Taml"
#define TAML_ID_ATTRIBUTE_NAME          "TamlId"
#define TAML_REFID_ATTRIBUTE_NAME       "TamlRefId"
#define TAML_REF_FIELD_NAME             "TamlRefField"
#define TAML_OBJECTNAME_ATTRIBUTE_NAME  "Name"

//-----------------------------------------------------------------------------

class TamlXmlWriter;
class TamlXmlReader;
class TamlBinaryWriter;
class TamlBinaryReader;

//-----------------------------------------------------------------------------

class Taml : public SimObject
{
    friend class TamlXmlWriter;
    friend class TamlXmlReader;
    friend class TamlBinaryWriter;
    friend class TamlBinaryReader;

public:
    enum TamlFormatMode
    {
        InvalidFormat = 0,
        XmlFormat,
        BinaryFormat
    };

private:
    typedef SimObject Parent;
    typedef Vector<TamlWriteNode*>                  typeNodeVector;
    typedef HashMap<SimObjectId, TamlWriteNode*>    typeCompiledHash;

    typeNodeVector      mCompiledNodes;
    typeCompiledHash    mCompiledObjects;
    U32                 mMasterNodeId;
    TamlFormatMode      mFormatMode;
    bool                mCompressed;

private:
    void resetCompilation( void );

    TamlWriteNode* compileObject( SimObject* pSimObject );
    void compileStaticFields( TamlWriteNode* pTamlWriteNode );
    void compileDynamicFields( TamlWriteNode* pTamlWriteNode );
    void compileChildren( TamlWriteNode* pTamlWriteNode );
    void compileCollection( TamlWriteNode* pTamlWriteNode );

    bool write( FileStream& stream, SimObject* pSimObject );
    SimObject* read( FileStream& stream );
    template<typename T> inline T* read( FileStream& stream )
    {
        SimObject* pSimObject = read( stream );
        if ( pSimObject == NULL )
            return NULL;
        T* pObj = dynamic_cast<T*>( pSimObject );
        if ( pObj != NULL )
            return pObj;
        pSimObject->deleteObject();
        return NULL;
    }

    static SimObject* createType( StringTableEntry typeName );

    /// Taml callbacks.
    inline void tamlPreWrite( TamlCallbacks* pCallbacks )                                           { pCallbacks->onTamlPreWrite(); }
    inline void tamlPostWrite( TamlCallbacks* pCallbacks )                                          { pCallbacks->onTamlPostWrite(); }
    inline void tamlPreRead( TamlCallbacks* pCallbacks )                                            { pCallbacks->onTamlPreRead(); }
    inline void tamlPostRead( TamlCallbacks* pCallbacks, const TamlCollection& customCollection )   { pCallbacks->onTamlPostRead( customCollection ); }
    inline void tamlAddParent( TamlCallbacks* pCallbacks, SimObject* pParentObject )                { pCallbacks->onTamlAddParent( pParentObject ); }
    inline void tamlCustomWrite( TamlCallbacks* pCallbacks, TamlCollection& customCollection )      { pCallbacks->onTamlCustomWrite( customCollection ); }
    inline void tamlCustomRead( TamlCallbacks* pCallbacks, const TamlCollection& customCollection ) { pCallbacks->onTamlCustomRead( customCollection ); }

public:
    Taml() :
      mFormatMode(XmlFormat),
      mCompressed(true) {}
    virtual ~Taml() {}

    virtual bool onAdd() { if ( !Parent::onAdd() ) return false; resetCompilation(); return true; }
    virtual void onRemove() { resetCompilation(); Parent::onRemove(); }
    static void initPersistFields();

    /// Format mode.
    inline void setFormatMode( const TamlFormatMode formatMode ) { mFormatMode = formatMode != Taml::InvalidFormat ? formatMode : Taml::XmlFormat; }
    inline TamlFormatMode getFormatMode( void ) const { return mFormatMode; }

    /// Compression.
    inline void setCompressed( const bool compressed ) { mCompressed = compressed; }
    inline bool getCompressed( void ) const { return mCompressed; }

    /// Write.
    bool write( SimObject* pSimObject, const char* pFilename );

    /// Read.
    template<typename T> inline T* read( const char* pFilename )
    {
        SimObject* pSimObject = read( pFilename );
        if ( pSimObject == NULL )
            return NULL;
        T* pObj = dynamic_cast<T*>( pSimObject );
        if ( pObj != NULL )
            return pObj;
        pSimObject->deleteObject();
        return NULL;
    }
    SimObject* read( const char* pFilename );

    /// Declare Console Object.
    DECLARE_CONOBJECT( Taml );
};

//-----------------------------------------------------------------------------

extern Taml::TamlFormatMode getFormatModeEnum(const char* label);
extern const char* getFormatModeDescription(const Taml::TamlFormatMode formatMode);

#endif // _TAML_H_