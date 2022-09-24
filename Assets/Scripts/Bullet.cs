using System;
using Game.CharacterControl;
using Game.Common;
using Game.Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.WeaponSystem
{
    public enum BulletEffectType
    {
        None,
        Spark,
        Explosion
    }
    
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _holdSeconds = 2;
        [SerializeField] private int _bulletForce = 500;
        [SerializeField] private int _damageAmount = 10;
        [SerializeField] private float _explosionDamageMultiplier = 1.25f;
        [SerializeField] private int _knockbackForce = 400;
        [SerializeField] private Rigidbody2D _rigidbody;

        private bool _isFiring;
        private float _fireTime;

        public void DoFire(Vector3 firePosition, Vector3 fireDirection)
        {
            transform.position = firePosition;
            var fireDiection2D = new Vector2(fireDirection.x, fireDirection.y).normalized;
            _rigidbody.AddForce(fireDiection2D * _bulletForce);

            _isFiring = true;
            _fireTime = 0;
        }

        public void Stop(BulletEffectType effectType = BulletEffectType.None)
        {
            _rigidbody.velocity = Vector2.zero;
            _isFiring = false;

            switch (effectType)
            {
                case BulletEffectType.Spark:
                    EffectManager.Instance.SpawnHitEffect(transform.position);
                    break;
                case BulletEffectType.Explosion:
                    EffectManager.Instance.SpawnExplosionEffect(transform.position);
                    break;
            }
            
            BulletManager.Instance.ReleaseBullet(this);
        }

        private void Update()
        {
            if (_isFiring)
            {
                _fireTime += Time.deltaTime;
                
                if (_fireTime >= _holdSeconds)  Stop();
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag.Equals("Enemy")) // || col.tag.Equals("Player"))
            {
                var damagebleObj = col.GetComponent<CharacterContrellerBase>();
                if (damagebleObj == null || damagebleObj.IsDied) return;

                var knockbackDirection = damagebleObj.Position - _rigidbody.position;
                
                var isExplode = Random.Range(0, 100) < 26;
                var damageAmount = isExplode ? (int)(_damageAmount * _explosionDamageMultiplier) : _damageAmount;
                
                damagebleObj.TakeDamage(damageAmount);
                damagebleObj.KnockBack(knockbackDirection, _knockbackForce);

                GameManager.Instance.Freeze(0.03f);
                
                StartCoroutine(Helper.DelayToInvoke(() =>
                {
                    Stop(isExplode ? BulletEffectType.Explosion : BulletEffectType.Spark);
                }, 0.05f));
            }
            else if (col.tag.Equals("Wall"))
            {
                StartCoroutine(Helper.DelayToInvoke(() =>
                {
                    Stop(BulletEffectType.Spark);
                }, 0.01f));
            }
        }
    }
}