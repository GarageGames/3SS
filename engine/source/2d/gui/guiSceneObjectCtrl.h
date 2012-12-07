//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _GUI_SCENE_OBJECT_CTRL_H_
#define _GUI_SCENE_OBJECT_CTRL_H_

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

#ifndef _GUICONTROL_H_
#include "gui/guiControl.h"
#endif

#ifndef _GUIBUTTONCTRL_H_
#include "gui/buttons/guiButtonCtrl.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

#ifndef _VECTOR2_H_
#include "2d/core/Vector2.h"
#endif

//-----------------------------------------------------------------------------

class GuiSceneObjectCtrl : public GuiButtonCtrl
{
private:
   typedef GuiButtonCtrl Parent;

protected:
   StringTableEntry                 mSceneObjectName;
   SimObjectPtr<SceneObject>     mSelectedSceneObject;
   S32                              mMargin;
   StringTableEntry                 mCaption;
   bool                             mHasTexture;
   BatchRender                   mBatchRenderer;
   DebugStats                    mDebugStats;
   
public:
    GuiSceneObjectCtrl();
    static void initPersistFields();

    void setSceneObject( const char *name  );
    inline SceneObject* getSceneObject() { return mSelectedSceneObject; };

    void setCaption( const char* caption );

    /// GuiControl
    bool onWake();
    void onSleep();
    void inspectPostApply();
    void onRender(Point2I offset, const RectI &updateRect);

    void onMouseEnter(const GuiEvent &event);
    void onMouseLeave(const GuiEvent &event);
    void onMouseUp(const GuiEvent &event);
    void onMouseDragged(const GuiEvent &event);

    /// Declare Console Object.
    DECLARE_CONOBJECT( GuiSceneObjectCtrl );
};

#endif // _GUI_SCENE_OBJECT_CTRL_H_
