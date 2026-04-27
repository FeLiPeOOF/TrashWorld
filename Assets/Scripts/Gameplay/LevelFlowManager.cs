using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelFlowManager : MonoBehaviour
{
    private const string MainMenuSceneName = "MainMenu";
    private const string FirstLevelSceneName = "Level01";

    private GameObject gameOverPanel;
    private GameObject levelSucceededPanel;
    private GameObject gameCompletedPanel;
    private Text waveLabel;
    private Canvas targetCanvas;

    private void Awake()
    {
        targetCanvas = FindFirstObjectByType<Canvas>();

        if (targetCanvas == null)
        {
            return;
        }

        EnsureOverlayPanels();
        EnsureWaveLabel();
    }

    public void UpdateWaveLabel(int currentWave, int totalWaves)
    {
        if (waveLabel == null)
        {
            EnsureWaveLabel();
        }

        if (waveLabel != null)
        {
            waveLabel.text = "Wave " + currentWave + "/" + totalWaves;
        }
    }

    public void ShowGameOver()
    {
        ShowPanel(gameOverPanel);
    }

    public void HandleLevelCleared()
    {
        if (TryGetNextGameplayScene(out _))
        {
            ShowPanel(levelSucceededPanel);
        }
        else
        {
            ShowPanel(gameCompletedPanel);
        }
    }

    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenuSceneName);
    }

    public void LoadNextLevel()
    {
        if (!TryGetNextGameplayScene(out string nextSceneName))
        {
            GoToMainMenu();
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }

    private void ShowPanel(GameObject panelToShow)
    {
        if (panelToShow == null)
        {
            return;
        }

        gameOverPanel?.SetActive(panelToShow == gameOverPanel);
        levelSucceededPanel?.SetActive(panelToShow == levelSucceededPanel);
        gameCompletedPanel?.SetActive(panelToShow == gameCompletedPanel);
        Time.timeScale = 0f;
    }

    private void EnsureOverlayPanels()
    {
        gameOverPanel = CreateOverlayPanel(
            "Game Over",
            "Game Over",
            new[]
            {
                new PanelButtonData("Retry", RetryLevel),
                new PanelButtonData("Main Menu", GoToMainMenu)
            });

        levelSucceededPanel = CreateOverlayPanel(
            "Level Succeeded",
            "Level Succeeded!",
            new[]
            {
                new PanelButtonData("Next Level", LoadNextLevel),
                new PanelButtonData("Main Menu", GoToMainMenu)
            });

        gameCompletedPanel = CreateOverlayPanel(
            "Game Completed",
            "Game Completed!",
            new[]
            {
                new PanelButtonData("Replay", ReplayGame),
                new PanelButtonData("Main Menu", GoToMainMenu)
            });
    }

    public void ReplayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(FirstLevelSceneName);
    }

    private void EnsureWaveLabel()
    {
        GameObject labelObject = CreateUIObject("WaveLabel", targetCanvas.transform);
        RectTransform rect = labelObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -110f);
        rect.sizeDelta = new Vector2(320f, 40f);

        waveLabel = labelObject.AddComponent<Text>();
        waveLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        waveLabel.fontSize = 28;
        waveLabel.alignment = TextAnchor.MiddleCenter;
        waveLabel.color = Color.white;
        waveLabel.text = "Wave 1/1";
    }

    private GameObject CreateOverlayPanel(string objectName, string title, IReadOnlyList<PanelButtonData> buttons)
    {
        GameObject panel = CreateUIObject(objectName, targetCanvas.transform);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.08f, 0.12f, 0.16f, 0.82f);

        CreateLabel(title, panel.transform, new Vector2(0f, 140f), 46);

        float startY = 20f;
        for (int i = 0; i < buttons.Count; i++)
        {
            CreateButton(buttons[i], panel.transform, new Vector2(0f, startY - (i * 110f)));
        }

        panel.SetActive(false);
        return panel;
    }

    private void CreateLabel(string textValue, Transform parent, Vector2 anchoredPosition, int fontSize)
    {
        GameObject labelObject = CreateUIObject(textValue + " Label", parent);
        RectTransform rect = labelObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(500f, 80f);

        Text label = labelObject.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = fontSize;
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.text = textValue;
    }

    private void CreateButton(PanelButtonData buttonData, Transform parent, Vector2 anchoredPosition)
    {
        GameObject buttonObject = CreateUIObject(buttonData.Text + " Button", parent);
        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(260f, 78f);

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.95f, 0.75f, 0.28f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(buttonData.Action);

        GameObject textObject = CreateUIObject(buttonData.Text + " Text", buttonObject.transform);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text buttonText = textObject.AddComponent<Text>();
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 28;
        buttonText.fontStyle = FontStyle.Bold;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = new Color(0.12f, 0.12f, 0.12f, 1f);
        buttonText.text = buttonData.Text;
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject gameObject = new GameObject(name);
        gameObject.transform.SetParent(parent, false);
        gameObject.layer = 5;
        return gameObject;
    }

    private bool TryGetNextGameplayScene(out string nextSceneName)
    {
        Scene currentScene = SceneManager.GetActiveScene();

        for (int i = currentScene.buildIndex + 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneName == MainMenuSceneName)
            {
                continue;
            }

            if (!sceneName.StartsWith("Level"))
            {
                continue;
            }

            nextSceneName = sceneName;
            return true;
        }

        nextSceneName = string.Empty;
        return false;
    }

    private readonly struct PanelButtonData
    {
        public PanelButtonData(string text, UnityEngine.Events.UnityAction action)
        {
            Text = text;
            Action = action;
        }

        public string Text { get; }
        public UnityEngine.Events.UnityAction Action { get; }
    }
}
