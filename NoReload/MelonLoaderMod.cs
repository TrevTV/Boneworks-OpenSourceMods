using HarmonyLib;
using MelonLoader;
using ModThatIsNotMod.BoneMenu;
using StressLevelZero.Props.Weapons;

namespace NoReload
{
    public static class BuildInfo
    {
        public const string Name = "NoReload";
        public const string Author = "trev";
        public const string Company = null;
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }

    public class NoReload : MelonMod
    {
        public static bool Enabled = true;

        public override void OnApplicationStart()
        {
            MenuCategory i = MenuManager.CreateCategory("NoReload", UnityEngine.Color.white);
            i.CreateBoolElement("Enabled", UnityEngine.Color.white, true, (x) => Enabled = x);
        }
    }

    [HarmonyPatch(typeof(Magazine), "GetNextBullet")]
    static class BulletPatch
    {
        static void Prefix(Magazine __instance)
        {
            if (NoReload.Enabled) __instance.ResetMagazine();
        }
    }
}
