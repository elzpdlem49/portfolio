using System.Collections;
using System.Collections.Generic;
using TextRPG;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    ParticleSystem _ps;
    public ParticleSystem.Particle[] particles;
    float damageRadius = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        Collider meteorCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        _ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[_ps.main.maxParticles]; // 배열 초기화
    }
    /*// 기본 트리거엔터를 사용하면 파티클의 콜라이더가 고정되어있는 문제가 발생
    // 파티클 자체의 콜라이더를 사용해야한다
    void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is an enemy or has the tag "Enemy" or "Annie"
        if (other.CompareTag("Enemy") || other.CompareTag("Annie"))
        {
            // Calculate the distance between the meteor and the collided object
            float distance = Vector3.Distance(transform.position, other.transform.position);

            // If the object is within the damage radius, apply damage
            if (distance <= damageRadius)
            {
                ApplyDamage(other.gameObject);
            }
        }
        Destroy(gameObject, 2f);
    }*/

    private void OnParticleCollision(GameObject other) // 옳바르게 사용가능한 파티클 충돌체크
    {
        Debug.Log($"{name} hit! by {other.gameObject.name}");

        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius, LayerMask.GetMask("Enemy", "Annie"));
        Debug.Log("count:" + colliders.Length);

        foreach (var collider in colliders)
        {
            ApplyDamage(collider.gameObject);
        }

        Destroy(gameObject, 1f);
    }
   /* //파티클 트리거는 파티클을 호출할때 사용하는 함수이기때문에 현재 구현에서는 사용할수 없다
    private void OnParticleTrigger()
    {
        Debug.Log("OnParticleTrigger");
        List<ParticleSystem.Particle> insideParticles = new List<ParticleSystem.Particle>();
        int numInsideParticles = _ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, insideParticles);
        Debug.Log("GetTriggerParticles");
        for (int i = 0; i < numInsideParticles; i++)
        {
            Vector3 particlePosition = insideParticles[i].position;

            // 주변에 있는 Enemy 또는 Annie에게 데미지 적용
            Collider[] colliders = Physics.OverlapSphere(particlePosition, damageRadius, LayerMask.GetMask("Enemy", "Annie"));
            Debug.Log("count:" + colliders.Length);
            foreach (var collider in colliders)
            {
                ApplyDamage(collider.gameObject);
            }
        }
        Debug.Log("GetTriggerParticles");
        Destroy(gameObject);
    }*/

    void ApplyDamage(GameObject target)
    {
        Enemycontroller enemy = target.GetComponent<Enemycontroller>();
        Annie annie = target.GetComponent<Annie>();

        // Apply damage based on the type of target
        if (enemy != null)
        {
            enemy.TakeDamage(PlayerMove.Instance.m_cPlayer.m_sStatus.nStr);
            if (enemy.Death())
            {
                target.gameObject.SetActive(false);
                PoolManager.instance.RemoveFromPool(target.gameObject);
                Player.GetExp(3);
            }
        }
        else if (annie != null)
        {
            Annie.Instance.m_Annie.m_nHp -= PlayerMove.Instance.m_cPlayer.m_sStatus.nStr;
            if (Annie.Instance.m_Annie.Death())
            {
                target.gameObject.SetActive(false);
                Player.GetExp(5);
            }
        }
    }
}
