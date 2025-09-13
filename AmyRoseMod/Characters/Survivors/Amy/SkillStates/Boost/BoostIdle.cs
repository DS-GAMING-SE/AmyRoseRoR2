using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.Networking;
using HedgehogUtils.Boost;
using Amy.Survivors.Amy.Components;

namespace Amy.Survivors.Amy.SkillStates
{
    public class BoostIdle : HedgehogUtils.Boost.EntityStates.BoostIdle
    {
        public SkillDef hammerSwingSkillDef;
        public AmyHammerSpinController hammerSpinController;
        
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

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.inputBank && hammerSpinController && base.skillLocator && base.skillLocator.primary)
            {
                if (inputBank.skill1.justPressed)
                {
                    base.skillLocator.primary.ExecuteIfReady();
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
    }
}