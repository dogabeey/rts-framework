using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.RTS
{
    public class GameLobby : MonoBehaviour
    {
        [Serializable]
        public class SlotData
        {
            public Faction faction;
            public bool isHuman;
            [ColorUsage(false, false)] public Color color = Color.white;
            public int allianceID;
            public int mapSlotIndex;
            [Range(1, 5)] public int difficultyLevel = 1;
            [Range(1, 5)] public int resourceLevel = 1;
        }

        public MapController map;
        public List<SlotData> slots = new List<SlotData>();
        public int startingResources = 1000;
        public int startingPopulation = 10;
        public int maxPopulation = 100;
        [SerializeReference] public GameMode gameMode;

        public List<MapController> GetAvailableMapsForGameMode(GameMode gameMode)
        {
            List<MapController> availableMaps = new List<MapController>();

            for (int i = 0; i < MapsManager.Instance.availableMapsPerGameMode.Count; i++)
            {
                MapsManager.AvailableMapsPerGameMode map = MapsManager.Instance.availableMapsPerGameMode[i];
                if (map.gameMode == gameMode)
                {
                    availableMaps.Add(map.availableMaps[i]);
                }
            }

            return availableMaps;
        }
    }
}
