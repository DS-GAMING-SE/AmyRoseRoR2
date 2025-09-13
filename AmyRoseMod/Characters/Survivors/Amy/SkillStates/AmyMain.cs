using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class AmyMain : GenericCharacterMain
    {
        public override void OnEnter()
        {
            base.OnEnter();
            base.modelLocator.normalizeToFloor = true;
        }

        public override void OnExit()
        {
            base.modelLocator.normalizeToFloor = false;
            base.OnExit();
        }
    }
}
