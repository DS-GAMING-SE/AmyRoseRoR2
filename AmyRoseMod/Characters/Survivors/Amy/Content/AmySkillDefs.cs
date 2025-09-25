using EntityStates;
using HedgehogUtils.Boost;
using HedgehogUtils.Boost.EntityStates;
using HedgehogUtils.Forms;
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static HedgehogUtils.Forms.SkillDefs;

namespace Amy.Survivors.Amy
{
    public static class AmySkillDefs
    {
        public interface IAmyBoost
        {
            SkillDef hammerSpinSkillDef { get; set; }
        }
        public class AmyBoostSkillDef : HedgehogUtils.Boost.SkillDefs.BoostSkillDef, IAmyBoost
        {
            public SkillDef hammerSpinSkillDef { get; set; }
        }

        public class AmyRequiresFormBoostSkillDef : HedgehogUtils.Boost.SkillDefs.RequiresFormBoostSkillDef, IAmyBoost
        {
            public SkillDef hammerSpinSkillDef { get; set; }
        }

        public class RequiresFormSteppedSkillDef : SteppedSkillDef
        {
            public FormDef requiredForm;

            private GenericSkill skillSlot;
            private object source;
            private GenericSkill.SkillOverridePriority priority;

            public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
            {
                RequiresFormSteppedSkillDefInstanceData formInstance = new RequiresFormSteppedSkillDefInstanceData
                {
                    formComponent = skillSlot.GetComponent<FormComponent>()
                };

                this.skillSlot = skillSlot;
                this.source = skillSlot.skillOverrides[skillSlot.currentSkillOverride].source;
                this.priority = skillSlot.skillOverrides[skillSlot.currentSkillOverride].priority;

                if (formInstance.formComponent.activeForm != requiredForm)
                {
                    skillSlot.UnsetSkillOverride(source, this, priority);
                }
                else
                {
                    formInstance.formComponent.OnFormChanged += OnFormChanged;
                }
                return formInstance;
            }
            public override void OnUnassigned([NotNull] GenericSkill skillSlot)
            {
                if (skillSlot.skillInstanceData != null && ((RequiresFormSteppedSkillDefInstanceData)skillSlot.skillInstanceData).formComponent)
                {
                    ((RequiresFormSteppedSkillDefInstanceData)skillSlot.skillInstanceData).formComponent.OnFormChanged -= OnFormChanged;
                }
            }

            public void OnFormChanged(FormDef previous, FormDef newForm)
            {
                if (newForm != requiredForm)
                {
                    skillSlot.UnsetSkillOverride(source, this, priority);
                }
            }

            public class RequiresFormSteppedSkillDefInstanceData : SteppedSkillDef.InstanceData
            {
                public FormComponent formComponent;
            }
        }
    }
}
