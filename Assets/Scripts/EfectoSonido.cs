using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfectoSonido : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private int pointsPerPerfectJump = 10;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle")) 
        {
            audioSource.Play();

            ScoreManager.instance.AddPoints(pointsPerPerfectJump);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
