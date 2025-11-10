using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class TriggerChecker : MonoBehaviour
{
    public DrunkZone drunkZone;

    void OnTriggerEnter(Collider other)
    {
        drunkZone.ActivarDrunk();
        Debug.Log("Trigger checker: Entré en zona " + other.gameObject.name);
    }

    void OnTriggerExit(Collider other)
    {
        drunkZone.DesactivarDrunk();
        Debug.Log("Trigger checker: Salí de zona " + other.gameObject.name);
    }
}
