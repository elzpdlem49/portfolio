using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using TextRPG;
using Unity.VisualScripting;

public class BossCont : MonoBehaviour
{
    public static BossCont Instance;
    [SerializeField]
    public Player m_Boss;
    public PlayerMove player;
    public float moveSpeed = 2f;
    public float serchRange = 10f;
    public float attackRange = 4f;


    Rigidbody bossRb;
    public float attackCooldown = 5;
    public int m_BossDamage1 = 1;
    public int m_BossDamage2 = 2;
    public int m_BossDamage3 = 3;
    public float disToPlayer;

    public bool isCooldown;
    public bool isTouch;

    bool isRushing; // 돌진 중인지 여부를 나타내는 플래그
    public float chargeDuration = 1.5f;
    public float pushBackForce = 10f; // 돌진 지속 시간

    public enum BossState
    {
        Idle,
        FollowPlayer,
        Attack
    }
    public BossState m_eCurrentState;

    // Start is called before the first frame update
    void Start()
    {
        bossRb = GetComponent<Rigidbody>();
        m_Boss = new Player(name, 100, 100, 10, 9);
        Instance = this;
        m_eCurrentState = BossState.Idle;
        //StartCoroutine(BossFight());
    }
    public void OnGUI()
    {
        //오브젝트의 3d좌표를 2d좌표(스크린좌표)로 변환하여 GUI를 그린다.
        Vector3 vPos = this.transform.position;
        Vector3 vPosToScreen = Camera.main.WorldToScreenPoint(vPos); //월드좌표를 스크린좌표로 변환한다.
        vPosToScreen.y = Screen.height - vPosToScreen.y; //y좌표의 축이 하단을 기준으로 정렬되므로 상단으로 변환한다.
        int h = 40;
        int w = 100;
        Rect rectGUI = new Rect(vPosToScreen.x, vPosToScreen.y, w, h);
        //GUI.Box(rectGUI, "MoveBlock:" + isMoveBlock);
        GUI.Box(rectGUI, string.Format("HP:{1}\nMP:{0}", m_Boss.m_nMp, m_Boss.m_nHp));
    }
    // Update is called once per frame
    private void Update()
    {
        UpdateBossState();
        disToPlayer = Vector3.Distance(transform.position, player.transform.position);
    }
    void FixedUpdate()
    {
        SetBossState();
    }
    void SetBossState()
    {
        switch(m_eCurrentState)
        {
            case BossState.Idle:

                break;
            case BossState.FollowPlayer:
                FollowPlayer();
                break;
            case BossState.Attack:
                StartCoroutine(AttackPattern());
                break;
        }
    }
    void UpdateBossState()
    {
        switch(m_eCurrentState)
        {
            case BossState.Idle:
                if(disToPlayer < serchRange)
                {
                    m_eCurrentState = BossState.FollowPlayer;
                }
                break;
            case BossState.FollowPlayer:
                if(disToPlayer > serchRange)
                {
                    m_eCurrentState = BossState.Idle;
                }
                else if (disToPlayer < attackRange)
                {
                    m_eCurrentState = BossState.Attack;
                }
                break;
            case BossState.Attack:
                if(disToPlayer > attackRange)
                {
                    m_eCurrentState = BossState.FollowPlayer;
                }
                break;
        }
    }
    void FollowPlayer()
    {
        if (!isTouch)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            if (disToPlayer < attackRange)
            {
                bossRb.velocity = Vector3.zero;
                // 바라보는 방향
                /* Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up); 
                 toRotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 5f);
                 enemyRigidbody.MoveRotation(toRotation);*/

                StartCoroutine(AttackPattern());
            }
            else if (disToPlayer < serchRange)
            {
                bossRb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                toRotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 5f);
            }
            else
            {
                // 돌진 시작
                StartCoroutine(RushAtPlayer(direction));
            }
        }
    }
    IEnumerator AttackPattern()
    {
        if(isCooldown)
        {
            yield break;
        }
        isCooldown = true;
        int random = Random.Range(0, 3);

        switch (2)
        {
            case 0:
                AttackPattern1();
                break;
            case 1:
                AttackPattern2();
                break;
            case 2:
                yield return StartCoroutine(ChargeAttack());
                break;
        }
        yield return new WaitForSeconds(attackCooldown);
        isCooldown=false;
    }
    void AttackPattern1()
    {
        Debug.Log("Executing Attack Pattern 1");
        PlayerMove.Instance.m_cPlayer.m_nHp -= m_Boss.m_sStatus.nStr * m_BossDamage1;
    }

    void AttackPattern2()
    {
        Debug.Log("Executing Attack Pattern 2");
        PlayerMove.Instance.m_cPlayer.m_nHp -= m_Boss.m_sStatus.nStr * m_BossDamage2;
    }
    IEnumerator ChargeAttack()
    {
        if (isRushing)
        {
            yield break; // Prevent multiple charges at the same time
        }

        Debug.Log("Charging at the player!");

        Vector3 chargeDirection = (player.transform.position - transform.position).normalized;

        // Perform the charge attack
        yield return StartCoroutine(RushAtPlayer(chargeDirection));
    }
    IEnumerator RushAtPlayer(Vector3 rushDirection) // ChargeAtPlayer를 RushAtPlayer로 변경
    {
        isRushing = true;

        // 플레이어를 향해 돌진하는 속도 설정
        bossRb.velocity = rushDirection * (moveSpeed * 2f);

        // 돌진 지속 시간 동안 대기
        yield return new WaitForSeconds(chargeDuration);

        // 돌진 플래그 초기화
        isRushing = false;

        // 속도 초기화
        bossRb.velocity = Vector3.zero;
        bossRb.angularVelocity = Vector3.zero;

        // 플레이어를 향해 회전
        Quaternion toRotation = Quaternion.LookRotation(rushDirection, Vector3.up);
        bossRb.MoveRotation(toRotation);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (isRushing && collision.gameObject.CompareTag("Player"))
        {
            // 플레이어를 밀어냄
            Vector3 pushBackDirection = (collision.transform.position - transform.position).normalized;
            collision.rigidbody.AddForce(pushBackDirection * pushBackForce, ForceMode.Impulse);

            // 플레이어에게 피해 입힘
            PlayerMove.Instance.m_cPlayer.m_nHp -= m_Boss.m_sStatus.nStr * m_BossDamage3;
        }
    }
}
