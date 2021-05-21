using System.Collections.Generic;
using UnityEngine;

namespace ChatSim.Data
{
    [CreateAssetMenu(fileName = "MessageBase", menuName = "ScriptableObjects/MessageBase", order = 1)]
    public class MessageBase : ScriptableObject
    {
        public List<string> messages;
    }
}
