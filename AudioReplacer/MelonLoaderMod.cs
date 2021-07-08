using MelonLoader;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace AudioReplacer
{
    public static class BuildInfo
    {
        public const string Name = "AudioReplacer";
        public const string Author = "trev";
        public const string Company = null;
        public const string Version = "1.1.1";
        public const string DownloadLink = null;
    }

    public class AudioReplacer : MelonMod
    {
        public static Dictionary<string, AudioClip> AudioClips = new Dictionary<string, AudioClip>();
        public static bool LogSounds;

        private string pathToSongs = Directory.GetCurrentDirectory() + "\\UserData\\CustomAudio\\";

        public override void OnApplicationStart()
        {
            BassImporter reader = new BassImporter();
            if (!Directory.Exists(pathToSongs))
                Directory.CreateDirectory(pathToSongs);

            var category = MelonPreferences.CreateCategory("AudioReplacer", "");
            category.CreateEntry("LogSounds", false);
            LogSounds = category.GetEntry<bool>("LogSounds").Value;

            string[] wavs = Directory.GetFiles(pathToSongs, "*.wav *.mp3");
            for (int i = 0; i < wavs.Length; i++)
            {
                reader.Import(wavs[i]);

                if (reader.audioClip == null)
                {
                    MelonLogger.Error("Failed to import " + Path.GetFileName(wavs[i]));
                    continue;
                }
                reader.audioClip.hideFlags = HideFlags.DontUnloadUnusedAsset;
                AudioClips.Add(reader.audioClip.name, reader.audioClip);
                MelonLogger.Msg("Added: " + reader.audioClip.name);
            }

            HarmonyInstance.Patch(AccessTools.Method(typeof(AudioSource), "Play", new Type[0]), new HarmonyMethod(typeof(Patches).GetMethod("AudioPlayPatch")));
            HarmonyInstance.Patch(AccessTools.Method(typeof(AudioSource), "Play", new Type[1] { typeof(ulong) }), new HarmonyMethod(typeof(Patches).GetMethod("AudioPlayPatch")));
        }
    }
}
