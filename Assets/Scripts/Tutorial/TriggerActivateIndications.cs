using UnityEngine;

public class TriggerActivateIndications : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger con player");
            Indications.instance.NextIndication();
            Indications.instance.ActivateIndications();
            Destroy(this);
        }
    }
}
