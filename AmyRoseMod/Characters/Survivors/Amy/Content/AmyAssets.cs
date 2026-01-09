using AmyRoseMod.Characters.Survivors.Amy.Components;
using AmyRoseMod.Modules;
using R2API;
using RoR2;
using RoR2.Audio;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
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
        public static GameObject superHammerSwingEffect;
        public static GameObject swordSwingEffect;
        public static GameObject hammerHitImpactEffect;
        public static GameObject superHammerHitImpactEffect;

        public static GameObject secondaryChargedEffect;
        public static GameObject superSecondaryChargedEffect;

        public static GameObject hammerSpinSpinningEffect;
        public static GameObject superHammerSpinSpinningEffect;

        public static GameObject multiLockExplosionEffect;
        public static GameObject superMultiLockExplosionEffect;

        public static GameObject amyBoostFlashEffect;
        public static GameObject amyBoostAuraEffect;

        public static GameObject superAmyBoostFlashEffect;
        public static GameObject superAmyBoostAuraEffect;

        public static GameObject multiLockEndEffect;
        public static GameObject superMultiLockEndEffect;

        // materials
        public static Material hammerSwingMaterial;
        public static Material superHammerSwingMaterial;
        public static Material heartImpactMaterial;
        public static Material superHeartImpactMaterial;
        public static Material heartMaterial;
        public static Material superHeartMaterial;

        public static Material hammerSpinMaterial;
        public static Material superHammerSpinMaterial;

        public static Material multiLockHeartMaterial;
        public static Material superMultiLockHeartMaterial;

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

                multiLockEndEffect.transform.Find("AmyMultiLockEndSparkles").GetComponent<ParticleSystemRenderer>().sharedMaterial = x.Result;
                superMultiLockEndEffect.transform.Find("AmyMultiLockEndSparkles").GetComponent<ParticleSystemRenderer>().sharedMaterial = x.Result;
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
        // Spaghetti... Async loading makes everything cool and good but it makes code ugly and hard to organize
        private static void CreateEffects(AssetBundle assetBundle)
        {
            CreateMultiLockExplosionEffect(assetBundle);

            // Multi-Lock End
            multiLockEndEffect = assetBundle.LoadEffect("AmyMultiLockEndEffect", true, 1f);
            var multiLockEndVFXAttributes = multiLockEndEffect.GetComponent<VFXAttributes>();
            multiLockEndVFXAttributes.secondaryParticleSystem = new ParticleSystem[]{ multiLockEndEffect.transform.Find("AmyMultiLockEndSparklesTiny").GetComponent<ParticleSystem>() };
            multiLockEndVFXAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
            superMultiLockEndEffect = assetBundle.LoadEffect("AmySuperMultiLockEndEffect", true, 1f);
            var superMultiLockEndVFXAttributes = superMultiLockEndEffect.GetComponent<VFXAttributes>();
            superMultiLockEndVFXAttributes.secondaryParticleSystem = new ParticleSystem[] { superMultiLockEndEffect.transform.Find("AmyMultiLockEndSparklesTiny").GetComponent<ParticleSystem>() };
            superMultiLockEndVFXAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;

            hammerSpinSpinningEffect = CreateHammerSpinEffect(assetBundle.LoadAsset<GameObject>("AmyHammerSpinEffect"));
            superHammerSpinSpinningEffect = CreateHammerSpinEffect(assetBundle.LoadAsset<GameObject>("AmySuperHammerSpinEffect"));
            var hammerSpinSpinningMesh = hammerSpinSpinningEffect.transform.GetChild(0).GetComponent<MeshFilter>();
            var superHammerSpinSpinningMesh = superHammerSpinSpinningEffect.transform.GetChild(0).GetComponent<MeshFilter>();
            var hammerSpinRing = hammerSpinSpinningEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>();
            var superHammerSpinRing = superHammerSpinSpinningEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>();
            AsyncOperationHandle<Mesh> asyncTorusMesh = Addressables.LoadAssetAsync<Mesh>("RoR2/DLC3/mdlTorusVFXRing04.fbx");
            asyncTorusMesh.Completed += delegate (AsyncOperationHandle<Mesh> x)
            {
                hammerSpinRing.mesh = x.Result;
                superHammerSpinRing.mesh = x.Result;
            };
            AsyncOperationHandle<Material> asyncHammerSpinMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC3/Drifter/matDrifterTornadoStreaks_02.mat");
            asyncHammerSpinMaterial.Completed += delegate (AsyncOperationHandle<Material> x)
            {
                hammerSpinMaterial = new Material(x.Result);
                hammerSpinMaterial.SetTexture("_RemapTex", assetBundle.LoadAsset<Texture>("texRampAmyEnergy"));
                hammerSpinMaterial.SetFloat("_AlphaBoost", 6.3f);
                hammerSpinRing.sharedMaterial = hammerSpinMaterial;
                superHammerSpinMaterial = new Material(x.Result);
                superHammerSpinMaterial.SetTexture("_RemapTex", assetBundle.LoadAsset<Texture>("texRampAmySuperEnergy"));
                superHammerSpinMaterial.SetFloat("_AlphaBoost", 9f);
                superHammerSpinRing.sharedMaterial = superHammerSpinMaterial;
            };

            // Hammer Swing VFX
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

                hammerSwingLargeEffect = PrefabAPI.InstantiateClone(hammerSwingEffect, "AmyRoseHammerSwingLargeEffect", false);
                ParticleSystem.MainModule hammerSwingLargeMain = hammerSwingLargeEffect.transform.Find("SwingTrail").GetComponent<ParticleSystem>().main;
                // original radius is (1.2, 1.2, 2.25)
                hammerSwingLargeMain.startSizeXMultiplier = 1.8f;
                hammerSwingLargeMain.startSizeYMultiplier = 1.8f;
                hammerSwingLargeMain.startSizeZMultiplier = 4.5f;

                superHammerSwingEffect = PrefabAPI.InstantiateClone(hammerSwingEffect, "AmyRoseHammerSwingSuperEffect", false);
                ParticleSystem.MainModule hammerSwingSuperMain = superHammerSwingEffect.transform.Find("SwingTrail").GetComponent<ParticleSystem>().main;
                // original radius is (1.2, 1.2, 2.25)
                hammerSwingSuperMain.startSizeXMultiplier = 5f;
                hammerSwingSuperMain.startSizeYMultiplier = 5f;
                hammerSwingSuperMain.startSizeZMultiplier = 12f;
                GameObject hammerSwingSuperBlur = GameObject.Instantiate(superHammerSwingEffect.transform.Find("SwingTrail").gameObject, superHammerSwingEffect.transform);
                hammerSwingSuperBlur.GetComponent<ParticleSystemRenderer>().sharedMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Croco/matCrocoSlashDistortion.mat").WaitForCompletion();
                ParticleSystem.MainModule hammerSwingSuperBlurMain = hammerSwingSuperBlur.GetComponent<ParticleSystem>().main;
                hammerSwingSuperBlurMain.startSizeXMultiplier = 4f;
                hammerSwingSuperBlurMain.startSizeYMultiplier = 4f;
                hammerSwingSuperBlurMain.startSizeZMultiplier = 2f;
                hammerSwingSuperBlurMain.startDelay = 0.2f;

                hammerSpinSpinningMesh.sharedMesh = hammerSwingRender.mesh;
                superHammerSpinSpinningMesh.sharedMesh = hammerSwingRender.mesh;

                // Hammer Swing Material
                AsyncOperationHandle<Material> asyncHammerSwingMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Loader/matLoaderSwingThick.mat");
                asyncHammerSwingMaterial.Completed += delegate (AsyncOperationHandle<Material> y)
                {
                    hammerSwingMaterial = new Material(y.Result);
                    hammerSwingMaterial.SetTexture("_RemapTex", assetBundle.LoadAsset<Texture>("texRampAmyHammer"));
                    hammerSwingMaterial.SetVector("_TintColor", new Vector4(0.3f, 0.3f, 0.3f, 1f));
                    hammerSwingRender.sharedMaterial = hammerSwingMaterial;
                    hammerSwingLargeEffect.transform.Find("SwingTrail").GetComponent<ParticleSystemRenderer>().sharedMaterial = hammerSwingMaterial;
                    hammerSpinSpinningMesh.GetComponent<MeshRenderer>().sharedMaterial = hammerSwingMaterial;
                    superHammerSpinSpinningMesh.GetComponent<MeshRenderer>().sharedMaterial = hammerSwingMaterial;
                    superHammerSwingMaterial = new Material(y.Result);
                    superHammerSwingMaterial.SetTexture("_RemapTex", assetBundle.LoadAsset<Texture>("texRampAmySuperEnergy"));
                    superHammerSwingMaterial.SetVector("_TintColor", new Vector4(0.6f, 0.6f, 0.6f, 1f));
                    superHammerSwingEffect.transform.Find("SwingTrail").GetComponent<ParticleSystemRenderer>().sharedMaterial = superHammerSwingMaterial;
                };
            };

            hammerHitImpactEffect = _assetBundle.LoadEffect("AmyHammerHitEffect", false, 0);
            hammerHitImpactEffect.AddComponent<DestroyOnParticleEnd>().trackedParticleSystem = hammerHitImpactEffect.transform.Find("HammerHitHeartImpact").GetComponent<ParticleSystem>();
            superHammerHitImpactEffect = _assetBundle.LoadEffect("AmySuperHammerHitEffect", false, 0);
            superHammerHitImpactEffect.AddComponent<DestroyOnParticleEnd>().trackedParticleSystem = superHammerHitImpactEffect.transform.Find("HammerHitHeartImpact").GetComponent<ParticleSystem>();

            // Tracer Glow Material
            AsyncOperationHandle<Material> asyncTracerMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matTracerBright.mat");
            asyncTracerMaterial.Completed += delegate (AsyncOperationHandle<Material> x)
            {
                hammerHitImpactEffect.transform.Find("HammerHitSparks").GetComponent<ParticleSystemRenderer>().sharedMaterial = x.Result;
                superHammerHitImpactEffect.transform.Find("HammerHitSparks").GetComponent<ParticleSystemRenderer>().sharedMaterial = x.Result;

                multiLockEndEffect.transform.Find("AmyMultiLockEndSparklesTiny").GetComponent<ParticleSystemRenderer>().sharedMaterial = x.Result;
                superMultiLockEndEffect.transform.Find("AmyMultiLockEndSparklesTiny").GetComponent<ParticleSystemRenderer>().sharedMaterial = x.Result;
            };

            // Heart/Impact Material
            AsyncOperationHandle<Material> asyncHeartImpactMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniRing1Generic.mat");
            asyncHeartImpactMaterial.Completed += delegate (AsyncOperationHandle<Material> x)
            {
                heartImpactMaterial = new Material(x.Result);
                heartImpactMaterial.SetTexture("_RemapTex", assetBundle.LoadAsset<Texture>("texRampAmyEnergy"));
                heartImpactMaterial.SetTexture("_MainTex", assetBundle.LoadAsset<Texture>("texAmyVFXHeartImpact"));
                heartImpactMaterial.SetFloat("_AlphaBoost", 4.5f);
                hammerHitImpactEffect.transform.Find("HammerHitHeartImpact").GetComponent<ParticleSystemRenderer>().sharedMaterial = heartImpactMaterial;
                
                superHeartImpactMaterial = new Material(x.Result);
                superHeartImpactMaterial.SetTexture("_RemapTex", assetBundle.LoadAsset<Texture>("texRampAmySuperEnergy"));
                superHeartImpactMaterial.SetTexture("_MainTex", assetBundle.LoadAsset<Texture>("texAmyVFXHeartImpact"));
                superHeartImpactMaterial.SetFloat("_AlphaBoost", 4.5f);
                superHammerHitImpactEffect.transform.Find("HammerHitHeartImpact").GetComponent<ParticleSystemRenderer>().sharedMaterial = superHeartImpactMaterial;

                heartMaterial = new Material(x.Result);
                heartMaterial.SetTexture("_RemapTex", assetBundle.LoadAsset<Texture>("texRampAmyEnergy"));
                heartMaterial.SetTexture("_MainTex", assetBundle.LoadAsset<Texture>("texAmyVFXHeart"));
                multiLockEndEffect.transform.Find("AmyMultiLockEndHearts").GetComponent<ParticleSystemRenderer>().sharedMaterial = heartMaterial;
                hammerSpinSpinningMesh.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = heartMaterial;

                superHeartMaterial = new Material(x.Result);
                superHeartMaterial.SetTexture("_RemapTex", assetBundle.LoadAsset<Texture>("texRampAmySuperEnergy"));
                superHeartMaterial.SetTexture("_MainTex", assetBundle.LoadAsset<Texture>("texAmyVFXHeart"));
                superMultiLockEndEffect.transform.Find("AmyMultiLockEndHearts").GetComponent<ParticleSystemRenderer>().sharedMaterial = superHeartMaterial;
                superHammerSpinSpinningMesh.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = superHeartMaterial;
            };

            // Boost
            amyBoostFlashEffect = HedgehogUtils.Assets.CreateNewBoostFlash("AmyBoostFlash", 1, 1f,
                new Color(1, 1, 1), AmySurvivor.amyColor, new Color(0.5f, 0.07f, 0.3f), AmySurvivor.amyColor);
            amyBoostAuraEffect = HedgehogUtils.Assets.CreateNewBoostAura("AmyBoostAura", 1, 0.4f,
                new Color(1, 1, 1), AmySurvivor.amyColor, new Color(0.5f, 0.07f, 0.3f), AmySurvivor.amyColor);
            superAmyBoostFlashEffect = HedgehogUtils.Assets.CreateNewBoostFlash("AmySuperBoostFlash", 1.3f, 1.6f,
                new Color(1, 1, 1), AmySurvivor.superAmyColor, new Color(1f, 0.2f, 0.3f), AmySurvivor.superAmyColor);
            superAmyBoostAuraEffect = HedgehogUtils.Assets.CreateNewBoostAura("AmySuperBoostAura", 1.3f, 0.8f,
                new Color(1, 1, 1), AmySurvivor.superAmyColor, new Color(1f, 0.2f, 0.3f), AmySurvivor.superAmyColor);

            // Multi-Lock Heart Material
            AsyncOperationHandle<Material> asyncMultiLockHeartMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Grandparent/matGrandParentSunCore.mat");
            asyncMultiLockHeartMaterial.Completed += delegate (AsyncOperationHandle<Material> x)
            {
                multiLockHeartMaterial = CreateMultiLockHeart(x.Result, assetBundle.LoadAsset<Texture>("texRampAmyEnergy"));
                superMultiLockHeartMaterial = CreateMultiLockHeart(x.Result, assetBundle.LoadAsset<Texture>("texRampAmySuperEnergy"));
            };
        }
        private static GameObject CreateHammerSpinEffect(GameObject start)
        {
            GameObject newSpin = start;
            newSpin.AddComponent<VFXAttributes>().DoNotCullPool = true;
            var hammerSpinSpinningMesh = newSpin.transform.GetChild(0).GetComponent<MeshFilter>();
            hammerSpinSpinningMesh.gameObject.AddComponent<RotateObject>().rotationSpeed = new Vector3(0, 0, -1200);
            var hammerSpinSpinningAnimateAlpha = hammerSpinSpinningMesh.gameObject.AddComponent<AnimateShaderAlpha>();
            hammerSpinSpinningAnimateAlpha.timeMax = 0.4f;
            hammerSpinSpinningAnimateAlpha.alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
            hammerSpinSpinningAnimateAlpha.disableOnEnd = true;
            hammerSpinSpinningAnimateAlpha.enabled = false;
            var hammerSpinSpinningDisable = newSpin.AddComponent<DisableParticleEmissionAndDestroyOnTimer>();
            hammerSpinSpinningDisable.waitDuration = 0.65f;
            var hammerSpinRing = newSpin.transform.GetChild(1).GetComponent<ParticleSystemRenderer>();
            hammerSpinSpinningDisable.particleSystems = new List<ParticleSystem> { hammerSpinSpinningMesh.transform.GetChild(0).GetComponent<ParticleSystem>(), hammerSpinRing.GetComponent<ParticleSystem>() };
            return newSpin;
        }

        private static Material CreateMultiLockHeart(Material input, Texture ramp)
        {
            Material newMat = new Material(input);
            newMat.SetTexture("_RemapTex", ramp);
            newMat.SetFloat("_FresnelPower", -1f);
            newMat.SetFloat("_AlphaBoost", 7.2f);
            newMat.SetFloat("_AlphaBias", 0.5f);
            return newMat;
        }

        private static void CreateMultiLockExplosionEffect(AssetBundle assetBundle)
        {
            AsyncOperationHandle<GameObject> asyncMultiLockExplosion = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Vagrant/VagrantTrackingBombExplosion.prefab");
            asyncMultiLockExplosion.Completed += delegate (AsyncOperationHandle<GameObject> x)
            {
                multiLockExplosionEffect = CreateMultiLockExplosion(x.Result, "AmyRoseMultiLockExplosionEffect", AmySurvivor.amyColor, new Color(1, 0, 0.5f), AmyStaticValues.specialMultiLockBlastRadius);
                AddNewEffectDef(multiLockExplosionEffect, "Play_amyrose_multilock_projectile_hit");
                superMultiLockExplosionEffect = CreateMultiLockExplosion(x.Result, "AmyRoseSuperMultiLockExplosionEffect", AmySurvivor.superAmyColor, new Color(1, 0.3f, 0.3f), AmyStaticValues.superSpecialMultiLockBlastRadius);
                AddNewEffectDef(superMultiLockExplosionEffect, "Play_amyrose_multilock_projectile_hit");

                AsyncOperationHandle<Material> asyncMultiLockExplosionMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matJellyfishLightningSphere.mat");
                asyncMultiLockExplosionMaterial.Completed += delegate (AsyncOperationHandle<Material> y)
                {
                    multiLockExplosionMaterial = new Material(y.Result);
                    multiLockExplosionMaterial.SetTexture("_RemapTex", assetBundle.LoadAsset<Texture>("texRampAmyEnergy"));

                    multiLockExplosionEffect.transform.Find("Nova Sphere").GetComponent<ParticleSystemRenderer>().sharedMaterial = multiLockExplosionMaterial;

                    superMultiLockExplosionMaterial = new Material(multiLockExplosionMaterial);
                    superMultiLockExplosionMaterial.SetTexture("_RemapTex", assetBundle.LoadAsset<Texture>("texRampAmySuperEnergy"));
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

            multiLockExplosion.blastRadius = AmyStaticValues.specialMultiLockBlastRadius + 0.5f;
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
                _assetBundle.LoadAsset<GameObject>("AmyRoseSuperMultiLockHeartGhost").transform.Find("Mesh").GetComponent<Renderer>().sharedMaterial = superMultiLockHeartMaterial;
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
