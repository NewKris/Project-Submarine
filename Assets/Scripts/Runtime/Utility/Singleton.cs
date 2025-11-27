using UnityEngine;

namespace WereHorse.Runtime.Utility {
    public static class Singleton {
        public static bool MakeSingleton<T>(ref T singleton, T instance) where T : MonoBehaviour {
            if (singleton) {
                Object.Destroy(instance.gameObject);
                return false;
            }
            else {
                Object.DontDestroyOnLoad(instance.gameObject);
                singleton = instance;
                return true;
            }
        }

        public static bool RemoveSingleton<T>(ref T singleton, T instance) where T : MonoBehaviour {
            if (singleton == instance) {
                singleton = null;
                return true;
            }

            return false;
        }
    }
}