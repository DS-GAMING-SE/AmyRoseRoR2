using AmyRoseMod.Characters.Survivors.Amy;
using EntityStates;
using R2API;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AmyRoseMod.Modules.BaseStates
{
    public abstract class BaseMeleeAttack : BaseSkillState, SteppedSkillDef.IStepSetter
    {
        public int swingIndex;

        protected string hitboxGroupName = "SwordGroup";

        protected DamageTypeCombo damageType = DamageType.Generic;
        protected float damageCoefficient = 3.5f;
        protected float procCoefficient = 1f;
        protected float pushForce = 300f;
        protected Vector3 bonusForce = Vector3.zero;
        protected float baseDuration = 1f;
        protected int maximumOverlapTargets = -1;

        protected float attackStartPercentTime = 0.2f;
        protected float attackEndPercentTime = 0.4f;

        protected float earlyExitPercentTime = 0.4f;

        protected float hitStopDuration = 0.012f;
        protected float attackRecoil = 0.75f;
        protected float hitHopVelocity = 4f;

        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string muzzleString = "SwingCenter";
        protected string playbackRateParam = "Slash.playbackRate";
        protected GameObject swingEffectPrefab;

        protected bool poolSwingEffect = true;
        private GameObject swingEffectInstance;
        private EffectManagerHelper _emh_swingEffectInstance;

        protected GameObject hitEffectPrefab;
        protected NetworkSoundEventIndex impactSound = NetworkSoundEventIndex.Invalid;
        protected OverlapAttack attack;

        public float duration;
        private bool hasFired;
        private float hitPauseTimer;
        protected bool inHitPause;
        private bool hasHopped;
        protected float stopwatch;
        protected Animator animator;
        private HitStopCachedState hitStopCachedState;
        private Vector3 storedVelocity;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            animator = GetModelAnimator();
            StartAimMode(0.5f + duration, false);

            PlayAttackAnimation();
        }

        protected virtual void ModifyOverlapAttack(OverlapAttack attack)
        {

        }

        protected virtual void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash" + (1 + swingIndex), playbackRateParam, duration, 0.05f);
        }

        public override void OnExit()
        {
            if (inHitPause)
            {
                RemoveHitstop();
            }
            if (poolSwingEffect)
            {
                ReturnSwingEffect();
            }
            base.OnExit();
        }

        protected virtual void PlaySwingEffect()
        {
            EffectManager.SimpleMuzzleFlash(swingEffectPrefab, gameObject, muzzleString, false);
        }
        protected void PlayPooledSwingEffect()
        {
            PlayPooledSwingEffect(swingEffectPrefab, swingEffectInstance, _emh_swingEffectInstance, duration * (1 - attackStartPercentTime - (1 - attackEndPercentTime)));
            /*if (this.swingEffectPrefab)
            {
                Transform transform = base.FindModelChild(this.muzzleString);
                if (transform)
                {
                    if (!EffectManager.ShouldUsePooledEffect(this.swingEffectPrefab))
                    {
                        this.swingEffectInstance = GameObject.Instantiate(this.swingEffectPrefab, transform);
                    }
                    else
                    {
                        this._emh_swingEffectInstance = EffectManager.GetAndActivatePooledEffect(this.swingEffectPrefab, transform, true);
                        this.swingEffectInstance = this._emh_swingEffectInstance.gameObject;
                    }
                    ScaleParticleSystemDuration component = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                    if (component)
                    {
                        component.newDuration = duration * (1 - attackStartPercentTime - (1 - attackEndPercentTime));
                    }
                    swingEffectInstance.transform.localRotation = Quaternion.Euler(90, 0, 0);
                }
            }*/
        }

        protected virtual void PlayPooledSwingEffect(GameObject prefab, GameObject instance, EffectManagerHelper emh, float duration)
        {
            if (prefab)
            {
                Transform transform = base.FindModelChild(this.muzzleString);
                if (transform)
                {
                    if (!EffectManager.ShouldUsePooledEffect(prefab))
                    {
                        instance = GameObject.Instantiate(prefab, transform);
                    }
                    else
                    {
                        emh = EffectManager.GetAndActivatePooledEffect(prefab, transform, true);
                        instance = emh.gameObject;
                    }
                    ScaleParticleSystemDuration component = instance.GetComponent<ScaleParticleSystemDuration>();
                    if (component)
                    {
                        component.newDuration = duration;
                    }
                    instance.transform.localRotation = Quaternion.Euler(90, 0, 0);
                }
            }
        }
        protected void ReturnSwingEffect()
        {
            ReturnSwingEffect(swingEffectInstance, _emh_swingEffectInstance);
            /*if (this._emh_swingEffectInstance != null && this._emh_swingEffectInstance.OwningPool != null)
            {
                this._emh_swingEffectInstance.OwningPool.ReturnObject(this._emh_swingEffectInstance);
            }
            else if (this.swingEffectInstance)
            {
                GameObject.Destroy(this.swingEffectInstance);
            }
            this.swingEffectInstance = null;
            this._emh_swingEffectInstance = null;*/
        }

        protected virtual void ReturnSwingEffect(GameObject instance, EffectManagerHelper emh)
        {
            if (emh != null && emh.OwningPool != null)
            {
                emh.OwningPool.ReturnObject(emh);
            }
            else if (instance)
            {
                GameObject.Destroy(instance);
            }
            instance = null;
            emh = null;
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

        protected void ApplyHitstop()
        {
            if (!inHitPause && hitStopDuration > 0f)
            {
                storedVelocity = characterMotor.velocity;
                hitStopCachedState = CreateHitStopCachedState(characterMotor, animator, playbackRateParam);
                hitPauseTimer = hitStopDuration / attackSpeedStat;
                inHitPause = true;
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
            hasFired = true;
            Util.PlayAttackSpeedSound(swingSoundString, gameObject, attackSpeedStat);
            if (poolSwingEffect)
            {
                PlayPooledSwingEffect();
            }
            else
            {
                PlaySwingEffect();
            }

            if (isAuthority)
            {
                AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);
                PrepareAttackStats();
                PushForceToTargetedLaunch();
                CreateOverlap();
            }
        }

        protected virtual void PrepareAttackStats()
        {

        }

        protected virtual void PushForceToTargetedLaunch()
        {

        }

        protected void CreateOverlap()
        {
            attack = new OverlapAttack();
            attack.damageType = damageType;
            attack.attacker = gameObject;
            attack.inflictor = gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = damageCoefficient * damageStat;
            attack.procCoefficient = procCoefficient;
            attack.hitEffectPrefab = hitEffectPrefab;
            attack.forceVector = bonusForce;
            attack.pushAwayForce = pushForce;
            attack.hitBoxGroup = FindHitBoxGroup(hitboxGroupName);
            attack.isCrit = RollCrit();
            attack.impactSound = impactSound;

            ModifyOverlapAttack(attack);
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
            }
            else
            {
                if (characterMotor) characterMotor.velocity = Vector3.zero;
                if (animator) animator.SetFloat(playbackRateParam, 0f);
            }

            bool fireStarted = stopwatch >= duration * attackStartPercentTime;
            bool fireEnded = stopwatch >= duration * attackEndPercentTime;

            //to guarantee attack comes out if at high attack speed the stopwatch skips past the firing duration between frames
            if (fireStarted && !fireEnded || fireStarted && fireEnded && !hasFired)
            {
                if (!hasFired)
                {
                    EnterAttack();
                }
                FireAttack();
            }

            if (stopwatch >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        private void RemoveHitstop()
        {
            ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
            inHitPause = false;
            characterMotor.velocity = storedVelocity;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (stopwatch >= duration * earlyExitPercentTime)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.Skill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(swingIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            swingIndex = reader.ReadInt32();
        }

        public void SetStep(int i)
        {
            swingIndex = i;
        }
    }
}