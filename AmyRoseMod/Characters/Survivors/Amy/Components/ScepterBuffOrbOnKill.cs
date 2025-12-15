using AmyRoseMod.Characters.Survivors.Amy.Content;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;
using static AmyRoseMod.Characters.Survivors.Amy.Content.AmyOrbs;

namespace AmyRoseMod.Characters.Survivors.Amy.Components
{
    public class ScepterBuffOrbOnKill : MonoBehaviour, IOnDamageInflictedServerReceiver
    {
        public ProjectileController projectileController;
        public int buffMaxStacks = AmyStaticValues.scepterSpecialMultiLockBuffMaxStack;
        private void Awake()
        {
            this.projectileController = base.GetComponent<ProjectileController>();
        }

        public void OnDamageInflictedServer(DamageReport damageReport)
        {
            if (this.projectileController.owner && damageReport.victim && damageReport.victimBody && damageReport.victimBody.healthComponent)
            {
                if (!damageReport.victimBody.healthComponent.alive)
                {
                    CharacterBody component = this.projectileController.owner.GetComponent<CharacterBody>();
                    if (component)
                    {
                        SendBuffOrb(damageReport.victim.transform.position, component.mainHurtBox, buffMaxStacks);
                    }
                }
            }
        }

        public static void SendBuffOrb(Vector3 origin, HurtBox target, int buffMaxStacks)
        {
            MultiLockScepterBuffOrb orb = new MultiLockScepterBuffOrb
            {
                origin = origin,
                target = target,
                buffMaxStacks = buffMaxStacks
            };
            OrbManager.instance.AddOrb(orb);
        }
    }
}
