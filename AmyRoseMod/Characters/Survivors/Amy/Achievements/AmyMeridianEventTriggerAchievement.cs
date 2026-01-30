using RoR2;
using AmyRoseMod.Modules.Achievements;
using RoR2.Achievements;

namespace AmyRoseMod.Characters.Survivors.Amy.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 5U, typeof(AmyRoseMeridianEventTriggerServerAchievement))]
    public class AmyMeridianEventTriggerAchievement : BaseAchievement
    {
        public const string identifier = AmySurvivor.AMY_PREFIX + "meridianAchievement";
        public const string unlockableIdentifier = AmySurvivor.AMY_PREFIX + "meridianUnlockable";

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("AmyRoseBody");
        }

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            base.SetServerTracked(true);
        }

        public override void OnBodyRequirementBroken()
        {
            base.SetServerTracked(false);
            base.OnBodyRequirementBroken();
        }

        // Token: 0x020014A2 RID: 5282
        private class AmyRoseMeridianEventTriggerServerAchievement : BaseServerAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                EntityStates.FalseSonBoss.SkyJumpDeathState.falseSonDeathEvent += this.OnMeridianEventTriggerActivated;
            }

            public override void OnUninstall()
            {
                base.OnUninstall();
                EntityStates.FalseSonBoss.SkyJumpDeathState.falseSonDeathEvent -= this.OnMeridianEventTriggerActivated;
            }

            private void OnMeridianEventTriggerActivated()
            {
                base.Grant();
            }
        }
    }
}