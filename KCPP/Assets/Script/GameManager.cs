using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextRPG;
using static TextRPG.PlayerManager;
using System;

public class GameManager : MonoBehaviour
{
    [Header("# Player Info")]
    public int playerId;
    public float health;
    public int maxhealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };

    public List<GameObject> m_listGUIScenes;
    public enum E_GUI_STATE { TITLE, THEEND, GAMEOVER, PLAY }
    public E_GUI_STATE m_curGUIState;

    public StatusBar m_guiHPBar;
    public StatusBar m_guiEXPBar;
    public void EventStart()
    {
        SetGUIScene(E_GUI_STATE.PLAY);
    }

    public void EventGameOver()
    {
        SetGUIScene(E_GUI_STATE.GAMEOVER);
    }

    public void EventEnd()
    {
        SetGUIScene(E_GUI_STATE.THEEND);
    }

    public void EventRetry()
    {
        SetGUIScene(E_GUI_STATE.PLAY);
    }
    public void EventExit()
    {
        Application.Quit();
    }
    public void EventUpdateStatus(int playerIdx = 0)
    {
        Player player = m_listPlayer[playerIdx].m_cPlayer;
     
        if(player != null )
        {
            m_guiHPBar.SetBar(player.m_nHp, player.m_sStatus.nHP);

            m_guiEXPBar.SetBar(player.m_nExp, player.m_sStatus.nEXP);
        }
    }

   
    
    public void EventChageScene(int stateNumber)
    {
        SetGUIScene((E_GUI_STATE)stateNumber);
    }
    public void ShowScenec(E_GUI_STATE state)
    {
        for (int i = 0; i < m_listGUIScenes.Count; i++)
        {
            if ((E_GUI_STATE)i == state)
                m_listGUIScenes[i].SetActive(true);
            else
                m_listGUIScenes[i].SetActive(false);
        }
    }
    public void SetGUIScene(E_GUI_STATE state)
    {
        switch (state)
        {
            case E_GUI_STATE.TITLE:
                Time.timeScale = 0;
                break;
            case E_GUI_STATE.THEEND:
                Time.timeScale = 0;
                break;
            case E_GUI_STATE.GAMEOVER:
                Time.timeScale = 0;
                break;
            case E_GUI_STATE.PLAY:
                Time.timeScale = 1;
                break;
        }
        ShowScenec(state);
        m_curGUIState = state;
    }
    public void UpdateState()
    {
        switch (m_curGUIState)
        {
            case E_GUI_STATE.TITLE:
                break;
            case E_GUI_STATE.THEEND:
                break;
            case E_GUI_STATE.GAMEOVER:
                break;
            case E_GUI_STATE.PLAY:
                EventUpdateStatus();
               /* if (Input.GetKeyDown(KeyCode.I))
                {
                    PopupIventroy();
                }*/

                break;
        }
    }
    public List<PlayerMove> m_listPlayer;

    static GameManager m_cInstance;
    public static GameManager GetInstance()
    {
        return m_cInstance;
    }
    private void Awake()
    {
        m_cInstance = this;
        /*m_cItemManager.Init();
        m_cPlayerManager.Init();

        PlayerMovement playerMovement = m_listPlayer[0].GetComponent<PlayerMovement>();
        playerMovement.m_cPlayer = m_cPlayerManager.GetPlayer(PlayerManager.E_PLAYER.JHON_LEAMON);
        m_cItemManager.SetPlayerAllData(playerMovement.m_cPlayer);*/
    }
    private void Start()
    {
        SetGUIScene(m_curGUIState);
    }
    void Update()
    {
        UpdateState();
        PlayerUpdate();
    }
    void PlayerUpdate()
    {
        foreach (var player in m_listPlayer)
        {
            if (player.m_cPlayer.Death())
            {
                GameManager.GetInstance().EventGameOver();
            }
        }
    }
}
