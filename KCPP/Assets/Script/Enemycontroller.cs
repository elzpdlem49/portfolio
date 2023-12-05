using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemycontroller : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 10f;
    public float serchRange = 20f;
    public float attackRange = 5f;
    Rigidbody enemyRigidbody;

    public bool isTouch;

    public Transform[] patrolWaypoints;
    int currentWaypointIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (Vector3.Distance(transform.position, player.position) < serchRange)
        {
            FollowPlayer();
        }
        else
        {
            Patrol();
        }

    }
    void Patrol()
    {
        Vector3 direction = (patrolWaypoints[currentWaypointIndex].position - transform.position).normalized;
        enemyRigidbody.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);

        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        toRotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 5f);
        enemyRigidbody.MoveRotation(toRotation);

        if (Vector3.Distance(transform.position, patrolWaypoints[currentWaypointIndex].position) < 1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
        }
    }
    void FollowPlayer()
    {
        if (!isTouch)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            if (Vector3.Distance(transform.position, player.position) < attackRange)
            {
                enemyRigidbody.velocity = Vector3.zero;

            }
            else
            {
                enemyRigidbody.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                toRotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 5f);
            }
        }
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
