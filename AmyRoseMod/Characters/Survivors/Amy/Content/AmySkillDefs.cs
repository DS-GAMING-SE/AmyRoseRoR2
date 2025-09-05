using EntityStates;
using HedgehogUtils.Boost.EntityStates;
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HedgehogUtils.Boost;

namespace Amy.Survivors.Amy
{
    public static class AmySkillDefs
    {
        public interface IAmyBoost
        {
            SkillDef hammerSwingSkillDef { get; set; }
        }
        public class AmyBoostSkillDef : HedgehogUtils.Boost.SkillDefs.BoostSkillDef, IAmyBoost
        {
            public SkillDef hammerSwingSkillDef { get; set; }
        }

        public class AmyRequiresFormBoostSkillDef : HedgehogUtils.Boost.SkillDefs.RequiresFormBoostSkillDef, IAmyBoost
        {
            public SkillDef hammerSwingSkillDef { get; set; }
        }

        public interface IAmyHammerSmash
        {
            SerializableEntityStateType aerialActivationState { get; set; }
        }

        public class AmyHammerSmashSkillDef : SkillDef, IAmyHammerSmash
        {
            public SerializableEntityStateType aerialActivationState { get; set; }

            public override EntityState InstantiateNextState([NotNull] GenericSkill skillSlot)
            {
                return DetermineNextState(skillSlot, activationState, aerialActivationState);
            }

            public static EntityState DetermineNextState(GenericSkill skillSlot, SerializableEntityStateType grounded, SerializableEntityStateType aerial)
            {
                if (skillSlot.characterBody.characterMotor.isGrounded || HedgehogUtils.Helpers.Flying(skillSlot.gameObject, out _))
                {
                    EntityState entityState = EntityStateCatalog.InstantiateState(ref grounded);
                    ISkillState skillState = entityState as ISkillState;
                    if (skillState != null)
                    {
                        skillState.activatorSkillSlot = skillSlot;
                    }
                    return entityState;
                }
                else
                {
                    EntityState entityState = EntityStateCatalog.InstantiateState(ref aerial);
                    ISkillState skillState = entityState as ISkillState;
                    if (skillState != null)
                    {
                        skillState.activatorSkillSlot = skillSlot;
                    }
                    return entityState;
                }
            }
        }
        public class AmyRequiresFormHammerSmashSkillDef : HedgehogUtils.Forms.SkillDefs.RequiresFormSkillDef, IAmyHammerSmash
        {
            public SerializableEntityStateType aerialActivationState { get; set; }

            public override EntityState InstantiateNextState([NotNull] GenericSkill skillSlot)
            {
                return AmyHammerSmashSkillDef.DetermineNextState(skillSlot, activationState, aerialActivationState);
            }
        }
    }
}
