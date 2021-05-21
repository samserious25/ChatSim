using ChatSim.ServerClient;
using System.Collections.Generic;

namespace ChatSim.Data
{
    public class ChatHistory
    {
        public string ChannelName { get; private set; }
        public List<Message> Messages { get; private set; }

        public ChatHistory(string channelName)
        {
            ChannelName = channelName;
        }

        public void Save(List<Message> messages)
        {
            Messages = new List<Message>();
            Messages.AddRange(messages);
        }
    }
}
