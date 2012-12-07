<?php

function RemoveFile($f)
{
   if (is_file($f))
   {
      echo "Removing $f\n";
      unlink($f);
   }
   else
      echo "$f is not a file\n";
}

function RemoveDir($sDir)
{
   if (is_dir($sDir))
   {
      $sDir = rtrim($sDir, '/');
      $oDir = dir($sDir);

      while (($sFile = $oDir->read()) !== false)
      {
         if ($sFile != '.' && $sFile != '..')
            if (!is_link("$sDir/$sFile") && is_dir("$sDir/$sFile"))
               RemoveDir("$sDir/$sFile");
            else
               unlink("$sDir/$sFile");
      }

      $oDir->close();

      echo "Removing $sDir\n";
      rmdir($sDir);

      return true;
   }

   return false;
}

function RemoveFilePattern($sDir, $pattern, $skipPattern, $recursive)
{
   echo "Removing $pattern from $sDir\n";

   if (strlen($pattern) == 0)
      return;

   if (is_dir($sDir))
   {
      $sDir = rtrim($sDir, '/');
      $oDir = dir($sDir);

      while (($sFile = $oDir->read()) !== false)
      {
         if ($sFile == '.' || $sFile == '..')
         {
            //echo "Skipping . or ..\n";
            continue;
         }

         if ($recursive && is_dir("$sDir/$sFile"))
         {
            //echo "Recursing into $sDir/$sFile\n";
            RemoveFilePattern("$sDir/$sFile", $pattern, $skipPattern, $recursive);
         }

         // Test against our skip pattern
         if (strlen($skipPattern) > 0)
         {
            if ($skipPattern[0] == '.')
            {
               // Handle a file extension pattern
               if (strcasecmp(strrchr($sFile, '.'), $skipPattern) == 0)
               {
                  echo "Skipping $sFile because it ends in $skipPattern\n";
                  continue;
               }
            }
            else
            {
               // Handle a direct pattern
               if (strcasecmp($sFile, $skipPattern) == 0)
               {
                  echo "Skipping $sFile because it matches $skipPattern\n";
                  continue;
               }
            }
         }

         if ($pattern[0] == '.')
         {
            // Handle a file extension pattern
            if (strcasecmp(strrchr($sFile, '.'), $pattern) != 0)
            {
               echo "Skipping $sFile because it doesn't end in $pattern\n";
               continue;
            }

            echo "Removing $sDir/$sFile\n";
            RemoveFile("$sDir/$sFile");
         }
         else
         {
            // Handle a direct pattern
            if (strcasecmp($sFile, $pattern) != 0)
            {
               echo "Skipping $sFile because it doesn't match $pattern\n";
               continue;
            }

            echo "Removing $sDir/$sFile\n";

            if (is_dir("$sDir/$sFile"))
               RemoveDir("$sDir/$sFile");
            else
               RemoveFile("$sDir/$sFile");
         }
      }

      $oDir->close();

      return true;
   }

   return false;
}

function CopyDir($srcdir, $dstdir, $recursive, $verbose = false)
{
   global $EXCLUDES, $MAC;

   if (!is_dir($dstdir))
      mkdir($dstdir);

   if ($curdir = opendir($srcdir))
   {
      while ($file = readdir($curdir))
      {
         $found = false;
         foreach($EXCLUDES as $v)
         {
            if ($v[0] == '.' && strcmp($v, ".svn") != 0)
            {
               // Handle a file extension pattern
               if (strcasecmp(strrchr($file, '.'), $v) == 0)
               {
                  echo "Skipping $file because it ended with $v\n";
                  $found = true;
                  break;
               }
            }
            else
            {
               // Check against the file name directly
               if (strcasecmp($file, $v) == 0)
               {
                  echo "Skipping $file because it is named $v\n";
                  $found = true;
                  break;
               }
            }
         }

         if ($found)
            continue;

         if ($file != '.' && $file != '..')
         {
            $srcfile = $srcdir . '/' . $file;
            $dstfile = $dstdir . '/' . $file;
            if (is_file($srcfile))
            {
               if (is_file($dstfile))
                  $ow = filemtime($srcfile) - filemtime($dstfile);
               else
                  $ow = 1;

               if ($ow > 0)
               {
                  if ($verbose)
                     echo "Copying $srcfile\n";

                  if (copy($srcfile, $dstfile))
                  {
                     touch($dstfile, filemtime($srcfile));
                     
                     if ($MAC)
                        chmod($dstfile, fileperms($srcfile));
                  }
                  else
                     Error("Error: File '$srcfile' could not be copied!");
               }                  
            }
            else if ($recursive && is_dir($srcfile))
               CopyDir($srcfile, $dstfile, $recursive, $verbose);
         }
      }

      closedir($curdir);
   }
}

function MoveDir($srcdir, $dstdir, $verbose = false)
{
   if ($verbose)
      echo "Moving $srcdir\n";

   rename($srcdir, $dstdir);
}

function CopyFile($srcfile, $dstfile, $verbose = false)
{
   if ($verbose)
      echo "Copying $srcfile\n";
	  
   echo($srcfile);
   echo($destfile);
   
   copy($srcfile, $dstfile);
}

?>