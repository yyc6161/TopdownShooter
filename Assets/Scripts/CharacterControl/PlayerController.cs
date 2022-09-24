using System;
using Game.Common;
using Game.WeaponSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.CharacterControl
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : CharacterContrellerBase
    {
        [SerializeField] private AimWeapon _aimWeapon;
        [SerializeField] private float _fireInterval = 0.2f;

        [Header("UI")]
        [SerializeField] private HealthBar _healthBar;

        [Header("音效")]
        [SerializeField] private AudioClip _getHurtSound;
        [SerializeField] private AudioSource _audioSource;

        private bool _isOnFire;
        private float _fireTime;

        private PlayerInput _playerInput;

        protected override void Awake()
        {
            base.Awake();

            _healthBar.InitHealth(CurrentHealth, MaxHealth);

            _audioSource.clip = _getHurtSound;
            _playerInput = GetComponent<PlayerInput>();
            
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            GameManager.Instance.EnergyBar.OnSuperChanged += OnSuperChanged;
            
            _playerInput.enabled = GameManager.Instance.GameState == GameState.Running;
        }

        private void OnDestroy()
        {
            // GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        }

        private void Update()
        {
            if (GameManager.Instance.GameState != GameState.Running) return;
            
            if (_isOnFire)
            {
                _fireTime -= Time.deltaTime;

                if (_fireTime <= 0)
                {
                    _aimWeapon.Fire();
                    _fireTime = _fireInterval;
                }
            }
        }

        protected override void FixedUpdate()
        {
            if (IsTakingDamage == false)
            {
                HandleCharacterInput();
            }
            else
            {
                UpdateAnimation();
            }
        }

        public override void TakeDamage(int damageAmount)
        {
            base.TakeDamage(damageAmount);
            _healthBar.TakeDamage(damageAmount);
            PlayGetHurtSound();
            
            EffectManager.Instance.ShowPlayerHurtScreenEffect();
            GameManager.Instance.ShakeCamera(4);
        }
        
        protected override void Death()
        {
            base.Death();
            GameManager.Instance.GameOver();
        }

        private void HandleCharacterInput()
        {
            Rigidbody.MovePosition(Rigidbody.position + MoveVector * MoveSpeed * Time.deltaTime);
        }

        private void PlayGetHurtSound()
        {
            _audioSource.Play();
        }
        
        private void OnGameStateChanged(GameState preState, GameState newState)
        {
            _playerInput.enabled = newState == GameState.Running;
        }
        
        private void OnSuperChanged(bool isSuperMode)
        {
            _aimWeapon.SetSuperGunActive(isSuperMode);
        }

        #region 玩家输入事件

        private void OnMove(InputValue value)
        {
            MoveVector = value.Get<Vector2>();
            
            UpdateAnimation();
        }

        private void OnFire(InputValue value)
        {
            if (value.isPressed)
            {
                _aimWeapon.Fire();
                
                _isOnFire = true;
                _fireTime = _fireInterval;
            }
            else
            {
                _isOnFire = false;
            }
        }

        private void OnChangeWeapon(InputValue value)
        {
            _aimWeapon.ChangeWeapon();   // 先不使用快捷键切换，而使用能量条来切换
        }

        #endregion
    }
}
