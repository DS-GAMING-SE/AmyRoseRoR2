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

            string desc = $"{Tokens.wipIcon} Amy is a skilled fighter who makes use of a wide arsenal of weaponry to take down his foes.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine
             + "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine
             + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine
             + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            string outro = $"..and so she left, {Tokens.wipIcon}.";
            string outroFailure = $"..and so she vanished, {Tokens.wipIcon}.";

            Language.Add(prefix + "NAME", "Amy");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", $"{Tokens.wipIcon} ??? {Tokens.wipIcon}");
            Language.Add(prefix + "LORE", $"{Tokens.wipIcon}");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_HAMMER_NAME", "Piko Piko Hammer");
            Language.Add(prefix + "PRIMARY_HAMMER_DESCRIPTION", Tokens.agilePrefix + $"{Tokens.UtilityText("Launching")}. Swing your hammer for <style=cIsDamage>{100f * AmyStaticValues.primaryHammerDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_HAMMER_SMASH_NAME", $"Piko Piko Smash");
            Language.Add(prefix + "SECONDARY_HAMMER_SMASH_DESCRIPTION", $"{Tokens.UtilityText("Launching")}. Charge a hammer swing dealing {Tokens.DamageValueText(AmyStaticValues.secondaryHammerChargeMinimumDamageCoefficient,AmyStaticValues.secondaryHammerChargeMaximumDamageCoefficient)}. If airborne, quickly descend, {Tokens.DamageText("attack")} the ground where you land, and {Tokens.UtilityText("rebound upwards")}.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_BOOST_NAME", "Boost");
            LanguageAPI.Add(prefix + "UTILITY_BOOST_DESCRIPTION", $"Spend boost meter to <style=cIsUtility>move {100f * AmyStaticValues.boostListedSpeedCoefficient}% faster</style> than normal. If airborne, do a short <style=cIsUtility>mid-air dash</style>.\nPress the primary skill to begin a {Tokens.UtilityText("hammer-spin ") + Tokens.wipIcon}, repeatedly dealing {Tokens.DamageValueText(AmyStaticValues.boostHammerSpinDamageCoefficient)}. Running out of boost meter while hammer-spinning will make you {Tokens.RedText("dizzy")}.");
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
