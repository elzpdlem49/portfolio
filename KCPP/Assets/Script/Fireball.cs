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
        // 플레이어가 설정되었을 때만 플레이어 방향으로 날아가도록 합니다
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            
            transform.Translate(direction * fireballSpeed * Time.deltaTime);
        }
    }

    // 플레이어를 추적하기 위한 메서드
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

