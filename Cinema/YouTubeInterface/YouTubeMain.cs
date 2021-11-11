using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using YoutubeDownloader.Services;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;

namespace Cinema
{
    public class YouTubeMain : MonoBehaviour
    {
        public YouTubeMain(IntPtr ptr) : base(ptr) { }

        public static Action<VideoInfo> OnVideoQueue;
        public static Queue<VideoInfo> VideoQueue;
        public static YouTubeMain Instance;

        public RectTransform ContentTransform;
        public GameObject ButtonPrefab;
        public Button SearchButton;
        public InputField Search;
        public YouTubeVideoPlayer YTDisplay;

        public QueueUIInfo QueueInfo;

        private QueryService queryService;
        private HttpClient httpClient;

        private List<GameObject> searchResults;

        private void Start()
        {
            Instance = this;

            VideoQueue = new Queue<VideoInfo>();

            queryService = new QueryService();
            httpClient = new HttpClient();
            searchResults = new List<GameObject>();

            MelonCoroutines.Start(CoStart());
        }

        public IEnumerator CoStart()
        {
            while (SearchButton == null)
                yield return null;
            SearchButton.onClick.AddListener(new Action(SearchAndDisplayReultsFromInputField));
            MelonLogger.Msg("Finished setting up the YouTube interface");
        }

        public void SearchAndDisplayReultsFromInputField()
        {
            foreach (GameObject go in searchResults)
                Destroy(go);
            searchResults.Clear();

            Search.interactable = false;
            SearchButton.interactable = false;
            MelonCoroutines.Start(QueryForVideos(Search.text, (results) => DisplaySearchResults(results)));
        }

        private void DisplaySearchResults(IVideo[] results)
        {
            foreach (IVideo video in results)
            {
                GameObject button = Instantiate(ButtonPrefab, ContentTransform);
                button.GetComponentInChildren<Text>().text = video.Title;

                VideoInfo info = new VideoInfo()
                {
                    id = video.Id.Value,
                    title = video.Title
                };

                MelonCoroutines.Start(SetupThumbnail(button.GetComponent<Image>(), info, video));

                button.GetComponent<Button>().onClick.AddListener(
                    new Action(() =>
                    {
                        GameObject queueItem = Instantiate(QueueInfo.ItemPrefab, QueueInfo.ContentTransform);
                        queueItem.GetComponentInChildren<Text>().text = video.Title;
                        queueItem.SetActive(true);
                        queueItem.GetComponent<Image>().sprite = button.GetComponent<Button>().image.sprite;

                        info.queueItem = queueItem;

                        VideoQueue.Enqueue(info);
                        OnVideoQueue.Invoke(info);
                        YTDisplay.AttemptQueuePlay(null);
                    }));

                button.SetActive(true);
                searchResults.Add(button);
            }

            Search.interactable = true;
            SearchButton.interactable = true;
        }

        internal IEnumerator SetupThumbnail(Image image, VideoInfo videoInfo, IVideo video)
        {
            Thumbnail thumb = video.Thumbnails.FirstOrDefault(t => t.Resolution.Width >= 720);
            if (thumb != null)
            {
                var downloadTask = httpClient.GetAsync(thumb.Url.Substring(0, thumb.Url.IndexOf("?")));
                while (!downloadTask.IsCompleted)
                    yield return null;

                var getBytesTask = downloadTask.Result.Content.ReadAsByteArrayAsync();
                while (!getBytesTask.IsCompleted)
                    yield return null;
                byte[] imgBytes = getBytesTask.Result;

                Texture2D tex = new Texture2D(2, 2);
                ImageConversion.LoadImage(tex, imgBytes);
                Sprite sprite = CreateSpriteImpl(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero, 100f, 0u, Vector4.zero);
                image.sprite = sprite;
                videoInfo.thumbnail = sprite;
            }
        }

        internal static Sprite CreateSpriteImpl(Texture2D texture, Rect rect, Vector2 pivot, float pixelsPerUnit, uint extrude, Vector4 border)
            => Sprite.CreateSprite_Injected(texture, ref rect, ref pivot, pixelsPerUnit, extrude, (SpriteMeshType)1, ref border, false);

        private IEnumerator QueryForVideos(string query, Action<IVideo[]> callback)
        {
            var parsedQueries = queryService.ParseMultilineQuery(query);
            var executeTask = queryService.ExecuteQueriesAsync(parsedQueries);

            while (!executeTask.IsCompleted)
                yield return null;

            var executedQueries = executeTask.Result;
            callback(executedQueries.SelectMany(q => q.Videos).Distinct().ToArray());
        }

        public class VideoInfo
        {
            public string id;
            public Sprite thumbnail;
            public string title;
            public GameObject queueItem;
        }

        public struct QueueUIInfo
        {
            public Transform ContentTransform;
            public GameObject ItemPrefab;
        }
    }
}