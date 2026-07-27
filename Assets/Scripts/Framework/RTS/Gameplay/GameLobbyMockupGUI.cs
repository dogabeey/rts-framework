using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.RTS
{
    /// <summary>
    /// A small runtime mockup for configuring a <see cref="GameLobby"/>.
    /// Attach it to the same GameObject as the lobby, then enter Play mode.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameLobby))]
    public sealed class GameLobbyMockupGUI : MonoBehaviour
    {
        private const float PanelWidth = 440f;

        [SerializeField] private GameLobby lobby;

        private Vector2 scrollPosition;
        private readonly List<MapController> availableMaps = new List<MapController>();
        private readonly List<Type> gameModeTypes = new List<Type>();

        private void Awake()
        {
            if (lobby == null)
            {
                lobby = GetComponent<GameLobby>();
            }

            RefreshGameModeTypes();
        }

        private void OnGUI()
        {
            if (lobby == null)
            {
                return;
            }

            var panelHeight = Mathf.Min(Screen.height - 24f, 720f);
            GUILayout.BeginArea(new Rect(12f, 12f, PanelWidth, panelHeight), GUI.skin.box);
            GUILayout.Label("Game Lobby Mockup", GUI.skin.label);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            DrawLobbySettings();
            DrawSlots();
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawLobbySettings()
        {
            GUILayout.Label("Lobby", GUI.skin.box);

            DrawGameMode();
            DrawMap();

            lobby.startingResources = IntField("Starting Resources", lobby.startingResources);
            lobby.startingPopulation = IntField("Starting Population", lobby.startingPopulation);
            lobby.maxPopulation = IntField("Max Population", lobby.maxPopulation);
        }

        private void DrawGameMode()
        {
            RefreshGameModeTypes();
            var labels = new string[gameModeTypes.Count + 1];
            labels[0] = "None";

            var selectedIndex = 0;
            for (var i = 0; i < gameModeTypes.Count; i++)
            {
                labels[i + 1] = gameModeTypes[i].Name;
                if (lobby.gameMode != null && lobby.gameMode.GetType() == gameModeTypes[i])
                {
                    selectedIndex = i + 1;
                }
            }

            var newIndex = Popup("Game Mode", selectedIndex, labels);
            if (newIndex != selectedIndex)
            {
                lobby.gameMode = newIndex == 0
                    ? null
                    : (GameMode)Activator.CreateInstance(gameModeTypes[newIndex - 1]);
                lobby.map = null;
            }

            if (lobby.gameMode != null)
            {
                GUILayout.Label(lobby.gameMode.Description, GUI.skin.box);
            }
        }

        private void DrawMap()
        {
            RefreshAvailableMaps();
            var labels = new string[availableMaps.Count + 1];
            labels[0] = "None";

            var selectedIndex = 0;
            for (var i = 0; i < availableMaps.Count; i++)
            {
                var map = availableMaps[i];
                labels[i + 1] = map == null || string.IsNullOrWhiteSpace(map.mapName)
                    ? "Unnamed Map"
                    : map.mapName;
                if (map == lobby.map)
                {
                    selectedIndex = i + 1;
                }
            }

            var newIndex = Popup("Map", selectedIndex, labels);
            if (newIndex != selectedIndex)
            {
                lobby.map = newIndex == 0 ? null : availableMaps[newIndex - 1];
            }

            if (lobby.map != null && !string.IsNullOrWhiteSpace(lobby.map.mapDescription))
            {
                GUILayout.Label(lobby.map.mapDescription, GUI.skin.box);
            }
        }

        private void DrawSlots()
        {
            GUILayout.Space(8f);
            GUILayout.Label("Player Slots", GUI.skin.box);

            if (lobby.slots == null)
            {
                lobby.slots = new List<GameLobby.SlotData>();
            }

            for (var i = 0; i < lobby.slots.Count; i++)
            {
                var slot = lobby.slots[i];
                if (slot == null)
                {
                    slot = new GameLobby.SlotData();
                    lobby.slots[i] = slot;
                }

                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Slot {i + 1}");
                if (GUILayout.Button("Remove", GUILayout.Width(80f)))
                {
                    lobby.slots.RemoveAt(i);
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    break;
                }
                GUILayout.EndHorizontal();

                slot.isHuman = Toggle("Human", slot.isHuman);
                slot.color = ColorField("Color", slot.color);
                slot.allianceID = IntField("Alliance ID", slot.allianceID);
                slot.mapSlotIndex = IntField("Map Slot Index", slot.mapSlotIndex);
                slot.difficultyLevel = Slider("Difficulty", slot.difficultyLevel, 1, 5);
                slot.resourceLevel = Slider("Resource Level", slot.resourceLevel, 1, 5);

                DrawFaction(slot);
                GUILayout.EndVertical();
            }

            if (GUILayout.Button("Add Slot"))
            {
                lobby.slots.Add(new GameLobby.SlotData());
            }
        }

        private static void DrawFaction(GameLobby.SlotData slot)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Faction", GUILayout.Width(140f));
            GUILayout.Label(slot.faction == null ? "None" : slot.faction.GetType().Name);
            if (GUILayout.Button(slot.faction == null ? "Create" : "Clear", GUILayout.Width(80f)))
            {
                slot.faction = slot.faction == null ? new Faction() : null;
            }
            GUILayout.EndHorizontal();
        }

        private void RefreshAvailableMaps()
        {
            availableMaps.Clear();
            if (lobby.map != null)
            {
                availableMaps.Add(lobby.map);
            }

            if (!MapsManager.Exists())
            {
                return;
            }

            var mapsManager = MapsManager.Instance;
            if (mapsManager.availableMapsPerGameMode == null)
            {
                return;
            }

            foreach (var mapSet in mapsManager.availableMapsPerGameMode)
            {
                if (mapSet?.availableMaps == null)
                {
                    continue;
                }

                foreach (var map in mapSet.availableMaps)
                {
                    if (map != null && !availableMaps.Contains(map))
                    {
                        availableMaps.Add(map);
                    }
                }
            }
        }

        private void RefreshGameModeTypes()
        {
            gameModeTypes.Clear();
            foreach (var type in typeof(GameMode).Assembly.GetTypes())
            {
                if (!type.IsAbstract && typeof(GameMode).IsAssignableFrom(type)
                    && type.GetConstructor(Type.EmptyTypes) != null)
                {
                    gameModeTypes.Add(type);
                }
            }
            gameModeTypes.Sort((left, right) => string.Compare(left.Name, right.Name, StringComparison.Ordinal));
        }

        private static int IntField(string label, int value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(140f));
            var text = GUILayout.TextField(value.ToString());
            GUILayout.EndHorizontal();
            return int.TryParse(text, out var result) ? result : value;
        }

        private static int Slider(string label, int value, int min, int max)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(140f));
            value = Mathf.RoundToInt(GUILayout.HorizontalSlider(value, min, max));
            GUILayout.Label(value.ToString(), GUILayout.Width(24f));
            GUILayout.EndHorizontal();
            return value;
        }

        private static bool Toggle(string label, bool value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(140f));
            value = GUILayout.Toggle(value, string.Empty);
            GUILayout.EndHorizontal();
            return value;
        }

        private static int Popup(string label, int selectedIndex, string[] labels)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(140f));
            selectedIndex = GUILayout.SelectionGrid(selectedIndex, labels, 1);
            GUILayout.EndHorizontal();
            return selectedIndex;
        }

        private static Color ColorField(string label, Color value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(140f));
            value.r = FloatField("R", value.r);
            value.g = FloatField("G", value.g);
            value.b = FloatField("B", value.b);
            value.a = FloatField("A", value.a);
            GUILayout.EndHorizontal();
            return value;
        }

        private static float FloatField(string label, float value)
        {
            GUILayout.Label(label, GUILayout.Width(16f));
            var text = GUILayout.TextField(value.ToString("0.##"), GUILayout.Width(48f));
            return float.TryParse(text, out var result) ? Mathf.Clamp01(result) : value;
        }
    }
}
