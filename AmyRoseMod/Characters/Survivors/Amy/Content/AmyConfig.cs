using AmyRoseMod.Characters.Survivors.Amy.SkillStates;
using AmyRoseMod.Modules;
using BepInEx.Configuration;
using static System.Collections.Specialized.BitVector32;

namespace AmyRoseMod.Characters.Survivors.Amy
{
    public static class AmyConfig
    {
        public static ConfigEntry<MultiLockCameraProvider.CameraMovementModes> multiLockSmoothCamera;

        public static void Init()
        {
            /*string section = "Amy";

            someConfigBool = Config.BindAndOptions(
                section,
                "someConfigBool",
                true,
                "this creates a bool config, and a checkbox option in risk of options");

            someConfigFloat = Config.BindAndOptions(
                section,
                "someConfigfloat",
                5f);//blank description will default to just the name

            someConfigFloatWithCustomRange = Config.BindAndOptions(
                section,
                "someConfigfloat2",
                5f,
                0,
                50,
                "if a custom range is not passed in, a float will default to a slider with range 0-20. risk of options only has sliders");*/
            multiLockSmoothCamera = Config.BindAndOptions<MultiLockCameraProvider.CameraMovementModes>(
                "Comfort",
                "Multi-Lock Camera",
                MultiLockCameraProvider.CameraMovementModes.MoveAndRotate,
                "Controls how the camera moves when using the Multi-Lock skill.\n\nChanging this config will not affect other players.\nDefault is true");
        }
    }
}
