using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject m_menuActive = null;
    [SerializeField] private GameObject m_menuPause = null;
    [SerializeField] private GameObject m_menuWin, m_menuLoss = null;
    [SerializeField] private TMP_Text m_waveNumText = null;
    [SerializeField] private TMP_Text m_goalCountText = null;
    [SerializeField] private int m_wavesRequired = 2;

    [SerializeField] private TMP_Text m_currentAmmo;
    [SerializeField] private TMP_Text m_MaxAmmo;

    public GameObject m_damageFlash = null;
    public Image m_playerHealthBar = null;
    public Image m_playerOverShield = null;
    public bool m_isPaused = false;
    public GameObject m_player = null;
    public PlayerController m_playerController = null;
    public bool m_isBuffActive = false;

    private WaveManager waveManager;
    private float m_timeScaleOriginal = 1.0f;
    private int m_goalCount = 0;
    private gunStats m_gun;

  

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        m_timeScaleOriginal = Time.timeScale;
        m_player = GameObject.FindWithTag("Player");
        m_playerController = m_player.GetComponent<PlayerController>();
        waveManager = FindObjectOfType<WaveManager>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (m_menuActive == null)
            {
                StatePaused();
                m_menuActive = m_menuPause;
                m_menuActive.SetActive(true);
            }
            else if (m_menuActive == m_menuPause)
            {
                StateUnpaused();
            }
        }
    }

    public void StatePaused()
    {
        m_isPaused = !m_isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void StateUnpaused()
    {
        m_isPaused = !m_isPaused;
        Time.timeScale = m_timeScaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        m_menuActive.SetActive(false);
        m_menuActive = null;
    }

    public void UpdateGameGoal(int amount)
    {
        m_goalCount += amount;

        if (m_goalCountText != null)
        {
            m_goalCountText.text = m_goalCount.ToString("F0");
        }

        if (m_goalCount <= 0)
        {

            if (waveManager != null)
            {
                int currentWaveNumber = waveManager.GetCurrentWave();
                if (currentWaveNumber >= m_wavesRequired)
                {
                    StatePaused();
                    m_menuActive = m_menuWin;
                    m_menuActive.SetActive(true);
                    return;
                }

                Debug.Log("No enemies left. Starting next wave...");
                waveManager.StartNextWave();
                m_waveNumText.text = waveManager.GetCurrentWave().ToString();
            }
            else
            {
                Debug.LogError("WaveManager not found! Ending the game.");
                StatePaused();
                m_menuActive = m_menuWin;
                m_menuActive.SetActive(true);
            }
        }
    }

    public void Lose()
    {
        StatePaused();
        m_menuActive = m_menuLoss;
        m_menuActive.SetActive(true);
    }

    public void AmmoCount(string ammoMax, string ammoCurr)
    {
        m_currentAmmo.text = ammoCurr;
        m_MaxAmmo.text = ammoMax;
    }

   

}


