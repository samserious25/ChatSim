using System.Collections.Generic;

namespace ChatSim.ServerClient
{
    public class Channel
    {
        public List<ClientBase> Clients { get; private set; }
        public string Name { get; private set; }

        public Channel(string channelName)
        {
            Name = channelName;
            Clients = new List<ClientBase>();
        }

        public void AddClient(UserData clientData)
        {
            ClientBase client = new ClientBase(clientData);
            Clients.Add(client);
        }

        public void RemoveClient(ClientBase client)
        {
            if (!Clients.Contains(client))
                return;

            Clients.Remove(client);
        }
    }
}