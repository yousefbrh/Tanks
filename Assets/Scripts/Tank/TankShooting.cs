using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public Rigidbody m_Shell;
    public Transform m_FireTransform;
    public Slider m_HeatSlider;
    public Slider m_AimSlider;
    public AudioSource m_ShootingAudio;
    public AudioClip m_ChargingClip;
    public AudioClip m_FireClip;
    public float m_MinLaunchForce = 15f;
    public float m_MaxLaunchForce = 30f;
    public float m_MaxChargeTime = 0.75f;
    public Color m_FullHeatColor = Color.red;
    public Color m_ZeroHeatColor = Color.yellow;
    public int m_PushFireKeyLimit = 7;
    public Image m_FillImage;

    [SerializeField]private string m_FireButton;
    private float m_CurrentLaunchForce;
    private float m_ChargeSpeed;
    private bool m_Fired;
    private bool m_Heated;
    private WaitForSeconds m_CoolDown;
    private IEnumerator m_StopCoroutine;
    private PhotonView _view;

    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
        m_FireButton = "Fire2";
    }


    private void Start()
    {
        m_HeatSlider.value = 0;
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        m_CoolDown = new WaitForSeconds(5);
    }

    private void Update()
    {
        FillReduce();
    }
    
    private void Fire(float force)
    {
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

        shellInstance.velocity = force * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
    }

    private IEnumerator HeatHandle()
    {
        m_Heated = true;
        yield return m_CoolDown;
    }

    private IEnumerator FireHandle()
    {
        if (!_view)
        {
            _view = GetComponent<PhotonView>();
        }
        m_Heated = false;
        while (m_HeatSlider.value < 99)
        {
            m_AimSlider.value = m_MinLaunchForce;

            if (_view.IsMine)
            {
                if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
                {
                    m_Fired = true;
                    _view.RPC("ReachedMaxLunchedForce", RpcTarget.AllViaServer);
                }
                else if (Input.GetButtonDown(m_FireButton))
                {
                    _view.RPC("ButtonDown", RpcTarget.AllViaServer);
                }
                else if (Input.GetButton(m_FireButton) && !m_Fired)
                {
                    m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
                    m_AimSlider.value = m_CurrentLaunchForce;
                }
                else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
                {
                    _view.RPC("ButtonUp", RpcTarget.AllViaServer, m_CurrentLaunchForce);
                }
            }

            m_FillImage.color = Color.Lerp(m_ZeroHeatColor, m_FullHeatColor, m_HeatSlider.value/m_HeatSlider.maxValue);
            yield return null;
        }
    }

    [PunRPC]
    private void ReachedMaxLunchedForce()
    {
        m_CurrentLaunchForce = m_MaxLaunchForce;
        Fire(m_CurrentLaunchForce);
        m_HeatSlider.value += m_HeatSlider.maxValue / m_PushFireKeyLimit;
    }

    [PunRPC]
    private void ButtonDown()
    {
        m_Fired = false;
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_ShootingAudio.clip = m_ChargingClip;
        m_ShootingAudio.Play();
    }

    [PunRPC]
    private void ButtonUp(float force)
    {
        Fire(force);
        m_HeatSlider.value += m_HeatSlider.maxValue / m_PushFireKeyLimit;
    }

    private void FillReduce()
    {
        if (!m_Heated)
        {
            m_HeatSlider.value -= 30 * Time.deltaTime;
        }
        else
        {
            m_HeatSlider.value -= 20 * Time.deltaTime;
        }
        m_FillImage.color = Color.Lerp(m_ZeroHeatColor, m_FullHeatColor, m_HeatSlider.value/m_HeatSlider.maxValue);
    }

    public void EnableCoroutine()
    {
        StartCoroutine(TankShootingLoop());
    }

    private IEnumerator TankShootingLoop()
    {
        yield return StartCoroutine(FireHandle());
        StopCoroutine(FireHandle());
        yield return StartCoroutine(HeatHandle());
        StopCoroutine(HeatHandle());
        EnableCoroutine();
    }

    public void DisableShooting()
    {
        StopAllCoroutines();
        m_HeatSlider.value = 0;
    }
}