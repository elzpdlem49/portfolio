using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TextRPG;
using static PlayerMove;
using static Enemycontroller;
using Unity.VisualScripting;

public class Annie : MonoBehaviour
{
    public static Annie Instance;
    [SerializeField]
    public Player m_Annie;

    public float moveSpeed = 5f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public int damage = 10;
    public GameObject fireballPrefab;
    public float fireballSpeed = 10f;
    public float fireballDuration = 3f;
    public float m_fAngle = 90;
    public float m_fRadius = 3;
    public float m_fSite = 3;
    public float m_Range = 1;
    public float fireballRange = 5f;
    public float incinerationRange = 3f;
    public ParticleSystem ashieldParticleSystem;
    public ParticleSystem aflameIncinerationParticleSystem;
    public LayerMask m_LayerMask;
    public GameObject m_objTarget = null;
    bool m_isHit;
    Rigidbody m_rigidbody;
    Animator m_anim;
    public float disToPlayer;
    private float stunDuration = 2f; // Set the stun duration (in seconds) as needed
    private float stunEndTime;

    public enum AnnieState
    {
        Idle,
        FollowPlayer,
        Attack
    }
    public AnnieState m_eCurrentState;
    public enum Skill
    {
        Fireball,
        Incineration,
        LavaShield,
        Summon
    }

    private Skill[] skills = { Skill.Fireball, Skill.Incineration, Skill.LavaShield, Skill.Summon };

    public int stunStack = 0;

    
    private Dictionary<Skill, float> skillCooldowns = new Dictionary<Skill, float>();
    public float skillCooldownInterval = 3f; 
    private float nextSkillCooldown;

    private void Awake()
    {
        
    }
    private void Start()
    {
        m_Annie = new Player(name, 2000, 100, 10, 0, 0);
        m_rigidbody = GetComponent<Rigidbody>();
        Instance = this;
        m_anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (AnPlayer.Instance.AIControl)
        {
            UpdateAIState();
        }
    }
    // 매 프레임마다 호출되는 Update 메서드
    void FixedUpdate()
    {
        if (m_objTarget != null)
            disToPlayer = Vector3.Distance(transform.position, m_objTarget.transform.position);
        if (AnPlayer.Instance.AIControl)
        {
            SetAIState();
        }
    }

    void MoveTowardsPlayer()
    {
        /*Vector3 direction = (m_objTarget.transform.position - transform.position).normalized;

        transform.Translate(direction * moveSpeed * Time.deltaTime);
        transform.LookAt(m_objTarget.transform.position);*/
        Vector3 direction = (m_objTarget.transform.position - transform.position).normalized;
        m_rigidbody.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        toRotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 3f);
        m_rigidbody.MoveRotation(toRotation);
    }

    void SetAIState()
    {
        switch (m_eCurrentState)
        {
            case AnnieState.Idle:
                break;
            case AnnieState.FollowPlayer:
                SetClosestTarget();
                MoveTowardsPlayer();
                m_anim.SetBool("isWalk", true);
                break;
            case AnnieState.Attack:
                UseRandomSkillWithCooldown();
                UpdateCooldowns();
                break;
        }
    }
    void UpdateAIState()
    {
        switch (m_eCurrentState)
        {
            case AnnieState.Idle:
                if (disToPlayer < detectionRange)
                {
                    m_eCurrentState = AnnieState.FollowPlayer;
                }
                break;
            case AnnieState.FollowPlayer:
                if (m_objTarget == null || !m_objTarget.activeSelf || disToPlayer < attackRange)
                {
                    SetClosestTarget();
                    m_eCurrentState = AnnieState.Attack;
                    m_anim.SetBool("isWalk", false);
                }
                break;
            case AnnieState.Attack:
                if (m_objTarget == null || !m_objTarget.activeSelf || disToPlayer > attackRange)
                {
                    SetClosestTarget();
                    m_eCurrentState = AnnieState.FollowPlayer;
                    //m_anim.SetBool("isAttack", false);
                }
                break;
            default:
                break;
        }
    }

    void UseRandomSkillWithCooldown()
    {
        if (Time.time >= nextSkillCooldown)
        {
            Skill randomSkill = skills[Random.Range(0, skills.Length)];

            float skillRange = GetSkillRange(randomSkill);

            if (Vector3.Distance(transform.position, m_objTarget.transform.position) <= skillRange)
            {
                LookAtPlayer();
                UseSkill(randomSkill);
                nextSkillCooldown = Time.time + skillCooldownInterval;
            }
        }
    }
    void LookAtPlayer()
    {
        Vector3 direction = (m_objTarget.transform.position - transform.position).normalized;
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        m_rigidbody.MoveRotation(toRotation);
    }
    float GetSkillRange(Skill skill)
    {
        switch (skill)
        {
            case Skill.Fireball:
                return fireballRange;
            case Skill.Incineration:
                return incinerationRange;
            case Skill.LavaShield:
                return fireballRange;
            case Skill.Summon:
                return fireballRange;
            default:
                return 0f;
        }
    }
    void UseSkill(Skill skill)
    {
        switch (skill)
        {
            case Skill.Fireball:
                Fireball();
                m_anim.SetTrigger("Fireball");
                Debug.Log("Annie AI: 파이어볼 시전 중");
                break;
            case Skill.Incineration:
                Incineration();
                m_anim.SetTrigger("Incineration");
                aflameIncinerationParticleSystem.Play();
                StartCoroutine(StopFlameIncinerationEffect());
                Debug.Log("Annie AI: 화염 소각 시전 중");
                break;
            case Skill.LavaShield:
                m_anim.SetTrigger("LavaShield");
                ActivateLavaShield();
                break;
            /*case Skill.Summon:
                Summon();
                break;*/
        }

        if (stunStack == 4 && skill != Skill.LavaShield)
        {
            Stun();
        }
        else if (stunStack == 4)
        {
            stunStack = 4;
        }
        else
        {
            stunStack++;
        }
    }
   
    void Fireball()
    {
        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        
        Fireball fireBall = fireball.GetComponent<Fireball>();

        fireBall.SetTarget(m_objTarget);
    }
    void Incineration()
    {
        
        float fHalfAngle = m_fAngle / 2;
        Vector3 vPos = transform.position;
        Vector3 vForward = transform.forward;
        Quaternion quaternionRight = Quaternion.AngleAxis(fHalfAngle, transform.up);
        Quaternion quaternionLeft = Quaternion.AngleAxis(fHalfAngle, transform.up * -1);
        Vector3 vRight = quaternionRight * vForward;
        Vector3 vLeft = quaternionLeft * vForward;
        Vector3 vRightPos = vPos + vRight * m_fSite;
        Vector3 vLeftPos = vPos + vLeft * m_fSite;


        Debug.DrawLine(vPos, vLeftPos, Color.red);
        Debug.DrawLine(vPos, vRightPos, Color.red);
        Debug.DrawRay(vPos, vForward * m_fSite, Color.yellow);
        Collider[] colliders = Physics.OverlapSphere(vPos, m_fSite, m_LayerMask);

        foreach (Collider collider in colliders)
        {
            Vector3 vTargetPos = collider.transform.position;
            Vector3 vToTarget = vTargetPos - vPos;

            float fTargetAngle = Vector3.Angle(vForward, vToTarget);
            float fRightAngle = Vector3.Angle(vForward, vRight);
            float fLeftAngle = Vector3.Angle(vForward, vLeft);

            if (fTargetAngle < fHalfAngle)
            {
                float distanceToTarget = Vector3.Distance(transform.position, vTargetPos);
                if (distanceToTarget <= attackRange)
                {
                    Debug.DrawLine(vPos, vTargetPos, Color.green);
                    //m_objTarget = collider.gameObject;
                    //SetTarget(collider.gameObject);
                    Enemycontroller enemyController = collider.GetComponent<Enemycontroller>();
                    PlayerMove player = collider.GetComponent<PlayerMove>();
                    if (m_objTarget.CompareTag("Blue"))
                    {
                        if (enemyController != null)
                        {
                            enemyController.TakeDamage(m_Annie.m_sStatus.nStr);

                            if (enemyController.Death())
                            {
                                collider.gameObject.SetActive(false);

                                PoolManager.instance.RemoveFromPool(collider.gameObject);
                                //Player.GetExp(3);
                            }
                        }
                        else if (player != null)
                        {
                            PlayerMove.Instance.m_cPlayer.m_nHp -= m_Annie.m_sStatus.nStr;
                            if (PlayerMove.Instance.m_cPlayer.Death())
                            {
                                collider.gameObject.SetActive(false);
                                //Player.GetExp(5);
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.DrawLine(vPos, vTargetPos, Color.blue);
                m_isHit = false;
            }

            Debug.DrawRay(vPos, vToTarget, Color.green);//방향이 반대로 나옴. 원인 확인 필요
        }
    }
    IEnumerator StopFlameIncinerationEffect()
    {
        float effectDuration = 0.5f; // 필요에 따라 지속 시간을 조정하세요

        yield return new WaitForSeconds(effectDuration);

        // 화염 소각 효과 중지
        aflameIncinerationParticleSystem.Stop();
    }
    public void SetTarget(GameObject target)
    {
        m_objTarget = target;
    }
    private void OnDrawGizmos()
    {
        //Incineration();
        Gizmos.DrawWireSphere(transform.position, m_fRadius);
    }
    void ActivateLavaShield()
    {
        Debug.Log("Annie AI: 라바 실드 활성화 중");

        StartCoroutine(LavaShieldCoroutine());
    }

    IEnumerator LavaShieldCoroutine()
    {
        m_Annie.m_nHp += 10;
        ashieldParticleSystem.Play();
        float shieldDuration = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < shieldDuration)
        {
            yield return null;

            if (PlayerMove.Instance.IsHit)
            {
                PlayerMove.Instance.m_cPlayer.m_nHp -= 1;
                PlayerMove.Instance.OnHit();
            }

            elapsedTime += Time.deltaTime;
        }
        ashieldParticleSystem.Stop();
        Debug.Log("Annie AI: 라바 실드 비활성화 중");
        m_Annie.m_nHp -= 10;
    }

    void Summon()
    {
        Debug.Log("Annie AI: 소환 중");
        // 소환 로직을 여기에 구현
    }
    public bool controlEnabled = true;
    void Stun()
    {
        Debug.Log("Annie AI: 스턴!");
        StartCoroutine(StunCoroutine());
        stunStack = 0;
    }
    IEnumerator StunCoroutine()
    {
        controlEnabled = false;
        PlayerMove.Instance.m_eCurrentState = PlayerState.Idle; // 스턴 중에 플레이어 상태를 아이들로 설정합니다.

        Debug.Log("플레이어: 스턴 중!");
        stunEndTime = Time.time + stunDuration; // 스턴이 끝나는 시간을 계산합니다.

        while (Time.time < stunEndTime)
        {
            yield return null;
        }

        // 스턴 기간이 끝나면 플레이어 컨트롤을 다시 활성화합니다.
        controlEnabled = true;

        Debug.Log("플레이어: 스턴 종료!");
    }

    void UpdateCooldowns()
    {
        foreach (var skill in skillCooldowns.Keys.ToList())
        {
            if (Time.time >= skillCooldowns[skill])
            {
                skillCooldowns.Remove(skill);
            }
        }
    }
    void SetClosestTarget()
    {
        GameObject[] blueMinions = GameObject.FindGameObjectsWithTag("Blue");
        GameObject player = GameObject.FindGameObjectWithTag("Blue");

        if (blueMinions.Length > 0)
        {
            GameObject closestMinion = FindClosestTarget(blueMinions);

            if (closestMinion != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                float distanceToMinion = Vector3.Distance(transform.position, closestMinion.transform.position);

                if (distanceToPlayer < distanceToMinion)
                {
                    // Player is closer than the closest blue minion
                    m_objTarget = player;
                }
                else
                {
                    // Set the closest blue minion as the target
                    m_objTarget = closestMinion;
                }
            }
            else
            {
                Debug.LogError("No closest minion found.");
            }
        }
        else
        {
            Debug.LogError("No blue minions found.");
        }
    }

    GameObject FindClosestTarget(GameObject[] targets)
    {
        if (targets.Length > 0)
        {
            GameObject closestTarget = null;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject target in targets)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < closestDistance)
                {
                    closestTarget = target;
                    closestDistance = distance;
                }
            }

            return closestTarget;
        }
        else
        {
            return null;
        }
    }

}