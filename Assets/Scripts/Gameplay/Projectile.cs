using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float damage = 25f;

    private Vector2 direction = Vector2.right;

    public void Initialize(Vector2 travelDirection, float projectileDamage, float projectileSpeed, float projectileLifetime)
    {
        direction = travelDirection.normalized;
        damage = projectileDamage;
        speed = projectileSpeed;
        lifetime = projectileLifetime;
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        lifetime -= Time.deltaTime;

        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable == null)
        {
            damageable = other.GetComponentInParent<IDamageable>();
        }

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
