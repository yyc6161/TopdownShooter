using Game.Shared;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.WeaponSystem
{
    public class BulletManager : SingletonMono<BulletManager>
    {
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _bulletPoolTransform;
        
        private ObjectPool<Bullet> _bulletPool;

        protected override void Awake()
        {
            base.Awake();
            InitBulletPool();
        }

        public void FireBullet(Vector3 firePosition, Vector3 fireDirection)
        {
            var bullet = _bulletPool.Get();
            bullet.DoFire(firePosition, fireDirection);
        }

        public void ReleaseBullet(Bullet bullet)
        {
            _bulletPool.Release(bullet);
        }

        private void InitBulletPool()
        {
            _bulletPool = new ObjectPool<Bullet>(() =>
                {
                    var obj = Instantiate(_bulletPrefab, _bulletPoolTransform);
                    return obj;
                },
                obj => obj.gameObject.SetActive(true),
                obj => obj.gameObject.SetActive(false),
                obj => DestroyImmediate(obj.gameObject),
                false);
        }
    }
}