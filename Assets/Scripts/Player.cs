using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] runSprites;
    public Sprite climbSprite;
    private int spriteIndex;

    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;

    private readonly Collider2D[] overlaps = new Collider2D[4];
    private Vector2 direction;

    private bool grounded;
    private bool climbing;

    public float moveSpeed = 3f;
    public float jumpStrength = 4f;

    private AudioSource audioSource;
    public AudioClip soundA;
    public AudioClip soundD;
    private float footstepDelay = 0.3f;
    private float nextFootstepTime;
    public AudioClip jumpSound;

    [Header("Ataque y Espada")]
    private bool puedeAtacar = false;
    public float duracionArmaValida = 8f;

    public GameObject espadaVisual;

    public Vector3 posicionEspadaA = new Vector3(0.5f, 0.5f, 0f);
    public Vector3 posicionEspadaB = new Vector3(0.5f, -0.2f, 0f);

    public float tiempoAtaqueVisible = 0.15f;
    private bool estaAtacandoValida = false;
    public AudioClip ataqueSound;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (espadaVisual != null)
        {
            espadaVisual.SetActive(false);
        }
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(AnimateSprite), 1f / 12f, 1f / 12f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    // Eje horizontal con A (izquierda) y D (derecha)
    private float GetEjeX()
    {
        float x = 0f;
        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.A)) x -= 1f;
        return x;
    }

    // Eje vertical con W (arriba) y S (abajo)
    private float GetEjeY()
    {
        float y = 0f;
        if (Input.GetKey(KeyCode.W)) y += 1f;
        if (Input.GetKey(KeyCode.S)) y -= 1f;
        return y;
    }

    private void Update()
    {
        CheckCollision();
        SetDirection();

        // --- ENTRADA DE ATAQUE (Tecla F) ---
        bool presionoAtacar = Input.GetKeyDown(KeyCode.F);

        if (puedeAtacar && presionoAtacar && !estaAtacandoValida)
        {
            StartCoroutine(SecuenciaAtaqueEspada());
        }
    }

    private void CheckCollision()
    {
        grounded = false;
        climbing = false;

        float skinWidth = 0.1f;

        Vector2 size = capsuleCollider.bounds.size;
        size.y += skinWidth;
        size.x /= 2f;

        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0f, overlaps);

        for (int i = 0; i < amount; i++)
        {
            GameObject hit = overlaps[i].gameObject;

            if (hit.layer == LayerMask.NameToLayer("Ground"))
            {
                grounded = hit.transform.position.y < (transform.position.y - 0.5f + skinWidth);
                Physics2D.IgnoreCollision(overlaps[i], capsuleCollider, !grounded);
            }
            else if (hit.layer == LayerMask.NameToLayer("Ladder"))
            {
                climbing = true;
            }
        }
    }

    private void SetDirection()
    {
        float inputHorizontal = GetEjeX();
        float inputVertical = GetEjeY();

        // Salto con la barra espaciadora
        bool presionoSalto = Input.GetKeyDown(KeyCode.Space);

        if (climbing)
        {
            direction.y = inputVertical * moveSpeed;
        }
        else if (grounded && presionoSalto)
        {
            direction = Vector2.up * jumpStrength;
            PlaySound(jumpSound);
        }
        else
        {
            direction += Physics2D.gravity * Time.deltaTime;
        }

        direction.x = inputHorizontal * moveSpeed;

        if (grounded)
        {
            direction.y = Mathf.Max(direction.y, -1f);
        }

        if (direction.x > 0.05f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (direction.x < -0.05f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + direction * Time.fixedDeltaTime);
    }

    private void AnimateSprite()
    {
        if (climbing)
        {
            spriteRenderer.sprite = climbSprite;
        }
        else if (direction.x != 0f)
        {
            spriteIndex++;

            if (spriteIndex >= runSprites.Length)
            {
                spriteIndex = 0;
            }

            if (spriteIndex > 0 && spriteIndex <= runSprites.Length)
            {
                spriteRenderer.sprite = runSprites[spriteIndex];
            }

            if (grounded && !climbing && Time.time >= nextFootstepTime)
            {
                float inputX = GetEjeX();

                if (inputX < -0.1f)
                {
                    PlaySound(soundA);
                    nextFootstepTime = Time.time + footstepDelay;
                }
                else if (inputX > 0.1f)
                {
                    PlaySound(soundD);
                    nextFootstepTime = Time.time + footstepDelay;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objective"))
        {
            // GameManager.instance.LevelComplete();
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (!puedeAtacar)
            {
                GameManager.instance.PlayerDied();
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void ActivarArma()
    {
        StopAllCoroutines();
        puedeAtacar = true;
        if (espadaVisual != null) espadaVisual.SetActive(false);
        estaAtacandoValida = false;

        StartCoroutine(DesactivarArmaDespuesDeTiempo());
    }

    IEnumerator DesactivarArmaDespuesDeTiempo()
    {
        yield return new WaitForSeconds(duracionArmaValida);
        puedeAtacar = false;
        if (espadaVisual != null) espadaVisual.SetActive(false);
        Debug.Log("El tiempo del arma terminó. Ya no puedes atacar.");
    }

    IEnumerator SecuenciaAtaqueEspada()
    {
        estaAtacandoValida = true;

        Debug.Log("¡Ataque ejecutado!");
        PlaySound(ataqueSound);

        if (espadaVisual != null)
        {
            espadaVisual.transform.localPosition = posicionEspadaA;
            espadaVisual.SetActive(true);

            yield return new WaitForSeconds(tiempoAtaqueVisible / 2f);

            espadaVisual.transform.localPosition = posicionEspadaB;

            AtacarLogica();

            yield return new WaitForSeconds(tiempoAtaqueVisible / 2f);

            espadaVisual.SetActive(false);
        }
        else
        {
            AtacarLogica();
        }

        estaAtacandoValida = false;
    }

    void AtacarLogica()
    {
        Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(transform.position, 1.3f);

        foreach (Collider2D col in enemigosGolpeados)
        {
            if (col.CompareTag("Obstacle"))
            {
                if (ScoreManager.instance != null)
                {
                    ScoreManager.instance.AddPoints(30);
                }

                Destroy(col.gameObject);
                Debug.Log("Obstáculo/Enemigo destruido desde el ataque. +30 puntos.");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}