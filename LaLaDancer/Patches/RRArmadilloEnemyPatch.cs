using HarmonyLib;
using RhythmRift.Enemies;
using UnityEngine;

namespace LaLaDancer.Patches;


[HarmonyPatch(typeof(RRArmadilloEnemy))]
public static class RRArmadilloEnemyPatch {
    [HarmonyPatch(nameof(RRArmadilloEnemy.ExpectedFollowUpActionTrueBeatNumber), MethodType.Getter)]
    [HarmonyPostfix]
    public static void ExpectedFollowUpActionTrueBeatNumber(RRArmadilloEnemy __instance, ref float __result) {
        if(Config.Bugfixes.ArmadilloSfx && __instance.ShouldClampToSubdivisions) {
            // armadillos on subdiv 4 or above don't clamp their sound effects
            // this causes a mismatch between the hit timings and the sounds
            var subdiv = __instance._currentNumBeatSubdivisions;
            __result = Mathf.Round(__result * subdiv) / subdiv;
        }
    }
}
