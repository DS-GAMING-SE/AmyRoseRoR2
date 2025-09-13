using RoR2;
using UnityEngine;
using R2API;
using RoR2.Skills;

namespace Amy.Survivors.Amy.Components
{
    public class AmyHammerSpinController : MonoBehaviour
    {
        public Transform modelTransform;
        public Transform modelParentTransform;

        public ModelLocator modelLocator;

        public bool leanActive = false;
        public bool leanFrozen;

        public Vector3 currentNormal;
        public Vector3 targetNormal;
        private Vector3 normalSmoothDampVelocity;

        private CharacterBody characterBody;
        private EntityStateMachine bodyState;

        private const float maxAngleLean = 18f;

        protected SkillDef currentSkillDefOverride;

        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
        }

        public void Start()
        {
            bodyState = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
        }

        public void ActivateLean()
        {
            if (modelLocator)
            {
                modelLocator.autoUpdateModelTransform = false;
            }
            currentNormal = modelLocator.currentNormal;
            leanActive = true;
        }

        public void DeactivateLean()
        {
            if (modelLocator)
            {
                modelLocator.autoUpdateModelTransform = true;
            }
            modelLocator.currentNormal = currentNormal;
            leanActive = false;
        }

        public void FixedUpdate()
        {
            if (currentSkillDefOverride && !ShouldHaveSkillOverride())
            {
                RemoveSkillOverride();
            }
        }

        public void LateUpdate()
        {
            if (leanActive)
            {
                UpdateModelTransform();
            }
        }

        private void UpdateModelTransform()
        {
            Vector3 position = this.modelParentTransform.position;
            Quaternion quaternion = this.modelParentTransform.rotation;
            
            if (!leanFrozen)
            {
                if (characterBody && characterBody.characterMotor)
                {
                    float velocityReachedMovementSpeed = Mathf.Clamp01(characterBody.characterMotor.velocity.magnitude / (characterBody.moveSpeed * 0.7f));
                    targetNormal = Vector3.RotateTowards(Vector3.up, characterBody.characterMotor.velocity.normalized, Mathf.Lerp(0, maxAngleLean * 0.017453292f, velocityReachedMovementSpeed), float.PositiveInfinity);
                }
                else
                {
                    targetNormal = Vector3.up;
                }

                SmoothNormals(Time.deltaTime);
            }
            
            quaternion = Quaternion.FromToRotation(Vector3.up, this.currentNormal) * quaternion;
            this.modelTransform.SetPositionAndRotation(position, quaternion);
        }

        private void SmoothNormals(float deltaTime)
        {
            this.currentNormal = Vector3.SmoothDamp(this.currentNormal, this.targetNormal, ref this.normalSmoothDampVelocity, 0.2f, float.PositiveInfinity, deltaTime);
        }

        public bool ApplySkillOverride(GenericSkill activatorSkillSlot, out SkillDef hammerSwingSkillDef)
        {
            hammerSwingSkillDef = null;
            if (activatorSkillSlot && activatorSkillSlot.skillDef is AmySkillDefs.IAmyBoost)
            {
                hammerSwingSkillDef = (activatorSkillSlot.skillDef as AmySkillDefs.IAmyBoost).hammerSpinSkillDef;
                if (currentSkillDefOverride)
                {
                    if (currentSkillDefOverride == hammerSwingSkillDef)
                    {
                        return true;
                    }
                    else
                    {
                        RemoveSkillOverride();
                    }
                }
                if (hammerSwingSkillDef && characterBody.skillLocator)
                {
                    currentSkillDefOverride = hammerSwingSkillDef;
                    characterBody.skillLocator.primary.SetSkillOverride(this, hammerSwingSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveSkillOverride()
        {
            if (currentSkillDefOverride && characterBody.skillLocator)
            {
                characterBody.skillLocator.primary.UnsetSkillOverride(this, currentSkillDefOverride, GenericSkill.SkillOverridePriority.Contextual);
                currentSkillDefOverride = null;
                return true;
            }
            return false;
        }

        public bool ShouldHaveSkillOverride()
        {
            if (characterBody && characterBody.skillLocator && characterBody.skillLocator.utility && characterBody.skillLocator.utility.skillDef is AmySkillDefs.IAmyBoost)
            {
                AmySkillDefs.IAmyBoost amyDef = characterBody.skillLocator.utility.skillDef as AmySkillDefs.IAmyBoost;
                HedgehogUtils.Boost.SkillDefs.IBoostSkill boostDef = characterBody.skillLocator.utility.skillDef as HedgehogUtils.Boost.SkillDefs.IBoostSkill;
                return bodyState && 
                    (characterBody.skillLocator.utility.skillDef.activationState.stateType == bodyState.state.GetType() ||
                    amyDef.hammerSpinSkillDef.activationState.stateType == bodyState.state.GetType() ||
                    boostDef.boostIdleState.stateType == bodyState.state.GetType());
            }
            return false;
        }
    }
}