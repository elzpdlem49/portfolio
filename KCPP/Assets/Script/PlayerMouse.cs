using System.Collections;
using System.Collections.Generic;
using TextRPG;
using UnityEngine;
using static PlayerMove;
using static TextRPG.PlayerManager;

public class PlayerMouse : MonoBehaviour
{
    public float moveSpeed;
    private bool isMove;
    private Vector3 destination;
    Rigidbody m_Rigdbody;
    Animator animator;

    void Start()
    {
        m_Rigdbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                //Debug.Log($"충돌된 물체 이름 : {hit.transform.name}, Position : {hit.point}");
                destination = hit.point;
                isMove = true;
            }
        }
        Move();
    }


    private void Move()
    {
        if (isMove)
        {
            bool isAlived = Vector3.Distance(destination, transform.position) <= 0.1f;
            if (isAlived)
            {
                isMove = false;
                animator.SetBool("isWalk", false);
            }
            else
            {
                Vector3 direction = destination - transform.position;
                transform.forward = direction;
                transform.position += direction.normalized * moveSpeed * Time.deltaTime;
                animator.SetBool("isWalk", true );
            }
        }
    }

}