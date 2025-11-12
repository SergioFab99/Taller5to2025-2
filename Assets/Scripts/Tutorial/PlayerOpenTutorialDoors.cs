using UnityEngine;

public class PlayerOpenTutorialDoors : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject canInteract;

    PlayerInputActions _inputActions;
    [SerializeField] private bool onTrigger;
    //[SerializeField] private bool interact;
    [SerializeField] private bool openDoor1, openDoor2;
    private OpenDoor openDoor;
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        canInteract = canvas.transform.Find("InteractBackground").gameObject;
        canInteract.SetActive(false);
    }

    void Update()
    {
        //UpdateInput();
        if(onTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Deactivate();
            ActiveCheckPoints();
        }
        if (SetUpTutorial.checkPoint2)
        {
            SetUpTutorial.checkPoint1 = false;
        }
        if (SetUpTutorial.checkPoint3)
        {
            SetUpTutorial.checkPoint2 = false;
        }
    }

    public void UpdateInput()
    {
        /*_inputActions = new PlayerInputActions();
        _inputActions.Enable();
        var input = _inputActions.Player;
        interact = input.Interact.WasPressedThisFrame();*/
    }
    private void OnDestroy()
    {
        /*_inputActions.Disable();
        _inputActions.Dispose();*/
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Door1"))
        {
            openDoor = other.gameObject.GetComponent<OpenDoor>();
            canInteract.SetActive(true);
            onTrigger = true;
            openDoor1 = true;
        }
        if (other.gameObject.CompareTag("Door2"))
        {
            openDoor = other.gameObject.GetComponent<OpenDoor>();
            canInteract.SetActive(true);
            onTrigger = true;
            openDoor2 = true;
        }
        if (other.gameObject.CompareTag("CheckPoint3"))
        {
            SetUpTutorial.checkPoint3 = true;
            SetUpTutorial.enemyDefeatCount = 0;
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Door1"))
        {
            canInteract.SetActive(false);
            onTrigger = false;
            openDoor1 = false;
        }
        if (other.gameObject.CompareTag("Door2"))
        {
            canInteract.SetActive(false);
            onTrigger = false;
            openDoor2 = false;
        }
    }

    void Deactivate()
    {
        canInteract.SetActive(false);
    }
    void ActiveCheckPoints()
    {
        if(openDoor != null)
        {
            openDoor.CallStartOpen();
            openDoor.starMoveDoor = true;
        }        
        if (!SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3 && openDoor1)
        {
            SetUpTutorial.camera2 = true;
            SetUpTutorial.checkPoint1 = true;
            SetUpTutorial.spawnPoint = gameObject.transform.position;

        }
        if (SetUpTutorial.checkPoint1 && !SetUpTutorial.checkPoint2 && !SetUpTutorial.checkPoint3 && openDoor2)
        {
            SetUpTutorial.camera3 = true;
            SetUpTutorial.checkPoint2 = true;
            SetUpTutorial.spawnPoint = gameObject.transform.position;
        }
    }
}
