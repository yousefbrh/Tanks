using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PUN
{
    public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_InputField createInputField;
        [SerializeField] private TMP_InputField joinInputField;

        public void CreateRoom()
        {
            PhotonNetwork.CreateRoom(createInputField.text);
        }

        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom(joinInputField.text);
        }

        public override void OnJoinedRoom()
        {
            SceneManager.LoadScene("Main");
        }
    }
}