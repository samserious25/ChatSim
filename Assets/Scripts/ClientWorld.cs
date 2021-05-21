using ChatSim.Data;
using ChatSim.ServerClient;
using ChatSim.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChatSim.World
{
    public class ClientWorld : MonoBehaviour
    {
        public static event Action<Message> MessageRecieved;

        [Space(10)]
        [SerializeField]
        private bool emulateActivity;
        [SerializeField]
        private MessageBase messageBase;

        [Space(10)]
        [SerializeField]
        private MessagerUI messagerUI;

        [Space(10)]
        [SerializeField]
        private ClientData myClientData;
        [SerializeField]
        private string myActiveChannelName;

        [Space(10)]
        public List<ChannelData> channels;

        public List<Client> AllClients { get; private set; }
        public List<Message> AllMesssages { get; private set; }
        public List<ChatHistory> ChatHistories { get; private set; }
        public static Client MyClient { get; private set; }

        private string activeChannel;
        private bool leaveRoutinActive;
        private bool joinRoutinActive;
        private bool sendMessageRoutinActive;

        private void OnEnable()
        {
            MessagerUI.MessageSend += OnMessageSendButton;
            MessagerUI.GoChannels += OnGoChannels;
            ChatChannel.GoChannel += ChangeChannel;
        }

        private void OnDisable()
        {
            MessagerUI.MessageSend -= OnMessageSendButton;
            MessagerUI.GoChannels -= OnGoChannels;
            ChatChannel.GoChannel -= ChangeChannel;
        }

        private void Start()
        {
            Application.targetFrameRate = 60;

            new Server();

            AllClients = new List<Client>();
            AllMesssages = new List<Message>();
            ChatHistories = new List<ChatHistory>();

            MyClient = CreateClient(myClientData.nickName, myClientData.clientRole, true);

            MyClient.JoinedChannel += messagerUI.ClientJoinedChannel;
            MyClient.MessageRecieved += messagerUI.ClientRecievedMessage;

            messagerUI.SetActiveChannel(myActiveChannelName);

            MyClient.Connect();

            activeChannel = myActiveChannelName;

            if (!emulateActivity)
            {
                messagerUI.DisableEmulationUI();
                MyClient.JoinChannel(myActiveChannelName);
            }
            else
            {
                for (int i = 0; i < channels.Count; i++)
                    MyClient.JoinChannel(channels[i].channelName);
            }

            for (int i = 0; i < channels.Count; i++)
                ConnectClientsFromChannelData(channels[i]);

            if (emulateActivity)
            {
                leaveRoutinActive = false;
                joinRoutinActive = false;
                sendMessageRoutinActive = false;

                StartCoroutine(JoinRoutine());
            }
        }

        private void ChangeChannel(string channel)
        {
            activeChannel = channel;
            messagerUI.SetActiveChannel(activeChannel);

            ChatHistory chatHistory = ChatHistories.FirstOrDefault(x => x.ChannelName == activeChannel);

            if (chatHistory != null)
                for (int i = 0; i < chatHistory.Messages.Count; i++)
                {
                    AllMesssages.Add(chatHistory.Messages[i]);
                    messagerUI.ClientRecievedMessage(chatHistory.Messages[i]);
                }

            if (emulateActivity)
            {
                StartCoroutine(JoinRoutine());
                StartCoroutine(LeaveRoutine());
                StartCoroutine(SendMessageRoutine());
            }
        }

        // Client create and join channel

        private Client CreateClient(string name, ClientRole role, bool isMine = false)
        {
            UserData userData = new UserData() { ClientName = name, ClientRole = role };
            return new Client(userData, isMine);
        }

        private Client ConnectClient(ClientData clientData)
        {
            Client client = GetRegisteredClient(clientData);
            client.Connect();
            return client;
        }

        private Client GetRegisteredClient(ClientData clientData)
        {
            Client connectedClient = AllClients.FirstOrDefault(x => x.Data.ClientName == clientData.nickName);

            if (connectedClient == null)
            {
                connectedClient = CreateClient(clientData.nickName, clientData.clientRole);
                AllClients.Add(connectedClient);
            }

            return connectedClient;
        }

        private void JoinClient(Client client, ChannelData channel)
        {
            client.JoinChannel(channel.channelName);
            client.SetActiveChannel(activeChannel);
        }

        private void ConnectClientsFromChannelData(ChannelData channel)
        {
            if (channels.Count == 0)
            {
                Debug.LogError("No channels created");
                return;
            }

            if (channel == null)
                return;

            for (int i = 0; i < channel.clients.Count; i++)
            {
                Client client = ConnectClient(channel.clients[i]);

                if (!emulateActivity)
                    JoinClient(client, channel);
            }
        }

        // Message sending

        private void OnMessageSendButton(string messageText)
        {
            if (emulateActivity)
            {
                Message message = new Message(MyClient.Data.ClientName, MyClient.Data.ClientRole, activeChannel, messageText);
                AllMesssages.Add(message);
                MyClient.SendMessageToAll(message);
                return;
            }

            SendMessageFromRandomClient(messageText);
        }

        private void SendMessageFromRandomClient(string messageText)
        {
            List<Client> clientsInActiveChannel = AllClients.Where(x => x.ActiveChannel == activeChannel).ToList();
            int randomIndex = UnityEngine.Random.Range(0, clientsInActiveChannel.Count);
            Message message = new Message(clientsInActiveChannel[randomIndex].Data.ClientName, clientsInActiveChannel[randomIndex].Data.ClientRole, activeChannel, messageText);
            AllMesssages.Add(message);

            clientsInActiveChannel[randomIndex].SendMessageToAll(message);
            MessageRecieved?.Invoke(message);
        }

        // Save chat history

        private void OnGoChannels(List<string> channels)
        {
            StopAllCoroutines();

            ChatHistory chatHistory = ChatHistories.FirstOrDefault(x => x.ChannelName == activeChannel);

            if (chatHistory != null)
                chatHistory.Save(AllMesssages);
            else
            {
                chatHistory = new ChatHistory(activeChannel);
                chatHistory.Save(AllMesssages);
                ChatHistories.Add(chatHistory);
            }

            AllMesssages.Clear();
        }

        // Emulate activity

        private IEnumerator LeaveRoutine()
        {
            leaveRoutinActive = true;
            ChannelData activeChannelData = channels.FirstOrDefault(x => x.channelName == activeChannel);

            while (AllClients.Count > 0)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(10f, 20f));

                var clients = AllClients.Where(x => x.ActiveChannel == activeChannel).ToList();

                if (clients != null && clients.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, clients.Count);
                    clients[randomIndex].LeaveChannel(activeChannel);
                }

                if (!joinRoutinActive)
                    StartCoroutine(JoinRoutine());
            }

            if (!joinRoutinActive)
                StartCoroutine(JoinRoutine());

            if (!sendMessageRoutinActive)
                StartCoroutine(SendMessageRoutine());

            leaveRoutinActive = false;
        }

        private IEnumerator JoinRoutine()
        {
            joinRoutinActive = true;
          
            ChannelData activeChannelData = channels.FirstOrDefault(x => x.channelName == activeChannel);
            
            var nonJoined = AllClients.Where(x => x.ActiveChannel != activeChannel || string.IsNullOrEmpty(x.ActiveChannel)).ToList();
            int nonJoinedCount = nonJoined.Count;

            while (nonJoinedCount > 0)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 4f));
                List<Client> clients = new List<Client>();
                for (int j = 0; j < activeChannelData.clients.Count; j++)
                {
                    for (int i = 0; i < nonJoined.Count; i++)
                    {
                        if (AllClients[i].Data.ClientName == activeChannelData.clients[j].nickName)
                            clients.Add(AllClients[i]);
                    }
                }

                int randomIndex = UnityEngine.Random.Range(0, clients.Count);

                if (clients.Count > 0)
                {
                    var clientInChannel = AllClients.Where(x => x.Data.ClientName == clients[0].Data.ClientName);
                    JoinClient(clients[randomIndex], activeChannelData);
                    nonJoinedCount--;
                }

                if (!leaveRoutinActive)
                    StartCoroutine(LeaveRoutine());

                if (!sendMessageRoutinActive)
                    StartCoroutine(SendMessageRoutine());
            }

            if (!leaveRoutinActive)
                StartCoroutine(LeaveRoutine());

            if (!sendMessageRoutinActive)
                StartCoroutine(SendMessageRoutine());

            joinRoutinActive = false;
        }

        private IEnumerator SendMessageRoutine()
        {
            sendMessageRoutinActive = true;

            while (AllClients.Count > 0)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 5f));

                var clients = AllClients.Where(x => x.ActiveChannel == activeChannel).ToList();

                if (clients != null && clients.Count > 0)
                {
                    int randomClientIndex = UnityEngine.Random.Range(0, clients.Count);
                    int randomMessageIndex = UnityEngine.Random.Range(0, messageBase.messages.Count);

                    string messageText = messageBase.messages[randomMessageIndex];

                    Message message = new Message(clients[randomClientIndex].Data.ClientName, clients[randomClientIndex].Data.ClientRole, activeChannel, messageText);
                    AllMesssages.Add(message);
                    clients[randomClientIndex].SendMessageToAll(message);
                }
            }

            sendMessageRoutinActive = false;
        }
    }
}
