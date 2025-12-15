using  AmyRoseMod.Characters.Survivors.Amy.Components;
using AmyRoseMod.Characters.Survivors.Amy.Content;
using EntityStates;
using HedgehogUtils;
using HedgehogUtils.Boost;
using RoR2;
using RoR2.Audio;
using RoR2.Orbs;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates.SuperFormUpgrades
{
    public class SuperMultiLockAttack : MultiLockAttack
    {
        public override Type nextStateType { get { return typeof(SuperMultiLockEnd); } }
        public override void OnEnter()
        {
            base.OnEnter();
            orbSpeed = AmyStaticValues.superSpecialMultiLockOrbSpeed;
        }
        
        public override void FireOrb()
        {
            if (!target) { return; }
            orb = AmyOrbs.CreateMultiLockOrb<AmyOrbs.MultiLockOrb>(AmyStaticValues.specialMultiLockDamageCoefficient * damageStat, base.gameObject, this.outer, Util.CheckRoll(this.critStat, base.characterBody.master), 
                AmyAssets.superMultiLockProjectilePrefab, orbSpeed, orbStartPosition, target, OrbStorageUtility.Get("Prefabs/Effects/OrbEffects/HuntressGlaiveOrbEffect"));
            OrbManager.instance.AddOrb(orb);
        }
    }
}