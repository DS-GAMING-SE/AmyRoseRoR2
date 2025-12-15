using System;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates.ScepterUpgrades
{
    public class ScepterMultiLockTargeting : MultiLockTargeting
    {
        public override Type nextStateType { get { return typeof(ScepterMultiLockAttack); } }
    }
}