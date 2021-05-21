using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System;

namespace ChatSim.UI
{
    public class ChannelsUI : MonoBehaviour
    {
        [SerializeField]
        private RectTransform channelsPanel;
        [SerializeField]
        private Transform channelsContent;

        [Space(10)]
        [SerializeField]
        private GameObject channelPrefab;

        private void OnEnable()
        {
            MessagerUI.GoChannels += SwipeToChannels;
            ChatChannel.GoChannel += SwipeToChat;
        }

        private void OnDisable()
        {
            MessagerUI.GoChannels -= SwipeToChannels;
            ChatChannel.GoChannel -= SwipeToChat;
        }

        private void Start()
        {
            channelsPanel.gameObject.SetActive(true);
            channelsPanel.DOAnchorPos(new Vector2(-channelsPanel.rect.width, 0f), 0f).OnComplete(() => CompleteMoveChannels());
        }

        private void CreateChannels(List<string> joinedChannels)
        {
            if (channelsContent.childCount > 0)
                for (int i = 0; i < channelsContent.childCount; i++)
                    Destroy(channelsContent.GetChild(i).gameObject);

            for (int i = 0; i < joinedChannels.Count; i++)
            {
                ChatChannel chatChannel = Instantiate(channelPrefab, channelsContent).GetComponent<ChatChannel>();
                chatChannel.FillContent(joinedChannels[i]);
            }
        }

        private void SwipeToChannels(List<string> joinedChannels)
        {
            channelsPanel.gameObject.SetActive(true);
            CreateChannels(joinedChannels);
            channelsPanel.DOAnchorPos(new Vector2(0f, 0f), 0.25f);
        }

        private void SwipeToChat(string channelName)
        {
            channelsPanel.DOAnchorPos(new Vector2(-channelsPanel.rect.width, 0f), 0.25f).OnComplete(() => CompleteMoveChannels());
        }

        private void CompleteMoveChannels()
        {
            channelsPanel.gameObject.SetActive(false);
        }
    }
}
