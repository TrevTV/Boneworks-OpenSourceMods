using HarmonyLib;
using MelonLoader;
using StressLevelZero.SFX;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ModThatIsNotMod;
using ModThatIsNotMod.BoneMenu;

namespace CustomMusicMachineSongs
{
    public static class BuildInfo
    {
        public const string Name = "CustomMusicMachineSongs";
        public const string Author = "trev";
        public const string Company = null;
        public const string Version = "1.5.1";
        public const string DownloadLink = null;
    }

    public class Main : MelonMod
    {
        /*
         * Credit to Hello-Meow on GitHub for the audio importer
         * All I changed was to make it single threaded (BONEWORKS doesn't like multithreading)
         */
        public static Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
        static string pathToSongs = Directory.GetCurrentDirectory() + "\\UserData\\Music\\";
        static GameObject monoDisk;

        public override void OnApplicationStart()
        {
            MelonPreferences.CreateCategory("CustomMonoDiskSongs");
            MelonPreferences.CreateEntry("CustomMonoDiskSongs", "RemoveOriginalSongs", false);

            if (!Directory.Exists(pathToSongs))
                Directory.CreateDirectory(pathToSongs);

            MenuCategory ui = MenuManager.CreateCategory("MonoDisk", Color.white);
            
            ui.CreateFunctionElement("Spawn MonoDisk", Color.white, SpawnMonoDisk);

            LoadSongs();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (monoDisk == null)
            {
                GameObject maybeMonoDisk = GameObject.FindObjectOfType<JukeBoxMachine>()?.gameObject;
                if (maybeMonoDisk != null)
                {
                    GameObject _monoDisk = Object.Instantiate(maybeMonoDisk, Vector3.zero, Quaternion.identity);
                    _monoDisk.SetActive(false);

                    monoDisk = _monoDisk;
                    Object.DontDestroyOnLoad(monoDisk);

                    SpawnMenu.AddItem(CustomItems.CreateSpawnableObject(monoDisk, "MonoDisk", StressLevelZero.Data.CategoryFilters.GADGETS, StressLevelZero.Pool.PoolMode.REUSEOLDEST, 2));
                    MelonLogger.Msg("Found MonoDisk and added to spawngun");
                }
            }
        }

        public static void LoadSongs()
        {
            BassImporter bassImporter = new BassImporter();

            string[] wavs = Directory.GetFiles(pathToSongs, "*.wav *.mp3");
            for (int i = 0; i < wavs.Length; i++)
            {
                bassImporter.Import(wavs[i]);
                AudioClip clip = bassImporter.audioClip;
                if (clip == null)
                {
                    MelonLogger.Msg("Failed to import " + Path.GetFileName(wavs[i]));
                    continue;
                }
                audioClips.Add(clip.name, clip);
                clip.hideFlags = HideFlags.DontUnloadUnusedAsset;
                MelonLogger.Msg("Added " + clip.name);
            }
        }

        public static void SpawnMonoDisk()
        {
            if (monoDisk != null)
            {
                GameObject @object = Object.Instantiate(monoDisk, Player.GetPlayerHead().transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                @object.SetActive(true);
            }
            else
                MelonLogger.Msg("Unable to spawn MonoDisk, please load into a sandbox containing it first");
        }
    }

    [HarmonyPatch(typeof(JukeBoxMachine), "Start")]
    public static class JukePatch
    {
        public static void Postfix(JukeBoxMachine __instance)
        {
            if (MelonPreferences.GetEntryValue<bool>("CustomMonoDiskSongs", "RemoveOriginalSongs"))
            {
                __instance.clip_track = new AudioClip[0];
                __instance.name_track = new string[0];
            }

            // this is messy but is the best way to add to a Il2Cpp array
            List<string> names = new List<string>();
            names.AddRange(__instance.name_track);

            List<AudioClip> songs = new List<AudioClip>();
            songs.AddRange(__instance.clip_track);

            foreach (var value in Main.audioClips)
            {
                names.Add(value.Key);
                songs.Add(value.Value);
            }

            __instance.name_track = names.ToArray();
            __instance.clip_track = songs.ToArray();
        }
    }
}