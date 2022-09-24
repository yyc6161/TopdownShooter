using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.CharacterControl
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Slider _fadeSlider;
        [SerializeField] private float _fadeTime = 1f;
        
        // 最大生命值
        private int _maxHealth = 100;
        // 当前生命值
        private int _curHealth;
        private float _fadeProgress;

        private void Awake()
        {
            _curHealth = _maxHealth;
            _fadeSlider.value = 1;

            UpdateBar();
        }

        public void InitHealth(int cur, int max)
        {
            _curHealth = cur;
            _maxHealth = max;
            UpdateBar();
        }

        public void TakeDamage(int damage)
        {
            _curHealth -= damage;
            if (_curHealth <= 0)
            {
                _curHealth = 0;
            }

            UpdateBar();
        }

        private void Update()
        {
            if (_fadeProgress > 0)
            {
                _fadeSlider.value -= _fadeProgress * Time.deltaTime;

                if (_fadeSlider.value <= _healthSlider.value)
                {
                    _fadeSlider.value = _healthSlider.value;
                    _fadeProgress = 0;
                }
            }
        }

        private void UpdateBar()
        {
            float percent = _curHealth / (float)_maxHealth;
            _healthSlider.value = percent;
            _text.text = _curHealth+" / "+_maxHealth;

            _fadeProgress = (_fadeSlider.value - _healthSlider.value) / _fadeTime;
        }
    }
}