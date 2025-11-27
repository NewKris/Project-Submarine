using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WereHorse.Runtime.Ui {
    public class MenuController : MonoBehaviour {
        public void JoinLobbyAsOffline() {
            GameManager.clientType = ClientType.OFFLINE;
            SceneManager.LoadScene("Lobby");
        }
        
        public void JoinLobbyAsClient() {
            GameManager.clientType = ClientType.CLIENT;
            NetworkManager.Singleton.StartClient();
        }
        
        public void JoinLobbyAsHost() {
            GameManager.clientType = ClientType.HOST;
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
        
        public void ExitGame() {
            GameManager.ExitGame();
        }
    }
}