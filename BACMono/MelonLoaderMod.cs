using System.IO;
using MelonLoader;
using UnityEngine;
using ModThatIsNotMod;
using System.Reflection;
using System;

namespace BACMono
{
    public static class BuildInfo
    {
        public const string Name = "BACMono";
        public const string Author = "trev";
        public const string Company = null;
        public const string Version = "1.1.0";
        public const string DownloadLink = null;
    }

    public delegate void CMHandler(string mapName);

    public class BACMono : MelonMod
    {
        private GameObject setupBac;

        public override void OnApplicationStart()
        {
            UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<BACController>();
            UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<TriggerCheck>();

            SetupAssets();
            EnableCustomMapsSupport();
        }

        private void EnableCustomMapsSupport()
        {
            /*try
            {
                CMHandler handler = CMSpawn;
                Assembly a = Assembly.LoadFrom(Path.Combine(Directory.GetCurrentDirectory(), "Mods", "CustomMaps.dll"));
                Type t = a.GetType("CustomMaps.CustomMaps");
                t.GetEvent("OnCustomMapLoad").AddEventHandler(null, handler);
            }
            catch { }*/
            CustomMaps.CustomMaps.OnCustomMapLoad += CMSpawn;
        }

        private void CMSpawn(string mapName)
        {
            //MelonLogger.Msg("[BAC_DEBUG] MAP LOADED: " + mapName);
            GameObject.Find("CUSTOM_MAP_ROOT").transform.Find("BACMono").gameObject.AddComponent<BACController>();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            SpawnMenu.AddItem(CustomItems.CreateSpawnableObject(setupBac, "BAC Mono", StressLevelZero.Data.CategoryFilters.GADGETS, StressLevelZero.Pool.PoolMode.REUSEOLDEST, 2));
        }

        public void SetupAssets()
        {
            MemoryStream memoryStream;
            using (Stream stream = Assembly.GetManifestResourceStream("BACMono.mono"))
            {
                memoryStream = new MemoryStream((int)stream.Length);
                stream.CopyTo(memoryStream);
            }
            AssetBundle assetBundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());
            GameObject bac = assetBundle.LoadAsset("Assets/BAC Mono.prefab").Cast<GameObject>();
            bac.SetActive(false);
            bac.AddComponent<BACController>();
            GameObject.DontDestroyOnLoad(bac);
            bac.hideFlags = HideFlags.DontUnloadUnusedAsset;
            setupBac = bac;
        }
    }
}
