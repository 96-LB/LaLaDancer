using System.Collections;
using HarmonyLib;
using RhythmRift;

namespace LaLaDancer.Patches;


[HarmonyPatch(typeof(RRStageController))]
public static class RRStageControllerPatch {
    [HarmonyPatch(nameof(RRStageController.InitializeBackgroundRoutine))]
    [HarmonyPostfix]
    public static void InitializeBackgroundRoutine(RRStageController __instance, ref IEnumerator __result) {
        // since the original function is a coroutine, we need to wrap the output to properly postfix
        var original = __result;
        __result = Wrapper();
        
        IEnumerator Wrapper() {
            yield return original;
            
            if(Config.Bugfixes.CustomParticles && __instance._customTrackVfxConfig?.CustomParticleImagePath != null) {
                var textureSheetAnimation = __instance._rhythmRiftBackgroundFx._customCharacterParticles.textureSheetAnimation;
                textureSheetAnimation.startFrameMultiplier = 1;
            }
        }
    }
}
