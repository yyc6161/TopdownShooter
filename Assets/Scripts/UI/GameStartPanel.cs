using Game.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameStartPanel : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        
        private void Awake()
        {
            _startButton.onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            GameManager.Instance.SwitchGameState(GameState.Running);
            gameObject.SetActive(false);
        }
    }
}