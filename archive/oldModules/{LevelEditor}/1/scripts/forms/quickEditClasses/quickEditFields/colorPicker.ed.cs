//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createColorPicker(%this, %accessor, %label, %tooltip)
{
   %container = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "300 130";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
   };

   %labelControl = new GuiTextCtrl() {
      canSaveDynamicFields = "0";
      Profile = "EditorFontHLBold";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "16 8";
      Extent = "128 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      text = %label;
      maxLength = "1024";
   };
   %container.add(%labelControl);

   %rLabel = new GuiTextCtrl() {
      canSaveDynamicFields = "0";
      Profile = "EditorFontHL";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "20 32";
      Extent = "128 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      text = "Red";
      maxLength = "1024";
   };
   %container.add(%rLabel);

   %gLabel = new GuiTextCtrl() {
      canSaveDynamicFields = "0";
      Profile = "EditorFontHL";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "20 56";
      Extent = "128 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      text = "Green";
      maxLength = "1024";
   };
   %container.add(%gLabel);

   %bLabel = new GuiTextCtrl() {
      canSaveDynamicFields = "0";
      Profile = "EditorFontHL";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "20 80";
      Extent = "128 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      text = "Blue";
      maxLength = "1024";
   };
   %container.add(%bLabel);

   %aLabel = new GuiTextCtrl() {
      canSaveDynamicFields = "0";
      Profile = "EditorFontHL";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "20 104";
      Extent = "128 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      text = "Alpha";
      maxLength = "1024";
   };
   %container.add(%aLabel);
   
   %r = new GuiTextEditCtrl() {
      class = QuickEditColorTextEdit;
      index = 0;
      canSaveDynamicFields = "0";
      Profile = "EditorTextEdit";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "64 32";
      Extent = "48 20";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      maxLength = "1024";
      historySize = "0";
      password = "0";
      tabComplete = "0";
      sinkAllKeyEvents = "0";
      password = "0";
      passwordMask = "*";
      precision = %precision;
      accessor = %accessor;
      undoLabel = %label;
      object = %this.object;
   };
   %container.add(%r);
   
   %g = new GuiTextEditCtrl() {
      class = QuickEditColorTextEdit;
      index = 1;
      canSaveDynamicFields = "0";
      Profile = "EditorTextEdit";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "64 56";
      Extent = "48 20";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      maxLength = "1024";
      historySize = "0";
      password = "0";
      tabComplete = "0";
      sinkAllKeyEvents = "0";
      password = "0";
      passwordMask = "*";
      precision = %precision;
      accessor = %accessor;
      undoLabel = %label;
      object = %this.object;
   };
   %container.add(%g);
   
   %b = new GuiTextEditCtrl() {
      class = QuickEditColorTextEdit;
      index = 2;
      internalName = %accessor @ "TextEdit";
      canSaveDynamicFields = "0";
      Profile = "EditorTextEdit";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "64 80";
      Extent = "48 20";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      maxLength = "1024";
      historySize = "0";
      password = "0";
      tabComplete = "0";
      sinkAllKeyEvents = "0";
      password = "0";
      passwordMask = "*";
      precision = %precision;
      accessor = %accessor;
      undoLabel = %label;
      object = %this.object;
   };
   %container.add(%b);
   
   %a = new GuiTextEditCtrl() {
      class = QuickEditColorTextEdit;
      index = 3;
      internalName = %accessor @ "TextEdit";
      canSaveDynamicFields = "0";
      Profile = "EditorTextEdit";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "64 104";
      Extent = "48 20";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      maxLength = "1024";
      historySize = "0";
      password = "0";
      tabComplete = "0";
      sinkAllKeyEvents = "0";
      password = "0";
      passwordMask = "*";
      precision = %precision;
      accessor = %accessor;
      undoLabel = %label;
      object = %this.object;
   };
   %container.add(%a);
   
   %alpha = new GuiColorPickerCtrl() {
      canSaveDynamicFields = "0";
      Profile = "GuiDefaultProfile";
      class = QuickEditAlphaPicker;
      HorizSizing = "right";
      VertSizing = "bottom";
      position = "278 24";
      Extent = "15 100";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      BaseColor = "1 1 1 1";
      PickColor = "0.6 1 0 1";
      SelectorGap = "1";
      DisplayMode = "VertAlpha";
      ActionOnMove = "0";
      accessor = %accessor;
      undoLabel = %label;
      object = %this.object;
   };
   %container.add(%alpha);
   
   %color = new GuiColorPickerCtrl() {
      canSaveDynamicFields = "0";
      internalName = "QuickEditColorPicker";
      Profile = "GuiDefaultProfile";
      class = QuickEditColorPicker;
      HorizSizing = "right";
      VertSizing = "bottom";
      position = "128 24";
      Extent = "145 100";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      BaseColor = "0.6 1 0 1";
      PickColor = "0 0 0 1";
      SelectorGap = "1";
      DisplayMode = "HorizBrightnessColor";
      ActionOnMove = "0";
      accessor = %accessor;
      undoLabel = %label;
      object = %this.object;
      redText = %r;
      greenText = %g;
      blueText = %b;
      alphaText = %a;
   };
   %container.add(%color);
   
   %alpha.colorControl = %color;
   %color.alphaControl = %alpha;
   
   %r.colorControl = %color;
   %g.colorControl = %color;
   %b.colorControl = %color;
   %a.colorControl = %color;
   
   %r.altCommand = "ToolManager.getLastWindow().setFirstResponder(); " @ %r @ ".updateProperty(" @ %color @ ".object" @ ");";
   %r.validate = %r @ ".updateProperty(" @ %color @ ".object" @ ");";
   %g.altCommand = "ToolManager.getLastWindow().setFirstResponder(); " @ %g @ ".updateProperty(" @ %color @ ".object" @ ");";
   %g.validate = %g @ ".updateProperty(" @ %color @ ".object" @ ");";
   %b.altCommand = "ToolManager.getLastWindow().setFirstResponder(); " @ %b @ ".updateProperty(" @ %color @ ".object" @ ");";
   %b.validate = %b @ ".updateProperty(" @ %color @ ".object" @ ");";
   %a.altCommand = "ToolManager.getLastWindow().setFirstResponder(); " @ %a @ ".updateProperty(" @ %color @ ".object" @ ");";
   %a.validate = %a @ ".updateProperty(" @ %color @ ".object" @ ");";
   
   %color.command = %color @ ".updateProperty(" @ %color @ ".object" @ ");";
   %color.altCommand = %color @ ".onColorPicked(" @ %color @ ".object" @ ");";
   %alpha.command = %color @ ".updateProperty(" @ %color @ ".object" @ ");";
   %alpha.altCommand = %color @ ".onColorPicked(" @ %color @ ".object" @ ");";
   
   %this.addProperty(%color);
   %this.add(%container);
   return %container;
}

function QuickEditColorPicker::onWake(%this)
{
   %color = QuickEditField::getObjectValue(%this, %this.object);
   %this.pickColor = getWords(%color, 0, 2);
   %this.alphaControl.pickColor = 1 - getWord(%color, 3);
   %this.oldPickColor = %color;
}

function QuickEditColorPicker::setProperty(%this, %object)
{
   %color = QuickEditField::getObjectValue(%this, %this.object);
   %this.redText.setText(mFloatLength(getWord(%color, 0) * 255, 0));
   %this.greenText.setText(mFloatLength(getWord(%color, 1) * 255, 0));
   %this.blueText.setText(mFloatLength(getWord(%color, 2) * 255, 0));
   %this.alphaText.setText(mFloatLength(getWord(%color, 3) * 255, 0));
}

function QuickEditColorPicker::onColorPicked(%this, %object)
{
   %color = getWords(%this.pickColor, 0, 2) SPC (1 - getWord(%this.alphaControl.pickColor, 0));
   QuickEditField::setObjectValue(%this, %object, %color);
   
   %this.redText.setText(mFloatLength(getWord(%color, 0) * 255, 0));
   %this.greenText.setText(mFloatLength(getWord(%color, 1) * 255, 0));
   %this.blueText.setText(mFloatLength(getWord(%color, 2) * 255, 0));
   %this.alphaText.setText(mFloatLength(getWord(%color, 3) * 255, 0));
}

function QuickEditColorTextEdit::updateProperty(%this, %object)
{
   %color = QuickEditField::getObjectValue(%this.colorControl, %object);
   %r = mFloatLength(getWord(%color, 0) * 255, 0);
   %g = mFloatLength(getWord(%color, 1) * 255, 0);
   %b = mFloatLength(getWord(%color, 2) * 255, 0);
   %a = mFloatLength(getWord(%color, 3) * 255, 0);
   
   %oldColor = %r;
   if (%this.index == 1) %oldColor = %g;
   else if (%this.index == 2) %oldColor = %b;
   else if (%this.index == 3) %oldColor = %a;
   
   %newColor = mFloatLength(%this.getText(), 0);
   if (%newColor < 0) %newColor = 0;
   if (%newColor > 255) %newColor = 255;
   
   if (%newColor == %oldColor)
      return;
   
   %newColor /= 255;
   %color = setWord(%color, %this.index, %newColor);
   QuickEditField::updateProperty(%this.colorControl, %object, %color);
   %this.oldPickColor = %color;
}

function QuickEditColorTextEdit::setProperty(%this, %object)
{
}

function QuickEditColorPicker::updateProperty(%this, %object)
{
   %color = getWords(%this.pickColor, 0, 2) SPC (1 - getWord(%this.alphaControl.pickColor, 0));
   QuickEditField::updateProperty(%this, %object, %color, %this.oldPickColor);
   %this.oldPickColor = %color;
}
