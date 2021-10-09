using MelonLoader;
using StressLevelZero.Pool;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using ModThatIsNotMod;
using System.Xml.Linq;
using WNP78.Grenades;
using StressLevelZero.Data;

namespace StickyBomb
{
    public static class BuildInfo
    {
        public const string Name = "StickyBomb";
        public const string Author = "trev";
        public const string Company = null;
        public const string Version = "1.0.1";
        public const string DownloadLink = null;
    }

    public class StickyBomb : MelonMod
    {
        public GameObject detonator;

        public override void OnApplicationStart()
        {
            UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<Detonator>();
            UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<StickyBombHandler>();

            LoadAssets();
        }

        public void LoadAssets()
        {
            #region Detonator

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
            SpawnMenu.AddItem(CustomItems.CreateSpawnableObject(detonator, "Detonator", CategoryFilters.GADGETS, PoolMode.REUSEOLDEST, 2));

            #endregion

            #region Bomb

            MemoryStream bombStream;
            using (Stream bombFileStream = Assembly.GetManifestResourceStream("StickyBomb.pipebomb"))
            {
                bombStream = new MemoryStream((int)bombFileStream.Length);
                bombFileStream.CopyTo(bombStream);
            }
            AssetBundle grenade = AssetBundle.LoadFromMemory(bombStream.ToArray());
            grenade.hideFlags = HideFlags.DontUnloadUnusedAsset;
            TextAsset text = grenade.LoadAsset<TextAsset>("Grenades.xml");
            var xml = XDocument.Parse(text.text);

            foreach (var grenadeXml in xml.Root.Elements("Grenade"))
            {
                var prefab = grenade.LoadAsset<GameObject>((string)grenadeXml.Attribute("prefab"));
                prefab.hideFlags = HideFlags.DontUnloadUnusedAsset;
                CustomItems.FixObjectShaders(prefab);
                var guid = Guid.NewGuid();
                prefab.name = "GrenadeGuid_" + guid.ToString();
                Dictionary<Guid, XElement> definitions =
                    (Dictionary<Guid, XElement>)typeof(GrenadesMod)
                    .GetField("definitions", AccessTools.all)
                    .GetValue(MelonHandler.Mods.SingleOrDefault((m) => Path.GetFileNameWithoutExtension(m.Location).Contains("Grenades")));
                definitions[guid] = grenadeXml;
                var g = prefab.AddComponent<Grenade>();
                prefab.AddComponent<StickyBombHandler>();
                SpawnMenu.AddItem(CustomItems.CreateSpawnableObject(
                    prefab,
                    (string)grenadeXml.Attribute("name") ?? "[Grenade]",
                    (CategoryFilters)Enum.Parse(typeof(CategoryFilters), (string)grenadeXml.Attribute("category") ?? "GADGETS", true),
                    PoolMode.REUSEOLDEST,
                    (int?)grenadeXml.Attribute("pool") ?? 4
                ));
            }

            #endregion
        }
    }
}
