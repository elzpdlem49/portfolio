using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    [SerializeField] RectTransform m_rectBar;
    [SerializeField] RectTransform m_rectBackGround;

    public void SetBar(float cur, float max)
    {
        Vector2 vRectSize = m_rectBar.sizeDelta;
        vRectSize.x = m_rectBackGround.sizeDelta.x * (cur / max);//230 * 50/100// 115 * 0.5 =57.5
        m_rectBar.sizeDelta = vRectSize;
    }

    // Start is called before the first frame update
    void Start()
    {
        //SetBar(50, 100);
    }

    // Update is called once per frame
    void Update()
    {
        //SetBar(50, 100);
    }
}
