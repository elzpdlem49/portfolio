using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform player;
    //public Vector3 offset = new Vector3(0f, 10f, -10f);
    public float smoothSpeed = 5f;

    public float mouseSensitivity = 400f;

    private float MouseY;
    private float MouseX;
    public bool isMouseRotate = true;


    private void Start()
    {
        MouseX = transform.rotation.eulerAngles.x;
        MouseY = transform.rotation.eulerAngles.y;
    }
    void LateUpdate()
    {
        CheckMouseInput();
        Vector3 desiredPosition = player.position - transform.forward * 5f;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(player.position);
    }
    void CheckMouseInput()
    {
        if (Input.GetMouseButton(1))
        {
            Rotate();
        }
    }
    private void Rotate()
    {
        if (isMouseRotate)
        {
            MouseX -= Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
            MouseY += Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
            MouseX = Mathf.Clamp(MouseX, -1f, 70f);
            transform.localRotation = Quaternion.Euler(MouseX, MouseY, 0f);
        }
    }
    public void EnableMouseRotation()
    {
        isMouseRotate = true;
    }

    public void DisableMouseRotation()
    {
        isMouseRotate = false;
    }
}