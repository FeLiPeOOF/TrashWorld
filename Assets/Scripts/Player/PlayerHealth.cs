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

    private bool isDead;

    public bool IsDead => isDead;

    private void Start()
    {
        currentHealth = maxHealth;
        EnsureDependencies();
        UpdateHealthBar();
    }

    private void Update()
    {
        if (enableDebugDamage && Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(debugDamageAmount);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        UpdateHealthBar();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void HealToFull()
    {
        isDead = false;
        currentHealth = maxHealth;
        UpdateHealthBar();
        gameObject.SetActive(true);
    }

    private void UpdateHealthBar()
    {
        EnsureDependencies();

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth / Mathf.Max(1f, maxHealth));
        }
    }

    private void EnsureDependencies()
    {
        if (healthBar == null)
        {
            healthBar = FindFirstObjectByType<HealthBar>();
        }

        if (gameOverMenu == null)
        {
            gameOverMenu = GameOverMenu.Instance;
        }
    }

    private void Die()
    {
        isDead = true;

        if (gameOverMenu != null)
        {
            gameOverMenu.ShowGameOver();
        }
        else
        {
            LevelFlowManager levelFlow = FindFirstObjectByType<LevelFlowManager>();
            if (levelFlow != null)
            {
                levelFlow.ShowGameOver();
            }
        }

        gameObject.SetActive(false);
    }
}
