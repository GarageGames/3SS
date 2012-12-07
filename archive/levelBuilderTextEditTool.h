//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERTEXTEDITTOOL_H_
#define _LEVELBUILDERTEXTEDITTOOL_H_

#include "editor/levelBuilderBaseEditTool.h"
#include "editor/levelBuilderSelectionTool.h"
#include "2d/TextObject.h"
#include "string/stringBuffer.h"
#include "collection/undo.h"

class UndoTextEdit;

//-----------------------------------------------------------------------------
/// This class implements a TGB level builder tool for editing t2dTextObjects.
/// Currently the tool supports moving the cursor position via clicking and
/// arrow keys, highlighting text via click and drag, and inserting and
/// deleting text. The tool has it's own undo manager for handling incremental
/// changes to the object's text and an undo action that handles the text edit
/// across the whole editing session.
/// 
/// Future Additions:
/// Support for common text editing shortcuts. Like, shift + left highlights
/// the next character to the left.
//-----------------------------------------------------------------------------
class LevelBuilderTextEditTool : public LevelBuilderBaseEditTool
{
   typedef LevelBuilderBaseTool Parent;
   F32   mAngle;
public:
   DECLARE_CONOBJECT(LevelBuilderTextEditTool);

   LevelBuilderTextEditTool();
   
   /// @name SimObject_Overrides
   /// @{
   virtual bool onAdd();
   virtual void onRemove();
   /// @}

   /// @name Base_Tool_Overrides
   /// @{
   virtual bool onActivate(LevelBuilderSceneWindow* sceneWindow);
   virtual void onDeactivate();
   virtual bool onAcquireObject( SceneObject* object );
   virtual void onRelinquishObject( SceneObject* object );

   virtual void onRenderScene( LevelBuilderSceneWindow* sceneWindow );
   /// @}

   /// @name Object_Editing
   /// @{

   /// Begin editing a scene object.
   /// 
   /// @param object The object to edit.
   void editObject( SceneObject* object );

   /// This cancels an edit, not applying any changes.
   void cancelEdit();

   /// This cancels an edit, applying changes.
   void finishEdit();
   /// @}

   /// @name Input_Events
   /// @{

   /// If no text object is selected for editing, a text object is looked for
   /// in the mouse pick list. If one is found, that object is edited,
   /// otherwise a new text object is created and selected for editing. If an
   /// object is selected for editing, the cursor position is set to the
   /// location of the mouse.
   /// 
   /// @param sceneWindow The scene window the event was passed from.
   /// @param mouseStatus Information about the current state of the mouse.
   /// @return A value of 'true' indicates that the event was used and should
   /// not be passed along to any other objects.
   virtual bool onMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );

   /// Sets the highlight block of the text object being edited from the
   /// position the mouse was pressed to the current position of the mouse.
   /// 
   /// @param sceneWindow The scene window the event was passed from.
   /// @param mouseStatus Information about the current state of the mouse.
   /// @return A value of 'true' indicates that the event was used and should
   /// not be passed along to any other objects.
   virtual bool onMouseDragged( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );

   /// Called when the mouse is released.
   /// 
   /// @param sceneWindow The scene window the event was passed from.
   /// @param mouseStatus Information about the current state of the mouse.
   /// @return A value of 'true' indicates that the event was used and should
   /// not be passed along to any other objects.
   virtual bool onMouseUp( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
   
   /// Performs an action on the text object being edited depending on the key
   /// that was pressed. Currently implemented keys are:
   /// Left, Right, Home, and End - Moves the cursor position.
   /// Any Renderable Character - Inserts the character at the cursor position.
   /// Delete and Backspace - Erases the character after or before the cursor position.
   /// 
   /// @param sceneWindow The scene window the event was passed from.
   /// @param event The event object.
   /// @return A value of 'true' indicates that the event was used and should
   /// not be passed along to any other objects.
   virtual bool onKeyDown(LevelBuilderSceneWindow* sceneWindow, const GuiEvent &event);

   /// This function simply calls onKeyDown.
   /// 
   /// @param sceneWindow The scene window the event was passed from.
   /// @param event The event object.
   /// @return A value of 'true' indicates that the event was used and should
   /// not be passed along to any other objects.
   virtual bool onKeyRepeat(LevelBuilderSceneWindow* sceneWindow, const GuiEvent &event);

   /// This function does nothing but eat events that should not be passed
   /// along to any other objects.
   /// @param sceneWindow The scene window the event was passed from.
   /// @param event The event object.
   /// @return A value of 'true' indicates that the event was used and should
   /// not be passed along to any other objects.
   virtual bool onKeyUp(LevelBuilderSceneWindow* sceneWindow, const GuiEvent &event);
   /// @}

   /// @name Undo_Management
   /// @{

   /// This tool uses its own undo manager for incremental edits.
   virtual bool hasUndoManager() { return true; };

   /// Undo the last incremental edit.
   virtual bool undo() { mUndoManager.undo(); return true; };

   /// Redo the last undone incremental edit.
   virtual bool redo() { mUndoManager.redo(); return true; };
   /// @}

protected:
   /// The scene window using this tool.
   LevelBuilderSceneWindow* mSceneWindow;

   /// The text object being edited.
   TextObject*           mTextObject;
   
   /// Determines whether or not an object is editable by this tool.
   /// 
   /// @param obj The object to check.
   /// @return Whether or not the object can be edited.
   bool isEditable( SceneObject* obj );

private:
   /// Draws the sizing nobs - but only those used for word wrap resizing.
   void drawWordWrapSizingNuts(LevelBuilderSceneWindow* sceneWindow, const RectF& rect);

   /// Storage for the text that will be set on the text object.
   StringBuffer mTextBuffer;

   /// The undo action used for the entire editing session. 
   UndoTextEdit* mUndoAction;

   /// Sizing undo action.
   UndoScaleAction* mCurrentUndo;

   /// Whether or not to add the sizing undo.
   bool mAddUndo;

   /// The aspect ratio of the text object when the mouse was pressed.
   F32 mMouseDownAR;
};

//-----------------------------------------------------------------------------
/// Undoes a change in the text string of a TextObject.
/// 
/// Usage:
/// Call setOldText, passing it a text object, to set the value of the text
/// prior to any edit. Call setNewText after the edit has been made.
//-----------------------------------------------------------------------------
class UndoTextEdit : public UndoAction
{
   typedef UndoAction Parent;

private:
   TextObject* mTextObject;
   StringBuffer mOldText;
   StringBuffer mNewText;

public:
   UndoTextEdit() : UndoAction( "Text Changed" ) { mTextObject = NULL; };
   void setOldText( TextObject* textObject ) { mTextObject = textObject; mOldText.set( &mTextObject->getText() ); };
   void setNewText() { mNewText.set( &mTextObject->getText() ); };
   virtual void undo() { mTextObject->setText( mOldText ); };
   virtual void redo() { mTextObject->setText( mNewText ); };
};

#endif

#endif // TORQUE_TOOLS
