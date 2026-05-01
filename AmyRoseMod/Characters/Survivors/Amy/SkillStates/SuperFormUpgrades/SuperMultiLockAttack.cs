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
            orbSpeed = AmyStaticValues.superSpecialMultiLockOrbSpeed;
            base.OnEnter();
        }
        protected override GameObject GetProjectilePrefab()
        {
            return AmyAssets.superMultiLockProjectilePrefab;
        }
        protected override void CreateOnHitVFX()
        {
            EffectManager.SimpleEffect(AmyAssets.superMultiLockHeartSpawnEffect, targetLastPosition, Quaternion.identity, false);
        }
    }
}