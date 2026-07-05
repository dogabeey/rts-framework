using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Singleton;
using Game.SimpleJSON;
using Game.SaveManagement;
using Game.EventManagement;
using UnityEngine.UI;
using Sirenix.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.GameManagement
{
    public interface IManager
    {
        public void OnInit();
        public void OnUpdate();
        public void OnApplicationPause();
        public void OnApplicationQuit();
    }

    public class GameManager : SingletonComponent<GameManager>
    {
        [Header("References")]
        public List<World> worlds;
        public Transform levelContainer;
        public ParticleSystem winParticle;

        private World currentWorld;

        public World CurrentWorld
        {
            get
            {
                return currentWorld;
            }
            set
            {

                currentWorld = value;
            }
        }
        protected override void Awake()
        {
            base.Awake();

            Application.targetFrameRate = 60;
            if(!worlds.IsNullOrEmpty())
            {
                CurrentWorld = worlds[0];
                if (!FindAnyObjectByType<LevelScene>())
                    LoadCurrentLevel();
            }

        }
        public void LoadLevel(LevelScene levelScene)
        {
            EndCurrentLevel();
            World.Instance.CurrentLevel = Instantiate(levelScene, levelContainer);
        }
        public void LoadCurrentLevel()
        {
            LoadLevel(FindCurrentLevel());
        }
        public void EndCurrentLevel()
        {
            if (World.Instance.CurrentLevel != null)
            {
                Destroy(World.Instance.CurrentLevel.gameObject);
                World.Instance.CurrentLevel = null;

            }
        }
        public void LoadNextLevel()
        {
            if (World.Instance.CurrentLevel != null)
            {
                LoadLevel(FindNextLevel());
            }
        }
        public void ResetCurrentLevel()
        {
            if (World.Instance.CurrentLevel != null)
            {
                LoadLevel(FindCurrentLevel());
            }
        }
        private LevelScene FindCurrentLevel()
        {
            return World.Instance.levelScenes[World.Instance.lastPlayedLevelIndex % World.Instance.levelScenes.Count];
        }
        private LevelScene FindNextLevel()
        {
            World.Instance.lastPlayedLevelIndex++;
            return World.Instance.levelScenes[World.Instance.lastPlayedLevelIndex % World.Instance.levelScenes.Count];
        }

    }
}

