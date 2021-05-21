using System;
using UnityEngine;

namespace ChatSim.ServerClient
{
    public class Client : ClientBase
    {
        public static event Action<UserData, ConnectionStatus> ConnectionRequest;
        public static event Action<UserData, string, ConnectionStatus> JoinRequest;
        public static event Action<Message> MessageSend;
        public event Action<string, ClientOwner, ConnectionStatus> ConnectedToServer;
        public event Action<string, string, ClientOwner, ConnectionStatus> JoinedChannel;
        public event Action<Message> MessageRecieved;

        public bool IsMine { get; private set; }
        public bool Connected { get; private set; }

        public Client(UserData clientData, bool isMine = false) : base(clientData)
        {
            IsMine = isMine;

            Server.ClientConnectionStatus += OnConnectionStatus;
            Server.ClientJoinStatus += OnJoinStatus;
            Server.SendMessageToAll += OnRecieveMessage;
        }

        public void Connect()
        {
            if (Connected)
                return;

            ConnectionRequest?.Invoke(Data, ConnectionStatus.Connection);
        }

        public void Disconnect()
        {
            if (!Connected)
                return;

            ConnectionRequest?.Invoke(Data, ConnectionStatus.Disconnection);
        }

        public void JoinChannel(string channelName)
        {
            if (!Connected)
                return;

            if (JoinedChannels.Contains(channelName))
                return;

            JoinRequest?.Invoke(Data, channelName, ConnectionStatus.JoinChannel);
        }

        public void LeaveChannel(string channelName)
        {
            if (!Connected)
                return;

            JoinRequest?.Invoke(Data, channelName, ConnectionStatus.LeaveChannel);
        }

        public void SendMessageToAll(Message message)
        {
            if (!Connected)
                return;

            MessageSend?.Invoke(message);
        }

        private void OnRecieveMessage(Message message)
        {
            MessageRecieved?.Invoke(message);
            //Debug.Log(Data.ClientName + ": " + message.ClientName + " " + message.Text);
        }

        private void OnConnectionStatus(UserData clientData, ConnectionStatus connectionStatus)
        {
            switch (connectionStatus)
            {
                case ConnectionStatus.Connection:
                    OnConnectedToServer(clientData);
                    break;
                case ConnectionStatus.Disconnection:
                    OnDisconnectedFromServer(clientData);
                    break;
            }
        }

        private void OnConnectedToServer(UserData clientData)
        {
            ClientOwner clientOwner = clientData.ClientName != Data.ClientName ? ClientOwner.Other : ClientOwner.Mine;

            if (clientOwner == ClientOwner.Mine)
                Connected = true;

            ConnectedToServer?.Invoke(clientData.ClientName, clientOwner, ConnectionStatus.Connection);
            // Debug.Log(Data.ClientName + " " + "connected to server");
        }

        private void OnDisconnectedFromServer(UserData clientData)
        {
            ClientOwner clientOwner = clientData.ClientName != Data.ClientName ? ClientOwner.Other : ClientOwner.Mine;

            if (clientOwner == ClientOwner.Mine)
            {
                Connected = false;
                ActiveChannel = null;
                JoinedChannels.Clear();

                Server.ClientConnectionStatus -= OnConnectionStatus;
                Server.ClientJoinStatus -= OnJoinStatus;
                Server.SendMessageToAll -= OnRecieveMessage;
            }

            ConnectedToServer?.Invoke(clientData.ClientName, clientOwner, ConnectionStatus.Disconnection);
            //Debug.Log(Data.ClientName + " " + "disconnected from server");
        }

        private void OnJoinStatus(UserData clientData, ConnectionStatus connectionStatus, string channelName)
        {
            switch (connectionStatus)
            {
                case ConnectionStatus.JoinChannel:
                    OnJoinedChannel(clientData, channelName);
                    break;
                case ConnectionStatus.LeaveChannel:
                    OnLeftChannel(clientData, channelName);
                    break;
            }
        }

        private void OnJoinedChannel(UserData clientData, string channelName)
        {
            ClientOwner clientOwner = clientData.ClientName != Data.ClientName ? ClientOwner.Other : ClientOwner.Mine;

            if (clientOwner == ClientOwner.Mine)
            {
                if (!JoinedChannels.Contains(channelName))
                {
                    JoinedChannels.Add(channelName);
                    SetActiveChannel(channelName);

                    JoinedChannel?.Invoke(clientData.ClientName, channelName, clientOwner, ConnectionStatus.JoinChannel);
                }
            }
            else
                JoinedChannel?.Invoke(clientData.ClientName, channelName, clientOwner, ConnectionStatus.JoinChannel);

            //Debug.Log(Data.ClientName + " joined channel" + " '" + channelName + "'");
        }

        private void OnLeftChannel(UserData clientData, string channelName)
        {
            ClientOwner clientOwner = clientData.ClientName != Data.ClientName ? ClientOwner.Other : ClientOwner.Mine;

            if (clientOwner == ClientOwner.Mine)
            {
                if (JoinedChannels.Contains(channelName))
                {
                    JoinedChannels.Remove(channelName);

                    if (JoinedChannels.Count > 0)
                        ActiveChannel = JoinedChannels[0];
                    else
                        ActiveChannel = null;

                    JoinedChannel?.Invoke(clientData.ClientName, channelName, clientOwner, ConnectionStatus.LeaveChannel);
                }
            }

            JoinedChannel?.Invoke(clientData.ClientName, channelName, clientOwner, ConnectionStatus.LeaveChannel);
            //Debug.Log(Data.ClientName + " left channel" + " '" + channelName + "'");
        }
    }
}