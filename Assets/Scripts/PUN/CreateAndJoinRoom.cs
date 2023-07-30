using Managers;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace PUN
{
    public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_InputField createInputField;
        [SerializeField] private TMP_InputField joinInputField;

        private MultiplayerManager _multiplayerManager;

        private void Start()
        {
            _multiplayerManager = MultiplayerManager.Instance;
        }

        public void CreateRoom()
        {
            _multiplayerManager.CreateRoom(createInputField.text);
        }

        public void JoinRoom()
        {
            _multiplayerManager.JoinRoom(joinInputField.text);
        }
    }
}