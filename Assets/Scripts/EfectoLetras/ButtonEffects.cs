using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Configuración Cartoon")]
    [Tooltip("Factor de escala al pasar el mouse (ej. 1.2 para 20% más grande)")]
    [SerializeField] private float hoverScale = 1.2f;
    [Tooltip("Duración de la animación")]
    [SerializeField] private float duration = 0.2f;
    [Tooltip("Curva de animación para el rebote")]
    [SerializeField] private AnimationCurve bounceCurve = new AnimationCurve(
        new Keyframe(0f, 0f), 
        new Keyframe(0.6f, 1.15f), 
        new Keyframe(0.85f, 0.95f),
        new Keyframe(1f, 1f)
    );

    private Vector3 originalScale;
    private Coroutine currentCoroutine;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(AnimateScale(originalScale * hoverScale));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(AnimateScale(originalScale));
    }

    private IEnumerator AnimateScale(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            
            float curveValue = bounceCurve.Evaluate(t);
            
            transform.localScale = Vector3.LerpUnclamped(startScale, targetScale, curveValue);
            
            yield return null;
        }
        
        transform.localScale = targetScale;
        currentCoroutine = null;
    }
}
