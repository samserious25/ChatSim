using ChatSim.Data;
using ChatSim.ServerClient;
using ChatSim.World;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ChatSim.UI
{
    public class MessagerUI : MonoBehaviour
    {
        public static event Action<string> MessageSend;
        public static event Action<List<string>> GoChannels;

        [SerializeField]
        private ClientsBase clientsData;

        [Space(10)]
        [SerializeField]
        private Transform chatContentTransform;
        [SerializeField]
        GameObject chatMessagePrefab;
        [SerializeField]
        GameObject joinMessagePrefab;

        [Space(10)]
        [SerializeField]
        private Button publishButton;
        [SerializeField]
        private Button deleteButton;
        [SerializeField]
        private Button completeDeleteButton;
        [SerializeField]
        private Button goChannelsButton;
        [SerializeField]
        private InputField messageInput;
        [SerializeField]
        private HorizontalLayoutGroup bottomPanelHorizontalLayoutGroup;
        [SerializeField]
        private Text channelName;
        [SerializeField]
        private GameObject topPanel;

        private List<ChatMessage> chatMessages = new List<ChatMessage>();
        private readonly List<JoinMessage> joinMessages = new List<JoinMessage>();

        private int messsageCounter;
        private string activeChannel;
        private bool deleteMode;
        private bool emulationDisabled;
        private string previousClientName;
        private ChatMessage previousChatMessage;

        private void OnEnable()
        {
            ChatMessage.MessageDelete += OnMessageDelete;
        }

        private void OnDisable()
        {
            ChatMessage.MessageDelete += OnMessageDelete;
        }

        private void Start()
        {
            publishButton.onClick.AddListener(PublishMessage);
            deleteButton.onClick.AddListener(EnableDeleteMessages);
            completeDeleteButton.onClick.AddListener(CompleteDelete);
            goChannelsButton.onClick.AddListener(GoChannelsAndClear);
        }

        public void DisableEmulationUI()
        {
            topPanel.SetActive(false);
            emulationDisabled = true;
        }

        private void GoChannelsAndClear()
        {
            if (chatMessages.Count > 0)
            {
                for (int i = 0; i < chatMessages.Count; i++)
                    Destroy(chatMessages[i].gameObject);

                chatMessages.Clear();
            }

            if (joinMessages.Count > 0)
            {
                for (int i = 0; i < joinMessages.Count; i++)
                    Destroy(joinMessages[i].gameObject);

                joinMessages.Clear();
            }

            GoChannels?.Invoke(ClientWorld.MyClient.JoinedChannels);
        }

        public void SetActiveChannel(string activeChannel)
        {
            this.activeChannel = activeChannel;
            channelName.text = activeChannel;
        }

        public void ClientJoinedChannel(string clientName, string channelName, ClientOwner clientOwner, ConnectionStatus connectionStatus)
        {
            if (emulationDisabled)
                return;

            if (activeChannel != channelName)
                return;

            JoinMessage joinMessage = Instantiate(joinMessagePrefab, chatContentTransform).GetComponent<JoinMessage>();

            switch (connectionStatus)
            {
                case ConnectionStatus.JoinChannel:
                    joinMessage.SetJoinText(clientName, true);
                    break;
                case ConnectionStatus.LeaveChannel:
                    joinMessage.SetJoinText(clientName, false);
                    break;
            }

            joinMessages.Add(joinMessage);
        }

        public void ClientRecievedMessage(Message message)
        {
            ChatMessage chatMessage = Instantiate(chatMessagePrefab, chatContentTransform).GetComponent<ChatMessage>();
            chatMessage.FillContent(message, clientsData.data, messsageCounter++);
            chatMessage.SetCorrectBubbleSprite(message, previousClientName, previousChatMessage);

            previousClientName = message.ClientName;
            previousChatMessage = chatMessage;

            chatMessages.Add(chatMessage);
            AnimateMessageBubble(chatMessage);
        }

        private void PublishMessage()
        {
            if (string.IsNullOrEmpty(messageInput.text))
                return;

            string messsageText = messageInput.text.TrimStart();
            messageInput.text = null;

            MessageSend?.Invoke(messsageText.TrimEnd());
        }

        private void OnMessageDelete(int messageNumber)
        {
            ChatMessage message = chatMessages.FirstOrDefault(x => x.MessageNumber == messageNumber);

            if (message != null)
            {
                chatMessages.Remove(message);
                AnimateMessageDelete(message);
            }
        }

        private void EnableDeleteMessages()
        {
            deleteMode = true;
            EnableBottomElements(false);

            bottomPanelHorizontalLayoutGroup.childAlignment = TextAnchor.MiddleRight;

            for (int i = 0; i < chatMessages.Count; i++)
                chatMessages[i].EnableDeleteButton();
        }

        private void CompleteDelete()
        {
            deleteMode = false;
            EnableBottomElements(true);

            bottomPanelHorizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;

            for (int i = 0; i < chatMessages.Count; i++)
                chatMessages[i].DisableDeleteButton();
        }

        private void EnableBottomElements(bool enable)
        {
            publishButton.gameObject.SetActive(enable);
            messageInput.gameObject.SetActive(enable);
            deleteButton.gameObject.SetActive(enable);
            completeDeleteButton.gameObject.SetActive(!enable);
        }

        private void AnimateMessageBubble(ChatMessage message)
        {
            message.transform.localScale = Vector3.zero;
            message.transform.DOScale(Vector3.one, 0.25f).OnComplete(() => OnEndAnimateMessageBubble(message));
        }

        private void OnEndAnimateMessageBubble(ChatMessage message)
        {
            if (deleteMode)
                message.EnableDeleteButton();
        }

        private void AnimateMessageDelete(ChatMessage message)
        {
            LayoutElement layoutElement = message.transform.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = message.GetComponent<RectTransform>().rect.height;
            DOVirtual.Float(layoutElement.preferredHeight, 0f, 0.25f, result =>
            {
                layoutElement.preferredHeight = result;
            });
            message.transform.DOScale(Vector3.zero, 0.25f).OnComplete(() => Destroy(message.gameObject));
        }
    }
}
