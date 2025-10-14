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
        public static void Init()
        {
            // Mastery
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                AmyMasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(AmyMasteryAchievement.unlockableIdentifier),
                AmySurvivor.instance.assetBundle.LoadAsset<Sprite>("texPaladinSkinIcon"));
            masterySkinUnlockableConfig = CreateUnlockableConfig("Mastery");
            masterySkinUnlockableConfig.SettingChanged += new EventHandler(delegate (object o, EventArgs a)
            { UnlockableConfigChanged(masterySkinUnlockableConfig.Value, masterySkinUnlockableDef, AmyMasteryAchievement.unlockableIdentifier); });



            On.RoR2.UserProfile.OnLogin += ConfigUnlocks;
        }
        private static void ConfigUnlocks(On.RoR2.UserProfile.orig_OnLogin orig, UserProfile self)
        {
            orig(self);
            if (!self.HasAchievement(AmyMasteryAchievement.unlockableIdentifier) && masterySkinUnlockableConfig.Value)
            {
                self.AddAchievement(AmyMasteryAchievement.unlockableIdentifier, true);
            }
        }

        private static ConfigEntry<bool> CreateUnlockableConfig(string achievementName)
        {
            return Config.BindAndOptions("Unlockables", achievementName, false, $"Unlock or relock the achievement \"Amy: {achievementName}\". Locking the achievement may require restarting the game for the achievement to be obtainable again.");
        }

        private static void UnlockableConfigChanged(bool configValue, UnlockableDef unlockableDef, string achievementToken)
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
