using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextRPG;
public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance;
    [SerializeField] 
    public Player m_cPlayer;
    Rigidbody m_Rigdbody;
    Vector3 m_Walk;
    Vector3 m_Run;
    public float m_speed = 1f;
    public float m_RunSpeed = 4f;
    public float m_JumpForce = 5f;
    public float m_dashForce = 15f;
    public float m_DashCoolTime = 0.5f;
    public float m_OriginalSpeed;
    
    public bool isWalking = false;
    public bool isRuning = true;
    public bool isRunorWalk;
    public bool isDashing = false;
    public bool isGrounded;
    //public bool isRolling;
    float hz;
    float vt;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        m_cPlayer = new Player(this.gameObject.name, 100, 100, 10, 0, 0);
        m_Rigdbody = GetComponent<Rigidbody>();
        m_OriginalSpeed = m_speed;
        Instance = this;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        if (Input.GetButtonDown("Jump") && !isDashing)
        {
            StartCoroutine(Dash());
        }
        if (Input.GetButtonDown("Fire1"))
        {
            ToggleRun();
        }

        anim.SetBool("Walk", isWalking);
        anim.SetBool("Run", isRuning);
        
    }
    void ToggleRun()
    {
        isRunorWalk = !isRunorWalk;
        
    }
    
    private void FixedUpdate()
    {
        hz = Input.GetAxis("Horizontal");
        vt = Input.GetAxis("Vertical");

        /*m_Move = new Vector3(hz, 0f, vt).normalized * m_speed * Time.deltaTime;
        m_Run = new Vector3(hz, 0f, vt).normalized * m_RunSpeed * Time.deltaTime;
        //transform.Translate(m_Move);
        m_Rigdbody.MovePosition(transform.position + m_Move);*/
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f;
        Vector3 moveDirection = (hz * Camera.main.transform.right + vt * cameraForward).normalized;

        m_Walk = moveDirection * m_speed * Time.deltaTime;
        m_Run = moveDirection * m_RunSpeed * Time.deltaTime;

        if (m_Walk.magnitude > 0.01f)
        {
            if (isRunorWalk)
            {
                isRuning = true;
                isWalking = false;
            }
            else
            {
                isWalking = true;
                isRuning = false;
            }
            Quaternion toRotation = Quaternion.LookRotation(m_Walk, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 10f);
        }
        else
        {
            if (isRunorWalk)
            {
                isRuning = false;
            }
            else
            {
                isWalking = false;
            }
        }
        if (isRuning)
        {
            Runing();
        }
        else
            m_Rigdbody.MovePosition(transform.position + m_Walk); // 플레이어 이동 
    }
    void Runing()
    {
        m_Rigdbody.MovePosition(transform.position + m_Run);
    }
    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
   
    void Jump()
    {
        m_Rigdbody.AddForce(Vector3.up * m_JumpForce, ForceMode.Impulse);
    }

    IEnumerator Dash()
    {
        isDashing = true;
        m_speed = m_dashForce;

        m_Rigdbody.AddForce(m_Walk.normalized * m_dashForce, ForceMode.Impulse);

        yield return new WaitForSeconds(m_DashCoolTime);

        m_speed = m_OriginalSpeed;
        isDashing = false;
    }  
}  
