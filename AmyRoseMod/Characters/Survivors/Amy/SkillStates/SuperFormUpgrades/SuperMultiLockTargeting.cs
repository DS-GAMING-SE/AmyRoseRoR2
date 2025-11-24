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
        public override void PrepareStatsStart()
        {
            maxTargets = AmyStaticValues.superSpecialMultiLockMaxTargets;
        }

        protected override void UpdateSearchStats()
        {
            orbBounceRange = AmyStaticValues.superSpecialMultiLockBounceRange;
            this.search.maxDistanceFilter = AmyStaticValues.superSpecialMultiLockSearchRange;
        }

        public override void SetNextStateToAttack()
        {
            this.outer.SetNextStateToMain();
            EntityStateMachine body = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
            if (body)
            {
                MultiLockAttack state = (MultiLockAttack)EntityStateCatalog.InstantiateState(typeof(SuperMultiLockAttack));
                state.target = targetHurtBoxes[0];
                state.targets = targetHurtBoxes;
                state.orbStartPosition = base.transform.position;
                state.firstAttack = true;
                state.orbBounceRange = this.orbBounceRange;
                body.SetNextState(state);
            }
        }
    }
}