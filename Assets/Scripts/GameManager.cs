using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int lives = 3;

    private int savedScore;
    private float savedTime;

    // CANDADO: Evita que el jugador reciba daño múltiple en el mismo fotograma/choque
    private bool isResetting = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // Si ya existe una instancia activa, destruimos esta copia DE INMEDIATO
            // para que no interfiera con las variables del original.
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Cada vez que inicia un nuevo nivel de forma limpia, guardamos el estado inicial
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // En cuanto la nueva escena aparece, liberamos el candado para poder recibir daño otra vez
        isResetting = false;

        // Evitamos guardar snapshot si estamos en el menú o en el GameOver
        if (scene.name != "GameOverMenu" && scene.name != "WinMenu")
        {
            SaveSnapshot();
        }
    }

    // Guardar snapshot al inicio de cada nivel
    public void SaveSnapshot()
    {
        if (ScoreManager.instance != null && TimerManager.instance != null)
        {
            savedScore = ScoreManager.instance.GetScore();
            savedTime = TimerManager.instance.GetTime();
        }
    }

    // Restaurar snapshot si muere en ese nivel
    public void RestoreSnapshot()
    {
        if (ScoreManager.instance != null && TimerManager.instance != null)
        {
            ScoreManager.instance.SetScore(savedScore);
            TimerManager.instance.SetTime(savedTime);
        }
    }

    public void PlayerDied()
    {
        // Si el candado está activo, ignoramos cualquier colisión extra repetida
        if (isResetting) return;

        // Activamos el candado inmediatamente
        isResetting = true;

        lives--;

        if (lives > 0)
        {
            RestoreSnapshot();
            // Recargamos la escena actual
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        if (TimerManager.instance != null)
        {
            TimerManager.instance.StopTimer();
        }
        SceneManager.LoadScene("GameOverMenu");
    }

    public void EndGame()
    {
        if (TimerManager.instance != null)
        {
            int bonus = Mathf.CeilToInt(TimerManager.instance.GetTime());
            ScoreManager.instance.AddPoints(bonus);
        }

        // Cambiamos la llamada vieja por la nueva pantalla de registro arcade
        TerminarJuegoYRegistrar();
    }

    public void TerminarJuegoYRegistrar()
    {
        if (TimerManager.instance != null)
        {
            TimerManager.instance.StopTimer();
        }

        // En lugar de ir directo a WinMenu, cargamos la escena de registro
        SceneManager.LoadScene("RegistroArcadeMenu");
    }

    // Este método lo llamaremos desde la pantalla de registro cuando el jugador presione "Aceptar"
    public void GuardarNuevoRegistro(string iniciales)
    {
        int puntosActuales = (ScoreManager.instance != null) ? ScoreManager.instance.GetScore() : 0;
        float tiempoActual = (TimerManager.instance != null) ? TimerManager.instance.GetTime() : 0f;

        // 1. Cargamos el Top 5 actual desde PlayerPrefs
        List<HighScoreElement> listaHighScores = CargarHighScores();

        // 2. Añadimos el nuevo registro del jugador (CORREGIDO con 'Add' en mayúscula)
        listaHighScores.Add(new HighScoreElement(iniciales, puntosActuales, tiempoActual));

        // 3. Ordenamos la lista
        listaHighScores.Sort();

        // 4. Si la lista supera los 5 registros, cortamos el sobrante
        if (listaHighScores.Count > 5)
        {
            listaHighScores.RemoveAt(5);
        }

        // 5. Guardamos el nuevo Top 5 de vuelta en PlayerPrefs
        for (int i = 0; i < 5; i++)
        {
            if (i < listaHighScores.Count)
            {
                PlayerPrefs.SetString("ArcadeInitials" + i, listaHighScores[i].initials);
                PlayerPrefs.SetInt("ArcadeScore" + i, listaHighScores[i].score);
                PlayerPrefs.SetFloat("ArcadeTime" + i, listaHighScores[i].time);
            }
            else
            {
                PlayerPrefs.SetString("ArcadeInitials" + i, "---");
                PlayerPrefs.SetInt("ArcadeScore" + i, 0);
                PlayerPrefs.SetFloat("ArcadeTime" + i, 0f);
            }
        }
        PlayerPrefs.Save();

        // Una vez guardado, mandamos al jugador a la tabla de posiciones final
        SceneManager.LoadScene("WinMenu");
    }

    // Método auxiliar para leer los datos actuales
    public List<HighScoreElement> CargarHighScores()
    {
        List<HighScoreElement> lista = new List<HighScoreElement>();
        for (int i = 0; i < 5; i++)
        {
            string init = PlayerPrefs.GetString("ArcadeInitials" + i, "---");
            int sc = PlayerPrefs.GetInt("ArcadeScore" + i, 0);
            float tm = PlayerPrefs.GetFloat("ArcadeTime" + i, 0f);

            // Solo lo agregamos si es un registro válido
            if (init != "---")
            {
                lista.Add(new HighScoreElement(init, sc, tm));
            }
        }
        return lista;
    }

    [System.Serializable]
    public class HighScoreElement : System.IComparable<HighScoreElement>
    {
        public string initials;
        public int score;
        public float time;

        public HighScoreElement(string initials, int score, float time)
        {
            this.initials = initials;
            this.score = score;
            this.time = time;
        }

        // Aquí ocurre la magia del ordenamiento Arcade:
        public int CompareTo(HighScoreElement other)
        {
            if (other == null) return 1;

            // 1. Primero comparamos por PUNTOS (Mayor puntaje va primero)
            if (this.score != other.score)
            {
                return other.score.CompareTo(this.score);
            }

            // 2. Si los puntos son IGUALES, desempatamos por TIEMPO (Mayor tiempo restante va primero)
            return other.time.CompareTo(this.time);
        }
    }
}