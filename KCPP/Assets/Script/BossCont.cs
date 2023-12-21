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

    bool isRushing; // ���� ������ ���θ� ��Ÿ���� �÷���
    public float chargeDuration = 1.5f;
    public float pushBackForce = 10f; // ���� ���� �ð�

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
        //������Ʈ�� 3d��ǥ�� 2d��ǥ(��ũ����ǥ)�� ��ȯ�Ͽ� GUI�� �׸���.
        Vector3 vPos = this.transform.position;
        Vector3 vPosToScreen = Camera.main.WorldToScreenPoint(vPos); //������ǥ�� ��ũ����ǥ�� ��ȯ�Ѵ�.
        vPosToScreen.y = Screen.height - vPosToScreen.y; //y��ǥ�� ���� �ϴ��� �������� ���ĵǹǷ� ������� ��ȯ�Ѵ�.
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
                // �ٶ󺸴� ����
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
                // ���� ����
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
    IEnumerator RushAtPlayer(Vector3 rushDirection) // ChargeAtPlayer�� RushAtPlayer�� ����
    {
        isRushing = true;

        // �÷��̾ ���� �����ϴ� �ӵ� ����
        bossRb.velocity = rushDirection * (moveSpeed * 2f);

        // ���� ���� �ð� ���� ���
        yield return new WaitForSeconds(chargeDuration);

        // ���� �÷��� �ʱ�ȭ
        isRushing = false;

        // �ӵ� �ʱ�ȭ
        bossRb.velocity = Vector3.zero;
        bossRb.angularVelocity = Vector3.zero;

        // �÷��̾ ���� ȸ��
        Quaternion toRotation = Quaternion.LookRotation(rushDirection, Vector3.up);
        bossRb.MoveRotation(toRotation);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (isRushing && collision.gameObject.CompareTag("Player"))
        {
            // �÷��̾ �о
            Vector3 pushBackDirection = (collision.transform.position - transform.position).normalized;
            collision.rigidbody.AddForce(pushBackDirection * pushBackForce, ForceMode.Impulse);

            // �÷��̾�� ���� ����
            PlayerMove.Instance.m_cPlayer.m_nHp -= m_Boss.m_sStatus.nStr * m_BossDamage3;
        }
    }
}
