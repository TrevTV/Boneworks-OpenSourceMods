using MelonLoader;
using StressLevelZero.Props.Weapons;

namespace AmmoHUDToggler
{
    public static class BuildInfo
    {
        public const string Name = "hud_remover";
        public const string Author = "trev";
        public const string Company = null;
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }

    public class AmmoHUDToggler : MelonMod
    {
        public override void OnApplicationStart()
        {
            HarmonyInstance.Patch(typeof(Gun).GetMethod("Awake"), null, new HarmonyLib.HarmonyMethod(typeof(AmmoHUDToggler).GetMethod("GunPatch")));
        }

        public static void GunPatch(Gun __instance)
        {
            __instance.chargingHandleHelperRenderer = null;
            __instance.magazineHelperRenderer = null;
        }

        public override void OnSceneWasInitialized(int buildIndex, string levelName)
        {
            if (ModThatIsNotMod.Player.GetRigManager() != null)
            ModThatIsNotMod.Player.GetRigManager().transform.Find("[UIRig]").Find("PLAYERUI").Find("Hud").gameObject.SetActive(false);
        }
    }
}
