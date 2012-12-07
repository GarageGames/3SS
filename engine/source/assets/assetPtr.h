//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _ASSET_PTR_H_
#define _ASSET_PTR_H_

#ifndef _ASSET_MANAGER_H
#include "assetManager.h"
#endif

//-----------------------------------------------------------------------------

class AssetPtrCallback
{
    friend class AssetManager;

private:
    virtual void onAssetRefreshed( AssetPtrBase* pAssetPtrBase ) = 0;    
};

//-----------------------------------------------------------------------------

class AssetPtrBase
{
public:
    AssetPtrBase() {};
    virtual ~AssetPtrBase()
    {
        // Unregister any notifications.
        unregisterRefreshNotify();
    };

    /// Referencing.
    virtual void clear( void ) = 0;
    virtual void setAssetId( const char* pAssetId ) = 0;
    virtual StringTableEntry getAssetId( void ) const = 0;
    virtual StringTableEntry getAssetType( void ) const = 0;
    virtual bool isAssetId( const char* pAssetId ) const = 0;

    /// Validity.
    virtual bool isNull( void ) const = 0;
    virtual bool notNull( void ) const = 0;

    /// Notification.
    void registerRefreshNotify( AssetPtrCallback* pCallback )
    {
        AssetDatabase.registerAssetPtrRefreshNotify( this, pCallback );
    }

    void unregisterRefreshNotify( void )
    {
        AssetDatabase.unregisterAssetPtrRefreshNotify( this );
    }
};

//-----------------------------------------------------------------------------

template<typename T> class AssetPtr : public AssetPtrBase
{
private:
    SimObjectPtr<T> mpAsset;

public:
    AssetPtr() {}
    AssetPtr( const char* pAssetId )
    {
        // Finish if this is an invalid asset Id.
        if ( pAssetId == NULL || *pAssetId == 0 )
            return;

        // Acquire asset.
        mpAsset = AssetDatabase.acquireAsset<T>( pAssetId );
    }
    AssetPtr( const AssetPtr<T>& assetPtr )
    {
        // Does the asset pointer have an asset?
        if ( assetPtr.notNull() )
        {
            // Yes, so acquire the asset.
            mpAsset = AssetDatabase.acquireAsset<T>( assetPtr->getAssetId() );
        }
    }
    virtual ~AssetPtr()
    {
        // Do we have an asset?
        if ( notNull() )
        {
            // Yes, so release it.
            AssetDatabase.releaseAsset( mpAsset->getAssetId() );
        }
    }

    /// Assignment.
    AssetPtr<T>& operator=( const char* pAssetId )
    {
        // Do we have an asset?
        if ( notNull() )
        {
            // Yes, so finish if the asset Id is already assigned.
            if ( isAssetId( pAssetId ) )
                return *this;

            // No, so release it.
            AssetDatabase.releaseAsset( mpAsset->getAssetId() );
        }

        // Is the asset Id at least okay to attempt to acquire the asset?
        if ( pAssetId != NULL && *pAssetId != 0 )
        {
            // Yes, so acquire the asset.
            mpAsset = AssetDatabase.acquireAsset<T>( pAssetId );
        }
        else
        {
            // No, so remove reference.
            mpAsset = NULL;
        }

        // Return Reference.
        return *this;
    }

    AssetPtr<T>& operator=( const AssetPtr<T>& assetPtr )
    {
        // Set asset pointer.
        *this = assetPtr->getAssetId();

        // Return Reference.
        return *this;
    }

    /// Referencing.
    virtual void clear( void )
    {
        // Do we have an asset?
        if ( notNull() )
        {
            // Yes, so release it.
            AssetDatabase.releaseAsset( mpAsset->getAssetId() );
        }

        // Reset the asset reference.
        mpAsset = NULL;
    }

    T* operator->( void ) const { return mpAsset; }
    T& operator*( void ) const { return *mpAsset; }
    operator T*( void ) const { return mpAsset; }
    virtual void setAssetId( const char* pAssetId ) { *this = pAssetId; }
    virtual StringTableEntry getAssetId( void ) const { return isNull() ? StringTable->EmptyString : mpAsset->getAssetId(); }
    virtual StringTableEntry getAssetType( void ) const { return isNull() ? StringTable->EmptyString : mpAsset->getClassName(); }
    virtual bool isAssetId( const char* pAssetId ) const { return pAssetId == NULL ? isNull() : getAssetId() == StringTable->insert(pAssetId); }

    /// Validity.
    virtual bool isNull( void ) const { return mpAsset.isNull(); }
    virtual bool notNull( void ) const { return !mpAsset.isNull(); }
};

#endif // _ASSET_PTR_H_
