using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoPlataforma : MonoBehaviour
{
    [SerializeField] private float velocidad;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private float distancia;
    [SerializeField] private bool moviendoDerecha;

    // 1. Agregamos la máscara de capa para filtrar
    [SerializeField] private LayerMask queEsSuelo;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // 2. Le pasamos 'queEsSuelo' al final del Raycast. 
        // Ahora el rayo atravesará jugadores, paredes invisibles y enemigos, buscando solo el piso.
        RaycastHit2D informacionSuelo = Physics2D.Raycast(controladorSuelo.position, Vector2.down, distancia, queEsSuelo);

        rb.velocity = new Vector2(velocidad, rb.velocity.y);

        // Si el rayo NO encuentra suelo real, da la vuelta
        if (informacionSuelo.collider == null)
        {
            Girar();
        }
    }

    private void Girar()
    {
        moviendoDerecha = !moviendoDerecha;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
        velocidad *= -1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(controladorSuelo.transform.position, controladorSuelo.transform.position + Vector3.down * distancia);
    }

    // ==========================================
    // NUEVA LÓGICA: DETECCIÓN DE DAÑO AL JUGADOR
    // ==========================================

    // Caso 1: Si el enemigo tiene un Collider físico sólido (el jugador choca y rebota)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HacerDanoAlJugador();
        }
    }

    // Caso 2: Si el enemigo tiene activada la casilla "Is Trigger" (el jugador lo atraviesa)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HacerDanoAlJugador();
        }
    }

    private void HacerDanoAlJugador()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.PlayerDied();
        }
        else
        {
            Debug.LogWarning("¡Ojo! No se encontró el GameManager en la escena.");
        }
    }
}