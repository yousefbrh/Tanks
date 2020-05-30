using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PerkManager : MonoBehaviour
{
    public GameObject HealthPerk;
    public float m_WaitingForNextSpawn = 10f;

    public float xMin;
    public float xMax;

    
    public float zMin;
    public float zMax;
    public float m_CountDown = 10f;
    private WaitForSeconds m_DestroyCountDown = new WaitForSeconds(20);

    private int count;
    // Update is called once per frame
    void Update()
    {
        m_CountDown -= Time.deltaTime;
        if (m_CountDown <= 0 && count <= 5)
        {
            SpawnHealthPerk();
            m_CountDown = m_WaitingForNextSpawn;
        }

    }

    private void SpawnHealthPerk()
    {
        GameObject gameObject;
        Vector3 pos = new Vector3(Random.Range(xMin , xMax) , HealthPerk.transform.position.y , Random.Range(zMin , zMax));
        gameObject = Instantiate(HealthPerk, pos, Quaternion.identity);
        count++;
        StartCoroutine(DestroyHealthPerk(gameObject));
    }

    private IEnumerator DestroyHealthPerk(GameObject gameObject)
    {
        yield return m_DestroyCountDown;
        Destroy(gameObject);
        count--;
    }
}
