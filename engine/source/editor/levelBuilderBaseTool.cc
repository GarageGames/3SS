//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER


#include "console/console.h"
#include "console/consoleInternal.h"
#include "graphics/dgl.h"
#include "editor/levelBuilderBaseTool.h"

IMPLEMENT_CONOBJECT(LevelBuilderBaseTool);

LevelBuilderBaseTool::LevelBuilderBaseTool() : SimObject(),
                                               mActive(false),
                                               mNutColor(0, 0, 255, 200),
                                               mNutOutlineColor(255, 255, 255),
                                               mNutSize(3)
{
   VECTOR_SET_ASSOCIATION(mNutList);
   mToolName = StringTable->insert("Base Tool");
}

LevelBuilderBaseTool::~LevelBuilderBaseTool()
{
}

ConsoleMethod( LevelBuilderBaseTool, getToolName, const char*, 2, 2, "() \n"
			  "@return Returns the name of this tool")
{
   return object->getToolName();
}

ConsoleMethod( LevelBuilderBaseTool, setToolName, void, 3, 3, "(string toolName) Set the name associated with this tool"
			  "@return No return value"
			  "@note %tool.setToolName(string toolName)")
{
   object->setToolName( argv[2] );
}

ConsoleMethod( LevelBuilderBaseTool, getToolTexture, const char*, 2, 2, "() \n@return Returns this tools associated texture"
			  "@note %tool.getToolTexture()")
{
   return object->getToolTexture();
}

ConsoleMethod( LevelBuilderBaseTool, setToolTexture, void, 3, 3, "(fileName) Set the texture associated with this tool"
			  "@param fileName The path rleative file name of the texture."
			  "@return No return value."
			  "@note %tool.setToolTexture(string - NO PATH - filename)")
{
   object->setToolTexture( argv[2] );
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


bool LevelBuilderBaseTool::onAdd()
{
   if( !Parent::onAdd() )
      return false;

   StringTableEntry objectName = getName();
   if( objectName && objectName[0] && getClassRep() )
   {    
      Namespace *parent = getClassRep()->getNameSpace();
      if( Con::linkNamespaces( parent->mName, objectName ) )
         mNameSpace = Con::lookupNamespace( objectName );
   }

   return true;
}

bool LevelBuilderBaseTool::onActivate(LevelBuilderSceneWindow* sceneWindow)
{
   if (mActive)
      return false;
   
   if (!sceneWindow || !sceneWindow->getSceneEdit())
      return false;

   // We're an active tool now, let's keep track.
   mActive = true;
   return true;
}

void LevelBuilderBaseTool::onDeactivate()
{
   // We're not active anymore.
   mActive = false;
}

bool LevelBuilderBaseTool::onAcquireObject(SceneObject* object)
{
   // Can't acquire objects if we're not active.
   if (!mActive)
      return false;

   // Ok, we'll take it.
   return true;
}          

void LevelBuilderBaseTool::onRelinquishObject(SceneObject* object)
{
}

void LevelBuilderBaseTool::drawNut(Point2I position)
{
   RectI r(position.x - mNutSize, position.y - mNutSize, 2 * mNutSize + 1, 2 * mNutSize + 1);
   r.inset(1, 1);
   dglDrawRectFill(r, mNutColor);
   r.inset(-1, -1);
   dglDrawRect(r, mNutOutlineColor);
}

void LevelBuilderBaseTool::drawArrowNut(Point2I position)
{
   RectF r( (F32)(position.x - (F32)mNutSize), (F32)(position.y - (F32)mNutSize), (F32)(2.0f * mNutSize), (F32)(2.0f * mNutSize) );

   glEnable(GL_BLEND);
   glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
   glDisable(GL_TEXTURE_2D);

   glColor4ubv( &mNutColor.red );

   glBegin( GL_TRIANGLE_STRIP );
      glVertex2f( r.point.x + 0.5f, r.point.y + 0.5f );
      glVertex2f( r.point.x + r.extent.x + 0.5f, r.point.y + 0.5f );
      glVertex2f( r.point.x + 0.5f, r.point.y + r.extent.y + 0.5f);
      glVertex2f( r.point.x + r.extent.x+ 0.5f, r.point.y + r.extent.y + 0.5f );
      glVertex2f( r.point.x + r.extent.x - ( r.extent.x / 2.0f ) + 0.5f, r.point.y + r.extent.y + ( r.extent.y / 2.0f ) + 0.5f );
   glEnd();

   glColor4ubv( &mNutOutlineColor.red );

   glBegin( GL_LINE_STRIP );
      glVertex2f( r.point.x + 0.5f, r.point.y + 0.5f);
      glVertex2f( r.point.x + r.extent.x + 0.5f, r.point.y + 0.5f);
      glVertex2f( r.point.x + r.extent.x + 0.5f, r.point.y + r.extent.y + 0.5f );
      glVertex2f( r.point.x + r.extent.x - ( r.extent.x / 2.0f ) + 0.5f, r.point.y + r.extent.y + ( r.extent.y / 2.0f ) + 0.5f );
      glVertex2f( r.point.x + 0.5f, r.point.y + r.extent.y + 0.5f );
      glVertex2f( r.point.x + 0.5f, r.point.y + 0.5f );
   glEnd();
}

bool LevelBuilderBaseTool::inNut(Point2I pt, S32 x, S32 y)
{
   S32 checkSize = mNutSize << 1;
   S32 dx = pt.x - x;
   S32 dy = pt.y - y;
   return dx <= checkSize && dx >= -checkSize && dy <= checkSize && dy >= -checkSize;
}

#endif // TORQUE_TOOLS
