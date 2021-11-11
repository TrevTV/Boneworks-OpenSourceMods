using Newtonsoft.Json.Linq;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using MelonLoader;
using System.Text;

namespace Cinema
{
    public class YouTubeVideoPlayer : MonoBehaviour
    {
        public YouTubeVideoPlayer(System.IntPtr ptr) : base(ptr) { }

        public static bool UseRealtimeGI;

        private VideoPlayer videoPlayer;
        private Renderer renderer;

        private int frameSkip = 5;
        private int frame = 0;

        private Color emissiveColor = new Color32(191, 191, 191, 255);
        private Color offColor = new Color32(0, 0, 0, 255);

        private void Start()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            renderer = GetComponent<Renderer>();
            emissiveColor *= 4;

            // thanks herp
            videoPlayer.loopPointReached = (
                    (videoPlayer.loopPointReached == null)
                    ? new System.Action<VideoPlayer>(AttemptQueuePlay)
                    : Il2CppSystem.Delegate.Combine(videoPlayer.loopPointReached, (Il2CppSystem.Action<VideoPlayer>)AttemptQueuePlay).Cast<VideoPlayer.EventHandler>());

            UpdateEmissionStatus();
        }

        private void LateUpdate()
        {
            if (UseRealtimeGI)
            {
                if (frame >= frameSkip)
                {
                    if (videoPlayer.isPlaying)
                        RendererExtensions.UpdateGIMaterials(renderer);
                    frame = 0;

                }
                else
                    frame++;
            }
        }

        public void UpdateEmissionStatus() => renderer.sharedMaterial.SetColor("_EmissionColor", UseRealtimeGI ? emissiveColor : offColor);

        public void AttemptQueuePlay(VideoPlayer vp)
        {
            if (videoPlayer.isPlaying || YouTubeMain.VideoQueue.Count == 0)
                return;

            if (YouTubeMain.VideoQueue.Count != 0)
            {
                var video = YouTubeMain.VideoQueue.Dequeue();
                if (video.queueItem)
                    Destroy(video.queueItem);
                MelonCoroutines.Start(PlayVideo(video, Cinema.YTDLPPath));
            }
            else
                videoPlayer.Stop();
        }

        public IEnumerator PlayVideo(YouTubeMain.VideoInfo video, string ytDlpLoc)
        {
            string jsonPath = Path.Combine(Cinema.DataDirectory, "temp_ytdlp.json");
            string batPath = Path.Combine(Cinema.DataDirectory, "temp_ytdlp.bat");

            #region Receive Video Data

            StringBuilder writer = new StringBuilder();
            writer.AppendLine("@echo off");
            writer.AppendLine($"\"{ytDlpLoc}\" -J -v --no-check-formats {video.id} > \"{jsonPath}\"");
            writer.AppendLine("exit");
            File.WriteAllText(batPath, writer.ToString());

            MelonLogger.Msg("Beginning process call");
            Process ytDlp = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = batPath,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };

            ytDlp.Start();
            while (!ytDlp.HasExited)
                yield return null;

            #endregion

            #region Find Highest Quality Muxed Video

            RawVideoInfo currentHighestVideo = new RawVideoInfo();
            JObject jObj = JObject.Parse(File.ReadAllText(jsonPath));
            foreach (JToken token in jObj["formats"])
            {
                if (token["width"].ToObject<object>() == null) continue;

                int width = token["width"].ToObject<int>();
                int height = token["height"].ToObject<int>();

                if (token["vcodec"].ToString() != "none"
                    && token["acodec"].ToString() != "none"
                    && width > currentHighestVideo.width
                    && height > currentHighestVideo.height)
                {
                    currentHighestVideo.width = width;
                    currentHighestVideo.height = height;
                    currentHighestVideo.url = token["url"].ToString();
                }
            }

            #endregion

            File.Delete(jsonPath);
            File.Delete(batPath);

            if (!string.IsNullOrEmpty(currentHighestVideo.url))
            {
                MelonLogger.Msg("Found video with url " + currentHighestVideo.url);
                videoPlayer.url = currentHighestVideo.url;
                videoPlayer.Play();
            }
            else
                MelonLogger.Error("Failed to find muxed video, not queuing");
        }

        private class RawVideoInfo
        {
            public string url = string.Empty;
            public int width = 0;
            public int height = 0;
        }
    }
}