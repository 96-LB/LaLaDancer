using System.Collections;
using HarmonyLib;
using RhythmRift;
using RiftOfTheNecroManager;
using Shared;
using Shared.Audio;
using Shared.RhythmEngine;
using Shared.RiftInput;
using Unity.Mathematics;
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
    
    [HarmonyPatch(nameof(RRStageController.CanPause), MethodType.Getter)]
    [HarmonyPostfix]
    public static void CanPause(RRStageController __instance, ref bool __result) {
        if(Config.QOL.CountdownPausing) {
            // the game overly restricts when you can pause, so we relax the rules
            __result = !__instance._isShowingCalibrationResults
                && !__instance._isPostGameScreenVisible
                && !__instance._stageFlowUiController.IsShowingPauseScreen;
        }
    }
    
    [HarmonyPatch(typeof(StageController<RRBeatmapPlayer>), nameof(StageController<>.HandleUnpauseRoutine))]
    [HarmonyPostfix]
    public static void HandleUnpauseRoutine(RRStageController __instance) {
        if(Config.QOL.CountdownPausing && !__instance._isDisplayingDialogue) {
            // to enable unpausing during the countdown, we switch back to gameplay sooner
            // otherwise, the pause input gets eaten by the ui context
            InputAccessor.Instance.EnterInputContext(InputAccessor.InputContext.Gameplay, shouldDisableOtherMaps: true);
        }
    }
        
    [HarmonyPatch(typeof(StageController<RRBeatmapPlayer>), nameof(StageController<>.HandlePauseRoutine))]
    [HarmonyPostfix]
    public static void HandlePauseRoutine(RRStageController __instance) {
        if(Config.QOL.CountdownPausing) {
            // since we can unpause during the countdown, we need to stop it if it's happening
            __instance._pauseRoutine?.Pipe(__instance.StopCoroutine);
        }
    }
}
