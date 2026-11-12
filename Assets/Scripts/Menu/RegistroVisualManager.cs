using UnityEngine;
using TMPro;

public class RegistroVisualManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputFieldIniciales;

    // Este método lo llamará el botón "ACEPTAR" al hacer clic
    public void ConfirmarRegistro()
    {
        // Validamos que el input field no esté vacío antes de leerlo
        if (inputFieldIniciales == null)
        {
            Debug.LogError("No has asignado el Input Field en el Inspector.");
            return;
        }

        string textoIngresado = inputFieldIniciales.text.Trim().ToUpper();

        // Si el jugador no escribió nada, ponemos AAA por defecto
        if (string.IsNullOrEmpty(textoIngresado))
        {
            textoIngresado = "AAA";
        }

        // Si escribió menos de 3 letras, rellenamos con 'X'
        while (textoIngresado.Length < 3)
        {
            textoIngresado += "X";
        }

        // Enviamos las iniciales al GameManager para que procese el ordenamiento y salve
        if (GameManager.instance != null)
        {
            GameManager.instance.GuardarNuevoRegistro(textoIngresado);
        }
        else
        {
            Debug.LogError("No se encontró el GameManager en la escena para guardar el registro.");
        }
    }
}