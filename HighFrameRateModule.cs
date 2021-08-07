using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;

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
            using (new DetourContext {After = new List<string> {"*"}}) {
                IL.Celeste.SpeedrunTimerDisplay.DrawTime += RecolorTimer;
            }
        }

        public override void Unload() {
            On.Monocle.Scene.Begin -= SceneOnBegin;
            IL.Celeste.SpeedrunTimerDisplay.DrawTime -= RecolorTimer;
        }

        private void SceneOnBegin(On.Monocle.Scene.orig_Begin orig, Scene self) {
            orig(self);
            ApplyFrameRate();
        }

        private void RecolorTimer(ILContext il) {
            ILCursor cursor = new(il);
            if (!cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdarg(3))) {
                return;
            }

            if (!cursor.TryGotoNext(MoveType.Before
                , instr => instr.MatchLdcI4(0)
                , instr => instr.OpCode == OpCodes.Stloc_S)) {
                return;
            }

            ILLabel afterInstr = cursor.MarkLabel();

            cursor.Index = 0;
            if (!cursor.TryGotoNext(MoveType.AfterLabel, instr => instr.MatchLdarg(3))) {
                return;
            }

            cursor.EmitDelegate<Func<bool>>(() => Settings.Enabled && !Settings.OverworldOnly && Settings.FrameRate != 60);

            ILLabel beforeInstr = cursor.DefineLabel();
            cursor.Emit(OpCodes.Brfalse, beforeInstr);

            cursor.Emit(OpCodes.Ldstr, "bdf2ce");
            cursor.Emit(OpCodes.Call, typeof(Calc).GetMethod("HexToColor", new[] {typeof(string)}));
            cursor.Emit(OpCodes.Ldarg, 6);
            cursor.Emit(OpCodes.Call, typeof(Color).GetMethod("op_Multiply"));
            cursor.Emit(OpCodes.Stloc, 5);

            cursor.Emit(OpCodes.Ldstr, "92deab");
            cursor.Emit(OpCodes.Call, typeof(Calc).GetMethod("HexToColor", new[] {typeof(string)}));
            cursor.Emit(OpCodes.Ldarg, 6);
            cursor.Emit(OpCodes.Call, typeof(Color).GetMethod("op_Multiply"));
            cursor.Emit(OpCodes.Stloc, 6);

            cursor.Emit(OpCodes.Br, afterInstr);
            cursor.MarkLabel(beforeInstr);
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