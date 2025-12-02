using System.IO;
using System.Collections.Generic;

public class LectorCSV
{
    public List<string[]> LeerDatos(string ruta)
    {
        List<string[]> datos = new List<string[]>();
        foreach (string linea in File.ReadAllLines(ruta))
        {
            string[] partes = linea.Split(',');
            datos.Add(partes);
        }
        return datos;
    }
}