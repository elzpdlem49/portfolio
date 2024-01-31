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
    public GameObject meteorPrefab;
    public float fireballSpeed = 10f;
    public float fireballDuration = 3f;
    public float m_fAngle = 90;
    public float m_fRadius = 3;
    public float m_fSite = 3;
    public float m_Range = 1;
    public float fireballRange = 5f;
    public float incinerationRange = 3f;
    public ParticleSystem shieldParticleSystem;
    public ParticleSystem flameIncinerationParticleSystem;
    public LayerMask m_LayerMask;
    public GameObject m_objTarget = null;
    bool m_isHit;
    Animator m_anim;

    private float stunDuration = 2f; // Set the stun duration (in seconds) as needed
    private float stunEndTime;

    public enum Skill
    {
        Move,
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
        shieldParticleSystem.Stop();
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
        //SetNearestTarget();
        SetMouseTarget();
        
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
    void RotatePlayerTowardsMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(targetPosition);
        }
    }
    bool isQActive = false;
    bool isWActive = false;
    private void OnDrawGizmos()
    {
        // Q 키를 눌렀을 때 공격 범위 표시
        if (isQActive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AnPlayer.Instance.attackRange);
        }
        
    }
    float detectionRadius = 10f;

    bool IsEnemyAtMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Enemy", "Annie");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) ;
        {
            return hit.collider != null && (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Annie"));
        }
    }
    public bool isUsingSkill = false;
    void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // 공격 범위를 표시하도록 플래그 설정
            isQActive = true;
        }
        if (isQActive && IsEnemyAtMousePosition()) 
        {
            if (Input.GetMouseButtonDown(0))
            {
                PlayerMove.Instance.isMove = false;
                isUsingSkill = true;
                Fireball();
                m_anim.SetTrigger("Fireball");
                //Debug.Log("Annie P: 파이어볼");
                IncrementSkillCounter();
                RotatePlayerTowardsMouse();
                isQActive = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            PlayerMove.Instance.isMove = false;
            isUsingSkill = true;
            Incineration();
            m_anim.SetTrigger("Incineration");
            //Debug.Log("Annie P: 화염소각");
            IncrementSkillCounter();
            RotatePlayerTowardsMouse();
            isWActive = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isUsingSkill = true;
            m_anim.SetTrigger("LavaShield");
            IncrementSkillCounter();
            ActivateLavaShield();

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerMove.Instance.isMove = false;
            isUsingSkill = true;
            MeteorS();
            
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

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int layerMask = LayerMask.GetMask("Enemy", "Annie");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // Check if the collided object is an enemy
            if (hit.collider != null && (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Annie")))
            {
                GameObject fireball = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);

                Fireball fireBall = fireball.GetComponent<Fireball>();

                // Set the clicked enemy as the target
                fireBall.SetTarget(hit.collider.gameObject);
            }
        }
        isUsingSkill = false;
    }
    void Incineration()
    {
        flameIncinerationParticleSystem.Play();

        // 효과의 지속 시간을 제어하기 위한 코루틴 실행
        StartCoroutine(StopFlameIncinerationEffect());
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
                            collider.gameObject.SetActive(false);

                            PoolManager.instance.RemoveFromPool(collider.gameObject);
                            Player.GetExp(3);
                        }
                    }
                    if (annie != null)
                    {
                        Annie.Instance.m_Annie.m_nHp -= PlayerMove.Instance.m_cPlayer.m_sStatus.nStr;
                        if (Annie.Instance.m_Annie.Death())
                        {
                            collider.gameObject.SetActive(false);
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
        isUsingSkill = false;
    }
    IEnumerator StopFlameIncinerationEffect()
    {
        float effectDuration = 0.5f; // 필요에 따라 지속 시간을 조정하세요

        yield return new WaitForSeconds(effectDuration);

        // 화염 소각 효과 중지
        flameIncinerationParticleSystem.Stop();
    }
    public void SetTarget(GameObject newTarget)
    {
        m_objTarget = newTarget;
    }
    void SetMouseTarget()
    {
        GameObject mouseTarget = GetClickedEnemy();

        if (mouseTarget != null)
        {
            SetTarget(mouseTarget);
        }
    }
    GameObject GetClickedEnemy()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("Enemy", "Annie");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return hit.collider.gameObject;
        }
        return null;
    }
    //가장 가까운 적을 찾는 로직
    /*void SetNearestTarget()
    {
        GameObject nearestTarget = GetClickedEnemy();

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
    }*/

    void ActivateLavaShield()
    {
        //Debug.Log("Annie P: 라바 쉴드");

        StartCoroutine(LavaShieldCoroutine());
    }

    IEnumerator LavaShieldCoroutine()
    {
        PlayerMove.Instance.m_cPlayer.m_nHp += 10;

        shieldParticleSystem.Play();
        float shieldDuration = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < shieldDuration)
        {
            yield return null;

            elapsedTime += Time.deltaTime;
        }

        shieldParticleSystem.Stop();
        //Debug.Log("Annie P: 라바 쉴드 종료");
        PlayerMove.Instance.m_cPlayer.m_nHp -= 10;
    }

    void MeteorS()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int layerMask = LayerMask.GetMask("Enemy", "Annie");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider != null && (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Annie")))
            {
                Vector3 spawnPosition = hit.point;

                GameObject meteor = Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);

                Meteor meteorScript = meteor.GetComponent<Meteor>();
            }
        }
        isUsingSkill = false;
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
    
    /*public void OnGUI()
    {
        Vector3 vPos = this.transform.position;
        Vector3 vPosToScreen = Camera.main.WorldToScreenPoint(vPos);
        vPosToScreen.y = Screen.height - vPosToScreen.y; 
        int h = 50;
        int w = 100;

        Rect rectGUI = new Rect(vPosToScreen.x, vPosToScreen.y, w, h);
        //GUI.Box(rectGUI, "MoveBlock:" + isMoveBlock);
        GUI.Box(rectGUI, string.Format(":{0}", stunStack));
    }*/
}