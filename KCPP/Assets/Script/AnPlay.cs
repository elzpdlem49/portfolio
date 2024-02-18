using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TextRPG;
using static PlayerMove;
using static Fireball;
using static UnityEngine.UI.GridLayoutGroup;
using System;

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
    public float m_fRadius = 4;
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

    public float fireballCooldown = 0f;
    public float incinerationCooldown = 0f;
    public float lavaShieldCooldown = 0f;
    public float meteorCooldown = 0f;
    public enum SkillType
    {
        Fireball,
        Incineration,
        LavaShield,
        Meteor
    }



    public int stunStack = 0;


    public List<float> skillCooldowns = new List<float>();

    private void Awake()
    {
        shieldParticleSystem.Stop();
    }
    private void Start()
    {
        Instance = this;
        m_anim = GetComponent<Animator>();
        skillCooldowns.Add(fireballCooldown);
        skillCooldowns.Add(incinerationCooldown);
        skillCooldowns.Add(lavaShieldCooldown);
        skillCooldowns.Add(meteorCooldown);
    }
    void Update()
    {
        if(Annie.Instance.controlEnabled)
        {
            UseSkill();
            UpdateSkillCooldowns();
            SkillUIManager.instance.UpdateSkillUI();
        }
        if (Input.GetMouseButtonDown(1))
        {
            // 마우스 위치에 있는 적을 타겟으로 설정
            GameObject mouseTarget = GetClickedEnemy();

            // 타겟이 존재하면 기본 공격 수행
            if (mouseTarget != null)
            {
                SetTarget(mouseTarget);
                PerformBasicAttack();
            }
        }

        //SetNearestTarget();
        SetMouseTarget();
        
    }
    void PerformBasicAttack()
    {
        // Check if the target is not null
        if (m_objTarget != null)
        {
            // Get the NexusHealth component from the target (assuming Nexus has NexusHealth script attached)
            NexusHealth nexusHealth = m_objTarget.GetComponent<NexusHealth>();

            // Check if NexusHealth component exists
            if (nexusHealth != null)
            {
                // Deal damage to the Nexus using the TakeDamage method
                nexusHealth.TakeDamage(PlayerMove.Instance.m_cPlayer.m_sStatus.nStr);

                // Add any additional logic for effects or behavior after dealing damage
            }
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

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return hit.collider != null && (hit.collider.CompareTag("Red") || hit.collider.CompareTag("Annie"));
        }
        return false;
    }
    public bool isUsingSkill = false;
    public void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.Q) && skillCooldowns[0] <= 0)
        {
            isQActive = true;
        }
        if (isQActive && IsEnemyAtMousePosition())
        {
            if (Input.GetMouseButtonDown(0))
            {
                PlayerMove.Instance.isMove = false;
                isUsingSkill = true;
                RotatePlayerTowardsMouse();
                m_anim.SetTrigger("Fireball");
                Fireball();
                IncrementSkillCounter();
                isQActive = false;
                skillCooldowns[0] = 3f;  // 파이어볼 쿨다운 설정
            }
        }
        if (Input.GetKeyDown(KeyCode.W) && skillCooldowns[1] <= 0)
        {
            PlayerMove.Instance.isMove = false;
            isUsingSkill = true;
            RotatePlayerTowardsMouse();
            m_anim.SetTrigger("Incineration");
            Incineration();
            IncrementSkillCounter();
            isWActive = false;
            skillCooldowns[1] = 5f;  // 화염소각 쿨다운 설정
        }

        if (Input.GetKeyDown(KeyCode.E) && skillCooldowns[2] <= 0)
        {
            isUsingSkill = true;
            m_anim.SetTrigger("LavaShield");
            IncrementSkillCounter();
            ActivateLavaShield();
            skillCooldowns[2] = 7f;  // 라바 쉴드 쿨다운 설정
        }

        if (Input.GetKeyDown(KeyCode.R) && skillCooldowns[3] <= 0)
        {
            PlayerMove.Instance.isMove = false;
            isUsingSkill = true;
            MeteorS();
            skillCooldowns[3] = 10f;  // 메테오 쿨다운 설정
        }
        if (stunStack == 4)
        {
            Stun();
        }
    }
 
    void UpdateSkillCooldowns()
    {
        // 각 스킬에 대한 쿨다운 갱신
        for (int i = 0; i < skillCooldowns.Count; i++)
        {
            skillCooldowns[i] -= Time.deltaTime;
            skillCooldowns[i] = Mathf.Max(0f, skillCooldowns[i]);
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
            if (hit.collider != null && (hit.collider.CompareTag("Red") || hit.collider.CompareTag("Annie")))
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
                    m_objTarget = collider.gameObject;
                    SetTarget(collider.gameObject);
                    Enemycontroller enemyController = collider.GetComponent<Enemycontroller>();
                    Annie annie = collider.GetComponent<Annie>();
                    if (m_objTarget.CompareTag("Red"))
                    {
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
            if (hit.collider != null && (hit.collider.CompareTag("Red") || hit.collider.CompareTag("Annie")))
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