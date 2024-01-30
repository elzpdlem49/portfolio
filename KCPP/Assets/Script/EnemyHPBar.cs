using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    public Transform target; // ���� Transform�� ���⿡ �Ҵ�
    public Camera mainCamera; // ���� ī�޶� ���⿡ �Ҵ�
    public Slider slider; // Unity UI Slider�� ���⿡ �Ҵ�
    [SerializeField] RectTransform m_rectBackGround;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.GetInstance(); ;
    }
    public void SetSlider(float cur, float max)
    {
        if (slider != null)
        {
            slider.value = cur / max; // �����̴� ���� 0.0���� 1.0 ���̷� ����
        }
    }
    void Update()
    {
        if (target != null && mainCamera != null)
        {
            // ���� ��ġ�� ���� ī�޶��� forward �������� ȸ��
            transform.position = target.position;
            transform.forward = mainCamera.transform.forward;
        }
    }
}
