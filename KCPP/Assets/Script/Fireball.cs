using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextRPG;

public class Fireball : MonoBehaviour
{
    public float fireballSpeed = 10f;
    private Rigidbody rb;
    private GameObject target;
    Annie m_Annie;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Launch();
    }
    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;

            rb.velocity = direction * fireballSpeed;
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
            PlayerMove.Instance.m_cPlayer.m_nHp -= 10;

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

