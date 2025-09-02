using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.Networking;
using HedgehogUtils.Boost;
using Amy.Survivors.Amy;

namespace Amy.Survivors.Amy.SkillStates
{
    public class Boost : HedgehogUtils.Boost.EntityStates.Boost, ISkillState
    {
        protected override BuffDef buff => AmyBuffs.boostBuff;
        public GenericSkill activatorSkillSlot { get; set; }
        protected SkillDef hammerSwingSkillDef;
        public override void OnEnter()
        {
            base.OnEnter();
            if (base.modelLocator)
            {
                modelLocator.normalizeToFloor = true;
            }

            if (base.isAuthority)
            {
                EntityStateMachine weaponState = EntityStateMachine.FindByCustomName(base.gameObject, "Weapon");
                if (weaponState) { weaponState.SetNextStateToMain(); }

                if (ApplySkillOverride(this, activatorSkillSlot, base.skillLocator, out SkillDef skillDef))
                {
                    hammerSwingSkillDef = skillDef;
                }
            }
        }

        public override void OnExit()
        {
            if (base.modelLocator)
            {
                modelLocator.normalizeToFloor = false;
            }
            if (base.isAuthority)
            {
                RemoveSkillOverride(this, activatorSkillSlot, hammerSwingSkillDef, base.skillLocator);
            }
            base.OnExit();
        }

        public override bool CanExecuteSkill(GenericSkill skillSlot)
        {
            return skillSlot && skillSlot.skillDef && skillSlot.skillDef.activationStateMachineName != "Weapon";
        }
        public override GameObject GetFlashPrefab()
        {
            return AmyAssets.amyBoostFlashEffect;
        }

        public override GameObject GetAuraPrefab()
        {
            return AmyAssets.amyBoostAuraEffect;
        }

        public override Material GetOverlayMaterial()
        {
            return null;
        }

        public static bool ApplySkillOverride(object source, GenericSkill activatorSkillSlot, SkillLocator skillLocator, out SkillDef hammerSwingSkillDef)
        {
            hammerSwingSkillDef = null;
            if (activatorSkillSlot && activatorSkillSlot.skillDef is AmySkillDefs.AmyBoostSkillDef)
            {
                hammerSwingSkillDef = (activatorSkillSlot.skillDef as AmySkillDefs.AmyBoostSkillDef).hammerSwingSkillDef;
                if (hammerSwingSkillDef && skillLocator)
                {
                    skillLocator.primary.SetSkillOverride(source, hammerSwingSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                    return true;
                }
            }
            return false;
        }

        public static bool RemoveSkillOverride(object source, GenericSkill activatorSkillSlot, SkillDef hammerSwingSkillDef, SkillLocator skillLocator)
        {
            if (hammerSwingSkillDef && skillLocator)
            {
                skillLocator.primary.UnsetSkillOverride(source, hammerSwingSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                return true;
            }
            return false;
        }
    }
}