using UnityEngine;
using UnityEngine.UI;
using ChatSim.UI;
using ChatSim.ServerClient;
using System.Collections.Generic;
using System.Linq;
using ChatSim.Data;
using DG.Tweening;
using System;

namespace ChatSim.UI
{
    public class ChatMessage : MonoBehaviour
    {
        public static event Action<int> MessageDelete;

        [SerializeField]
        private Sprite adminMainBubble;

        [SerializeField]
        private Sprite userMainBubble;

        [SerializeField]
        private Sprite adminCommonBubble;

        [SerializeField]
        private Sprite userCommonBubble;

        [Space(10)]
        [SerializeField]
        private HorizontalLayoutGroup chatMessageLayoutGroup;
        [SerializeField]
        private HorizontalLayoutGroup messageContainerLayoutGroup;
        [SerializeField]
        private VerticalLayoutGroup textContainerLayoutGroup;
        [SerializeField]
        private Image avatarImage;
        [SerializeField]
        private Image bubbleImage;
        [SerializeField]
        private Button deleteButton;

        [Space(10)]
        [SerializeField]
        private Text nickNameText;
        [SerializeField]
        private Text messageText;
        [SerializeField]
        private Text dateText;

        [Space(10)]
        [SerializeField] private GameObject padding1;
        [SerializeField] private GameObject padding2;

        public int MessageNumber { get; private set; }
        public string PreviousClientName { get; private set; }

        private ChatMessage previousChatMessage;

        private void Start()
        {
            DOTween.Init();
            deleteButton.onClick.AddListener(DeleteMessage);
        }

        private void DeleteMessage() => MessageDelete(MessageNumber);

        public void FillContent(Message message, List<ClientData> clientsData, int messageNumber)
        {
            MessageNumber = messageNumber;

            switch (message.ClientRole)
            {
                case ClientRole.Admin:
                    chatMessageLayoutGroup.reverseArrangement = true;
                    messageContainerLayoutGroup.reverseArrangement = true;
                    chatMessageLayoutGroup.childAlignment = TextAnchor.LowerRight;
                    textContainerLayoutGroup.childAlignment = TextAnchor.MiddleRight;
                    FillMessageFields(message, clientsData);
                    break;
                case ClientRole.User:
                    FillMessageFields(message, clientsData);
                    break;
            }
        }

        public void SetCorrectBubbleSprite(Message message, string previousClientName, ChatMessage previousChatMessage)
        {
            bubbleImage.sprite = GetCorrectBubbleSprite(message, previousClientName, previousChatMessage);
        }

        public void SetCorrectSprite(Sprite sprite)
        {
            bubbleImage.sprite = sprite;
            padding1.SetActive(true);
            padding2.SetActive(false);
        }

        private Sprite GetCorrectBubbleSprite(Message message, string previousClientName, ChatMessage previousChatMessage = null)
        {
            Sprite sprite = null;

            padding1.SetActive(false);
            padding2.SetActive(true);

            switch (message.ClientRole)
            {
                case ClientRole.Admin:
                    if (string.IsNullOrEmpty(previousClientName))
                        sprite = adminMainBubble;
                    else
                    {
                        if (previousClientName == message.ClientName)
                        {
                            sprite = adminMainBubble;
                            if (previousChatMessage != null)
                                previousChatMessage.SetCorrectSprite(adminCommonBubble);
                            padding1.SetActive(false);
                            padding2.SetActive(true);
                        }
                        else
                        {
                            sprite = adminMainBubble;
                        }
                    }
                    break;
                case ClientRole.User:
                    if (string.IsNullOrEmpty(previousClientName))
                        sprite = userMainBubble;
                    else
                    {
                        if (previousClientName == message.ClientName)
                        {
                            sprite = userMainBubble;
                            if (previousChatMessage != null)
                                previousChatMessage.SetCorrectSprite(userCommonBubble);
                            padding1.SetActive(false);
                            padding2.SetActive(true);
                        }
                        else
                        {
                            sprite = userMainBubble;
                        }
                    }
                    break;
            }

            return sprite;
        }

        private void FillMessageFields(Message message, List<ClientData> clientsData)
        {
            avatarImage.sprite = GetAvatar(message.ClientName, clientsData);
            nickNameText.text = message.ClientName;
            messageText.text = message.Text;
            dateText.text = message.Date;
        }

        private Sprite GetAvatar(string clientName, List<ClientData> clientsData)
        {
            return clientsData.FirstOrDefault(x => x.nickName == clientName).avatar;
        }

        public void EnableDeleteButton()
        {
            LayoutElement layoutElement = deleteButton.GetComponent<LayoutElement>();
            layoutElement.preferredHeight = 0f;
            layoutElement.preferredWidth = 0f;
            layoutElement.minWidth = 0f;
            layoutElement.minHeight = 0f;

            deleteButton.gameObject.SetActive(true);

            DOVirtual.Float(0f, 125f, 0.25f, result =>
            {
                layoutElement.minWidth = result;
                layoutElement.minHeight = result;
                layoutElement.preferredWidth = result;
                layoutElement.preferredHeight = result;
            });
        }

        public void DisableDeleteButton()
        {
            LayoutElement layoutElement = deleteButton.GetComponent<LayoutElement>();

            DOVirtual.Float(125f, 0f, 0.25f, result =>
            {
                layoutElement.minWidth = result;
                layoutElement.minHeight = result;
                layoutElement.preferredWidth = result;
                layoutElement.preferredHeight = result;
            });
        }
    }
}
