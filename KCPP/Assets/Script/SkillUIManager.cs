using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    public List<Image> skillImages;
    public List<Text> text;

    public static SkillUIManager instance;

    private void Awake()
    {
        instance = this;
    }
    public void UpdateSkillUI()
    {
        // Iterate through all skills and update their UI
        for (int i = 0; i < skillImages.Count; i++)
        {
            // ����: �� ��ų �̹����� �����ϴ� ��ٿ� �� �����ͼ� UI�� �ݿ�
            float cooldown = AnPlayer.Instance.skillCooldowns[i];
            float maxCooldown = GetMaxCooldownForSkill(i);

            skillImages[i].fillAmount = cooldown / maxCooldown;
        }
    }
    private float GetMaxCooldownForSkill(int skillIndex)
    {
        switch (skillIndex)
        {
            case 0:
                return AnPlayer.Instance.fireballCooldown;
            case 1:
                return AnPlayer.Instance.incinerationCooldown;
             case 2:
                 return AnPlayer.Instance.lavaShieldCooldown;
             case 3:
                 return AnPlayer.Instance.meteorCooldown;
            default:
                return 0f;
        }
    }
}