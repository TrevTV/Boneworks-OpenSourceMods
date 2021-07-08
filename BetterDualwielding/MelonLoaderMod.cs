using MelonLoader;
using UnityEngine;
using UnhollowerRuntimeLib;
using ModThatIsNotMod;
using ModThatIsNotMod.BoneMenu;
using StressLevelZero.Props.Weapons;

namespace BetterDualwielding
{
    public static class BuildInfo
    {
        public const string Name = "Better Dualwielding";
        public const string Author = "trevtv";
        public const string Company = null;
        public const string Version = "1.1.0";
        public const string DownloadLink = null;
    }

    public class Main : MelonMod
    {
        public static bool enabled = true;
        public static AudioSource audioSource;

        public override void OnApplicationStart()
        {
            MenuCategory ui = MenuManager.CreateCategory("Better Dualwielding", Color.white);
            ui.CreateBoolElement("Enabled", Color.white, true, (b) => enabled = b);

            ClassInjector.RegisterTypeInIl2Cpp<PouchScript>();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            GameObject pouch = Player.GetRigManager().GetComponentInChildren<AmmoPouch>().gameObject.transform.Find("AmmoInsert").gameObject;
            GameObject belt = Player.GetRigManager().transform.Find("[SkeletonRig (GameWorld Brett)]").Find("Brett@neutral").Find("geoGrp").Find("brett_accessories_belt_mesh").gameObject;
            pouch.AddComponent<PouchScript>();

            belt.AddComponent<BoxCollider>().isTrigger = true;
            belt.AddComponent<PouchScript>();

            audioSource = belt.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = Audio.sfxMixer;
        }
    }
}