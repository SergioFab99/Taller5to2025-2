using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class DisplayInteractHUD : MonoBehaviour
{
    Coroutine coroutine;
    bool startCoroutine;
    [SerializeField] float timeBetween;
    [SerializeField] private PlayerCombat playerCombat;    
    [SerializeField] private Transform cam;
    [SerializeField] private GameObject player;
    [SerializeField] string thisScene;
    private GameObject stairUp;
    [SerializeField] private OpenDoor openDoor;
    Player playerTp;
    GameObject canvas, canInteract;
    private bool isHittingDoor, isHittingStair;
    public static bool thisIsTutorial, thisIsLevel1;
    void Start()
    {
        thisScene = SceneManager.GetActiveScene().name;
        canvas = GameObject.Find("Canvas (1)");
        canInteract = canvas.transform.Find("InteractBackground").gameObject;
        playerCombat = GameObject.Find("CombatManager").GetComponent<PlayerCombat>();
        cam = playerCombat.playerCamera._camera.transform;
        playerTp = player.GetComponent<Player>();
        coroutine = StartCoroutine(Display(timeBetween));
        if(thisScene == "LevelTutorial")
        {
            thisIsTutorial = true;
            Debug.Log($"thisIsTutorial = {thisIsTutorial}");
        }
        else if(thisScene == "Level1B")
        {
            thisIsLevel1 = true;
            thisIsTutorial = false;
        }
        else
        {
            thisIsTutorial = false;
        }
    }

    void Update()
    {
        if(cam == null)
        {
            cam = playerCombat.playerCamera._camera.transform;
        }

        ActiveInteracts();
    }
    IEnumerator Display(float timeBetween)
    {
        while (!startCoroutine)
        {
            Ray ray = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 2.5f)) 
            {
                if (hit.collider.CompareTag("Interactuable") || hit.collider.CompareTag("Grabbable") || hit.collider.CompareTag("PickUpWeapon"))
                {
                    Debug.Log("S� hay");
                    Debug.Log(canInteract);
                    canInteract.SetActive(true);
                    openDoor = hit.collider.gameObject.GetComponent<OpenDoor>();
                    if (hit.collider.gameObject.name == "StairCollider")
                    {
                        var stairHit = hit.collider.gameObject;
                        stairUp = stairHit.transform.Find("StairUp").gameObject;
                        isHittingStair = true;
                    }
                    else isHittingStair = false;
                    hit.collider.gameObject.TryGetComponent<OpenDoor>(out OpenDoor door);
                    isHittingDoor = door;

                    yield return new WaitForSeconds(timeBetween);
                }
                else
                {
                    if (thisIsLevel1 && NPCInteraction.OnTriggerNpc)
                    {
                        yield return null;
                    }
                    else
                    {
                        Debug.Log("No hay");
                        Debug.Log(canInteract);
                        canInteract.SetActive(false);
                        isHittingDoor = false;
                        isHittingStair = false;
                        yield return new WaitForSeconds(timeBetween);
                    }
                    
                }
            }
            else
            {
                if (thisIsLevel1 && NPCInteraction.OnTriggerNpc)
                {
                    yield return null;
                }
                else
                {
                    Debug.Log("No hay");
                    Debug.Log(canInteract);
                    canInteract.SetActive(false);
                    isHittingDoor = false;
                    isHittingStair = false;
                    yield return new WaitForSeconds(timeBetween);
                }
            }
            yield return new WaitForSeconds(0f);
        }

    }
    void ActiveInteracts()
    {
        if (Input.GetKeyDown(KeyCode.E) && Time.timeScale == 1)
        {
            if (openDoor != null && isHittingDoor)
            {
                Doors();
            }
            if(stairUp != null && isHittingStair)
            {
                Stairs();
            }
        }
    }
    void Stairs()
    {
        playerTp.Teleport(stairUp.transform.position);
    }

    void Doors()
    {
        if (thisIsTutorial)
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
    public void DoorsAttacking()
    {
        if (thisIsTutorial)
        {
            return;
        }
        else
        {
            if (openDoor != null)
            {
                openDoor.CallStartPushOpen();
                openDoor.starMoveDoor = true;
            }
        }
    }
}
