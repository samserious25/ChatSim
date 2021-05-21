using UnityEngine;
using ChatSim.ServerClient;

namespace ChatSim.Data
{
    [CreateAssetMenu(fileName = "Client", menuName = "ScriptableObjects/Client", order = 0)]
    public class ClientData : ScriptableObject
    {
        public Sprite avatar;
        public string nickName = "Client";
        public ClientRole clientRole = ClientRole.User;
    }
}
