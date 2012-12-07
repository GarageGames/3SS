<?php

function CompileXCode($dir, $project)
{
   global $SETTINGS, $COMPILER;

   $descriptorspec = array(
      0 => array("pipe", "r"),  // stdin is a pipe that the child will read from
      1 => array("pipe", "w"),  // stdout is a pipe that the child will write to
      2 => array("pipe", "w")   // stderr is a pipe that the child will write to
      );

   $command = $SETTINGS['buildcmd']." ".$SETTINGS['options']." -project \"".$project."\"";
   $process = proc_open($command, $descriptorspec, $pipes, $dir, NULL);

   if (is_resource($process)) {

      echo("\nBuilding with ".$COMPILER."\n");

      while(!feof($pipes[1]))
         echo(fgets($pipes[1], 1024));

      fclose($pipes[0]);
      fclose($pipes[1]);
      fclose($pipes[2]);

      $return_value = proc_close($process);

      if ($return_value != 0)
         Error("Error compiling: ".$command." ".$dir);

   }
   else
      Error("Unable to open XCode process, XCode installed?");
}

// On Windows, we compile solutions
function CompileSolution($dir, $sln)
{
   global $SETTINGS, $COMPILER;

   $DXSDK_DIR = getenv("DXSDK_DIR");

   if (!$DXSDK_DIR)
      Error("DXSDK_DIR variable is not set. Make sure the DirectX SDK is installed properly.");

   $descriptorspec = array(
      0 => array("pipe", "r"),  // stdin is a pipe that the child will read from
      1 => array("pipe", "w"),  // stdout is a pipe that the child will write to
      2 => array("pipe", "w")   // stderr is a pipe that the child will write to
      );

   $proc_options = array('bypass_shell' => "1");
   
   echo("\nDescriptor ".$descriptorspec."\n");
   echo("\nPipes ".$pipes."\n");
   echo("\nDir ".$dir."\n");
   
   $process = proc_open('C:\\Windows\\System32\cmd.exe', $descriptorspec, $pipes, $dir, NULL);

   if (is_resource($process)) {

      echo("\nInitializing ".$COMPILER." environment variables:\n   ====>".$SETTINGS['envvar']."\n\n");
      fwrite($pipes[0], $SETTINGS['envvar']." x86\n");

      fwrite($pipes[0], '"'.$DXSDK_DIR."Utilities\Bin\dx_setenv.cmd\"\n");

      fwrite($pipes[0], $SETTINGS['buildcmd']." \"$sln\" ".$SETTINGS['options']."\n");

      fwrite($pipes[0], "exit\n");

      while(!feof($pipes[1]))
         echo(fgets($pipes[1], 1024));

      fclose($pipes[0]);
      fclose($pipes[1]);
      fclose($pipes[2]);

      $return_value = proc_close($process);

      if ($return_value != 0)
         Error("Error compiling $sln");

   }
   else
      Error("Unable to open process cmd.exe, permission issue?");

}

function GenerateProjects($dir, $config)
{
   global $MAC;

   $descriptorspec = array(
      0 => array("pipe", "r"),  // stdin is a pipe that the child will read from
      1 => array("pipe", "w"),  // stdout is a pipe that the child will write to
      2 => array("pipe", "w")   // stderr is a pipe that the child will write to
      );

   $command = 'C:\\Windows\\System32\cmd.exe';

   if ($MAC)
      $command = "/bin/sh";

   $proc_options = array('bypass_shell' => "1");
   $process = proc_open($command, $descriptorspec, $pipes, $dir, NULL);

   if (is_resource($process)) {

      if ($MAC)
         fwrite($pipes[0], "/usr/bin/php ../../Tools/projectGenerator/projectGenerator.php buildFiles/config/".$config."\n");
      else
         fwrite($pipes[0], "..\\..\\engine\\bin\\php\\php ..\\..\\Tools\\projectGenerator\\projectGenerator.php buildFiles/config/".$config." ..\\..\n");

      fwrite($pipes[0], "exit\n");

      while(!feof($pipes[1]))
         echo(fgets($pipes[1], 1024));

      fclose($pipes[0]);
      fclose($pipes[1]);
      fclose($pipes[2]);

      $return_value = proc_close($process);

      if ($return_value != 0)
         Error("Error generating project for $dir");

   }
   else
      Error("Unable to open process cmd.exe, permission issue?");
}

?>