using Amy.Survivors.Amy;
using Amy.Survivors.Amy.SkillStates;
using EntityStates;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements.Experimental;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class HammerSmashAerial : BaseSkillState
    {
        public float charge;

        public float baseHitStopDuration;
        protected float hitStopDuration;

        public float baseDuration;
        protected float duration;

        protected bool hasJumped;

        public OverlapAttack overlapAttack;
        public HealthComponent targetToIgnore;

        public override void OnEnter()
        {
            base.OnEnter();
            baseHitStopDuration = Mathf.Lerp(0.3f, 0.4f, charge);
            baseDuration = 0.7f;
            PrepareStats();
            hitStopDuration = baseHitStopDuration / attackSpeedStat;
            duration = baseDuration / attackSpeedStat;
            if (NetworkServer.active)
            {
                base.characterBody.AddTimedBuff(AmyBuffs.hammerSmashSpeedBuff, AmyStaticValues.secondaryHammerAirJumpBuffDuration);
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

            PlayAttackAnimation();
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
                base.characterMotor.velocity = Vector3.zero;
                overlapAttack.Fire();
            }
            else if (!hasJumped)
            {
                hasJumped = true;
                Jump();
            }
            if (hasJumped && fixedAge > duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }

        }

        protected virtual void PlayAttackAnimation()
        {
            PlayAnimation("Gesture, Override", "Slash1", "Slash.playbackRate", hitStopDuration);
        }

        protected virtual void PlayJumpAnimation()
        {
            PlayAnimation("FullBody, Override", "Roll", "Roll.playbackRate", duration);
        }

        protected virtual void Jump()
        {
            PlayJumpAnimation();
            Vector3 targetDirection = base.inputBank ? Vector3.Lerp(Vector3.up, base.inputBank.moveVector * 3f, base.inputBank.moveVector.magnitude * AmyStaticValues.secondaryHammerAirJumpMaxLerpFromUp) : Vector3.up;
            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity = targetDirection * base.characterBody.jumpPower * AmyStaticValues.secondaryHammerAirJumpHeightMultiplier;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
