using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Annie : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public int damage = 10;
    public GameObject fireballPrefab;
    public GameObject lavaShieldPrefab;
    public float fireballSpeed = 10f;
    public float fireballDuration = 3f;
    public float m_fAngle = 90;
    public float m_fSite = 3;
    public float m_Range = 1;
    public float shieldCooldown = 3f;

    public LayerMask m_LayerMask;
    public GameObject m_objTarget = null;
    bool m_isHit;
    public enum Skill
    {
        Fireball,
        Incineration,
        LavaShield,
        Summon
    }

    private Skill[] skills = { Skill.Fireball, Skill.Incineration, Skill.LavaShield, Skill.Summon };

    public int stunStack = 0;

    // 각 스킬에 대한 쿨다운
    private Dictionary<Skill, float> skillCooldowns = new Dictionary<Skill, float>();
    public float skillCooldownInterval = 3f; // 원하는 쿨다운 간격 값 설정
    private float nextSkillCooldown;

    // 매 프레임마다 호출되는 Update 메서드
    void Update()
    {
        // 플레이어가 감지 범위 내에 있는지 확인
        Vector3 direction = (m_objTarget.transform.position - transform.position).normalized;
        if (Vector3.Distance(transform.position, m_objTarget.transform.position) <= detectionRange)
        {
            // 플레이어 쪽으로 이동
            MoveTowardsPlayer();

            // 플레이어가 공격 범위 내에 있는지 확인
            if (Vector3.Distance(transform.position, m_objTarget.transform.position) <= attackRange)
            {
                UseRandomSkillWithCooldown();
            }
        }

        // 스킬 쿨다운 업데이트
        UpdateCooldowns();
    }

    void MoveTowardsPlayer()
    {
        // 플레이어 쪽으로 이동하는 방향 계산
        Vector3 direction = (m_objTarget.transform.position - transform.position).normalized;

        // 플레이어 쪽으로 이동
        transform.Translate(direction * moveSpeed * Time.deltaTime);
        transform.LookAt(transform.position);
    }

    void UseRandomSkillWithCooldown()
    {
        // 일반 스킬 쿨다운이 지났는지 확인
        if (Time.time >= nextSkillCooldown)
        {
            // Annie의 스킬 중에서 랜덤으로 선택
            Skill randomSkill = skills[Random.Range(0, skills.Length)];

            // 선택한 스킬 사용
            UseSkill(randomSkill);

            // 일반 스킬 쿨다운을 설정
            nextSkillCooldown = Time.time + skillCooldownInterval;
        }
    }

    void UseSkill(Skill skill)
    {
        switch (skill)
        {
            case Skill.Fireball:
                Fireball();
                break;
            case Skill.Incineration:
                Incineration();
                break;
            case Skill.LavaShield:
                ActivateLavaShield();
                break;
            case Skill.Summon:
                Summon();
                break;
        }

        if (stunStack == 4)
        {
            Stun();
        }
        else
        {
            stunStack++;
        }
    }

    // 이전과 동일한 방식으로 Annie의 각 스킬을 구현

    void Fireball()
    {
        // 여기에 파이어볼 로직을 구현합니다
        Debug.Log("Annie AI: 파이어볼 시전 중");

        // 파이어볼 프로젝타일을 생성합니다 (Unity 편집기에서 미리 제작된 프리팹을 할당해야 합니다)
        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);

        // 플레이어 쪽으로 이동하는 방향을 계산합니다
        Vector3 direction = (m_objTarget.transform.position - transform.position).normalized;

        // 파이어볼에 이동을 시뮬레이션하기 위해 힘을 가합니다
        fireball.GetComponent<Rigidbody>().AddForce(direction * fireballSpeed, ForceMode.Impulse);

        // 선택 사항: 일정 시간이 지난 후 파이어볼을 파괴할 수 있도록 타이머를 설정합니다
        Destroy(fireball, fireballDuration);
    }
    void Incineration()
    {
        Debug.Log("Annie AI: 화염 소각 시전 중");

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
        int nLayer = 1 << LayerMask.NameToLayer("PlayerLayer");
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
                Debug.DrawLine(vPos, vTargetPos, Color.green);
                //m_objTarget = collider.gameObject;
                SetTarget(collider.gameObject);
                
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

    void ActivateLavaShield()
    {
        Debug.Log("Annie AI: Activating Lava Shield");

        // Implement lava shield logic here
        StartCoroutine(LavaShieldCoroutine());
    }

    IEnumerator LavaShieldCoroutine()
    {
        // Assuming you have a lava shield prefab, instantiate it here
        GameObject lavaShield = Instantiate(lavaShieldPrefab, transform.position, Quaternion.identity);

        // Apply the shield effect to the player or AI (adjust as needed)
        // For example, you might enable a "Shield" component on the player or AI

        // Reflect damage during the shield duration
        yield return new WaitForSeconds(3f); // Shield duration

        // After 3 seconds, destroy the shield or deactivate the "Shield" component
        Destroy(lavaShield);

        // Optionally, you can add a cooldown period before the shield can be activated again
        yield return new WaitForSeconds(shieldCooldown);

        // Add any additional logic or cooldown handling here
    }

    void Summon()
    {
        Debug.Log("Annie AI: 소환 중");
        // 소환 로직을 여기에 구현
    }

    void Stun()
    {
        Debug.Log("Annie AI: 스턴!");
        // 스턴 로직을 여기에 구현
        // 예를 들어, AI의 이동을 중지하거나 일정 시간 동안 스킬 사용을 비활성화할 수 있습니다.
        stunStack = 0; // 스턴 후 스택 초기화
    }

    // 스킬 쿨다운을 업데이트하는 도우미 메서드
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
}