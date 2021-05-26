﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoMod.RuntimeDetour;
using System.Reflection;
using WaspPile.EyeIntheSky.Rulesets;

namespace WaspPile.EyeIntheSky
{
    public static class LensHooks
    {
        public static void InitSpritesFilter(anyInitSprites orig, IDrawable self, RoomCamera.SpriteLeaser sleaser, RoomCamera rcam)
        {
            orig(self, sleaser, rcam);
            GeneralRuleset therule;
            if (BrotherBigEyes.TryGetRules(self.GetType(), out therule)) 
            {
                var onMyInit = therule.DoOnInit;
                if (onMyInit != null)
                {
                    if (onMyInit.additionalSlots.HasValue) Array.Resize(ref sleaser.sprites, sleaser.sprites.Length + onMyInit.additionalSlots.Value);
                    if (onMyInit.spriteReplacements != null) foreach (var kvp in onMyInit.spriteReplacements)
                        {
                            if (kvp.Key < sleaser.sprites.Length) sleaser.sprites[kvp.Key] = (FSprite)kvp.Value.ShallowClone();
                        }
                }
            }
        }
        
        public delegate void anyInitSprites(IDrawable self, RoomCamera.SpriteLeaser sleaser, RoomCamera rcam);
        public delegate void anyDrawSprites(IDrawable self, RoomCamera.SpriteLeaser sleaser, RoomCamera rcam, float timeStacker);
        public delegate void anyApplyPal(IDrawable self, RoomCamera.SpriteLeaser sleaser, RoomCamera rcam, RoomPalette pal);

        public static void AttemptApply(Type tp)
        {
            foreach (var child in tp.GetNestedTypes()) { AttemptApply(child); }
            if (tp == idw || tp.GetInterface(idw.Name) == null) return;
            var tarmethod = tp.GetMethod(nameof(IDrawable.InitiateSprites));
            if (tarmethod != null) try
            {
                allmyhooks.Add(new Hook(tarmethod, typeof(LensHooks).GetMethod(nameof(InitSpritesFilter))));
            }
            catch (Exception e)
            {
                encounteredErrors.Add(e);
            }
        }

        internal static Type idw = typeof(IDrawable);
        internal static List<Hook> allmyhooks { get { _amh = _amh ?? new List<Hook>(); return _amh; } set { _amh = value; }  }
        internal static List<Exception> encounteredErrors { get { _ee = _ee ?? new List<Exception>(); return _ee; } set { _ee = value; } }
        private static List<Hook> _amh;
        private static List<Exception> _ee;
        internal static List<Assembly> AffectedAssemblies { get { _aa = _aa ?? new List<Assembly>(); return _aa; } set { _aa = value; } }
        private static List<Assembly> _aa;

        public static void ApplyToVanilla()
        {
            ApplyToAssembly(idw.Assembly);
        }
        public static void ApplyToAssembly(Assembly asm)
        {
            if (AffectedAssemblies.Contains(asm)) return;
            foreach (var type in asm.GetTypes()) AttemptApply(type);
            AffectedAssemblies.Add(asm);
        }

        public static void DisposeOfAll()
        {
            foreach (var hk in allmyhooks) { hk.Dispose(); }
            allmyhooks.Clear();
        }
    }
}
