using UnityEngine;

namespace Game.Common
{
    public class ExplosionTrack : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private float _fadeTime = 1f;
        [SerializeField] private float _keepTime = 10f;

        public float TotalTime => _fadeTime * 2 + _keepTime;

        private float _fadeInTime;
        private float _keepingTime;
        private float _fadeOutTime;
        private float _progress;
        
        public void Show()
        {
            var color = _sprite.color;
            _sprite.color = new Color(color.r, color.g, color.b, 0);

            _fadeInTime = _fadeTime;
            _fadeOutTime = _fadeTime;
            _keepingTime = _keepTime;
            _progress = 1 / _fadeTime;
        }

        private void Update()
        {
            if (_fadeInTime > 0)
            {
                _fadeInTime -= Time.deltaTime;
                
                var color = _sprite.color;
                var newAlpha = color.a + _progress * Time.deltaTime;
                _sprite.color = new Color(color.r, color.g, color.b, newAlpha);
            }
            else if (_keepingTime > 0)
            {
                _keepingTime -= Time.deltaTime;
            }
            else if (_fadeOutTime > 0)
            {
                _fadeOutTime -= Time.deltaTime;
                
                var color = _sprite.color;
                var newAlpha = color.a - _progress * Time.deltaTime;
                _sprite.color = new Color(color.r, color.g, color.b, newAlpha);
            }
        }
    }
}