using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public HealthBar healthBar;
    public GameOverMenu gameOverMenu;
    
    [Header("Debug")]
    public bool enableDebugDamage = true;
    public float debugDamageAmount = 10f;


    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(1f);
    }

    void Update()
    {
        if (enableDebugDamage && Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(debugDamageAmount);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        float normalized = currentHealth / maxHealth;
        healthBar.SetHealth(normalized);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Game Over!");
        gameOverMenu.ShowGameOver();
        gameObject.SetActive(false);
    }   
}
