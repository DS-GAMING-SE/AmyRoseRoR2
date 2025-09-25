using Amy.Modules;
using Amy.Survivors.Amy.Achievements;
using LookingGlass.LookingGlassLanguage;
using R2API;
using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Amy.Survivors.Amy
{
    public static class AmyTokens
    {
        public static void Init()
        {
            AddAmyTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Amy.txt");
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddAmyTokens()
        {
            string prefix = AmySurvivor.AMY_PREFIX;

            string desc = $"Amy is a fast and floaty melee survivor who specializes in high damage attacks and vertical mobility.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + $"< ! > All of Amy's hammer attacks can launch lightweight enemies. Even if you're fighting a powerful enemy, don't neglect the smaller enemies. They can be used to your advantage for extra damage." + Environment.NewLine + Environment.NewLine
             + "< ! > Piko Piko Smash used in the air is both a powerful attack and a tool for getting high in the air. Use it to approach aerial enemies and launch them down to ground level." + Environment.NewLine + Environment.NewLine
             + "< ! > Boost's Hammer-Spin used while stationary delivers a fast flurry of attacks. While maintaining high speeds, it does high single-hit damage and gives speed rivaling Sonic himself. Even getting dizzy can be beneficial, just make sure you're careful about when you let that happen." + Environment.NewLine + Environment.NewLine
             + $"< ! > {Tokens.wipIcon}" + Environment.NewLine + Environment.NewLine;

            string outro = $"..and so she left, {Tokens.wipIcon}.";
            string outroFailure = $"..and so she vanished, {Tokens.wipIcon}.";

            Language.Add(prefix + "NAME", "Amy");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", $"{Tokens.wipIcon} ??? {Tokens.wipIcon}");
            Language.Add(prefix + "LORE", $"{Tokens.wipIcon}");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "PALADIN_SKIN_NAME", "Paladin");
            Language.Add(prefix + "RIDERS_SKIN_NAME", "Riders");
            Language.Add(prefix + "FORTUNE_TELLER_SKIN_NAME", "Fortune Teller");
            Language.Add(prefix + "MALWARE_SKIN_NAME", "Malware");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_HAMMER_NAME", "Piko Piko Hammer");
            Language.Add(prefix + "PRIMARY_HAMMER_DESCRIPTION", Tokens.agilePrefix + $"{Tokens.UtilityText("Launching")}. Swing your hammer for <style=cIsDamage>{100f * AmyStaticValues.primaryHammerDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_HAMMER_SMASH_NAME", $"Piko Piko Smash");
            Language.Add(prefix + "SECONDARY_HAMMER_SMASH_DESCRIPTION", $"{Tokens.UtilityText("Launching")}. Charge a hammer swing dealing {Tokens.DamageValueText(AmyStaticValues.secondaryHammerChargeMinimumDamageCoefficient,AmyStaticValues.secondaryHammerChargeMaximumDamageCoefficient)}. If airborne, quickly descend, {Tokens.DamageText("attack")} where you land, and {Tokens.UtilityText("rebound upwards")}.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_BOOST_NAME", "Boost");
            LanguageAPI.Add(prefix + "UTILITY_BOOST_DESCRIPTION", $"Spend boost meter to <style=cIsUtility>move {100f * AmyStaticValues.boostListedSpeedCoefficient}% faster</style> than normal. If airborne, do a short <style=cIsUtility>mid-air dash</style>.\nPress the primary skill to begin a {Tokens.DamageText("hammer-spin")}. Running out of boost meter while hammer-spinning will make you {Tokens.RedText("dizzy")}.");

            LanguageAPI.Add(prefix + "UTILITY_HAMMER_SPIN_NAME", "Hammer-Spin");
            LanguageAPI.Add(prefix + "UTILITY_HAMMER_SPIN_DESCRIPTION", $"{Tokens.UtilityText("Launching")}. Spin dealing {Tokens.DamageValueText(AmyStaticValues.boostHammerSpinDamageCoefficient, AmyStaticValues.boostHammerSpinFastDamageCoefficient)} repeatedly. Maintaining high speed gradually increases {Tokens.UtilityText("movement speed")} and {Tokens.DamageText("damage")}. Running out of boost meter will make you {Tokens.RedText("dizzy")}.");

            LanguageAPI.Add(prefix + "HAMMER_SPIN_KEYWORD", $"<style=CKeywordName>Hammer-Spin</style><style=cSub>{Tokens.UtilityText("Launching")}. Deal {Tokens.DamageValueText(AmyStaticValues.boostHammerSpinDamageCoefficient, AmyStaticValues.boostHammerSpinFastDamageCoefficient)} repeatedly. Maintaining high speed gradually increases {Tokens.UtilityText("movement speed")} and {Tokens.DamageText("damage")}.</style>");
            LanguageAPI.Add(prefix + "DIZZY_KEYWORD", $"<style=CKeywordName>Dizzy</style><style=cSub>{Tokens.RedText("No sprinting or using skills")}. Lasts until the {Tokens.UtilityText("boost meter")} is recharged. Increases {Tokens.UtilityText("boost meter")} recharge rate.</style>");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_MULTILOCK_NAME", $"Multi-Lock {Tokens.wipIcon}");
            Language.Add(prefix + "SPECIAL_MULTILOCK_DESCRIPTION", $"Enter {Tokens.UtilityText("target painting mode")}, then bounce between targets dealing {Tokens.DamageValueText(AmyStaticValues.specialMultiLockDamageCoefficient)}. Can target up to {AmyStaticValues.specialMultiLockMaxTargets}.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(AmyMasteryAchievement.identifier), "Amy: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(AmyMasteryAchievement.identifier), "As Amy, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
