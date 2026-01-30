using RoR2;
using AmyRoseMod.Modules.Achievements;
using RoR2.Achievements;

namespace AmyRoseMod.Characters.Survivors.Amy.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 5U, null)]
    public class AmyDecompileAchievement : BasePerSurvivorDecompileAchievement
    {
        public const string identifier = AmySurvivor.AMY_PREFIX + "decompileAchievement";
        public const string unlockableIdentifier = AmySurvivor.AMY_PREFIX + "decompileUnlockable";
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("AmyRoseBody");
        }
    }
}