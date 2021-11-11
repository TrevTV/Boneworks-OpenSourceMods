using Entanglement.Extensions;
using Entanglement.Modularity;
using Entanglement.Network;
using MelonLoader;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;

namespace Cinema.Entanglement
{
    public class SyncModule : EntanglementModule
    {
        public override void OnModuleLoaded()
        {
            ModuleLogger.Msg("Beginning module initialization...");
            NetworkMessage.RegisterHandler<CinemaMessageHandler>();
            YouTubeMain.OnVideoQueue += OnVideoQueue;
            // maybe update new player queue to match everyone else's
            //DiscordIntegration.lobbyManager.OnMemberConnect += LobbyManager_OnMemberConnect;
        }

        public void OnVideoQueue(YouTubeMain.VideoInfo video)
        {
            ModuleLogger.Msg("Active Node Null? " + Node.activeNode);
            ModuleLogger.Msg("ConnectedUsers Count: " + Node.activeNode?.connectedUsers?.Count ?? "NULL");
            if (Node.activeNode == null || Node.activeNode.connectedUsers.Count == 0) return;

            var msg = NetworkMessage.CreateMessage(132, new CinemaMessageData { video = video });
            Node.activeNode.BroadcastMessage(NetworkChannel.Reliable, msg.GetBytes());
        }
    }

    public class CinemaMessageHandler : NetworkMessageHandler
    {
        public override byte? MessageIndex => 132;

        public override NetworkMessage CreateMessage(NetworkMessageData data)
        {
            if (!(data is CinemaMessageData)) throw new Exception("Unexpected message type");
            var cmd = data as CinemaMessageData;

            var msg = new NetworkMessage();
            msg.messageType = MessageIndex.Value;
            msg.messageData = new byte[0];
            YouTubeMain.VideoInfo info = cmd.video;
            msg.messageData = Encoding.UTF8.GetBytes(info.id);

            return msg;
        }

        public override void HandleMessage(NetworkMessage message, long sender)
        {
            ModuleLogger.Msg("Cinema Message from: " + sender);

            MelonCoroutines.Start(QueueVideoFromId(Encoding.UTF8.GetString(message.messageData)));
        }

        private IEnumerator QueueVideoFromId(string videoId)
        {
            var getVideoTask = Cinema.YouTubeClient.Videos.GetAsync(VideoId.Parse(videoId));
            while (!getVideoTask.IsCompleted)
                yield return null;

            IVideo video = getVideoTask.Result;
            GameObject queueItem = GameObject.Instantiate(YouTubeMain.Instance.QueueInfo.ItemPrefab, YouTubeMain.Instance.QueueInfo.ContentTransform);
            queueItem.GetComponentInChildren<Text>().text = video.Title;
            queueItem.SetActive(true);
            YouTubeMain.VideoInfo info = new YouTubeMain.VideoInfo()
            {
                id = videoId,
                title = video.Title,
                queueItem = queueItem
            };
            MelonCoroutines.Start(YouTubeMain.Instance.SetupThumbnail(queueItem.GetComponent<Image>(), info, video));
            YouTubeMain.VideoQueue.Enqueue(info);
            YouTubeMain.Instance.YTDisplay.AttemptQueuePlay(null);
        }
    }

    public class CinemaMessageData : NetworkMessageData
    {
        public YouTubeMain.VideoInfo video;
    }
}
