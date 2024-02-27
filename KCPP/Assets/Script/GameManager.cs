using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextRPG;
using System;

public class GameManager : MonoBehaviour
{
    [Header("# 플레이어 정보")]

    public List<GameObject> m_listGUIScenes;
    public enum E_GUI_STATE { 메인, 승리, 패배, 플레이 }
    public E_GUI_STATE m_curGUIState;

    public StatusBar m_guiHPBar;
    public StatusBar m_guiEXPBar;
    public StatusBar m_guiAnnieHPBar;
    public ItemManager m_itemManager;
    public SkillUIManager m_skillUIManager;

    public List<Transform> playerSpawnPoints;
    public List<Transform> enemySpawnPoints;

    public Transform playerTr;
    public Transform annieTr;
    public float displayDistance = 10f;

    public List<PlayerMove> m_listPlayer;
    public List<Annie> m_listAnnie;
    public List<Enemycontroller> m_listEnemycontroller;

    static GameManager m_cInstance;
    public static GameManager GetInstance()
    {
        return m_cInstance;
    }

    private void Awake()
    {
        m_cInstance = this;
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

    public void EventStart()
    {
        SetGUIScene(E_GUI_STATE.플레이);
    }

    public void EventGameOver()
    {
        SetGUIScene(E_GUI_STATE.패배);
    }

    public void EventEnd()
    {
        SetGUIScene(E_GUI_STATE.승리);
    }

    public void EventRetry()
    {
        SetGUIScene(E_GUI_STATE.플레이);
    }

    public void EventExit()
    {
        Application.Quit();
    }

    public void PlayerUpdateStatus(int playerIdx = 0)
    {
        Player player = m_listPlayer[playerIdx].m_cPlayer;

        if (player != null)
        {
            m_guiHPBar.SetBar(player.m_nHp, player.m_sStatus.nHP);
            m_itemManager.gameObject.SetActive(true);
            m_skillUIManager.gameObject.SetActive(true);
            SkillUIManager.instance.UpdateSkillUI();
            m_guiEXPBar.SetBar(player.m_nExp, player.m_nNextExp[Math.Min(player.m_nLevel - 1, player.m_nNextExp.Length - 1)]);
        }
    }

    public void AnnieUpdateStatus()
    {
        foreach (var player in m_listAnnie)
        {
            if (player.m_Annie != null)
            {
                m_guiAnnieHPBar.SetBar(player.m_Annie.m_nHp, player.m_Annie.m_sStatus.nHP);
            }
        }

        float distance = Vector3.Distance(playerTr.position, annieTr.position);

        if (distance <= displayDistance)
        {
            m_guiAnnieHPBar.gameObject.SetActive(true);
        }
        else
        {
            m_guiAnnieHPBar.gameObject.SetActive(false);
        }
    }

    public void EventChangeScene(int stateNumber)
    {
        SetGUIScene((E_GUI_STATE)stateNumber);
    }

    public void ShowScene(E_GUI_STATE state)
    {
        for (int i = 0; i < m_listGUIScenes.Count; i++)
        {
            m_listGUIScenes[i].SetActive((E_GUI_STATE)i == state);
        }
    }

    public void SetGUIScene(E_GUI_STATE state)
    {
        switch (state)
        {
            case E_GUI_STATE.메인:
            case E_GUI_STATE.승리:
            case E_GUI_STATE.패배:
                Time.timeScale = 0;
                break;
            case E_GUI_STATE.플레이:
                Time.timeScale = 1;
                break;
        }
        ShowScene(state);
        m_curGUIState = state;
    }

    public void UpdateState()
    {
        switch (m_curGUIState)
        {
            case E_GUI_STATE.메인:
            case E_GUI_STATE.승리:
            case E_GUI_STATE.패배:
                break;
            case E_GUI_STATE.플레이:
                PlayerUpdateStatus();
                AnnieUpdateStatus();
                break;
        }
    }
}
