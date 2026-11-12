using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreddyEnemigo : MonoBehaviour
{
    public float velocidad = 2f;
    public Transform sueloCheck;
    public float radioSuelo = 0.1f;
    public LayerMask capaSuelo;
    private bool mirandoDerecha = true;
    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Movimiento constante
        rb.velocity = new Vector2((mirandoDerecha ? 1 : -1) * velocidad, rb.velocity.y);

        anim.SetBool("Caminando", Mathf.Abs(rb.velocity.x) > 0.1f);

        // Detección del suelo
        bool haySuelo = Physics2D.OverlapCircle(sueloCheck.position, radioSuelo, capaSuelo);

        // Si no hay suelo, se da la vuelta
        if (!haySuelo)
        {
            Girar();
        }
    }

    void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (sueloCheck != null)
            Gizmos.DrawWireSphere(sueloCheck.position, radioSuelo);
    }
}
