using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DrunkZone : MonoBehaviour
{
    public Volume volume;
    private MotionBlur motionBlur;

    void Start()
    {
        if (volume != null && volume.profile != null)
        {
            if (!volume.profile.TryGet(out motionBlur))
                Debug.LogWarning("No se encontró MotionBlur en el Volume Profile");
        }
            
        if (motionBlur != null)
            motionBlur.active = false;
    }

    public void ActivarDrunk()
    {
        if (motionBlur != null)
        {
            motionBlur.active = true;
            motionBlur.intensity.value = 1f;
            Debug.Log("DrunkZone ACTIVADO: Motion Blur ON (Intensidad 1)");
        }
        else
        {
            Debug.LogError("No se puede activar: Motion Blur no está referenciado");
        }
    }

    public void DesactivarDrunk()
    {
        if (motionBlur != null)
        {
            motionBlur.active = false;
            Debug.Log("DrunkZone DESACTIVADO: Motion Blur OFF");
        }
        else
        {
            Debug.LogError("No se puede desactivar: Motion Blur no está referenciado");
        }
    }
}
