using System;
using Game.CharacterControl;
using Game.Shared;
using Game.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Common
{
    public enum GameState
    {
        Null,
        Pregame,
        Running,
        Paused
    }
    
    public class GameManager : SingletonMono<GameManager>
    {
        public Action<GameState, GameState> OnGameStateChanged;

        public const string SaveName = "PlayerRecord";
        
        public PlayerController Player;
        public CameraController CameraController;
        
        public AiController EnemyPrefab;
        public Transform EnemyPool;

        [Header("UI")] 
        [SerializeField] private TextMeshProUGUI _killNumberText;
        [SerializeField] private GameStartPanel _gameStartPanel;
        [SerializeField] private GameOverPanel _gameOverPanel;
        [SerializeField] private EnergyBar _energyBar;

        private float _frozenTime;
        private bool _isFrozen;

        private int _killNum;

        public EnergyBar EnergyBar => _energyBar;
        
        /// 当前游戏状态
        public GameState GameState { get; private set; } = GameState.Null;

        protected override void Awake()
        {
            base.Awake();
            
            _killNumberText.gameObject.SetActive(true);
            _gameOverPanel.gameObject.SetActive(false);

            SwitchGameState(GameState.Pregame);
        }

        private void Update()
        {
            if (GameState != GameState.Running) return;
            
            if (_isFrozen)
            {
                _frozenTime -= Time.unscaledDeltaTime;
                
                if (_frozenTime <= 0)
                {
                    _isFrozen = false;
                    Time.timeScale = 1;
                }
            }
        }
        
        public void SwitchGameState(GameState newState)
        {
            // 新状态与当前状态相同时，不做任何操作
            if (newState == GameState) return;
            
            var preState = GameState;
            GameState = newState;

            switch (newState)
            {
                case GameState.Pregame:
                    Time.timeScale = 0f;
                    _gameStartPanel.gameObject.SetActive(true);
                    break;
                case GameState.Running:
                    OnGameRunning();
                    break;
                case GameState.Paused:
                    OnPaused();
                    break;
            }
            
            OnGameStateChanged?.Invoke(preState, newState);
        }

        /// <summary>
        /// 冻结时间
        /// </summary>
        /// <param name="seconds">冻结的时间秒数，默认0.02s</param>
        public void Freeze(float seconds = 0.02f)
        {
            _isFrozen = true;
            _frozenTime = seconds;
            Time.timeScale = 0;
        }

        public void ShakeCamera(float shakeLevel = -1)
        {
            CameraController.Shake(shakeLevel);
        }

        public void GameOver()
        {
            _killNumberText.gameObject.SetActive(false);

            var record = PlayerPrefs.GetInt(SaveName, 0);
            _gameOverPanel.Show(_killNum, record);

            if (_killNum > record)
            {
                PlayerPrefs.SetInt(SaveName, _killNum);
            }
            
            SwitchGameState(GameState.Paused);
        }
        
        public void ExitGame()
        {
#if UNITY_EDITOR    //在编辑器模式下
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void ReplayGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            SwitchGameState(GameState.Running);
        }

        public void OnKillEnemy()
        {
            _killNum++;
            _killNumberText.text = $"击杀数：<color=yellow>{_killNum}</color>";
            
            _energyBar.Charge();
        }
        
        private void OnGameRunning()
        {
            Time.timeScale = 1;
        }

        private void OnPaused()
        {
            Time.timeScale = 0;
        }
    }
}