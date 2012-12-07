//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER


#include "console/console.h"
#include "editor/levelBuilderTextObjectTool.h"

// Implement Our Console Object
IMPLEMENT_CONOBJECT(LevelBuilderTextObjectTool);

LevelBuilderTextObjectTool::LevelBuilderTextObjectTool() : LevelBuilderCreateTool(),
                                                           mDefaultFont( StringTable->insert( "Arial" ) ),
                                                           mDefaultSize( 96 ),
                                                           mDefaultHeight( 10 ),
                                                           mDefaultAlignment( TextObject::CENTER ),
                                                           mDefaultWordWrap( true ),
                                                           mDefaultClipText( true ),
                                                           mDefaultAutoSize( true ),
                                                           mDefaultAspectRatio( 1.0f ),
                                                           mDefaultLineSpacing( 0.0f ),
                                                           mDefaultCharacterSpacing( 0.0f )
{
   // Set our tool name
   mToolName = StringTable->insert("Text Object Tool");
}

LevelBuilderTextObjectTool::~LevelBuilderTextObjectTool()
{
}

SceneObject* LevelBuilderTextObjectTool::createObject()
{
   TextObject* textObject = dynamic_cast<TextObject*>(ConsoleObject::create("TextObject"));
   // So no funny resizing happens
   textObject->setEditing( true );
   return textObject;
}

ConsoleMethod(LevelBuilderTextObjectTool, setFontDB, void, 3, 3, "Sets font Datablock for the Text Object.")
{
}

void LevelBuilderTextObjectTool::showObject()
{
   mCreatedObject->setVisible(true);
}

Vector2 LevelBuilderTextObjectTool::getDefaultSize(LevelBuilderSceneWindow* sceneWindow)
{
   return Vector2( 1.0f, static_cast<TextObject*>( mCreatedObject )->getLineHeight() );
}

void LevelBuilderTextObjectTool::onObjectCreated()
{
   TextObject* textObject = dynamic_cast<TextObject*>( mCreatedObject );
   AssertFatal( textObject, "LevelBuilderTextObjectTool::onObjectCreated - Created object is not a text object." );

   textObject->setEditing( false );

   Parent::onObjectCreated();
}

ConsoleFunction( enumerateFonts, const char*, 1, 1, "() Retrieves a list of all fonts on the system.\n"
              "@return A tab delimited list of the fonts." )
{
   Vector<StringTableEntry> fonts;
   PlatformFont::enumeratePlatformFonts( fonts );

   if( fonts.empty() )
      return "";

   S32 bufferSize = 0;
   for( Vector<StringTableEntry>::const_iterator iter = fonts.begin(); iter != fonts.end(); iter++ )
      bufferSize += dStrlen( *iter ) + 1;

   char* fontList = Con::getReturnBuffer( bufferSize );
   dStrcpy( fontList, fonts[0] );
   for( Vector<StringTableEntry>::const_iterator iter = fonts.begin() + 1; iter != fonts.end(); iter++ )
   {
      dStrcat( fontList, "\t" );
      dStrcat( fontList, *iter );
   }

   //S32 length = dStrlen( fontList );

   return fontList;
}

#endif // TORQUE_TOOLS
