using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TextRPG;
using TMPro;

public class GUIManager : MonoBehaviour
{
    public enum InfoType { Level, StunStack};
    public InfoType type;

    TextMeshProUGUI m_Text;
    // Start is called before the first frame update
    private void Awake()
    {
        m_Text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(type)
        {
            case InfoType.Level:
                m_Text.text = string.Format("Lv.{0:F0}", PlayerMove.Instance.m_cPlayer.m_nLevel);
                break;
            case InfoType.StunStack:
                m_Text.text = string.Format("{0}", Annie.Instance.stunStack);
                break;
        }
        
    }
}
