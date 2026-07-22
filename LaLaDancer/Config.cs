using RiftOfTheNecroManager;

namespace LaLaDancer;


public static class Config {
    public static class Bugfixes {
        const string GROUP = "Bugfixes";
        
        public static Setting<bool> ArmadilloHitSounds { get; } = new(GROUP, "Fix Armadillo Hitsounds", true, "Fixes the timing of hitsounds for armadillos on subdivisions not divisible by 3.");
    }
}
