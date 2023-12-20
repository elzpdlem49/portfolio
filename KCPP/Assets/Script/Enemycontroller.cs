using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextRPG;
//using static TextRPG.PlayerManager;
using Unity.VisualScripting;

public class Enemycontroller : MonoBehaviour
{
    public static Enemycontroller Instance;
    [SerializeField] 
    public Player m_Enemy;
    public Transform player;
    public float moveSpeed = 1f;
    public float serchRange = 5f;
    public float attackRange = 1f;
    Rigidbody enemyRigidbody;
    public Animator anim;
    public bool isTouch;

    

    public enum EnemyState
    {
        Idle,
        Patrol,
        FollowPlayer,
        Attack
    }
    public EnemyState m_eCurrentState;

    public Transform[] patrolWaypoints;
    int currentWaypointIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody>();
        m_Enemy = new Player(name, 100, 100, 10, 0);
        Instance = this;
        anim = GetComponent<Animator>();
        m_eCurrentState = EnemyState.Patrol;
    }
    private void Update()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        EnemyAIState();
        /*if (Vector3.Distance(transform.position, player.position) < serchRange)
        {
            FollowPlayer();
        }
        else
        {
            Patrol();
        }*/

    }
    void SetState()
    {
        switch(m_eCurrentState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Patrol:
                Patrol();
                anim.SetBool("isWalk", true);
                break;
            case EnemyState.FollowPlayer:
                FollowPlayer();
                anim.SetBool("isRun", true);
                break;
            case EnemyState.Attack:
                break;
        }
    }
    void EnemyAIState()
    {
        switch(m_eCurrentState)
        {
            case EnemyState.Patrol:
                if ((Vector3.Distance(transform.position, player.position) < serchRange))
                {
                    m_eCurrentState = EnemyState.FollowPlayer;
                    anim.SetBool("isWalk", false);
                }
                break;
            case EnemyState.FollowPlayer:
                if (!(Vector3.Distance(transform.position, player.position) < serchRange))
                {
                    m_eCurrentState = EnemyState.Patrol;
                    anim.SetBool("isRun", false);
                }
                
                    break;
            case EnemyState.Attack:

                break;
            default:
                break;
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

            /*if (Vector3.Distance(transform.position, player.position) < attackRange)
            {
                enemyRigidbody.velocity = Vector3.zero;

            }
            else*/
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
            PlayerMove.Instance.m_cPlayer.m_nHp -= m_Enemy.m_sStatus.nStr; //�ִϸ��̼�
            isTouch = true;
            /*if (PlayerMove.Instance.m_cPlayer.Death())
            {
                Destroy(collision.gameObject);
            }*/
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
    public void OnGUI()
    {
        //������Ʈ�� 3d��ǥ�� 2d��ǥ(��ũ����ǥ)�� ��ȯ�Ͽ� GUI�� �׸���.
        Vector3 vPos = this.transform.position;
        Vector3 vPosToScreen = Camera.main.WorldToScreenPoint(vPos); //������ǥ�� ��ũ����ǥ�� ��ȯ�Ѵ�.
        vPosToScreen.y = Screen.height - vPosToScreen.y; //y��ǥ�� ���� �ϴ��� �������� ���ĵǹǷ� ������� ��ȯ�Ѵ�.
        int h = 40;
        int w = 100;
        Rect rectGUI = new Rect(vPosToScreen.x, vPosToScreen.y, w, h);
        //GUI.Box(rectGUI, "MoveBlock:" + isMoveBlock);
        GUI.Box(rectGUI, string.Format("HP:{1}\nMP:{0}", m_Enemy.m_nMp, m_Enemy.m_nHp));
    }
}
