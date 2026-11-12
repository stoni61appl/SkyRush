using UnityEngine;

public class Barrel : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 1f;
    // Quitamos 'dano' si no se usa, o la dejamos por si la necesitas para otra lógica interna

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. COMPORTAMIENTO FISICO: Si choca con el suelo, se impulsa
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            rb.AddForce(collision.transform.right * speed, ForceMode2D.Impulse);
        }

        // 2. ATAQUE AL JUGADOR: Si choca con el Player, le resta una vida
        if (collision.gameObject.CompareTag("Player"))
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
}