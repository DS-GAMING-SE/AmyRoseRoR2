using  AmyRoseMod.Characters.Survivors.Amy;
using  AmyRoseMod.Characters.Survivors.Amy.SkillStates;
using EntityStates;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using static EntityStates.BaseState;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class HammerSmashAerial : BaseSkillState
    {
        public float charge;

        public float baseHitStopDuration;
        protected float hitStopDuration;

        public float baseDuration;
        protected float duration;

        protected bool bufferedJump;
        protected bool hasJumped;

        public OverlapAttack overlapAttack;
        public HealthComponent targetToIgnore;

        private Animator animator;
        private float cachedAnimationDuration;

        public override void OnEnter()
        {
            base.OnEnter();
            baseHitStopDuration = Mathf.Lerp(0.3f, 0.4f, charge);
            baseDuration = 0.7f;
            PrepareStats();
            hitStopDuration = (baseHitStopDuration / 2) + ((baseHitStopDuration / 2) / attackSpeedStat);
            duration = baseDuration / attackSpeedStat;
            if (NetworkServer.active)
            {
                base.characterBody.AddTimedBuff(JunkContent.Buffs.IgnoreFallDamage, 0.3f);
            }
            if (base.isAuthority)
            {
                overlapAttack = new OverlapAttack();
                overlapAttack.damageType = DamageTypeCombo.GenericSecondary | DamageType.Stun1s;
                overlapAttack.damage = damageStat * Mathf.Lerp(AmyStaticValues.secondaryHammerChargeMinimumDamageCoefficient, AmyStaticValues.secondaryHammerChargeMaximumDamageCoefficient, charge);
                overlapAttack.hitBoxGroup = FindHitBoxGroup("LargeStomp");
                overlapAttack.isCrit = RollCrit();
                overlapAttack.procCoefficient = charge == 1 ? 1.5f : 1f;
                overlapAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                overlapAttack.inflictor = base.gameObject;
                overlapAttack.attacker = base.gameObject;
                overlapAttack.teamIndex = GetTeam();
                overlapAttack.pushAwayForce = 200f;
                overlapAttack.forceVector = Vector3.up * 500f;
                if (targetToIgnore)
                {
                    overlapAttack.addIgnoredHitList(targetToIgnore);
                }
                PrepareAttack();
            }
            animator = GetModelAnimator();

            Util.PlaySound("Play_amyrose_hit_heavy", base.gameObject);

            PlayJumpAnimation();
            cachedAnimationDuration = animator.GetFloat("Slash.playbackRate");
        }

        public virtual void PrepareStats()
        {

        }

        public virtual void PrepareAttack()
        {

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                base.characterBody.isSprinting = true;
            }
            if (fixedAge <= hitStopDuration)
            {
                if (base.inputBank.skill2.down)
                {
                    bufferedJump = true;
                }
                if (animator) animator.SetFloat("Slash.playbackRate", 0.1f);
                base.characterMotor.velocity = Vector3.zero;
                overlapAttack.Fire();
            }
            else if (!hasJumped)
            {
                hasJumped = true;
                if (base.inputBank.skill2.down || bufferedJump)
                {
                    base.inputBank.skill2.hasPressBeenClaimed = true;
                    Jump();
                    return;
                }
                else
                {
                    SmallHop(base.characterMotor, 12f);
                    if (animator) animator.SetFloat("Slash.playbackRate", cachedAnimationDuration);
                }
            }
            if (hasJumped && fixedAge > duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }

        }

        protected virtual void PlayJumpAnimation()
        {
            PlayAnimation("FullBody, Override", "SecondaryAirLand", "Slash.playbackRate", duration);
        }

        protected virtual void Jump()
        {
            this.outer.SetNextState(new HammerSmashAerialJump());
        }

        public override void OnExit()
        {
            if (!hasJumped && animator)
            {
                animator.SetFloat("Slash.playbackRate", cachedAnimationDuration);
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
