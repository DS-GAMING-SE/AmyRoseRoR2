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
        public static T CreateMultiLockOrb<T>(float damage, GameObject attacker, bool crit, GameObject projectilePrefab, float speed, Vector3 origin, List<HurtBox> targets, GameObject orbEffectPrefab) where T : MultiLockOrb
        {
            if (targets.Count == 0) { return null; }
            MultiLockOrb orb = new MultiLockOrb
            {
                target = targets.FirstOrDefault(),
                remainingTargets = targets,
                attacker = attacker,
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
            public List<HurtBox> remainingTargets;

            public float speed = 60f;

            public float range = 60f;

            public GameObject orbEffectPrefab;

            public GameObject projectilePrefab;

            public GameObject attacker;

            public float damage;

            public bool isCrit;

            public override void OnArrival()
            {
                base.OnArrival();
                remainingTargets.RemoveAt(0);
                FireProjectile();
                StunTarget();
                if (remainingTargets.Count > 0 && Mathf.Abs((remainingTargets[0].transform.position - target.transform.position).magnitude) <= range)
                {
                    FireNextOrb();
                }
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
                if (projectilePrefab)
                {
                    Vector3 position = target.transform.position;
                    if (target.healthComponent && target.healthComponent.body)
                    {
                        position += Vector3.up * Mathf.Min(target.healthComponent.body.bestFitRadius * 0.5f, AmyStaticValues.specialMultiLockBlastRadius);
                    }
                    ProjectileManager.instance.FireProjectile(projectilePrefab, position, 
                        Quaternion.LookRotation((target.transform.position - origin).normalized, Vector3.up), attacker, damage, 0, isCrit, DamageColorIndex.Default, target.gameObject);
                }
            }

            protected virtual void FireNextOrb()
            {
                OrbManager.instance.AddOrb(CreateMultiLockOrb<MultiLockOrb>(damage, attacker, isCrit, projectilePrefab, speed, target.transform.position, remainingTargets, orbEffectPrefab));
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
        }
    }
}
