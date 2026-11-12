using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerDev : MonoBehaviour
{
    public static GameManagerDev instance;

    public int lives = 3;

    private int savedScore;
    private float savedTime;

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

    // Guardar snapshot al inicio de cada nivel
    public void SaveSnapshot()
    {
        savedScore = ScoreManager.instance.GetScore();
        savedTime = TimerManager.instance.GetTime();
    }

    // Restaurar snapshot si muere en ese nivel
    public void RestoreSnapshot()
    {
        ScoreManager.instance.SetScore(savedScore);
        TimerManager.instance.SetTime(savedTime);
    }

    public void PlayerDied()
    {
        lives--;

        if (lives > 0)
        {
            RestoreSnapshot();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        // Aquí cargas tu escena de Game Over
        SceneManager.LoadScene("GameOverMenu");
    }

    public void EndGame()
    {
        // Al terminar nivel 3, sumar tiempo sobrante como bonus
        int bonus = Mathf.CeilToInt(TimerManager.instance.GetTime());
        ScoreManager.instance.AddPoints(bonus);

        // Aquí guardas el puntaje final en ranking
        SaveHighScore(ScoreManager.instance.GetScore(), "AAA"); // luego reemplazas "AAA" por las iniciales del jugador

        SceneManager.LoadScene("WinMenu");
    }

    private void SaveHighScore(int score, string initials)
    {
        for (int i = 4; i > 0; i--)
        {
            PlayerPrefs.SetInt("Score" + i, PlayerPrefs.GetInt("Score" + (i - 1), 0));
            PlayerPrefs.SetString("Initials" + i, PlayerPrefs.GetString("Initials" + (i - 1), "---"));
        }
        PlayerPrefs.SetInt("Score0", score);
        PlayerPrefs.SetString("Initials0", initials);
        PlayerPrefs.Save();
    }
}
