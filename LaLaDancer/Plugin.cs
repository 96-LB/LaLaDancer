using System.Collections;
using BepInEx;
using RiftOfTheNecroManager;
using Shared.SceneLoading;
using UnityEngine;

namespace LaLaDancer;


[BepInPlugin("com.lalabuff.necrodancer.laladancer", "LaLaDancer", "0.2.0")]
[NecroManagerInfo(menuNameOverride: "LaLaDancer")]
public class Plugin : RiftPlugin {
    public bool AntiSoftlockActive { get; private set; } = false;
    public float AntiSoftlockHoldTime { get; private set; } = float.PositiveInfinity;
    public int AntiSoftlockTicks { get; private set; } = 0;
    
    protected override void OnInit() {
        AntiSoftlockActive = true;
    }
    
    public void Update() {
        if(!AntiSoftlockActive || !LaLaDancer.Config.QOL.EnableAntiSoftlock) {
            return;
        }
        
        var key = LaLaDancer.Config.QOL.AntiSoftlockKey;
        
        if(Input.GetKeyDown(key)) {
            StartAntiSoftlock();
        } else if(Input.GetKeyUp(key) && float.IsFinite(AntiSoftlockHoldTime)) {
            ResetAntiSoftlock();
            Sfx.Play(Sfx.Cancel);
        } else if(Input.GetKey(key) && AntiSoftlockTicks > 3) {
            ResetAntiSoftlock();
            StartCoroutine(ForceLoadMainMenu());
            Sfx.Play(Sfx.Confirm);
        } else if(Input.GetKey(key) && Time.time - AntiSoftlockHoldTime >= AntiSoftlockTicks) {
            AntiSoftlockTicks++;
            Sfx.Play(Sfx.AddCharacter);
        }
    }
    
    public void StartAntiSoftlock() {
        AntiSoftlockTicks = 0;
        AntiSoftlockHoldTime = Time.time;
    }
    
    public void ResetAntiSoftlock() {
        AntiSoftlockTicks = 0;
        AntiSoftlockHoldTime = float.PositiveInfinity;
    }
    
    public IEnumerator ForceLoadMainMenu() {
        if(SceneLoadingController.Instance.IsLoading) {
            // if we're on a loading screen, tell the game that loading has finished
            SceneLoadingController.Instance._hasSceneAnnouncedLoadingComplete = true;
            
            // give it some time to clean up before we eject to main menu
            var time = Time.unscaledTime;
            yield return new WaitUntil(() => !SceneLoadingController.Instance.IsLoading || Time.unscaledTime - time >= 2f);
        }
        SceneLoadingController.Instance.GoToScene("MainMenu");
    }
}
