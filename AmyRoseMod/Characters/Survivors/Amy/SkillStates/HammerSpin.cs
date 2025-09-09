using Amy.Modules.BaseStates;
using Amy.Survivors.Amy.Components;
using AmyRoseMod.Characters.Survivors.Amy.Content;
using AmyRoseMod.Characters.Survivors.Amy.SkillStates;
using EntityStates;
using HedgehogUtils.Boost;
using R2API;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.Networking;
using static EntityStates.BaseState;

namespace Amy.Survivors.Amy.SkillStates
{
    public class HammerSpin : GenericCharacterMain, ISkillState
    {
        protected string hitboxGroupName = "SwordGroup";

        protected DamageTypeCombo damageType = DamageTypeCombo.GenericUtility;
        protected float damageCoefficient = 3.5f;
        protected float procCoefficient = 1f;
        protected float pushForce = 300f;
        protected Vector3 bonusForce = Vector3.zero;
        protected float baseAttacksPerSecond;
        protected float timeUntilNextAttack;

        protected float hitStopDuration;
        protected float hitHopVelocity;

        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string muzzleString = "SwingCenter";
        protected string playbackRateParam = "Slash.playbackRate";
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab;
        protected NetworkSoundEventIndex impactSound = NetworkSoundEventIndex.Invalid;
        protected OverlapAttack attack;

        protected bool hasFired;
        protected bool hasHopped;
        public float duration;
        public float minDuration;
        private float hitPauseTimer;
        protected bool inHitPause;
        protected float stopwatch;
        protected Animator animator;
        private HitStopCachedState hitStopCachedState;
        private Vector3 storedVelocity;

        protected LeanIntoVelocityModelTransform lean;
        protected BoostLogic boostLogic;

        public GenericSkill activatorSkillSlot { get; set; }
        public SkillDef hammerSpinSkillDef;

        protected virtual float boostMeterDrain
        {
            get { return 0.63f; }
        }

        protected float buffStackTimer;
        protected float buffsPerSecond;

        private float previousAirControl;

        public override void OnEnter()
        {
            PrepareBaseStats();
            buffStackTimer = buffsPerSecond;
            animator = GetModelAnimator();
            swingSoundString = "HenrySwordSwing";
            hitSoundString = "";
            muzzleString = "SwingLeft";
            playbackRateParam = "Slash.playbackRate";
            swingEffectPrefab = AmyAssets.swordSwingEffect;
            hitEffectPrefab = AmyAssets.swordHitImpactEffect;

            impactSound = AmyAssets.swordHitSoundEvent.index;

            base.OnEnter();
            lean = base.GetComponent<LeanIntoVelocityModelTransform>();
            if (lean)
            {
                lean.Activate();
            }

            boostLogic = base.GetComponent<BoostLogic>();

            previousAirControl = characterMotor.airControl;
            characterMotor.airControl = 1f;

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(AmyBuffs.boostBuff);
            }
            if (base.isAuthority)
            {
                if (Boost.ApplySkillOverride(this, activatorSkillSlot, base.skillLocator, out SkillDef skillDef))
                {
                    hammerSpinSkillDef = skillDef;
                }
            }
        }

        protected virtual void PrepareBaseStats()
        {
            minDuration = 0.7f;
            timeUntilNextAttack = 0.5f;
            hitStopDuration = 0.06f;
            hitHopVelocity = 4f;
            buffsPerSecond = AmyStaticValues.boostHammerSpinBuffPerSecond;
        }

        protected virtual void PrepareAttackStats()
        {
            hitboxGroupName = "Spin";

            damageType = DamageTypeCombo.GenericUtility;
            damageType.AddModdedDamageType(HedgehogUtils.Launch.DamageTypes.launchOnKill);
            damageType.AddModdedDamageType(AmyDamageTypes.angleUpKnockbackIfGrounded);
            damageCoefficient = AmyStaticValues.boostHammerSpinDamageCoefficient;
            procCoefficient = AmyStaticValues.boostHammerSpinProcCoefficient;
            pushForce = AmyStaticValues.boostHammerSpinLaunchForce;
            baseAttacksPerSecond = AmyStaticValues.boostHammerSpinAttacksPerSecond;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            hitPauseTimer -= Time.deltaTime;

            if (hitPauseTimer <= 0f && inHitPause)
            {
                RemoveHitstop();
            }

            if (!inHitPause)
            {
                stopwatch += Time.deltaTime;
                if (timeUntilNextAttack > 0)
                {
                    timeUntilNextAttack -= Time.fixedDeltaTime;
                    if (timeUntilNextAttack <= 0)
                    {
                        hasFired = false;
                        hasHopped = false;
                    }
                }

                if (NetworkServer.active && buffStackTimer > 0 && base.characterBody.GetBuffCount(AmyBuffs.hammerSpinSpeedBuff) < AmyStaticValues.boostHammerSpinBuffMaxStacks)
                {
                    buffStackTimer -= Time.fixedDeltaTime;
                    if (buffStackTimer <= 0)
                    {
                        base.characterBody.AddBuff(AmyBuffs.hammerSpinSpeedBuff);
                        buffStackTimer = buffsPerSecond;
                    }
                }
            }
            else
            {
                if (characterMotor) characterMotor.velocity = Vector3.zero;
                if (animator) animator.SetFloat(playbackRateParam, 0f);
            }

            if (base.characterMotor)
            {
                base.characterMotor.velocity.y = Mathf.Max(characterMotor.velocity.y, -3);
            }

            DrainBoostMeter();

            //to guarantee attack comes out if at high attack speed the stopwatch skips past the firing duration between frames
            if (!hasFired)
            {
                hasFired = true;
                EnterAttack();
                FireAttack();
            }

            if (stopwatch >= minDuration && isAuthority && ((!inputBank || !boostLogic || !inputBank.skill1.down)))
            {
                outer.SetNextStateToMain();
                return;
            }
            if (boostLogic.boostMeter <= 0 || !boostLogic.boostAvailable)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        protected virtual void DrainBoostMeter()
        {
            if (boostMeterDrain > 0)
            {
                if (NetworkServer.active)
                {
                    boostLogic.RemoveBoost(boostMeterDrain);
                }
                boostLogic.boostMeterDrain = boostMeterDrain;
                boostLogic.boostDraining = true;
            }
        }
        protected virtual void OnHitEnemyAuthority()
        {
            Util.PlaySound(hitSoundString, gameObject);

            if (!hasHopped)
            {
                if (characterMotor && !characterMotor.isGrounded && hitHopVelocity > 0f)
                {
                    SmallHop(characterMotor, hitHopVelocity);
                }

                hasHopped = true;
            }

            ApplyHitstop();
        }

        public override bool CanExecuteSkill(GenericSkill skillSlot)
        {
            return false;
        }

        protected void ApplyHitstop()
        {
            if (!inHitPause && hitStopDuration > 0f)
            {
                storedVelocity = characterMotor.velocity;
                hitStopCachedState = CreateHitStopCachedState(characterMotor, animator, playbackRateParam);
                hitPauseTimer = hitStopDuration / base.characterBody.attackSpeed;
                inHitPause = true;
                lean.frozen = true;
            }
        }

        private void FireAttack()
        {
            if (isAuthority)
            {
                if (attack.Fire())
                {
                    OnHitEnemyAuthority();
                }
            }
        }

        protected virtual void EnterAttack()
        {
            Util.PlayAttackSpeedSound(swingSoundString, gameObject, base.characterBody.attackSpeed);

            if (isAuthority)
            {
                PrepareAttackStats();
                CreateOverlap();
                timeUntilNextAttack = 1 / (baseAttacksPerSecond * base.characterBody.attackSpeed);
            }
        }

        private void RemoveHitstop()
        {
            ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
            inHitPause = false;
            characterMotor.velocity = storedVelocity;
            lean.frozen = false;
        }

        protected void CreateOverlap()
        {
            attack = new OverlapAttack();
            attack.damageType = damageType;
            attack.attacker = gameObject;
            attack.inflictor = gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = damageCoefficient * base.characterBody.damage;
            attack.procCoefficient = procCoefficient;
            attack.hitEffectPrefab = hitEffectPrefab;
            attack.forceVector = bonusForce;
            attack.pushAwayForce = pushForce;
            attack.hitBoxGroup = FindHitBoxGroup(hitboxGroupName);
            attack.isCrit = RollCrit();
            attack.impactSound = impactSound;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        public override void OnExit()
        {
            if (lean) { lean.Deactivate(); }
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(AmyBuffs.boostBuff);
                base.characterBody.SetBuffCount(AmyBuffs.hammerSpinSpeedBuff.buffIndex, 0);
            }
            if (base.isAuthority)
            {
                Boost.RemoveSkillOverride(this, activatorSkillSlot, hammerSpinSkillDef, base.skillLocator);
            }
            base.characterMotor.airControl = previousAirControl;
            base.OnExit();
        }
    }
}