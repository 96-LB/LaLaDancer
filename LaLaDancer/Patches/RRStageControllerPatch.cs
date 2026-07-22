using System.Collections;
using HarmonyLib;
using RhythmRift;
using RiftOfTheNecroManager;
using Shared.Audio;
using Shared.RhythmEngine;
using UnityEngine;

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
                // this is erroneously set to 0.75 for vanilla spritesheets
                var textureSheetAnimation = __instance._rhythmRiftBackgroundFx._customCharacterParticles.textureSheetAnimation;
                textureSheetAnimation.startFrameMultiplier = 1;
            }
        }
    }
    
    [HarmonyPatch(nameof(RRStageController.HandleBeatUpdate))]
    [HarmonyPostfix]
    public static void HandleBeatUpdate(RRStageController __instance, FmodTimeCapsule fmodTimeCapsule) {
        if(Config.Bugfixes.BlademasterSfx) {
            // blademasters use the global bpm flag to set the speed of their sound effect
            // the game sets this flag only at the start of the beatmap, so bpm changes cause problems
            AudioManager.Instance.SetGlobalBPM(__instance.BeatmapPlayer.GetCurrentBeatLengthInSeconds(fmodTimeCapsule.CurrentBeatNumber));
        }
    }
    
    [HarmonyPatch(nameof(RRStageController.BeginPlay))]
    [HarmonyPrefix]
    public static void BeginPlay(RRStageController __instance) {
        if(Config.Bugfixes.Countdown && __instance._beatmaps.Count > 0) {
            // countdowns for custom charts should use the bpm at the start beat, not the hardcoded countdownBpm
            var startBeat = Mathf.Max(2, __instance._practiceModeStartBeatNumber - __instance._practiceModeTotalBeatsSkippedBeforeStartBeatmap);
            var beatmap = __instance._beatmaps[0];
            var bpm = 60 / (beatmap.GetTimeFromBeatNumber(startBeat) - beatmap.GetTimeFromBeatNumber(startBeat - 1));
            __instance._customTrackCountdownBpm = bpm;
        }
    }
}
