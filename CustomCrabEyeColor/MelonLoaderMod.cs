using MelonLoader;
using UnityEngine;
using HarmonyLib;

namespace CustomCrabEyeColor
{
    public static class BuildInfo
    {
        public const string Name = "CustomCrabEyeColor";
        public const string Author = "trev";
        public const string Company = null;
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }

    public class CustomCrabEyeColor : MelonMod
    {
        private static Color baseColor;
        private static Color agroColor;

        private static Color defaultBase = new Color(0.146f, 2.049f, 2.54f, 1);
        private static Color defaultArgo = new Color(2.54f, 0.31f, 0.143f, 1);

        public override void OnApplicationStart()
        {
            MelonPreferences.CreateCategory("CustomCrabEyeColor");
            MelonPreferences.CreateEntry("CustomCrabEyeColor", "BaseColor", ColorToPrefString(defaultBase));
            MelonPreferences.CreateEntry("CustomCrabEyeColor", "AgroColor", ColorToPrefString(defaultArgo));

            string[] colorRgb = MelonPreferences.GetEntryValue<string>("CustomCrabEyeColor", "BaseColor").Split(',');
            baseColor = ValueArrayToColor(colorRgb, defaultBase);

            colorRgb = MelonPreferences.GetEntryValue<string>("CustomCrabEyeColor", "AgroColor").Split(',');
            agroColor = ValueArrayToColor(colorRgb, defaultArgo);

            HarmonyInstance.Patch(typeof(StressLevelZero.AI.AIBrain).GetMethod("Awake"), new HarmonyMethod(typeof(CustomCrabEyeColor).GetMethod("Initiate")));
        }

        public static void Initiate(StressLevelZero.AI.AIBrain __instance)
        {
            if (__instance == null 
                || __instance.behaviour == null 
                || !__instance.gameObject.name.Contains("Crablet")) return;

            __instance.behaviour.baseColor = baseColor;
            __instance.behaviour.agroColor = agroColor;
        }

        public Color ValueArrayToColor(string[] arr, Color defaultColor)
        {
            if (arr.Length < 3)
            {
                MelonLogger.Msg("Color array didn't contain 3 values, using default");
                return defaultColor;
            }

            return new Color(StrToFloat(arr[0]), StrToFloat(arr[1]), StrToFloat(arr[2]), 255);
        }

        public float StrToFloat(string str)
            => float.Parse(str, System.Globalization.CultureInfo.InvariantCulture);

        public string ColorToPrefString(Color color)
            => $"{color.r},{color.g},{color.b}";
    }
}
