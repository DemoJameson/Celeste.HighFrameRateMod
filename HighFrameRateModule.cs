using System;

namespace Celeste.Mod.HighFrameRateMod {
    public class HighFrameRateModule : EverestModule {
        public override Type SettingsType => typeof(HighFrameRateSettings);
        public override void Load() {}
        public override void Unload() { }
    }
}
