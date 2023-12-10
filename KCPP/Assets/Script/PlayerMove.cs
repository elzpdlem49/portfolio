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
    Vector3 m_Move;
    Vector3 m_Run;
    public float m_speed = 8f;
    public float m_RunSpeed = 12f;
    public float m_JumpForce = 5f;
    public float m_dashForce = 15f;
    public float m_DashCoolTime = 0.5f;
    public float m_OriginalSpeed;
    
    public bool isRuning = false;
    public bool isDashing = false;
    public bool isGrounded;
    public bool isRolling;
    float hz;
    float vt;

    // Start is called before the first frame update
    void Start()
    {
        m_cPlayer = new Player(this.gameObject.name, 100, 100, 10, 0, 0);
        m_Rigdbody = GetComponent<Rigidbody>();
        m_OriginalSpeed = m_speed;
        Instance = this;
    }

    public void OnGUI()
    {
        //오브젝트의 3d좌표를 2d좌표(스크린좌표)로 변환하여 GUI를 그린다.
        Vector3 vPos = this.transform.position;
        Vector3 vPosToScreen = Camera.main.WorldToScreenPoint(vPos); //월드좌표를 스크린좌표로 변환한다.
        vPosToScreen.y = Screen.height - vPosToScreen.y; //y좌표의 축이 하단을 기준으로 정렬되므로 상단으로 변환한다.
        int h = 40;
        int w = 100;
        Rect rectGUI = new Rect(vPosToScreen.x, vPosToScreen.y, w, h);
        //GUI.Box(rectGUI, "MoveBlock:" + isMoveBlock);
        GUI.Box(rectGUI, string.Format("HP:{1}\nMP:{0}", m_cPlayer.m_nMp, m_cPlayer.m_nHp));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        if (Input.GetButtonDown("Fire1") && !isDashing)
        {
            StartCoroutine(Dash());
        }
        if (Input.GetButtonDown("Fire3"))
        {
            isRuning = true;
        }
        if (Input.GetButtonUp("Fire3"))
        {
            isRuning = false;
        }
        if(Input.GetMouseButtonDown(2))
        {
            FindAndLookAtEnemy();
        }
    }

    private void FixedUpdate()
    {
        hz = Input.GetAxis("Horizontal");
        vt = Input.GetAxis("Vertical");

        m_Move = new Vector3(hz, 0f, vt).normalized * m_speed * Time.deltaTime;
        m_Run = new Vector3(hz, 0f, vt).normalized * m_RunSpeed * Time.deltaTime;
        //transform.Translate(m_Move);
        m_Rigdbody.MovePosition(transform.position + m_Move);
        

        if (m_Move.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(m_Move, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 10f);

        }

        if (m_Move.magnitude > 0.1f )
        {
            Quaternion toRotation = Quaternion.LookRotation(m_Move, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 10f);
        }
        if(isRuning)
        {
            Runing();
        }
        else
            m_Rigdbody.MovePosition(transform.position + m_Move); // 플레이어 이동 
       
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

        m_Rigdbody.AddForce(m_Move.normalized * m_dashForce, ForceMode.Impulse);

        yield return new WaitForSeconds(m_DashCoolTime);

        m_speed = m_OriginalSpeed;
        isDashing = false;
    }
    void FindAndLookAtEnemy()
    {
        // Assume your enemies are tagged with "Enemy", you can change it accordingly
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length > 0)
        {
            // Find the nearest enemy
            GameObject nearestEnemy = GetNearestEnemy(enemies);

            // Look at the enemy
            if (nearestEnemy != null)
            {
                Vector3 directionToEnemy = nearestEnemy.transform.position - transform.position;
                directionToEnemy.y = 0; // Ensure the rotation is only horizontal
                Quaternion toRotation = Quaternion.LookRotation(directionToEnemy, Vector3.up);
                transform.rotation = toRotation;
            }
        }
    }
    GameObject GetNearestEnemy(GameObject[] enemies)
    {
        GameObject nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
}  
