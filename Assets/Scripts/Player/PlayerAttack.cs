using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float attackCooldown = 0.35f;
    [SerializeField] private float projectileSpeed = 13f;
    [SerializeField] private float projectileLifetime = 1.4f;
    [SerializeField] private float projectileDamage = 25f;
    [SerializeField] private Vector2 spawnOffset = new Vector2(0.8f, 0.1f);
    [SerializeField] private Sprite projectileSprite;
    [SerializeField] private float projectileScale = 0.5f;

    private static Sprite fallbackProjectileSprite;
    private PlayerController playerController;
    private float cooldownTimer;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (ShouldAttack())
        {
            FireProjectile();
        }
    }

    private bool ShouldAttack()
    {
        bool keyboardAttack = Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.LeftControl);
        MobileInput mobileInput = playerController != null ? playerController.mobileInput : null;
        bool mobileAttack = mobileInput != null && mobileInput.ConsumeAttackPressed();

        if (!keyboardAttack && !mobileAttack)
        {
            return false;
        }

        return cooldownTimer <= 0f;
    }

    private void FireProjectile()
    {
        float facing = playerController != null ? playerController.FacingDirection : 1f;
        Vector3 spawnPosition = transform.position + new Vector3(spawnOffset.x * facing, spawnOffset.y, 0f);

        GameObject projectileObject = new GameObject("Projectile");
        projectileObject.transform.position = spawnPosition;

        SpriteRenderer renderer = projectileObject.AddComponent<SpriteRenderer>();
        renderer.sprite = projectileSprite != null ? projectileSprite : GetProjectileSprite();
        renderer.color = Color.white;
        renderer.sortingOrder = 2;
        projectileObject.transform.localScale = new Vector3(projectileScale, projectileScale, 1f);

        Rigidbody2D body = projectileObject.AddComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.isKinematic = true;

        CircleCollider2D collider = projectileObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        Projectile projectile = projectileObject.AddComponent<Projectile>();
        projectile.Initialize(new Vector2(facing, 0f), projectileDamage, projectileSpeed, projectileLifetime);

        cooldownTimer = attackCooldown;
    }

    private static Sprite GetProjectileSprite()
    {
        if (fallbackProjectileSprite != null)
        {
            return fallbackProjectileSprite;
        }

        Texture2D texture = Texture2D.whiteTexture;
        fallbackProjectileSprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f);

        return fallbackProjectileSprite;
    }
}
