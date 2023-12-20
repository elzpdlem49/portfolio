using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using TextRPG;
using Unity.VisualScripting;

public class BossCont : MonoBehaviour
{
    public static BossCont Instance;
    [SerializeField]
    public Player m_Boss;
    public Transform trfPlayer;
    public float moveSpeed = 10f;
    public float serchRange = 20f;
    public float attackRange = 3f;


    Rigidbody bossRb;
    public float attackCooldown = 5;
    public int m_BossDamage1 = 1;
    public int m_BossDamage2 = 2;
    public int m_BossDamage3 = 3;

    public bool isCooldown;
    public bool isTouch;

    bool isCharging; // 돌진 중인지 여부를 나타내는 플래그
    public float chargeDuration = 1.5f; // 돌진 지속 시간

    // Start is called before the first frame update
    void Start()
    {
        bossRb = GetComponent<Rigidbody>();
        m_Boss = new Player(name, 100, 100, 10, 9);
        Instance = this;
        //StartCoroutine(BossFight());
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
        GUI.Box(rectGUI, string.Format("HP:{1}\nMP:{0}", m_Boss.m_nMp, m_Boss.m_nHp));
    }
    // Update is called once per frame
    private void Update()
    {
        /*if (Vector3.Distance(transform.position, trfPlayer.position) < attackRange)
        {
            StartCoroutine(AttackPattern());
        }*/
    }
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, trfPlayer.position) < serchRange)
        {
            FollowPlayer();
        }
    }
    void FollowPlayer()
    {
        if (!isTouch)
        {
            Vector3 direction = (trfPlayer.position - transform.position).normalized;
            if (Vector3.Distance(transform.position, trfPlayer.position) < attackRange)
            {
                bossRb.velocity = Vector3.zero;
                // 바라보는 방향
                /* Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up); 
                 toRotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 5f);
                 enemyRigidbody.MoveRotation(toRotation);*/

                StartCoroutine(AttackPattern());
            }
            else if (Vector3.Distance(transform.position, trfPlayer.position) < serchRange)
            {
                bossRb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                toRotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * 5f);
            }
            else
            {
                // 돌진 시작
                StartCoroutine(ChargeAtPlayer(direction));
            }
        }
    }

    /*IEnumerator BossFight()
    {
        while (true)
        {
            yield return StartCoroutine(AttackPattern());
            yield return new WaitForSeconds(attackCooldown);
        }
    }*/

    /*IEnumerator AttackPattern()
    {
        isAttackCooldown = true;
        Debug.Log("Boss is attacking!");
        int random = Random.Range(0, 3);

        switch (random)
        {
            case 0:
                yield return StartCoroutine(AttackPattern1());
                break;
            case 1:
                yield return StartCoroutine(AttackPattern2());
                break;
            case 2:
                yield return StartCoroutine(AttackPattern3());
                break;
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttackCooldown = false;
    }
    IEnumerator AttackPattern1()
    {
        Debug.Log("Executing Attack Pattern 1");
        PlayerMove.Instance.m_cPlayer.m_nHp -= m_Boss.m_sStatus.nStr * m_BossDamage1;
        yield return new WaitForSeconds(attackCooldown); ;
    }

    IEnumerator AttackPattern2()
    {
        Debug.Log("Executing Attack Pattern 2");
        PlayerMove.Instance.m_cPlayer.m_nHp -= m_Boss.m_sStatus.nStr * m_BossDamage2;
        yield return new WaitForSeconds(attackCooldown); ;
    }

    IEnumerator AttackPattern3()
    {
        Debug.Log("Executing Attack Pattern 3");
        PlayerMove.Instance.m_cPlayer.m_nHp -= m_Boss.m_sStatus.nStr * m_BossDamage3;
        yield return new WaitForSeconds(attackCooldown); ;
    }*/

    IEnumerator AttackPattern()
    {
        if(isCooldown)
        {
            yield break;
        }
        isCooldown = true;
        int random = Random.Range(0, 3);

        switch (random)
        {
            case 0:
                AttackPattern1();
                break;
            case 1:
                AttackPattern2();
                break;
            /*case 2:
                yield return StartCoroutine(ChargeAttack());*/
                break;
        }
        /*if (PlayerMove.Instance.m_cPlayer.Death())
        {
            Destroy(PlayerMove.Instance.gameObject);
        }*/
        yield return new WaitForSeconds(attackCooldown);
        isCooldown=false;
    }
    void AttackPattern1()
    {
        Debug.Log("Executing Attack Pattern 1");
        PlayerMove.Instance.m_cPlayer.m_nHp -= m_Boss.m_sStatus.nStr * m_BossDamage1;
    }

    void AttackPattern2()
    {
        Debug.Log("Executing Attack Pattern 2");
        PlayerMove.Instance.m_cPlayer.m_nHp -= m_Boss.m_sStatus.nStr * m_BossDamage2;
    }
    IEnumerator ChargeAttack()
    {
        if (isCharging)
        {
            yield break; // Prevent multiple charges at the same time
        }

        Debug.Log("Charging at the player!");

        Vector3 chargeDirection = (trfPlayer.position - transform.position).normalized;

        // Perform the charge attack
        yield return StartCoroutine(ChargeAtPlayer(chargeDirection));
    }
    IEnumerator ChargeAtPlayer(Vector3 chargeDirection)
    {
        isCharging = true;

        // 이동 속도를 높여 돌진
        bossRb.velocity = chargeDirection * (moveSpeed * 2f);

        // 돌진 지속 시간만큼 대기
        yield return new WaitForSeconds(chargeDuration);

        // 돌진이 끝나면 플래그를 초기화
        isCharging = false;

        // 플레이어를 다시 추적하도록 이동 속도를 원래로 복구
        bossRb.velocity = Vector3.zero;
        bossRb.angularVelocity = Vector3.zero; // 각속도 초기화

        // 플레이어를 향해 바라보기
        Quaternion toRotation = Quaternion.LookRotation(chargeDirection, Vector3.up);
        bossRb.MoveRotation(toRotation);
    }
    /* private void OnCollisionEnter(Collision collision)
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
     }*/
}
