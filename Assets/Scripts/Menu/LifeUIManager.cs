using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LifeUIManager : MonoBehaviour
{
    [Header("Opción 1: Texto")]
    [SerializeField] private TMP_Text livesText; // Arrastra aquí tu componente de texto

    [Header("Opción 2: Iconos (Imágenes)")]
    [SerializeField] private Image[] heartImages; // Un arreglo con tus imágenes de corazones en el Canvas
    [SerializeField] private Sprite heartFull;    // Sprite del corazón activo/lleno
    [SerializeField] private Sprite heartEmpty;   // Sprite del corazón vacío/apagado (Opcional)

    private void OnEnable()
    {
        // Nos suscribimos a la carga de escenas para redibujar las vidas al reiniciar el nivel
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        ActualizarInterfazVidas();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Forzamos la actualización visual en cuanto carga la escena
        ActualizarInterfazVidas();
    }

    public void ActualizarInterfazVidas()
    {
        // Validamos que el GameManager exista
        if (GameManager.instance == null) return;

        int vidasActuales = GameManager.instance.lives;

        // 1. Actualización por TEXTO (Si está asignado)
        if (livesText != null)
        {
            livesText.text = "X " + vidasActuales; // O "Vidas: " + vidasActuales
        }

        // 2. Actualización por ICONOS (Si tienes imágenes asignadas)
        if (heartImages != null && heartImages.Length > 0)
        {
            for (int i = 0; i < heartImages.Length; i++)
            {
                if (i < vidasActuales)
                {
                    // Si el índice es menor a las vidas actuales, el corazón se muestra lleno
                    heartImages[i].sprite = heartFull;
                    heartImages[i].enabled = true; // Asegura que sea visible
                }
                else
                {
                    // Si el jugador ya perdió esta vida:
                    if (heartEmpty != null)
                    {
                        heartImages[i].sprite = heartEmpty; // Cambia a corazón vacío
                    }
                    else
                    {
                        heartImages[i].enabled = false; // Si no tienes sprite vacío, simplemente oculta el icono
                    }
                }
            }
        }
    }
}