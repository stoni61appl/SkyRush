using UnityEngine;
using System.IO.Ports;
using System;

public class ArduinoController : MonoBehaviour
{
    // --- NUEVA VARIABLE: Instancia estática (Singleton) ---
    public static ArduinoController instancia;

    // Cambia "COM3" por el puerto de tu Arduino. En Mac/Linux usa algo como "/dev/tty.usbmodem..."
    public string portName = "COM8";
    public int baudRate = 115200;

    private SerialPort stream;

    // Variables públicas para que otros scripts las lean
    [Header("Datos del Control")]
    public float ejeX = 0f;
    public float ejeY = 0f;
    public bool botonSaltar = false;
    public bool botonAtacar = false;
    public bool botonPausa = false;

    private void Awake()
    {
        // =========================================================================
        // CONFIGURACIÓN DEL SINGLETON Y PERSISTENCIA (Para transiciones de nivel)
        // =========================================================================
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject); // Hace que este objeto sea inmortal entre niveles
        }
        else if (instancia != this)
        {
            // Si pasamos al Nivel 2 y Unity encuentra otro ArduinoController, lo destruye de inmediato
            // para que no duplique lecturas en el puerto COM ni rompa la comunicación
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Inicializar el puerto serie
        stream = new SerialPort(portName, baudRate);
        stream.ReadTimeout = 50;

        try
        {
            stream.Open();
            Debug.Log("Puerto serie abierto correctamente en " + portName);
        }
        catch (Exception e)
        {
            Debug.LogError("No se pudo abrir el puerto serie: " + e.Message);
        }
    }

    void Update()
    {
        if (stream != null && stream.IsOpen)
        {
            try
            {
                string data = stream.ReadLine(); // Lee la línea completa hasta el salto de línea

                if (!string.IsNullOrEmpty(data))
                {
                    ParsearDatos(data);
                }
            }
            catch (TimeoutException)
            {
                // Es normal que ocurra ocasionalmente si Arduino no ha mandado datos aún
            }
            catch (Exception e)
            {
                Debug.LogWarning("Error leyendo el puerto: " + e.Message);
            }
        }
    }

    void ParsearDatos(string datosRaw)
    {
        // Separar la cadena por las comas
        string[] valores = datosRaw.Split(',');

        if (valores.Length >= 5)
        {
            // 1. Procesar Joystick (Mapear de 0-1023 a un rango de -1 a 1)
            int rawX = int.Parse(valores[0]);
            int rawY = int.Parse(valores[1]);

            // Restamos 512 para que el centro sea 0, y dividimos entre 512 para normalizarlo
            ejeX = (rawX - 512f) / 512f;
            ejeY = (rawY - 512f) / 512f;

            // Filtro "Deadzone" para evitar que el personaje se mueva solo por desgaste del joystick
            if (Mathf.Abs(ejeX) < 0.1f) ejeX = 0f;
            if (Mathf.Abs(ejeY) < 0.1f) ejeY = 0f;

            // 2. Procesar Botones (1 = true, 0 = false)
            botonSaltar = valores[2] == "1";
            botonAtacar = valores[3] == "1";
            botonPausa = valores[4] == "1";
        }
    }

    void OnApplicationQuit()
    {
        // Cerrar el puerto al salir del juego para no bloquear el Arduino
        if (stream != null && stream.IsOpen)
        {
            stream.Close();
        }
    }
}