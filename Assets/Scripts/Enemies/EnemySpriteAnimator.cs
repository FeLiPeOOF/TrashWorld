using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemySpriteAnimator : MonoBehaviour
{
    [Header("Frames")]
    [SerializeField] private Sprite[] walkFrames;
    [SerializeField] private Sprite[] attackFrames;
    [SerializeField] private Sprite[] deathFrames;

    [Header("Timing")]
    [SerializeField] private float walkFrameRate = 0.16f;
    [SerializeField] private float attackFrameRate = 0.12f;
    [SerializeField] private float deathFrameRate = 0.18f;

    private EnemyController enemyController;
    private SpriteRenderer spriteRenderer;
    private float frameTimer;
    private int frameIndex;
    private EnemyController.EnemyVisualState currentState;

    private void OnValidate()
    {
        LoadDefaultFrames();
    }

    private void Awake()
    {
        LoadDefaultFrames();
        enemyController = GetComponent<EnemyController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LoadDefaultFrames()
    {
#if UNITY_EDITOR
        if (walkFrames == null || walkFrames.Length == 0)
        {
            walkFrames = new[]
            {
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Enemies/Lixoso/Walk/lixosoWalk_01.png"),
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Enemies/Lixoso/Walk/lixosoWalk_02.png")
            };
        }

        if (attackFrames == null || attackFrames.Length == 0)
        {
            attackFrames = new[]
            {
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Enemies/Lixoso/Attack/lixosoAttack_01.png")
            };
        }

        if (deathFrames == null || deathFrames.Length == 0)
        {
            deathFrames = new[]
            {
                AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Enemies/Lixoso/Death/lixosoDeath_01.png")
            };
        }
#endif
    }

    private void Update()
    {
        if (enemyController == null || spriteRenderer == null)
        {
            return;
        }

        EnemyController.EnemyVisualState nextState = enemyController.CurrentState;

        if (nextState != currentState)
        {
            currentState = nextState;
            frameIndex = 0;
            frameTimer = 0f;
            ApplyCurrentFrame();
            return;
        }

        Sprite[] frames = GetFramesForState(currentState);

        if (frames == null || frames.Length <= 1)
        {
            ApplyCurrentFrame();
            return;
        }

        frameTimer += Time.deltaTime;

        if (frameTimer >= GetFrameRateForState(currentState))
        {
            frameTimer = 0f;
            frameIndex = (frameIndex + 1) % frames.Length;
            ApplyCurrentFrame();
        }
    }

    private void ApplyCurrentFrame()
    {
        Sprite[] frames = GetFramesForState(currentState);

        if (frames == null || frames.Length == 0)
        {
            return;
        }

        if (frameIndex >= frames.Length)
        {
            frameIndex = frames.Length - 1;
        }

        if (frames[frameIndex] != null)
        {
            spriteRenderer.sprite = frames[frameIndex];
        }
    }

    private Sprite[] GetFramesForState(EnemyController.EnemyVisualState state)
    {
        switch (state)
        {
            case EnemyController.EnemyVisualState.Attack:
                return attackFrames;
            case EnemyController.EnemyVisualState.Dead:
                return deathFrames;
            default:
                return walkFrames;
        }
    }

    private float GetFrameRateForState(EnemyController.EnemyVisualState state)
    {
        switch (state)
        {
            case EnemyController.EnemyVisualState.Attack:
                return attackFrameRate;
            case EnemyController.EnemyVisualState.Dead:
                return deathFrameRate;
            default:
                return walkFrameRate;
        }
    }
}
