using MelonLoader;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Cinema
{
    public static class BuildInfo
    {
        public const string Name = "Cinema";
        public const string Author = "trev";
        public const string Company = null;
        public const string Version = "1.0.1";
        public const string DownloadLink = null;
    }

    public class Cinema : MelonMod
    {
        private VideoPlayer currentPlayer;
        private bool uiOpen;
        private string url;

        public override void OnApplicationStart()
        {
            CustomMaps.CustomMaps.OnCustomMapLoad += CustomMaps_OnCustomMapLoad;
            MelonLogger.Msg("Loaded Cinema");
        }

        public override void OnUpdate()
        {
            if (currentPlayer != null && Input.GetKeyDown(KeyCode.C))
            {
                url = "";
                uiOpen = !uiOpen;
            }
            if (currentPlayer == null && uiOpen)
            {
                url = "";
                uiOpen = false;
            }
        }

        public override void OnGUI()
        {
            if (uiOpen)
            {
                GUILayoutOption[] option = null;
                GUILayout.BeginArea(new Rect(Screen.width / 2 - 250, Screen.height / 2 - 400, 500f, 800f));
                GUILayout.Box("Cinema Display", option);

                GUILayout.Label("URL", option);
                url = GUILayout.TextField(url, 999, option);

                if (GUILayout.Button("Play", option))
                {
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        PlayVideo(url);
                    }
                }

                GUILayout.EndArea();
            }
        }

        private void CustomMaps_OnCustomMapLoad(string obj)
        {
            if (obj.Contains("cinema"))
            {
                MelonLogger.Msg("Setting up cinema display...");
                GameObject screen = GameObject.Find("HDScreen");

                currentPlayer = screen.GetComponent<VideoPlayer>();
                var audioSource = screen.GetComponent<AudioSource>();

                currentPlayer.SetTargetAudioSource(0, audioSource);
            }
        }

        private void PlayVideo(string path)
        {
            if (currentPlayer == null)
                return;

            uiOpen = false;

            if (path.Contains("youtube.com") || path.Contains("youtu.be"))
            {
                //currentPlayer.Stop();
                string finalPath = Task.Run(() => GetStreamURL(path)).Result;
                currentPlayer.url = finalPath;
                currentPlayer.Play();
            }
        }

        async public Task<string> GetStreamURL(string url)
        {
            // knah and Slaynash assisted me with some of this
            var youtube = new YoutubeClient();
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
            var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

            return streamInfo.Url;
        }
    }
}
