using UnityEngine;
using UnityEngine.UI;

public class NexusHealth : MonoBehaviour
{
    public Transform target; // 적의 Transform을 여기에 할당
    public Camera mainCamera; // 메인 카메라를 여기에 할당
    public Slider healthSlider;
    public float maxHealth = 100f;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
    void Update()
    {
        if (target != null && mainCamera != null)
        {
            // 적의 위치를 메인 카메라의 forward 방향으로 회전
            //transform.position = target.position;
            transform.forward = mainCamera.transform.forward;
        }
    }
    void UpdateHealthBar()
    {
        // Update the slider value based on the current health
        healthSlider.value = currentHealth / maxHealth;
    }

    public void TakeDamage(float damage)
    {
        // Subtract damage from current health
        currentHealth -= damage;

        // Ensure health doesn't go below zero
        currentHealth = Mathf.Max(currentHealth, 0f);

        // Update the health bar
        UpdateHealthBar();

        // Check for Nexus destruction or other actions on low health
        if (currentHealth <= 0f)
        {
            // Nexus destroyed, perform necessary actions
            GameManager.GetInstance().EventEnd();
        }
    }

    void DestroyNexus()
    {
        // Perform actions when the Nexus is destroyed
        Debug.Log("Nexus Destroyed!");
        // You might want to play an animation, show a game over screen, etc.
    }
}