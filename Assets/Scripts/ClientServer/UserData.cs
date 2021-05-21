namespace ChatSim.ServerClient
{
    public struct UserData
    {
        public string ClientName;
        public ClientRole ClientRole;

        public UserData(string clientName, ClientRole clientRole)
        {
            ClientName = clientName;
            ClientRole = clientRole;
        }
    }
}