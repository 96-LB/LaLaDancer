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
        public static Setting<bool> StaticAssets { get; } = new(GROUP, "Fix Static Custom Assets", true, "Ensures that custom enemy assets are loaded even when the static monsters accessibility setting is enabled.");
        public static Setting<bool> Kerning { get; } = new(GROUP, "Fix GI Kerning", true, ColorText.Blue.Text("[REQUIRES RESTART]") + " Fixes the kerning between the characters 'G' and 'I' on the custom music menu.");
    }
    
    public static class QOL {
        const string GROUP = "QOL";
        
        public static Setting<bool> SkipSplashScreen { get; } = new(GROUP, "Skip Splash Screen", false, "Skips the splash screen on game startup.");
        public static Setting<bool> CountdownPausing { get; } = new(GROUP, "Enable Countdown Pausing", false, "Allows the game to be paused or the level to be restarted during the countdown.");
        public static Setting<bool> DisableAutomaticPause { get; } = new(GROUP, "Disable Automatic Pause", false, "Prevents the game from automatically pausing when you unfocus the application.");
        public static Setting<bool> EnableAntiSoftlock { get; } = new(GROUP, "Enable Anti-Softlock", false, "Enables the anti-softlock key to reset to main menu.");
        public static Setting<KeyCode> AntiSoftlockKey { get; } = new(GROUP, "Anti-Softlock Key", KeyCode.F8, "Resets the game to the main menu when held for three seconds.");
    }
}
