using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    public enum EnemyVisualState
    {
        Walk,
        Attack,
        Dead
    }

    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float moveSpeed = 2.25f;
    [SerializeField] private float contactDamage = 10f;
    [SerializeField] private float contactDamageCooldown = 1f;
    [SerializeField] private float attackStateDuration = 0.2f;

    private PlayerHealth playerHealth;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float currentHealth;
    private float damageCooldownTimer;
    private float attackStateTimer;
    private bool isDead;
    private WaveSpawner ownerSpawner;

    public EnemyVisualState CurrentState
    {
        get
        {
            if (isDead)
            {
                return EnemyVisualState.Dead;
            }

            if (attackStateTimer > 0f)
            {
                return EnemyVisualState.Attack;
            }

            return EnemyVisualState.Walk;
        }
    }

    public void Initialize(PlayerHealth targetPlayer, WaveSpawner spawner)
    {
        playerHealth = targetPlayer;
        ownerSpawner = spawner;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        damageCooldownTimer -= Time.deltaTime;
        attackStateTimer -= Time.deltaTime;

        if (playerHealth == null || playerHealth.IsDead)
        {
            if (playerHealth == null)
            {
                playerHealth = FindFirstObjectByType<PlayerHealth>();
            }

            return;
        }

        Vector3 direction = playerHealth.transform.position - transform.position;
        if (Mathf.Abs(direction.x) > 0.05f && rb != null)
        {
            float horizontalDirection = Mathf.Sign(direction.x);
            rb.velocity = new Vector2(horizontalDirection * moveSpeed, rb.velocity.y);

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = direction.x < 0f;
            }
        }
        else if (rb != null)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0f)
        {
            return;
        }

        currentHealth -= damage;

        if (spriteRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashDamage());
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDamagePlayer(collision.collider);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamagePlayer(collision.collider);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (isDead || damageCooldownTimer > 0f)
        {
            return;
        }

        PlayerHealth health = other.GetComponent<PlayerHealth>();

        if (health == null)
        {
            health = other.GetComponentInParent<PlayerHealth>();
        }

        if (health == null)
        {
            return;
        }

        health.TakeDamage(contactDamage);
        damageCooldownTimer = contactDamageCooldown;
        attackStateTimer = attackStateDuration;
    }

    private void Die()
    {
        isDead = true;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        ownerSpawner?.NotifyEnemyKilled(this);
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator FlashDamage()
    {
        Color original = spriteRenderer.color;
        spriteRenderer.color = new Color(1f, 0.45f, 0.45f, 1f);
        yield return new WaitForSeconds(0.08f);

        if (!isDead)
        {
            spriteRenderer.color = original;
        }
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
}
