using HarmonyLib;
using RhythmRift;
using RiftOfTheNecroManager;
using Shared;
using Shared.Audio;
using Shared.RhythmEngine;
using Shared.RiftInput;

namespace LaLaDancer.Patches;


[HarmonyPatch(typeof(RRStageController))]
public static class RRStageControllerPatch {
    [HarmonyPatch(nameof(RRStageController.HandleBeatUpdate))]
    [HarmonyPostfix]
    public static void HandleBeatUpdate(RRStageController __instance, FmodTimeCapsule fmodTimeCapsule) {
        if(Config.Bugfixes.BlademasterSfx) {
            // blademasters use the global bpm flag to set the speed of their sound effect
            // the game sets this flag only at the start of the beatmap, so bpm changes cause problems
            var bpm = 60 / __instance.BeatmapPlayer.GetCurrentBeatLengthInSeconds(fmodTimeCapsule.CurrentBeatNumber);
            AudioManager.Instance.SetGlobalBPM(bpm);
        }
    }
    
    [HarmonyPatch(nameof(RRStageController.BeginPlay))]
    [HarmonyPrefix]
    public static void BeginPlay(RRStageController __instance) {
        if(Config.Bugfixes.Countdown && __instance._beatmaps.Count > 0) {
            // countdowns for custom charts should use the bpm at the start beat, not the hardcoded countdownBpm
            var startBeat = __instance._practiceModeStartBeatNumber - __instance._practiceModeTotalBeatsSkippedBeforeStartBeatmap;
            var bpm = 60 / __instance.BeatmapPlayer.GetCurrentBeatLengthInSeconds((int)startBeat);
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
