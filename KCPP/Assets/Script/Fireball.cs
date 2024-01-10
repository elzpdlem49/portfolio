using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextRPG;

public class Fireball : MonoBehaviour
{
    public float fireballSpeed = 1f;
    private Rigidbody rb;
    public GameObject target;
    public TargetType targetType;
    Annie m_Annie;

    public enum TargetType
    {
        Player,
        Enemy,
        Annie
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        transform.position += Vector3.up * 1.0f;
        Launch();
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            // Rotate the fireball towards the target
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Quaternion toRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(toRotation);

            // Move the fireball forward
            rb.velocity = transform.forward * fireballSpeed;
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject == target)
        {
            if (target == PlayerMove.Instance.gameObject)
            {
                PlayerMove.Instance.m_cPlayer.m_nHp -= Annie.Instance.m_Annie.m_sStatus.nStr;
            }
            else if (target == AnPlayer.Instance.m_objTarget)
            {
                Enemycontroller enemy = collider.gameObject.GetComponent<Enemycontroller>();
                Annie annie = collider.gameObject.GetComponent<Annie>();
                if (enemy != null)
                {
                    enemy.TakeDamage(PlayerMove.Instance.m_cPlayer.m_sStatus.nStr);

                    if (enemy.Death())
                    {
                        Destroy(collider.gameObject);
                        Player.GetExp(3);
                    }
                }
                else if (annie != null)
                {
                    Annie.Instance.m_Annie.m_nHp -= PlayerMove.Instance.m_cPlayer.m_sStatus.nStr;
                    if (Annie.Instance.m_Annie.Death())
                    {
                        Destroy(collider.gameObject);
                        Player.GetExp(5);
                    }
                }
            }
            Destroy(gameObject);
        }
    }

    void Launch()
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            rb.velocity = direction * fireballSpeed;
        }
        else
        {
            // If there is no target, destroy the fireball
            Destroy(gameObject);
        }
    }
}