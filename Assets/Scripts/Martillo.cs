using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Martillo : MonoBehaviour
{
    private bool atacando;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !atacando)
        {
            Atacando();
            Destroy(gameObject);
        }

    }
    private void Atacando() 
    {
        atacando = true;
    }
    private void DesactivaAtaque() 
    {
        atacando = false;
    
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // if (collision.CompareTag("Player")) 
        {

        }
        
    }
}
