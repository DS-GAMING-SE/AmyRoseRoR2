using AmyRoseMod.Modules.BaseStates;
using AmyRoseMod.Characters.Survivors.Amy;
using AmyRoseMod.Characters.Survivors.Amy.SkillStates;
using AmyRoseMod.Characters.Survivors.Amy.Content;
using EntityStates;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class HammerSmashCharge : BaseState 
    {
        protected float baseChargeTime;
        protected float chargeTime;

        protected float minDuration;
        protected float minAirDuration;

        // 0-1
        public float charge;

        public bool maxChargeReached;

        protected float chargePerFixedUpdate
        {
            get { return (1 / Mathf.Max(chargeTime - minDuration, 0.1f)) * Time.fixedDeltaTime; }
        }

        private CharacterCameraParamsData aimingCameraParams = new CharacterCameraParamsData
        {
            maxPitch = 80f,
            minPitch = -80f,
            pivotVerticalOffset = 0.8f,
            idealLocalCameraPos = new Vector3(0f, 0f, -9f),
            wallCushion = 0.1f
        };
        private CameraTargetParams.CameraParamsOverrideHandle camOverrideHandle;

        public override void OnEnter()
        {
            base.OnEnter();
            PrepareStats();
            chargeTime = baseChargeTime / characterBody.attackSpeed;

            PlayAnimation("UpperBody, Override", "SecondaryCharge");

            this.camOverrideHandle = base.cameraTargetParams.AddParamsOverride(new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = aimingCameraParams,
                priority = 1f
            }, chargeTime * 0.5f);
        }

        protected virtual void PrepareStats()
        {
            baseChargeTime = AmyStaticValues.secondaryHammerBaseChargeTime;
            minDuration = 0.4f;
            minAirDuration = 0.2f;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= minDuration)
            {
                charge = Mathf.Min(charge + chargePerFixedUpdate, 1);
                if (charge == 1 && !maxChargeReached)
                {
                    maxChargeReached = true;
                    ReachedMaxCharge();
                }
                if (base.isAuthority && inputBank && !inputBank.skill2.down)
                {
                    SetNextStateToSmash();
                    return;
                }
            }
            else if (fixedAge >= minAirDuration && base.isAuthority && inputBank && !inputBank.skill2.down && !base.characterMotor.isGrounded && !characterMotor.isFlying)
            {
                SetNextStateToSmash();
                return;
            }
        }

        public override void OnExit()
        {
            PlayAnimation("UpperBody, Override", "BufferEmpty");
            base.cameraTargetParams.RemoveParamsOverride(this.camOverrideHandle, 0.5f);
            base.OnExit();
        }

        protected virtual void ReachedMaxCharge()
        {
            EffectManager.SimpleMuzzleFlash(AmyAssets.swordHitImpactEffect, base.gameObject, "Head", false);
        }

        protected virtual void SetNextStateToSmash()
        {
            if (base.characterMotor.isGrounded || characterMotor.isFlying)
            {
                HammerSmashGrounded state = (HammerSmashGrounded)EntityStateCatalog.InstantiateState(typeof(HammerSmashGrounded));
                if (state != null)
                {
                    state.charge = charge;
                }
                this.outer.SetNextState(state);
            }
            else
            {
                HammerSmashChargeAerial state = (HammerSmashChargeAerial)EntityStateCatalog.InstantiateState(typeof(HammerSmashChargeAerial));
                if (state != null)
                {
                    state.charge = charge;
                }
                this.outer.SetNextState(state);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
