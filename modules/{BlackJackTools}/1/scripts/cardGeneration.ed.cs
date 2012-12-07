
function CardGenerationGUI::onWake(%this)
{
   //set the extent so that it covers the entire editor window
   %windowXExtent = getWord($pref::Video::windowedRes, 0);
   %windowYExtent = getWord($pref::Video::windowedRes, 1);
   
   //TODO might need to adjust this some to account for the toolbar
   %this.setExtent(%windowXExtent, %windowYExtent);
   screenGreyOut.setExtent(%windowXExtent, %windowYExtent);
   
   //calculate where to move the preview
   %yPosition = %windowYExtent - 210;
   
   GenerationWindow.setPosition(0, %yPosition);
   
   %this.schedule(3000, generateDeck);

}

function CardGenerationGUI::refreshGeneratorWindow(%this)
{
   //set the card front background
   GenerationWindow-->CardFrontBackground.setBitmap(expandPath($DeckPath @ $DeckTool::currentDeckEdit.cardFrontImage));
   
  
   switch$($DeckTool::cardValue)
   {
      case "A":
      
         if($DeckTool::currentDeckEdit.faceStyle $= "full")
            setPipPattern(GenerationWindow, "default");
         else
            setPipPattern(GenerationWindow, "A");   
         
         setFaceStyle(GenerationWindow, $DeckTool::currentDeckEdit.faceStyle);
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.spadesFaceAce));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueAceSpade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueAceSpade));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueAceSpade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueAceSpade));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipSpade));
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.clubsFaceAce));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueAceClub));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueAceClub));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueAceClub));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueAceClub));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipClub));
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.diamondsFaceAce));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueAceDiamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueAceDiamond));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueAceDiamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueAceDiamond));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipDiamond));
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.heartsFaceAce));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueAceHeart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueAceHeart));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueAceHeart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueAceHeart));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipHeart));
            }
         }
      
      case "2":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(GenerationWindow, "default");
         else
            setPipPattern(GenerationWindow, "2");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(GenerationWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(GenerationWindow, "default");   
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.spadesFace2));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue2Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue2Spade));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue2Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue2Spade));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipSpade));
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.clubsFace2));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue2Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue2Club));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue2Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue2Club));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipClub));
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.diamondsFace2));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue2Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue2Diamond));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue2Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue2Diamond));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipDiamond));
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.heartsFace2));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue2Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue2Heart));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue2Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue2Heart));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipHeart));
            }
         }
         
      
      case "3":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(GenerationWindow, "default");
         else
            setPipPattern(GenerationWindow, "3");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(GenerationWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(GenerationWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.spadesFace3));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue3Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue3Spade));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue3Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue3Spade));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipSpade));
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.clubsFace3));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue3Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue3Club));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue3Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue3Club));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipClub));
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.diamondsFace3));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue3Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue3Diamond));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue3Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue3Diamond));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipDiamond));
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.heartsFace3));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue3Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue3Heart));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue3Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue3Heart));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipHeart));
            }
         }
         
      
      case "4":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(GenerationWindow, "default");
         else
            setPipPattern(GenerationWindow, "4");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(GenerationWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(GenerationWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.spadesFace4));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue4Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue4Spade));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue4Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue4Spade));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipSpade));
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.clubsFace4));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue4Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue4Club));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue4Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue4Club));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipClub));
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.diamondsFace4));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue4Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue4Diamond));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue4Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue4Diamond));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipDiamond));
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.heartsFace4));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue4Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue4Heart));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue4Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue4Heart));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipHeart));
            }
         }
      
      case "5":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(GenerationWindow, "default");
         else
            setPipPattern(GenerationWindow, "5");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(GenerationWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(GenerationWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.spadesFace5));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue5Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue5Spade));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue5Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue5Spade));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipSpade));
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.clubsFace5));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue5Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue5Club));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue5Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue5Club));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipClub));
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.diamondsFace5));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue5Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue5Diamond));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue5Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue5Diamond));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipDiamond));
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.heartsFace5));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue5Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue5Heart));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue5Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue5Heart));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipHeart));
            }
         }
         
      
      case "6":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(GenerationWindow, "default");
         else
            setPipPattern(GenerationWindow, "6");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(GenerationWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(GenerationWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.spadesFace6));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue6Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue6Spade));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue6Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue6Spade));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipSpade));
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.clubsFace6));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue6Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue6Club));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue6Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue6Club));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipClub));
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.diamondsFace6));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue6Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue6Diamond));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue6Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue6Diamond));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipDiamond));
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.heartsFace6));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue6Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue6Heart));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue6Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue6Heart));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipHeart));
            }
         }
         
      
      case "7":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(GenerationWindow, "default");
         else
            setPipPattern(GenerationWindow, "7");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(GenerationWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(GenerationWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.spadesFace7));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue7Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue7Spade));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue7Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue7Spade));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipSpade));
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.clubsFace7));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue7Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue7Club));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue7Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue7Club));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipClub));
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.diamondsFace7));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue7Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue7Diamond));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue7Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue7Diamond));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipDiamond));
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.heartsFace7));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue7Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue7Heart));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue7Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue7Heart));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipHeart));
            }
         }
         
      
      case "8":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(GenerationWindow, "default");
         else
            setPipPattern(GenerationWindow, "8");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(GenerationWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(GenerationWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.spadesFace8));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue8Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue8Spade));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue8Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue8Spade));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipSpade));
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.clubsFace8));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue8Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue8Club));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue8Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue8Club));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipClub));
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.diamondsFace8));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue8Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue8Diamond));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue8Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue8Diamond));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipDiamond));
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.heartsFace8));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue8Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue8Heart));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue8Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue8Heart));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipHeart));
            }
         }
         
      
      case "9":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(GenerationWindow, "default");
         else
            setPipPattern(GenerationWindow, "9");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(GenerationWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(GenerationWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.spadesFace9));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue9Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue9Spade));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue9Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue9Spade));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipSpade));
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.clubsFace9));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue9Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue9Club));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue9Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue9Club));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipClub));
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.diamondsFace9));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue9Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue9Diamond));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue9Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue9Diamond));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipDiamond));
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.heartsFace9));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue9Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue9Heart));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue9Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue9Heart));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipHeart));
            }
         }
         
      
      case "10":
         if($DeckTool::currentDeckEdit.faceStyle $= "full" && $DeckTool::currentDeckEdit.allFacesSpecified)
            setPipPattern(GenerationWindow, "default");
         else
            setPipPattern(GenerationWindow, "10");   
         
         if($DeckTool::currentDeckEdit.allFacesSpecified)
            setFaceStyle(GenerationWindow, $DeckTool::currentDeckEdit.faceStyle);
         else
            setFaceStyle(GenerationWindow, "default");
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.spadesFace10));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue10Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue10Spade));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue10Spade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue10Spade));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipSpade));
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.clubsFace10));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue10Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue10Club));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue10Club));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue10Club));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipClub));
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.diamondsFace10));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue10Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue10Diamond));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue10Diamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue10Diamond));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipDiamond));
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.heartsFace10));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue10Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue10Heart));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValue10Heart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValue10Heart));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipHeart));
            }
         }
         
      
      case "J":
         if($DeckTool::currentDeckEdit.faceStyle $= "full")
            setPipPattern(GenerationWindow, "default");
         else
            setPipPattern(GenerationWindow, "J");   
         
         setFaceStyle(GenerationWindow, $DeckTool::currentDeckEdit.faceStyle);
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.spadesFaceJack));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueJackSpade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueJackSpade));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueJackSpade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueJackSpade));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipSpade));
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.clubsFaceJack));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueJackClub));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueJackClub));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueJackClub));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueJackClub));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipClub));
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.diamondsFaceJack));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueJackDiamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueJackDiamond));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueJackDiamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueJackDiamond));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipDiamond));
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.heartsFaceJack));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueJackHeart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueJackHeart));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueJackHeart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueJackHeart));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipHeart));
            }
         }
         
      
      case "Q":
         if($DeckTool::currentDeckEdit.faceStyle $= "full")
            setPipPattern(GenerationWindow, "default");
         else
            setPipPattern(GenerationWindow, "Q");   
         
         setFaceStyle(GenerationWindow, $DeckTool::currentDeckEdit.faceStyle);
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.spadesFaceQueen));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueQueenSpade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueQueenSpade));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueQueenSpade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueQueenSpade));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipSpade));
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.clubsFaceQueen));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueQueenClub));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueQueenClub));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueQueenClub));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueQueenClub));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipClub));
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.diamondsFaceQueen));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueQueenDiamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueQueenDiamond));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueQueenDiamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueQueenDiamond));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipDiamond));
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.heartsFaceQueen));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueQueenHeart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueQueenHeart));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueQueenHeart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueQueenHeart));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipHeart));
            }
         }
         
      
      case "K":
         if($DeckTool::currentDeckEdit.faceStyle $= "full")
            setPipPattern(GenerationWindow, "default");
         else
            setPipPattern(GenerationWindow, "K");   
         
         setFaceStyle(GenerationWindow, $DeckTool::currentDeckEdit.faceStyle);
         
         if($DeckTool::cardSuit $= "S")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.spadesFaceKing));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueKingSpade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueKingSpade));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueKingSpade));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueKingSpade));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipSpade));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipSpade));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipSpade));
            }
            
         }
         else if($DeckTool::cardSuit $= "C")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.clubsFaceKing));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueKingClub));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueKingClub));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueKingClub));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueKingClub));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipClub));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipClub));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipClub));
            }
         }
         else if($DeckTool::cardSuit $= "D")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.diamondsFaceKing));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueKingDiamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueKingDiamond));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueKingDiamond));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueKingDiamond));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipDiamond));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipDiamond));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipDiamond));
            }
         }
         else if($DeckTool::cardSuit $= "H")
         {
            setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.heartsFaceKing));
            
            if($DeckTool::currentDeckEdit.valueStyle $= "standard")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueKingHeart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueKingHeart));
            }
            else if($DeckTool::currentDeckEdit.valueStyle $= "different")
            {
               setTopValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topValueKingHeart));
               setBottomValueImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomValueKingHeart));
            }
            
            if($DeckTool::currentDeckEdit.pipStyle $= "standard")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "face")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
            }
            else if($DeckTool::currentDeckEdit.pipStyle $= "all")
            {
               setTopPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.topPipHeart));
               setFacePipImages(GenerationWindow, $DeckTool::cardSuit, expandPath($DeckPath @ $DeckTool::currentDeckEdit.facePipHeart));
               setBottomPipImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.bottomPipHeart));
            }
         }
         
      case "back":
         setFaceStyle(GenerationWindow, "full");
         setPipPattern(GenerationWindow, "default");
         setCardFaceImage(GenerationWindow, expandPath($DeckPath @ $DeckTool::currentDeckEdit.cardBackImage));
         
      default:
         error("$DeckTool::cardValue set to an invalid value.  $DeckTool::cardValue = " @ $DeckPath @ $DeckTool::cardValue);
         
   }
   
   Canvas.repaint();
}
function CardGenerationGUI::generateDeck(%this)
{
   echo("*** Generating Deck ***");
   
   echo("*** Phase 1: Creating Base Image ***");
   //create the base image
   %image = new PNGImage(DeckBaseImage);
   
   DeckBaseImage.CreateBaseImage(1872, 1050, 1);
   
   echo("*** Phase 2: Creating Spade Cards ***");
   //capture each card
   for(%i = 0; %i < 13; %i++)
   {
      echo("*** Generating Card ***");
      $DeckTool::cardSuit = "S";
      switch(%i)
      {
         case 0:
            $DeckTool::cardValue = "A";
            %this.refreshGeneratorWindow();
            
         case 1:
            $DeckTool::cardValue = "2";
            %this.refreshGeneratorWindow();
         case 2:
            $DeckTool::cardValue = "3";
            %this.refreshGeneratorWindow();
         case 3:
            $DeckTool::cardValue = "4";
            %this.refreshGeneratorWindow();
         case 4:
            $DeckTool::cardValue = "5";
            %this.refreshGeneratorWindow();
         case 5:
            $DeckTool::cardValue = "6";
            %this.refreshGeneratorWindow();
         case 6:
            $DeckTool::cardValue = "7";
            %this.refreshGeneratorWindow();
         case 7:
            $DeckTool::cardValue = "8";
            %this.refreshGeneratorWindow();
         case 8:
            $DeckTool::cardValue = "9";
            %this.refreshGeneratorWindow();
         case 9:
            $DeckTool::cardValue = "10";
            %this.refreshGeneratorWindow();
         case 10:
            $DeckTool::cardValue = "J";
            %this.refreshGeneratorWindow();
         case 11:
            $DeckTool::cardValue = "Q";
            %this.refreshGeneratorWindow();
         case 12:
            $DeckTool::cardValue = "K";
            %this.refreshGeneratorWindow();
      }
      
      //capture the card
      %tempCard = expandPath("^project/tempGenerateCard.png");
      CaptureScreenArea(0, 0, 144, 210, %tempCard, 1);
      
      echo("*** Adding card " @ %i @ " to Spade Row ***");
      
      //calculate the x position for the base image
      %xPosition = %i * 144;
      
      DeckBaseImage.MergeOn(%xPosition, 0, %tempCard);
   }
   
   echo("*** Phase 3: Creating Club Cards ***");
   //capture each card
   for(%i = 0; %i < 13; %i++)
   {
      $DeckTool::cardSuit = "C";
      switch(%i)
      {
         case 0:
            $DeckTool::cardValue = "A";
            %this.refreshGeneratorWindow();
         case 1:
            $DeckTool::cardValue = "2";
            %this.refreshGeneratorWindow();
         case 2:
            $DeckTool::cardValue = "3";
            %this.refreshGeneratorWindow();
         case 3:
            $DeckTool::cardValue = "4";
            %this.refreshGeneratorWindow();
         case 4:
            $DeckTool::cardValue = "5";
            %this.refreshGeneratorWindow();
         case 5:
            $DeckTool::cardValue = "6";
            %this.refreshGeneratorWindow();
         case 6:
            $DeckTool::cardValue = "7";
            %this.refreshGeneratorWindow();
         case 7:
            $DeckTool::cardValue = "8";
            %this.refreshGeneratorWindow();
         case 8:
            $DeckTool::cardValue = "9";
            %this.refreshGeneratorWindow();
         case 9:
            $DeckTool::cardValue = "10";
            %this.refreshGeneratorWindow();
         case 10:
            $DeckTool::cardValue = "J";
            %this.refreshGeneratorWindow();
         case 11:
            $DeckTool::cardValue = "Q";
            %this.refreshGeneratorWindow();
         case 12:
            $DeckTool::cardValue = "K";
            %this.refreshGeneratorWindow();
      }
      echo("*** Generating Card ***");
      //capture the card
      %tempCard = expandPath("^project/tempGenerateCard.png");
      CaptureScreenArea(0, 0, 144, 210, %tempCard, 1);
      
      echo("*** Adding card " @ %i @ " to Club Row ***");
      
      //calculate the x position for the base image
      %xPosition = %i * 144;
      
      DeckBaseImage.MergeOn(%xPosition, 210, %tempCard);
   }
   
   echo("*** Phase 4: Creating Heart Cards ***");
   //capture each card
   for(%i = 0; %i < 13; %i++)
   {
      $DeckTool::cardSuit = "H";
      switch(%i)
      {
         case 0:
            $DeckTool::cardValue = "A";
            %this.refreshGeneratorWindow();
         case 1:
            $DeckTool::cardValue = "2";
            %this.refreshGeneratorWindow();
         case 2:
            $DeckTool::cardValue = "3";
            %this.refreshGeneratorWindow();
         case 3:
            $DeckTool::cardValue = "4";
            %this.refreshGeneratorWindow();
         case 4:
            $DeckTool::cardValue = "5";
            %this.refreshGeneratorWindow();
         case 5:
            $DeckTool::cardValue = "6";
            %this.refreshGeneratorWindow();
         case 6:
            $DeckTool::cardValue = "7";
            %this.refreshGeneratorWindow();
         case 7:
            $DeckTool::cardValue = "8";
            %this.refreshGeneratorWindow();
         case 8:
            $DeckTool::cardValue = "9";
            %this.refreshGeneratorWindow();
         case 9:
            $DeckTool::cardValue = "10";
            %this.refreshGeneratorWindow();
         case 10:
            $DeckTool::cardValue = "J";
            %this.refreshGeneratorWindow();
         case 11:
            $DeckTool::cardValue = "Q";
            %this.refreshGeneratorWindow();
         case 12:
            $DeckTool::cardValue = "K";
            %this.refreshGeneratorWindow();
      }
      echo("*** Generating Card ***");
      //capture the card
      %tempCard = expandPath("^project/tempGenerateCard.png");
      CaptureScreenArea(0, 0, 144, 210, %tempCard, 1);
      
      echo("*** Adding card " @ %i @ " to Heart Row ***");
      
      //calculate the x position for the base image
      %xPosition = %i * 144;
      
      DeckBaseImage.MergeOn(%xPosition, 420, %tempCard);
   }
   
   echo("*** Phase 5: Creating Diamond Cards ***");
   //capture each card
   for(%i = 0; %i < 13; %i++)
   {
      $DeckTool::cardSuit = "D";
      switch(%i)
      {
         case 0:
            $DeckTool::cardValue = "A";
            %this.refreshGeneratorWindow();
         case 1:
            $DeckTool::cardValue = "2";
            %this.refreshGeneratorWindow();
         case 2:
            $DeckTool::cardValue = "3";
            %this.refreshGeneratorWindow();
         case 3:
            $DeckTool::cardValue = "4";
            %this.refreshGeneratorWindow();
         case 4:
            $DeckTool::cardValue = "5";
            %this.refreshGeneratorWindow();
         case 5:
            $DeckTool::cardValue = "6";
            %this.refreshGeneratorWindow();
         case 6:
            $DeckTool::cardValue = "7";
            %this.refreshGeneratorWindow();
         case 7:
            $DeckTool::cardValue = "8";
            %this.refreshGeneratorWindow();
         case 8:
            $DeckTool::cardValue = "9";
            %this.refreshGeneratorWindow();
         case 9:
            $DeckTool::cardValue = "10";
            %this.refreshGeneratorWindow();
         case 10:
            $DeckTool::cardValue = "J";
            %this.refreshGeneratorWindow();
         case 11:
            $DeckTool::cardValue = "Q";
            %this.refreshGeneratorWindow();
         case 12:
            $DeckTool::cardValue = "K";
            %this.refreshGeneratorWindow();
      }
      echo("*** Generating Card ***");
      //capture the card
      %tempCard = expandPath("^project/tempGenerateCard.png");
      CaptureScreenArea(0, 0, 144, 210, %tempCard, 1);
      
      echo("*** Adding card " @ %i @ " to Diamond Row ***");
      
      //calculate the x position for the base image
      %xPosition = %i * 144;
      
      DeckBaseImage.MergeOn(%xPosition, 630, %tempCard);
   }
   
   echo("*** Phase 6: Creating Backing Cards ***");
   //capture each card
   for(%i = 0; %i < 13; %i++)
   {
      $DeckTool::cardSuit = "D";
      $DeckTool::cardValue = "back";
      %this.refreshGeneratorWindow();

      echo("*** Generating Card ***");
      //capture the card
      %tempCard = expandPath("^project/tempGenerateCard.png");
      CaptureScreenArea(0, 0, 144, 210, %tempCard, 1);
      
      echo("*** Adding card " @ %i @ " to Backing Row ***");
      
      //calculate the x position for the base image
      %xPosition = %i * 144;
      
      DeckBaseImage.MergeOn(%xPosition, 840, %tempCard);
   }
   
   echo("*** Phase 7: Save Sprite Sheet ***");
   
   %spriteSheet = expandPath("^project/data/images/" @ $DeckTool::currentDeckEdit.Name @ ".png");
   
   if(DeckBaseImage.SaveImage(%spriteSheet))
      createDeckSprite($DeckTool::currentDeckEdit.internalName, "data/images/" @ $DeckTool::currentDeckEdit.Name);
   else
      error("ERROR CREATING SPRITE FOR DECK");   
   
   Canvas.popDialog(CardGenerationGUI);
}

function createDeckSprite(%name, %image)
{
   // 1. Create the datablock
   %datablock = "datablock ImageAsset(" @ %name @ ")\n";
   %datablock = %datablock SPC "{\n";
   %datablock = %datablock SPC "imageFile = \"" @ %image @ "\";\n";
   %datablock = %datablock SPC "imageMode = CELL;\n";
   %datablock = %datablock SPC "useHDImage = 0;\n";
   %datablock = %datablock SPC "frameCount = -1;\n";
   %datablock = %datablock SPC "filterMode = NONE;\n";
   %datablock = %datablock SPC "filterPad = 0;\n";
   %datablock = %datablock SPC "preferPerf = 1;\n";
   %datablock = %datablock SPC "cellRowOrder = 1;\n";
   %datablock = %datablock SPC "cellOffsetX = 0;\n";
   %datablock = %datablock SPC "cellOffsetY = 0;\n";
   %datablock = %datablock SPC "cellStrideX = 0;\n";
   %datablock = %datablock SPC "cellStrideY = 0;\n";
   %datablock = %datablock SPC "cellCountX = 13;\n";
   %datablock = %datablock SPC "cellCountY = 5;\n";
   %datablock = %datablock SPC "cellWidth = 144;\n";
   %datablock = %datablock SPC "cellHeight = 210;\n";
   %datablock = %datablock SPC "preload = 1;\n";
   %datablock = %datablock SPC "allowUnload = 0;\n";
   %datablock = %datablock SPC "compressPVR = 0;\n";
   %datablock = %datablock SPC "optimised = 0;\n";
   %datablock = %datablock SPC "force16bit = 0;\n";
   %datablock = %datablock @ "};\n";
   
   // Eval the script
   eval(%datablock);
   
   // If you need to recompile based on changes to the imageMap:
   // %name.compile();
   
   // 2. Add image map to ProjectNameTags
   if(!ProjectNameTags.isMember(%name))
      ProjectNameTags.add(%name);

   // 3. Tag image map
   %tag = "Deck";
   %tagID = ProjectNameTags.getTagId(%tag);
   ProjectNameTags.tag(%name.getID(), %tagID);

   // 4. LBProjectObj.addDatablock
   LBProjectObj.addDatablock( %name, true );

   // 5. Refresh the advanced editor
   GuiFormManager::SendContentMessage($LBCreateSiderBar, %this, "refreshall 0");
   
   // 6. Refresh the Asset Library
   AssetLibrary.schedule(100, "updateGui");
}