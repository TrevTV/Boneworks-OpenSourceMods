using UnityEngine;
using MelonLoader;

namespace AudioReplacer
{
    public static class Patches
    {
        public static void AudioPlayPatch(AudioSource __instance)
        {
            if (__instance.clip == null) return;

            if (AudioReplacer.LogSounds)
                MelonLogger.Msg($"Playing \"{__instance.clip.name}\" from object \"{__instance.gameObject.name}\"");

            if (AudioReplacer.AudioClips.TryGetValue(__instance.clip.name, out AudioClip replaceClip))
                __instance.clip = replaceClip;
        }
    }
}
