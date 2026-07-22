using System.Collections;
using HarmonyLib;
using RhythmRift;
using RiftOfTheNecroManager;
using Shared.Audio;
using Shared.RhythmEngine;

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
    
    [HarmonyPatch(nameof(RRStageController.HandleBeatUpdate))]
    [HarmonyPostfix]
    public static void HandleBeatUpdate(RRStageController __instance, FmodTimeCapsule fmodTimeCapsule) {
        if(Config.Bugfixes.BlademasterSfx) {
            AudioManager.Instance.SetGlobalBPM(__instance.BeatmapPlayer.GetCurrentBeatLengthInSeconds(fmodTimeCapsule.CurrentBeatNumber));
        }
    }
    
    [HarmonyPatch(nameof(RRStageController.BeginPlay))]
    [HarmonyPrefix]
    public static void BeginPlay(RRStageController __instance) {
        if(Config.Bugfixes.Countdown && __instance._beatmaps.Count > 0) {
            var startBeat = __instance._isPracticeMode ? __instance._practiceModeStartBeatNumber - __instance._practiceModeTotalBeatsSkippedBeforeStartBeatmap : 0f;
            var beatmap = __instance._beatmaps[0];
            var bpm = 60 / (beatmap.GetTimeFromBeatNumber(startBeat) - beatmap.GetTimeFromBeatNumber(startBeat - 1));
            __instance._customTrackCountdownBpm = bpm;
        }
    }
}
