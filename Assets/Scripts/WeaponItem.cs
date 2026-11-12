using UnityEngine;

public class WeaponItem : MonoBehaviour
{
    [Header("Sonido")]
    public AudioClip sonidoRecogida; // Arrastra aquí tu audio en el Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player jugador = other.GetComponent<Player>();

            if (jugador != null)
            {
                // Reproduce el sonido en la posición actual del ítem antes de borrarlo
                if (sonidoRecogida != null)
                {
                    AudioSource.PlayClipAtPoint(sonidoRecogida, transform.position);
                }

                jugador.ActivarArma();
                Destroy(gameObject);
            }
        }
    }
}