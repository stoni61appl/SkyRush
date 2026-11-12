using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Necesario para detectar el cambio de niveles

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    private int score = 0;

    [SerializeField] private TMP_Text scoreText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Nos suscribimos al evento de carga de escenas de Unity
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Nos desuscribimos para evitar fugas de memoria
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Se ejecuta automáticamente cada vez que se carga un nuevo nivel
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReconectarUI();
    }

    public void AddPoints(int points)
    {
        score += points;
        UpdateUI();
    }

    public int GetScore() => score;
    public void SetScore(int value)
    {
        score = value;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    // Busca el nuevo componente de texto en la escena actual
    private void ReconectarUI()
    {
        // OBTENEMOS EL NOMBRE DE LA ESCENA ACTUAL
        string escenaActual = SceneManager.GetActiveScene().name;

        // CORRECCIÓN: Si estamos en menús, soltamos el texto para no alterarlo por error
        if (escenaActual == "MainMenu" || escenaActual == "GameOverMenu" || escenaActual == "WinMenu")
        {
            scoreText = null;
            return;
        }

        // Opción 1: Busca un objeto con la etiqueta "ScoreText"
        GameObject textoObj = GameObject.FindWithTag("ScoreText");

        if (textoObj != null)
        {
            scoreText = textoObj.GetComponent<TMP_Text>();
        }
        else
        {
            // Opción 2: Si no encuentra el Tag, busca el componente de forma genérica
            scoreText = Object.FindFirstObjectByType<TMP_Text>();
        }

        // Refrescamos el texto de la interfaz con el puntaje que ya llevábamos acumulado
        UpdateUI();
    }

    // ==========================================
    // NUEVO: RESETEAR PUNTOS AL VOLVER A JUGAR
    // ==========================================
    public void ResetearScore()
    {
        score = 0;
        scoreText = null;
    }
}