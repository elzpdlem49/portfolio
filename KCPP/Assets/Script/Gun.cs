using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float shotPower = 500f;
    public GameObject prefabBullet;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shot();
        }
    }
    public void Shot()
    {
        GameObject Bullet = Instantiate(prefabBullet);
        Bullet.transform.position = this.gameObject.transform.position;
        Bullet.GetComponent<Rigidbody>().AddForce(transform.forward * shotPower);
    }
}
