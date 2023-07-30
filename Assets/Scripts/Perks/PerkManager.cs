using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class PerkManager : MonoBehaviour
{
    public GameObject m_Health;
    public float xMin;
    public float xMax;
    public float zMin;
    public float zMax;
    
    private WaitForSeconds m_CountDown = new WaitForSeconds(5);
    private int m_Count;
    private HealthPerk m_HealthPerk;
    private Vector3 m_Position;
    private List<GameObject> m_Instance = new List<GameObject>();
    
    private void Start()
    {
        m_HealthPerk = m_Health.GetComponent<HealthPerk>();
    }

    private IEnumerator PerkLoopManager()
    {
        yield return m_CountDown;
        yield return StartCoroutine(SetPosition());
        StopCoroutine(SetPosition());
        yield return StartCoroutine(SpawnHealthPerk());
        StopCoroutine(SpawnHealthPerk());
        StopCoroutine(PerkLoopManager());
        EnableClass();
    }

    private IEnumerator SetPosition()
         {
             m_Position = new Vector3(Random.Range(xMin , xMax) , m_Health.transform.position.y , Random.Range(zMin , zMax));
             while (m_HealthPerk.CheckCollide(m_Position))
             {
                 m_Position = new Vector3(Random.Range(xMin , xMax) , m_Health.transform.position.y , Random.Range(zMin , zMax));
                 yield return null;
             }
         }
    private IEnumerator SpawnHealthPerk()
    {
        m_Instance.Add(PhotonNetwork.Instantiate(m_Health.name, m_Position, Quaternion.identity));
        yield break;
    }

    public void EnableClass()
    {
        StartCoroutine(PerkLoopManager());
    }

    public void DisableClass()
    {
        for (int i = 0; i < m_Instance.Count; i++)
        {
            PhotonNetwork.Destroy(m_Instance[i]);
        }
        StopAllCoroutines();
    }
}
