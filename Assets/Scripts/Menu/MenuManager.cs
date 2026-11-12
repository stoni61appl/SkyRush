using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Esta es la función que usa el botón de la pantalla de Game Over
    public void VolverAlMenu()
    {
        // 1. Le devolvemos las 3 vidas al GameManager para la próxima partida
        if (GameManager.instance != null)
        {
            GameManager.instance.lives = 3;
        }

        // 2. Reseteamos el reloj al tiempo inicial (los 120 segundos)
        if (TimerManager.instance != null)
        {
            TimerManager.instance.ResetearTimer();
        }

        // 3. Reseteamos el puntaje a 0
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetearScore();
        }

        // 4. Ahora sí, volvemos limpios al menú principal
        SceneManager.LoadScene("MainMenu");
    }

    // Tu otra función para iniciar directamente si tienes botón de reintentar
    public void IniciarPartidaLimpia()
    {
        if (GameManager.instance != null) GameManager.instance.lives = 3;
        if (TimerManager.instance != null) TimerManager.instance.ResetearTimer();
        if (ScoreManager.instance != null) ScoreManager.instance.ResetearScore();

        SceneManager.LoadScene("Nivel1");
    }
}