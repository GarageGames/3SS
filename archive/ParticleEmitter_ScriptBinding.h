//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getParentEffect, S32, 2, 2, "()\n @return Returns the effect containing this emitter.")
{
   ParticleEffect* effect = object->getParentEffect();
   if (effect)
      return effect->getId();

   return 0;
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, selectGraph, void, 3, 3, "(graphName) - Select Graph Name.")
{
    // Select Graph.
    object->selectGraph( argv[2] );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, addDataKey, S32, 4, 4, "(time, value) - Add Data-Key to Graph.")
{
    // Add Data Key.
    return object->addDataKey( dAtof(argv[2]), dAtof(argv[3]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, removeDataKey, bool, 3, 3, "(keyIndex) - Remove Data-Key from Graph.")
{
    // Remove Data Key.
    return object->removeDataKey( dAtoi(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, clearDataKeys, bool, 2, 2, "Clear Data-Key(s) from Graph.")
{
    // Clear Data Keys
    return object->clearDataKeys();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setDataKeyValue, bool, 4, 4, "(keyIndex, value) - Set Data-Key Value in Graph.")
{
    // Set Data Key Value.
    return object->setDataKeyValue( dAtoi(argv[2]), dAtof(argv[3]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getDataKey, const char*, 3, 3, "(keyIndex) - Get Data-Key Time/Value from Graph.")
{
    // Fetch Key Index.
    S32 keyIndex = dAtoi(argv[2]);

    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer(32);
    // Format Buffer.
    dSprintf(pBuffer, 32, "%f %f", object->getDataKeyTime( keyIndex ), object->getDataKeyValue( keyIndex ) );
    // Return buffer.
    return pBuffer;
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getDataKeyCount, S32, 2, 2, "Get Data-Key Count from Graph.")
{
    // Get Data Key Count.
    return object->getDataKeyCount();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getMinValue, F32, 2, 2, "Get Min-Value from Graph.")
{
    // Get Min Value.
    return object->getMinValue();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getMaxValue, F32, 2, 2, "Get Max-Value from Graph.")
{
    // Get Max Value.
    return object->getMaxValue();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getMinTime, F32, 2, 2, "Get Min-Time from Graph.")
{
    // Get Min Time.
    return object->getMinTime();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getMaxTime, F32, 2, 2, "Get Max-Value from Graph.")
{
    // Get Max Time.
    return object->getMaxTime();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getGraphValue, F32, 3, 3, "(time) - Get Value from Graph.")
{
    // Get Graph Value.
    return object->getGraphValue( dAtof(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setTimeRepeat, bool, 3, 3, "(timeRepeat) - Set Time-Repeat for Graph.")
{
    // Set Time Repeat.
    return object->setTimeRepeat( dAtof(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getTimeRepeat, F32, 2, 2, "Get Time-Repeat for Graph.")
{
    // Get Time Repeat.
    return object->getTimeRepeat();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setValueScale, bool, 3, 3, "(valueScale) - Set Value-Scale For Graph.")
{
    // Set Value Scale.
    return object->setValueScale( dAtof(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getValueScale, F32, 2, 2, "Get Value-Scale for Graph.")
{
    // Get Value Scale.
    return object->getValueScale();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setVisible, void, 3, 3, "(status?) - Set the Emitters Visibility.")
{
    // Set Emitter Visibility.
    object->setEmitterVisible( dAtob(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setEmitterName, void, 3, 3, "(emitterName$) - Set the Emitters Name.")
{
    // Set Emitter Name.
    object->setEmitterName( argv[2] );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setFixedAspect, void, 3, 3, "(fixedAspect) - Set Fixed-Aspect.")
{
    // Set Fixed Aspect.
    object->setFixedAspect( dAtob(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setFixedForceAngle, void, 3, 3, "(fixedForceAngle) - Set Fixed-Force Angle.")
{
    // Set Fixed-Force Angle.
    object->setFixedForceAngle( dAtof(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setParticleOrientationMode, void, 3, 3, "(particleOrientationMode$) - Set Particle Orientation.")
{
    // Set Particle Orientation Mode.
    object->setParticleOrientationMode( getParticleOrientationMode(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setAlignAngleOffset, void, 3, 3, "(alignAngleOffset) - Set Align-Orientation Angle Offset.")
{
    // Set Align Angle Offset.
    object->setAlignAngleOffset( dAtof(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setAlignKeepAligned, void, 3, 3, "(keepAligned) - Set Align-Orientation Keep-Aligned Flag.")
{
    // Set Align Keep Aligned.
    object->setAlignKeepAligned( dAtob(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setRandomAngleOffset, void, 3, 3, "(randomAngle) - Set Random-Orientation Angle-Offset.")
{
    // Set Random Angle Offset.
    object->setRandomAngleOffset( dAtof(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setRandomArc, void, 3, 3, "(randomArc) - Set Random-Orientation Arc.")
{
    // Set Random Arc.
    object->setRandomArc( dAtof(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setFixedAngleOffset, void, 3, 3, "(randomAngle) - Set Fixed-Orientation Angle-Offset.")
{
    // Set Fixed Angle Offset.
    object->setFixedAngleOffset( dAtof(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setEmitterType, void, 3, 3, "(emitterType$) - Set Emitter Type.")
{
    // Set Emitter Type.
    object->setEmitterType( getEmitterType(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setImageMap, bool, 3, 4, "(imageMapName$, [int frame]) - Set the ImageMap/Frame.")
{
    // Calculate Frame.
    U32 frame = argc >= 4 ? dAtoi(argv[3]) : 0;

    // Set ImageMap/Frame.
    return object->setImageMap( argv[2], frame );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setAnimation, bool, 3, 3, "(animationName$) - Set the Animation.")
{
    // Set Animation Name.
    return object->setAnimation( argv[2] );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setPivotPoint, void, 3, 4, "(pivotX / pivotY) - Set the Pivot-Point.")
{
   // The pivot point.
   F32 pivotX, pivotY;

   // Grab the element count.
   U32 elementCount =Utility::mGetStringElementCount(argv[2]);

   // ("pivotX pivotY")
   if ((elementCount == 2) && (argc < 4))
   {
      pivotX = dAtof(Utility::mGetStringElement(argv[2], 0));
      pivotY = dAtof(Utility::mGetStringElement(argv[2], 1));
   }

   // (pivotX, pivotY)
   else if ((elementCount == 1) && (argc > 3))
   {
      pivotX = dAtof(argv[2]);
      pivotY = dAtof(argv[3]);
   }

   // Invalid
   else
   {
      Con::warnf("TileLayer::setPivotPoint() - Invalid number of parameters!");
      return;
   }

    // Set Pivot Point.
    object->setPivotPoint(Vector2(pivotX, pivotY));
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setUseEffectEmission, void, 3, 3, "(useEffectEmission) - Set Use-Effect-Emission Flag.")
{
    // Set Use Effect Emission.
    object->setUseEffectEmission( dAtob(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setLinkEmissionRotation, void, 3, 3, "(linkEmissionRotation) - Set Link-Emission-Rotation Flag.")
{
    // Set Link Emission Rotation.
    object->setLinkEmissionRotation( dAtob(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setIntenseParticles, void, 3, 3, "(intenseParticles) - Set Intense-Particles Flag.")
{
    // Set Intense Particles.
    object->setIntenseParticles( dAtob(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setSingleParticle, void, 3, 3, "(singleParticle) - Set Single-Particle Flag.")
{
    // Set Single Particle.
    object->setSingleParticle( dAtob(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setAttachPositionToEmitter, void, 3, 3, "(attachPositionToEmitter) - Set Attach-Position-To-Emitter Flag.")
{
    // Set Attach Position To Emitter.
    object->setAttachPositionToEmitter( dAtob(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setAttachRotationToEmitter, void, 3, 3, "(attachRotationToEmitter) - Set Attach-Rotation-To-Emitter Flag.")
{
    // Set Attach Rotation To Emitter.
    object->setAttachRotationToEmitter( dAtob(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setOrderedParticles, void, 3, 3,  "(ordered?) - Set whether the particles are ordered or not.  Non-ordered is much faster rendering!\n"
                                                                    "@param ordered Whether the particles are ordered or not.\n"
                                                                    "@return No return value." )
{
    // Set ordered particles.
    object->setOrderedParticles( dAtob(argv[2]) );
}

//-----------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getOrderedParticles, bool, 2, 2,  "() - Gets whether the particles are ordered or not.\n"
                                                                    "@return Whether the particles are ordered or not." )
{
    // Get ordered particles.
    return object->getOrderedParticles();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setFirstInFrontOrder, void, 3, 3, "(firstInFrontOrder) - Set First-InFront-Order Flag.")
{
    // Set First In Front Order.
    object->setFirstInFrontOrder( dAtob(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setBlending, void, 3, 5, "(blendStatus?, [srcBlendFactor], [dstBlendFactor]) - Sets the Rendering Blend Options.")
{
    // Calculate Blending Factors.
    S32 srcBlendFactor = argc >= 4 ? getSrcBlendFactorEnum(argv[3]) : GL_SRC_ALPHA;
    S32 dstBlendFactor = argc >= 5 ? getDstBlendFactorEnum(argv[4]) : GL_ONE_MINUS_SRC_ALPHA;

    // Check Source Factor.
    if ( srcBlendFactor == GL_INVALID_BLEND_FACTOR )
    {
        // Warn.
        Con::warnf("ParticleEmitter::setBlending() - Invalid srcBlendFactor '%s', defaulting to SRC_ALPHA!", argv[3]);
        // USe Default.
        srcBlendFactor = GL_SRC_ALPHA;
    }

    // Check Destination Factor.
    if ( dstBlendFactor == GL_INVALID_BLEND_FACTOR )
    {
        // Warn.
        Con::warnf("ParticleEmitter::setBlending() - Invalid dstBlendFactor '%s', defaulting to ONE_MINUS_SRC_ALPHA!", argv[4]);
        // USe Default.
        dstBlendFactor = GL_ONE_MINUS_SRC_ALPHA;
    }

    // Set Blending.
    object->setBlending( dAtob(argv[2]), srcBlendFactor, dstBlendFactor );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getBlending, const char*, 2, 2, "Gets the Rendering Blend Options.")
{
    // Create Returnable Buffer.
    char* pBuffer = Con::getReturnBuffer(128);
    // Format Buffer.
    dSprintf(pBuffer, 128, "%d %s %s", S32(object->getBlendingStatus()), getSrcBlendFactorDescription((GLenum)object->getSrcBlendFactor()), getDstBlendFactorDescription((GLenum)object->getDstBlendFactor()) );
    // Return buffer.
    return pBuffer;
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getVisible, bool, 2, 2, "Get the Emitters Visibility.")
{
    // Get Emitter Visibility.
    return object->getEmitterVisible();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getEmitterName, const char*, 2, 2, "Get the Emitters Name.")
{
    // Get Emitter Name.
    return object->getEmitterName();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getFixedAspect, bool, 2, 2, "Get Fixed-Aspect.")
{
    // Get Fixed Aspect.
    return object->getFixedAspect();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getFixedForceAngle, F32, 2, 2, "Get Fixed-Force-Direction.")
{
    // Get Fixed-Force Angle.
    return object->getFixedForceAngle();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getParticleOrientation, const char*, 2, 2, "Get Particle Orientation.")
{
    // Get Particle Orientation.
    return object->getParticleOrientation();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getParticleOrientationMode, const char*, 2, 2, "Get Particle Orientation.")
{
    // Get Particle Orientation.
    return object->getParticleOrientation();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getAlignAngleOffset, F32, 2, 2, "Get Align-Orientation Angle-Offset.")
{
    // Get Align Angle Offset.
    return object->getAlignAngleOffset();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getAlignKeepAligned, F32, 2, 2, "Get Align-Orientation Keep-Aligned Flag.")
{
    // Get Align Keep Aligned.
    return object->getAlignKeepAligned();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getRandomAngleOffset, F32, 2, 2, "Get Random-Orientation Angle-Offset.")
{
    // Get Random Angle Offset.
    return object->getRandomAngleOffset();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getRandomArc, F32, 2, 2, "Get Random-Orientation Arc.")
{
    // Get Random Arc.
    return object->getRandomArc();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getFixedAngleOffset, F32, 2, 2, "Get Fixed-Orientation Angle-Offset.")
{
    // Get Fixed Angle Offset.
    return object->getFixedAngleOffset();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getEmitterType, const char*, 2, 2, "Get Emitter Type.")
{
    // Get Emitter Type.
    return object->getEmitterType();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getImageMapNameFrame, const char*, 2, 2, "Get ImageMap Name and Frame.")
{
    // Get ImageMap Name/Frame.
    return object->getImageMapNameFrame();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getAnimation, const char*, 2, 2, "Get Animation Name.")
{
    // Get Animation Name.
    return object->getAnimation();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getPivotPoint, const char*, 2, 2, "Get Pivot-Point.")
{
    // Get Pivot-Point.
    return object->getPivotPoint();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getUseEffectEmission, bool, 2, 2, "Get Use-Effect-Emission Flag.")
{
    // Get Use Effect Emission.
    return object->getUseEffectEmission();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getLinkEmissionRotation, bool, 2, 2, "Get Link-Emission-Rotation Flag.")
{
    // Get Link Emission Rotation.
    return object->getLinkEmissionRotation();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getIntenseParticles, bool, 2, 2, "Get Intense-Particles Flag.")
{
    // Get Intense Particles.
    return object->getIntenseParticles();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getSingleParticle, bool, 2, 2, "Get Single-Particle Flag.")
{
    // Get Single Particle.
    return object->getSingleParticle();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getAttachPositionToEmitter, bool, 2, 2, "Get Attach-Position-To-Emitter Flag.")
{
    // Get Attach Position To Emitter.
    return object->getAttachPositionToEmitter();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getAttachRotationToEmitter, bool, 2, 2, "Get Attach-Rotation-To-Emitter Flag.")
{
    // Get Attach Rotation To Emitter.
    return object->getAttachRotationToEmitter();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getFirstInFrontOrder, bool, 2, 2, "Get First-In-Front-Order Flag.")
{
    // Get First In Front Order.
    return object->getFirstInFrontOrder();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getUsingAnimation, bool, 2, 2, "Get Using Animation Flag.")
{
    // Get 'Using Animation' Flag.
    return object->getUsingAnimation();
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, loadEmitter, bool, 3, 3, "(emitterFile$) - Loads a Particle Emitter.")
{
    // Load Emitter.
    return object->loadEmitter( argv[2] );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, saveEmitter, bool, 3, 3, "(emitterFile$) - Save a Particle Emitter.")
{
    // Save Emitter.
    return object->saveEmitter( argv[2] );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, setEmitterCollisionStatus, void, 3, 3, "Set the emitter collision status.")
{
    // Set Emitter Collision Status.
    object->setEmitterCollisionStatus( dAtob(argv[2]) );
}

//------------------------------------------------------------------------------

ConsoleMethod(ParticleEmitter, getEffectCollisionStatus, bool, 2, 2, "Get the emitter collision status.")
{
    // Get Emitter Collision Status.
    return object->getEmitterCollisionStatus();
}
