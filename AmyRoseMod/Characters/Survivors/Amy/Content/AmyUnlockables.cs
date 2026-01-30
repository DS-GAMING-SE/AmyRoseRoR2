using AmyRoseMod.Characters.Survivors.Amy.Achievements;
using AmyRoseMod.Modules;
using BepInEx.Configuration;
using RoR2;
using System;
using System.Linq;
using UnityEngine;

namespace AmyRoseMod.Characters.Survivors.Amy
{
    public static class AmyUnlockables
    {
        public static UnlockableDef masterySkinUnlockableDef = null;
        public static ConfigEntry<bool> masterySkinUnlockableConfig;

        public static UnlockableDef grandMasterySkinUnlockableDef = null;
        public static ConfigEntry<bool> grandMasterySkinUnlockableConfig;

        public static UnlockableDef meridianSkinUnlockableDef = null;
        public static ConfigEntry<bool> meridianSkinUnlockableConfig;

        public static UnlockableDef decompileSkinUnlockableDef = null;
        public static ConfigEntry<bool> decompileSkinUnlockableConfig;

        public static UnlockableDef purgeSkinUnlockableDef = null;
        public static ConfigEntry<bool> purgeSkinUnlockableConfig;
        public static void Init()
        {
            // Mastery
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                AmyMasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(AmyMasteryAchievement.unlockableIdentifier),
                AmySurvivor.instance.assetBundle.LoadAsset<Sprite>("texMainSkinIcon"));
            masterySkinUnlockableConfig = CreateUnlockableConfig("Mastery");
            masterySkinUnlockableConfig.SettingChanged += new EventHandler(delegate (object o, EventArgs a)
            { UnlockableConfigChanged(masterySkinUnlockableConfig.Value, ref masterySkinUnlockableDef, AmyMasteryAchievement.unlockableIdentifier); });

            // Grand Mastery
            grandMasterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                AmyGrandMasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(AmyGrandMasteryAchievement.unlockableIdentifier),
                AmySurvivor.instance.assetBundle.LoadAsset<Sprite>("texPaladinSkinIcon"));
            grandMasterySkinUnlockableConfig = CreateUnlockableConfig("Grand Mastery");
            grandMasterySkinUnlockableConfig.SettingChanged += new EventHandler(delegate (object o, EventArgs a)
            { UnlockableConfigChanged(grandMasterySkinUnlockableConfig.Value, ref grandMasterySkinUnlockableDef, AmyGrandMasteryAchievement.unlockableIdentifier); });

            // Prime Meridian
            meridianSkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                AmyMeridianEventTriggerAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(AmyMeridianEventTriggerAchievement.unlockableIdentifier),
                AmySurvivor.instance.assetBundle.LoadAsset<Sprite>("texMainSkinIcon"));
            meridianSkinUnlockableConfig = CreateUnlockableConfig("Prime Meridian");
            meridianSkinUnlockableConfig.SettingChanged += new EventHandler(delegate (object o, EventArgs a)
            { UnlockableConfigChanged(meridianSkinUnlockableConfig.Value, ref meridianSkinUnlockableDef, AmyMeridianEventTriggerAchievement.unlockableIdentifier); });

            // Decompile
            decompileSkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                AmyDecompileAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(AmyDecompileAchievement.unlockableIdentifier),
                AmySurvivor.instance.assetBundle.LoadAsset<Sprite>("texMalwareSkinIcon"));
            decompileSkinUnlockableConfig = CreateUnlockableConfig("Decompile");
            decompileSkinUnlockableConfig.SettingChanged += new EventHandler(delegate (object o, EventArgs a)
            { UnlockableConfigChanged(decompileSkinUnlockableConfig.Value, ref decompileSkinUnlockableDef, AmyDecompileAchievement.unlockableIdentifier); });

            // Purge
            purgeSkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                AmyPurgeAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(AmyPurgeAchievement.unlockableIdentifier),
                AmySurvivor.instance.assetBundle.LoadAsset<Sprite>("texMainSkinIcon"));
            purgeSkinUnlockableConfig = CreateUnlockableConfig("Purge");
            purgeSkinUnlockableConfig.SettingChanged += new EventHandler(delegate (object o, EventArgs a)
            { UnlockableConfigChanged(purgeSkinUnlockableConfig.Value, ref purgeSkinUnlockableDef, AmyPurgeAchievement.unlockableIdentifier); });

            On.RoR2.UserProfile.OnLogin += ConfigUnlocks;
        }
        private static void ConfigUnlocks(On.RoR2.UserProfile.orig_OnLogin orig, UserProfile self)
        {
            orig(self);
            if (masterySkinUnlockableConfig.Value)
            {
                if (!self.HasAchievement(AmyMasteryAchievement.unlockableIdentifier))
                {
                    self.AddAchievement(AmyMasteryAchievement.unlockableIdentifier, true);
                }
                if (!self.HasUnlockable(masterySkinUnlockableDef))
                {
                    self.GrantUnlockable(masterySkinUnlockableDef);
                }
            }
            if (grandMasterySkinUnlockableConfig.Value)
            {
                if (!self.HasAchievement(AmyGrandMasteryAchievement.unlockableIdentifier))
                {
                    self.AddAchievement(AmyGrandMasteryAchievement.unlockableIdentifier, true);
                }
                if (!self.HasUnlockable(grandMasterySkinUnlockableDef))
                {
                    self.GrantUnlockable(grandMasterySkinUnlockableDef);
                }
            }
            if (meridianSkinUnlockableConfig.Value)
            {
                if (!self.HasAchievement(AmyMeridianEventTriggerAchievement.unlockableIdentifier))
                {
                    self.AddAchievement(AmyMeridianEventTriggerAchievement.unlockableIdentifier, true);
                }
                if (!self.HasUnlockable(meridianSkinUnlockableDef))
                {
                    self.GrantUnlockable(meridianSkinUnlockableDef);
                }
            }
            if (decompileSkinUnlockableConfig.Value)
            {
                if (!self.HasAchievement(AmyDecompileAchievement.unlockableIdentifier))
                {
                    self.AddAchievement(AmyDecompileAchievement.unlockableIdentifier, true);
                }
                if (!self.HasUnlockable(decompileSkinUnlockableDef))
                {
                    self.GrantUnlockable(decompileSkinUnlockableDef);
                }
            }
            if (purgeSkinUnlockableConfig.Value)
            {
                if (!self.HasAchievement(AmyPurgeAchievement.unlockableIdentifier))
                {
                    self.AddAchievement(AmyPurgeAchievement.unlockableIdentifier, true);
                }
                if (!self.HasUnlockable(purgeSkinUnlockableDef))
                {
                    self.GrantUnlockable(purgeSkinUnlockableDef);
                }
            }
        }

        private static ConfigEntry<bool> CreateUnlockableConfig(string achievementName)
        {
            return Config.BindAndOptions("Unlockables", achievementName, false, $"Unlock or relock the achievement \"Amy: {achievementName}\". Locking the achievement may require restarting the game for the achievement to be obtainable again.");
        }

        private static void UnlockableConfigChanged(bool configValue, ref UnlockableDef unlockableDef, string achievementToken)
        {
            UserProfile user = LocalUserManager.readOnlyLocalUsersList.FirstOrDefault(v => v != null)?.userProfile;

            if (configValue)
            {
                if (!user.HasAchievement(achievementToken))
                {
                    user.AddAchievement(achievementToken, true);
                }

                if (!user.HasUnlockable(unlockableDef))
                {
                    user.GrantUnlockable(unlockableDef);
                }
            }
            else
            {
                if (user.HasAchievement(achievementToken))
                {
                    foreach (var notification in RoR2.UI.AchievementNotificationPanel.instancesList)
                        UnityEngine.Object.Destroy(notification.gameObject);
                    user.RevokeAchievement(achievementToken);
                }

                if (user.HasUnlockable(unlockableDef))
                {
                    user.RevokeUnlockable(unlockableDef);
                    user.RequestEventualSave();
                }
            }
        }
    }
}
