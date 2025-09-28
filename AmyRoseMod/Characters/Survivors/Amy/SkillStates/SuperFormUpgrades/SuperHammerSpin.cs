using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.Networking;
using HedgehogUtils.Boost;
using  AmyRoseMod.Characters.Survivors.Amy;
using  AmyRoseMod.Characters.Survivors.Amy.Components;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates.SuperFormUpgrades
{
    public class SuperHammerSpin : HammerSpin, ISkillState
    {
        protected override BuffDef boostBuff => AmyBuffs.superBoostBuff;

    }
}