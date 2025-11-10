using UnityEngine;

public class TriggerChecker : MonoBehaviour
{
    public DrunkCameraShake drunkCameraShake; 

    void OnTriggerEnter(Collider other)
    {
        if (drunkCameraShake != null)
        {
            drunkCameraShake.ActivateDrunk(5f);
            Debug.Log("Efecto borracho activo");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (drunkCameraShake != null)
        {
            drunkCameraShake.DeactivateDrunk();
            Debug.Log("Efecto borracho desactivado");
        }
    }
}
