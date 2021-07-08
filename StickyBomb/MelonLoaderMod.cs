using MelonLoader;
using StressLevelZero.Pool;
using System;
using System.IO;
using UnityEngine;
using WNP78.Grenades;
using HarmonyLib;
using ModThatIsNotMod;
using StressLevelZero.Interaction;

namespace StickyBomb
{
    public static class BuildInfo
    {
        public const string Name = "StickyBomb";
        public const string Author = "trev";
        public const string Company = null;
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }

    public class StickyBomb : MelonMod
    {
        public GameObject detonator;

        public override void OnApplicationStart()
        {
            UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<Detonator>();
            UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<StickyBombHandler>();

            Hooking.OnGrabObject += OnPlayerGrabObject;

            // todo: the harmony patches here dont work due to il2cpp mono weirdness
            LoadAssets();
            HarmonyInstance.Patch(AccessTools.Method(typeof(Grenade), "Start"), null, new HarmonyMethod(typeof(StickyBomb).GetMethod("GrenadeStart")));
            HarmonyInstance.Patch(AccessTools.Method(typeof(Grenade), "Reset"), null, new HarmonyMethod(typeof(StickyBomb).GetMethod("GrenadeReset")));
        }

        public void LoadAssets()
        {
            MemoryStream memoryStream;
            using (Stream stream = Assembly.GetManifestResourceStream("StickyBomb.det"))
            {
                memoryStream = new MemoryStream((int)stream.Length);
                stream.CopyTo(memoryStream);
            }
            AssetBundle bundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());
            detonator = bundle.LoadAsset("Assets/_Grenade/Build/Detonator.prefab").Cast<GameObject>();
            detonator.hideFlags = HideFlags.DontUnloadUnusedAsset;
            CustomItems.FixObjectShaders(detonator);

            detonator.transform.Find("ScaleFix").Find("ButtonCollider").gameObject.AddComponent<Detonator>();
            SpawnMenu.AddItem(CustomItems.CreateSpawnableObject(detonator, "Detonator", StressLevelZero.Data.CategoryFilters.GADGETS, PoolMode.REUSEOLDEST, 2));
        }

        public static void GrenadeStart(Grenade __instance)
        {
            StickyBombHandler handler = __instance.GetComponent<StickyBombHandler>();
            if (__instance.GetComponent<Poolee>().pool.name == "pool - Pipe Bomb" && !handler)
                __instance.gameObject.AddComponent<StickyBombHandler>();
        }

        public static void GrenadeReset(Grenade __instance)
        {
            StickyBombHandler handler = __instance.GetComponent<StickyBombHandler>();
            if (handler)
                handler.Disconnect();
        }

        private void OnPlayerGrabObject(GameObject obj, Hand hand)
        {
            StickyBombHandler handler = obj.GetComponentInParent<StickyBombHandler>();
            if (handler)
                handler.Disconnect();
        }
    }
}
