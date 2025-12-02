using UnityEngine;

public class TriggerActivateIndications : MonoBehaviour
{
    private void Awake()
    {
        if(SetUpTutorial.checkPoint1 || SetUpTutorial.checkPoint2 || SetUpTutorial.checkPoint3)
        {
            Destroy(this);
        }
    }
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
