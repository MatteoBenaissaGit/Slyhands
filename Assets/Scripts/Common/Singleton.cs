using UnityEngine;

namespace Common
{
    public abstract class Singleton<TClass> : MonoBehaviour where TClass : class
    {
        public static TClass Instance { get; private set; }

        [Header("Singleton"), SerializeField] protected bool isPersistant;

        protected Singleton() {}
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"More than one <b>\"{typeof(TClass).Name}\"</b> singleton in scene.", gameObject);
                Destroy(this);
                return;
            }
            
            Instance = this as TClass;

            if (isPersistant)
            {
                if (transform.parent != null)
                {
                    transform.parent = null;
                    Debug.LogWarning($"Singleton <b>\"{typeof(TClass).Name}\"</b> is a child, it has been removed from his parent.", gameObject);
                }
                DontDestroyOnLoad(this);
            }
            
            Application.quitting += ResetInstance;
            
            InternalAwake();
        }

        protected virtual void InternalAwake()
        {
            
        }

        protected virtual void OnDestroy()
        {
            if (isPersistant == false)
            {
                ResetInstance();
            }
        }

        public static void ResetInstance()
        {
            Instance = null;
        }

        public void DestroyInstance()
        {
            Destroy(gameObject);
            ResetInstance();
        }
    }
}