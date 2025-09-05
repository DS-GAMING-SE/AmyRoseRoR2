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

        public static BuffDef dizzyDebuff;

        public static void Init(AssetBundle assetBundle)
        {
            boostBuff = Modules.Content.CreateAndAddBuff("bdAmyRoseBoost",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite,
                AmySurvivor.amyColor,
                false,
                false);

            hammerSmashSpeedBuff = Modules.Content.CreateAndAddBuff("bdAmyRoseHammerSmashSpeed",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite,
                AmySurvivor.amyColor,
                false,
                false);

            hammerSpinSpeedBuff = Modules.Content.CreateAndAddBuff("bdAmyRoseHammerSpinSpeed",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite,
                AmySurvivor.amyColor,
                false,
                false);

            dizzyDebuff = Modules.Content.CreateAndAddBuff("bdAmyRoseDizzy",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Slow60").iconSprite,
                Color.white,
                false,
                true);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LookingGlass.PluginInfo.PLUGIN_GUID))
            {
                RoR2Application.onLoad += LookingGlassSetup;
            }
        }

        private static void LookingGlassSetup()
        {
            if (Language.languagesByName.TryGetValue("en", out RoR2.Language en))
            {
                RegisterLookingGlassBuff(en, boostBuff, "Amy Boost", $"Gain <style=cIsUtility>+{AmyStaticValues.boostArmor} armor</style>. Gain <style=cIsUtility>+{AmyStaticValues.boostListedSpeedCoefficient * 100}% movement speed</style>.");
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void RegisterLookingGlassBuff(Language lang, BuffDef buff, string name, string description)
        {
            LookingGlassLanguageAPI.SetupToken(lang, $"NAME_{buff.name}", name);
            LookingGlassLanguageAPI.SetupToken(lang, $"DESCRIPTION_{buff.name}", description);
        }
    }
}
