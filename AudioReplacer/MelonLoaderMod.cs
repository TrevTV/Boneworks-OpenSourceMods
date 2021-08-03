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
        public const string Version = "1.2.0";
        public const string DownloadLink = null;
    }

    public class AudioReplacer : MelonMod
    {
        public static Dictionary<string, AudioClip> AudioClips = new Dictionary<string, AudioClip>();
        public static bool LogSounds;

        private string customAudioPath = Path.Combine(MelonUtils.UserDataDirectory, "CustomAudio");

        private string[] allowedExts = new string[] { ".wav", ".mp3" };

        public override void OnApplicationStart()
        {
            BassImporter reader = new BassImporter();
            if (!Directory.Exists(customAudioPath))
                Directory.CreateDirectory(customAudioPath);

            var category = MelonPreferences.CreateCategory("AudioReplacer", "");
            category.CreateEntry("LogSounds", false);
            LogSounds = category.GetEntry<bool>("LogSounds").Value;

            string[] audioFiles = Directory.GetFiles(customAudioPath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < audioFiles.Length; i++)
            {
                if (!allowedExts.Contains(Path.GetExtension(audioFiles[i]))) continue;

                reader.Import(audioFiles[i]);

                if (reader.audioClip == null)
                {
                    MelonLogger.Error("Failed to import " + Path.GetFileName(audioFiles[i]));
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
