using UnityEngine;

namespace DarkNaku {
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
        private static object _lock = new object();
        private static bool _initialized = false;
        private static bool _isDestroyed = false;

        private static T _instance = null;
        public static T Instance {
            get {
                lock (_lock) {
                    if (_isDestroyed) return null;

                    if (_instance == null) {
                        _instance = FindObjectOfType<T>();

                        if (_instance == null) {
                            GameObject go = new GameObject();
                            _instance = go.AddComponent<T>();
                            go.name = "[SINGLETON] " + typeof(T).ToString();
                        }

                        DontDestroyOnLoad(_instance.gameObject);
                    }

                    SingletonBehaviour<T> singleton = _instance as SingletonBehaviour<T>;

                    if (_initialized == false) {
                        singleton.OnInitialize();
                        _initialized = true;
                    }

                    return _instance;
                }
            }
        }

        protected void OnDestroy() {
            OnDestroying();
            _instance = null;
            _isDestroyed = true;
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnDestroying() { }
    }
}