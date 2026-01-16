using AmyRoseMod;
using AmyRoseMod.Characters.Survivors.Amy;
using HedgehogUtils.Launch;
using R2API;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace AmyRoseMod.Characters.Survivors.Amy.Content
{
    public static class AmyOrbs
    {
        public static T CreateMultiLockOrb<T>(float damage, GameObject attacker, EntityStateMachine bodyState, bool crit, GameObject projectilePrefab, float speed, Vector3 origin, HurtBox target, GameObject orbEffectPrefab) where T : MultiLockOrb
        {
            if (!target) { return null; }
            MultiLockOrb orb = new MultiLockOrb
            {
                target = target,
                attacker = attacker,
                bodyState = bodyState,
                damage = damage,
                isCrit = crit,
                speed = speed,
                origin = origin,
                projectilePrefab = projectilePrefab,
                orbEffectPrefab = orbEffectPrefab
            };
            return (T)orb;
        }
        
        public class MultiLockOrb : Orb
        {
            public float speed = 60f;

            public GameObject orbEffectPrefab;

            public GameObject projectilePrefab;

            public GameObject attacker;

            public EntityStateMachine bodyState;

            public float damage;

            public bool isCrit;

            public override void OnArrival()
            {
                base.OnArrival();
                FireProjectile();
                StunTarget();
            }

            public override void Begin()
            {
                base.duration = base.distanceToTarget / this.speed;
                if (this.orbEffectPrefab)
                {
                    EffectData effectData = new EffectData
                    {
                        scale = 1f,
                        origin = this.origin,
                        genericFloat = base.duration
                    };
                    effectData.SetHurtBoxReference(this.target);
                    EffectManager.SpawnEffect(this.orbEffectPrefab, effectData, true);
                }
            }

            protected virtual void FireProjectile()
            {
                if (projectilePrefab && target)
                {
                    Vector3 forward = target.transform.position - origin;
                    forward.y = 0;
                    ProjectileManager.instance.FireProjectile(projectilePrefab, PositionAboveTarget(), 
                        Quaternion.LookRotation(forward.normalized, Vector3.up), attacker, damage, 0, isCrit, DamageColorIndex.Default, target.gameObject);
                }
            }

            protected void StunTarget()
            {
                if (target && target.healthComponent && target.healthComponent.gameObject.TryGetComponent<SetStateOnHurt>(out SetStateOnHurt stun))
                {
                    if (stun.targetStateMachine && stun.canBeStunned && stun.spawnedOverNetwork)
                    {
                        stun.SetStun(AmyStaticValues.specialMultiLockDetonationTime);
                    }
                }
            }

            private Vector3 PositionAboveTarget()
            {
                return target.transform.position + (Vector3.up * target.collider.bounds.extents.y * 1.2f);
            }
        }

        public class MultiLockScepterBuffOrb : Orb
        {
            public int buffMaxStacks = AmyStaticValues.scepterSpecialMultiLockBuffMaxStack;
            
            private CharacterBody targetBody;
            public override void Begin()
            {
                base.duration = 0.4f;
                EffectData effectData = new EffectData
                {
                    origin = this.origin,
                    genericFloat = base.duration
                };
                effectData.SetHurtBoxReference(this.target);
                EffectManager.SpawnEffect(AmyAssets.scepterMultiLockOrbEffect, effectData, true);
                targetBody = target.healthComponent ? target.healthComponent.body : null;
            }
            public override void OnArrival()
            {
                if (targetBody)
                {
                    targetBody.AddTimedBuff(AmyBuffs.scepterMultiLockBuff, AmyStaticValues.scepterSpecialMultiLockBuffDuration, buffMaxStacks);
                    if (targetBody.healthComponent)
                    {
                        targetBody.healthComponent.HealFraction(AmyStaticValues.scepterSpecialMultiLockHealAmount, default);
                    }
                    EffectData data = new EffectData
                    {
                        rootObject = targetBody.gameObject,
                        start = targetBody.corePosition
                    };
                    EffectManager.SpawnEffect(AmyAssets.scepterMultiLockOrbFlash, data, true);
                }
            }
        }
    }
}
