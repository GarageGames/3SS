//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER


#include "editor/levelBuilderTextEditTool.h"
#include "editor/levelBuilderSceneEdit.h"
#include "console/console.h"
#include "graphics/dgl.h"

IMPLEMENT_CONOBJECT( LevelBuilderTextEditTool );

//-----------------------------------------------------------------------------
// Constructor/Destructor
//-----------------------------------------------------------------------------
LevelBuilderTextEditTool::LevelBuilderTextEditTool() :   LevelBuilderBaseEditTool(),
                                                         mSceneWindow( NULL ),
                                                         mTextObject( NULL ),
                                                         mUndoAction( NULL ),
                                                         mAngle(0.0f),
                                                         mAddUndo( false )
{
   mToolName = StringTable->insert("Text Edit Tool");
}

bool LevelBuilderTextEditTool::onAdd()
{
   // Call parent.
   if ( !Parent::onAdd() )
      return false;

   // Register the undo manager.
   if ( !mUndoManager.registerObject() )
      return false;

   mUndoManager.setModDynamicFields( true );
   mUndoManager.setModStaticFields( true );

   return true;
}

void LevelBuilderTextEditTool::onRemove()
{
   // Clean up the undo manager.
   mUndoManager.unregisterObject();

   Parent::onRemove();
}

//-----------------------------------------------------------------------------
// Base Tool Overrides
//-----------------------------------------------------------------------------
bool LevelBuilderTextEditTool::onActivate(LevelBuilderSceneWindow* sceneWindow)
{
   if ( !Parent::onActivate( sceneWindow ) )
      return false;

   mSceneWindow = sceneWindow;
   mTextObject = NULL;

   return true;
}

void LevelBuilderTextEditTool::onDeactivate()
{
   finishEdit();

   mTextObject = NULL;
   mSceneWindow = NULL;

   Parent::onDeactivate();
}

bool LevelBuilderTextEditTool::onAcquireObject( SceneObject* object )
{
   // Make sure the object is editable and we are ready for editing.
   if( !isEditable( object ) || !mSceneWindow )
      return false;

   if( !Parent::onAcquireObject( object ) ) 
      return false;
   
   // Only acquire a new object if one isn't already being edited.
   if ( !mTextObject )
   {
      finishEdit();
      editObject(object);
   }
   
   return true;
}

void LevelBuilderTextEditTool::onRelinquishObject( SceneObject* object )
{
   if( !mSceneWindow || !mTextObject )
   {
      Parent::onRelinquishObject( object );
      return;
   }

   // Only care about the object we are actually editing.
   if ( object == mTextObject )
      finishEdit();

   Parent::onRelinquishObject(object);
}

//-----------------------------------------------------------------------------
// Editing
//-----------------------------------------------------------------------------
void LevelBuilderTextEditTool::editObject( SceneObject* object )
{
   if ( !mSceneWindow || !isEditable( object ) )
      return;

   mAngle = object->getAngle();
   object->setAngle(0.0f);

   // If it passed the isEditable test, we know it's a TextObject.
   mTextObject = static_cast<TextObject*>( object );

   // Copy the object's text to the local buffer.
   mTextBuffer.set( &mTextObject->getText() );

   // Render the cursor.
   mTextObject->showCursor( true );

   mTextObject->setEditing( true );

   // Create the full undo action.
   mUndoAction = new UndoTextEdit();
   mUndoAction->setOldText( mTextObject );
}

void LevelBuilderTextEditTool::cancelEdit()
{
   // For now, this just does the same thing as finishEdit.
   finishEdit();
}

void LevelBuilderTextEditTool::finishEdit()
{
   // Nothing to do if we aren't editing anything.
   if ( !mTextObject || !mSceneWindow )
      return;
   
   // Clear the undo manager.
   mUndoManager.clearAll();

   // Reset Rotation
   mTextObject->setAngle( mAngle );

   // Hide the cursor.
   mTextObject->showCursor( false );

   // Reset the highlight block.
   mTextObject->setHighlightBlock( 0, -1 );

   mTextObject->setEditing( false );

   // Set the undo action.
   if( mUndoAction )
   {
      mUndoAction->setNewText();
      mUndoAction->addToManager( &mSceneWindow->getSceneEdit()->getUndoManager() );
   }

   mTextObject = NULL;
}

bool LevelBuilderTextEditTool::isEditable(SceneObject* obj)
{
   // Can only edit text objects or their derivatives.
   if( dynamic_cast<TextObject*>( obj ) )
      return true;

   return false;
}

//-----------------------------------------------------------------------------
// Mouse Events
//-----------------------------------------------------------------------------
bool LevelBuilderTextEditTool::onMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   if ( mSceneWindow != sceneWindow )
      return false;

   LevelBuilderSceneEdit* mOwner = sceneWindow->getSceneEdit();
   AssertFatal( mOwner, "LevelBuilderTextEditTool::onMouseDragged - Scene Window does not have an owner." );

   mCurrentUndo = NULL;
   mAddUndo = false;

   // Acquire Object
   if (!mTextObject)
   {
      bool editting = false;
      // Search the pick list for a TextObject.
      for( S32 i = 0; i < mouseStatus.pickList.size(); i++ )
      {
          SceneObject* pObj = mouseStatus.pickList[i].mpSceneObject;

         if( isEditable( pObj ) )
         {
            // Found one. Acquire it.
            editting = true;
            sceneWindow->getSceneEdit()->requestAcquisition( pObj );
            mTextObject->setCursorPosition( mouseStatus.mousePoint2D );
            break;
         }
      }

      if( !editting )
      {
         // Could not find a text object at the cursor position. Create a new one.
         char* pos = Con::getArgBuffer( 32 );
         dSprintf( pos, 32, "%f %f", mouseStatus.mousePoint2D.x, mouseStatus.mousePoint2D.y );
         Con::executef( sceneWindow->getSceneEdit(), 2, "createTextObject", pos );
      }

      return true;
   }

   mMouseDownAR = mTextObject->getSize().x / mTextObject->getSize().y;

   // Clicked on the word wrap resizer.
   mSizingState = getSizingState( sceneWindow, mouseStatus.event.mousePoint, mTextObject->getAABBRectangle() );
   if( mSizingState & SizingTop )
      mSizingState &= ~SizingTop;
   
   if( mSizingState & SizingBottom )
      mSizingState &= ~SizingBottom;

   if( mSizingState != SizingNone )
   {
      // Create the undo object.
      mCurrentUndo = new UndoScaleAction( mOwner, "Scale Objects" );
      mCurrentUndo->addObject( mTextObject );
   }

   // Clicked inside the object bounds.
   else
   {
      RectF bounds = mTextObject->getAABBRectangle();
      if( !mTextObject->getClipText() )
      {
         RectF textBounds = mTextObject->getTextBounds();
         bounds = RectF( bounds.point, ( textBounds.point + textBounds.extent ) - bounds.point );
      }

      F32 left = bounds.point.x;
      F32 right = bounds.point.x + bounds.extent.x;
      F32 top = bounds.point.y;
      F32 bottom = bounds.point.y + bounds.extent.y;

      if( ( mouseStatus.mousePoint2D.x > left ) && 
          ( mouseStatus.mousePoint2D.x < right ) && 
          ( mouseStatus.mousePoint2D.y > top ) && 
          ( mouseStatus.mousePoint2D.y < bottom ) )
      {
         // Reset the highlight block.
         mTextObject->setHighlightBlock( mTextObject->getCharacterPosition( mouseStatus.mousePoint2D ), -1 );
      }

      // Clicked outside the object.
      else
         mOwner->setDefaultToolActive();
   }

   return true;
}

bool LevelBuilderTextEditTool::onMouseDragged(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)
{
   if (!mTextObject || (sceneWindow != mSceneWindow))
      return false;

   LevelBuilderSceneEdit* mOwner = sceneWindow->getSceneEdit();
   AssertFatal( mOwner, "LevelBuilderTextEditTool::onMouseDragged - Scene Window does not have an owner." );

   // Resize the object.
   if( mSizingState != SizingNone )
   {
      Vector2 newSize, newPosition;
      bool flipX, flipY;

      // Ctrl scales uniformly.
      bool uniform = false;

      // Pathed objects force uniform scaling.
      if( mTextObject->getAttachedToPath() || mouseStatus.event.modifier & SI_CTRL )
         uniform = true;

      // Shift scales with maintained aspect ratio.
      bool shiftScale = false;

      F32 rotation = mRadToDeg(mTextObject->getAngle());
      if( ( mNotZero( rotation ) &&
            mNotEqual( rotation, 90.0f ) &&
            mNotEqual( rotation, 180.0f ) &&
            mNotEqual( rotation, 270.0f ) &&
            mNotEqual( rotation, 360.0f ) ) ||
            ( mouseStatus.event.modifier & SI_SHIFT ) )
      {
         shiftScale = true;
      }

      scale( mOwner, mTextObject->getSize(), mTextObject->getPosition(), mouseStatus.mousePoint2D,
             uniform, shiftScale, mMouseDownAR, newSize, newPosition, flipX, flipY );

      mAddUndo = true;
      mTextObject->setSize( newSize );
      if( !uniform ) mTextObject->setPosition( newPosition );
      if( flipX ) mTextObject->setFlipX( !mTextObject->getFlipX() );
      if( flipY ) mTextObject->setFlipY( !mTextObject->getFlipY() );

      if( mTextObject->getWordWrap() )
         mTextObject->autoSizeHeight();

      mOwner->onObjectSpatialChanged();
   }

   // Set the new highlight block as determined by the mouse drag rect.
   else
      mTextObject->setHighlightBlock( mTextObject->getCharacterPosition( mouseStatus.dragRect2D.point ), mTextObject->getCharacterPosition( mouseStatus.mousePoint2D ) );

   return true;
}

bool LevelBuilderTextEditTool::onMouseUp(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)
{
   if (!mTextObject || (sceneWindow != mSceneWindow))
      return false;

   if( ( mAddUndo ) && ( mCurrentUndo ) )
   {
      mCurrentUndo->setNewSize( mTextObject );
      mCurrentUndo->addToManager( &mUndoManager );
      mCurrentUndo = NULL;
   }
   else if( mCurrentUndo )
   {
      delete mCurrentUndo;
      mCurrentUndo = NULL;
   }

   return true;
}

//-----------------------------------------------------------------------------
// Key Events
//-----------------------------------------------------------------------------
bool LevelBuilderTextEditTool::onKeyDown(LevelBuilderSceneWindow* sceneWindow, const GuiEvent &event)
{
   if (!mTextObject || (sceneWindow != mSceneWindow))
      return false;

   // Make sure the text object is valid for rendering and editing.
   if( !mTextObject->hasFont() )
      return false;

   // Grab the font from the object.
   GFont* font = mTextObject->getFont().font;

   // Modifier keys.
   if( ( event.modifier & SI_CTRL ) && ( event.modifier & SI_SHIFT ) )
   {
   }

   else if( event.modifier & SI_SHIFT )
   {

      switch( event.keyCode )
      {
      // Cut.
      case KEY_DELETE:
         break;

      // Paste.
      case KEY_INSERT:
         break;
      }
   }

   else if( event.modifier & SI_CTRL )
   {
      switch( event.keyCode )
      {
      // Select all.
      case KEY_A:
         mTextObject->setHighlightBlock( 0, mTextObject->getText().length() );
         break;

      // Undo.
      case KEY_Z:
         mUndoManager.undo();
         break;

      // Redo.
      case KEY_Y:
         mUndoManager.redo();
         break;

      // Cut.
      case KEY_X:
         break;

      // Copy.
      case KEY_INSERT:
      case KEY_C:
         break;

      // Paste.
      case KEY_V:
         break;

      // Move to the beginning of the text string.
      case KEY_HOME:
         mTextObject->setCursorPosition( 0 );
         break;

      // Move to the end of the text string.
      case KEY_END:
         mTextObject->setCursorPosition( mTextObject->getText().length() );
         break;
      }

      // Just eat the rest of these.
      return true;
   }

   else if( event.modifier & SI_ALT )
   {
      // Nothing doing here yet.
      return true;
   }

   // conv will hold the character to insert into the text string.
   UTF16 conv[2] = { 0, 0 };
   if( font->isValidChar( event.ascii ) )
      conv[0] = event.ascii;

   switch( event.keyCode )
   {
      // Inserts a line break.
      case KEY_RETURN:
      case KEY_NUMPADENTER:
         conv[0] = '\n';
         break;

      // Print nothing for escape, backspace, and delete.
      case KEY_DELETE:
         conv[0] = 0;
         mTextObject->deleteCharacter();
         if( mTextObject->getWordWrap() )
            mTextObject->autoSizeHeight();

         break;

      case KEY_BACKSPACE:
         conv[0] = 0;
         mTextObject->backspace();
         if( mTextObject->getWordWrap() )
            mTextObject->autoSizeHeight();

         break;

      case KEY_ESCAPE:
         conv[0] = 0;
         break;

      // Moves the cursor left.
      case KEY_LEFT:
         conv[0] = 0;
         mTextObject->showCursorAtEndOfLine( false );
         mTextObject->setCursorPosition( mTextObject->getCursorPosition() - 1 );
         break;

      // Moves the cursor right.
      case KEY_RIGHT:
         conv[0] = 0;
         mTextObject->showCursorAtEndOfLine( true );
         mTextObject->setCursorPosition( mTextObject->getCursorPosition() + 1 );
         break;

      // Moves the cursor up a line.
      case KEY_UP:
         conv[0] = 0;
         {
            RectF charBounds = mTextObject->getCharacterBounds( mTextObject->getCursorPosition() );
            Vector2 cursorPos = charBounds.point;
            cursorPos.y -= mTextObject->getLineHeight() * 0.5f;
            mTextObject->setCursorPosition( cursorPos );
         }
         break;

      // Moves the cursor down a line.
      case KEY_DOWN:
         conv[0] = 0;
         {
            RectF charBounds = mTextObject->getCharacterBounds( mTextObject->getCursorPosition() );
            Vector2 cursorPos = charBounds.point;
            cursorPos.y += mTextObject->getLineHeight() * 1.5f;
            mTextObject->setCursorPosition( cursorPos );
         }
         break;

      // Moves the cursor to the beginning of the line it's on.
      case KEY_HOME:
         conv[0] = 0;
         {
            S32 offset = mTextObject->getShowCursorAtEndOfLine() ? -1 : 0;
            S32 index = mTextObject->getLineIndex( mTextObject->getLineNumber( mTextObject->getCursorPosition() + offset ) );

            mTextObject->setCursorPosition( index );
            mTextObject->showCursorAtEndOfLine( false );
         }
         break;

      // Moves the cursor to the end of the line it's on.
      case KEY_END:
         conv[0] = 0;
         {
            S32 offset = mTextObject->getShowCursorAtEndOfLine() ? -1 : 0;
            S32 index = mTextObject->getLineIndex( mTextObject->getLineNumber( mTextObject->getCursorPosition() + offset ) + 1 );

            if( index == -1 )
               index = mTextObject->getText().length();

            mTextObject->setCursorPosition( index );
            mTextObject->showCursorAtEndOfLine( true );
         }
         break;
   }

   // If we got a character...
   if( *conv )
   {
      // Create an undo action for the insert.
      UndoTextEdit* undo = new UndoTextEdit();
      undo->setOldText( mTextObject );

      // Delete any highlighted text.
      mTextObject->deleteHighlighted();
      // Insert the new text.
      mTextObject->insertText( conv );

      // Push the undo action.
      undo->setNewText();
      undo->addToManager( &mUndoManager );

      if( mTextObject->getWordWrap() )
         mTextObject->autoSizeHeight();

   }

   // Update the level builder.
   sceneWindow->getSceneEdit()->onObjectSpatialChanged( mTextObject );

   return true;
}

bool LevelBuilderTextEditTool::onKeyRepeat( LevelBuilderSceneWindow* sceneWindow, const GuiEvent& event )
{
   // Just act like the key was pressed again.
   return onKeyDown( sceneWindow, event );
}

bool LevelBuilderTextEditTool::onKeyUp( LevelBuilderSceneWindow* sceneWindow, const GuiEvent& event )
{
   switch( event.keyCode )
   {
   // Pass escape to the parent.
   case KEY_ESCAPE:
      return false;
   }

   // Eat all other onKeyUp messages.
   return true;
}

void LevelBuilderTextEditTool::onRenderScene( LevelBuilderSceneWindow* sceneWindow )
{
   Parent::onRenderScene( sceneWindow );

   if (!mTextObject || (sceneWindow != mSceneWindow))
      return;

   // Render a bounding box if word wrapping is enabled.
   if( !mTextObject->getWordWrap() )
      return;

   RectI objRect = sceneWindow->getObjectBoundsWindow( mTextObject );
   objRect.point = sceneWindow->localToGlobalCoord( objRect.point );
   ColorI lineColor = ColorI( 205, 205, 205, 255 );

   // D3D does not support line stippling so this will show as a solid line in
   // D3D mode.
   glEnable( GL_LINE_STIPPLE );
   glEnable( GL_LINE_SMOOTH );
   glLineWidth( 2.0f );
   glLineStipple( 6, 0xAAAA );
   dglDrawRect( objRect, lineColor );
   glLineWidth( 1.0f );
   glDisable( GL_LINE_SMOOTH );
   glDisable( GL_LINE_STIPPLE );

   

   drawWordWrapSizingNuts( sceneWindow, mTextObject->getAABBRectangle() );

}

void LevelBuilderTextEditTool::drawWordWrapSizingNuts(LevelBuilderSceneWindow* sceneWindow, const RectF& rect)
{
   Vector2 upperLeft = Vector2(rect.point);
   Vector2 lowerRight = Vector2(rect.point + rect.extent);

   // Convert to window coords.
   Vector2 windowUpperLeft, windowLowerRight;
   sceneWindow->sceneToWindowPoint(upperLeft, windowUpperLeft);
   sceneWindow->sceneToWindowPoint(lowerRight, windowLowerRight);
   windowUpperLeft = sceneWindow->localToGlobalCoord(Point2I(S32(windowUpperLeft.x), S32(windowUpperLeft.y)));
   windowLowerRight = sceneWindow->localToGlobalCoord(Point2I(S32(windowLowerRight.x), S32(windowLowerRight.y)));

   RectI selectionRect = RectI(S32(windowUpperLeft.x), S32(windowUpperLeft.y),
                               S32(windowLowerRight.x - windowUpperLeft.x),
                               S32(windowLowerRight.y - windowUpperLeft.y));

   // Middle Left Sizing Knob
   drawArrowNut( Point2I( selectionRect.point.x, selectionRect.point.y  ));
   // Middle Right Sizing Knob
   drawArrowNut( Point2I( selectionRect.point.x + selectionRect.extent.x , selectionRect.point.y  ));
}

//-----------------------------------------------------------------------------
// Console Methods
//-----------------------------------------------------------------------------
ConsoleMethod( LevelBuilderTextEditTool, editObject, void, 3, 3, "( TextObject object ) Selects an object for editing.\n"
              "@param object The object to edit." )
{
   SceneObject* obj = dynamic_cast<SceneObject*>( Sim::findObject( argv[2] ) );
   if( obj )
      object->editObject( obj );
   else
      Con::warnf( "LevelBuilderTextEditTool::editObject - Object %s is not a SceneObject.", argv[2] );
}

ConsoleMethod( LevelBuilderTextEditTool, finishEdit, void, 2, 2, "() Applies changes and ends editing of an object." )
{
   object->finishEdit();
}

ConsoleMethod( LevelBuilderTextEditTool, cancelEdit, void, 2, 2, "() Cancels editing of an object." )
{
   object->cancelEdit();
}

#endif // TORQUE_TOOLS
