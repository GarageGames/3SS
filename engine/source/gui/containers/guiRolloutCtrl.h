//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
#ifndef _GUI_ROLLOUTCTRL_H_
#define _GUI_ROLLOUTCTRL_H_

#ifndef _GUICONTROL_H_
#include "gui/guiControl.h"
#endif

#ifndef _GUISTACKCTRL_H_
#include "gui/containers/guiStackCtrl.h"
#endif

#ifndef _H_GUIDEFAULTCONTROLRENDER_
#include "gui/guiDefaultControlRender.h"
#endif

#ifndef _GUITICKCTRL_H_
#include "gui/guiTickCtrl.h"
#endif

class GuiRolloutCtrl : public GuiTickCtrl
{
private:
   typedef GuiControl Parent;
public:
   // Members
   StringTableEntry       mCaption;
   RectI                  mHeader;
   RectI                  mExpanded;
   RectI                  mChildRect;
   Point2I                mMargin;
   bool                   mIsExpanded;
   bool                   mIsAnimating;
   bool                   mCollapsing;
   S32                    mAnimateDestHeight;
   S32                    mAnimateStep;
   S32                    mDefaultHeight;
   bool                   mCanCollapse;

   GuiCursor*  mDefaultCursor;
   GuiCursor*  mVertSizingCursor;


   // Theme Support
   enum
   {
      CollapsedLeft = 0,
      CollapsedCenter,
      CollapsedRight,
      TopLeftHeader,
      TopMidHeader,      
      TopRightHeader,  
      MidPageLeft,
      MidPageCenter,
      MidPageRight,
      BottomLeftHeader, 
      BottomMidHeader,   
      BottomRightHeader,   
      NumBitmaps           ///< Number of bitmaps in this array
   };
   bool  mHasTexture;   ///< Indicates whether we have a texture to render the tabs with
   RectI *mBitmapBounds;///< Array of rectangles identifying textures for tab book

   // Constructor/Destructor/Conobject Declaration
   GuiRolloutCtrl();
   ~GuiRolloutCtrl();
   DECLARE_CONOBJECT(GuiRolloutCtrl);

   // Persistence
   static void initPersistFields();

   // Control Events
   bool onWake();
   void addObject(SimObject *obj);
   void removeObject(SimObject *obj);
   virtual void childResized(GuiControl *child);

   // Mouse Events
   virtual void onMouseDown( const GuiEvent &event );
   virtual void onMouseUp( const GuiEvent &event );

   // Sizing Helpers
   virtual void calculateHeights();
   void resize( const Point2I &newPosition, const Point2I &newExtent );
   virtual void sizeToContents();
   inline bool isExpanded(){ return mIsExpanded; };

   // Sizing Animation Functions
   void animateTo( S32 height );
   virtual void processTick();


   void collapse() { animateTo( mHeader.extent.y ); };
   void expand() { animateTo( mExpanded.extent.y ); };
   void instantCollapse();
   void instantExpand();

   // Property - "Collapsed"
   static bool setCollapsed(void* obj, const char* data)  
   { 
      bool bCollapsed = dAtob( data );
      if( bCollapsed )
         static_cast<GuiRolloutCtrl*>(obj)->instantCollapse();
      else
         static_cast<GuiRolloutCtrl*>(obj)->instantExpand();
      return false; 
   };


   // Control Rendering
   virtual void onRender(Point2I offset, const RectI &updateRect);
   bool onAdd();

};
#endif