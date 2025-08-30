using RoR2;
using Amy.Modules.Achievements;

namespace Amy.Survivors.Amy.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    public class AmyMasteryAchievement : BaseMasteryAchievement
    {
        public const string identifier = AmySurvivor.AMY_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = AmySurvivor.AMY_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => AmySurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}