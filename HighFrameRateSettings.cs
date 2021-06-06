using Monocle;
using System;

namespace Celeste.Mod.HighFrameRateMod {
    public class HighFrameRateSettings : EverestModuleSettings {
        private bool enabled = false;
        private bool overworldOnly = false;
        private int frameRate = 120;

        public bool Enabled {
            get => enabled;
            set {
                enabled = value;
                HighFrameRateModule.Instance.ApplyFrameRate();
            }
        }

        public bool OverworldOnly {
            get => overworldOnly;
            set {
                overworldOnly = value;
                HighFrameRateModule.Instance.ApplyFrameRate();
            }
        }

        [SettingRange(10, 999, true)]
        public int FrameRate {
            get => frameRate;
            set {
                frameRate = value;
                HighFrameRateModule.Instance.ApplyFrameRate();
            }
        }
    }
}