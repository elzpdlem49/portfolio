using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextRPG;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(this.gameObject.name + " OnCollisionEnter: " + collision.gameObject.name);
        if (collision.gameObject.tag == "Enemy")
        {
            Enemycontroller enemy = collision.gameObject.GetComponent<Enemycontroller>();

            if (enemy != null)
            {
                enemy.TakeDamage(PlayerMove.Instance.m_cPlayer.m_sStatus.nStr);

                if (enemy.Death())
                {
                    Destroy(collision.gameObject);
                    Player.GetExp(3);
                }
            }
        }
        Destroy(this.gameObject);

        if(collision.gameObject.tag == "Boss")
        {
            BossCont.Instance.m_Boss.m_nHp -= PlayerMove.Instance.m_cPlayer.m_sStatus.nStr - BossCont.Instance.m_Boss.m_sStatus.nDef;
            if (BossCont.Instance.m_Boss.Death())
            {
                Destroy(collision.gameObject);
                Player.GetExp(10);
            }
        }
        if (collision.gameObject.tag == "Annie")
        {
            Annie.Instance.m_Annie.m_nHp -= PlayerMove.Instance.m_cPlayer.m_sStatus.nStr;
            if (Annie.Instance.m_Annie.Death())
            {
                Destroy(collision.gameObject);
                Player.GetExp(5);
            }
        }
    }
}
