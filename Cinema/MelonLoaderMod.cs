using MelonLoader;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using YoutubeExplode;
using ModThatIsNotMod.BoneMenu;

namespace Cinema
{
    public static class BuildInfo
    {
        public const string Name = "Cinema";
        public const string Author = "trev";
        public const string Company = null;
        public const string Version = "1.1.0";
        public const string DownloadLink = null;
    }

    public class Cinema : MelonMod
    {
        public static string DataDirectory;
        public static string YTDLPPath;
        public static YoutubeClient YouTubeClient;

        public static MelonPreferences_Entry<bool> UseRealtimeGIDefault;

        public override void OnApplicationStart()
        {
            UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<YouTubeMain>();
            UnhollowerRuntimeLib.ClassInjector.RegisterTypeInIl2Cpp<YouTubeVideoPlayer>();

            SetupDirectories();
            SetupModPrefs();

            CustomMaps.CustomMaps.OnCustomMapLoad += CustomMaps_OnCustomMapLoad;
            Entanglement.SyncModuleSetup.Init();
        }

        private void SetupDirectories()
        {
            DataDirectory = Path.Combine(MelonUtils.UserDataDirectory, "Cinema");
            YTDLPPath = Path.Combine(DataDirectory, "yt-dlp.exe");
            if (!Directory.Exists(DataDirectory))
                Directory.CreateDirectory(DataDirectory);
            YouTubeClient = new YoutubeClient();

            if (!File.Exists(YTDLPPath))
            {
                byte[] rawDlp;
                using (Stream str = Assembly.GetManifestResourceStream("Cinema.yt-dlp.exe"))
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    str.CopyTo(memoryStream);
                    rawDlp = memoryStream.ToArray();
                }
                File.WriteAllBytes(YTDLPPath, rawDlp);
            }
        }

        private void SetupModPrefs()
        {
            var category = MelonPreferences.CreateCategory("Cinema");
            UseRealtimeGIDefault = category.CreateEntry(nameof(UseRealtimeGIDefault), false);

            YouTubeVideoPlayer.UseRealtimeGI = UseRealtimeGIDefault.Value;
            MenuCategory boneCategory = MenuManager.CreateCategory("Cinema", Color.white);
            boneCategory.CreateBoolElement("Realtime Emission", Color.white, UseRealtimeGIDefault.Value, (b) =>
            {
                YouTubeVideoPlayer.UseRealtimeGI = b;
                YouTubeMain.Instance?.YTDisplay?.UpdateEmissionStatus();
            });
        }

        private void CustomMaps_OnCustomMapLoad(string obj)
        {
            GameObject display = GameObject.Find("!Display");
            if (display)
            {
                MelonLogger.Msg("Setting up cinema display...");
                YouTubeVideoPlayer vp = display.AddComponent<YouTubeVideoPlayer>();
                display.GetComponent<AudioSource>().outputAudioMixerGroup = ModThatIsNotMod.Audio.musicMixer;

                Transform root = display.transform.parent;
                Transform youtubeStuff = root.transform.Find("YouTubeStuff");
                YouTubeMain ytMain = youtubeStuff.transform.Find("YouTubeMain").gameObject.AddComponent<YouTubeMain>();

                ytMain.ContentTransform = youtubeStuff.Find("YouTubeSearch/group_popUp/panel_global/Scroll View/Viewport/Content").GetComponent<RectTransform>();
                ytMain.ButtonPrefab = youtubeStuff.Find("YouTubeSearch/group_popUp/panel_global/Scroll View/Viewport/Content/Button").gameObject;
                ytMain.SearchButton = youtubeStuff.Find("YouTubeSearch/group_popUp/panel_global/Button").GetComponent<Button>();
                ytMain.Search = youtubeStuff.Find("YouTubeSearch/group_popUp/panel_global/InputField").GetComponent<InputField>();
                ytMain.YTDisplay = vp;

                ytMain.QueueInfo.ContentTransform = youtubeStuff.Find("VideoQueue/group_popUp/panel_global/Scroll View/Viewport/Content").GetComponent<RectTransform>();
                ytMain.QueueInfo.ItemPrefab = youtubeStuff.Find("VideoQueue/group_popUp/panel_global/Scroll View/Viewport/Content/Item").gameObject;

                foreach (Button key in root.Find("YouTubeStuff/YouTubeSearch/group_popUp/panel_global/Keys").GetComponentsInChildren<Button>())
                {
                    key.onClick.AddListener(new System.Action(() =>
                    {
                        if (key.name == "Backspace")
                        {
                            if (ytMain.Search.text.Length != 0)
                                ytMain.Search.text = ytMain.Search.text.Remove(ytMain.Search.text.Length - 1);
                            return;
                        }
                        else if (key.name == "Space")
                        {
                            ytMain.Search.text += " ";
                            return;
                        }

                        ytMain.Search.text += key.name.ToLower();
                    }));
                }
            }
        }
    }
}
