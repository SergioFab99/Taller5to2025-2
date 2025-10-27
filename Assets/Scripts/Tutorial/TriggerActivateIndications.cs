using UnityEngine;

public class TriggerActivateIndications : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Indications.instance.NextIndication();
            Indications.instance.ActivateIndications();
            Destroy(this);
        }
    }
}
