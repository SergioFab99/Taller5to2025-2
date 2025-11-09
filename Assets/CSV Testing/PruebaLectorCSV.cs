using UnityEngine;
using System.Collections.Generic;

public class PruebaLectorCSV : MonoBehaviour
{
    void Start()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Personajes");
        if (csvFile == null)
        {
            Debug.LogError("Archivo 'Personajes.csv' no encontrado en Assets/Resources/");
            return;
        }

        // Leer las líneas del archivo
        string[] lineas = csvFile.text.Split('\n');
        List<string[]> datos = new List<string[]>();

        foreach (string linea in lineas)
        {
            if (string.IsNullOrWhiteSpace(linea)) continue;

            string[] partes = linea.Split(',');
            for (int i = 0; i < partes.Length; i++)
            {
                partes[i] = partes[i].Trim();
            }
            datos.Add(partes);
        }

        Debug.Log("=== DATOS LEÍDOS ===");
        for (int i = 0; i < datos.Count; i++)
        {
            string[] fila = datos[i];
            Debug.Log($"Fila {i}: [{string.Join(", ", fila)}]");
        }
    }
}