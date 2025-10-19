using RoR2;
using UnityEngine;
using AmyRoseMod.Modules;
using System;
using RoR2.Projectile;
using AmyRoseMod.Characters.Survivors.Amy.Components;

namespace AmyRoseMod.Characters.Survivors.Amy
{
    public static class AmyAssets
    {
        // particle effects
        public static GameObject swordSwingEffect;
        public static GameObject swordHitImpactEffect;

        public static GameObject bombExplosionEffect;

        public static GameObject amyBoostFlashEffect;
        public static GameObject amyBoostAuraEffect;

        // networked hit sounds
        public static NetworkSoundEventDef swordHitSoundEvent;

        //projectiles
        public static GameObject multiLockProjectilePrefab;

        private static AssetBundle _assetBundle;

        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            swordHitSoundEvent = Modules.Content.CreateAndAddNetworkSoundEventDef("HenrySwordHit");

            CreateEffects();

            CreateProjectiles();
        }

        #region effects
        private static void CreateEffects()
        {
            CreateBombExplosionEffect();

            swordSwingEffect = _assetBundle.LoadEffect("HenrySwordSwingEffect", true);
            swordHitImpactEffect = _assetBundle.LoadEffect("ImpactHenrySlash");

            amyBoostFlashEffect = HedgehogUtils.Assets.CreateNewBoostFlash("AmyBoostFlash", 1, 1f,
                new Color(1, 1, 1), AmySurvivor.amyColor, new Color(0.5f, 0.07f, 0.3f), AmySurvivor.amyColor);


            amyBoostAuraEffect = HedgehogUtils.Assets.CreateNewBoostAura("AmyBoostAura", 1, 0.4f,
                new Color(1, 1, 1), AmySurvivor.amyColor, new Color(0.5f, 0.07f, 0.3f), AmySurvivor.amyColor);
        }

        private static void CreateBombExplosionEffect()
        {
            bombExplosionEffect = _assetBundle.LoadEffect("BombExplosionEffect", "HenryBombExplosion");

            if (!bombExplosionEffect)
                return;

            ShakeEmitter shakeEmitter = bombExplosionEffect.AddComponent<ShakeEmitter>();
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.duration = 0.2f;
            shakeEmitter.radius = 100f;
            shakeEmitter.scaleShakeRadiusWithLocalScale = false;

            shakeEmitter.wave = new Wave
            {
                amplitude = 0.3f,
                frequency = 40f,
                cycleOffset = 0f
            };

        }
        #endregion effects

        #region projectiles
        private static void CreateProjectiles()
        {
            CreateMultiLockProjectile();
			Modules.Content.AddProjectilePrefab(multiLockProjectilePrefab);
        }

        private static void CreateMultiLockProjectile()
        {
            //highly recommend setting up projectiles in editor, but this is a quick and dirty way to prototype if you want
            multiLockProjectilePrefab = Asset.CloneProjectilePrefab("CaptainAirstrikeProjectile1", "AmyRoseMultiLockProjectile");

            ProjectileImpactExplosion multiLockExplosion = multiLockProjectilePrefab.GetComponent<ProjectileImpactExplosion>();
            multiLockProjectilePrefab.GetComponent<ProjectileDamage>().damageType = DamageTypeCombo.GenericSpecial;
            multiLockProjectilePrefab.AddComponent<ProjectileTargetComponent>();
            multiLockProjectilePrefab.AddComponent<ProjectileAttachToTargetComponent>();

            multiLockExplosion.blastRadius = AmyStaticValues.specialMultiLockBlastRadius;
            multiLockExplosion.blastDamageCoefficient = 1f;
            multiLockExplosion.bonusBlastForce = Vector3.zero;
            multiLockExplosion.falloffModel = BlastAttack.FalloffModel.None;
            multiLockExplosion.lifetime = AmyStaticValues.specialMultiLockDetonationTime;
            multiLockExplosion.impactEffect = bombExplosionEffect;
            multiLockExplosion.lifetimeExpiredSound = Modules.Content.CreateAndAddNetworkSoundEventDef("HenryBombExplosion");

            ProjectileController multiLockController = multiLockProjectilePrefab.GetComponent<ProjectileController>();

            if (_assetBundle.LoadAsset<GameObject>("AmyRoseMultiLockHeartGhost") != null)
                multiLockController.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("AmyRoseMultiLockHeartGhost");
            
            multiLockController.startSound = "";
        }
        #endregion projectiles
    }
}
