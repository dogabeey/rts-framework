using System.Collections.Generic;
using Game.Singleton;
using UnityEngine;

namespace Game.RTS
{
    public class MapsManager : SingletonComponent<MapsManager>
    {
        public class AvailableMapsPerGameMode
        {
            public GameMode gameMode;
            public List<MapController> availableMaps = new List<MapController>();
        }

        public List<AvailableMapsPerGameMode> availableMapsPerGameMode = new List<AvailableMapsPerGameMode>();
    }
}
