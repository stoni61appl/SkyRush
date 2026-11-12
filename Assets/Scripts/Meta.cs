using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meta : MonoBehaviour
{
    // Este método de Unity detecta cuando algo entra en el Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si lo que tocó la meta fue el jugador
        // Asegúrate de que tu personaje tenga la etiqueta "Player" en Unity
        if (collision.CompareTag("Player"))
        {
            Debug.Log("¡El jugador llegó a la meta!");

            // Le avisamos al GameManager que termine el juego y cargue el registro arcade
            if (GameManager.instance != null)
            {
                GameManager.instance.EndGame();
            }
            else
            {
                Debug.LogError("No se encontró el GameManager para finalizar el juego.");
            }
        }
    }
}