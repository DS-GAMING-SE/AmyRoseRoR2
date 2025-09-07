using Amy.Survivors.Amy;
using Amy.Survivors.Amy.SkillStates;
using EntityStates;
using HedgehogUtils.Boost;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class HammerSmashChargeAerial : BaseSkillState
    {
        public OverlapAttack overlapAttack;

        public float startingHeight;

        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority)
            {
                overlapAttack = new OverlapAttack();
                overlapAttack.maximumOverlapTargets = 1;
                overlapAttack.procCoefficient = 1f;
                overlapAttack.damage = damageStat * AmyStaticValues.secondaryHammerChargeMinimumDamageCoefficient;
                overlapAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                overlapAttack.damageType = DamageTypeCombo.GenericSecondary; 
                overlapAttack.damageType.AddModdedDamageType(HedgehogUtils.Launch.DamageTypes.launch);
                overlapAttack.forceVector = Vector3.down * AmyStaticValues.secondaryHammerLaunchForce;
                overlapAttack.isCrit = RollCrit();
                overlapAttack.attacker = base.gameObject;
                overlapAttack.inflictor = base.gameObject;
                overlapAttack.hitBoxGroup = FindHitBoxGroup("Stomp");
                overlapAttack.teamIndex = base.teamComponent.teamIndex;
                PrepareAttackStats();
                if (base.characterMotor)
                {
                    SmallHop(characterMotor, characterBody.jumpPower);
                    base.characterMotor.onHitGroundAuthority += OnGroundHit;
                    startingHeight = transform.position.y;
                }
            }
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(JunkContent.Buffs.IgnoreFallDamage);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                base.characterBody.isSprinting = false;
                if (base.characterMotor)
                {
                    base.characterMotor.velocity.y = Mathf.Max(-AmyStaticValues.secondaryHammerAirFallMaxSpeed, base.characterMotor.velocity.y - (AmyStaticValues.secondaryHammerAirFallAcceleration * Time.fixedDeltaTime));

                    if (base.characterMotor.velocity.y <= -4)
                    {
                        BeforeAttackFire();
                        List<HurtBox> hit = new List<HurtBox>();
                        if (overlapAttack.Fire(hit))
                        {
                            if (hit.Count > 0 && hit[0] && hit[0].healthComponent)
                            ForgeOnGroundHit();
                            SetNextStateToSmash(hit[0].healthComponent);
                            return;
                        }
                    }
                }
                if (fixedAge >= AmyStaticValues.secondaryHammerAirFallMaxFallDuration)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            base.characterMotor.onHitGroundAuthority -= OnGroundHit;
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(JunkContent.Buffs.IgnoreFallDamage);
            }
        }

        public void OnGroundHit(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            SetNextStateToSmash(null);
        }

        public virtual void PrepareAttackStats()
        {

        }

        protected virtual void BeforeAttackFire()
        {
            float charge = CalculateCharge();
            overlapAttack.damage = damageStat * Mathf.Lerp(AmyStaticValues.secondaryHammerChargeMinimumDamageCoefficient, AmyStaticValues.secondaryHammerChargeMaximumDamageCoefficient, charge);
            overlapAttack.procCoefficient = charge == 1 ? AmyStaticValues.secondaryHammerChargeMaximumProcCoefficient : AmyStaticValues.secondaryHammerChargeMinimumProcCoefficient;
        }

        protected void ForgeOnGroundHit()
        {
            base.characterMotor.jumpCount = 1;
            if (base.gameObject.TryGetComponent<BoostLogic>(out BoostLogic boost))
            {
                CharacterMotor.HitGroundInfo hitGround = new CharacterMotor.HitGroundInfo();
                hitGround.isValidForEffect = false;
                hitGround.velocity = base.characterMotor.velocity;
                hitGround.position = base.characterBody.footPosition;
                boost.ResetAirBoost(ref hitGround);
            }
        }

        public virtual float CalculateCharge()
        {
            return Mathf.Clamp01((startingHeight - transform.position.y) / AmyStaticValues.secondaryHammerAirFallDistanceForMaxCharge);
        }

        public virtual void SetNextStateToSmash(HealthComponent targetToIgnore)
        {
            HammerSmashAerial state = (HammerSmashAerial)EntityStateCatalog.InstantiateState(typeof(HammerSmashAerial));
            if (state != null)
            {
                state.charge = CalculateCharge();
                state.targetToIgnore = targetToIgnore;
            }
            this.outer.SetNextState(state);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
