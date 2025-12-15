using System;
using AmyRoseMod.Characters.Survivors.Amy.SkillStates.SuperFormUpgrades;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates.ScepterUpgrades
{
    public class ScepterSuperMultiLockTargeting : SuperMultiLockTargeting
    {
        public override Type nextStateType { get { return typeof(ScepterSuperMultiLockAttack); } }
    }
}