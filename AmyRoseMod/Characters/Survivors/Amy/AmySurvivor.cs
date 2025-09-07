using BepInEx.Configuration;
using Amy.Modules;
using Amy.Modules.Characters;
using Amy.Survivors.Amy.Components;
using Amy.Survivors.Amy.SkillStates;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using AmyRoseMod.Modules;
using AmyRoseMod.Characters.Survivors.Amy.Content;
using AmyRoseMod.Characters.Survivors.Amy.SkillStates;

namespace Amy.Survivors.Amy
{
    public class AmySurvivor : SurvivorBase<AmySurvivor>
    {
        //used to load the assetbundle for this character. must be unique
        public override string assetBundleName => "amyroseassetbundle"; //if you do not change this, you are giving permission to deprecate the mod

        //the name of the prefab we will create. conventionally ending in "Body". must be unique
        public override string bodyName => "AmyRoseBody"; //if you do not change this, you get the point by now

        //name of the ai master for vengeance and goobo. must be unique
        public override string masterName => "AmyRoseMonsterMaster"; //if you do not

        //the names of the prefabs you set up in unity that we will use to build your character
        public override string modelPrefabName => "mdlHenry";
        public override string displayPrefabName => "HenryDisplay";

        public const string AMY_PREFIX = AmyPlugin.DEVELOPER_PREFIX + "_AMY_ROSE_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => AMY_PREFIX;

        public static Color amyColor = new Color(1f, 0.5f, 0.9f);
        
        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = AMY_PREFIX + "NAME",
            subtitleNameToken = AMY_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texHenryIcon"),
            bodyColor = amyColor,
            sortPosition = 100,

            crosshair = Asset.LoadCrosshair("Standard"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 110f,
            healthRegen = 1f,
            armor = 0f,

            jumpCount = 2,
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "SwordModel",
                    material = assetBundle.LoadMaterial("matHenry"),
                },
                new CustomRendererInfo
                {
                    childName = "GunModel",
                },
                new CustomRendererInfo
                {
                    childName = "Model",
                }
        };

        public override UnlockableDef characterUnlockableDef => AmyUnlockables.characterUnlockableDef;
        
        public override ItemDisplaysBase itemDisplays => null;

        //set in base classes
        public override AssetBundle assetBundle { get; protected set; }

        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }

        public override void Initialize()
        {
            //uncomment if you have multiple characters
            //ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Amy");

            //if (!characterEnabled.Value)
            //    return;

            base.Initialize();
        }

        public override void InitializeCharacter()
        {
            //need the character unlockable before you initialize the survivordef
            AmyUnlockables.Init();

            base.InitializeCharacter();

            AmyConfig.Init();
            AmyStates.Init();
            AmyTokens.Init();

            AmyAssets.Init(assetBundle);
            AmyBuffs.Init(assetBundle);
            AmyDamageTypes.Initialize();

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            RecalculateStats.Initialize();
        }

        private void AdditionalBodySetup()
        {
            AddHitboxes();
            bodyPrefab.AddComponent<HedgehogUtils.Miscellaneous.MomentumPassive>();
            bodyPrefab.AddComponent<HedgehogUtils.Boost.BoostLogic>();
            bodyPrefab.AddComponent<HedgehogUtils.Miscellaneous.StayOnGround>();
            //bodyPrefab.AddComponent<HuntressTrackerComopnent>();
            //anything else here
        }

        public void AddHitboxes()
        {
            //example of how to create a HitBoxGroup. see summary for more details
            Prefabs.SetupHitBoxGroup(characterModelObject, "HorizontalSwing", "HorizontalSwingHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "VerticalSwing", "VerticalSwingHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "LargeSwing", "LargeSwingHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "Stomp", "StompHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "LargeStomp", "LargeStompHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "Spin", "SpinHitbox");
        }

        public override void InitializeEntityStateMachines() 
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //the main "Body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(EntityStates.GenericCharacterMain), typeof(EntityStates.SpawnTeleporterState));
            //if you set up a custom main characterstate, set it up here
                //don't forget to register custom entitystates in your HenryStates.cs

            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon2");
        }

        #region skills
        public override void InitializeSkills()
        {
            //remove the genericskills from the commando body we cloned
            Skills.ClearGenericSkills(bodyPrefab);
            //add our own
            AddPassiveSkill();
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtilitySkills();
            AddSpecialSkills();
        }

        //skip if you don't have a passive
        //also skip if this is your first look at skills
        private void AddPassiveSkill()
        {
            //option 1. fake passive icon just to describe functionality we will implement elsewhere
            bodyPrefab.GetComponent<SkillLocator>().passiveSkill = new SkillLocator.PassiveSkill
            {
                enabled = true,
                skillNameToken = HedgehogUtils.Language.momentumPassiveNameToken,
                skillDescriptionToken = HedgehogUtils.Language.momentumPassiveDescriptionToken,
                icon = assetBundle.LoadAsset<Sprite>("texPassiveMomentumIcon"),
            };
            #region Multiple Passives
            //option 2. a new SkillFamily for a passive, used if you want multiple selectable passives
            /*GenericSkill passiveGenericSkill = Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, "PassiveSkill");
            SkillDef passiveSkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "HenryPassive",
                skillNameToken = AMY_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = AMY_PREFIX + "PASSIVE_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AGILE" },
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),

                //unless you're somehow activating your passive like a skill, none of the following is needed.
                //but that's just me saying things. the tools are here at your disposal to do whatever you like with

                //activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Shoot)),
                //activationStateMachineName = "Weapon1",
                //interruptPriority = EntityStates.InterruptPriority.Skill,

                //baseRechargeInterval = 1f,
                //baseMaxStock = 1,

                //rechargeStock = 1,
                //requiredStock = 1,
                //stockToConsume = 1,

                //resetCooldownTimerOnUse = false,
                //fullRestockOnAssign = true,
                //dontAllowPastMaxStocks = false,
                //mustKeyPress = false,
                //beginSkillCooldownOnSkillEnd = false,

                //isCombatSkill = true,
                //canceledFromSprinting = false,
                //cancelSprintingOnActivation = false,
                //forceSprintDuringState = false,

            });
            Skills.AddSkillsToFamily(passiveGenericSkill.skillFamily, passiveSkillDef1);*/
            #endregion
        }
        public static SteppedSkillDef primaryMelee;
        public static SkillDef secondarySmash;
        public static AmySkillDefs.AmyBoostSkillDef utilityBoost;
        public static SkillDef specialMultilock;

        //if this is your first look at skilldef creation, take a look at Secondary first
        private void AddPrimarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Primary);

            //the primary skill is created using a constructor for a typical primary
            //it is also a SteppedSkillDef. Custom Skilldefs are very useful for custom behaviors related to casting a skill. see ror2's different skilldefs for reference
            primaryMelee = Skills.CreateSkillDef<SteppedSkillDef>(new SkillDefInfo
                (
                    "AmyPrimaryHammer",
                    AMY_PREFIX + "PRIMARY_HAMMER_NAME",
                    AMY_PREFIX + "PRIMARY_HAMMER_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texPrimaryHammerSwingIcon"),
                    new EntityStates.SerializableEntityStateType(typeof(PrimaryHammer)),
                    "Weapon",
                    true
                ));
            primaryMelee.stepCount = 4;
            primaryMelee.stepGraceDuration = 0.5f;
            primaryMelee.keywordTokens = new string[] { "KEYWORD_AGILE", HedgehogUtils.Language.launchKeyword };

            Skills.AddPrimarySkills(bodyPrefab, primaryMelee);
        }

        private void AddSecondarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Secondary);

            //here is a basic skill def with all fields accounted for
            secondarySmash = Skills.CreateSkillDef<SkillDef>(new SkillDefInfo
            {
                skillName = "AmySecondaryHammerSmash",
                skillNameToken = AMY_PREFIX + "SECONDARY_HAMMER_SMASH_NAME",
                skillDescriptionToken = AMY_PREFIX + "SECONDARY_HAMMER_SMASH_DESCRIPTION",
                keywordTokens = new string[] { HedgehogUtils.Language.launchKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSecondaryHammerSmashIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(HammerSmashCharge)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 4f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = true,
                canceledFromSprinting = true,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,

            });

            Skills.AddSecondarySkills(bodyPrefab, secondarySmash);
        }

        private void AddUtilitySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Utility);

            //here's a skilldef of a typical movement skill.
            utilityBoost = Skills.CreateSkillDef<AmySkillDefs.AmyBoostSkillDef>(new SkillDefInfo
            {
                skillName = "AmyBoost",
                skillNameToken = AMY_PREFIX + "UTILITY_BOOST_NAME",
                skillDescriptionToken = AMY_PREFIX + "UTILITY_BOOST_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityBoostIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(Boost)),
                activationStateMachineName = "Body",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 0f,
                baseMaxStock = 1,

                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = true,
            });
            utilityBoost.boostIdleState = new EntityStates.SerializableEntityStateType(typeof(BoostIdle));
            utilityBoost.brakeState = new EntityStates.SerializableEntityStateType(typeof(Brake));
            utilityBoost.boostHUDColor = amyColor;
            //utilityBoost.hammerSwingState = new EntityStates.SerializableEntityStateType(typeof(BoostIdle));

            Skills.AddUtilitySkills(bodyPrefab, utilityBoost);
        }

        private void AddSpecialSkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Special);

            //a basic skill. some fields are omitted and will just have default values
            SkillDef specialMultilock = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "AmySpecialMultiLock",
                skillNameToken = AMY_PREFIX + "SPECIAL_MULTILOCK_NAME",
                skillDescriptionToken = AMY_PREFIX + "SPECIAL_MULTILOCK_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpecialMultiLockIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ThrowBomb)),
                //setting this to the "weapon2" EntityStateMachine allows us to cast this skill at the same time primary, which is set to the "weapon" EntityStateMachine
                activationStateMachineName = "Weapon2", interruptPriority = EntityStates.InterruptPriority.Skill,

                baseMaxStock = 1,
                baseRechargeInterval = 8f,

                isCombatSkill = true,
                mustKeyPress = true,
            });

            Skills.AddSpecialSkills(bodyPrefab, specialMultilock);
        }
        #endregion skills
        
        #region skins
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ModelSkinController displaySkinController = displayPrefab.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Skins.CreateSkinDef("DEFAULT_SKIN",
                //assetBundle.LoadAsset<Sprite>("texMainSkin"),
                R2API.Skins.CreateSkinIcon(amyColor, new Color (0.95f, 0.07f, 0.2f), new Color(0.9f, 0.84f, 0.2f), new Color(1f, 0.75f, 0.5f), Color.white),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
                //pass in meshes as they are named in your assetbundle
            //currently not needed as with only 1 skin they will simply take the default meshes
                //uncomment this when you have another skin
            //defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySword",
            //    "meshHenryGun",
            //    "meshHenry");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin
            
            ////creating a new skindef as we did before
            //SkinDef masterySkin = Modules.Skins.CreateSkinDef(AMY_PREFIX + "MASTERY_SKIN_NAME",
            //    assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
            //    defaultRendererinfos,
            //    prefabCharacterModel.gameObject,
            //    HenryUnlockables.masterySkinUnlockableDef);

            ////adding the mesh replacements as above. 
            ////if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            //masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySwordAlt",
            //    null,//no gun mesh replacement. use same gun mesh
            //    "meshHenryAlt");

            ////masterySkin has a new set of RendererInfos (based on default rendererinfos)
            ////you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            //masterySkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[2].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");

            ////here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            //masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            //{
            //    new SkinDef.GameObjectActivation
            //    {
            //        gameObject = childLocator.FindChildGameObject("GunModel"),
            //        shouldActivate = false,
            //    }
            //};
            ////simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            //skins.Add(masterySkin);
            
            #endregion

            skinController.skins = skins.ToArray();
            displaySkinController.skins = skins.ToArray();
        }
        #endregion skins

        //Character Master is what governs the AI of your character when it is not controlled by a player (artifact of vengeance, goobo)
        public override void InitializeCharacterMaster()
        {
            //you must only do one of these. adding duplicate masters breaks the game.

            //if you're lazy or prototyping you can simply copy the AI of a different character to be used
            //Modules.Prefabs.CloneDopplegangerMaster(bodyPrefab, masterName, "Merc");

            //how to set up AI in code
            AmyAI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }
    }
}