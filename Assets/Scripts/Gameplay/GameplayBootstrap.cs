using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameplayBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitializeGameplaySystems()
    {
        Time.timeScale = 1f;

        Scene activeScene = SceneManager.GetActiveScene();

        if (activeScene.name == "MainMenu")
        {
            return;
        }

        PlayerController playerController = Object.FindFirstObjectByType<PlayerController>();

        if (playerController == null)
        {
            return;
        }

        PlayerHealth playerHealth = playerController.GetComponent<PlayerHealth>();

        if (playerController.GetComponent<PlayerAttack>() == null)
        {
            playerController.gameObject.AddComponent<PlayerAttack>();
        }

        LevelFlowManager levelFlowManager = Object.FindFirstObjectByType<LevelFlowManager>();
        if (levelFlowManager == null)
        {
            levelFlowManager = new GameObject("LevelFlowManager").AddComponent<LevelFlowManager>();
        }

        if (playerHealth != null && playerHealth.gameOverMenu == null)
        {
            GameOverMenu gameOverMenu = Object.FindFirstObjectByType<GameOverMenu>();

            if (gameOverMenu == null)
            {
                gameOverMenu = new GameObject("GameOverMenu").AddComponent<GameOverMenu>();
            }

            playerHealth.gameOverMenu = gameOverMenu;
        }

        if (Object.FindFirstObjectByType<WaveSpawner>() == null)
        {
            new GameObject("WaveSpawner").AddComponent<WaveSpawner>();
        }
    }
}
