using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.Networking;
using HedgehogUtils.Boost;
using Amy.Survivors.Amy;
using Amy.Survivors.Amy.Components;

namespace Amy.Survivors.Amy.SkillStates
{
    public class Boost : HedgehogUtils.Boost.EntityStates.Boost, ISkillState
    {
        protected override BuffDef buff => AmyBuffs.boostBuff;

        public AmyHammerSpinController hammerSpinController;
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
                hammerSpinController = base.GetComponent<AmyHammerSpinController>();
                if (hammerSpinController)
                {
                    hammerSpinController.ApplySkillOverride(activatorSkillSlot, out SkillDef skillDef);
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
    }
}