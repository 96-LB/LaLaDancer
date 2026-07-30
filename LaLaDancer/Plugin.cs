using BepInEx;
using RiftOfTheNecroManager;
using Shared.Audio;
using Shared.SceneLoading;
using UnityEngine;

namespace LaLaDancer;


[BepInPlugin("com.lalabuff.necrodancer.laladancer", "LaLaDancer", "0.1.1")]
[NecroManagerInfo(menuNameOverride: "LaLaDancer")]
public class Plugin : RiftPlugin {
    public bool AntiSoftlockActive { get; private set; } = false;
    public float AntiSoftlockHoldTime { get; private set; } = 0f;
    public int AntiSoftlockTicks { get; private set; } = 0;
    
    protected override void OnInit() {
        AntiSoftlockActive = true;
    }
    
    public void Update() {
        if(AntiSoftlockActive && LaLaDancer.Config.QOL.EnableAntiSoftlock) {
            if(Input.GetKeyDown(LaLaDancer.Config.QOL.AntiSoftlockKey)) {
                StartAntiSoftlock();
            } else if(Input.GetKeyUp(LaLaDancer.Config.QOL.AntiSoftlockKey) && AntiSoftlockHoldTime > 0f) {
                ResetAntiSoftlock();
                Sfx.Play(Sfx.Cancel);
            } else if(Input.GetKey(LaLaDancer.Config.QOL.AntiSoftlockKey) && Time.time - AntiSoftlockHoldTime >= 3f) {
                ResetAntiSoftlock();
                SceneLoadingController.Instance.IsLoading = false;
                SceneLoadingController.Instance.GoToScene("MainMenu");
            } else if(Input.GetKey(LaLaDancer.Config.QOL.AntiSoftlockKey) && Time.time - AntiSoftlockHoldTime >= AntiSoftlockTicks) {
                AntiSoftlockTicks++;
                Sfx.Play(Sfx.AddCharacter);
            }
        }
    }
    
    public void StartAntiSoftlock() {
        AntiSoftlockTicks = 0;
        AntiSoftlockHoldTime = Time.time;
    }
    
    public void ResetAntiSoftlock() {
        AntiSoftlockTicks = 0;
        AntiSoftlockHoldTime = 0f;
    }
}
