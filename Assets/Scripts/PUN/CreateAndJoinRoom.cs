using System;
using Managers;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace PUN
{
    public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_InputField createInputField;
        [SerializeField] private TMP_InputField joinInputField;
        [SerializeField] private GameObject roomIsFullText;
        [SerializeField] private GameObject roomCapacityPanel;
        [SerializeField] private TMP_Dropdown dropdown;

        private MultiplayerManager _multiplayerManager;
        private float _tempTime;
        private bool _canCountDown;

        private void Start()
        {
            _multiplayerManager = MultiplayerManager.Instance;
            _multiplayerManager.onRoomIsFull += RoomIsFullCallback;
        }

        private void Update()
        {
            if (!_canCountDown) return;
            _tempTime += Time.deltaTime;
            if (_tempTime >= 3f)
            {
                TurnOffRoomIsFullText();
                _canCountDown = false;
                _tempTime = 0;
            }
        }

        public void ShowRoomCapacityPanel()
        {
            roomCapacityPanel.SetActive(true);
        }

        public void CreateRoom()
        {
            var capacityText = dropdown.options[dropdown.value].text;
            var capacityInt = int.Parse(capacityText);
            _multiplayerManager.CreateRoom(createInputField.text, capacityInt);
        }

        public void JoinRoom()
        {
            _multiplayerManager.JoinRoom(joinInputField.text);
        }

        private void RoomIsFullCallback()
        {
            roomIsFullText.SetActive(true);
            _canCountDown = true;
            _tempTime = 0;
        }
        
        private void TurnOffRoomIsFullText()
        {
            roomIsFullText.SetActive(false);
        }
    }
}