EXE_NAME=TGB
EXE_DEDICATED_NAME=$(EXE_NAME)d
BIN_DIRECTORY=../../../games

SOURCE.AUDIO=\
	audio/audio.cc \
	audio/audioBuffer.cc \
	audio/audioDataBlock.cc \
	audio/audioFunctions.cc \
	audio/audioStreamSourceFactory.cc \
	audio/vorbisStream.cc \
	audio/vorbisStreamSource.cc \
	audio/wavStreamSource.cc \
	audio/oggMixedStreamSource.cc

SOURCE.CONSOLE=\
	console/astAlloc.cc \
        console/astNodes.cc \
        console/cmdgram.cc \
        console/CMDscan.cc \
        console/codeBlock.cc \
        console/compiledEval.cc \
        console/compiler.cc \
        console/console.cc \
        console/consoleDoc.cc \
        console/consoleFunctions.cc \
        console/consoleInternal.cc \
        console/consoleLogger.cc \
        console/consoleObject.cc \
        console/consoleParser.cc \
        console/consoleTypes.cc \
        console/dynamicTypes.cc \
        console/scriptObject.cc \
        console/simBase.cc \
        console/simDictionary.cc \
        console/simManager.cc \
        console/stringStack.cc \
        console/telnetConsole.cc \
        console/telnetDebugger.cc \
        console/typeValidators.cc

SOURCE.CORE=\
	core/bitRender.cc \
        core/bitStream.cc \
        core/bitTables.cc \
        core/chunkFile.cc \
        core/crc.cc \
        core/dataChunker.cc \
        core/dnet.cc \
        core/fileObject.cc \
        core/fileStream.cc \
        core/filterStream.cc \
        core/findMatch.cc \
        core/frameAllocator.cc \
        core/idGenerator.cc \
        core/iTickable.cc \
        core/memStream.cc \
        core/nStream.cc \
        core/nTypes.cc \
        core/resDictionary.cc \
        core/resizeStream.cc \
        core/resManager.cc \
        core/stringBuffer.cc \
        core/stringTable.cc \
        core/tagDictionary.cc \
        core/theoraPlayer.cc \
        core/tokenizer.cc \
        core/tVector.cc \
        core/unicode.cc \
        core/zipAggregate.cc \
        core/zipHeaders.cc \
        core/zipSubStream.cc

SOURCE.DGL=\
	dgl/bitmapBm8.cc \
        dgl/bitmapBmp.cc \
        dgl/bitmapGif.cc \
        dgl/bitmapJpeg.cc \
        dgl/bitmapPng.cc \
        dgl/dgl.cc \
        dgl/dglMatrix.cc \
        dgl/gBitmap.cc \
        dgl/gDynamicTexture.cc \
        dgl/gFont.cc \
        dgl/gNewFont.cc \
        dgl/gPalette.cc \
        dgl/gTexManager.cc \
        dgl/gVectorField.cc \
        dgl/lensFlare.cc \
        dgl/materialList.cc \
        dgl/materialPropertyMap.cc \
        dgl/splineUtil.cc \
        dgl/stripCache.cc

SOURCE.GUI=\
	gui/buttons/guiBitmapButtonCtrl.cc \
        gui/buttons/guiBorderButton.cc \
        gui/buttons/guiButtonBaseCtrl.cc \
        gui/buttons/guiButtonCtrl.cc \
        gui/buttons/guiCheckBoxCtrl.cc \
        gui/buttons/guiIconButtonCtrl.cc \
        gui/buttons/guiRadioCtrl.cc \
        gui/buttons/guiToolboxButtonCtrl.cc \
        gui/containers/guiAutoScrollCtrl.cc \
        gui/containers/guiCtrlArrayCtrl.cc \
        gui/containers/guiDragAndDropCtrl.cc \
        gui/containers/guiDynamicCtrlArrayCtrl.cc \
        gui/containers/guiFormCtrl.cc \
        gui/containers/guiFrameCtrl.cc \
        gui/containers/guiPaneCtrl.cc \
        gui/containers/guiRolloutCtrl.cc \
        gui/containers/guiScrollCtrl.cc \
        gui/containers/guiStackCtrl.cc \
        gui/containers/guiTabBookCtrl.cc \
        gui/containers/guiWindowCtrl.cc \
        gui/controls/guiBackgroundCtrl.cc \
        gui/controls/guiBitmapBorderCtrl.cc \
        gui/controls/guiBitmapCtrl.cc \
        gui/controls/guiColorPicker.cc \
        gui/controls/guiConsole.cc \
        gui/controls/guiConsoleEditCtrl.cc \
        gui/controls/guiConsoleTextCtrl.cc \
        gui/controls/guiDirectoryFileListCtrl.cc \
        gui/controls/guiDirectoryTreeCtrl.cc \
        gui/controls/guiListBoxCtrl.cc \
        gui/controls/guiMLTextCtrl.cc \
        gui/controls/guiMLTextEditCtrl.cc \
        gui/controls/guiPopUpCtrl.cc \
        gui/controls/guiPopUpCtrlEx.cc \
        gui/controls/guiSliderCtrl.cc \
        gui/controls/guiTabPageCtrl.cc \
        gui/controls/guiTextCtrl.cc \
        gui/controls/guiTextEditCtrl.cc \
        gui/controls/guiTextEditSliderCtrl.cc \
        gui/controls/guiTextListCtrl.cc \
        gui/controls/guiTreeViewCtrl.cc \
        gui/core/guiArrayCtrl.cc \
        gui/core/guiCanvas.cc \
        gui/core/guiControl.cc \
        gui/core/guiDefaultControlRender.cc \
        gui/core/guiScriptNotifyControl.cc \
        gui/core/guiTypes.cc \
        gui/editor/guiControlListPopup.cc \
        gui/editor/guiDebugger.cc \
        gui/editor/guiEditCtrl.cc \
	gui/editor/guiFilterCtrl.cc \
        gui/editor/guiGraphCtrl.cc \
        gui/editor/guiImageList.cc \
        gui/editor/guiInspector.cc \
        gui/editor/guiInspectorTypes.cc \
        gui/editor/guiMenuBar.cc \
        gui/editor/guiSeparatorCtrl.cc \
        gui/game/guiAviBitmapCtrl.cc \
        gui/game/guiChunkedBitmapCtrl.cc \
        gui/game/guiFadeinBitmapCtrl.cc \
        gui/game/guiMessageVectorCtrl.cc \
        gui/game/guiProgressCtrl.cc \
        gui/utility/guiBubbleTextCtrl.cc \
        gui/utility/guiInputCtrl.cc \
        gui/utility/guiMouseEventCtrl.cc \
        gui/utility/guiTransitionCtrl.cc \
        gui/utility/messageVector.cc \
        gui/shiny/guiTheoraCtrl.cc \
        gui/shiny/guiEffectCanvas.cc \
        gui/shiny/guiTickCtrl.cc
	
SOURCE.MATH=\
	math/mathTypes.cc \
        math/mathUtils.cc \
        math/mBox.cc \
        math/mConsoleFunctions.cc \
        math/mMathAMD.cc \
        math/mMath_C.cc \
        math/mMathFn.cc \
        math/mMathSSE.cc \
        math/mMatrix.cc \
        math/mPlaneTransformer.cc \
        math/mQuadPatch.cc \
        math/mQuat.cc \
        math/mRandom.cc \
        math/mSolver.cc \
        math/mSplinePatch.cc \
        math/mMath_ASM.asm \
        math/mMathAMD_ASM.asm \
        math/mMathSSE_ASM.asm

SOURCE.PLATFORM=\
	platform/gameInterface.cc \
        platform/platformAssert.cc \
        platform/platform.cc \
        platform/platformCPU.cc \
        platform/platformFileIO.cc \
        platform/platformMemory.cc \
        platform/platformRedBook.cc \
        platform/platformString.cc \
        platform/platformVideo.cc \
        platform/platformCPUInfo.asm \
        platform/profiler.cc

SOURCE.PLATFORMPPC=\
	platformPPC/ppcAudio.cc \
	platformPPC/ppcCPUInfo.cc \
	platformPPC/ppcConsole.cc \
	platformPPC/ppcFileio.cc \
	platformPPC/ppcFont.cc \
	platformPPC/ppcGL.cc \
	platformPPC/ppcInput.cc \
	platformPPC/ppcMath.cc \
	platformPPC/ppcMemory.cc \
	platformPPC/ppcNet.cc \
	platformPPC/ppcOGLVideo.cc \
	platformPPC/ppcProcessControl.cc \
	platformPPC/ppcStrings.cc \
	platformPPC/ppcTime.cc \
	platformPPC/ppcUtils.cc \
	platformPPC/ppcWindow.cc

SOURCE.PLATFORMWIN32=\
	platformWin32/winAsmBlit.cc \
	platformWin32/winCPUInfo.cc \
	platformWin32/winConsole.cc \
	platformWin32/winD3DVideo.cc \
	platformWin32/winDInputDevice.cc \
	platformWin32/winDirectInput.cc \
	platformWin32/winFileio.cc \
	platformWin32/winFont.cc \
	platformWin32/winGL.cc \
	platformWin32/winGLSpecial.cc \
	platformWin32/winInput.cc \
	platformWin32/winMath.cc \
	platformWin32/winMath_ASM.cc \
	platformWin32/winMemory.cc \
	platformWin32/winMutex.cc \
	platformWin32/winNet.cc \
	platformWin32/winOGLVideo.cc \
	platformWin32/winOpenAL.cc \
	platformWin32/winProcessControl.cc \
	platformWin32/winRedbook.cc \
	platformWin32/winSemaphore.cc \
	platformWin32/winStrings.cc \
	platformWin32/winThread.cc \
	platformWin32/winTime.cc \
	platformWin32/winV2Video.cc \
	platformWin32/winWindow.cc \
	platformWin32/winWindow.cc \
	platformWin32/registry/winRegistryAccess.cpp \
	platformWin32/registry/winRegistryManager.cpp \

SOURCE.SIM=\
	sim/actionMap.cc \
        sim/connectionStringTable.cc \
        sim/netConnection.cc \
        sim/netDownload.cc \
        sim/netEvent.cc \
        sim/netGhost.cc \
        sim/netInterface.cc \
        sim/netObject.cc \
        sim/netStringTable.cc

SOURCE.TS=\
        ts/tsAnimate.cc \
        ts/tsDecal.cc \
        ts/tsDump.cc \
        ts/tsIntegerSet.cc \
        ts/tsLastDetail.cc \
        ts/tsMaterialList.cc \
        ts/tsMesh.cc \
        ts/tsPartInstance.cc \
        ts/tsShapeAlloc.cc \
        ts/tsShape.cc \
        ts/tsShapeConstruct.cc \
        ts/tsShapeInstance.cc \
        ts/tsShapeOldRead.cc \
        ts/tsSortedMesh.cc \
        ts/tsThread.cc \
        ts/tsTransform.cc

SOURCE.GAME=\
	game/banList.cc \
	game/demoGame.cc \
	game/main.cc \
	game/version.cc \
	game/gameConnection.cc

SOURCE.GAME.NET=\
	game/net/httpObject.cc \
	game/net/net.cc \
	game/net/netTest.cc \
	game/net/serverQuery.cc \
	game/net/tcpObject.cc

SOURCE.I18N=\
	i18n/lang.cc \
	i18n/i18n.cc

SOURCE.UTIL=\
	util/rectClipper.cpp \
	util/triBoxCheck.cpp \
	util/undo.cc

SOURCE.PLATFORMX86UNIX=\
	platform/platformNetAsync.cc \
	platformX86UNIX/x86UNIXAsmBlit.cc \
	platformX86UNIX/x86UNIXCPUInfo.cc \
	platformX86UNIX/x86UNIXConsole.cc \
	platformX86UNIX/x86UNIXFileio.cc \
	platformX86UNIX/x86UNIXFont.cc \
	platformX86UNIX/x86UNIXGL.cc \
	platformX86UNIX/x86UNIXInput.cc \
	platformX86UNIX/x86UNIXInputManager.cc \
	platformX86UNIX/x86UNIXIO.cc \
	platformX86UNIX/x86UNIXMath.cc \
	platformX86UNIX/x86UNIXMath_ASM.cc \
	platformX86UNIX/x86UNIXMemory.cc \
	platformX86UNIX/x86UNIXMessageBox.cc \
	platformX86UNIX/x86UNIXMutex.cc \
	platformX86UNIX/x86UNIXNet.cc \
	platformX86UNIX/x86UNIXOGLVideo.cc \
	platformX86UNIX/x86UNIXOpenAL.cc \
	platformX86UNIX/x86UNIXProcessControl.cc \
	platformX86UNIX/x86UNIXRedbook.cc \
	platformX86UNIX/x86UNIXSemaphore.cc \
	platformX86UNIX/x86UNIXStrings.cc \
	platformX86UNIX/x86UNIXThread.cc \
	platformX86UNIX/x86UNIXTime.cc \
	platformX86UNIX/x86UNIXWindow.cc \
	platformX86UNIX/x86UNIXUtils.cc

SOURCE.PLATFORMX86UNIXDEDICATED=\
	platform/platformNetAsync.cc \
	platformX86UNIX/x86UNIXCPUInfo.cc \
	platformX86UNIX/x86UNIXConsole.cc \
	platformX86UNIX/x86UNIXDedicatedStub.cc \
	platformX86UNIX/x86UNIXFileio.cc \
	platformX86UNIX/x86UNIXIO.cc \
	platformX86UNIX/x86UNIXMath.cc \
	platformX86UNIX/x86UNIXMath_ASM.cc \
	platformX86UNIX/x86UNIXMemory.cc \
	platformX86UNIX/x86UNIXMutex.cc \
	platformX86UNIX/x86UNIXNet.cc \
	platformX86UNIX/x86UNIXProcessControl.cc \
	platformX86UNIX/x86UNIXSemaphore.cc \
	platformX86UNIX/x86UNIXStrings.cc \
	platformX86UNIX/x86UNIXThread.cc \
	platformX86UNIX/x86UNIXTime.cc \
	platformX86UNIX/x86UNIXWindow.cc \
	platformX86UNIX/x86UNIXUtils.cc
	


SOURCE.PLATFORMLINUX=$(SOURCE.PLATFORMX86UNIX)
SOURCE.PLATFORMLINUXDEDICATED=$(SOURCE.PLATFORMX86UNIXDEDICATED)

SOURCE.PLATFORMOpenBSD=$(SOURCE.PLATFORMX86UNIX)
SOURCE.PLATFORMOpenBSDDEDICATED=$(SOURCE.PLATFORMX86UNIXDEDICATED)

SOURCE.PLATFORMFreeBSD=$(SOURCE.PLATFORMX86UNIX)
SOURCE.PLATFORMFreeBSDDEDICATED=$(SOURCE.PLATFORMX86UNIXDEDICATED)

SOURCE.T2D=\
	T2D/t2dActiveTile.cc \
        T2D/t2dAnimatedSprite.cc \
        T2D/t2dAnimationController.cc \
        T2D/t2dBaseDatablock.cc \
        T2D/t2dChunkedImageDatablock.cc \
        T2D/t2dChunkedSprite.cc \
        T2D/t2dGraphField.cc \
        T2D/t2dImageMapDatablock.cc \
        T2D/t2dParticleEffect.cc \
        T2D/t2dParticleEmitter.cc \
        T2D/t2dPath.cc \
        T2D/t2dPhysics.cc \
        T2D/t2dSceneContainer.cc \
        T2D/t2dSceneGraph.cc \
        T2D/t2dSceneObject.cc \
        T2D/t2dSceneObjectGroup.cc \
        T2D/t2dSceneObjectSet.cc \
        T2D/t2dSceneWindow.cc \
        T2D/t2dScroller.cc \
        T2D/t2dShape3D.cc \
        T2D/t2dShapeVector.cc \
        T2D/t2dStaticSprite.cc \
        T2D/t2dTileMap.cc \
        T2D/t2dTrigger.cc \
        T2D/t2dUtility.cc \
        T2D/t2dVector.cc \
	T2D/t2dQuadBatch.cpp \
        T2D/activeTiles/t2dGunTurret.cc \
        T2D/activeTiles/t2dTestTile.cc \
        T2D/levelBuilder/gui2DDatablockDropDown.cc \
        T2D/levelBuilder/guiParticleGraphCtrl.cc \
        T2D/levelBuilder/guiT2DObjectCtrl.cc \
        T2D/levelBuilder/levelBuilder3DShapeTool.cc \
        T2D/levelBuilder/levelBuilderAnimatedSpriteTool.cc \
        T2D/levelBuilder/levelBuilderBaseEditTool.cc \
        T2D/levelBuilder/levelBuilderBaseTool.cc \
        T2D/levelBuilder/levelBuilderCameraTool.cc \
        T2D/levelBuilder/levelBuilderChunkedSpriteTool.cc \
        T2D/levelBuilder/levelBuilderCreateTool.cc \
        T2D/levelBuilder/levelBuilderLinkPointTool.cc \
        T2D/levelBuilder/levelBuilderMountTool.cc \
        T2D/levelBuilder/levelBuilderParticleTool.cc \
        T2D/levelBuilder/levelBuilderPathEditTool.cc \
        T2D/levelBuilder/levelBuilderPathTool.cc \
        T2D/levelBuilder/levelBuilderPolyTool.cc \
        T2D/levelBuilder/levelBuilderSceneEdit.cc \
        T2D/levelBuilder/levelBuilderSceneObjectTool.cc \
        T2D/levelBuilder/levelBuilderSceneWindow.cc \
        T2D/levelBuilder/levelBuilderScrollerTool.cc \
        T2D/levelBuilder/levelBuilderSelectionTool.cc \
	T2D/levelBuilder/levelBuilderSortPointTool.cc \
        T2D/levelBuilder/levelBuilderStaticSpriteTool.cc \
	T2D/levelBuilder/levelBuilderTileMapEditTool.cc \
        T2D/levelBuilder/levelBuilderTileMapTool.cc \
	T2D/levelBuilder/levelBuilderTriggerTool.cc \
        T2D/levelBuilder/levelBuilderWorldLimitTool.cc

# jmq: added the stuff after SOURCE.TS for tools build hack
SOURCE.ENGINE =\
	$(SOURCE.COLLISION) \
        $(SOURCE.CONSOLE) \
        $(SOURCE.CORE) \
        $(SOURCE.DGL) \
        $(SOURCE.I18N) \
        $(SOURCE.UTIL) \
        $(SOURCE.INTERIOR) \
        $(SOURCE.MATH) \
        $(SOURCE.PLATFORM) \
        $(SOURCE.SCENEGRAPH) \
        $(SOURCE.SIM) \
        $(SOURCE.TERRAIN) \
        $(SOURCE.TS) \
        $(SOURCE.AUDIO) \
        $(SOURCE.GUI) \
        $(SOURCE.GAME) \
        $(SOURCE.GAME.FPS) \
        $(SOURCE.GAME.NET) \
        $(SOURCE.GAME.FX) \
        $(SOURCE.GAME.VEHICLES) \
        $(SOURCE.T2D)

ifeq "$(OS)" "WIN32"
SOURCE.ENGINE += $(SOURCE.PLATFORM$(OS))
else
SOURCE.ENGINE += $(SOURCE.PLATFORM$(OS)DEDICATED)
endif

SOURCE.TESTAPP =\
	$(SOURCE.AUDIO) \
        $(SOURCE.COLLISION) \
        $(SOURCE.CONSOLE) \
        $(SOURCE.CORE) \
        $(SOURCE.DGL) \
        $(SOURCE.I18N) \
        $(SOURCE.UTIL) \
        $(SOURCE.EDITOR) \
        $(SOURCE.GUI) \
        $(SOURCE.GAME) \
        $(SOURCE.GAME.FPS) \
        $(SOURCE.GAME.NET) \
        $(SOURCE.GAME.FX) \
        $(SOURCE.GAME.VEHICLES) \
        $(SOURCE.INTERIOR) \
        $(SOURCE.MATH) \
        $(SOURCE.PLATFORM) \
        $(SOURCE.SCENEGRAPH) \
        $(SOURCE.SIM) \
        $(SOURCE.TERRAIN) \
        $(SOURCE.TS) \
        $(SOURCE.T2D)

SOURCE.TESTAPP_CLIENT =\
	$(SOURCE.TESTAPP) \
	$(SOURCE.PLATFORM$(OS)) \

SOURCE.TESTAPP_DEDICATED =\
	$(SOURCE.TESTAPP) \
	$(SOURCE.PLATFORM$(OS)DEDICATED) \

SOURCE.TESTAPP_CLIENT.OBJ:=$(addprefix $(DIR.OBJ)/, $(addsuffix $O, $(basename $(SOURCE.TESTAPP_CLIENT))) )
SOURCE.TESTAPP_DEDICATED.OBJ:=$(addprefix $(DIR.OBJ)/, $(addsuffix $O, $(basename $(SOURCE.TESTAPP_DEDICATED))) )
SOURCE.ENGINE.OBJ:=$(addprefix $(DIR.OBJ)/, $(addsuffix $O, $(basename $(SOURCE.ENGINE))) )
SOURCE.ALL += $(SOURCE.TESTAPP_CLIENT)
targetsclean += torqueClean

#---------------------------------------
# Set up include variables here.
INCLUDES_BASE = -I../lib/zlib -I../lib/lungif -I../lib/lpng -I../lib/ljpeg -I../lib/directx8 -I../lib/vorbis/include -I../lib/xiph/include
INCLUDES_LINUX = $(INCLUDES_BASE) -I../lib/openal/LINUX
INCLUDES_OpenBSD = $(INCLUDES_BASE) -I../lib/openal/OpenBSD
INCLUDES_FreeBSD = $(INCLUDES_BASE) -I../lib/openal/FreeBSD
INCLUDES_WIN32 = $(INCLUDES_BASE) -I../lib/openal/win32

#----------------------------------------
# normal binary
$(EXE_NAME): $(DIR.OBJ)/$(EXE_NAME)$(EXT.EXE)

DIR.LIST = $(addprefix $(DIR.OBJ)/, $(sort $(dir $(SOURCE.TESTAPP_CLIENT))))

$(DIR.LIST): targets.torque.mk

$(DIR.OBJ)/$(EXE_NAME)$(EXT.EXE): CFLAGS += $(INCLUDES_$(OS))

$(DIR.OBJ)/$(EXE_NAME)$(EXT.EXE): LIB.PATH +=$(DIR.OBJ) \

$(DIR.OBJ)/$(EXE_NAME)$(EXT.EXE): LINK.LIBS.GENERAL += \
	$(PRE.LIBRARY.LIB)ljpeg$(EXT.LIB) \
	$(PRE.LIBRARY.LIB)lpng$(EXT.LIB) \
	$(PRE.LIBRARY.LIB)lungif$(EXT.LIB) \
	$(PRE.LIBRARY.LIB)zlib$(EXT.LIB) 

$(DIR.OBJ)/$(EXE_NAME)$(EXT.EXE): $(DIR.OBJ) $(DIR.LIST) $(SOURCE.TESTAPP_CLIENT.OBJ) 
	$(DO.LINK.CONSOLE.EXE)
	$(CP) $(DIR.OBJ)/$(EXE_NAME)$(BUILD_SUFFIX).* $(BIN_DIRECTORY)


#----------------------------------------
# engine library
engine: $(DIR.OBJ)/engine$(EXT.LIB)

DIR.LIST = $(addprefix $(DIR.OBJ)/, $(sort $(dir $(SOURCE.ENGINE))))

$(DIR.LIST): targets.torque.mk

# unix build needs to add DEDICATED to the CFLAGS
EXTRAFLAGS=
ifneq "$(OS)" "WIN32"
EXTRAFLAGS=-DDEDICATED -DTORQUE_ENGINE
endif

$(DIR.OBJ)/engine$(EXT.LIB): CFLAGS += $(EXTRAFLAGS) -DTORQUE_MAX_LIB $(INCLUDES_$(OS))

$(DIR.OBJ)/engine$(EXT.LIB): $(DIR.OBJ) $(DIR.LIST) $(SOURCE.ENGINE.OBJ)
	$(DO.LINK.LIB)

#----------------------------------------
# dedicated server build (unix only)
dedicated: $(DIR.OBJ)/$(EXE_DEDICATED_NAME)$(EXT.EXE)

DIR.LIST = $(addprefix $(DIR.OBJ)/, $(sort $(dir $(SOURCE.TESTAPP_DEDICATED))))

$(DIR.LIST): targets.torque.mk

$(DIR.OBJ)/$(EXE_DEDICATED_NAME)$(EXT.EXE): CFLAGS += -DDEDICATED $(INCLUDES_$(OS))

$(DIR.OBJ)/$(EXE_DEDICATED_NAME)$(EXT.EXE): LIB.PATH +=$(DIR.OBJ) \

$(DIR.OBJ)/$(EXE_DEDICATED_NAME)$(EXT.EXE): LINK.LIBS.GENERAL = \
	$(LINK.LIBS.SERVER) \
	$(PRE.LIBRARY.LIB)ljpeg$(EXT.LIB) \
	$(PRE.LIBRARY.LIB)lpng$(EXT.LIB) \
	$(PRE.LIBRARY.LIB)lungif$(EXT.LIB) \
	$(PRE.LIBRARY.LIB)zlib$(EXT.LIB)

$(DIR.OBJ)/$(EXE_DEDICATED_NAME)$(EXT.EXE): $(DIR.OBJ) $(DIR.LIST) $(SOURCE.TESTAPP_DEDICATED.OBJ)
	$(DO.LINK.CONSOLE.EXE)
	$(CP) $(DIR.OBJ)/$(EXE_DEDICATED_NAME)$(BUILD_SUFFIX).* $(BIN_DIRECTORY)

#----------------------------------------
torqueClean:
ifneq ($(wildcard $(EXE_NAME)_DEBUG.*),)
	-$(RM)  $(EXE_NAME)$(BUILD_SUFFIX)*
endif
ifneq ($(wildcard $(EXE_NAME)_RELEASE.*),)
	-$(RM)  $(EXE_NAME)_RELEASE*
endif
