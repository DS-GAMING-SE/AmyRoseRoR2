using Amy.Modules.BaseStates;
using Amy.Survivors.Amy;
using Amy.Survivors.Amy.SkillStates;
using AmyRoseMod.Characters.Survivors.Amy.Content;
using EntityStates;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class HammerSmashCharge : BaseState 
    {
        protected float baseChargeTime;
        protected float chargeTime;

        protected float minDuration;

        // 0-1
        public float charge;

        public bool maxChargeReached;

        protected float chargePerFixedUpdate
        {
            get { return (1 / Mathf.Max(chargeTime - minDuration, 0.1f)) * Time.fixedDeltaTime; }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            PrepareStats();
            chargeTime = baseChargeTime / characterBody.attackSpeed;
        }

        protected virtual void PrepareStats()
        {
            baseChargeTime = AmyStaticValues.secondaryHammerBaseChargeTime;
            minDuration = 0.4f;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= minDuration)
            {
                charge = Mathf.Min(charge + chargePerFixedUpdate, 1);
                if (charge == 1 && !maxChargeReached)
                {
                    maxChargeReached = true;
                    ReachedMaxCharge();
                }
                if (base.isAuthority && inputBank && !inputBank.skill2.down)
                {
                    SetNextStateToSmash();
                    return;
                }
            }
        }

        protected virtual void ReachedMaxCharge()
        {
            EffectManager.SimpleMuzzleFlash(AmyAssets.swordHitImpactEffect, base.gameObject, "Head", true);
        }

        protected virtual void SetNextStateToSmash()
        {
            HammerSmashGrounded state = EntityStateCatalog.InstantiateState(typeof(HammerSmashGrounded)) as HammerSmashGrounded;
            state.charge = charge;
            this.outer.SetNextState(state);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
