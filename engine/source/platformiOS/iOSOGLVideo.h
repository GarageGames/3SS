//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _iOSOGLVIDEO_H_
#define _iOSOGLVIDEO_H_

#import "UIKit/UIKit.h"
#import "UIKit/UIAccelerometer.h"

#include "platform/platformVideo.h"

class OpenGLDevice : public DisplayDevice
{
private:

   /// Gamma value
   F32 mGamma;

   /// Cleans up the opengl context, and destroys the rendering window
   bool cleanupContextAndWindow();

public:
   OpenGLDevice();
   static DisplayDevice* create();

   /// The following are inherited from DisplayDevice
   void initDevice();
   bool activate( U32 width, U32 height, U32 bpp, bool fullScreen );
   
   void shutdown();
   
   bool setScreenMode( U32 width, U32 height, U32 bpp, bool fullScreen, bool forceIt = false, bool repaint = true );
   void swapBuffers();
   
   const char* getDriverInfo();
   bool getGammaCorrection(F32 &g);
   bool setGammaCorrection(F32 g);
   bool setVerticalSync( bool on );
};








#include "platformiOS/platformGL.h"



//struct EAGLContext;
/*
 This class wraps the CAEAGLLayer from CoreAnimation into a convenient UIView subclass.
 The view content is basically an EAGL surface you render your OpenGL scene into.
 Note that setting the view non-opaque will only work if the EAGL surface has an alpha channel.
 */
@interface iOSOGLVideo : UIView <UIAccelerometerDelegate> {
	
@private
	/* The pixel dimensions of the backbuffer */
	GLint backingWidth;
	GLint backingHeight;
	
	EAGLContext *context;
	
	/* OpenGL names for the renderbuffer and framebuffers used to render to this view */
	GLuint viewRenderbuffer, viewFramebuffer;
	
	/* OpenGL name for the depth buffer that is attached to viewFramebuffer, if it exists (0 if it does not exist) */
	GLuint depthRenderbuffer;

	bool isLayedOut;
	CGFloat currentAngle;//for knowing our current oriantion
}

- (void)swapBuffers;
- (void)rotateByAngle:(CGFloat)angle;//rotate BY a certain degree
- (void)rotateToAngle:(CGFloat)angle;//rotate TO a certain degree
- (void)centerOnPoint:(CGPoint)point;//set the center position

@end







#endif // _iOSOGLVIDEO_H_
