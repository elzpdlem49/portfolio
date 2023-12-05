using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(this.gameObject.name + " OnCollisionEnter: " + collision.gameObject.name);

        if (collision.gameObject.tag == "Enemy")
        {
            //Destroy(collision.gameObject);// 사라지는거에서 피해를 준다로 바꾼다
        }
        Destroy(this.gameObject);

    }
}
