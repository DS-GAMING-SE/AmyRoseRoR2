using UnityEngine;
using HedgehogUtils.Forms;
using HedgehogUtils.Forms.SuperForm;
using RoR2.Skills;
using RoR2;

namespace Amy.Survivors.Amy.Components
{
    internal class AmySuperFormComponent : MonoBehaviour
    {
        public CharacterBody characterBody;
        public FormComponent formComponent;

        public static AmySkillDefs.RequiresFormSteppedSkillDef superPrimaryMelee;
        public static SkillDefs.RequiresFormSkillDef superSecondarySmash;
        public static AmySkillDefs.AmyRequiresFormBoostSkillDef superUtilityBoost;
        public static SkillDefs.RequiresFormSkillDef superSpecialMultiLock;

        private void Awake()
        {
            formComponent = GetComponent<FormComponent>();
            characterBody = GetComponent<CharacterBody>();
        }

        public void Start()
        {
            if (formComponent && characterBody)
            {
                formComponent.OnFormChanged += OnFormChanged;
            }
        }

        public void OnFormChanged(FormDef previous, FormDef current)
        {
            if (characterBody && characterBody.hasAuthority && characterBody.skillLocator && current == SuperFormDef.superFormDef)
            {
                SetSkillOverride(characterBody.skillLocator.primary, AmySurvivor.primaryMelee, superPrimaryMelee);
                SetSkillOverride(characterBody.skillLocator.secondary, AmySurvivor.secondarySmash, superSecondarySmash);
                SetSkillOverride(characterBody.skillLocator.utility, AmySurvivor.utilityBoost, superUtilityBoost);
                SetSkillOverride(characterBody.skillLocator.special, AmySurvivor.specialMultilock, superSpecialMultiLock);
            }
        }

        public void SetSkillOverride(GenericSkill skillSlot, SkillDef skillDefToReplace, SkillDef superSkill)
        {
            if (skillSlot)
            {
                if (skillSlot.baseSkill == skillDefToReplace)
                {
                    skillSlot.SetSkillOverride(this, superSkill, GenericSkill.SkillOverridePriority.Upgrade);
                }
            }
        }
    }
}