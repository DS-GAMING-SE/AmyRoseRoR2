using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.Networking;
using HedgehogUtils.Boost;
using AmyRoseMod.Characters.Survivors.Amy;
using AmyRoseMod.Characters.Survivors.Amy.Components;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates.SuperFormUpgrades
{
    public class SuperBoost : Boost, ISkillState
    {
        protected override BuffDef buff => AmyBuffs.superBoostBuff;

        public override GameObject GetFlashPrefab()
        {
            return AmyAssets.superAmyBoostFlashEffect;
        }

        public override GameObject GetAuraPrefab()
        {
            return AmyAssets.superAmyBoostAuraEffect;
        }

        public override Material GetOverlayMaterial()
        {
            return LegacyResourcesAPI.Load<Material>("Materials/matStrongerBurn");
        }
    }
}