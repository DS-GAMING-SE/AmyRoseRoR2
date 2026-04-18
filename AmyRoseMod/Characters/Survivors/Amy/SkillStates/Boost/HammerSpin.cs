using AmyRoseMod.Characters.Survivors.Amy.Components;
using AmyRoseMod.Characters.Survivors.Amy.Content;
using AmyRoseMod.Characters.Survivors.Amy.SkillStates;
using AmyRoseMod.Modules.BaseStates;
using EntityStates;
using HedgehogUtils.Boost;
using HedgehogUtils.Voicelines;
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

        protected string playbackRateParam = "Slash.playbackRate";
        protected virtual GameObject spinEffectPrefab { get { return AmyAssets.hammerSpinSpinningEffect; } }
        protected virtual GameObject hitEffectPrefab { get { return AmyAssets.hammerHitImpactEffect; } }
        protected NetworkSoundEventIndex impactSound = AmyAssets.hammerHitSoundEvent.index;
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

        protected LoopSoundManager.SoundLoopPtr hammerSpinLoopSound;
        protected GameObject spinEffectInstance;
        protected EffectManagerHelper emh;

        private bool exitToEndLag;

        public override void OnEnter()
        {
            PrepareBaseStats();
            buffStackTimer = buffsPerSecond;
            animator = GetModelAnimator();
            playbackRateParam = "Slash.playbackRate";
            PlayAnimation("FullBody, Override", "HammerSpin");

            impactSound = AmyAssets.hammerHitSoundEvent.index;

            base.OnEnter();

            hammerSpinLoopSound = LoopSoundManager.PlaySoundLoopLocal(base.gameObject, AmyAssets.hammerSpinLoopSoundDef);

            hammerSpinController = base.GetComponent<AmyHammerSpinController>();
            if (!hammerSpinController) { this.outer.SetNextStateToMain(); return; }
            hammerSpinController.ActivateSpin();

            boostLogic = base.GetComponent<BoostLogic>();
            boostLogic.boostBeingUsed = true;

            previousAirControl = characterMotor.airControl;
            characterMotor.airControl = 1f;

            animator.SetBool("ignoreAim", true);
            aimAnimator.enabled = false;

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(boostBuff);
                AddSpinBuff(hammerSpinBuff.buffIndex, 1);
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
            emh = EffectManager.GetAndActivatePooledEffect(spinEffectPrefab, FindModelChild("MainHurtbox"), true);
            spinEffectInstance = emh.gameObject;

            VoicelineComponent.TryPlayVoiceline(gameObject, "Play_amyrose_voiceline_charge", VoicelinePriority.Skill);
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
            float speedLerp = Mathf.Max(0f, (base.characterBody.GetBuffCount(hammerSpinBuff.buffIndex) - AmyStaticValues.boostHammerSpinBuffPercentPerEffectiveStack)) / 100f;
            hitboxGroupName = "Spin";

            damageType = DamageTypeCombo.GenericUtility;
            damageType.AddModdedDamageType(HedgehogUtils.Launch.DamageTypes.launchOnKill);
            damageType.AddModdedDamageType(AmyDamageTypes.angleUpKnockbackIfGrounded);
            damageCoefficient = Mathf.Lerp(AmyStaticValues.boostHammerSpinDamageCoefficient, AmyStaticValues.boostHammerSpinFastDamageCoefficient, speedLerp);
            procCoefficient = Mathf.Lerp(AmyStaticValues.boostHammerSpinMinProcCoefficient, AmyStaticValues.boostHammerSpinMaxProcCoefficient, speedLerp);
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

                if (buffStackTimer > 0)
                {
                    if (base.isAuthority)
                    {
                        UpdateIsHighSpeed();
                    }
                    buffStackTimer -= Time.fixedDeltaTime;
                    if (buffStackTimer <= 0)
                    {
                        buffStackTimer = buffsPerSecond;
                        if (NetworkServer.active)
                        {
                            UpdateBuffs();
                        }
                    }
                }
            }
            else
            {
                if (characterMotor) characterMotor.velocity = Vector3.zero;
                if (animator) animator.SetFloat(playbackRateParam, 0.1f);
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
                FireAttack();
            }

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
            hammerSpinController.SetHighSpeed(vel.magnitude >= base.characterBody.moveSpeed * buffStackNeededSpeedPercent);
        }

        protected virtual void UpdateBuffs()
        {
            if (hammerSpinController.highSpeed)
            {
                if (base.characterBody.GetBuffCount(hammerSpinBuff) < 100)
                {
                    AddSpinBuff(hammerSpinBuff.buffIndex, 1);
                }
            }
            else
            {
                if (GetSpinBuffCount(hammerSpinBuff.buffIndex) > 1)
                {
                    RemoveSpinBuff(hammerSpinBuff.buffIndex, 2);
                }
            }
        }
        protected void AddSpinBuff(BuffIndex buffIndex, int effectiveStack)
        {
            base.characterBody.SetBuffCount(buffIndex, Mathf.Min(base.characterBody.GetBuffCount(hammerSpinBuff) + effectiveStack * AmyStaticValues.boostHammerSpinBuffPercentPerEffectiveStack, 100));
        }
        protected void RemoveSpinBuff(BuffIndex buffIndex, int effectiveStack)
        {
            base.characterBody.SetBuffCount(buffIndex, Mathf.Max(AmyStaticValues.boostHammerSpinBuffPercentPerEffectiveStack, base.characterBody.GetBuffCount(hammerSpinBuff) - effectiveStack * AmyStaticValues.boostHammerSpinBuffPercentPerEffectiveStack));
        }
        protected int GetSpinBuffCount(BuffIndex buffIndex)
        {
            return base.characterBody.GetBuffCount(buffIndex) / AmyStaticValues.boostHammerSpinBuffPercentPerEffectiveStack;
        }
        protected virtual void OnHitEnemyAuthority()
        {
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
            //Util.PlayAttackSpeedSound(swingSoundString, gameObject, base.characterBody.attackSpeed);

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
            this.outer.SetNextStateToMain();
            exitToEndLag = true;
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
            PlayAnimation("FullBody, Override", "BufferEmpty");
            animator.SetBool("ignoreAim", false);
            if (exitToEndLag)
            {
                EntityStateMachine weapon = EntityStateMachine.FindByCustomName(base.gameObject, "Weapon");
                if (weapon)
                {
                    weapon.SetNextState(EntityStateCatalog.InstantiateState(typeof(HammerSpinEndLag)));
                }
            }
            boostLogic.boostDraining = false;
            boostLogic.boostBeingUsed = false;
            hammerSpinController.DeactivateSpin();
            hammerSpinController.leanFrozen = false;
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(boostBuff);
                base.characterBody.SetBuffCount(hammerSpinBuff.buffIndex, 0);
            }
            LoopSoundManager.StopSoundLoopLocal(hammerSpinLoopSound);
            base.characterMotor.airControl = previousAirControl;
            if (spinEffectInstance)
            {
                if (spinEffectInstance.TryGetComponent(out DisableParticleEmissionAndDestroyOnTimer disable))
                {
                    disable.DisableParticlesStartTimer();
                }
                if (spinEffectInstance.transform.GetChild(0).TryGetComponent(out AnimateShaderAlpha alpha))
                {
                    alpha.enabled = true;
                    alpha.Restart();
                }
            }
            else { Destroy(spinEffectInstance); }
            base.OnExit();
        }

        public virtual void OnSkillChanged(GenericSkill skill)
        {
            SetNextStateToEndLag();
        }
    }
}