using AmyRoseMod.Characters.Survivors.Amy.SkillStates;
using AmyRoseMod.Characters.Survivors.Amy.SkillStates.SuperFormUpgrades;

namespace AmyRoseMod.Characters.Survivors.Amy
{
    public static class AmyStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(AmyMain));

            Modules.Content.AddEntityState(typeof(PrimaryHammer));

            Modules.Content.AddEntityState(typeof(HammerSmashCharge));
            Modules.Content.AddEntityState(typeof(HammerSmashGrounded));
            Modules.Content.AddEntityState(typeof(HammerSmashChargeAerial));
            Modules.Content.AddEntityState(typeof(HammerSmashAerial));

            Modules.Content.AddEntityState(typeof(SuperHammerSmashCharge));
            Modules.Content.AddEntityState(typeof(SuperHammerSmashGrounded));

            Modules.Content.AddEntityState(typeof(Boost));
            Modules.Content.AddEntityState(typeof(BoostIdle));
            Modules.Content.AddEntityState(typeof(Brake));

            Modules.Content.AddEntityState(typeof(HammerSpin));
            Modules.Content.AddEntityState(typeof(Dizzy));
            Modules.Content.AddEntityState(typeof(HammerSpinEndLag));

            Modules.Content.AddEntityState(typeof(SuperBoost));

            Modules.Content.AddEntityState(typeof(SuperHammerSpin));

            Modules.Content.AddEntityState(typeof(MultiLockTargeting));
            Modules.Content.AddEntityState(typeof(MultiLockAttack));
            Modules.Content.AddEntityState(typeof(MultiLockEnd));
        }
    }
}
