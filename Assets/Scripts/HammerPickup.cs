using UnityEngine;

public class HammerPickup : MonoBehaviour
{
    public AudioClip sonidoRecogida;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificamos si el objeto que entró en el trigger tiene el tag "Player"
        if (other.CompareTag("Player"))
        {
            // Opcional: Aquí podrías reproducir un sonido de "recolectado" 
            // antes de destruir el objeto

            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddPoints(50);
            }

            if (sonidoRecogida != null)
            {
                AudioSource.PlayClipAtPoint(sonidoRecogida, transform.position);
            }

            Destroy(gameObject);
        }
    }
}