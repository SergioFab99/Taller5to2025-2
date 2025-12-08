using UnityEngine;
using UnityEngine.SceneManagement;
public class EnemyOpenDoor : MonoBehaviour
{
    private OpenDoor openDoor;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Interactuable"))
        {
            openDoor = other.gameObject.GetComponent<OpenDoor>();
            other.gameObject.TryGetComponent<OpenDoor>(out OpenDoor door);
            if(door) Doors();
        }
    }
    void Doors()
    {
        if (DisplayInteractHUD.thisIsTutorial)
        {
            return;
        }
        else
        {
            if (openDoor != null)
            {
                openDoor.CallStartJustOpen();
                openDoor.starMoveDoor = true;
            }
        }
    }
}
