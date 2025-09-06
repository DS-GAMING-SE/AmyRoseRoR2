using Amy.Modules.BaseStates;
using AmyRoseMod.Characters.Survivors.Amy.Content;
using AmyRoseMod.Characters.Survivors.Amy.SkillStates;
using EntityStates;
using R2API;
using RoR2;
using RoR2.Audio;
using System;
using UnityEngine;
using UnityEngine.Networking;
using static EntityStates.BaseState;

namespace Amy.Survivors.Amy.SkillStates
{
    public class HammerSpin : BaseSkillState
    {
        protected string hitboxGroupName = "SwordGroup";

        protected DamageTypeCombo damageType = DamageTypeCombo.GenericUtility;
        protected float damageCoefficient = 3.5f;
        protected float procCoefficient = 1f;
        protected float pushForce = 300f;
        protected Vector3 bonusForce = Vector3.zero;

        protected float hitStopDuration = 0.1f;
        protected float hitHopVelocity = 6f;

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
        private float hitPauseTimer;
        protected bool inHitPause;
        protected float stopwatch;
        protected Animator animator;
        private HitStopCachedState hitStopCachedState;
        private Vector3 storedVelocity;

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

        }

        protected virtual void PrepareAttackStats()
        {
            hitboxGroupName = "LargeSwing";

            damageType = DamageTypeCombo.GenericUtility;
            damageType.AddModdedDamageType(HedgehogUtils.Launch.DamageTypes.launchOnKill);
            damageCoefficient = AmyStaticValues.boostHammerSpinDamageCoefficient;
            procCoefficient = AmyStaticValues.boostHammerSpinProcCoefficient;
            pushForce = AmyStaticValues.boostHammerSpinLaunchForce;
        }

        protected virtual void PushForceToTargetedLaunch()
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

            if (base.characterMotor)
            {
                base.characterMotor.velocity.y = Mathf.Max(characterMotor.velocity.y, -3);
            }

            //to guarantee attack comes out if at high attack speed the stopwatch skips past the firing duration between frames
            if (!hasFired)
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

            if (isAuthority)
            {
                PrepareAttackStats();
                PushForceToTargetedLaunch();
                CreateOverlap();
            }
        }

        private void RemoveHitstop()
        {
            ConsumeHitStopCachedState(hitStopCachedState, characterMotor, animator);
            inHitPause = false;
            characterMotor.velocity = storedVelocity;
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
        }

        protected virtual void DecideLaunchDirection(Vector3 aim)
        {
            bonusForce = aim * pushForce;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}