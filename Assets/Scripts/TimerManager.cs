using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Necesario para detectar el cambio de escenas

public class TimerManager : MonoBehaviour
{
    public static TimerManager instance;

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private int startTime = 120;

    private float currentTime;
    private bool isRunning = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Inicializamos el tiempo aquí por primera vez para que no se resetee en el Start
            currentTime = startTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Nos suscribimos al evento de Unity que avisa cuando una escena se ha cargado
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Nos desuscribimos al destruir el objeto para evitar fugas de memoria
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Este método se ejecuta AUTOMÁTICAMENTE cada vez que cambias de nivel
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Verificamos en qué escena estamos parado justo ahora
        string escenaActual = scene.name;

        // Si entramos a cualquiera de los menús, apagamos el timer por completo
        if (escenaActual == "MainMenu" || escenaActual == "GameOverMenu" || escenaActual == "WinMenu" || escenaActual == "RegistroArcadeMenu")
        {
            isRunning = false;
        }
        else
        {
            // Si es un nivel de juego normal, entonces sí que siga corriendo
            isRunning = true;
        }
    }

    private void Start()
    {
        // Si es el primer nivel, arranca el contador
        isRunning = true;
    }

    private void Update()
    {
        if (isRunning)
        {
            // ¡NUEVO: Si el juego está corriendo pero perdimos el texto (por un reinicio), lo buscamos otra vez!
            if (timerText == null)
            {
                ReconectarUI();
            }

            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                isRunning = false;
                if (GameManager.instance != null)
                {
                    GameManager.instance.GameOver(); // Tiempo agotado
                }
            }

            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (timerText != null)
        {
            timerText.text = "Tiempo: " + Mathf.CeilToInt(currentTime);
        }
    }

    // Método clave para encontrar el nuevo Canvas en la escena cargada
    private void ReconectarUI()
    {
        // Obtenemos el nombre de la escena actual
        string escenaActual = SceneManager.GetActiveScene().name;

        // Si estamos en menús, soltamos el texto para no alterarlos por error
        if (escenaActual == "MainMenu" || escenaActual == "GameOverMenu" || escenaActual == "WinMenu" || escenaActual == "RegistroArcadeMenu")
        {
            timerText = null;
            return;
        }

        // Opción 1: Busca un objeto en la nueva escena que tenga la etiqueta "TimerText"
        GameObject textoObj = GameObject.FindWithTag("TimerText");

        if (textoObj != null)
        {
            timerText = textoObj.GetComponent<TMP_Text>();

            // Forzamos el texto con el valor actual de golpe para evitar el vacío "TIME:"
            timerText.text = "Tiempo: " + Mathf.CeilToInt(currentTime);
        }
        else
        {
            // Opción 2: Si no encuentra el Tag, intenta buscar cualquier TMP_Text en la escena
            timerText = Object.FindFirstObjectByType<TMP_Text>();

            if (timerText != null)
            {
                timerText.text = "Tiempo: " + Mathf.CeilToInt(currentTime);
            }
        }

        UpdateUI();
    }

    public float GetTime() => currentTime;

    public void SetTime(float value)
    {
        currentTime = value;
        UpdateUI();
    }

    public void StopTimer() => isRunning = false;
    public void StartTimer() => isRunning = true;

    public void ResetearTimer()
    {
        currentTime = startTime;
        isRunning = false;
    }
}