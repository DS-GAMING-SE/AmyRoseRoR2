using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.Networking;
using HedgehogUtils.Boost;

namespace Amy.Survivors.Amy.SkillStates
{
    public class BoostIdle : HedgehogUtils.Boost.EntityStates.BoostIdle
    {
        public SkillDef hammerSwingSkillDef;
        
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

                if (Boost.ApplySkillOverride(this, activatorSkillSlot, base.skillLocator, out SkillDef skillDef))
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
                Boost.RemoveSkillOverride(this, activatorSkillSlot, hammerSwingSkillDef, base.skillLocator);
            }
            base.OnExit();
        }
    }
}