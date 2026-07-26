using System.Collections.Generic;
using UnityEngine;

namespace Game.RTS
{
    public class GameLobby : MonoBehaviour
    {
        public class SlotData
        {
            public Faction faction;
            public bool isHuman;
            public Color color;
            public int allianceID;
            public int mapSlotIndex;
            [Range(1, 5)] public int difficultyLevel = 1;
            [Range(1, 5)] public int resourceLevel = 1;
        }

        public List<SlotData> slots = new List<SlotData>();
        public int startingResources = 1000;
        public int startingPopulation = 10;
        public int maxPopulation = 100;
        [SerializeReference] public GameMode gameMode;
    }
}
