using System.Collections.Generic;
using UnityEngine;

namespace ChatSim.Data
{
    [CreateAssetMenu(fileName = "ClientsBase", menuName = "ScriptableObjects/ClientsBase", order = 2)]
    public class ClientsBase : ScriptableObject
    {
        public List<ClientData> data;
    }
}
