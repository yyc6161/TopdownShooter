using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.WeaponSystem
{
    public class Shell : MonoBehaviour
    {
        [SerializeField] private float _flyTime = 0.5f;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private int _velocityFactor = 2;
        [SerializeField] private float _randomFactor = 0.5f;
        
        [SerializeField] private Rigidbody2D _rigidbody;
        
        private bool _isFlying;
        private float _time;

        public void FlyTo(Vector2 direction)
        {
            _isFlying = true;
            _time = _flyTime;

            var randomPoint = Random.insideUnitCircle;
            direction += randomPoint * _randomFactor;
            
            _rigidbody.linearVelocity = direction * _velocityFactor;
        }
        
        private void FixedUpdate()
        {
            if (_isFlying)
            {
                _time -= Time.deltaTime;

                if (_time > 0)
                {
                    transform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);
                }
                else
                {
                    _rigidbody.linearVelocity = Vector2.zero;
                    _isFlying = false;
                }
            }
        }
    }
}