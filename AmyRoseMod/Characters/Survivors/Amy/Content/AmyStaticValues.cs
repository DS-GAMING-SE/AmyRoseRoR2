using System;

namespace AmyRoseMod.Characters.Survivors.Amy
{
    public static class AmyStaticValues
    {
        public const float bombDamageCoefficient = 16f;
        #region Primary Hammer

        public const float primaryHammerDamageCoefficient = 3f;

        public const float primaryHammerLaunchForce = 250f;

        #endregion

        #region Secondary Hammer Smash

        public const float secondaryHammerChargeMinimumDamageCoefficient = 7f;

        public const float secondaryHammerChargeMaximumDamageCoefficient = 14f;

        public const float secondaryHammerChargeMinimumProcCoefficient = 1f;

        public const float secondaryHammerChargeMaximumProcCoefficient = 1.5f;

        public const float secondaryHammerBaseChargeTime = 2.5f;

        public const float secondaryHammerLaunchForce = 400f;

        #region Air Version

        public const float secondaryHammerAirFallDistanceForMaxCharge = 33f;

        public const float secondaryHammerAirFallAcceleration = 60f;

        public const float secondaryHammerAirFallMaxSpeed = 80f;

        public const float secondaryHammerAirFallMaxFallDuration = 4f;

        public const float secondaryHammerAirJumpHeightMultiplier = 2.5f;

        public const float secondaryHammerAirJumpMaxLerpFromUp = 0.18f;

        public const float secondaryHammerAirJumpBuffSpeedCoefficient = 0.2f;

        public const float secondaryHammerAirJumpBuffDuration = 3f;


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

        public const float boostHammerSpinFastDamageCoefficient = 7f;

        public const float boostHammerSpinProcCoefficient = 0.6f;

        public const float boostHammerSpinLaunchForce = 250f;

        public const float boostHammerSpinAttacksPerSecond = 4f;

        public const float boostHammerSpinFastAttacksPerSecond = 0.6f;

        public const float boostHammerSpinAccelerationBaseDivide = 3.5f;

        public const float boostHammerSpinAccelerationStackDivide = 0.75f;

        public const float boostHammerSpinBuffSpeedCoefficient = 0.1f;

        public const float boostHammerSpinBuffPerSecond = 0.7f;

        public const int boostHammerSpinBuffMaxStacks = 6;

        public const float dizzyDuration = 3f;

        public const float boostHammerSpinEndLagBaseDuration = 0.7f;

        #endregion
        #endregion

        #region Special Multi-Lock

        public const float specialMultiLockDamageCoefficient = 8f;

        public const float specialMultiLockBlastRadius = 4f;

        public const float specialMultiLockDetonationTime = 1f;

        public const int specialMultiLockMaxTargets = 5;

        public const float specialMultiLockEndDuration = 0.9f;

        public const float specialMultiLockEndLingeringInvincibilityDuration = 0.4f;

        #endregion
    }
}