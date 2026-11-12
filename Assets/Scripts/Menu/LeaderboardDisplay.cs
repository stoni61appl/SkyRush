using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LeaderboardDisplay : MonoBehaviour
{
    // Arrastra tus 5 renglones de texto diferentes desde tu Canvas en el Inspector
    [SerializeField] private TMP_Text[] renglonesTexto;

    void Start()
    {
        MostrarTabla();
    }

    public void MostrarTabla()
    {
        if (GameManager.instance == null) return;

        // CORRECCIÓN: Le avisamos a C# que la lista viene desde GameManager
        List<GameManager.HighScoreElement> top5 = GameManager.instance.CargarHighScores();

        for (int i = 0; i < renglonesTexto.Length; i++)
        {
            if (i < top5.Count)
            {
                // Formateamos el tiempo para que se vea limpio (ejemplo: 45.2s)
                string tiempoFormateado = top5[i].time.ToString("F1") + "s";

                renglonesTexto[i].text = $"{i + 1}. {top5[i].initials} ---- Ptos: {top5[i].score} -- Tmp: {tiempoFormateado}";
            }
            else
            {
                renglonesTexto[i].text = $"{i + 1}. --- ---- Ptos: 0 -- Tmp: 0.0s";
            }
        }
    }
}