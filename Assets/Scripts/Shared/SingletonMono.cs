using UnityEngine;

namespace Game.Shared
{
    /// <summary>
    /// 继承自MonoBehaviour的单例
    /// </summary>
    public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = (T) this;
            }
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}