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
    
    [HarmonyPatch(nameof(RRArmadilloEnemy.Initialize))]
    [HarmonyPostfix]
    public static void Initialize(RRArmadilloEnemy __instance) {
        if(Config.Bugfixes.StaticAssets) {
            // forces static armadillo sprites to go through the custom asset swap pipeline
            __instance.CurrentSprite = __instance._defaultSprite;
        }
    }
    
    [HarmonyPatch(nameof(RRArmadilloEnemy.FreezeMovementAnimation))]
    [HarmonyPostfix]
    public static void FreezeMovementAnimation(RRArmadilloEnemy __instance) {
        if(Config.Bugfixes.StaticAssets) {
            // duplicate coded; this is too simple for a state class
            __instance.CurrentSprite = __instance._defaultSprite;
        }
    }
}
