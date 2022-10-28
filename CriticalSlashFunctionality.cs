using System;
using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using HKMirror.Reflection.SingletonClasses;

namespace CriticalSlash {
    public static class CriticalSlashFunctionality{
        private static float _cachedNailChargeTimer;
        private static bool _isCritical;
        public static void Hook()
        {
            On.PlayMakerFSM.OnEnable += OnFsmEnable;
            On.HeroController.CanNailArt += OnCanNailArt;
            ModHooks.HitInstanceHook += ApplyCriticalSlashDamage;
        }
        
        public static void Unhook()
        {
            On.PlayMakerFSM.OnEnable -= OnFsmEnable;
            On.HeroController.CanNailArt -= OnCanNailArt;
            ModHooks.HitInstanceHook -= ApplyCriticalSlashDamage;
        }

        private static HitInstance ApplyCriticalSlashDamage(Fsm owner, HitInstance hit)
        {
            var isNailArt = hit.Source.name.Contains("Great Slash") || hit.Source.name.Contains("Dash Slash") || hit.Source.name.Contains("Hit L") || hit.Source.name.Contains("Hit R");
            if (isNailArt && _isCritical) 
                hit.DamageDealt = (int) (hit.DamageDealt * (1+CriticalSlash.GS.DamageIncreasePercent/100));
            return hit;
        }

        private static bool OnCanNailArt(On.HeroController.orig_CanNailArt orig, HeroController self)
        {
            // The default CanNailArt method resets the nail charge timer, so we cache it.
            _cachedNailChargeTimer = HeroControllerR.nailChargeTimer;
            return orig(self);
        }

        private static void OnFsmEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);
            if (self.gameObject.name != "Knight" || self.FsmName != "Nail Arts") return;
            self.Intercept(new TransitionInterceptor(){
                fromState = "Flash 2",
                eventName = "FINISHED",
                toStateDefault = "Facing?",
                toStateCustom = "Facing?",
                shouldIntercept = () => true,
                onIntercept = NailArtUsed,
            });
            self.Intercept(new TransitionInterceptor(){
                fromState = "DSlash Start",
                eventName = "FINISHED",
                toStateDefault = "Facing? 2",
                toStateCustom = "Facing? 2",
                shouldIntercept = () => true,
                onIntercept = NailArtUsed,
            });
            self.Intercept(new TransitionInterceptor(){
                fromState = "Flash",
                eventName = "FINISHED",
                toStateDefault = "Cyclone Start",
                toStateCustom = "Cyclone Start",
            shouldIntercept = () => true,
            onIntercept = NailArtUsed,
            });
        }

        private static void SetRecoveryAnimSpeed(float scale)
        {
            HeroControllerR.animCtrl.animator.GetClipByName("NA Big Slash").fps = 20*scale;
            HeroControllerR.animCtrl.animator.GetClipByName("NA Cyclone End").fps = 30*scale;
            HeroControllerR.animCtrl.animator.GetClipByName("NA Dash Slash").fps = 20*scale;
        }

        private static void NailArtUsed(string interceptedState, string interceptedEvent)
        {
            _isCritical = _cachedNailChargeTimer - HeroControllerR.nailChargeTime <= CriticalSlash.GS.WindowInCentiSeconds/100;
            if (_isCritical)
            {
                // Might replace with more custom animation/sound but I think this is kind of fitting.
                HeroControllerR.carefreeShield.SetActive(true);
                SetRecoveryAnimSpeed(1/Math.Max(1-CriticalSlash.GS.RecoverySpeedIncreasePercent/100, 0.01f));
            }
            else
            {
                SetRecoveryAnimSpeed(1);
            }
        }
    }
}