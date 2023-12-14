using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public GameObject m_prefabObject;
    public GameObject m_objInstance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (m_objInstance == null)
        {
            Debug.Log("RE");
            m_objInstance = Instantiate(m_prefabObject, this.transform.position, Quaternion.identity);
        }
    }
}
