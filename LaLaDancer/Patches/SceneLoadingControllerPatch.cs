using HarmonyLib;
using RiftOfTheNecroManager;
using Shared.SceneLoading;

namespace LaLaDancer.Patches;


[HarmonyPatch(typeof(SceneLoadingController))]
public static class SceneLoadingControllerPatch {
    [HarmonyPatch(nameof(SceneLoadingController.GoToSceneRoutine))]
    [HarmonyPrefix]
    public static void GoToSceneRoutine(ref SceneLoadData.SceneToLoadMetaData sceneToLoadMetaData, ref bool shouldShowLoadingScreen) {
        if(Config.QOL.SkipSplashScreen && sceneToLoadMetaData.SceneName == "SplashScreen") {
            sceneToLoadMetaData.SceneName = "MainMenu";
            shouldShowLoadingScreen = true;
        }
    }
}
