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
        public static T CreateMultiLockOrb<T>(float damage, GameObject attacker, EntityStateMachine bodyState, bool crit, GameObject projectilePrefab, float speed, float range, Vector3 origin, List<HurtBox> targets, GameObject orbEffectPrefab) where T : MultiLockOrb
        {
            if (targets.Count == 0) { return null; }
            MultiLockOrb orb = new MultiLockOrb
            {
                target = targets.FirstOrDefault(),
                remainingTargets = targets,
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
            public List<HurtBox> remainingTargets;

            public float speed = 60f;

            public float range = 60f;

            public GameObject orbEffectPrefab;

            public GameObject projectilePrefab;

            public GameObject attacker;

            public EntityStateMachine bodyState;

            public float damage;

            public bool isCrit;

            public override void OnArrival()
            {
                base.OnArrival();
                remainingTargets.RemoveAt(0);
                FireProjectile();
                StunTarget();
                for (int i = 0; i < remainingTargets.Count; i++)
                {
                    if (remainingTargets.Count <= 0)
                    {
                        break;
                    }
                    if (remainingTargets[0] && Mathf.Abs((remainingTargets[0].transform.position - target.transform.position).magnitude) <= range)
                    {
                        FireNextOrb();
                        return;
                    }
                    else
                    {
                        remainingTargets.RemoveAt(0);
                        i--;
                    }
                }
                SetNextStateToEnd();
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
                    ProjectileManager.instance.FireProjectile(projectilePrefab, PositionAboveTarget(), 
                        Quaternion.LookRotation((target.transform.position - origin).normalized, Vector3.up), attacker, damage, 0, isCrit, DamageColorIndex.Default, target.gameObject);
                }
            }

            protected virtual void FireNextOrb()
            {
                OrbManager.instance.AddOrb(CreateMultiLockOrb<MultiLockOrb>(damage, attacker, bodyState, isCrit, projectilePrefab, speed, range, target.transform.position, remainingTargets, orbEffectPrefab));
            }

            protected virtual void SetNextStateToEnd()
            {
                SkillStates.MultiLockEnd state = new SkillStates.MultiLockEnd();
                state.teleportPosition = PositionAboveTarget();
                bodyState.SetNextState(state);
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
    }
}
