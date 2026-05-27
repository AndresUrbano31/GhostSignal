using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Boot, MainMenu, Playing, Paused, GameOver, Victory }
    public GameState CurrentState { get; private set; }

    [Header("Ghost Signal Stats")]
    public int identityNodesCollected = 0;
    public int lives = 3;
    public int currentLevel = 1;
    public int score = 0;

    [Header("Nodes needed per level")]
    public int[] nodesPerLevel = { 2, 4, 6, 8, 10 };

    public int NodesNeeded => nodesPerLevel[currentLevel - 1];

    private bool isTransitioning = false; // Seguro para evitar bucles de carga

    [Header("Audio (SFX)")]
    public AudioClip collectSound; // El archivo de audio del diamante
    public AudioClip damageSound;  // El archivo de audio del golpe
    private AudioSource sfxSource; // El reproductor de Unity

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Autodetectar el nivel actual para pruebas
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.Contains("Level1")) currentLevel = 1;
        else if (sceneName.Contains("Level2")) currentLevel = 2;
        else if (sceneName.Contains("Level3")) currentLevel = 3;
        else if (sceneName.Contains("Level4")) currentLevel = 4;
        else if (sceneName.Contains("Level5")) currentLevel = 5;

        // Inicializar el reproductor de audio automáticamente
        sfxSource = gameObject.AddComponent<AudioSource>();

        ChangeState(GameState.Boot);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.Boot:
                ChangeState(GameState.Playing);
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                Debug.Log("ARIA: Las memorias se han perdido.");
                break;
            case GameState.Victory:
                Time.timeScale = 0f;
                Debug.Log("KAI: Las memorias están a salvo.");
                break;
        }
    }

    void Update()
    {
        if (CurrentState == GameState.Playing && Input.GetKeyDown(KeyCode.Escape))
            ChangeState(GameState.Paused);
        else if (CurrentState == GameState.Paused && Input.GetKeyDown(KeyCode.Escape))
            ChangeState(GameState.Playing);
    }

    public void CollectNode()
    {
        // Si ya estamos cambiando de nivel, ignoramos colisiones extra
        if (isTransitioning) return;

        identityNodesCollected++;
        score += 100;

        // Reproducir el sonido del nodo al instante
        if (collectSound != null) sfxSource.PlayOneShot(collectSound);

        if (identityNodesCollected >= NodesNeeded)
        {
            isTransitioning = true; // Bloqueamos el sistema para la transición

            if (currentLevel >= 5)
            {
                ChangeState(GameState.Victory);
            }
            else
            {
                // Pausa de 0.2 segundos para no colapsar el motor de físicas
                Invoke("LoadNextLevel", 0.2f);
            }
        }
    }

    public void LoadNextLevel()
    {
        currentLevel++;
        identityNodesCollected = 0;
        lives = 3; // Restauramos las vidas al cambiar de nivel
        isTransitioning = false; // Quitamos el seguro para el nuevo nivel
        ChangeState(GameState.Playing);
        string nextScene = "Level" + currentLevel + "_" + GetLevelName(currentLevel);
        SceneManager.LoadScene(nextScene);
    }

    string GetLevelName(int level)
    {
        switch (level)
        {
            case 1: return "Amber";
            case 2: return "Circuit";
            case 3: return "Terminal";
            case 4: return "System";
            case 5: return "Core";
            default: return "Amber";
        }
    }

    public void PlayerDied()
    {
        // Reproducir el sonido de choque/daño
        if (damageSound != null) sfxSource.PlayOneShot(damageSound);

        lives--;
        if (lives <= 0)
            ChangeState(GameState.GameOver);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartGame()
    {
        identityNodesCollected = 0;
        lives = 3;
        currentLevel = 1;
        score = 0;
        isTransitioning = false;
        ChangeState(GameState.Playing);
        SceneManager.LoadScene("Level1_Amber");
    }

    // NUEVO: vuelve al menú principal y resetea todo el estado del juego
    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        identityNodesCollected = 0;
        lives = 3;
        currentLevel = 1;
        score = 0;
        isTransitioning = false;
        ChangeState(GameState.MainMenu);
        SceneManager.LoadScene("MainMenu");
    }
}