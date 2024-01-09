using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TextRPG;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    private TextRPG.ItemManager itemManager;
    public GameObject itemPanel; // UI 패널에 대한 참조

    private TextRPG.Item currentItem; // 현재 선택된 아이템

    // Start is called before the first frame update
    void Start()
    {
        itemManager = new TextRPG.ItemManager();
        itemManager.Init();

        // 패널이 활성 상태로 시작하도록 설정
        itemPanel.SetActive(true);

        // 인벤토리의 첫 번째 아이템을 현재 아이템으로 설정합니다.
        currentItem = itemManager.GetItem(TextRPG.ItemManager.E_ITEM.HPPOSTION_S);
    }

    // Update is called once per frame
    void Update()
    {
        // 'R' 키를 누를 때 아이템을 사용합니다.
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 현재 선택된 아이템이 있는지 확인합니다.
            if (currentItem != null)
            {
                // 아이템을 사용하여 플레이어의 체력을 증가시킵니다.
                PlayerMove.Instance.m_cPlayer.Consumable(currentItem.m_sStatus);
                Debug.Log($"{currentItem.m_strName} 사용 중. 플레이어 체력 증가: {currentItem.m_sStatus.nHP}");
                // 아이템의 사용 후 로직을 추가하세요.
            }
            else
            {
                Debug.Log("선택된 아이템이 없습니다");
            }
        }
    }
}