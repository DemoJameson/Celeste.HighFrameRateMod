using Monocle;
using System;

namespace Celeste.Mod.HighFrameRateMod {
    public class HighFrameRateSettings : EverestModuleSettings {
        private const int DefaultFrameRate = 60;

        private bool enabled = false;
        private int frameRate = 120;

        public bool Enabled {
            get => enabled;
            set {
                enabled = value;
                ApplyFrameRate();
            }
        }

        [SettingRange(10, 999, true)]
        public int FrameRate {
            get => frameRate;
            set {
                frameRate = value;
                ApplyFrameRate();
            }
        }

        private void ApplyFrameRate() {
            int newFrameRate = enabled ? frameRate : DefaultFrameRate;
            Engine.Instance.TargetElapsedTime = new TimeSpan((long) Math.Round(10000000.0 / newFrameRate));
        }
    }
}