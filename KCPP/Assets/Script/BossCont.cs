using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class BossCont : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 10f;
    public float serchRange = 20f;
    public float attackRange = 3f;
    Rigidbody enemyRigidbody;
    public float attackCooldown = 5f;
    bool isAttackCooldown = false;
    public float m_BossDamage1 = 10;
    public float m_BossDamage2 = 20;
    public float m_BossDamage3 = 30;

    public bool isTouch;
    // Start is called before the first frame update
    void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody>();
        //StartCoroutine(BossFight());
    }

    // Update is called once per frame
    private void Update()
    {
        if (Vector3.Distance(transform.position, player.position) < attackRange && !isAttackCooldown)
        {
            StartCoroutine(AttackPattern());
        }
    }
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, player.position) < serchRange)
        {
            FollowPlayer();
        }
    }
    void FollowPlayer()
    {
        if (!isTouch)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            enemyRigidbody.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            toRotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 5f);
        }
    }

   /* IEnumerator BossFight()
    {
        while (true)
        {
            yield return null;
        }
    }*/

    IEnumerator AttackPattern()
    {
        isAttackCooldown = true;
        Debug.Log("Boss is attacking!");
        int random = Random.Range(0, 3);

        switch (random)
        {
            case 0:
                yield return StartCoroutine(AttackPattern1());
                break;
            case 1:
                yield return StartCoroutine(AttackPattern2());
                break;
            case 2:
                yield return StartCoroutine(AttackPattern3());
                break;
        }

        yield return new WaitForSeconds(attackCooldown);
    }
    IEnumerator AttackPattern1()
    {
        Debug.Log("Executing Attack Pattern 1");
        yield return null;
    }

    IEnumerator AttackPattern2()
    {
        Debug.Log("Executing Attack Pattern 2");
        yield return null;
    }

    IEnumerator AttackPattern3()
    {
        Debug.Log("Executing Attack Pattern 3");
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == ("Player"))
        {
            isTouch = true;
            Debug.Log(collision.gameObject.name);
        }
        else if (collision.gameObject.CompareTag("Sword"))
        {
            Debug.Log("Sword");
        }
        else if (collision.gameObject.tag == "Bullet")
        {

        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == ("Player"))
        {
            isTouch = false;
        }
    }
}
