//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "gui/guiBubbleTextCtrl.h"
#include "gui/guiCanvas.h"

IMPLEMENT_CONOBJECT(GuiBubbleTextCtrl);

//------------------------------------------------------------------------------
void GuiBubbleTextCtrl::popBubble()
{
	// Release the mouse:
	mInAction = false;
	mouseUnlock();

	// Pop the dialog
	getRoot()->popDialogControl(mDlg);

	// Kill the popup
	mDlg->removeObject(mPopup);
	mPopup->removeObject(mMLText);
	mMLText->deleteObject();
	mPopup->deleteObject();
	mDlg->deleteObject();
}

//------------------------------------------------------------------------------
void GuiBubbleTextCtrl::onMouseDown(const GuiEvent &event)
{
	if (mInAction)
	{
		popBubble();

		return;
	}

	mDlg = new GuiControl();
   AssertFatal(mDlg, "Failed to create the GuiControl for the BubbleTextCtrl");
   mDlg->setField("profile","GuiModelessDialogProfile");
   mDlg->setField("horizSizing", "width");
	mDlg->setField("vertSizing", "height");
	//MIN_RESOLUTION defined in platformWin32/platformGL.h
	mDlg->setField("extent", MIN_RESOLUTION_XY_STRING);

   mPopup = new GuiControl();
   AssertFatal(mPopup, "Failed to create the GuiControl for the BubbleTextCtrl");
   mPopup->setField("profile","GuiBubblePopupProfile");

   mMLText = new GuiMLTextCtrl();
   AssertFatal(mMLText, "Failed to create the GuiMLTextCtrl for the BubbleTextCtrl");
	mMLText->setField("profile","GuiBubbleTextProfile");
	mMLText->setField("position", "2 2");
	mMLText->setField("extent", "296 51");	
	
	mMLText->setText((char*)mText,dStrlen(mText));

	mMLText->registerObject();
	mPopup->registerObject();
	mDlg->registerObject();

	mPopup->addObject(mMLText);
	mDlg->addObject(mPopup);

	mPopup->resize(event.mousePoint,Point2I(300,55));

	getRoot()->pushDialogControl(mDlg,0);
	mouseLock();

	mInAction = true;
}
