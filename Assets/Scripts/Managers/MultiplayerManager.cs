using System;
using Photon.Pun;
using UnityEngine;

namespace Managers
{
    public class MultiplayerManager : MonoBehaviourPunCallbacks
    {
        public static MultiplayerManager Instance;
        
        public Action<GameObject> onGameObjectCreated;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }
        
        private void Start()
        {
            ConnectUsingSetting();
            PhotonNetwork.onGameObjectCreated += GameObjectCreated;
        }

        public void ConnectUsingSetting()
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        
        public override void OnConnectedToMaster()
        {
            JoinLobby();
        }

        private void JoinLobby()
        {
            PhotonNetwork.JoinLobby();
        }
        
        public override void OnJoinedLobby()
        {
            PhotonNetwork.LoadLevel("Lobby");
        }
        
        public void CreateRoom(string text)
        {
            PhotonNetwork.CreateRoom(text);
        }

        public void JoinRoom(string text)
        {
            PhotonNetwork.JoinRoom(text);
        }
        
        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel("Main");
        }
        
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);
            Debug.Log(message);
        }

        public bool IsMasterClient()
        {
            return PhotonNetwork.IsMasterClient;
        }

        public void GameObjectCreated(GameObject obj)
        {
            onGameObjectCreated?.Invoke(obj);
        }

        public GameObject Instantiate(string objectName, Vector3 position, Quaternion rotation)
        {
            return PhotonNetwork.Instantiate(objectName, position, rotation);
        }

        public void Destroy(GameObject obj)
        {
            if (IsMasterClient())
                PhotonNetwork.Destroy(obj);
        }
    }
}