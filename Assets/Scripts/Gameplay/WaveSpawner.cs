using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class WaveDefinition
    {
        public int enemyCount = 3;
        public float spawnInterval = 0.35f;
    }

    [SerializeField] private List<WaveDefinition> waves = new List<WaveDefinition>
    {
        new WaveDefinition { enemyCount = 3, spawnInterval = 0.5f },
        new WaveDefinition { enemyCount = 5, spawnInterval = 0.45f },
        new WaveDefinition { enemyCount = 7, spawnInterval = 0.35f }
    };
    [SerializeField] private float timeBetweenWaves = 1.5f;
    [SerializeField] private float spawnHeight = -1.7f;
    [SerializeField] private Sprite enemySprite;
    [SerializeField] private float enemyScale = 3.5f;

    private static Sprite fallbackEnemySprite;
    private readonly List<EnemyController> aliveEnemies = new List<EnemyController>();
    private PlayerHealth playerHealth;
    private LevelFlowManager levelFlowManager;
    private BoxCollider2D groundCollider;
    private int currentWaveIndex = -1;
    private bool finishedSpawning;

    public int TotalWaves => waves.Count;
    public int CurrentWaveNumber => Mathf.Clamp(currentWaveIndex + 1, 0, TotalWaves);

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (enemySprite == null)
        {
            enemySprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Enemies/Lixoso/Walk/lixosoWalk_01.png");
        }
#endif
    }

    private void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        levelFlowManager = FindFirstObjectByType<LevelFlowManager>();

        GameObject groundObject = GameObject.FindGameObjectWithTag("Ground");
        if (groundObject != null)
        {
            groundCollider = groundObject.GetComponent<BoxCollider2D>();
        }

        StartCoroutine(SpawnWavesRoutine());
    }

    public void NotifyEnemyKilled(EnemyController enemy)
    {
        aliveEnemies.Remove(enemy);

        if (finishedSpawning && aliveEnemies.Count == 0)
        {
            levelFlowManager?.HandleLevelCleared();
        }
    }

    private IEnumerator SpawnWavesRoutine()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < waves.Count; i++)
        {
            currentWaveIndex = i;
            levelFlowManager?.UpdateWaveLabel(CurrentWaveNumber, TotalWaves);

            WaveDefinition wave = waves[i];

            for (int enemyIndex = 0; enemyIndex < wave.enemyCount; enemyIndex++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(wave.spawnInterval);
            }

            while (aliveEnemies.Count > 0)
            {
                yield return null;
            }

            if (i < waves.Count - 1)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        finishedSpawning = true;
        levelFlowManager?.HandleLevelCleared();
    }

    private void SpawnEnemy()
    {
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        GameObject enemyObject = new GameObject("Enemy");
        enemyObject.transform.position = GetSpawnPosition();
        enemyObject.transform.localScale = new Vector3(enemyScale, enemyScale, 1f);

        SpriteRenderer renderer = enemyObject.AddComponent<SpriteRenderer>();
        renderer.sprite = enemySprite != null ? enemySprite : GetEnemySprite();
        renderer.color = Color.white;
        renderer.sortingOrder = 1;

        BoxCollider2D collider = enemyObject.AddComponent<BoxCollider2D>();
        Vector2 spriteSize = renderer.sprite != null ? renderer.sprite.bounds.size : new Vector2(1f, 1f);
        collider.size = 0.5f * spriteSize;
        collider.offset = renderer.sprite != null ? renderer.sprite.bounds.center : Vector2.zero;

        enemyObject.transform.position = GetGroundedSpawnPosition(enemyObject.transform.position.x, collider);

        Rigidbody2D rb = enemyObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.freezeRotation = true;

        EnemyController enemy = enemyObject.AddComponent<EnemyController>();
        enemyObject.AddComponent<EnemySpriteAnimator>();
        enemy.Initialize(playerHealth, this);
        aliveEnemies.Add(enemy);
    }

    private Vector3 GetSpawnPosition()
    {
        if (groundCollider != null)
        {
            Bounds bounds = groundCollider.bounds;
            float padding = 1f;
            float minX = bounds.min.x + padding;
            float maxX = bounds.max.x - padding;
            bool spawnOnLeft = playerHealth == null || Random.value > 0.5f;

            if (playerHealth != null)
            {
                spawnOnLeft = playerHealth.transform.position.x > (minX + maxX) * 0.5f;
            }

            float edgeOffset = 1.5f;
            float centerX = (minX + maxX) * 0.5f;
            float randomX = spawnOnLeft
                ? Random.Range(minX, Mathf.Max(minX + edgeOffset, centerX - 1f))
                : Random.Range(Mathf.Min(centerX + 1f, maxX - edgeOffset), maxX);
            return new Vector3(randomX, spawnHeight, 0f);
        }

        float fallbackX = Random.Range(-7f, 7f);
        return new Vector3(fallbackX, spawnHeight, 0f);
    }

    private Vector3 GetGroundedSpawnPosition(float xPosition, BoxCollider2D collider)
    {
        if (groundCollider == null)
        {
            return new Vector3(xPosition, spawnHeight, 0f);
        }

        float colliderHeight = collider.size.y * enemyScale;
        float groundTop = groundCollider.bounds.max.y;
        float yPosition = groundTop + (colliderHeight * 0.5f) - (collider.offset.y * enemyScale);
        return new Vector3(xPosition, yPosition, 0f);
    }

    private static Sprite GetEnemySprite()
    {
        if (fallbackEnemySprite != null)
        {
            return fallbackEnemySprite;
        }

        Texture2D texture = Texture2D.whiteTexture;
        fallbackEnemySprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f);

        return fallbackEnemySprite;
    }
}
