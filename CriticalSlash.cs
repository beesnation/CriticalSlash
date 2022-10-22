using System;
using Modding;
using System.Collections.Generic;
using Satchel.BetterMenus;
using UnityEngine;

namespace CriticalSlash
{
    public class CriticalSlash : Mod, IGlobalSettings<CriticalSlashSettings>, ITogglableMod, ICustomMenuMod
    {
        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();
        internal static CriticalSlash Instance;
        public static CriticalSlashSettings GS { get; private set; } = new();
        public void OnLoadGlobal(CriticalSlashSettings s) => GS = s;
        public CriticalSlashSettings OnSaveGlobal() => GS;
        private Menu MenuRef;
        
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");
            Instance = this;
            CriticalSlashFunctionality.Hook();
        }

        public void Unload()
        {
            Log("Unloading");
            CriticalSlashFunctionality.Unhook();
        }

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? maybeToggleDelegates)
        {
            if (maybeToggleDelegates is not {} toggleDelegates) throw new InvalidOperationException();
            MenuRef ??= new Menu("Critical Slash",
                new Element[]
                {
                    new HorizontalOption("Enabled", "", new []{"False", "True"},
                        val => toggleDelegates.SetModEnabled(val != 0),
                        () => toggleDelegates.GetModEnabled() ? 1 : 0),
                    new CustomSlider("Window in Centi-seconds", 
                        val => GS.WindowInCentiSeconds = val,
                        () => GS.WindowInCentiSeconds,
                        0f,
                        50f,
                        true),
                    new CustomSlider("Recovery Time Removed %", 
                        val => GS.RecoverySpeedIncreasePercent = val,
                        () => GS.RecoverySpeedIncreasePercent,
                        0f,
                        100f,
                        true),
                    new CustomSlider("Damage Increase %", 
                        val => GS.DamageIncreasePercent = val,
                        () => GS.DamageIncreasePercent,
                        0f,
                        200f,
                        true),
                    new MenuButton("Reset to Defaults", "", button =>
                    {
                        GS = new CriticalSlashSettings();
                        MenuRef.Update();
                    })
                });
            return MenuRef.GetMenuScreen(modListMenu);
        }
        public bool ToggleButtonInsideMenu => true;
    }
}