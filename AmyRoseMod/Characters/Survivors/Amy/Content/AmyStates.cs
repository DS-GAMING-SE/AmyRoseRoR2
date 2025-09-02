using Amy.Survivors.Amy.SkillStates;
using AmyRoseMod.Characters.Survivors.Amy.SkillStates;

namespace Amy.Survivors.Amy
{
    public static class AmyStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(PrimaryHammer));

            Modules.Content.AddEntityState(typeof(HammerSmashCharge));
            Modules.Content.AddEntityState(typeof(HammerSmashGrounded));

            Modules.Content.AddEntityState(typeof(Boost));
            Modules.Content.AddEntityState(typeof(BoostIdle));
            Modules.Content.AddEntityState(typeof(Brake));

            Modules.Content.AddEntityState(typeof(ThrowBomb));
        }
    }
}
