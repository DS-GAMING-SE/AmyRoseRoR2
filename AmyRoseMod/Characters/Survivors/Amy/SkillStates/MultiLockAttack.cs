using  AmyRoseMod.Characters.Survivors.Amy.Components;
using AmyRoseMod.Characters.Survivors.Amy.Content;
using EntityStates;
using HedgehogUtils;
using HedgehogUtils.Boost;
using RoR2;
using RoR2.Audio;
using RoR2.Orbs;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates
{
    public class MultiLockAttack : BaseState
    {
        public List<HurtBox> targets;

        public float maxDurationFailsafe = 5f;

        public float orbBounceRange;

        protected CharacterModel characterModel;
        
        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority)
            {
                EntityStateMachine weaponState = EntityStateMachine.FindByCustomName(base.gameObject, "Weapon");
                if (weaponState) { weaponState.SetNextStateToMain(); }
                if (base.skillLocator)
                {
                    skillLocator.special.DeductStock(1);
                }
            }
            if (NetworkServer.active)
            {
                FireOrb();
                base.characterBody.AddBuff(RoR2Content.Buffs.Intangible);
            }
            if (base.modelLocator && base.modelLocator.modelTransform)
            {
                characterModel = base.modelLocator.modelTransform.GetComponent<CharacterModel>();
                if (characterModel)
                {
                    characterModel.invisibilityCount++;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.characterMotor)
            {
                base.characterMotor.velocity = Vector3.zero;
            }
            if (base.isAuthority && fixedAge >= maxDurationFailsafe)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public virtual void FireOrb()
        {
            AmyOrbs.MultiLockOrb orb = AmyOrbs.CreateMultiLockOrb<AmyOrbs.MultiLockOrb>(AmyStaticValues.specialMultiLockDamageCoefficient * damageStat, base.gameObject, this.outer, Util.CheckRoll(this.critStat, base.characterBody.master), AmyAssets.multiLockProjectilePrefab,
                90f, orbBounceRange, base.gameObject.transform.position, targets, OrbStorageUtility.Get("Prefabs/Effects/OrbEffects/HuntressGlaiveOrbEffect"));
            OrbManager.instance.AddOrb(orb);
        }

        public override void OnExit()
        {
            if (characterModel)
            {
                characterModel.invisibilityCount--;
            }
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.Intangible);
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}