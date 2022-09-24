using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class EnergyBar : MonoBehaviour
    {
        public Action<bool> OnSuperChanged;

        [SerializeField] private Slider _slider;
        [SerializeField] private float _maxEnergy = 100;
        [SerializeField] private float _fillSpeed;
        [SerializeField] private float _fadeSpeed;
        [SerializeField] private float _consumeSpeed;
        [SerializeField] private float _superConsumeSpeed;
        
        [Header("模式改变时变化")]
        [SerializeField] private Image _fillImage;
        [SerializeField] private Color _chargeColor;
        [SerializeField] private Color _consumeColor;

        private float _targetEnergy;
        private float _currentEnergy;
        
        public bool IsSuperMode { get; private set; }

        public void Charge()
        {
            if (IsSuperMode) return;
            
            _targetEnergy += _fillSpeed;
        }

        private void Update()
        {
            UpdateBar();
        }

        private void UpdateBar()
        {
            _targetEnergy -= (IsSuperMode? _superConsumeSpeed : _consumeSpeed) * Time.deltaTime;
            _targetEnergy = Mathf.Clamp(_targetEnergy, 0, _maxEnergy);

            _currentEnergy = Mathf.MoveTowards(_currentEnergy, _targetEnergy, _fadeSpeed * Time.deltaTime);

            var newValue = _currentEnergy / _maxEnergy;

            if (IsSuperMode && _slider.value > 0 && newValue == 0)
            {
                // 超级时间结束
                _fillImage.color = _chargeColor;
                IsSuperMode = false;
                OnSuperChanged?.Invoke(false);
            }

            if (!IsSuperMode && Math.Abs(newValue - 1) < 0.001)
            {
                // 超级时间开始
                _fillImage.color = _consumeColor;
                IsSuperMode = true;
                OnSuperChanged?.Invoke(true);
            }
            
            _slider.value = newValue;
        }
    }
}