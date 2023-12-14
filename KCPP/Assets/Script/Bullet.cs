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
            Enemycontroller.Instance.m_Enemy.m_nHp -= PlayerMove.Instance.m_cPlayer.m_sStatus.nStr;
            if(Enemycontroller.Instance.m_Enemy.Death())
                Destroy(collision.gameObject);
        }
        Destroy(this.gameObject);

        if(collision.gameObject.tag == "Boss")
        {
            BossCont.Instance.m_Boss.m_nHp -= PlayerMove.Instance.m_cPlayer.m_sStatus.nStr - BossCont.Instance.m_Boss.m_sStatus.nDef;
            if(BossCont.Instance.m_Boss.Death())
                Destroy(collision.gameObject);
        }
    }
}
