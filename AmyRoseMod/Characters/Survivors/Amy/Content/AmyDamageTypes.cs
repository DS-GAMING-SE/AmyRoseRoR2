using Amy;
using HedgehogUtils.Launch;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AmyRoseMod.Characters.Survivors.Amy.Content
{
    public static class AmyDamageTypes
    {
        public static DamageAPI.ModdedDamageType angleUpKnockbackIfGrounded;

        public static DamageAPI.ModdedDamageType launchNoAutoAim;

        public static void Initialize()
        {
            angleUpKnockbackIfGrounded = DamageAPI.ReserveDamageType();
            launchNoAutoAim = DamageAPI.ReserveDamageType();

            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
        }
        public static void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active && damageInfo.damageType.HasModdedDamageType(angleUpKnockbackIfGrounded) && self && self.body && self.body.characterMotor && self.body.characterMotor.isGrounded)
            {
                damageInfo.force = HedgehogUtils.Launch.LaunchManager.AngleAwayFromGround(damageInfo.force, self.body.characterMotor.estimatedGroundNormal);
            }
            orig(self, damageInfo);
            if (NetworkServer.active && damageInfo.HasModdedDamageType(launchNoAutoAim) && self && damageInfo.attacker && damageInfo.attacker.TryGetComponent<CharacterBody>(out CharacterBody body))
            {
                if (HedgehogUtils.Launch.LaunchManager.AttackCanLaunch(self, body, damageInfo))
                {
                    LaunchManager.Launch(self.body, body, damageInfo.force.normalized, LaunchManager.launchSpeed, damageInfo.damage, damageInfo.damage * 0.5f, damageInfo.crit, 1f, LaunchManager.baseDuration * damageInfo.procCoefficient);
                }
            }
        }
    }
}
