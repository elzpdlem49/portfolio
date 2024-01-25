using System.Collections;
using System.Collections.Generic;
using TextRPG;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    protected Player player;

    Rigidbody rigidBody;
    float MoveSpeed = 3f;

    private void Start()
    {
        player = GetComponent<Player>();
        rigidBody = GetComponent<Rigidbody>();
    }
    public Vector3 direction { get; private set; }
    
    
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0f, input.y);
    }

    protected void Move()
    {
        LookAt();
        rigidBody.velocity = direction * MoveSpeed + Vector3.up * rigidBody.velocity.y;
    }

    protected void LookAt()   // 캐릭터가 이동하는 방향을 바라봄
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetAngle = Quaternion.LookRotation(direction);
            rigidBody.rotation = targetAngle;
        }
    }
    void FixedUpdate()
    {
        Move();
    }
}
