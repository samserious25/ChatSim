using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ChatSim.ServerClient
{
    public class Server
    {
        public static event Action<UserData, ConnectionStatus> ClientConnectionStatus;
        public static event Action<UserData, ConnectionStatus, string> ClientJoinStatus;
        public static event Action<Message> SendMessageToAll;
        public List<Channel> Channels { get; private set; }
        public List<ClientBase> ConnectedClients { get; private set; }

        public Server()
        {
            Channels = new List<Channel>();
            ConnectedClients = new List<ClientBase>();

            Client.ConnectionRequest += OnClientConnectionRequest;
            Client.JoinRequest += OnClientJoinRequest;
            Client.MessageSend += OnRecieveMessage;
        }

        private void AddPlayerToChannel(UserData clientData, string channelName)
        {
            Channel channel = GetChannel(channelName);

            if (channel == null)
            {
                channel = new Channel(channelName);
                Channels.Add(channel);
            }
            else
            {
                var client = channel.Clients.FirstOrDefault(x => x.Data.ClientName == clientData.ClientName);

                if (client != null)
                    return;
            }

            channel.AddClient(clientData);
        }

        public void OnClientConnectionRequest(UserData clientData, ConnectionStatus connectionStatus)
        {
            switch (connectionStatus)
            {
                case ConnectionStatus.Connection:
                    Connect(clientData);
                    break;
                case ConnectionStatus.Disconnection:
                    Disconnect(clientData);
                    break;
            }
        }

        private void Connect(UserData clientData)
        {
            var client = ConnectedClients.FirstOrDefault(x => x.Data.ClientName == clientData.ClientName);

            if (client != null)
                return;

            ConnectedClients.Add(new ClientBase(clientData));

            ClientConnectionStatus?.Invoke(clientData, ConnectionStatus.Connection);
        }

        public void Disconnect(UserData clientData)
        {
            for (int i = 0; i < Channels.Count; i++)
                LeaveChannel(clientData, Channels[i].Name);

            ConnectedClients.Remove(GetPlayer(clientData.ClientName));
            Debug.Log(clientData.ClientName + " disconnected");
            ClientConnectionStatus?.Invoke(clientData, ConnectionStatus.Disconnection);
        }

        public void OnClientJoinRequest(UserData clientData, string channelName, ConnectionStatus connectionStatus)
        {
            switch (connectionStatus)
            {
                case ConnectionStatus.JoinChannel:
                    JoinChannel(clientData, channelName);
                    break;
                case ConnectionStatus.LeaveChannel:
                    LeaveChannel(clientData, channelName);
                    break;
            }
        }

        private void JoinChannel(UserData clientData, string channelName)
        {
            AddPlayerToChannel(clientData, channelName);
            ClientJoinStatus?.Invoke(clientData, ConnectionStatus.JoinChannel, channelName);
        }

        private ClientBase GetPlayerWithChannel(string clientName, string channelName)
        {
            Channel channel = GetChannel(channelName);

            if (channel == null)
                return null;

            return channel.Clients.FirstOrDefault(x => x.Data.ClientName == clientName);
        }

        private ClientBase GetPlayer(string clientName)
        {
            return ConnectedClients.FirstOrDefault(x => x.Data.ClientName == clientName);
        }

        private void LeaveChannel(UserData clientData, string channelName)
        {
            ClientBase client = GetPlayerWithChannel(clientData.ClientName, channelName);

            if (client == null)
                return;

            RemoveClientFromChannel(client, channelName);
            ClientJoinStatus?.Invoke(clientData, ConnectionStatus.LeaveChannel, channelName);
        }

        private void RemoveClientFromChannel(ClientBase client, string channelName)
        {
            Channel channel = GetChannel(channelName);

            if (channel == null)
                return;

            ClientBase clientInChannel = GetPlayerWithChannel(channelName, channelName);

            if (clientInChannel == null)
                return;

            channel.RemoveClient(client);
        }

        private Channel GetChannel(string channelName)
        {
            return Channels.FirstOrDefault(x => x.Name == channelName);
        }

        private void OnRecieveMessage(Message message)
        {
            Channel channel = GetChannel(message.ChannelName);

            if (channel == null)
                return;

            SendMessage(message);
        }

        private void SendMessage(Message message)
        {
            SendMessageToAll?.Invoke(message);
        }
    }
}