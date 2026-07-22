using RiftOfTheNecroManager;

namespace LaLaDancer;


public static class Config {
    public static class Bugfixes {
        const string GROUP = "Bugfixes";
        
        public static Setting<bool> ArmadilloHitSounds { get; } = new(GROUP, "Fix Armadillo Hitsounds", true, "Fixes the timing of hitsounds for armadillos on subdivisions not divisible by 3.");
        public static Setting<bool> ScoreDisplay { get; } = new(GROUP, "Fix Score Display", true, "Adds more digits to the in-game score display for scores above 10 million.");
        public static Setting<bool> CustomParticles { get; } = new(GROUP, "Fix Custom Particles", true, "Fixes a bug which causes only the first 75% of custom particles to be displayed in custom charts.");
    }
    
    public static class QOL {
        const string GROUP = "QOL";
        
        public static Setting<bool> SkipSplashScreen { get; } = new(GROUP, "Skip Splash Screen", false, "Skips the splash screen on game startup.");
    }
}
