using AmyRoseMod.Characters.Survivors.Amy;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class HammerSmashAerialJump: BaseState
    {
        protected float baseDuration;
        protected float duration;
        protected static float earlyExitPercent = 0.7f;
        
        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                base.characterBody.AddTimedBuff(AmyBuffs.hammerSmashSpeedBuff, AmyStaticValues.secondaryHammerAirJumpBuffDuration);
            }
            PrepareStats();
            duration = baseDuration / attackSpeedStat;
            if (base.isAuthority)
            {
                Jump();
            }
            PlayAnimation();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }
        protected virtual void Jump()
        {
            Vector3 velocity = Vector3.up * (base.inputBank ? 1 - (base.inputBank.moveVector.magnitude * AmyStaticValues.secondaryHammerAirJumpHeightReductionWhenAngled) : 1f);
            velocity *= base.characterBody.jumpPower * AmyStaticValues.secondaryHammerAirJumpHeightMultiplier;
            if (base.inputBank)
            {
                velocity += base.characterBody.moveSpeed * AmyStaticValues.secondaryHammerAirJumpHorizontalSpeedMult * base.inputBank.moveVector;
            }
            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity = velocity;
        }

        protected virtual void PrepareStats()
        {
            baseDuration = 1f;
        }
        protected virtual void PlayAnimation()
        {
            PlayCrossfade("FullBody, Override", "SecondaryAirRebound", "Slash.playbackRate", duration * 1.3f, 0.1f * duration);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return baseDuration / duration >= earlyExitPercent ? InterruptPriority.Skill : InterruptPriority.Pain;
        }
    }
}
