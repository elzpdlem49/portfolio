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
        particles = new ParticleSystem.Particle[_ps.main.maxParticles]; // �迭 �ʱ�ȭ
    }
    /*// �⺻ Ʈ���ſ��͸� ����ϸ� ��ƼŬ�� �ݶ��̴��� �����Ǿ��ִ� ������ �߻�
    // ��ƼŬ ��ü�� �ݶ��̴��� ����ؾ��Ѵ�
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

    private void OnParticleCollision(GameObject other) // �ǹٸ��� ��밡���� ��ƼŬ �浹üũ
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
   /* //��ƼŬ Ʈ���Ŵ� ��ƼŬ�� ȣ���Ҷ� ����ϴ� �Լ��̱⶧���� ���� ���������� ����Ҽ� ����
    private void OnParticleTrigger()
    {
        Debug.Log("OnParticleTrigger");
        List<ParticleSystem.Particle> insideParticles = new List<ParticleSystem.Particle>();
        int numInsideParticles = _ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, insideParticles);
        Debug.Log("GetTriggerParticles");
        for (int i = 0; i < numInsideParticles; i++)
        {
            Vector3 particlePosition = insideParticles[i].position;

            // �ֺ��� �ִ� Enemy �Ǵ� Annie���� ������ ����
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
