using AmyRoseMod.Modules.BaseStates;
using AmyRoseMod.Characters.Survivors.Amy;
using AmyRoseMod.Characters.Survivors.Amy.SkillStates;
using AmyRoseMod.Characters.Survivors.Amy.Content;
using EntityStates;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates.SuperFormUpgrades
{
    public class SuperHammerSmashCharge : HammerSmashCharge
    {
        
        protected override void SetNextStateToSmash()
        {
            if (base.characterMotor.isGrounded || characterMotor.isFlying)
            {
                HammerSmashGrounded state = (HammerSmashGrounded)EntityStateCatalog.InstantiateState(typeof(SuperHammerSmashGrounded));
                if (state != null)
                {
                    state.charge = charge;
                }
                this.outer.SetNextState(state);
            }
            else
            {
                HammerSmashChargeAerial state = (HammerSmashChargeAerial)EntityStateCatalog.InstantiateState(typeof(HammerSmashChargeAerial));
                if (state != null)
                {
                    state.charge = charge;
                }
                this.outer.SetNextState(state);
            }
        }
        protected override void ReachedMaxCharge()
        {
            EffectManager.SimpleMuzzleFlash(AmyAssets.superSecondaryChargedEffect, base.gameObject, "Head", false);
            Util.PlaySound("Play_amyrose_hammer_smash_charged", base.gameObject);
        }
    }
}
