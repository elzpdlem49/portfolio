using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TextRPG;
using static TextRPG.PlayerManager;
using System.Security.Cryptography;

public class Annie : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public int damage = 10;
    public GameObject fireballPrefab;
    public float fireballSpeed = 10f;
    public float fireballDuration = 3f;
    public float m_fAngle = 90;
    public float m_fSite = 3;
    public float m_Range = 1;

    public LayerMask m_LayerMask;
    public GameObject m_objTarget = null;
    bool m_isHit;
    [SerializeField]
    Player m_Annie;

    public static Annie instance;
    public enum Skill
    {
        Fireball,
        Incineration,
        LavaShield,
        Summon
    }

    private Skill[] skills = { Skill.Fireball, Skill.Incineration, Skill.LavaShield, Skill.Summon };

    public int stunStack = 0;

    // �� ��ų�� ���� ��ٿ�
    private Dictionary<Skill, float> skillCooldowns = new Dictionary<Skill, float>();
    public float skillCooldownInterval = 3f; // ���ϴ� ��ٿ� ���� �� ����
    private float nextSkillCooldown;

    private void Awake()
    {
        Annie.instance = this;
    }
    private void Start()
    {
        m_Annie = new Player(this.gameObject.name, 100, 100, 10, 0, 0);
    }
    // �� �����Ӹ��� ȣ��Ǵ� Update �޼���
    void Update()
    {
        // �÷��̾ ���� ���� ���� �ִ��� Ȯ��
        Vector3 direction = (m_objTarget.transform.position - transform.position).normalized;
        if (Vector3.Distance(transform.position, m_objTarget.transform.position) <= detectionRange)
        {
            // �÷��̾� ������ �̵�
            MoveTowardsPlayer();

            // �÷��̾ ���� ���� ���� �ִ��� Ȯ��
            if (Vector3.Distance(transform.position, m_objTarget.transform.position) <= attackRange)
            {
                UseRandomSkillWithCooldown();
            }
        }

        // ��ų ��ٿ� ������Ʈ
        UpdateCooldowns();
    }

    void MoveTowardsPlayer()
    {
        // �÷��̾� ������ �̵��ϴ� ���� ���
        Vector3 direction = (m_objTarget.transform.position - transform.position).normalized;

        // �÷��̾� ������ �̵�
        transform.Translate(direction * moveSpeed * Time.deltaTime);
        transform.LookAt(transform.position);
    }

    void UseRandomSkillWithCooldown()
    {
        // �Ϲ� ��ų ��ٿ��� �������� Ȯ��
        if (Time.time >= nextSkillCooldown)
        {
            // Annie�� ��ų �߿��� �������� ����
            Skill randomSkill = skills[Random.Range(0, skills.Length)];

            // ������ ��ų ���
            UseSkill(randomSkill);

            // �Ϲ� ��ų ��ٿ��� ����
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
    void Fireball()
    {
        Debug.Log("Annie AI: ���̾ ���� ��");

        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);

        Fireball fireBall = fireball.GetComponent<Fireball>();

        fireBall.SetTarget(m_objTarget);
    }
    void Incineration()
    {
        Debug.Log("Annie AI: ȭ�� �Ұ� ���� ��");

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

            Debug.DrawRay(vPos, vToTarget, Color.green);//������ �ݴ�� ����. ���� Ȯ�� �ʿ�
        }
    }
    public void SetTarget(GameObject target)
    {
        m_objTarget = target;
    }
    void ActivateLavaShield()
    {
        Debug.Log("Annie AI: ��� �ǵ� Ȱ��ȭ ��");

        // ��� �ǵ� ������ ���⿡ �����մϴ�
        StartCoroutine(LavaShieldCoroutine());
    }

    IEnumerator LavaShieldCoroutine()
    {
        m_Annie.m_nHp +=  10;

        // ���� ���� �ð� ���� �ݻ� �������� �����ϴ�
        yield return new WaitForSeconds(3f); // ���� ���� �ð�

        m_Annie.m_nHp -=  10;
    }
    void Summon()
    {
        Debug.Log("Annie AI: ��ȯ ��");
        // ��ȯ ������ ���⿡ ����
    }

    void Stun()
    {
        Debug.Log("Annie AI: ����!");
        // ���� ������ ���⿡ ����
        // ���� ���, AI�� �̵��� �����ϰų� ���� �ð� ���� ��ų ����� ��Ȱ��ȭ�� �� �ֽ��ϴ�.
        stunStack = 0; // ���� �� ���� �ʱ�ȭ
    }

    // ��ų ��ٿ��� ������Ʈ�ϴ� ����� �޼���
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