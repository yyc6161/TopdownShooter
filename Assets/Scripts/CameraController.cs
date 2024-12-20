using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Game.Common
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Transform _followerTransform;
        [SerializeField] private Transform _mouseIndicatorTransform;
        [SerializeField] private float _smoothSpeed = 2f;
        [SerializeField, Range(0, 0.5f)] private float _progress = 0.5f;

        [Header("屏幕震动效果")] 
        [SerializeField] private float _shakeLevel = 2;
        [SerializeField] private float _shakeHoldTime = 0.15f;
        [SerializeField] private float _shakeFps = 45f;
        
        private Camera _camera;
        private Transform _transform;

        private bool _isShakeCamera;
        private float _shakeFrameTime;
        private float _shakingTimer;
        private float _frameTime;
        private float _shakeDelta = 0.005f;
        private Rect _defaultRect;
        private Rect _changeRect;

        private float _currentShakeLevel;

        private void Start()
        {
            _camera = Camera.main;
            _transform = transform;
            
            _defaultRect = _camera.rect;
            _shakeFrameTime = 1f / _shakeFps;
        }
        
        public void Shake(float shakeLevel = -1)
        {
            _isShakeCamera = true;
            
            _shakingTimer = _shakeHoldTime;
            _frameTime = _shakeFrameTime;
            _changeRect = _defaultRect;

            _currentShakeLevel = shakeLevel < 0 ? _shakeLevel : shakeLevel;
        }

        private void Update()
        {
            var mouseScreenPos = Mouse.current.position.ReadValue();
            var mousePos = _camera.ScreenToWorldPoint(mouseScreenPos);
            mousePos.z = 0;
            var followerTargetPos = (mousePos - _playerTransform.position) * _progress + _playerTransform.position;
            _mouseIndicatorTransform.position = mousePos;
            
            var smoothPos = Vector3.Lerp(_followerTransform.position, followerTargetPos, _smoothSpeed *  Time.deltaTime);
            _followerTransform.position = smoothPos;

            if (_isShakeCamera)
            {
                DoShakeCamera();
            }
        }

        private void LateUpdate()
        {
            var followerPos = _followerTransform.position;
            var newPos = new Vector3(followerPos.x, followerPos.y, _transform.position.z);

            _transform.position = newPos;
        }

        private void DoShakeCamera()
        {
            _shakingTimer -= Time.deltaTime;
            if (_shakingTimer <= 0)
            {
                _isShakeCamera = false;
                _camera.rect = _defaultRect;
            }
            else
            {
                _frameTime += Time.deltaTime;
                
                if (_frameTime < _shakeFrameTime) return;
                
                _frameTime -= _shakeFrameTime;
                _changeRect.x = _defaultRect.x + _shakeDelta * (_currentShakeLevel * (Random.value - 0.5f));
                _changeRect.y = _defaultRect.y + _shakeDelta * (_currentShakeLevel * (Random.value - 0.5f));

                _camera.rect = _changeRect;
            }
        }
    }
}