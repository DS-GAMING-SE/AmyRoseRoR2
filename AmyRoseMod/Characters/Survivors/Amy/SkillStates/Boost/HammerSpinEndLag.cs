using AmyRoseMod.Characters.Survivors.Amy;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class HammerSpinEndLag : BaseState
    {
        protected float baseDuration;
        protected float duration;
        
        public override void OnEnter()
        {
            base.OnEnter();
            PrepareStats();
            duration = baseDuration / attackSpeedStat;
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

        protected virtual void PrepareStats()
        {
            baseDuration = AmyStaticValues.boostHammerSpinEndLagBaseDuration;
        }
        protected virtual void PlayAnimation()
        {
            PlayAnimation("FullBody, Override", "HammerSpinEnd", "Slash.playbackRate", duration * 1.4f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
