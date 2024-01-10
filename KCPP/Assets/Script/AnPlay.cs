using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TextRPG;
using static PlayerMove;
using static Fireball;
using static UnityEngine.UI.GridLayoutGroup;

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
    void Update()
    {
        if(Annie.Instance.controlEnabled)
        {
            UseSkill();
        }
        SetNearestTarget();
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
            Fireball();
            m_anim.SetTrigger("Fireball");
            Debug.Log("Annie P: 파이어볼");
            IncrementSkillCounter();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Incineration();
            m_anim.SetTrigger("Incineration");
            Debug.Log("Annie P: 화염소각");
            IncrementSkillCounter();

        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            m_anim.SetTrigger("LavaShield");
            IncrementSkillCounter();
            ActivateLavaShield();
        }
        if (stunStack == 4)
        {
            Stun();
        }
    }
    void IncrementSkillCounter()
    {
        stunStack++;
    }
    void Fireball()
    {
        float offset = 1f;

        Vector3 spawnPosition = transform.position + transform.forward * offset;

        GameObject fireball = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);

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
            if (fTargetAngle < fHalfAngle)
            {
                float distanceToTarget = Vector3.Distance(transform.position, vTargetPos);
                if (distanceToTarget <= attackRange)
                {
                    Debug.DrawLine(vPos, vTargetPos, Color.green);
                    m_objTarget = collider.gameObject;
                    SetTarget(collider.gameObject);
                    Enemycontroller enemyController = collider.GetComponent<Enemycontroller>();
                    Annie annie = collider.GetComponent<Annie>();
                    if (enemyController != null)
                    {
                        enemyController.TakeDamage(PlayerMove.Instance.m_cPlayer.m_sStatus.nStr);

                        if (enemyController.Death())
                        {
                            Destroy(collider.gameObject);
                            Player.GetExp(3);
                        }
                    }
                    if (annie != null)
                    {
                        Annie.Instance.m_Annie.m_nHp -= PlayerMove.Instance.m_cPlayer.m_sStatus.nStr;
                        if (Annie.Instance.m_Annie.Death())
                        {
                            Destroy(collider.gameObject);
                            Player.GetExp(5);
                        }
                    }
                }
                else
                {
                    Debug.DrawLine(vPos, vTargetPos, Color.red);
                    m_objTarget = null;
                }

            }
            else
            {
                Debug.DrawLine(vPos, vTargetPos, Color.blue);
                m_isHit = false;
            }

            Debug.DrawRay(vPos, vToTarget, Color.green);
        }
    }
    public void SetTarget(GameObject newTarget)
    {
        m_objTarget = newTarget;
    }
    void SetNearestTarget()
    {
        GameObject nearestTarget = FindNearestEnemy();

        if (nearestTarget != null)
        {
            SetTarget(nearestTarget);
        }
    }
    GameObject FindNearestEnemy()
    {
        // 참조 대상(owner) 주변에서 가장 가까운 Enemy를 찾아서 반환
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, LayerMask.GetMask("Enemy", "Annie"));

        GameObject nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = collider.gameObject;
            }
        }

        return nearestEnemy;
    }
    private void OnDrawGizmos()
    {
        //Incineration();
        Gizmos.DrawWireSphere(transform.position, m_fRadius);
    }
    void ActivateLavaShield()
    {
        Debug.Log("Annie P: 라바 쉴드");

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

        Debug.Log("Annie P: 라바 쉴드 종료");
        PlayerMove.Instance.m_cPlayer.m_nHp -= 10;
    }

    void Summon()
    {
        Debug.Log("Annie : ��ȯ ��");
        // ��ȯ ������ ���⿡ ����
    }
    public bool AIControl = true;
    void Stun()
    {
        Debug.Log("Annie : 기절");
        StartCoroutine(StunCoroutine());
        stunStack = 0;
    }
    IEnumerator StunCoroutine()
    {
        AIControl = false;
        Enemycontroller.Instance.m_eCurrentState = Enemycontroller.EnemyState.Idle;

        Debug.Log(": 기절!");
        stunEndTime = Time.time + stunDuration; 

        while (Time.time < stunEndTime)
        {
            yield return null;
        }

        AIControl = true;

        Debug.Log(": 기절 종료!");
    }
    
    public void OnGUI()
    {
        Vector3 vPos = this.transform.position;
        Vector3 vPosToScreen = Camera.main.WorldToScreenPoint(vPos);
        vPosToScreen.y = Screen.height - vPosToScreen.y; 
        int h = 50;
        int w = 100;

        Rect rectGUI = new Rect(vPosToScreen.x, vPosToScreen.y, w, h);
        //GUI.Box(rectGUI, "MoveBlock:" + isMoveBlock);
        GUI.Box(rectGUI, string.Format(":{0}", stunStack));
    }
}