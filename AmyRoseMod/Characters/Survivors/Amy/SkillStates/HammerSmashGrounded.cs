using Amy.Modules.BaseStates;
using AmyRoseMod.Characters.Survivors.Amy.Content;
using AmyRoseMod.Characters.Survivors.Amy.SkillStates;
using EntityStates;
using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Amy.Survivors.Amy.SkillStates
{
    public class HammerSmashGrounded : BaseMeleeAttack
    {
        public float charge;
        public override void OnEnter()
        {
            PrepareAnimationStats();

            swingSoundString = "HenrySwordSwing";
            hitSoundString = "";
            muzzleString = "SwingLeft";
            playbackRateParam = "Slash.playbackRate";
            swingEffectPrefab = AmyAssets.swordSwingEffect;
            hitEffectPrefab = AmyAssets.swordHitImpactEffect;

            impactSound = AmyAssets.swordHitSoundEvent.index;

            base.OnEnter();
            base.StartAimMode(duration * 1.3f);
        }

        protected virtual void PrepareAnimationStats()
        {
            baseDuration = 1.5f;

            //0-1 multiplier of baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.1f;
            attackEndPercentTime = 0.3f;

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
            damageType.AddModdedDamageType(AmyDamageTypes.launchNoAutoAim);
            damageCoefficient = Mathf.Lerp(AmyStaticValues.secondaryHammerChargeMinimumDamageCoefficient, AmyStaticValues.secondaryHammerChargeMaximumDamageCoefficient, charge);
            procCoefficient = charge == 1 ? AmyStaticValues.secondaryHammerChargeMaximumProcCoefficient : AmyStaticValues.secondaryHammerChargeMinimumProcCoefficient;
            pushForce = AmyStaticValues.secondaryHammerLaunchForce;
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
            if (base.characterMotor && stopwatch <= duration * attackEndPercentTime)
            {
                base.characterMotor.velocity.y = Mathf.Max(characterMotor.velocity.y, -3);
            }
        }

        protected virtual void DecideLaunchDirection(Vector3 aim)
        {
            bonusForce = aim * pushForce;
        }

        protected override void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash1", playbackRateParam, duration, 0.1f * duration);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (stopwatch >= duration * earlyExitPercentTime)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.PrioritySkill;
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