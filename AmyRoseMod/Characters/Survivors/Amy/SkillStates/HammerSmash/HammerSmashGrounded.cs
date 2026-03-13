using AmyRoseMod.Modules.BaseStates;
using AmyRoseMod.Characters.Survivors.Amy.Content;
using AmyRoseMod.Characters.Survivors.Amy.SkillStates;
using EntityStates;
using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class HammerSmashGrounded : BaseMeleeAttack
    {
        protected override GameObject swingEffectPrefab { get { return AmyAssets.hammerSwingLargeEffect; } }
        protected override GameObject hitEffectPrefab { get { return null; } }
        public float charge;
        private bool hasHit;
        private bool hasHitGround;
        protected float hammerHitGroundPercentTime;
        public override void OnEnter()
        {
            PrepareAnimationStats();

            swingSoundString = "Play_amyrose_swing_heavy";
            muzzleString = "LargeSwingDown";
            playbackRateParam = "Slash.playbackRate";

            base.OnEnter();
            base.StartAimMode(duration * 1.3f);
        }

        protected virtual void PrepareAnimationStats()
        {
            baseDuration = 1.5f;

            //0-1 multiplier of baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.12f;
            attackEndPercentTime = 0.25f;
            hammerHitGroundPercentTime = 0.19f;

            //this is the point at which the attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 0.75f;

            hitStopDuration = Mathf.Lerp(0.2f, 0.27f, charge);
            attackRecoil = 1f;
            hitHopVelocity = 6f;
        }

        protected override void PrepareAttackStats()
        {
            base.PrepareAttackStats();
            hitboxGroupName = "LargeSwing";

            damageType = DamageTypeCombo.GenericSecondary;
            damageType.AddModdedDamageType(HedgehogUtils.Launch.DamageTypes.launch);
            damageType.AddModdedDamageType(HedgehogUtils.Launch.DamageTypes.removeLaunchAutoAim);
            damageCoefficient = Mathf.Lerp(AmyStaticValues.secondaryHammerChargeMinimumDamageCoefficient, AmyStaticValues.secondaryHammerChargeMaximumDamageCoefficient, charge);
            procCoefficient = charge == 1 ? AmyStaticValues.secondaryHammerChargeMaximumProcCoefficient : AmyStaticValues.secondaryHammerChargeMinimumProcCoefficient;
            pushForce = charge == 1  ? AmyStaticValues.secondaryHammerMaxLaunchForce : AmyStaticValues.secondaryHammerMinLaunchForce;
        }

        protected override void PushForceToTargetedLaunch()
        {
            if (pushForce != 0)
            {
                Vector3 aim = base.inputBank ? base.inputBank.aimDirection : base.characterDirection.forward;
                DecideLaunchDirection(aim.normalized);
                pushForce = 0f;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            //float minY = Mathf.Lerp(-2f, -4f, (fixedAge - (duration * attackEndPercentTime)) * (1 / (duration * attackEndPercentTime)));
            if (stopwatch <= duration * attackEndPercentTime)
            {
                if (base.characterMotor && !base.characterMotor.isFlying)
                {
                    base.characterMotor.velocity.y = Mathf.Max(characterMotor.velocity.y, -3);
                }
            }
            if (stopwatch > duration * hammerHitGroundPercentTime && !hasHitGround)
            {
                hasHitGround = true;
                if (base.isAuthority && base.characterMotor.isGrounded)
                {
                    EffectManager.SimpleMuzzleFlash(AmyAssets.secondaryHitGroundEffect, base.gameObject, "SecondaryGround", true);
                }
            }
        }

        protected virtual void DecideLaunchDirection(Vector3 aim)
        {
            bonusForce = aim * pushForce;
        }

        protected override void PlayAttackAnimation()
        {
            PlayAnimation("FullBody, Override", "SecondaryAttack", "Slash.playbackRate", duration * 1.1f);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
            if (!hasHit)
            {
                hasHit = true;
                PlayHitEffect(base.transform.position + bonusForce.normalized + new Vector3(0,0.5f,0f), Quaternion.LookRotation(bonusForce));
            }
        }
        protected virtual void PlayHitEffect(Vector3 position, Quaternion direction)
        {
            EffectManager.SimpleEffect(AmyAssets.secondaryHitEffect, position, direction, true);
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (stopwatch >= duration * earlyExitPercentTime)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.Pain;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(charge);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            charge = reader.ReadSingle();
        }
    }
}