using AmyRoseMod.Characters.Survivors.Amy.Components;
using AmyRoseMod.Modules;
using R2API;
using RoR2;
using RoR2.Audio;
using RoR2.Projectile;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AmyRoseMod.Characters.Survivors.Amy
{
    public static class AmyAssets
    {
        public static GameObject multiLockCrosshair;
        
        // particle effects
        public static GameObject hammerSwingEffect;
        public static GameObject hammerSwingLargeEffect;
        public static GameObject hammerSwingSuperEffect;
        public static GameObject swordSwingEffect;
        public static GameObject hammerHitImpactEffect;

        public static GameObject secondaryChargedEffect;
        public static GameObject superSecondaryChargedEffect;

        public static GameObject multiLockExplosionEffect;
        public static GameObject superMultiLockExplosionEffect;

        public static GameObject amyBoostFlashEffect;
        public static GameObject amyBoostAuraEffect;

        // materials
        public static Material hammerSwingMaterial;
        public static Material heartImpactMaterial;

        public static Material multiLockHeartMaterial;

        public static Material multiLockExplosionMaterial;
        public static Material superMultiLockExplosionMaterial;

        // networked hit sounds
        public static NetworkSoundEventDef hammerHitSoundEvent;
        public static NetworkSoundEventDef hammerHitHeavySoundEvent;
        public static LoopSoundDef hammerSpinLoopSoundDef;

        //projectiles
        public static GameObject multiLockProjectilePrefab;
        public static GameObject superMultiLockProjectilePrefab;

        public static GameObject scepterMultiLockProjectilePrefab;
        public static GameObject scepterSuperMultiLockProjectilePrefab;

        private static AssetBundle _assetBundle;

        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            hammerHitSoundEvent = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_hit");
            hammerHitHeavySoundEvent = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_amyrose_hit_heavy");
            hammerSpinLoopSoundDef = ScriptableObject.CreateInstance<LoopSoundDef>();
            hammerSpinLoopSoundDef.startSoundName = "Play_amyrose_hammer_spin_loop";
            hammerSpinLoopSoundDef.stopSoundName = "Stop_amyrose_hammer_spin_loop";

            CreateMultiLockCrosshair(assetBundle);

            CreateEffects(assetBundle);

            CreateProjectiles();

            AsyncOperationHandle<Material> asyncSparkleMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matWideGlow.mat");
            asyncSparkleMaterial.Completed += delegate (AsyncOperationHandle<Material> x)
            {
                _assetBundle.LoadAsset<GameObject>("AmyRoseMultiLockHeartGhost").transform.Find("MultiLockHeartSparkles").GetComponent<ParticleSystemRenderer>().sharedMaterial = x.Result;
                _assetBundle.LoadAsset<GameObject>("AmyRoseSuperMultiLockHeartGhost").transform.Find("MultiLockHeartSparkles").GetComponent<ParticleSystemRenderer>().sharedMaterial = x.Result;
                secondaryChargedEffect = _assetBundle.LoadEffect("AmySecondaryChargedEffect", true, 0.3f);
                secondaryChargedEffect.GetComponent<ParticleSystemRenderer>().sharedMaterial = x.Result;
                superSecondaryChargedEffect = _assetBundle.LoadEffect("AmySuperSecondaryChargedEffect", true, 0.3f);
                superSecondaryChargedEffect.GetComponent<ParticleSystemRenderer>().sharedMaterial = x.Result;
            };
        }

        private static void CreateMultiLockCrosshair(AssetBundle assetBundle)
        {
            AsyncOperationHandle<GameObject> asyncCrosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/SimpleDotCrosshair.prefab");
            asyncCrosshair.Completed += delegate (AsyncOperationHandle<GameObject> x)
            {
                multiLockCrosshair = PrefabAPI.InstantiateClone(x.Result, "AmyRoseMultiLockCrosshair", false);
                multiLockCrosshair.AddComponent<AmyMultiLockCrosshairController>();
            };
            AmyMultiLockCrosshairController.multiLockHeartPrefab = assetBundle.LoadAsset<GameObject>("MultiLockHeartUI");
        }

        #region effects
        private static void CreateEffects(AssetBundle assetBundle)
        {
            CreateMultiLockExplosionEffect(assetBundle);

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
                // original radius is (1.2, 1.2, 2.25)
                hammerSwingMain.startSizeXMultiplier = 1.4f;
                hammerSwingMain.startSizeYMultiplier = 1.4f;
                hammerSwingMain.startSizeZMultiplier = 3f;

                ParticleSystemRenderer hammerSwingRender = swingTrailObject.GetComponent<ParticleSystemRenderer>();
                hammerSwingRender.sharedMaterial = hammerSwingMaterial;

                hammerSwingLargeEffect = PrefabAPI.InstantiateClone(hammerSwingEffect, "AmyRoseHammerSwingLargeEffect", false);
                ParticleSystem.MainModule hammerSwingLargeMain = hammerSwingLargeEffect.transform.Find("SwingTrail").GetComponent<ParticleSystem>().main;
                // original radius is (1.2, 1.2, 2.25)
                hammerSwingLargeMain.startSizeXMultiplier = 1.8f;
                hammerSwingLargeMain.startSizeYMultiplier = 1.8f;
                hammerSwingLargeMain.startSizeZMultiplier = 4.5f;

                hammerSwingSuperEffect = PrefabAPI.InstantiateClone(hammerSwingEffect, "AmyRoseHammerSwingSuperEffect", false);
                ParticleSystem.MainModule hammerSwingSuperMain = hammerSwingSuperEffect.transform.Find("SwingTrail").GetComponent<ParticleSystem>().main;
                // original radius is (1.2, 1.2, 2.25)
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

            hammerHitImpactEffect = _assetBundle.LoadEffect("AmyHammerHitEffect", false, 0);
            hammerHitImpactEffect.AddComponent<DestroyOnParticleEnd>().trackedParticleSystem = hammerHitImpactEffect.transform.Find("HammerHitHeartImpact").GetComponent<ParticleSystem>();
            AsyncOperationHandle<Material> asyncTracerMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matTracerBright.mat");
            asyncTracerMaterial.Completed += delegate (AsyncOperationHandle<Material> x)
            {
                hammerHitImpactEffect.transform.Find("HammerHitSparks").GetComponent<ParticleSystemRenderer>().sharedMaterial = x.Result;
            };
            AsyncOperationHandle<Material> asyncHeartImpactMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniRing1Generic.mat");
            asyncHeartImpactMaterial.Completed += delegate (AsyncOperationHandle<Material> x)
            {
                heartImpactMaterial = new Material(x.Result);
                heartImpactMaterial.SetTexture("_RemapTex", assetBundle.LoadAsset<Texture>("texRampAmyEnergy"));
                heartImpactMaterial.SetTexture("_MainTex", assetBundle.LoadAsset<Texture>("texAmyVFXHeartImpact"));
                hammerHitImpactEffect.transform.Find("HammerHitHeartImpact").GetComponent<ParticleSystemRenderer>().sharedMaterial = heartImpactMaterial;
            };

            amyBoostFlashEffect = HedgehogUtils.Assets.CreateNewBoostFlash("AmyBoostFlash", 1, 1f,
                new Color(1, 1, 1), AmySurvivor.amyColor, new Color(0.5f, 0.07f, 0.3f), AmySurvivor.amyColor);


            amyBoostAuraEffect = HedgehogUtils.Assets.CreateNewBoostAura("AmyBoostAura", 1, 0.4f,
                new Color(1, 1, 1), AmySurvivor.amyColor, new Color(0.5f, 0.07f, 0.3f), AmySurvivor.amyColor);

            AsyncOperationHandle<Material> asyncMultiLockHeartMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Grandparent/matGrandParentSunCore.mat");
            asyncMultiLockHeartMaterial.Completed += delegate (AsyncOperationHandle<Material> x)
            {
                multiLockHeartMaterial = new Material(x.Result);
                multiLockHeartMaterial.SetTexture("_RemapTex", assetBundle.LoadAsset<Texture>("texRampAmyEnergy"));
                multiLockHeartMaterial.SetFloat("_FresnelPower", -1f);
                multiLockHeartMaterial.SetFloat("_AlphaBoost", 7.2f);
                multiLockHeartMaterial.SetFloat("_AlphaBias", 0.5f);
            };
        }

        private static void CreateMultiLockExplosionEffect(AssetBundle assetBundle)
        {
            AsyncOperationHandle<GameObject> asyncMultiLockExplosion = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Vagrant/VagrantTrackingBombExplosion.prefab");
            asyncMultiLockExplosion.Completed += delegate (AsyncOperationHandle<GameObject> x)
            {
                multiLockExplosionEffect = CreateMultiLockExplosion(x.Result, "AmyRoseMultiLockExplosionEffect", AmySurvivor.amyColor, new Color(1, 0, 0.5f), AmyStaticValues.specialMultiLockBlastRadius);
                AddNewEffectDef(multiLockExplosionEffect, "Play_amyrose_multilock_projectile_hit");
                superMultiLockExplosionEffect = CreateMultiLockExplosion(x.Result, "AmyRoseSuperMultiLockExplosionEffect", AmySurvivor.superAmyColor, new Color(1, 0.8f, 0.2f), AmyStaticValues.superSpecialMultiLockBlastRadius);
                AddNewEffectDef(superMultiLockExplosionEffect, "Play_amyrose_multilock_projectile_hit");

                AsyncOperationHandle<Material> asyncMultiLockExplosionMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matJellyfishLightningSphere.mat");
                asyncMultiLockExplosionMaterial.Completed += delegate (AsyncOperationHandle<Material> y)
                {
                    multiLockExplosionMaterial = new Material(y.Result);
                    multiLockExplosionMaterial.SetTexture("_RemapTex", assetBundle.LoadAsset<Texture>("texRampAmyEnergy"));

                    multiLockExplosionEffect.transform.Find("Nova Sphere").GetComponent<ParticleSystemRenderer>().sharedMaterial = multiLockExplosionMaterial;

                    superMultiLockExplosionMaterial = new Material(multiLockExplosionMaterial);
                    superMultiLockExplosionMaterial.SetTexture("_RemapTex", assetBundle.LoadAsset<Texture>("texRampAmyEnergy")); // super ramp energy once that exists
                    superMultiLockExplosionMaterial.SetFloat("_RimPower", 4f);

                    superMultiLockExplosionEffect.transform.Find("Nova Sphere").GetComponent<ParticleSystemRenderer>().sharedMaterial = superMultiLockExplosionMaterial;
                };
            };

        }

        private static GameObject CreateMultiLockExplosion(GameObject original, string name, Color color1, Color color2, float radius)
        {
            GameObject prefab = PrefabAPI.InstantiateClone(original, name);
            if (!prefab)
                return prefab;
            prefab.GetComponent<EffectComponent>().soundName = "";

            ShakeEmitter shakeEmitter = prefab.GetComponent<ShakeEmitter>();
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.duration = 0.2f;
            shakeEmitter.radius = 50f;
            shakeEmitter.scaleShakeRadiusWithLocalScale = false;

            shakeEmitter.wave = new Wave
            {
                amplitude = 0.3f,
                frequency = 40f,
                cycleOffset = 0f
            };

            GameObject.Destroy(prefab.transform.Find("Water, Billboard").gameObject);
            GameObject.Destroy(prefab.transform.Find("Sparks").gameObject);
            GameObject.Destroy(prefab.transform.Find("Lightning, Radial").gameObject);

            var dashParticle = prefab.transform.Find("Dash, Bright").GetComponent<ParticleSystem>().main;
            dashParticle.startColor = new ParticleSystem.MinMaxGradient { colorMin = color1, colorMax = color2 };

            var flashClusterParticle = prefab.transform.Find("FlashCluster").GetComponent<ParticleSystem>().main;
            flashClusterParticle.startColor = color1;

            var light = prefab.transform.Find("Point Light").GetComponent<Light>();
            light.intensity = 40f;
            light.color = color1;

            prefab.transform.localScale = Vector3.one * (radius / 4f);

            return prefab;
        }
        #endregion effects

        #region projectiles
        private static void CreateProjectiles()
        {
            CreateMultiLockProjectile();
            CreateSuperMultiLockProjectile();
			Modules.Content.AddProjectilePrefab(multiLockProjectilePrefab);
            Modules.Content.AddProjectilePrefab(superMultiLockProjectilePrefab);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter"))
            {
                CreateScepterMultiLockProjectiles();
            }
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
            multiLockExplosion.impactEffect = multiLockExplosionEffect;

            ProjectileController multiLockController = multiLockProjectilePrefab.GetComponent<ProjectileController>();

            if (_assetBundle.LoadAsset<GameObject>("AmyRoseMultiLockHeartGhost"))
            {
                _assetBundle.LoadAsset<GameObject>("AmyRoseMultiLockHeartGhost").transform.Find("Mesh").GetComponent<Renderer>().sharedMaterial = multiLockHeartMaterial;
                multiLockController.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("AmyRoseMultiLockHeartGhost");
            }
            multiLockController.startSound = "Play_amyrose_multilock_projectile_spawn";
        }

        private static void CreateSuperMultiLockProjectile()
        {
            superMultiLockProjectilePrefab = PrefabAPI.InstantiateClone(multiLockProjectilePrefab, "AmyRoseSuperMultiLockProjectile");

            ProjectileImpactExplosion multiLockExplosion = superMultiLockProjectilePrefab.GetComponent<ProjectileImpactExplosion>();
            multiLockExplosion.blastRadius = AmyStaticValues.superSpecialMultiLockBlastRadius;
            multiLockExplosion.impactEffect = superMultiLockExplosionEffect;

            ProjectileController multiLockController = superMultiLockProjectilePrefab.GetComponent<ProjectileController>();

            if (_assetBundle.LoadAsset<GameObject>("AmyRoseSuperMultiLockHeartGhost"))
            {
                _assetBundle.LoadAsset<GameObject>("AmyRoseSuperMultiLockHeartGhost").transform.Find("Mesh").GetComponent<Renderer>().sharedMaterial = multiLockHeartMaterial;
                multiLockController.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("AmyRoseSuperMultiLockHeartGhost");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void CreateScepterMultiLockProjectiles()
        {
            scepterMultiLockProjectilePrefab = PrefabAPI.InstantiateClone(multiLockProjectilePrefab, "AmyRoseScepterMultiLockProjectile");
            scepterMultiLockProjectilePrefab.AddComponent<ScepterBuffOrbOnKill>();

            scepterSuperMultiLockProjectilePrefab = PrefabAPI.InstantiateClone(superMultiLockProjectilePrefab, "AmyRoseScepterSuperMultiLockProjectile");
            scepterSuperMultiLockProjectilePrefab.AddComponent<ScepterBuffOrbOnKill>().buffMaxStacks = AmyStaticValues.scepterSuperSpecialMultiLockBuffMaxStack;

            Modules.Content.AddProjectilePrefab(scepterMultiLockProjectilePrefab);
            Modules.Content.AddProjectilePrefab(scepterSuperMultiLockProjectilePrefab);
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
