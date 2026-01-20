using AmyRoseMod.Characters.Survivors.Amy.SkillStates.SuperFormUpgrades;
using System;
using UnityEngine;
using RoR2;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates.ScepterUpgrades
{
    public class ScepterSuperMultiLockTargeting : SuperMultiLockTargeting
    {
        public override Type nextStateType { get { return typeof(ScepterSuperMultiLockAttack); } }
        protected override Indicator CreateIndicator(Transform targetTransform)
        {
            Indicator targetIndicator = new Indicator(base.gameObject, HedgehogUtils.Assets.lockOnIndicator);
            targetIndicator.targetTransform = targetTransform;
            targetIndicator.active = true;
            if (targetIndicator.hasVisualizer)
            {
                targetIndicator.visualizerInstance.GetComponent<HedgehogUtils.Miscellaneous.LockOnIndicator>().SetColors(AmySurvivor.superAmyColor, AmySurvivor.scepterAmyColor);
            }
            return targetIndicator;
        }
    }
}