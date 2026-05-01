using AmyRoseMod.Characters.Survivors.Amy.Content;
using AmyRoseMod.Characters.Survivors.Amy.SkillStates.SuperFormUpgrades;
using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates.ScepterUpgrades
{
    public class ScepterSuperMultiLockAttack : SuperMultiLockAttack
    {
        protected override GameObject GetProjectilePrefab()
        {
            return AmyAssets.scepterSuperMultiLockProjectilePrefab;
        }
    }
}