using Amy.Survivors.Amy.Achievements;
using RoR2;
using UnityEngine;

namespace Amy.Survivors.Amy
{
    public static class AmyUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                AmyMasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(AmyMasteryAchievement.identifier),
                AmySurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
