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

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class MultiLockTargeting : BaseState
    {
        public List<HurtBox> targets;

        public BullseyeSearch search;

        public float noTargetMinDuration = 0.4f;

        public int maxTargets = AmyStaticValues.specialMultiLockMaxTargets;

        
        public override void OnEnter()
        {
            base.OnEnter();
            PrepareStats();
            if (base.isAuthority)
            {
                targets = new List<HurtBox>();
                PrepareSearch();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                if (targets.Count < maxTargets)
                {
                    Search();
                }

                if (!base.inputBank.skill4.down)
                {
                    if (targets.Count > 0)
                    {
                        SetNextStateToAttack();
                        return;
                    }
                    else if (fixedAge >= noTargetMinDuration)
                    {
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public virtual void PrepareStats()
        {

        }

        protected virtual void PrepareSearch()
        {
            this.search = new BullseyeSearch();
            this.search.teamMaskFilter = TeamMask.GetEnemyTeams(this.teamComponent.teamIndex);
            this.search.filterByDistinctEntity = true;
            this.search.filterByLoS = true;
            this.search.sortMode = BullseyeSearch.SortMode.Angle;
            this.search.minDistanceFilter = 0;
            this.search.minAngleFilter = 0;
            this.search.maxAngleFilter = 8;
            this.search.viewer = base.characterBody;
        }

        protected virtual void UpdateSearch()
        {
            this.search.maxDistanceFilter = 40;
            this.search.searchOrigin = inputBank.GetAimRay().origin;
            this.search.searchDirection = inputBank.GetAimRay().direction;
        }

        protected virtual void Search()
        {
            UpdateSearch();
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);
            for (int i = 0; i < this.targets.Count; i++)
            {
                if (targets[i] && targets[i].healthComponent && targets[i].healthComponent.alive)
                {
                    this.search.FilterOutGameObject(targets[i].healthComponent.gameObject);
                }
                else
                {
                    targets.RemoveAt(i);
                    i--;
                }
            }
            HurtBox hit = this.search.GetResults().FirstOrDefault(target => target.healthComponent && target.healthComponent.alive);
            if (hit)
            {
                this.targets.Add(hit);
            }
        }

        public virtual void SetNextStateToAttack()
        {
            this.outer.SetNextStateToMain();
            EntityStateMachine body = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
            if (body)
            {
                MultiLockAttack state = (MultiLockAttack)EntityStateCatalog.InstantiateState(typeof(MultiLockAttack));
                state.targets = this.targets;
                body.SetNextState(state);
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}