using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public HealthBar healthBar;
    public GameOverMenu gameOverMenu;

    [Header("Damage Animation")]
    public float blinkDuration = 0.15f;
    public int blinkCount = 3;
    public Color damageColor = Color.red;

    [Header("Debug")]
    public bool enableDebugDamage = true;
    public float debugDamageAmount = 10f;

    private bool isDead;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine currentBlinkCoroutine;

    public bool IsDead => isDead;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
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
        TriggerDamageAnimation();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void TriggerDamageAnimation()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (currentBlinkCoroutine != null)
        {
            StopCoroutine(currentBlinkCoroutine);
        }
        currentBlinkCoroutine = StartCoroutine(BlinkRed());
    }

    private IEnumerator BlinkRed()
    {
        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(blinkDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(blinkDuration);
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
