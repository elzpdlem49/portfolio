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
    //public Player m_Annie;
    public float moveSpeed = 1f;
    public float serchRange = 5f;
    public float attackRange = 1f;
    public float attackCool = 2f;
    public float searchRadius = 10f;
    Rigidbody enemyRigidbody;
    public Animator anim;
    public bool isTouch;
    public bool isCooldown = false;
    public float disToPlayer;
    public GameObject m_objTarget;
    public EnemyHPBar m_guiEnemyHPBar;
    int currentWaypointIndex = 0;

    public Transform[] patrolWaypoints;
    public enum Faction
    {
        Blue,
        Red
    }
    public Faction m_eFaction;

    public void SetFaction(Faction faction)
    {
        m_eFaction = faction;
    }
    public enum EnemyState
    {
        Idle,
        Patrol,
        FollowPlayer,
        Attack
    }
    public EnemyState m_eCurrentState;

    // Start is called before the first frame update
    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody>();
        m_Enemy = new Player(name, 100, 100, 0, 10, 0);
        //m_Annie = new Player(name, 100, 100, 0, 10, 0);
        Instance = this;
        anim = GetComponent<Animator>();
        m_eCurrentState = EnemyState.Patrol;
    }
    void Start()
    {

    }
    private void Update()
    {
        UpdateAIState();
        EnemeyUpdateStatus();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (m_objTarget != null)
        {
            disToPlayer = Vector3.Distance(transform.position, m_objTarget.transform.position);
        }
        SetAIState();
    }
    void SetAIState()
    {
        switch (m_eCurrentState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Patrol:
                Patrol();
                SetTargetBasedOnFaction();
                anim.SetBool("isWalk", true);
                break;
            case EnemyState.FollowPlayer:
                FollowTarget();
                anim.SetBool("isWalk", true);
                break;
            case EnemyState.Attack:
                if (!isCooldown)
                {
                    StartCoroutine(AttackCoolTime());
                }

                break;
        }
    }
    void UpdateAIState()
    {
        switch (m_eCurrentState)
        {
            case EnemyState.Patrol:
                if (m_objTarget != null && m_eFaction == Faction.Red && disToPlayer < serchRange)
                {
                    m_eCurrentState = EnemyState.FollowPlayer;
                }
                else if (m_objTarget != null && m_eFaction == Faction.Blue && disToPlayer < serchRange)
                {
                    m_eCurrentState = EnemyState.FollowPlayer;
                }
                break;
            case EnemyState.FollowPlayer:
                if (m_objTarget == null || !m_objTarget.activeSelf || disToPlayer > serchRange)
                {
                    // If the target is null, inactive, or out of range, find a new target
                    SetTargetBasedOnFaction();
                    m_eCurrentState = EnemyState.Patrol;
                }
                else if (disToPlayer < attackRange)
                {
                    m_eCurrentState = EnemyState.Attack;
                    anim.SetBool("isWalk", false);
                }
                break;
            case EnemyState.Attack:
                if (m_objTarget == null || !m_objTarget.activeSelf || disToPlayer > attackRange)
                {
                    // If the target is null, inactive, or out of range, find a new target
                    SetTargetBasedOnFaction();
                    m_eCurrentState = EnemyState.Patrol;
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
    void FollowTarget()
    {
        //if (!isTouch)
        {
            Vector3 direction = (m_objTarget.transform.position - transform.position).normalized;
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
        Enemycontroller targetController = m_objTarget.GetComponent<Enemycontroller>();
        // 여기서 소속에 따라 공격 로직을 구분할 수 있습니다.
        if (m_eFaction == Faction.Red)
        {
            
            // 레드(적) 미니언의 공격 로직
            if (targetController != null && targetController.m_eFaction == Faction.Blue)
            {
                // 대상이 블루(우호적) 미니언인 경우, 대상의 스태미너를 감소시킵니다.
                targetController.TakeDamage(m_Enemy.m_sStatus.nStr);
            }
            else if (m_objTarget.CompareTag("Blue"))
            {
                PlayerMove.Instance.m_cPlayer.m_nHp -= 10;
            }
        }
        else if (m_eFaction == Faction.Blue)
        {
            // 블루(우호적) 미니언의 공격 로직
            if (targetController != null && targetController.m_eFaction == Faction.Red)
            {
                // 대상이 레드(적) 미니언인 경우, 대상의 스태미너를 감소시킵니다.
                targetController.TakeDamage(m_Enemy.m_sStatus.nStr);
            }
            else if (m_objTarget.CompareTag("Red"))
            {
                Annie.Instance.m_Annie.m_nHp -= 10;
            }
        }

        Vector3 targetPosition = m_objTarget.transform.position;
        targetPosition.y = transform.position.y; // Keep the same height to avoid tilting
        transform.LookAt(targetPosition);

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
    public void TakeDamage(int damage)
    {
        m_Enemy.m_nHp -= damage;
        Death();
    }

    public bool Death()
    {
        if (m_Enemy.m_nHp <= 0)
        {
            // GameObject를 비활성화하고 적이 죽었음을 나타내기 위해 true를 반환
            gameObject.SetActive(false);
            PoolManager.instance.RemoveFromPool(gameObject);
            return true;
        }
        return false;
    }
    public void EnemeyUpdateStatus()
    {
        m_guiEnemyHPBar.SetSlider(m_Enemy.m_nHp, m_Enemy.m_sStatus.nHP);
    }
    void SetTargetBasedOnFaction()
    {
        switch (m_eFaction)
        {
            case Faction.Red:
                m_objTarget = FindClosestTargetOfTag("Blue");
                break;
            case Faction.Blue:
                m_objTarget = FindClosestTargetOfTag("Red");
                break;
            // Add more cases if you have additional factions
            default:
                break;
        }
    }
    GameObject FindClosestTargetOfTag(string targetTag)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius);

        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(targetTag) && collider.gameObject.activeSelf)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestTarget = collider.gameObject;
                    closestDistance = distance;
                }
            }
        }

        return closestTarget;
    }
}
