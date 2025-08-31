using Amy.Survivors.Amy.SkillStates;

namespace Amy.Survivors.Amy
{
    public static class AmyStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(PrimaryHammer));

            Modules.Content.AddEntityState(typeof(Shoot));

            Modules.Content.AddEntityState(typeof(Boost));
            Modules.Content.AddEntityState(typeof(BoostIdle));
            Modules.Content.AddEntityState(typeof(Brake));

            Modules.Content.AddEntityState(typeof(ThrowBomb));
        }
    }
}
