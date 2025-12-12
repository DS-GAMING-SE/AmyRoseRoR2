using AmyRoseMod.Characters.Survivors.Amy;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using Unity.Collections;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates.SuperFormUpgrades
{
    public class SuperMultiLockEnd : MultiLockEnd
    {
        protected override void PrepareStats()
        {
            base.PrepareStats();
            startSpeed = 25f;
        }
    }
}
