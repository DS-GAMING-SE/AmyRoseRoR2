using AmyRoseMod.Characters.Survivors.Amy;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class MultiLockEnd : BaseState
    {
        protected float baseDuration;
        protected float duration;

        public Vector3 teleportPosition;
        
        public override void OnEnter()
        {
            base.OnEnter();
            PrepareStats();
            duration = baseDuration / attackSpeedStat;
            PlayAnimation();
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            if (base.isAuthority)
            {
                TeleportHelper.TeleportBody(base.characterBody, teleportPosition, true);
                SmallHop(base.characterMotor, 20f);
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.characterMotor)
            {
                Vector3 vel = Vector3.zero;
                vel.y = Mathf.Max(-1f, base.characterMotor.velocity.y);
                base.characterMotor.velocity = vel;
            }
            if (fixedAge >= duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }
        public override void OnExit()
        {
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, AmyStaticValues.specialMultiLockEndLingeringInvincibilityDuration);
            }
            base.OnExit();
        }

        protected virtual void PrepareStats()
        {
            baseDuration = AmyStaticValues.specialMultiLockEndDuration;
        }
        protected virtual void PlayAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash1", "Slash.playbackRate", duration, 0.1f * duration);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
