using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using Amy;
using Amy.Survivors.Amy;

namespace AmyRoseMod.Modules
{
    internal class RecalculateStats
    {
        public static void Initialize()
        {
            RecalculateStatsAPI.GetStatCoefficients += AmyRecalculateStats;
        }
        private static void AmyRecalculateStats(CharacterBody self, RecalculateStatsAPI.StatHookEventArgs stats)
        {
            if (self)
            {
                if (self.HasBuff(AmyBuffs.boostBuff))
                {
                    HedgehogUtils.Boost.BoostLogic.BoostStats(self, stats, AmyStaticValues.boostListedSpeedCoefficient);
                    stats.armorAdd += AmyStaticValues.boostArmor;
                }

                if (self.HasBuff(AmyBuffs.hammerSmashSpeedBuff))
                {
                    stats.moveSpeedMultAdd += AmyStaticValues.secondaryHammerAirJumpBuffSpeedCoefficient;
                }

                if (self.HasBuff(AmyBuffs.dizzyDebuff))
                {
                    stats.armorAdd -= AmyStaticValues.dizzyDebuffArmorReduction;
                }
            }
        }
    }
}
