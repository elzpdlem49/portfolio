using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextRPG;
using UnityEngine.Playables;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance;
    [SerializeField] 
    public Player m_cPlayer;
    Rigidbody m_Rigdbody;
    public Vector3 m_Walk;

    public float m_speed = 1f;
    public float m_RunSpeed = 3f;
    public float m_SprintSpeed = 7f;
    public float m_JumpForce = 5f;
    public float m_dashForce = 10f;
    public float m_RollCoolTime = 0.5f;
    public float m_OriginalSpeed;
    public float lastSpacebarPressTime = 0f;
    
    public bool isRunorWalk = true;
    public bool isGrounded;
    public bool isRolling;
    public bool m_Jumping = false;

    float hz;
    float vt;

    
    public Animator anim;

    private enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Rolling,
        Sprinting,
        Jumping
    }

    private PlayerState m_eCurrentState;

    // Start is called before the first frame update
    void Start()
    {
        m_cPlayer = new Player(this.gameObject.name, 100, 100, 10, 0, 0);
        m_Rigdbody = GetComponent<Rigidbody>();
        m_OriginalSpeed = m_speed;
        Instance = this;
        anim = GetComponent<Animator>();
        m_eCurrentState = PlayerState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleRun();
        }
    }
    void ToggleRun()
    {
        isRunorWalk = !isRunorWalk;
    }
    
    private void FixedUpdate()
    {
        HandleMovement();
        hz = Input.GetAxis("Horizontal");
        vt = Input.GetAxis("Vertical");
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f;
        Vector3 moveDirection = (hz * Camera.main.transform.right + vt * cameraForward).normalized;

        m_Walk = moveDirection * m_speed * Time.deltaTime;

        if (m_Walk.magnitude > 0.01f)
        {
            Quaternion toRotation = Quaternion.LookRotation(m_Walk, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 10f);
        }
    }
    void Walk()
    {
        m_Rigdbody.MovePosition(transform.position + m_Walk); // 플레이어 이동 
    }
    void Runing()
    {
        m_Rigdbody.MovePosition(transform.position + m_Walk * m_RunSpeed);
    }
    void Sprint()
    {
        m_Rigdbody.MovePosition(transform.position + m_Walk * m_SprintSpeed);
    }
    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    IEnumerator Roll()
    {
        isRolling = true;
        m_speed = m_dashForce;

        //m_Rigdbody.AddForce(m_Walk.normalized * m_dashForce, ForceMode.Impulse);

        yield return new WaitForSeconds(m_RollCoolTime);

        m_speed = m_OriginalSpeed;
        isRolling = false;
    }
    public int count = 0;
    void HandleInput()
    {
        switch (m_eCurrentState)
        {
            case PlayerState.Idle:
                // "Idle" 상태에서의 입력 처리
                if (m_Walk.magnitude > 0.01f && !isRunorWalk)
                {
                    m_eCurrentState = PlayerState.Walking;
                    anim.SetBool("isWalk", true);
                    anim.SetBool("RunatWalk", false);
                }
                else if (m_Walk.magnitude > 0.01f && isRunorWalk)
                {
                    m_eCurrentState = PlayerState.Running;
                    anim.SetBool("isRun", true);
                    anim.SetBool("RunatWalk", true);
                }
                break;

            case PlayerState.Walking:
                // "Walking" 상태에서의 입력 처리
                if (m_Walk.magnitude < 0.01f)
                {
                    m_eCurrentState = PlayerState.Idle;
                    anim.SetBool("isWalk", false);
                }
                else if (m_Walk.magnitude > 0.01f && isRunorWalk)
                {
                    m_eCurrentState = PlayerState.Running;
                    anim.SetBool("RunatWalk", true);
                    anim.SetBool("isWalk", false);
                }
                else if (Input.GetKey(KeyCode.Space))
                {
                    lastSpacebarPressTime = Time.time;
                    m_eCurrentState = PlayerState.Sprinting;
                    anim.SetBool("isSprint", true);
                }
                else if (isRolling)
                {
                    m_eCurrentState= PlayerState.Rolling;
                    anim.SetBool("isRoll", true) ;
                }
                break;

            case PlayerState.Running:
                // "Running" 상태에서의 입력 처리
                if (m_Walk.magnitude < 0.01f)
                {
                    m_eCurrentState = PlayerState.Idle;
                    anim.SetBool("isRun", false);
                }
                else if (m_Walk.magnitude > 0.01f && !isRunorWalk)
                {
                    m_eCurrentState = PlayerState.Walking;
                    anim.SetBool("RunatWalk", false);
                    anim.SetBool("isRun", false);
                }
                else if (Input.GetKey(KeyCode.Space))
                {
                    lastSpacebarPressTime = Time.time;
                    m_eCurrentState = PlayerState.Sprinting;
                    anim.SetBool("isSprint", true);
                }
                else if (isRolling)
                {
                    m_eCurrentState = PlayerState.Rolling;
                    anim.SetBool("isRoll", true);
                }
                break;

            case PlayerState.Jumping:
                // "Jumping" 상태에서의 입력 처리
                if (isGrounded)
                {
                    m_eCurrentState = PlayerState.Idle;
                }
                break;
            case PlayerState.Sprinting:
                if (m_Walk.magnitude < 0.01f)
                {
                    m_eCurrentState = PlayerState.Idle;
                    anim.SetBool("isSprint", false);
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    
                    count = (count + 1) % 2;
                    if (count == 1)
                    {
                        m_eCurrentState = PlayerState.Jumping;
                        anim.SetTrigger("Jump");
                    }
                }
                else if (!Input.GetKey(KeyCode.Space) && Time.time - lastSpacebarPressTime > 2f)
                {
                    m_eCurrentState = PlayerState.Running;
                    anim.SetBool("isSprint", false);
                    anim.SetBool("isRun", true);
                }
                break;

            default:
                break;
        }
    }
   
    void HandleMovement()
    {
        // 현재 상태에 따른 움직임 처리
        switch (m_eCurrentState)
        {
            case PlayerState.Idle:
                break;

            case PlayerState.Walking:
                // "Walking" 상태에서의 움직임 처리
                Walk();
                anim.SetBool("isWalk", true);
                break;

            case PlayerState.Running:
                // "Running" 상태에서의 움직임 처리
                Runing();
                anim.SetBool("isRun", true);
                break;

            case PlayerState.Sprinting:
                Sprint();
                anim.SetBool("isSprint", true);
                break;

            default:
                break;
        }
    }
}  
