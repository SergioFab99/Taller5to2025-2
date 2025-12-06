using UnityEngine;
using TMPro;

public class LetterEffects : MonoBehaviour
{
    [Header("Configuración del Efecto (Color)")]
    [SerializeField] private TMP_Text textComponent;
    [Tooltip("Velocidad de la animación de color")]
    [SerializeField] private float speed = 4.0f; 
    [Tooltip("Frecuencia de la onda de color")]
    [SerializeField] private float waveFrequency = 0.5f; 
    [Tooltip("Opacidad mínima para el efecto tenue")]
    [SerializeField] [Range(0f, 1f)] private float minAlpha = 0.2f; 
    [Tooltip("Opacidad máxima")]
    [SerializeField] [Range(0f, 1f)] private float maxAlpha = 1.0f; 

    [Header("Configuración de Deformación (Bandera)")]
    [Tooltip("Amplitud de la onda física (altura)")]
    [SerializeField] private float warpAmplitude = 5.0f;
    [Tooltip("Frecuencia de la onda física (ancho)")]
    [SerializeField] private float warpFrequency = 0.05f;
    [Tooltip("Velocidad de la onda física")]
    [SerializeField] private float warpSpeed = 5.0f;

    void Awake()
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (textComponent == null) return;

        textComponent.ForceMeshUpdate();
        
        TMP_TextInfo textInfo = textComponent.textInfo;
        int characterCount = textInfo.characterCount;

        if (characterCount == 0) return;

        for (int i = 0; i < characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            
            Color32[] newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            float offset = (characterCount - i) * waveFrequency;
            float wave = Mathf.Sin(Time.time * speed + offset);
            
            float t = (wave + 1f) * 0.5f;

            byte alpha = (byte)(Mathf.Lerp(minAlpha, maxAlpha, t) * 255);

            Color32 c0 = newVertexColors[vertexIndex + 0];
            Color32 c1 = newVertexColors[vertexIndex + 1];
            Color32 c2 = newVertexColors[vertexIndex + 2];
            Color32 c3 = newVertexColors[vertexIndex + 3];

            c0.a = alpha;
            c1.a = alpha;
            c2.a = alpha;
            c3.a = alpha;

            newVertexColors[vertexIndex + 0] = c0;
            newVertexColors[vertexIndex + 1] = c1;
            newVertexColors[vertexIndex + 2] = c2;
            newVertexColors[vertexIndex + 3] = c3;

            
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                Vector3 orig = vertices[vertexIndex + j];
                orig.y += Mathf.Sin(Time.time * warpSpeed + orig.x * warpFrequency) * warpAmplitude;
                vertices[vertexIndex + j] = orig;
            }
        }

        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
}
