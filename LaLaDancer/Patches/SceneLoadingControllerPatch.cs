using HarmonyLib;
using Shared.SceneLoading;

namespace LaLaDancer.Patches;


[HarmonyPatch(typeof(SceneLoadingController))]
public static class SceneLoadingControllerPatch {
    [HarmonyPatch(nameof(SceneLoadingController.GoToSceneRoutine))]
    [HarmonyPrefix]
    public static void GoToSceneRoutine(ref SceneLoadData.SceneToLoadMetaData sceneToLoadMetaData, ref bool shouldShowLoadingScreen) {
        if(Config.QOL.SkipSplashScreen && sceneToLoadMetaData.SceneName == "SplashScreen") {
            // redirect splash screen to main menu
            sceneToLoadMetaData.SceneName = "MainMenu";
            shouldShowLoadingScreen = true;
        }
    }
}
