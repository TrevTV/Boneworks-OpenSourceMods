using System.IO;
using System.Reflection;
using System.Collections;
using MelonLoader;
using UnityEngine;

namespace AlwaysReverb
{
    public static class BuildInfo
    {
        public const string Name = "AlwaysReverb";
        public const string Author = "trev";
        public const string Company = null;
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }

    public class AlwaysReverb : MelonMod
    {
        public GameObject thiReverbThing;

        public override void OnApplicationStart() => LoadReverbZone();

        public override void OnSceneWasInitialized(int buildIndex, string sceneName) => MelonCoroutines.Start(AddReverbZone());

        public IEnumerator AddReverbZone()
        {
            yield return new WaitForSeconds(5);
            GameObject.Instantiate(thiReverbThing, ModThatIsNotMod.Player.GetRigManager().transform.Find("[PhysicsRig]"));
            MelonLogger.Msg("Added reverb zone");
        }

        public void LoadReverbZone()
        {
            Stream stream = Assembly.GetManifestResourceStream("AlwaysReverb.reverb");

            byte[] data;
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                data = ms.ToArray();
            }

            Il2CppAssetBundle bundle = Il2CppAssetBundleManager.LoadFromMemory(data);
            thiReverbThing = bundle.LoadAsset("ReverbZone.prefab").Cast<GameObject>();
            thiReverbThing.hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
    }
}
