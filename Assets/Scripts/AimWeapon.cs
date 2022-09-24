using System;
using System.Collections;
using Game.CharacterControl;
using Game.Common;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Game.WeaponSystem
{
    public class AimWeapon : MonoBehaviour
    {
        [SerializeField] private PlayerController _character;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Transform _gunTransform;
        [SerializeField, Range(0,1)] private float _recoilForce = 0.5f;
        [SerializeField] private float _recoilRecoverySpeed = 10;
        [SerializeField] private int _characterRecoilForce = 200;
        
        [Header("枪械配置")]
        [SerializeField] private SpriteRenderer _gunSprite;
        [SerializeField] private float _superGunFactor = 0.8f;
        [SerializeField] private Color _superGunColor;
        [SerializeField] private float _superGunScale = 1.25f;
        [SerializeField] private float _scaleTime = 1f;
        [SerializeField] private float _superRecoilFactor = 1.25f;

        [Header("射击视效")]
        [SerializeField] private SpriteRenderer _muzzleSprite;
        [SerializeField, Range(0, 1)] private float _muzzleFadeSpeed = 0.15f;
        [SerializeField] private float _bulletRandomRange = 0.5f;
        [SerializeField] private Transform _shellSpawnPoint;

        [Header("射击音效")]
        [SerializeField] private AudioClip _fireAudio;
        [SerializeField] private AudioClip _reloadAudio;
        [SerializeField] private AudioSource _audioSource;

        private Camera _mainCamera;
        private Vector3 _mousePos;
        private Transform _characterTransform;

        private ObjectPool<GameObject> _bulletPool;

        private const float DefaultScale = 1f; 
        private bool _isSuperGun;
        private float _targetScale = DefaultScale;
        private float _scaleProgress;

        private int _characterRealRecoilForce;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _muzzleSprite.color = Color.clear;
            _characterTransform = _character.transform;
            _scaleProgress = Mathf.Abs(_superGunScale - DefaultScale) / _scaleTime;
            _characterRealRecoilForce = _characterRecoilForce;
        }

        private void Update()
        {
            if (GameManager.Instance.GameState != GameState.Running) return;

            HandleScale();
            HandleAimDirection();
            HandleWeaponPosition();
        }

        public void Fire()
        {
            var characterPos = _characterTransform.position;
            var firePos = _firePoint.transform.position;
            
            var randomPos = Random.insideUnitCircle * _bulletRandomRange;
            var targetPos = new Vector3(_mousePos.x + randomPos.x, _mousePos.y + randomPos.y, 0);
            var fireDirection = targetPos - characterPos;
            BulletManager.Instance.FireBullet(firePos, fireDirection);

            if (_isSuperGun)
            {
                var verticalVector = Vector3.Cross(fireDirection, Vector3.forward) * _superGunFactor;
            
                randomPos = Random.insideUnitCircle * _bulletRandomRange;
                targetPos = new Vector3(_mousePos.x + randomPos.x, _mousePos.y + randomPos.y, 0);
                var vec1 = targetPos + verticalVector;
                var vec1Direction = vec1 - characterPos;
                BulletManager.Instance.FireBullet(firePos, vec1Direction);
            
                var vec2 = targetPos - verticalVector;
                var vec2Direction = vec2 - characterPos;
                BulletManager.Instance.FireBullet(firePos, vec2Direction);
            }

            PlayFireEffect();
        }

        public void ChangeWeapon()
        {
            _isSuperGun = !_isSuperGun;
            
            _audioSource.clip = _reloadAudio;
            _audioSource.Play();
            
            _gunSprite.color = _isSuperGun ? _superGunColor : Color.white;
            _targetScale = _isSuperGun ? _superGunScale : 1f;
            _characterRealRecoilForce = _isSuperGun ? (int)(_characterRecoilForce * _superRecoilFactor) : _characterRecoilForce;
        }

        public void SetSuperGunActive(bool active)
        {
            if (_isSuperGun == active) return;

            ChangeWeapon();
        }

        private void HandleAimDirection()
        {
            var mouseScreenPos = Mouse.current.position.ReadValue();
            if (mouseScreenPos.x < 0 || mouseScreenPos.x > Screen.width || mouseScreenPos.y < 0 || mouseScreenPos.y > Screen.height) return;

            _mousePos = _mainCamera.ScreenToWorldPoint(mouseScreenPos);

            var aimDirection = (_mousePos - transform.position).normalized;
            var angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, angle);

            var localScale = transform.localScale;
            localScale.y = angle > 90 || angle < -90 ? -Math.Abs(localScale.y) : Math.Abs(localScale.y);
            transform.localScale = localScale;
        }

        private void HandleWeaponPosition()
        {
            if (transform.localPosition == Vector3.zero) return;
            
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, _recoilRecoverySpeed * Time.deltaTime);
        }
        
        private IEnumerator MuzzleFade()
        {
            while(_muzzleSprite.color.a > 0)
            {
                var color = _muzzleSprite.color;
                color = new Color(color.r , color.g, color.b ,color.a - _muzzleFadeSpeed);
                _muzzleSprite.color = color;
                yield return new WaitForFixedUpdate();
            }
        }

        private void PlayFireEffect()
        {
            // 枪口闪光
            _muzzleSprite.color = Color.white;
            StartCoroutine(MuzzleFade());
            
            // 枪口冒烟
            var firePos = _firePoint.position;
            EffectManager.Instance.SpawnSmog(firePos);
            
            // 枪械后坐力
            var direction = (_gunTransform.position - firePos).normalized;
            transform.position += direction * _recoilForce;
            
            // 角色后坐力
            _character.KnockBack(direction, _characterRecoilForce);
            
            // 屏幕震动
            GameManager.Instance.ShakeCamera();
            
            // 枪击音效
            _audioSource.clip = _fireAudio;
            _audioSource.Play();
            
            // 生成弹壳
            EffectManager.Instance.SpawnShell(_shellSpawnPoint.position, _shellSpawnPoint.up);
        }

        private void HandleScale()
        {
            if (Math.Abs(transform.localScale.x - _targetScale) < 0.0001f) return;
            
            var scale = Mathf.MoveTowards(transform.localScale.x, _targetScale, Time.deltaTime * _scaleProgress);
            transform.localScale = Vector3.one * scale;
        }
    }
}