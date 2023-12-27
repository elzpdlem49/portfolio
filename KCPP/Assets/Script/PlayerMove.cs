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
    public int count = 0;
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
    public bool isSprinting = false;

    float hz;
    float vt;
    private bool m_isHit; // New variable to track if the player is hit

    public bool IsHit
    {
        get { return m_isHit; }
    }
    public void OnHit()
    {
        m_isHit = true;
    }
    public Animator anim;

    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Sprinting,
        Rolling,
        Jumping,
        Parrying,
        FindEnemy
    }

    public PlayerState m_eCurrentState;

    public enum FindState
    {
        Idle,
        Forward,
        Left,
        Right,
        Backward,
        Rolling,
        
    }
    public FindState eFindCrurrentState;
    // Start is called before the first frame update
    void Start()
    {
        m_cPlayer = new Player(name, 100, 100, 10, 0, 0);
        m_Rigdbody = GetComponent<Rigidbody>();
        m_OriginalSpeed = m_speed;
        Instance = this;
        anim = GetComponent<Animator>();
        m_eCurrentState = PlayerState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (Annie.Instance.controlEnabled)
        {
            HandleInput();
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                ToggleRun();
            }
            isRolling = FindEnemy.Instance.isCameraFixed && Input.GetKeyDown(KeyCode.Space);
            isSprinting = !FindEnemy.Instance.isCameraFixed && Input.GetKeyDown(KeyCode.Space);
        }
    }
    void ToggleRun()
    {
        isRunorWalk = !isRunorWalk;
    }
    
    private void FixedUpdate()
    {
        if (Annie.Instance.controlEnabled)
        {
            HandleMovement();
            hz = Input.GetAxis("Horizontal");
            vt = Input.GetAxis("Vertical");
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0f;
            Vector3 moveDirection = (hz * Camera.main.transform.right + vt * cameraForward).normalized;

            m_Walk = moveDirection * m_speed * Time.deltaTime;
            if (!FindEnemy.Instance.isCameraFixed)
            {
                if (m_Walk.magnitude > 0.01f)
                {
                    Quaternion toRotation = Quaternion.LookRotation(m_Walk, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 10f);
                }
            }
        }
        
    }
    void Walk()
    {
        m_Rigdbody.MovePosition(transform.position + m_Walk);
        anim.SetBool("isWalk", true);
    }
    void Runing()
    {
        m_Rigdbody.MovePosition(transform.position + m_Walk * m_RunSpeed);
        anim.SetBool("isRun", true);
    }
    void Sprint()
    {
        m_Rigdbody.MovePosition(transform.position + m_Walk * m_SprintSpeed);
        anim.SetBool("isSprint", true);
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
        Debug.Log("RollCor");
        anim.SetBool("isRoll", true);
        //m_Rigdbody.AddForce(m_Walk.normalized * m_dashForce, ForceMode.Impulse);

        yield return new WaitForSeconds(m_RollCoolTime);
        anim.SetBool("isRoll", false);
        //m_speed = m_OriginalSpeed;
        isRolling = false;
    }
    
    void FindEnemyState()
    {
        
        switch (eFindCrurrentState)
        {
            case FindState.Idle:
                if(hz > 0)
                {
                    eFindCrurrentState = FindState.Forward;
                }
                break;
            case FindState.Forward:
                anim.SetBool("isForward", true);
                break;
            case FindState.Backward:
                break;
            case FindState.Left:
                break;
            case FindState.Right: 
                break;
            default:
                break;
        }
    }
    void HandleInput()
    {
        switch (m_eCurrentState)
        {
            case PlayerState.Idle:
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
                else if (isRolling)
                {
                    m_eCurrentState = PlayerState.Rolling;
                }
                else if(FindEnemy.Instance.isCameraFixed)
                {
                    m_eCurrentState = PlayerState.FindEnemy;
                    anim.SetBool("isFind", true);
                }
                break;

            case PlayerState.Walking:
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
                else if (isSprinting)
                {
                    lastSpacebarPressTime = Time.time;
                    m_eCurrentState = PlayerState.Sprinting;
                    anim.SetBool("isSprint", true);
                }
                else if (isRolling)
                {
                    m_eCurrentState= PlayerState.Rolling;
                    
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
                else if (isSprinting)
                {
                    lastSpacebarPressTime = Time.time;
                    m_eCurrentState = PlayerState.Sprinting;
                    anim.SetBool("isSprint", true);
                }
                else if (isRolling)
                {
                    m_eCurrentState = PlayerState.Rolling;
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
                else if (Input.GetKeyDown(KeyCode.Space)) // 점프 스택 증가 초기화
                {
                    count = (count + 1) % 2;
                    if (count == 1)
                    {
                        m_eCurrentState = PlayerState.Jumping;
                        anim.SetTrigger("Jump");
                    }
                }
                else if (!Input.GetKey(KeyCode.Space) && Time.time - lastSpacebarPressTime > 1f && !isRunorWalk)
                {
                    m_eCurrentState = PlayerState.Walking;
                    anim.SetBool("isSprint", false);
                }
                else if (!Input.GetKey(KeyCode.Space) && Time.time - lastSpacebarPressTime > 1f && isRunorWalk)
                {
                    m_eCurrentState = PlayerState.Running;
                    anim.SetBool("isSprint", false);
                }
                break;
            case PlayerState.Rolling:
                if(!isRolling)
                {
                    m_eCurrentState = PlayerState.Idle;
                }
                break;
            case PlayerState.Parrying:
                break;
            case PlayerState.FindEnemy:
                if (!FindEnemy.Instance.isCameraFixed)
                {
                    m_eCurrentState = PlayerState.Idle;
                    anim.SetBool("isFind", false);
                }
                break;
            default:
                break;
        }
    }
   
    void HandleMovement()
    {
        switch (m_eCurrentState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Walking:
                Walk();
                break;
            case PlayerState.Running:
                Runing();
                break;
            case PlayerState.Sprinting:
                Sprint();
                break;
            case PlayerState.Rolling:
                StartCoroutine(Roll());
                break;
            case PlayerState.Parrying:
                break;
            case PlayerState.FindEnemy:
                FindEnemyState();
                break;
            default:
                break;
        }
    }

    void DisableContralos()
    {
        Annie.Instance.controlEnabled = false;
    }

    void EnableControls()
    {
        Annie.Instance.controlEnabled = true;
    }
}  
