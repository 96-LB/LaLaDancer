using HarmonyLib;
using RhythmRift;
using TMPro;

namespace LaLaDancer.Patches;


[HarmonyPatch(typeof(RRStageUIView))]
public static class UIPatch {
    [HarmonyPatch(nameof(RRStageUIView.UpdateScore))]
    [HarmonyPostfix]
    public static void UpdateScore(RRStageUIView __instance) {
        if(Config.Bugfixes.ScoreDisplay) {
            __instance._scoreText.overflowMode = TextOverflowModes.Overflow;
            __instance._scoreText.enableWordWrapping = false;
        }
    }
}
