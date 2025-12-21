using AmyRoseMod.Characters.Survivors.Amy;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using Unity.Collections;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class MultiLockEnd : BaseState
    {
        protected float baseDuration;
        protected float duration;
        protected const float vfxDurationPercent = 0.2f;
        private bool vfxPlayed;

        protected float startSpeed;

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
            Util.PlaySound("Play_amyrose_multilock_end", base.gameObject);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.characterMotor && base.characterDirection)
            {
                Vector3 vel = characterDirection.forward;
                vel *= 0.7f;
                vel.y = 1f;
                vel *= startSpeed * (1f - (fixedAge / duration));
                vel *= attackSpeedStat;
                base.characterMotor.velocity = vel;
            }
            if (fixedAge >= duration * vfxDurationPercent && !vfxPlayed)
            {
                PlayVFX();
                vfxPlayed = true;
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
            startSpeed = 17f;
        }
        protected virtual void PlayAnimation()
        {
            PlayCrossfade("FullBody, Override", "MultiLock", "Slash.playbackRate", duration * 1.3f, 0.1f * duration);
        }

        protected virtual void PlayVFX()
        {
            EffectManager.SimpleMuzzleFlash(AmyAssets.multiLockEndEffect, base.gameObject, "MultiLockEndTransform", false);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
