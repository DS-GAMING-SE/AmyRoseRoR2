using AmyRoseMod.Modules.BaseStates;
using AmyRoseMod.Characters.Survivors.Amy.Content;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates.SuperFormUpgrades
{
    public class SuperPrimaryHammer : PrimaryHammer
    {
        protected override GameObject hitEffectPrefab { get { return AmyAssets.superHammerHitImpactEffect; } }

        protected GameObject swingSuperEffectPrefab;
        private GameObject swingSuperEffectInstance;
        private EffectManagerHelper _emh_swingSuperEffectInstance;

        protected OverlapAttack superAttack;

        protected float superSwingSoundStartPercentTime = 0.1f;
        protected float superAttackStartPercentTime = 0.25f;
        protected float superAttackEndPercentTime = 0.5f;

        private bool hasFired;
        private bool soundFired;

        public override void OnEnter()
        {
            base.OnEnter();
            swingSuperEffectPrefab = AmyAssets.superHammerSwingEffect;
            PrepareSuperOverlapAttack();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (stopwatch >= superSwingSoundStartPercentTime && !soundFired)
            {
                soundFired = true;
                Util.PlaySound("Play_amyrose_swing_super", gameObject);
            }
            if (stopwatch >= superAttackStartPercentTime && (stopwatch <= superAttackEndPercentTime || !hasFired))
            {
                if (!hasFired)
                {
                    hasFired = true;
                    PlayPooledSwingEffect(swingSuperEffectPrefab, ref swingSuperEffectInstance, ref _emh_swingSuperEffectInstance, duration * (1 - superAttackStartPercentTime - (1 - superAttackEndPercentTime)));
                }
                if (base.isAuthority)
                {
                    superAttack.Fire();
                }
            }
        }

        public override void OnExit()
        {
            ReturnSwingEffect(ref swingSuperEffectInstance, ref _emh_swingSuperEffectInstance);
            base.OnExit();
        }

        protected virtual void PrepareSuperOverlapAttack()
        {
            superAttack = new OverlapAttack();
            superAttack.damage = AmyStaticValues.superPrimaryHammerAfterimageDamageCoefficient * damageStat;
            superAttack.procCoefficient = AmyStaticValues.superPrimaryHammerAfterimageProcCoefficient;
            superAttack.attacker = base.gameObject;
            superAttack.inflictor = base.gameObject;
            superAttack.isCrit = RollCrit();
            superAttack.damageType = DamageTypeCombo.GenericPrimary;
            superAttack.teamIndex = GetTeam();
            superAttack.hitBoxGroup = FindHitBoxGroup("SuperSwing");
        }
        protected override void PrepareAttackStats()
        {
            base.PrepareAttackStats();
            pushForce = AmyStaticValues.superPrimaryHammerLaunchForce;
        }

        protected override void PrepareAnimationStats()
        {
            base.PrepareAnimationStats();
            hitHopVelocity = 0f;
        }
    }
}