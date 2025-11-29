using UnityEngine;

namespace WereHorse.Runtime {
    public static class GameManager {
        public static void ExitGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}