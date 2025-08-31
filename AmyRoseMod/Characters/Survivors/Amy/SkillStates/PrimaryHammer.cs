using Amy.Modules.BaseStates;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace Amy.Survivors.Amy.SkillStates
{
    public class PrimaryHammer : BaseMeleeAttack
    {
        // 0 makes launches perfectly straight, 1 makes them go in the direction of the attack
        public float launchDirectionLerp;
        public override void OnEnter()
        {
            PrepareAttackStats();
            if (pushForce != 0 && base.characterDirection)
            {
                DecideLaunchDirection();
            }
            PrepareAnimationStats();

            swingSoundString = "HenrySwordSwing";
            hitSoundString = "";
            muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            playbackRateParam = "Slash.playbackRate";
            swingEffectPrefab = AmyAssets.swordSwingEffect;
            hitEffectPrefab = AmyAssets.swordHitImpactEffect;

            impactSound = AmyAssets.swordHitSoundEvent.index;

            base.OnEnter();
        }

        protected virtual void PrepareAttackStats()
        {
            hitboxGroupName = "SwordGroup";

            damageType = DamageTypeCombo.GenericPrimary;
            damageType.AddModdedDamageType(HedgehogUtils.Launch.DamageTypes.launchOnKill);
            damageCoefficient = AmyStaticValues.primaryHammerDamageCoefficient;
            procCoefficient = 1f;
            pushForce = AmyStaticValues.primaryHammerLaunchForce;
            bonusForce = Vector3.zero;
            launchDirectionLerp = 0.5f;
        }

        protected virtual void DecideLaunchDirection()
        {
            if (swingIndex == 0) // left
            {
                bonusForce = Vector3.Lerp(base.characterDirection.forward, Vector3.Cross(base.characterDirection.forward, Vector3.up), launchDirectionLerp).normalized * pushForce;
            }
            if (swingIndex == 1) // right
            {
                bonusForce = Vector3.Lerp(base.characterDirection.forward, Vector3.Cross(base.characterDirection.forward, Vector3.down), launchDirectionLerp).normalized * pushForce;
            }
            if (swingIndex == 2) // up
            {
                bonusForce = Vector3.Lerp(base.characterDirection.forward, Vector3.up, launchDirectionLerp).normalized * pushForce;
            }
            if (swingIndex == 3) // down
            {
                bonusForce = Vector3.Lerp(base.characterDirection.forward, Vector3.down, launchDirectionLerp).normalized * pushForce;
            }
            pushForce = 0f;
        }
        protected virtual void PrepareAnimationStats()
        {
            baseDuration = 1.1f;

            //0-1 multiplier of baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.2f;
            attackEndPercentTime = 0.4f;

            //this is the point at which the attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 0.6f;

            hitStopDuration = 0.012f;
            attackRecoil = 0.5f;
            hitHopVelocity = 4f;
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