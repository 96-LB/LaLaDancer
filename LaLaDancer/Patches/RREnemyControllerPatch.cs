using System;
using HarmonyLib;
using RhythmRift;
using RhythmRift.Enemies;
using Shared.RhythmEngine;
using Shared;
using System.Collections.Generic;
using UnityEngine;
using Shared.Audio;
using FMODUnity;
using RiftOfTheNecroManager;

namespace LalaDancer.Patches;

using Config = LaLaDancer.Config;

public class AudioState : State<string, AudioState> {
    internal EnemyAudioData first;
    internal EnemyAudioData second;
    internal EnemyAudioData special;
    
    internal ref EnemyAudioData Audio(bool? isFollowUp) {
        if(isFollowUp == null) {
            return ref special; // cursed
        } else if(isFollowUp.Value) {
            return ref second;
        } else {
            return ref first;
        }
    }
    
    private void UnQueue_Internal(Guid[] guids) {
        foreach(Guid guid in guids) {
            if(guid != Guid.Empty) {
                AudioManager.Instance.StopAudioEvent(guid);
            }
        }
    }
    
    internal void UnQueue(bool? isFollowUp) {
        ref var audio = ref Audio(isFollowUp);
        UnQueue_Internal([audio.HitAudioId, audio.HitCryAudioId, audio.MissAudioId, audio.AttackAudioId]);
        audio = new();
    }
    
    internal void Hit() {
        UnQueue_Internal([first.MissAudioId, first.AttackAudioId]);
        first = second;
        second = new();
    }
    
    internal void SetLane(float laneNumber) {
        foreach(Guid guid in new[] {
            first.HitAudioId, first.HitCryAudioId, first.MissAudioId, first.AttackAudioId,
            second.HitAudioId, second.HitCryAudioId, second.MissAudioId, second.AttackAudioId,
            special.HitAudioId, special.HitCryAudioId, special.MissAudioId, special.AttackAudioId
        }) {
            if(guid != Guid.Empty) {
                AudioManager.Instance.SetLaneNumber(guid, laneNumber);
            }
        }
    }
}

internal struct EnemyAudioData {
    public float TargetTrueBeatNumber;
    public Guid HitAudioId;
    public Guid HitCryAudioId;
    public Guid MissAudioId;
    public Guid AttackAudioId;
    
    public readonly bool HasAnyAudio =>
        HitAudioId != Guid.Empty
        || HitCryAudioId != Guid.Empty
        || MissAudioId != Guid.Empty
        || AttackAudioId != Guid.Empty;
    
    public readonly bool IsTargetingBeatNumber(float beatNumber) {
        return Mathf.Abs(beatNumber - TargetTrueBeatNumber) < 0.01f;
    }
}


[HarmonyPatch(typeof(RREnemyController))]
public static class RREnemyControllerPatch {
    [HarmonyPatch(nameof(RREnemyController.TryQueueActionRowSounds))]
    [HarmonyPrefix]
    public static bool TryQueueActionRowSounds(RREnemyController __instance, ref FmodTimeCapsule fmodTimeCapsule) {
        if(!Config.Bugfixes.PredictiveHitSounds) {
            return true;
        }
        
        if(__instance._isGameOver || __instance._stopQueueingEnemyAudio || __instance._activeEnemies.Count < 1) {
            return false;
        }
        
        float mostExtremeBeatProgressForRating = __instance._inputRatingsDefinition.GetMostExtremeBeatProgressForRating(
            InputRating.Ok,
            fmodTimeCapsule.BeatLengthInSeconds,
            shouldReturnEarliestTime: false
        );
        
        foreach(var activeEnemy in __instance._activeEnemies) {
            if(!activeEnemy.ShouldQueueSounds() || activeEnemy.IsDying) {
                continue;
            }
            
            float beat = activeEnemy.NextActionRowTrueBeatNumber;
            if(activeEnemy.IsHoldNote && activeEnemy.IsBeingHeld) {
                beat = activeEnemy.NextUpdateTrueBeatNumber;
                if(
                    beat == activeEnemy.NextActionRowTrueBeatNumber
                    || beat > activeEnemy.GetLastHoldBeatNumber() + 0.05f
                ) {
                    continue;
                }
            }
            if(float.IsInfinity(beat) || fmodTimeCapsule.TrueBeatNumber > beat) {
                continue;
            }
            
            float delay = __instance.ComputeAudioQueueDelay(fmodTimeCapsule, beat);
            float timeUntilLatestInputThreshold = __instance.ComputeAudioQueueDelay(fmodTimeCapsule, beat + mostExtremeBeatProgressForRating);
            
            RREnemyControllerPatch_Internal.TryQueueActionRowSoundsForEnemy(
                activeEnemy, beat, delay, timeUntilLatestInputThreshold, false,
                __instance._tileGridAccessor, __instance._vibeChainHitEventRef, __instance._inputHitEventRef, __instance._enemyMissedEventRef, __instance._isPracticeMode, __instance._shouldQueueEnemyAttackAudio
             );
            
            if(activeEnemy.IsExpectingFollowUpAction) {
                beat = activeEnemy.ExpectedFollowUpActionTrueBeatNumber;
                if(float.IsInfinity(beat) || fmodTimeCapsule.TrueBeatNumber > beat) {
                    continue;
                }
                
                delay =  __instance.ComputeAudioQueueDelay(fmodTimeCapsule, beat);
                timeUntilLatestInputThreshold = __instance.ComputeAudioQueueDelay(fmodTimeCapsule, beat + mostExtremeBeatProgressForRating);
                RREnemyControllerPatch_Internal.TryQueueActionRowSoundsForEnemy(
                    activeEnemy, beat, delay, timeUntilLatestInputThreshold, true,
                    __instance._tileGridAccessor, __instance._vibeChainHitEventRef, __instance._inputHitEventRef, __instance._enemyMissedEventRef, __instance._isPracticeMode, __instance._shouldQueueEnemyAttackAudio
                );
            }
        }
        return false;
    }
    
    [HarmonyPatch(nameof(RREnemyController.TryUpdateAudioLaneNumbers))]
    [HarmonyPostfix]
    public static void TryUpdateAudioLaneNumbers(RREnemyController __instance, RREnemy enemyToUpdate) {
        if(!Config.Bugfixes.PredictiveHitSounds) {
            return;
        }
        
        float laneNumber = enemyToUpdate.TargetGridPosition.x - Mathf.Floor(__instance._tileGridAccessor.NumColumns / 2f);
        AudioState.Of(enemyToUpdate.EnemyId).SetLane(laneNumber);
    }
    
    [HarmonyPatch(nameof(RREnemyController.HasAnyQueuedInputHitSounds))]
    [HarmonyPrefix]
    public static bool HasAnyQueuedInputHitSounds(string enemyInstanceId, ref bool __result) {
        if(!Config.Bugfixes.PredictiveHitSounds) {
            return true;
        }
        
        var audio = AudioState.Of(enemyInstanceId);
        __result = audio.first.HitAudioId != Guid.Empty || audio.first.HitCryAudioId != Guid.Empty;
        return false;
    }
    
    [HarmonyPatch(nameof(RREnemyController.SetIsGameOver))]
    [HarmonyPostfix]
    public static void SetIsGameOver(bool isGameOver) {
        if(isGameOver) {
            foreach(var (_, audio) in AudioState.All()) {
                audio.UnQueue(false);
                audio.UnQueue(true);
                audio.UnQueue(null);
            }
        }
    }
    
    [HarmonyPatch(nameof(RREnemyController.TryProcessHitEnemySoundReactions))]
    [HarmonyPrefix]
    public static bool TryProcessHitEnemySoundReactions(RREnemy enemy) {
        if(!Config.Bugfixes.PredictiveHitSounds) {
            return true;
        }
        
        var audio = AudioState.Of(enemy.EnemyId);
        
        if(audio.first.HitAudioId == Guid.Empty && audio.first.HitCryAudioId == Guid.Empty) {
            Log.Error($"Enemy {enemy.DisplayName} ({enemy.EnemyId}) was hit before its update but somehow did not have any queued input hit sounds. {enemy}");
        } else {
            AudioManager.Instance.IsAudioEventPlaying(audio.first.HitCryAudioId);
            audio.Hit();
        }
        
        return false;
    }
    
    
    [HarmonyPatch(nameof(RREnemyController.TryQueueSpecialSounds))]
    [HarmonyPrefix]
    public static bool TryQueueSpecialSounds(RREnemyController __instance, ref FmodTimeCapsule fmodTimeCapsule) {
        if(!Config.Bugfixes.PredictiveHitSounds) {
            return true;
        }
        
        if(__instance._stopQueueingEnemyAudio) {
            return false;
        }
        
        AudioManager instance = AudioManager.Instance;
        for(int i = 0; i < __instance._activeEnemies.Count; i++) {
            RREnemy rREnemy = __instance._activeEnemies[i];
            ref var audio = ref AudioState.Of(rREnemy.EnemyId).special;
            float laneNumber = rREnemy.TargetGridPosition.x - Mathf.Floor(__instance._tileGridAccessor.NumColumns / 2f);
            float nextUpdateTrueBeatNumber = rREnemy.NextUpdateTrueBeatNumber;
            
            if(
                float.IsInfinity(nextUpdateTrueBeatNumber)
                || fmodTimeCapsule.TrueBeatNumber > nextUpdateTrueBeatNumber
                || audio.IsTargetingBeatNumber(nextUpdateTrueBeatNumber)
                || !rREnemy.SpecialSoundEventRef.HasValue
                || rREnemy.SpecialSoundEventRef.Value.IsNull
            ) {
                continue;
            }
            
            float delayInSeconds = __instance.ComputeAudioQueueDelay(fmodTimeCapsule, nextUpdateTrueBeatNumber);
            audio = new() {
                HitAudioId = RREnemyControllerPatch_Internal.QueueAudioEvent(rREnemy.SpecialSoundEventRef.Value, laneNumber, delayInSeconds, instance),
                TargetTrueBeatNumber = nextUpdateTrueBeatNumber
            };
        }
        return false;
    }
    
    [HarmonyPatch(nameof(RREnemyController.HandleCleanSpecialAudioData))]
    [HarmonyPrefix]
    public static bool HandleCleanSpecialAudioData(RREnemy enemy) {
        AudioState.Of(enemy.EnemyId).special = new();
        return !Config.Bugfixes.PredictiveHitSounds;
    }
    
    [HarmonyPatch(nameof(RREnemyController.HandleEnemyActionRowSoundQueueingRequest))]
    [HarmonyPrefix]
    public static bool HandleEnemyActionRowSoundQueueingRequest() {
        return !Config.Bugfixes.PredictiveHitSounds;
    }
}

public static class RREnemyControllerPatch_Internal {
    public static Guid QueueAudioEvent(
        EventReference eventRef,
        float laneNumber,
        float delayInSeconds,
        AudioManager instance,
        bool shouldCache = true
    ) {
        if(!eventRef.IsNull) {
            var guid = instance.PlayAudioEvent(eventRef, 0f, shouldCache, 0u, delayInSeconds);
            instance.SetLaneNumber(guid, laneNumber);
            return guid;
        }
        return Guid.Empty;
    }
    
    public static void TryQueueActionRowSoundsForEnemy(
        RREnemy enemy,
        float targetTrueBeatNumber,
        float timeUntilNextBeat,
        float timeUntilLatestInputThreshold,
        bool isFollowUp,
        IRRGridDataAccessor tileGridAccessor,
        EventReference vibeChainHitEventRef,
        EventReference inputHitEventRef,
        EventReference enemyMissedEventRef,
        bool isPracticeMode,
        bool shouldQueueEnemyAttackAudio
    ) {
        var state = AudioState.Of(enemy.EnemyId);
        ref var audio = ref state.Audio(isFollowUp);
        if(
            timeUntilNextBeat > LatencyManager.AudioLatencyOffset - LatencyManager.VideoLatencyOffset + 0.1f
            || audio.IsTargetingBeatNumber(targetTrueBeatNumber)
        ) {
            return;
        }
        state.UnQueue(isFollowUp);
        
        AudioManager instance = AudioManager.Instance;
        float laneNumber = (float)(enemy.TargetGridPosition.x - Mathf.Floor(tileGridAccessor.NumColumns / 2f));
        Guid guid = Guid.Empty;
        if(enemy.ShouldPlayHitSoundInActionRow) {
            guid = QueueAudioEvent(enemy.IsPartOfVibeChain ? vibeChainHitEventRef : inputHitEventRef, laneNumber, timeUntilNextBeat, instance);
        }
        
        EventReference enemyHitCryEventRef = enemy.GetEnemyHitCryEventRef(isFollowUp, targetTrueBeatNumber);
        Guid guid2 = QueueAudioEvent(enemyHitCryEventRef, laneNumber, timeUntilNextBeat, instance, shouldCache: false);
        
        float delayInSeconds = timeUntilLatestInputThreshold + Time.fixedDeltaTime * 2f;
        Guid guid3 = Guid.Empty;
        if(!isPracticeMode) {
            guid3 = QueueAudioEvent(enemyMissedEventRef, laneNumber, delayInSeconds, instance);
        }
        
        Guid guid4 = Guid.Empty;
        if(enemy.AttackDamage > 0 && enemy.ShouldPlayAttackSoundInActionRow && shouldQueueEnemyAttackAudio) {
            guid4 = QueueAudioEvent(enemy.AttackSoundEventRef, laneNumber, delayInSeconds, instance);
        }
        
        audio = new() {
            TargetTrueBeatNumber = targetTrueBeatNumber,
            HitAudioId = guid,
            HitCryAudioId = guid2,
            MissAudioId = guid3,
            AttackAudioId = guid4,
        };
    }
}
