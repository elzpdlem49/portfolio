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
            //Destroy(collision.gameObject);// ������°ſ��� ���ظ� �شٷ� �ٲ۴�
        }
        Destroy(this.gameObject);

    }
}
