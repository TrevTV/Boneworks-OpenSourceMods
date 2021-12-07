using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Pool;
using StressLevelZero.Props.Weapons;
using System.Collections.Generic;
using System.Linq;

namespace BoltUnlocker
{
    public static class BuildInfo
    {
        public const string Name = "BoltUnlocker";
        public const string Author = "trev";
        public const string Company = null;
        public const string Version = "1.0.1";
        public const string DownloadLink = null;
    }

    // this code is the opposite of professional but i dont give a fuck
    public class Core : MelonMod
    {
        public static MelonPreferences_Entry<string[]> doShitTo;
        public static List<string> poolsToDoShitTo;

        public override void OnApplicationStart()
        {
            MelonPreferences_Category category = MelonPreferences.CreateCategory("BoltUnlocker");
            doShitTo = category.CreateEntry("DoTheThingTo", new string[] { "Gun names here", "CaSe SeNsItIvE" });
            poolsToDoShitTo = new List<string>();
            HarmonyInstance.Patch(typeof(Gun).GetMethod("CompleteSlidePull"), null, typeof(Core).GetMethod(nameof(OnCompleteSlidePull)).ToNewHarmonyMethod());
            Hooking.OnGrabObject += Hooking_OnGrabObject;
            MelonPreferences.Save();
        }

        private void Hooking_OnGrabObject(UnityEngine.GameObject obj, StressLevelZero.Interaction.Hand hand)
        {
            // i love modding :thumbsup:
            if (obj == null) return;
            Poolee poolee = obj.GetComponentInParent<Poolee>();
            if (poolee == null) return;
            if (poolee.pool == null) return;
            if (poolee.pool.Prefab == null) return;

            if (!poolsToDoShitTo.Contains(poolee.pool.Prefab.name))
            {
                MelonLogger.Msg("checking pool of prefab: " + poolee.pool.Prefab.name);
                if (doShitTo.Value.Contains(poolee.pool.Prefab.name))
                    poolsToDoShitTo.Add(poolee.pool.Prefab.name);
            }
        }

        public static void OnCompleteSlidePull(Gun __instance)
        {
            //MelonLogger.Msg("hi this fucker just did a thing: " + __instance.name);
            if (poolsToDoShitTo.Contains(SimpleHelpers.GetCleanObjectName(__instance.name))
                && __instance.chamberedCartridge == null && !__instance._isSlideGrabbed)
            {
                //MelonLogger.Msg("doing the thing :thumbsup:");
                __instance.isFiring = false;
                __instance.SlideRelease();
            }
        }
    }
}
