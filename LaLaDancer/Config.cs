using RiftOfTheNecroManager;
using UnityEngine;

namespace LaLaDancer;


public static class Config {
    public static class Bugfixes {
        const string GROUP = "Bugfixes";
        
        public static Setting<bool> PredictiveSfx { get; } = new(GROUP, "Fix Trap Hitsounds", true, "Fixes the timing of hitsounds for enemies which are affected by traps near the action row.");
        public static Setting<bool> ArmadilloSfx { get; } = new(GROUP, "Fix Armadillo Hitsounds", true, "Fixes the timing of hitsounds for armadillos on subdivisions not divisible by 3.");
        public static Setting<bool> BlademasterSfx { get; } = new(GROUP, "Fix Blademaster Sounds", true, "Fixes the speed of blademaster sound effects on charts with BPM changes.");
        public static Setting<bool> ScoreDisplay { get; } = new(GROUP, "Fix Score Display", true, "Adds more digits to the in-game score display for scores above 10 million.");
        public static Setting<bool> CustomParticles { get; } = new(GROUP, "Fix Custom Particles", true, "Fixes a bug which causes only the first 75% of custom particles to be displayed in custom charts.");
        public static Setting<bool> Countdown { get; } = new(GROUP, "Fix Countdown Speed", true, "Fixes the countdown speed when using practice mode in a chart with BPM changes.");
    }
    
    public static class QOL {
        const string GROUP = "QOL";
        
        public static Setting<bool> SkipSplashScreen { get; } = new(GROUP, "Skip Splash Screen", false, "Skips the splash screen on game startup.");
        public static Setting<bool> EnableAntiSoftlock { get; } = new(GROUP, "Enable Anti-Softlock", false, "Enables the anti-softlock key to reset to main menu.");
        public static Setting<KeyCode> AntiSoftlockKey { get; } = new(GROUP, "Anti-Softlock Key", KeyCode.F8, "Resets the game to the main menu when held for three seconds.");
    }
}
