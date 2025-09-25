using Amy.Modules;
using BepInEx.Configuration;
using LookingGlass;
using LookingGlass.LookingGlassLanguage;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using UnityEngine;

namespace Amy.Survivors.Amy
{
    public static class AmyBuffs
    {
        public static BuffDef boostBuff;

        public static BuffDef hammerSmashSpeedBuff;

        public static BuffDef hammerSpinSpeedBuff;

        public static void Init(AssetBundle assetBundle)
        {
            boostBuff = Modules.Content.CreateAndAddBuff("bdAmyRoseBoost",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite,
                AmySurvivor.amyColor,
                false,
                false);

            hammerSmashSpeedBuff = Modules.Content.CreateAndAddBuff("bdAmyRoseHammerSmashSpeed",
                assetBundle.LoadAsset<Sprite>("texHammerSmashBuffIcon"),
                Color.white,
                false,
                false);

            hammerSpinSpeedBuff = Modules.Content.CreateAndAddBuff("bdAmyRoseHammerSpinSpeed",
                assetBundle.LoadAsset<Sprite>("texHammerSpinBuffIcon"),
                Color.white,
                true,
                false);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LookingGlass.PluginInfo.PLUGIN_GUID))
            {
                RoR2Application.onLoad += LookingGlassSetup;
            }
        }

        private static void LookingGlassSetup()
        {
            if (RoR2.Language.languagesByName.TryGetValue("en", out RoR2.Language en))
            {
                RegisterLookingGlassBuff(en, boostBuff, "Amy Boost", $"Gain <style=cIsDamage>+{AmyStaticValues.boostArmor} armor</style>. Gain <style=cIsUtility>+{AmyStaticValues.boostListedSpeedCoefficient * 100}% movement speed</style>.");
                RegisterLookingGlassBuff(en, hammerSmashSpeedBuff, "Amy Smash Speed", $"Gain <style=cIsUtility>+{AmyStaticValues.secondaryHammerAirJumpBuffSpeedCoefficient * 100f}% movement speed</style>.");
                RegisterLookingGlassBuff(en, hammerSpinSpeedBuff, "Amy Spin Speed", $"Gain <style=cIsUtility>+{AmyStaticValues.boostHammerSpinBuffSpeedCoefficient * 100f}% movement speed</style>. Reduce {Tokens.RedText("acceleration")}. Increase hammer-spin {Tokens.DamageText("damage")}. Reduce hammer-spin {Tokens.RedText("attack speed")}.");
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void RegisterLookingGlassBuff(RoR2.Language lang, BuffDef buff, string name, string description)
        {
            LookingGlassLanguageAPI.SetupToken(lang, $"NAME_{buff.name}", name);
            LookingGlassLanguageAPI.SetupToken(lang, $"DESCRIPTION_{buff.name}", description);
        }
    }
}
