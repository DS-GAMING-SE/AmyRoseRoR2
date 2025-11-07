using AmyRoseMod.Modules.BaseStates;
using AmyRoseMod.Characters.Survivors.Amy.Content;
using AmyRoseMod.Characters.Survivors.Amy.SkillStates;
using EntityStates;
using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates.SuperFormUpgrades
{
    public class SuperHammerSmashGrounded : HammerSmashGrounded
    {
        protected override void PrepareAttackStats()
        {
            base.PrepareAttackStats();
            pushForce = AmyStaticValues.superSecondaryHammerLaunchForce;
        }
    }
}