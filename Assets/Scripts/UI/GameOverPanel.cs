using System;
using Game.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _killNumberText;
        [SerializeField] private TextMeshProUGUI _killNumberRecordText;
        [SerializeField] private GameObject _newRecord;
        [SerializeField] private Button _tryAgainButton;
        [SerializeField] private Button _exitGameButton;

        private void Awake()
        {
            _tryAgainButton.onClick.AddListener(TryAgain);
            _exitGameButton.onClick.AddListener(ExitGame);
        }

        private void TryAgain()
        {
            GameManager.Instance.ReplayGame();
        }
        
        private void ExitGame()
        {
            GameManager.Instance.ExitGame();
        }

        public void Show(int killNum, int record)
        {
            _killNumberText.text = $"击杀数：<color=yellow>{killNum}</color>";

            var isNew = killNum > record;

            _killNumberRecordText.text = $"最高记录：{(isNew ? killNum : record)}";
            _newRecord.SetActive(isNew);
            
            gameObject.SetActive(true);
        }
    }
}