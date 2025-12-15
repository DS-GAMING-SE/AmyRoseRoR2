using AmyRoseMod.Characters.Survivors.Amy.Content;
using RoR2;
using RoR2.Orbs;
using AmyRoseMod.Characters.Survivors.Amy.SkillStates.SuperFormUpgrades;

namespace AmyRoseMod.Characters.Survivors.Amy.SkillStates.ScepterUpgrades
{
    public class ScepterSuperMultiLockAttack : SuperMultiLockAttack
    {
        public override void FireOrb()
        {
            if (!target) { return; }
            orb = AmyOrbs.CreateMultiLockOrb<AmyOrbs.MultiLockOrb>(AmyStaticValues.specialMultiLockDamageCoefficient * damageStat, base.gameObject, this.outer, Util.CheckRoll(this.critStat, base.characterBody.master), 
                AmyAssets.scepterSuperMultiLockProjectilePrefab, orbSpeed, orbStartPosition, target, OrbStorageUtility.Get("Prefabs/Effects/OrbEffects/HuntressGlaiveOrbEffect"));
            OrbManager.instance.AddOrb(orb);
        }
    }
}