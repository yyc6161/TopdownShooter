using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PlayerHurtPanel : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private float _fadeTime = 1f;

        private Color _defaultColor;
        private bool _isInit;
        private float _progress;

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
        }

        private void Update()
        {
            if (_image.color.a > 0)
            {
                var newAlpha = _image.color.a - _progress * Time.deltaTime;
                _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, newAlpha);
            }
        }
    }
}