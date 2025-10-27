using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DisplayInteractHUD : MonoBehaviour
{
    Coroutine coroutine;
    bool startCoroutine;
    [SerializeField] float timeBetween;
    [SerializeField] private PlayerCombat playerCombat;
    DefaultGrabThrowSettings DefaultGrabThrowSettings;
    [SerializeField] private Transform cam;
    void Start()
    {
        playerCombat = GameObject.Find("CombatManager").GetComponent<PlayerCombat>();
        cam = playerCombat.playerCamera._camera.transform;
        DefaultGrabThrowSettings = playerCombat.DefaultGrabThrowSettings;
        coroutine = StartCoroutine(Display(timeBetween));
    }

    void Update()
    {
        if(cam == null)
        {
            cam = playerCombat.playerCamera._camera.transform;

        }
    }
    IEnumerator Display(float timeBetween)
    {
        while (!startCoroutine)
        {
            Ray ray = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, DefaultGrabThrowSettings.grabRange - 1)) 
            {
                if (hit.collider.CompareTag("Interactuable") || hit.collider.CompareTag("Grabbable") || hit.collider.CompareTag("PickUpWeapon"))
                {
                    var canvas = GameObject.Find("Canvas");
                    var canInteract = canvas.transform.Find("InteractBackground").gameObject;
                    Debug.Log("Sí hay");
                    Debug.Log(canInteract);
                    canInteract.SetActive(true);
                    yield return new WaitForSeconds(timeBetween);
                }
                else
                {
                    var canvas = GameObject.Find("Canvas");
                    var canInteract = canvas.transform.Find("InteractBackground").gameObject;
                    Debug.Log("No hay");
                    Debug.Log(canInteract);
                    canInteract.SetActive(false);
                    yield return new WaitForSeconds(timeBetween);
                }
            }
            yield return new WaitForSeconds(0f);
        }

    }
}
