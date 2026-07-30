using HarmonyLib;
using RhythmRift.Enemies;
using RiftOfTheNecroManager;

namespace LaLaDancer.Patches;


public class RRWyrmEnemyState : State<RRWyrmEnemy, RRWyrmEnemyState> {
    public void FixStaticCustomAssets() {
        if(!Config.Bugfixes.StaticAssets || !Instance.IsInSideLane || !string.IsNullOrWhiteSpace(Instance.CurrentSprite.name)) {
            return;
        }
        
        // select a sprite name based on type of wyrm segment
        var name = Instance.EnemyTypeId switch {
            7794 => "RR_WyrmMonster_Head_Side_00",
            8079 => "RR_Monster_Wyrm_Side_Body",
            9888 => "RR_Monster_Wyrm_Side_Tail",
            _ => ""
        };
        
        if(Instance.CustomAssetProvider._sprites.TryGetValue(name, out var sprite)) {
            Instance.CurrentSprite = sprite;
        }
    }
}

[HarmonyPatch(typeof(RRWyrmEnemy))]
public static class RRWyrmEnemyPatch {
    [HarmonyPatch(nameof(RRWyrmEnemy.Initialize))]
    [HarmonyPostfix]
    public static void Initialize(RRWyrmEnemy __instance) {
        RRWyrmEnemyState.Of(__instance).FixStaticCustomAssets();
    }
    
    [HarmonyPatch(nameof(RRWyrmEnemy.FreezeMovementAnimation))]
    [HarmonyPostfix]
    public static void FreezeMovementAnimation(RRWyrmEnemy __instance) {
        RRWyrmEnemyState.Of(__instance).FixStaticCustomAssets();
    }
}
