using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class AIContllor : MonoBehaviour
{
    [SerializeField]
    GameObject m_objTarget;
    [SerializeField]
    LayerMask m_sLayerMask;

    Gun gun;
    public enum E_AI_STATUS
    {
        NONE,
        TRACKING,
        MOVE,
        ATTACK
    }

    public E_AI_STATUS m_eCurAIState = E_AI_STATUS.NONE;

    public void SetState(E_AI_STATUS state)
    {
        Debug.Log(string.Format("{0}.SetState:{1}", gameObject.name, state));
        m_eCurAIState = state;
        switch (state)
        {
            case E_AI_STATUS.TRACKING:
                ProcessTracking();
                break;
            case E_AI_STATUS.MOVE:
                transform.LookAt(transform);
                //EqmentChange();
                break;
            case E_AI_STATUS.ATTACK:
                StartCoroutine(AttackCoolTime());
                break;
        }
    }

    public void UpdateState()
    {
        switch (m_eCurAIState)
        {
            case E_AI_STATUS.TRACKING:
                ProcessTracking();
                break;
            case E_AI_STATUS.MOVE:
                ProcessMove();
                break;
            case E_AI_STATUS.ATTACK:
                ProcessAttack();
                break;
        }
    }

    [SerializeField]
    public float m_fSite = 5;
    public void ProcessTracking()
    {
        Vector3 vPos = this.transform.position;
        Collider[] colliders = Physics.OverlapSphere(vPos, m_fSite, m_sLayerMask);

        foreach (Collider collider in colliders)
        {
            m_objTarget = collider.gameObject;
            SetState(E_AI_STATUS.MOVE);
            break;
        }
    }
    public virtual void Translate(Vector3 dir, float axis, float speed)
    {
        Vector3 vMove = dir * axis * speed * Time.deltaTime;
        //m_cAnimationController.Run(axis);
        transform.Translate(vMove);
    }
    public void ProcessMove()
    {
        Vector3 vPos = this.transform.position;
        Vector3 vTargetPos = m_objTarget.transform.position;

        float fDist = Vector3.Distance(vPos, vTargetPos);

        if (fDist >= m_fRange)
            Translate(Vector3.forward, 1, 1f);
        else
            SetState(E_AI_STATUS.ATTACK);
    }

    [SerializeField]
    public float m_fRange = 2;
    public float m_fAtkSpd = 1;
    public void ProcessAttack()
    {
        if (!m_objTarget)
        {
            //EqmentChange();
            SetState(E_AI_STATUS.TRACKING);
        }
    }

    IEnumerator AttackCoolTime()
    {
        while (m_eCurAIState == E_AI_STATUS.ATTACK)
        {
            gun.Shot();
            yield return new WaitForSeconds(m_fAtkSpd);
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 vPos = this.transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(vPos, m_fSite);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(vPos, m_fRange);
    }

    // Start is called before the first frame update
    void Start()
    {
        //_Initialize();
        SetState(m_eCurAIState);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }
}
