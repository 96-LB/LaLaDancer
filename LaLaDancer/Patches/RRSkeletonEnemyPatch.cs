using HarmonyLib;
using RhythmRift.Enemies;
using RiftOfTheNecroManager;

namespace LaLaDancer.Patches;


public class RRSkeletonEnemyState : State<RRSkeletonEnemy, RRSkeletonEnemyState> {
    public void FixStaticCustomAssets() {
        if(!Config.Bugfixes.StaticAssets || !string.IsNullOrWhiteSpace(Instance.CurrentSprite.name)) {
            return;
        }
        
        // select a sprite name based on current state
        var name = Instance.EnemyTypeId switch {
            2202 or 1911 or 6471 => Instance._currentShieldHealth switch {
                <= 0 => "Monster_Skeleton_00",
                1 => "Monster_Skeleton_Shield_00",
                >= 2 => "Monster_Skeleton_Triplet_01",
            },
            6803 or 4871 => Instance._isHeadless
                ? "Monster_Skeleton_Headless_Yellow_00"
                : Instance.IsHoldingShield ? "Monster_Skeleton_Shield_Yellow_00" : "Monster_Skeleton_Yellow_00",
            2716 or 3307 => Instance._isHeadless
                ? "Monster_Skeleton_Headless_Black_00"
                : Instance.IsHoldingShield ? "Monster_Skeleton_Shield_Black_00" : "Monster_Skeleton_Black_00",
            _ => ""
        };
        
        if(Instance.CustomAssetProvider._sprites.TryGetValue(name, out var sprite)) {
            Instance.CurrentSprite = sprite;
        }
    }
}

[HarmonyPatch(typeof(RRSkeletonEnemy))]
public static class RRSkeletonEnemyPatch {
    [HarmonyPatch(nameof(RRSkeletonEnemy.Initialize))]
    [HarmonyPostfix]
    public static void Initialize(RRSkeletonEnemy __instance) {
        RRSkeletonEnemyState.Of(__instance).FixStaticCustomAssets();
    }
    
    [HarmonyPatch(nameof(RRSkeletonEnemy.FreezeMovementAnimation))]
    [HarmonyPostfix]
    public static void FreezeMovementAnimation(RRSkeletonEnemy __instance) {
        RRSkeletonEnemyState.Of(__instance).FixStaticCustomAssets();
    }
    
    [HarmonyPatch(nameof(RRSkeletonEnemy.ProcessIncomingAttack))]
    [HarmonyPostfix]
    public static void ProcessIncomingAttack(RRSkeletonEnemy __instance) {
        RRSkeletonEnemyState.Of(__instance).FixStaticCustomAssets();
    }

}
