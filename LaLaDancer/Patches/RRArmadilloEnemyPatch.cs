using HarmonyLib;
using RhythmRift.Enemies;
using RiftOfTheNecroManager;
using UnityEngine;

namespace LaLaDancer.Patches;


[HarmonyPatch(typeof(RRArmadilloEnemy))]
public static class RRArmadilloEnemyPatch {
    [HarmonyPatch(nameof(RRArmadilloEnemy.ExpectedFollowUpActionTrueBeatNumber), MethodType.Getter)]
    [HarmonyPostfix]
    public static void ExpectedFollowUpActionTrueBeatNumber(RRArmadilloEnemy __instance, ref float __result) {
        if(Config.Bugfixes.ArmadilloHitSounds && __instance.ShouldClampToSubdivisions) {
            var subdiv = __instance._currentNumBeatSubdivisions;
            __result = Mathf.Round(__result * subdiv) / subdiv;
        }
    }
}
