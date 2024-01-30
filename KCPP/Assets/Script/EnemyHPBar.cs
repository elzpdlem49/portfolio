using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    public Transform target; // 적의 Transform을 여기에 할당
    public Camera mainCamera; // 메인 카메라를 여기에 할당
    public Slider slider; // Unity UI Slider를 여기에 할당
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
            slider.value = cur / max; // 슬라이더 값을 0.0에서 1.0 사이로 설정
        }
    }
    void Update()
    {
        if (target != null && mainCamera != null)
        {
            // 적의 위치를 메인 카메라의 forward 방향으로 회전
            transform.position = target.position;
            transform.forward = mainCamera.transform.forward;
        }
    }
}
