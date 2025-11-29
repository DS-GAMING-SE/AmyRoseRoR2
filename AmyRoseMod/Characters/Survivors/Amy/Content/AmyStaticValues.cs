using System;

namespace AmyRoseMod.Characters.Survivors.Amy
{
    public static class AmyStaticValues
    {
        #region Primary Hammer

        public const float primaryHammerDamageCoefficient = 3f;

        public const float primaryHammerLaunchForce = 250f;

        #region Super Primary Hammer

        public const float superPrimaryHammerAfterimageDamageCoefficient = primaryHammerDamageCoefficient / 2f;

        public const float superPrimaryHammerAfterimageProcCoefficient = 0;

        public const float superPrimaryHammerLaunchForce = 400f;

        #endregion

        #endregion

        #region Secondary Hammer Smash

        public const float secondaryHammerChargeMinimumDamageCoefficient = 8f;

        public const float secondaryHammerChargeMaximumDamageCoefficient = 24f;

        public const float secondaryHammerChargeMinimumProcCoefficient = 1f;

        public const float secondaryHammerChargeMaximumProcCoefficient = 1.5f;

        public const float secondaryHammerBaseChargeTime = 3f;

        public const float secondaryHammerLaunchForce = 400f;

        #region Air Version

        public const float secondaryHammerAirFallDistanceForMaxCharge = 45f;

        public const float secondaryHammerAirFallAcceleration = 80f;

        public const float secondaryHammerAirFallMaxSpeed = 100f;

        public const float secondaryHammerAirFallMaxFallDuration = 4f;

        public const float secondaryHammerAirJumpHeightMultiplier = 2.5f;

        public const float secondaryHammerAirJumpHeightReductionWhenAngled = 0.18f;

        public const float secondaryHammerAirJumpHorizontalSpeedMult = 2.3f;

        public const float secondaryHammerAirJumpBuffSpeedCoefficient = 0.2f;

        public const float secondaryHammerAirJumpBuffDuration = 3f;

        #endregion

        #region Super Secondary Hammer Smash
        public const float superSecondaryHammerLaunchForce = 10000f;
        #endregion

        #endregion

        #region Utility Boost

        public const float boostListedSpeedCoefficient = 0.35f;

        public const float boostArmor = 50;

        #region Super Utility Boost
        public const float superBoostListedSpeedCoefficient = 0.75f;
        #endregion

        #region Hammer-Spin

        public const float boostHammerSpinDamageCoefficient = 1.6f;

        public const float boostHammerSpinFastDamageCoefficient = 10f;

        public const float boostHammerSpinMinProcCoefficient = 0.6f;

        public const float boostHammerSpinMaxProcCoefficient = 1f;

        public const float boostHammerSpinLaunchForce = 250f;

        public const float boostHammerSpinAttacksPerSecond = 4f;

        public const float boostHammerSpinBuffMaxAttacksPerSecond = 0.6f;

        #region Buff
        public const float boostHammerSpinAccelerationBaseDivide = 4f;

        public const float boostHammerSpinAccelerationMaxDivide = 6f;

        public const float boostHammerSpinBuffMaxSpeedCoefficient = 0.5f;

        public const float boostHammerSpinBuffPerSecond = 0.7f;

        public const int boostHammerSpinBuffMaxEffectiveStacks = 5;

        public const int boostHammerSpinBuffPercentPerEffectiveStack = 100 / boostHammerSpinBuffMaxEffectiveStacks;
        #endregion

        public const float dizzyDuration = 3f;

        public const float boostHammerSpinEndLagBaseDuration = 0.7f;

        #region Super Hammer-Spin
        public const float superBoostHammerSpinLaunchForce = 400f;
        #endregion

        #endregion
        #endregion

        #region Special Multi-Lock

        public const float specialMultiLockDamageCoefficient = 11f;

        public const float specialMultiLockBlastRadius = 4f;

        public const float specialMultiLockDetonationTime = 1f;

        public const float specialMultiLockOrbSpeed = 900f;

        public const float specialMultiLockSearchRange = 47f;

        public const float specialMultiLockBounceRange = 60f;

        public const int specialMultiLockMaxTargets = 5;

        public const float specialMultiLockEndDuration = 0.9f;

        public const float specialMultiLockEndLingeringInvincibilityDuration = 0.4f;

        #region Super Special Multi-Lock

        public const float superSpecialMultiLockBlastRadius = 8f;

        public const float superSpecialMultiLockOrbSpeed = 140f;

        public const float superSpecialMultiLockSearchRange = 80f;

        public const float superSpecialMultiLockBounceRange = 100f;

        public const int superSpecialMultiLockMaxTargets = 10;


        #endregion

        #endregion
    }
}