using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using System.Collections.Generic;


public class GameBuild
{
  [MenuItem("Tools/BuildGame/Windows 64 Build")]
  public static void BuildGameWin64()
  {
    BuildGame(BuildTarget.StandaloneWindows64);
  }

  [MenuItem("Tools/BuildGame/WebGl Build")]
  public static void BuildWebGl()
  {
    BuildGame(BuildTarget.WebGL);
  }

  private static void BuildGame(BuildTarget p_target)
  {
    string targetName = BuildPipeline.GetBuildTargetName(p_target);
    string buildpath = "Build/Lager25_" + targetName + "/";
    BuildOptions buildoptions = BuildOptions.None;

    EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
    List<string> scenes = new List<string>();

    for(int index = 0; index < buildScenes.Length; ++index)
    {
      if(buildScenes[index].path.Contains("/Game/"))
      {
        scenes.Add(buildScenes[index].path);
      }
    }

    switch(p_target)
    {
      case BuildTarget.StandaloneWindows64:
        buildpath += "Lager25_win64.exe";
        break;
      case BuildTarget.WebGL:
        break;
    }

    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
    buildPlayerOptions.scenes = scenes.ToArray();
    buildPlayerOptions.target = p_target;
    buildPlayerOptions.locationPathName = buildpath;
    buildPlayerOptions.options = buildoptions;

    BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

    BuildSummary summary = report.summary;

    Debug.Log("Lager25 Build " + PlayerSettings.bundleVersion + ":" + summary.result);
    Debug.Log("Version:" + PlayerSettings.bundleVersion);
    Debug.Log("Scenes built:" + scenes.Count);
    int sceneIndex = 1;
    foreach(var scene in scenes)
    {
      Debug.Log("Scene " + sceneIndex + "/" + scenes.Count + ":" + scene);
      ++sceneIndex;
    }
    Debug.Log("Size(Mb):" + (summary.totalSize / 1024f) / 1024f );
    Debug.Log("Time:" + summary.totalTime);
    Debug.Log("Started:" + summary.buildStartedAt);
    Debug.Log("Ended:" + summary.buildEndedAt);
    Debug.Log("Errors:" + summary.totalErrors);
    Debug.Log("Warnings:" + summary.totalWarnings);
    Debug.Log(summary.outputPath);
  }
}
