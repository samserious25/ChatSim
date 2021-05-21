using System;

namespace ChatSim.ServerClient
{
    public struct Message
    {
        public string ClientName;
        public ClientRole ClientRole;
        public string ChannelName;
        public string Text;
        public string Date;

        public Message(string clientName, ClientRole clientRole, string channelName, string text)
        {
            ClientName = clientName;
            ClientRole = clientRole;
            ChannelName = channelName;
            Text = text;
            Date = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
