using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextRPG;
using UnityEngine.UI;
using System.Globalization;

public class ItemManager : MonoBehaviour
{
    private TextRPG.ItemManager itemManager;
    public GameObject itemPanel; // UI �гο� ���� ����

    private TextRPG.Item currentItem; // ���� ���õ� ������

    // Start is called before the first frame update
    void Start()
    {
        itemManager = new TextRPG.ItemManager();
        itemManager.Init();

        // �г��� Ȱ�� ���·� �����ϵ��� ����
        itemPanel.SetActive(true);

        // �κ��丮�� ù ��° �������� ���� ���������� �����մϴ�.
        currentItem = itemManager.GetItem(TextRPG.ItemManager.E_ITEM.HPPOSTION_S);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // ���� ���õ� �������� �ִ��� Ȯ���մϴ�.
            if (currentItem != null)
            {
                // �������� ����Ͽ� �÷��̾��� ü���� ������ŵ�ϴ�.
                PlayerMove.Instance.m_cPlayer.Consumable(currentItem.m_sStatus);
                Debug.Log($"{currentItem.m_strName} ��� ��. �÷��̾� ü�� ����: {currentItem.m_sStatus.nHP}");
                // �������� ��� �� ������ �߰��ϼ���.
            }
            else
            {
                Debug.Log("���õ� �������� �����ϴ�");
            }
        }
    }
}