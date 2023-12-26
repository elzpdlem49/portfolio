using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextRPG;

public class Fireball : MonoBehaviour
{
    public float fireballSpeed = 10f;

    private GameObject target;
    Annie m_Annie;
    void Update()
    {
        // �÷��̾ �����Ǿ��� ���� �÷��̾� �������� ���ư����� �մϴ�
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            
            transform.Translate(direction * fireballSpeed * Time.deltaTime);
        }
    }

    // �÷��̾ �����ϱ� ���� �޼���
    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }
    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject == target)
        {
            PlayerMove.Instance.m_cPlayer.m_nHp -= 10;

            Destroy(gameObject);
        }
    }
}

