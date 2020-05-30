using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPerk : MonoBehaviour
{
    // public GameObject m_Tank;
    // public GameObject m_LevelArt;
    public LayerMask m_LayerMask;
    // Start is called before the first frame update

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        Collider[] colliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2,
            Quaternion.identity, m_LayerMask);
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
        Destroy(gameObject);
    }
}
