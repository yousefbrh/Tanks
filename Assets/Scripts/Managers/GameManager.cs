using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;              
    public GameObject m_TankPrefab;         
    public List<TankManager> m_Tanks;
    public GameObject m_PerkManager;
    public LayerMask tankLayer;


    private int m_RoundNumber;   
    private bool _isStarted;
    private bool _isFinished;       
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait; 
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    private PerkManager m_perk;
    private MultiplayerManager _multiplayerManager;

    public static GameManager Instance;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    private void Start()
    {
        _multiplayerManager = MultiplayerManager.Instance;
        m_perk = m_PerkManager.GetComponent<PerkManager>();
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);
        SpawnTank();
        SetCameraTargets();
        _multiplayerManager.onGameObjectCreated += SetupEnemies;
        StartCoroutine(GameLoop());
    }

    private void DisablePerk()
    {
        m_PerkManager.SetActive(false);
        m_perk.DisableClass();
    }

    private void EnablePerk()
    {
        if (MultiplayerManager.Instance.IsMasterClient())
        {
            m_PerkManager.SetActive(true);
            m_perk.EnableClass();
        }
    }
    
    private void SpawnTank()
    {
        var go = _multiplayerManager.Instantiate(m_TankPrefab.name, Vector3.zero, Quaternion.identity);
        SetupEnemies(go);
    }

    public void SetupEnemies(GameObject go)
    {
        if ((tankLayer & (1 << go.layer)) == 0) return;
        var view = go.GetComponent<PhotonView>();
        var target = m_Tanks.Find(t => t.ID == view.ViewID);
        go.transform.position = target.m_SpawnPoint.position;
        go.transform.rotation = target.m_SpawnPoint.rotation;
        target.m_Instance = go;
        target.Setup();
        SetCameraTargets();
    }

    private void SetCameraTargets()
    {
        var availableTargets = m_Tanks.FindAll(t => t.m_Instance != null);

        var targets = availableTargets.Select(availableTarget => availableTarget.m_Instance.transform).ToList();

        m_CameraControl.m_Targets = targets;
    }
    
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            SceneManager.LoadScene("Round " + m_RoundNumber , LoadSceneMode.Additive);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }
    
    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();
        DisablePerk();
        
        m_CameraControl.SetStartPositionAndSize();
        m_RoundNumber++;
        m_MessageText.text = "Round " + m_RoundNumber;
        yield return m_StartWait;
    }
    
    private IEnumerator RoundPlaying()
    {
        EnableTankControl();
        EnablePerk();
        
        m_MessageText.text = string.Empty;

        while (!_isFinished)
        {
            yield return null; 
        }
    }

    private IEnumerator RoundEnding()
    {
        DisableTankControl();
        DisablePerk();
        m_RoundWinner = null;
        m_RoundWinner = GetRoundWinner();

        if (m_RoundWinner != null)
        {
            m_RoundWinner.m_Wins++;
        }

        m_GameWinner = GetGameWinner();

        string message = EndMessage();

        m_MessageText.text = message;
        
        yield return m_EndWait;
    }

    private bool OneTankLeft()
    {
        var availableTanks = m_Tanks.FindAll(t => t.m_Instance != null);
        var numTanksLeft = availableTanks.Count(availableTank => availableTank.m_Instance.activeSelf);
        return numTanksLeft <= 1;
    }

    private TankManager GetRoundWinner()
    {
        var availableTanks = m_Tanks.FindAll(t => t.m_Instance != null);
        var winner = availableTanks.Find(t => t.m_Instance.activeSelf);
        return winner;
    }
    
    private TankManager GetGameWinner()
    {
        var availableTanks = m_Tanks.FindAll(t => t.m_Instance != null);
        return availableTanks.FirstOrDefault(availableTank => availableTank.m_Wins == m_NumRoundsToWin);
    }
    
    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";
        
        var availableTanks = m_Tanks.FindAll(t => t.m_Instance != null);

        foreach (var availableTank in availableTanks)
        {
            message += availableTank.m_ColoredPlayerText + ": " + availableTank.m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }

    private void ResetAllTanks()
    {
        var availableTanks = m_Tanks.FindAll(t => t.m_Instance != null);

        foreach (var availableTank in availableTanks)
        {
            availableTank.Reset();
        }
    }

    private void EnableTankControl()
    {
        var availableTanks = m_Tanks.FindAll(t => t.m_Instance != null);

        foreach (var availableTank in availableTanks)
        {
            availableTank.EnableControl();
        }
    }
    
    private void DisableTankControl()
    {
        var availableTanks = m_Tanks.FindAll(t => t.m_Instance != null);

        foreach (var availableTank in availableTanks)
        {
            availableTank.DisableControl();
        }
    }
}