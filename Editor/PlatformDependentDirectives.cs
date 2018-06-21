#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public class PlatformDependentDirectives
{
    // Custom compiler defines:

    // LOWENDDEVICE : Mobiles (Phones, XR and consoles) and SmartTV
    // HIGHENDDEVICE : Desktop, Consoles
    // MOBILE: Mobile Phones
    // DESKTOP: Standalone + WebPlayer
    // WEBPlatform: WebPlayer, WebGL(HTML5), Windows Store
    // SMARTTV: Smart TV
    // XR : Is a XR device
    static PlatformDependentDirectives()
    {
        CreateDeviceGeneralizations ();
    }

#if !UNITY_2017_1_OR_NEWER
    private static BuildTargetGroup[] highEndBuildTargetGroups = new BuildTargetGroup[]{BuildTargetGroup.Standalone,BuildTargetGroup.WebGL,BuildTargetGroup.XboxOne,BuildTargetGroup.PS4, BuildTargetGroup.WebGL,BuildTargetGroup.WSA};
    private static BuildTargetGroup[] lowEndBuildTargetGroups = new BuildTargetGroup[]{BuildTargetGroup.Android,BuildTargetGroup.iOS,BuildTargetGroup.WSA,BuildTargetGroup.Tizen,BuildTargetGroup.SamsungTV,BuildTargetGroup.PSM,BuildTargetGroup.PSP2};
	private static BuildTargetGroup[] mobileBuildTargetGroups = new BuildTargetGroup[]{BuildTargetGroup.Android,BuildTargetGroup.iOS,BuildTargetGroup.WSA,BuildTargetGroup.Tizen};
    private static BuildTargetGroup[] desktopBuildTargetGroups = new BuildTargetGroup[]{BuildTargetGroup.Standalone,BuildTargetGroup.WebGL, BuildTargetGroup.WebGL};
    private static BuildTargetGroup[] webBuildTargetGroups = new BuildTargetGroup[]{BuildTargetGroup.WebGL, BuildTargetGroup.WebGL,BuildTargetGroup.WSA};
#elif !UNITY_2017_2_OR_NEWER
    private static BuildTargetGroup[] highEndBuildTargetGroups = new BuildTargetGroup[]{BuildTargetGroup.Standalone,BuildTargetGroup.WebGL,BuildTargetGroup.XboxOne,BuildTargetGroup.PS4, BuildTargetGroup.WebGL,BuildTargetGroup.WSA};
    private static BuildTargetGroup[] lowEndBuildTargetGroups = new BuildTargetGroup[]{BuildTargetGroup.Android,BuildTargetGroup.iOS,BuildTargetGroup.WSA,BuildTargetGroup.Tizen,BuildTargetGroup.SamsungTV,BuildTargetGroup.PSP2};
	private static BuildTargetGroup[] mobileBuildTargetGroups = new BuildTargetGroup[]{BuildTargetGroup.Android,BuildTargetGroup.iOS,BuildTargetGroup.WSA,BuildTargetGroup.Tizen};
    private static BuildTargetGroup[] desktopBuildTargetGroups = new BuildTargetGroup[]{BuildTargetGroup.Standalone,BuildTargetGroup.WebGL, BuildTargetGroup.WebGL};
    private static BuildTargetGroup[] webBuildTargetGroups = new BuildTargetGroup[]{BuildTargetGroup.WebGL, BuildTargetGroup.WebGL,BuildTargetGroup.WSA};
#elif !UNITY_2017_3_OR_NEWER
    private static BuildTargetGroup[] highEndBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.WebGL, BuildTargetGroup.XboxOne, BuildTargetGroup.PS4, BuildTargetGroup.WebGL, BuildTargetGroup.WSA };
    private static BuildTargetGroup[] lowEndBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA, BuildTargetGroup.Tizen, BuildTargetGroup.PSP2 };
    private static BuildTargetGroup[] mobileBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA, BuildTargetGroup.Tizen };
    private static BuildTargetGroup[] desktopBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.WebGL, BuildTargetGroup.WebGL };
    private static BuildTargetGroup[] webBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.WebGL, BuildTargetGroup.WebGL, BuildTargetGroup.WSA };
    //private static BuildTargetGroup[] xrBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.Tizen, BuildTargetGroup.PS4, BuildTargetGroup.WebGL };
    //private static BuildTargetGroup[] tVBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.tvOS };
#elif !UNITY_2017_4_OR_NEWER
    private static BuildTargetGroup[] highEndBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.WebGL, BuildTargetGroup.XboxOne, BuildTargetGroup.PS4, BuildTargetGroup.WebGL, BuildTargetGroup.WSA };
    private static BuildTargetGroup[] lowEndBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA, BuildTargetGroup.PSP2 };
    private static BuildTargetGroup[] mobileBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA };
    private static BuildTargetGroup[] desktopBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.WebGL, BuildTargetGroup.WebGL };
    private static BuildTargetGroup[] webBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.WebGL, BuildTargetGroup.WebGL, BuildTargetGroup.WSA };
#elif !UNITY_2018_1_OR_NEWER
    private static BuildTargetGroup[] highEndBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.WebGL, BuildTargetGroup.XboxOne, BuildTargetGroup.PS4, BuildTargetGroup.WebGL, BuildTargetGroup.WSA };
    private static BuildTargetGroup[] lowEndBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA, BuildTargetGroup.PSP2 };
    private static BuildTargetGroup[] mobileBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA };
    private static BuildTargetGroup[] desktopBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.WebGL, BuildTargetGroup.WebGL };
    private static BuildTargetGroup[] webBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.WebGL, BuildTargetGroup.WebGL, BuildTargetGroup.WSA };
#elif !UNITY_2018_2_OR_NEWER
    private static BuildTargetGroup[] highEndBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.WebGL, BuildTargetGroup.XboxOne, BuildTargetGroup.PS4, BuildTargetGroup.WebGL, BuildTargetGroup.WSA };
    private static BuildTargetGroup[] lowEndBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA, BuildTargetGroup.PSP2 };
    private static BuildTargetGroup[] mobileBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA, };
    private static BuildTargetGroup[] desktopBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.WebGL, BuildTargetGroup.WebGL };
    private static BuildTargetGroup[] webBuildTargetGroups = new BuildTargetGroup[] { BuildTargetGroup.WebGL, BuildTargetGroup.WebGL, BuildTargetGroup.WSA };
#endif

    private static void CreateDeviceGeneralizations()
    {
		for (int i = 0; i < lowEndBuildTargetGroups.Length; i++)
		{
			if (!PlayerSettings.GetScriptingDefineSymbolsForGroup(lowEndBuildTargetGroups[i]).Contains("LOWENDDEVICE"))
			    PlayerSettings.SetScriptingDefineSymbolsForGroup(lowEndBuildTargetGroups[i],"LOWENDDEVICE");
		}
        for (int i = 0; i < highEndBuildTargetGroups.Length; i++)
		{
			if (!PlayerSettings.GetScriptingDefineSymbolsForGroup(highEndBuildTargetGroups[i]).Contains("HIGHENDDEVICE"))
				PlayerSettings.SetScriptingDefineSymbolsForGroup(highEndBuildTargetGroups[i], PlayerSettings.GetScriptingDefineSymbolsForGroup(highEndBuildTargetGroups[i]) + ";" + "HIGHENDDEVICE");
		}
		for (int i = 0; i < mobileBuildTargetGroups.Length; i++)
		{
			if (!PlayerSettings.GetScriptingDefineSymbolsForGroup(mobileBuildTargetGroups[i]).Contains("MOBILE"))
				PlayerSettings.SetScriptingDefineSymbolsForGroup(mobileBuildTargetGroups[i], PlayerSettings.GetScriptingDefineSymbolsForGroup(mobileBuildTargetGroups[i]) + ";" + "MOBILE");
		}
		for (int i = 0; i < desktopBuildTargetGroups.Length; i++)
		{
			if (!PlayerSettings.GetScriptingDefineSymbolsForGroup(desktopBuildTargetGroups[i]).Contains("DESKTOP"))
				PlayerSettings.SetScriptingDefineSymbolsForGroup(desktopBuildTargetGroups[i], PlayerSettings.GetScriptingDefineSymbolsForGroup(desktopBuildTargetGroups[i]) + ";" + "DESKTOP");
		}
		for (int i = 0; i < webBuildTargetGroups.Length; i++)
		{
			if (!PlayerSettings.GetScriptingDefineSymbolsForGroup(webBuildTargetGroups[i]).Contains("WEBPLATFORM"))
				PlayerSettings.SetScriptingDefineSymbolsForGroup(webBuildTargetGroups[i], PlayerSettings.GetScriptingDefineSymbolsForGroup(webBuildTargetGroups[i]) + ";" + "WEBPLATFORM");
        }
    }
}
#endif