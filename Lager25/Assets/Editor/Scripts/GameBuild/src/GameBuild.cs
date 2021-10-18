using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;


public class GameBuild
{
  public static string[] m_scenes = { "Assets/Scenes/PlayScene.unity" };
  
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

    switch(p_target)
    {
      case BuildTarget.StandaloneWindows64:
        buildpath += "Lager25_win64.exe";
        break;
      case BuildTarget.WebGL:
        break;
    }
    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
    buildPlayerOptions.scenes = m_scenes;
    buildPlayerOptions.target = p_target;
    buildPlayerOptions.locationPathName = buildpath;
    buildPlayerOptions.options = buildoptions;

    BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

    BuildSummary summary = report.summary;

    bool assertResult = summary.result == BuildResult.Succeeded;
    Debug.Assert(assertResult, "Lager25 Build " + PlayerSettings.bundleVersion + ":" + summary.result);
    Debug.Assert(assertResult, "Version:" + PlayerSettings.bundleVersion);
    Debug.Assert(assertResult, "Size(Mb):" + (summary.totalSize / 1024f) / 1024f );
    Debug.Assert(assertResult, "Time:" + summary.totalTime);
    Debug.Assert(assertResult, "Started:" + summary.buildStartedAt);
    Debug.Assert(assertResult, "Ended:" + summary.buildEndedAt);
    Debug.Assert(assertResult, "Errors:" + summary.totalErrors);
    Debug.Assert(assertResult, "Warnings:" + summary.totalWarnings);
    Debug.Assert(assertResult, summary.outputPath);
  }
}
