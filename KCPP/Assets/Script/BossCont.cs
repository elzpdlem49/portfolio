using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using TextRPG;

public class BossCont : MonoBehaviour
{
    public Transform trfPlayer;
    public TextRPG.Status m_Boss;
    public float moveSpeed = 10f;
    public float serchRange = 20f;
    public float attackRange = 3f;


    Rigidbody bossRb;
    public float attackCooldown = 5f;
    bool isAttackCooldown = false;
    public int m_BossDamage1 = 10;
    public int m_BossDamage2 = 20;
    public int m_BossDamage3 = 30;

    public bool isTouch;
    // Start is called before the first frame update
    void Start()
    {
        bossRb = GetComponent<Rigidbody>();
        m_Boss = new Status(1000, 1000, 10, 10);
        //StartCoroutine(BossFight());
    }

    // Update is called once per frame
    private void Update()
    {
        if (Vector3.Distance(transform.position, trfPlayer.position) < attackRange && !isAttackCooldown)
        {
            StartCoroutine(AttackPattern());
        }
    }
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, trfPlayer.position) < serchRange)
        {
            FollowPlayer();
        }
    }
    void FollowPlayer()
    {
        if (!isTouch)
        {
            Vector3 direction = (trfPlayer.position - transform.position).normalized;
            if (Vector3.Distance(transform.position, trfPlayer.position) < attackRange)
            {
                bossRb.velocity = Vector3.zero;

               /* Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                toRotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 5f);
                enemyRigidbody.MoveRotation(toRotation);*/

                StartCoroutine(AttackPattern());
            }
            else
            {
                bossRb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                toRotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 5f);
            }
        }
    }

    /*IEnumerator BossFight()
    {
        while (true)
        {
            yield return StartCoroutine(AttackPattern());
            yield return new WaitForSeconds(attackCooldown);
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
        //m_cPlayer.m_nHp -= m_BossDamage1;
        yield return null;
    }

    IEnumerator AttackPattern2()
    {
        Debug.Log("Executing Attack Pattern 2");
        //m_cPlayer.m_nHp -= m_BossDamage2;
        yield return null;
    }

    IEnumerator AttackPattern3()
    {
        Debug.Log("Executing Attack Pattern 3");
        //m_cPlayer.m_nHp -= m_BossDamage3;
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
