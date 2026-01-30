using RoR2;
using AmyRoseMod.Modules.Achievements;
using RoR2.Achievements;

namespace AmyRoseMod.Characters.Survivors.Amy.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 5U, typeof(KillSolusHeartServerAchievement))]
    public class AmyPurgeAchievement : BasePerSurvivorPurgeAchievement
    {
        public const string identifier = AmySurvivor.AMY_PREFIX + "purgeAchievement";
        public const string unlockableIdentifier = AmySurvivor.AMY_PREFIX + "purgeUnlockable";
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("AmyRoseBody");
        }
        protected class KillSolusHeartServerAchievement : BasePerSurvivorPurgeAchievement.BasePerSurvivorKillSolusHeartServerAchievement
        {
        }
    }
}