//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SCENE_WINDOW_H_
#define _SCENE_WINDOW_H_

#ifndef _GUICONTROL_H_
#include "gui/guiControl.h"
#endif

#ifndef _VECTOR_H_
#include "collection/vector.h"
#endif

#ifndef _SCENE_H_
#include "2d/scene/Scene.h"
#endif

#ifndef _VECTOR2_H_
#include "2d/core/Vector2.h"
#endif

#ifndef _UTILITY_H_
#include "2d/core/Utility.h"
#endif

//-----------------------------------------------------------------------------

class SceneWindow : public GuiControl, public virtual Tickable
{
    typedef GuiControl Parent;

public:
    /// Camera View.
    struct CameraView
    {
        // Source.
        RectF           mSourceArea;
        F32             mCameraZoom;

        // Destination.
        RectF           mDestinationArea;
        Point2F         mSceneMin;
        Point2F         mSceneMax;
        Point2F         mSceneWindowScale;
        
        CameraView()
        {
           mSourceArea = RectF(0.0f, 0.0f, 10.0f, 10.0f);
           mCameraZoom = 1.0f;
           mDestinationArea = RectF(0.0f, 0.0f, 10.0f, 10.0f);
           mSceneMin = Point2F(0.0f, 0.0f);
           mSceneMax = Point2F(10.0f, 10.0f);
           mSceneWindowScale = Point2F(1.0f, 1.0f);
        }
    };

private:
    /// Cameras.
    CameraView mCameraCurrent, mCameraSource, mCameraTarget;

    // Camera Interpolation.
    Vector<CameraView>  mCameraQueue;
    S32                 mMaxQueueItems;
    F32                 mCameraTransitionTime;
    F32                 mPreCameraTime;
    F32                 mPostCameraTime;
    F32                 mRenderCameraTime;
    F32                 mCurrentCameraTime;
    bool                mMovingCamera;

    /// Tick Properties.
    Point2F             mPreTickPosition;
    Point2F             mPostTickPosition;

    /// Camera Attachment.
    bool                mCameraMounted;
    SceneObject*        mpMountedTo;
    Vector2             mMountOffset;
    U32                 mMountToID;
    F32                 mMountForce;

    /// View Limit.
    bool                mViewLimitActive;
    Vector2             mViewLimitMin;
    Vector2             mViewLimitMax;
    Vector2             mViewLimitArea;

    /// Camera Shaking.
    bool                mCameraShaking;
    F32                 mShakeLife;
    F32                 mCurrentShake;
    F32                 mShakeRamp;
    Vector2             mCameraShakeOffset;

    /// Misc.
    Scene*              mpScene;
    S32                 mLastRenderTime;
    bool                mLockMouse;
    bool                mWindowDirty;

    // Input Events.
    bool                mUseWindowInputEvents;
    bool                mUseObjectInputEvents;
    U32                 mInputEventGroupMaskFilter;
    U32                 mInputEventLayerMaskFilter;
    bool                mInputEventInvisibleFilter;
    SimSet              mInputEventWatching;
    typeWorldQueryResultVector mInputEventQuery;
    typeSceneObjectVector mInputEventEntering;
    typeSceneObjectVector mInputEventLeaving;

    // Input event names.
    StringTableEntry    mInputEventDownName;
    StringTableEntry    mInputEventUpName;
    StringTableEntry    mInputEventMovedName;
    StringTableEntry    mInputEventDraggedName;
    StringTableEntry    mInputEventEnterName;
    StringTableEntry    mInputEventLeaveName;

    StringTableEntry    mMouseEventRightMouseDownName;
    StringTableEntry    mMouseEventRightMouseUpName;
    StringTableEntry    mMouseEventRightMouseDraggedName;
    StringTableEntry    mMouseEventWheelUpName;
    StringTableEntry    mMouseEventWheelDownName;
    StringTableEntry    mMouseEventEnterName;
    StringTableEntry    mMouseEventLeaveName;


    /// Render Masks.
    U32                 mRenderLayerMask;
    U32                 mRenderGroupMask;

    Resource<GFont>     mpDebugFont;
    char                mDebugText[256];
    ColorF              mDebugBannerForegroundColor;
    ColorF              mDebugBannerBackgroundColor;

    /// Default Font.
    void setDefaultFont( void );

    /// Handling Input Events.
    void dispatchInputEvent( StringTableEntry name, const GuiEvent& event );
    void sendWindowInputEvent( StringTableEntry name, const GuiEvent& event );
    void sendObjectInputEvent( StringTableEntry, const GuiEvent& event );

    inline void calculateCameraView( CameraView* pCameraView );

public:

    /// Camera Interpolation Mode.
    enum CameraInterpolationMode
    {
        LINEAR,             ///< Standard Linear.
        SIGMOID             ///< Slow Start / Slow Stop.

    } mCameraInterpolationMode;

    SceneWindow();
    virtual ~SceneWindow();

    virtual bool onAdd();
    virtual void onRemove();

    /// Initialization.
    virtual void setScene( Scene* pScene );
    virtual void resetScene( void );
    inline void setRenderGroups( const U32 groupMask) { mRenderGroupMask = groupMask; }
    inline void setRenderLayers( const U32 layerMask) { mRenderLayerMask = layerMask; }
    inline void setRenderMasks( const U32 layerMask,const  U32 groupMask ) { mRenderLayerMask = layerMask; mRenderGroupMask = groupMask; }
    inline U32 getRenderLayerMask( void ) { return mRenderLayerMask; }
    inline U32 getRenderGroupMask( void ) { return mRenderGroupMask; }
    void setDebugBanner( const char* pFontname, const U32 fontSize, const ColorF& foregroundColor, const ColorF& backgroundColor );
    inline void setDebugBannerBackgroundColor( const ColorF& backgroundColor ) { mDebugBannerBackgroundColor = backgroundColor; }
    inline void setDebugBannerForegroundColor( const ColorF& foregroundColor ) { mDebugBannerForegroundColor = foregroundColor; }

    /// Get scene.
    inline Scene* getScene( void ) const { return mpScene; };

    /// Mouse.
    void setLockMouse( bool lockStatus ) { mLockMouse = lockStatus; };
    bool getLockMouse( void ) { return mLockMouse; };
    Vector2 getMousePosition( void );
    void setMousePosition( const Vector2& mousePosition );

    /// Input.
    void setObjectInputEventFilter( const U32 groupMask, const U32 layerMask, const bool useInvisible = false );
    void setObjectInputEventGroupFilter( const U32 groupMask );
    void setObjectInputEventLayerFilter( const U32 layerMask );
    void setObjectInputEventInvisibleFilter( const bool useInvisible );
    inline void setUseWindowInputEvents( const bool inputStatus ) { mUseWindowInputEvents = inputStatus; };
    inline void setUseObjectInputEvents( const bool inputStatus ) { mUseObjectInputEvents = inputStatus; };
    inline bool getUseWindowInputEvents( void ) const { return mUseWindowInputEvents; };
    inline bool getUseObjectInputEvents( void ) const { return mUseObjectInputEvents; };
    inline void clearWatchedInputEvents( void ) { mInputEventWatching.clear(); }
    inline void removeFromInputEventPick(SceneObject* pSceneObject ) { mInputEventWatching.removeObject((SimObject*)pSceneObject); }

    /// Coordinate Conversion.
    void windowToScenePoint( const Vector2& srcPoint, Vector2& dstPoint ) const;
    void sceneToWindowPoint( const Vector2& srcPoint, Vector2& dstPoint ) const;

    /// Mounting.
    void mount( SceneObject* pSceneObject, Vector2 mountOffset, F32 mountForce, bool sendToMount );
    void dismount( void );
    void dismountMe( SceneObject* pSceneObject );
    void calculateCameraMount( const F32 elapsedTime );
    void interpolateCameraMount( const F32 timeDelta );

    /// View Limit.
    void setViewLimitOn( const Vector2& limitMin, const Vector2& limitMax );
    inline void setViewLimitOff( void ) { mViewLimitActive = false; };
    inline bool isViewLimitOn( void ) const { return mViewLimitActive; }
    inline Vector2 getViewLimitMin( void ) const { return mViewLimitMin; }
    inline Vector2 getViewLimitMax( void ) const { return mViewLimitMax; }
    inline void clampCurrentCameraViewLimit( void );

    /// Tick Processing.
    void zeroCameraTime( void );
    void resetTickCameraTime( void );
    void updateTickCameraTime( void );
    void resetTickCameraPosition( void );

    virtual void interpolateTick( F32 delta );
    virtual void processTick();
    virtual void advanceTime( F32 timeDelta ) {};

    /// Current Camera,
    virtual void setCurrentCameraArea( const RectF& cameraWindow );
    virtual void setCurrentCameraPosition( Vector2 centerPosition, F32 width, F32 height );
    void setCurrentCameraZoom( F32 zoomFactor );

    /// Target Camera.
    virtual void setTargetCameraArea( const RectF& cameraWindow );
    virtual void setTargetCameraPosition( Vector2 centerPosition, F32 width, F32 height );
    void setTargetCameraZoom( F32 zoomFactor );

    /// Camera Interpolation Time/Mode.
    void setCameraInterpolationTime( F32 interpolationTime );
    void setCameraInterpolationMode( CameraInterpolationMode interpolationMode );

    /// Camera Movement.
    void startCameraMove( F32 interpolationTime );
    void stopCameraMove( void );
    void completeCameraMove( void );
    void undoCameraMove( F32 interpolationTime );
    F32 interpolate( F32 from, F32 to, F32 delta );
    F32 linearInterpolate( F32 from, F32 to, F32 delta );
    F32 sigmoidInterpolate( F32 from, F32 to, F32 delta );
    void updateCamera( void );

    /// Camera Accessors.
    inline Vector2 getCurrentCameraPosition( void ) const               { return mCameraCurrent.mSourceArea.centre(); }
    inline RectF getCurrentCameraArea( void ) const                     { return mCameraCurrent.mSourceArea; }
    inline Vector2 getCurrentCameraRenderPosition( void )               { calculateCameraView( &mCameraCurrent ); return mCameraCurrent.mDestinationArea.centre(); }
    inline RectF getCurrentCameraRenderArea( void )                     { calculateCameraView( &mCameraCurrent ); return mCameraCurrent.mDestinationArea; }
    inline F32 getCameraInterpolationTime( void )                       { return mCameraTransitionTime; }
    inline F32 getCurrentCameraWidth( void ) const                      { return mCameraCurrent.mSourceArea.len_x(); }
    inline F32 getCurrentCameraHeight( void ) const                     { return mCameraCurrent.mSourceArea.len_y(); }
    inline F32 getCurrentCameraZoom( void ) const                       { return mCameraCurrent.mCameraZoom; }
    inline const Vector2 getCurrentCameraWindowScale( void ) const      { return mCameraCurrent.mSceneWindowScale; }
    inline const CameraView& getCurrentCamera(void) const               { return mCameraCurrent; }
    inline const Vector2& getCameraShake(void) const                    { return mCameraShakeOffset; }
    inline bool isCameraMounted( void ) const                           { return mCameraMounted; }
    inline bool isCameraMoving( void ) const                            { return mMovingCamera; }

    /// Camera Shake.
    void startCameraShake( F32 magnitude, F32 time );
    void stopCameraShake( void );

    static void initPersistFields();

    /// GuiControl
    virtual void resize(const Point2I &newPosition, const Point2I &newExtent);
    void onMouseDown( const GuiEvent& event );
    void onMouseUp( const GuiEvent& event );
    void onMouseMove( const GuiEvent& event );
    void onMouseDragged( const GuiEvent& event );
    void onMouseEnter( const GuiEvent& event );
    void onMouseLeave( const GuiEvent& event );
    void onRightMouseDown( const GuiEvent& event );
    void onRightMouseUp( const GuiEvent& event );
    void onRightMouseDragged( const GuiEvent& event );
    bool onMouseWheelDown( const GuiEvent &event );
    bool onMouseWheelUp( const GuiEvent &event );
    virtual void onRender( Point2I offset, const RectI& updateRect );

    /// Declare Console Object.
    DECLARE_CONOBJECT(SceneWindow);

protected:
    static bool writeLockMouse( void* obj, StringTableEntry pFieldName ) { return static_cast<SceneWindow*>(obj)->mLockMouse == true; }
    static bool writeUseWindowInputEvents( void* obj, StringTableEntry pFieldName ) { return static_cast<SceneWindow*>(obj)->mUseWindowInputEvents == false; }
    static bool writeUseObjectInputEvents( void* obj, StringTableEntry pFieldName ) { return static_cast<SceneWindow*>(obj)->mUseObjectInputEvents == true; }
};

#endif // _SCENE_WINDOW_H_
