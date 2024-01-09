using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TextRPG;
using static PlayerMove;

public class AnPlayer : MonoBehaviour
{
    public static AnPlayer Instance;

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

    public LayerMask m_LayerMask;
    public GameObject m_objTarget = null;
    bool m_isHit;
    Animator m_anim;

    private float stunDuration = 2f; // Set the stun duration (in seconds) as needed
    private float stunEndTime;

    public enum Skill
    {
        Fireball,
        Incineration,
        LavaShield,
        Summon
    }

   
   
    public int stunStack = 0;


    private Dictionary<Skill, float> skillCooldowns = new Dictionary<Skill, float>();
    public float skillCooldownInterval = 3f;
    private float nextSkillCooldown;

    private void Awake()
    {

    }
    private void Start()
    {
        Instance = this;
        m_anim = GetComponent<Animator>();
    }
    // 매 프레임마다 호출되는 Update 메서드
    void Update()
    {
        UseSkill();
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
    void UseSkill()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Fireball();
            m_anim.SetTrigger("Fireball");
            Debug.Log("Annie P: 파이어볼 시전 중");
            stunStack++;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Incineration();
            m_anim.SetTrigger("Incineration");
            Debug.Log("Annie P: 화염 소각 시전 중");
            stunStack++;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            m_anim.SetTrigger("LavaShield");
            ActivateLavaShield();
            stunStack++;
        }
        if (stunStack == 4 != Input.GetKey(KeyCode.LeftShift))
        {
            Stun();
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
        int nLayer = 1 << LayerMask.NameToLayer("Enemy");
        Collider[] colliders =
            Physics.OverlapSphere(vPos, m_fSite, m_LayerMask);

        foreach (Collider collider in colliders)
        {
            Vector3 vTargetPos = collider.transform.position;
            Vector3 vToTarget = vTargetPos - vPos;

            float fTargetAngle = Vector3.Angle(vForward, vToTarget);
            float fRightAngle = Vector3.Angle(vForward, vRight);
            float fLeftAngle = Vector3.Angle(vForward, vLeft);

            //Debug.Log(collider.gameObject.name + " TargetAngle:" + fTargetAngle + "/" + fHalfAngle + "(" + fRightAngle + "/" + fLeftAngle + ")");
            if (fTargetAngle < fHalfAngle)
            {
                float distanceToTarget = Vector3.Distance(transform.position, vTargetPos);
                if (distanceToTarget <= attackRange)
                {
                    Debug.DrawLine(vPos, vTargetPos, Color.green);
                    m_objTarget = collider.gameObject;
                    SetTarget(collider.gameObject);

                }
                else
                {
                    Debug.DrawLine(vPos, vTargetPos, Color.red); // 타겟이 공격 범위 밖에 있는 경우
                    m_objTarget = null; // 공격 범위 밖에 있으면 타겟을 리셋합니다.
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
    public void SetTarget(GameObject target)
    {
        m_objTarget = target;
    }
    private void OnDrawGizmos()
    {
        Incineration();
        Gizmos.DrawWireSphere(transform.position, m_fRadius);
    }
    void ActivateLavaShield()
    {
        Debug.Log("Annie P: 라바 실드 활성화 중");

        StartCoroutine(LavaShieldCoroutine());
    }

    IEnumerator LavaShieldCoroutine()
    {
        PlayerMove.Instance.m_cPlayer.m_nHp += 10;

        float shieldDuration = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < shieldDuration)
        {
            yield return null;

            elapsedTime += Time.deltaTime;
        }

        Debug.Log("Annie P: 라바 실드 비활성화 중");
        PlayerMove.Instance.m_cPlayer.m_nHp -= 10;
    }

    void Summon()
    {
        Debug.Log("Annie : 소환 중");
        // 소환 로직을 여기에 구현
    }
    public bool controlEnabled = true;
    void Stun()
    {
        Debug.Log("Annie : 스턴!");
        StartCoroutine(StunCoroutine());
        stunStack = 0;
    }
    IEnumerator StunCoroutine()
    {
        controlEnabled = false;
        Enemycontroller.Instance.m_eCurrentState = Enemycontroller.EnemyState.Idle;

        Debug.Log(": 스턴 중!");
        stunEndTime = Time.time + stunDuration; // 스턴이 끝나는 시간을 계산합니다.

        while (Time.time < stunEndTime)
        {
            yield return null;
        }
        // 스턴 기간이 끝나면 플레이어 컨트롤을 다시 활성화합니다.
        controlEnabled = true;

        Debug.Log(": 스턴 종료!");
    }
}