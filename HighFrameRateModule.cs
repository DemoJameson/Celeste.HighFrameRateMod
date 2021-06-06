using Monocle;
using System;

namespace Celeste.Mod.HighFrameRateMod {
    public class HighFrameRateModule : EverestModule {
        private const int DefaultFrameRate = 60;
        public static HighFrameRateModule Instance { get; private set; }

        public HighFrameRateModule() {
            Instance = this;
        }

        public override Type SettingsType => typeof(HighFrameRateSettings);
        public HighFrameRateSettings Settings => (HighFrameRateSettings) _Settings;

        public override void Load() {
            On.Monocle.Scene.Begin += SceneOnBegin;
        }

        public override void Unload() {
            On.Monocle.Scene.Begin -= SceneOnBegin;
        }

        private void SceneOnBegin(On.Monocle.Scene.orig_Begin orig, Scene self) {
            orig(self);
            ApplyFrameRate();
        }

        public void ApplyFrameRate() {
            int newFrameRate = Settings.FrameRate;
            if (Settings.OverworldOnly && Engine.Scene is Level or LevelLoader or LevelExit) {
                newFrameRate = DefaultFrameRate;
            }
            if (!Settings.Enabled) {
                newFrameRate = DefaultFrameRate;
            }
            Engine.Instance.TargetElapsedTime = new TimeSpan((long) Math.Round(10000000.0 / newFrameRate));
        }
    }
}