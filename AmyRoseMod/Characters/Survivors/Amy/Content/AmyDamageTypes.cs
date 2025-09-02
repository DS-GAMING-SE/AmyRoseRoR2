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

        public static void Initialize()
        {
            angleUpKnockbackIfGrounded = DamageAPI.ReserveDamageType();
            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
        }
        public static void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active && damageInfo.damageType.HasModdedDamageType(angleUpKnockbackIfGrounded) && self && self.body && self.body.characterMotor && self.body.characterMotor.isGrounded)
            {
                damageInfo.force = HedgehogUtils.Launch.LaunchManager.AngleAwayFromGround(damageInfo.force, self.body.characterMotor.estimatedGroundNormal);
            }
            orig(self, damageInfo);
        }
    }
}
