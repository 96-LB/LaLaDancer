using HarmonyLib;
using LaLaDancer;
using RhythmRift;
using RhythmRift.Enemies;
using Shared.RhythmEngine;

namespace LalaDancer.Patches;


[HarmonyPatch(typeof(RREnemyController))]
public static class RREnemyControllerPatch {
    [HarmonyPatch(nameof(RREnemyController.HasTimingAlteringTrapBelow))]
    [HarmonyPostfix]
    public static void HasTimingAlteringTrapsBelow(ref bool __result) {
        // this occasionally misfires, so we just disable when the bugfix is turned on
        __result &= !Config.Bugfixes.PredictiveHitSounds;
    }
    
    [HarmonyPatch(nameof(RREnemyController.TryQueueActionRowSoundsForEnemy))]
    [HarmonyPrefix]
    public static bool TryQueueActionRowSoundsForEnemy(float timeUntilNextBeat) {
        // sound effects are queued way too early; run this function no more than 100ms before we need to
        return !Config.Bugfixes.PredictiveHitSounds || (timeUntilNextBeat > LatencyManager.AudioLatencyOffset - LatencyManager.VideoLatencyOffset + .1f);
    }
}
