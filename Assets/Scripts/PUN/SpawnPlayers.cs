using System;
using System.Collections.Generic;
using System.IO;
using DefaultNamespace;
using Photon.Pun;
using UnityEngine;

namespace PUN
{
    public class SpawnPlayers : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private List<Transform> placements;

        private void Start()
        {
            var target = placements.PickRandom();
            PhotonNetwork.Instantiate(playerPrefab.name, target.position, Quaternion.identity);
        }
    }
}