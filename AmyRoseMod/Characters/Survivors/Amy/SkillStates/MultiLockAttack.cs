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
        public HurtBox target;
        
        public List<HurtBox> targets;

        protected Vector3 targetLastPosition;

        public bool firstAttack;

        public Vector3 orbStartPosition;

        public float orbSpeed = 90f;

        public float orbBounceRange;

        protected float predictedTimeUntilArrival;

        protected AmyOrbs.MultiLockOrb orb;

        protected CharacterModel characterModel;
        
        public override void OnEnter()
        {
            base.OnEnter();
            if (base.isAuthority && firstAttack)
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
            if (target)
            {
                predictedTimeUntilArrival = Vector3.Distance(orbStartPosition, target.transform.position) / orbSpeed;
                predictedTimeUntilArrival += 0.1f;
            }
            else
            {
                targetLastPosition = base.transform.position;
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

            if (target)
            {
                targetLastPosition = target.transform.position;
            }

            if (base.isAuthority)
            {
                if (base.characterMotor)
                {
                    base.characterMotor.velocity = Vector3.zero;
                }
                if (fixedAge >= predictedTimeUntilArrival) // on orb hits
                {
                    if (targets.Count > 1)
                    {
                        targets.RemoveAt(0);
                        SanitizeTargets();
                        if (targets.Count > 0)
                        {
                            SetNextStateToOrb();
                            return;
                        }
                    }
                    SetNextStateToEnd();
                    return;
                }
            }
        }

        protected void SanitizeTargets()
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (!targets[0] || Vector3.Distance(targets[0].transform.position, targetLastPosition) > orbBounceRange)
                {
                    targets.RemoveAt(0);
                    i--;
                }
                else
                {
                    break;
                }
            }
        }

        public virtual void FireOrb()
        {
            orb = AmyOrbs.CreateMultiLockOrb<AmyOrbs.MultiLockOrb>(AmyStaticValues.specialMultiLockDamageCoefficient * damageStat, base.gameObject, this.outer, Util.CheckRoll(this.critStat, base.characterBody.master), 
                AmyAssets.multiLockProjectilePrefab, orbSpeed, orbStartPosition, target, OrbStorageUtility.Get("Prefabs/Effects/OrbEffects/HuntressGlaiveOrbEffect"));
            OrbManager.instance.AddOrb(orb);
        }

        public virtual void SetNextStateToOrb()
        {
            MultiLockAttack nextState = (MultiLockAttack)EntityStateCatalog.InstantiateState(typeof(MultiLockAttack));
            nextState.firstAttack = false;
            nextState.orbStartPosition = targetLastPosition;
            nextState.target = targets[0];
            nextState.targets = targets;
            nextState.orbBounceRange = orbBounceRange;
            nextState.orbSpeed = orbSpeed;
            this.outer.SetNextState(nextState);
        }

        public virtual void SetNextStateToEnd()
        {
            MultiLockEnd nextState = (MultiLockEnd)EntityStateCatalog.InstantiateState(typeof(MultiLockEnd));
            nextState.teleportPosition = targetLastPosition;
            this.outer.SetNextState(nextState);
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

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(HurtBoxReference.FromHurtBox(target));
            writer.Write(orbStartPosition);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            target = reader.ReadHurtBoxReference().ResolveHurtBox();
            orbStartPosition = reader.ReadVector3();
        }
    }
}