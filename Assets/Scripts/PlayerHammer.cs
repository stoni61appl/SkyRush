using UnityEngine;

public class PlayerHammer : MonoBehaviour
{
    [Header("Hammer")]
    public bool hasHammer = false;
    public float hammerDuration = 10f;

    [Header("Visual")]
    public GameObject hammerVisual;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask obstacleLayer;

    private float hammerTimer;

   

    void Update()
    {
        if (hasHammer)
        {
            hammerTimer -= Time.deltaTime;

            if (hammerTimer <= 0)
            {
                DisableHammer();
            }
        }
    }

    public void ActivateHammer()
    {
        hasHammer = true;
        hammerTimer = hammerDuration;

        if (hammerVisual != null)
            hammerVisual.SetActive(true);

        Debug.Log("Martillo activado");
    }

    void DisableHammer()
    {
        hasHammer = false;

        if (hammerVisual != null)
            hammerVisual.SetActive(false);

        Debug.Log("Martillo terminado");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHammer && collision.gameObject.CompareTag("Obstacle"))
        if (hasHammer && collision.gameObject.CompareTag("Obstacle"))
        {
            Destroy(collision.gameObject);
        }
    }
}