using AmyRoseMod.Modules.BaseStates;
using AmyRoseMod.Characters.Survivors.Amy.Components;
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

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
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

        protected AmyHammerSpinController hammerSpinController;
        protected BoostLogic boostLogic;

        public GenericSkill activatorSkillSlot { get; set; }
        public SkillDef hammerSpinSkillDef;

        protected virtual BuffDef boostBuff { get { return AmyBuffs.boostBuff; } }
        protected virtual BuffDef hammerSpinBuff { get { return AmyBuffs.hammerSpinSpeedBuff; } }

        protected virtual float boostMeterDrain
        {
            get { return 36f; }
        }

        protected float buffStackNeededSpeedPercent;
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
            hammerSpinController = base.GetComponent<AmyHammerSpinController>();
            if (!hammerSpinController) { this.outer.SetNextStateToMain(); return; }
            hammerSpinController.ActivateSpin();

            boostLogic = base.GetComponent<BoostLogic>();
            boostLogic.boostBeingUsed = true;

            previousAirControl = characterMotor.airControl;
            characterMotor.airControl = 1f;

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(boostBuff);
                base.characterBody.AddBuff(hammerSpinBuff);
            }
            if (base.isAuthority)
            {
                base.characterBody.skillLocator.primary.onSkillChanged += OnSkillChanged;

                hammerSpinController.ApplySkillOverride(activatorSkillSlot, out SkillDef skillDef);
                hammerSpinSkillDef = skillDef;

                attack = new OverlapAttack();
                attack.attacker = gameObject;
                attack.inflictor = gameObject;
                attack.teamIndex = GetTeam();
            }
        }

        protected virtual void PrepareBaseStats()
        {
            minDuration = 0.7f;
            timeUntilNextAttack = 0.5f;
            hitStopDuration = 0.06f;
            hitHopVelocity = 4f;
            buffsPerSecond = AmyStaticValues.boostHammerSpinBuffPerSecond;
            buffStackNeededSpeedPercent = 0.7f;
        }

        protected virtual void PrepareAttackStats()
        {
            float speedLerp = ((float)(base.characterBody.GetBuffCount(hammerSpinBuff)) - 1f) / (((float)AmyStaticValues.boostHammerSpinBuffMaxStacks - 1));
            hitboxGroupName = "Spin";

            damageType = DamageTypeCombo.GenericUtility;
            damageType.AddModdedDamageType(HedgehogUtils.Launch.DamageTypes.launchOnKill);
            damageType.AddModdedDamageType(AmyDamageTypes.angleUpKnockbackIfGrounded);
            damageCoefficient = Mathf.Lerp(AmyStaticValues.boostHammerSpinDamageCoefficient, AmyStaticValues.boostHammerSpinFastDamageCoefficient, speedLerp);
            procCoefficient = AmyStaticValues.boostHammerSpinProcCoefficient;
            pushForce = AmyStaticValues.boostHammerSpinLaunchForce;
            baseAttacksPerSecond = Mathf.Lerp(AmyStaticValues.boostHammerSpinAttacksPerSecond, AmyStaticValues.boostHammerSpinBuffMaxAttacksPerSecond, speedLerp);
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

                if (NetworkServer.active && buffStackTimer > 0)
                {
                    UpdateIsHighSpeed();

                    buffStackTimer -= Time.fixedDeltaTime;
                    if (buffStackTimer <= 0)
                    {
                        buffStackTimer = buffsPerSecond;
                        UpdateBuffs();
                    }
                }
            }
            else
            {
                if (characterMotor) characterMotor.velocity = Vector3.zero;
                if (animator) animator.SetFloat(playbackRateParam, 0f);
            }

            if (base.characterMotor && !base.characterMotor.isFlying)
            {
                base.characterMotor.velocity.y = Mathf.Max(characterMotor.velocity.y, -3);
            }

            DrainBoostMeter();

            if (!hasFired)
            {
                hasFired = true;
                EnterAttack();
            }
            PrepareAttackStats();
            if (base.isAuthority)
            {
                UpdateOverlapStats();
            }
            FireAttack();

            if (stopwatch >= minDuration && isAuthority && ((!inputBank || !boostLogic || !inputBank.skill1.down)))
            {
                SetNextStateToEndLag();
                return;
            }
            if (boostLogic.boostMeter <= 0 || !boostLogic.boostAvailable && isAuthority)
            {
                SetNextStateToDizzy();
                return;
            }
        }

        protected virtual void DrainBoostMeter()
        {
            if (boostMeterDrain > 0)
            {
                if (NetworkServer.active)
                {
                    boostLogic.RemoveBoost(boostMeterDrain * Time.fixedDeltaTime);
                }
                boostLogic.boostMeterDrain = boostMeterDrain;
                boostLogic.boostDraining = true;
            }
        }

        protected virtual void UpdateIsHighSpeed()
        {
            Vector3 vel = hammerSpinController.estimatedVelocity;
            if (!base.characterMotor.isFlying) { vel.y = 0; }
            hammerSpinController.highSpeed = vel.magnitude >= base.characterBody.moveSpeed * buffStackNeededSpeedPercent;
        }

        protected virtual void UpdateBuffs()
        {
            if (hammerSpinController.highSpeed)
            {
                if (base.characterBody.GetBuffCount(hammerSpinBuff) < AmyStaticValues.boostHammerSpinBuffMaxStacks)
                {
                    base.characterBody.AddBuff(hammerSpinBuff);
                }
            }
            else
            {
                if (base.characterBody.GetBuffCount(hammerSpinBuff) > 1)
                {
                    base.characterBody.SetBuffCount(hammerSpinBuff.buffIndex, Math.Max(1, base.characterBody.GetBuffCount(hammerSpinBuff) - 2));
                }
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
                hammerSpinController.leanFrozen = true;
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

            PrepareAttackStats();
            timeUntilNextAttack = 1 / (baseAttacksPerSecond * base.characterBody.attackSpeed);
            if (isAuthority)
            {
                attack.retriggerTimeout = timeUntilNextAttack;
            }
        }

        private void RemoveHitstop()
        {
            ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
            inHitPause = false;
            characterMotor.velocity = storedVelocity;
            hammerSpinController.leanFrozen = false;
        }

        protected void UpdateOverlapStats()
        {
            attack.damageType = damageType;
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

        protected virtual void SetNextStateToEndLag()
        {
            EntityStateMachine weapon = EntityStateMachine.FindByCustomName(base.gameObject, "Weapon");
            if (weapon)
            {
                weapon.SetNextState(EntityStateCatalog.InstantiateState(typeof(HammerSpinEndLag)));
            }
            this.outer.SetNextStateToMain();
        }

        protected virtual void SetNextStateToDizzy()
        {
            this.outer.SetNextState(EntityStateCatalog.InstantiateState(typeof(Dizzy)));
        }

        public override void OnExit()
        {
            if (base.isAuthority)
            {
                base.characterBody.skillLocator.primary.onSkillChanged -= OnSkillChanged;
            }
            boostLogic.boostDraining = false;
            boostLogic.boostBeingUsed = false;
            if (hammerSpinController) 
            { 
                hammerSpinController.DeactivateSpin();
            }
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(boostBuff);
                base.characterBody.SetBuffCount(hammerSpinBuff.buffIndex, 0);
            }
            base.characterMotor.airControl = previousAirControl;
            base.OnExit();
        }

        public virtual void OnSkillChanged(GenericSkill skill)
        {
            SetNextStateToEndLag();
        }
    }
}