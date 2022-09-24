using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class SuperModePanel : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private float _fadeTime = 1f;

        private Color _defaultColor;
        private bool _isInit;
        private float _progress;

        private bool _isShow;
        private bool _isFadeIn;
        
        private void Init()
        {
            if (_isInit) return;

            _isInit = true;
            _defaultColor = _image.color;
            _progress = _image.color.a / _fadeTime;
        }
        
        public void Show()
        {
            Init();
            
            gameObject.SetActive(true);
            _image.color = _defaultColor;
            _isFadeIn = false;
            _isShow = true;
        }

        public void Hide()
        {
            _isShow = false;
            _image.color = Color.clear;
        }

        private void Update()
        {
            if (!_isShow) return;

            float newAlpha;
            if (_isFadeIn)
            {
                newAlpha = _image.color.a + _progress * Time.deltaTime;

                if (newAlpha >= _defaultColor.a) _isFadeIn = false;
            }
            else
            {
                newAlpha = _image.color.a - _progress * Time.deltaTime;
                
                if (newAlpha <= 0.3f) _isFadeIn = true;
            }
            
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, newAlpha);
        }
    }
}