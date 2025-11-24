using AmyRoseMod.Characters.Survivors.Amy.Components;
using AmyRoseMod.Modules;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AmyRoseMod.Characters.Survivors.Amy
{
    public static class AmyAssets
    {
        // particle effects
        public static GameObject hammerSwingEffect;
        public static GameObject hammerSwingLargeEffect;
        public static GameObject hammerSwingSuperEffect;
        public static GameObject swordSwingEffect;
        public static GameObject swordHitImpactEffect;

        public static GameObject bombExplosionEffect;

        public static GameObject amyBoostFlashEffect;
        public static GameObject amyBoostAuraEffect;

        // materials
        public static Material hammerSwingMaterial;

        // networked hit sounds
        public static NetworkSoundEventDef swordHitSoundEvent;

        //projectiles
        public static GameObject multiLockProjectilePrefab;
        public static GameObject superMultiLockProjectilePrefab;

        private static AssetBundle _assetBundle;

        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            swordHitSoundEvent = Modules.Content.CreateAndAddNetworkSoundEventDef("HenrySwordHit");

            CreateEffects(assetBundle);

            CreateProjectiles();
        }

        #region effects
        private static void CreateEffects(AssetBundle assetBundle)
        {
            CreateBombExplosionEffect();

            AsyncOperationHandle<Material> asyncHammerSwingMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Loader/matLoaderSwingThick.mat");
            asyncHammerSwingMaterial.Completed += delegate (AsyncOperationHandle<Material> x)
            {
                hammerSwingMaterial = new Material(x.Result);
                hammerSwingMaterial.SetTexture("_RemapTex", assetBundle.LoadAsset<Texture>("texRampAmyHammer"));
                hammerSwingMaterial.SetVector("_TintColor", new Vector4(0.3f, 0.3f, 0.3f, 1f));
            };

            AsyncOperationHandle<GameObject> asyncHammerSwingParticle = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderSwingBasic.prefab");
            asyncHammerSwingParticle.Completed += delegate (AsyncOperationHandle<GameObject> x)
            {
                hammerSwingEffect = PrefabAPI.InstantiateClone(x.Result, "AmyRoseHammerSwingEffect", false);
                Transform swingTrailObject = hammerSwingEffect.transform.Find("SwingTrail");
                ParticleSystem.MainModule hammerSwingMain = swingTrailObject.GetComponent<ParticleSystem>().main;
                // original size is (1.2, 1.2, 2.25)
                hammerSwingMain.startSizeXMultiplier = 1.4f;
                hammerSwingMain.startSizeYMultiplier = 1.4f;
                hammerSwingMain.startSizeZMultiplier = 3f;

                ParticleSystemRenderer hammerSwingRender = swingTrailObject.GetComponent<ParticleSystemRenderer>();
                hammerSwingRender.sharedMaterial = hammerSwingMaterial;

                hammerSwingLargeEffect = PrefabAPI.InstantiateClone(hammerSwingEffect, "AmyRoseHammerSwingLargeEffect", false);
                ParticleSystem.MainModule hammerSwingLargeMain = hammerSwingLargeEffect.transform.Find("SwingTrail").GetComponent<ParticleSystem>().main;
                // original size is (1.2, 1.2, 2.25)
                hammerSwingLargeMain.startSizeXMultiplier = 1.8f;
                hammerSwingLargeMain.startSizeYMultiplier = 1.8f;
                hammerSwingLargeMain.startSizeZMultiplier = 4.5f;

                hammerSwingSuperEffect = PrefabAPI.InstantiateClone(hammerSwingEffect, "AmyRoseHammerSwingSuperEffect", false);
                ParticleSystem.MainModule hammerSwingSuperMain = hammerSwingSuperEffect.transform.Find("SwingTrail").GetComponent<ParticleSystem>().main;
                // original size is (1.2, 1.2, 2.25)
                hammerSwingSuperMain.startSizeXMultiplier = 5f;
                hammerSwingSuperMain.startSizeYMultiplier = 5f;
                hammerSwingSuperMain.startSizeZMultiplier = 12f;
                GameObject hammerSwingSuperBlur = GameObject.Instantiate(hammerSwingSuperEffect.transform.Find("SwingTrail").gameObject, hammerSwingSuperEffect.transform);
                hammerSwingSuperBlur.GetComponent<ParticleSystemRenderer>().sharedMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Croco/matCrocoSlashDistortion.mat").WaitForCompletion();
                ParticleSystem.MainModule hammerSwingSuperBlurMain = hammerSwingSuperBlur.GetComponent<ParticleSystem>().main;
                hammerSwingSuperBlurMain.startSizeXMultiplier = 4f;
                hammerSwingSuperBlurMain.startSizeYMultiplier = 4f;
                hammerSwingSuperBlurMain.startSizeZMultiplier = 2f;
                hammerSwingSuperBlurMain.startDelay = 0.2f;
            };

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
            CreateSuperMultiLockProjectile();
			Modules.Content.AddProjectilePrefab(multiLockProjectilePrefab);
            Modules.Content.AddProjectilePrefab(superMultiLockProjectilePrefab);
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

        private static void CreateSuperMultiLockProjectile()
        {
            superMultiLockProjectilePrefab = PrefabAPI.InstantiateClone(multiLockProjectilePrefab, "AmyRoseSuperMultiLockProjectile");

            ProjectileImpactExplosion multiLockExplosion = superMultiLockProjectilePrefab.GetComponent<ProjectileImpactExplosion>();
            multiLockExplosion.blastRadius = AmyStaticValues.superSpecialMultiLockBlastRadius;
            multiLockExplosion.impactEffect = bombExplosionEffect;

            ProjectileController multiLockController = multiLockProjectilePrefab.GetComponent<ProjectileController>();

            if (_assetBundle.LoadAsset<GameObject>("AmyRoseMultiLockHeartGhost") != null)
                multiLockController.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("AmyRoseMultiLockHeartGhost");
        }
        #endregion projectiles

        private static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }

        private static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = soundName;

            Modules.Content.AddEffectDef(newEffectDef);
        }
    }
}
