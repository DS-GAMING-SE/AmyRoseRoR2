using Amy.Modules;
using Amy.Survivors.Amy.Achievements;
using LookingGlass.LookingGlassLanguage;
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

            string desc = "Amy is a skilled fighter who makes use of a wide arsenal of weaponry to take down his foes.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
             + "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine
             + "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine
             + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine
             + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so she left, ?????.";
            string outroFailure = "..and so she vanished, ?????.";

            Language.Add(prefix + "NAME", "Amy");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "?????");
            Language.Add(prefix + "LORE", "sample lore");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Momentum");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "<style=cIsUtility>Build up speed</style> by <style=cIsUtility>running down hill</style> to move up to <style=cIsUtility>100% faster</style>. <style=cIsHealth>Lose speed by running up hill to move up to 33% slower.</style>\nIf <style=cIsUtility>flying</style>, <style=cIsUtility>build up speed</style> by <style=cIsUtility>moving in a straight line.</style>");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_SLASH_NAME", "Sword");
            Language.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", Tokens.agilePrefix + $"Swing forward for <style=cIsDamage>{100f * AmyStaticValues.swordDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_GUN_NAME", "Handgun");
            Language.Add(prefix + "SECONDARY_GUN_DESCRIPTION", Tokens.agilePrefix + $"Fire a handgun for <style=cIsDamage>{100f * AmyStaticValues.gunDamageCoefficient}% damage</style>.");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_ROLL_NAME", "Roll");
            Language.Add(prefix + "UTILITY_ROLL_DESCRIPTION", "Roll a short distance, gaining <style=cIsUtility>300 armor</style>. <style=cIsUtility>You cannot be hit during the roll.</style>");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_BOMB_NAME", "Bomb");
            Language.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", $"Throw a bomb for <style=cIsDamage>{100f * AmyStaticValues.bombDamageCoefficient}% damage</style>.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(AmyMasteryAchievement.identifier), "Amy: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(AmyMasteryAchievement.identifier), "As Amy, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
