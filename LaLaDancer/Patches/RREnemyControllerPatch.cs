using HarmonyLib;
using RhythmRift;
using RhythmRift.Enemies;
using Shared.RhythmEngine;
using UnityEngine;

namespace LaLaDancer.Patches;


[HarmonyPatch(typeof(RREnemyController))]
public static class RREnemyControllerPatch {
    [HarmonyPatch(nameof(RREnemyController.HasTimingAlteringTrapBelow))]
    [HarmonyPostfix]
    public static void HasTimingAlteringTrapsBelow(ref bool __result) {
        // this occasionally misfires, so we just disable the function when the bugfix is turned on
        __result &= !Config.Bugfixes.PredictiveSfx;
    }
    
    [HarmonyPatch(nameof(RREnemyController.TryQueueActionRowSoundsForEnemy))]
    [HarmonyPrefix]
    public static bool TryQueueActionRowSoundsForEnemy(float timeUntilNextBeat) {
        // sound effects are queued way too early; run this function no more than 100ms before we need to
        // minimum margin of 200ms in case the enemy is hit early and stops queueing audio (175ms input window)
        var margin = Mathf.Max(0.2f, LatencyManager.AudioLatencyOffset - LatencyManager.VideoLatencyOffset + .1f);
        return !Config.Bugfixes.PredictiveSfx || (timeUntilNextBeat < margin);
    }
}
