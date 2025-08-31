using System;

namespace Amy.Survivors.Amy
{
    public static class AmyStaticValues
    {
        public const float gunDamageCoefficient = 4.2f;

        public const float bombDamageCoefficient = 16f;
        #region Primary Hammer
        public const float primaryHammerDamageCoefficient = 3f;

        public const float primaryHammerLaunchForce = 250f;
        #endregion

        #region Secondary Hammer Smash
        public const float secondaryHammerChargeMinimumDamageCoefficient = 6.5f;

        public const float secondaryHammerChargeMaximumDamageCoefficient = 10f;

        public const float secondaryHammerChargeMinimumProcCoefficient = 1f;

        public const float secondaryHammerChargeMaximumProcCoefficient = 1.5f;

        public const float secondaryHammerBaseChargeTime = 2.5f;

        public const float secondaryHammerLaunchForce = 400f;
        #endregion

        #region Utility Boost
        public const float boostListedSpeedCoefficient = 0.35f;

        public const float boostArmor = 50;
        #endregion

        #region Utility Boost Hammer-Spin
        public const float boostHammerSpinDamageCoefficient = 1.2f;
        #endregion

        #region Special Multi-Lock
        public const float specialMultiLockDamageCoefficient = 8f;

        public const int specialMultiLockMaxTargets = 5;
        #endregion
    }
}