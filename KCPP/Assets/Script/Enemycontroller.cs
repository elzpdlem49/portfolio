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
        m_Enemy = new Player(name, 100, 100, 10, 0);
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
        GUI.Box(rectGUI, string.Format("HP:{1}\nMP:{0}", m_Enemy.m_nMp, m_Enemy.m_nHp));
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
            PlayerMove.Instance.m_cPlayer.m_nHp -= m_Enemy.m_sStatus.nStr; //애니메이션
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
}
