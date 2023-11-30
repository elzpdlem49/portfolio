using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Collider m_ColSphere;
    public float attackForce = 10f;
    public bool m_Attack = true;
    // Start is called before the first frame update
    void Start()
    {
        m_ColSphere = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }
    void Attack()
    {
        if (m_ColSphere != null)
        {
            StartCoroutine(EnAttack(0.5f));
            Debug.Log("Attack");
        }
    }
    IEnumerator EnAttack(float delay)
    {
        m_Attack = false;
        m_ColSphere.enabled = true;

        yield return new WaitForSeconds(delay);

        m_ColSphere.enabled = false;
        m_Attack = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            if (enemyRigidbody != null)
            {
                Vector3 pushDirection = (collision.gameObject.transform.position - transform.position).normalized;
                enemyRigidbody.AddForce(pushDirection * attackForce, ForceMode.Impulse);
            }
        }
    }
}
