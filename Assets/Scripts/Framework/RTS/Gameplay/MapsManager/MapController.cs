using System.Collections.Generic;
using UnityEngine;

namespace Game.RTS
{
    public class MapController : MonoBehaviour
    {
        public class MapSlot
        {
            public int index;
            public Transform startPosition;
        }
        
        public string mapName;
        public string mapDescription;
        public List<MapSlot> mapSlots = new List<MapSlot>();
    }
}
