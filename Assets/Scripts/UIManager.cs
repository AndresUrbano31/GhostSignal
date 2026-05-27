using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD — Vidas")]
    public Image[] heartImages;          // 3 corazones
    public Sprite heartFull;
    public Sprite heartEmpty;

    [Header("HUD — Puntaje")]
    public TextMeshProUGUI scoreText;    // "000900"

    [Header("HUD — Nodos")]
    public Image[] nodeCircles;          // 10 círculos
    public Slider nodeProgressBar;       // barra teal
    public TextMeshProUGUI nodesText;    // "NODOS 9/10"

    [Header("HUD — Pausa")]
    public GameObject pauseButtonObj;

    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject victoryPanel;

    [Header("Colores nodos")]
    public Color nodeActiveColor   = new Color(0f, 1f, 0.83f, 1f);   // #00FFD4
    public Color nodeInactiveColor = new Color(0f, 1f, 0.83f, 0.2f); // teal transparente

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        UpdateHearts();
        UpdateScore();
        UpdateNodes();
        UpdatePanels();
    }

    void UpdateHearts()
    {
        int lives = GameManager.Instance.lives;
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;
            
            // Si el índice es menor a las vidas, lo prende (true). Si no, lo apaga (false).
            heartImages[i].gameObject.SetActive(i < lives);
        }
    }

    void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = GameManager.Instance.score.ToString("D6");
    }

    void UpdateNodes()
    {
        int collected = GameManager.Instance.identityNodesCollected;
        int needed    = GameManager.Instance.NodesNeeded;

        if (nodesText != null)
            nodesText.text = $"NODOS {collected}/{needed}";

        if (nodeProgressBar != null)
        {
            nodeProgressBar.maxValue = needed;
            nodeProgressBar.value    = collected;
        }

        for (int i = 0; i < nodeCircles.Length; i++)
        {
            if (nodeCircles[i] == null) continue;
            nodeCircles[i].color = (i < collected) ? nodeActiveColor : nodeInactiveColor;
        }
    }

    void UpdatePanels()
    {
        if (pausePanel != null)
            pausePanel.SetActive(
                 GameManager.Instance.CurrentState == GameManager.GameState.Paused);
        if (victoryPanel != null)
            victoryPanel.SetActive(
                GameManager.Instance.CurrentState == GameManager.GameState.Victory);
    }

    public void OnRestartButton() => GameManager.Instance.RestartGame();
    public void OnNextButton()    => GameManager.Instance.LoadNextLevel();
    public void OnResumeButton()  => GameManager.Instance.ChangeState(GameManager.GameState.Playing);
    public void OnPauseButton()   => GameManager.Instance.ChangeState(GameManager.GameState.Paused);
}