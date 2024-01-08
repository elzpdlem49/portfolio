using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TextRPG;
using static PlayerMove;

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
    
    public LayerMask m_LayerMask;
    public GameObject m_objTarget = null;
    bool m_isHit;
    Rigidbody m_rigidbody;
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
        m_Annie = new Player(name, 100, 100, 10, 0, 0);
        m_rigidbody = GetComponent<Rigidbody>();
        Instance = this;
        m_rigidbody.isKinematic = true;
        m_anim = GetComponent<Animator>();
    }
    // �� �����Ӹ��� ȣ��Ǵ� Update �޼���
    void FixedUpdate()
    {
        //Vector3 direction = (m_objTarget.transform.position - transform.position).normalized;
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
        Vector3 direction = (m_objTarget.transform.position - transform.position).normalized;

        float yOffset = 1.6f;
        transform.Translate(direction * moveSpeed * Time.deltaTime);
        transform.LookAt(m_objTarget.transform.position);
    }

    void UseRandomSkillWithCooldown()
    {
        if (Time.time >= nextSkillCooldown)
        {
            Skill randomSkill = skills[Random.Range(0, skills.Length)];

            float skillRange = GetSkillRange(randomSkill);

            if (Vector3.Distance(transform.position, m_objTarget.transform.position) <= skillRange)
            {
                UseSkill(randomSkill);
                nextSkillCooldown = Time.time + skillCooldownInterval;
            }
        }
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
                Debug.Log("Annie AI: ���̾ ���� ��");
                break;
            case Skill.Incineration:
                Incineration();
                m_anim.SetTrigger("Incineration");
                Debug.Log("Annie AI: ȭ�� �Ұ� ���� ��");
                PlayerMove.Instance.m_cPlayer.m_nHp -= 5;
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
                m_objTarget = collider.gameObject;
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
    private void OnDrawGizmos()
    {
        Incineration();
        Gizmos.DrawWireSphere(transform.position, m_fRadius);
    }
    void ActivateLavaShield()
    {
        Debug.Log("Annie AI: ��� �ǵ� Ȱ��ȭ ��");

        StartCoroutine(LavaShieldCoroutine());
    }

    IEnumerator LavaShieldCoroutine()
    {
        m_Annie.m_nHp += 10;

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

        Debug.Log("Annie AI: ��� �ǵ� ��Ȱ��ȭ ��");
        m_Annie.m_nHp -= 10;
    }

    void Summon()
    {
        Debug.Log("Annie AI: ��ȯ ��");
        // ��ȯ ������ ���⿡ ����
    }
    public bool controlEnabled = true;
    void Stun()
    {
        Debug.Log("Annie AI: ����!");
        StartCoroutine(StunCoroutine());
        stunStack = 0;
    }
    IEnumerator StunCoroutine()
    {
        controlEnabled = false;
        PlayerMove.Instance.m_eCurrentState = PlayerState.Idle; // ���� �߿� �÷��̾� ���¸� ���̵�� �����մϴ�.

        Debug.Log("�÷��̾�: ���� ��!");
        stunEndTime = Time.time + stunDuration; // ������ ������ �ð��� ����մϴ�.

        while (Time.time < stunEndTime)
        {
            yield return null;
        }

        // ���� �Ⱓ�� ������ �÷��̾� ��Ʈ���� �ٽ� Ȱ��ȭ�մϴ�.
        controlEnabled = true;

        Debug.Log("�÷��̾�: ���� ����!");
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
    public void OnGUI()
    {
        //������Ʈ�� 3d��ǥ�� 2d��ǥ(��ũ����ǥ)�� ��ȯ�Ͽ� GUI�� �׸���.
        Vector3 vPos = this.transform.position;
        Vector3 vPosToScreen = Camera.main.WorldToScreenPoint(vPos); //������ǥ�� ��ũ����ǥ�� ��ȯ�Ѵ�.
        vPosToScreen.y = Screen.height - vPosToScreen.y; //y��ǥ�� ���� �ϴ��� �������� ���ĵǹǷ� ������� ��ȯ�Ѵ�.
        int h = 50;
        int w = 100;
        
        Rect rectGUI = new Rect(vPosToScreen.x, vPosToScreen.y, w, h);
        //GUI.Box(rectGUI, "MoveBlock:" + isMoveBlock);
        GUI.Box(rectGUI, string.Format("{2}\nHP:{1}\nMP:{0}", m_Annie.m_nMp, m_Annie.m_nHp, stunStack));
    }
}