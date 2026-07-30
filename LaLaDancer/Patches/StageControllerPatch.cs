using HarmonyLib;
using RhythmRift;
using Shared;

namespace LaLaDancer.Patches;


[HarmonyPatch(typeof(StageController<RRBeatmapPlayer>))]
public static class StageControllerPatch {
    [HarmonyPatch(nameof(StageController<>.PerformAutomaticPause))]
    [HarmonyPrefix]
    public static bool PerformAutomaticPause() {
        // disable automatic pausing if the config option is set
        return !Config.QOL.DisableAutomaticPause;
    }
}
