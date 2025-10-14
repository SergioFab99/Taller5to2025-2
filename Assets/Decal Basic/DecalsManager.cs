using UnityEngine;
using UnityEngine.Rendering.Universal;

[DisallowMultipleComponent]
public sealed class DecalsManager : MonoBehaviour
{
    [SerializeField] private DecalProjector[] projectors;
    [SerializeField] private float tiempoActivo = 2f;
    [SerializeField] private float tiempoInactivo = 2f;
    [SerializeField] private bool iniciarActivo = true;

    private float temporizador;
    private bool activo;

    private void Awake()
    {
        if (projectors == null || projectors.Length == 0)
        {
            projectors = GetComponentsInChildren<DecalProjector>(true);
        }
    }

    private void OnEnable()
    {
        activo = iniciarActivo;
        AplicarEstado();
        temporizador = DuracionActual();
    }

    private void Update()
    {
        temporizador -= Time.deltaTime;
        if (temporizador > 0f)
        {
            return;
        }

        activo = !activo;
        AplicarEstado();
        temporizador = DuracionActual();
    }

    private float DuracionActual() => Mathf.Max(0.01f, activo ? tiempoActivo : tiempoInactivo);

    private void AplicarEstado()
    {
        if (projectors == null)
        {
            return;
        }

        for (int i = 0; i < projectors.Length; i++)
        {
            if (projectors[i] != null)
            {
                projectors[i].enabled = activo;
            }
        }
    }
}
