using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.Networking;
using HedgehogUtils.Boost;
using System.Collections.Generic;
using System.Linq;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates.SuperFormUpgrades
{
    public class SuperMultiLockTargeting : MultiLockTargeting
    {
        public override Type nextStateType { get { return typeof(SuperMultiLockAttack); } }
        public override void PrepareStatsStart()
        {
            maxTargets = AmyStaticValues.superSpecialMultiLockMaxTargets;
        }

        protected override void UpdateSearchStats()
        {
            orbBounceRange = AmyStaticValues.superSpecialMultiLockBounceRange;
            this.search.maxDistanceFilter = AmyStaticValues.superSpecialMultiLockSearchRange;
        }
    }
}