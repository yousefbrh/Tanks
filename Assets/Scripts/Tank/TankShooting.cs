using System;
using System.Collections;
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
    
    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;
    private bool m_Heated;
    private WaitForSeconds m_CoolDown;

    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;
        m_HeatSlider.value = 0;
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        StartCoroutine(Shooting());
        m_CoolDown = new WaitForSeconds(5);
    }
    private void Update()
    {
        FillReduce();
    }


    private void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;
        
        Rigidbody shellInstance = Instantiate(m_Shell , m_FireTransform.position , m_FireTransform.rotation) as Rigidbody;

        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
    }

    private IEnumerator Shooting()
    {
        yield return StartCoroutine(FireHandle());
    }
    private IEnumerator HeatHandle()
    {
        m_Heated = true;
        yield return m_CoolDown;
        StartCoroutine(FireHandle());
    }

    private IEnumerator FireHandle()
    {
        m_Heated = false;
        while (m_HeatSlider.value < 99)
        {
            m_AimSlider.value = m_MinLaunchForce;

            if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
            {
                m_CurrentLaunchForce = m_MaxLaunchForce;
                Fire();
                m_HeatSlider.value += m_HeatSlider.maxValue / m_PushFireKeyLimit;
            }
            else if (Input.GetButtonDown(m_FireButton))
            {
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;
                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play();
            }
            else if (Input.GetButton(m_FireButton) && !m_Fired)
            {
                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
                m_AimSlider.value = m_CurrentLaunchForce;
            }
            else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
            {
                Fire();
                m_HeatSlider.value += m_HeatSlider.maxValue / m_PushFireKeyLimit;
            }
            m_FillImage.color = Color.Lerp(m_ZeroHeatColor, m_FullHeatColor, m_HeatSlider.value);
            yield return null;
        }
        yield return StartCoroutine(HeatHandle());
        
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
    }
}