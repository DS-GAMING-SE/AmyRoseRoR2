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
using RoR2.UI;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class MultiLockTargeting : BaseState
    {
        public List<Indicator> targets;
        public List<HurtBox> targetHurtBoxes;

        public BullseyeSearch search;

        public float noTargetMinDuration = 0.4f;

        public float orbBounceRange = AmyStaticValues.specialMultiLockBounceRange;

        public int maxTargets;

        public Action<int> OnTargetsChanged;

        protected static string[] targetBodyBlacklist = { "GravekeeperTrackingFireball" };

        protected CrosshairUtils.OverrideRequest crosshair;

        public virtual Type nextStateType { get { return typeof(MultiLockAttack); } }


        public override void OnEnter()
        {
            base.OnEnter();
            PrepareStatsStart();
            if (base.isAuthority)
            {
                crosshair = CrosshairUtils.RequestOverrideForBody(characterBody, AmyAssets.multiLockCrosshair, CrosshairUtils.OverridePriority.PrioritySkill);
                targets = new List<Indicator>();
                targetHurtBoxes = new List<HurtBox>();
                PrepareSearch();
                base.characterBody.skillLocator.special.onSkillChanged += OnSkillChanged;
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
            if (base.isAuthority)
            {
                foreach (var target in targets)
                {
                    target.active = false;
                }
                crosshair.Dispose();
                base.characterBody.skillLocator.special.onSkillChanged -= OnSkillChanged;
            }
            base.OnExit();
        }

        public virtual void PrepareStatsStart()
        {
            maxTargets = AmyStaticValues.specialMultiLockMaxTargets;
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

        protected virtual void UpdateSearchStats()
        {
            this.search.maxDistanceFilter = AmyStaticValues.specialMultiLockSearchRange;
        }

        protected virtual void Search()
        {
            this.search.searchOrigin = inputBank.GetAimRay().origin;
            this.search.searchDirection = inputBank.GetAimRay().direction;
            UpdateSearchStats();

            this.search.RefreshCandidates();

            SanitizeAndFilterTargets();

            HurtBox hit = this.search.GetResults().FirstOrDefault(target => target.healthComponent && target.healthComponent.alive && target.healthComponent.body && 
                !targetBodyBlacklist.Contains(BodyCatalog.GetBodyName(target.healthComponent.body.bodyIndex))
                && (this.targets.Count == 0 || Vector3.Distance(target.transform.position, targetHurtBoxes[targetHurtBoxes.Count - 1].transform.position) < orbBounceRange));
            if (hit)
            {
                AddTarget(hit);
            }
        }

        public void SanitizeAndFilterTargets()
        {
            this.search.FilterOutGameObject(base.gameObject);
            for (int i = 0; i < this.targets.Count; i++)
            {
                if (targetHurtBoxes[i] && targetHurtBoxes[i].healthComponent && targetHurtBoxes[i].healthComponent.alive &&
                    Vector3.Distance(targetHurtBoxes[i].transform.position, base.transform.position) < this.search.maxDistanceFilter &&
                    (i == 0 || Vector3.Distance(targetHurtBoxes[i].transform.position, targetHurtBoxes[i - 1].transform.position) < orbBounceRange))
                {
                    this.search.FilterOutGameObject(targetHurtBoxes[i].healthComponent.gameObject);
                }
                else
                {
                    RemoveTarget(i);
                    i--;
                }
            }
        }

        public virtual void AddTarget(HurtBox target)
        {
            Indicator targetIndicator = new Indicator(base.gameObject, LegacyResourcesAPI.Load<GameObject>("Prefabs/EngiMissileTrackingIndicator"));
            targetIndicator.targetTransform = target.transform;
            targetIndicator.active = true;
            this.targets.Add(targetIndicator);
            this.targetHurtBoxes.Add(target);
            OnTargetsChanged?.Invoke(targets.Count);
            Util.PlaySound("Play_hedgehogutils_lockon", base.gameObject);
        }

        public virtual void RemoveTarget(int index)
        {
            targets[index].active = false;
            targets.RemoveAt(index);
            targetHurtBoxes.RemoveAt(index);
            OnTargetsChanged?.Invoke(targets.Count);
        }

        public virtual void SetNextStateToAttack()
        {
            this.outer.SetNextStateToMain();
            EntityStateMachine body = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
            if (body)
            {
                MultiLockAttack state = (MultiLockAttack)EntityStateCatalog.InstantiateState(nextStateType);
                state.target = targetHurtBoxes[0];
                state.targets = targetHurtBoxes;
                state.orbStartPosition = base.transform.position;
                state.firstAttack = true;
                state.orbBounceRange = this.orbBounceRange;
                body.SetNextState(state);
            }
        }

        private void OnSkillChanged(GenericSkill skill)
        {
            this.outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}