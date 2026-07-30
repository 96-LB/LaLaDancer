using HarmonyLib;
using RiftOfTheNecroManager;
using TMPro;

namespace LaLaDancer.Patches;


[HarmonyPatch(typeof(TMP_FontAsset))]
public static class TMP_FontAssetPatch {
    [HarmonyPatch("Awake")]
    [HarmonyPostfix]
    public static void Awake(TMP_FontAsset __instance) {
        if(!Config.Bugfixes.Kerning || __instance.name != "avertastdpe-extrabolditalic SDF - SoftMaskable") {
            return;
        }
        
        var kerning = __instance.fontFeatureTable.glyphPairAdjustmentRecords;
        for(int i = 0; i < kerning.Count; i++) {
            if(kerning[i].firstAdjustmentRecord.glyphIndex == 60 && kerning[i].secondAdjustmentRecord.glyphIndex == 72) {
                kerning.RemoveAt(i);
            }
        }
        
        __instance.ReadFontAssetDefinition();
    }
}
