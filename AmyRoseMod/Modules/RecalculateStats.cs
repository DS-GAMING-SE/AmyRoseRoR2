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
            On.RoR2.CharacterBody.RecalculateStats += RecalcStatAPIDoesntHaveAcceleration;
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

                if (self.HasBuff(AmyBuffs.hammerSpinSpeedBuff))
                {
                    stats.moveSpeedMultAdd += self.GetBuffCount(AmyBuffs.hammerSpinSpeedBuff) * AmyStaticValues.boostHammerSpinBuffSpeedCoefficient;
                }

                if (self.HasBuff(AmyBuffs.dizzyDebuff))
                {
                    stats.armorAdd -= AmyStaticValues.dizzyDebuffArmorReduction;
                }
            }
        }
        private static void RecalcStatAPIDoesntHaveAcceleration(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self.HasBuff(AmyBuffs.hammerSpinSpeedBuff))
            {
                self.acceleration /= AmyStaticValues.boostHammerSpinAccelerationDivide;
            }
        }
    }
}
