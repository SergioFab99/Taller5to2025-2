using UnityEngine;
using TMPro;

public class LetterEffects : MonoBehaviour
{
    [Header("Configuración del Efecto (Máquina de Escribir Antigua)")]
    [SerializeField] private TMP_Text textComponent;
    [Tooltip("Tiempo entre la aparición de cada letra")]
    [SerializeField] private float revealSpeed = 0.1f; 
    [Tooltip("Opacidad mínima para letras no reveladas")]
    [SerializeField] [Range(0f, 1f)] private float minAlpha = 0.0f; 
    [Tooltip("Opacidad máxima para letras reveladas")]
    [SerializeField] [Range(0f, 1f)] private float maxAlpha = 1.0f;
    [Tooltip("Velocidad del parpadeo después de revelar")]
    [SerializeField] private float blinkSpeed = 2.0f;
    [Tooltip("Espaciado del degradado de parpadeo")]
    [SerializeField] private float blinkSpacing = 0.5f;

    private float startTime;
    private string lastText;

    void Awake()
    {
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();
        if (textComponent != null)
        {
            lastText = textComponent.text;
            startTime = Time.time;
        }
    }

    void Update()
    {
        if (textComponent == null) return;

        if (textComponent.text != lastText)
        {
            startTime = Time.time;
            lastText = textComponent.text;
        }

        textComponent.ForceMeshUpdate();
        
        TMP_TextInfo textInfo = textComponent.textInfo;
        int characterCount = textInfo.characterCount;

        if (characterCount == 0) return;

        int revealedCount = Mathf.Min(characterCount, Mathf.FloorToInt((Time.time - startTime) / revealSpeed));

        for (int i = 0; i < characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            
            Color32[] newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            byte alpha;
            if (revealedCount < characterCount)
            {
                if (i < revealedCount)
                {
                    alpha = (byte)(maxAlpha * 255);
                }
                else
                {
                    alpha = (byte)(minAlpha * 255);
                }
            }
            else
            {
                float phase = Time.time * blinkSpeed - i * blinkSpacing;
                float t = (Mathf.Sin(phase) + 1f) * 0.5f;
                alpha = (byte)(Mathf.Lerp(minAlpha, maxAlpha, t) * 255);
            }

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
        }

        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}
