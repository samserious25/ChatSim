using ChatSim.Data;
using ChatSim.ServerClient;
using ChatSim.UI;
using System.Collections.Generic;

[System.Serializable]
public class ChannelData
{
    public string channelName;
    public List<ClientData> clients;
}
