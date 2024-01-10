using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextRPG;
//using static TextRPG.PlayerManager;
using Unity.VisualScripting;
using UnityEngine.AI;
using UnityEditor;

public class Enemycontroller : MonoBehaviour
{
    public static Enemycontroller Instance;
    [SerializeField] 
    public Player m_Enemy;
    public float moveSpeed = 1f;
    public float serchRange = 5f;
    public float attackRange = 1f;
    public float attackCool = 2f;
    Rigidbody enemyRigidbody;
    public Animator anim;
    public bool isTouch;
    public bool isCooldown = false;
    public float disToPlayer;
    public GameObject m_objTarget = null;
    public enum EnemyState
    {
        Idle,
        Patrol,
        FollowPlayer,
        Attack
    }
    public EnemyState m_eCurrentState;

    public Transform[] patrolWaypoints;
    int currentWaypointIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody>();
        m_Enemy = new Player(name, 100, 100, 0, 10, 0);
        Instance = this;
        anim = GetComponent<Animator>();
        m_eCurrentState = EnemyState.Patrol;
    }
    private void Update()
    {

        UpdateAIState();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        disToPlayer = Vector3.Distance(transform.position, m_objTarget.transform.position);
        SetAIState();
    }
    void SetAIState()
    {
        switch(m_eCurrentState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Patrol:
                Patrol();
                anim.SetBool("isWalk", true);
                break;
            case EnemyState.FollowPlayer:
                FollowPlayer();
                anim.SetBool("isWalk", true);
                break;
            case EnemyState.Attack:
                if(!isCooldown)
                {
                    StartCoroutine(AttackCoolTime());
                }
                
                break;
        }
    }
    void UpdateAIState()
    {
        switch(m_eCurrentState)
        {
            case EnemyState.Patrol:
                if (disToPlayer < serchRange)
                {
                    m_eCurrentState = EnemyState.FollowPlayer;
                }
                break;
            case EnemyState.FollowPlayer:
                if (disToPlayer > serchRange)
                {
                    m_eCurrentState = EnemyState.Patrol;
                }
                else if (disToPlayer < attackRange)
                {
                    m_eCurrentState = EnemyState.Attack;
                    anim.SetBool("isWalk", false);
                }
                break;
            case EnemyState.Attack:
                if(disToPlayer > attackRange)
                {
                    m_eCurrentState = EnemyState.FollowPlayer;
                    anim.SetBool("isAttack", false);
                }
                break;
            default:
                break;
        }
    }
    void Patrol()
    {
        Vector3 direction = (patrolWaypoints[currentWaypointIndex].position - transform.position).normalized;
        Vector3 enemyPosition = transform.position + direction * moveSpeed * Time.deltaTime;
        enemyPosition.y = 0;
        enemyRigidbody.MovePosition(enemyPosition);

        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        toRotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 3f);
        enemyRigidbody.MoveRotation(toRotation);

        float distanceToWaypoint = Vector3.Distance(transform.position, patrolWaypoints[currentWaypointIndex].position);
        if (distanceToWaypoint < 0.5f)
        {
            enemyRigidbody.velocity = Vector3.zero;
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
        }
    }
    private void OnDrawGizmos()
    {
        //remainingDistance를 시각적으로 표현되도록 만들어보기
        Vector3 vPos = this.transform.position;
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(vPos, 3f);

        Vector3 vWayPointPos = patrolWaypoints[currentWaypointIndex].position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(vWayPointPos, 2f);
    }
    void FollowPlayer()
    {
        //if (!isTouch)
        {
            Vector3 direction = (m_objTarget.transform.position -transform.position).normalized;
            enemyRigidbody.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            toRotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 3f);
            enemyRigidbody.MoveRotation(toRotation);
        }
    }
    IEnumerator AttackCoolTime()
    {
        isCooldown = true; // 쿨다운 시작

        anim.SetBool("isWalk", false); // 공격 중에는 걷지 않음
        anim.SetBool("isAttack", true); // 공격 애니메이션 시작

        // 여기에 필요한 공격 로직 추가
        PlayerMove.Instance.m_cPlayer.m_nHp -= m_Enemy.m_sStatus.nStr;

        yield return new WaitForSeconds(attackCool);

        anim.SetBool("isAttack", false); // 공격 애니메이션 종료
        isCooldown = false; // 쿨다운 종료

        m_eCurrentState = EnemyState.FollowPlayer; // 공격 후 다시 플레이어를 따라가도록 설정
    }
   
   /* private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == ("Player"))
        {
            PlayerMove.Instance.m_cPlayer.m_nHp -= m_Enemy.m_sStatus.nStr; //애니메이션
            isTouch = true;
        }
        else if (collision.gameObject.CompareTag("Sword"))
        {
            Debug.Log("Sword");
        }
        else if (collision.gameObject.tag == "Bullet")
        {

        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == ("Player"))
        {
            isTouch = false;
        }
    }*/
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
        GUI.Box(rectGUI, string.Format("HP:{0}\n", m_Enemy.m_nHp));
    }
    public void TakeDamage(int damage)
    {
        m_Enemy.m_nHp -= damage;
    }
    
    public bool Death()
    {
        return m_Enemy.m_nHp <= 0;
    }
}
