using AmyRoseMod.Characters.Survivors.Amy.Content;
using RoR2;
using RoR2.Orbs;
using UnityEngine;


namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates.ScepterUpgrades
{
    public class ScepterMultiLockAttack : MultiLockAttack
    {
        protected override GameObject GetProjectilePrefab()
        {
            return AmyAssets.scepterMultiLockProjectilePrefab;
        }
    }
}