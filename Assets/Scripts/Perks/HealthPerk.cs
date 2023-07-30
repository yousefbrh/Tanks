using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class HealthPerk : MonoBehaviour
{
    public LayerMask m_LayerMaskForAccess;
    public LayerMask m_LayerMaskForDeny;
    
    private void Start()
    {
        DestroyerWithDelay(10);
    }

    public bool CheckCollide(Vector3 pos)
    {
        return Physics.CheckSphere(pos, 5f, m_LayerMaskForDeny);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Shell"))
            return;
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2,
            Quaternion.identity, m_LayerMaskForAccess);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            if (!targetRigidbody)
            {
                continue;
            }
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
            if (!targetHealth)
            {
                continue;
            }

            targetHealth.TakeHealth(25f);
        }
        
        Destroyer();
    }

    private void DestroyerWithDelay(float time)
    {
        Invoke(nameof(Destroyer), time);
    }

    private void Destroyer()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(gameObject);
    }
}
