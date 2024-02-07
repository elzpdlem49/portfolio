using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    float cooltime = 10f;
    float cooltime_max = 10f;
    public Image disable;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(CoolTimeFunc());
        }
    }
    IEnumerator CoolTimeFunc()
    {
        while(cooltime > 0.0f)
        {
            cooltime -= Time.deltaTime;

            disable.fillAmount = cooltime/cooltime_max;

            yield return new WaitForFixedUpdate();
        }
    }
}