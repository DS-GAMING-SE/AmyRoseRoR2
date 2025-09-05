using Amy.Modules.BaseStates;
using AmyRoseMod.Characters.Survivors.Amy.Content;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace Amy.Survivors.Amy.SkillStates
{
    public class PrimaryHammer : BaseMeleeAttack
    {
        // 0 makes launches perfectly straight where you aim, 1 makes them go in the direction of the attack's swing animation (based on what Amy's animations will be)
        public float launchDirectionLerp;
        public override void OnEnter()
        {
            PrepareAnimationStats();

            swingSoundString = "HenrySwordSwing";
            hitSoundString = "";
            muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            playbackRateParam = "Slash.playbackRate";
            swingEffectPrefab = AmyAssets.swordSwingEffect;
            hitEffectPrefab = AmyAssets.swordHitImpactEffect;

            impactSound = AmyAssets.swordHitSoundEvent.index;

            base.OnEnter();
            base.StartAimMode(duration * 1.5f);
        }

        protected virtual void PrepareAnimationStats()
        {
            baseDuration = 0.9f;

            //0-1 multiplier of baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.2f;
            attackEndPercentTime = 0.4f;

            //this is the point at which the attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 0.75f;

            hitStopDuration = 0.08f;
            attackRecoil = 0.5f;
            hitHopVelocity = 8.5f;
        }

        protected override void PrepareAttackStats()
        {
            base.PrepareAttackStats();
            hitboxGroupName = swingIndex < 2 ? "HorizontalSwing" : "VerticalSwing";

            damageType = DamageTypeCombo.GenericPrimary;
            damageType.AddModdedDamageType(HedgehogUtils.Launch.DamageTypes.launchOnKill);
            damageCoefficient = AmyStaticValues.primaryHammerDamageCoefficient;
            procCoefficient = 1f;
            pushForce = AmyStaticValues.primaryHammerLaunchForce;
            launchDirectionLerp = 0.5f;
            maximumOverlapTargets = 4;
        }

        protected override void PushForceToTargetedLaunch()
        {
            if (pushForce != 0 && base.characterDirection)
            {
                Vector3 aim = base.inputBank ? base.inputBank.aimDirection : base.characterDirection.forward;
                aim.y = 0;
                DecideLaunchDirection(aim.normalized);
                pushForce = 0f;
            }
        }

        protected virtual void DecideLaunchDirection(Vector3 aim)
        {
            if (swingIndex == 0) // left
            {
                bonusForce = Vector3.Lerp(aim, Vector3.Cross(aim, Vector3.up), launchDirectionLerp).normalized * pushForce;
            }
            if (swingIndex == 1) // right
            {
                bonusForce = Vector3.Lerp(aim, Vector3.Cross(aim, Vector3.down), launchDirectionLerp).normalized * pushForce;
            }
            if (swingIndex == 2) // up
            {
                bonusForce = Vector3.Lerp(aim, Vector3.up, launchDirectionLerp).normalized * pushForce;
            }
            if (swingIndex == 3) // down
            {
                bonusForce = Vector3.Lerp(aim, Vector3.down, launchDirectionLerp).normalized * pushForce;
                damageType.AddModdedDamageType(AmyDamageTypes.angleUpKnockbackIfGrounded);
            }
        }

        protected override void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash" + (1 + (swingIndex % 2)), playbackRateParam, duration, 0.1f * duration);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}