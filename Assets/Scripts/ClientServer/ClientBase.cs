using System.Collections.Generic;

namespace ChatSim.ServerClient
{
    public class ClientBase
    {
        public UserData Data { get; private set; }
        public List<string> JoinedChannels { get; private set; }
        public string ActiveChannel { get; protected set; }

        public ClientBase(UserData clientData)
        {
            Data = clientData;
            JoinedChannels = new List<string>();
        }

        public void SetActiveChannel(string channelName)
        {
            ActiveChannel = channelName;
        }
    }
}