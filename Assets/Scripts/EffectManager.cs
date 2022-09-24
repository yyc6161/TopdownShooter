using Game.Shared;
using Game.UI;
using Game.WeaponSystem;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Common
{
    public class EffectManager : SingletonMono<EffectManager>
    {
        [Header("UI")] 
        [SerializeField] private PlayerHurtPanel _playerHurtPanel;
        [SerializeField] private SuperModePanel _superModePanel;
        
        [Header("对象池预制体")]
        [SerializeField] private ParticleSystem _smogPrefab;
        [SerializeField] private ParticleSystem _bulletHitEffectPrefab;
        [SerializeField] private ParticleSystem _explosionPrefab;
        [SerializeField] private Transform _particleTransform;
        [SerializeField] private ExplosionTrack _explosionTrackPrefab;

        [Header("弹壳")]
        [SerializeField] private Shell _shellPrefab;
        [SerializeField] private Transform _shellParentTransform;
        [SerializeField] private float _shellKeepTime = 20f;
        
        [Header("音效")] 
        [SerializeField] private AudioSource _soundAudioSource;
        [SerializeField] private AudioClip _hitSound;
        [SerializeField] private AudioClip _explosionSound;

        private ObjectPool<ParticleSystem> _smogPool;
        private ObjectPool<ParticleSystem> _bulletHitPool;
        private ObjectPool<ParticleSystem> _explosionPool;
        private ObjectPool<Shell> _shellPool;
        private ObjectPool<ExplosionTrack> _explosionTrackPool;

        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        public void ShowPlayerHurtScreenEffect()
        {
            _playerHurtPanel.Show();
        }

        public void SpawnSmog(Vector3 pos)
        {
            SpawnParticleEffect(pos, _smogPool);
        }
        
        public void SpawnHitEffect(Vector3 pos)
        {
            SpawnParticleEffect(pos, _bulletHitPool);
            
            _soundAudioSource.clip = _hitSound;
            _soundAudioSource.Play();
        }

        public void SpawnExplosionEffect(Vector3 pos)
        {
            SpawnParticleEffect(pos, _explosionPool);

            var track = _explosionTrackPool.Get();
            track.transform.position = pos;
            track.Show();
            StartCoroutine(Helper.DelayToInvoke(() =>
            {
                _explosionTrackPool.Release(track);
            }, track.TotalTime));
            
            _soundAudioSource.clip = _explosionSound;
            _soundAudioSource.Play();
        }

        public void SpawnShell(Vector3 spawnPos, Vector2 direction)
        {
            var obj = _shellPool.Get();
            obj.transform.position = spawnPos;
            obj.FlyTo(direction);
            
            StartCoroutine(Helper.DelayToInvoke(() =>
            {
                _shellPool.Release(obj);
            }, _shellKeepTime));
        }

        private void SpawnParticleEffect(Vector3 pos, ObjectPool<ParticleSystem> pool)
        {
            var obj = pool.Get();
            obj.transform.position = pos;

            var lifeTime = obj.main.duration;
            
            if (obj.main.loop == false && lifeTime > 0)
            {
                StartCoroutine(Helper.DelayToInvoke(() =>
                {
                    pool.Release(obj);
                }, lifeTime));
            }
        }

        private void Init()
        {
            _smogPool = InitParticlePool(_smogPrefab);
            _bulletHitPool = InitParticlePool(_bulletHitEffectPrefab);
            _explosionPool = InitParticlePool(_explosionPrefab);

            _shellPool = InitPool(_shellPrefab, _shellParentTransform);
            _explosionTrackPool = InitPool(_explosionTrackPrefab, _particleTransform);
            
            GameManager.Instance.EnergyBar.OnSuperChanged += OnSuperChanged;
        }

        private void OnSuperChanged(bool active)
        {
            if (active)
            {
                _superModePanel.Show();
            }
            else
            {
                _superModePanel.Hide();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            // if (GameManager.Instance.EnergyBar != null)
            // {
            //     GameManager.Instance.EnergyBar.OnSuperChanged -= OnSuperChanged;
            // }
        }

        private ObjectPool<ParticleSystem> InitParticlePool(ParticleSystem prefab)
        {
            return new ObjectPool<ParticleSystem>(() =>
                {
                    var obj = Instantiate(prefab, _particleTransform);
                    return obj;
                },
                obj => obj.gameObject.SetActive(true),
                obj => obj.gameObject.SetActive(false),
                obj => DestroyImmediate(obj.gameObject),
                false);
        }
        
        private ObjectPool<T> InitPool<T>(T prefab, Transform parent) where T : MonoBehaviour
        {
            return new ObjectPool<T>(() =>
                {
                    var obj = Instantiate(prefab, parent);
                    return obj;
                },
                obj => obj.gameObject.SetActive(true),
                obj => obj.gameObject.SetActive(false),
                obj => DestroyImmediate(obj.gameObject),
                false);
        }
    }
}