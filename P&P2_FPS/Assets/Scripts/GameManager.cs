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
    [SerializeField] private GameObject m_ammoMenu = null;
    [SerializeField] private TMP_Text m_goalCountText = null;

    [SerializeField] private TMP_Text m_currentAmmo;
    [SerializeField] private TMP_Text m_MaxAmmo;

    public GameObject m_damageFlash = null;
    public Image m_playerHealthBar = null;
    public Image m_playerOverShield = null;
    public bool m_isPaused = false;
    public GameObject m_player = null;
    public PlayerController m_playerController = null;
    public bool m_isBuffActive = false;
    public int currentLevel = 0;

    private float m_timeScaleOriginal = 1.0f;
    private int m_goalCount = 0;
    private gunStats m_gun;

    // boss/sub-boss management
    public Wraith Wraith; // reference to the Wraith script
    
    private bool bossFightActive = false; // tracks if the boss fight is active

    private AudioClip m_currentLevelMusic = null;

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
        m_playerController = m_player?.GetComponent<PlayerController>();
        

    }

    private void Start()
    {
        if (AudioManger.instance != null)
        {
            m_currentLevelMusic = AudioManger.instance.backgroundMusic.clip;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (m_menuActive == null)
            {
                if (m_ammoMenu.activeSelf)
                {
                    m_ammoMenu.SetActive(false);
                }

                StatePaused();
                m_menuActive = m_menuPause;
                m_menuActive.SetActive(true);

                if(m_currentLevelMusic != null)
                {
                    AudioManger.instance.PlayBackgroundMusic(AudioManger.instance.mainMenuMusic);
                }
            }
            else if (m_menuActive == m_menuPause)
            {
                StateUnpaused();
                m_menuActive.SetActive(false);
                m_menuActive = null;
                if(m_currentLevelMusic != null)
                {
                    AudioManger.instance.PlayBackgroundMusic(m_currentLevelMusic);
                }
            }
        }

        if (Input.GetButtonDown("OpenInventory") && m_menuActive == null)
        {
            if (!m_ammoMenu.activeSelf)
            {
                StatePaused();
                m_ammoMenu.SetActive(true);
            }
            else
            {
                StateUnpaused();
                m_ammoMenu.SetActive(false);
            }
        }
    }

    public void StatePaused()
    {
        m_isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void StateUnpaused()
    {
        m_isPaused = false;
        Time.timeScale = m_timeScaleOriginal;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateGameGoal(int amount)
    {
        m_goalCount += amount;

        if (m_goalCountText != null)
        {
            m_goalCountText.text = m_goalCount.ToString("F0");
        }

        //if (m_goalCount <= 0)
        //{
        //    Debug.Log("Game goal reached!");
        //    StatePaused();
        //    m_menuActive = m_menuWin;
        //    m_menuActive.SetActive(true);
        //}
    }

    public void WinGame()
    {
        StatePaused();
        m_menuActive = m_menuWin;
        m_menuActive.SetActive(true);
    }

    public void StartBossFight()
    {
        if (!bossFightActive)
        {
            bossFightActive = true;
            Wraith.gameObject.SetActive(true);
            Wraith.ChangePhase(1);
            Debug.Log("Boss fight started.");
        }
    }

    public void EndBossFight()
    {
        if (bossFightActive)
        {
            bossFightActive = false;
            Wraith.gameObject.SetActive(false);
            Debug.Log("Boss fight ended.");
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






