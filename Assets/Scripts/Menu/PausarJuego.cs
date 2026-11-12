using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausarJuego : MonoBehaviour
{
    // --- NUEVA LÍNEA: Instancia estática (Singleton) ---
    public static PausarJuego instancia;

    public GameObject menuPausa;
    public bool juegoPausado = false;

    private void Awake()
    {
        // Creamos la referencia para que sea accesible desde cualquier script
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AlternarPausa();
        }
    }

    // Centralizamos la lógica en una función pública
    public void AlternarPausa()
    {
        if (juegoPausado)
        {
            Reanudar();
        }
        else
        {
            Pausar();
        }
    }

    public void Reanudar()
    {
        menuPausa.SetActive(false);
        Time.timeScale = 1;
        juegoPausado = false;
    }

    public void Pausar()
    {
        menuPausa.SetActive(true);
        Time.timeScale = 0;
        juegoPausado = true;
    }
}